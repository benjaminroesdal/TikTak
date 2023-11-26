﻿using Azure.Core;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TikTakServer.ApplicationServices
{
    public class GoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _androidClientId;
        private readonly string _googleTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo";


        public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _clientId = configuration["ClientId"];
            _androidClientId = configuration["AndroidClientId"];
        }

        public async Task<GoogleInfoModel> VerifyTokenAsync(string accessToken)
        {
            var response = await _httpClient.GetAsync($"{_googleTokenInfoUrl}?access_token={accessToken}");
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var googleInfo = JsonConvert.DeserializeObject<GoogleInfoModel>(jsonContent);

            if (googleInfo.Audience != _clientId && googleInfo.Audience != _androidClientId)
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
            var googleInfo = JsonConvert.DeserializeObject<Root>(jsonContent);
            var countryName = googleInfo.results.Where(e => e.types.Contains("country")).First().address_components.FirstOrDefault().long_name;
            return countryName;
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


    public class Root
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public List<string> types { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }
}
