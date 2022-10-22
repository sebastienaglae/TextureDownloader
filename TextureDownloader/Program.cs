using System.Net;
using TextureDownloader.Texture;
using TextureDownloader.Texture.Enums;
using TextureDownloader.Texture.Utils;
using static TextureDownloader.Texture.Enums.TextureOption;

namespace TextureDownloader
{
    public class Program
    {
        private static string[] finishedTextures;
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
            Console.CursorVisible = false;
            LoadFinishedFile();
            TextureWebsite textureWeb = TextureWebsite.AMBIENT_CG;
            string pathManifest = await Download.DownloadManifestFile(textureWeb, new WebClient());
            var (parameters, arguments) = CSV.ReadCsv(pathManifest);
            Resolution[] resolutions = { Resolution.ONE_K };
            Extension[] extensions = { Extension.ALL };
            Quality[] qualities = { Quality.ALL };
            var textures = TextureRessourcesFactory.CreateTextures(textureWeb, arguments, resolutions, extensions, qualities);
            textures.ForEach(x => x.IsFinished = AlreadyFinished(x));
            ShowInfo(textures);
            Bloc.SameLine(true);
            Console.ReadKey();
            Bloc remain = new Bloc(1);
            await ThreadManager.CreateAllOperation(textures.FindAll(x => !x.IsFinished), textureWeb, @"D:\Textures\", remain);
            await Task.Delay(-1);
        }

        private static void LoadFinishedFile()
        {
            if (!File.Exists("finished.txt"))
            {
                finishedTextures = new string[0];
                return;
            }
            finishedTextures = File.ReadAllLines("finished.txt");
        }

        private static bool AlreadyFinished(TextureRessources textureRessources)
        {
            var name = $"{textureRessources.Name} {textureRessources.Size} {textureRessources.TextureWebsite} {textureRessources.Attribute}";
            foreach (string finishedTexture in finishedTextures)
            {
                if (finishedTexture.Contains(name))
                    return true;
            }

            return false;
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