using System.Data;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface IPracticionerCommand
{
    Task<PracticionerDTO?> GetPracticionerAsync(string originalId, CancellationToken cancellationToken,
        IDbTransaction transaction);

    Task<Guid> InsertPracticionerAsync(PracticionerDTO practicioner, CancellationToken cancellationToken,
        IDbTransaction transaction);
    
    Task UpdatePracticionerAsync(PracticionerDTO practicioner, CancellationToken cancellationToken,
        IDbTransaction transaction);
}