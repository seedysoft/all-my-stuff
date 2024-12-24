using Xunit;

namespace Seedysoft.Libs.Cryptography.Tests;

public sealed class CryptoTests
{
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.")]
    [Theory]
    public void EncryptThenDecryptTest(string textToEncrypt)
    {
        string Key = System.Security.Cryptography.RandomNumberGenerator.GetString("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 32);

        string encryptedBytes = Crypto.EncryptText(textToEncrypt, Key);
        System.Diagnostics.Debug.WriteLine(encryptedBytes);

        string decryptedText = Crypto.DecryptText(encryptedBytes, Key);
        System.Diagnostics.Debug.WriteLine(decryptedText);

        Assert.Equal(textToEncrypt, decryptedText);
    }
}
