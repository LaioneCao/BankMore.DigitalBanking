namespace BankMore.Accounts.Api.Application.OpenAccount
{

    public class OpenAccountResult
    {
        public Guid ContaId { get; init; }
        public int NumeroConta { get; init; }
        public string NomeTitular { get; init; } = string.Empty;
        public string CPFTitular { get; init; } = string.Empty;
        public bool Ativa { get; init; }
    }
}
