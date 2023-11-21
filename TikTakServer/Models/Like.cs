namespace TikTakServer.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public DateTime LikeDate { get; set; }
        public User User { get; set; }
        public Video Video { get; set; }
    }
}
