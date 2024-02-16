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
    private readonly IOrganizationService _organizationService;
    
    public StoreRecordService(IAzureSqlDbConnectionFactory connectionFactory, IOrganizationService organizationService)
    {
        _connectionFactory = connectionFactory;
        _organizationService = organizationService;
    }
    
    public async Task StoreRecord(FhirResponse fhirResponse, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);
        connection.Open();
        var transaction = connection.BeginTransaction();
        try
        {
         
            var organizationCommand = new OrganizationCommand(connection);
            foreach (var organization in fhirResponse.Organizations)
            {
                await _organizationService.PutOrganizations(fhirResponse.Organizations, connection, transaction,
                    cancellationToken);
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Dispose();
        }
    }
}