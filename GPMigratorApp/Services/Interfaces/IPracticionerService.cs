using System.Data;
using GPMigratorApp.DTOs;

namespace GPMigratorApp.Services.Interfaces;

public interface IPracticionerService
{
    Task PutPracticioners(IEnumerable<PracticionerDTO> practicioners, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);

}