namespace Seedysoft.Libs.Cryptography;

public static class Crypto
{
    private static readonly System.Text.Encoding Encoding = System.Text.Encoding.Latin1;

    internal static bool CanEncryptText(
        string textToEncrypt,
        string key,
        System.Security.Cryptography.CipherMode cipherMode = System.Security.Cryptography.CipherMode.CBC)
    {
        try
        {
            byte[] textBytes = Encoding.GetBytes(textToEncrypt);
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] encryptedBytes = EncryptBytes(textBytes, keyBytes, cipherMode);
            string encryptedText = Convert.ToBase64String(encryptedBytes);

            return true;
        }
        catch { }

        return false;
    }
    internal static string EncryptText(
        string textToEncrypt,
        string key,
        System.Security.Cryptography.CipherMode cipherMode = System.Security.Cryptography.CipherMode.CBC)
    {
        return CanEncryptText(textToEncrypt, key, cipherMode)
            ? Convert.ToBase64String(EncryptBytes(Encoding.GetBytes(textToEncrypt), Convert.FromBase64String(key), cipherMode))
            : throw new InvalidDataException($"Cannot Encrypt {textToEncrypt} with {key} key and mode {cipherMode}");
    }

    internal static bool CanDecryptText(
        string encryptedText,
        string key,
        System.Security.Cryptography.CipherMode cipherMode = System.Security.Cryptography.CipherMode.CBC)
    {
        if (string.IsNullOrWhiteSpace(encryptedText) || string.IsNullOrWhiteSpace(key))
            return false;

        try
        {
            byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] decryptedBytes = DecryptBytes(encryptedTextBytes, keyBytes, cipherMode);
            string decryptedText = Encoding.GetString(decryptedBytes);

            return true;
        }
        catch (Exception) { /* ignored */ }

        return false;
    }
    public static string DecryptText(
        string encryptedText,
        string key,
        System.Security.Cryptography.CipherMode cipherMode = System.Security.Cryptography.CipherMode.CBC)
    {
        return CanDecryptText(encryptedText, key, cipherMode)
            ? Encoding.GetString(DecryptBytes(Convert.FromBase64String(encryptedText), Convert.FromBase64String(key), cipherMode))
            : throw new InvalidDataException($"Cannot Decrypt {encryptedText} with {key} key and mode {cipherMode}");
    }

    private static byte[] EncryptBytes(
        byte[] inputBuffer,
        byte[] key,
        System.Security.Cryptography.CipherMode cipherMode)
    {
        ArgumentNullException.ThrowIfNull(inputBuffer);

        byte[] iv;
        byte[] cipherText;

        using (System.Security.Cryptography.Aes cipher = BuildCryptographicObject(key, cipherMode))
        {
            using System.Security.Cryptography.ICryptoTransform symmetricEncryptor = cipher.CreateEncryptor();
            iv = cipher.IV;

            cipherText = Transform(symmetricEncryptor, inputBuffer, 0, inputBuffer.Length);
        }

        int totalLength = iv.Length + cipherText.Length;

        byte[] combinedData = new byte[totalLength];
        int outputOffset = 0;

        AppendBytes(iv, combinedData, ref outputOffset);
        AppendBytes(cipherText, combinedData, ref outputOffset);

        System.Diagnostics.Debug.Assert(outputOffset == combinedData.Length);

        return combinedData;
    }

    private static byte[] DecryptBytes(
        byte[] encryptedBytes,
        byte[] key,
        System.Security.Cryptography.CipherMode cipherMode)
    {
        ArgumentNullException.ThrowIfNull(encryptedBytes);

        using System.Security.Cryptography.Aes cipher = BuildCryptographicObject(key, cipherMode);
        int cipherTextOffset = cipher.IV.Length;

        byte[] iv = new byte[cipherTextOffset];
        Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
        cipher.IV = iv;

        using System.Security.Cryptography.ICryptoTransform decryptor = cipher.CreateDecryptor();

        return Transform(decryptor, encryptedBytes, cipherTextOffset, encryptedBytes.Length - cipherTextOffset);
    }

    private static byte[] Transform(
        System.Security.Cryptography.ICryptoTransform cryptoTransform,
        byte[] inputBuffer,
        int inputOffset,
        int inputCount)
    {
        ArgumentNullException.ThrowIfNull(cryptoTransform);
        ArgumentNullException.ThrowIfNull(inputBuffer);
        ArgumentOutOfRangeException.ThrowIfLessThan(inputOffset, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(inputCount, 0);

        if (cryptoTransform.CanTransformMultipleBlocks)
            return cryptoTransform.TransformFinalBlock(inputBuffer, inputOffset, inputCount);

        using MemoryStream memoryStream = new();
        using System.Security.Cryptography.CryptoStream cryptoStream =
            new(memoryStream, cryptoTransform, System.Security.Cryptography.CryptoStreamMode.Write);
        cryptoStream.Write(inputBuffer, inputOffset, inputCount);
        cryptoStream.FlushFinalBlock();

        return memoryStream.ToArray();
    }

    private static System.Security.Cryptography.Aes BuildCryptographicObject(
        byte[] key,
        System.Security.Cryptography.CipherMode cipherMode)
    {
        ArgumentNullException.ThrowIfNull(key);

        var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key; //aes.KeySize = masterKey.Length * 8L;
        aes.Mode = cipherMode;
        aes.Padding = System.Security.Cryptography.PaddingMode.ISO10126;

        return aes;
    }

    private static void AppendBytes(byte[] newData, byte[] combinedData, ref int writeOffset)
    {
        Buffer.BlockCopy(newData, 0, combinedData, writeOffset, newData.Length);
        writeOffset += newData.Length;
    }
}
