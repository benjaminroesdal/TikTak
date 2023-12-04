using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;
using TikTakServer.Repositories;

namespace TikTakServer.Facades
{
    public class VideoFacade : IVideoFacade
    {
        private readonly IVideoRepository _videoRepository;
        private readonly UserRequestAndClaims _userRequestAndClaims;

        public VideoFacade(IVideoRepository videoRepository, UserRequestAndClaims userRequestAndClaims)
        {
            _videoRepository = videoRepository;
            _userRequestAndClaims = userRequestAndClaims;
        }

        public async Task<VideoDao> GetVideo(string id) 
            => await _videoRepository.GetVideo(id);

        public async Task<List<VideoModel>> GetRandomVideos(int videoAmount)
            => await _videoRepository.GetRandomVideos(videoAmount);

        public async Task SaveVideo(VideoModel video)
            => await _videoRepository.SaveVideo(video);

        public async Task<ICollection<TagDao>> AddTag(ICollection<TagModel> tag)
            => await _videoRepository.AddTag(tag);

        public async Task RemoveVideoByStorageId(string id)
            => await _videoRepository.RemoveVideoByStorageId(id);

        public async Task CountUserVideoInteraction(UserTagInteraction interaction)
            => await _videoRepository.CountUserVideoInteraction(interaction);

        public async Task RegisterVideoLike(Like like)
            => await _videoRepository.RegisterVideoLike(like);

        public async Task<VideoModel> GetRandomVideoBlobId(string name)
            => await _videoRepository.GetRandomVideoBlobId(name);
    }
}
