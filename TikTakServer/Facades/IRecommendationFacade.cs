using TikTakServer.Models;

namespace TikTakServer.Facades
{
    public interface IRecommendationFacade
    {
        Task<List<VideoAndOwnedUserInfo>> GetFyp();
    }
}
