namespace TikTakServer.Handlers
{
    public interface IJwtHandler
    {
        string CreateJwtAccess(string userEmail, string userImg);
        string CreateRefreshToken();
    }
}
