namespace TikTakServer.Handlers
{
    public interface IHlsHandler
    {
        Task<HlsObj> ConvertToHls(Stream stream, string guid);
        Task ClearTempFiles(string guid, string path);
    }
}
