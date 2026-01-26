namespace BankMore.Accounts.Api.Application.CloseAccount
{

    public sealed class CloseAccountCommand
    {
        public string Senha { get; init; } = string.Empty;
    }
}
