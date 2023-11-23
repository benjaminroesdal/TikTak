using TikTakServer.Database;
using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly TikTakContext _context;
        public UserRepository(TikTakContext context)
        {
            _context = context;
        }

        public List<UserTagInteractionDao> GetUserTagInteractions(int userId)
        {
            var userInteractions = _context.UserTagsInteractions.Where(i => i.UserId == userId).OrderBy(x => x.InteractionCount).ToList();
            return userInteractions;
        }
    }
}
