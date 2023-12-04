using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Facades
{
    public interface IUserFacade
    {
        Task<User> CreateUser(User user);
        Task<User> GetUser(string refreshToken);
        Task<User> GetUserOnRefreshToken(string refreshToken);
        Task<User> GetUserByVideoBlobId(string blobId);
        Task<bool> UserExists(string email);
        Task CreateTokensOnUser(string email, string refreshToken);
        Task<bool> IsRefreshTokenValid(string refreshToken);
        Task<List<UserTagInteractionModel>> GetUserTagInteractions();
        Task RemoveRefreshToken(string refreshToken);
    }
}
