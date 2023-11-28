using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public interface ITagRepository
    {
        Task<int> GetTagCount(string name);
        Task<string> GetRandomTag();
    }
}
