namespace BankMore.Accounts.Api.Domain.Repo
{
    public interface IBalanceQueryRepository
    {
        Task<decimal> GetSaldoAsync(Guid contaId);
    }
}
