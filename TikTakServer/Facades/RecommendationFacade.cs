using TikTakServer.Managers;
using TikTakServer.Models.Business;

namespace TikTakServer.Facades
{
    public class RecommendationFacade : IRecommendationFacade
    {
        private readonly IVideoFacade _videoFacade;
        private readonly IRecommendationManager _recommendationManager;
        private readonly IUserFacade _userFacade;
        public RecommendationFacade(IVideoFacade videoFacade,
            IRecommendationManager recommendationManager,
            IUserFacade userFacade)
        {
            _videoFacade = videoFacade;
            _recommendationManager = recommendationManager;
            _userFacade = userFacade;
        }
        public async Task<List<VideoAndOwnedUserInfo>> GetFyp()
        {
            var userPrefTags = _recommendationManager.GetRandomTagsBasedOnUserPreference();
            var blobIds = new List<VideoModel>();
            for (int i = 0; i < userPrefTags.Count; i++)
            {
                blobIds.Add(_videoFacade.GetRandomVideoBlobId(userPrefTags[i]));
            }
            var infoToFrontend = new List<VideoAndOwnedUserInfo>();

            for (int i = 0; i < blobIds.Count; i++)
            {
                var vidOwner = await _userFacade.GetUserByVideoBlobId(blobIds[i].BlobStorageId);
                infoToFrontend.Add(new VideoAndOwnedUserInfo(vidOwner, blobIds[i]));
            }

            return infoToFrontend;
        }
    }
}
