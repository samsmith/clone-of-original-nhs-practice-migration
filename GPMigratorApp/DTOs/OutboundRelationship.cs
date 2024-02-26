namespace GPMigratorApp.DTOs;

public class OutboundRelationship
{
    public Guid? Id { get; set; }
    public string? Type { get; set; }
    public string? OriginalId { get; set; }
}