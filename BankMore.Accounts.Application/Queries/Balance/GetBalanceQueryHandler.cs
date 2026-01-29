using BankMore.Accounts.Domain.Repo;
using MediatR;

namespace BankMore.Accounts.Application.Queries.Balance
{
    public sealed class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, GetBalanceResult>
    {
        private readonly IContaCorrenteRepository _contaRepo;
        private readonly IBalanceQueryRepository _balanceRepo;

        public GetBalanceQueryHandler(IContaCorrenteRepository contaRepo, IBalanceQueryRepository balanceRepo)
        {
            _contaRepo = contaRepo;
            _balanceRepo = balanceRepo;
        }

       
        public async Task<GetBalanceResult> Handle(GetBalanceQuery query, CancellationToken ct)
        {
            var conta = await _contaRepo.GetByIdAsync(query.ContaId);
            if (conta is null)
                throw new BusinessException("Conta corrente inválida.", "INVALID_ACCOUNT");

            if (!conta.Ativa)
                throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");

            var saldo = await _balanceRepo.GetSaldoAsync(query.ContaId);

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
