using System.Data;
using Dapper;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data;

public class PatientCommand
{
    private readonly string _connectionString;
    
    public PatientCommand(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<Guid> CreatePatientAsync(PatientDTO patient, CancellationToken cancellationToken, SqlTransaction transaction)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            const string insertPatient =
                @"INSERT INTO [dbo].[Patient]
                   ([Id]
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
                   ,[PatientNumber]
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
                   ,[NominatedPharmacy])
             VALUES
                   (
                    @PatientId
                   )";

            await connection.OpenAsync(cancellationToken);
            
            var commandDefinition = new CommandDefinition(insertPatient, new
            {
                PatientId = patient.PatientId,
            }, cancellationToken: cancellationToken, transaction: transaction);
            
            var result = await connection.ExecuteScalarAsync<Guid?>(commandDefinition);
            if (result is null)
            {
                throw new DataException("Error: User request was not successful.");
            }

            return result.Value;
        }

}