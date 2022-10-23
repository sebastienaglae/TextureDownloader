using TextureDownloader.Texture.Enums;

namespace TextureDownloader.Texture.Utils;

public static class Find
{
    public static (bool isValid, TextureOption.Extension? extFound) IsValidExtension(string message)
    {
        foreach (var extension in (TextureOption.Extension[]) Enum.GetValues(typeof(TextureOption.Extension)))
            if (message.Contains(extension.ToString(), StringComparison.CurrentCultureIgnoreCase))
                return (true, extension);

        return (false, null);
    }
}