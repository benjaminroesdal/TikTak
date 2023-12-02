namespace TikTakServer.Managers
{
    public interface IRecommendationManager
    {
        List<string> GetRandomTagsBasedOnUserPreference();

    }
}
