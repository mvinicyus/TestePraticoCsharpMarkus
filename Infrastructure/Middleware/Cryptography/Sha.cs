using Domain.Interface.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Middleware.Cryptography
{
    public class Sha : ISha
    {
        private readonly string _preSalt;
        private readonly string _posSalt;

        public Sha()
        {
            _preSalt = Environment.GetEnvironmentVariable("PreSalt");
            _posSalt = Environment.GetEnvironmentVariable("PosSalt");
        }

        public string Encrypt(string dataToEncrypt)
        {
            string encryptedData;

            using (var sha512 = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes($"{_preSalt}{dataToEncrypt}{_posSalt}");
                var hash = sha512.ComputeHash(bytes);
                encryptedData = GetStringFromHash(hash);
            }

            return encryptedData;
        }

        private static string GetStringFromHash(byte[] hash)
        {
            var result = new StringBuilder();

            for (var i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString();
        }
    }
}
