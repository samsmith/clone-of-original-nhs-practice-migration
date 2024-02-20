using System.Data;
using GPMigratorApp.DTOs;

namespace GPMigratorApp.Services.Interfaces;

public interface ILocationService
{
    Task PutLocations(IEnumerable<LocationDTO> locations, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);

}