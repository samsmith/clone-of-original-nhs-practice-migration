using System.Data;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface ILocationCommand
{
    Task<LocationDTO?> GetLocationAsync(string originalId, CancellationToken cancellationToken,
        IDbTransaction transaction);

    Task<Guid> InsertLocationAsync(LocationDTO location, CancellationToken cancellationToken,
        IDbTransaction transaction);
    
    Task UpdateLocationAsync(LocationDTO location, CancellationToken cancellationToken,
        IDbTransaction transaction);
}