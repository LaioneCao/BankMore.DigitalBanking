namespace BankMore.Accounts.Api.Application.Movements
{
    public sealed class CreateMovementCommand
    {
        public string RequisitionId { get; init; } = string.Empty; // chave de idempotência
        public int? NumeroConta { get; init; }
        public decimal Valor { get; init; }
        public string TipoMovimento { get; init; } = string.Empty; // "C" ou "D"
    }
}
