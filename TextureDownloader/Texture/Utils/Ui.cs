namespace TextureDownloader.Texture.Utils;

public static class Ui
{
    private const string WasDownloaded = "■";
    private const string WasNotDownloaded = " ";

    public static string GetProgressBar(int percentage, int size)
    {
        var wasDownloadedSize = (int) (percentage / 100f * size);
        var wasNotDownloadedSize = size - wasDownloadedSize;
        var temp = "";
        for (var i = 0; i < wasDownloadedSize; i++)
            temp += WasDownloaded;

        for (var i = 0; i < wasNotDownloadedSize; i++)
            temp += WasNotDownloaded;

        return temp;
    }
}