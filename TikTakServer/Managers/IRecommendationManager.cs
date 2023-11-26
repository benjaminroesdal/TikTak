using TikTakServer.Models;

namespace TikTakServer.Managers
{
    public interface IRecommendationManager
    {
        List<string> GetRandomTagsBasedOnUserPreference();

    }
}
