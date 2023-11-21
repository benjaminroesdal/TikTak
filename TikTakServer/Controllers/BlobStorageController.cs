using Microsoft.AspNetCore.Mvc;
using System.IO;
using TikTakServer.ApplicationServices;
using TikTakServer.Facades;

namespace TikTakServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlobStorageController : Controller
    {
        private readonly IBlobStorageService _blobStorageService;

        public BlobStorageController(IBlobStorageService blobService)
        {
            _blobStorageService = blobService;
        }

        [HttpPost("PostBlob")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> PostBlob(IFormFile file)
        {
            await _blobStorageService.UploadBlob(file);
            return Ok();
        }

        [HttpPost("RemoveBlob")]
        public async Task<IActionResult> RemoveBlob([FromBody] string blobName)
        {
            await _blobStorageService.RemoveBlobs(blobName);
            return Ok();
        }

        [HttpPost("GetBlobManifest")]
        public async Task<IActionResult> DownloadManifest([FromBody] string blobName)
        {
            var manifest = await _blobStorageService.DownloadManifest();
            return File(manifest, "application/vnd.apple.mpegurl", blobName + ".M3U8");
        }
    }
}
