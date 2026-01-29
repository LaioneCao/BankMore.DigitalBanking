namespace BankMore.Accounts.Application.Login
{
    public sealed class LoginCommand
    {
        public string? CPFTitular { get; init; }
        public int? NumeroConta { get; init; }
        public string Senha { get; init; } = string.Empty;
    }
}
