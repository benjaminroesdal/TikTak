using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Models;
using TikTakServer.Models.Business;

namespace TikTakServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : Controller
    {
        private readonly TikTakContext _context;

        public VideoController(TikTakContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("CountUserVideoInteraction")]
        public async Task<IActionResult> CountUserVideoInteraction([FromBody] UserTagInteraction tagInteraction)
        {
            if (tagInteraction.UserId == 0 || tagInteraction.VideoId == 0)
                return BadRequest("VideoId or UserId not specified");

            List<TagDao> videoTags = _context.Videos.Where(v => v.Id == tagInteraction.VideoId).SelectMany(x => x.Tags).ToList();
            var usedTagsByUser = _context.Users.Where(u => u.Id == tagInteraction.UserId).SelectMany(x => x.UserTagInteractions).ToList();
            var unusedTags = videoTags.Where(x => !usedTagsByUser.Any(y => y.TagId == x.Id)).ToList();
            var usedTags = videoTags.Where(x => usedTagsByUser.Any(y => y.TagId == x.Id)).ToList();

            //foreach (var tag in unusedTags)
            //{
            //    UserTagInteractionDao dao = new UserTagInteractionDao(tagInteraction);
            //    await _context.UserTagsInteractions.AddAsync(dao);
            //    await _context.SaveChangesAsync();
            //}

            //foreach (var tag in usedTags)
            //{
            //    var userTagInteraction = await _context.UserTagsInteractions.Where(x => x.Tag == tag).FirstOrDefaultAsync();
            //    userTagInteraction.InteractionCount++;
            //    await _context.SaveChangesAsync();
            //}

            return Ok(DateTime.Now);
        }

        [HttpPost]
        [Route("RegisterVideoLike")]
        public async Task<IActionResult> RegisterVideoLike([FromBody] Like like)
        {
            if (like == null || like.UserId == 0 || like.VideoId == 0)
                return BadRequest("Could not register video like. Video og user ID not specified or 0");

            if (like.LikeDate == DateTime.MinValue)
                return BadRequest("Couldnt register like. Date time was default");

            LikeDao likeDao = new LikeDao(like);

            await _context.Likes.AddAsync(likeDao);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
