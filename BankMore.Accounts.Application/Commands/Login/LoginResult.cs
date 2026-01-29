namespace BankMore.Accounts.Application.Commands.Login
{
    public sealed class LoginResult
    {
        public string Token { get; init; } = string.Empty;
        public Guid ContaId { get; init; }
        public int NumeroConta { get; init; }
    }
}
