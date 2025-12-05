namespace MyNetflixClone.Services;

public class TMDBService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly System.Text.Json.JsonSerializerOptions _jsonOptions;
    private const string BaseUrl = "https://api.themoviedb.org/3";
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";

    public TMDBService(IConfiguration configuration)
    {
        _apiKey = configuration["TMDB:ApiKey"] ?? string.Empty;
        _httpClient = new HttpClient();
        _jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<MovieMetadata?> SearchMovieAsync(string title)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return null;

        try
        {
            var encodedTitle = Uri.EscapeDataString(title);
            var url = $"{BaseUrl}/search/movie?api_key={_apiKey}&query={encodedTitle}";

            var response = await _httpClient.GetStringAsync(url);
            var searchResult = System.Text.Json.JsonSerializer.Deserialize<TMDBSearchResult>(response, _jsonOptions);

            if (searchResult?.Results?.Count > 0)
            {
                var movie = searchResult.Results[0];

                // Get additional movie details
                var detailsUrl = $"{BaseUrl}/movie/{movie.Id}?api_key={_apiKey}&append_to_response=credits";
                var detailsResponse = await _httpClient.GetStringAsync(detailsUrl);
                var details = System.Text.Json.JsonSerializer.Deserialize<TMDBMovieDetails>(detailsResponse, _jsonOptions);

                return new MovieMetadata
                {
                    Title = movie.Title ?? title,
                    Description = movie.Overview,
                    Year = movie.ReleaseDate?.Year,
                    Rating = movie.VoteAverage,
                    PosterUrl = !string.IsNullOrEmpty(movie.PosterPath)
                        ? $"{ImageBaseUrl}{movie.PosterPath}"
                        : null,
                    BackdropUrl = !string.IsNullOrEmpty(movie.BackdropPath)
                        ? $"https://image.tmdb.org/t/p/original{movie.BackdropPath}"
                        : null,
                    Genre = details?.Genres != null ? string.Join(", ", details.Genres.Select(g => g.Name)) : null,
                    Duration = details?.Runtime,
                    Director = details?.Credits?.Crew?.FirstOrDefault(c => c.Job == "Director")?.Name,
                    Cast = details?.Credits?.Cast != null ? string.Join(", ", details.Credits.Cast.Take(5).Select(c => c.Name)) : null
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching movie metadata: {ex.Message}");
        }

        return null;
    }

    public async Task<byte[]?> DownloadPosterAsync(string posterUrl)
    {
        try
        {
            return await _httpClient.GetByteArrayAsync(posterUrl);
        }
        catch
        {
            return null;
        }
    }
}
