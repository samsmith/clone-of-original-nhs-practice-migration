using System.Data;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface IObservationCommand
{
    Task<ObservationDTO?> GetObservationAsync(string originalId, CancellationToken cancellationToken, IDbTransaction transaction);

    Task<Guid> InsertObservationAsync(ObservationDTO observation, CancellationToken cancellationToken,
        IDbTransaction transaction);

    Task UpdateObservationAsync(ObservationDTO observation, CancellationToken cancellationToken,
        IDbTransaction transaction);
}