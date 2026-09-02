namespace Seedysoft.Libs.Cryptography;

public static class Crypto
{
    private static readonly System.Text.Encoding Encoding = System.Text.Encoding.Latin1;

    private static bool CanEncryptText(string textToEncrypt, string key, CipherMode cipherMode = CipherMode.CBC)
    {
        try
        {
            byte[] textBytes = Encoding.GetBytes(textToEncrypt);
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] encryptedBytes = EncryptBytes(textBytes, keyBytes, cipherMode);
            string encryptedText = Convert.ToBase64String(encryptedBytes);

    //    return false;
    //}

        return false;
    }
    internal static string EncryptText(string textToEncrypt, string key, CipherMode cipherMode = CipherMode.CBC)
    {
        return CanEncryptText(textToEncrypt, key, cipherMode)
            ? Convert.ToBase64String(EncryptBytes(Encoding.GetBytes(textToEncrypt), Convert.FromBase64String(key), cipherMode))
            : throw new InvalidDataException($"Cannot Encrypt {textToEncrypt} with {key} key and mode {cipherMode}");
    }

    internal static bool CanDecryptText(string encryptedText, string key, CipherMode cipherMode = CipherMode.CBC)
    {
        if (!string.IsNullOrWhiteSpace(encryptedText) && !string.IsNullOrWhiteSpace(key))
        {
            try
            {
                byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = Convert.FromBase64String(key);
                byte[] decryptedBytes = DecryptBytes(Convert.FromBase64String(encryptedText), Convert.FromBase64String(key), cipherMode);
                string decryptedText = Encoding.GetString(decryptedBytes);

                return true;
            }
            catch { }
        }
        catch (Exception) { /* ignored */ }

        return false;
    }

    public static string DecryptText(string encryptedText, byte[] key)
    {
        return CanDecryptText(encryptedText, key)
            ? System.Text.Encoding.Latin1.GetString(DecryptBytes(Convert.FromBase64String(encryptedText), key))
            : throw new InvalidDataException($"Cannot Decrypt '{encryptedText}'");
    }

    private static byte[] DecryptBytes(byte[] encryptedBytes, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(encryptedBytes);

        using System.Security.Cryptography.Aes cipher = BuildCryptographicObject(key);
        int cipherTextOffset = cipher.IV.Length;

        byte[] iv = new byte[cipherTextOffset];
        Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
        cipher.IV = iv;

        using System.Security.Cryptography.ICryptoTransform decryptor = cipher.CreateDecryptor();

        return Transform(decryptor, encryptedBytes, cipherTextOffset, encryptedBytes.Length - cipherTextOffset);
    }

    private static System.Security.Cryptography.Aes BuildCryptographicObject(byte[] key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key; //aes.KeySize = masterKey.Length * 8L;
        aes.Mode = System.Security.Cryptography.CipherMode.CBC;
        aes.Padding = System.Security.Cryptography.PaddingMode.ISO10126;

        return aes;
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

    public static string Encrypt(ReadOnlySpan<byte> key, string plainText)
    {
        byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = EncryptOrDecrypt(isForEncryption: true, keyBytes: key, bytesToProcess: plainBytes);

        return Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(ReadOnlySpan<byte> key, string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        byte[] decryptedBytes = EncryptOrDecrypt(isForEncryption: false, keyBytes: key, bytesToProcess: cipherBytes);

        return System.Text.Encoding.UTF8.GetString(decryptedBytes);
    }

    private static byte[] EncryptOrDecrypt(bool isForEncryption, ReadOnlySpan<byte> keyBytes, byte[] bytesToProcess)
    {
        Org.BouncyCastle.Crypto.IBlockCipher symmetricBlockCipher = new Org.BouncyCastle.Crypto.Engines.AesEngine();
        Org.BouncyCastle.Crypto.Modes.IBlockCipherMode symmetricBlockCipherMode = new Org.BouncyCastle.Crypto.Modes.CbcBlockCipher(symmetricBlockCipher);
        Org.BouncyCastle.Crypto.Paddings.IBlockCipherPadding symmetricBlockCipherPadding = new Org.BouncyCastle.Crypto.Paddings.Pkcs7Padding();
        Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher cbcCipher = new(symmetricBlockCipherMode, symmetricBlockCipherPadding);

        int blockSize = cbcCipher.GetBlockSize();

        Org.BouncyCastle.Crypto.ICipherParameters cipherParameters =
            new Org.BouncyCastle.Crypto.Parameters.ParametersWithIV(
                parameters: new Org.BouncyCastle.Crypto.Parameters.KeyParameter(keyBytes),
                iv: keyBytes.Slice(4, blockSize));

        cbcCipher.Init(forEncryption: isForEncryption, parameters: cipherParameters);

        byte[] outputData = new byte[cbcCipher.GetOutputSize(bytesToProcess.Length)];
        int processLength = cbcCipher.ProcessBytes(bytesToProcess, 0, bytesToProcess.Length, outputData, 0);
        int finalLength = cbcCipher.DoFinal(outputData, processLength);
        byte[] finalTextData = new byte[outputData.Length - (blockSize - finalLength)];
        Array.Copy(outputData, 0, finalTextData, 0, finalTextData.Length);

        return finalTextData;
    }
}
