namespace TikTakServer.Models
{
    public class UserInfoAndFypIds
    {
        public string UserId { get; set; }
        public string ProfileImage { get; set; }
        public string Email { get; set; }
        public List<string> BlobVideoStorageIds { get; set; }
        public UserInfoAndFypIds(UserRequestAndClaims userInfo, List<string> videoBlobIds)
        {
            UserId = userInfo.UserId;
            ProfileImage = userInfo.ProfileImage;
            Email = userInfo.Email;
            BlobVideoStorageIds = videoBlobIds;
        }
    }
}
