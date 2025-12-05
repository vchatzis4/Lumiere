using System.Text.RegularExpressions;

namespace Lumière.Services;

public class ParsedFilename
{
    public string Title { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string? Quality { get; set; }
    public string? Source { get; set; }
    public string OriginalFilename { get; set; } = string.Empty;
}

public static class FilenameParser
{
    // Common release group tags to remove
    private static readonly string[] ReleaseGroups = new[]
    {
        "YIFY", "YTS", "RARBG", "SPARKS", "GECKOS", "FGT", "EVO", "STUTTERSHIT",
        "AMIABLE", "DRONES", "CMRG", "NTG", "ION10", "FLUX", "TEPES", "TOMMY"
    };

    // Quality patterns
    private static readonly Regex QualityPattern = new(
        @"\b(2160p|1080p|720p|480p|4K|UHD)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Source patterns
    private static readonly Regex SourcePattern = new(
        @"\b(BluRay|Blu-Ray|BDRip|BRRip|WEB-DL|WEBRip|WEB|HDRip|DVDRip|HDTV|DVDScr|CAM|TS|TELESYNC|HC|PROPER|REPACK|REMUX)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Year pattern (1920-2029)
    private static readonly Regex YearPattern = new(
        @"\b((?:19|20)\d{2})\b",
        RegexOptions.Compiled);

    // Codec patterns to remove
    private static readonly Regex CodecPattern = new(
        @"\b(x264|x265|H\.?264|H\.?265|HEVC|XviD|DivX|AAC|AC3|DTS|MP3|FLAC|10bit|HDR|HDR10|SDR|Atmos)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Audio channel patterns
    private static readonly Regex AudioPattern = new(
        @"\b(5\.1|7\.1|2\.0|6CH|8CH)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Scene info patterns
    private static readonly Regex ScenePattern = new(
        @"\b(EXTENDED|UNRATED|DIRECTORS\.?CUT|THEATRICAL|REMASTERED|IMAX|3D|DUAL|MULTI)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static ParsedFilename Parse(string filePath)
    {
        var result = new ParsedFilename
        {
            OriginalFilename = Path.GetFileName(filePath)
        };

        // Get filename without extension
        var filename = Path.GetFileNameWithoutExtension(filePath);

        // Extract quality
        var qualityMatch = QualityPattern.Match(filename);
        if (qualityMatch.Success)
        {
            result.Quality = qualityMatch.Value.ToUpperInvariant();
        }

        // Extract source
        var sourceMatch = SourcePattern.Match(filename);
        if (sourceMatch.Success)
        {
            result.Source = sourceMatch.Value;
        }

        // Extract year - find all years and use the most likely one
        var yearMatches = YearPattern.Matches(filename);
        if (yearMatches.Count > 0)
        {
            // Prefer year that appears after title (usually first occurrence between 1920-current+1)
            foreach (Match match in yearMatches)
            {
                if (int.TryParse(match.Value, out int year))
                {
                    if (year >= 1920 && year <= DateTime.Now.Year + 1)
                    {
                        result.Year = year;
                        break;
                    }
                }
            }
        }

        // Clean up the title
        var title = filename;

        // Replace common separators with spaces
        title = title.Replace('.', ' ').Replace('_', ' ').Replace('-', ' ');

        // Remove everything after the year (if found)
        if (result.Year.HasValue)
        {
            var yearIndex = title.IndexOf(result.Year.Value.ToString());
            if (yearIndex > 0)
            {
                title = title.Substring(0, yearIndex);
            }
        }

        // Remove quality, source, codec patterns
        title = QualityPattern.Replace(title, " ");
        title = SourcePattern.Replace(title, " ");
        title = CodecPattern.Replace(title, " ");
        title = AudioPattern.Replace(title, " ");
        title = ScenePattern.Replace(title, " ");

        // Remove release group names
        foreach (var group in ReleaseGroups)
        {
            title = Regex.Replace(title, $@"\b{group}\b", " ", RegexOptions.IgnoreCase);
        }

        // Remove content in brackets/parentheses (often contains junk)
        title = Regex.Replace(title, @"\[.*?\]", " ");
        title = Regex.Replace(title, @"\(.*?\)", " ");

        // Clean up whitespace
        title = Regex.Replace(title, @"\s+", " ").Trim();

        // Remove trailing dashes or dots
        title = title.TrimEnd('-', '.', ' ');

        // Capitalize first letter of each word (Title Case)
        title = ToTitleCase(title);

        result.Title = string.IsNullOrWhiteSpace(title)
            ? Path.GetFileNameWithoutExtension(filePath)
            : title;

        return result;
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpperInvariant(words[i][0]) +
                           (words[i].Length > 1 ? words[i].Substring(1).ToLowerInvariant() : "");
            }
        }
        return string.Join(" ", words);
    }
}
