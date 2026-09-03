namespace Seedysoft.Libs.Cryptography;

public static class Crypto
{
    private static readonly System.Text.Encoding Encoding = System.Text.Encoding.Latin1;

    //private static bool CanEncryptText(string textToEncrypt, string key, CipherMode cipherMode = CipherMode.CBC)
    //{
    //    try
    //    {
    //        byte[] textBytes = Encoding.GetBytes(textToEncrypt);
    //        byte[] keyBytes = Convert.FromBase64String(key);
    //        byte[] encryptedBytes = EncryptBytes(textBytes, keyBytes, cipherMode);
    //        string encryptedText = Convert.ToBase64String(encryptedBytes);

    //        return true;
    //    }
    //    catch { }

    //    return false;
    //}
    //internal static string EncryptText(string textToEncrypt, string key, CipherMode cipherMode = CipherMode.CBC)
    //{
    //    return CanEncryptText(textToEncrypt, key, cipherMode)
    //        ? Convert.ToBase64String(EncryptBytes(Encoding.GetBytes(textToEncrypt), Convert.FromBase64String(key), cipherMode))
    //        : throw new InvalidDataException($"Cannot Encrypt {textToEncrypt} with {key} key and mode {cipherMode}");
    //}

    //internal static bool CanDecryptText(string encryptedText, string key, CipherMode cipherMode = CipherMode.CBC)
    //{
    //    if (!string.IsNullOrWhiteSpace(encryptedText) && !string.IsNullOrWhiteSpace(key))
    //    {
    //        try
    //        {
    //            byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);
    //            byte[] keyBytes = Convert.FromBase64String(key);
    //            byte[] decryptedBytes = DecryptBytes(Convert.FromBase64String(encryptedText), Convert.FromBase64String(key), cipherMode);
    //            string decryptedText = Encoding.GetString(decryptedBytes);

    //            return true;
    //        }
    //        catch { }
    //    }

    //    return false;
    //}
    //public static string DecryptText(string encryptedText, string key, CipherMode cipherMode = CipherMode.CBC)
    //{
    //    return CanDecryptText(encryptedText, key, cipherMode)
    //        ? Encoding.GetString(DecryptBytes(Convert.FromBase64String(encryptedText), Convert.FromBase64String(key), cipherMode))
    //        : throw new InvalidDataException($"Cannot Decrypt {encryptedText} with {key} key and mode {cipherMode}");
    //}

    //private static byte[] EncryptBytes(byte[] inputBuffer, byte[] key, CipherMode cipherMode)
    //{
    //    ArgumentNullException.ThrowIfNull(inputBuffer);

    //    byte[] iv;
    //    byte[] cipherText;

    //    using (Aes cipher = BuildCryptographicObject(key, cipherMode))
    //    {
    //        using ICryptoTransform symmetricEncryptor = cipher.CreateEncryptor();
    //        iv = cipher.IV;

    //        cipherText = Transform(symmetricEncryptor, inputBuffer, 0, inputBuffer.Length);
    //    }

    //    int totalLength = iv.Length + cipherText.Length;

    //    byte[] combinedData = new byte[totalLength];
    //    int outputOffset = 0;

    //    AppendBytes(iv, combinedData, ref outputOffset);
    //    AppendBytes(cipherText, combinedData, ref outputOffset);

    //    System.Diagnostics.Debug.Assert(outputOffset == combinedData.Length);

    //    return combinedData;
    //}

    //private static byte[] DecryptBytes(byte[] encryptedBytes, byte[] key, CipherMode cipherMode)
    //{
    //    ArgumentNullException.ThrowIfNull(encryptedBytes);

    //    using Aes cipher = BuildCryptographicObject(key, cipherMode);
    //    int cipherTextOffset = cipher.IV.Length;

    //    byte[] iv = new byte[cipherTextOffset];
    //    Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
    //    cipher.IV = iv;

    //    using ICryptoTransform decryptor = cipher.CreateDecryptor();

    //    return Transform(decryptor, encryptedBytes, cipherTextOffset, encryptedBytes.Length - cipherTextOffset);
    //}

    //private static byte[] Transform(ICryptoTransform cryptoTransform, byte[] inputBuffer, int inputOffset, int inputCount)
    //{
    //    ArgumentNullException.ThrowIfNull(cryptoTransform);
    //    ArgumentNullException.ThrowIfNull(inputBuffer);
    //    ArgumentOutOfRangeException.ThrowIfLessThan(inputOffset, 0);
    //    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(inputCount, 0);

