using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;
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

        public Task<VideoDao> GetVideo(string id) 
            => _videoRepository.GetVideo(id);

        public Task<List<string>> GetFyp(List<string> videoIds)
            => _videoRepository.GetFyp(videoIds);

        public Task CreateVideo(VideoDao video)
            => _videoRepository.CreateVideo(video);

        public Task<ICollection<TagDao>> AddTag(ICollection<TagModel> tag)
            => _videoRepository.AddTag(tag);

        public Task RemoveVideoByStorageId(string id)
            => _videoRepository.RemoveVideoByStorageId(id);

        public Task CountUserVideoInteraction(UserTagInteraction interaction)
            => _videoRepository.CountUserVideoInteraction(interaction);

        public Task RegisterVideoLike(Like like)
            => _videoRepository.RegisterVideoLike(like);

        public string GetRandomVideoBlobId(string name)
            => _videoRepository.GetRandomVideoBlobId(name);
    }
}
