using System.Data;
using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Database.Providers;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Database.Providers.RetryPolicy;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;
using Microsoft.Identity.Client;

namespace GPMigratorApp.Services;

public class StoreRecordService : IStoreRecordService
{
    private readonly IAzureSqlDbConnectionFactory _connectionFactory;
    private readonly IOrganizationService _organizationService;
    private readonly ILocationService _locationService;
    private readonly IPracticionerService _practicionerService;
    private readonly IPatientService _patientService;

    public StoreRecordService(IAzureSqlDbConnectionFactory connectionFactory, IOrganizationService organizationService,
        ILocationService locationService, IPracticionerService practicionerService, IPatientService patientService)
    {
        _connectionFactory = connectionFactory;
        _organizationService = organizationService;
        _locationService = locationService;
        _practicionerService = practicionerService;
        _patientService = patientService;
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

            await _patientService.PutPatients(fhirResponse.Patients, connection, transaction,
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