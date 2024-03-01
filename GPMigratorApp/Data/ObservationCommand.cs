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

public class ObservationCommand : IObservationCommand
{
    private readonly IDbConnection _connection;
    
    public ObservationCommand(IDbConnection connection)
    {
        _connection = connection;
    }


    public async Task<ObservationDTO?> GetObservationAsync(string originalId, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    string getExisting =
		    @$"SELECT
      				   [{nameof(ObservationDTO.Id)}]							= observation.Id
                      ,[{nameof(ObservationDTO.OriginalId)}]                  	= observation.OriginalId
      				  ,[{nameof(ObservationDTO.Status)}]                  		= observation.Status
      				  ,[{nameof(ObservationDTO.Category)}]                  	= observation.Category
      				  ,[{nameof(ObservationDTO.Code)}]                  		= observation.Code
      				  ,[{nameof(ObservationDTO.Context)}]                  		= observation.ContextId
      				  ,[{nameof(ObservationDTO.EffectiveDate)}]                 = observation.EffectiveDate
      				  ,[{nameof(ObservationDTO.EffectiveDateFrom)}]             = observation.EffectiveDateFrom
      				  ,[{nameof(ObservationDTO.EffectiveDateTo)}]               = observation.EffectiveDateTo
      				  ,[{nameof(ObservationDTO.Issued)}]                  		= observation.Issued
      				  ,[{nameof(ObservationDTO.Interpretation)}]                = observation.Interpretation
      				  ,[{nameof(ObservationDTO.DataAbsentReason)}]              = observation.DataAbsentReason
      				  ,[{nameof(ObservationDTO.Comment)}]                  		= observation.Comment
      				  ,[{nameof(ObservationDTO.BodySite)}]                  	= observation.BodySite
      				  ,[{nameof(ObservationDTO.Method)}]                  		= observation.Method
      				  ,[{nameof(ObservationDTO.ReferenceRangeLow)}]             = observation.ReferenceRangeLow
      				  ,[{nameof(ObservationDTO.ReferenceRangeHigh)}]            = observation.ReferenceRangeHigh
      				  ,[{nameof(ObservationDTO.ReferenceRangeType)}]            = observation.ReferenceRangeType
      				  ,[{nameof(ObservationDTO.ReferenceRangeAppliesTo)}]       = observation.ReferenceRangeAppliesTo
      				  ,[{nameof(ObservationDTO.ReferenceRangeAgeHigh)}]         = observation.ReferenceRangeAgeHigh
      				  ,[{nameof(ObservationDTO.ReferenceRangeAgeLow)}]          = observation.ReferenceRangeAgeLow
      				  ,[{nameof(ObservationDTO.EntityId)}]                  	= observation.Entityid
				  	  ,[{nameof(OutboundRelationship.Id)}]                  	= basedOn.Id
				  	  ,[{nameof(OutboundRelationship.Type)}]                  	= basedOn.EntityType
				  	  ,[{nameof(PatientDTO.Id)}]								= patient.Id
                      ,[{nameof(PatientDTO.OriginalId)}]                  		= patient.OriginalId
                      ,[{nameof(PatientDTO.Gender)}]                    		= patient.Gender
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
				  	  ,[{nameof(OutboundRelationship.Id)}]                  	= performer.Id
				  	  ,[{nameof(OutboundRelationship.Type)}]                  	= performer.EntityType
				  	  ,[{nameof(ObservationDTO.Id)}]							= relatedTo.Id
                      ,[{nameof(ObservationDTO.OriginalId)}]                  	= relatedTo.OriginalId
      				  ,[{nameof(ObservationDTO.Status)}]                  		= relatedTo.Status
      				  ,[{nameof(ObservationDTO.Category)}]                  	= relatedTo.Category
      				  ,[{nameof(ObservationDTO.Code)}]                  		= relatedTo.Code
      				  ,[{nameof(ObservationDTO.Context)}]                  		= relatedTo.ContextId
      				  ,[{nameof(ObservationDTO.EffectiveDate)}]                 = relatedTo.EffectiveDate
      				  ,[{nameof(ObservationDTO.EffectiveDateFrom)}]             = relatedTo.EffectiveDateFrom
      				  ,[{nameof(ObservationDTO.EffectiveDateTo)}]               = relatedTo.EffectiveDateTo
      				  ,[{nameof(ObservationDTO.Issued)}]                  		= relatedTo.Issued
      				  ,[{nameof(ObservationDTO.Interpretation)}]                = relatedTo.Interpretation
      				  ,[{nameof(ObservationDTO.DataAbsentReason)}]              = relatedTo.DataAbsentReason
      				  ,[{nameof(ObservationDTO.Comment)}]                  		= relatedTo.Comment
      				  ,[{nameof(ObservationDTO.BodySite)}]                  	= relatedTo.BodySite
      				  ,[{nameof(ObservationDTO.Method)}]                  		= relatedTo.Method
      				  ,[{nameof(ObservationDTO.ReferenceRangeLow)}]             = relatedTo.ReferenceRangeLow
      				  ,[{nameof(ObservationDTO.ReferenceRangeHigh)}]            = relatedTo.ReferenceRangeHigh
      				  ,[{nameof(ObservationDTO.ReferenceRangeType)}]            = relatedTo.ReferenceRangeType
      				  ,[{nameof(ObservationDTO.ReferenceRangeAppliesTo)}]       = relatedTo.ReferenceRangeAppliesTo
      				  ,[{nameof(ObservationDTO.ReferenceRangeAgeHigh)}]         = relatedTo.ReferenceRangeAgeHigh
      				  ,[{nameof(ObservationDTO.ReferenceRangeAgeLow)}]          = relatedTo.ReferenceRangeAgeLow
      				  ,[{nameof(ObservationDTO.EntityId)}]                  	= relatedTo.Entityid
 				  FROM [dbo].[Observation] observation
  				  LEFT JOIN [dbo].[Entity] basedOn ON basedOn.Id = observation.BasedOn
  				  LEFT JOIN [dbo].[Patient] patient ON patient.Id = observation.SubjectId
				  LEFT JOIN [dbo].[Entity] performer ON performer.Id = observation.PerformerId
  				  LEFT JOIN [dbo].[Observation] relatedTo ON relatedTo.Id = observation.RelatedTo
				  WHERE observation.OriginalId = @OriginalId";
        
            var reader = await _connection.QueryMultipleAsync(getExisting, new
            {
                OriginalId = originalId
            }, transaction: transaction);
            var observations = reader.Read<ObservationDTO, OutboundRelationship?, PatientDTO?, OutboundRelationship?,ObservationDTO?, ObservationDTO >(
                (observation, basedOn, subject, performer, relatedTo ) =>
                {
	                if (basedOn is not null)
                    {
	                    observation.BasedOn = basedOn;
                    }
	                if (subject is not null)
	                {
		                observation.Subject = subject;
	                }
	                if (basedOn is not null)
	                {
		                observation.Performer = performer;
	                }
	                if (basedOn is not null)
	                {
		                observation.RelatedTo = relatedTo;
	                }
	                
	                return observation;
                }, splitOn: $"{nameof(OutboundRelationship.Id)},{nameof(PatientDTO.Id)},{nameof(PracticionerDTO.Id)},{nameof(ObservationDTO.Id)}");

            return observations.FirstOrDefault();
    }

