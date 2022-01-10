using static TextureDownloader.Texture.Enums.TextureOption;
using static TextureDownloader.Texture.Ambient.AmbientParameter;
using TextureDownloader.Texture.Enums;

namespace TextureDownloader.Texture
{
    public abstract class TextureRessourcesFactory
    {
        public static List<TextureRessources> CreateTextures(TextureWebsite textureWebsite, List<List<string>> arguments, Resolution[] resolutions, Extension[] extensions, Quality[] qualities)
        {
            switch (textureWebsite)
            {
                case TextureWebsite.AMBIENT_CG:
                    return CreateAmbientTextures(arguments, resolutions, extensions, qualities);
            }

            return new List<TextureRessources>();
        }

        private static List<TextureRessources> CreateAmbientTextures(List<List<string>> arguments, Resolution[] resolutions, Extension[] extensions, Quality[] qualities)
        {
            List<TextureRessources> textureRessources = new();
            foreach (var item in arguments)
            {
                bool rightOption = true;
                foreach (var resolution in resolutions)
                    if (!item[(int)DOWNLOAD_ATTRIBUTE].Contains(GetResolutionString(resolution)) && resolution != Resolution.ALL)
                        rightOption = false;

                if (!rightOption)
                    continue;

                foreach (var extension in extensions)
                    if (!item[(int)DOWNLOAD_ATTRIBUTE].Contains(extension.ToString()) && extension != Extension.ALL)
                        rightOption = false;

                if (!rightOption)
                    continue;

                foreach (var quality in qualities)
                    if (!item[(int)DOWNLOAD_ATTRIBUTE].Contains(quality.ToString()) && quality != Quality.ALL)
                        rightOption = false;

                if (rightOption)
                    textureRessources.Add(new TextureRessources(
                        item[(int)ASSET_ID],
                        item[(int)RAW_LINK],
                        item[(int)DOWNLOAD_ATTRIBUTE],
                        item[(int)FILE_TYPE],
                        item[(int)SIZE],
                        TextureWebsite.AMBIENT_CG
                        ));
            }

            return textureRessources;
        }

    }
}
