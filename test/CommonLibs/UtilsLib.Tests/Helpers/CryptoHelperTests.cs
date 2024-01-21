using Seedysoft.UtilsLib.Helpers;

namespace Seedysoft.UtilsLib.Tests.Helpers;

public sealed class CryptoHelperTests(CryptoHelperTestsFixture cipherFixture)
    : IClassFixture<CryptoHelperTestsFixture>
{
    public CryptoHelperTestsFixture CipherFixture { get; } = cipherFixture;

    [Fact]
    public void EncryptThenDecryptTest()
    {
        Span<byte> plainTextBytes = System.Text.Encoding.Latin1.GetBytes(CipherFixture.TextToEncrypt);
        Span<byte> encryptedBytes = CryptoHelper.Encrypt(CipherFixture.Key, plainTextBytes);
        System.Diagnostics.Debug.WriteLine(Convert.ToHexString(encryptedBytes));

        Span<byte> decryptedBytes = CryptoHelper.Decrypt(CipherFixture.Key, encryptedBytes);
        string decryptedText = System.Text.Encoding.Latin1.GetString(decryptedBytes);
        System.Diagnostics.Debug.WriteLine(decryptedText);

        Assert.Equal(CipherFixture.TextToEncrypt, decryptedText);
    }
}

public sealed class CryptoHelperTestsFixture
{
    public string TextToEncrypt { get; }
    public byte[] Key { get; }

    public CryptoHelperTestsFixture()
    {
        TextToEncrypt = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.";
        Key = System.Security.Cryptography.RandomNumberGenerator.GetBytes(CryptoHelper.KeySize);
    }
}
