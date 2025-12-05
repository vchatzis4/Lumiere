using System.Text.Json.Serialization;

namespace Lumière.Services;

public class TMDBSearchResult
{
    [JsonPropertyName("results")]
    public List<TMDBMovie>? Results { get; set; }
}
