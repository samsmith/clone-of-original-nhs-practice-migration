using System.Data;
using GPMigratorApp.DTOs;

namespace GPMigratorApp.Services.Interfaces;

public interface IObservationService
{
    Task PutObservations(IEnumerable<ObservationDTO> observations, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);

}