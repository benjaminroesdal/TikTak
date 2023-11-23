using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public interface IUserRepository
    {
        List<UserTagInteractionDao> GetUserTagInteractions(int userid);
    }
}
