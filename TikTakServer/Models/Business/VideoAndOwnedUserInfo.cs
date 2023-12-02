using TikTakServer.Models.DaoModels;

namespace TikTakServer.Models.Business
{
    public class VideoAndOwnedUserInfo
    {
        public int UserId { get; set; }
        public string ProfileImage { get; set; }
        public string Email { get; set; }
        public VideoModel Video { get; set; }
        public VideoAndOwnedUserInfo(UserDao user, VideoModel video)
        {
            UserId = user.Id;
            ProfileImage = user.ImageUrl;
            Email = user.Email;
            Video = video;
        }
    }
}
