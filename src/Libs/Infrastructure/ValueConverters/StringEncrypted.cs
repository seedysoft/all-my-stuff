using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Seedysoft.Libs.Infrastructure.ValueConverters;

public static class StringEncrypted
{
    public static ValueConverter<string, string> StringEncryptedValueConverter
    {
        get
        {
            return new ValueConverter<string, string>(
                convertToProviderExpression: static plainText => Encrypt(plainText)!,
                convertFromProviderExpression: static encryptedText => Decrypt(encryptedText)!);
        }
    }

    public static ValueConverter<string?, string?> NullableStringEncryptedValueConverter
    {
        get
        {
            return new ValueConverter<string?, string?>(
                convertToProviderExpression: static plainText => Encrypt(plainText),
                convertFromProviderExpression: static encryptedText => Decrypt(encryptedText));
        }
    }

    private static string? Decrypt(string? encrypted)
    {
        if (string.IsNullOrWhiteSpace(encrypted))
            return null;

        string s = Cryptography.Crypto.CanDecryptText(encrypted, Core.Helpers.EnvironmentHelper.GetMasterKey())
            ? Cryptography.Crypto.DecryptText(encrypted, Core.Helpers.EnvironmentHelper.GetMasterKey())
            : encrypted;

        return s;
    }
    private static string? Encrypt(string? text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? null
            : Cryptography.Crypto.EncryptText(text, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }
}
