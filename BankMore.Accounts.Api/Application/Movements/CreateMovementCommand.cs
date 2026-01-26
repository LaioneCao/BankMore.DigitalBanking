namespace BankMore.Accounts.Api.Application.Movements
{
    public sealed class CreateMovementCommand
    {
        public string RequisitionId { get; init; } = string.Empty;
        public int? NumeroConta { get; init; }
        public decimal Valor { get; init; }
        public string TipoMovimento { get; init; } = string.Empty; // "C" ou "D"
    }
}
