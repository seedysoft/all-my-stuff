namespace Seedysoft.Libs.Cryptography.Tests;

public sealed class CryptoTests : Core.Tests.TUnitTestClassBase
{
    //[Test]
    //[CombinedDataSources]
    //public async Task EncryptTextThenDecryptTextTest(
    //    [Arguments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.")]
    //    string textToEncrypt)
    //{
    //    byte[] bytes = GetKey(32).ToArray();

    //    string encryptedText = Crypto.EncryptText(textToEncrypt, bytes);
    //    Console.WriteLine(encryptedText);

    //    string decryptedText = Crypto.DecryptText(encryptedText, bytes);
    //    Console.WriteLine(decryptedText);

    //    _ = await Assert.That(textToEncrypt).IsEqualTo(decryptedText);
    //}

    [Test]
    [CombinedDataSources]
    public async Task EncryptThenDecryptTest(
        [Arguments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.")]
        string textToEncrypt)
    {
        ReadOnlySpan<byte> bytes = GetKey(32);

        string encryptedText = Crypto.Encrypt(bytes, textToEncrypt);
        Console.WriteLine(encryptedText);

        string decryptedText = Crypto.Decrypt(bytes, encryptedText);
        Console.WriteLine(decryptedText);

        _ = await Assert.That(textToEncrypt).IsEqualTo(decryptedText);
    }

    private static ReadOnlySpan<byte> GetKey(int length) => System.Security.Cryptography.RandomNumberGenerator.GetBytes(length);
}
