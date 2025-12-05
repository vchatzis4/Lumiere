using System.Text.Json.Serialization;

namespace MyNetflixClone.Services;

public class TMDBGenre
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
