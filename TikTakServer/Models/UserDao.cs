using TikTakServer.Models.Business;

namespace TikTakServer.Models
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
        public virtual UserTokenDao Token { get; set; }

        public UserDao()
        {
            
        }

        public UserDao(User user)
        {
            DateOfBirth = user.DateOfBirth;
        }
    }
}
