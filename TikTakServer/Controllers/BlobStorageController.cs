﻿using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost("PostBlob")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> PostBlob([FromForm] PostBlobModel file)
        {
            if (file.Tags.Count == 0)
            {
                return BadRequest("Cannot upload blob, no tags was specified");
            }

            await _blobStorageService.UploadBlob(file);
            return Ok();
        }

        [Authorize]
        [HttpPost("RemoveBlob")]
        public async Task<IActionResult> RemoveBlob([FromBody] string blobName)
        {
            if (String.IsNullOrEmpty(blobName))
            {
                return BadRequest("No blobname specified, could not remove blob");
            }

            await _blobStorageService.RemoveBlobs(blobName);
            return Ok();
        }

        [HttpGet("GetBlobManifest")]
        public async Task<IActionResult> DownloadManifest([FromQuery] string id)
        {
            if (id == null)
            {
                return BadRequest("No blob id specified");
            }

            var manifest = await _blobStorageService.DownloadManifest(id);
            return File(manifest, "application/vnd.apple.mpegurl", "manifest" + ".M3U8");
        }

        [Authorize]
        [HttpGet("GetFyp")]
        public async Task<IActionResult> GetFyp()
        {
            var videoAndOwnedUserInfo = await _blobStorageService.GetFyp();

            if (videoAndOwnedUserInfo == null || videoAndOwnedUserInfo.Count == 0)
            {
                return BadRequest("No user info and blob IDs found");
            }

            return Ok(videoAndOwnedUserInfo);
        }
    }
}
