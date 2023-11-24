namespace TikTakServer.Facades
{
    public interface IRecommendationFacade
    {
        Task<List<string>> GetFyp(int userId);
    }
}
