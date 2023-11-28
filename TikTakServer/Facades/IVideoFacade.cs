using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Facades
{
    public interface IVideoFacade
    {
        Task<VideoDao> GetVideo(string id);
        Task<List<string>> GetFyp(List<string> videoIds);
        Task CreateVideo(VideoDao video);
        Task<ICollection<TagDao>> AddTag(ICollection<TagModel> tag);
        Task RemoveVideoByStorageId(string id);
        Task CountUserVideoInteraction(UserTagInteraction interaction);
        Task RegisterVideoLike(Like like);
        string GetRandomVideoBlobId(string name);
    }
}
