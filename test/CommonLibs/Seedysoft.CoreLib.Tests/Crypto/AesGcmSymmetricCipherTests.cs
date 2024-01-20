namespace Seedysoft.CoreLib.Crypto.Tests;

public sealed class AesGcmSymmetricCipherTests(AesGcmSymmetricCipherTestsFixture cipherFixture)
    : IClassFixture<AesGcmSymmetricCipherTestsFixture>
{
    public AesGcmSymmetricCipherTestsFixture CipherFixture { get; } = cipherFixture;

    [Fact]
    public void EncryptThenDecryptTest()
    {
        string encryptedText;
        Span<byte> encryptedBytes;
        using (AesGcmSymmetricCipher aesGcmSymmetricCipher = new(CipherFixture.Key))
        {
            Span<byte> plainTextBytes = System.Text.Encoding.Latin1.GetBytes(CipherFixture.TextToEncrypt);
            encryptedBytes = new byte[AesGcmSymmetricCipher.CalcSizeForEncrypted(plainTextBytes)];
            aesGcmSymmetricCipher.Encrypt(plainTextBytes, encryptedBytes);
            encryptedText = System.Text.Encoding.Latin1.GetString(encryptedBytes);
            System.Diagnostics.Debug.WriteLine(Convert.ToHexString(encryptedBytes));
        }

        string decryptedText = string.Empty;
        using (AesGcmSymmetricCipher aesGcmSymmetricCipher = new(CipherFixture.Key))
        {
            Span<byte> decryptedBytes = new byte[AesGcmSymmetricCipher.CalcSizeForPlain(encryptedBytes)];
            _ = aesGcmSymmetricCipher.Decrypt(encryptedBytes, decryptedBytes);
            decryptedText = System.Text.Encoding.Latin1.GetString(decryptedBytes);
            System.Diagnostics.Debug.WriteLine(decryptedText);
        }

        Assert.Equal(CipherFixture.TextToEncrypt, decryptedText);
    }
}

public sealed class AesGcmSymmetricCipherTestsFixture
{
    public string TextToEncrypt { get; }
    public byte[] Key { get; }

    public AesGcmSymmetricCipherTestsFixture()
    {
        TextToEncrypt = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.";
        Key = System.Security.Cryptography.RandomNumberGenerator.GetBytes(AesGcmSymmetricCipher.KeySize);
    }
}
