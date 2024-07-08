using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Seedysoft.Libs.Cryptography.Tests;

[TestClass]
public sealed class CryptoTests
{
    [TestMethod]
    public void EncryptThenDecryptTest()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        string TextToEncrypt = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.";
        string Key = System.Security.Cryptography.RandomNumberGenerator.GetString(chars, 32);

        string encryptedBytes = Crypto.EncryptText(TextToEncrypt, Key);
        System.Diagnostics.Debug.WriteLine(encryptedBytes);

        string decryptedText = Crypto.DecryptText(encryptedBytes, Key);
        System.Diagnostics.Debug.WriteLine(decryptedText);

        Assert.AreEqual(TextToEncrypt, decryptedText);
    }
}
