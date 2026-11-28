namespace Seedysoft.Photo.Lib.Exif;

// Strings coding constants. In the lower 16 bits the code page number (1 to 65535) is coded.
// In the higher 16 bits the EXIF tag type and additional infos are coded.
public enum StrCoding
{
    Utf8 = StrCodingFormat.TypeAscii | 65001, // Default value for all tags of type "ExifTagType.Ascii". 
    UsAscii = StrCodingFormat.TypeAscii | 20127,
    WestEuropeanWin = StrCodingFormat.TypeAscii | 1252,
    UsAsciiUndef = StrCodingFormat.TypeUndefined | 20127, // For the tags "ExifVersion", "FlashPixVersion" and others.			   
    Utf16LeByte = StrCodingFormat.TypeByte | 1200, // For the Microsoft tags "XpTitle", "XpComment", "XpAuthor", "XpKeywords" and "XpSubject".
    IdCodeUtf16 = StrCodingFormat.TypeUndefinedWithIdCode | 1200, // Default value for the tag "UserComment".
    IdCodeUsAscii = StrCodingFormat.TypeUndefinedWithIdCode | 20127,
    IdCodeWestEu = StrCodingFormat.TypeUndefinedWithIdCode | 1252,
}
