using System.Data;
using GPMigratorApp.DTOs;

namespace GPMigratorApp.Services.Interfaces;

public interface IPatientService
{
    Task PutPatientsAsync(IEnumerable<PatientDTO> patients, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken);

    Task<PatientDTO> GetPatientAsync(string nhsNumber, CancellationToken cancellationToken);
}