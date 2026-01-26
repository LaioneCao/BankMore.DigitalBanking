using BankMore.Accounts.Api.Domain.Repo;
using BankMore.Accounts.Api.Domain.Services;

namespace BankMore.Accounts.Api.Application.CloseAccount
{
    public sealed class CloseAccountCommandHandler
    {
        private readonly IContaCorrenteRepository _repo;
        private readonly IPasswordHasher _hasher;

        public CloseAccountCommandHandler(IContaCorrenteRepository repo, IPasswordHasher hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task HandleAsync(Guid contaId, CloseAccountCommand command)
        {

            var conta = await _repo.GetByIdAsync(contaId);
            if (conta is null)
                throw new BusinessException("Conta corrente inválida.", "INVALID_ACCOUNT");

            var ok = _hasher.Verify(command.Senha, conta.SenhaHash, conta.Salt);
            if (!ok)
                throw new UnauthorizedAccessException("Usuário não autorizado.");

            await _repo.InativarAsync(contaId);
        }
    }


    public sealed class BusinessException : Exception
    {
        public string Type { get; }

        public BusinessException(string message, string type) : base(message)
            => Type = type;
    }
}
