using TikTakServer.Models.DaoModels;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public interface IVideoRepository
    {
        Task CreateVideo(VideoDao video);
        Task RemoveVideoByStorageId(string id);
        Task<VideoDao> GetVideo(string id);
        Task<List<VideoModel>> GetRandomVideos(int videoAmount);
        Task CountUserVideoInteraction(UserTagInteraction interaction);
        Task RegisterVideoLike(Like like);
        VideoModel GetRandomVideoBlobId(string name);
        Task<ICollection<TagDao>> AddTag(ICollection<TagModel> tag);
    }
}
