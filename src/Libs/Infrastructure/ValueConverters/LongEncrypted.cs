using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Seedysoft.Libs.Infrastructure.ValueConverters;

public static class LongEncrypted
{
    public static ValueConverter<long, string> StringEncryptedValueConverter
    {
        get
        {
            return new ValueConverter<long, string>(
                convertToProviderExpression: static plainLong => Encrypt(plainLong)!,
                convertFromProviderExpression: static encryptedText => Decrypt(encryptedText)!.Value);
        }
    }

    public static ValueConverter<long?, string?> NullableStringEncryptedValueConverter
    {
        get
        {
            return new ValueConverter<long?, string?>(
                convertToProviderExpression: static plainLong => Encrypt(plainLong),
                convertFromProviderExpression: static encryptedText => Decrypt(encryptedText));
        }
    }

    private static long? Decrypt(string? encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            return null;

        string s =
            Cryptography.Crypto.CanDecryptText(encryptedText, Core.Helpers.EnvironmentHelper.GetMasterKey())
            ? Cryptography.Crypto.DecryptText(encryptedText, Core.Helpers.EnvironmentHelper.GetMasterKey())
            : encryptedText;

        return long.Parse(s);
    }
    private static string? Encrypt(long? l)
    {
        return l.HasValue
            ? Cryptography.Crypto.EncryptText(l.Value.ToString(), Core.Helpers.EnvironmentHelper.GetMasterKey())
            : null;
    }
}
