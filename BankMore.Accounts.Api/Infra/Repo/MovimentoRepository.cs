using BankMore.Accounts.Api.Application.ListAccounts;
using BankMore.Accounts.Api.Application.ListMovements;
using BankMore.Accounts.Api.Domain.Repo;
using Dapper;

namespace BankMore.Accounts.Api.Infra.Repo
{
    public sealed class MovimentoRepository : IMovimentoRepository
    {
        private readonly IDbConnectionFactory _factory;

        public MovimentoRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task AddAsync(Guid contaId, string tipo, decimal valor, DateTime dataUtc)
        {
            const string sql = @"
                                INSERT INTO movimento
                                (
                                  idmovimento,
                                  idcontacorrente,
                                  datamovimento,
                                  tipomovimento,
                                  valor
                                )
                                VALUES
                                (
                                  @IdMovimento,
                                  @IdConta,
                                  @DataMovimento,
                                  @Tipo,
                                  @Valor
                                );
                                ";

            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdConta = contaId.ToString(),
                DataMovimento = dataUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Tipo = tipo,
                Valor = valor
            });
        }

        public async Task<IEnumerable<MovementListItem>> ListByContaIdAsync(Guid contaId)
        {
            const string sql = @"
                                SELECT
                                  idmovimento   AS IdMovimento,
                                  datamovimento AS DataMovimento,
                                  tipomovimento AS TipoMovimento,
                                  valor         AS Valor
                                FROM movimento
                                WHERE idcontacorrente = @IdConta
                                ORDER BY datamovimento DESC;
                                ";

            using var conn = _factory.CreateConnection();

            // SQLite pode devolver valor como double; Dapper converte pra decimal na maioria dos casos.
            var rows = await conn.QueryAsync<MovementListItem>(sql, new { IdConta = contaId.ToString() });
            return rows;
        }
    }
}
