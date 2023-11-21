namespace TikTakServer.Repositories
{
    public interface IBlobStorageRepository
    {
        public Task<BinaryData> GetBlob(string blobName, string containerName);
        public Task<string> UploadBlob(string blobName, string containerName, string path);
        public Task RemoveBlob(string blobName, string containerName);
    }
}
