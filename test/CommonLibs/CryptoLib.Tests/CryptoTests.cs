namespace Seedysoft.CryptoLib.Tests;

public sealed class CryptoTests(CryptoTestsFixture cipherFixture)
    : IClassFixture<CryptoTestsFixture>
{
    public CryptoTestsFixture CipherFixture { get; } = cipherFixture;

    [Fact]
    public void EncryptThenDecryptTest()
    {
        string encryptedBytes = Crypto.EncryptText(CipherFixture.TextToEncrypt, CipherFixture.Key);
        System.Diagnostics.Debug.WriteLine(encryptedBytes);

        string decryptedText = Crypto.DecryptText(encryptedBytes, CipherFixture.Key);
        System.Diagnostics.Debug.WriteLine(decryptedText);

        Assert.Equal(CipherFixture.TextToEncrypt, decryptedText);
    }
}

public sealed class CryptoTestsFixture
{
    public string TextToEncrypt { get; }
    public string Key { get; }

    public CryptoTestsFixture()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        TextToEncrypt = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.";
        Key = System.Security.Cryptography.RandomNumberGenerator.GetString(chars, 32);
    }
}
