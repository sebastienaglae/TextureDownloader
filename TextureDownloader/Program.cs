using System.Net;
using TextureDownloader.Texture;
using TextureDownloader.Texture.Enums;
using TextureDownloader.Texture.Utils;
using static TextureDownloader.Texture.Enums.TextureOption;

namespace TextureDownloader
{
    public class Program
    {
        public static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            TexAsync().GetAwaiter().GetResult();
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Bloc.SetCursorToEnd();
        }

        private static async Task MainAsync()
        {

        }
        private static async Task TexAsync()
        {
            TextureWebsite textureWeb = TextureWebsite.AMBIENT_CG;
            string pathManifest = await Download.DownloadManifestFile(textureWeb, new WebClient());
            var (parameters, arguments) = CSV.ReadCsv(pathManifest);
            Resolution[] resolutions = { Resolution.ONE_K };
            Extension[] extensions = { Extension.JPG };
            Quality[] qualities = { Quality.ALL };
            var textures = TextureRessourcesFactory.CreateTextures(textureWeb, arguments, resolutions, extensions, qualities);
            ShowInfo(textures);
            //await Download.DownloadTextureFileAndConvert(textures, textureWeb, @".\asset\");
            await ThreadManager.CreateAllOperation(textures, textureWeb, @".\asset\");
            await Task.Delay(-1);
        }

        private static void ShowInfo(List<TextureRessources> tex)
        {
            var space = Disk.CurrentDiskSpace();
            int size = Download.GetTextureSize(tex);

            Debug.Jump();
            Debug.Log($"Available space {space.free} {space.unit}, Total space {space.total} {space.unit}");
            Debug.Log($"The total space needed is {size} Mo");
            Debug.Log($"The total space needed is {size / 1024f} Go");
            Debug.Log($"The total space needed is {size / 1048576f} To");
            Debug.Log($"The number of file {tex.Count}");
            Debug.Jump();
        }
    }
}