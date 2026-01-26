using BankMore.Accounts.Api.Domain.Repo;
using System.Text.Json;

namespace BankMore.Accounts.Api.Application.Movements
{
    public sealed class CreateMovementCommandHandler
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

        public async Task HandleAsync(Guid contaLogadaId, int contaLogadaNumero, CreateMovementCommand command)
        {
            // Idempotência: se já processou essa requisição, retorna sucesso (204) de novo
            if (string.IsNullOrWhiteSpace(command.RequisitionId))
                throw new BusinessException("Identificação da requisição é obrigatória.", "INVALID_VALUE");

            if (await _idemRepo.ExistsAsync(command.RequisitionId))
                return;

            // Resolve conta alvo
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
                contaAlvoId = contaLogadaId;
                contaAlvoNumero = contaLogadaNumero;
            }

            // Validações
            if (command.Valor <= 0)
                throw new BusinessException("Apenas valores positivos podem ser recebidos.", "INVALID_VALUE");

            var tipo = (command.TipoMovimento ?? "").Trim().ToUpperInvariant();
            if (tipo is not ("C" or "D"))
                throw new BusinessException("Tipo de movimento inválido. Use 'C' ou 'D'.", "INVALID_TYPE");

            // Se conta alvo é diferente da conta logada, só permite CRÉDITO
            if (contaAlvoNumero != contaLogadaNumero && tipo != "C")
                throw new BusinessException("Apenas crédito é permitido para movimentação em conta diferente do usuário logado.", "INVALID_TYPE");

            // Conta alvo precisa estar ativa
            var contaParaStatus = contaAlvoNumero == contaLogadaNumero
                ? await _contaRepo.GetByIdAsync(contaLogadaId)
                : await _contaRepo.GetByNumeroAsync(contaAlvoNumero);

            if (contaParaStatus is null)
                throw new BusinessException("Conta corrente inválida.", "INVALID_ACCOUNT");

            if (!contaParaStatus.Ativa)
                throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");

            // Persistir movimento
            await _movRepo.AddAsync(contaAlvoId, tipo, command.Valor, DateTime.UtcNow);

            // Registrar idempotência (resultado pode ser vazio, pois 204)
            var reqJson = JsonSerializer.Serialize(command);
            await _idemRepo.SaveAsync(command.RequisitionId, reqJson, "NO_CONTENT");
        }
    }
}
