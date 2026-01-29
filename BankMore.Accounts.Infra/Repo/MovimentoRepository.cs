using BankMore.Accounts.Domain.Entities;
using BankMore.Accounts.Domain.Repo;
using Dapper;

namespace BankMore.Accounts.Infra.Repo
{
    public sealed class MovimentoRepository : IMovimentoRepository
    {
        private readonly IDbConnectionFactory _factory;

        public MovimentoRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task AddAsync(Movimento movimento)
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
                IdConta = movimento.ContaCorrenteId.ToString(),
                DataMovimento = movimento.DataMovimento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                movimento.Tipo,
                movimento.Valor
            });
        }
    }
}
