namespace BankMore.Transfers.Api.Contracts
{
    public sealed class CreateTransferRequest
    {
        public string RequisitionId { get; init; } = string.Empty;
        public int NumeroContaDestino { get; init; }
        public decimal Valor { get; init; }

    }
}
