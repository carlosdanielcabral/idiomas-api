using System.Security.Cryptography;
using System.Text;
using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Infrastructure.Service.Encryption;

public class AesGcmEncryptionService : IEncryptionService
{
    private const int NONCE_SIZE = 12;
    private const int TAG_SIZE = 16;
    private const int KEY_SIZE = 32;

    private readonly byte[] _key;

    public AesGcmEncryptionService(string encryptionKey)
    {
        this._key = this.DeriveKey(encryptionKey);
    }

    public string Encrypt(string plainText)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] nonce = new byte[NONCE_SIZE];
        byte[] tag = new byte[TAG_SIZE];
        byte[] cipherBytes = new byte[plainBytes.Length];

        using (RandomNumberGenerator random = RandomNumberGenerator.Create())
        {
            random.GetBytes(nonce);
        }

        using (AesGcm aesGcm = new(this._key, TAG_SIZE))
        {
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);
        }

        byte[] result = new byte[NONCE_SIZE + TAG_SIZE + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, NONCE_SIZE);
        Buffer.BlockCopy(tag, 0, result, NONCE_SIZE, TAG_SIZE);
        Buffer.BlockCopy(cipherBytes, 0, result, NONCE_SIZE + TAG_SIZE, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        if (cipherBytes.Length < NONCE_SIZE + TAG_SIZE)
        {
            throw new CryptographicException("Invalid cipher text: data too short");
        }

        byte[] nonce = new byte[NONCE_SIZE];
        byte[] tag = new byte[TAG_SIZE];
        int cipherLength = cipherBytes.Length - NONCE_SIZE - TAG_SIZE;
        byte[] cipher = new byte[cipherLength];

        Buffer.BlockCopy(cipherBytes, 0, nonce, 0, NONCE_SIZE);
        Buffer.BlockCopy(cipherBytes, NONCE_SIZE, tag, 0, TAG_SIZE);
        Buffer.BlockCopy(cipherBytes, NONCE_SIZE + TAG_SIZE, cipher, 0, cipherLength);

        byte[] plainBytes = new byte[cipherLength];

        using (AesGcm aesGcm = new(this._key, TAG_SIZE))
        {
            aesGcm.Decrypt(nonce, cipher, tag, plainBytes);
        }

        return Encoding.UTF8.GetString(plainBytes);
    }

    private byte[] DeriveKey(string encryptionKey)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

        if (keyBytes.Length == KEY_SIZE)
        {
            return keyBytes;
        }

        byte[] derivedKey = new byte[KEY_SIZE];

        if (keyBytes.Length > KEY_SIZE)
        {
            Buffer.BlockCopy(keyBytes, 0, derivedKey, 0, KEY_SIZE);
        }
        else
        {
            Buffer.BlockCopy(keyBytes, 0, derivedKey, 0, keyBytes.Length);
        }

        return derivedKey;
    }
}
