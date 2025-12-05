using System.Text.Json.Serialization;

namespace Lumière.Services;

public class TMDBGenre
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
