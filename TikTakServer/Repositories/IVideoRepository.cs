using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public interface IVideoRepository
    {
        Task SaveVideo(VideoModel video);
        Task RemoveVideoByStorageId(string id);
        Task<List<VideoModel>> GetRandomVideos(int videoAmount);
        Task IncrementUserVideoInteraction(string blobStorageId);
        Task RegisterVideoLike(Like like);
        Task<VideoModel> GetRandomVideoBlobId(string tagName);
    }
}
