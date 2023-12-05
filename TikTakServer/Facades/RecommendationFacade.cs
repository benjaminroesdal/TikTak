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

        /// <summary>
        /// Gets a fyp based on user preferences
        /// </summary>
        /// <returns>list of VideoUserInfo models containing information regarding each returned video</returns>
        public async Task<List<VideoAndOwnedUserInfo>> GetFyp()
        {
            var videoInfo = new List<VideoAndOwnedUserInfo>();
            var userPrefTags = await _recommendationManager.GetRandomTagsBasedOnUserPreference();
            if (userPrefTags.Count == 0)
            {
                var randomVideos = await _videoFacade.GetRandomVideos(3);
                await ConstructInfoModelForVideo(videoInfo, randomVideos);
                return videoInfo;
            }

            var blobIds = new List<VideoModel>();
            for (int i = 0; i < userPrefTags.Count; i++)
            {
                blobIds.Add(await _videoFacade.GetRandomVideoBlobId(userPrefTags[i]));
            }
            await ConstructInfoModelForVideo(videoInfo, blobIds);

            return videoInfo;
        }

        /// <summary>
        /// Constructs the VideoAndOwnedUserInfo model with relevant information regarding the video.
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <param name="blobIds"></param>
        /// <returns></returns>
        private async Task ConstructInfoModelForVideo(List<VideoAndOwnedUserInfo> videoInfo, List<VideoModel> blobIds)
        {
            for (int i = 0; i < blobIds.Count; i++)
            {
                var vidOwner = await _userFacade.GetUserByVideoBlobId(blobIds[i].BlobStorageId);
                videoInfo.Add(new VideoAndOwnedUserInfo(vidOwner, blobIds[i]));
            }
        }
    }
}
