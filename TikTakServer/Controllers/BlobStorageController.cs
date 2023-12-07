using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TikTakServer.ApplicationServices;
using TikTakServer.Models.Business;

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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ",ApiKey")]
        public async Task<IActionResult> PostBlob([FromForm] PostBlobModel file)
        {
            await _blobStorageService.UploadBlob(file);
            return Ok();
        }

        [HttpPost("RemoveBlob")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ",ApiKey")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ",ApiKey")]
        public async Task<IActionResult> GetFyp()
        {
            var videoAndOwnedUserInfo = await _blobStorageService.GetFyp();
            return Ok(videoAndOwnedUserInfo);
        }
    }
}
