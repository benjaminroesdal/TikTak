
using TikTakServer.Repositories;

namespace TikTakServer.Facades
{
    public class BlobStorageFacade : IBlobStorageFacade
    {
        private readonly IBlobStorageRepository _repository;
        public BlobStorageFacade(IBlobStorageRepository repository)
        {
            _repository = repository;
        }

        public async Task<BinaryData> GetBlob(string blobName, string containerName)
        {
            return await _repository.GetBlob(blobName, containerName);
        }

        public async Task RemoveBlob(string blobName, string containerName)
        {
            await _repository.RemoveBlob(blobName, containerName);
        }

        public async Task<string> UploadBlob(string blobName, string containerName, string path)
        {
            return await _repository.UploadBlob(blobName,  containerName, path);
        }
    }
}
