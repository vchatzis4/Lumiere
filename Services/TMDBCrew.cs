using System.Text.Json.Serialization;

namespace Lumière.Services;

public class TMDBCrew
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("job")]
    public string Job { get; set; } = string.Empty;
}
