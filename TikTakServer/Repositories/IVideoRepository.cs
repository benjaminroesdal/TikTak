using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public interface IVideoRepository
    {
        Task CreateVideo(VideoDao video);
        Task RemoveVideoByStorageId(string id);
        Task<VideoDao> GetVideo(string id);
        Task<List<string>> GetFyp();
    }
}
