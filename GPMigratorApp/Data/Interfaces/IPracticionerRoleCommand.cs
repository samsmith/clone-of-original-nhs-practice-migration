using System.Data;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface IPracticionerRoleCommand
{
    Task<PracticionerRoleDTO?> GetPracticionerRoleAsync(string originalId, CancellationToken cancellationToken,
        IDbTransaction transaction);

    Task<Guid> InsertPracticionerRoleAsync(PracticionerRoleDTO practicionerRole, CancellationToken cancellationToken,
        IDbTransaction transaction);
    
    Task UpdatePracticionerRoleAsync(PracticionerRoleDTO practicionerRole, CancellationToken cancellationToken,
        IDbTransaction transaction);
}