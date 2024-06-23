using System.Text;

namespace Seedysoft.Libs.TuyaDeviceControl;

internal class ProtocolVersionsAndHeaders
{
    public const string VERSION_31 = "3.1";
    public const string VERSION_32 = "3.2";
    public const string VERSION_33 = "3.3";
    public const string VERSION_34 = "3.4";
    public const string VERSION_35 = "3.5";
    public static readonly byte[] PROTOCOL_VERSION_BYTES_31 = Encoding.ASCII.GetBytes(VERSION_31);
    public static readonly byte[] PROTOCOL_VERSION_BYTES_32 = Encoding.ASCII.GetBytes(VERSION_32);
    public static readonly byte[] PROTOCOL_VERSION_BYTES_33 = Encoding.ASCII.GetBytes(VERSION_33);
    public static readonly byte[] PROTOCOL_VERSION_BYTES_34 = Encoding.ASCII.GetBytes(VERSION_34);
    public static readonly byte[] PROTOCOL_VERSION_BYTES_35 = Encoding.ASCII.GetBytes(VERSION_35);
    public static readonly byte[] PROTOCOL_3x_HEADER = new byte[12];
    public static readonly byte[] PROTOCOL_33_HEADER = [.. PROTOCOL_VERSION_BYTES_33, .. PROTOCOL_3x_HEADER];
    public static readonly byte[] PROTOCOL_34_HEADER = [.. PROTOCOL_VERSION_BYTES_34, .. PROTOCOL_3x_HEADER];
    public static readonly byte[] PROTOCOL_35_HEADER = [.. PROTOCOL_VERSION_BYTES_35, .. PROTOCOL_3x_HEADER];

    /// <summary>
    /// 4*uint32: prefix, seqno, cmd, length [, retcode].
    /// </summary>
    public const string MESSAGE_HEADER_FMT = ">4I";
    /// <summary>
    /// 4*uint32: prefix, seqno, cmd, length [, retcode].
    /// </summary>
    public const string MESSAGE_HEADER_FMT_55AA = MESSAGE_HEADER_FMT;
    /// <summary>
    /// // 4*uint32: prefix, unknown, seqno, cmd, length.
    /// </summary>
    public const string MESSAGE_HEADER_FMT_6699 = ">IHIII";
    /// <summary>
    /// retcode for received messages.
    /// </summary>
    public const string MESSAGE_RETCODE_FMT = ">I";
    /// <summary>
    /// 2*uint32: crc, suffix.
    /// </summary>
    public const string MESSAGE_END_FMT = ">2I";
    /// <summary>
    /// 2*uint32: crc, suffix.
    /// </summary>
    public const string MESSAGE_END_FMT_55AA = MESSAGE_END_FMT;
    /// <summary>
    /// 32s:hmac, uint32:suffix
    /// </summary>
    /// <remarks>'10s' means a single 10-byte string mapping to or from a single Python byte string.</remarks>
    public const string MESSAGE_END_FMT_HMAC = ">32sI";
    /// <summary>
    /// 16s:tag, uint32:suffix
    /// </summary>
    public const string MESSAGE_END_FMT_6699 = ">16sI";

    public const uint PREFIX_VALUE = 0x000055aa;
    public const uint PREFIX_55AA_VALUE = PREFIX_VALUE;
    public static readonly byte[] PREFIX_BIN = [0x00, 0x00, 0x55, 0xaa];
    public static readonly byte[] PREFIX_55AA_BIN = PREFIX_BIN;
    public const uint PREFIX_6699_VALUE = 0x00006699;
    public static readonly byte[] PREFIX_6699_BIN = [0x00, 0x00, 0x66, 0x99];

    public const uint SUFFIX_VALUE = 0x0000aa55;
    public const uint SUFFIX_55AA_VALUE = SUFFIX_VALUE;
    public static readonly byte[] SUFFIX_BIN = [0x00, 0x00, 0xaa, 0x55];
    public static readonly byte[] SUFFIX_55AA_BIN = SUFFIX_BIN;
    public const uint SUFFIX_6699_VALUE = 0x00009966;
    public static readonly byte[] SUFFIX_6699_BIN = [0x00, 0x00, 0x99, 0x66];
}
