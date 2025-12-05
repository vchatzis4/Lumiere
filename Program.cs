using Microsoft.EntityFrameworkCore;
using Lumière.Data;
using Lumière.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

// Configure SQLite Database
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "movies.db");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Add custom services
builder.Services.AddScoped<TMDBService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddHostedService<FileWatcherService>();

// Configure URLs
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Check if we need to recreate the database due to schema changes
    if (File.Exists(dbPath))
    {
        try
        {
            // Try to query for a movie with slug - if it fails, we need to recreate
            var testQuery = await db.Movies.Select(m => m.Slug).FirstOrDefaultAsync();
        }
        catch
        {
            Console.WriteLine("⚠️  Database schema needs to be updated.");
            Console.WriteLine("⚠️  The database will be deleted and recreated.");
            Console.WriteLine("⚠️  Your movies will be automatically re-scanned.");
            Console.WriteLine("Press any key to continue or Ctrl+C to cancel...");
            Console.ReadKey();

            File.Delete(dbPath);
            Console.WriteLine("Database deleted.");
        }
    }

    db.Database.EnsureCreated();

    // Generate slugs for existing movies that don't have them
    var moviesWithoutSlugs = await db.Movies.Where(m => string.IsNullOrEmpty(m.Slug)).ToListAsync();
    if (moviesWithoutSlugs.Any())
    {
        Console.WriteLine($"Generating slugs for {moviesWithoutSlugs.Count} movies...");
        var usedSlugs = new HashSet<string>();

        foreach (var movie in moviesWithoutSlugs)
        {
            var baseSlug = Lumière.Models.Movie.GenerateSlug(movie.Title);
            var slug = baseSlug;
            var counter = 1;

            // Ensure slug is unique
            while (usedSlugs.Contains(slug) || await db.Movies.AnyAsync(m => m.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            movie.Slug = slug;
            usedSlugs.Add(slug);
        }

        await db.SaveChangesAsync();
        Console.WriteLine("Slugs generated successfully!");
    }

    // Optional: Scan for movies on startup
    var movieService = scope.ServiceProvider.GetRequiredService<MovieService>();
    Console.WriteLine("Scanning for movies...");
    var newMovies = await movieService.ScanFolderForMoviesAsync();
    Console.WriteLine($"Found {newMovies.Count} new movies");
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles(); // Serves files from wwwroot

// Serve Media folder as static files with proper MIME types
var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
provider.Mappings[".srt"] = "text/plain"; // SRT subtitle files
provider.Mappings[".vtt"] = "text/vtt"; // WebVTT subtitle files

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Media")),
    RequestPath = "/Media",
    ContentTypeProvider = provider
});

app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// API Endpoints for video streaming and watch position
app.MapGet("/api/stream/{id:int}", async (int id, MovieService movieService, HttpContext context) =>
{
    var movie = await movieService.GetMovieByIdAsync(id);
    if (movie == null || !File.Exists(movie.FilePath))
    {
        return Results.NotFound();
    }

    var fileInfo = new FileInfo(movie.FilePath);
    var fileStream = new FileStream(movie.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

    return Results.File(fileStream, "video/mp4", enableRangeProcessing: true);
});

app.MapPost("/api/watchposition/{id:int}", async (int id, HttpContext context, MovieService movieService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(body);

    if (data != null && data.TryGetValue("position", out int position))
    {
        await movieService.UpdateWatchPositionAsync(id, position);
        return Results.Ok();
    }

    return Results.BadRequest();
});

// API Endpoint for serving subtitles (converts SRT to VTT)
app.MapGet("/api/subtitle/{id:int}", async (int id, MovieService movieService) =>
{
    try
    {
        var movie = await movieService.GetMovieByIdAsync(id);
        if (movie == null)
        {
            return Results.NotFound($"Movie with ID {id} not found");
        }

        if (string.IsNullOrEmpty(movie.SubtitlePath))
        {
            return Results.NotFound($"Movie '{movie.Title}' has no subtitle path");
        }

        if (!File.Exists(movie.SubtitlePath))
        {
            return Results.NotFound($"Subtitle file not found at: {movie.SubtitlePath}");
        }

        // Read the SRT file with UTF-8 encoding
        var srtContent = await File.ReadAllTextAsync(movie.SubtitlePath, System.Text.Encoding.UTF8);

        // Remove BOM if present
        srtContent = srtContent.TrimStart('\uFEFF');

        // Convert SRT to VTT format (replace comma with period in timestamps)
        var vttContent = "WEBVTT\n\n" + srtContent.Replace(",", ".");

        return Results.Text(vttContent, "text/vtt");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error serving subtitle for movie ID {id}: {ex.Message}");
        return Results.Problem($"Error loading subtitle: {ex.Message}");
    }
});

// API Endpoint for refreshing movie metadata from TMDB
app.MapPost("/api/movies/{id:int}/refresh-metadata", async (int id, MovieService movieService) =>
{
    try
    {
        var success = await movieService.RefreshMetadataAsync(id);
        if (success)
        {
            return Results.Ok(new { message = "Metadata refreshed successfully" });
        }
        return Results.NotFound(new { message = "Movie not found or no TMDB results" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error refreshing metadata for movie ID {id}: {ex.Message}");
        return Results.Problem($"Error refreshing metadata: {ex.Message}");
    }
});

// Auto-open browser
var url = "http://localhost:5000";
Task.Run(() =>
{
    Thread.Sleep(1500); // Wait for server to start
    OpenBrowser(url);
});

Console.WriteLine($"Netflix Clone is running at {url}");
Console.WriteLine("Press Ctrl+C to stop the application");

app.Run();

static void OpenBrowser(string url)
{
    try
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unable to auto-open browser: {ex.Message}");
        Console.WriteLine($"Please manually open: {url}");
    }
}
