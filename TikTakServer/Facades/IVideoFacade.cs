using TikTakServer.Models.Business;

namespace TikTakServer.Facades
{
    public interface IVideoFacade
    {
        Task<List<VideoModel>> GetRandomVideos(int videoAmount);
        Task SaveVideo(VideoModel video);
        Task RemoveVideoByStorageId(string id);
        Task IncrementUserVideoInteraction(string blobStorageId);
        Task RegisterVideoLike(Like like);
        Task<VideoModel> GetRandomVideoBlobId(string tagName);
    }
}
