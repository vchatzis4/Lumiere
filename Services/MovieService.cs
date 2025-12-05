using Microsoft.EntityFrameworkCore;
using MyNetflixClone.Data;
using MyNetflixClone.Models;

namespace MyNetflixClone.Services;

public class MovieService
{
    private readonly ApplicationDbContext _context;
    private readonly TMDBService _tmdbService;
    private readonly string _mediaPath;
    private readonly string _postersPath;
    private readonly string _subtitlesPath;

    public MovieService(ApplicationDbContext context, TMDBService tmdbService, IConfiguration configuration)
    {
        _context = context;
        _tmdbService = tmdbService;
        _mediaPath = configuration["MediaSettings:MoviesPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Media", "Movies");
        _postersPath = configuration["MediaSettings:PostersPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Media", "Posters");
        _subtitlesPath = configuration["MediaSettings:SubtitlesPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Media", "Subtitles");

        // Ensure directories exist
        Directory.CreateDirectory(_mediaPath);
        Directory.CreateDirectory(_postersPath);
        Directory.CreateDirectory(_subtitlesPath);
    }

    private async Task<List<Movie>> FilterValidMoviesAsync(List<Movie> movies)
    {
        var validMovies = new List<Movie>();
        foreach (var movie in movies)
        {
            if (!string.IsNullOrEmpty(movie.FilePath) && File.Exists(movie.FilePath))
            {
                validMovies.Add(movie);
            }
            else
            {
                // Movie file doesn't exist, remove from database
                _context.Movies.Remove(movie);
            }
        }

        if (movies.Count != validMovies.Count)
        {
            await _context.SaveChangesAsync();
        }

        return validMovies;
    }

    public async Task<List<Movie>> GetAllMoviesAsync()
    {
        var movies = await _context.Movies.OrderByDescending(m => m.DateAdded).ToListAsync();
        return await FilterValidMoviesAsync(movies);
    }

    public async Task<Movie?> GetMovieByIdAsync(int id)
    {
        return await _context.Movies.FindAsync(id);
    }

    public async Task<Movie?> GetMovieBySlugAsync(string slug)
    {
        return await _context.Movies.FirstOrDefaultAsync(m => m.Slug == slug);
    }

