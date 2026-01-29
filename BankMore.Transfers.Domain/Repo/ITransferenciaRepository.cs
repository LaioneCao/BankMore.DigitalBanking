namespace BankMore.Transfers.Domain.Repo
{
    public interface ITransferenciaRepository
    {
        Task AddAsync(Guid idTransferencia, Guid origemId, Guid destinoId, decimal valor, DateTime dataUtc);
    }
}
