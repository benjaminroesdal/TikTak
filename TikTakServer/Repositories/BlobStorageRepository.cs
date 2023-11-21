
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace TikTakServer.Repositories
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly BlobServiceClient blobServiceClient;
        public BlobStorageRepository(BlobServiceClient client)
        {
            blobServiceClient = client;
        }

        public async Task<BinaryData> GetBlob(string blobName, string containerName)
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            var blob = await blobClient.DownloadContentAsync();
            return blob.Value.Content;
        }

        public async Task RemoveBlob(string blobName, string containerName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> UploadBlob(string blobName, string containerName, string path)
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            // Upload data from the local file
            try
            {
                blobClient.Upload(path, true);
            }
            catch (Exception e)
            {

                throw;
            }
            return "Ok";
        }
    }
}
