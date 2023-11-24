using TikTakServer.Models.Business;
using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public interface IUserRepository
    {
        Task<UserDao> CreateUser(UserDao user);
        Task<UserDao> ValidateRefreshToken(string refreshToken);
        Task InvalidateRefreshToken(string refreshToken);
        Task<UserDao> GetUser(string email);
    }
}
