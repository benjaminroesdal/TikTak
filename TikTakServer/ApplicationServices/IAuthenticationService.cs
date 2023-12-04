using TikTakServer.Models.Business;

namespace TikTakServer.ApplicationServices
{
    public interface IAuthenticationService
    {
        Task<User> RefreshAccessToken(string refreshToken);
        Task<User> Login(string googleAccessToken, string name, string imgUrl);
        Task Logout(string refreshToken);
    }
}
