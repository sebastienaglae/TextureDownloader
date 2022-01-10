using TextureDownloader.Texture.Enums;

namespace TextureDownloader.Texture.Utils
{
    public class Find
    {
        public static (bool isValid, TextureOption.Extension? extFound) isValidExtension(string message)
        {
            foreach (TextureOption.Extension extension in (TextureOption.Extension[])Enum.GetValues(typeof(TextureOption.Extension)))
                if (message.Contains(extension.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    return (true, extension);

            return (false, null);
        }
    }
}
