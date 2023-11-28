using TikTakServer.Models.DaoModels;

namespace TikTakServer.Models.Business
{
    public class Like
    {
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public DateTime LikeDate { get; set; }

        public Like()
        {
            
        }
        public Like(LikeDao likeDao)
        {
            UserId = likeDao.UserId;
            VideoId = likeDao.VideoId;
            LikeDate = likeDao.LikeDate;
        }
    }
}
