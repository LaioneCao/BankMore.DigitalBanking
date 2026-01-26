using BankMore.Accounts.Api.Domain.Repo;
using Dapper;

namespace BankMore.Accounts.Api.Infra.Repo
{
    public sealed class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly IDbConnectionFactory _factory;

        public IdempotenciaRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            const string sql = @"SELECT 1 FROM idempotencia WHERE chave_idempotencia = @Key LIMIT 1;";
            using var conn = _factory.CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<int?>(sql, new { Key = key });
            return result.HasValue;
        }

        public async Task SaveAsync(string key, string requisicao, string resultado)
        {
            const string sql = @"
                                INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                                VALUES (@Key, @Req, @Res);
                                ";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { Key = key, Req = requisicao, Res = resultado });
        }
    }
}
