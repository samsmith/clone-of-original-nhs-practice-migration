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


    public async Task<OrganizationDTO?> GetOrganizationAsync(string originalId, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    string getExisting =
		    @$"SELECT [{nameof(OrganizationDTO.Id)}]							= org.Id
                      ,[{nameof(OrganizationDTO.ODSCode)}]                      = org.ODSCode
                      ,[{nameof(OrganizationDTO.PeriodStart)}]                  = org.PeriodStart
                      ,[{nameof(OrganizationDTO.PeriodEnd)}]                    = org.PeriodEnd
                      ,[{nameof(OrganizationDTO.Type)}]                         = org.Type
                      ,[{nameof(OrganizationDTO.Name)}]                         = org.Name
                      ,[{nameof(OrganizationDTO.Telecom)}]                      = org.Telecom
					  ,[{nameof(OrganizationDTO.EntityId)}]                     = org.EntityId
                      ,[{nameof(LocationDTO.Id)}]							    = loc.Id
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
                      ,[{nameof(OrganizationDTO.Id)}]							= partof.Id
					  ,[{nameof(OrganizationDTO.Name)}]							= partof.Name
					  ,[{nameof(OrganizationDTO.ODSCode)}]						= partof.ODSCode
                      ,[{nameof(ContactDTO.Id)}]								= contact.Id
					  ,[{nameof(ContactDTO.Title)}]								= contact.Title
					  ,[{nameof(ContactDTO.GivenName)}]							= contact.GivenName
					  ,[{nameof(ContactDTO.MiddleNames)}]						= contact.MiddleName
					  ,[{nameof(ContactDTO.Surname )}]							= contact.Surname
			          ,[{nameof(AddressDTO.Id)}]								= contactaddress.Id
					  ,[{nameof(AddressDTO.Use)}]								= contactaddress.[Use]
					  ,[{nameof(AddressDTO.HouseNameFlatNumber)}]				= contactaddress.HouseNameFlatNumber
					  ,[{nameof(AddressDTO.NumberAndStreet)}]					= contactaddress.NumberAndStreet
					  ,[{nameof(AddressDTO.Village)}]							= contactaddress.Village
					  ,[{nameof(AddressDTO.Town )}]								= contactaddress.Town
					  ,[{nameof(AddressDTO.County )}]							= contactaddress.County
					  ,[{nameof(AddressDTO.Postcode)}]							= contactaddress.Postcode
					  ,[{nameof(AddressDTO.From)}]								= contactaddress.[From]
					  ,[{nameof(AddressDTO.To )}]								= contactaddress.[To]
                  FROM [dbo].[Organization] org 
				  LEFT JOIN [dbo].[Location] loc ON loc.Id = org.MainLocationID
				  LEFT JOIN [dbo].[Address] address ON address.Id = org.AddressID
				  LEFT JOIN [dbo].[Organization] partof ON partof.Id = org.PartOfID
				  LEFT JOIN [dbo].[Contact] contact ON contact.Id = org.ContactID
				  LEFT JOIN [dbo].[Address] contactaddress ON contactaddress.Id = contact.AddressID
				  WHERE org.OriginalId = @OriginalId";
        
            var reader = await _connection.QueryMultipleAsync(getExisting, new
            {
	            OriginalId = originalId
            }, transaction: transaction);
            var organizations = reader.Read<OrganizationDTO, LocationDTO, AddressDTO,OrganizationDTO, ContactDTO,AddressDTO,OrganizationDTO>(
                (organization, location, address, partof, contact, contactaddress) =>
                {
                    if (location is not null)
                    {
	                    organization.MainLocation = location;
                    }
                    
                    if (address is not null)
                    {
	                    organization.Address = address;
                    }

                    if (partof is not null)
                    {
	                    organization.PartOf = partof;
                    }
                    
                    if (contact is not null)
                    {
	                    organization.Contact = contact;
	                    if (contactaddress is not null)
		                    organization.Contact.Address = contactaddress;
                    }

                    return organization;
                }, splitOn: $"{nameof(LocationDTO.Id)}, {nameof(AddressDTO.Id)},{nameof(OrganizationDTO.Id)},{nameof(ContactDTO.Id)},{nameof(AddressDTO.Id)}");

            return organizations.FirstOrDefault();
    }
    
    public async Task<Guid> InsertOrganizationAsync(OrganizationDTO organization, CancellationToken cancellationToken, IDbTransaction transaction)
    {
        organization.Id = Guid.NewGuid();
        organization.EntityId = Guid.NewGuid();
                    
        var entity = @"INSERT INTO [dbo].[Entity]
                        ([Id],
                         [OriginalId],
                        [EntityType])
                    VALUES
                        (@Id,
                        @OriginalId,
                        @Type)";
                    
        var entityDefinition = new CommandDefinition(entity, new
        {
            Id = organization.EntityId,
            Type = EntityTypes.Organization,
            OriginalId = organization.OriginalId
        }, cancellationToken: cancellationToken, transaction: transaction);
                    
        await _connection.ExecuteAsync(entityDefinition);
        
        

        if (organization.Address is not null)
        {
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
        }

        const string insertOrganization =
                @"INSERT INTO [dbo].[Organization]
                       ([Id],
                       [OriginalId],
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
                       @OriginalId,
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
                OriginalId = organization.OriginalId,
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
    public async Task<Guid> UpdateOrganizationAsync(OrganizationDTO organization, CancellationToken cancellationToken, IDbTransaction transaction)
    {
         const string updateOrganization =
                @"UPDATE [dbo].[Organization]
				  SET
                       [ODSCode] = @ODSCode,
				       [OriginalId] = @OriginalId,
                       [PeriodStart] = @PeriodStart,
                       [PeriodEnd] = @PeriodEnd,
                       [Type] = @Type,
                       [Name] = @Name,
                       [Telecom] = @Telecom,
                       [MainLocationID] = @MainLocation,
                       [AddressID] = @AddressId,
                       [PartOfID] = @PartOf,
                       [ContactID] = @Contact
                 WHERE Id = @Id";
            
            var commandDefinition = new CommandDefinition(updateOrganization, new
            {
	            Id = organization.Id,
	            OriginalId = organization.OriginalId,
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
                
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return organization.Id;
    }

}