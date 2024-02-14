using System.Data;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface IOrganizationCommand
{
    Task<Guid> PutOrganizationAsync(OrganizationDTO organization, CancellationToken cancellationToken, IDbTransaction transaction);
}