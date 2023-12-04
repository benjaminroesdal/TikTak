using TikTakServer.Models.Business;

namespace TikTakServer.Models.DaoModels
{
    public class UserDao
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public virtual ICollection<VideoDao> Videos {get; set;}
        public virtual ICollection<UserTagInteractionDao> UserTagInteractions {get; set;}
        public virtual ICollection<LikeDao> Likes { get; set; }
        public virtual ICollection<UserTokenDao> Tokens { get; set; }

        public UserDao()
        {
            
        }

        public UserDao(User user)
        {
            Email = user.Email;
            FullName = user.FullName;
            ImageUrl = user.ImageUrl;
            DateOfBirth = DateTime.Now;
            Tokens = new List<UserTokenDao>()
            {
                new UserTokenDao()
                {
                    RefreshToken = user.RefreshToken,
                    RefreshTokenExpiry = DateTime.Now.AddYears(2)
                }
            };
        }
    }
}
