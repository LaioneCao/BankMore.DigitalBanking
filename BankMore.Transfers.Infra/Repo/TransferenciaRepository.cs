using BankMore.Transfers.Domain.Repo;
using Dapper;

namespace BankMore.Transfers.Infra.Repo
{
    public sealed class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly IDbConnectionFactory _factory;

        public TransferenciaRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task AddAsync(Guid idTransferencia, Guid origemId, Guid destinoId, decimal valor, DateTime dataUtc)
        {
            const string sql = @"
                                INSERT INTO transferencia
                                (idtransferencia, idcontacorrente_origem, idcontacorrente_destino, datamovimento, valor)
                                VALUES
                                (@Id, @Origem, @Destino, @Data, @Valor);";

            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                Id = idTransferencia.ToString(),
                Origem = origemId.ToString(),
                Destino = destinoId.ToString(),
                Data = dataUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Valor = valor
            });
        }
    }
}
