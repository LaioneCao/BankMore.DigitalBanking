using BankMore.Accounts.Domain.Repo;
using BankMore.Accounts.Domain.Services;

namespace BankMore.Accounts.Application.Commands.Login
{
    public sealed class LoginCommandHandler
    {
        private readonly IContaCorrenteRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenService _jwt;

        public LoginCommandHandler(IContaCorrenteRepository repo, IPasswordHasher hasher, IJwtTokenService jwt)
        {
            _repo = repo;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<LoginResult> HandleAsync(LoginCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Senha))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            var hasCpf = !string.IsNullOrWhiteSpace(command.CPFTitular);
            var hasNumero = command.NumeroConta.HasValue;

            if (!hasCpf && !hasNumero)
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            var conta = hasNumero
                ? await _repo.GetByNumeroAsync(command.NumeroConta!.Value)
                : await _repo.GetByCpfAsync(command.CPFTitular!);

            if (conta is null)
                throw new UnauthorizedAccessException("Usuário não autorizado.");

            var ok = _hasher.Verify(command.Senha, conta.SenhaHash, conta.Salt);
            if (!ok)
                throw new UnauthorizedAccessException("Usuário não autorizado.");

            var token = _jwt.GenerateToken(conta.Id, conta.Numero);

            return new LoginResult
            {
                Token = token,
                ContaId = conta.Id,
                NumeroConta = conta.Numero
            };
        }
    }
}
