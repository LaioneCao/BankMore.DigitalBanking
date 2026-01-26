namespace BankMore.Accounts.Api.Domain.Repo
{
    public interface IIdempotenciaRepository
    {
        Task<bool> ExistsAsync(string key);
        Task SaveAsync(string key, string requisicao, string resultado);
    }
}
