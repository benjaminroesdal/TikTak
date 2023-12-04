namespace TikTakServer.Handlers
{
    public interface IHlsHandler
    {
        Task<HlsObj> ConvertToHls(Stream stream, string guid);
        void ClearTempFiles(string guid, string path);
    }
}
