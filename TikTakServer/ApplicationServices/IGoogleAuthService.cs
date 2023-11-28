using TikTakServer.Models.Business;

namespace TikTakServer.ApplicationServices
{
    public interface IGoogleAuthService
    {
        Task<GoogleInfoModel> VerifyTokenAsync(string accessToken);
        Task<string> GetCountryOfLocation(double longi, double lati);
    }
}
