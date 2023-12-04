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
            var video = await _context.Videos.Where(e => e.BlobStorageId == id).FirstAsync();
            return video;
        }

        public async Task<List<VideoModel>> GetRandomVideos(int videoAmount)
        {
            var videoCount = _context.Videos.Count() - 1;
            List<VideoModel> videos = new List<VideoModel>();
            for (int i = 0; i < videoAmount; i++)
            {
                var rnd = new Random().Next(0, videoCount);
                var video = await _context.Videos.Include(e => e.Likes).Include(e => e.Tags).ElementAtAsync(rnd);
                videos.Add(new VideoModel(video));
            }

            return videos;
        }

        public async Task SaveVideo(VideoModel video)
        {
            var user = await _context.Users.Where(e => e.Email == _requestAndClaims.Email).FirstAsync();
            var tags = await _context.Tags.Where(e => video.Tags.Any(x => e.Name == x.Name)).ToListAsync();
            var videoDao = new VideoDao(video)
            {
                User = user,
            };
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

        public async Task RemoveVideoByStorageId(string id)
        {
            var dao = await _context.Videos.Where(e => e.BlobStorageId == id).FirstAsync();
            _context.Videos.Remove(dao);
            await _context.SaveChangesAsync(true);
        }

        public async Task CountUserVideoInteraction(UserTagInteraction interaction)
        {
            var videoId = await _context.Videos.Where(x => x.BlobStorageId == interaction.BlobStorageId).Select(y => y.Id).FirstAsync();

            List<TagDao> videoTags = await _context.Videos
                .Where(v => v.Id == videoId)
                .SelectMany(x => x.Tags)
                .ToListAsync();
            var usedTagsByUser = await _context.Users
                .Where(u => u.Email == _requestAndClaims.Email)
                .SelectMany(x => x.UserTagInteractions)
                .ToListAsync();
            var unusedTags = videoTags
                .Where(x => !usedTagsByUser
                .Any(y => y.TagId == x.Id))
                .ToList();
            var usedTags = videoTags
                .Where(x => usedTagsByUser
                .Any(y => y.TagId == x.Id))
                .ToList();

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
            var video = await _context.Videos
                .Where(x => x.BlobStorageId == like.BlobStorageId)
                .Include(x => x.User)
                .FirstAsync();

            LikeDao likeDao = new LikeDao(like, video.Id, video.User.Id);

            await _context.Likes.AddAsync(likeDao);
            await _context.SaveChangesAsync();
        }

        public async Task<VideoModel> GetRandomVideoBlobId(string name)
        {
            Random rnd = new Random();
            var tagCount = await _tagRepository.GetTagCount(name);
            if (tagCount == 0)
            {
                var videoCount = _context.Videos.Count() - 1;
                var randomVideo = _context.Videos
                    .Include(e => e.Tags)
                    .ElementAt(rnd.Next(0, videoCount));
                return new VideoModel(randomVideo);
            }

            var rd = rnd.Next(0, tagCount);
            var randomTagVideo = _context.Videos
                .Include(video => video.Tags)
                .Include(e => e.Likes)
                .Where(x => x.Tags.Any(e => e.Name == name))
                .ElementAt(rd);
            return new VideoModel(randomTagVideo);
        }

        private async Task IncrementTagInteraction(List<TagDao> interactions)
        {
            foreach (var tag in interactions)
            {
                var userTagInteraction = await _context.UserTagsInteractions.Where(x => x.Tag == tag).FirstAsync();
                userTagInteraction.InteractionCount++;
                await _context.SaveChangesAsync();
            }

        }

        private async Task AddNewTagInteractions(List<TagDao> tags)
        {
            var user = await _context.Users.FirstAsync(e => e.Email == _requestAndClaims.Email);
            foreach (var tag in tags)
            {
                UserTagInteractionDao dao = new UserTagInteractionDao(user.Id.ToString(), tag.Id);
                await _context.UserTagsInteractions.AddAsync(dao);
                await _context.SaveChangesAsync();
            }
        }
    }
}
