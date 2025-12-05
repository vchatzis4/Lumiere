using System.Text.Json.Serialization;

namespace MyNetflixClone.Services;

public class TMDBMovieDetails
{
    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }

    [JsonPropertyName("genres")]
    public List<TMDBGenre>? Genres { get; set; }

    [JsonPropertyName("credits")]
    public TMDBCredits? Credits { get; set; }
}
