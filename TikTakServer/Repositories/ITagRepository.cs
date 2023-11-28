using TikTakServer.Models;

namespace TikTakServer.Repositories
{
    public interface ITagRepository
    {
        int GetTagCount(string name);
        Task<string> GetRandomTag();
    }
}
