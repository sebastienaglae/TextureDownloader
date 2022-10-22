using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TextureDownloader.Texture.Ambient;
using TextureDownloader.Texture.Enums;
using TextureDownloader.Texture.Utils;

namespace TextureDownloader.Texture
{
    public static class ThreadManager
    {
        private static int maxOperation = 20;
        private static List<Thread> threads = new List<Thread>();
        private static List<TextureRessources> texturesToCreate;
        private static List<Bloc> blocs = new List<Bloc>();
        private static TextureWebsite textureWebsite;
        private static string folderDownload;
        private static List<Task> downloadTasks = new List<Task>();


        static async void SetMaxOperation(int i)
        {
            maxOperation = i;
            //Console.Out.WriteAsync
            // List<Task<int>> downloadTasks = new List<Task<int>>();
            //Task<int> finishedTask = await Task.WhenAny(downloadTasks);
            // Remplir de 10 task
            // Quand un a terminé ajouter une nouvelle task
        }

        public static async Task CreateAllOperation(List<TextureRessources> textures, TextureWebsite textureWebsite, string folder, Bloc remain)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            texturesToCreate = textures;
            folderDownload = folder;

            for (int i = 0; i < maxOperation + 1; i++)
            {
                blocs.Add(new Bloc(6));
            }


            remain.WriteToBloc($"{textures.Count} is remaining !");

            for (int i = 0; i < maxOperation; i++)
            {
                var pickTex = Pick();
                if (pickTex == null)
                    break;

                downloadTasks.Add(Create(pickTex, blocs[i]));
            }
            while (downloadTasks.Count != 0)
            {
                Task finishedTask = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(finishedTask);
                remain.WriteToBloc($"{textures.Count} is remaining !");

                var pickTex = Pick();
                if (pickTex == null)
                    break;

                downloadTasks.Add(Create(pickTex, blocs.Where((bloc) => bloc.IsFree()).First()));
                pickTex.IsFinished = true;
                SaveFinishedTexture(pickTex);
                Console.Beep();
            }
        }

        private static void SaveFinishedTexture(TextureRessources textureRessources)
        {
            //save in unique file
            string path = Path.Combine(folderDownload, "finished.txt");
            if (!File.Exists(path))
                File.Create(path).Close();

            var name = $"{textureRessources.Name} {textureRessources.Size} {textureRessources.TextureWebsite} {textureRessources.Attribute}";
            File.AppendAllText(path, $"{name} {Environment.NewLine}");
        }



        private static async Task Create(TextureRessources textureRessources, Bloc bloc)
        {
            bloc.UnFree();
            //Debug.Log($"#Texture N°{item.i}/{textureCount}");
            WebClient webClient = new();
            webClient.DownloadProgressChanged += (sender, e) =>
            {
                int sizeMo = (int)(e.TotalBytesToReceive / 1000000);
                bloc.WriteToBloc($"Downloading File {UI.GetProgressBar(e.ProgressPercentage, 20)} {e.ProgressPercentage}% (Size: {sizeMo} Mo)");
            };

            string filename = folderDownload + $"{textureRessources.Name}_{textureRessources.Attribute}.{textureRessources.Ext}";
            bloc.WriteToBloc($"Downloading File from {textureWebsite} ({filename}) ...");
            bloc.NextLine();
            string path = await Download.DownloadFile(textureRessources.Url, filename,
                textureWebsite.ToString(), webClient);
            GC.Collect();
            bloc.WriteToBloc($"Downloading File {UI.GetProgressBar(100, 20)} {100}% (State : Finished, Imported) !");
            bloc.NextLine();
            bloc.WriteToBloc($"Successfully File {textureWebsite} ({filename}) !");
            bloc.NextLine();
            AmbientToUnity ambientToUnity = new AmbientToUnity(path, textureRessources);

            ambientToUnity.AmbientUnityEvent += (s, e) =>
            {
                bloc.WriteToBloc($"Converting '{e.Text}' ({e.State}) {UI.GetProgressBar(e.Progress, 20)}");
            };

            bloc.WriteToBloc($"Converting ambient to Unity");
            bloc.NextLine();
            var resultConvert = ambientToUnity.ConvertToUnity();
            GC.Collect();
            bloc.NextLine();
            bloc.WriteToBloc($"Successfully converted ambient to Unity");
            if (!resultConvert.isConverted)
                bloc.WriteToBloc($"Cannot convert textures to Unity !");
            bloc.Free();
        }

        private static TextureRessources? Pick()
        {
            if (texturesToCreate.Count == 0)
                return null;
            TextureRessources tex = texturesToCreate[0];
            texturesToCreate.RemoveAt(0);
            return tex;
        }
    }
}
