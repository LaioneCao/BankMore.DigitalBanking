using Microsoft.Data.Sqlite;
using System.Data;

namespace BankMore.Transfers.Api.Infra
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConnectionStrings:Default não configurada.");
        }

        public IDbConnection CreateConnection()
            => new SqliteConnection(_connectionString);
    }

}
