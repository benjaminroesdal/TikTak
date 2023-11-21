﻿
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using TikTakServer.Facades;
using TikTakServer.Repositories;
using TikTakServer.Models;
using TikTakServer.Handlers;
using System.IO;
using System;

namespace TikTakServer.ApplicationServices
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly IBlobStorageFacade blobStorageFacade;
        private readonly IVideoRepository videoRepository;
        private const string containerName = "tiktaks";

        public BlobStorageService(BlobServiceClient blobServiceClient, IBlobStorageFacade blobStorageFacade, IVideoRepository videoRepository)
        {

            this.blobServiceClient = blobServiceClient;
            this.blobStorageFacade = blobStorageFacade;
            this.videoRepository = videoRepository;
        }

        public Task DownloadBlob(string blobName)
        {
            throw new NotImplementedException();
        }

        public async Task<MemoryStream> DownloadManifest(string id)
        {
            var storageId = await videoRepository.GetVideo(id);
            var manifest = await blobStorageFacade.GetBlob(storageId.BlobStorageId + ".M3U8", containerName);
            var stream = new MemoryStream(manifest.ToArray());
            return stream;
        }

        public async Task<List<string>> GetFyp()
        {
            var storageIds = await videoRepository.GetFyp();
            return storageIds;
        }

        public async Task RemoveBlobs(string blobId)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: blobId))
            {
                await blobStorageFacade.RemoveBlob(blobItem.Name, containerName);
            }
            videoRepository.RemoveVideoByStorageId(blobId);
        }

        public async Task UploadBlob(IFormFile file)
        {
            var handler = new HlsHandler();
            var blobGuid = Guid.NewGuid().ToString();
            HlsObj hlsObj = new HlsObj();
            using (var stream = file.OpenReadStream())
            {
                hlsObj = await handler.ConvertToHls(stream, blobGuid);
            }
            await blobStorageFacade.UploadBlob(blobGuid + $".M3U8", containerName, hlsObj.Path + $"\\{blobGuid}.M3U8");
            for (int i = 0; i < hlsObj.FileCount; i++)
            {
                await blobStorageFacade.UploadBlob(blobGuid + $"{i}.ts", containerName, hlsObj.Path + $"\\{blobGuid}{i}.ts");
            }
            await handler.ClearTempFiles(hlsObj.Guid, hlsObj.Path);
            videoRepository.CreateVideo(new Video()
            {
                BlobStorageId = blobGuid,
                UploadDate = DateTime.Now,
                UserId = 1
            });
        }
    }
}
