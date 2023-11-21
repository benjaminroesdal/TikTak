using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public interface IVideoRepository
    {
        Task CreateVideo(Video video);
        Task RemoveVideoByStorageId(string id);
        Task<Video> GetVideo(string id);
        Task<List<string>> GetFyp();
    }
}
