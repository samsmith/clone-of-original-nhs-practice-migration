namespace GPMigratorApp.DTOs;

public class ObservationDTO
{
    public Guid Id { get; set; }
    public string OriginalId { get; set; }
    public IdentifierDTO? Identifier { get; set; }
    public OutboundRelationship? BasedOn { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
    public CodeDTO? Code { get; set; }
    public PatientDTO Subject { get; set; }
    public EncounterDTO? Context { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? EffectiveDateFrom { get; set; }
    public DateTime? EffectiveDateTo { get; set; }
    public PeriodDTO? EffectivePeriod { get; set; }
    public DateTime? Issued { get; set; }
    public OutboundRelationship? Performer { get; set; }
    public string? Interpretation { get; set; }
    public string? DataAbsentReason { get; set; }
    public string? Comment { get; set; }
    public string? BodySite { get; set; }
    public string? Method { get; set; }
    public DeviceDTO? Device { get; set; }
    public string? ReferenceText { get; set; } 
    public decimal? ReferenceRangeLow { get; set; } 
    public string? ReferenceRangeLowUnit { get; set; }
    public decimal? ReferenceRangeHigh { get; set; } 
    public string? ReferenceRangeHighUnit { get; set; }
    public string? ReferenceRangeType { get; set; } 
    public string? ReferenceRangeAppliesTo { get; set; } 
    public decimal? ReferenceRangeAgeHigh { get; set; } 
    public decimal? ReferenceRangeAgeLow { get; set; } 
    public ObservationDTO? RelatedTo { get; set; } 
    public Guid EntityId { get; set; }
}