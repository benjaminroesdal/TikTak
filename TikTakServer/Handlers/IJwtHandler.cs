namespace TikTakServer.Handlers
{
    public interface IJwtHandler
    {
        string CreateJwtAccess(int userId, string userEmail, string userImg, string countryName);
        string CreateRefreshToken();
    }
}
