using System.Data;
using GPMigratorApp.Data;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;

namespace GPMigratorApp.Services;

public class PatientService : IPatientService
{
    public async Task PutPatients(IEnumerable<PatientDTO> patients, IDbConnection connection,
        IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var patientCommand = new PatientCommand(connection);
        foreach (var patient in patients)
        {
            var existingRecord =
                await patientCommand.GetPatientAsync(patient.OriginalId, cancellationToken, transaction);
            if (existingRecord is null)
            {
                await patientCommand.InsertPatientAsync(patient, cancellationToken, transaction);
            }
            else
            {
                if (patient.HomeAddress != null && existingRecord.HomeAddress != null)
                {
                    patient.HomeAddress.Id = existingRecord.HomeAddress.Id;
                }
                if (patient.ManagingOrganization != null && existingRecord.ManagingOrganization != null)
                {
                    patient.ManagingOrganization.Id = existingRecord.ManagingOrganization.Id;
                }
                if (patient.UsualGP != null && existingRecord.UsualGP != null)
                {
                    patient.UsualGP.Id = existingRecord.UsualGP.Id;
                }
                
                existingRecord.OriginalId = patient.OriginalId;
                existingRecord.Active = patient.Active;
                existingRecord.UsualGP = patient.UsualGP;
                existingRecord.Gender = patient.Gender;
                existingRecord.DateOfBirthUTC = patient.DateOfBirthUTC;
                existingRecord.DateOfDeathUTC = patient.DateOfDeathUTC;
                existingRecord.Title = patient.Title;
                existingRecord.GivenName = patient.GivenName;
                existingRecord.MiddleNames = patient.MiddleNames;
                existingRecord.Surname = patient.Surname;
                existingRecord.DateOfRegistrationUTC = patient.DateOfRegistrationUTC;
                existingRecord.NhsNumber = patient.NhsNumber;
                existingRecord.PatientTypeDescription = patient.PatientTypeDescription;
                existingRecord.DummyType = patient.DummyType;
                existingRecord.ResidentialInstituteCode = patient.ResidentialInstituteCode;
                existingRecord.NHSNumberStatus = patient.NHSNumberStatus;
                existingRecord.CarerName = patient.CarerName;
                existingRecord.CarerRelation = patient.CarerRelation;
                existingRecord.PersonGuid = patient.PersonGuid;
                existingRecord.DateOfDeactivation = patient.DateOfDeactivation;
                existingRecord.Deleted = patient.Deleted;
                existingRecord.SpineSensitive = patient.SpineSensitive;
                existingRecord.IsConfidential = patient.IsConfidential;
                existingRecord.EmailAddress = patient.EmailAddress;
                existingRecord.HomePhone = patient.HomePhone;
                existingRecord.MobilePhone = patient.MobilePhone;
                existingRecord.ProcessingId = patient.ProcessingId;
                existingRecord.Ethnicity = patient.Ethnicity;
                existingRecord.Religion = patient.Religion;
                existingRecord.HomeAddress = patient.HomeAddress;
                existingRecord.EntityId = patient.EntityId;
                
                await patientCommand.UpdatePatientAsync(existingRecord, cancellationToken, transaction);
            }
        }
    }
}