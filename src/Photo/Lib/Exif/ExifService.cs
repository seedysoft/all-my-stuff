using System.Text;

namespace Seedysoft.Photo.Lib.Exif;

internal partial class ExifService
{
    #region Public area

    public ImageType ImageType { get; private set; }
    public int MakerNoteOriginalOffset { get; private set; }
    public bool ExifBlockWasIllegal { get; private set; }
    public const int IfdShift = 16;

    /// <summary>
    /// Load EXIF data from an image file. If the file is a valid image file but without
    /// an EXIF block, an empty EXIF block is created. An exception occurs in the following situations:
    ///  •The file does not exist.
    ///  •The access to the file is denied.
    ///  •The file is not a valid JPEG, TIFF or PNG file.
    /// </summary>
    /// <param name="fileNameWithPath">File name of the image file.</param>
    /// <param name="options">Load options.</param>
    public ExifService(string fileNameWithPath, ExifLoadOptions options = 0) : this()
    {
        _FileNameWithPath = Path.GetFullPath(fileNameWithPath);
        using FileStream ImageFile = File.OpenRead(_FileNameWithPath);
        ReadFromStream(ImageFile, options);
    }
    public ExifService(Stream imageStream, ExifLoadOptions options = 0) : this() =>
        ReadFromStream(imageStream, options);

    public static ExifService Empty()
    {
        ExifService EmptyBlock = new();
        EmptyBlock.SetEmptyExifBlock();

        return EmptyBlock;
    }

    /// <summary>
    /// Save the EXIF data in a new file or overwrite the current file.
    /// </summary>
    /// <param name="destFileNameWithPath">File name for saving the image and the EXIF data. If this
    /// parameter is "null", the EXIF data is replaced in the current file.</param>
    /// <param name="SaveOptions">Save options.</param>
    /// <exception cref="ExifException"></exception>
    public void Save(string? destFileNameWithPath = null/*, ExifSaveOptions SaveOptions = 0*/)
    {
        string? SourceFileNameWithPath = _FileNameWithPath ?? throw new ExifException(ExifErrCode.InternalError);

        if (destFileNameWithPath != null)
            destFileNameWithPath = Path.GetFullPath(destFileNameWithPath);

        bool IsDestFileTempFile = false;
        if ((destFileNameWithPath == null) ||
            (string.Compare(SourceFileNameWithPath, destFileNameWithPath, StringComparison.OrdinalIgnoreCase) == 0))
        {
            int BackslashPosition = SourceFileNameWithPath.LastIndexOf('\\');
            int DotPosition = SourceFileNameWithPath.LastIndexOf('.');
            if (DotPosition <= BackslashPosition)
                DotPosition = SourceFileNameWithPath.Length;
            destFileNameWithPath = SourceFileNameWithPath.Insert(DotPosition, "~");
            IsDestFileTempFile = true;
        }

        bool DestFileCreated = false;
        FileStream? SourceFile = null, DestFile = null;
        try
        {
            SourceFile = File.OpenRead(SourceFileNameWithPath);
            DestFile = File.Open(destFileNameWithPath, FileMode.Create);
            DestFileCreated = true;
            Save(SourceFile, DestFile/*, SaveOptions*/);
            SourceFile.Dispose();
            SourceFile = null;
            DestFile.Dispose();
            DestFile = null;
            if (IsDestFileTempFile)
            {
                File.Delete(SourceFileNameWithPath); // Delete original image file. If the original file cannot be deleted, delete
                                                     // the temporary file within the "catch" block.
                File.Move(destFileNameWithPath, SourceFileNameWithPath);
            }
        }
        catch
        {
            SourceFile?.Dispose();

            DestFile?.Dispose();

            if (DestFileCreated)
                File.Delete(destFileNameWithPath);

            throw;
        }
    }
    public void Save(Stream sourceStream, Stream destStream/*, ExifSaveOptions SaveOptions = 0*/)
    {
        ImageType SourceImageType = CheckStreamTypeAndCompatibility(sourceStream);
        if (SourceImageType != ImageType)
            throw new ExifException(ExifErrCode.ImageTypesDoNotMatch);

        if (SourceImageType == ImageType.Jpeg)
            SaveJpeg(sourceStream, destStream);
        else if (SourceImageType == ImageType.Tiff)
            SaveTiff(sourceStream, destStream);
        else if (SourceImageType == ImageType.Png)
            SavePng(sourceStream, destStream);
    }

    /// <summary>
    /// Read a string from a tag.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="value">Tag data as a string. In case of an error an empty string is given back.</param>
    /// <param name="coding">Tag type and code page in which the string is coded.</param>
    /// <returns>true  = The tag was successfully read. false = The tag was not found or is of wrong type.</returns>
    public bool GetTagValue(ExifTag tagSpec, out string value, StrCoding coding)
    {
        bool Success = false;
        var StringTagFormat = (StrCodingFormat)((uint)coding & 0xFFFF0000);
        ushort CodePage = (ushort)coding;
        if (StringTagFormat == StrCodingFormat.TypeUndefinedWithIdCode)
        {
            Success = GetTagValueWithIdCode(tagSpec, out value, CodePage);
        }
        else
        {
            value = string.Empty;
            if (GetTagItem(tagSpec, out TagItem? t))
            {
                int i = t.ValueCount;
                int j = t.ValueIndex;

                if (CodePage is 1200 or 1201) // UTF16LE or UTF16BE
                {
                    // Remove all null terminating characters. Here a null terminating character consists of 2 zero-bytes.
                    while ((i >= 2) && (t.ValueData[j + i - 2] == 0) && (t.ValueData[j + i - 1] == 0))
                        i -= 2;
                }
                else
                {
                    // Remove all null terminating characters.
                    while ((i >= 1) && (t.ValueData[j + i - 1] == 0))
                        i--;
                }

                if (CodePage != 0)
                {
                    if (((StringTagFormat == StrCodingFormat.TypeAscii) && (t.TagType == ExifTagType.Ascii)) ||
                        ((StringTagFormat == StrCodingFormat.TypeUndefined) && (t.TagType == ExifTagType.Undefined)) ||
                        ((StringTagFormat == StrCodingFormat.TypeByte) && (t.TagType == ExifTagType.Byte)))
                    {
                        value = Encoding.GetEncoding(CodePage).GetString(t.ValueData, j, i);
                        Success = true;
                    }
                }
            }
        }

        return Success;
    }
    /// <summary>
    /// Write a string to a tag.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="value">String to be written.</param>
    /// <param name="coding">Tag type and code page in which the string should be coded.</param>
    /// <returns>true = The tag was successfully written. false = Error. Tag ID is not allowed to be written or IFD is illegal.</returns>
    public bool SetTagValue(ExifTag tagSpec, string value, StrCoding coding)
    {
        bool Success = false;
        var StringTagFormat = (StrCodingFormat)((uint)coding & 0xFFFF0000);
        ushort CodePage = (ushort)coding;
        if (StringTagFormat == StrCodingFormat.TypeUndefinedWithIdCode)
        {
            Success = SetTagValueWithIdCode(tagSpec, value, CodePage);
        }
        else
        {
            ExifTagType tagType = StringTagFormat == StrCodingFormat.TypeUndefined
                ? ExifTagType.Undefined
                : StringTagFormat == StrCodingFormat.TypeByte ? ExifTagType.Byte : ExifTagType.Ascii;

            int NullTermBytes = 0;
            if (tagType != ExifTagType.Undefined)
            {
                if (CodePage is 1200 or 1201) // UTF16LE or UTF16BE
                    NullTermBytes = 2; // Write null terminating character with 2 zero-bytes
                else
                    NullTermBytes = 1; // Write null terminating character with 1 zero-byte
            }

            if (CodePage != 0)
            {
                byte[] StringAsByteArray = Encoding.GetEncoding(CodePage).GetBytes(value);
                int StrByteLen = StringAsByteArray.Length;
                int TotalByteCount = StrByteLen + NullTermBytes;
                TagItem? t = PrepareTagForCompleteWriting(tagSpec, tagType, TotalByteCount);
                if (t != null)
                {
                    Array.Copy(StringAsByteArray, 0, t.ValueData, t.ValueIndex, StrByteLen);
                    if (NullTermBytes >= 1)
                        t.ValueData[t.ValueIndex + StrByteLen] = 0;
                    if (NullTermBytes >= 2)
                        t.ValueData[t.ValueIndex + StrByteLen + 1] = 0;
                    Success = true;
                }
            }
        }

        return Success;
    }

    /// <summary>
    /// Read a 32 bit signed integer number from a tag. The tag type must be one of the following values:
    ///  ExifTagType.Byte, ExifTagType.UShort, ExifTagType.ULong, ExifTagType.SLong.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="value">Out: 32 bit integer number with sign. If the tag type is "ExifTagType.ULong" and
    ///  the tag contains a number from 0x80000000 to 0xFFFFFFFF, it is treated as an error.</param>
    /// <param name="index">Array index of the value to be read.</param>
    /// <returns>true = The tag was successfully read. false = The tag was not found, is of wrong type
    /// or is out of range. In this case "Value" is 0.</returns>
    public bool GetTagValue(ExifTag tagSpec, out int value, int index = 0)
    {
        bool Success = false;
        uint TempValue = 0;
        if (GetTagItem(tagSpec, out TagItem? t) && ReadUintElement(t, index, out TempValue))
        {
            Success = (t.TagType != ExifTagType.ULong) || ((int)TempValue >= 0);
            if (!Success)
                TempValue = 0;
        }

        value = (int)TempValue;

        return Success;
    }
    /// <summary>
    /// Write a 32 bit signed integer number to a tag.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="value">32 bit integer number with sign.</param>
    /// <param name="tagType">Tag type to be written: ExifTagType.Byte, ExifTagType.UShort, ExifTagType.ULong
    /// or ExifTagType.SLong</param>
    /// <param name="index">Array index of the value to be written.</param>
    /// <returns>true = The tag was successfully written. false = The tag specification is illegal or
    /// "Value" is out of range.</returns>
    public bool SetTagValue(ExifTag tagSpec, int value, ExifTagType tagType, int index = 0)
    {
        bool Success = false;
        switch (tagType)
        {
            case ExifTagType.Byte:
                if (value is >= 0 and <= 255)
                    Success = true;
                break;
            case ExifTagType.UShort:
                if (value is >= 0 and <= 65535)
                    Success = true;
                break;
            case ExifTagType.ULong:
                if (value >= 0)
                    Success = true;
                break;
            case ExifTagType.SLong:
                Success = true;
                break;
        }

        if (Success)
        {
            TagItem? t = PrepareTagForArrayItemWriting(tagSpec, tagType, index);
            Success = WriteUintElement(t, index, (uint)value);
        }

        return Success;
    }

    public bool GetTagValue(ExifTag tagSpec, out uint value, int index = 0)
    {
        bool Success = false;
        uint TempValue = 0;
        if (GetTagItem(tagSpec, out TagItem? t) && ReadUintElement(t, index, out TempValue))
        {
            Success = (t.TagType != ExifTagType.SLong) || ((int)TempValue >= 0);
            if (!Success)
                TempValue = 0;
        }

        value = TempValue;

        return Success;
    }
    public bool SetTagValue(ExifTag tagSpec, uint value, ExifTagType tagType, int index = 0)
    {
        bool Success = false;
        switch (tagType)
        {
            case ExifTagType.Byte:
                if (value is >= 0 and <= 255)
                    Success = true;
                break;
            case ExifTagType.UShort:
                if (value is >= 0 and <= 65535)
                    Success = true;
                break;
            case ExifTagType.ULong:
                Success = true;
                break;
            case ExifTagType.SLong:
                if ((int)value >= 0)
                    Success = true;
                break;
        }

        if (Success)
        {
            TagItem? t = PrepareTagForArrayItemWriting(tagSpec, tagType, index);
            Success = WriteUintElement(t, index, value);
        }

        return Success;
    }

    public bool GetTagValue(ExifTag tagSpec, out ExifRational value, int index = 0)
    {
        bool Success = false;

        if (GetTagItem(tagSpec, out TagItem? t) && ReadURationalElement(t, index, out ExifRational exifRational))
        {
            value = exifRational;

            Success = true;
        }
        else
        {
            value = new ExifRational(0, 0);
        }

        return Success;
    }
    public bool SetTagValue(ExifTag tagSpec, ExifRational value, ExifTagType tagType, int index = 0)
    {
        bool Success = false;
        switch (tagType)
        {
            case ExifTagType.SRational:
                if ((value.Numer < 0x80000000) && (value.Denom < 0x80000000))
                {
                    if (value.IsNeg)
                        value.Numer = (uint)-(int)value.Numer;

                    Success = true;
                }
                else
                {
                    Success = false;
                }

                break;

            case ExifTagType.URational:
                Success = !value.IsNegative();
                break;
        }

        if (Success)
        {
            TagItem? t = PrepareTagForArrayItemWriting(tagSpec, tagType, index);
            Success = WriteURationalElement(t, index, value);
        }

        return Success;
    }

