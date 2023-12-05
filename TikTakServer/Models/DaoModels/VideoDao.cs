using TikTakServer.Models.Business;

namespace TikTakServer.Models.DaoModels
{
    public class VideoDao
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BlobStorageId { get; set; }
        public DateTime UploadDate { get; set; }
        public UserDao User { get; set; }
        public virtual ICollection<TagDao> Tags { get; set; }
        public virtual ICollection<LikeDao> Likes { get; set; }

        public VideoDao()
        {
            
        }

        public VideoDao(VideoModel videoModel)
        {
            BlobStorageId = videoModel.BlobStorageId;
            UploadDate = videoModel.UploadDate;
            Tags = new List<TagDao>();
        }
    }
}
