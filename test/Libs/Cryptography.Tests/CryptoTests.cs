using Xunit;

namespace Seedysoft.Libs.Cryptography.Tests;

public sealed class CryptoTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper) : Core.Tests.XUnitTestClassBase(testOutputHelper)
{
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nulla tellus, elementum sit amet nunc.")]
    [Theory]
    public void EncryptThenDecryptTest(string textToEncrypt)
    {
        string Key = System.Security.Cryptography.RandomNumberGenerator.GetString("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 32);

        string encryptedText = Crypto.EncryptText(textToEncrypt, Key);
        TestOutputHelper.WriteLine(encryptedText);

        string decryptedText = Crypto.DecryptText(encryptedText, Key);
        TestOutputHelper.WriteLine(decryptedText);

        Assert.Equal(textToEncrypt, decryptedText);
    }

    [InlineData("BwIllPBt5jYQNI4cSUS/5RXVIkco/jzejRzw3Zto+Z+dICI2G5hdvtZJXWX5Pgtz/XryRkuWvO6bYkQ5ZRcbog==")]
    [InlineData("T//joAXGQWjFV5A8tXWNupL4LY2VIlxlQwootELcKxg=")]
    [InlineData("dlCflk9MN8tKc9n907vjLYUMTC/SOGbH5iMDs59A3zp2EYPfiFgJWXUxDUW1d+faWY5nL7Lm0ZdGzfNn0NoDWQ==")]
    [InlineData("QKgHgZZSz203oX8THS7+SeXN9+guLQ9Z5nwtPSLXm9N+d5Zl8aHcJWENfN0fNzPr")]
    [InlineData("onk5Cdizg5zdfGYZrevy/euwPw8+6ud+xnmJHL5ttSE=")]
    [InlineData("Le6SHuow241NWkGryu3Oxw1zxU3W764ZsYnI8DIS6qIqySuvjfG+VC21vQfI9LRHz7Z5OtHB5+ueOldpAxw4SQ==")]
    [InlineData("SCBuUEtXdttS8y7PmF7kMbKxX1XffEwyJDGi9Pup1fMPbeEN7h9NoiEnljgjExoc")]
    [InlineData("/ocVZWJEaAKt9flYpvqBk1mnEyaiAf/czLUi/uM4NPY=")]
    [InlineData("YELrW9up56lqpc3AsdmF/aZATyIg1ezCNd/4nzL8nr2K0C7fN0vgNpd0F2w26wVK")]
    [InlineData("glciZvLOPOCZSeNiWATEH/7rffo+16DEyTi4wOOmzO8wnSDtR1+d1wS8T8kEcPGU")]
    [Theory]
    public void ChangePasswordsCipherMode(string pass)
    {
        if (Crypto.CanDecryptText(pass, Core.Helpers.EnvironmentHelper.GetMasterKey(), System.Security.Cryptography.CipherMode.ECB))
        {
            string decryptedText = Crypto.DecryptText(pass, Core.Helpers.EnvironmentHelper.GetMasterKey(), System.Security.Cryptography.CipherMode.ECB);

            string encryptedText = Crypto.EncryptText(decryptedText, Core.Helpers.EnvironmentHelper.GetMasterKey());

            TestOutputHelper.WriteLine($"{pass[..8]}... should be: {encryptedText}");

            Assert.Fail();
        }

        Assert.True(Crypto.CanDecryptText(pass, Core.Helpers.EnvironmentHelper.GetMasterKey()));
    }
}
