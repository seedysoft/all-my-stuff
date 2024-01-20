using Seedysoft.TuyaDeviceControlLib.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace Seedysoft.TuyaDeviceControlLib;

internal class AESCipher(byte[] key)
{
    public byte[] Key { get; } = key;

    public byte[] Encrypt(
        byte[] raw,
        bool useBase64 = true,
        bool pad = true,
        byte[]? initialVector = null,
        byte[]? header = null)
    {
        byte[] cryptedText = [];
        if (initialVector == null)
        {
            if (pad)
                raw = AESCipherExtensions.Pad(raw, 16);

            using var cipher = Aes.Create();
            cipher.Key = Key;
            cipher.Mode = CipherMode.ECB;
            cryptedText = cipher.EncryptEcb(raw, PaddingMode.None);
        }
        else
        {
            initialVector = AESCipherExtensions.GetEncryptionInitVector(initialVector);
            using AesGcm cipher = new(Key, AesGcm.NonceByteSizes.MaxSize);

            byte[] tag = [];
            cipher.Encrypt(initialVector ?? [], raw, cryptedText, tag, header);

            cryptedText = new byte[(initialVector ?? []).Length + cryptedText.Length + tag.Length];
            Buffer.BlockCopy(initialVector ?? [], 0, cryptedText, 0, (initialVector ?? []).Length);
            Buffer.BlockCopy(cryptedText, 0, cryptedText, (initialVector ?? []).Length, cryptedText.Length);
            Buffer.BlockCopy(tag, 0, cryptedText, (initialVector ?? []).Length + cryptedText.Length, tag.Length);
        }

        return useBase64 ? Encoding.ASCII.GetBytes(Convert.ToBase64String(cryptedText)) : cryptedText;
    }

    public byte[] Decrypt(
        byte[] enc,
        bool useBase64 = true,
        bool decodeText = true,
        bool verifyPadding = false,
        byte[]? initialVector = null,
        byte[]? header = null,
        byte[]? tag = null)
    {
        if (initialVector == null)
        {
            if (useBase64)
                enc = Convert.FromBase64String(Encoding.ASCII.GetString(enc));

            if (enc.Length % 16 != 0)
                throw new ArgumentException("invalid length");
        }

        byte[] raw = [];
        if (initialVector != null)
        {
            byte[]? initialVectorDecrypted;
            byte[] encDecrypted;
            (initialVectorDecrypted, encDecrypted) = AESCipherExtensions.GetDecryptionInitVector(initialVector, enc);
            using AesGcm cipher = new(Key, AesGcm.NonceByteSizes.MaxSize);
            cipher.Decrypt(initialVectorDecrypted ?? [], enc, tag ?? [], raw, header);
        }
        else
        {
            using var cipher = Aes.Create();
            cipher.Key = Key;
            cipher.Mode = CipherMode.ECB;

            raw = cipher.DecryptEcb(enc, PaddingMode.None);
            raw = AESCipherExtensions.Unpad(raw, verifyPadding);
        }

        return decodeText ? Encoding.ASCII.GetBytes(Encoding.UTF8.GetString(raw)) : raw;
    }
}
