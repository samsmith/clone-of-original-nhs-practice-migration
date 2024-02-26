using System.Reflection;
using System.Runtime.Serialization;
using DotNetGPSystem;
using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.Data.Types;
using GPMigratorApp.DTOs;
using GPMigratorApp.GPConnect.Helpers;
using Hl7.Fhir.Introspection;
using Hl7.Fhir.Model;

namespace GPMigratorApp.GPConnect.Profiles;

[Serializable]
[DataContract]
[FhirType("Organization","http://hl7.org/fhir/StructureDefinition/Organization", IsResource=true)]
public class GPConnectObservation : Observation
{
    public GPConnectObservation(Observation observation, IEnumerable<PatientDTO> patients, IEnumerable<PracticionerDTO> practicioners, )
    {
        InitInhertedProperties(observation);
    }

    public OrganizationDTO GetDTO()
    {
        var dto = new ObservationDTO()
        {
            OriginalId = this.Id,
    
    Identifier  = this.Identifier.FirstOrDefault(),
        
    BasedOn = new OutboundRelationship{OriginalId = this.BasedOn?.FirstOrDefault()?.Reference, Type = this.BasedOn?.FirstOrDefault()?.Type},
    Status = this.Status.Value.ToString(),
    Category = this.Category.FirstOrDefault()?.Coding?.FirstOrDefault()?.Code,
    Code = this.Code.Coding?.FirstOrDefault()?.Code,
    Subject = this.Subject.Reference
    Context
    EffectiveDate
    EffectivePeriod
    Performer
        return dto;
        }
       
        code.Coding
    }
    
    
    public string? ODSCode()
    {
        return this.Identifier.FirstOrDefault()?.Value;
        
    }
    
    private void InitInhertedProperties (object encounter)
    {
        foreach (var propertyInfo in encounter.GetType().GetProperties())
        {
            var props = typeof(Observation).GetProperties().Where(p => !p.GetIndexParameters().Any());
            foreach (var prop in props)
            {
                if (prop.CanWrite)
                    prop.SetValue(this, prop.GetValue(encounter));
            }
        }
    }
}