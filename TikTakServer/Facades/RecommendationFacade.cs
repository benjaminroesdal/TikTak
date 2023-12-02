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
            var infoToFrontend = new List<VideoAndOwnedUserInfo>();
            var userPrefTags = _recommendationManager.GetRandomTagsBasedOnUserPreference();
            if (userPrefTags.Count == 0)
            {
                var randomVideos = await _videoFacade.GetRandomVideos(3);
                await ConstructInfoModelForVideo(infoToFrontend, randomVideos);
                return infoToFrontend;
            }

            var blobIds = new List<VideoModel>();
            for (int i = 0; i < userPrefTags.Count; i++)
            {
                blobIds.Add(_videoFacade.GetRandomVideoBlobId(userPrefTags[i]));
            }
            await ConstructInfoModelForVideo(infoToFrontend, blobIds);

            return infoToFrontend;
        }

        private async Task ConstructInfoModelForVideo(List<VideoAndOwnedUserInfo> infoToFrontend, List<VideoModel> blobIds)
        {
            for (int i = 0; i < blobIds.Count; i++)
            {
                var vidOwner = await _userFacade.GetUserByVideoBlobId(blobIds[i].BlobStorageId);
                infoToFrontend.Add(new VideoAndOwnedUserInfo(vidOwner, blobIds[i]));
            }
        }
    }
}
