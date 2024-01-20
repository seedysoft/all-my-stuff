using System.Diagnostics;

namespace Seedysoft.CoreLib.Crypto.Tests;

public sealed class CryptoHelperTests(CryptoHelperTestsFixture cryptoHelperFixture)
    : IClassFixture<CryptoHelperTestsFixture>
{
    public CryptoHelperTestsFixture CryptoHelperFixture { get; } = cryptoHelperFixture;

    [Fact]
    public void EncryptThenDecryptTest()
    {
        string encryptedText = CryptoHelper.Encrypt(CryptoHelperFixture.TextToEncrypt/*, CryptoHelperFixture.Key, CryptoHelperFixture.Nonce*/);
        Debug.WriteLine(encryptedText);

        string decryptedText = string.Empty;
        try
        {
            decryptedText = CryptoHelper.Decrypt(encryptedText/*, CryptoHelperFixture.Key, CryptoHelperFixture.Nonce*/);
            Debug.WriteLine(decryptedText);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        Assert.Equal(CryptoHelperFixture.TextToEncrypt, decryptedText);
    }
}

public sealed class CryptoHelperTestsFixture : IDisposable
{
    public CryptoHelperTestsFixture()
    {
        TextToEncrypt = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.";
        //Key = RandomNumberGenerator.GetBytes(AesGcm.TagByteSizes.MaxSize);
        //Nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize);
    }

    public string TextToEncrypt { get; }
    //public byte[] Key { get; }
    //public byte[] Nonce { get; }

    public void Dispose() { }
}
