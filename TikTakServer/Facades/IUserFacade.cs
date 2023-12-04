using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Facades
{
    public interface IUserFacade
    {
        Task CountUserTagInteraction(UserTagInteraction interaction);
        Task RegisterVideoLike(Like like);
        Task<User> CreateUser(User user);
        Task<User> GetUserOnRefreshToken(string refreshToken);
        Task<UserDao> GetUserByVideoBlobId(string blobId);
        Task<bool> UserExists(string email);
        Task CreateTokensOnUser(string email, string refreshToken);
        Task<bool> IsRefreshTokenValid(string refreshToken);
        Task<List<UserTagInteractionDao>> GetUserTagInteractions();
        Task RemoveRefreshToken(string refreshToken);
    }
}
