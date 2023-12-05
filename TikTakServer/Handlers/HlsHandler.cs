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

        /// <summary>
        /// Converts provided MP4 stream into segments and manifest file with provided guid as name.
        /// </summary>
        /// <param name="stream">MP4 stream to convert</param>
        /// <param name="guid">name for files</param>
        /// <returns>Object containing information about local path, filecount and name of converted files.</returns>
        public async Task<HlsModel> ConvertToHls(Stream stream, string guid)
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
                CustomOutputArgs = "-profile:v baseline -level 3.0 -s 720x1280 -b:v 2000k -start_number 0 -hls_time 4 -hls_list_size 0 -f hls"
            };

            _converter.ConvertMedia(tempFilePath, "mp4", tempOutputPath, "hls", hlsSettings);
            var hlsResult = await ReplaceChunkLinks(guid);
            return hlsResult;
        }

        /// <summary>
        /// Loops through a local manifest file of the provided guid and replaces segment links in the file with azure path.
        /// </summary>
        /// <param name="guid">Guid to process manifest file on.</param>
        /// <returns>HlsModel</returns>
        private async Task<HlsModel> ReplaceChunkLinks(string guid)
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
            return new HlsModel() { FileCount = fileCount, Guid = guid, Path = tempFolderPath };
        }

        /// <summary>
        /// Clears local temp files with provided GUID at provided path
        /// </summary>
        /// <param name="guid">To remove files with this guild in the name</param>
        /// <param name="path">Path to remove files from</param>
        public void ClearTempFiles(string guid, string path)
        {
            string[] files = Directory.GetFiles(path).Where(e => e.Contains(guid)).ToArray();
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }

    public class HlsModel
    {
        public int FileCount { get; set; }
        public string Guid { get; set; }
        public string Path { get; set; }
    }
}
