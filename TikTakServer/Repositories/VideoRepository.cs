using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Managers;
using TikTakServer.Models;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly TikTakContext _context;
        private readonly IRecommendationManager _recommendationManager;

        public VideoRepository(TikTakContext context)
        {
            _context = context;
        }

        public async Task<VideoDao> GetVideo(string id)
        {
            var video = _context.Videos.Where(e => e.BlobStorageId == id);
            return video.FirstOrDefault();
        }

        public async Task<List<string>> GetFyp(int userId)
        {
            var videoIds = _context.Videos.Select(e => e.BlobStorageId).ToList();
            var userPreferencedTags = _recommendationManager.GetRandomTagsBasedOnUserPreference(userId);
            var rndmVideoIds = new List<string>();
            foreach (var tag in userPreferencedTags)
            {
                rndmVideoIds.Add(_context.Videos.Where(x => x.Tags == userPreferencedTags).Select(x => x.BlobStorageId).ToString());
            }
            
            return videoIds;
        }

        public Task CreateVideo(VideoDao video)
        {
            var result = _context.Add(video);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task RemoveVideoByStorageId(string id)
        {
            var dao = _context.Videos.Where(e => e.BlobStorageId == id).FirstOrDefault();
            _context.Remove(dao);
            _context.SaveChanges(true);
            return Task.CompletedTask;
        }

        public Task CountUserVideoInteraction(UserTagInteraction interaction)
        {
            List<TagDao> videoTags = _context.Videos.Where(v => v.Id == interaction.VideoId).SelectMany(x => x.Tags).ToList();
            var usedTagsByUser = _context.Users.Where(u => u.Id == interaction.UserId).SelectMany(x => x.UserTagInteractions).ToList();
            var unusedTags = videoTags.Where(x => !usedTagsByUser.Any(y => y.TagId == x.Id)).ToList();
            var usedTags = videoTags.Where(x => usedTagsByUser.Any(y => y.TagId == x.Id)).ToList();

            IncrementTagInteraction(usedTags);
            AddNewTagInteractions(interaction, unusedTags);

            return Task.CompletedTask;
        }

        public async Task RegisterVideoLike(Like like)
        {
            LikeDao likeDao = new LikeDao(like);

            await _context.Likes.AddAsync(likeDao);
            await _context.SaveChangesAsync();
        }

        private async Task IncrementTagInteraction(List<TagDao> interactions)
        {
            foreach (var tag in interactions)
            {
                var userTagInteraction = await _context.UserTagsInteractions.Where(x => x.Tag == tag).FirstOrDefaultAsync();
                userTagInteraction.InteractionCount++;
                await _context.SaveChangesAsync();
            }

        }

        private async Task AddNewTagInteractions(UserTagInteraction newInteractions, List<TagDao> tags)
        {
            foreach (var tag in tags)
            {
                UserTagInteractionDao dao = new UserTagInteractionDao(newInteractions);
                await _context.UserTagsInteractions.AddAsync(dao);
                await _context.SaveChangesAsync();
            }
        }
    }
}
