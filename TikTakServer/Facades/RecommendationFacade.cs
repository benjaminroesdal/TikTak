
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
        private readonly IUserRepository _userRepository;
        public RecommendationFacade(TikTakContext context, IVideoRepository videoRepository, IRecommendationManager recommendationManager, IUserRepository userRepository, UserRequestAndClaims userRequestAndClaims)
        {
            _videoRepository = videoRepository;
            _recommendationManager = recommendationManager;
            _userRequestAndClaims = userRequestAndClaims;
            _userRepository = userRepository;
        }
        public async Task<List<VideoAndOwnedUserInfo>> GetFyp()
        {
            var userPrefTags = _recommendationManager.GetRandomTagsBasedOnUserPreference();
            var blobIds = new List<string>();
            for (int i = 0; i < userPrefTags.Count; i++)
            {
                blobIds.Add(await _videoRepository.GetRandomVideoBlobId(userPrefTags[i]));
            }
            var fypIds = await _videoRepository.GetFyp(blobIds);
            var infoToFrontend = new List<VideoAndOwnedUserInfo>();

            for (int i = 0; i < fypIds.Count; i++)
            {
                var vidOwner = await _userRepository.GetUserByVideoBlobId(fypIds[i]);
                infoToFrontend.Add(new VideoAndOwnedUserInfo(vidOwner, fypIds[i]));
            }



            return infoToFrontend;
        }
    }
}
