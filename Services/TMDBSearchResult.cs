using System.Text.Json.Serialization;

namespace MyNetflixClone.Services;

public class TMDBSearchResult
{
    [JsonPropertyName("results")]
    public List<TMDBMovie>? Results { get; set; }
}
