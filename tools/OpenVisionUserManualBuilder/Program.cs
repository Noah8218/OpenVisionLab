using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Markdig;

internal static class Program
{
    private static readonly ManualLocaleSource[] ManualLocales =
    {
        new(
            "ko",
            "manual-manifest.json",
            "manual-visuals.json",
            "OpenVisionLab_User_Manual.ko.html",
            "기능, 화면 용어, Metric 검색",
            "현재 OpenVisionLab 한국어 UI 캡처"),
        new(
            "en",
            "manual-manifest.en.json",
            "manual-visuals.en.json",
            "OpenVisionLab_User_Manual.en.html",
            "Search features, UI terms, and metrics",
            "Current OpenVisionLab English UI capture")
    };

    private static readonly string[] ExpectedToolIds =
    {
        "Threshold", "Filter", "Morphology", "Arithmetic", "EdgeDetection",
        "RotateAndScale", "AffineTransform", "Histogram", "HSV", "Mean",
        "Blob", "Contour", "Line", "Matching", "EdgeBasedMatching",
        "FeatureMatching", "Pipeline"
    };

    private static readonly MarkdownPipeline MarkdownPipeline =
        new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    private static int Main(string[] args)
    {
        try
        {
            string repoRoot = ResolveRepoRoot(args);
            string manualRoot = Path.Combine(repoRoot, "docs", "manual");
            string generatedRoot = Path.Combine(manualRoot, "generated");
            Directory.CreateDirectory(generatedRoot);
            JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };
            List<GuideManualEntry> generatedManuals = new();
            string? manualVersion = null;
            string? applicationVersion = null;

            foreach (ManualLocaleSource locale in ManualLocales)
            {
                ManualSourceManifest manifest = JsonSerializer.Deserialize<ManualSourceManifest>(
                    File.ReadAllText(Path.Combine(manualRoot, locale.ManifestFile), Encoding.UTF8),
                    jsonOptions)
                    ?? throw new InvalidOperationException($"Manual manifest is empty: {locale.ManifestFile}");
                ManualVisualManifest visuals = JsonSerializer.Deserialize<ManualVisualManifest>(
                    File.ReadAllText(Path.Combine(manualRoot, locale.VisualsFile), Encoding.UTF8),
                    jsonOptions)
                    ?? throw new InvalidOperationException($"Manual visual manifest is empty: {locale.VisualsFile}");

                ValidateManifest(manifest, manualRoot);
                ValidateVisuals(visuals, manifest, manualRoot);
                if (manualVersion != null &&
                    (!string.Equals(manualVersion, manifest.ManualVersion, StringComparison.Ordinal) ||
                     !string.Equals(applicationVersion, manifest.ApplicationVersion, StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException("Localized manual versions do not match.");
                }

                manualVersion ??= manifest.ManualVersion;
                applicationVersion ??= manifest.ApplicationVersion;
                string html = BuildHtml(manifest, visuals, manualRoot, locale);
                string htmlPath = Path.Combine(generatedRoot, locale.OutputFile);
                WriteAtomic(htmlPath, html);
                string hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(htmlPath)));
                generatedManuals.Add(new GuideManualEntry(locale.Language, locale.OutputFile, hash));
                Console.WriteLine(
                    $"Manual={locale.Language} PASS Sections={manifest.Sections.Count} Tools={ExpectedToolIds.Length} SHA256={hash}");
            }

            string legacyManualPath = Path.Combine(generatedRoot, "OpenVisionLab_User_Manual.html");
            if (File.Exists(legacyManualPath))
            {
                File.Delete(legacyManualPath);
            }

            GuideManifest guideManifest = new(
                2,
                manualVersion ?? throw new InvalidOperationException("Manual version is missing."),
                applicationVersion ?? throw new InvalidOperationException("Application version is missing."),
                generatedManuals);
            string guideManifestPath = Path.Combine(generatedRoot, "guide-manifest.json");
            string guideJson = JsonSerializer.Serialize(
                guideManifest,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }) + Environment.NewLine;
            WriteAtomic(guideManifestPath, guideJson);

