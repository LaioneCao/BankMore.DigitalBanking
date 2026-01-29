using BankMore.Transfers.Domain.Repo;
using BankMore.Transfers.Domain.Services;
using MediatR;
using System.Text.Json;

namespace BankMore.Transfers.Application.Commands.CreateTransfer
{
    public sealed class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand, Unit>
    {
        private readonly IIdempotenciaRepository _idem;
        private readonly ITransferenciaRepository _repo;
        private readonly IAccountsClient _accounts;

        public CreateTransferCommandHandler(
            IIdempotenciaRepository idem,
            ITransferenciaRepository repo,
            IAccountsClient accounts)
        {
            _idem = idem;
            _repo = repo;
            _accounts = accounts;
        }

        public async Task<Unit> Handle(CreateTransferCommand command, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(command.RequisitionId))
                throw new BusinessException("Identificação da requisição é obrigatória.", "INVALID_VALUE");

            if (await _idem.ExistsAsync(command.RequisitionId))
                return Unit.Value;

            if (command.Valor <= 0)
                throw new BusinessException("Apenas valores positivos podem ser recebidos.", "INVALID_VALUE");

            if (command.NumeroContaDestino <= 0)
                throw new BusinessException("Conta destino inválida.", "INVALID_ACCOUNT");

            // 1) débito
            var debitId = $"{command.RequisitionId}-DEBIT";
            await _accounts.DebitAsync(command.BearerToken, debitId, command.Valor);

            try
            {
                // 2) crédito destino
                var creditId = $"{command.RequisitionId}-CREDIT";
                await _accounts.CreditAsync(command.BearerToken, creditId, command.NumeroContaDestino, command.Valor);
            }
            catch
            {
                // 3) estorno (crédito na origem)
                var creditSelfId = $"{command.RequisitionId}-CREDITSELF";
                await _accounts.CreditSelfAsync(command.BearerToken, creditSelfId, command.Valor);
                throw;
            }

            var transferId = Guid.NewGuid();
            await _repo.AddAsync(transferId, command.ContaOrigemId, Guid.Empty, command.Valor, DateTime.UtcNow);

            await _idem.SaveAsync(
                command.RequisitionId,
                JsonSerializer.Serialize(command),
                "NO_CONTENT");

            return Unit.Value;
        }
    }

    public sealed class BusinessException : Exception
    {
        public string Type { get; }
        public BusinessException(string message, string type) : base(message) => Type = type;
    }
}
