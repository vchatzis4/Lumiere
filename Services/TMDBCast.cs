using System.Text.Json.Serialization;

namespace Lumière.Services;

public class TMDBCast
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
