using MediatR;
using BankMore.Accounts.Domain.Entities;
using BankMore.Accounts.Domain.Repo;
using BankMore.Accounts.Domain.Services;

namespace BankMore.Accounts.Application.Commands.OpenAccount;

public class OpenAccountCommandHandler : IRequestHandler<OpenAccountCommand, OpenAccountResult>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public OpenAccountCommandHandler(IContaCorrenteRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }



    public async Task<OpenAccountResult> Handle(OpenAccountCommand command, CancellationToken ct)
    {
        var result = _passwordHasher.Hash(command.Senha);

        int numero = Random.Shared.Next(10000, 100000);

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

}
