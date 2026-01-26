using BankMore.Accounts.Api.Application.ListAccounts;
using BankMore.Accounts.Api.Domain.Entities;
using BankMore.Accounts.Api.Domain.Repo;
using Dapper;

namespace BankMore.Accounts.Api.Infra.Repo
{
    public sealed class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly IDbConnectionFactory _factory;

        public ContaCorrenteRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task AddAsync(ContaCorrente conta)
        {
            const string sql = @"
                                INSERT INTO contacorrente
                                (
                                  idcontacorrente,
                                  numero,
                                  cpf,
                                  nome,
                                  ativo,
                                  senha,
                                  salt
                                )
                                VALUES
                                (
                                  @Id,
                                  @Numero,
                                  @Cpf,
                                  @Nome,
                                  @Ativo,
                                  @SenhaHash,
                                  @Salt
                                );
                                ";

            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                Id = conta.Id.ToString(),
                Numero = conta.Numero,
                Cpf = conta.CPFTitular,
                Nome = conta.NomeTitular,
                Ativo = conta.Ativa ? 1 : 0,
                SenhaHash = conta.SenhaHash,
                Salt = conta.Salt
            });
        }

        public async Task<ContaCorrente?> GetByCpfAsync(string cpf)
        {
            cpf = new string((cpf ?? "").Where(char.IsDigit).ToArray());

            const string sql = @"
                                SELECT
                                  idcontacorrente AS Id,
                                  numero          AS Numero,
                                  cpf             AS CPFTitular,
                                  nome            AS NomeTitular,
                                  ativo           AS Ativa,
                                  senha           AS SenhaHash,
                                  salt            AS Salt
                                FROM contacorrente
                                WHERE cpf = @Cpf
                                LIMIT 1;
                                ";

            using var conn = _factory.CreateConnection();
            var row = await conn.QueryFirstOrDefaultAsync<dynamic>(sql, new { Cpf = cpf });
            if (row is null) return null;

            return ContaCorrente.Rehidratar(
                id: Guid.Parse((string)row.Id),
                numero: (int)row.Numero,
                cpfTitular: (string)row.CPFTitular,
                nomeTitular: (string)row.NomeTitular,
                ativa: ((long)row.Ativa) == 1,
                senhaHash: (string)row.SenhaHash,
                salt: (string)row.Salt
            );
        }

        public async Task<ContaCorrente?> GetByNumeroAsync(int numeroConta)
        {
            const string sql = @"
                                SELECT
                                    idcontacorrente AS Id,
                                    numero          AS Numero,
                                    cpf             AS CPFTitular,
                                    nome            AS NomeTitular,
                                    ativo           AS Ativa,
                                    senha           AS SenhaHash,
                                    salt            AS Salt
                                FROM contacorrente
                                WHERE numero = @Numero
                                LIMIT 1;
                                ";

            using var conn = _factory.CreateConnection();
            var row = await conn.QueryFirstOrDefaultAsync<dynamic>(sql, new { Numero = numeroConta });
            if (row is null) return null;

            return ContaCorrente.Rehidratar(
                id: Guid.Parse((string)row.Id),
                numero: (int)row.Numero,
                cpfTitular: (string)row.CPFTitular,
                nomeTitular: (string)row.NomeTitular,
                ativa: ((long)row.Ativa) == 1,
                senhaHash: (string)row.SenhaHash,
                salt: (string)row.Salt
            );
        }

        public async Task<ContaCorrente?> GetByIdAsync(Guid id)
        {
            const string sql = @"
                                SELECT
                                    idcontacorrente AS Id,
                                    numero          AS Numero,
                                    cpf             AS CPFTitular,
                                    nome            AS NomeTitular,
                                    ativo           AS Ativa,
                                    senha           AS SenhaHash,
                                    salt            AS Salt
                                FROM contacorrente
                                WHERE idcontacorrente = @Id
                                LIMIT 1;
                                ";

            using var conn = _factory.CreateConnection();
            var row = await conn.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id.ToString() });
            if (row is null) return null;

            return ContaCorrente.Rehidratar(
                id: Guid.Parse((string)row.Id),
                numero: (int)row.Numero,
                cpfTitular: (string)row.CPFTitular,
                nomeTitular: (string)row.NomeTitular,
                ativa: ((long)row.Ativa) == 1,
                senhaHash: (string)row.SenhaHash,
                salt: (string)row.Salt
            );
        }

        public async Task InativarAsync(Guid id)
        {
            const string sql = @"
                                UPDATE contacorrente
                                SET ativo = 0
                                WHERE idcontacorrente = @Id;
                                ";

            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { Id = id.ToString() });
        }


        public async Task<IEnumerable<AccountListItem>> ListAsync()
        {
            const string sql = @"
                                SELECT
                                  idcontacorrente AS ContaId,
                                  numero          AS NumeroConta,
                                  cpf             AS CPFTitular,
                                  nome            AS NomeTitular,
                                  ativo           AS Ativa
                                FROM contacorrente
                                ORDER BY nome;
                                ";

            using var conn = _factory.CreateConnection();

            var rows = await conn.QueryAsync<dynamic>(sql);

            return rows.Select(r => new AccountListItem
            {
                ContaId = Guid.Parse((string)r.ContaId),
                NumeroConta = (int)r.NumeroConta,
                CPFTitular = (string)r.CPFTitular,
                NomeTitular = (string)r.NomeTitular,
                Ativa = ((long)r.Ativa) == 1
            });
        }

    }
}