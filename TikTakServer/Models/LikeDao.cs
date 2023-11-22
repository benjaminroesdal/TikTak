using TikTakServer.Models.Business;

namespace TikTakServer.Models
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

        public LikeDao(Like like)
        {
            UserId = like.UserId;
            VideoId = like.VideoId;
            LikeDate = like.LikeDate;
        }
    }
}
