using System.Data;

namespace GPMigratorApp.Data.Database.Providers.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateReadOnlyConnection();
        IDbConnection CreateWriteOnlyConnection();
    }
}
