using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Facades
{
    public interface IVideoFacade
    {
        Task<VideoDao> GetVideo(string id);
        Task<List<VideoModel>> GetRandomVideos(int videoAmount);
        Task SaveVideo(VideoModel video);
        Task RemoveVideoByStorageId(string id);
        Task CountUserVideoInteraction(UserTagInteraction interaction);
        Task RegisterVideoLike(Like like);
        Task<VideoModel> GetRandomVideoBlobId(string name);
    }
}
