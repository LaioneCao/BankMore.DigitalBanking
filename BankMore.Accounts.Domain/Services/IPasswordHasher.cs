namespace BankMore.Accounts.Domain.Services
{
    public interface IPasswordHasher
    {
        PasswordHashResult Hash(string password);
        bool Verify(string password, string storedHash, string storedSalt);
    }

    public sealed record PasswordHashResult(string Hash, string Salt);
}
