using System.Data;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface IOrganizationCommand
{
    Task<OrganizationDTO?> GetOrganizationAsync(string odsCode, CancellationToken cancellationToken,
        IDbTransaction transaction);

    Task<Guid> InsertOrganizationAsync(OrganizationDTO organization, CancellationToken cancellationToken,
        IDbTransaction transaction);
    
    Task<Guid> UpdateOrganizationAsync(OrganizationDTO organization, CancellationToken cancellationToken,
        IDbTransaction transaction);
}