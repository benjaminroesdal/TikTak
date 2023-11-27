using TikTakServer.Models;

namespace TikTakServer.ApplicationServices
{
    public interface IBlobStorageService
    {
        Task RemoveBlobs(string blobId);
        Task UploadBlob(PostBlobModel file);
        Task<MemoryStream> DownloadManifest(string id);
        Task DownloadBlob(string blobName);
        Task<UserInfoAndFypIds> GetFyp();
    }
}
