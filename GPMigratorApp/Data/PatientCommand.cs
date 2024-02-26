using System.Data;
using Dapper;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.Data.Types;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data;

public class PatientCommand : IPatientCommand
{
    private readonly IDbConnection _connection;
    
    public PatientCommand(IDbConnection connection)
    {
	    _connection = connection;
    }
    
     public async Task<PatientDTO?> GetPatientAsync(string originalId, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    string getExisting =
		    @$"SELECT 
					   [{nameof(PatientDTO.Id)}]								= patient.Id
                      ,[{nameof(PatientDTO.OriginalId)}]                  		= patient.OriginalId
                      ,[{nameof(PatientDTO.Gender)}]                    		= patient.Sex
                      ,[{nameof(PatientDTO.Title)}]                        		= patient.Title
                      ,[{nameof(PatientDTO.GivenName)}]                   		= patient.GivenName
					  ,[{nameof(PatientDTO.MiddleNames)}]                  		= patient.MiddleNames
                      ,[{nameof(PatientDTO.Surname)}]							= patient.Surname
  					  ,[{nameof(PatientDTO.DateOfBirthUTC)}]               		= patient.DateOfBirthUtc					  
                      ,[{nameof(PatientDTO.DateOfDeathUTC)}]                	= patient.DateOfDeathUTC
                      ,[{nameof(PatientDTO.DateOfRegistrationUTC)}]         	= patient.DateOfRegistrationUTC
                      ,[{nameof(PatientDTO.NhsNumber)}]             			= patient.NhsNumber
                      ,[{nameof(PatientDTO.PatientTypeDescription)}]        	= patient.PatientTypeDescription
                      ,[{nameof(PatientDTO.DummyType)}]                    		= patient.DummyType
					  ,[{nameof(PatientDTO.ResidentialInstituteCode)}]      	= patient.ResidentialInstituteCode
                      ,[{nameof(PatientDTO.NHSNumberStatus)}]					= patient.NHSNumberStatus
                      ,[{nameof(PatientDTO.CarerName)}]							= patient.CarerName
  					  ,[{nameof(PatientDTO.DateOfBirthUTC)}]                	= patient.DateOfBirthUtc					  
                      ,[{nameof(PatientDTO.OriginalId)}]                  		= patient.OriginalId
                      ,[{nameof(PatientDTO.CarerRelation)}]                 	= patient.CarerRelation
                      ,[{nameof(PatientDTO.PersonGuid)}]             			= patient.PersonGuid
                      ,[{nameof(PatientDTO.DateOfDeactivation)}]                = patient.DateOfDeactivation
                      ,[{nameof(PatientDTO.Deleted)}]                    		= patient.Deleted
					  ,[{nameof(PatientDTO.Active)}]                  			= patient.Active
                      ,[{nameof(PatientDTO.SpineSensitive)}]					= patient.SpineSensitive
                      ,[{nameof(PatientDTO.IsConfidential)}]					= patient.IsConfidential
  					  ,[{nameof(PatientDTO.EmailAddress)}]               		= patient.EmailAddress
                      ,[{nameof(PatientDTO.HomePhone)}]							= patient.HomePhone
					  ,[{nameof(PatientDTO.MobilePhone)}]						= patient.MobilePhone
					  ,[{nameof(PatientDTO.ProcessingId)}]						= patient.ProcessingId
					  ,[{nameof(PatientDTO.Ethnicity)}]							= patient.Ethnicity
					  ,[{nameof(PatientDTO.Religion)}]							= patient.Religion
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
					                                       

                  FROM [dbo].[Patient] patient
				  LEFT JOIN [dbo].[Address] address ON address.Id = patient.HomeAddress
				  LEFT JOIN [dbo].[Organization] org ON org.Id = patient.ManagingOrganization
				  LEFT JOIN [dbo].[Practicioner] prac ON prac.Id = patient.Practicioner
				  WHERE patient.OriginalId = @OriginalId";
        
            var reader = await _connection.QueryMultipleAsync(getExisting, new
            {
	            OriginalId = originalId
            }, transaction: transaction);
            var patients = reader.Read<PatientDTO, AddressDTO?, OrganizationDTO?, PracticionerDTO?, PatientDTO>(
                (patient, address, organization, practicioner) =>
                {
	                if (address is not null)
	                {
		                patient.HomeAddress = address;
	                }
	                if (organization is not null)
	                {
		                patient.ManagingOrganization = organization;
	                }
	                if (practicioner is not null)
	                {
		                patient.UsualGP = practicioner;
	                }

	                return patient;
                }, splitOn: $"{nameof(AddressDTO.Id)},{nameof(OrganizationDTO.Id)},{nameof(PracticionerDTO.Id)}");

            return patients.FirstOrDefault();
    }
    
         
     public async Task<PatientDTO?> GetPatientByNHSNumberAsync(string nhsNumber, CancellationToken cancellationToken)
    {
	    string getExisting =
		    @$"SELECT 
					   [{nameof(PatientDTO.Id)}]								= patient.Id
                      ,[{nameof(PatientDTO.OriginalId)}]                  		= patient.OriginalId
                      ,[{nameof(PatientDTO.Gender)}]                    		= patient.Sex
                      ,[{nameof(PatientDTO.Title)}]                        		= patient.Title
                      ,[{nameof(PatientDTO.GivenName)}]                   		= patient.GivenName
					  ,[{nameof(PatientDTO.MiddleNames)}]                  		= patient.MiddleNames
                      ,[{nameof(PatientDTO.Surname)}]							= patient.Surname
  					  ,[{nameof(PatientDTO.DateOfBirthUTC)}]               		= patient.DateOfBirthUtc					  
                      ,[{nameof(PatientDTO.DateOfDeathUTC)}]                	= patient.DateOfDeathUTC
                      ,[{nameof(PatientDTO.DateOfRegistrationUTC)}]         	= patient.DateOfRegistrationUTC
                      ,[{nameof(PatientDTO.NhsNumber)}]             			= patient.NhsNumber
                      ,[{nameof(PatientDTO.PatientTypeDescription)}]        	= patient.PatientTypeDescription
                      ,[{nameof(PatientDTO.DummyType)}]                    		= patient.DummyType
					  ,[{nameof(PatientDTO.ResidentialInstituteCode)}]      	= patient.ResidentialInstituteCode
                      ,[{nameof(PatientDTO.NHSNumberStatus)}]					= patient.NHSNumberStatus
                      ,[{nameof(PatientDTO.CarerName)}]							= patient.CarerName
  					  ,[{nameof(PatientDTO.DateOfBirthUTC)}]                	= patient.DateOfBirthUtc					  
                      ,[{nameof(PatientDTO.OriginalId)}]                  		= patient.OriginalId
                      ,[{nameof(PatientDTO.CarerRelation)}]                 	= patient.CarerRelation
                      ,[{nameof(PatientDTO.PersonGuid)}]             			= patient.PersonGuid
                      ,[{nameof(PatientDTO.DateOfDeactivation)}]                = patient.DateOfDeactivation
                      ,[{nameof(PatientDTO.Deleted)}]                    		= patient.Deleted
					  ,[{nameof(PatientDTO.Active)}]                  			= patient.Active
                      ,[{nameof(PatientDTO.SpineSensitive)}]					= patient.SpineSensitive
                      ,[{nameof(PatientDTO.IsConfidential)}]					= patient.IsConfidential
  					  ,[{nameof(PatientDTO.EmailAddress)}]               		= patient.EmailAddress
                      ,[{nameof(PatientDTO.HomePhone)}]							= patient.HomePhone
					  ,[{nameof(PatientDTO.MobilePhone)}]						= patient.MobilePhone
					  ,[{nameof(PatientDTO.ProcessingId)}]						= patient.ProcessingId
					  ,[{nameof(PatientDTO.Ethnicity)}]							= patient.Ethnicity
					  ,[{nameof(PatientDTO.Religion)}]							= patient.Religion
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
					                                       

                  FROM [dbo].[Patient] patient
				  LEFT JOIN [dbo].[Address] address ON address.Id = patient.HomeAddress
				  LEFT JOIN [dbo].[Organization] org ON org.Id = patient.ManagingOrganization
				  LEFT JOIN [dbo].[Practicioner] prac ON prac.Id = patient.Practicioner
				  WHERE patient.NHSNumber = @NhsNumber";
        
            var reader = await _connection.QueryMultipleAsync(getExisting, new
            {
	            NhsNumber = nhsNumber
            }
            
            
            
            
            
            
            
            
            
            
            
            
            );
            var patients = reader.Read<PatientDTO, AddressDTO?, OrganizationDTO?, PracticionerDTO?, PatientDTO>(
                (patient, address, organization, practicioner) =>
                {
	                if (address is not null)
	                {
		                patient.HomeAddress = address;
	                }
	                if (organization is not null)
	                {
		                patient.ManagingOrganization = organization;
	                }
	                if (practicioner is not null)
	                {
		                patient.UsualGP = practicioner;
	                }

	                return patient;
                }, splitOn: $"{nameof(AddressDTO.Id)},{nameof(OrganizationDTO.Id)},{nameof(PracticionerDTO.Id)}");

            return patients.FirstOrDefault();
    }
    
