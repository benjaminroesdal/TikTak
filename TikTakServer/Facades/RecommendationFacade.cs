
using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Managers;
using TikTakServer.Models;
using TikTakServer.Models.Business;
using TikTakServer.Repositories;

namespace TikTakServer.Facades
{
    public class RecommendationFacade : IRecommendationFacade
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IRecommendationManager _recommendationManager;
        private readonly UserRequestAndClaims _userRequestAndClaims;
        public RecommendationFacade(TikTakContext context, IVideoRepository videoRepository, IRecommendationManager recommendationManager, UserRequestAndClaims userRequestAndClaims)
        {
            _videoRepository = videoRepository;
            _recommendationManager = recommendationManager;
            _userRequestAndClaims = userRequestAndClaims;
        }
        public async Task<UserRequestAndClaims> GetFyp()
        {
            var userPrefTags = _recommendationManager.GetRandomTagsBasedOnUserPreference();
            var blobIds = new List<string>();
            for (int i = 0; i < userPrefTags.Count; i++)
            {
                blobIds.Add(await _videoRepository.GetRandomVideoBlobId(userPrefTags[i]));
            }

            var fypIds = await _videoRepository.GetFyp(blobIds);
            UserInfoAndFypIds infoToFrontend = new UserInfoAndFypIds(_userRequestAndClaims, fypIds);

            return infoToFrontend;
        }
    }
}
