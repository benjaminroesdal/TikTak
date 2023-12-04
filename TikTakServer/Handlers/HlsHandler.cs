using NReco.VideoConverter;

namespace TikTakServer.Handlers
{
    public class HlsHandler : IHlsHandler
    {
        private readonly FFMpegConverter _converter;
        private const string AzureBlobPath = "https://tiktakstorage.blob.core.windows.net/tiktaks/";

        public HlsHandler()
        {
            _converter = new FFMpegConverter();
        }

        public async Task<HlsObj> ConvertToHls(Stream stream, string guid)
        {
            var tempFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "TempFiles");
            var tempFilePath = Path.Combine(tempFolderPath, Guid.NewGuid().ToString());
            var hlsPlaylistName = $"{guid}.m3u8";
            var tempOutputPath = Path.Combine(tempFolderPath, hlsPlaylistName);

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }

            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            var hlsSettings = new ConvertSettings
            {
                CustomOutputArgs = "-profile:v baseline -level 3.0 -s 720x1280 -start_number 0 -hls_time 10 -hls_list_size 0 -f hls"
            };

            _converter.ConvertMedia(tempFilePath, "mp4", tempOutputPath, "hls", hlsSettings);
            var hlsResult = await ReplaceChunkLinks(guid);
            return hlsResult;
        }


        private async Task<HlsObj> ReplaceChunkLinks(string guid)
        {
            var tempFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "TempFiles");
            int fileCount = 0;
            string[] lines = await File.ReadAllLinesAsync(tempFolderPath + $"\\{guid}.M3U8");
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].EndsWith(".ts"))
                {
                    lines[i] = AzureBlobPath + lines[i];
                    fileCount++;
                }
            }

            await File.WriteAllLinesAsync(tempFolderPath + $"\\{guid}.M3U8", lines);
            return new HlsObj() { FileCount = fileCount, Guid = guid, Path = tempFolderPath };
        }

        public void ClearTempFiles(string guid, string path)
        {
            string[] files = Directory.GetFiles(path).Where(e => e.Contains(guid)).ToArray();
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }

    public class HlsObj
    {
        public int FileCount { get; set; }
        public string Guid { get; set; }
        public string Path { get; set; }
    }
}
