using MediatR;

namespace BankMore.Transfers.Application.Commands.CreateTransfer
{
    public sealed class CreateTransferCommand : IRequest<Unit>
    {
        public string RequisitionId { get; init; } = string.Empty;
        public int NumeroContaDestino { get; init; }
        public decimal Valor { get; init; }


        public Guid ContaOrigemId { get; init; }
        public string BearerToken { get; init; } = string.Empty;


    }
}
