using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNetflixClone.Models;
using MyNetflixClone.Services;

namespace MyNetflixClone.Pages.Movies;

public class DetailsModel : PageModel
{
    private readonly MovieService _movieService;

    public DetailsModel(MovieService movieService)
    {
        _movieService = movieService;
    }

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
}
