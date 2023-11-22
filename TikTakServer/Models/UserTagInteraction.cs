namespace TikTakServer.Models
{
    public class UserTagInteraction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TagId { get; set; }
        public int InteractionCount { get; set; }
        public UserDao User { get; set; }
        public Tag Tag { get; set; }
    }
}
