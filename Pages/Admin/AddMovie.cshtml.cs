using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lumière.Services;

namespace Lumière.Pages.Admin;

public class AddMovieModel : PageModel
{
    private readonly MovieService _movieService;

    public AddMovieModel(MovieService movieService)
    {
        _movieService = movieService;
    }

    [BindProperty]
    public string FilePath { get; set; } = string.Empty;

    [BindProperty]
    public string? CustomTitle { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            ErrorMessage = "Please provide a file path.";
            return Page();
        }

        if (!System.IO.File.Exists(FilePath))
        {
            ErrorMessage = $"File not found: {FilePath}";
            return Page();
        }

        try
        {
            var movie = await _movieService.AddMovieAsync(FilePath, CustomTitle);
            SuccessMessage = $"Movie '{movie.Title}' added successfully!";

            // Clear form
            FilePath = string.Empty;
            CustomTitle = null;

            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error adding movie: {ex.Message}";
            return Page();
        }
    }
}
