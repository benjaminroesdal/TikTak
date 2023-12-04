using Azure.Storage.Blobs;

namespace TikTakServer.Repositories
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly BlobServiceClient blobServiceClient;
        public BlobStorageRepository(BlobServiceClient client)
        {
            blobServiceClient = client;
        }

        /// <summary>
        /// Gets the blob with provided name from container of provided contaimerName
        /// </summary>
        /// <param name="blobName">blobName to find in container</param>
        /// <param name="containerName">container to search for blob in</param>
        /// <returns>BinaryData of the blob</returns>
        public async Task<BinaryData> GetBlob(string blobName, string containerName)
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            var blob = await blobClient.DownloadContentAsync();
            return blob.Value.Content;
        }

        /// <summary>
        /// Removes blob with provided name from container with provided container name
        /// </summary>
        /// <param name="blobName">blob to remove</param>
        /// <param name="containerName">container to remove blob from</param>
        public async Task RemoveBlob(string blobName, string containerName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Uploads blob from the provided path to container with provided containerName.
        /// </summary>
        /// <param name="blobName">name for the blob</param>
        /// <param name="containerName">container to upload blob to</param>
        /// <param name="path">local path of file to upload</param>
        public void UploadBlob(string blobName, string containerName, string path)
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

                throw new Exception("Blob was not able to be uploaded:" + e.Message);
            }
        }
    }
}
