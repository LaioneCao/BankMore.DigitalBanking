using BankMore.Accounts.Domain.Entities;
using BankMore.Accounts.Domain.Repo;
using MediatR;
using System.Text.Json;

namespace BankMore.Accounts.Application.Commands.Movements
{
    public sealed class CreateMovementCommandHandler : IRequestHandler<CreateMovementCommand, Unit>
    {
        private readonly IContaCorrenteRepository _contaRepo;
        private readonly IMovimentoRepository _movRepo;
        private readonly IIdempotenciaRepository _idemRepo;

        public CreateMovementCommandHandler(
            IContaCorrenteRepository contaRepo,
            IMovimentoRepository movRepo,
            IIdempotenciaRepository idemRepo)
        {
            _contaRepo = contaRepo;
            _movRepo = movRepo;
            _idemRepo = idemRepo;
        }

        public async Task<Unit> Handle(CreateMovementCommand command, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(command.RequisitionId))
                throw new BusinessException("Identificação da requisição é obrigatória.", "INVALID_VALUE");

            if (await _idemRepo.ExistsAsync(command.RequisitionId))
                return Unit.Value;

            Guid contaAlvoId;
            int contaAlvoNumero;

            if (command.NumeroConta.HasValue)
            {
                contaAlvoNumero = command.NumeroConta.Value;
                var contaAlvo = await _contaRepo.GetByNumeroAsync(contaAlvoNumero);
                if (contaAlvo is null)
                    throw new BusinessException("Conta corrente inválida.", "INVALID_ACCOUNT");

                contaAlvoId = contaAlvo.Id;
            }
            else
            {
                contaAlvoId = command.ContaIdLogada;
                contaAlvoNumero = command.NumeroContaLogada;
            }

            // Validações
            if (command.Valor <= 0)
                throw new BusinessException("Apenas valores positivos podem ser recebidos.", "INVALID_VALUE");

            var tipo = (command.TipoMovimento ?? "").Trim().ToUpperInvariant();
            if (tipo is not ("C" or "D"))
                throw new BusinessException("Tipo de movimento inválido. Use 'C' ou 'D'.", "INVALID_TYPE");

            if (contaAlvoNumero != command.NumeroContaLogada && tipo != "C")
                throw new BusinessException("Apenas crédito é permitido para movimentação em conta diferente do usuário logado.", "INVALID_TYPE");

            var contaParaStatus = contaAlvoNumero == command.NumeroContaLogada
                ? await _contaRepo.GetByIdAsync(command.ContaIdLogada)
                : await _contaRepo.GetByNumeroAsync(contaAlvoNumero);

            if (contaParaStatus is null)
                throw new BusinessException("Conta corrente inválida.", "INVALID_ACCOUNT");

            if (!contaParaStatus.Ativa)
                throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");

            var movimento = new Movimento(contaAlvoId, tipo, command.Valor);

            await _movRepo.AddAsync(movimento);

            var reqJson = JsonSerializer.Serialize(command);
            await _idemRepo.SaveAsync(command.RequisitionId, reqJson, "NO_CONTENT");

            return Unit.Value;
        }
    }
}
