namespace TikTakServer.Models.Business
{
    public class CreateUserRequest
    {
        public string GoogleAccessToken { get; set; }
        public string FulLName { get; set; }
        public string ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
