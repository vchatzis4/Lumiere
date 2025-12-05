using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lumière.Models;
using Lumière.Services;

namespace Lumière.Pages;

public class WatchModel : PageModel
{
    private readonly MovieService _movieService;

    public WatchModel(MovieService movieService)
    {
        _movieService = movieService;
    }

    public Movie? Movie { get; set; }

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        Movie = await _movieService.GetMovieBySlugAsync(slug);

        if (Movie == null)
        {
            return NotFound();
        }

        return Page();
    }
}
