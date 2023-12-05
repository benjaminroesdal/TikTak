namespace TikTakServer.Handlers
{
    public interface IHlsHandler
    {
        Task<HlsModel> ConvertToHls(Stream stream, string guid);
        void ClearTempFiles(string guid, string path);
    }
}
