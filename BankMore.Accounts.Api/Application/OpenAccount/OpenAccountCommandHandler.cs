using BankMore.Accounts.Api.Application.OpenAccount;
using BankMore.Accounts.Api.Domain.Entities;
using BankMore.Accounts.Api.Domain.Repo;
using BankMore.Accounts.Api.Domain.Services;

namespace BankMore.Accounts.Api.Application.OpenAccount;

public class OpenAccountCommandHandler
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public OpenAccountCommandHandler(IContaCorrenteRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<OpenAccountResult> HandleAsync(OpenAccountCommand command)
    {
        var result = _passwordHasher.Hash(command.Senha);

        int numero = GerarNumeroConta();

        var conta = new ContaCorrente(
            numero,
            command.CPFTitular,
            command.NomeTitular,
            result.Hash,
            result.Salt
        );

        await _repository.AddAsync(conta);

        return new OpenAccountResult
        {
            ContaId = conta.Id,
            NumeroConta = conta.Numero,
            NomeTitular = conta.NomeTitular,
            CPFTitular = conta.CPFTitular,
            Ativa = conta.Ativa
        };
    }

    private static int GerarNumeroConta()
    {
        return Random.Shared.Next(10000, 100000);
    }
}
