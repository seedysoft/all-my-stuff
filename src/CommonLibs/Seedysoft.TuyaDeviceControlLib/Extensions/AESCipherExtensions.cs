using System.Text;

namespace Seedysoft.TuyaDeviceControlLib.Extensions;

internal static class AESCipherExtensions
{
    internal static byte[]? GetEncryptionInitVector(byte[]? iv)
    {
        return iv == null || iv.Length == 0
            ? iv
            : Encoding.UTF8.GetBytes(((int)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds * 10).ToString()[..12]);
    }

    internal static (byte[]?, byte[]) GetDecryptionInitVector(byte[]? iv, byte[] data)
    {
        return iv == null || iv.Length == 0
            ? ((byte[]?, byte[]))(iv, data)
            : (data[..12], data[12..]);
    }

    internal static byte[] Pad(byte[] s, int bs)
    {
        int padnum = bs - (s.Length % bs);
        byte padbyte = (byte)padnum;

        return [.. s, .. Enumerable.Repeat(padbyte, padnum)];
    }

    internal static byte[] Unpad(byte[] s, bool verifyPadding = false)
    {
        //int padlen = s[s.Length - 1];
        int padlen = s[^1];

        return padlen switch
        {
            < 1 or > 16 => throw new ArgumentException("invalid padding length byte"),

            _ => verifyPadding && !s.Skip(s.Length - padlen).SequenceEqual(Enumerable.Repeat((byte)padlen, padlen))
                ? throw new ArgumentException("invalid padding data")
                : s.Take(s.Length - padlen).ToArray(),
        };
    }
}
