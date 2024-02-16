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
public class GPConnectLocation : Location
{
    private readonly IEnumerable<OrganizationDTO> _organizations;
    public GPConnectLocation(Location organization, IEnumerable<OrganizationDTO> organizations)
    {
        InitInhertedProperties(organization);
        _organizations = organizations;
    }

    public LocationDTO GetDTO()
    {
        var dto = new LocationDTO()
        {
            OriginalId = this.Id,
            ODSSiteCode = ODSCode(),
            Status = this.Status.Value.ToString(),
            OperationalStatus = this.OperationalStatus.Code,
            Alias = this.Alias.FirstOrDefault(),
            Description = this.Description,
            Name = this.Name,
            Telecom = this.Telecom.FirstOrDefault()?.ValueElement.Value,
            Type = this.TypeName,
            PhysicalType = this.PhysicalType.Text,
            
        };
        if (this.Address.Any())
            dto.Address = new AddressDTO(this.Address);
        if(this.ManagingOrganization is not null)
            dto.ManagingOrganization = _organizations.FirstOrDefault(x=> x.OriginalId == ReferenceHelper.GetId(this.ManagingOrganization.Reference));
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
            var props = typeof(Organization).GetProperties().Where(p => !p.GetIndexParameters().Any());
            foreach (var prop in props)
            {
                if (prop.CanWrite)
                    prop.SetValue(this, prop.GetValue(encounter));
            }
        }
    }
}