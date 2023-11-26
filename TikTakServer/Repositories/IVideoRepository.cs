using TikTakServer.Models;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public interface IVideoRepository
    {
        Task CreateVideo(VideoDao video);
        Task RemoveVideoByStorageId(string id);
        Task<VideoDao> GetVideo(string id);
        Task<List<string>> GetFyp(List<string> vidIds);
        Task CountUserVideoInteraction(UserTagInteraction interaction);
        Task RegisterVideoLike(Like like);
        Task<int> GetTagCount(string name);
        Task<string> GetRandomVideoBlobId(string name);
    }
}
