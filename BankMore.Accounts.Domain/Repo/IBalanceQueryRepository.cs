namespace BankMore.Accounts.Domain.Repo
{
    public interface IBalanceQueryRepository
    {
        Task<decimal> GetSaldoAsync(Guid contaId);
    }
}
