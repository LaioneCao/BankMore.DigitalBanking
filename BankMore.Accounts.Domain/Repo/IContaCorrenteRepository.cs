using BankMore.Accounts.Domain.Entities;

namespace BankMore.Accounts.Domain.Repo
{
    public interface IContaCorrenteRepository
    {
        Task AddAsync(ContaCorrente conta);
        Task<ContaCorrente?> GetByNumeroAsync(int numeroConta);
        Task<ContaCorrente?> GetByCpfAsync(string cpf);
        Task<ContaCorrente?> GetByIdAsync(Guid id);
        Task InativarAsync(Guid id);
    }
}
