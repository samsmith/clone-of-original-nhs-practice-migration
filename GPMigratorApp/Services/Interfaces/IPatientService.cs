using System.Data;
using GPMigratorApp.DTOs;

namespace GPMigratorApp.Services.Interfaces;

public interface IPatientService
{
    Task PutPatients(IEnumerable<PatientDTO> patients, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken);
}