    public async Task<List<Movie>> SearchMoviesAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        var movies = await _context.Movies
            .Where(m => m.Title.ToLower().Contains(lowerSearchTerm) ||
                       (m.Description != null && m.Description.ToLower().Contains(lowerSearchTerm)))
            .ToListAsync();
        return await FilterValidMoviesAsync(movies);
    }

    public async Task<List<Movie>> GetMoviesByGenreAsync(string genre)
    {
        var movies = await _context.Movies
            .Where(m => m.Genre != null && m.Genre.Contains(genre))
            .ToListAsync();
        return await FilterValidMoviesAsync(movies);
    }

    public async Task<List<string>> GetAllGenresAsync()
    {
        var genres = await _context.Movies
            .Where(m => m.Genre != null)
            .Select(m => m.Genre!)
            .ToListAsync();

        var allGenres = new HashSet<string>();
        foreach (var genreString in genres)
        {
            var splitGenres = genreString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var genre in splitGenres)
            {
                allGenres.Add(genre);
            }
        }

        return allGenres.OrderBy(g => g).ToList();
    }

    public async Task<List<Movie>> GetRecentlyAddedAsync(int count = 10)
    {
        var movies = await _context.Movies
            .OrderByDescending(m => m.DateAdded)
            .Take(count)
            .ToListAsync();
        return await FilterValidMoviesAsync(movies);
    }

    public async Task<List<Movie>> GetContinueWatchingAsync(int count = 10)
    {
        var movies = await _context.Movies
            .Where(m => m.LastWatchedPosition > 0 && m.Duration.HasValue && m.LastWatchedPosition < (m.Duration.Value * 60 * 0.95))
            .OrderByDescending(m => m.DateAdded)
            .Take(count)
            .ToListAsync();
        return await FilterValidMoviesAsync(movies);
    }

    public async Task<Movie> AddMovieAsync(string filePath, string? customTitle = null)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var title = customTitle ?? fileName;

        // Check for subtitle file in Subtitles folder
        var subtitleFileName = Path.GetFileNameWithoutExtension(filePath) + ".srt";
        var subtitlePath = Path.Combine(_subtitlesPath, subtitleFileName);
        var hasSubtitle = File.Exists(subtitlePath);

        Console.WriteLine($"[AddMovie] Movie: {fileName}");
        Console.WriteLine($"[AddMovie] Looking for subtitle: {subtitlePath}");
        Console.WriteLine($"[AddMovie] Subtitle exists: {hasSubtitle}");

        // Generate unique slug
        var baseSlug = Movie.GenerateSlug(title);
        var slug = baseSlug;
        var counter = 1;
        while (await _context.Movies.AnyAsync(m => m.Slug == slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        var movie = new Movie
        {
            Title = title,
            Slug = slug,
            FilePath = filePath,
            SubtitlePath = hasSubtitle ? subtitlePath : null,
            DateAdded = DateTime.UtcNow
        };

        // Try to fetch metadata from TMDB
        var metadata = await _tmdbService.SearchMovieAsync(title);
        if (metadata != null)
        {
            movie.Title = metadata.Title;

            // Update slug based on TMDB title (ensure uniqueness)
            var newBaseSlug = Movie.GenerateSlug(metadata.Title);
            var newSlug = newBaseSlug;
            var newCounter = 1;
            while (await _context.Movies.AnyAsync(m => m.Slug == newSlug && m.Id != movie.Id))
            {
                newSlug = $"{newBaseSlug}-{newCounter}";
                newCounter++;
            }
            movie.Slug = newSlug;

            movie.Description = metadata.Description;
            movie.Year = metadata.Year;
            movie.Genre = metadata.Genre;
            movie.Rating = metadata.Rating;
            movie.Duration = metadata.Duration;
            movie.Director = metadata.Director;
            movie.Cast = metadata.Cast;

            // Download poster
            if (!string.IsNullOrEmpty(metadata.PosterUrl))
            {
                try
                {
                    Console.WriteLine($"Downloading poster from: {metadata.PosterUrl}");
                    var posterBytes = await _tmdbService.DownloadPosterAsync(metadata.PosterUrl);
                    if (posterBytes != null && posterBytes.Length > 0)
                    {
                        var posterFileName = $"{movie.Title.Replace(" ", "_").Replace("/", "_").Replace(":", "_")}_{DateTime.Now.Ticks}.jpg";
                        var posterPath = Path.Combine(_postersPath, posterFileName);
                        await System.IO.File.WriteAllBytesAsync(posterPath, posterBytes);
                        movie.PosterPath = posterFileName;
                        Console.WriteLine($"Poster saved: {posterFileName} ({posterBytes.Length} bytes)");
                    }
                    else
                    {
                        Console.WriteLine("Poster download returned no data");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading poster: {ex.Message}");
                }
            }

            // Download backdrop
            if (!string.IsNullOrEmpty(metadata.BackdropUrl))
            {
                try
                {
                    Console.WriteLine($"Downloading backdrop from: {metadata.BackdropUrl}");
                    var backdropBytes = await _tmdbService.DownloadPosterAsync(metadata.BackdropUrl);
                    if (backdropBytes != null && backdropBytes.Length > 0)
                    {
                        var backdropFileName = $"{movie.Title.Replace(" ", "_").Replace("/", "_").Replace(":", "_")}_{DateTime.Now.Ticks}_backdrop.jpg";
                        var backdropPath = Path.Combine(_postersPath, backdropFileName);
                        await System.IO.File.WriteAllBytesAsync(backdropPath, backdropBytes);
                        movie.BackdropPath = backdropFileName;
                        Console.WriteLine($"Backdrop saved: {backdropFileName} ({backdropBytes.Length} bytes)");
                    }
                    else
                    {
                        Console.WriteLine("Backdrop download returned no data");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading backdrop: {ex.Message}");
                }
            }
        }

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return movie;
    }

    public async Task UpdateMovieAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateWatchPositionAsync(int movieId, int positionInSeconds)
    {
        var movie = await GetMovieByIdAsync(movieId);
        if (movie != null)
        {
            movie.LastWatchedPosition = positionInSeconds;
            await UpdateMovieAsync(movie);
        }
    }

    public async Task DeleteMovieAsync(int id)
    {
        var movie = await GetMovieByIdAsync(id);
        if (movie != null)
        {
            // Only remove from database, keep all files (movie, subtitle, poster, backdrop)
            // This prevents users from accidentally losing their media files permanently
            Console.WriteLine($"Removing movie '{movie.Title}' from database (files will be kept)");

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Movie removed from library. Files remain at:");
            if (!string.IsNullOrEmpty(movie.FilePath))
                Console.WriteLine($"  - Movie: {movie.FilePath}");
            if (!string.IsNullOrEmpty(movie.SubtitlePath))
                Console.WriteLine($"  - Subtitle: {movie.SubtitlePath}");
        }
    }

    public async Task<List<Movie>> ScanFolderForMoviesAsync(string? folderPath = null)
    {
        var scanPath = folderPath ?? _mediaPath;
        var videoExtensions = new[] { ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".webm" };
        var newMovies = new List<Movie>();

        if (!Directory.Exists(scanPath))
        {
            Directory.CreateDirectory(scanPath);
            return newMovies;
        }

        var files = Directory.GetFiles(scanPath, "*.*", SearchOption.AllDirectories)
            .Where(f => videoExtensions.Contains(Path.GetExtension(f).ToLower()))
            .ToList();

        foreach (var file in files)
        {
            // Check if movie already exists in database
            var existingMovie = await _context.Movies.FirstOrDefaultAsync(m => m.FilePath == file);
            if (existingMovie == null)
            {
                var movie = await AddMovieAsync(file);
                newMovies.Add(movie);
            }
        }

        return newMovies;
    }
}
