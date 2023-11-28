using Microsoft.AspNetCore.Mvc;
using TikTakServer.Facades;
using TikTakServer.Models.Business;

namespace TikTakServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : Controller
    {
        private readonly IUserFacade _userFacade;

        public VideoController(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        [HttpPost]
        [Route("CountUserVideoInteraction")]
        public async Task<IActionResult> CountUserVideoInteraction([FromBody] UserTagInteraction tagInteraction)
        {
            if (tagInteraction.UserId == 0 || tagInteraction.VideoId == 0)
                return BadRequest("VideoId or UserId not specified");

            await _userFacade.CountUserTagInteraction(tagInteraction);

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

            await _userFacade.RegisterVideoLike(like);

            return Ok();
        }
    }
}
