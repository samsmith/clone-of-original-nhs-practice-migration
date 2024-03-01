using System.Data;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;

namespace GPMigratorApp.Services;

public class CodingService: ICodingService
{
    public CodingService()
    {
        
    }
    
    public async Task<Guid> PutCoding(CodeDTO code, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var practicionerCommand = new CodingCommand(connection);
        
            var existingRecord =
                await practicionerCommand.GetCodingAsync(code.SnomedCode, code.ReadCode, cancellationToken, transaction);
            if (existingRecord is null)
            {
                return await practicionerCommand.InsertCodeAsync(code.SnomedCode, code.ReadCode, code.Description, cancellationToken, transaction);
            }
            
            return existingRecord.Id;
        }
}