    public async Task<Guid> InsertPatientAsync(PatientDTO patient, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    patient.Id = Guid.NewGuid();
	    patient.EntityId = Guid.NewGuid();
	                
        var entity = @"INSERT INTO [dbo].[Entity]
                        ([Id],
                        [EntityType])
                    VALUES
                        (@Id,
                        @Type)";
                    
        var entityDefinition = new CommandDefinition(entity, new
        {
            Id = patient.EntityId,
            Type = EntityTypes.Patient,
        }, cancellationToken: cancellationToken, transaction: transaction);
                    
        await _connection.ExecuteAsync(entityDefinition);
        
        

        if (patient.HomeAddress is not null)
        {
	        patient.HomeAddress.Id = Guid.NewGuid();
	        
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
		        Id = patient.HomeAddress.Id,
		        Use = patient.HomeAddress.Use,
		        HouseName = patient.HomeAddress.HouseNameFlatNumber,
		        Number = patient.HomeAddress.NumberAndStreet,
		        Village = patient.HomeAddress.Village,
		        Town = patient.HomeAddress.Town,
		        County = patient.HomeAddress.County,
		        Postcode = patient.HomeAddress.Postcode,
		        From = patient.HomeAddress.From,
		        To = patient.HomeAddress.To
	        }, cancellationToken: cancellationToken, transaction: transaction);

