namespace Seedysoft.Photo.Lib.Exif;

public enum StrCodingFormat
{
    TypeAscii = 0x00000000, // Tag type is "ExifTagType.Ascii". A null terminating character is added when writing.
    TypeUndefined = 0x00010000, // Tag type is "ExifTagType.Undefined". A null terminating character is not present.
    TypeByte = 0x00020000, // Tag type is "ExifTagType.Byte". A null terminating character is added when writing.
    TypeUndefinedWithIdCode = 0x00030000, // Tag type is "ExifTagType.Undefined" and an additional ID code is present. A null terminating character is not present.
};
