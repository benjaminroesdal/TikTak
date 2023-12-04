namespace TikTakServer.Managers
{
    public interface IRecommendationManager
    {
        Task<List<string>> GetRandomTagsBasedOnUserPreference();

    }
}
