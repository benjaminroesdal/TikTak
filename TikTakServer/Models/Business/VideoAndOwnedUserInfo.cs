using TikTakServer.Models.DaoModels;

namespace TikTakServer.Models.Business
{
    public class VideoAndOwnedUserInfo
    {
        public string ProfileImage { get; set; }
        public string Email { get; set; }
        public VideoModel Video { get; set; }
        public VideoAndOwnedUserInfo(User user, VideoModel video)
        {
            ProfileImage = user.ImageUrl;
            Email = user.Email;
            Video = video;
        }
    }
}
