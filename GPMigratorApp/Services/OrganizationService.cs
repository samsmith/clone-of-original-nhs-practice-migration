using System.Data;
using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GPMigratorApp.Services;

public class OrganizationService: IOrganizationService
{
    
    public OrganizationService()
    {
        
    }
    
    public async Task PutOrganizations(IEnumerable<OrganizationDTO> organizations,IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    { 
        var organizationCommand = new OrganizationCommand(connection);
        foreach (var organization in organizations)
        {
            var existingRecord = await organizationCommand.GetOrganizationAsync(organization.OriginalId, cancellationToken, transaction);
            if (existingRecord is null)
            {
                await organizationCommand.InsertOrganizationAsync(organization, cancellationToken,transaction);
            }
            else
            {
                if (organization.Address != null && existingRecord.Address != null)
                {
                    organization.Address.Id = existingRecord.Address.Id;
                }

                existingRecord.OriginalId = organization.OriginalId;
                existingRecord.ODSCode = organization.ODSCode;
                existingRecord.PeriodStart = organization.PeriodStart;
                existingRecord.PeriodEnd = organization.PeriodEnd;
                existingRecord.Type = organization.Type;
                existingRecord.Name = organization.Name;
                existingRecord.MainLocation = organization.MainLocation;
                existingRecord.Address = organization.Address;

                existingRecord.PartOf = organization.PartOf;
                existingRecord.Contact = organization.Contact;
                
                await organizationCommand.UpdateOrganizationAsync(existingRecord, cancellationToken, transaction);
            }
        }
    }
}