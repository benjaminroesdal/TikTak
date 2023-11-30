using TikTakServer.Models.DaoModels;

namespace TikTakServer.Repositories
{
    public interface IUserRepository
    {
        Task<UserDao> CreateUser(UserDao user);
        Task<UserDao> ValidateRefreshToken(string refreshToken);
        Task<UserDao> GetUser(string email);
        Task CreateTokensOnUser(string email, string refreshToken);
        Task<bool> UserExists(string email);
        List<UserTagInteractionDao> GetUserTagInteractions();
        Task<UserDao> GetUserByVideoBlobId(string blobId);
        Task RemoveRefreshToken(string refreshToken);
    }
}