    public async Task<Guid> InsertObservationAsync(ObservationDTO observation, CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    observation.Id = Guid.NewGuid();
	    observation.EntityId = Guid.NewGuid();
                    
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
            Id = observation.EntityId,
            Type = EntityTypes.Observation,
            OriginalId = observation.OriginalId
        }, cancellationToken: cancellationToken, transaction: transaction);
                    
        await _connection.ExecuteAsync(entityDefinition);

        const string insertObservation =
                @"INSERT INTO [dbo].[Observation]
           		([Id]
           		,[OriginalId]
           		,[Status]
           		,[Category]
           		,[Code]
           		,[BasedOn]
           		,[SubjectId]
           		,[ContextId]
           		,[EffectiveDate]
           		,[EffectiveDateFrom]
           		,[EffectiveDateTo]
           		,[Issued]
           		,[PerformerId]
           		,[Interpretation]
           		,[DataAbsentReason]
           		,[Comment]
           		,[BodySite]
           		,[Method]
           		,[Device]
           		,[ReferenceText]
           		,[ReferenceRangeLow]
           		,[ReferenceRangeLowUnit]
           		,[ReferenceRangeHigh]
           		,[ReferenceRangeHighUnit]
           		,[ReferenceRangeType]
           		,[ReferenceRangeAppliesTo]
           		,[ReferenceRangeAgeHigh]
           		,[ReferenceRangeAgeLow]
           		,[RelatedTo]
           		,[Entityid])
     		VALUES
           		(@Id
           		,@OriginalId
           		,@Status
           		,@Category
           		,@Code
           		,(select Id from dbo.Entity where OriginalId = @BasedOn)
                ,(select Id from dbo.Patient where OriginalId = @SubjectId)
           		,@ContextId
           		,@EffectiveDate
           		,@EffectiveDateFrom
           		,@EffectiveDateTo
           		,@Issued
           		,(select Id from dbo.Entity where OriginalId = @PerformerId)
           		,@Interpretation
           		,@DataAbsentReason
           		,@Comment
           		,@BodySite
           		,@Method
           		,@Device
     		    ,@ReferenceText
           		,@ReferenceRangeLow
     		    ,@ReferenceRangeLowUnit
           		,@ReferenceRangeHigh
     		  	,@ReferenceRangeHighUnit
           		,@ReferenceRangeType
           		,@ReferenceRangeAppliesTo
           		,@ReferenceRangeAgeHigh
           		,@ReferenceRangeAgeLow
           		,(select Id from dbo.Observation where OriginalId = @RelatedTo)
           		,@Entityid)";
            
            var commandDefinition = new CommandDefinition(insertObservation, new
            {
                Id = observation.Id,
                OriginalId = observation.OriginalId,
                Status = observation.Status,
                Category = observation.Category,
                Code = observation.Code?.Id,
                BasedOn = observation.BasedOn?.OriginalId,
                SubjectId = observation.Subject.OriginalId,
                ContextId = observation.Context?.OriginalId,
                EffectiveDate = observation.EffectiveDate,
                EffectiveDateFrom = observation.EffectiveDateFrom,
                EffectiveDateTo = observation.EffectiveDateTo,
                Issued = observation.Issued,
                PerformerId = observation.Performer?.OriginalId,
                Interpretation = observation.Interpretation,
                DataAbsentReason = observation.DataAbsentReason,
                Comment = observation.Comment,
                BodySite = observation.BodySite,
                Method = observation.Method,
                Device = observation.Device?.OriginalId,
                ReferenceText = observation.ReferenceText,
                ReferenceRangeLow = observation.ReferenceRangeLow,
                ReferenceRangeLowUnit = observation.ReferenceRangeLowUnit,
                ReferenceRangeHigh = observation.ReferenceRangeHigh,
                ReferenceRangeHighUnit = observation.ReferenceRangeHighUnit,
                ReferenceRangeType = observation.ReferenceRangeType,
                ReferenceRangeAppliesTo = observation.ReferenceRangeAppliesTo,
                ReferenceRangeAgeHigh = observation.ReferenceRangeAgeHigh,
                ReferenceRangeAgeLow = observation.ReferenceRangeAgeLow,
                RelatedTo = observation.RelatedTo?.OriginalId,
                Entityid = observation.EntityId
                
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return observation.Id;
    }

    public async Task UpdateObservationAsync(ObservationDTO observation, CancellationToken cancellationToken, IDbTransaction transaction)
    {
         const string updateOrganization =
                @"UPDATE [dbo].[Observation]
				  SET
                         [OriginalId] 				= @OriginalId
           				,[Status] 					= @Status
           				,[Category] 				= @Category
           				,[Code] 					= @Code
           				,[BasedOn]					= @BasedOn
           				,[SubjectId]				= @SubjectId
           				,[ContextId]				= @ContextId
           				,[EffectiveDate]			= @EffectiveDate
           				,[EffectiveDateFrom]		= @EffectiveDateFrom
           				,[EffectiveDateTo]			= @EffectiveDateTo
           				,[Issued]					= @Issued
           				,[PerformerId]				= @PerformerId
           				,[Interpretation]			= @Interpretation
           				,[DataAbsentReason]			= @DataAbsentReason
           				,[Comment]					= @Comment
           				,[BodySite]					= @BodySite
           				,[Method]					= @Method
           				,[Device]					= @Device
           				,[ReferenceRangeLow]		= @ReferenceRangeLow
           				,[ReferenceRangeHigh]		= @ReferenceRangeHigh
           				,[ReferenceRangeType]		= @ReferenceRangeType
           				,[ReferenceRangeAppliesTo]	= @ReferenceRangeAppliesTo
           				,[ReferenceRangeAgeHigh]	= @ReferenceRangeAgeHigh
           				,[ReferenceRangeAgeLow]		= @ReferenceRangeAgeLow
           				,[RelatedTo]				= @RelatedTo
           				,[Entityid]					= @Entityid
                 WHERE Id = @Id";
            
            var commandDefinition = new CommandDefinition(updateOrganization, new
            {
	            Id = observation.Id,
	            OriginalId = observation.OriginalId,
	            Status = observation.Status,
	            Category = observation.Category,
	            Code = observation.Code,
	            BasedOn = observation.BasedOn?.OriginalId,
	            SubjectId = observation.Subject.OriginalId,
	            ContextId = observation.Context?.OriginalId,
	            EffectiveDate = observation.EffectiveDate,
	            EffectiveDateFrom = observation.EffectiveDateFrom,
	            EffectiveDateTo = observation.EffectiveDateTo,
	            Issued = observation.Issued,
	            PerformerId = observation.Performer?.OriginalId,
	            Interpretation = observation.Interpretation,
	            DataAbsentReason = observation.DataAbsentReason,
	            Comment = observation.Comment,
	            BodySite = observation.BodySite,
	            Method = observation.Method,
	            Device = observation.Device?.OriginalId,
	            ReferenceRangeLow = observation.ReferenceRangeLow,
	            ReferenceRangeHigh = observation.ReferenceRangeHigh,
	            ReferenceRangeType = observation.ReferenceRangeType,
	            ReferenceRangeAppliesTo = observation.ReferenceRangeAppliesTo,
	            ReferenceRangeAgeHigh = observation.ReferenceRangeAgeHigh,
	            ReferenceRangeAgeLow = observation.ReferenceRangeAgeLow,
	            RelatedTo = observation.RelatedTo?.OriginalId,
	            Entityid = observation.EntityId

            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await _connection.ExecuteAsync(commandDefinition);
            if (result == 0)
            {
                throw new DataException("Error: User request was not successful.");
            }
    }

}