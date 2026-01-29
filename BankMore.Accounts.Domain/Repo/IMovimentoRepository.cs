using BankMore.Accounts.Domain.Entities;

namespace BankMore.Accounts.Domain.Repo
{
    public interface IMovimentoRepository
    {
        Task AddAsync(Movimento movimento);

    }
}
