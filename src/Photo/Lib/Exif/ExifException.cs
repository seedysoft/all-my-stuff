namespace Seedysoft.Photo.Lib.Exif;

public class ExifException(ExifErrCode ErrorCode) : Exception
{
    public override string Message
    {
        get
        {
            return ErrorCode switch
            {
                ExifErrCode.ImageTypeIsNotSupported => "Image type is not supported!",
                ExifErrCode.ImageHasUnsupportedFeatures => "Image has unsupported features!",
                ExifErrCode.InternalImageStructureIsWrong => "Internal image structure is wrong!",
                ExifErrCode.ExifBlockHasIllegalContent => "EXIF block has an illegal content!",
                ExifErrCode.ExifDataAreTooLarge => "EXIF data are too large: 64 kB for JPEG files, 2 GB for TIFF files!",
                ExifErrCode.ImageTypesDoNotMatch => "Image types do not match!",
                ExifErrCode.InternalError => "Internal error!",
                _ => "Unknown error!",
            };
        }
    }
}
