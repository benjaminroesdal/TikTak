namespace TikTakServer.Models.Business
{
    public class User
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AccessToken { get; set; }

        public User(UserDao user, string accessToken)
        {
            Email = user.Email;
            FullName = user.FullName;
            ImageUrl = user.ImageUrl;
            DateOfBirth = user.DateOfBirth;
            AccessToken = accessToken;
        }
    }
}
