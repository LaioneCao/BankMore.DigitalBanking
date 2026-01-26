using BankMore.Accounts.Api.Domain.Repo;
using Dapper;

namespace BankMore.Accounts.Api.Infra.Repo
{
    public sealed class BalanceQueryRepository : IBalanceQueryRepository
    {
        private readonly IDbConnectionFactory _factory;

        public BalanceQueryRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<decimal> GetSaldoAsync(Guid contaId)
        {
            const string sql = @"
                                SELECT
                                    COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) -
                                    COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) AS Saldo
                                FROM movimento
                                WHERE idcontacorrente = @IdConta;
                                ";

            using var conn = _factory.CreateConnection();

            // SQLite pode retornar double; convertemos com segurança para decimal
            var saldo = await conn.QueryFirstOrDefaultAsync<double>(sql, new { IdConta = contaId.ToString() });
            return Convert.ToDecimal(saldo);
        }
    }
}
