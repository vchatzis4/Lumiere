using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNetflixClone.Models;
using MyNetflixClone.Services;

namespace MyNetflixClone.Pages;

public class IndexModel : PageModel
{
    private readonly MovieService _movieService;

    public IndexModel(MovieService movieService)
    {
        _movieService = movieService;
    }

    public Movie? FeaturedMovie { get; set; }
    public List<Movie> ContinueWatching { get; set; } = new();
    public List<Movie> RecentlyAdded { get; set; } = new();
    public Dictionary<string, List<Movie>> GenreMovies { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Get featured movie (most recent with backdrop)
        var allMovies = await _movieService.GetAllMoviesAsync();
        FeaturedMovie = allMovies.FirstOrDefault(m => !string.IsNullOrEmpty(m.BackdropPath))
                        ?? allMovies.FirstOrDefault();

        // Get continue watching
        ContinueWatching = await _movieService.GetContinueWatchingAsync(10);

        // Get recently added
        RecentlyAdded = await _movieService.GetRecentlyAddedAsync(10);

        // Get movies by genre
        var genres = await _movieService.GetAllGenresAsync();
        foreach (var genre in genres.Take(5)) // Limit to 5 genres for home page
        {
            var genreMovies = await _movieService.GetMoviesByGenreAsync(genre);
            if (genreMovies.Any())
            {
                GenreMovies[genre] = genreMovies.Take(10).ToList();
            }
        }
    }
}
