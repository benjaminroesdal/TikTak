namespace TikTakServer.Repositories
{
    public interface IRecommendationRepository
    {
        Task<List<int>> FindRecommendations();

    }
}
