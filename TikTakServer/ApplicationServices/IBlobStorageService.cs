using TikTakServer.Models.Business;

namespace TikTakServer.ApplicationServices
{
    public interface IBlobStorageService
    {
        Task RemoveBlobs(string blobId);
        Task UploadBlob(PostBlobModel file);
        Task<MemoryStream> DownloadManifest(string id);
        Task<List<VideoAndOwnedUserInfo>> GetFyp();
    }
}
