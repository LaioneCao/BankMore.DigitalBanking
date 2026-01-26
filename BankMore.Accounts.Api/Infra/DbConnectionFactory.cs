using System.Data;
using Microsoft.Data.Sqlite;

namespace BankMore.Accounts.Api.Infra
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
                ?? throw new InvalidOperationException("Connection string 'Default' não encontrada.");
        }

        public IDbConnection CreateConnection()
            => new SqliteConnection(_connectionString);
    }
}
