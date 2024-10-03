using System.Diagnostics;
using System.Text;

namespace Seedysoft.Libs.TuyaDeviceControl;

// This is a crude implementation of a format string based struct converter for C#.
// This is probably not the best implementation, the fastest implementation, the most bug-proof implementation, or even the most functional implementation.
// It's provided as-is for free. Enjoy.

// bool => "?",
// char => "c",
// int => "i",
// uint => "I",
// long => "q",
// ulong => "Q",
// short => "h",
// ushort => "H",
// float => "f",
// double => "d",
// byte => "B",
// sbyte => "b",

public static class StructConverter
{
    private const string BigEndianChar = ">";
    private const string LittleEndianChar = "<";
    private static readonly string[] EndianChars = [BigEndianChar, LittleEndianChar];

    public static int CalcSize(string fmt)
    {
        // First we parse the format string to make sure it's proper.
        if (fmt.Length < 1)
            throw new ArgumentException("Format string cannot be empty.");

        if (EndianChars.Contains(fmt[..1]))
            fmt = fmt[1..];

        int totalByteLength = 0;
        string multiplier = string.Empty;
        foreach (char c in fmt.ToCharArray())
        {
            //Debug.WriteLine($"  Format character found={c}");
            if (char.IsNumber(c))
            {
                multiplier += c;
                continue;
            }

            int mult = string.IsNullOrEmpty(multiplier) ? 1 : int.Parse(multiplier);
            totalByteLength += mult * GetFormatLength(c);

            multiplier = string.Empty;
        }

        return totalByteLength;
    }

    /// <summary>
    /// Convert an array of objects to a byte array, along with a string that can be used with <see cref="Unpack(string, byte[])"/>.
    /// </summary>
    /// <param name="items">An object array of items to convert</param>
    /// <param name="isLittleEndian">Set to False if you want to use big endian output</param>
    /// <param name="NeededFormatStringToRecover">Variable to place an 'Unpack'-compatible format string into</param>
    /// <returns>A <see cref="byte[]"/> containing the objects provided in binary format.</returns>
    public static byte[] Pack(string fmt, params object[] items)
    {
        int fmtSize = CalcSize(fmt);

        // make a byte list to hold the bytes of output
        var outputBytes = new List<byte>(fmtSize);

        // should we be flipping bits for proper endinanness?
        bool isLittleEndian = fmt.StartsWith(LittleEndianChar);

        // start working on the output string
        StringBuilder outString = new(isLittleEndian ? LittleEndianChar : BigEndianChar);

        List<object> itemsArray = new(items);
        if (itemsArray is System.Collections.IEnumerable enumerable)
        {
            foreach (object? item in enumerable)
            {
                if (item is System.Collections.IEnumerable innerEnumerable)
                {
                    itemsArray.InsertRange(itemsArray.IndexOf(innerEnumerable), innerEnumerable.Cast<object>());
                    _ = itemsArray.Remove(innerEnumerable);
                    break;
                }
            }
        }

        // convert each item in the objects to the representative bytes
        foreach (object o in itemsArray)
        {
            byte[] theseBytes = TypeAgnosticGetBytes(o);
            if (BitConverter.IsLittleEndian)
                theseBytes = theseBytes.Reverse().ToArray();
            outputBytes.AddRange(theseBytes);

            _ = outString.Append(GetFormatSpecifierFor(o));
        }

        string obtainedFormat = outString.ToString();
        int obtainedSize = CalcSize(obtainedFormat);

        return obtainedSize == fmtSize
            ? ([.. outputBytes])
            : throw new ArgumentException($"Different format sizes. Obtained '{obtainedFormat}({obtainedSize})' expected '{fmt}({fmtSize})'.");
    }

