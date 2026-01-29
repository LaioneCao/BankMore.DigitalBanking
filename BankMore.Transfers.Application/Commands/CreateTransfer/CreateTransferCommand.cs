namespace BankMore.Transfers.Application.Commands.CreateTransfer
{
    public sealed class CreateTransferCommand
    {
        public string RequisitionId { get; init; } = string.Empty;
        public int NumeroContaDestino { get; init; }
        public decimal Valor { get; init; }
    }
}
