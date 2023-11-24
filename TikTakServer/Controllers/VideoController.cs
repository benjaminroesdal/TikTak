using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TikTakServer.ApplicationServices;
using TikTakServer.Database;
using TikTakServer.Models;
using TikTakServer.Models.Business;

namespace TikTakServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : Controller
    {
        private readonly IRecommendationService _recommendationService;

        public VideoController(TikTakContext context, IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpPost]
        [Route("CountUserVideoInteraction")]
        public async Task<IActionResult> CountUserVideoInteraction([FromBody] UserTagInteraction tagInteraction)
        {
            if (tagInteraction.UserId == 0 || tagInteraction.VideoId == 0)
                return BadRequest("VideoId or UserId not specified");

            //await _recommendationService.CountUserTagInteraction(tagInteraction);

            return Ok();
        }

        [HttpPost]
        [Route("RegisterVideoLike")]
        public async Task<IActionResult> RegisterVideoLike([FromBody] Like like)
        {
            if (like == null || like.UserId == 0 || like.VideoId == 0)
                return BadRequest("Could not register video like. Video og user ID not specified or 0");

            if (like.LikeDate == DateTime.MinValue)
                return BadRequest("Couldnt register like. Date time was default");

            //await _recommendationService.RegisterVideoLike(like);

            return Ok();
        }
    }
}
