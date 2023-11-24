namespace TikTakServer.Models
{
    public class TagDao
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<VideoDao> Videos { get; set; }
    }
}
