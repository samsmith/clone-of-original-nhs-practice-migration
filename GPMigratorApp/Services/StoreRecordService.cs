using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;

namespace GPMigratorApp.Services;

public class StoreRecordService : IStoreRecordService
{
    private readonly IAzureSqlDbConnectionFactory _connectionFactory;
    private readonly IOrganizationService _organizationService;
    private readonly ILocationService _locationService;
    private readonly IPracticionerService _practicionerService;

    public StoreRecordService(IAzureSqlDbConnectionFactory connectionFactory, IOrganizationService organizationService,
        ILocationService locationService, IPracticionerService practicionerService)
    {
        _connectionFactory = connectionFactory;
        _organizationService = organizationService;
        _locationService = locationService;
        _practicionerService = practicionerService;
    }

    public async Task StoreRecord(FhirResponse fhirResponse, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);
        connection.Open();
        var transaction = connection.BeginTransaction();
        try
        {
            await _organizationService.PutOrganizations(fhirResponse.Organizations, connection, transaction,
                cancellationToken);


            await _locationService.PutLocations(fhirResponse.Locations, connection, transaction,
                cancellationToken);
            
            await _practicionerService.PutPracticioners(fhirResponse.Practitioners, fhirResponse.PractitionerRoles, connection, transaction,
                cancellationToken);


            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Dispose();
        }
    }
}