using TikTakServer.Models.Business;
using TikTakServer.Repositories;

namespace TikTakServer.Facades
{
    public class VideoFacade : IVideoFacade
    {
        private readonly IVideoRepository _videoRepository;

        public VideoFacade(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<List<VideoModel>> GetRandomVideos(int videoAmount)
            => await _videoRepository.GetRandomVideos(videoAmount);

        public async Task SaveVideo(VideoModel video)
            => await _videoRepository.SaveVideo(video);

        public async Task RemoveVideoByStorageId(string id)
            => await _videoRepository.RemoveVideoByStorageId(id);

        public async Task IncrementUserVideoInteraction(string blobStorageId)
            => await _videoRepository.IncrementUserVideoInteraction(blobStorageId);

        public async Task RegisterVideoLike(Like like)
            => await _videoRepository.RegisterVideoLike(like);

        public async Task<VideoModel> GetRandomVideoBlobId(string name)
            => await _videoRepository.GetRandomVideoBlobId(name);
    }
}
