using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Accounts.Infra
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();

            using var connection = factory.CreateConnection();

            const string sql = @"
                                CREATE TABLE IF NOT EXISTS contacorrente (
                                    idcontacorrente TEXT(37) PRIMARY KEY,
                                    numero INTEGER NOT NULL UNIQUE,
                                    cpf TEXT(11) NOT NULL,
                                    nome TEXT(100) NOT NULL,
                                    ativo INTEGER(1) NOT NULL DEFAULT 0,
                                    senha TEXT(100) NOT NULL,
                                    salt TEXT(100) NOT NULL,
                                    CHECK (ativo IN (0,1))
                                );

                                CREATE TABLE IF NOT EXISTS movimento (
                                    idmovimento TEXT(37) PRIMARY KEY,
                                    idcontacorrente TEXT(37) NOT NULL,
                                    datamovimento TEXT(25) NOT NULL,
                                    tipomovimento TEXT(1) NOT NULL,
                                    valor REAL NOT NULL,
                                    CHECK (tipomovimento IN ('C','D')),
                                    FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
                                );

                                CREATE TABLE IF NOT EXISTS idempotencia (
                                    chave_idempotencia TEXT(37) PRIMARY KEY,
                                    requisicao TEXT(1000),
                                    resultado TEXT(1000)
                                );
                                ";

            await connection.ExecuteAsync(sql);
        }
    }
}
