namespace GPMigratorApp.DTOs;

public class OutboundRelationship
{
    public Guid? Id { get; set; }
    public int? Type { get; set; }
    public string? OriginalId { get; set; }
}