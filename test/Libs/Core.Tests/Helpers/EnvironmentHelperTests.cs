namespace Seedysoft.Libs.Core.Tests.Helpers;

public class EnvironmentHelperTests : Libs.Tests.TUnitTestClassBase
{
    [Test]
    [CombinedDataSources]
    public async Task ChangePasswordsCipherModeTest(
        //[Arguments("yBO1GqKyHGwNNyc2BQx4gjkVYD770H7d0PJ3kqCXi2hYvNzE+MbK2wGlIdfPgcUd")]
        [Arguments("T//joAXGQWjFV5A8tXWNupL4LY2VIlxlQwootELcKxg=")]
        [Arguments("dlCflk9MN8tKc9n907vjLYUMTC/SOGbH5iMDs59A3zp2EYPfiFgJWXUxDUW1d+faWY5nL7Lm0ZdGzfNn0NoDWQ==")]
        [Arguments("QKgHgZZSz203oX8THS7+SeXN9+guLQ9Z5nwtPSLXm9N+d5Zl8aHcJWENfN0fNzPr")]
        [Arguments("onk5Cdizg5zdfGYZrevy/euwPw8+6ud+xnmJHL5ttSE=")]
        [Arguments("Le6SHuow241NWkGryu3Oxw1zxU3W764ZsYnI8DIS6qIqySuvjfG+VC21vQfI9LRHz7Z5OtHB5+ueOldpAxw4SQ==")]
        [Arguments("SCBuUEtXdttS8y7PmF7kMbKxX1XffEwyJDGi9Pup1fMPbeEN7h9NoiEnljgjExoc")]
        [Arguments("/ocVZWJEaAKt9flYpvqBk1mnEyaiAf/czLUi/uM4NPY=")]
        [Arguments("YELrW9up56lqpc3AsdmF/aZATyIg1ezCNd/4nzL8nr2K0C7fN0vgNpd0F2w26wVK")]
        [Arguments("PROra9eGTBudL8YmXFM4gkqVelfR97p564IX1lp32Mg=")]
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

        Console.WriteLine($"Old: '{decryptedText}'    New: '{encrypted}'");

        _ = await Assert.That(decrypted).IsNotDefault().And.IsEqualTo(decryptedText);
    }

    [Test]
    [CombinedDataSources]
    public async Task ChangeEncryptionTest(
        [Arguments("seedysoft@gmail.com")]
        string pass)
    {
        string encryptedText = Core.Helpers.EnvironmentHelper.Encrypt(pass);

        _ = await Assert.That(encryptedText).IsNotDefault();
    }
}
