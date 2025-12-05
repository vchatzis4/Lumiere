using System.Text.Json.Serialization;

namespace MyNetflixClone.Services;

public class TMDBCredits
{
    [JsonPropertyName("cast")]
    public List<TMDBCast>? Cast { get; set; }

    [JsonPropertyName("crew")]
    public List<TMDBCrew>? Crew { get; set; }
}
