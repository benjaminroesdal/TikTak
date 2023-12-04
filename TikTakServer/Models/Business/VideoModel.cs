using TikTakServer.Models.DaoModels;

namespace TikTakServer.Models.Business
{
    public class VideoModel
    {
        public string BlobStorageId { get; set; }
        public DateTime UploadDate { get; set; }
        public List<TagModel> Tags { get; set; }
        public int Likes { get; set; }

        public VideoModel()
        {
            
        }

        public VideoModel(VideoDao dao)
        {
            Tags = new List<TagModel>();
            dao.Tags.ToList().ForEach(e => Tags.Add(new TagModel() { Name = e.Name}));
            Likes = dao.Likes.Count();
            BlobStorageId = dao.BlobStorageId;
            UploadDate = dao.UploadDate;
        }
    }
}
