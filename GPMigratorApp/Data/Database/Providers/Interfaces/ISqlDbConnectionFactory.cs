using System.Data;

namespace GPMigratorApp.Data.Database.Providers.Interfaces
{
    public interface ISqlDbConnectionFactory<T> where T : class, IDbConnection
    {
        ValueTask<T> GetReadOnlyConnectionAsync(CancellationToken cancellationToken);

        ValueTask<T> GetReadWriteConnectionAsync(CancellationToken cancellationToken);
    }

    public sealed class FailedToConnectToSqlDatabaseException : ApplicationException
    {
        public FailedToConnectToSqlDatabaseException(string? message) : base(message) { }
    }
}
