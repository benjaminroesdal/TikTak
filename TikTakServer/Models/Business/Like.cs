using TikTakServer.Models.DaoModels;

namespace TikTakServer.Models.Business
{
    public class Like
    {
        public string BlobStorageId { get; set; }
        public DateTime LikeDate { get; set; }

        public Like()
        {
            
        }
    }
}
