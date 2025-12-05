using Lumière.Data;

namespace Lumière.Services;

public class FileWatcherService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private FileSystemWatcher? _watcher;
    private readonly string _mediaPath;

    public FileWatcherService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _mediaPath = configuration["MediaSettings:MoviesPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Media", "Movies");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_mediaPath))
        {
            Directory.CreateDirectory(_mediaPath);
        }

        _watcher = new FileSystemWatcher(_mediaPath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.*",
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        _watcher.Created += OnFileCreated;

        return Task.CompletedTask;
    }

    private async void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        var videoExtensions = new[] { ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".webm" };

        if (videoExtensions.Contains(Path.GetExtension(e.FullPath).ToLower()))
        {
            // Wait a bit to ensure file is fully copied
            await Task.Delay(2000);

            using var scope = _serviceProvider.CreateScope();
            var movieService = scope.ServiceProvider.GetRequiredService<MovieService>();

            try
            {
                await movieService.AddMovieAsync(e.FullPath);
                Console.WriteLine($"Automatically added new movie: {e.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding movie {e.Name}: {ex.Message}");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _watcher?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}
