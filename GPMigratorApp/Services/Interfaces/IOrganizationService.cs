using System.Data;
using GPMigratorApp.DTOs;

namespace GPMigratorApp.Services.Interfaces;

public interface IOrganizationService
{
    Task PutOrganizations(IEnumerable<OrganizationDTO> organizations, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);

}