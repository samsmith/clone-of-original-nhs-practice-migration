using System.Data;
using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GPMigratorApp.Services;

public class LocationService: ILocationService
{
    
    public LocationService()
    {
        
    }
    
    public async Task PutLocations(IEnumerable<LocationDTO> locations,IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    { 
        var locationCommand = new LocationCommand(connection);
        foreach (var location in locations)
        {
            var existingRecord = await locationCommand.GetLocationAsync(location.OriginalId, cancellationToken, transaction);
            if (existingRecord is null)
            {
                await locationCommand.InsertLocationAsync(location, cancellationToken,transaction);
            }
            else
            {
                if (location.Address != null && existingRecord.Address != null)
                {
                    location.Address.Id = existingRecord.Address.Id;
                }

                existingRecord.ODSSiteCode = location.ODSSiteCode;
                existingRecord.Status = location.Status;
                existingRecord.OperationalStatus = location.OperationalStatus;
                existingRecord.Name = location.Name;
                existingRecord.Alias = location.Alias;
                existingRecord.Description = location.Description;
                existingRecord.Type = location.Type;
                existingRecord.Telecom = location.Telecom;
                existingRecord.PhysicalType = location.PhysicalType;

                await locationCommand.UpdateLocationAsync(existingRecord, cancellationToken, transaction);
            }
        }
    }
}