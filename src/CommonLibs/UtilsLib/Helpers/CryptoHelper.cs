namespace Seedysoft.UtilsLib.Helpers;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// https://github.com/Bobris/BTDB/blob/380f955577b6abd1f7e0915b1f7ee5d4fdadf865/BTDB/Encrypted/AesGcmSymmetricCipher.cs#L18
/// </remarks>
public sealed class CryptoHelper
{
    public const int KeySize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly static object _lock = new();

    public static Span<byte> Encrypt(byte[] key, ReadOnlySpan<byte> plainBytes)
    {
        Span<byte> encryptedBytes = new byte[CryptoHelper.CalcSizeForEncrypted(plainBytes)];

        try
        {
            using System.Security.Cryptography.AesGcm _aes = new(key, TagSize);
            lock (_lock)
            {
                System.Security.Cryptography.RandomNumberGenerator.Fill(encryptedBytes[..NonceSize]);
                encryptedBytes[0] &= 0x0f; // 4 bits left for future algorithm type
                _aes.Encrypt(
                    encryptedBytes[..NonceSize],
                    plainBytes,
                    encryptedBytes[(NonceSize + TagSize)..],
                    encryptedBytes[NonceSize..(NonceSize + TagSize)]);
            }
        }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine(e); }

        return encryptedBytes;
    }

    public static Span<byte> Decrypt(byte[] key, ReadOnlySpan<byte> encryptedBytes)
    {
        if (encryptedBytes.Length < NonceSize + TagSize || (encryptedBytes[0] & 0xf0) != 0)
            return encryptedBytes.ToArray();

        try
        {
            Span<byte> decryptedBytes = new byte[CryptoHelper.CalcSizeForPlain(encryptedBytes)];

            lock (_lock)
            {
                using System.Security.Cryptography.AesGcm _aes = new(key, TagSize);
                _aes.Decrypt(
                    encryptedBytes[..NonceSize],
                    encryptedBytes.Slice(NonceSize + TagSize, decryptedBytes.Length),
                    encryptedBytes.Slice(NonceSize, TagSize),
                    decryptedBytes);
            }

            return decryptedBytes;
        }
        catch (System.Security.Cryptography.CryptographicException e) { System.Diagnostics.Debug.WriteLine(e); }

        return encryptedBytes.ToArray();
    }

    public static int CalcSizeForEncrypted(ReadOnlySpan<byte> plainInput)
        => NonceSize + TagSize + plainInput.Length;
    public static int CalcSizeForPlain(ReadOnlySpan<byte> encryptedText)
        => Math.Max(0, encryptedText.Length - NonceSize - TagSize);
}
