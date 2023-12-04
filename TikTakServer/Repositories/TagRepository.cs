using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;

namespace TikTakServer.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly TikTakContext _context;
        public TagRepository(TikTakContext context)
        {
            _context = context;
        }
        public async Task<string> GetRandomTag()
        {
            var rnd = new Random();
            var count = await _context.Tags.CountAsync();
            var rndTag = rnd.Next(count);
            return await _context.Tags.
                Where(x => x.Id == rndTag)
                .Select(y => y.Name)
                .FirstAsync();
        }

        public async Task<int> GetTagCount(string name)
        {
            return await _context.Tags
                .Where(x => x.Name == name)
                .SelectMany(e => e.Videos)
                .Distinct()
                .CountAsync();
        }
    }
}
