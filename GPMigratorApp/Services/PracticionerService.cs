using System.Data;
using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GPMigratorApp.Services;

public class PracticionerService: IPracticionerService
{
    
    public PracticionerService()
    {
        
    }
    
    public async Task PutPracticioners(IEnumerable<PracticionerDTO> practicioners,IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    { 
        var practicionerCommand = new PracticionerCommand(connection);
        foreach (var practicioner in practicioners)
        {
            var existingRecord = await practicionerCommand.GetPracticionerAsync(practicioner.OriginalId, cancellationToken, transaction);
            if (existingRecord is null)
            {
                await practicionerCommand.InsertPracticionerAsync(practicioner, cancellationToken,transaction);
            }
            else
            {
                if (practicioner.Address != null && existingRecord.Address != null)
                {
                    practicioner.Address.Id = existingRecord.Address.Id;
                }

                existingRecord.OriginalId = practicioner.OriginalId;
                existingRecord.SdsUserId = practicioner.SdsUserId;
                existingRecord.SdsRoleProfileId = practicioner.SdsRoleProfileId;
                existingRecord.Title = practicioner.Title;
                existingRecord.GivenName = practicioner.GivenName;
                existingRecord.MiddleNames = practicioner.MiddleNames;
                existingRecord.Surname = practicioner.Surname;
                existingRecord.Sex = practicioner.Sex;
                existingRecord.Address = practicioner.Address;

                
                await practicionerCommand.UpdatePracticionerAsync(existingRecord, cancellationToken, transaction);
            }
        }
    }
}