namespace TextureDownloader.Texture.Utils;

public static class Disk
{
    public static (float size, string unit) ByteToKo(long byteSize)
    {
        return (byteSize / 1024, "Ko");
    }

    public static (float size, string unit) ByteToMo(long byteSize)
    {
        return (ByteToKo(byteSize).size / 1024, "Mo");
    }

    public static (float size, string unit) ByteToGo(long byteSize)
    {
        return (ByteToMo(byteSize).size / 1024, "Go");
    }

    public static (float size, string unit) ByteToTo(long byteSize)
    {
        return (ByteToGo(byteSize).size / 1024, "To");
    }

    public static (float free, float total, string unit) CurrentDiskSpace()
    {
        var drive = DriveInfo.GetDrives().First(drive => Directory.GetCurrentDirectory().Contains(drive.Name));

        return (ByteToGo(drive.AvailableFreeSpace).size, ByteToGo(drive.TotalSize).size, "Go");
    }
}