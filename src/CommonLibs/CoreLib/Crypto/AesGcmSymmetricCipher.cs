namespace Seedysoft.CoreLib.Crypto;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// https://github.com/Bobris/BTDB/blob/380f955577b6abd1f7e0915b1f7ee5d4fdadf865/BTDB/Encrypted/AesGcmSymmetricCipher.cs#L18
/// </remarks>
public sealed class AesGcmSymmetricCipher(byte[] key) : IDisposable
{
    public const int KeySize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly System.Security.Cryptography.AesGcm _aes = new(key, TagSize);
    private readonly object _lock = new();

    public void Encrypt(ReadOnlySpan<byte> plainInputBytes, Span<byte> outputBuffer)
    {
        try
        {
            lock (_lock)
            {
                System.Security.Cryptography.RandomNumberGenerator.Fill(outputBuffer[..NonceSize]);
                outputBuffer[0] &= 0x0f; // 4 bits left for future algorithm type
                _aes.Encrypt(
                    outputBuffer[..NonceSize],
                    plainInputBytes,
                    outputBuffer[(NonceSize + TagSize)/*, plainInputBytes.Length*/..],
                    outputBuffer[NonceSize..(NonceSize + TagSize)]);
            }
        }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine(e); }
    }

    public bool Decrypt(ReadOnlySpan<byte> encryptedInputBytes, Span<byte> outputBuffer)
    {
        if ((encryptedInputBytes.Length < (NonceSize + TagSize)) || ((encryptedInputBytes[0] & 0xf0) != 0))
            return false;

        try
        {
            lock (_lock)
            {
                _aes.Decrypt(
                    encryptedInputBytes[..NonceSize],
                    encryptedInputBytes.Slice(NonceSize + TagSize, outputBuffer.Length),
                    encryptedInputBytes.Slice(NonceSize, TagSize),
                    outputBuffer);
            }

            return true;
        }
        catch (System.Security.Cryptography.CryptographicException e)
        {
            System.Diagnostics.Debug.WriteLine(e);
            return false;
        }
    }

    public static int CalcSizeForEncrypted(ReadOnlySpan<byte> plainInput)
        => NonceSize + TagSize + plainInput.Length;
    public static int CalcSizeForPlain(ReadOnlySpan<byte> encryptedText)
        => Math.Max(0, encryptedText.Length - NonceSize - TagSize);

    public void Dispose() => _aes.Dispose();
}
