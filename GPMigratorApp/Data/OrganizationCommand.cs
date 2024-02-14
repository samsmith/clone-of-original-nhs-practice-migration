using System.Data;
using Dapper;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.Data.Types;
using GPMigratorApp.DTOs;
using Hl7.Fhir.Model;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data;

public class OrganizationCommand : IOrganizationCommand
{
    private readonly IDbConnection _connection;
    
    public OrganizationCommand(IDbConnection connection)
    {
        _connection = connection;
    }
    
    
    public async Task<Guid> PutOrganizationAsync(OrganizationDTO organization, CancellationToken cancellationToken, IDbTransaction transaction)
        {
            const string getExisting = 
                @$"SELECT 
                    [{{nameof(OrganizationLocation.OrganizationId)}}]             = org.Id,
                    [{{nameof(OrganizationLocation.OrganizationEntityId)}}]       = org.EntityId,
                    [{{nameof(OrganizationLocation.LocationId)}}]				  = loc.Id,
                    [{{nameof(OrganizationLocation.LocationEntityId)}}]			  = loc.EntityId
				FROM Organization org
				JOIN Location loc on loc.Id = @LocationODS
                WHERE LOWER(ODSCode) = LOWER(@ODSCode)
				";


            var organizations = await _connection.QueryAsync<OrganizationLocation>(getExisting, new
            {
                ODSCode = organization.ODSCode,
                LocationODS = organization.MainLocation?.ODSSiteCode
            }, transaction: transaction);

            
                var existingOrg = organizations.FirstOrDefault();

                if (existingOrg is null || !existingOrg.OrganizationId.HasValue)
                {
                    organization.Id = Guid.NewGuid();
                    organization.EntityId = Guid.NewGuid();
                    
                    var entity = @"INSERT INTO [dbo].[Entity]
                        ([Id],
                        [EntityType])
                    VALUES
                        (@Id,
                        @Type)";
                    
                    var entityDefinition = new CommandDefinition(entity, new
                    {
                        Id = organization.EntityId,
                        Type = EntityTypes.Organization,
                    }, cancellationToken: cancellationToken, transaction: transaction);
                    
                    await _connection.ExecuteAsync(entityDefinition);
                }
                else
                {
                    organization.Id = existingOrg.OrganizationId.Value;
                    organization.EntityId = existingOrg.OrganizationEntityId.Value;
                }

                if (existingOrg?.LocationId != null && organization.MainLocation is not null)
                {
                    organization.MainLocation.Id = existingOrg.LocationId.Value;
                    organization.MainLocation.EntityId = existingOrg.LocationEntityId.Value;
                }
                else if (existingOrg is {LocationId: null} && organization.MainLocation is not null)
                {
                    organization.MainLocation.Id = Guid.NewGuid();
                    organization.MainLocation.EntityId = Guid.NewGuid();
                    var entity = @"INSERT INTO [dbo].[Entity]
                        ([Id],
                        [EntityType])
                    VALUES
                        (@Id,
                        @Type)";
                    
                    var entityDefinition = new CommandDefinition(entity, new
                    {
                        Id = organization.EntityId,
                        Type = EntityTypes.Location,
                    }, cancellationToken: cancellationToken, transaction: transaction);
                    
                    await _connection.ExecuteAsync(entityDefinition);
                }

                organization.Address.Id = Guid.NewGuid();
                
                var address = @"INSERT INTO [dbo].[Address]
                       ([Id]
                       ,[Use]
                       ,[HouseNameFlatNumber]
                       ,[NumberAndStreet]
                       ,[Village]
                       ,[Town]
                       ,[County]
                       ,[Postcode]
                       ,[From]
                       ,[To])
                 VALUES
                       (@Id
                       ,@Use
                       ,@HouseName
                       ,@Number
                       ,@Village
                       ,@Town
                       ,@County
                       ,@Postcode
                       ,@From
                       ,@To)";
                    
                var organizationAddressDefinition = new CommandDefinition(address, new
                {
                    Id = organization.Address.Id,
                    Use = organization.Address.Use,
                    HouseName = organization.Address.HouseNameFlatNumber,
                    Number = organization.Address.NumberAndStreet,
                    Village = organization.Address.Village,
                    Town = organization.Address.Town,
                    County = organization.Address.County,
                    Postcode = organization.Address.Postcode,
                    From = organization.Address.From,
                    To = organization.Address.To
                }, cancellationToken: cancellationToken, transaction: transaction);
                
                await _connection.ExecuteAsync(organizationAddressDefinition);
                
                if (organization.MainLocation is not null)
                    
                {   organization.MainLocation.Address.Id = Guid.NewGuid();
                    
                    var LocationAddressDefinition = new CommandDefinition(address, new
                    {
                        Id = organization.MainLocation.Address.Id,
                        Use = organization.MainLocation.Address.Use,
                        HouseName = organization.MainLocation.Address.HouseNameFlatNumber,
                        Number = organization.MainLocation.Address.NumberAndStreet,
                        Village = organization.MainLocation.Address.Village,
                        Town = organization.MainLocation.Address.Town,
                        County = organization.MainLocation.Address.County,
                        Postcode = organization.MainLocation.Address.Postcode,
                        From = organization.MainLocation.Address.From,
                        To = organization.MainLocation.Address.To
                    }, cancellationToken: cancellationToken, transaction: transaction);
                    await _connection.ExecuteAsync(LocationAddressDefinition);
                    
                    return await InsertOrganizationLocation(organization, cancellationToken, transaction);
                }
                return await InsertOrganization(organization, cancellationToken, transaction);
                
        }

    
    private async Task<Guid> InsertOrganization(OrganizationDTO organization, CancellationToken cancellationToken, IDbTransaction transaction)
    {
         const string insertOrganization =
                @"INSERT INTO [dbo].[Organization]
                       ([Id],
                       [ODSCode],
                       [PeriodStart],
                       [PeriodEnd],
                       [Type],
                       [Name],
                       [Telecom],
                       [MainLocationID],
                       [AddressID],
                       [PartOfID],
                       [ContactID],
                       [EntityId])
                 VALUES
                       (@Id,
                       @ODSCode,
                       @PeriodStart,
                       @PeriodEnd,
                       @Type,
                       @Name,
                       @Telecom,
                       @MainLocation,
                       @AddressId,
                       @PartOf,
                       @Contact,
                       @EntityId)";
            
            var commandDefinition = new CommandDefinition(insertOrganization, new
            {
                Id = organization.Id,
                ODSCode = organization.ODSCode,
                PeriodStart = organization.PeriodStart,
                PeriodEnd = organization.PeriodEnd,
                Type = organization.Type,
                Name = organization.Name,
                Telecom = organization.Telecom,
                MainLocation = organization.MainLocation?.Id,
                AddressId = organization.Address?.Id,
                PartOf = organization.PartOf?.Id,
                Contact = organization.Contact?.Id,
                EntityId = organization.EntityId,
                
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return organization.Id;
    }
    private async Task<Guid> InsertOrganizationLocation(OrganizationDTO organization, CancellationToken cancellationToken, IDbTransaction transaction)
    {
         const string insertOrganization =
                @"INSERT INTO [dbo].[Organization]
                       ([Id],
                       [ODSCode],
                       [PeriodStart],
                       [PeriodEnd],
                       [Type],
                       [Name],
                       [Telecom],
                       [MainLocationID],
                       [AddressID],
                       [PartOfID],
                       [ContactID],
                       [EntityId])
                 VALUES
                       (@Id,
                       @ODSCode,
                       @PeriodStart,
                       @PeriodEnd,
                       @Type,
                       @Name,
                       @Telecom,
                       @MainLocation,
                       @AddressId,
                       @PartOf,
                       @Contact,
                       @EntityId)
                       
                INSERT INTO [dbo].[Location]
                       ([Id],
                       [ODSSiteCodeID],
                       [Status],
                       [OperationalStatus],
                       [Name],
                       [Alias],
                       [Description],
                       [Type],
                       [Telecom],
                       [AddressID],
                       [PhysicalType],
                       [ManagingOrganizationID],
                       [PartOfID],
                       [EntityId])
                 VALUES
                       (@LocationId,
                       @ODSSiteCode,
                       @LocationStatus,
                       @OperationalStatus,
                       @LocationName,
                       @Alias,
                       @Description,
                       @LocationType,
                       @LocationTelecom,
                       @LocationAddressId,
                       @PhysicalType,
                       @ManagingOrganizationID,
                       @PartOfId,
                       @LocationEntityId)";
            
            var commandDefinition = new CommandDefinition(insertOrganization, new
            {
                Id = organization.Id,
                ODSCode = organization.ODSCode,
                PeriodStart = organization.PeriodStart,
                PeriodEnd = organization.PeriodEnd,
                Type = organization.Type,
                Name = organization.Name,
                Telecom = organization.Telecom,
                MainLocation = organization.MainLocation?.Id,
                AddressId = organization.Address?.Id,
                PartOf = organization.PartOf?.Id,
                Contact = organization.Contact?.Id,
                EntityId = organization.EntityId,
                LocationId = organization.MainLocation?.Id,
                ODSSiteCode = organization.MainLocation?.ODSSiteCode,
                LocationStatus = organization.MainLocation?.Status,
                OperationalStatus = organization.MainLocation?.OperationalStatus,
                LocationName = organization.MainLocation?.Name,
                Alias = organization.MainLocation?.Alias,
                Description = organization.MainLocation?.Description,
                LocationType = organization.MainLocation?.Type,
                LocationTelecom = organization.MainLocation?.Telecom,
                LocationAddressId = organization.MainLocation?.Address?.Id,
                PhysicalType = organization.MainLocation?.PhysicalType,
                ManagingOrganizationID = organization.Id,
                PartOfId = organization.MainLocation?.PartOf?.Id,
                LocationEntityId = organization.MainLocation?.EntityId
                
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return organization.Id;
    }

}