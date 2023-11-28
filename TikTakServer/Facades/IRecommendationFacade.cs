using TikTakServer.Models.Business;

namespace TikTakServer.Facades
{
    public interface IRecommendationFacade
    {
        Task<List<VideoAndOwnedUserInfo>> GetFyp();
    }
}
