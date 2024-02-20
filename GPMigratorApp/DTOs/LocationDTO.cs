namespace GPMigratorApp.DTOs;

public class LocationDTO
{
    public Guid Id { get; set; }
    public string? OriginalId{ get; set; }
    public string? ODSSiteCode { get; set; }
    public string? Status { get; set; }
    public string? OperationalStatus { get; set; }
    public string? Name { get; set; }
    public string? Alias { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Telecom { get; set; }
    public AddressDTO? Address { get; set; }
    public string? PhysicalType { get; set; }
    public OrganizationDTO? ManagingOrganization { get; set; }
    public LocationDTO? PartOf { get; set; }
    public Guid? EntityId { get; set; }
}