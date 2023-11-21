namespace TikTakServer.Models
{
    public class Video
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BlobStorageId { get; set; }
        public DateTime UploadDate { get; set; }
        public User User { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
