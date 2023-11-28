using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Facades
{
    public interface IUserFacade
    {
        Task CountUserTagInteraction(UserTagInteraction interaction);
        Task RegisterVideoLike(Like like);
        Task<UserDao> CreateUser(UserDao user);
        Task<UserDao> GetUser(string email);
        Task<UserDao> GetUserByVideoBlobId(string blobId);
        Task<bool> UserExists(string email);
        Task CreateTokensOnUser(string email, string refreshToken);
        Task<UserDao> ValidateRefreshToken(string refreshToken);
        List<UserTagInteractionDao> GetUserTagInteractions();
    }
}
