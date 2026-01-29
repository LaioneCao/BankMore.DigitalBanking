using BankMore.Transfers.Domain.Repo;
using BankMore.Transfers.Domain.Services;
using System.Text.Json;

namespace BankMore.Transfers.Application.CreateTransfer
{
    public sealed class CreateTransferCommandHandler
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

        public async Task HandleAsync(Guid contaOrigemId, string bearerToken, CreateTransferCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.RequisitionId))
                throw new BusinessException("Identificação da requisição é obrigatória.", "INVALID_VALUE");

            if (await _idem.ExistsAsync(command.RequisitionId))
                return;

            if (command.Valor <= 0)
                throw new BusinessException("Apenas valores positivos podem ser recebidos.", "INVALID_VALUE");

            if (command.NumeroContaDestino <= 0)
                throw new BusinessException("Conta destino inválida.", "INVALID_ACCOUNT");

            // 1) débito
            await _accounts.DebitAsync(bearerToken, command.RequisitionId, command.Valor);

            try
            {
                // 2) crédito destino
                await _accounts.CreditAsync(bearerToken, command.RequisitionId, command.NumeroContaDestino, command.Valor);
            }
            catch
            {
                // 3) estorno (crédito na origem)
                await _accounts.CreditSelfAsync(bearerToken, command.RequisitionId + "-REVERSAL", command.Valor);
                throw;
            }

            var transferId = Guid.NewGuid();
            await _repo.AddAsync(transferId, contaOrigemId, Guid.Empty, command.Valor, DateTime.UtcNow);

            await _idem.SaveAsync(
                command.RequisitionId,
                JsonSerializer.Serialize(command),
                "NO_CONTENT");
        }
    }

    public sealed class BusinessException : Exception
    {
        public string Type { get; }
        public BusinessException(string message, string type) : base(message) => Type = type;
    }
}
