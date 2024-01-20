using System.Security.Cryptography;

namespace Seedysoft.CoreLib.Crypto;

public static class CryptoHelper
{
    // TODO                  Encrypt aes.Key
    public static string Encrypt(string plainText)
    {
        // Create instance of Aes for symmetric encryption of the data.
        using var aes = Aes.Create();

        // Use RSACryptoServiceProvider to encrypt the AES key.
        // rsa is previously instantiated:
        //   _rsa = new RSACryptoServiceProvider(cspp);
        //byte[] keyEncrypted = _rsa.Encrypt(aes.Key, false);
        byte[] keyEncrypted = [.. aes.Key];

        // Create byte arrays to contain the length values of the key and IV.
        byte[] LenK = BitConverter.GetBytes(keyEncrypted.Length);
        byte[] LenIV = BitConverter.GetBytes(aes.IV.Length);

        // Write the following to the out Stream:
        // - length of the key
        // - length of the IV
        // - encrypted key
        // - the IV
        // - the encrypted cipher content

        using MemoryStream outMs = new();
        outMs.Write(LenK.AsSpan());
        outMs.Write(LenIV.AsSpan());
        outMs.Write(keyEncrypted.AsSpan());
        outMs.Write(aes.IV.AsSpan());

        // Now write the cipher text using a CryptoStream for encrypting.
        using ICryptoTransform transform = aes.CreateEncryptor();
        using var outStreamEncrypted = new CryptoStream(outMs, transform, CryptoStreamMode.Write);
        // By encrypting a chunk at a time, you can save memory and accommodate large files.
        int count = 0;

        // blockSizeBytes can be any arbitrary size.
        int blockSizeBytes = aes.BlockSize / 8;
        byte[] data = new byte[blockSizeBytes];
        int bytesRead = 0;

        using (MemoryStream inMs = new(System.Text.Encoding.UTF8.GetBytes(plainText)))
        {
            do
            {
                count = inMs.Read(data, 0, blockSizeBytes);
                outStreamEncrypted.Write(data, 0, count);
                bytesRead += blockSizeBytes;
            } while (count > 0);
        }

        outStreamEncrypted.FlushFinalBlock();

        return Convert.ToBase64String(outMs.ToArray());
    }

    public static string Decrypt(string encryptedText)
    {
        // Create byte arrays to get the length of the encrypted key and IV.
        // These values were stored as 4 bytes each at the beginning of the encrypted package.
        byte[] LenK = new byte[4];
        byte[] LenIV = new byte[4];

        using MemoryStream inMs = new(Convert.FromBase64String(encryptedText));
        _ = inMs.Seek(0, SeekOrigin.Begin);
        _ = inMs.Read(LenK, 0, 3);
        _ = inMs.Seek(LenK.Length, SeekOrigin.Begin);
        _ = inMs.Read(LenIV, 0, 3);

        // Convert the lengths to integer values.
        int lenK = BitConverter.ToInt32(LenK, 0);
        int lenIV = BitConverter.ToInt32(LenIV, 0);

        // Determine the start position of the cipher text (startC) and its length(lenC).
        int startC = LenK.Length + LenIV.Length + lenK + lenIV;

        // Create the byte arrays for the encrypted Aes key, the IV, and the cipher text.
        byte[] KeyEncrypted = new byte[lenK];
        byte[] IV = new byte[lenIV];

        // Extract the key and IV starting from index 8 after the length values.
        _ = inMs.Seek(LenK.Length + LenIV.Length, SeekOrigin.Begin);
        _ = inMs.Read(KeyEncrypted, 0, lenK);
        _ = inMs.Seek(LenK.Length + LenIV.Length + lenK, SeekOrigin.Begin);
        _ = inMs.Read(IV, 0, lenIV);

        // Use RSACryptoServiceProvider to decrypt the AES key.
        //byte[] KeyDecrypted = _rsa.Decrypt(KeyEncrypted, false);
        byte[] KeyDecrypted = [.. KeyEncrypted];

        // Create instance of Aes for symmetric decryption of the data.
        using var aes = Aes.Create();

        // Decrypt the key.
        using ICryptoTransform transform = aes.CreateDecryptor([.. KeyDecrypted], IV);

        // Decrypt the cipher text 
        using MemoryStream outMs = new();
        int count = 0;

        // blockSizeBytes can be any arbitrary size.
        int blockSizeBytes = aes.BlockSize / 8;
        byte[] data = new byte[blockSizeBytes];

        // By decrypting a chunk a time, you can save memory and accommodate large files.

        // Start at the beginning of the cipher text.
        _ = inMs.Seek(startC, SeekOrigin.Begin);
        using var outStreamDecrypted = new CryptoStream(outMs, transform, CryptoStreamMode.Write);
        do
        {
            count = inMs.Read(data, 0, blockSizeBytes);
            outStreamDecrypted.Write(data, 0, count);
        } while (count > 0);

        outStreamDecrypted.FlushFinalBlock();

        return System.Text.Encoding.UTF8.GetString(outMs.ToArray());
    }
}
