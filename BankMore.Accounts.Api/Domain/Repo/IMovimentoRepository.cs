using BankMore.Accounts.Api.Application.ListAccounts;
using BankMore.Accounts.Api.Application.ListMovements;

namespace BankMore.Accounts.Api.Domain.Repo
{
    public interface IMovimentoRepository
    {
        Task AddAsync(Guid contaId, string tipo, decimal valor, DateTime dataUtc);

        Task<IEnumerable<MovementListItem>> ListByContaIdAsync(Guid contaId);
    }
}
