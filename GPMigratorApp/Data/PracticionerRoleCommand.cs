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

public class PracticionerRoleCommand : IPracticionerRoleCommand
{
    private readonly IDbConnection _connection;
    
    public PracticionerRoleCommand(IDbConnection connection)
    {
        _connection = connection;
    }


    public async Task<PracticionerRoleDTO?> GetPracticionerRoleAsync(string originalId, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    string getExisting =
		    @$"SELECT [{nameof(PracticionerRoleDTO.Id)}]						= pracRole.Id
                      ,[{nameof(PracticionerRoleDTO.OriginalId)}]               = pracRole.OriginalId
                      ,[{nameof(PracticionerRoleDTO.Active)}]                   = pracRole.Active
                      ,[{nameof(PracticionerRoleDTO.PeriodStart)}]              = pracRole.PeriodStart
                      ,[{nameof(PracticionerRoleDTO.PeriodEnd)}]                = pracRole.PeriodEnd
                      ,[{nameof(PracticionerRoleDTO.SDSJobRoleName)}]           = pracRole.SDSJobRoleName
					  ,[{nameof(PracticionerRoleDTO.Speciality)}]               = pracRole.Speciality
                      ,[{nameof(PracticionerRoleDTO.Telecom)}]					= pracRole.Telecom
                      ,[{nameof(PracticionerDTO.Id)}]							= prac.Id
                      ,[{nameof(PracticionerDTO.OriginalId)}]                  	= prac.OriginalId
                      ,[{nameof(PracticionerDTO.SdsUserId)}]                    = prac.SdsUserId
                      ,[{nameof(PracticionerDTO.SdsRoleProfileId)}]             = prac.SdsRoleProfileId
                      ,[{nameof(PracticionerDTO.Title)}]                        = prac.Title
                      ,[{nameof(PracticionerDTO.GivenName)}]                    = prac.GivenName
					  ,[{nameof(PracticionerDTO.MiddleNames)}]                  = prac.MiddleNames
                      ,[{nameof(PracticionerDTO.Surname)}]						= prac.Surname
                      ,[{nameof(PracticionerDTO.Sex)}]							= prac.Gender
  					  ,[{nameof(PracticionerDTO.DateOfBirthUtc)}]               = prac.DateOfBirthUtc
					  ,[{nameof(OrganizationDTO.Id)}]							= org.Id
                      ,[{nameof(OrganizationDTO.ODSCode)}]                      = org.ODSCode
                      ,[{nameof(OrganizationDTO.PeriodStart)}]                  = org.PeriodStart
                      ,[{nameof(OrganizationDTO.PeriodEnd)}]                    = org.PeriodEnd
                      ,[{nameof(OrganizationDTO.Type)}]                         = org.Type
                      ,[{nameof(OrganizationDTO.Name)}]                         = org.Name
                      ,[{nameof(OrganizationDTO.Telecom)}]                      = org.Telecom
					  ,[{nameof(OrganizationDTO.EntityId)}]                     = org.EntityId
					  ,[{nameof(LocationDTO.Id)}]								= loc.Id
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

                  FROM [dbo].[PracticionerRole] pracRole
				  LEFT JOIN [dbo].[Practicioner] prac ON prac.Id = pracRole.Practicioner
				  LEFT JOIN [dbo].[Organization] org ON org.Id = pracRole.Organization
				  LEFT JOIN [dbo].[Location] loc ON loc.Id = pracRole.Location
				  WHERE pracRole.OriginalId = @OriginalId";
        
            var reader = await _connection.QueryMultipleAsync(getExisting, new
            {
                OriginalId = originalId
            }, transaction: transaction);
            var practicionerRoles = reader.Read<PracticionerRoleDTO, PracticionerDTO, OrganizationDTO?, LocationDTO?, PracticionerRoleDTO>(
                (practicionerRole, practicioner, organization, location) =>
                {
	                practicionerRole.Practicioner = practicioner;

	                if (organization is not null)
	                {
		                practicionerRole.Organization = organization;
	                }
	                if (location is not null)
	                {
		                practicionerRole.Location = location;
	                }
	                
	                return practicionerRole;
                }, splitOn: $"{nameof(PracticionerDTO.Id)},{nameof(OrganizationDTO.Id)},{nameof(LocationDTO.Id)}");

            return practicionerRoles.FirstOrDefault();
    }

    public async Task<Guid> InsertPracticionerRoleAsync(PracticionerRoleDTO practicionerRole, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    practicionerRole.Id = Guid.NewGuid();
	    
        const string insertPracticioner =
                @"INSERT INTO [dbo].[PracticionerRole]
                       	  ([Id],
						  [OriginalId],
						  [Active],
						  [PeriodStart],
						  [PeriodEnd],
						  [Practicioner],
						  [Organization],
						  [SDSJobRoleName],
						  [Speciality],
						  [Location],
						  [Telecom])
                 VALUES
                       (@Id,
                       @OriginalId,
                       @Active,
                       @PeriodStart,
                       @PeriodEnd,
                       (select Id from dbo.Practicioner where OriginalId = @Practicioner),
                       (select Id from dbo.Organization where OriginalId = @Organization),
                       @SDSJobRoleName,
                       @Speciality,
                       (select Id from dbo.Location where OriginalId = @Location),
                       @Telecom)";
            
            var commandDefinition = new CommandDefinition(insertPracticioner, new
            {
                Id = practicionerRole.Id,
                OriginalId = practicionerRole.OriginalId,
                Active = practicionerRole.Active,
                PeriodStart = practicionerRole.PeriodStart,
                PeriodEnd = practicionerRole.PeriodEnd,
                Practicioner = practicionerRole.Practicioner?.OriginalId,
                Organization = practicionerRole.Organization?.OriginalId,
                SDSJobRoleName = practicionerRole.SDSJobRoleName,
                Speciality = practicionerRole.Speciality,
                Location = practicionerRole.Location?.OriginalId,
                Telecom = practicionerRole.Telecom
                
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return practicionerRole.Id;
    }

    public async Task UpdatePracticionerRoleAsync(PracticionerRoleDTO practicionerRole, CancellationToken cancellationToken,
	    IDbTransaction transaction)
    {
         const string updateRole =
                @"UPDATE [dbo].[PracticionerRole]
				  SET
					   [OriginalId] = @OriginalId,
					   [Active] = @Active,
					   [PeriodStart] = @PeriodStart,
					   [PeriodEnd] = @PeriodEnd,
					   [Practicioner] = @Practicioner,
					   [Organization] = @Organization,
					   [SDSJobRoleName] = @SDSJobRoleName,
					   [Speciality] = @Speciality,
					   [Location] = @Location,
					   [Telecom] = @Telecom
                 WHERE Id = @Id";
            
            var commandDefinition = new CommandDefinition(updateRole, new
            {
	            Id = practicionerRole.Id,
	            OriginalId = practicionerRole.OriginalId,
	            Active = practicionerRole.Active,
	            PeriodStart = practicionerRole.PeriodStart,
	            PeriodEnd = practicionerRole.PeriodEnd,
	            Practicioner = practicionerRole.Practicioner?.Id,
	            Organization = practicionerRole.Organization?.Id,
	            SDSJobRoleName = practicionerRole.SDSJobRoleName,
	            Speciality = practicionerRole.Speciality,
	            Location = practicionerRole.Location?.Id,
	            Telecom = practicionerRole.Telecom

            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }
    }

}