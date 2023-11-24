using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TikTakServer.ApplicationServices
{
    public class GoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _googleTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo";


        public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _clientId = configuration["ClientId"];
        }

        public async Task<GoogleInfoModel> VerifyTokenAsync(string accessToken)
        {
            var response = await _httpClient.GetAsync($"{_googleTokenInfoUrl}?access_token={accessToken}");
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var googleInfo = JsonConvert.DeserializeObject<GoogleInfoModel>(jsonContent);

            if (googleInfo.Audience != _clientId)
            {
                throw new UnauthorizedAccessException("Token was not issued for this application");
            }

            return googleInfo;
        }
    }


    public class GoogleInfoModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("exp")]
        public string ExpireDate { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("email_verified")]
        public string EmailVerified { get; set; }
        [JsonProperty("aud")]
        public string Audience { get; set; }
    }
}
