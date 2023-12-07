using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TikTakServer.Facades;
using TikTakServer.Models.Business;

namespace TikTakServer.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ",ApiKey")]
    [Route("[controller]")]
    public class VideoController : Controller
    {
        private readonly IVideoFacade _videoFacade;

        public VideoController(IVideoFacade videoFacade)
        {
            _videoFacade = videoFacade;
        }

        [HttpPost]
        [Route("IncrementUserVideoInteraction")]
        public async Task<IActionResult> IncrementUserVideoInteraction([FromBody] string blobStorageId)
        {
            if (string.IsNullOrEmpty(blobStorageId))
                return BadRequest("VideoId or blob storage ID not specified");

            await _videoFacade.IncrementUserVideoInteraction(blobStorageId);

            return Ok();
        }

        [HttpPost]
        [Route("RegisterVideoLike")]
        public async Task<IActionResult> RegisterVideoLike([FromBody] Like like)
        {
            if (like == null || string.IsNullOrEmpty(like.BlobStorageId))
                return BadRequest("Could not register video like. Blob storage og user ID not specified or 0");

            if (like.LikeDate == DateTime.MinValue)
                return BadRequest("Couldnt register like. Date time was default");

            await _videoFacade.RegisterVideoLike(like);
            await _videoFacade.IncrementUserVideoInteraction(like.BlobStorageId);

            return Ok();
        }
    }
}
