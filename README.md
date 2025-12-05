# Lumière - Personal Film Collection

A self-contained desktop application for managing and streaming your personal movie collection with a luxurious cinema-inspired interface.

## Features

- **Cinema Noir Luxe UI**: Elegant dark theme with warm gold accents and film grain effects
- **Video Player**: Built-in HTML5 video player with Plyr
- **Resume Playback**: Automatically resumes from where you left off
- **Auto Metadata**: Fetches movie information (poster, description, cast) from TMDB
- **Library Dashboard**: Easy movie management interface
- **Folder Scanning**: Automatically detects new movies in your media folder
- **Search & Filter**: Find movies by title, genre, year, or rating
- **Self-Contained**: Single executable with embedded database
- **Portable**: Run from USB stick - no installation required

## Screenshots

The interface features:
- Elegant serif typography (Cormorant Garamond)
- Warm gold accent color scheme
- Subtle film grain overlay
- Smooth animations and hover effects
- 3D tilt effect on movie cards

## Requirements

- .NET 8 SDK (for development)
- Windows, Linux, or macOS
- TMDB API Key (optional, for automatic metadata fetching)

## Quick Start

### 1. Configure the Application

Copy the template configuration file:
```bash
cp appsettings.template.json appsettings.json
```

### 2. Get a TMDB API Key (Optional but Recommended)

1. Go to https://www.themoviedb.org/settings/api
2. Create a free account and request an API key
3. Copy your API key
4. Open `appsettings.json` and add it:

```json
{
  "TMDB": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

### 3. Build the Application

#### Development Mode
```bash
dotnet restore
dotnet run
```

The application will start and automatically open your browser at http://localhost:5000

#### Production Build (Single Executable)
```bash
# For Windows
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./publish

# For Linux
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -o ./publish

# For macOS
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -o ./publish
```

The executable will be in the `publish` folder.

## How to Use

### Adding Movies

#### Method 1: Automatic Scanning
1. Place your video files in the `Media/Movies` folder
2. Go to Library (http://localhost:5000/Admin)
3. Click "Scan Media Folder"
4. The app will automatically add movies and fetch metadata from TMDB

#### Method 2: Manual Addition
1. Go to Library
2. Click "Add Movie Manually"
3. Enter the full path to your video file
4. Optionally provide a custom title
5. Click "Add Movie"

### Supported Video Formats
- .mp4
- .mkv
- .avi
- .mov
- .wmv
- .flv
- .webm

### Subtitles
Place a `.srt` file with the same name as your video file in the `Media/Subtitles` folder:
```
Media/
  ├── Movies/
  │   └── MyMovie.mp4
  └── Subtitles/
      └── MyMovie.srt
```

## Project Structure

```
Lumiere/
├── Data/               # Database context
├── Models/             # Data models
├── Services/           # Business logic (TMDB, Movie service, etc.)
├── Pages/              # Razor Pages (UI)
│   ├── Admin/          # Library dashboard pages
│   └── Shared/         # Shared layout
├── wwwroot/            # Static files
│   ├── css/            # Stylesheets (Cinema Noir Luxe theme)
│   └── js/             # JavaScript (animations, interactions)
├── Media/              # Media storage
│   ├── Movies/         # Video files
│   ├── Posters/        # Downloaded posters
│   └── Subtitles/      # Subtitle files
├── appsettings.template.json  # Configuration template
└── movies.db           # SQLite database (auto-created)
```

## Configuration

Copy `appsettings.template.json` to `appsettings.json` and customize:

```json
{
  "TMDB": {
    "ApiKey": "your-tmdb-api-key"
  },
  "MediaSettings": {
    "MoviesPath": "Media/Movies",
    "PostersPath": "Media/Posters"
  }
}
```

> **Note**: `appsettings.json` is gitignored to protect your API key. Only the template is committed.

## Database

The app uses SQLite for storage. The database file `movies.db` is created automatically on first run.

### Schema

**Movies Table:**
- Id (Primary Key)
- Title, Slug
- FilePath, PosterPath, BackdropPath
- Description, Year, Genre, Rating, Duration
- Director, Cast
- LastWatchedPosition, DateAdded
- SubtitlePath

## Features Explained

### Continue Watching
Movies you've partially watched appear in the "Continue Watching" section with a progress bar.

### Recently Added
Shows the latest movies added to your library.

### Genre Carousels
Movies are automatically organized into genre categories based on TMDB metadata.

### Watch Position Tracking
The app saves your playback position every 5 seconds, so you can resume exactly where you left off.

### File Watcher
The FileWatcherService monitors your Media/Movies folder and automatically adds new files as they're copied in.

### Keyboard Shortcuts
- Press `/` to focus the search bar
- Press `Escape` to unfocus

## Distributing Your Library

To share your movie collection:

1. Build the single-file executable (see Build instructions above)
2. Copy these items to a USB stick or shared folder:
   - `Lumiere.exe` (the executable from `publish/`)
   - `Media/` folder (with your movies and posters)
   - `movies.db` (the database)
   - `appsettings.json` (configuration with your API key)

3. They just double-click the .exe and start watching!

## Troubleshooting

### Movies not showing metadata
- Make sure you've added your TMDB API key to appsettings.json
- Check that the movie title matches a movie on TMDB
- Try editing the movie and clicking "Refresh from TMDB"

### Video won't play
- Ensure the video format is supported
- Check that the file path is correct
- Try a different browser (Chrome/Edge recommended)

### Subtitles not appearing
- Ensure the .srt file has the exact same name as the video file
- Place subtitle files in `Media/Subtitles/`

## Technology Stack

- **.NET 8**: ASP.NET Core with Razor Pages
- **SQLite**: Embedded database with Entity Framework Core
- **Plyr**: HTML5 video player with custom controls
- **TMDB API**: Movie metadata provider
- **Cinema Noir Luxe**: Custom CSS theme with Cormorant Garamond & Outfit fonts

## Design

The UI follows a "Cinema Noir Luxe" aesthetic:
- **Colors**: Deep blacks (#0a0a0a) with warm gold accents (#d4a853)
- **Typography**: Cormorant Garamond (display) + Outfit (body)
- **Effects**: Film grain overlay, vignette, glow shadows
- **Animations**: Staggered reveals, 3D card tilt, momentum scrolling

## License

This is a personal project for organizing legally owned media. Not for piracy or commercial use.

## Credits

- Movie metadata from [The Movie Database (TMDB)](https://www.themoviedb.org/)
- Video player: [Plyr](https://plyr.io/)
- Fonts: [Google Fonts](https://fonts.google.com/)

---

**Lumière** — Your Private Film Collection
