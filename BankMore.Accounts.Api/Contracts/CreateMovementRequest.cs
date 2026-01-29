namespace BankMore.Accounts.Api.Contracts
{
    public sealed class CreateMovementRequest
    {
        public string RequisitionId { get; init; } = string.Empty;
        public int? NumeroConta { get; init; }
        public decimal Valor { get; init; }
        public string TipoMovimento { get; init; } = string.Empty;
    }
}
