using System.Data;
using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GPMigratorApp.Services;

public class LocationService: IOrganizationService
{
    
    public LocationService()
    {
        
    }
    
    public async Task PutLocations(IEnumerable<LocationDTO> locations,IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    { 
        var organizationCommand = new OrganizationCommand(connection);
        foreach (var location in locations.Where(x=> !x.ODSSiteCode.IsNullOrEmpty()))
        {
            var existingRecord = await organizationCommand.GetOrganizationAsync(location.ODSSiteCode, cancellationToken, transaction);
            if (existingRecord is null)
            {
                await organizationCommand.InsertOrganizationAsync(location, cancellationToken,transaction);
            }
            else
            {
                if (location.Address != null && existingRecord.Address != null)
                {
                    location.Address.Id = existingRecord.Address.Id;
                }

                existingRecord.ODSCode = location.ODSCode;
                existingRecord.PeriodStart = location.PeriodStart;
                existingRecord.PeriodEnd = location.PeriodEnd;
                existingRecord.Type = location.Type;
                existingRecord.Name = location.Name;
                existingRecord.MainLocation = location.MainLocation;
                existingRecord.Address = location.Address;

                existingRecord.PartOf = location.PartOf;
                existingRecord.Contact = location.Contact;
                
                await organizationCommand.UpdateOrganizationAsync(existingRecord, cancellationToken, transaction);
            }
        }
    }
}