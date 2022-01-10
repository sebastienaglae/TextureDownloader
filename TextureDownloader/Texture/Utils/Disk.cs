namespace TextureDownloader.Texture.Utils
{
    public abstract class Disk
    {
        public static (float size, string unit) ByteToKo(long byteSize) => (byteSize / 1024, "Ko");
        public static (float size, string unit) ByteToMo(long byteSize) => (ByteToKo(byteSize).size / 1024, "Mo");
        public static (float size, string unit) ByteToGo(long byteSize) => (ByteToMo(byteSize).size / 1024, "Go");
        public static (float size, string unit) ByteToTo(long byteSize) => (ByteToGo(byteSize).size / 1024, "To");

        public static (float free, float total, string unit) CurrentDiskSpace()
        {
            var drive = DriveInfo.GetDrives().Where((drive) => Directory.GetCurrentDirectory().Contains(drive.Name)).First();

            return (ByteToGo(drive.AvailableFreeSpace).size, ByteToGo(drive.TotalSize).size, "Go");
        }
    }
}
