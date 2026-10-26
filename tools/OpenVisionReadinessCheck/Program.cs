using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace OpenVisionReadinessCheck;

internal static class Program
{
    private static readonly List<string> Failures = new();

    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        // Preserve the reflection-free interface used by distribution verification.
        if (args.Length > 0 && args[0] == "--read-assembly-metadata")
            return AssemblyMetadataReader.Run(args.Skip(1).ToArray());

        string root = args.Length > 0 ? Path.GetFullPath(args[0]) : FindRepoRoot(AppContext.BaseDirectory);
        if (root == null || !Directory.Exists(root))
        {
            Console.Error.WriteLine("NG | Repository root was not found.");
            return 1;
        }

        // Public build inputs belong to Original. Internal regression, architecture
        // and screenshot checks are preserved and executed in Dev.
        Check("Public build inputs", () => CheckBuildInputs(root));
        Check("Public recipe schema", () => CheckRecipeSchema(root));
        Check("Localized offline manuals", () => CheckManuals(root));
        Check("Public sample catalogs", () =>
        {
            foreach (string name in new[] { "OpenVisionLab.PublicSampleCatalog.csv", "OpenVisionLab.ProductSampleCatalog.csv" })
                if (File.ReadLines(RequiredFile(root, "docs/samples/" + name)).Take(2).Count() != 2)
                    throw new InvalidDataException("Sample catalog has no data rows: " + name);
        });
        foreach (string failure in Failures) Console.Error.WriteLine("NG | " + failure);
        Console.WriteLine(Failures.Count == 0
            ? "OpenVisionLab public source readiness passed."
            : "OpenVisionLab public source readiness failed.");
        return Failures.Count == 0 ? 0 : 1;
    }

    private static void CheckBuildInputs(string root)
    {
        foreach (string path in new[]
        {
            "OpenVisionLab.sln", "global.json", "Directory.Build.props", "LICENSE", "NOTICE",
            "README.md", "README.ko.md", "docs/runbooks/BUILD.md", ".github/workflows/ci.yml",
            "docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json",
            "dll/OpenVisionLab-Vision-SDK/sdk-manifest.json",
            "tools/VerifySourceBuild.ps1", "tools/VerifyReleaseCandidate.ps1",
            "tools/TestExternalReferences.ps1", "tools/TestThirdPartyNoticeCoverage.ps1",
            "tools/TestPublicSampleAssets.ps1", "tools/TestDocumentationIndex.ps1",
            "tools/RunVisionSampleCatalog.ps1", "tools/BuildCleanRuntime.ps1", "tools/TestReleaseDistribution.ps1",
            "tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj",
            "tools/WindowsSandbox/InvokeSourceBuildSandbox.ps1", "tools/WindowsSandbox/RunSourceBuildSandbox.ps1"
        }) RequiredFile(root, path);

        using (JsonDocument sdk = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "global.json"))))
            if (!Version.TryParse(sdk.RootElement.GetProperty("sdk").GetProperty("version").GetString(), out _))
                throw new InvalidDataException("global.json SDK version is invalid.");

        string solution = File.ReadAllText(Path.Combine(root, "OpenVisionLab.sln"));
        MatchCollection projects = Regex.Matches(solution, @"Project\([^\r\n]+?= ""[^""]+"", ""(?<path>[^""]+\.csproj)""");
        if (projects.Count == 0) throw new InvalidDataException("Solution has no C# projects.");
        foreach (Match project in projects)
        {
            string projectPath = RequiredFile(root, project.Groups["path"].Value);
            XDocument document = XDocument.Load(projectPath);
            foreach (XElement reference in document.Descendants("ProjectReference"))
            {
                string include = reference.Attribute("Include")?.Value;
                include = include?.Replace("$(OpenVisionLabRepoRoot)", root + Path.DirectorySeparatorChar, StringComparison.Ordinal);
                if (string.IsNullOrWhiteSpace(include)) throw new InvalidDataException("Empty ProjectReference: " + projectPath);
                RequiredFile(root, Path.GetRelativePath(root, Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath)!, include))));
            }
        }
        XDocument app = XDocument.Load(RequiredFile(root, "src/OpenVisionLab/OpenVisionLab.csproj"));
        if (app.Descendants("UseWPF").FirstOrDefault()?.Value != "true" ||
            app.Descendants("OutputType").FirstOrDefault()?.Value != "WinExe" ||
            app.Descendants("TargetFramework").FirstOrDefault()?.Value != "net8.0-windows7.0" ||
            !app.Descendants("PlatformTarget").Any(item => item.Value == "x64"))
            throw new InvalidDataException("The application must remain a .NET 8 Windows x64 WPF executable.");
        if (string.IsNullOrWhiteSpace(app.Descendants("Version").FirstOrDefault()?.Value))
            throw new InvalidDataException("Product version is missing.");
    }

    private static void CheckRecipeSchema(string root)
    {
        string canonical = RequiredFile(root, "docs/contracts/vision/schemas/VISION_PIPELINE_RECIPE_SCHEMA.xsd");
        XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
        using XmlReader reader = XmlReader.Create(canonical, settings);
        XmlSchemaSet schemas = new() { XmlResolver = null };
        schemas.ValidationEventHandler += (_, args) => throw new InvalidDataException(args.Message);
        schemas.Add(null, reader);
        schemas.Compile();
        if (schemas.Count == 0) throw new InvalidDataException("Recipe schema is empty.");
    }

    private static void CheckManuals(string root)
    {
        string folder = Path.Combine(root, "docs/manual/generated");
        using JsonDocument manifest = JsonDocument.Parse(File.ReadAllText(RequiredFile(root, "docs/manual/generated/guide-manifest.json")));
        JsonElement data = manifest.RootElement;
        if (data.GetProperty("schemaVersion").GetInt32() != 2)
            throw new InvalidDataException("Unsupported Guide manifest schema.");
        string version = XDocument.Load(Path.Combine(root, "src/OpenVisionLab/OpenVisionLab.csproj"))
            .Descendants("Version").First().Value;
        if (data.GetProperty("applicationVersion").GetString() != version)
            throw new InvalidDataException("Guide manifest and product versions differ.");

        HashSet<string> languages = new(StringComparer.Ordinal);
        foreach (JsonElement manual in data.GetProperty("manuals").EnumerateArray())
        {
            string language = manual.GetProperty("language").GetString();
            if ((language != "ko" && language != "en") || !languages.Add(language))
                throw new InvalidDataException("Unsupported or duplicate Guide language.");
            string file = manual.GetProperty("file").GetString();
            if (file != $"OpenVisionLab_User_Manual.{language}.html")
                throw new InvalidDataException("Unexpected Guide filename for " + language);
            string path = RequiredFile(root, Path.GetRelativePath(root, Path.Combine(folder, file)));
            string hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
            if (!string.Equals(hash, manual.GetProperty("sha256").GetString(), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Guide SHA-256 mismatch: " + file);
            if (!Regex.IsMatch(File.ReadAllText(path), @"<html\b[^>]*\blang=[""']" + language + @"[""']", RegexOptions.IgnoreCase))
                throw new InvalidDataException("Guide HTML language mismatch: " + file);
        }
        if (!languages.SetEquals(new[] { "ko", "en" }))
            throw new InvalidDataException("Both Korean and English Guides are required.");
    }

    private static string RequiredFile(string root, string relative)
    {
        string prefix = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        string path = Path.GetFullPath(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)));
        if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
            throw new FileNotFoundException("Required public input is absent or outside the checkout: " + relative);
        if (new FileInfo(path).Length == 0) throw new InvalidDataException("Required public input is empty: " + relative);
        return path;
    }

    private static void Check(string name, Action action)
    {
        try { action(); Console.WriteLine("OK | " + name); }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException or XmlException or XmlSchemaException or JsonException or InvalidOperationException or KeyNotFoundException or FormatException or ArgumentException)
        { Failures.Add(name + ": " + ex.Message); }
    }

    private static string FindRepoRoot(string start)
    {
        for (DirectoryInfo current = new(start); current != null; current = current.Parent)
            if (File.Exists(Path.Combine(current.FullName, "OpenVisionLab.sln"))) return current.FullName;
        return null;
    }
}
