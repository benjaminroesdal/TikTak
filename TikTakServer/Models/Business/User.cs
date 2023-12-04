using TikTakServer.Models.DaoModels;

namespace TikTakServer.Models.Business
{
    public class User
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public User()
        {
            
        }
        public User(UserDao user, string refreshToken)
        {
            Email = user.Email;
            FullName = user.FullName;
            ImageUrl = user.ImageUrl;
            DateOfBirth = user.DateOfBirth;
            RefreshToken = refreshToken;
        }
    }
}
