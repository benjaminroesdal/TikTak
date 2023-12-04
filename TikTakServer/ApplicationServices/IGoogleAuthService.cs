using TikTakServer.Models.Business;

namespace TikTakServer.ApplicationServices
{
    public interface IGoogleAuthService
    {
        Task<GoogleInfoModel> VerifyToken(string accessToken);
    }
}
