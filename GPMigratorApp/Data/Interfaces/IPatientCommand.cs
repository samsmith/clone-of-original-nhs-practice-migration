using System.Data;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface IPatientCommand
{
    Task<PatientDTO?> GetPatientAsync(string nhsNumber, CancellationToken cancellationToken, IDbTransaction transaction);
    Task<Guid> InsertPatientAsync(PatientDTO patient, CancellationToken cancellationToken, IDbTransaction transaction);
    Task UpdatePatientAsync(PatientDTO patient, CancellationToken cancellationToken, IDbTransaction transaction);
}