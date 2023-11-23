using TikTakServer.Database;
using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public class RecommendationRepository
    {
        private readonly TikTakContext _context;
        public RecommendationRepository(TikTakContext context)
        {
            _context = context;
        }

        public void GetRecommendations()
        {

        }

        public async Task<List<UserTagInteractionDao>> GetTagInteractions(int userId)
        {
            var userInteractions = await _context.UserTagsInteractions.Where(i => i.UserId == userId).OrderBy(x => x.InteractionCount).ToListAsync();
            return userInteractions;
        }
    }
}
