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
    public GPConnectObservation(Observation observation, IEnumerable<PatientDTO> patients )
    {
        InitInhertedProperties(observation);
        _patients = patients;
    }

    public ObservationDTO GetDTO()
    {
        var dto = new ObservationDTO()
        {
            OriginalId = this.Id,

            Identifier = new IdentifierDTO(this.Identifier.FirstOrDefault()),
            
            Status = this.Status?.ToString(),
            Category = this.Category?.FirstOrDefault()?.Coding?.FirstOrDefault()?.Code,
            
            Subject = _patients.FirstOrDefault(x => x.OriginalId == ReferenceHelper.GetId(this.Subject.Reference)),
            Context = null,
            EffectivePeriod = null,
            Interpretation = this.Interpretation?.Coding?.FirstOrDefault()?.Code,
            DataAbsentReason = this.DataAbsentReason?.Coding?.FirstOrDefault()?.Code,
            Comment = this.Comment,
            BodySite = this.BodySite?.Coding?.FirstOrDefault()?.Code,
            Method = this.Method?.Coding?.FirstOrDefault()?.Code,
            Device = null,
           
        };
        if (this.Code is not null)
        {
            dto.Code = new CodeDTO
            {
                ReadCode = this.Code.Coding?.FirstOrDefault(x => x.System == "http://read.info/readv2")?.Code,
                SnomedCode = this.Code.Coding?.FirstOrDefault(x => x.System == "http://snomed.info/sct")?.Code,
                Description = this.Code.Coding?.FirstOrDefault(x=> x.System == "http://snomed.info/sct")?.Display
            };
        }

        if (this.Related?.FirstOrDefault()?.Target?.Reference is not null)
        {
            //TO DO - MAKE THIS WORK FOR QUESTIONNAIRE RESPONSE AS WELL
            
            var type = ReferenceHelper.GetType(this.Related.FirstOrDefault().Target.Reference);
            if(type == "Observation")
                dto.RelatedTo = new ObservationDTO
                {OriginalId = ReferenceHelper.GetId(this.Related.FirstOrDefault().Target.Reference)};
        }

        var effectiveDate = this.Effective?.FirstOrDefault().Value;
        if (effectiveDate is not null)
        {
            dto.EffectiveDate = DateTime.Parse(effectiveDate.ToString());
        }

        if (this.Performer is not null)
        {
            var type = ReferenceHelper.GetType(this.Performer?.FirstOrDefault()?.Reference);
            if (type == "Practitioner")
            {
                dto.Performer = new OutboundRelationship
                {

                    OriginalId = this.Performer?.FirstOrDefault()?.Reference,
                    Type = (int) EntityTypes.Practitioner
                };
            }
            else
            {
                dto.Performer = null;
            }
        }

        if (this.BasedOn?.FirstOrDefault()?.Type == "Practitioner")
        {
            dto.BasedOn = new OutboundRelationship
            {
                OriginalId = this.BasedOn?.FirstOrDefault()?.Reference, 
                Type = (int) EntityTypes.Practitioner
            };
        }
        else
        {
            dto.BasedOn = null;
        }
        
        if(this.Related?.FirstOrDefault()?.Target?.Reference is not null)
            dto.RelatedTo = new ObservationDTO { OriginalId = this.Related.FirstOrDefault().Target.Reference};
        var reference = this.ReferenceRange.FirstOrDefault();
        if (reference is not null)
        {
            dto.ReferenceText = reference.Text;
            dto.ReferenceRangeLow = reference.Low?.Value;
            dto.ReferenceRangeLowUnit = reference.Low?.Unit;
            dto.ReferenceRangeHigh = reference.High?.Value;
            dto.ReferenceRangeHighUnit = reference.High?.Unit;
            dto.ReferenceRangeType = reference.Type?.Coding?.FirstOrDefault()?.Code;
            dto.ReferenceRangeAppliesTo = reference.AppliesTo?.FirstOrDefault()?.Coding?.FirstOrDefault()?.Code;
            dto.ReferenceRangeAgeHigh = reference.Age?.High?.Value;
            dto.ReferenceRangeAgeLow = reference.Age?.Low?.Value;
        }

        return dto;
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