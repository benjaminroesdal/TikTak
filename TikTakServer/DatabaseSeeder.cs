using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Models.DaoModels;

namespace TikTakServer
{
    public class DatabaseSeeder
    {
        private readonly TikTakContext _context;

        public DatabaseSeeder(TikTakContext context)
        {
            _context = context;
        }

        public void SeedDb()
        {
            var containsUsers = _context.Users.Any();
            List<TagDao> tags = new List<TagDao>();
            if (containsUsers)
            {
                var user = _context.Users.Include(e => e.UserTagInteractions).Include(e => e.Videos).First();
                _context.Database.Migrate();

            if (!_context.Tags.Any())
            {
                tags = new List<TagDao>
                {
                    new TagDao() { Name = "Dog" },
                    new TagDao() { Name = "Cat" },
                    new TagDao() { Name = "Cars" },
                    new TagDao() { Name = "Dance" },
                    new TagDao() { Name = "Football" },
                    new TagDao() { Name = "Mountain" },
                    new TagDao() { Name = "Elephant" },
                    new TagDao() { Name = "Outdoor" },
                    new TagDao() { Name = "Caps" },
                    new TagDao() { Name = "Sleep" },
                    new TagDao() { Name = "DIY" },
                    new TagDao() { Name = "Routine" },
                    new TagDao() { Name = "Morning" },
                    new TagDao() { Name = "Noon" },
                    new TagDao() { Name = "Night" }
                };    
            }

            if (!_context.Videos.Any())
            {
                List<VideoDao> videos = new List<VideoDao>
                {
                    new VideoDao()
                {
                    UserId = user.Id,
                    BlobStorageId = "9dd6789d-f27f-4411-8549-9e6e287e6bee",
                    UploadDate = DateTime.Now,
                    Tags = tags
                },
                new VideoDao()
                {
                    UserId = user.Id,
                    BlobStorageId = "933c546f-8401-44f4-8be9-ac56d8c20df4",
                    UploadDate = DateTime.Now,
                    Tags = tags
                },
                new VideoDao()
                {
                    UserId = user.Id,
                    BlobStorageId = "8fe579fb-c191-453b-a61d-73b472e3e124",
                    UploadDate = DateTime.Now,
                    Tags = tags
                },
                new VideoDao()
                {
                    UserId = user.Id,
                    BlobStorageId = "831b5d59-cb28-4c03-82e4-3782edb110ed",
                    UploadDate = DateTime.Now,
                    Tags = tags
                },
                new VideoDao()
                {
                    UserId = user.Id,
                    BlobStorageId = "7f8b54ea-b3ce-4fd5-9866-dbb3fc8e9017",
                    UploadDate = DateTime.Now,
                    Tags = tags
                },
                new VideoDao()
                {
                    UserId = user.Id,
                    BlobStorageId = "73594b29-d954-40e6-9238-0d98b27b761f",
                    UploadDate = DateTime.Now,
                    Tags = tags
                },
                new VideoDao()
                {
                    UserId = user.Id,
                    BlobStorageId = "523e213d-8e00-4337-a66c-50c9c192c692",
                    UploadDate = DateTime.Now,
                    Tags = tags
                }
                };
                    _context.Videos.AddRange(videos);
            }

            if (!_context.UserTagsInteractions.Any())
            {
                List<UserTagInteractionDao> interactions = new List<UserTagInteractionDao>
                {
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(0), InteractionCount = 8 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(1), InteractionCount = 4 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(2), InteractionCount = 5 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(3), InteractionCount = 20 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(4), InteractionCount = 3 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(5), InteractionCount = 25 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(6), InteractionCount = 10 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(7), InteractionCount = 30 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(8), InteractionCount = 25 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(9), InteractionCount = 3 },
                    new UserTagInteractionDao() { UserId = user.Id, Tag = tags.ElementAt(10), InteractionCount = 3 }
                };
                    _context.UserTagsInteractions.AddRange(interactions);
            }
                _context.SaveChanges();
            }
        }
    }
}
