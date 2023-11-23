using TikTakServer.Models;

namespace TikTakServer.Managers
{
    public interface IRecommendationManager
    {
        List<TagDao> GetRandomTagsBasedOnUserPreference(int userid);
    }
}
