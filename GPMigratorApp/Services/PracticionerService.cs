using System.Data;
using GPMigratorApp.Data;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;

namespace GPMigratorApp.Services;

public class PracticionerService: IPracticionerService
{
    
    public PracticionerService()
    {
        
    }
    
    public async Task PutPracticioners(IEnumerable<PracticionerDTO> practicioners, IEnumerable<PracticionerRoleDTO> practicionerRoles, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    { 
        var practicionerCommand = new PracticionerCommand(connection);
        foreach (var practicioner in practicioners)
        {
            var existingRecord =
                await practicionerCommand.GetPracticionerAsync(practicioner.OriginalId, cancellationToken, transaction);
            if (existingRecord is null)
            {
                await practicionerCommand.InsertPracticionerAsync(practicioner, cancellationToken, transaction);
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

        await PutPracticionerRoles(practicionerRoles, connection, transaction, cancellationToken);
    }
    
    private async Task PutPracticionerRoles(IEnumerable<PracticionerRoleDTO> practicionerRoles, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var practicionerRoleCommand = new PracticionerRoleCommand(connection);
        foreach (var practicionerRole in practicionerRoles)
        {
            var existingRoleRecord =
                await practicionerRoleCommand.GetPracticionerRoleAsync(practicionerRole.OriginalId, cancellationToken,
                    transaction);
            if (existingRoleRecord is null)
            {
                await practicionerRoleCommand.InsertPracticionerRoleAsync(practicionerRole, cancellationToken,
                    transaction);
            }
            else
            {
                if (practicionerRole.Practicioner != null && existingRoleRecord.Practicioner != null)
                {
                    practicionerRole.Practicioner.Id = existingRoleRecord.Practicioner.Id;
                }

                if (practicionerRole.Organization != null && existingRoleRecord.Organization != null)
                {
                    practicionerRole.Organization.Id = existingRoleRecord.Organization.Id;
                }

                if (practicionerRole.Location != null && existingRoleRecord.Location != null)
                {
                    practicionerRole.Location.Id = existingRoleRecord.Location.Id;
                }

                existingRoleRecord.OriginalId = practicionerRole.OriginalId;
                existingRoleRecord.Active = practicionerRole.Active;
                existingRoleRecord.PeriodStart = practicionerRole.PeriodStart;
                existingRoleRecord.PeriodEnd = practicionerRole.PeriodEnd;
                existingRoleRecord.Practicioner = practicionerRole.Practicioner;
                existingRoleRecord.Organization = practicionerRole.Organization;
                existingRoleRecord.SDSJobRoleName = practicionerRole.SDSJobRoleName;
                existingRoleRecord.Speciality = practicionerRole.Speciality;
                existingRoleRecord.Location = practicionerRole.Location;


                await practicionerRoleCommand.UpdatePracticionerRoleAsync(existingRoleRecord, cancellationToken,
                    transaction);
            }
        }
    }
}