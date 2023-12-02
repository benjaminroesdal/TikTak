using TikTakServer.Models.Business;

namespace TikTakServer.Models.DaoModels
{
    public class UserTagInteractionDao
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TagId { get; set; }
        public int InteractionCount { get; set; }
        public UserDao User { get; set; }
        public TagDao Tag { get; set; }

        public UserTagInteractionDao()
        {
            
        }

        public UserTagInteractionDao(string userId, int tagId)
        {
            UserId = int.Parse(userId);
            TagId = tagId;
            InteractionCount = 1;
        }
    }

}
