namespace Seedysoft.Photo.Lib.Exif;

public enum ExifErrCode
{
    InternalError = 0,
    ImageTypeIsNotSupported = 1,
    ImageHasUnsupportedFeatures = 2,
    InternalImageStructureIsWrong = 3,
    ExifBlockHasIllegalContent = 4, // Internal image structure is OK but the EXIF block has an illegal content
    ExifDataAreTooLarge = 5,
    ImageTypesDoNotMatch = 6,
};
