using BankMore.Accounts.Domain.Repo;

namespace BankMore.Accounts.Application.Balance
{
    public sealed class GetBalanceQueryHandler
    {
        private readonly IContaCorrenteRepository _contaRepo;
        private readonly IBalanceQueryRepository _balanceRepo;

        public GetBalanceQueryHandler(IContaCorrenteRepository contaRepo, IBalanceQueryRepository balanceRepo)
        {
            _contaRepo = contaRepo;
            _balanceRepo = balanceRepo;
        }

        public async Task<GetBalanceResult> HandleAsync(Guid contaId)
        {
            var conta = await _contaRepo.GetByIdAsync(contaId);
            if (conta is null)
                throw new BusinessException("Conta corrente inválida.", "INVALID_ACCOUNT");

            if (!conta.Ativa)
                throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");

            var saldo = await _balanceRepo.GetSaldoAsync(contaId);

            return new GetBalanceResult
            {
                NumeroConta = conta.Numero,
                NomeTitular = conta.NomeTitular,
                DataHoraResposta = DateTime.UtcNow,
                SaldoAtual = saldo
            };
        }
    }
}
