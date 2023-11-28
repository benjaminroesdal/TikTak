using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Models.DaoModels;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly TikTakContext _context;

        public VideoRepository(TikTakContext context)
        {
            _context = context;
        }

        public async Task<VideoDao> GetVideo(string id)
        {
            var video = _context.Videos.Where(e => e.BlobStorageId == id);
            return video.FirstOrDefault();
        }

        public async Task<List<string>> GetFyp(List<string> videoIds)
        {
            var fypIds = new List<string>();
            foreach (var id in videoIds)
            {
                fypIds.Add(_context.Videos.Where(x => x.BlobStorageId == id).Select(x => x.BlobStorageId).FirstOrDefault());
            }

            return fypIds;
        }

        public async Task<Task> CreateVideo(VideoDao video)
        {
            video.Tags = video.Tags;
            var result = _context.Add(video);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task<ICollection<TagDao>> AddTag(ICollection<TagModel> tag)
        {
            var updatedDaoList = new List<TagDao>();
            foreach (var item in tag)
            {
                var result = _context.Tags.Any(e => e.Name == item.Name);
                TagDao tagDao;
                if (result)
                {
                    updatedDaoList.Add(_context.Tags.First(e => e.Name == item.Name));
                }
                if (!result)
                {
                    updatedDaoList.Add(_context.Tags.Add(new TagDao() { Name = item.Name}).Entity);
                }
            }
            _context.SaveChanges();
            return updatedDaoList;
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

        public async Task<int> GetTagCount(string name)
        {
            return _context.Tags.Where(x => x.Name == name).SelectMany(e => e.Videos).Distinct().Count();
        }

        public async Task<string> GetRandomVideoBlobId(string name)
        {
            Random rnd = new Random();
            var tagCount = await GetTagCount(name);
            if (tagCount == 0)
            {
                var videoCount = _context.Videos.Count();
                return _context.Videos.Select(x => x.BlobStorageId).ElementAt(rnd.Next(0, videoCount));
            }

            var rd = rnd.Next(0, tagCount);
            var blobId = _context.Videos.Include(video => video.Tags).Where(x => x.Tags.Any(e => e.Name == name)).Select(y => y.BlobStorageId).ElementAt(rd);
            return blobId;
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
