using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Models.DaoModels;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly TikTakContext _context;
        private readonly ITagRepository _tagRepository;
        private readonly UserRequestAndClaims _requestAndClaims;

        public VideoRepository(TikTakContext context, ITagRepository tagRepository, UserRequestAndClaims requestAndClaims)
        {
            _context = context;
            _tagRepository = tagRepository;
            _requestAndClaims = requestAndClaims;
        }

        public async Task<VideoDao> GetVideo(string id)
        {
            var video = await _context.Videos.Where(e => e.BlobStorageId == id).FirstOrDefaultAsync();
            return video;
        }

        public async Task<List<string>> GetFyp(List<string> videoIds)
        {
            var fypIds = new List<string>();
            foreach (var id in videoIds)
            {
                var fypId = await _context.Videos.Where(x => x.BlobStorageId == id).Select(x => x.BlobStorageId).FirstOrDefaultAsync();
                fypIds.Add(fypId);
            }

            return fypIds;
        }

        public async Task CreateVideo(VideoDao video)
        {
            video.Tags = video.Tags;
            _context.Add(video);
            await _context.SaveChangesAsync();
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
                    updatedDaoList.Add(_context.Tags.Add(new TagDao() { Name = item.Name }).Entity);
                }
            }
            await _context.SaveChangesAsync();
            return updatedDaoList;
        }

        public Task RemoveVideoByStorageId(string id)
        {
            var dao = _context.Videos.Where(e => e.BlobStorageId == id).FirstOrDefault();
            _context.Videos.Remove(dao);
            _context.SaveChanges(true);
            return Task.CompletedTask;
        }

        public async Task CountUserVideoInteraction(UserTagInteraction interaction)
        {
            var videoId = _context.Videos.Where(x => x.BlobStorageId == interaction.BlobStorageId).Select(y => y.Id).FirstOrDefault();

            List<TagDao> videoTags = _context.Videos.Where(v => v.Id == videoId).SelectMany(x => x.Tags).ToList();
            var usedTagsByUser = _context.Users.Where(u => u.Id == int.Parse(_requestAndClaims.UserId)).SelectMany(x => x.UserTagInteractions).ToList();
            var unusedTags = videoTags.Where(x => !usedTagsByUser.Any(y => y.TagId == x.Id)).ToList();
            var usedTags = videoTags.Where(x => usedTagsByUser.Any(y => y.TagId == x.Id)).ToList();

            if (unusedTags.Count > 0)
            {
                await AddNewTagInteractions(unusedTags);
            }

            if (usedTags.Count > 0)
            {
                await IncrementTagInteraction(usedTags);
            }
        }

        public async Task RegisterVideoLike(Like like)
        {
            var videoId = _context.Videos.Where(x => x.BlobStorageId == like.BlobStorageId).Select(i => i.Id).FirstOrDefault();

            LikeDao likeDao = new LikeDao(like, videoId);

            await _context.Likes.AddAsync(likeDao);
            await _context.SaveChangesAsync();
        }

        public string GetRandomVideoBlobId(string name)
        {
            Random rnd = new Random();
            var tagCount = _tagRepository.GetTagCount(name);
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

        private async Task AddNewTagInteractions(List<TagDao> tags)
        {
            foreach (var tag in tags)
            {
                UserTagInteractionDao dao = new UserTagInteractionDao(_requestAndClaims.UserId, tag.Id);
                await _context.UserTagsInteractions.AddAsync(dao);
                await _context.SaveChangesAsync();
            }
        }
    }
}
