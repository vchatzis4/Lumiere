using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNetflixClone.Models;
using MyNetflixClone.Services;

namespace MyNetflixClone.Pages.Admin;

public class AdminIndexModel : PageModel
{
    private readonly MovieService _movieService;
    private readonly IConfiguration _configuration;

    public AdminIndexModel(MovieService movieService, IConfiguration configuration)
    {
        _movieService = movieService;
        _configuration = configuration;
    }

    public List<Movie> Movies { get; set; } = new();
    public string? SearchTerm { get; set; }
    public List<Movie>? ScanResult { get; set; }
    public string MediaPath { get; set; } = string.Empty;
    public string DbPath { get; set; } = string.Empty;
    public string? TmdbApiKey { get; set; }

    public async Task OnGetAsync(string? search)
    {
        SearchTerm = search;

        if (!string.IsNullOrEmpty(search))
        {
            Movies = await _movieService.SearchMoviesAsync(search);
        }
        else
        {
            Movies = await _movieService.GetAllMoviesAsync();
        }

        MediaPath = _configuration["MediaSettings:MoviesPath"] ?? "Media/Movies";
        DbPath = Path.Combine(Directory.GetCurrentDirectory(), "movies.db");
        TmdbApiKey = _configuration["TMDB:ApiKey"];
    }

    public async Task<IActionResult> OnPostScanFolderAsync()
    {
        ScanResult = await _movieService.ScanFolderForMoviesAsync();
        Movies = await _movieService.GetAllMoviesAsync();

        MediaPath = _configuration["MediaSettings:MoviesPath"] ?? "Media/Movies";
        DbPath = Path.Combine(Directory.GetCurrentDirectory(), "movies.db");
        TmdbApiKey = _configuration["TMDB:ApiKey"];

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _movieService.DeleteMovieAsync(id);
        return RedirectToPage();
    }
}
