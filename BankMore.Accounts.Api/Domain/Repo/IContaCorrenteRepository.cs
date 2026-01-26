using BankMore.Accounts.Api.Application.ListAccounts;
using BankMore.Accounts.Api.Domain.Entities;

namespace BankMore.Accounts.Api.Domain.Repo
{
    public interface IContaCorrenteRepository
    {
        Task AddAsync(ContaCorrente conta);
        Task<ContaCorrente?> GetByNumeroAsync(int numeroConta);
        Task<ContaCorrente?> GetByCpfAsync(string cpf);
        Task<IEnumerable<AccountListItem>> ListAsync();
        Task<ContaCorrente?> GetByIdAsync(Guid id);
        Task InativarAsync(Guid id);
    }
}
