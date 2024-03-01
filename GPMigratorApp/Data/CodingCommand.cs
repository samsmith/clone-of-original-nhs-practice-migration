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

public class CodingCommand : ICodingCommand
{
    private readonly IDbConnection _connection;
    
    public CodingCommand(IDbConnection connection)
    {
        _connection = connection;
    }


    public async Task<CodeDTO?> GetCodingAsync(string? snomedCode, string? readCode,CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    string getExisting =
		    @$"SELECT [{nameof(CodeDTO.Id)}]								= code.Id
                      ,[{nameof(CodeDTO.ReadCode)}]                  	    = code.ReadCode
                      ,[{nameof(CodeDTO.SnomedCode)}]                  		= code.SnomedCode
                      ,[{nameof(CodeDTO.LocalCode)}]                  		= code.LocalCode
                      ,[{nameof(CodeDTO.NationalCode)}]                		= code.NationalCode
                      ,[{nameof(CodeDTO.Description)}]                      = code.Description

                  FROM [dbo].[Coding] code
				  WHERE code.ReadCode = @ReadCode 
				  AND code.SnomedCode = @SnomedCode";
        
            var reader = await _connection.QueryMultipleAsync(getExisting, new
            {
	            ReadCode = readCode,
	            SnomedCode = snomedCode
            }, transaction: transaction);
            var codes = reader.Read<CodeDTO>();
            return codes.FirstOrDefault();
    }

    public async Task<Guid> InsertCodeAsync(string? snomedCode, string? readCode, string? description,
	    CancellationToken cancellationToken, IDbTransaction transaction)
    {
	    var code = new CodeDTO
		    {Id = Guid.NewGuid(), ReadCode = readCode, SnomedCode = snomedCode, Description = description};

	    var codeCommand = @"INSERT INTO [dbo].[Coding]
                        	([Id],
                        	[ReadCode],
                        	[SnomedCode],
                        	[Description])
                    		VALUES
                        	(@Id,
                        	@ReadCode,
	            			@SnomedCode,
                        	@Description)";

	    var codeDefinition = new CommandDefinition(codeCommand, new
	    {
		    Id = code.Id,
		    ReadCode = code.ReadCode,
		    SnomedCode = code.SnomedCode,
		    Description = code.Description
	    }, cancellationToken: cancellationToken, transaction: transaction);

	    await _connection.ExecuteAsync(codeDefinition);

	    return code.Id;
    }

}