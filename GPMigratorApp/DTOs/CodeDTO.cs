namespace GPMigratorApp.DTOs;

public class CodeDTO
{
    public Guid Id { get; set; }
    public string? ReadCode { get; set; }
    public string? SnomedCode { get; set; }
    public string? LocalCode { get; set; }
    public string? NationalCode { get; set; }
    public string? Description  { get; set; }
    
}