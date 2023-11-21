
namespace TikTakServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public virtual ICollection<Video> Videos {get; set;}
        public virtual ICollection<UserTagInteraction> UserTagInteractions {get; set;}
        public virtual ICollection<Like> Likes { get; set; }
    }
}
