using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<bool> IsRefreshTokenValid(string refreshToken);
        Task<User> GetUserOnRefreshToken(string refreshToken);
        Task CreateTokensOnUser(string email, string refreshToken);
        Task<bool> UserExists(string email);
        Task<List<UserTagInteractionDao>> GetUserTagInteractions();
        Task<UserDao> GetUserByVideoBlobId(string blobId);
        Task RemoveRefreshToken(string refreshToken);
    }
}
