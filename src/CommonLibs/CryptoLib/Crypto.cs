using System.Security.Cryptography;

namespace Seedysoft.CryptoLib;

public static class Crypto
{
    private static readonly System.Text.Encoding TextEncoding = System.Text.Encoding.Latin1;

    public static string EncryptText(string textToEncrypt, string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(textToEncrypt);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return Convert.ToBase64String(EncryptBytes(TextEncoding.GetBytes(textToEncrypt), Convert.FromBase64String(key)));
    }

    public static string DecryptText(string encryptedText, string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedText);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return TextEncoding.GetString(DecryptBytes(Convert.FromBase64String(encryptedText), Convert.FromBase64String(key)));
    }

    private static byte[] EncryptBytes(byte[] inputBuffer, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(inputBuffer);

        byte[] iv;
        byte[] cipherText;

        using (Aes cipher = BuildCryptographicObject(key))
        {
            using ICryptoTransform symmetricEncryptor = cipher.CreateEncryptor();
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

    private static byte[] DecryptBytes(byte[] encryptedBytes, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(encryptedBytes);

        using Aes cipher = BuildCryptographicObject(key);
        int cipherTextOffset = cipher.IV.Length;

        byte[] iv = new byte[cipherTextOffset];
        Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
        cipher.IV = iv;

        using ICryptoTransform decryptor = cipher.CreateDecryptor();

        return Transform(decryptor, encryptedBytes, cipherTextOffset, encryptedBytes.Length - cipherTextOffset);
    }

    private static byte[] Transform(ICryptoTransform cryptoTransform, byte[] inputBuffer, int inputOffset, int inputCount)
    {
        ArgumentNullException.ThrowIfNull(cryptoTransform);
        ArgumentNullException.ThrowIfNull(inputBuffer);
        ArgumentOutOfRangeException.ThrowIfLessThan(inputOffset, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(inputCount, 0);

        if (cryptoTransform.CanTransformMultipleBlocks)
            return cryptoTransform.TransformFinalBlock(inputBuffer, inputOffset, inputCount);

        using MemoryStream memoryStream = new();
        using CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        cryptoStream.Write(inputBuffer, inputOffset, inputCount);
        cryptoStream.FlushFinalBlock();

        return memoryStream.ToArray();
    }

    private static Aes BuildCryptographicObject(byte[] key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var aes = Aes.Create();
        aes.Key = key; //aes.KeySize = masterKey.Length * 8L;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.ISO10126;

        return aes;
    }

    private static void AppendBytes(byte[] newData, byte[] combinedData, ref int writeOffset)
    {
        Buffer.BlockCopy(newData, 0, combinedData, writeOffset, newData.Length);
        writeOffset += newData.Length;
    }
}
