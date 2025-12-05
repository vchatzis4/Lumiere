using System.Text.Json.Serialization;

namespace Lumière.Services;

public class TMDBCredits
{
    [JsonPropertyName("cast")]
    public List<TMDBCast>? Cast { get; set; }

    [JsonPropertyName("crew")]
    public List<TMDBCrew>? Crew { get; set; }
}
