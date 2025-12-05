using System.Text.Json.Serialization;

namespace Lumière.Models;

public class TMDBSearchResult
{
    [JsonPropertyName("results")]
    public List<TMDBMovie>? Results { get; set; }
}

public class TMDBMovie
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    [JsonPropertyName("release_date")]
    public DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("vote_average")]
    public double? VoteAverage { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }
}

public class TMDBMovieDetails
{
    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }

    [JsonPropertyName("genres")]
    public List<TMDBGenre>? Genres { get; set; }

    [JsonPropertyName("credits")]
    public TMDBCredits? Credits { get; set; }
}

public class TMDBCredits
{
    [JsonPropertyName("cast")]
    public List<TMDBCast>? Cast { get; set; }

    [JsonPropertyName("crew")]
    public List<TMDBCrew>? Crew { get; set; }
}

public class TMDBCast
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class TMDBCrew
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("job")]
    public string Job { get; set; } = string.Empty;
}

public class TMDBGenre
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
