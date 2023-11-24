
using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Managers;
using TikTakServer.Models.Business;
using TikTakServer.Repositories;

namespace TikTakServer.Facades
{
    public class RecommendationFacade : IRecommendationFacade
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IRecommendationManager _recommendationManager;
        public RecommendationFacade(TikTakContext context, IVideoRepository videoRepository, IRecommendationManager recommendationManager)
        {
            _videoRepository = videoRepository;
            _recommendationManager = recommendationManager;
        }
        public async Task<List<string>> GetFyp(int userId)
        {
            var userPrefTags = _recommendationManager.GetRandomTagsBasedOnUserPreference(userId);
            var blobIds = new List<string>();
            for (int i = 0; i < userPrefTags.Count; i++)
            {
                blobIds.Add(await _videoRepository.GetRandomVideoBlobId(userPrefTags[i]));
            }

            return await _videoRepository.GetFyp(blobIds);
        }
    }
}
