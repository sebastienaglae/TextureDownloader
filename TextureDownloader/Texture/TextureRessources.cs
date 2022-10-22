using TextureDownloader.Texture.Enums;

namespace TextureDownloader.Texture
{
    public class TextureRessources
    {
        private readonly string name;
        private readonly string url;
        private readonly string attribute;
        private readonly string ext;
        private readonly string size;
        private readonly TextureWebsite textureWebsite;
        private bool isFinished;

        public TextureRessources(string name, string url, string attribute, string ext, string size, TextureWebsite textureWebsite)
        {
            this.name = name;
            this.url = url;
            this.attribute = attribute;
            this.ext = ext;
            this.size = size;
            this.textureWebsite = textureWebsite;
            this.isFinished = false;
        }

        public string Url { get => url; }
        public string Name { get => name; }
        public string Ext { get => ext; }
        public string Attribute { get => attribute; }
        public string Size { get => size; }
        public TextureWebsite TextureWebsite { get => textureWebsite; }
        public bool IsFinished { get => isFinished; set => isFinished = value; }
    }
}
