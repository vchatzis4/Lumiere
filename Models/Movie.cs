namespace MyNetflixClone.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? PosterPath { get; set; }
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
    public double? Rating { get; set; }
    public int? Duration { get; set; } // in minutes
    public int LastWatchedPosition { get; set; } // in seconds
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public string? SubtitlePath { get; set; }
    public string? BackdropPath { get; set; }
    public string? Director { get; set; }
    public string? Cast { get; set; }

    public static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');
        return slug;
    }
}