    //    if (cryptoTransform.CanTransformMultipleBlocks)
    //        return cryptoTransform.TransformFinalBlock(inputBuffer, inputOffset, inputCount);

    //    using MemoryStream memoryStream = new();
    //    using CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
    //    cryptoStream.Write(inputBuffer, inputOffset, inputCount);
    //    cryptoStream.FlushFinalBlock();

    //    return memoryStream.ToArray();
    //}

    //private static Aes BuildCryptographicObject(byte[] key, CipherMode cipherMode)
    //{
    //    ArgumentNullException.ThrowIfNull(key);

    //    var aes = Aes.Create();
    //    aes.Key = key; //aes.KeySize = masterKey.Length * 8L;
    //    aes.Mode = cipherMode;
    //    aes.Padding = PaddingMode.ISO10126;

    //    return aes;
    //}

    //private static void AppendBytes(byte[] newData, byte[] combinedData, ref int writeOffset)
    //{
    //    Buffer.BlockCopy(newData, 0, combinedData, writeOffset, newData.Length);
    //    writeOffset += newData.Length;
    //}

    public static string Encrypt(
        string key,
        string plainText,
        System.Security.Cryptography.CipherMode cipherMode = System.Security.Cryptography.CipherMode.CBC)
    {
        Org.BouncyCastle.Crypto.IBlockCipher symmetricBlockCipher = new Org.BouncyCastle.Crypto.Engines.AesEngine();
        Org.BouncyCastle.Crypto.Modes.IBlockCipherMode symmetricBlockMode = GetBlockCipherMode(cipherMode, symmetricBlockCipher);

        Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher cipher = new(cipherMode: symmetricBlockMode/*, padding: new Org.BouncyCastle.Crypto.Paddings.Pkcs7Padding()*/);

        cipher.Init(forEncryption: true, parameters: GetCipherParameters(Encoding.GetBytes(key)));
        int blockSize = cipher.GetBlockSize();
        byte[] plainTextData = Encoding.GetBytes(plainText);
        byte[] cipherTextData = new byte[cipher.GetOutputSize(plainTextData.Length)];
        int processLength = cipher.ProcessBytes(plainTextData, 0, plainTextData.Length, cipherTextData, 0);
        int finalLength = cipher.DoFinal(cipherTextData, processLength);
        byte[] finalCipherTextData = new byte[cipherTextData.Length - (blockSize - finalLength)];
        Array.Copy(cipherTextData, 0, finalCipherTextData, 0, finalCipherTextData.Length);

        return Encoding.GetString(finalCipherTextData);
    }

    public static string Decrypt(
        string key,
        string cipherText,
        System.Security.Cryptography.CipherMode cipherMode = System.Security.Cryptography.CipherMode.CBC)
    {
        Org.BouncyCastle.Crypto.IBlockCipher symmetricBlockCipher = new Org.BouncyCastle.Crypto.Engines.AesEngine();
        Org.BouncyCastle.Crypto.Modes.IBlockCipherMode symmetricBlockMode = GetBlockCipherMode(cipherMode, symmetricBlockCipher);

        Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher cipher = new(cipherMode: symmetricBlockMode/*, padding: new Org.BouncyCastle.Crypto.Paddings.Pkcs7Padding()*/);

        cipher.Init(forEncryption: false, parameters: GetCipherParameters(Encoding.GetBytes(key)));
        int blockSize = cipher.GetBlockSize();
        byte[] cipherTextData = Encoding.GetBytes(cipherText);
        byte[] plainTextData = new byte[cipher.GetOutputSize(cipherTextData.Length)];
        int processLength = cipher.ProcessBytes(cipherTextData, 0, cipherTextData.Length, plainTextData, 0);
        int finalLength = cipher.DoFinal(plainTextData, processLength);
        byte[] finalPlainTextData = new byte[plainTextData.Length - (blockSize - finalLength)];
        Array.Copy(plainTextData, 0, finalPlainTextData, 0, finalPlainTextData.Length);

        return Encoding.GetString(finalPlainTextData);
    }

    private static Org.BouncyCastle.Crypto.Modes.IBlockCipherMode GetBlockCipherMode(
        System.Security.Cryptography.CipherMode cipherMode,
        Org.BouncyCastle.Crypto.IBlockCipher symmetricBlockCipher)
    {
        return cipherMode switch
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

    private static Org.BouncyCastle.Crypto.ICipherParameters GetCipherParameters(byte[] myKey)
    {
        Org.BouncyCastle.Crypto.ICipherParameters keyParam = new Org.BouncyCastle.Crypto.Parameters.KeyParameter(myKey);

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
