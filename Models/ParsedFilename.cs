namespace Lumière.Models;

public class ParsedFilename
{
    public string Title { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string? Quality { get; set; }
    public string? Source { get; set; }
    public string OriginalFilename { get; set; } = string.Empty;
}
