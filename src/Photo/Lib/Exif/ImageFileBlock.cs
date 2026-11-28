namespace Seedysoft.Photo.Lib.Exif;

public enum ImageFileBlock
{
    // The values must be enumerated consecutively
    Unknown = 0, // Internal value, do not use
    Exif = 1,
    Iptc = 2,
    Xmp = 3,
    JpegComment = 4,
    PngMetaData = 5,
    PngDateChanged = 6,
};
