using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureDownloader.Texture.Utils
{
    public class UI
    {
        private const string was_downloaded = "■";
        private const string was_not_downloaded = " ";

        public static string GetProgressBar(int percentage, int size)
        {
            int wasDownloadedSize = (int)(percentage / 100f * size);
            int wasNotDownloadedSize = size - wasDownloadedSize;
            string temp = "";
            for (int i = 0; i < wasDownloadedSize; i++)
                temp += was_downloaded;

            for (int i = 0; i < wasNotDownloadedSize; i++)
                temp += was_not_downloaded;

            return temp;
        }
    }
}
