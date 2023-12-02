using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Facades
{
    public interface IVideoFacade
    {
        Task<VideoDao> GetVideo(string id);
        Task<List<VideoModel>> GetRandomVideos(int videoAmount);
        Task CreateVideo(VideoDao video);
        Task<ICollection<TagDao>> AddTag(ICollection<TagModel> tag);
        Task RemoveVideoByStorageId(string id);
        Task CountUserVideoInteraction(UserTagInteraction interaction);
        Task RegisterVideoLike(Like like);
        VideoModel GetRandomVideoBlobId(string name);
    }
}
