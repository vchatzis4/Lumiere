using System.Text.Json.Serialization;

namespace MyNetflixClone.Services;

public class TMDBCast
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
