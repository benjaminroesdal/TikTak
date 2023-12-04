using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using TikTakServer.Facades;
using TikTakServer.Models.DaoModels;
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
        private readonly UserRequestAndClaims _requestClaims;
        private const string containerName = "tiktaks";

        public BlobStorageService(BlobServiceClient blobServiceClient,
            IBlobStorageFacade blobStorageFacade,
            IVideoFacade videoFacade,
            IRecommendationFacade recommendationManager,
            UserRequestAndClaims requestClaims,
            IHlsHandler hlsHandler)
        {

            this.blobServiceClient = blobServiceClient;
            this.blobStorageFacade = blobStorageFacade;
            this.videoFacade = videoFacade;
            recommendationFacade = recommendationManager;
            _requestClaims = requestClaims;
            this.hlsHandler = hlsHandler;
        }

        public async Task<MemoryStream> DownloadManifest(string id)
        {
            var storageId = await videoFacade.GetVideo(id);
            var manifest = await blobStorageFacade.GetBlob(storageId.BlobStorageId + ".M3U8", containerName);
            var stream = new MemoryStream(manifest.ToArray());
            return stream;
        }

        public async Task<List<VideoAndOwnedUserInfo>> GetFyp()
        {
            var vidAndUserInfo = await recommendationFacade.GetFyp();
            return vidAndUserInfo;
        }

        public async Task RemoveBlobs(string blobId)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: blobId))
            {
                await blobStorageFacade.RemoveBlob(blobItem.Name, containerName);
            }
            await videoFacade.RemoveVideoByStorageId(blobId);
        }

        public async Task UploadBlob(PostBlobModel file)
        {
            var blobGuid = Guid.NewGuid().ToString();
            HlsObj hlsObj = new HlsObj();
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