    /// <summary>
    /// Convert a <see cref="byte[]"/> into an <see cref="byte[][]"/> based on Python's "struct.unpack" protocol.
    /// </summary>
    /// <param name="fmt">A "struct.pack"-compatible format string</param>
    /// <param name="bytes">An <see cref="byte[]"/> to split</param>
    /// <returns><see cref="byte[][]"/>.</returns>
    /// <remarks>You are responsible for casting the <see cref="byte[]"/> in the array back to their proper types.</remarks>
    public static byte[][] Unpack(string fmt, params byte[] bytes)
    {
        // First we parse the format string to make sure it's proper.
        if (fmt.Length < 1)
            throw new ArgumentException("Format string cannot be empty.", nameof(fmt));

        //Debug.WriteLine($"Format string is length {fmt.Length}, {bytes.Length} bytes provided.");

        if (EndianChars.Contains(fmt[..1]))
            fmt = fmt[1..];

        // Now, we find out how long the byte array needs to be
        int totalByteLength = CalcSize(fmt);

        //Debug.WriteLine($"The byte array is expected to be {totalByteLength} bytes long.");

        // Test the byte array length to see if it contains as many bytes as is needed for the string.
        if (bytes.Length != totalByteLength)
            throw new ArgumentException("The number of bytes provided does not match the total length of the format string.");

        // Ok, we can go ahead and start parsing bytes!
        int byteArrayPosition = 0;

        //Debug.WriteLine($"Processing byte array...");

        List<byte[]> outputList = [];

        string multiplier = string.Empty;
        foreach (char c in fmt.ToCharArray())
        {
            //Debug.WriteLine($"  Format character found={c}");
            if (char.IsNumber(c))
            {
                multiplier += c;
                continue;
            }

            int mult = string.IsNullOrEmpty(multiplier) ? 1 : int.Parse(multiplier);

            int formatSize = GetFormatLength(c);

            for (int i = 0; i < mult; i++)
            {
                switch (c)
                {
                    case 'b': // char
                    case 'B': // unsigned char
                    case 'h': // short
                    case 'H': // ushort
                    case 'i': // int
                    case 'I': // uint
                    case 'l': // long
                    case 'L': // ulong
                    case 'q': // long long
                    case 'Q': // unsigned long long
                        outputList.Add(bytes[byteArrayPosition..(byteArrayPosition + formatSize)]);
                        break;

                    case 's': // char[]
                        formatSize = mult;
                        outputList.Add(bytes[byteArrayPosition..(byteArrayPosition + formatSize)]);
                        i += mult;
                        break;

                    case 'x':
                        Debug.WriteLine($"  Ignoring a byte");
                        break;

                    default:
                        throw new ArgumentException("You should not be here.");
                }

                byteArrayPosition += formatSize;
            }

            multiplier = string.Empty;
        }

        return [.. outputList];
    }

    private static int GetFormatLength(char c)
    {
        return c switch
        {
            'd' or 'q' or 'Q' => sizeof(long), // 8,
            'f' or 'i' or 'I' or 'l' or 'L' => sizeof(int), // 4
            'e' or 'h' or 'H' => sizeof(short), // 2
            '?' or 'b' or 'B' or 'c' or 's' or 'x' => sizeof(bool), // 1
            _ => throw new ArgumentException("Invalid character found in format string."),
        };
    }
    private static string GetFormatSpecifierFor(object o)
    {
        return o switch
        {
            bool => "?",
            byte => "B",
            sbyte => "b",
            byte[] => $"{((byte[])o).Length}s",
            char => "c",
            char[] => $"{((char[])o).Length}s",
            double => "d",
            float => "f",
            int => "i",
            uint => "I",
            long => "q",
            ulong => "Q",
            short => "h",
            ushort => "H",
            _ => throw new ArgumentException("Unsupported object type found")
        };
    }
    /// <summary>
    /// We use this function to provide an easier way to type-agnostically call the GetBytes method of the BitConverter class.
    /// This means we can have much cleaner code.
    /// </summary>
    /// <param name="o"></param>
    /// <returns><see cref="byte[]"/></returns>
    /// <exception cref="ArgumentException"></exception>
    private static byte[] TypeAgnosticGetBytes(object o)
    {
        return o switch
        {
            bool => BitConverter.GetBytes((bool)o),
            byte or sbyte => [(byte)o],
            char => BitConverter.GetBytes((char)o),
            double => BitConverter.GetBytes((double)o),
            float => BitConverter.GetBytes((float)o),
            int => BitConverter.GetBytes((int)o),
            uint => BitConverter.GetBytes((uint)o),
            long => BitConverter.GetBytes((long)o),
            ulong => BitConverter.GetBytes((ulong)o),
            short => BitConverter.GetBytes((short)o),
            ushort => BitConverter.GetBytes((ushort)o),
            _ => throw new ArgumentException("Unsupported object type found")
        };
    }
}
