using TikTakServer.Models.Business;
using TikTakServer.Repositories;

namespace TikTakServer.ApplicationServices
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IRecommendationRepository _recommendationRepository;
        public RecommendationService(IVideoRepository videoRepository, IRecommendationRepository recommendationRepository)
        {
            _videoRepository = videoRepository;
            _recommendationRepository = recommendationRepository;
        }

        public Task CountUserTagInteraction(UserTagInteraction interaction)
        {
            _videoRepository.CountUserVideoInteraction(interaction);
            return Task.CompletedTask;
        }

        public Task RegisterVideoLike(Like like)
        {
            _videoRepository.RegisterVideoLike(like);
            return Task.CompletedTask;
        }
    }
}
