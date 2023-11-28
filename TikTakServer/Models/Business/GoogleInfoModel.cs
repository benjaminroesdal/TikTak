using Newtonsoft.Json;

namespace TikTakServer.Models.Business
{
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
