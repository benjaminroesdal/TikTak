using TikTakServer.Models.DaoModels;

namespace TikTakServer.Models.Business
{
    public class VideoAndOwnedUserInfo
    {
        public int UserId { get; set; }
        public string ProfileImage { get; set; }
        public string Email { get; set; }
        public string BlobVideoStorageId { get; set; }
        public VideoAndOwnedUserInfo(UserDao user, string videoBlobId)
        {
            UserId = user.Id;
            ProfileImage = user.ImageUrl;
            Email = user.Email;
            BlobVideoStorageId = videoBlobId;
        }
    }
}
