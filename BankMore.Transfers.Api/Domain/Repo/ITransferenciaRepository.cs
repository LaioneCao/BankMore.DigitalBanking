namespace BankMore.Transfers.Api.Domain.Repo
{
    public interface ITransferenciaRepository
    {
        Task AddAsync(Guid idTransferencia, Guid origemId, Guid destinoId, decimal valor, DateTime dataUtc);
    }
}
