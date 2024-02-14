namespace GPMigratorApp.Data.IntermediaryModels;

public class OrganizationLocation
{
    public Guid? OrganizationId { get; set; }
    public Guid? OrganizationEntityId { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? LocationEntityId { get; set; }
}