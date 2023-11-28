using Newtonsoft.Json;
using TikTakServer.Models.Business;

namespace TikTakServer.ApplicationServices
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _androidClientId;
        private readonly string _googleTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo";
        private readonly string _id;


        public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _clientId = configuration["ClientId"];
            _androidClientId = configuration["AndroidClientId"];
            _id = configuration["AndroidClientId2"];
        }

        public async Task<GoogleInfoModel> VerifyToken(string accessToken)
        {
            var response = await _httpClient.GetAsync($"{_googleTokenInfoUrl}?access_token={accessToken}");
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var googleInfo = JsonConvert.DeserializeObject<GoogleInfoModel>(jsonContent);

            if (googleInfo.Audience != _clientId && googleInfo.Audience != _androidClientId && googleInfo.Audience != _id)
            {
                throw new UnauthorizedAccessException("Token was not issued for this application");
            }

            return googleInfo;
        }

        public async Task<string> GetCountryOfLocation(double longi, double lati)
        {
            var geoUrl = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lati},{longi}&key=AIzaSyCFzyvOxCneBvkJhTV1hj8R5UzpYMMpTHM";
            var response = await _httpClient.GetAsync(geoUrl);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var googleInfo = JsonConvert.DeserializeObject<LocationResult>(jsonContent);
            //var countryName = googleInfo.results.Where(e => e.types.Contains("country")).First().address_components.FirstOrDefault().long_name;
            return "Denmark";
        }
    }
}
