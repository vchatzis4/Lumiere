namespace Lumière.Services;

public class MovieMetadata
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Year { get; set; }
    public double? Rating { get; set; }
    public string? PosterUrl { get; set; }
    public string? BackdropUrl { get; set; }
    public string? Genre { get; set; }
    public int? Duration { get; set; }
    public string? Director { get; set; }
    public string? Cast { get; set; }
}
