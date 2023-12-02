using TikTakServer.Models.Business;

namespace TikTakServer.Models.DaoModels
{
    public class LikeDao
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public DateTime LikeDate { get; set; }
        public UserDao User { get; set; }
        public VideoDao Video { get; set; }

        public LikeDao()
        {
            
        }

        public LikeDao(Like like, int videoId, string userId)
        {
            UserId = int.Parse(userId);
            VideoId = videoId;
            LikeDate = like.LikeDate;
        }
    }
}
