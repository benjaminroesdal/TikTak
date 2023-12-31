﻿namespace TikTakServer.Facades
{
    public interface IBlobStorageFacade
    {
        public Task<BinaryData> GetBlob(string blobName, string containerName);
        public void UploadBlob(string blobName, string containerName, string path);
        public Task RemoveBlob(string blobName, string containerName);
    }
}
