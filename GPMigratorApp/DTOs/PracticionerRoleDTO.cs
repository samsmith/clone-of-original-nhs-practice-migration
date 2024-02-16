namespace GPMigratorApp.DTOs;

public class PracticionerRoleDTO : OutboundRelationship
{
    public PracticionerRoleDTO()
    {
    }
    public IdentifierDTO? Identifier { get; set; }
    
    public bool? Active { get; set; }
    
    public DateTime? PeriodStart { get; set; }
    
    public DateTime? PeriodEnd { get; set; }
    
    public PracticionerDTO? Practicioner { get; set; }
    
    public OrganizationDTO? Organization { get; set; }
    
    public string? SDSJobRoleName { get; set; }
    
    public string? Speciality { get; set; }
    
    public LocationDTO? Location { get; set; }

    public string? Telecom { get; set; }
}