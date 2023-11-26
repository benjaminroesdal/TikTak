using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using TikTakServer.ApplicationServices;
using TikTakServer.Facades;
using TikTakServer.Models;

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

        [HttpGet("GetBlobManifest")]
        public async Task<IActionResult> DownloadManifest([FromQuery] string id)
        {
            var manifest = await _blobStorageService.DownloadManifest(id);
            return File(manifest, "application/vnd.apple.mpegurl", "manifest" + ".M3U8");
        }

        [HttpGet("GetFyp")]
        public async Task<IActionResult> GetFyp()
        {
            var userInfoAndFypIds = await _blobStorageService.GetFyp();
            return Ok(userInfoAndFypIds.BlobVideoStorageIds);
        }
    }
}
