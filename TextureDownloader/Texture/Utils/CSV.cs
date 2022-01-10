namespace TextureDownloader.Texture.Utils
{
    public abstract class CSV
    {
        public static (List<string> parameters, List<List<string>> arguments) ReadCsv(string pathManifest)
        {
            List<string> parameters = new();
            List<List<string>> arguments = new();
            string[] fileContent = File.ReadAllLines(pathManifest);
            foreach (string parameter in fileContent[0].Split(","))
                parameters.Add(parameter);

            foreach (var line in fileContent.Skip(1))
                arguments.Add(new List<string>(line.Split(",")));

            return (parameters, arguments);
        }
    }
}
