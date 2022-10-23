using TextureDownloader.Texture.Enums;

namespace TextureDownloader.Texture;

public class TextureRessources
{
    public TextureRessources(string name, string url, string attribute, string ext, string size,
        TextureWebsite textureWebsite)
    {
        Name = name;
        Url = url;
        Attribute = attribute;
        Ext = ext;
        Size = size;
        TextureWebsite = textureWebsite;
        IsFinished = false;
    }

    public string Url { get; }

    public string Name { get; }

    public string Ext { get; }

    public string Attribute { get; }

    public string Size { get; }

    public TextureWebsite TextureWebsite { get; }

    public bool IsFinished { get; set; }
}