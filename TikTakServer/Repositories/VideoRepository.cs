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

        public async Task<VideoDao> GetVideo(string id)
        {
            var video = _context.Videos.Where(e => e.BlobStorageId == id);
            return video.FirstOrDefault();
        }

        public async Task<List<string>> GetFyp()
        {
            var videoIds = _context.Videos.Select(e => e.BlobStorageId).ToList();
            return videoIds;
        }

        public Task CreateVideo(VideoDao video)
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
