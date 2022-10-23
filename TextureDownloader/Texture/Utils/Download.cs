using System.Net;
using TextureDownloader.Texture.Enums;

namespace TextureDownloader.Texture.Utils;

public abstract class Download
{
    public static async Task<string> DownloadFile(string? url, string fileName, string websiteName, WebClient webClient)
    {
        if (url == null)
            throw new NullReferenceException($"The url is null {url}");

        await Task.Delay(1);
        await webClient.DownloadFileTaskAsync(url, fileName);
        await Task.Delay(1);

        return Path.GetFullPath(fileName);
    }

    public static int GetTextureSize(List<TextureRessources> textures)
    {
        double size = 0;
        foreach (var texture in textures)
            if (int.TryParse(texture.Size, out var r))
                size += r;

        return (int) (size / 1048576);
    }

    public static async Task<string> DownloadManifestFile(TextureWebsite textureWebsite, WebClient webClient)
    {
        return await DownloadFile(GetDownloadUrl(textureWebsite), $"cache_manifest_{textureWebsite}",
            textureWebsite.ToString(), webClient);
    }

    private static string? GetDownloadUrl(TextureWebsite textureWebsite)
    {
        return textureWebsite switch
        {
            TextureWebsite.AMBIENT_CG => "https://ambientcg.com/api/v2/downloads_csv",
            _ => null
        };
    }
}