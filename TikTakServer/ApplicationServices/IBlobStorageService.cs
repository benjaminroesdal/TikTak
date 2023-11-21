namespace TikTakServer.ApplicationServices
{
    public interface IBlobStorageService
    {
        Task RemoveBlobs(string blobId);
        Task UploadBlob(IFormFile file);
        Task<MemoryStream> DownloadManifest();
        Task DownloadBlob(string blobName);
    }
}
