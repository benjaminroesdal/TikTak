using TikTakServer.Models.DaoModels;

namespace TikTakServer.Models.Business
{
    public class Like
    {
        public int UserId { get; set; }
        public string BlobStorageId { get; set; }
        public DateTime LikeDate { get; set; }

        public Like()
        {
            
        }
        public Like(LikeDao likeDao)
        {
            UserId = likeDao.UserId;
            LikeDate = likeDao.LikeDate;
        }
    }
}
