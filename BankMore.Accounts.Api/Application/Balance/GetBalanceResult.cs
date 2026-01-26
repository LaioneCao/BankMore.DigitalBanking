namespace BankMore.Accounts.Api.Application.Balance
{
    public sealed class GetBalanceResult
    {
        public int NumeroConta { get; init; }
        public string NomeTitular { get; init; } = string.Empty;
        public DateTime DataHoraResposta { get; init; }
        public decimal SaldoAtual { get; init; } // no swagger vai aparecer como número (ex: 0.00)
    }
}
