using TikTakServer.Database;
using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly TikTakContext _context;

        public VideoRepository(TikTakContext context)
        {
            _context = context;
        }

        public async Task<Video> GetVideo()
        {
            var videos = _context.Videos.ToList();
            return videos.FirstOrDefault();
        }

        public Task CreateVideo(Video video)
        {
            var result = _context.Add(video);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task RemoveVideoByStorageId(string id)
        {
            var dao = _context.Videos.Where(e => e.BlobStorageId == id).FirstOrDefault();
            _context.Remove(dao);
            _context.SaveChanges(true);
            return Task.CompletedTask;
        }
    }
}
