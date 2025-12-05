using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNetflixClone.Models;
using MyNetflixClone.Services;

namespace MyNetflixClone.Pages;

public class SearchModel : PageModel
{
    private readonly MovieService _movieService;

    public SearchModel(MovieService movieService)
    {
        _movieService = movieService;
    }

    public List<Movie> Movies { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    [FromQuery(Name = "q")]
    public string? Query { get; set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(Query))
        {
            Movies = await _movieService.SearchMoviesAsync(Query);
        }
    }
}
