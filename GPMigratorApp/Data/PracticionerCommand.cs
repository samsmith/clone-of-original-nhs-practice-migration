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

public class PracticionerCommand : IPracticionerCommand
{
    private readonly IDbConnection _connection;
    
    public PracticionerCommand(IDbConnection connection)
    {
        _connection = connection;
    }


    public async Task<PracticionerDTO?> GetPracticionerAsync(string originalId, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    string getExisting =
		    @$"SELECT [{nameof(PracticionerDTO.Id)}]							= prac.Id
                      ,[{nameof(PracticionerDTO.OriginalId)}]                  	= prac.OriginalId
                      ,[{nameof(PracticionerDTO.SdsUserId)}]                    = prac.SdsUserId
                      ,[{nameof(PracticionerDTO.SdsRoleProfileId)}]             = prac.SdsRoleProfileId
                      ,[{nameof(PracticionerDTO.Title)}]                        = prac.Title
                      ,[{nameof(PracticionerDTO.GivenName)}]                    = prac.GivenName
					  ,[{nameof(PracticionerDTO.MiddleNames)}]                  = prac.MiddleNames
                      ,[{nameof(PracticionerDTO.Surname)}]						= prac.Surname
                      ,[{nameof(PracticionerDTO.Sex)}]							= prac.Gender
  					  ,[{nameof(PracticionerDTO.DateOfBirthUtc)}]               = prac.DateOfBirthUtc
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

                  FROM [dbo].[Practicioner] prac
				  LEFT JOIN [dbo].[Address] address ON address.Id = prac.AddressID
				  WHERE prac.OriginalId = @OriginalId";
        
            var reader = await _connection.QueryMultipleAsync(getExisting, new
            {
                OriginalId = originalId
            }, transaction: transaction);
            var locations = reader.Read<PracticionerDTO, AddressDTO?, PracticionerDTO>(
                (practicioner, address) =>
                {
	                if (address is not null)
                    {
	                    practicioner.Address = address;
                    }
	                
	                return practicioner;
                }, splitOn: $"{nameof(AddressDTO.Id)}");

            return locations.FirstOrDefault();
    }

    public async Task<Guid> InsertPracticionerAsync(PracticionerDTO practicioner, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    practicioner.Id = Guid.NewGuid();
	    practicioner.EntityId = Guid.NewGuid();
                    
        var entity = @"INSERT INTO [dbo].[Entity]
                        ([Id],
                        [EntityType])
                    VALUES
                        (@Id,
                        @Type)";
                    
        var entityDefinition = new CommandDefinition(entity, new
        {
            Id = practicioner.EntityId,
            Type = EntityTypes.Practitioner,
        }, cancellationToken: cancellationToken, transaction: transaction);
                    
        await _connection.ExecuteAsync(entityDefinition);
        
        

        if (practicioner.Address is not null)
        {
	        practicioner.Address.Id = Guid.NewGuid();
	        
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

	        var practicionerAddressDefinition = new CommandDefinition(address, new
	        {
		        Id = practicioner.Address.Id,
		        Use = practicioner.Address.Use,
		        HouseName = practicioner.Address.HouseNameFlatNumber,
		        Number = practicioner.Address.NumberAndStreet,
		        Village = practicioner.Address.Village,
		        Town = practicioner.Address.Town,
		        County = practicioner.Address.County,
		        Postcode = practicioner.Address.Postcode,
		        From = practicioner.Address.From,
		        To = practicioner.Address.To
	        }, cancellationToken: cancellationToken, transaction: transaction);

	        await _connection.ExecuteAsync(practicionerAddressDefinition);
        }

        const string insertPracticioner =
                @"INSERT INTO [dbo].[Practicioner]
                       	  ([Id],
						  [OriginalId],
						  [SdsUserId],
						  [SdsRoleProfileId],
						  [Title],
						  [GivenName],
						  [MiddleNames],
						  [Surname],
						  [Gender],
						  [DateOfBirthUtc],
						  [AddressID])
                 VALUES
                       (@Id,
                       @OriginalId,
                       @SdsUserId,
                       @SdsRoleProfileId,
                       @Title,
                       @GivenName,
                       @MiddleNames,
                       @Surname,
                       @Gender,
                       @DateOfBirthUtc,
                       @AddressID)";
            
            var commandDefinition = new CommandDefinition(insertPracticioner, new
            {
                Id = practicioner.Id,
                OriginalId = practicioner.OriginalId,
                SdsUserId = practicioner.SdsUserId,
                SdsRoleProfileId = practicioner.SdsRoleProfileId,
                Title = practicioner.Title,
                GivenName = practicioner.GivenName,
                MiddleNames = practicioner.MiddleNames,
                Surname = practicioner.Surname,
                Gender = practicioner.Sex,
                DateOfBirthUtc = practicioner.DateOfBirthUtc,
                AddressID = practicioner.Address?.Id
                
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return practicioner.Id;
    }

    public async Task UpdatePracticionerAsync(PracticionerDTO practicioner, CancellationToken cancellationToken, IDbTransaction transaction)
    {
         const string updateOrganization =
                @"UPDATE [dbo].[Practicioner]
				  SET
                       [Id] = @Id,
					   [OriginalId] = @OriginalId,
					   [SdsUserId] = @SdsUserId,
					   [SdsRoleProfileId] = @SdsRoleProfileId,
					   [Title] = @Title,
					   [GivenName] = @GivenName,
					   [MiddleNames] = @MiddleNames,
					   [Surname] = @Surname,
					   [Gender] = @Gender,
					   [DateOfBirthUtc] = @DateOfBirthUtc,
					   [AddressID] = @AddressID
                 WHERE Id = @Id";
            
            var commandDefinition = new CommandDefinition(updateOrganization, new
            {
	            Id = practicioner.Id,
	            OriginalId = practicioner.OriginalId,
	            SdsUserId = practicioner.SdsUserId,
	            SdsRoleProfileId = practicioner.SdsRoleProfileId,
	            Title = practicioner.Title,
	            GivenName = practicioner.GivenName,
	            MiddleNames = practicioner.MiddleNames,
	            Surname = practicioner.Surname,
	            Gender = practicioner.Sex,
	            Address = practicioner.Address?.Id,
	            DateOfBirthUtc = practicioner.DateOfBirthUtc,
	            AddressID = practicioner.Address?.Id

            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }
    }

}