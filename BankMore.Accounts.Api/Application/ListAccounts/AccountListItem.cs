namespace BankMore.Accounts.Api.Application.ListAccounts
{
    public sealed class AccountListItem
    {
        public Guid ContaId { get; init; }
        public int NumeroConta { get; init; }
        public string CPFTitular { get; init; } = string.Empty;
        public string NomeTitular { get; init; } = string.Empty;
        public bool Ativa { get; init; }
    }
}
