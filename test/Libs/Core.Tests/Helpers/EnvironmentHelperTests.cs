namespace Seedysoft.Libs.Core.Tests.Helpers;

public class EnvironmentHelperTests : Libs.Tests.TUnitTestClassBase
{
    [Test]
    [CombinedDataSources]
    public async Task ChangeEncryptionTest(
        [Arguments("5025594021")]
        string pass)
    {
        string encrypted = Core.Helpers.EnvironmentHelper.Encrypt(pass);

        Console.WriteLine($"Old: '{pass}'    New: '{encrypted}'");

        _ = await Assert.That(encrypted).IsNotDefault();
    }

    [Test]
    [CombinedDataSources]
    public async Task ChangePasswordsCipherModeTest(
        [Arguments("onk5Cdizg5zdfGYZrevy/euwPw8+6ud+xnmJHL5ttSE=")]
        string pass)
    {
        byte[] key = Core.Helpers.EnvironmentHelper.GetMasterKey().ToArray();

        string decryptedText;
        try
        {
            decryptedText = Core.Helpers.EnvironmentHelper.Decrypt(pass);
        }
        catch (Exception)
        {
            decryptedText = Cryptography.Crypto.DecryptText(pass, key);
        }

        string encrypted = Core.Helpers.EnvironmentHelper.Encrypt(decryptedText);
        string decrypted = Core.Helpers.EnvironmentHelper.Decrypt(encrypted);

        Console.WriteLine($"Old: '{pass}'({decryptedText})    New: '{encrypted}'");

        _ = await Assert.That(decrypted).IsNotDefault().And.IsEqualTo(decryptedText);
    }
}
