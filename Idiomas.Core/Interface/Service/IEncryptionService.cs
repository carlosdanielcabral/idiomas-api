namespace Idiomas.Core.Interface.Service;

public interface IEncryptionService
{
    string Encrypt(string plainText);

    string Decrypt(string cipherText);
}
