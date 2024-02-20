using System.Data;
using Dapper;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.Data.Types;
using GPMigratorApp.DTOs;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Data.SqlClient;
using Task = System.Threading.Tasks.Task;

namespace GPMigratorApp.Data;

public class LocationCommand : ILocationCommand
{
    private readonly IDbConnection _connection;
    
    public LocationCommand(IDbConnection connection)
    {
        _connection = connection;
    }


    public async Task<LocationDTO?> GetLocationAsync(string originalId, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    string getExisting =
		    @$"SELECT [{nameof(LocationDTO.Id)}]								= loc.Id
                      ,[{nameof(LocationDTO.OriginalId)}]                  	    = loc.OriginalId
                      ,[{nameof(LocationDTO.ODSSiteCode)}]                  	= loc.ODSSiteCodeID
                      ,[{nameof(LocationDTO.Status)}]                  			= loc.Status
                      ,[{nameof(LocationDTO.OperationalStatus)}]                = loc.OperationalStatus
                      ,[{nameof(LocationDTO.Name)}]                         	= loc.Name
                      ,[{nameof(LocationDTO.Alias)}]                         	= loc.Alias
                      ,[{nameof(LocationDTO.Description)}]                      = loc.Description
					  ,[{nameof(LocationDTO.Type)}]                     		= loc.Type
                      ,[{nameof(LocationDTO.Telecom)}]							= loc.Telecom
                      ,[{nameof(LocationDTO.PhysicalType)}]						= loc.PhysicalType
  					  ,[{nameof(LocationDTO.EntityId)}]                     	= loc.EntityId
                      ,[{nameof(AddressDTO.Id)}]								= address.Id
					  ,[{nameof(AddressDTO.Use)}]								= address.[Use]
					  ,[{nameof(AddressDTO.HouseNameFlatNumber)}]				= address.HouseNameFlatNumber
					  ,[{nameof(AddressDTO.NumberAndStreet)}]					= address.NumberAndStreet
					  ,[{nameof(AddressDTO.Village)}]							= address.Village
					  ,[{nameof(AddressDTO.Town )}]								= address.Town
					  ,[{nameof(AddressDTO.County )}]							= address.County
					  ,[{nameof(AddressDTO.Postcode)}]							= address.Postcode
					  ,[{nameof(AddressDTO.From)}]								= address.[From]
					  ,[{nameof(AddressDTO.To )}]								= address.[To]
					  ,[{nameof(OrganizationDTO.Id)}]							= org.Id
                      ,[{nameof(OrganizationDTO.ODSCode)}]                      = org.ODSCode
                      ,[{nameof(OrganizationDTO.PeriodStart)}]                  = org.PeriodStart
                      ,[{nameof(OrganizationDTO.PeriodEnd)}]                    = org.PeriodEnd
                      ,[{nameof(OrganizationDTO.Type)}]                         = org.Type
                      ,[{nameof(OrganizationDTO.Name)}]                         = org.Name
                      ,[{nameof(OrganizationDTO.Telecom)}]                      = org.Telecom
					  ,[{nameof(OrganizationDTO.EntityId)}]                     = org.EntityId
                      

                  FROM [dbo].[Location] loc
				  LEFT JOIN [dbo].[Organization] org ON loc.ManagingOrganizationID = org.Id
				  LEFT JOIN [dbo].[Address] address ON address.Id = loc.AddressID
				  LEFT JOIN [dbo].[Location] partof ON partof.Id = loc.PartOfID
				  WHERE loc.OriginalId = @OriginalId";
        
            var reader = await _connection.QueryMultipleAsync(getExisting, new
            {
                OriginalId = originalId
            }, transaction: transaction);
            var locations = reader.Read<LocationDTO, AddressDTO, OrganizationDTO,LocationDTO>(
                (location, address,organization) =>
                {
	                

	                if (address is not null)
                    {
	                    location.Address = address;
                    }

	                if (organization is not null)
	                    location.ManagingOrganization = organization;
                    
                   

                    return location;
                }, splitOn: $"{nameof(AddressDTO.Id)},{nameof(OrganizationDTO.Id)}");

            return locations.FirstOrDefault();
    }

