using System.Reflection;
using System.Runtime.Serialization;
using DotNetGPSystem;
using GPConnect.Provider.AcceptanceTests.Http;
using GPMigratorApp.DTOs;
using GPMigratorApp.GPConnect.Helpers;
using Hl7.Fhir.Introspection;
using Hl7.Fhir.Model;

namespace GPMigratorApp.GPConnect.Profiles;

[Serializable]
[DataContract]
[FhirType("Organization","http://hl7.org/fhir/StructureDefinition/Organization", IsResource=true)]
public class GPConnectPracticionerRole : PractitionerRole
{

    private IEnumerable<OrganizationDTO> _organisations;
    private IEnumerable<PracticionerDTO> _practicioners;
    private IEnumerable<LocationDTO> _locations;
    
    public GPConnectPracticionerRole(PractitionerRole practitioner, IEnumerable<OrganizationDTO> organisations, IEnumerable<PracticionerDTO> practicioners, IEnumerable<LocationDTO> locations)
    {
        InitInhertedProperties(practitioner);
        _organisations = organisations;
        _practicioners = practicioners;
        _locations = locations;
    }

    public PracticionerRoleDTO GetDTO()
    {

        var dto = new PracticionerRoleDTO
        {
            Identifier = new IdentifierDTO((this.Identifier.FirstOrDefault())),
            Active = this.Active,
            PeriodStart = DateTime.Parse(this.Period.Start),
            PeriodEnd = DateTime.Parse(this.Period.End),
            Practicioner = _practicioners.FirstOrDefault(x => x.OriginalId == ReferenceHelper.GetId(this.Practitioner.Reference)),
            Organization = _organisations.FirstOrDefault(x => x.OriginalId == ReferenceHelper.GetId(this.Organization.Reference)),
            SDSJobRoleName = this.Code.FirstOrDefault(x => x.ElementId == "PractitionerRole.code:sdsJobRoleName")?.Text,
            Speciality = this.Specialty.FirstOrDefault()?.Text,
            Location = _locations.FirstOrDefault(x =>
            {
                var reference = this.Location.FirstOrDefault()?.Reference;
                return reference != null && x.OriginalId ==
                    ReferenceHelper.GetId(reference);
            }),
            Telecom = this.Telecom.FirstOrDefault()?.Value
        };
        return dto;
    }
    
    private void InitInhertedProperties (object encounter)
    {
        foreach (var propertyInfo in encounter.GetType().GetProperties())
        {
            var props = typeof(Practitioner).GetProperties().Where(p => !p.GetIndexParameters().Any());
            foreach (var prop in props)
            {
                if (prop.CanWrite)
                    prop.SetValue(this, prop.GetValue(encounter));
            }
        }
    }
}