	        await _connection.ExecuteAsync(practicionerAddressDefinition);
        }
            const string insertPatient =
                @"INSERT INTO [dbo].[Patient]
                   ([Id]
                   ,[OriginalId]
                   ,[ManagingOrganization]
                   ,[Practicioner]
                   ,[Sex]
                   ,[DateOfBirthUTC]
                   ,[DateOfDeathUTC]
                   ,[Title]
                   ,[GivenName]
                   ,[MiddleNames]
                   ,[Surname]
                   ,[DateOfRegistrationUTC]
                   ,[NhsNumber]
                   ,[PatientTypeDescription]
                   ,[DummyType]
                   ,[ResidentialInstituteCode]
                   ,[NHSNumberStatus]
                   ,[CarerName]
                   ,[CarerRelation]
                   ,[PersonGuid]
                   ,[DateOfDeactivation]
                   ,[Deleted]
                   ,[Active]
                   ,[SpineSensitive]
                   ,[IsConfidential]
                   ,[EmailAddress]
                   ,[HomePhone]
                   ,[MobilePhone]
                   ,[ProcessingId]
                   ,[Ethnicity]
                   ,[Religion]
                   ,[HomeAddress]
                   ,[EntityId])
             VALUES
                   (
                    @PatientId,
                    @OriginalId,
                    (select Id from dbo.Organization where OriginalId = @Organization),
                    (select Id from dbo.Practicioner where OriginalId = @Practicioner),
                    @Sex,
                    @DateOfBirthUTC,
                    @DateOfDeathUTC,
                    @Title,
                    @GivenName,
                    @MiddleNames,
                    @Surname,
                    @DateOfRegistrationUTC,
                    @NhsNumber,
                    @PatientTypeDescription,
                    @DummyType,
                    @ResidentialInstituteCode,
                    @NHSNumberStatus,
                    @CarerName,
                    @CarerRelation,
                    @PersonGuid,
                    @DateOfDeactivation,
                    @Deleted,
                    @Active,
                    @SpineSensitive,
                    @IsConfidential,
                    @EmailAddress,
                    @HomePhone,
                    @MobilePhone,
                    @ProcessingId,
                    @Ethnicity,
                    @Religion,
                    @HomeAddress,
                 	@EntityId
                   )";

            var commandDefinition = new CommandDefinition(insertPatient, new
            {
                PatientId = patient.Id,
                OriginalId = patient.OriginalId,
                Organization = patient.ManagingOrganization.OriginalId,
                Active = patient.Active,
                Practicioner = patient.UsualGP?.OriginalId,
                Sex = patient.Gender,
                DateOfBirthUTC = patient.DateOfBirthUTC,
                DateOfDeathUTC = patient.DateOfDeathUTC,
                Title = patient.Title,
                GivenName = patient.GivenName,
                MiddleNames = patient.MiddleNames,
                Surname = patient.Surname,
                DateOfRegistrationUTC = patient.DateOfRegistrationUTC,
                NhsNumber = patient.NhsNumber,
                PatientTypeDescription = patient.PatientTypeDescription,
                DummyType = patient.DummyType,
                ResidentialInstituteCode = patient.ResidentialInstituteCode,
                NHSNumberStatus = patient.NHSNumberStatus,
                CarerName = patient.CarerName,
                CarerRelation = patient.CarerRelation,
                PersonGuid = patient.PersonGuid,
                DateOfDeactivation = patient.DateOfDeactivation,
                Deleted = patient.Deleted,
                SpineSensitive = patient.SpineSensitive,
                IsConfidential = patient.IsConfidential,
                EmailAddress = patient.EmailAddress,
                HomePhone = patient.HomePhone,
                MobilePhone = patient.MobilePhone,
                ProcessingId = patient.ProcessingId,
                Ethnicity = patient.Ethnicity,
                Religion = patient.Religion,
                HomeAddress = patient.HomeAddress?.Id,
                EntityId = patient.EntityId

            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result is 0)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return patient.Id;
        }

    public async Task UpdatePatientAsync(PatientDTO patient, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    const string updatePatient =
		    @"UPDATE [dbo].[Patient]
				  SET
                   		[OriginalId] 				= @OriginalId
                   		,[ManagingOrganization] 	= @Organization
                   		,[Practicioner] 			= @Practicioner
                   		,[Sex] 						= @Sex
                   		,[DateOfBirthUTC] 			= @DateOfBirthUTC
                   		,[DateOfDeathUTC]			= @DateOfDeathUTC
                   		,[Title]					= @Title
                   		,[GivenName]				= @GivenName
                   		,[MiddleNames]				= @MiddleNames
                   		,[Surname]					= @Surname
                   		,[DateOfRegistrationUTC]	= @DateOfRegistrationUTC
                   		,[NhsNumber]				= @NhsNumber
                   		,[PatientTypeDescription]	= @PatientTypeDescription
                   		,[DummyType]				= @DummyType
                   		,[ResidentialInstituteCode]	= @ResidentialInstituteCode
                   		,[NHSNumberStatus]			= @NHSNumberStatus
                   		,[CarerName]				= @CarerName
                   		,[CarerRelation]			= @CarerRelation
                   		,[PersonGuid]				= @PersonGuid
                   		,[DateOfDeactivation]		= @DateOfDeactivation
                   		,[Deleted]					= @Deleted
                   		,[Active]					= @Active
                   		,[SpineSensitive]			= @SpineSensitive
                   		,[IsConfidential]			= @IsConfidential
                   		,[EmailAddress]				= @EmailAddress
                   		,[HomePhone]				= @HomePhone
                   		,[MobilePhone]				= @MobilePhone
                   		,[ProcessingId]				= @ProcessingId
                   		,[Ethnicity]				= @Ethnicity
                   		,[Religion]					= @Religion
                   		,[HomeAddress]				= @HomeAddress
                 WHERE Id = @Id";
            
	    var commandDefinition = new CommandDefinition(updatePatient, new
	    {
		    Id = patient.Id,
		    OriginalId = patient.OriginalId,
		    Organization = patient.ManagingOrganization.Id,
		    Active = patient.Active,
		    Practicioner = patient.UsualGP?.Id,
		    Sex = patient.Gender,
		    DateOfBirthUTC = patient.DateOfBirthUTC,
		    DateOfDeathUTC = patient.DateOfDeathUTC,
		    Title = patient.Title,
		    GivenName = patient.GivenName,
		    MiddleNames = patient.MiddleNames,
		    Surname = patient.Surname,
		    DateOfRegistrationUTC = patient.DateOfRegistrationUTC,
		    NhsNumber = patient.NhsNumber,
		    PatientTypeDescription = patient.PatientTypeDescription,
		    DummyType = patient.DummyType,
		    ResidentialInstituteCode = patient.ResidentialInstituteCode,
		    NHSNumberStatus = patient.NHSNumberStatus,
		    CarerName = patient.CarerName,
		    CarerRelation = patient.CarerRelation,
		    PersonGuid = patient.PersonGuid,
		    DateOfDeactivation = patient.DateOfDeactivation,
		    Deleted = patient.Deleted,
		    SpineSensitive = patient.SpineSensitive,
		    IsConfidential = patient.IsConfidential,
		    EmailAddress = patient.EmailAddress,
		    HomePhone = patient.HomePhone,
		    MobilePhone = patient.MobilePhone,
		    ProcessingId = patient.ProcessingId,
		    Ethnicity = patient.Ethnicity,
		    Religion = patient.Religion,
		    HomeAddress = patient.HomeAddress?.Id,
		    EntityId = patient.EntityId

	    }, cancellationToken: cancellationToken, transaction: transaction);
            
	    var result = await _connection.ExecuteAsync(commandDefinition);
	    if (result == 0)
	    {
		    throw new DataException("Error: Patient was not updated");
	    }
    }
}