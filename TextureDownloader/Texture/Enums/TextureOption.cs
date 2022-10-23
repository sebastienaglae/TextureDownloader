namespace TextureDownloader.Texture.Enums;

public class TextureOption
{
    public enum Extension
    {
        PNG,
        JPG,
        HDR,
        TONEMAPPED,
        EXR,
        OBJ,
        SBSAR,
        ALL
    }

    public enum Quality
    {
        SQ,
        LQ,
        HQ,
        ALL
    }

    public enum Resolution
    {
        ONE_K = 1,
        TWO_K = 2,
        THREE_K = 3,
        FOUR_K = 4,
        FIVE_K = 5,
        SIX_K = 6,
        SEVEN_K = 7,
        EIGHT_K = 8,
        NINE_K = 9,
        TEN_K = 10,
        TWELVE_K = 12,
        FIFTHTEEN_K = 15,
        SIXTEEN_K = 16,
        THIRTYTWO_K = 32,
        THIRTYTHREE_K = 33,
        ALL = 0
    }

    public static string GetResolutionString(Resolution resolution)
    {
        return (int) resolution + "K";
    }
}