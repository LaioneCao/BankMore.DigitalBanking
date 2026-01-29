using BankMore.Accounts.Domain.Repo;
using BankMore.Accounts.Domain.Services;
using MediatR;

namespace BankMore.Accounts.Application.Commands.CloseAccount
{
    public sealed class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, Unit>
    {
        private readonly IContaCorrenteRepository _repo;
        private readonly IPasswordHasher _hasher;

        public CloseAccountCommandHandler(IContaCorrenteRepository repo, IPasswordHasher hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task<Unit> Handle(CloseAccountCommand command, CancellationToken ct)
        {

            var conta = await _repo.GetByIdAsync(command.ContaId);
            if (conta is null)
                throw new BusinessException("Conta corrente inválida.", "INVALID_ACCOUNT");

            var ok = _hasher.Verify(command.Senha, conta.SenhaHash, conta.Salt);
            if (!ok)
                throw new UnauthorizedAccessException("Usuário não autorizado.");

            await _repo.InativarAsync(conta.Id);

            return Unit.Value;
        }
    }


    public sealed class BusinessException : Exception
    {
        public string Type { get; }

        public BusinessException(string message, string type) : base(message)
            => Type = type;
    }
}
