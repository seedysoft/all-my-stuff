namespace Seedysoft.Photo.Lib.Exif;

internal class TagItem
{
    public ExifTagId TagId;
    public ExifTagType TagType;
    public int ValueCount; // Number of values of the tag. In general this is not the number of bytes of the tag!
    public byte[] ValueData; // Array which contains the tag data. There may be other data stored in this array.
    public int ValueIndex; // Array index from which the tag data starts.
    public int AllocatedByteCount; // Number of bytes which are allocated for the tag data. 
                                   // This number can be greater than the required number of bytes.
    public int ByteCount => ExifService.GetTagByteCount(TagType, ValueCount); // The number of bytes required to store the tag data, i. e. the number of bytes actually used for the tag data.
    public int OriginalDataOffset; // Offset of outsourced data in the image file. 0 = Tag does not have outsourced data or
                                   // tag was created after loading.

    public TagItem(ExifTag tagSpec, ExifTagType tagType, int valueCount)
      : this(ExifService.ExtractTagId(tagSpec), tagType, valueCount) { }
    public TagItem(ExifTagId tagId, ExifTagType tagType, int valueCount)
    {
        TagId = tagId;
        TagType = tagType;
        ValueCount = valueCount;
        int RequiredByteCount = ExifService.GetTagByteCount(tagType, valueCount);
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

        int NewByteCount = requiredByteCount;
        if (NewByteCount < MinByteCount)
            NewByteCount = MinByteCount;

        return new byte[NewByteCount];
    }

    /// <summary>
    /// Set tag type and value count and reallocate the tag memory if required.
    /// </summary>
    /// <param name="tagTtype">New tag type.</param>
    /// <param name="valueCount">New value count.</param>
    /// <param name="keepExistingData">true: Keep existing data. false: Discard existing data.</param>
    public void SetTagTypeAndValueCount(ExifTagType tagTtype, int valueCount, bool keepExistingData)
    {
        int RequiredByteCount = ExifService.GetTagByteCount(tagTtype, valueCount);
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

        TagType = tagTtype;
        ValueCount = valueCount;
    }
}
