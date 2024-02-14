using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;

namespace GPMigratorApp.Services;

public class StoreRecordService: IStoreRecordService
{
    private readonly IAzureSqlDbConnectionFactory _connectionFactory;
    
    public StoreRecordService(IAzureSqlDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task StoreRecord(FhirResponse fhirResponse, CancellationToken cancellationToken)
    { 
        var connection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);
        connection.Open();
        var transaction = connection.BeginTransaction();
        var organizationCommand = new OrganizationCommand(connection);
        foreach (var organization in fhirResponse.Organizations)
        {
           await organizationCommand.PutOrganizationAsync(organization, cancellationToken,transaction);
        }
        transaction.Commit();
    }
}