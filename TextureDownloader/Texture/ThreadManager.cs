using System.Net;
using TextureDownloader.Texture.Ambient;
using TextureDownloader.Texture.Enums;
using TextureDownloader.Texture.Utils;

namespace TextureDownloader.Texture;

public static class ThreadManager
{
    private static int maxOperation = 40;
    private static List<TextureRessources>? texturesToCreate;
    private static readonly List<Bloc> Blocs = new();
    private static TextureWebsite textureWebsite;
    private static readonly List<Task> DownloadTasks = new();

    private static void SetMaxOperation(int i)
    {
        maxOperation = i;
        //Console.Out.WriteAsync
        // List<Task<int>> downloadTasks = new List<Task<int>>();
        //Task<int> finishedTask = await Task.WhenAny(downloadTasks);
        // Remplir de 10 task
        // Quand un a terminé ajouter une nouvelle task
    }

    public static async Task CreateAllOperation(List<TextureRessources>? textures,
        string folder, Bloc remain)
    {
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        texturesToCreate = textures;

        for (var i = 0; i < maxOperation + 1; i++) Blocs.Add(new Bloc(20));


        remain.WriteToBloc($"{textures.Count} is remaining !");

        for (var i = 0; i < maxOperation; i++)
        {
            var pickTex = Pick();
            if (pickTex == null)
                break;

            DownloadTasks.Add(Create(pickTex, Blocs[i]));
        }

        while (DownloadTasks.Count != 0)
        {
            var finishedTask = await Task.WhenAny(DownloadTasks);
            DownloadTasks.Remove(finishedTask);
            remain.WriteToBloc($"{textures.Count} is remaining !");

            var pickTex = Pick();
            if (pickTex == null)
                break;

            DownloadTasks.Add(Create(pickTex, Blocs.First(bloc => bloc.IsFree())));
        }
    }

    private static void SaveFinishedTexture(TextureRessources textureRessources)
    {
        var path = Path.Combine(Program.texturePath, "finished.txt");
        if (!File.Exists(path))
            File.Create(path).Close();

        var name =
            $"{textureRessources.Name} {textureRessources.Size} {textureRessources.TextureWebsite} {textureRessources.Attribute}";
        File.AppendAllText(path, $"{name} {Environment.NewLine}");
    }

    private static void SaveFinishedTextureError(TextureRessources textureRessources)
    {
        var path = Path.Combine(Program.texturePath, "error.txt");
        if (!File.Exists(path))
            File.Create(path).Close();

        var name =
            $"{textureRessources.Name} {textureRessources.Size} {textureRessources.TextureWebsite} {textureRessources.Attribute}";
        File.AppendAllText(path, $"{name} {Environment.NewLine}");
    }


    private static async Task Create(TextureRessources textureRessources, Bloc bloc)
    {
        bloc.UnFree();
        try
        {
            WebClient webClient = new();
            webClient.DownloadProgressChanged += (_, e) =>
            {
                var sizeMo = (int) (e.TotalBytesToReceive / 1000000);
                bloc.WriteToBloc(
                    $"Downloading File {Ui.GetProgressBar(e.ProgressPercentage, 20)} {e.ProgressPercentage}% (Size: {sizeMo} Mo)");
            };

            var filename = Program.texturePath +
                           $"{textureRessources.Name}_{textureRessources.Attribute}.{textureRessources.Ext}";
            bloc.WriteToBloc($"Downloading File from {textureWebsite} ({filename}) ...");
            bloc.NextLine();
            var path = await Download.DownloadFile(textureRessources.Url, filename,
                textureWebsite.ToString(), webClient);
            GC.Collect();
            bloc.WriteToBloc($"Downloading File {Ui.GetProgressBar(100, 20)} {100}% (State : Finished, Imported) !");
            bloc.NextLine();
            bloc.WriteToBloc($"Successfully File {textureWebsite} ({filename}) !");
            bloc.NextLine();
            var ambientToUnity = new AmbientToUnity(path, textureRessources);

            ambientToUnity.AmbientUnityEvent += (_, e) =>
            {
                bloc.WriteToBloc($"Converting '{e.Text}' ({e.State}) {Ui.GetProgressBar(e.Progress, 20)}");
            };

            // bloc.WriteToBloc("Converting ambient to Unity");
            // bloc.NextLine();
            // var resultConvert = ambientToUnity.ConvertToUnity();
            // GC.Collect();
            // bloc.NextLine();
            // bloc.WriteToBloc("Successfully converted ambient to Unity");
            // if (!resultConvert.isConverted)
            // {
            //     bloc.WriteToBloc(resultConvert.message);
            //     await Task.Delay(10000);
            // }

            await Task.Delay(1000);
            textureRessources.IsFinished = true;
            SaveFinishedTexture(textureRessources);
        }
        catch (Exception)
        {
            SaveFinishedTextureError(textureRessources);
        }

        bloc.Free();
    }

    private static TextureRessources? Pick()
    {
        if (texturesToCreate.Count == 0)
            return null;
        var tex = texturesToCreate[0];
        texturesToCreate.RemoveAt(0);
        return tex;
    }
}