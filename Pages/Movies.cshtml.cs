using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lumière.Models;
using Lumière.Services;

namespace Lumière.Pages;

public class MoviesModel : PageModel
{
    private readonly MovieService _movieService;

    public MoviesModel(MovieService movieService)
    {
        _movieService = movieService;
    }

    public List<Movie> Movies { get; set; } = new();
    public List<string> Genres { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SelectedGenre { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "recent";

    public async Task OnGetAsync()
    {
        Genres = await _movieService.GetAllGenresAsync();

        if (!string.IsNullOrEmpty(SelectedGenre))
        {
            Movies = await _movieService.GetMoviesByGenreAsync(SelectedGenre);
        }
        else
        {
            Movies = await _movieService.GetAllMoviesAsync();
        }

        // Apply sorting
        Movies = SortBy switch
        {
            "title" => Movies.OrderBy(m => m.Title).ToList(),
            "year" => Movies.OrderByDescending(m => m.Year ?? 0).ToList(),
            "rating" => Movies.OrderByDescending(m => m.Rating ?? 0).ToList(),
            _ => Movies // recent is default (already sorted by DateAdded desc)
        };
    }
}
