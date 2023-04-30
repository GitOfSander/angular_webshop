using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Site.Models
{
    public class Encryptor
    {
        private readonly IDataProtector _protector;

        public Encryptor(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
        }

        public string Encrypt<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            return Encrypt(json);
        }

        public string Encrypt(string plaintext)
        {
            return _protector.Protect(plaintext);
        }

        public bool TryDecrypt<T>(string encryptedText, out T obj)
        {
            if (TryDecrypt(encryptedText, out var json))
            {
                obj = JsonConvert.DeserializeObject<T>(json);

                return true;
            }

            obj = default(T);

            return false;
        }

        public bool TryDecrypt(string encryptedText, out string decryptedText)
        {
            try
            {
                decryptedText = _protector.Unprotect(encryptedText);

                return true;
            }
            catch (CryptographicException)
            {
                decryptedText = null;

                return false;
            }
        }

        public string Decrypt(string encryptedText)
        {
            return _protector.Unprotect(encryptedText);
        }
    }
}