    public async Task<Guid> InsertLocationAsync(LocationDTO location, CancellationToken cancellationToken, IDbTransaction transaction)
    {
        location.Id = Guid.NewGuid();
        location.EntityId = Guid.NewGuid();
                    
        var entity = @"INSERT INTO [dbo].[Entity]
                        ([Id],
                        [EntityType])
                    VALUES
                        (@Id,
                        @Type)";
                    
        var entityDefinition = new CommandDefinition(entity, new
        {
            Id = location.EntityId,
            Type = EntityTypes.Location,
        }, cancellationToken: cancellationToken, transaction: transaction);
                    
        await _connection.ExecuteAsync(entityDefinition);
        
        

        if (location.Address is not null)
        {
	        location.Address.Id = Guid.NewGuid();
	        
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
		        Id = location.Address.Id,
		        Use = location.Address.Use,
		        HouseName = location.Address.HouseNameFlatNumber,
		        Number = location.Address.NumberAndStreet,
		        Village = location.Address.Village,
		        Town = location.Address.Town,
		        County = location.Address.County,
		        Postcode = location.Address.Postcode,
		        From = location.Address.From,
		        To = location.Address.To
	        }, cancellationToken: cancellationToken, transaction: transaction);

	        await _connection.ExecuteAsync(organizationAddressDefinition);
        }

        const string insertOrganization =
                @"INSERT INTO [dbo].[Location]
                       	  ([Id]
                       	  ,[OriginalId]
					      ,[ODSSiteCodeID]
					      ,[Status]
					      ,[OperationalStatus]
					      ,[Name]
					      ,[Alias]
					      ,[Description]
					      ,[Type]
					      ,[Telecom]
					      ,[AddressID]
					      ,[PhysicalType]
					      ,[ManagingOrganizationID]
					      ,[PartOfID]
					      ,[EntityId])
                 VALUES
                       (@Id,
                       @OriginalId,
                       @ODSCode,
                       @Status,
                       @OperationalStatus,
                       @Name,
                       @Alias,
                       @Description,
                       @Type,
                       @Telecom,
                       @Address,
                       @PhysicalType,
                       (select Id from dbo.Organization where OriginalId = @ManagingOrg),
                       @PartOf,
                       @EntityId)";
            
            var commandDefinition = new CommandDefinition(insertOrganization, new
            {
                Id = location.Id,
                OriginalId = location.OriginalId,
                ODSCode = location.ODSSiteCode,
                Status = location.Status,
                OperationalStatus = location.OperationalStatus,
                Name = location.Name,
                Alias = location.Alias,
                Description = location.Description,
                Type = location.Type,
                Telecom = location.Telecom,
                Address = location.Address?.Id,
                PhysicalType = location.PhysicalType,
                ManagingOrg = location.ManagingOrganization?.OriginalId,
                PartOf = location.PartOf?.Id,
                EntityId = location.EntityId,
                
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return location.Id;
    }

    public async Task UpdateLocationAsync(LocationDTO location, CancellationToken cancellationToken, IDbTransaction transaction)
    {
         const string updateOrganization =
                @"UPDATE [dbo].[Location]
				  SET
                       [ODSSiteCodeID] = @ODSCode,
                       [Status] = @Status,
                       [OperationalStatus] = @OperationalStatus,
                       [Name] = @Name,
                       [Alias] = @Alias,
                       [Description] = @Description,
                       [Type] = @Type,
                       [Telecom] = @Telecom,
                       [AddressID] = @Address,
                       [PhysicalType] = @PhysicalType,
				       [ManagingOrganizationID] = @ManagingOrg,
                       [PartOfID] = @PartOf
                 WHERE Id = @Id";
            
            var commandDefinition = new CommandDefinition(updateOrganization, new
            {
	            Id = location.Id,
	            ODSCode = location.ODSSiteCode,
	            Status = location.Status,
	            OperationalStatus = location.OperationalStatus,
	            Name = location.Name,
	            Alias = location.Alias,
	            Description = location.Description,
	            Type = location.Type,
	            Telecom = location.Telecom,
	            Address = location.Address?.Id,
	            PhysicalType = location.PhysicalType,
	            ManagingOrg = location.ManagingOrganization?.Id,
	            PartOf = location.PartOf?.Id,
                
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }
    }

}