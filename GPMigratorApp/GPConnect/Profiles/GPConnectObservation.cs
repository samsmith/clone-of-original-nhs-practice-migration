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
    private readonly IEnumerable<PatientDTO> _patients;
    private readonly IEnumerable<PracticionerDTO> _practicioners;
    public GPConnectObservation(Observation observation, IEnumerable<PatientDTO> patients, IEnumerable<PracticionerDTO> practicioners )
    {
        InitInhertedProperties(observation);
        _patients = patients;
        _practicioners = practicioners;
    }

    public ObservationDTO GetDTO()
    {
        var dto = new ObservationDTO()
        {
            OriginalId = this.Id,

            Identifier = new IdentifierDTO(this.Identifier.FirstOrDefault()),

            BasedOn = new OutboundRelationship
                { OriginalId = this.BasedOn?.FirstOrDefault()?.Reference, Type = this.BasedOn?.FirstOrDefault()?.Type },
            Status = this.Status.Value.ToString(),
            Category = this.Category.FirstOrDefault()?.Coding?.FirstOrDefault()?.Code,
            Code = this.Code.Coding?.FirstOrDefault()?.Code,
            Subject = _patients.FirstOrDefault(x => x.OriginalId == this.Subject.Reference),
            Context = null,
            EffectiveDate = (DateTime)this.Effective.FirstOrDefault(x => x.Key == "").Value,
            EffectivePeriod = null,
            Performer = new OutboundRelationship
            {
                OriginalId = this.Performer?.FirstOrDefault()?.Reference, Type = this.Performer?.FirstOrDefault()?.Type
            },
        };
        return dto;
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