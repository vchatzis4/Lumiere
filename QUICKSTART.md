# Lumière - Quick Start Guide

## Getting Started in 5 Minutes

### Step 1: Install .NET 8 SDK
Download and install from: https://dotnet.microsoft.com/download/dotnet/8.0

### Step 2: Configure the Application

Copy the template configuration:
```bash
cp appsettings.template.json appsettings.json
```

### Step 3: Get a TMDB API Key (Optional but Recommended)

1. Visit: https://www.themoviedb.org/settings/api
2. Sign up for a free account
3. Request an API key
4. Open `appsettings.json` and add your key:
   ```json
   {
     "TMDB": {
       "ApiKey": "YOUR_KEY_HERE"
     }
   }
   ```

### Step 4: Run the Application

**Option A: Quick Development Run**
```bash
cd MyNetflixClone
dotnet run
```

**Option B: Build Single Executable**

Windows:
```bash
build.bat
```

Linux/Mac:
```bash
chmod +x build.sh
./build.sh
```

### Step 5: Add Your First Movie

1. Place a video file in `Media/Movies/` folder
   - Example: `Media/Movies/Inception.mp4`

2. Open the app (http://localhost:5000)

3. Go to **Library** (top navigation)

4. Click **"Scan Media Folder"**

5. Watch your movie appear with poster and metadata!

## Navigation

| Menu Item | Purpose |
|-----------|---------|
| **Home** | Featured movie, carousels, continue watching |
| **Collection** | Browse all movies with filters |
| **Library** | Add, edit, delete movies |

## Supported Video Formats
.mp4, .mkv, .avi, .mov, .wmv, .flv, .webm

## Adding Subtitles
Place a .srt file with the same name in `Media/Subtitles/`:
```
Media/
  ├── Movies/
  │   └── Inception.mp4
  └── Subtitles/
      └── Inception.srt  ← Automatically detected!
```

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `/` | Focus search bar |
| `Escape` | Unfocus |

## Distributing to Friends

After building, copy these to a USB stick:
- `Lumiere.exe` (from publish folder)
- `Media/` folder
- `movies.db`
- `appsettings.json`

They just double-click and watch!

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Movies don't have posters | Add your TMDB API key to appsettings.json |
| Video won't play | Check file format, try Chrome/Edge |
| Can't find executable | Look in `publish/` folder |
| appsettings.json missing | Copy from appsettings.template.json |

## Need Help?
Check the full README.md for detailed documentation.

---

**Lumière** — Your Private Film Collection