            Console.WriteLine($"Manual=PASS Languages={generatedManuals.Count} Sections={generatedManuals.Count * 26} Tools={generatedManuals.Count * ExpectedToolIds.Length}");
            foreach (GuideManualEntry manual in generatedManuals)
            {
                Console.WriteLine($"HTML[{manual.Language}]={Path.Combine(generatedRoot, manual.File)}");
            }
            Console.WriteLine($"Manifest={guideManifestPath}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Manual=FAIL {ex.Message}");
            return 1;
        }
    }

    private static string ResolveRepoRoot(string[] args)
    {
        string start = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
            ? Path.GetFullPath(args[0])
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        DirectoryInfo? current = new(start);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "OpenVisionLab.sln")))
            {
                return current.FullName;
            }
            current = current.Parent;
        }
        throw new DirectoryNotFoundException($"OpenVisionLab repository root was not found from {start}.");
    }

    private static void ValidateManifest(ManualSourceManifest manifest, string manualRoot)
    {
        if (manifest.SchemaVersion != 1 || string.IsNullOrWhiteSpace(manifest.ManualVersion) ||
            string.IsNullOrWhiteSpace(manifest.ApplicationVersion) || manifest.Sections.Count == 0)
        {
            throw new InvalidOperationException("Manual manifest header is invalid.");
        }

        string[] duplicateIds = manifest.Sections.GroupBy(section => section.Id, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1).Select(group => group.Key).ToArray();
        if (duplicateIds.Length > 0)
        {
            throw new InvalidOperationException($"Duplicate manual section ids: {string.Join(", ", duplicateIds)}");
        }

        string[] actualTools = manifest.Sections.Where(section => !string.IsNullOrWhiteSpace(section.ToolId))
            .Select(section => section.ToolId!).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        string[] expectedTools = ExpectedToolIds.OrderBy(value => value, StringComparer.Ordinal).ToArray();
        if (!actualTools.SequenceEqual(expectedTools, StringComparer.Ordinal))
        {
            throw new InvalidOperationException(
                $"Tool coverage mismatch. Expected={string.Join(',', expectedTools)} Actual={string.Join(',', actualTools)}");
        }

        foreach (ManualSection section in manifest.Sections)
        {
            if (!Regex.IsMatch(section.Id, "^[a-z0-9][a-z0-9-]*$"))
            {
                throw new InvalidOperationException($"Invalid section id: {section.Id}");
            }
            ResolveOwnedPath(manualRoot, section.Path);
        }
    }

    private static void ValidateVisuals(
        ManualVisualManifest visuals,
        ManualSourceManifest manual,
        string manualRoot)
    {
        if (visuals.SchemaVersion != 1 || string.IsNullOrWhiteSpace(visuals.CaptureDate) ||
            string.IsNullOrWhiteSpace(visuals.Source))
        {
            throw new InvalidOperationException("Manual visual manifest header is invalid.");
        }

        string[] expectedSections = manual.Sections.Select(section => section.Id)
            .OrderBy(value => value, StringComparer.Ordinal).ToArray();
        string[] actualSections = visuals.Visuals.Select(visual => visual.SectionId)
            .OrderBy(value => value, StringComparer.Ordinal).ToArray();
        if (!actualSections.SequenceEqual(expectedSections, StringComparer.Ordinal))
        {
            throw new InvalidOperationException(
                $"Manual visual coverage mismatch. Expected={string.Join(',', expectedSections)} Actual={string.Join(',', actualSections)}");
        }

        foreach (ManualVisual visual in visuals.Visuals)
        {
            string imagePath = ResolveOwnedPath(manualRoot, visual.Image);
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Manual UI image was not found: {visual.Image}", imagePath);
            }
            if (string.IsNullOrWhiteSpace(visual.Alt) || string.IsNullOrWhiteSpace(visual.Caption) ||
                visual.Callouts.Count == 0)
            {
                throw new InvalidOperationException($"Manual visual content is incomplete: {visual.SectionId}");
            }

            int[] actualNumbers = visual.Callouts.Select(callout => callout.Number).OrderBy(value => value).ToArray();
            int[] expectedNumbers = Enumerable.Range(1, visual.Callouts.Count).ToArray();
            if (!actualNumbers.SequenceEqual(expectedNumbers))
            {
                throw new InvalidOperationException($"Manual callout numbers must be contiguous: {visual.SectionId}");
            }

            foreach (ManualCallout callout in visual.Callouts)
            {
                if (callout.X < 0 || callout.X > 100 || callout.Y < 0 || callout.Y > 100 ||
                    string.IsNullOrWhiteSpace(callout.Label))
                {
                    throw new InvalidOperationException($"Manual callout is invalid: {visual.SectionId} #{callout.Number}");
                }
            }
        }
    }

    private static string BuildHtml(
        ManualSourceManifest manifest,
        ManualVisualManifest visuals,
        string manualRoot,
        ManualLocaleSource locale)
    {
        StringBuilder navigation = new();
        StringBuilder body = new();
        Dictionary<string, ManualVisual> visualBySection = visuals.Visuals
            .ToDictionary(visual => visual.SectionId, StringComparer.OrdinalIgnoreCase);
        foreach (ManualSection section in manifest.Sections)
        {
            string path = ResolveOwnedPath(manualRoot, section.Path);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Manual section was not found: {section.Path}", path);
            }
            string markdown = File.ReadAllText(path, Encoding.UTF8);
            string sectionHtml = Markdown.ToHtml(markdown, MarkdownPipeline);
            sectionHtml = EmbedLocalImages(sectionHtml, Path.GetDirectoryName(path)!);
            string figureHtml = BuildManualFigure(visualBySection[section.Id], manualRoot, locale);
            int headingEnd = sectionHtml.IndexOf("</h1>", StringComparison.OrdinalIgnoreCase);
            sectionHtml = headingEnd >= 0
                ? sectionHtml.Insert(headingEnd + "</h1>".Length, figureHtml)
                : figureHtml + sectionHtml;
            navigation.Append("<a href=\"#").Append(Html(section.Id)).Append("\" data-manual-nav>")
                .Append(Html(section.Title)).AppendLine("</a>");
            body.Append("<section id=\"").Append(Html(section.Id)).Append("\" data-manual-section>")
                .Append(sectionHtml).AppendLine("</section>");
        }

        string document = $$$"""
<!doctype html>
<html lang="{{{Html(locale.Language)}}}" data-openvisionlab-manual-version="{{{Html(manifest.ManualVersion)}}}" data-openvisionlab-manual-language="{{{Html(locale.Language)}}}">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width,initial-scale=1">
  <title>{{{Html(manifest.Title)}}}</title>
  <style>
    :root{color-scheme:dark;--bg:#071113;--panel:#102226;--line:#2a555b;--text:#eef7f8;--muted:#a9c0c4;--accent:#45c5ce;--ok:#69d69a;--warn:#f4c542}
    *{box-sizing:border-box}html{scroll-behavior:smooth}body{margin:0;background:var(--bg);color:var(--text);font:16px/1.65 "Segoe UI","Malgun Gothic",sans-serif}
    header{position:sticky;top:0;z-index:3;padding:14px 20px;background:#0b191c;border-bottom:1px solid var(--line);display:flex;gap:16px;align-items:center}
    header strong{font-size:20px}header span{color:var(--muted);font-size:13px}#manual-search{margin-left:auto;width:min(360px,42vw);padding:9px 12px;border:1px solid var(--line);border-radius:6px;background:#071113;color:var(--text)}
    .layout{display:grid;grid-template-columns:290px minmax(0,1fr);min-height:calc(100vh - 60px)}nav{position:sticky;top:60px;height:calc(100vh - 60px);overflow:auto;padding:16px;background:#0b191c;border-right:1px solid var(--line)}
    nav a{display:block;padding:8px 10px;margin:2px 0;color:#cce9ec;text-decoration:none;border-radius:5px}nav a:hover,nav a:focus{background:#17363b;color:white}
    main{width:min(1040px,100%);padding:18px 42px 80px}section{padding:18px 0 34px;border-bottom:1px solid #17363b}h1{font-size:30px}h2{margin-top:30px;color:#a9f0f4}h3{color:#d5f3f5}code{background:#122b30;padding:2px 5px;border-radius:4px}pre{overflow:auto;background:#061012;padding:14px;border:1px solid var(--line);border-radius:7px}table{border-collapse:collapse;width:100%;display:block;overflow:auto}th,td{border:1px solid var(--line);padding:8px 10px;text-align:left;vertical-align:top}th{background:#16343a}img{max-width:100%;height:auto;border:1px solid var(--line);border-radius:7px}blockquote{margin-left:0;padding:8px 14px;border-left:4px solid var(--accent);background:#102226;color:#d7edef}.hidden{display:none!important}
    .manual-figure{margin:18px 0 28px;padding:12px;background:#0b191c;border:1px solid var(--line);border-radius:9px}.manual-shot{position:relative;line-height:0}.manual-shot img{display:block;width:100%}.manual-callout{position:absolute;transform:translate(-50%,-50%);display:grid;place-items:center;width:34px;height:34px;border:3px solid #fff;border-radius:50%;background:var(--warn);color:#071113;font-weight:800;font-size:18px;line-height:1;box-shadow:0 2px 9px #000}.manual-caption{margin:11px 2px 7px;color:#d7edef}.manual-callout-key{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:8px;margin:8px 0 0;padding:0;list-style:none}.manual-callout-key li{display:flex;gap:8px;align-items:flex-start;padding:8px;background:#102226;border:1px solid #21464c;border-radius:6px}.manual-callout-key b{flex:0 0 25px;display:grid;place-items:center;width:25px;height:25px;border-radius:50%;background:var(--warn);color:#071113;line-height:1}.manual-source{display:block;margin-top:8px;color:var(--muted);font-size:12px}
    @media(max-width:820px){header{flex-wrap:wrap}#manual-search{width:100%;margin-left:0}.layout{display:block}nav{position:relative;top:0;height:auto;max-height:260px;border-right:0;border-bottom:1px solid var(--line)}main{padding:16px 20px 60px}.manual-callout{width:28px;height:28px;font-size:15px;border-width:2px}.manual-callout-key{grid-template-columns:1fr}}
  </style>
</head>
<body>
<header><strong>{{{Html(manifest.Title)}}}</strong><span>Manual {{{Html(manifest.ManualVersion)}}} · OpenVisionLab {{{Html(manifest.ApplicationVersion)}}}</span><input id="manual-search" type="search" placeholder="{{{Html(locale.SearchPlaceholder)}}}" aria-label="{{{Html(locale.SearchPlaceholder)}}}"></header>
<div class="layout"><nav id="manual-nav">{{{navigation}}}</nav><main>{{{body}}}</main></div>
<script>
const search=document.getElementById('manual-search');
search.addEventListener('input',()=>{const q=search.value.trim().toLocaleLowerCase();document.querySelectorAll('[data-manual-section]').forEach(s=>s.classList.toggle('hidden',q!==''&&!s.innerText.toLocaleLowerCase().includes(q)));document.querySelectorAll('[data-manual-nav]').forEach(a=>{const target=document.querySelector(a.getAttribute('href'));a.classList.toggle('hidden',target&&target.classList.contains('hidden'));});});
</script>
</body>
</html>
""";
        if (document.Contains("Moved to canonical location", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Generated manual contains a moved-stub marker.");
        }
        return document
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Replace("\n", "\r\n", StringComparison.Ordinal)
            .TrimEnd() + "\r\n";
    }

    private static string BuildManualFigure(
        ManualVisual visual,
        string manualRoot,
        ManualLocaleSource locale)
    {
        string imagePath = ResolveOwnedPath(manualRoot, visual.Image);
        string extension = Path.GetExtension(imagePath).ToLowerInvariant();
        string mime = extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            _ => throw new InvalidOperationException($"Unsupported manual UI image: {visual.Image}")
        };
        string dataUri = $"data:{mime};base64,{Convert.ToBase64String(File.ReadAllBytes(imagePath))}";
        StringBuilder markers = new();
        StringBuilder key = new();
        foreach (ManualCallout callout in visual.Callouts.OrderBy(item => item.Number))
        {
            string x = callout.X.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
            string y = callout.Y.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
            markers.Append("<span class=\"manual-callout\" style=\"left:").Append(x)
                .Append("%;top:").Append(y).Append("%\" aria-hidden=\"true\">")
                .Append(callout.Number).AppendLine("</span>");
            key.Append("<li><b>").Append(callout.Number).Append("</b><span>")
                .Append(Html(callout.Label)).AppendLine("</span></li>");
        }

        return new StringBuilder()
            .Append("<figure class=\"manual-figure\" data-manual-figure><div class=\"manual-shot\">")
            .Append("<img src=\"").Append(dataUri).Append("\" alt=\"").Append(Html(visual.Alt)).Append("\">")
            .Append(markers).Append("</div><figcaption><p class=\"manual-caption\">")
            .Append(Html(visual.Caption)).Append("</p><ol class=\"manual-callout-key\">")
            .Append(key).Append("</ol><small class=\"manual-source\">")
            .Append(Html(locale.UiCaptureSourceLabel)).Append(" · ")
            .Append(Html(visual.Image)).AppendLine("</small></figcaption></figure>")
            .ToString();
    }

    private static string EmbedLocalImages(string html, string sourceDirectory)
    {
        return Regex.Replace(html, "src=\"(?<src>[^\"]+)\"", match =>
        {
            string src = match.Groups["src"].Value;
            if (src.StartsWith("data:", StringComparison.OrdinalIgnoreCase) ||
                src.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                src.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return match.Value;
            }
            string imagePath = Path.GetFullPath(Path.Combine(sourceDirectory, Uri.UnescapeDataString(src)));
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Manual image was not found: {src}", imagePath);
            }
            string mime = Path.GetExtension(imagePath).ToLowerInvariant() switch
            {
                ".png" => "image/png", ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif", ".svg" => "image/svg+xml",
                _ => throw new InvalidOperationException($"Unsupported manual image: {src}")
            };
            return $"src=\"data:{mime};base64,{Convert.ToBase64String(File.ReadAllBytes(imagePath))}\"";
        }, RegexOptions.IgnoreCase);
    }

    private static string ResolveOwnedPath(string root, string relativePath)
    {
        string rootPrefix = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        string fullPath = Path.GetFullPath(Path.Combine(root, relativePath));
        if (!fullPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Manual path escapes docs/manual: {relativePath}");
        }
        return fullPath;
    }

    private static void WriteAtomic(string path, string contents)
    {
        string temporaryPath = path + ".tmp";
        File.WriteAllText(temporaryPath, contents, new UTF8Encoding(false));
        File.Move(temporaryPath, path, true);
    }

    private static string Html(string value) => System.Net.WebUtility.HtmlEncode(value);

    private sealed record ManualSourceManifest(
        int SchemaVersion,
        string ManualVersion,
        string ApplicationVersion,
        string Title,
        List<ManualSection> Sections);
    private sealed record ManualSection(string Id, string Title, string Path, string Kind, string? ToolId);
    private sealed record ManualVisualManifest(
        int SchemaVersion,
        string CaptureDate,
        string Source,
        List<ManualVisual> Visuals);
    private sealed record ManualVisual(
        string SectionId,
        string Image,
        string Alt,
        string Caption,
        List<ManualCallout> Callouts);
    private sealed record ManualCallout(int Number, double X, double Y, string Label);
    private sealed record ManualLocaleSource(
        string Language,
        string ManifestFile,
        string VisualsFile,
        string OutputFile,
        string SearchPlaceholder,
        string UiCaptureSourceLabel);
    private sealed record GuideManifest(
        int SchemaVersion,
        string ManualVersion,
        string ApplicationVersion,
        List<GuideManualEntry> Manuals);
    private sealed record GuideManualEntry(string Language, string File, string Sha256);
}