    /// <summary>
    /// Read time stamp from a tag. The tag type must be "ExifTagType.Ascii" and the tag must contain
    /// a null terminated string with 19 characters in the format "yyyy:MM:dd HH:mm:ss" or with 10 characters
    /// in the format "yyyy:MM:dd".
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="value">Out: Date time stamp. In case of an error, the value "DateTime.MinValue" is returned.</param>
    /// <param name="format">Selection between time stamp format with 19 or 10 characters.</param>
    /// <returns>true = The tag was successfully read. false = The tag was not found, is of wrong type or wrong format.</returns>
    public bool GetTagValue(ExifTag tagSpec, out DateTime value, ExifDateFormat format = ExifDateFormat.DateAndTime)
    {
        bool Success = false;
        value = DateTime.MinValue;
        if (GetTagItem(tagSpec, out TagItem? t) && (t.TagType == ExifTagType.Ascii))
        {
            try
            {
                int i = t.ValueIndex;
                if ((t.ValueCount >= 10) && (t.ValueData[i + 4] == (byte)':') && (t.ValueData[i + 7] == (byte)':'))
                {
                    int Year = (CalculateTwoDigitDecNumber(t.ValueData, i) * 100) + CalculateTwoDigitDecNumber(t.ValueData, i + 2);
                    int Month = CalculateTwoDigitDecNumber(t.ValueData, i + 5);
                    int Day = CalculateTwoDigitDecNumber(t.ValueData, i + 8);
                    if ((format == ExifDateFormat.DateAndTime) &&
                        (t.ValueCount == 19 + 1) &&
                        (t.ValueData[i + 10] == (byte)' ') &&
                        (t.ValueData[i + 13] == (byte)':') &&
                        (t.ValueData[i + 16] == (byte)':') &&
                        (t.ValueData[i + 19] == 0))
                    {
                        int Hour = CalculateTwoDigitDecNumber(t.ValueData, i + 11);
                        int Minute = CalculateTwoDigitDecNumber(t.ValueData, i + 14);
                        int Second = CalculateTwoDigitDecNumber(t.ValueData, i + 17);
                        value = new DateTime(Year, Month, Day, Hour, Minute, Second);
                        Success = true;
                    }
                    else if ((format == ExifDateFormat.DateOnly) && (t.ValueCount == 10 + 1) && (t.ValueData[i + 10] == 0))
                    {
                        value = new DateTime(Year, Month, Day);
                        Success = true;
                    }
                }
            }
            catch
            {
                // Illegal time stamp
            }
        }

        return Success;
    }
    /// <summary>
    /// Write a time stamp to a tag. The tag type is set to "ExifTagType.Ascii" and the time stamp
    /// is written in the format "yyyy:MM:dd HH:mm:ss" or "yyyy:MM:dd".
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="value">Date time stamp.</param>
    /// <param name="format">Selection between time stamp format with 19 or 10 characters.</param>
    /// <returns>true: The tag was successfully written. false: Error occurred.</returns>
    public bool SetTagValue(ExifTag tagSpec, DateTime value, ExifDateFormat format = ExifDateFormat.DateAndTime)
    {
        bool Success = false;
        int sByteCount = 0;
        if (format == ExifDateFormat.DateAndTime)
            sByteCount = 19 + 1; // "+ 1" because a null character has to be written
        else if (format == ExifDateFormat.DateOnly)
            sByteCount = 10 + 1;

        if (sByteCount != 0)
        {
            TagItem? t = PrepareTagForCompleteWriting(tagSpec, ExifTagType.Ascii, sByteCount);
            if (t != null)
            {
                int i = t.ValueIndex;
                int Year = value.Year;
                ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Year / 100);
                ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, Year % 100);
                t.ValueData[i] = (byte)':';
                i++;
                ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, value.Month);
                t.ValueData[i] = (byte)':';
                i++;
                ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, value.Day);
                if (format == ExifDateFormat.DateAndTime)
                {
                    t.ValueData[i] = (byte)' ';
                    i++;
                    ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, value.Hour);
                    t.ValueData[i] = (byte)':';
                    i++;
                    ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, value.Minute);
                    t.ValueData[i] = (byte)':';
                    i++;
                    ConvertTwoDigitNumberToByteArr(t.ValueData, ref i, value.Second);
                }

                t.ValueData[i] = 0;
                i++;
                Success = true;
            }
        }

        return Success;
    }

    /// <summary>
    /// Read the tag data as raw data without copying the raw data.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="tagType">Out: Type of the tag.</param>
    /// <param name="valueCount">Out: Number of values which are stored in the tag.</param>
    /// <param name="RawData">Out: Array with the tag data, but there may also be other data stored in this array.
    ///                      There are the following restrictions:
    ///                      •The data returned in the array "RawData" must not be changed by the caller of this method.
    ///                      •After new data have been written into the specified tag, the array "RawData" must not be used any
    ///                       more. Therefore, the data returned in "RawData" should be copied as soon as possible.
    ///                      •If the tag does not exist, "RawData" is an empty array.</param>
    /// <param name="RawDataIndex">Out: Array index at which the tag data starts. The first data byte is stored in
    ///                      "RawData[RawDataIndex]" and the last data byte is stored in
    ///                      "RawData[RawDataIndex + GetTagByteCount(tagType, valueCount) - 1]"</param>
    /// <returns>true: The tag was successfully read. false: The tag was not found.</returns>
    public bool GetTagRawData(ExifTag tagSpec, out ExifTagType tagType, out int valueCount, out byte[] RawData, out int RawDataIndex)
    {
        bool Success = false;
        if (GetTagItem(tagSpec, out TagItem? t))
        {
            tagType = t.TagType;
            valueCount = t.ValueCount;
            RawData = t.ValueData;
            RawDataIndex = t.ValueIndex;
            Success = true;
        }
        else
        {
            tagType = 0;
            valueCount = 0;
            RawData = [];
            RawDataIndex = 0;
        }

        return Success;
    }
    /// <summary>
    /// Read the tag data as raw data with copying the raw data.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="tagType">Out: Type of the tag.</param>
    /// <param name="valueCount">Out: Number of values which are stored in the tag.</param>
    /// <param name="rawData">Out: Array with the tag data. If the tag does not exist, "RawData" is an empty array.</param>
    /// <returns>true: The tag was successfully read. false: The tag was not found.</returns>
    public bool GetTagRawData(ExifTag tagSpec, out ExifTagType tagType, out int valueCount, out byte[] rawData)
    {
        bool Success = false;
        if (GetTagItem(tagSpec, out TagItem? t))
        {
            tagType = t.TagType;
            valueCount = t.ValueCount;
            int k = t.ByteCount;
            rawData = new byte[k];
            Array.Copy(t.ValueData, t.ValueIndex, rawData, 0, k);
            Success = true;
        }
        else
        {
            tagType = 0;
            valueCount = 0;
            rawData = [];
        }

        return Success;
    }
    /// <summary>
    /// Write the tag data as raw data.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <param name="tagType">Type of the tag.</param>
    /// <param name="valueCount">Number of values which are stored in the tag. This is not the number of raw data bytes!
    /// The number of raw data bytes is automatically calculated by the parameters "tagType" and "valueCount".</param>
    /// <param name="RawData">rray with the tag data. The tag data is not copied by this method, therefore
    /// the array content must not be changed after the call of this method.</param>
    /// <param name="RawDataIndex">Array index at which the data starts. Set this parameter to 0, if the data starts with
    /// the first element of the array "RawData".</param>
    /// <returns>true: The tag was successfully set. false: Error occurred.</returns>
    public bool SetTagRawData(ExifTag tagSpec, ExifTagType tagType, int valueCount, byte[] RawData, int RawDataIndex = 0)
    {
        bool Success = false;
        ExifIfd Ifd = ExtractIfd(tagSpec);
        if ((uint)Ifd < ExifIfdCount)
        {
            int RawDataByteCount = GetTagByteCount(tagType, valueCount);
            Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
            if (IfdTagTable.TryGetValue(ExtractTagId(tagSpec), out TagItem? t))
            {
                t.TagType = tagType;
                t.ValueCount = valueCount;
                t.ValueData = RawData;
                t.ValueIndex = RawDataIndex;
                t.AllocatedByteCount = RawDataByteCount;
            }
            else
            {
                t = new TagItem(ExtractTagId(tagSpec), tagType, valueCount, RawData, RawDataIndex, RawDataByteCount);
                IfdTagTable.Add(t.TagId, t);
            }

            Success = true;
        }

        return Success;
    }

    public bool GetTagValueCount(ExifTag tagSpec, out int valueCount)
    {
        if (GetTagItem(tagSpec, out TagItem? t))
        {
            valueCount = t.ValueCount;
            return true;
        }
        else
        {
            valueCount = 0;
            return false;
        }
    }
    public bool SetTagValueCount(ExifTag tagSpec, int valueCount)
    {
        if (!GetTagItem(tagSpec, out TagItem? t) || (uint)valueCount > MaxTagValueCount)
            return false;

        t.SetTagTypeAndValueCount(t.TagType, valueCount, true);
        return true;
    }
    public bool SetTagValueCount(ExifTag tagSpec, int valueCount, ExifTagType tagType)
    {
        TagItem? t = PrepareTagForArrayItemWriting(tagSpec, tagType, valueCount - 1);

        return t != null;
    }

    public bool GetTagType(ExifTag tagSpec, out ExifTagType tagType)
    {
        if (GetTagItem(tagSpec, out TagItem? t))
        {
            tagType = t.TagType;
            return true;
        }
        else
        {
            tagType = 0;
            return false;
        }
    }

    public bool TagExists(ExifTag tagSpec)
    {
        ExifIfd Ifd = ExtractIfd(tagSpec);
        if ((uint)Ifd >= ExifIfdCount)
            return false;

        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];

        return IfdTagTable.ContainsKey(ExtractTagId(tagSpec));
    }

    public bool IfdExists(ExifIfd ifd)
    {
        if ((uint)ifd >= ExifIfdCount)
            return false;

        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)ifd];

        return IfdTagTable.Count > 0;
    }

    public bool ImageFileBlockExists(ImageFileBlock blockType) => ImageFileBlockInfo[(int)blockType] == ImageFileBlockState.Existent;

    /// <summary>
    /// Remove a single tag.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of the IFD in which the tag is stored and the tag ID.</param>
    /// <returns>true: Tag was successfully removed. false: Tag was not removed, because it doesn't exist.</returns>
    public bool RemoveTag(ExifTag tagSpec)
    {
        ExifIfd Ifd = ExtractIfd(tagSpec);
        if ((uint)Ifd >= ExifIfdCount)
            return false;

        Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];

        return IfdTagTable.Remove(ExtractTagId(tagSpec));
    }

    /// <summary>
    /// Remove all tags from a specific IFD.
    /// </summary>
    /// <param name="ifd">IFD from which all tags are to be removed. If the IFD "ThumbnailData" is
    /// removed, the thumbnail image is removed, too.</param>
    /// <returns>true: IFD was successfully removed. false: IFD was not removed, because it doesn't exist.</returns>
    public bool RemoveAllTagsFromIfd(ExifIfd ifd)
    {
        bool Removed = false;
        if (ifd == ExifIfd.PrimaryData)
        {
            RemoveAllTagsFromIfdPrimaryData();
            Removed = true;
        }
        else if ((uint)ifd < ExifIfdCount)
        {
            ClearIfd_Unchecked(ifd);
            if (ifd == ExifIfd.ThumbnailData)
                RemoveThumbnailImage(false);

            Removed = true;
        }

        return Removed;
    }

    /// <summary>
    /// Remove all tags from the EXIF block and remove the thumbnail image.
    /// </summary>
    public void RemoveAllTags()
    {
        RemoveAllTagsFromIfdPrimaryData();
        for (ExifIfd Ifd = ExifIfd.PrivateData; Ifd < (ExifIfd)ExifIfdCount; Ifd++)
            ClearIfd_Unchecked(Ifd);

        RemoveThumbnailImage(false);
        ImageFileBlockInfo[(int)ImageFileBlock.Exif] = ImageFileBlockState.Removed;
    }

    public void RemoveImageFileBlock(ImageFileBlock blockType)
    {
        if (blockType == ImageFileBlock.Exif)
        {
            RemoveAllTags();
        }
        else if (blockType != ImageFileBlock.Unknown)
        {
            if (ImageType == ImageType.Tiff)
            {
                if (blockType == ImageFileBlock.Xmp)
                    _ = RemoveTag(ExifTag.XmpMetadata);
                else if (blockType == ImageFileBlock.Iptc)
                    _ = RemoveTag(ExifTag.IptcMetadata);
            }

            ImageFileBlockInfo[(int)blockType] = ImageFileBlockState.Removed;
        }
    }

    /// <summary>
    /// Replace all EXIF tags and the thumbnail image by the EXIF data of another image file.
    /// </summary>
    /// <param name="sourceExifData">EXIF data of another image, that will be copied to the EXIF data of
    /// the current object. A deep copy is done.</param>
    public void ReplaceAllTagsBy(ExifService sourceExifData)
    {
        // Byte order handling
        bool SwapByteOrder = false;
        if (ImageType == ImageType.Tiff)
        {
            if (ByteOrder != sourceExifData.ByteOrder)
            {
                // In TIFF images the EXIF block cannot be completely replaced by another EXIF block because
                // it contains internal tags which must not be changed. If the byte order of the two EXIF blocks
                // is different, the byte order of the source EXIF block shall be adapted.
                SwapByteOrder = true;
            }
        }
        else
        {
            ByteOrder = sourceExifData.ByteOrder; // Complete EXIF block is copied, therefore the byte order is also copied.
        }

        // Remove all tags. But in TIFF images the TIFF internal tags, the XMP tag and the IPTC tag are not removed.
        RemoveAllTags();

        // Copy IFD Primary data.
        Dictionary<ExifTagId, TagItem> SourceIfdTagList = sourceExifData.TagTable[(uint)ExifIfd.PrimaryData];
        Dictionary<ExifTagId, TagItem> DestIfdTagList = TagTable[(uint)ExifIfd.PrimaryData];
        bool IsTiffImagePresent = (ImageType == ImageType.Tiff) || (sourceExifData.ImageType == ImageType.Tiff);
        bool CopyTag = true;
        foreach (TagItem SourceTag in SourceIfdTagList.Values)
        {
            if (IsTiffImagePresent)
                CopyTag = !IsInternalTiffTag(SourceTag.TagId) && !IsMetaDataTiffTag(SourceTag.TagId);

            if (CopyTag)
            {
                TagItem DestTag = CopyTagItemDeeply(ExifIfd.PrimaryData, SourceTag, SwapByteOrder);
                _ = AddIfNotExists(DestIfdTagList, DestTag);
            }
        }

        // Copy IFDs PrivateData, GpsInfoData and Interoperability
        ExifIfd[] TempIfds = [ExifIfd.PrivateData, ExifIfd.GpsInfoData, ExifIfd.Interoperability];
        foreach (ExifIfd Ifd in TempIfds)
        {
            SourceIfdTagList = sourceExifData.TagTable[(uint)Ifd];
            DestIfdTagList = TagTable[(uint)Ifd];
            foreach (TagItem SourceTag in SourceIfdTagList.Values)
            {
                TagItem DestTag = CopyTagItemDeeply(Ifd, SourceTag, SwapByteOrder);
                _ = AddIfNotExists(DestIfdTagList, DestTag);
            }
        }

        // Copy IFD ThumbnailData
        if (ImageType == ImageType.Jpeg)
        {
            SourceIfdTagList = sourceExifData.TagTable[(uint)ExifIfd.ThumbnailData];
            DestIfdTagList = TagTable[(uint)ExifIfd.ThumbnailData];
            foreach (TagItem SourceTag in SourceIfdTagList.Values)
            {
                TagItem DestTag = CopyTagItemDeeply(ExifIfd.ThumbnailData, SourceTag, SwapByteOrder);
                _ = AddIfNotExists(DestIfdTagList, DestTag);
            }

            if (sourceExifData.ThumbnailImageExists())
            {
                ThumbnailStartIndex = 0;
                ThumbnailByteCount = sourceExifData.ThumbnailByteCount;
                ThumbnailImage = new byte[ThumbnailByteCount];
                Array.Copy(sourceExifData.ThumbnailImage!, sourceExifData.ThumbnailStartIndex, ThumbnailImage, 0, ThumbnailByteCount);
            }
            else
            {
                RemoveThumbnailImage_Internal();
            }
        }

        // Determine new EXIF block state
        UpdateImageFileBlockInfo();

        // Copy MakerNote index
        MakerNoteOriginalOffset = sourceExifData.MakerNoteOriginalOffset;
    }

    public bool ThumbnailImageExists() => ThumbnailImage != null;

    /// <summary>
    /// Read the thumbnail image data.
    /// </summary>
    /// <param name="thumbnailData">Out: Array with the thumbnail image beginning with the JPEG marker 0xFF 0xD8.
    /// If no thumbnail image is defined, "null" is returned.</param>
    /// <param name="thumbnailIndex">Out: Index of the first data byte of the thumbnail image.</param>
    /// <param name="thumbnailByteCount">Out: Number of bytes of the thumbnail image.</param>
    /// <returns>true: Thumbnail image is given back. false: Thumbnail image does not exist.</returns>
    public bool GetThumbnailImage(out byte[]? thumbnailData, out int thumbnailIndex, out int thumbnailByteCount)
    {
        thumbnailData = ThumbnailImage;
        thumbnailIndex = ThumbnailStartIndex;
        thumbnailByteCount = ThumbnailByteCount;
        bool Success = ThumbnailImageExists();

        return Success;
    }

    /// <summary>
    /// Write the thumbnail image data.
    /// </summary>
    /// <param name="thumbnailData"> Array which contains the thumbnail image beginning with the
    /// JPEG marker 0xFF 0xD8. The array is not copied by this method so it should not be changed
    /// after calling this method.</param>
    /// <param name="thumbnailIndex">Index of the first byte of the thumbnail image.</param>
    /// <param name="thumbnailByteCount">Number of bytes of the thumbnail image. -1: All bytes
    /// until the end of the array are used.</param>
    /// <returns>true: Thumbnail image was successfully set. false: Error occurred.</returns>
    public bool SetThumbnailImage(byte[] thumbnailData, int thumbnailIndex = 0, int thumbnailByteCount = -1)
    {
        bool Success = false;
        if (thumbnailData != null)
        {
            ThumbnailImage = thumbnailData;
            ThumbnailStartIndex = thumbnailIndex;
            ThumbnailByteCount = thumbnailByteCount < 0 ? thumbnailData.Length - thumbnailIndex : thumbnailByteCount;
            _ = SetTagValue(ExifTag.JpegInterchangeFormat, 0, ExifTagType.ULong); // "0" is a dummy value
            _ = SetTagValue(ExifTag.JpegInterchangeFormatLength, ThumbnailByteCount, ExifTagType.ULong);
            Success = true;
        }

        return Success;
    }

    /// <summary>
    /// Remove the thumbnail image data.
    /// </summary>
    /// <param name="removeAlsoThumbnailTags">false: Remove the thumbnail image, the thumbnail pointer
    /// tag and the thumbnail size tag. true: Remove also all other tags in the IFD "thumbnail data".</param>
    public void RemoveThumbnailImage(bool removeAlsoThumbnailTags)
    {
        RemoveThumbnailImage_Internal();
        if (removeAlsoThumbnailTags)
        {
            ClearIfd_Unchecked(ExifIfd.ThumbnailData);
        }
        else
        {
            _ = RemoveTag(ExifTag.JpegInterchangeFormat);
            _ = RemoveTag(ExifTag.JpegInterchangeFormatLength);
        }
    }

    /// <summary>
    /// Get the byte order for the EXIF data.
    /// </summary>
    public ExifByteOrder ByteOrder { get; private set; }

    /// <summary>
    /// Extract the IFD of a tag specification.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of an IFD and a tag ID.</param>
    /// <returns>IFD that is coded in parameter "tagSpec".</returns>
    public static ExifIfd ExtractIfd(ExifTag tagSpec) => (ExifIfd)((uint)tagSpec >> IfdShift);

    /// <summary>
    /// Extract the tag ID of a tag specification.
    /// </summary>
    /// <param name="tagSpec">Tag specification, consisting of an IFD and a tag ID.</param>
    /// <returns>Tag ID that is coded in parameter "tagSpec".</returns>
    public static ExifTagId ExtractTagId(ExifTag tagSpec) => (ExifTagId)(ushort)tagSpec;

    /// <summary>
    /// Compose IFD and tag ID to a tag specification.
    /// </summary>
    /// <param name="ifd">IFD.</param>
    /// <param name="tagId">Tag ID.</param>
    /// <returns>Tag specification, consisting of an IFD and a tag ID.</returns>
    public static ExifTag ComposeTagSpec(ExifIfd ifd, ExifTagId tagId) =>
        (ExifTag)(((uint)ifd << IfdShift) | (uint)tagId);

    /// <summary>
    /// Get the number of bytes that a tag with the specified type and value count has.
    /// </summary>
    /// <param name="tagType">Tag type.</param>
    /// <param name="valueCount">Number of values within the tag.</param>
    /// <returns>Number of bytes that are needed to store the tag. In case of an illegal tag type
    /// 0 is returned.</returns>
    public static int GetTagByteCount(ExifTagType tagType, int valueCount) =>
        (uint)tagType < TypeByteCount.Length ? TypeByteCount[(uint)tagType] * valueCount : 0;

    public ushort ExifReadUInt16(byte[] data, int startIndex)
    {
        return ByteOrder == ExifByteOrder.BigEndian
            ? (ushort)(((uint)data[startIndex] << 8) | data[startIndex + 1])
            : (ushort)(((uint)data[startIndex + 1] << 8) | data[startIndex]);
    }
    public void ExifWriteUInt16(byte[] data, int startIndex, ushort value)
    {
        if (ByteOrder == ExifByteOrder.BigEndian)
        {
            data[startIndex] = (byte)(value >> 8);
            data[startIndex + 1] = (byte)value;
        }
        else
        {
            data[startIndex + 1] = (byte)(value >> 8);
            data[startIndex] = (byte)value;
        }
    }

    public uint ExifReadUInt32(byte[] data, int startIndex)
    {
        return ByteOrder == ExifByteOrder.BigEndian
            ? ((uint)data[startIndex] << 24) |
                ((uint)data[startIndex + 1] << 16) |
                ((uint)data[startIndex + 2] << 8) |
                data[startIndex + 3]
            : ((uint)data[startIndex + 3] << 24) |
               ((uint)data[startIndex + 2] << 16) |
               ((uint)data[startIndex + 1] << 8) |
               data[startIndex];
    }
    public void ExifWriteUInt32(byte[] data, int startIndex, uint value)
    {
        if (ByteOrder == ExifByteOrder.BigEndian)
        {
            data[startIndex] = (byte)(value >> 24);
            data[startIndex + 1] = (byte)(value >> 16);
            data[startIndex + 2] = (byte)(value >> 8);
            data[startIndex + 3] = (byte)value;
        }
        else
        {
            data[startIndex + 3] = (byte)(value >> 24);
            data[startIndex + 2] = (byte)(value >> 16);
            data[startIndex + 1] = (byte)(value >> 8);
            data[startIndex] = (byte)value;
        }
    }

    private static ushort ReadUInt16BE(byte[] data, int startIndex) =>
        (ushort)(((uint)data[startIndex] << 8) | data[startIndex + 1]);
    private static void WriteUInt16BE(byte[] data, int startIndex, ushort value)
    {
        data[startIndex] = (byte)(value >> 8);
        data[startIndex + 1] = (byte)value;
    }

    private static uint ReadUInt32BE(byte[] data, int startIndex)
    {
        return
            ((uint)data[startIndex] << 24) |
            ((uint)data[startIndex + 1] << 16) |
            ((uint)data[startIndex + 2] << 8) |
            data[startIndex + 3];
    }
    private static void WriteUInt32BE(byte[] data, int startIndex, uint value)
    {
        data[startIndex] = (byte)(value >> 24);
        data[startIndex + 1] = (byte)(value >> 16);
        data[startIndex + 2] = (byte)(value >> 8);
        data[startIndex + 3] = (byte)value;
    }

    /// <summary>
    /// Initialize enumeration for all tags of a specific IFD.
    /// </summary>
    /// <param name="ifd">IFD to be enumerated.</param>
    /// <returns>true: Initialization was successful. false: Eror occurred.</returns>
    public bool InitTagEnumeration(ExifIfd ifd)
    {
        if ((uint)ifd < ExifIfdCount)
        {
            ExifIfdForTagEnumeration = ifd;
            TagEnumerator = TagTable[(uint)ifd].Keys.GetEnumerator();
            return true;
        }
        else
        {
            TagEnumerator = new Dictionary<ExifTagId, TagItem>.KeyCollection.Enumerator();
            return false;
        }
    }

    /// <summary>
    /// Get the next tag of the IFD, that was defined by a previous call to the method "InitTagEnumeration".
    /// </summary>
    /// <param name="tagSpec"></param>
    /// <returns>true: Next tag was successfully read. false: There is no further tag available.</returns>
    public bool EnumerateNextTag(out ExifTag tagSpec)
    {
        bool Success = false;
        if (TagEnumerator.MoveNext())
        {
            ExifTagId TagId = TagEnumerator.Current;
            tagSpec = ComposeTagSpec(ExifIfdForTagEnumeration, TagId);
            Success = true;
        }
        else
        {
            tagSpec = 0;
        }

        return Success;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods for accessing EXIF tags on high level
    // -----------------------------------------------------------------------------------------------------------------

    #region Date Helpers

    public bool GetDateTaken(out DateTime value) =>
        GetDateAndTimeWithMillisecHelper(out value, ExifTag.DateTimeOriginal, ExifTag.SubsecTimeOriginal);
    public bool SetDateTaken(DateTime value) =>
        SetDateAndTimeWithMillisecHelper(value, ExifTag.DateTimeOriginal, ExifTag.SubsecTimeOriginal);
    public void RemoveDateTaken()
    {
        _ = RemoveTag(ExifTag.DateTimeOriginal);
        _ = RemoveTag(ExifTag.SubsecTimeOriginal);
    }

    public bool GetDateDigitized(out DateTime value) =>
        GetDateAndTimeWithMillisecHelper(out value, ExifTag.DateTimeDigitized, ExifTag.SubsecTimeDigitized);
    public bool SetDateDigitized(DateTime value) =>
        SetDateAndTimeWithMillisecHelper(value, ExifTag.DateTimeDigitized, ExifTag.SubsecTimeDigitized);
    public void RemoveDateDigitized()
    {
        _ = RemoveTag(ExifTag.DateTimeDigitized);
        _ = RemoveTag(ExifTag.SubsecTimeDigitized);
    }

    public bool GetDateChanged(out DateTime value) =>
        GetDateAndTimeWithMillisecHelper(out value, ExifTag.DateTime, ExifTag.SubsecTime);
    public bool SetDateChanged(DateTime value) =>
        SetDateAndTimeWithMillisecHelper(value, ExifTag.DateTime, ExifTag.SubsecTime);
    public void RemoveDateChanged()
    {
        _ = RemoveTag(ExifTag.DateTime);
        _ = RemoveTag(ExifTag.SubsecTime);
    }

    #endregion

    #region Gps Helpers

    public bool GetGpsLocation(out Libs.Core.Models.Location location)
    {
        if (GetTagValue(ExifTag.GpsLatitude, out ExifRational LatDeg, 0) && LatDeg.IsValid() &&
            GetTagValue(ExifTag.GpsLatitude, out ExifRational LatMin, 1) && LatMin.IsValid() &&
            GetTagValue(ExifTag.GpsLatitude, out ExifRational LatSec, 2) && LatSec.IsValid() &&
            GetTagValue(ExifTag.GpsLatitudeRef, out string? LatRef, StrCoding.Utf8) && (LatRef?.Length == 1) &&
            GetTagValue(ExifTag.GpsLongitude, out ExifRational LonDeg, 0) && LonDeg.IsValid() &&
            GetTagValue(ExifTag.GpsLongitude, out ExifRational LonMin, 1) && LonMin.IsValid() &&
            GetTagValue(ExifTag.GpsLongitude, out ExifRational LonSec, 2) && LonSec.IsValid() &&
            GetTagValue(ExifTag.GpsLongitudeRef, out string? LonRef, StrCoding.Utf8) && (LonRef?.Length == 1)
        )
        {
            location = new Libs.Core.Models.Location(
                (double)GeoCoordinate.ToDecimal(new GeoCoordinate()
                {
                    CardinalPoint = LatRef[0],
                    Degree = ExifRational.ToDecimal(LatDeg),
                    Minute = ExifRational.ToDecimal(LatMin),
                    Second = ExifRational.ToDecimal(LatSec),
                }),
                (double)GeoCoordinate.ToDecimal(new GeoCoordinate()
                {
                    CardinalPoint = LonRef[0],
                    Degree = ExifRational.ToDecimal(LonDeg),
                    Minute = ExifRational.ToDecimal(LonMin),
                    Second = ExifRational.ToDecimal(LonSec),
                }));

            return true;
        }
        else
        {
            location = Libs.Core.Models.Location.Empty;

            return false;
        }
    }

    //public bool GetGpsLongitude(out GeoCoordinate Value) =>
    //GetGpsCoordinateHelper(out Value, ExifTag.GpsLongitude, ExifTag.GpsLongitudeRef, 'W', 'E');

    //public bool SetGpsLongitude(GeoCoordinate Value) =>
    //    SetGpsCoordinateHelper(Value, ExifTag.GpsLongitude, ExifTag.GpsLongitudeRef, 'W', 'E');

    //public void RemoveGpsLongitude()
    //{
    //    _ = RemoveTag(ExifTag.GpsLongitude);
    //    _ = RemoveTag(ExifTag.GpsLongitudeRef);
    //}

    //public bool GetGpsLatitude(out GeoCoordinate Value) =>
    //    GetGpsCoordinateHelper(out Value, ExifTag.GpsLatitude, ExifTag.GpsLatitudeRef, 'N', 'S');

    //public bool SetGpsLatitude(GeoCoordinate Value) =>
    //    SetGpsCoordinateHelper(Value, ExifTag.GpsLatitude, ExifTag.GpsLatitudeRef, 'N', 'S');

    //public void RemoveGpsLatitude()
    //{
    //    _ = RemoveTag(ExifTag.GpsLatitude);
    //    _ = RemoveTag(ExifTag.GpsLatitudeRef);
    //}

    //public bool GetGpsAltitude(out decimal Value)
    //{
    //    bool Success = false;
    //    if (GetTagValue(ExifTag.GpsAltitude, out ExifRational AltitudeRat) && AltitudeRat.IsValid())
    //    {
    //        Value = ExifRational.ToDecimal(AltitudeRat);
    //        if (GetTagValue(ExifTag.GpsAltitudeRef, out uint BelowSeaLevel) && (BelowSeaLevel == 1))
    //            Value = -Value;

    //        Success = true;
    //    }
    //    else
    //    {
    //        Value = 0;
    //    }

    //    return Success;
    //}

    //public bool SetGpsAltitude(decimal Value)
    //{
    //    bool Success = false;
    //    var AltitudeRat = ExifRational.FromDecimal(Value);
    //    uint BelowSeaLevel = 0;
    //    if (AltitudeRat.IsNegative())
    //    {
    //        BelowSeaLevel = 1;
    //        AltitudeRat.Sign = false; // Remove negative sign from "AltitudeRat"
    //    }

    //    if (SetTagValue(ExifTag.GpsAltitude, AltitudeRat, ExifTagType.URational) &&
    //        SetTagValue(ExifTag.GpsAltitudeRef, BelowSeaLevel, ExifTagType.Byte))
    //    {
    //        Success = true;
    //    }

    //    return Success;
    //}

    //public void RemoveGpsAltitude()
    //{
    //    _ = RemoveTag(ExifTag.GpsAltitude);
    //    _ = RemoveTag(ExifTag.GpsAltitudeRef);
    //}

    //public bool GetGpsDateTimeStamp(out DateTime Value)
    //{
    //    bool Success = false;
    //    if (GetTagValue(ExifTag.GpsDateStamp, out Value, ExifDateFormat.DateOnly))
    //    {
    //        if (GetTagValue(ExifTag.GpsTimeStamp, out ExifRational Hour, 0) && !Hour.IsNegative() && Hour.IsValid() &&
    //            GetTagValue(ExifTag.GpsTimeStamp, out ExifRational Min, 1) && !Min.IsNegative() && Min.IsValid() &&
    //            GetTagValue(ExifTag.GpsTimeStamp, out ExifRational Sec, 2) && !Sec.IsNegative() && Sec.IsValid())
    //        {
    //            Value = Value.AddHours(((double)Hour.Numer) / Hour.Denom);
    //            Value = Value.AddMinutes(((double)Min.Numer) / Min.Denom);
    //            double ms = Math.Truncate((double)Sec.Numer * 1000 / Sec.Denom);
    //            Value = Value.AddMilliseconds(ms);
    //            Success = true;
    //        }
    //        else
    //        {
    //            Value = DateTime.MinValue;
    //        }
    //    }
    //    else
    //    {
    //        Value = DateTime.MinValue;
    //    }

    //    return Success;
    //}

    //public bool SetGpsDateTimeStamp(DateTime Value)
    //{
    //    bool Success = false;
    //    if (SetTagValue(ExifTag.GpsDateStamp, Value.Date, ExifDateFormat.DateOnly))
    //    {
    //        TimeSpan ts = Value.TimeOfDay;
    //        ExifRational Hour = new(ts.Hours, 1);
    //        ExifRational Min = new(ts.Minutes, 1);
    //        int ms = ts.Milliseconds;
    //        ExifRational Sec = ms == 0 ? new ExifRational(ts.Seconds, 1) : new ExifRational((ts.Seconds * 1000) + ms, 1000);

    //        if (SetTagValue(ExifTag.GpsTimeStamp, Hour, ExifTagType.URational, 0) &&
    //            SetTagValue(ExifTag.GpsTimeStamp, Min, ExifTagType.URational, 1) &&
    //            SetTagValue(ExifTag.GpsTimeStamp, Sec, ExifTagType.URational, 2))
    //        {
    //            Success = true;
    //        }
    //    }

    //    return Success;
    //}

    //public void RemoveGpsDateTimeStamp()
    //{
    //    _ = RemoveTag(ExifTag.GpsDateStamp);
    //    _ = RemoveTag(ExifTag.GpsTimeStamp);
    //}

    //private bool GetGpsCoordinateHelper(out GeoCoordinate Value, ExifTag ValueTag, ExifTag RefTag, char Cp1, char Cp2)
    //{
    //    bool Success = false;
    //    Value = new GeoCoordinate();

    //    if (GetTagValue(ValueTag, out ExifRational Deg, 0) && Deg.IsValid() &&
    //        GetTagValue(ValueTag, out ExifRational Min, 1) && Min.IsValid() &&
    //        GetTagValue(ValueTag, out ExifRational Sec, 2) && Sec.IsValid() &&
    //        GetTagValue(RefTag, out string? Ref, StrCoding.Utf8) && (Ref?.Length == 1))
    //    {
    //        char CardinalPoint = Ref[0];
    //        if ((CardinalPoint == Cp1) || (CardinalPoint == Cp2))
    //        {
    //            Value.Degree = ExifRational.ToDecimal(Deg);
    //            Value.Minute = ExifRational.ToDecimal(Min);
    //            Value.Second = ExifRational.ToDecimal(Sec);
    //            Value.CardinalPoint = CardinalPoint;
    //            Success = true;
    //        }
    //    }

    //    return Success;
    //}

    //private bool SetGpsCoordinateHelper(GeoCoordinate Value, ExifTag ValueTag, ExifTag RefTag, char Cp1, char Cp2)
    //{
    //    bool Success = false;
    //    var Deg = ExifRational.FromDecimal(Value.Degree);
    //    var Min = ExifRational.FromDecimal(Value.Minute);
    //    var Sec = ExifRational.FromDecimal(Value.Second);
    //    if (SetTagValue(ValueTag, Deg, ExifTagType.URational, 0) &&
    //        SetTagValue(ValueTag, Min, ExifTagType.URational, 1) &&
    //        SetTagValue(ValueTag, Sec, ExifTagType.URational, 2) &&
    //        ((Value.CardinalPoint == Cp1) || (Value.CardinalPoint == Cp2)) &&
    //        SetTagValue(RefTag, Value.CardinalPoint.ToString(), StrCoding.Utf8))
    //    {
    //        Success = true;
    //    }

    //    return Success;
    //}

    #endregion

    #endregion

    #region Private area

    private const int MinExifBlockLen = 2 + 4; // Minimum number of bytes for an EXIF block: Value for the number of tags of IFD Primary Data (2 bytes) +
                                               // Offset of IFD Thumbnail Data (JPEG) or offset of next EXIF block (TIFF) (4 bytes)
    private const int MaxTagValueCount = int.MaxValue / 8;
    private byte[]? SourceExifBlock; // Temporary storage for the EXIF block in JPEG and PNG images, starting at index 8.
                                     // In index 0 to 7 the TIFF header is stored.
    private Stream SourceExifStream; // Temporary stream from which the EXIF block is read in TIFF images.
    private readonly string? _FileNameWithPath;
    private static readonly int[] TypeByteCount = [
        0, // Type 0: illegal
        1, // Type 1: ExifTagType.Byte
        1, // Type 2: ExifTagType.Ascii, 8 bit character
        2, // Type 3: ExifTagType.UShort
        4, // Type 4: ExifTagType.ULong
        8, // Type 5: ExifTagType.URational
        1, // Type 6: ExifTagType.SByte
        1, // Type 7: ExifTagType.Undefined, 8 bit value
        2, // Type 8: ExifTagType.SShort
        4, // Type 9: ExifTagType.SLong
        8, // Type 10: ExifTagType.SRational
        4, // Type 11: ExifTagType.Float
        8,  // Type 12: ExifTagType.Double
    ];

    private const int IdCodeLength = 8;
    private static readonly byte[] IdCodeUtf16 = [(byte)'U', (byte)'N', (byte)'I', (byte)'C', (byte)'O', (byte)'D', (byte)'E', 0];
    private static readonly byte[] IdCodeAscii = [(byte)'A', (byte)'S', (byte)'C', (byte)'I', (byte)'I', 0, 0, 0];
    private static readonly byte[] IdCodeDefault = [0, 0, 0, 0, 0, 0, 0, 0];

    private Dictionary<ExifTagId, TagItem>[] TagTable;
    private byte[]? ThumbnailImage; // Array, which contains among other data the thumbnail image
    private int ThumbnailStartIndex, ThumbnailByteCount; // Range, in which the thumbnail image in the array "ThumbnailData" is stored

    private const int ExifIfdCount = 5; // Number of elements of the enum type "ExifIfd", i. e. the number of IFDs
    private ExifIfd ExifIfdForTagEnumeration;
    private Dictionary<ExifTagId, TagItem>.KeyCollection.Enumerator TagEnumerator;
    private enum ImageFileBlockState { NonExistent = 0, Removed, Existent };
    //private const ImageFileBlock LastImageFileBlock = ImageFileBlock.JpegComment;
    private readonly ImageFileBlockState[] ImageFileBlockInfo;
    private ExifErrCode ErrCodeForIllegalExifBlock;

    //private int ExifBlockFilePosition = 0;

    // JPEG files
    private const int JpegMaxExifBlockLen = 65534 - 2 - 6 - TiffHeaderLen; // Maximum number of bytes an EXIF block in a JPEG file may have
    private const ushort JpegApp0Marker = 0xFFE0;
    private const ushort JpegApp1Marker = 0xFFE1;
    //private const ushort JpegApp2Marker = 0xFFE2;
    private const ushort JpegApp13Marker = 0xFFED;
    //private const ushort JpegApp14Marker = 0xFFEE;
    private const ushort JpegCommentMarker = 0xFFFE; // Start of JPEG comment block
    private const ushort JpegSoiMarker = 0xFFD8; // Start of image (SOI) marker
    private const ushort JpegSosMarker = 0xFFDA; // Start of scan (SOS) marker
    private static readonly byte[] JpegExifSignature = [(byte)'E', (byte)'x', (byte)'i', (byte)'f', 0, 0];
    private static readonly byte[] JpegXmpInitialSignature = [ (byte)'h', (byte)'t', (byte)'t', (byte)'p', (byte)':',
      (byte)'/', (byte)'/', (byte)'n', (byte)'s', (byte)'.', (byte)'a', (byte)'d', (byte)'o', (byte)'b',
      (byte)'e', (byte)'.', (byte)'c', (byte)'o', (byte)'m', (byte)'/' ];
    private static readonly byte[] JpegIptcSignature = [ (byte)'P', (byte)'h', (byte)'o', (byte)'t', (byte)'o',
      (byte)'s', (byte)'h', (byte)'o', (byte)'p', (byte)' ', (byte)'3', (byte)'.', (byte)'0', 0 ];

    // TIFF files
    private const int TiffHeaderLen = 8; // Size of TIFF header. The TIFF header is also present in EXIF blocks of JPEG and PNG files.
    private const uint TiffHeaderSignatureLE = 0x49492A00;
    private const uint TiffHeaderSignatureBE = 0x4D4D002A;

    // PNG files
    private const uint PngHeaderPart1 = 0x89504E47;
    private const uint PngHeaderPart2 = 0x0D0A1A0A;
    private const uint PngIhdrChunk = ((uint)'I' << 24) | ((uint)'H' << 16) | ((uint)'D' << 8) | 'R';
    private const uint PngExifChunk = ((uint)'e' << 24) | ((uint)'X' << 16) | ((uint)'I' << 8) | 'f';
    private const uint PngItxtChunk = ((uint)'i' << 24) | ((uint)'T' << 16) | ((uint)'X' << 8) | 't';
    private const uint PngTextChunk = ((uint)'t' << 24) | ((uint)'E' << 16) | ((uint)'X' << 8) | 't';
    private const uint PngTimeChunk = ((uint)'t' << 24) | ((uint)'I' << 16) | ((uint)'M' << 8) | 'E';
    private const uint PngIendChunk = ((uint)'I' << 24) | ((uint)'E' << 16) | ((uint)'N' << 8) | 'D';
    private static readonly byte[] PngXmpSignature = [ (byte)'X', (byte)'M', (byte)'L', (byte)':', (byte)'c',
      (byte)'o', (byte)'m', (byte)'.', (byte)'a', (byte)'d', (byte)'o', (byte)'b', (byte)'e', (byte)'.',
      (byte)'x', (byte)'m', (byte)'p', 0 ];
    private static readonly byte[] PngIptcSignature = [ (byte)'R', (byte)'a', (byte)'w', (byte)' ', (byte)'p',
      (byte)'r', (byte)'o', (byte)'f', (byte)'i', (byte)'l', (byte)'e', (byte)' ', (byte)'t', (byte)'y',
      (byte)'p', (byte)'e', (byte)' ', (byte)'i', (byte)'p', (byte)'t', (byte)'c', 0 ];
    private static readonly int ImageFileBlockCount = Enum.GetValues<ImageFileBlock>().Length;

    private ExifService()
    {
        ByteOrder = ExifByteOrder.BigEndian;
        ImageFileBlockInfo = new ImageFileBlockState[ImageFileBlockCount];
        TagTable = [];
        SourceExifStream = Stream.Null;
    }

    private void ReadFromStream(Stream imageStream, ExifLoadOptions options)
    {
        SourceExifStream = imageStream;
        ErrCodeForIllegalExifBlock = ExifErrCode.ExifBlockHasIllegalContent;
        ImageType = CheckStreamTypeAndCompatibility(SourceExifStream);
        if (ImageType == ImageType.Jpeg)
            ReadJepg(options);
        else if (ImageType == ImageType.Tiff)
            ReadTiff();
        else if (ImageType == ImageType.Png)
            ReadPng(options);

        UpdateImageFileBlockInfo();

        SourceExifBlock = null;
        SourceExifStream = Stream.Null;
    }

    /// <summary>
    /// Read JPEG file from stream "SourceExifStream".
    /// </summary>
    /// <param name="options">Load options.</param>
    /// <exception cref="ExifException"></exception>
    private void ReadJepg(ExifLoadOptions options)
    {
        byte[] BlockContent = new byte[65536];
        ushort BlockMarker;
        bool ExifBlockFound = false;
        SourceExifStream.Position = 2;
        int TiffHeaderAndExifBlockLen = 0;
        do
        {
            ReadJpegBlock(SourceExifStream, BlockContent, out int BlockContentSize, out BlockMarker,
              out ImageFileBlock BlockType, out int MetaDataIndex);
            if (BlockType != ImageFileBlock.Unknown)
            {
                if ((BlockType == ImageFileBlock.Exif) && (!ExifBlockFound))
                {
                    ExifBlockFound = true;
                    TiffHeaderAndExifBlockLen = BlockContentSize - MetaDataIndex;
                    SourceExifBlock = new byte[TiffHeaderAndExifBlockLen];
                    Array.Copy(BlockContent, MetaDataIndex, SourceExifBlock, 0, TiffHeaderAndExifBlockLen);
                }

                ImageFileBlockInfo[(int)BlockType] = ImageFileBlockState.Existent;
            }
        }
        while (BlockMarker != JpegSosMarker);

        if (ExifBlockFound)
        {
            try
            {
                EvaluateTiffHeader(SourceExifBlock!, TiffHeaderAndExifBlockLen, out int IfdPrimaryDataOffset);
                _ = EvaluateExifBlock(IfdPrimaryDataOffset);
            }
            catch
            {
                if (!options.HasFlag(ExifLoadOptions.EmptyIfExifBlockIsIllegal))
                    throw;

                SetEmptyExifBlock();
                ExifBlockWasIllegal = true;
            }
        }
        else
        {
            SetEmptyExifBlock();
        }
    }

    /// <summary>
    /// Read TIFF file from stream "SourceExifStream".
    /// </summary>
    /// <exception cref="ExifException"></exception>
    private void ReadTiff()
    {
        byte[] BlockContent = new byte[8];
        SourceExifStream.Position = 0;
        SourceExifBlock = null;
        ErrCodeForIllegalExifBlock = ExifErrCode.InternalImageStructureIsWrong;
        int TiffHeaderBytesRead = SourceExifStream.Read(BlockContent, 0, TiffHeaderLen);
        EvaluateTiffHeader(BlockContent, TiffHeaderBytesRead, out int IfdPrimaryDataOffset);
        _ = EvaluateExifBlock(IfdPrimaryDataOffset);
    }

    /// <summary>
    /// Read PNG file from stream "SourceExifStream".
    /// </summary>
    /// <param name="options">Load options.</param>
    /// <exception cref="ExifException"></exception>
    private void ReadPng(ExifLoadOptions options)
    {
        byte[] TempData = new byte[65536];
        SourceExifStream.Position = 4;
        int k = SourceExifStream.Read(TempData, 0, 4);
        if ((k != 4) || (ReadUInt32BE(TempData, 0) != PngHeaderPart2))
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

        bool ExifBlockFound = false;
        uint ChunkType;
        byte[] BlockContent = new byte[30];
        int TiffHeaderAndExifBlockLen = 0;
        uint ExifCrc32Calculated = 0, ExifCrc32Stored = 0;
        do
        {
            ReadPngBlockHeader(SourceExifStream, TempData, out int DataLength, out ChunkType);
            int BytesRead = DetectPngImageBlock(SourceExifStream, BlockContent, ChunkType, out ImageFileBlock BlockType);
            if (BlockType != ImageFileBlock.Unknown)
            {
                if ((BlockType == ImageFileBlock.Exif) && (!ExifBlockFound))
                {
                    ExifBlockFound = true;
                    TiffHeaderAndExifBlockLen = DataLength;
                    SourceExifBlock = new byte[DataLength];
                    if (SourceExifStream.Read(SourceExifBlock, 0, DataLength) != DataLength)
                        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

                    uint ChunkCrc32 = CalculateCrc32(TempData, 4, 4, false); // Calculate CRC32 of chunk type
                    ExifCrc32Calculated = CalculateCrc32(SourceExifBlock, 0, DataLength, true, ChunkCrc32);
                    if (SourceExifStream.Read(TempData, 0, 4) != 4)
                        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

                    ExifCrc32Stored = ReadUInt32BE(TempData, 0);
                    BytesRead = DataLength + 4;
                }

                ImageFileBlockInfo[(int)BlockType] = ImageFileBlockState.Existent;
            }

            SourceExifStream.Position += DataLength + 4 - BytesRead;
        }
        while (ChunkType != PngIendChunk);

        if (ExifBlockFound)
        {
            try
            {
                if (ExifCrc32Calculated != ExifCrc32Stored)
                    throw new ExifException(ExifErrCode.ExifBlockHasIllegalContent);

                EvaluateTiffHeader(SourceExifBlock!, TiffHeaderAndExifBlockLen, out int IfdPrimaryDataOffset);
                _ = EvaluateExifBlock(IfdPrimaryDataOffset);
            }
            catch
            {
                if (!options.HasFlag(ExifLoadOptions.EmptyIfExifBlockIsIllegal))
                    throw;

                SetEmptyExifBlock();
                ExifBlockWasIllegal = true;
            }
        }
        else
        {
            SetEmptyExifBlock();
        }
    }

    /// <summary>
    /// Read block length and chunk type of an PNG image stream.
    /// </summary>
    /// <param name="imageStream">PNG image stream.</param>
    /// <param name="tempData">In: Allocated array with a length of 8 bytes. Out: Block length and
    /// chunk type as raw data.</param>
    /// <param name="blockLength">Out: Block length as 32 bit number.</param>
    /// <param name="chunkType">Out: Chunk typeas 32 bit number.</param>
    /// <exception cref="ExifException"></exception>
    private static void ReadPngBlockHeader(Stream imageStream, byte[] tempData, out int blockLength, out uint chunkType)
    {
        if (imageStream.Read(tempData, 0, 8) < 8)
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

        blockLength = (int)ReadUInt32BE(tempData, 0);
        if (blockLength < 0)
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

        chunkType = ReadUInt32BE(tempData, 4);
    }

    private static int DetectPngImageBlock(Stream imageStream, byte[] signatureData, uint chunkType, out ImageFileBlock blockType)
    {
        int BytesRead = 0;
        blockType = ImageFileBlock.Unknown;
        if (chunkType == PngExifChunk)
        {
            blockType = ImageFileBlock.Exif;
        }
        else if (chunkType == PngItxtChunk)
        {
            int BytesToRead = PngIptcSignature.Length;
            BytesRead = imageStream.Read(signatureData, 0, BytesToRead);
            if (ArrayStartsWith(signatureData, BytesRead, PngXmpSignature))
                blockType = ImageFileBlock.Xmp;
            else if (ArrayStartsWith(signatureData, BytesRead, PngIptcSignature))
                blockType = ImageFileBlock.Iptc;
        }
        else if (chunkType == PngTextChunk)
        {
            blockType = ImageFileBlock.PngMetaData;
        }
        else if (chunkType == PngTimeChunk)
        {
            blockType = ImageFileBlock.PngDateChanged;
        }

        return BytesRead;
    }

    // CRC32 checksum table for polynomial 0xEDB88320
    private static readonly uint[] Crc32ChecksumTable = [ 0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F,
      0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
      0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856, 0x646BA8C0,
      0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172,
      0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B, 0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75,
      0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
      0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190, 0x01DB7106,
      0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818,
      0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01, 0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B,
      0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
      0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A, 0x346ED9FC,
      0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086,
      0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F, 0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81,
      0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
      0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344, 0x8708A3D2,
      0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC,
      0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5, 0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767,
      0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
      0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE, 0xB2BD0B28,
      0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A,
      0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713, 0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D,
      0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
      0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278, 0xD70DD2EE,
      0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0,
      0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9, 0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693,
      0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D ];

    private static uint CalculateCrc32(byte[] data, int startIndex, int length, bool finalize = true, uint startCrc = 0xFFFFFFFF)
    {
        uint result = startCrc;
        int EndIndexPlus1 = startIndex + length;
        for (int i = startIndex; i < EndIndexPlus1; i++)
        {
            byte value = data[i];
            result = Crc32ChecksumTable[((byte)result) ^ value] ^ (result >> 8);
        }

        if (finalize)
            result = ~result;

        return result;
    }

    private void UpdateImageFileBlockInfo()
    {
        ImageFileBlockState NewExifBlockState = ImageFileBlockState.NonExistent;
        Dictionary<ExifTagId, TagItem> PrimaryDataIfdTable = TagTable[(uint)ExifIfd.PrimaryData];
        foreach (ExifTagId tid in PrimaryDataIfdTable.Keys)
        {
            if (ImageType == ImageType.Tiff)
            {
                if (!IsInternalTiffTag(tid) && !IsMetaDataTiffTag(tid))
                {
                    NewExifBlockState = ImageFileBlockState.Existent;
                    break;
                }
            }
            else
            {
                NewExifBlockState = ImageFileBlockState.Existent;
                break;
            }
        }

        if (NewExifBlockState == ImageFileBlockState.NonExistent)
        {
            ExifIfd[] TempIfds = [ExifIfd.PrivateData, ExifIfd.GpsInfoData, ExifIfd.Interoperability, ExifIfd.ThumbnailData];
            foreach (ExifIfd Ifd in TempIfds)
            {
                if (TagTable[(uint)Ifd].Count > 0)
                {
                    NewExifBlockState = ImageFileBlockState.Existent;
                    break;
                }
            }
        }

        ImageFileBlockInfo[(int)ImageFileBlock.Exif] = NewExifBlockState;

        if (ImageType == ImageType.Tiff)
        {
            // In TIFF images the XMP and IPTC block are stored as tags within the EXIF block
            ImageFileBlockInfo[(int)ImageFileBlock.Xmp] = PrimaryDataIfdTable.ContainsKey(ExifTagId.XmpMetadata)
                ? ImageFileBlockState.Existent
                : ImageFileBlockState.NonExistent;

            ImageFileBlockInfo[(int)ImageFileBlock.Iptc] = PrimaryDataIfdTable.ContainsKey(ExifTagId.IptcMetadata)
                ? ImageFileBlockState.Existent
                : ImageFileBlockState.NonExistent;
        }
    }

    private static void ReadJpegBlockMarker(Stream imageStream, out ushort blockMarker, out int blockContentSize)
    {
        byte[] TempBuffer = new byte[2];
        if (imageStream.Read(TempBuffer, 0, 2) != 2)
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

        blockMarker = ReadUInt16BE(TempBuffer, 0);
        if (blockMarker is 0xFF01 or (>= 0xFFD0 and <= 0xFFDA))
        {
            // JPEG block does not have a size specification
            blockContentSize = 0;
        }
        else if (blockMarker == 0xFFFF)
        {
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
        }
        else
        {
            if (imageStream.Read(TempBuffer, 0, 2) != 2)
                throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

            blockContentSize = ReadUInt16BE(TempBuffer, 0) - 2;

            if (blockContentSize < 0)
                throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
        }
    }

    /// <summary>
    /// Read a JPEG block from a JPEG image stream. If the JPEG block has a size specification, the block content is read and returned.
    /// </summary>
    /// <param name="ImageStream">JPEG image stream.</param>
    /// <param name="BlockContent">In: Allocated array with a size of at least 65536 bytes.
    /// Out: If the JPEG block has a size specification, the content of the JPEG block is returned here.
    /// Otherwise "BlockContent" remains unchanged.</param>
    /// <param name="BlockContentSize">Out: Size of block content in bytes. If the JPEG block doesn't
    /// have a size specification, "BlockContentSize" is set to 0.</param>
    /// <param name="BlockMarker">Out: Block marker.</param>
    /// <param name="BlockType">Out: Block type.</param>
    /// <param name="MetaDataIndex">Out: Index of meta data.</param>
    /// <exception cref="ExifException"></exception>
    private static void ReadJpegBlock(
        Stream ImageStream,
        byte[] BlockContent,
        out int BlockContentSize,
        out ushort BlockMarker,
        out ImageFileBlock BlockType,
        out int MetaDataIndex)
    {
        BlockType = ImageFileBlock.Unknown;
        MetaDataIndex = 0;
        ReadJpegBlockMarker(ImageStream, out BlockMarker, out int ContentSize);
        if (ContentSize > 0)
        {
            int k = ImageStream.Read(BlockContent, 0, ContentSize);
            if (k != ContentSize)
                throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

            if (BlockMarker == JpegApp1Marker)
            {
                if (ArrayStartsWith(BlockContent, ContentSize, JpegExifSignature))
                {
                    BlockType = ImageFileBlock.Exif;
                    MetaDataIndex = JpegExifSignature.Length;
                }
                else if (ArrayStartsWith(BlockContent, ContentSize, JpegXmpInitialSignature))
                {
                    BlockType = ImageFileBlock.Xmp;
                    int i = JpegXmpInitialSignature.Length;
                    while ((i < ContentSize) && (BlockContent[i] != 0))
                        i++;

                    MetaDataIndex = i + 1;
                }
            }
            else if (BlockMarker == JpegApp13Marker)
            {
                if (ArrayStartsWith(BlockContent, ContentSize, JpegIptcSignature))
                {
                    BlockType = ImageFileBlock.Iptc;
                    MetaDataIndex = JpegIptcSignature.Length;
                }
            }
            else if (BlockMarker == JpegCommentMarker)
            {
                BlockType = ImageFileBlock.JpegComment;
            }
        }

        BlockContentSize = ContentSize;
    }

    /// <summary>
    /// Read the first 4 bytes from an image stream and identify the image type .
    /// </summary>
    /// <param name="streamToBeChecked">Image stream to be read.</param>
    /// <returns>Image type of the stream.</returns>
    /// <exception cref="ExifException"></exception>
    private static ImageType CheckStreamTypeAndCompatibility(Stream streamToBeChecked)
    {
        byte[] TempBuffer = new byte[4];
        if (streamToBeChecked.Read(TempBuffer, 0, 4) != 4)
            throw new ExifException(ExifErrCode.ImageTypeIsNotSupported);

        uint ImageSignature = ReadUInt32BE(TempBuffer, 0);
        ImageType StreamImageType = (ImageSignature >> 16) switch
        {
            JpegSoiMarker => ImageType.Jpeg,
            _ => ImageSignature is TiffHeaderSignatureLE or TiffHeaderSignatureBE
                ? ImageType.Tiff
                : ImageSignature == PngHeaderPart1
                    ? ImageType.Png
                    : throw new ExifException(ExifErrCode.ImageTypeIsNotSupported),
        };

        // Check stream size
        if (streamToBeChecked.Length > int.MaxValue)
            throw new ExifException(ExifErrCode.ImageHasUnsupportedFeatures);

        // Check if stream is seekable
        const long CheckOffset = -2;
        long i = streamToBeChecked.Position;
        streamToBeChecked.Position += CheckOffset;
        if (streamToBeChecked.Position != (i + CheckOffset))
            // The stream for reading the image data must support the property "Position"
            throw new ExifException(ExifErrCode.ImageHasUnsupportedFeatures);

        streamToBeChecked.Position = i;

        return StreamImageType;
    }

    private void EvaluateTiffHeader(byte[] tiffHeader, int tiffHeaderBytesRead, out int ifdPrimaryDataOffset)
    {
        if (tiffHeaderBytesRead < TiffHeaderLen)
            throw new ExifException(ErrCodeForIllegalExifBlock);

        uint TiffHeaderSignature = ReadUInt32BE(tiffHeader, 0);
        ByteOrder = TiffHeaderSignature == TiffHeaderSignatureLE
            ? ExifByteOrder.LittleEndian
            : TiffHeaderSignature == TiffHeaderSignatureBE ? ExifByteOrder.BigEndian : throw new ExifException(ErrCodeForIllegalExifBlock);

        ifdPrimaryDataOffset = (int)ExifReadUInt32(tiffHeader, 4);
        if (ifdPrimaryDataOffset < TiffHeaderLen)
            throw new ExifException(ErrCodeForIllegalExifBlock);
    }

    private void SaveJpeg(Stream sourceStream, Stream destStream)
    {
        const long FirstBlockStartPosition = 2;
        ushort BlockMarker;
        byte[] BlockContent = new byte[65536];
        byte[] TempBuffer = new byte[4];

        sourceStream.Position = FirstBlockStartPosition;
        WriteUInt16BE(TempBuffer, 0, JpegSoiMarker);
        destStream.Write(TempBuffer, 0, 2);

        // Copy all APP0 blocks of the source file. These blocks should be the JFIF blocks.
        do
        {
            ReadJpegBlockMarker(sourceStream, out BlockMarker, out int BlockContentSize);
            if (BlockMarker == JpegSosMarker)
                break; // Start of JPEG image matrix reached

            long NextBlockStartPosition = sourceStream.Position + BlockContentSize;
            if (BlockMarker == JpegApp0Marker)
            {
                WriteUInt16BE(TempBuffer, 0, BlockMarker);
                WriteUInt16BE(TempBuffer, 2, (ushort)(BlockContentSize + 2));
                destStream.Write(TempBuffer, 0, 4);
                if (sourceStream.Read(BlockContent, 0, BlockContentSize) != BlockContentSize)
                    throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

                destStream.Write(BlockContent, 0, BlockContentSize);
            }

            sourceStream.Position = NextBlockStartPosition;
        }
        while (true);

        // Write new EXIF block if the new EXIF block is not empty
        _ = CreateExifBlock(out FlexArray NewExifBlock, TiffHeaderLen);
        int NewExifBlockLen = NewExifBlock.Length;
        if (NewExifBlockLen > JpegMaxExifBlockLen)
            throw new ExifException(ExifErrCode.ExifDataAreTooLarge);

        if (NewExifBlockLen > 0)
        {
            WriteUInt16BE(TempBuffer, 0, JpegApp1Marker);
            WriteUInt16BE(TempBuffer, 2, (ushort)(2 + 6 + TiffHeaderLen + NewExifBlockLen));
            destStream.Write(TempBuffer, 0, 4);
            destStream.Write(JpegExifSignature, 0, JpegExifSignature.Length);
            CreateTiffHeader(out byte[] TiffHeader, TiffHeaderLen);
            destStream.Write(TiffHeader, 0, TiffHeader.Length);
            destStream.Write(NewExifBlock.Buffer, 0, NewExifBlockLen);
        }

        // Return to the first block of the image
        sourceStream.Position = FirstBlockStartPosition;
        do
        {
            ReadJpegBlock(sourceStream, BlockContent, out int BlockContentSize, out BlockMarker, out ImageFileBlock BlockType, out _);
            if (BlockMarker == JpegSosMarker)
            {
                // Start of JPEG image matrix reached. Copy all remaining data from source stream to destination stream
                // without further interpretation.
                WriteUInt16BE(TempBuffer, 0, BlockMarker);
                destStream.Write(TempBuffer, 0, 2);
                int k;
                do
                {
                    k = sourceStream.Read(BlockContent, 0, BlockContent.Length);
                    destStream.Write(BlockContent, 0, k);
                }
                while (k == BlockContent.Length);
                break; // Destination stream was completely written
            }

            bool CopyBlockFromSourceStream = true;
            if (BlockMarker == JpegApp0Marker)
                CopyBlockFromSourceStream = false; // All APP0 blocks have already been copied
            else if (BlockType == ImageFileBlock.Exif)
                CopyBlockFromSourceStream = false; // EXIF block has already been written
            else if ((BlockType == ImageFileBlock.Xmp) && (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Removed))
                CopyBlockFromSourceStream = false;
            else if ((BlockType == ImageFileBlock.Iptc) && (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Removed))
                CopyBlockFromSourceStream = false;
            else if ((BlockType == ImageFileBlock.JpegComment) && (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Removed))
                CopyBlockFromSourceStream = false;

            if (CopyBlockFromSourceStream)
            {
                WriteUInt16BE(TempBuffer, 0, BlockMarker);
                WriteUInt16BE(TempBuffer, 2, (ushort)(BlockContentSize + 2));
                destStream.Write(TempBuffer, 0, 4);
                destStream.Write(BlockContent, 0, BlockContentSize);
            }
        }
        while (true);
    }

    private void SaveTiff(Stream sourceStream, Stream destStream)
    {
        sourceStream.Position = 0;
        byte[] TiffHeader = new byte[TiffHeaderLen];
        if (sourceStream.Read(TiffHeader, 0, TiffHeaderLen) != TiffHeaderLen)
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

        ExifByteOrder SourceStreamByteOrder = ExifByteOrder.LittleEndian;
        uint TiffHeaderSignature = ReadUInt32BE(TiffHeader, 0);
        if (TiffHeaderSignature == TiffHeaderSignatureBE)
            SourceStreamByteOrder = ExifByteOrder.BigEndian;

        if (ByteOrder != SourceStreamByteOrder)
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

        int SourceStreamExifBlockOffset = (int)ExifReadUInt32(TiffHeader, 4);
        uint CurrentDestStreamOffset = TiffHeaderLen;

        // TIFF file structure when saving a multi-page TIFF file:
        //
        // --------------------------------------------
        // TIFF Header (8 bytes)
        // --------------------------------------------
        // Image 1 matrix
        // --------------------------------------------
        // Image 1 EXIF block
        // --------------------------------------------
        // Image 2 matrix
        // --------------------------------------------
        // Image 2 EXIF block
        // --------------------------------------------
        // ...

        int ImageNumber = 0, NextExifBlockPointerIndex = 0;
        byte[] TempBuffer = new byte[65536];
        FlexArray? ExifBlockAsBinaryData = null;
        do
        {
            ExifService CurrentImageExif = CreateSubExifData(
                tiffStream: sourceStream,
                exifBlockOffset: ref SourceStreamExifBlockOffset,
                byteOrder: SourceStreamByteOrder,
                segmentOffsetTable: out uint[] SegmentOffsetTable,
                segmentByteCountTable: out uint[] SegmentByteCountTable,
                segmentOffsetsTag: out ExifTag SegmentOffsetsTag,
                segmentByteCountsTag: out ExifTag SegmentByteCountsTag);

            if (ImageNumber == 0)
                CurrentImageExif = this; // Discard the EXIF block returned by "CreateSubExifData"

            if (!CurrentImageExif.TagExists(ExifTag.ImageWidth) || !CurrentImageExif.TagExists(ExifTag.ImageLength))
                throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

            _ = CurrentImageExif.RemoveTag(ExifTag.FreeOffsets); // Remove tags for areas of unused bytes
            _ = CurrentImageExif.RemoveTag(ExifTag.FreeByteCounts);

            // Assign new segment offsets.
            int ImageSegmentCount = SegmentOffsetTable.Length;
            _ = CurrentImageExif.SetTagValueCount(SegmentOffsetsTag, ImageSegmentCount, ExifTagType.ULong);
            _ = CurrentImageExif.SetTagValueCount(SegmentByteCountsTag, ImageSegmentCount, ExifTagType.ULong);
            for (int k = 0; k < ImageSegmentCount; k++)
            {
                _ = CurrentImageExif.SetTagValue(SegmentOffsetsTag, CurrentDestStreamOffset, ExifTagType.ULong, k);
                uint SegmentSize = SegmentByteCountTable[k];
                _ = CurrentImageExif.SetTagValue(SegmentByteCountsTag, SegmentSize, ExifTagType.ULong, k);
                CurrentDestStreamOffset += SegmentSize;
            }

            bool HasImageMatrixFillByte = false;
            if ((CurrentDestStreamOffset & 0x1) != 0)
            {
                CurrentDestStreamOffset++;
                HasImageMatrixFillByte = true;
            }

            if (CurrentDestStreamOffset > int.MaxValue)
                throw new ExifException(ExifErrCode.ExifDataAreTooLarge);

            if (ImageNumber == 0)
            {
                // Write TIFF header to destination stream
                CurrentImageExif.ExifWriteUInt32(TiffHeader, 4, CurrentDestStreamOffset);
                destStream.Write(TiffHeader, 0, TiffHeaderLen);
            }
            else
            {
                // Insert the the pointer to the EXIF block of the current image into the binary data of the EXIF block of
                // the previous image. Then write the previous EXIF block to destination stream.
                CurrentImageExif.ExifWriteUInt32(ExifBlockAsBinaryData!.Buffer, NextExifBlockPointerIndex, CurrentDestStreamOffset);
                destStream.Write(ExifBlockAsBinaryData.Buffer, 0, ExifBlockAsBinaryData.Length);
            }

            // Write image matrix: copy image segments from source to destination stream
            for (int k = 0; k < ImageSegmentCount; k++)
            {
                int RemainingByteCount = (int)SegmentByteCountTable[k];
                int BytesToRead = TempBuffer.Length;
                sourceStream.Position = SegmentOffsetTable[k];
                do
                {
                    if (BytesToRead > RemainingByteCount)
                        BytesToRead = RemainingByteCount;
                    if (sourceStream.Read(TempBuffer, 0, BytesToRead) != BytesToRead)
                        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);

                    destStream.Write(TempBuffer, 0, BytesToRead);
                    RemainingByteCount -= BytesToRead;
                }
                while (RemainingByteCount > 0);
            }

            if (HasImageMatrixFillByte)
                destStream.WriteByte(0);

            NextExifBlockPointerIndex = CurrentImageExif.CreateExifBlock(out ExifBlockAsBinaryData, (int)CurrentDestStreamOffset);
            CurrentDestStreamOffset += (uint)ExifBlockAsBinaryData.Length;
            if (CurrentDestStreamOffset > int.MaxValue)
                throw new ExifException(ExifErrCode.ExifDataAreTooLarge);

            ImageNumber++;
        }
        while (SourceStreamExifBlockOffset != 0);

        destStream.Write(ExifBlockAsBinaryData.Buffer, 0, ExifBlockAsBinaryData.Length);
    }

    private static ExifService CreateSubExifData(
        Stream tiffStream,
        ref int exifBlockOffset,
        ExifByteOrder byteOrder,
        out uint[] segmentOffsetTable,
        out uint[] segmentByteCountTable,
        out ExifTag segmentOffsetsTag,
        out ExifTag segmentByteCountsTag)
    {
        ExifService SubExifBlock = new()
        {
            ByteOrder = byteOrder,
            ImageType = ImageType.Tiff,
            SourceExifBlock = null,
            SourceExifStream = tiffStream
        };

        exifBlockOffset = SubExifBlock.EvaluateExifBlock(exifBlockOffset);
        if (SubExifBlock.GetTagItem(ExifTag.StripOffsets, out TagItem? SegmentOffsetsTagItem) &&
            SubExifBlock.GetTagItem(ExifTag.StripByteCounts, out TagItem? SegmentByteCountsTagItem))
        {
            segmentOffsetsTag = ExifTag.StripOffsets;
            segmentByteCountsTag = ExifTag.StripByteCounts;
        }
        else if (SubExifBlock.GetTagItem(ExifTag.TileOffsets, out SegmentOffsetsTagItem) &&
                 SubExifBlock.GetTagItem(ExifTag.TileByteCounts, out SegmentByteCountsTagItem))
        {
            segmentOffsetsTag = ExifTag.TileOffsets;
            segmentByteCountsTag = ExifTag.TileByteCounts;
        }
        else
        {
            throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
        }

        int valueCount = SegmentOffsetsTagItem.ValueCount;
        segmentOffsetTable = new uint[valueCount];
        segmentByteCountTable = new uint[valueCount];
        bool IsOffsetTable16Bit = SegmentOffsetsTagItem.TagType == ExifTagType.UShort;
        bool ByteCountTable16Bit = SegmentByteCountsTagItem.TagType == ExifTagType.UShort;
        uint v;
        for (int i = 0; i < valueCount; i++)
        {
            v = IsOffsetTable16Bit
                ? SubExifBlock.ExifReadUInt16(SegmentOffsetsTagItem.ValueData, SegmentOffsetsTagItem.ValueIndex + (i << 1))
                : SubExifBlock.ExifReadUInt32(SegmentOffsetsTagItem.ValueData, SegmentOffsetsTagItem.ValueIndex + (i << 2));
            segmentOffsetTable[i] = v;
            v = ByteCountTable16Bit
                ? SubExifBlock.ExifReadUInt16(SegmentByteCountsTagItem.ValueData, SegmentByteCountsTagItem.ValueIndex + (i << 1))
                : SubExifBlock.ExifReadUInt32(SegmentByteCountsTagItem.ValueData, SegmentByteCountsTagItem.ValueIndex + (i << 2));
            segmentByteCountTable[i] = v;
        }

        return SubExifBlock;
    }

    private void SavePng(Stream sourceStream, Stream destStream)
    {
        uint ChunkType;
        byte[] TempData = new byte[65536];
        byte[] BlockContent = new byte[30]; // Used for signature data at the beginning of the block

        sourceStream.Position = 8;
        WriteUInt32BE(TempData, 0, PngHeaderPart1);
        WriteUInt32BE(TempData, 4, PngHeaderPart2);
        destStream.Write(TempData, 0, 8);
        do
        {
            bool CopyBlockFromSourceStream = true;
            long BlockStartStreamPos = sourceStream.Position;
            ReadPngBlockHeader(sourceStream, TempData, out int DataLength, out ChunkType);
            _ = DetectPngImageBlock(sourceStream, BlockContent, ChunkType, out ImageFileBlock BlockType);
            if (BlockType != ImageFileBlock.Unknown)
            {
                if ((BlockType == ImageFileBlock.Exif) || (ImageFileBlockInfo[(int)BlockType] == ImageFileBlockState.Removed))
                {
                    CopyBlockFromSourceStream = false; // EXIF block will always be replaced by a new EXIF block
                }
            }

            if (CopyBlockFromSourceStream)
            {
                // Copy current block from source to destination stream
                sourceStream.Position = BlockStartStreamPos;
                int RemainingByteCount = DataLength + 12;
                int BytesToRead = TempData.Length;
                do
                {
                    if (BytesToRead > RemainingByteCount)
                        BytesToRead = RemainingByteCount;
                    if (sourceStream.Read(TempData, 0, BytesToRead) == BytesToRead)
                    {
                        destStream.Write(TempData, 0, BytesToRead);
                        RemainingByteCount -= BytesToRead;
                    }
                    else
                    {
                        throw new ExifException(ExifErrCode.InternalImageStructureIsWrong);
                    }
                }
                while (RemainingByteCount > 0);
            }
            else
            {
                sourceStream.Position = BlockStartStreamPos + DataLength + 12;
            }

            if (ChunkType == PngIhdrChunk)
            {
                // Write new EXIF block after the IHDR chunk if the new EXIF block is not empty
                _ = CreateExifBlock(out FlexArray NewExifBlock, TiffHeaderLen);
                int NewExifBlockLen = NewExifBlock.Length;
                if (NewExifBlockLen > 0)
                {
                    CreateTiffHeader(out byte[] TiffHeader, TiffHeaderLen);
                    uint BlockLength = TiffHeaderLen + (uint)NewExifBlockLen;
                    WriteUInt32BE(TempData, 0, BlockLength);
                    WriteUInt32BE(TempData, 4, PngExifChunk);
                    destStream.Write(TempData, 0, 8);
                    destStream.Write(TiffHeader, 0, TiffHeaderLen);
                    destStream.Write(NewExifBlock.Buffer, 0, NewExifBlockLen);

                    uint Crc32 = CalculateCrc32(TempData, 4, 4, false); // Checksum over chunk type
                    Crc32 = CalculateCrc32(TiffHeader, 0, TiffHeaderLen, false, Crc32);
                    Crc32 = CalculateCrc32(NewExifBlock.Buffer, 0, NewExifBlockLen, true, Crc32);
                    WriteUInt32BE(TempData, 0, Crc32);
                    destStream.Write(TempData, 0, 4);
                }
            }
        }
        while (ChunkType != PngIendChunk);
    }

    private void SetEmptyExifBlock()
    {
        // The TIFF header, which is stored in the array elements from 0 to 7, will not be initialized
        // because it is not used any more.
        SourceExifBlock = new byte[TiffHeaderLen + MinExifBlockLen];
        ExifWriteUInt16(SourceExifBlock, TiffHeaderLen, 0); // Number of tags of IFD Primary Data
        ExifWriteUInt32(SourceExifBlock, TiffHeaderLen + 2, 0); // Offset of IFD Thumbnail Data
        _ = EvaluateExifBlock(TiffHeaderLen);
    }

    private void WriteTagToFlexArray(TagItem tempTagData, FlexArray writeData, ref int tagDataIndex, int exifBlockOffset)
    {
        int i = tagDataIndex;
        tagDataIndex += 12;
        ExifWriteUInt16(writeData.Buffer, i, (ushort)tempTagData.TagId);
        ExifWriteUInt16(writeData.Buffer, i + 2, (ushort)tempTagData.TagType);
        ExifWriteUInt32(writeData.Buffer, i + 4, (uint)tempTagData.ValueCount);
        int ByteCount = GetTagByteCount(tempTagData.TagType, tempTagData.ValueCount);
        if (ByteCount <= 4)
        {
            // The tag does not have outsourced data. In this case exactly 4 bytes have to be written.
            writeData.Buffer[i + 8] = tempTagData.ValueData[tempTagData.ValueIndex];
            byte v = 0;
            if (ByteCount >= 2)
                v = tempTagData.ValueData[tempTagData.ValueIndex + 1];
            writeData.Buffer[i + 9] = v;
            v = 0;
            if (ByteCount >= 3)
                v = tempTagData.ValueData[tempTagData.ValueIndex + 2];
            writeData.Buffer[i + 10] = v;
            v = 0;
            if (ByteCount >= 4)
                v = tempTagData.ValueData[tempTagData.ValueIndex + 3];
            writeData.Buffer[i + 11] = v;
        }
        else
        {
            // The tag has outsourced data. The outsourced data is added at the end of array "WriteData".
            int OutsourcedDataIndex = writeData.Length;
            ExifWriteUInt32(writeData.Buffer, i + 8, (uint)(exifBlockOffset + OutsourcedDataIndex));
            writeData.Length += ByteCount; // Allocate memory in array "WriteData"
            Array.Copy(tempTagData.ValueData, tempTagData.ValueIndex, writeData.Buffer, OutsourcedDataIndex, ByteCount);
            if ((ByteCount & 0x1) != 0)
            {
                // The EXIF block must have a 16 bit alignment. "ByteCount" is odd, therefore add a fill byte.
                writeData.Length++; // Allocate 1 byte in array "WriteData"
                writeData.Buffer[writeData.Length - 1] = 0;
            }
        }
    }

    private int CreateExifBlock(out FlexArray newExifBlock, int currentExifBlockOffset)
    {
        int TiffNextExifBlockPointerIndex = 0;

        FlexArray writeExifBlock = new(JpegMaxExifBlockLen);
        newExifBlock = writeExifBlock;
        int writeIndex = 0;

        UpdateIfdPointerTags(
            privateDataPointerTag: out TagItem? PrivateDataPointerTag,
            gpsInfoDataPointerTag: out TagItem? GpsInfoDataPointerTag,
            interoperabilityPointerTag: out TagItem? InteroperabilityPointerTag);

        CreateIfdPrimaryData(
            writeExifBlock: writeExifBlock,
            writeIndex: ref writeIndex,
            exifBlockOffset: currentExifBlockOffset,
            privateDataIfdPointerIndex: out int PrivateDataIfdPointerIndex,
            gpsInfoDataIfdPointerIndex: out int GpsInfoDataIfdPointerIndex,
            thumbnailDataIfdPointerIndex: out int ThumbnailDataIfdPointerIndex);

        CreateIfdPrivateData(
            writeExifBlock: writeExifBlock,
            writeIndex: ref writeIndex,
            exifBlockOffset: currentExifBlockOffset,
            privateDataIfdPointerIndex: PrivateDataIfdPointerIndex,
            privateDataPointerTag: PrivateDataPointerTag,
            interoperabilityIfdPointerIndex: out int interoperabilityIfdPointerIndex);

        CreateIfdGpsInfoData(
            writeExifBlock: writeExifBlock,
            writeIndex: ref writeIndex,
            exifBlockOffset: currentExifBlockOffset,
            gpsInfoDataIfdPointerIndex: GpsInfoDataIfdPointerIndex,
            gpsInfoDataPointerTag: GpsInfoDataPointerTag);

        CreateIfdInteroperability(
            writeExifBlock: writeExifBlock,
            writeIndex: ref writeIndex,
            exifBlockOffset: currentExifBlockOffset,
            interoperabilityIfdPointerIndex: interoperabilityIfdPointerIndex,
            interoperabilityPointerTag: InteroperabilityPointerTag);

        if (ImageType == ImageType.Tiff)
        {
            // TIFF files don't have a thumbnail image, but they may have multiple images
            TiffNextExifBlockPointerIndex = ThumbnailDataIfdPointerIndex;
            ExifWriteUInt32(writeExifBlock.Buffer, TiffNextExifBlockPointerIndex, 0); // Set pointer to next image provisorily to 0
        }
        else
        {
            CreateIfdThumbnailData(writeExifBlock, ref writeIndex, currentExifBlockOffset, ThumbnailDataIfdPointerIndex);
        }

        // Check if new EXIF block is empty. Empty EXIF blocks should not be written instead the EXIF block should be removed.
        if (writeExifBlock.Length <= MinExifBlockLen)
        {
            writeExifBlock.Length = 0;
        }

        return TiffNextExifBlockPointerIndex;
    }

    private void CreateTiffHeader(out byte[] tiffHeader, int exifBlockOffset)
    {
        tiffHeader = new byte[TiffHeaderLen];
        if (ByteOrder == ExifByteOrder.BigEndian)
        {
            WriteUInt32BE(tiffHeader, 0, TiffHeaderSignatureBE);
        }
        else if (ByteOrder == ExifByteOrder.LittleEndian)
        {
            WriteUInt32BE(tiffHeader, 0, TiffHeaderSignatureLE);
        }
        else
        {
            throw new ExifException(ExifErrCode.InternalError);
        }

        ExifWriteUInt32(tiffHeader, 4, (uint)exifBlockOffset);
    }

    private void UpdateIfdPointerTags(
        out TagItem? privateDataPointerTag,
        out TagItem? gpsInfoDataPointerTag,
        out TagItem? interoperabilityPointerTag)
    {
        Dictionary<ExifTagId, TagItem> PrivateDataIfdTable = TagTable[(uint)ExifIfd.PrivateData];
        Dictionary<ExifTagId, TagItem> InteroperabilityIfdTable = TagTable[(uint)ExifIfd.Interoperability];
        _ = PrivateDataIfdTable.TryGetValue(ExifTagId.InteroperabilityIfdPointer, out interoperabilityPointerTag);
        int InteroperabilityTagCount = InteroperabilityIfdTable.Count;
        if (InteroperabilityTagCount > 0)
        {
            if (interoperabilityPointerTag == null)
            {
                interoperabilityPointerTag = new TagItem(ExifTag.InteroperabilityIfdPointer, ExifTagType.ULong, 1);
                PrivateDataIfdTable.Add(ExifTagId.InteroperabilityIfdPointer, interoperabilityPointerTag);
            }
        }
        else
        {
            if (interoperabilityPointerTag != null)
            {
                _ = PrivateDataIfdTable.Remove(ExifTagId.InteroperabilityIfdPointer);
            }
        }

        Dictionary<ExifTagId, TagItem> PrimaryDataIfdTable = TagTable[(uint)ExifIfd.PrimaryData];
        Dictionary<ExifTagId, TagItem> GpsInfoDataIfdTable = TagTable[(uint)ExifIfd.GpsInfoData];
        _ = PrimaryDataIfdTable.TryGetValue(ExifTagId.GpsInfoIfdPointer, out gpsInfoDataPointerTag);
        int GpsInfoDataTagCount = GpsInfoDataIfdTable.Count;
        if (GpsInfoDataTagCount > 0)
        {
            if (gpsInfoDataPointerTag == null)
            {
                gpsInfoDataPointerTag = new TagItem(ExifTag.GpsInfoIfdPointer, ExifTagType.ULong, 1);
                PrimaryDataIfdTable.Add(ExifTagId.GpsInfoIfdPointer, gpsInfoDataPointerTag);
            }
        }
        else
        {
            if (gpsInfoDataPointerTag != null)
            {
                _ = PrimaryDataIfdTable.Remove(ExifTagId.GpsInfoIfdPointer);
            }
        }

        _ = PrimaryDataIfdTable.TryGetValue(ExifTagId.ExifIfdPointer, out privateDataPointerTag);
        if (PrivateDataIfdTable.Count > 0)
        {
            if (privateDataPointerTag == null)
            {
                privateDataPointerTag = new TagItem(ExifTag.ExifIfdPointer, ExifTagType.ULong, 1);
                PrimaryDataIfdTable.Add(ExifTagId.ExifIfdPointer, privateDataPointerTag);
            }
        }
        else
        {
            if (privateDataPointerTag != null)
            {
                _ = PrimaryDataIfdTable.Remove(ExifTagId.ExifIfdPointer);
            }
        }
    }

    private void CreateIfdPrimaryData(
        FlexArray writeExifBlock,
        ref int writeIndex,
        int exifBlockOffset,
        out int privateDataIfdPointerIndex,
        out int gpsInfoDataIfdPointerIndex,
        out int thumbnailDataIfdPointerIndex)
    {
        Dictionary<ExifTagId, TagItem> PrimaryDataIfdTable = TagTable[(uint)ExifIfd.PrimaryData];
        int PrimaryDataTagCount = PrimaryDataIfdTable.Count;
        privateDataIfdPointerIndex = -1;
        gpsInfoDataIfdPointerIndex = -1;
        int IfdFixedSize = 2 + (PrimaryDataTagCount * 12) + 4; // "+ 4" for thumbnail data pointer
        writeExifBlock.Length += IfdFixedSize; // Allocate memory in array "writeExifBlock"
        ExifWriteUInt16(writeExifBlock.Buffer, writeIndex, (ushort)PrimaryDataTagCount);
        writeIndex += 2;
        foreach (TagItem t in PrimaryDataIfdTable.Values)
        {
            if (t.TagId == ExifTagId.ExifIfdPointer)
            {
                t.TagType = ExifTagType.ULong;
                privateDataIfdPointerIndex = writeIndex + 8;  // The "Private Data IFD" pointer is unknown at this time and a dummy
                                                              // value is written for it. The index is stored, at which the actual pointer can be written later.
            }
            else if (t.TagId == ExifTagId.GpsInfoIfdPointer)
            {
                t.TagType = ExifTagType.ULong;
                gpsInfoDataIfdPointerIndex = writeIndex + 8; // The "GPS Info Data IFD" pointer is unknown at this time and a dummy
                                                             // value is written for it. The index is stored, at which the actual pointer can be written later.
            }

            WriteTagToFlexArray(t, writeExifBlock, ref writeIndex, exifBlockOffset);
        }

        // Now the thumbnail data pointer should be written. Because it is unknown at this time the index is stored.
        thumbnailDataIfdPointerIndex = writeIndex;
        writeIndex = writeExifBlock.Length; // Write the next IFD behind the outsourced data of the IFD Primary Data
    }

    private void CreateIfdPrivateData(
        FlexArray writeExifBlock,
        ref int writeIndex,
        int exifBlockOffset,
        int privateDataIfdPointerIndex,
        TagItem? privateDataPointerTag,
        out int interoperabilityIfdPointerIndex)
    {
        Dictionary<ExifTagId, TagItem> PrivateDataIfdTable = TagTable[(uint)ExifIfd.PrivateData];
        int PrivateDataTagCount = PrivateDataIfdTable.Count;
        interoperabilityIfdPointerIndex = -1;
        if ((privateDataPointerTag != null) && (PrivateDataTagCount > 0))
        {
            int iPrivateDataStart = writeIndex;
            int MakerNoteDataPointerIndex, OffsetSchemaValueIndex;
            bool AddOffsetSchemaTag;
            ExifWriteUInt32(privateDataPointerTag.ValueData, privateDataPointerTag.ValueIndex, (uint)(exifBlockOffset + writeIndex));
            ExifWriteUInt32(writeExifBlock.Buffer, privateDataIfdPointerIndex, (uint)(exifBlockOffset + writeIndex));
            do
            {
                MakerNoteDataPointerIndex = 0;
                OffsetSchemaValueIndex = 0;
                TagItem? OffsetSchemaTag = null;
                int IfdFixedSize = 2 + (PrivateDataTagCount * 12) + 4; // "+ 4" for null pointer at the end
                writeExifBlock.Length += IfdFixedSize; // Allocate memory in array "writeExifBlock"
                ExifWriteUInt16(writeExifBlock.Buffer, writeIndex, (ushort)PrivateDataTagCount);
                writeIndex += 2;
                foreach (TagItem t in PrivateDataIfdTable.Values)
                {
                    if (t.TagId == ExifTagId.InteroperabilityIfdPointer)
                    {
                        t.TagType = ExifTagType.ULong;
                        interoperabilityIfdPointerIndex = writeIndex + 8; // The "Interoperability IFD" pointer is unknown at this time and a dummy
                                                                          // value is written for it. The index is stored, at which the actual pointer can be written later.
                    }
                    else if (t.TagId == ExifTagId.MakerNote)
                    {
                        MakerNoteDataPointerIndex = writeIndex + 8;
                    }
                    else if (t.TagId == ExifTagId.OffsetSchema)
                    {
                        t.TagType = ExifTagType.SLong;
                        OffsetSchemaValueIndex = writeIndex + 8;
                        OffsetSchemaTag = t;
                    }

                    WriteTagToFlexArray(t, writeExifBlock, ref writeIndex, exifBlockOffset);
                }

                ExifWriteUInt32(writeExifBlock.Buffer, writeIndex, 0); // Write null pointer at the end of the tag list
                writeIndex = writeExifBlock.Length; // Write the next IFD behind the outsourced data of the IFD Private Data
                AddOffsetSchemaTag = CheckIfMakerNoteTagHasMoved(
                    dataBlock: writeExifBlock.Buffer,
                    makerNoteDataPointerIndex: MakerNoteDataPointerIndex,
                    offsetSchemaValueIndex: OffsetSchemaValueIndex,
                    offsetSchemaTag: OffsetSchemaTag);

                if (AddOffsetSchemaTag)
                {
                    TagItem t = new(ExifTag.OffsetSchema, ExifTagType.SLong, 1);
                    PrivateDataIfdTable.Add(ExifTagId.OffsetSchema, t);
                    PrivateDataTagCount++;
                    writeExifBlock.Length = iPrivateDataStart; // Free all memory that was allocated for IFD Private Data
                    writeIndex = iPrivateDataStart;
                }
            }
            while (AddOffsetSchemaTag);
        }
    }

    private void CreateIfdGpsInfoData(
        FlexArray writeExifBlock,
        ref int writeIndex,
        int exifBlockOffset,
        int gpsInfoDataIfdPointerIndex,
        TagItem? gpsInfoDataPointerTag)
    {
        Dictionary<ExifTagId, TagItem> GpsInfoDataIfdTable = TagTable[(uint)ExifIfd.GpsInfoData];
        int GpsInfoDataTagCount = GpsInfoDataIfdTable.Count;
        if ((gpsInfoDataPointerTag != null) && (GpsInfoDataTagCount > 0))
        {
            uint GpsInfoDataOffset = (uint)(exifBlockOffset + writeIndex);
            ExifWriteUInt32(gpsInfoDataPointerTag.ValueData, gpsInfoDataPointerTag.ValueIndex, GpsInfoDataOffset);
            ExifWriteUInt32(writeExifBlock.Buffer, gpsInfoDataIfdPointerIndex, GpsInfoDataOffset);
            int IfdFixedSize = 2 + (GpsInfoDataTagCount * 12) + 4;
            writeExifBlock.Length += IfdFixedSize;
            ExifWriteUInt16(writeExifBlock.Buffer, writeIndex, (ushort)GpsInfoDataTagCount);
            writeIndex += 2;
            foreach (TagItem t in GpsInfoDataIfdTable.Values)
            {
                WriteTagToFlexArray(t, writeExifBlock, ref writeIndex, exifBlockOffset);
            }

            ExifWriteUInt32(writeExifBlock.Buffer, writeIndex, 0); // Write null pointer at the end of the tag list
            writeIndex = writeExifBlock.Length; // Write the next IFD behind the outsourced data of the IFD GPS Info Data
        }
    }

    private void CreateIfdInteroperability(
        FlexArray writeExifBlock,
        ref int writeIndex,
        int exifBlockOffset,
        int interoperabilityIfdPointerIndex,
        TagItem? interoperabilityPointerTag)
    {
        Dictionary<ExifTagId, TagItem> InteroperabilityIfdTable = TagTable[(uint)ExifIfd.Interoperability];
        int InteroperabilityTagCount = InteroperabilityIfdTable.Count;
        if ((interoperabilityPointerTag != null) && (InteroperabilityTagCount > 0))
        {
            uint InteroperabilityOffset = (uint)(exifBlockOffset + writeIndex);
            ExifWriteUInt32(interoperabilityPointerTag.ValueData, interoperabilityPointerTag.ValueIndex, InteroperabilityOffset);
            ExifWriteUInt32(writeExifBlock.Buffer, interoperabilityIfdPointerIndex, InteroperabilityOffset);
            int IfdFixedSize = 2 + (InteroperabilityTagCount * 12) + 4;
            writeExifBlock.Length += IfdFixedSize;
            ExifWriteUInt16(writeExifBlock.Buffer, writeIndex, (ushort)InteroperabilityTagCount);
            writeIndex += 2;
            foreach (TagItem t in InteroperabilityIfdTable.Values)
            {
                WriteTagToFlexArray(t, writeExifBlock, ref writeIndex, exifBlockOffset);
            }

            ExifWriteUInt32(writeExifBlock.Buffer, writeIndex, 0); // Write null pointer at the end of the tag list
            writeIndex = writeExifBlock.Length; // Write the next IFD behind the outsourced data of the IFD Interoperability
        }
    }

    private void CreateIfdThumbnailData(
        FlexArray writeExifBlock,
        ref int writeIndex,
        int exifBlockOffset,
        int thumbnailDataIfdPointerIndex)
    {
        Dictionary<ExifTagId, TagItem> ThumbnailDataIfdTable = TagTable[(uint)ExifIfd.ThumbnailData];
        int ThumbnailDataTagCount = ThumbnailDataIfdTable.Count;
        if ((ThumbnailDataTagCount > 0) || ThumbnailImageExists())
        {
            ExifWriteUInt32(writeExifBlock.Buffer, thumbnailDataIfdPointerIndex, (uint)(exifBlockOffset + writeIndex));
            int IfdFixedSize = 2 + (ThumbnailDataTagCount * 12) + 4;
            writeExifBlock.Length += IfdFixedSize;
            ExifWriteUInt16(writeExifBlock.Buffer, writeIndex, (ushort)ThumbnailDataTagCount);
            writeIndex += 2;
            int ThumbnailImagePointerIndex = -1;
            TagItem? ThumbnailImagePointerTag = null;
            bool ThumbnailImageSizeTagExists = false;
            foreach (TagItem t in ThumbnailDataIfdTable.Values)
            {
                if (t.TagId == ExifTagId.JpegInterchangeFormat)
                {
                    // The tag "TagId_JpegInterchangeFormat" stores a pointer to the thumbnail image.
                    // The pointer is not known at this time, so the index, where the pointer has to be written, is stored.
                    t.TagType = ExifTagType.ULong;
                    ThumbnailImagePointerTag = t;
                    ThumbnailImagePointerIndex = writeIndex + 8;
                }
                else if (t.TagId == ExifTagId.JpegInterchangeFormatLength)
                {
                    t.TagType = ExifTagType.ULong;
                    ExifWriteUInt32(t.ValueData, t.ValueIndex, (uint)ThumbnailByteCount);
                    ThumbnailImageSizeTagExists = true;
                }

                WriteTagToFlexArray(t, writeExifBlock, ref writeIndex, exifBlockOffset);
            }

            if (ThumbnailImage != null)
            {
                // A thumbnail image is defined
                if ((ThumbnailImagePointerIndex < 0) || (!ThumbnailImageSizeTagExists) || (ThumbnailImagePointerTag == null))
                    throw new ExifException(ExifErrCode.InternalError);

                int ThumbnailImageIndex = writeExifBlock.Length;
                uint ThumbnailImageOffset = (uint)(exifBlockOffset + ThumbnailImageIndex);
                ExifWriteUInt32(ThumbnailImagePointerTag.ValueData, ThumbnailImagePointerTag.ValueIndex, ThumbnailImageOffset);
                ExifWriteUInt32(writeExifBlock.Buffer, ThumbnailImagePointerIndex, ThumbnailImageOffset);
                writeExifBlock.Length += ThumbnailByteCount;
                Array.Copy(ThumbnailImage, ThumbnailStartIndex, writeExifBlock.Buffer, ThumbnailImageIndex, ThumbnailByteCount);
                if ((writeExifBlock.Length & 0x1) != 0)
                {
                    writeExifBlock.Length++;
                    writeExifBlock[^1] = 0; // Insert fill byte
                }
            }
            else
            {
                // A thumbnail image is not defined
                if (ThumbnailImagePointerIndex >= 0)
                    throw new ExifException(ExifErrCode.InternalError);
            }

            ExifWriteUInt32(writeExifBlock.Buffer, writeIndex, 0); // Write null pointer at the end of the tag table
        }
        else
        {
            ExifWriteUInt32(writeExifBlock.Buffer, thumbnailDataIfdPointerIndex, 0);
        }
    }

    private bool CheckIfMakerNoteTagHasMoved(
        byte[] dataBlock,
        int makerNoteDataPointerIndex,
        int offsetSchemaValueIndex,
        TagItem? offsetSchemaTag)
    {
        bool MakerNoteHasMovedAndOffsetSchemaTagRequired = false;
        if ((MakerNoteOriginalOffset > 0) && (makerNoteDataPointerIndex > 0))
        {
            // "MakerNote" has existed and is now existing
            int MakerNoteCurrentOffset = (int)ExifReadUInt32(dataBlock, makerNoteDataPointerIndex);
            if (MakerNoteOriginalOffset != MakerNoteCurrentOffset)
            {
                // "MakerNote" has moved
                if ((offsetSchemaValueIndex > 0) && (offsetSchemaTag != null))
                {
                    int NewOffsetDifference = MakerNoteCurrentOffset - MakerNoteOriginalOffset;
                    ExifWriteUInt32(dataBlock, offsetSchemaValueIndex, (uint)NewOffsetDifference);
                    ExifWriteUInt32(offsetSchemaTag.ValueData, offsetSchemaTag.ValueIndex, (uint)NewOffsetDifference);
                }
                else
                {
                    MakerNoteHasMovedAndOffsetSchemaTagRequired = true;
                }
            }
            else
            {
                // "MakerNote" has not moved
                if ((offsetSchemaValueIndex > 0) && (offsetSchemaTag != null))
                {
                    ExifWriteUInt32(dataBlock, offsetSchemaValueIndex, 0); // Set move offset to 0
                    ExifWriteUInt32(offsetSchemaTag.ValueData, offsetSchemaTag.ValueIndex, 0);
                }
            }
        }
        else
        {
            if ((offsetSchemaValueIndex > 0) && (offsetSchemaTag != null))
            {
                ExifWriteUInt32(dataBlock, offsetSchemaValueIndex, 0); // Set move offset to 0
                ExifWriteUInt32(offsetSchemaTag.ValueData, offsetSchemaTag.ValueIndex, 0);
            }
        }

        return MakerNoteHasMovedAndOffsetSchemaTagRequired;
    }

    /// <summary>
    /// Create an EXIF tag and initialize it with raw data.
    /// </summary>
    /// <param name="ifdRawData">Raw data for the tag. The raw data are not copied, instead a reference
    /// to "IfdRawData" is assigned.</param>
    /// <param name="ifdRawDataIndex">Index of the first raw data byte, i. e. "IfdRawData[IfdRawDataIndex]"
    /// is the first raw data byte.</param>
    /// <returns>EXIF tag as "TagItem" object.</returns>
    /// <exception cref="ExifException"></exception>
    private TagItem CreateTagWithReferenceToIfdRawData(byte[] ifdRawData, int ifdRawDataIndex)
    {
        uint valueCount = ExifReadUInt32(ifdRawData, ifdRawDataIndex + 4);
        if (valueCount > MaxTagValueCount)
            throw new ExifException(ErrCodeForIllegalExifBlock);

        var TagId = (ExifTagId)ExifReadUInt16(ifdRawData, ifdRawDataIndex);
        var tagType = (ExifTagType)ExifReadUInt16(ifdRawData, ifdRawDataIndex + 2);
        int ValueByteCount = GetTagByteCount(tagType, (int)valueCount);
        int ValueIndex, AllocatedByteCount, OriginalOffset;
        byte[] TagData;

        if (ValueByteCount <= 4)
        {
            // The tag doesn't have outsourced data. In this case a memory space with a size of 4 bytes is available for the data.
            ValueIndex = ifdRawDataIndex + 8;
            TagData = ifdRawData;
            AllocatedByteCount = 4;
            OriginalOffset = 0;
        }
        else
        {
            // The tag has outsourced data.
            OriginalOffset = (int)ExifReadUInt32(ifdRawData, ifdRawDataIndex + 8);
            AllocatedByteCount = ValueByteCount;
            GetOutsourcedData(OriginalOffset, AllocatedByteCount, out TagData, out ValueIndex);
        }

        TagItem TempTagItem = new(TagId, tagType, (int)valueCount, TagData, ValueIndex, AllocatedByteCount, OriginalOffset);

        return TempTagItem;
    }

    private void GetOutsourcedData(int dataOffset, int dataLen, out byte[] tagData, out int tagDataIndex)
    {
        if (SourceExifBlock != null)
        {
            if ((uint)(dataOffset + dataLen) > (uint)SourceExifBlock.Length)
                throw new ExifException(ErrCodeForIllegalExifBlock);

            tagData = SourceExifBlock;
            tagDataIndex = dataOffset;
        }
        else
        {
            tagData = new byte[dataLen];
            SourceExifStream.Position = dataOffset; // + ExifBlockFilePosition;
            if (SourceExifStream.Read(tagData, 0, dataLen) != dataLen)
                throw new ExifException(ErrCodeForIllegalExifBlock);

            tagDataIndex = 0;
        }
    }

    private void InitIfdPrimaryData(byte[] ifdRawData, int ifdRawDataIndex, out int privateDataOffset, out int gpsInfoDataOffset, out int nextImageOffset)
    {
        privateDataOffset = 0;
        gpsInfoDataOffset = 0;
        int TagCount = ExifReadUInt16(ifdRawData, ifdRawDataIndex);
        ifdRawDataIndex += 2;
        var PrimaryDataIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
        TagTable[(uint)ExifIfd.PrimaryData] = PrimaryDataIfdTable;
        int j = 0;

        while (j < TagCount)
        {
            TagItem TempTagData = CreateTagWithReferenceToIfdRawData(ifdRawData, ifdRawDataIndex);
            if (AddIfNotExists(PrimaryDataIfdTable, TempTagData))
            {
                if (TempTagData.TagId == ExifTagId.ExifIfdPointer)
                    privateDataOffset = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
                else if (TempTagData.TagId == ExifTagId.GpsInfoIfdPointer)
                    gpsInfoDataOffset = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
            }

            ifdRawDataIndex += 12;
            j++;
        }

        nextImageOffset = (int)ExifReadUInt32(ifdRawData, ifdRawDataIndex);
    }

    private void InitIfdPrivateData(byte[] ifdRawData, int ifdRawDataIndex, out int interoperabilityOffset)
    {
        interoperabilityOffset = 0;
        int MakerNoteOffset = 0;
        int OffsetSchemaValue = 0;
        int TagCount = ExifReadUInt16(ifdRawData, ifdRawDataIndex);
        ifdRawDataIndex += 2;
        var PrivateDataIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
        TagTable[(uint)ExifIfd.PrivateData] = PrivateDataIfdTable;
        int j = 0;
        while (j < TagCount)
        {
            TagItem TempTagData = CreateTagWithReferenceToIfdRawData(ifdRawData, ifdRawDataIndex);
            if (AddIfNotExists(PrivateDataIfdTable, TempTagData))
            {
                if (TempTagData.TagId == ExifTagId.InteroperabilityIfdPointer)
                    interoperabilityOffset = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
                else if (TempTagData.TagId == ExifTagId.MakerNote)
                    MakerNoteOffset = TempTagData.OriginalDataOffset;
                else if (TempTagData.TagId == ExifTagId.OffsetSchema)
                    OffsetSchemaValue = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
            }

            ifdRawDataIndex += 12;
            j++;
        }

        MakerNoteOriginalOffset = MakerNoteOffset > 0 ? MakerNoteOffset - OffsetSchemaValue : 0;
    }

    private void InitIfdGpsInfoData(byte[] ifdRawData, int ifdRawDataIndex)
    {
        int TagCount = ExifReadUInt16(ifdRawData, ifdRawDataIndex);
        ifdRawDataIndex += 2;
        var GpsInfoDataIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
        TagTable[(uint)ExifIfd.GpsInfoData] = GpsInfoDataIfdTable;
        int j = 0;
        while (j < TagCount)
        {
            TagItem TempTagData = CreateTagWithReferenceToIfdRawData(ifdRawData, ifdRawDataIndex);
            _ = AddIfNotExists(GpsInfoDataIfdTable, TempTagData);
            ifdRawDataIndex += 12;
            j++;
        }
    }

    private void InitIfdInteroperability(byte[] ifdRawData, int ifdRawDataIndex)
    {
        int TagCount = ExifReadUInt16(ifdRawData, ifdRawDataIndex);
        ifdRawDataIndex += 2;
        var InteroperabilityIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
        TagTable[(uint)ExifIfd.Interoperability] = InteroperabilityIfdTable;
        int j = 0;
        while (j < TagCount)
        {
            TagItem TempTagData = CreateTagWithReferenceToIfdRawData(ifdRawData, ifdRawDataIndex);
            _ = AddIfNotExists(InteroperabilityIfdTable, TempTagData);
            ifdRawDataIndex += 12;
            j++;
        }
    }

    private void InitIfdThumbnailData(byte[] ifdRawData, int ifdRawDataIndex)
    {
        int ThumbnailImageDataOffset = 0;
        ThumbnailByteCount = 0;
        ThumbnailImage = null;
        int TagCount = ExifReadUInt16(ifdRawData, ifdRawDataIndex);
        ifdRawDataIndex += 2;
        var ThumbnailDataIfdTable = new Dictionary<ExifTagId, TagItem>(TagCount);
        TagTable[(uint)ExifIfd.ThumbnailData] = ThumbnailDataIfdTable;
        int j = 0;
        while (j < TagCount)
        {
            TagItem TempTagData = CreateTagWithReferenceToIfdRawData(ifdRawData, ifdRawDataIndex);
            if (AddIfNotExists(ThumbnailDataIfdTable, TempTagData))
            {
                if (TempTagData.TagId == ExifTagId.JpegInterchangeFormat)
                    ThumbnailImageDataOffset = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
                else if (TempTagData.TagId == ExifTagId.JpegInterchangeFormatLength)
                    ThumbnailByteCount = (int)ExifReadUInt32(TempTagData.ValueData, TempTagData.ValueIndex);
            }

            ifdRawDataIndex += 12;
            j++;
        }

        if (ThumbnailImageDataOffset > 0)
            GetOutsourcedData(ThumbnailImageDataOffset, ThumbnailByteCount, out ThumbnailImage, out ThumbnailStartIndex);
    }

    private void GetNextIfd(int ifdOffset, out byte[] ifdRawData, out int ifdRawDataIndex)
    {
        if (ifdOffset == 0)
        {
            // IFD does not exist. Create a dummy IFD table.
            ifdRawData = new byte[2];
            ifdRawDataIndex = 0;
        }
        else if (SourceExifBlock != null)
        {
            ifdRawData = SourceExifBlock;
            ifdRawDataIndex = ifdOffset;
        }
        else
        {
            SourceExifStream.Position = ifdOffset; // + ExifBlockFilePosition;
            ifdRawDataIndex = 0;
            byte[] TagCountArray = new byte[2];
            if (SourceExifStream.Read(TagCountArray, 0, 2) != 2)
                throw new ExifException(ErrCodeForIllegalExifBlock);

            int IfdTagCount = ExifReadUInt16(TagCountArray, 0);
            int IfdSize = 2 + (IfdTagCount * 12) + 4;
            ifdRawData = new byte[IfdSize];
            ifdRawData[0] = TagCountArray[0];
            ifdRawData[1] = TagCountArray[1];
            if (SourceExifStream.Read(ifdRawData, 2, IfdSize - 2) != (IfdSize - 2))
                throw new ExifException(ErrCodeForIllegalExifBlock);
        }
    }

    /// <summary>
    /// Evaluate EXIF block.
    ///  JPEG, PNG: The complete EXIF block must be stored in the array "SourceExifBlock" starting with
    ///             the array index "TifferHeaderLen" (= 8).
    ///  TIFF: Stream "SourceStream" is used to read the EXIF data successively. "SourceExifBlock" is
    ///        "null" in this case.
    /// </summary>
    /// <param name="primaryDataOffset">Start index of the IFD "Primary data".</param>
    /// <returns>Offset of the next EXIF block in TIFF images or 0 otherwise.</returns>
    private int EvaluateExifBlock(int primaryDataOffset)
    {
        TagTable = new Dictionary<ExifTagId, TagItem>[ExifIfdCount];

        GetNextIfd(primaryDataOffset, out byte[] IfdTable, out int IfdIndex);

        InitIfdPrimaryData(IfdTable, IfdIndex, out int PrivateDataOffset, out int GpsInfoDataOffset, out int ThumbnailDataOffset);

        GetNextIfd(PrivateDataOffset, out IfdTable, out IfdIndex);

        InitIfdPrivateData(IfdTable, IfdIndex, out int InteroperabilityOffset);

        GetNextIfd(GpsInfoDataOffset, out IfdTable, out IfdIndex);

        InitIfdGpsInfoData(IfdTable, IfdIndex);

        GetNextIfd(InteroperabilityOffset, out IfdTable, out IfdIndex);

        InitIfdInteroperability(IfdTable, IfdIndex);

        int TiffNextExifBlockOffset = 0;
        if (ImageType == ImageType.Tiff)
        {
            // TIFF files don't have a thumbnail image, but they may have multiple images
            IfdTable = new byte[2];
            InitIfdThumbnailData(IfdTable, 0); // Set empty tag table for IFD ThumbnailData
            TiffNextExifBlockOffset = ThumbnailDataOffset;
        }
        else
        {
            // In JPEG files "NextImageOffset" is the offset to the thumbnail image
            GetNextIfd(ThumbnailDataOffset, out IfdTable, out IfdIndex);
            InitIfdThumbnailData(IfdTable, IfdIndex);
        }

        return TiffNextExifBlockOffset;
    }

    private static bool AddIfNotExists(Dictionary<ExifTagId, TagItem> dict, TagItem tagData)
    {
        try
        {
            dict.Add(tagData.TagId, tagData);
            return true;
        }
        catch
        {
            // An exception occured because the tag ID already exists. In this case ignore the new tag item.
            return false;
        }
    }

    /// <summary>
    /// Compare a section of an array with another array.
    /// </summary>
    /// <param name="array1">First array to be compared.</param>
    /// <param name="startIndex1">Index in "Array1" where the comparison starts.</param>
    /// <param name="array2">Second array to be compared.</param>
    /// <returns>true: Arrays are equal. false: otherwise.</returns>
    private static bool CompareArrays(byte[] array1, int startIndex1, byte[] array2)
    {
        if (array1.Length < (startIndex1 + array2.Length))
            return false;

        bool IsEqual = true;
        int i = startIndex1;
        foreach (byte b in array2)
        {
            if (array1[i] != b)
            {
                IsEqual = false;
                break;
            }

            i++;
        }

        return IsEqual;
    }

    /// <summary>
    /// Compare "Array1" from index 0 to index "Array2.Length - 1" with "Array2".
    /// If "Array1" is smaller than "Array2", "false" is returned.
    /// </summary>
    /// <param name="array1">First array to be compared.</param>
    /// <param name="array1Length">Length of "Array1" to be compared.</param>
    /// <param name="array2">Second array to be compared.</param>
    /// <returns>true: Arrays are equal. false: otherwise.</returns>
    private static bool ArrayStartsWith(byte[] array1, int array1Length, byte[] array2)
    {
        if (array1Length < array2.Length)
            return false;

        bool IsEqual = true;
        int i = 0;
        foreach (byte b in array2)
        {
            if (array1[i] != b)
            {
                IsEqual = false;
                break;
            }

            i++;
        }

        return IsEqual;
    }

    private bool ReadUintElement(TagItem t, int elementIndex, out uint value)
    {
        bool Success = false;
        if ((elementIndex >= 0) && (elementIndex < t.ValueCount))
        {
            switch (t.TagType)
            {
                case ExifTagType.Byte:
                    value = t.ValueData[t.ValueIndex + elementIndex];
                    Success = true;
                    break;

                case ExifTagType.UShort:
                    value = ExifReadUInt16(t.ValueData, t.ValueIndex + (elementIndex << 1));
                    Success = true;
                    break;

                case ExifTagType.ULong:
                case ExifTagType.SLong:
                    value = ExifReadUInt32(t.ValueData, t.ValueIndex + (elementIndex << 2));
                    Success = true;
                    break;

                default:
                    value = 0;
                    break;
            }
        }
        else
        {
            value = 0;
        }

        return Success;
    }
    private bool WriteUintElement(TagItem? t, int elementIndex, uint value)
    {
        bool Success = false;
        if (t != null)
        {
            if (t.TagType == ExifTagType.Byte)
                t.ValueData[t.ValueIndex + elementIndex] = (byte)value;
            else if (t.TagType == ExifTagType.UShort)
                ExifWriteUInt16(t.ValueData, t.ValueIndex + (elementIndex << 1), (ushort)value);
            else
                ExifWriteUInt32(t.ValueData, t.ValueIndex + (elementIndex << 2), value);

            Success = true;
        }

        return Success;
    }

    private bool GetTagValueWithIdCode(ExifTag tagSpec, out string value, ushort codePage)
    {
        bool Success = false;
        value = string.Empty;

        if (GetTagItem(tagSpec, out TagItem? t) && (t.TagType == ExifTagType.Undefined) && (t.ValueCount >= IdCodeLength))
        {
            bool IsUtf16Coded = CompareArrays(t.ValueData, t.ValueIndex, IdCodeUtf16);
            bool IsAsciiCoded = CompareArrays(t.ValueData, t.ValueIndex, IdCodeAscii) || CompareArrays(t.ValueData, t.ValueIndex, IdCodeDefault);

            int i = t.ValueCount - IdCodeLength;
            int j = t.ValueIndex + IdCodeLength;

            if (IsUtf16Coded)
            {
                // The parameter "CodePage" is ignored in this case.
                // Remove all terminating null characters. Here a terminating null character consists of 2 zero-bytes.
                while ((i >= 2) && (t.ValueData[j + i - 2] == 0) && (t.ValueData[j + i - 1] == 0))
                    i -= 2;

                if (ByteOrder == ExifByteOrder.BigEndian)
                    codePage = 1201; // UTF16BE
                else
                    codePage = 1200; // UTF16LE

                value = Encoding.GetEncoding(codePage).GetString(t.ValueData, j, i);
                Success = true;
            }
            else if (IsAsciiCoded)
            {
                // Remove all null terminating characters.
                while ((i >= 1) && (t.ValueData[j + i - 1] == 0))
                    i--;

                if (codePage is 1200 or 1201) // UTF16LE or UTF16BE
                    codePage = 20127; // Ignore parameter "CodePage" and overwrite it with US ASCII

                value = Encoding.GetEncoding(codePage).GetString(t.ValueData, j, i);
                Success = true;
            }
        }

        return Success;
    }
    private bool SetTagValueWithIdCode(ExifTag tagSpec, string value, ushort codePage)
    {
        bool Success = false;
        if ((codePage == 1200) && (ByteOrder == ExifByteOrder.BigEndian))
            codePage = 1201; // Set code page to UTF16BE

        if ((codePage == 1201) && (ByteOrder == ExifByteOrder.LittleEndian))
            codePage = 1200; // Set code page to UTF16LE

        byte[] StringAsByteArray = Encoding.GetEncoding(codePage).GetBytes(value);
        int StrByteLen = StringAsByteArray.Length;
        int TotalByteCount = IdCodeLength + StrByteLen; // The ID code is a 8 byte header
        TagItem? t = PrepareTagForCompleteWriting(tagSpec, ExifTagType.Undefined, TotalByteCount);
        if (t != null)
        {
            byte[] RequiredIdCode = codePage is 1200 or 1201 ? IdCodeUtf16 : IdCodeAscii;
            Array.Copy(RequiredIdCode, 0, t.ValueData, t.ValueIndex, IdCodeLength);
            Array.Copy(StringAsByteArray, 0, t.ValueData, t.ValueIndex + IdCodeLength, StrByteLen);
            Success = true;
        }

        return Success;
    }

    private bool ReadURationalElement(TagItem t, int elementIndex, out ExifRational exifRational)
    {
        bool Success = false;
        exifRational = new();

        //t.tagType == ExifTagType.URational ? new ExifRational(Numer, Denom) : new ExifRational((int)Numer, (int)Denom);

        if ((elementIndex >= 0) &&
            (elementIndex < t.ValueCount) &&
            t.TagType is ExifTagType.SRational or ExifTagType.URational)
        {
            int i = t.ValueIndex + (elementIndex << 3);
            exifRational.Numer = ExifReadUInt32(t.ValueData, i);
            exifRational.Denom = ExifReadUInt32(t.ValueData, i + 4);
            Success = true;
        }

        return Success;
    }
    private bool WriteURationalElement(TagItem? t, int elementIndex, ExifRational exifRational)
    {
        bool Success = false;
        if (t != null)
        {
            int i = t.ValueIndex + (elementIndex << 3);
            ExifWriteUInt32(t.ValueData, i, exifRational.Numer);
            ExifWriteUInt32(t.ValueData, i + 4, exifRational.Denom);
            Success = true;
        }

        return Success;
    }

    private bool GetTagItem(ExifTag tagSpec, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out TagItem? t)
    {
        bool Success = false;
        t = null;
        ExifIfd Ifd = ExtractIfd(tagSpec);
        if ((uint)Ifd < ExifIfdCount)
        {
            Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
            if (IfdTagTable.TryGetValue(ExtractTagId(tagSpec), out TagItem? TempTagItem))
            {
                t = TempTagItem;
                Success = true;
            }
        }

        return Success;
    }

    /// <summary>
    /// Prepare tag for writing a single array item. If the tag does not exist it is created.
    /// </summary>
    /// <param name="tagSpec">IFD and tag ID of the value to be prepared for writing.</param>
    /// <param name="tagType">Tag type.</param>
    /// <param name="arrayIndex">Zero-based array index of the value to be written. If the array index
    /// is outside the current tag data, the tag data is automatically enlarged. If it is necessary
    /// to reallocate the tag memory, the old content is copied to the new memory.</param>
    /// <returns>Tag item, that is prepared for writing to the specified array index.
    /// In case of an error "null" is returned.</returns>
    private TagItem? PrepareTagForArrayItemWriting(ExifTag tagSpec, ExifTagType tagType, int arrayIndex)
    {
        TagItem? t = null;
        ExifIfd Ifd = ExtractIfd(tagSpec);
        if (((uint)Ifd < ExifIfdCount) && ((uint)arrayIndex < MaxTagValueCount))
        {
            Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
            if (IfdTagTable.TryGetValue(ExtractTagId(tagSpec), out t))
            {
                int valueCount = t.ValueCount;
                if (arrayIndex >= valueCount)
                    valueCount = arrayIndex + 1;

                t.SetTagTypeAndValueCount(tagType, valueCount, true);
            }
            else
            {
                int valueCount = arrayIndex + 1;
                t = new TagItem(tagSpec, tagType, valueCount);
                IfdTagTable.Add(t.TagId, t);
            }
        }

        return t;
    }

    /// <summary>
    /// Prepare tag for a complete writing of the tag content. The tag content is undefined after calling this method.
    /// </summary>
    /// <param name="tagSpec">IFD and tag ID of the value to be prepared for writing.</param>
    /// <param name="tagType">New tag type. The tag type must be valid, because it is not checked in this method!</param>
    /// <param name="valueCount">New number of array elements that the tag should contain. Must be equal
    /// or greater than 1. If it is necessary to reallocate the tag memory, the old content is not copied.</param>
    /// <returns>Tag item, that is prepared for writing of the specified number of array elements.
    /// In case of an error "null" is returned.</returns>
    private TagItem? PrepareTagForCompleteWriting(ExifTag tagSpec, ExifTagType tagType, int valueCount)
    {
        TagItem? t = null;
        ExifIfd Ifd = ExtractIfd(tagSpec);
        if (((uint)Ifd < ExifIfdCount) && ((uint)valueCount <= MaxTagValueCount))
        {
            Dictionary<ExifTagId, TagItem> IfdTagTable = TagTable[(uint)Ifd];
            if (IfdTagTable.TryGetValue(ExtractTagId(tagSpec), out t))
            {
                t.SetTagTypeAndValueCount(tagType, valueCount, false);
            }
            else
            {
                t = new TagItem(tagSpec, tagType, valueCount);
                IfdTagTable.Add(t.TagId, t);
            }
#if DEBUG
            int k = t.ValueIndex + t.AllocatedByteCount;
            for (int i = t.ValueIndex; i < k; i++)
                t.ValueData[i] = 0xcc;
#endif
        }

        return t;
    }

    private static int CalculateTwoDigitDecNumber(byte[] byteArr, int index)
    {
        int Value = -1;
        int d1 = byteArr[index];
        int d2 = byteArr[index + 1];
        if ((d1 >= '0') && (d1 <= '9') && (d2 >= '0') && (d2 <= '9'))
            Value = ((d1 - 0x30) * 10) + (d2 - 0x30);

        return Value;
    }

    /// <summary>
    /// Convert a number to exactly two decimal digits and store them in a byte array.
    /// </summary>
    /// <param name="byteArr">Out: Two decimal digits.</param>
    /// <param name="index">In: Index in "ByteArr", to which the digits should be written.
    /// Out: New index after writing the two digits.</param>
    /// <param name="value">Number from 0 to 99 to be converted.</param>
    private static void ConvertTwoDigitNumberToByteArr(byte[] byteArr, ref int index, int value)
    {
        byteArr[index] = (byte)((value / 10) + 0x30);
        index++;
        byteArr[index] = (byte)((value % 10) + 0x30);
        index++;
    }

    private void ClearIfd_Unchecked(ExifIfd ifd) => TagTable[(uint)ifd].Clear();

    private void RemoveAllTagsFromIfdPrimaryData()
    {
        if (ImageType == ImageType.Tiff)
        {
            // In TIFF images the IFD PrimaryData contains internal tags which must not be removed otherwise
            // the image will be damaged.
            // In TIFF images the XMP and IPTC block are stored as tags within the EXIF block. Therefore these
            // tags are not removed, too.
            Dictionary<ExifTagId, TagItem> PrimaryDataIfdTable = TagTable[(uint)ExifIfd.PrimaryData];
            List<ExifTagId> TagIdsToBeRemoved = [with(PrimaryDataIfdTable.Count)];
            foreach (ExifTagId tid in PrimaryDataIfdTable.Keys)
            {
                if (!IsInternalTiffTag(tid) && !IsMetaDataTiffTag(tid))
                    TagIdsToBeRemoved.Add(tid);
            }

            foreach (ExifTagId tid in TagIdsToBeRemoved)
                _ = PrimaryDataIfdTable.Remove(tid);
        }
        else
        {
            ClearIfd_Unchecked(ExifIfd.PrimaryData);
        }
    }

    private void RemoveThumbnailImage_Internal()
    {
        ThumbnailImage = null;
        ThumbnailStartIndex = 0;
        ThumbnailByteCount = 0;
    }

    private bool GetDateAndTimeWithMillisecHelper(out DateTime Value, ExifTag dateAndTimeTag, ExifTag millisecTag)
    {
        bool Success = false;
        if (GetTagValue(dateAndTimeTag, out Value))
        {
            Success = true;
            if (GetTagValue(millisecTag, out string? SubSec, StrCoding.Utf8))
            {
                string? s = SubSec;
                int len = s.Length;
                if (len > 3)
                    s = s[..3];

                if (int.TryParse(s, out int MilliSec) && (MilliSec >= 0))
                {
                    if (len == 1)
                        MilliSec *= 100;
                    else if (len == 2)
                        MilliSec *= 10;
                    Value = Value.AddMilliseconds(MilliSec);
                }
            }
        }

        return Success;
    }
    private bool SetDateAndTimeWithMillisecHelper(DateTime value, ExifTag dateAndTimeTag, ExifTag millisecTag)
    {
        bool Success = false;
        if (SetTagValue(dateAndTimeTag, value))
        {
            Success = true;
            int MilliSec = value.Millisecond;
            if ((MilliSec != 0) || TagExists(millisecTag))
            {
                string s = MilliSec.ToString("000"); // Write exactly 3 decimal digits
                Success = Success && SetTagValue(millisecTag, s, StrCoding.Utf8);
            }
        }

        return Success;
    }

    private static readonly ExifTagId[] InternalTiffTags = [
        ExifTagId.ImageWidth,
        ExifTagId.ImageLength,
        ExifTagId.BitsPerSample,
        ExifTagId.Compression,
        ExifTagId.PhotometricInterpretation,
        ExifTagId.Threshholding,
        ExifTagId.CellWidth,
        ExifTagId.CellLength,
        ExifTagId.FillOrder,
        ExifTagId.StripOffsets,
        ExifTagId.SamplesPerPixel,
        ExifTagId.RowsPerStrip,
        ExifTagId.StripByteCounts,
        ExifTagId.MinSampleValue,
        ExifTagId.MaxSampleValue,
        ExifTagId.PlanarConfiguration,
        ExifTagId.FreeOffsets,
        ExifTagId.FreeByteCounts,
        ExifTagId.GrayResponseUnit,
        ExifTagId.GrayResponseCurve,
        ExifTagId.T4Options,
        ExifTagId.T6Options,
        ExifTagId.TransferFunction,
        ExifTagId.Predictor,
        ExifTagId.WhitePoint,
        ExifTagId.PrimaryChromaticities,
        ExifTagId.ColorMap,
        ExifTagId.HalftoneHints,
        ExifTagId.TileWidth,
        ExifTagId.TileLength,
        ExifTagId.TileOffsets,
        ExifTagId.TileByteCounts,
        ExifTagId.ExtraSamples,
        ExifTagId.SampleFormat,
        ExifTagId.SMinSampleValue,
        ExifTagId.SMaxSampleValue,
        ExifTagId.TransferRange,
        ExifTagId.YCbCrCoefficients,
        ExifTagId.YCbCrSubSampling,
        ExifTagId.YCbCrPositioning,
        ExifTagId.ReferenceBlackWhite,
        (ExifTagId)0x0200,
        (ExifTagId)0x0201,
        (ExifTagId)0x0202,
        (ExifTagId)0x0203,
        (ExifTagId)0x0205,
        (ExifTagId)0x0206,
        (ExifTagId)0x0207,
        (ExifTagId)0x0208,
        (ExifTagId)0x0209,
    ];

    private static System.Collections.BitArray? InternalTiffTagsBitArray;

    private static void InitInternalTiffTags()
    {
        InternalTiffTagsBitArray = new System.Collections.BitArray(0x0215, false);
        int len = InternalTiffTagsBitArray.Length;
        foreach (ExifTagId TagId in InternalTiffTags)
        {
            int TagIdInt = (int)TagId;
            if (TagIdInt >= len)
            {
                len <<= 1;
                if (TagIdInt >= len)
                    len = TagIdInt + 1;
                InternalTiffTagsBitArray.Length = len;
            }

            InternalTiffTagsBitArray.Set(TagIdInt, true);
        }
    }

    /// <summary>
    /// Checks, if a tag is an internal tag of a TIFF image. Method can be used only for TIFF images.
    /// </summary>
    /// <param name="tagId">Tag ID of the tag be checked.</param>
    /// <returns>true: "TagId" is an internal tag ID. false: Otherwise.</returns>
    private static bool IsInternalTiffTag(ExifTagId tagId)
    {
        if (InternalTiffTagsBitArray == null)
            InitInternalTiffTags();

        int TagIdInt = (int)tagId;

        return TagIdInt < InternalTiffTagsBitArray!.Length && InternalTiffTagsBitArray.Get(TagIdInt);
    }

    /// <summary>
    /// Checks, if a tag is meta data tag of a TIFF image. Method can be used only for TIFF images.
    /// </summary>
    /// <param name="tagId">Tag ID of the tag be checked.</param>
    /// <returns>true: "TagId" is a meta data tag ID. false: Otherwise.</returns>
    private static bool IsMetaDataTiffTag(ExifTagId tagId) => tagId is ExifTagId.XmpMetadata or ExifTagId.IptcMetadata;

    private static void SwapByteOrderOfTagData(ExifIfd ifd, TagItem t)
    {
        // Special tag IDs
        if (ifd == ExifIfd.PrimaryData)
        {
            // There are meta data tags where the byte order must not be changed. These tags are only used in TIFF files.
            if (IsMetaDataTiffTag(t.TagId))
                return;
        }
        else if (ifd == ExifIfd.PrivateData)
        {
            if ((t.TagId == ExifTagId.UserComment) && (t.TagType == ExifTagType.Undefined))
            {
                // This tag is coded UTF16LE or UTF16BE depending on the byte order.
                int k = t.ValueIndex + 8; // Skip 8 byte ID code which is not UTF16 coded
                int Utf16CharCount = (t.ValueCount - 8) / 2;
                for (int i = 0; i < Utf16CharCount; i++)
                {
                    Swap2ByteValue(t.ValueData, k);
                    k += 2;
                }

                return;
            }
        }

        switch (t.TagType)
        {
            case ExifTagType.UShort:
            case ExifTagType.SShort:
                int k = t.ValueIndex;
                for (int i = 0; i < t.ValueCount; i++)
                {
                    Swap2ByteValue(t.ValueData, k);
                    k += 2;
                }

                break;

            case ExifTagType.ULong:
            case ExifTagType.SLong:
            case ExifTagType.Float:
                k = t.ValueIndex;
                for (int i = 0; i < t.ValueCount; i++)
                {
                    Swap4ByteValue(t.ValueData, k);
                    k += 4;
                }

                break;

            case ExifTagType.URational:
            case ExifTagType.SRational:
                k = t.ValueIndex;
                for (int i = 0; i < t.ValueCount; i++)
                {
                    Swap4ByteValue(t.ValueData, k); // Numerator of the rational number
                    k += 4;
                    Swap4ByteValue(t.ValueData, k); // Denominator of the rational number
                    k += 4;
                }

                break;

            case ExifTagType.Double:
                k = t.ValueIndex;
                for (int i = 0; i < t.ValueCount; i++)
                {
                    Array.Reverse(t.ValueData, k, 8);
                    k += 8;
                }

                break;
        }
    }

    private static void Swap2ByteValue(byte[] b, int i) => (b[i + 1], b[i]) = (b[i], b[i + 1]);

    private static void Swap4ByteValue(byte[] b, int i)
    {
        (b[i + 3], b[i]) = (b[i], b[i + 3]);
        (b[i + 2], b[i + 1]) = (b[i + 1], b[i + 2]);
    }

    private static TagItem CopyTagItemDeeply(ExifIfd ifd, TagItem tagItemToBeCopied, bool swapByteOrder)
    {
        TagItem NewTag = new(tagItemToBeCopied.TagId, tagItemToBeCopied.TagType, tagItemToBeCopied.ValueCount);
        Array.Copy(tagItemToBeCopied.ValueData, tagItemToBeCopied.ValueIndex, NewTag.ValueData, 0, NewTag.ByteCount);
        if (swapByteOrder)
            SwapByteOrderOfTagData(ifd, NewTag);

        return NewTag;
    }

    private class TagItem
    {
        public ExifTagId TagId;
        public ExifTagType TagType;
        public int ValueCount; // Number of values of the tag. In general this is not the number of bytes of the tag!
        public byte[] ValueData; // Array which contains the tag data. There may be other data stored in this array.
        public int ValueIndex; // Array index from which the tag data starts.
        public int AllocatedByteCount; // Number of bytes which are allocated for the tag data.
                                       // This number can be greater than the required number of bytes.
        public int ByteCount => GetTagByteCount(TagType, ValueCount); // The number of bytes required to store the tag data, i. e. the number of bytes actually used for the tag data.
        public int OriginalDataOffset; // Offset of outsourced data in the image file. 0 = Tag does not have outsourced data or
                                       // tag was created after loading.

        public TagItem(ExifTag tagSpec, ExifTagType tagType, int valueCount)
          : this(ExtractTagId(tagSpec), tagType, valueCount) { }

        public TagItem(ExifTagId tagId, ExifTagType tagType, int valueCount)
        {
            TagId = tagId;
            TagType = tagType;
            ValueCount = valueCount;
            int RequiredByteCount = GetTagByteCount(tagType, valueCount);
            ValueData = AllocTagMemory(RequiredByteCount);
            AllocatedByteCount = ValueData.Length;
            ValueIndex = 0;
        }

        public TagItem(
            ExifTagId tagId,
            ExifTagType tagType,
            int valueCount,
            byte[] valueArray,
            int valueIndex,
            int allocatedByteCount,
            int originalDataOffset = 0)
        {
            TagId = tagId;
            TagType = tagType;
            ValueCount = valueCount;
            ValueData = valueArray;
            ValueIndex = valueIndex;
            AllocatedByteCount = allocatedByteCount;
            OriginalDataOffset = originalDataOffset;
        }

        public static byte[] AllocTagMemory(int requiredByteCount)
        {
            const int MinByteCount = 32;

            return new byte[Math.Max(requiredByteCount, MinByteCount)];
        }

        /// <summary>
        /// Set tag type and value count and reallocate the tag memory if required.
        /// </summary>
        /// <param name="tagType">New tag type.</param>
        /// <param name="valueCount">New value count.</param>
        /// <param name="keepExistingData">true: Keep existing data. false: Discard existing data.</param>
        public void SetTagTypeAndValueCount(ExifTagType tagType, int valueCount, bool keepExistingData)
        {
            int RequiredByteCount = GetTagByteCount(tagType, valueCount);
            if (AllocatedByteCount < RequiredByteCount)
            {
                uint NewByteCount = (uint)AllocatedByteCount << 1; // Double the allocated byte count
                if (NewByteCount > int.MaxValue)
                    NewByteCount = int.MaxValue;
                else if (NewByteCount < (uint)RequiredByteCount)
                    NewByteCount = (uint)RequiredByteCount;

                byte[] NewTagData = AllocTagMemory((int)NewByteCount);
                if (keepExistingData)
                    Array.Copy(ValueData, ValueIndex, NewTagData, 0, AllocatedByteCount);

                AllocatedByteCount = NewTagData.Length;
                ValueData = NewTagData;
                ValueIndex = 0;
            }

            TagType = tagType;
            ValueCount = valueCount;
        }
    }

    private class FlexArray(int capacity)
    {
        public byte this[int i]
        {
            get => _Buffer[i];
            set => _Buffer[i] = value;
        }

        private byte[] _Buffer = new byte[capacity];
        public byte[] Buffer => _Buffer;

        public int Length
        {
            get;
            set
            {
                if (value > _Buffer.Length)
                {
                    uint k = (uint)_Buffer.Length;
                    k <<= 1;
                    if (k > int.MaxValue)
                        k = int.MaxValue;
                    else if ((uint)value > k)
                        k = (uint)value;

                    Array.Resize(ref _Buffer, (int)k);
                }

                field = value;
            }
        } = 0;
    }
    #endregion
}
