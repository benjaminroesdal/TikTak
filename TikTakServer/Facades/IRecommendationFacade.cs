using TikTakServer.Models;

namespace TikTakServer.Facades
{
    public interface IRecommendationFacade
    {
        Task<UserInfoAndFypIds> GetFyp();
    }
}
