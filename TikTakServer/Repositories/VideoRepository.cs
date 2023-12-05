using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Models.DaoModels;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly TikTakContext _context;
        private readonly UserRequestAndClaims _requestAndClaims;

        public VideoRepository(TikTakContext context, UserRequestAndClaims requestAndClaims)
        {
            _context = context;
            _requestAndClaims = requestAndClaims;
        }

        /// <summary>
        /// Gets x amount of random videos.
        /// </summary>
        /// <param name="videoAmount">Amount of random videos desired</param>
        /// <returns>List of random videos.</returns>
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

        /// <summary>
        /// Saves provided video to the database.
        /// </summary>
        /// <param name="video">Video to be saved</param>
        public async Task SaveVideo(VideoModel video)
        {
            var user = await _context.Users.Where(e => e.Email == _requestAndClaims.Email).FirstAsync();
            var tagNames = video.Tags.Select(x => x.Name).ToList();
            var existingTags = await _context.Tags.Where(e => tagNames.Contains(e.Name)).ToListAsync();
            var newTags = FindNonDuplicates(existingTags.Select(x => x.Name).ToList(), tagNames);
            var videoDao = new VideoDao(video)
            {
                User = user,
                Tags = new List<TagDao>()
            };
            videoDao.Tags = existingTags;
            newTags.ForEach(x => videoDao.Tags.Add(new TagDao()
            {
                Name = x
            }));
            _context.Videos.Add(videoDao);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a video from the database with the provided ID as BlobStorageId
        /// </summary>
        /// <param name="id">BlobStorageId to remove video on</param>
        public async Task RemoveVideoByStorageId(string id)
        {
            var dao = await _context.Videos.Where(e => e.BlobStorageId == id).FirstAsync();
            _context.Videos.Remove(dao);
            await _context.SaveChangesAsync(true);
        }

        /// <summary>
        /// Finds tags on the video of blobStorageId provided, and adds or increments the interaction count for that tag
        /// on the user initiating the request.
        /// </summary>
        /// <param name="blobStorageId"></param>
        public async Task IncrementUserVideoInteraction(string blobStorageId)
        {
            var videoId = await _context.Videos.Where(x => x.BlobStorageId == blobStorageId).Select(y => y.Id).FirstAsync();

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

        /// <summary>
        /// Saves a like in the database on the video corrosponding to the blobStorageId provided in the Like object parameter.
        /// </summary>
        /// <param name="like">Like object containing information about what video to save the like on, and date.</param>
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

        /// <summary>
        /// Gets a random video from a provided tag if exists, else returns a random video.
        /// </summary>
        /// <param name="tagName">Tag name to find random video on</param>
        /// <returns>VideoModel of the found video.</returns>
        public async Task<VideoModel> GetRandomVideoBlobId(string tagName)
        {
            Random rnd = new Random();
            var tagCount = await GetTagCount(tagName);
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
                .Where(x => x.Tags.Any(e => e.Name == tagName))
                .ElementAt(rd);
            return new VideoModel(randomTagVideo);
        }

        /// <summary>
        /// Loops through provided Tags to find userInteractions on this tag, and increments these with 1.
        /// </summary>
        /// <param name="interactions">List of tags to increment interactions on</param>
        private async Task IncrementTagInteraction(List<TagDao> interactions)
        {
            foreach (var tag in interactions)
            {
                var userTagInteraction = await _context.UserTagsInteractions.Where(x => x.Tag == tag).FirstAsync();
                userTagInteraction.InteractionCount++;
                await _context.SaveChangesAsync();
            }

        }

        /// <summary>
        /// Adds new TagInteractions in the database based on provided tags.
        /// </summary>
        /// <param name="tags">Tags to add interactions on.</param>
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

        /// <summary>
        /// Gets count of tags in DB and returns this.
        /// </summary>
        private async Task<int> GetTagCount(string name)
        {
            return await _context.Tags
                .Where(x => x.Name == name)
                .SelectMany(e => e.Videos)
                .Distinct()
                .CountAsync();
        }

        /// <summary>
        /// Returns the non-duplicate elements between two provided lists.
        /// </summary>
        private List<string> FindNonDuplicates(List<string> tagListOne, List<string> tagListTwo)
        {
            var nonDuplicateTags = tagListOne.Concat(tagListTwo)
                            .GroupBy(tag => tag)
                            .Where(group => group.Count() == 1)
                            .Select(group => group.Key).ToList();
            return nonDuplicateTags;
        }
    }
}
