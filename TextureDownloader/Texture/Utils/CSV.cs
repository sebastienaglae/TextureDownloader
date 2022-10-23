namespace TextureDownloader.Texture.Utils;

public abstract class CSV
{
    public static (List<string> parameters, List<List<string>> arguments) ReadCsv(string pathManifest)
    {
        var fileContent = File.ReadAllLines(pathManifest);
        var parameters = fileContent[0].Split(",").ToList();

        var arguments = fileContent.Skip(1).Select(line => new List<string>(line.Split(","))).ToList();

        return (parameters, arguments);
    }
}