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
        public async void CountUserVideoInteraction([FromBody] UserTagInteraction tagInteraction)
        {
            List<Tag> videoTags = _context.Videos.Where(v => v.Id == tagInteraction.VideoId).SelectMany(x => x.Tags).ToList();
            var usedTagsByUser = _context.Users.Where(u => u.Id == tagInteraction.UserId).SelectMany(x => x.UserTagInteractions).ToList();
            var unusedTagsByUser = videoTags.Select(x => x.Id).Where(x => x.Equals(""));


            foreach (var item in videoTags)
            {
                
            }
        }
    }
}
