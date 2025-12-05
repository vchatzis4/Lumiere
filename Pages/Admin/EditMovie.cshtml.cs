using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lumière.Models;
using Lumière.Services;

namespace Lumière.Pages.Admin;

public class EditMovieModel : PageModel
{
    private readonly MovieService _movieService;
    private readonly TMDBService _tmdbService;

    public EditMovieModel(MovieService movieService, TMDBService tmdbService)
    {
        _movieService = movieService;
        _tmdbService = tmdbService;
    }

    [BindProperty]
    public Movie? Movie { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Movie = await _movieService.GetMovieByIdAsync(id);

        if (Movie == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Movie == null)
        {
            return Page();
        }

        await _movieService.UpdateMovieAsync(Movie);
        return RedirectToPage("/Admin/Index");
    }

    public async Task<IActionResult> OnPostRefreshMetadataAsync(int id)
    {
        Movie = await _movieService.GetMovieByIdAsync(id);

        if (Movie == null)
        {
            return NotFound();
        }

        // Fetch fresh metadata from TMDB
        var metadata = await _tmdbService.SearchMovieAsync(Movie.Title);
        if (metadata != null)
        {
            Movie.Title = metadata.Title;
            Movie.Description = metadata.Description;
            Movie.Year = metadata.Year;
            Movie.Genre = metadata.Genre;
            Movie.Rating = metadata.Rating;
            Movie.Duration = metadata.Duration;
            Movie.Director = metadata.Director;
            Movie.Cast = metadata.Cast;

            // Download poster if available
            if (!string.IsNullOrEmpty(metadata.PosterUrl))
            {
                var posterBytes = await _tmdbService.DownloadPosterAsync(metadata.PosterUrl);
                if (posterBytes != null)
                {
                    var postersPath = Path.Combine(Directory.GetCurrentDirectory(), "Media", "Posters");
                    var posterFileName = $"{Movie.Title.Replace(" ", "_")}_{DateTime.Now.Ticks}.jpg";
                    var posterPath = Path.Combine(postersPath, posterFileName);
                    await System.IO.File.WriteAllBytesAsync(posterPath, posterBytes);
                    Movie.PosterPath = posterFileName;
                }
            }

            await _movieService.UpdateMovieAsync(Movie);
        }

        return Page();
    }
}
