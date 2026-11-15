namespace Seedysoft.Photo.Lib.Exif;

// Tag types. These constants are used as array indexes for the array "TypeByteCount".
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "As design")]
public enum ExifTagType
{
    Byte = 1,
    Ascii = 2,
    UShort = 3,
    ULong = 4,
    URational = 5,
    SByte = 6, // Only for TIFFs
    Undefined = 7,
    SShort = 8, // Only for TIFFs
    SLong = 9,
    SRational = 10,
    Float = 11, // Only for TIFFs
    Double = 12, // Only for TIFFs
}
