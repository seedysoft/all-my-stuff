namespace Seedysoft.Libs.TuyaDeviceControl.Extensions;

public static class TuyaExtensions
{
    /// <summary>
    /// Unpacks just the header part of a message into a TuyaHeader().
    /// </summary>
    /// <param name="data"></param>
    /// <returns><see cref="TuyaHeader"/></returns>
    /// <exception cref="DecodeException"></exception>
    internal static TuyaHeader ParseHeader(byte[] data)
    {
        string headerFormat = data[..4] == ProtocolVersionsAndHeaders.PREFIX_6699_BIN
            ? ProtocolVersionsAndHeaders.MESSAGE_HEADER_FMT_6699
            : ProtocolVersionsAndHeaders.MESSAGE_HEADER_FMT_55AA;

        int headerLength = StructConverter.CalcSize(headerFormat);

        if (data.Length < headerLength)
            throw new Exceptions.DecodeException("Not enough data to unpack header");

        uint seqno;
        uint cmd;
        uint payloadLength;
        uint totalLength;

        byte[][] unpacked = StructConverter.Unpack(headerFormat, data[..headerLength]);
        //for (int i = 0; i < unpacked.Length; i++)
        //    Debug.WriteLine($"unpacked[{i}]={Convert.ToHexString(unpacked[i])}");
        uint prefix = Convert.ToUInt32(Convert.ToHexString(unpacked[0]), 16);

        if (prefix == ProtocolVersionsAndHeaders.PREFIX_55AA_VALUE)
        {
            // #prefix, seqno, cmd, payload_len = unpacked
            seqno = Convert.ToUInt32(Convert.ToHexString(unpacked[1]), 16);
            cmd = Convert.ToUInt32(Convert.ToHexString(unpacked[2]), 16);
            payloadLength = Convert.ToUInt32(Convert.ToHexString(unpacked[3]), 16);
            totalLength = (uint)(payloadLength + headerLength);
        }
        else if (prefix == ProtocolVersionsAndHeaders.PREFIX_6699_VALUE)
        {
            // #prefix, unknown, seqno, cmd, payload_len = unpacked
            // #//seqno |= unknown << 32
            System.Diagnostics.Debug.WriteLine($"header unknown field='{Convert.ToHexString(unpacked[1])}'", 16);
            seqno = Convert.ToUInt32(Convert.ToHexString(unpacked[2]), 16);
            cmd = Convert.ToUInt32(Convert.ToHexString(unpacked[3]), 16);
            payloadLength = Convert.ToUInt32(Convert.ToHexString(unpacked[4]), 16);
            totalLength = (uint)(payloadLength + headerLength + ProtocolVersionsAndHeaders.SUFFIX_6699_BIN.Length);
        }
        else
        {
            throw new Exceptions.DecodeException($"Header prefix wrong! {prefix:X8} is not {ProtocolVersionsAndHeaders.SUFFIX_55AA_VALUE:X8} nor {ProtocolVersionsAndHeaders.SUFFIX_6699_VALUE:X8}");
        }

        // #sanity check. currently the max payload length is somewhere around 300 bytes
        return payloadLength > 1_000
            ? throw new Exceptions.DecodeException($"Header claims the packet size is over 1000 bytes! It is most likely corrupt. Claimed size: {payloadLength} bytes. fmt:{headerFormat} unpacked:{unpacked}")
            : new TuyaHeader(prefix, seqno, cmd, payloadLength, totalLength);
    }
}
