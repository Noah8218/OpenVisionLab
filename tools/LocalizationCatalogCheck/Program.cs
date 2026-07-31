using System.Text.RegularExpressions;

internal static class Program
{
    private static readonly Regex DirectKeyPattern = new Regex(
        @"OpenVisionLanguageService\.T\(\s*""(?<key>[^""]+)""",
        RegexOptions.Compiled);
    private static readonly Regex DirectFormatKeyPattern = new Regex(
        @"OpenVisionLanguageService\.TF\(\s*""(?<key>[^""]+)""",
        RegexOptions.Compiled);

    private static int Main(string[] args)
    {
        try
        {
            string repoRoot = ResolveRepoRoot(args);
            string catalogPath = Path.Combine(repoRoot, "src", "Libraries", "OpenVisionLab.Localization", "Resources", "LocalizationCatalog.tsv");
            Dictionary<string, CatalogEntry> catalog = LoadCatalog(catalogPath);
            List<string> usedKeys = FindDirectLocalizationKeys(repoRoot);
            List<string> missing = usedKeys
                .Where(key => !catalog.ContainsKey(key))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (missing.Count > 0)
            {
                throw new InvalidOperationException("Missing localization keys: " + string.Join(", ", missing));
            }

            Console.WriteLine($"LocalizationCatalog=OK | Entries={catalog.Count} | DirectKeys={usedKeys.Distinct(StringComparer.OrdinalIgnoreCase).Count()}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("LocalizationCatalog=NG");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static string ResolveRepoRoot(string[] args)
    {
        if (args.Length > 0 && Directory.Exists(args[0]))
        {
            return Path.GetFullPath(args[0]);
        }

        string current = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(current))
        {
            if (File.Exists(Path.Combine(current, "OpenVisionLab.sln")))
            {
                return current;
            }

            DirectoryInfo parent = Directory.GetParent(current);
            current = parent?.FullName;
        }

        return Directory.GetCurrentDirectory();
    }

    private static Dictionary<string, CatalogEntry> LoadCatalog(string catalogPath)
    {
        if (!File.Exists(catalogPath))
        {
            throw new FileNotFoundException("Localization catalog was not found.", catalogPath);
        }

        Dictionary<string, CatalogEntry> entries = new Dictionary<string, CatalogEntry>(StringComparer.OrdinalIgnoreCase);
        int lineNumber = 0;
        foreach (string line in File.ReadLines(catalogPath))
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split('\t');
            if (parts.Length < 3)
            {
                throw new InvalidOperationException($"Catalog line {lineNumber} must have Key, Korean, English columns.");
            }

            string key = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException($"Catalog line {lineNumber} has an empty key.");
            }

            if (entries.ContainsKey(key))
            {
                throw new InvalidOperationException($"Duplicate localization key: {key}");
            }

            if (string.IsNullOrWhiteSpace(parts[1]) || string.IsNullOrWhiteSpace(parts[2]))
            {
                throw new InvalidOperationException($"Catalog key '{key}' has an empty translation.");
            }

            entries.Add(key, new CatalogEntry(key, parts[1], parts[2]));
        }

        return entries;
    }

    private static List<string> FindDirectLocalizationKeys(string repoRoot)
    {
        string[] roots =
        {
            Path.Combine(repoRoot, "src", "OpenVisionLab"),
            Path.Combine(repoRoot, "src", "Libraries")
        };

        List<string> keys = new List<string>();
        foreach (string root in roots.Where(Directory.Exists))
        {
            foreach (string path in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                if (path.Contains(Path.Combine("src", "Libraries", "OpenVisionLab.Localization", "Resources"), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string text = File.ReadAllText(path);
                foreach (Match match in DirectKeyPattern.Matches(text))
                {
                    string key = match.Groups["key"].Value;
                    if (!key.EndsWith(".", StringComparison.Ordinal))
                    {
                        keys.Add(key);
                    }
                }

                foreach (Match match in DirectFormatKeyPattern.Matches(text))
                {
                    string key = match.Groups["key"].Value;
                    if (!key.EndsWith(".", StringComparison.Ordinal))
                    {
                        keys.Add(key);
                    }
                }
            }
        }

        return keys;
    }

    private sealed record CatalogEntry(string Key, string Korean, string English);
}
