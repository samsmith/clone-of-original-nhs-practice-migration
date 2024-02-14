using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Services.Interfaces;

public interface IStoreRecordService
{
    Task StoreRecord(FhirResponse fhirResponse, CancellationToken cancellationToken);
}