
using TikTakServer.Models.Business;

namespace TikTakServer.Models
{
    public class UserDao
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public virtual ICollection<VideoDao> Videos {get; set;}
        public virtual ICollection<UserTagInteractionDao> UserTagInteractions {get; set;}
        public virtual ICollection<LikeDao> Likes { get; set; }


        public UserDao()
        {
            
        }
        public UserDao(User user)
        {
            UserName = user.UserName;
            Password = user.Password;
            DateOfBirth = user.DateOfBirth;
        }
    }
}
