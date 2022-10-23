using System.Net;
using TextureDownloader.Texture;
using TextureDownloader.Texture.Enums;
using TextureDownloader.Texture.Utils;
using static TextureDownloader.Texture.Enums.TextureOption;

namespace TextureDownloader;

public class Program
{
    private static string[]? finishedTextures;

    public static void Main()
    {
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        TexAsync().GetAwaiter().GetResult();
    }

    private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        Bloc.SetCursorToEnd();
    }

    private static async Task TexAsync()
    {
        Console.CursorVisible = false;
        LoadFinishedFile();
        const TextureWebsite textureWeb = TextureWebsite.AMBIENT_CG;
        var pathManifest = await Download.DownloadManifestFile(textureWeb, new WebClient());
        var (_, arguments) = CSV.ReadCsv(pathManifest);
        Resolution[] resolutions = {Resolution.ONE_K};
        Extension[] extensions = {Extension.ALL};
        Quality[] qualities = {Quality.ALL};
        var textures =
            TextureRessourcesFactory.CreateTextures(textureWeb, arguments, resolutions, extensions, qualities);
        textures.ForEach(x => x.IsFinished = AlreadyFinished(x));
        ShowInfo(textures);
        Bloc.SameLine(true);
        Console.ReadKey();
        var remain = new Bloc(1);
        await ThreadManager.CreateAllOperation(textures.FindAll(x => !x.IsFinished), @"D:\Textures\",
            remain);
        await Task.Delay(-1);
    }

    private static void LoadFinishedFile()
    {
        if (!File.Exists("finished.txt"))
        {
            finishedTextures = Array.Empty<string>();
            return;
        }

        finishedTextures = File.ReadAllLines("finished.txt");
    }

    private static bool AlreadyFinished(TextureRessources textureRessources)
    {
        var name =
            $"{textureRessources.Name} {textureRessources.Size} {textureRessources.TextureWebsite} {textureRessources.Attribute}";
        return finishedTextures != null && finishedTextures.Any(finishedTexture => finishedTexture.Contains(name));
    }

    private static void ShowInfo(List<TextureRessources> tex)
    {
        var space = Disk.CurrentDiskSpace();
        var size = Download.GetTextureSize(tex);

        Debug.Jump();
        Debug.Log($"Available space {space.free} {space.unit}, Total space {space.total} {space.unit}");
        Debug.Log($"The total space needed is {size} Mo");
        Debug.Log($"The total space needed is {size / 1024f} Go");
        Debug.Log($"The total space needed is {size / 1048576f} To");
        Debug.Log($"The number of file {tex.Count}");
        Debug.Jump();
    }
}