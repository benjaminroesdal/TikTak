using TikTakServer.Models;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public interface IVideoRepository
    {
        Task<Task> CreateVideo(VideoDao video);
        Task RemoveVideoByStorageId(string id);
        Task<VideoDao> GetVideo(string id);
        Task<List<string>> GetFyp(List<string> vidIds);
        Task CountUserVideoInteraction(UserTagInteraction interaction);
        Task<Task> RegisterVideoLike(Like like);
        Task<string> GetRandomVideoBlobId(string name);
        Task<ICollection<TagDao>> AddTag(ICollection<TagModel> tag);
    }
}
