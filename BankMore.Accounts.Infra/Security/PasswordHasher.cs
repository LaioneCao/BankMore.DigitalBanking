using BankMore.Accounts.Domain.Services;
using System.Security.Cryptography;
using System.Text;

namespace BankMore.Accounts.Infra.Security
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        public PasswordHashResult Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Senha inválida.");

            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var salt = Convert.ToBase64String(saltBytes);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                10000,
                HashAlgorithmName.SHA256
            );

            var hashBytes = pbkdf2.GetBytes(32);
            var hash = Convert.ToBase64String(hashBytes);

            return new PasswordHashResult(hash, salt);
        }

        public bool Verify(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(storedHash) ||
                string.IsNullOrWhiteSpace(storedSalt))
                return false;

            byte[] saltBytes;
            try
            {
                saltBytes = Convert.FromBase64String(storedSalt);
            }
            catch
            {
                return false;
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
            var computedHash = Convert.ToBase64String(pbkdf2.GetBytes(32));

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(storedHash),
                Convert.FromBase64String(computedHash)
            );
        }
    }
}
