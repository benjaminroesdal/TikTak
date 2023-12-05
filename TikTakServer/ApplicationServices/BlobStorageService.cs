using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using TikTakServer.Facades;
using TikTakServer.Models.Business;
using TikTakServer.Handlers;

namespace TikTakServer.ApplicationServices
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly IBlobStorageFacade blobStorageFacade;
        private readonly IVideoFacade videoFacade;
        private readonly IRecommendationFacade recommendationFacade;
        private readonly IHlsHandler hlsHandler;
        private const string containerName = "tiktaks";

        public BlobStorageService(BlobServiceClient blobServiceClient,
            IBlobStorageFacade blobStorageFacade,
            IVideoFacade videoFacade,
            IRecommendationFacade recommendationManager,
            IHlsHandler hlsHandler)
        {

            this.blobServiceClient = blobServiceClient;
            this.blobStorageFacade = blobStorageFacade;
            this.videoFacade = videoFacade;
            recommendationFacade = recommendationManager;
            this.hlsHandler = hlsHandler;
        }

        /// <summary>
        /// Downloads a manifest file with the provided id, and returns it as a memory stream.
        /// </summary>
        /// <param name="id">Id to find manifest on</param>
        /// <returns>Manifest as memorystream</returns>
        public async Task<MemoryStream> DownloadManifest(string id)
        {
            var manifest = await blobStorageFacade.GetBlob(id + ".M3U8", containerName);
            var stream = new MemoryStream(manifest.ToArray());
            return stream;
        }

        /// <summary>
        /// Gets a personalized list of videos based on user preferences.
        /// </summary>
        /// <returns>List of VideoAndOwnedUserInfo</returns>
        public async Task<List<VideoAndOwnedUserInfo>> GetFyp()
        {
            var vidAndUserInfo = await recommendationFacade.GetFyp();
            return vidAndUserInfo;
        }

        /// <summary>
        /// Removes all blobs with provided blobId
        /// </summary>
        /// <param name="blobId">blobId to remove blobs on</param>
        public async Task RemoveBlobs(string blobId)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: blobId))
            {
                await blobStorageFacade.RemoveBlob(blobItem.Name, containerName);
            }
            await videoFacade.RemoveVideoByStorageId(blobId);
        }

        /// <summary>
        /// Takes provided IFormFile from PostBlobModel and uses hlsHandler to convert the file into HLS format
        /// uploads the video to blobStorage and saves it to the database with provided tags.
        /// </summary>
        /// <param name="file">PostBlobModel with File and Tags to upload.</param>
        public async Task UploadBlob(PostBlobModel file)
        {
            var blobGuid = Guid.NewGuid().ToString();
            HlsModel hlsObj = new HlsModel();
            using (var stream = file.File.OpenReadStream())
            {
                hlsObj = await this.hlsHandler.ConvertToHls(stream, blobGuid);
            }
            blobStorageFacade.UploadBlob(blobGuid + $".M3U8", containerName, hlsObj.Path + $"\\{blobGuid}.M3U8");
            for (int i = 0; i < hlsObj.FileCount; i++)
            {
                blobStorageFacade.UploadBlob(blobGuid + $"{i}.ts", containerName, hlsObj.Path + $"\\{blobGuid}{i}.ts");
            }
            this.hlsHandler.ClearTempFiles(hlsObj.Guid, hlsObj.Path);
            await videoFacade.SaveVideo(new VideoModel()
            {
                BlobStorageId = blobGuid,
                UploadDate = DateTime.Now,
                Tags = file.Tags
            });
        }
    }
}
