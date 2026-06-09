using System.IO;
using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Linq;
using System.Xml.Serialization;

string outputDirectory = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "bin", "Debug"));

string appAssemblyPath = Path.Combine(outputDirectory, "OpenVisionLab.dll");
string recipeRootDirectory = args.Length > 1
    ? Path.GetFullPath(args[1])
    : Path.Combine(outputDirectory, "RECIPE");
if (!File.Exists(appAssemblyPath))
{
    Console.Error.WriteLine($"OpenVisionLab.dll was not found: {appAssemblyPath}");
    return 2;
}

AssemblyLoadContext.Default.Resolving += (_, assemblyName) =>
{
    string candidate = Path.Combine(outputDirectory, $"{assemblyName.Name}.dll");
    return File.Exists(candidate) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(candidate) : null;
};

Assembly appAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(appAssemblyPath);

var checks = new[]
{
    new XmlRootCheck("OpenVisionLab.DataState", "CData"),
    new XmlRootCheck("OpenVisionLab.BlobProperty", "CPropertyBlob",
        ExpectedValues: new[] { new ExpectedValue("MIN_AREA", "200"), new ExpectedValue("MAX_AREA", "1000000") }),
    new XmlRootCheck("OpenVisionLab.ContourProperty", "CPropertyContour"),
    new XmlRootCheck("OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty", "CPropertyFeatureMatching"),
    new XmlRootCheck("OpenVisionLab.LineGaugeProperty", "CPropertyLineGuage"),
    new XmlRootCheck("OpenVisionLab.MatchingProperty", "CPropertyMatching"),
    new XmlRootCheck("OpenVisionLab.MeanProperty", "CPropertyMean"),
    new XmlRootCheck("OpenVisionLab.Vision._1._Tools.OpenCV.VisionProperty", "CPropertyVision",
        ExpectedValues: new[] { new ExpectedValue("Alpha", "30"), new ExpectedValue("Beta", "255") }),
    new XmlRootCheck("OpenVisionLab.ParameterProperty", "CPropertyParam",
        ExpectedValues: new[] { new ExpectedValue("NAME", "TEST"), new ExpectedValue("SpecSize", "10") }),
    new XmlRootCheck("OpenVisionLab.SpecProperty", "CPropertySpec",
        ExpectedValues: new[] { new ExpectedValue("DIST_MIN_MM", "0.5"), new ExpectedValue("DIST_MAX_MM", "2") }),
    new XmlRootCheck("OpenVisionLab.SystemStateConfig", "SYSTEM",
        """<?xml version="1.0" encoding="utf-8"?><SYSTEM><LastRecipe>SETUP001</LastRecipe><LastRecipeUpdateTime /></SYSTEM>""",
        new[] { new ExpectedValue("LastRecipe", "SETUP001") }),
    new XmlRootCheck("OpenVisionLab.ImageViewProperty", "PROPERTY",
        """<?xml version="1.0" encoding="utf-8"?><PROPERTY><ROWS>10</ROWS><COLUMNS>10</COLUMNS></PROPERTY>""",
        new[] { new ExpectedValue("ROWS", "10"), new ExpectedValue("COLUMNS", "10") }),
};

var failures = new List<string>();
foreach (XmlRootCheck check in checks)
{
    Type? type = appAssembly.GetType(check.TypeName);
    if (type == null)
    {
        failures.Add($"{check.TypeName}: type not found");
        continue;
    }

    try
    {
        string xml = check.SampleXml ?? $"""<?xml version="1.0" encoding="utf-8"?><{check.RootName} />""";
        using StringReader reader = new StringReader(xml);
        object? instance = new XmlSerializer(type).Deserialize(reader);
        if (instance == null)
        {
            failures.Add($"{check.TypeName}: deserialized instance was null");
            continue;
        }

        ValidateExpectedValues(check, instance, "deserialize", failures);

        object? reloaded = SaveAndReload(type, instance);
        if (reloaded == null)
        {
            failures.Add($"{check.TypeName}: saved XML reload returned null");
            continue;
        }

        ValidateExpectedValues(check, reloaded, "save-reload", failures);
    }
    catch (Exception exception)
    {
        failures.Add($"{check.TypeName} ({check.RootName}): {exception.GetBaseException().Message}");
    }
}

int recipeXmlFileCount = 0;
if (Directory.Exists(recipeRootDirectory))
{
    recipeXmlFileCount = ValidateRecipeXmlFiles(recipeRootDirectory, checks, appAssembly, failures);
}

if (failures.Count > 0)
{
    Console.Error.WriteLine("Recipe XML compatibility check failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine($"- {failure}");
    }

    return 1;
}

Console.WriteLine(recipeXmlFileCount > 0
    ? $"Recipe XML compatibility check passed for {checks.Length} XML roots and {recipeXmlFileCount} recipe XML files."
    : $"Recipe XML compatibility check passed for {checks.Length} XML roots.");
return 0;

static object? SaveAndReload(Type type, object instance)
{
    string tempPath = Path.Combine(Path.GetTempPath(), $"OpenVisionLabXmlCheck_{Guid.NewGuid():N}.xml");
    try
    {
        XmlSerializer serializer = new XmlSerializer(type);
        using (FileStream writeStream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            serializer.Serialize(writeStream, instance);
        }

        using FileStream readStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return serializer.Deserialize(readStream);
    }
    finally
    {
        if (File.Exists(tempPath))
        {
            File.Delete(tempPath);
        }
    }
}

static int ValidateRecipeXmlFiles(string recipeRootDirectory, IReadOnlyList<XmlRootCheck> checks, Assembly appAssembly, List<string> failures)
{
    int checkedFileCount = 0;
    Dictionary<string, List<XmlRootCheck>> checksByRoot = checks
        .GroupBy(check => check.RootName)
        .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);

    foreach (string xmlPath in Directory.EnumerateFiles(recipeRootDirectory, "*.xml", SearchOption.AllDirectories))
    {
        string? rootName = ReadRootName(xmlPath);
        if (rootName == null || !checksByRoot.TryGetValue(rootName, out List<XmlRootCheck>? candidates))
        {
            continue;
        }

        checkedFileCount++;
        IReadOnlyList<XmlRootCheck> fileCandidates = SelectCandidatesForRecipeFile(xmlPath, candidates);
        if (!CanDeserializeWithAnyCandidate(xmlPath, fileCandidates, appAssembly, out string error))
        {
            failures.Add($"{xmlPath}: {error}");
        }
    }

    return checkedFileCount;
}

static IReadOnlyList<XmlRootCheck> SelectCandidatesForRecipeFile(string xmlPath, IReadOnlyList<XmlRootCheck> candidates)
{
    string fileName = Path.GetFileNameWithoutExtension(xmlPath);
    string? expectedTypeName = fileName switch
    {
        "VISION" => "OpenVisionLab.DataState",
        "VisionPara" => "OpenVisionLab.Vision._1._Tools.OpenCV.VisionProperty",
        _ when fileName.StartsWith("Blob_", StringComparison.OrdinalIgnoreCase) => "OpenVisionLab.BlobProperty",
        _ when fileName.StartsWith("Contour_", StringComparison.OrdinalIgnoreCase) => "OpenVisionLab.ContourProperty",
        _ when fileName.StartsWith("Feature_", StringComparison.OrdinalIgnoreCase) => "OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty",
        _ when fileName.StartsWith("Line(", StringComparison.OrdinalIgnoreCase) => "OpenVisionLab.LineGaugeProperty",
        _ when fileName.StartsWith("Matching_", StringComparison.OrdinalIgnoreCase) => "OpenVisionLab.MatchingProperty",
        _ => null
    };

    if (expectedTypeName == null)
    {
        return candidates;
    }

    List<XmlRootCheck> matched = candidates
        .Where(candidate => candidate.TypeName == expectedTypeName)
        .ToList();

    return matched.Count > 0 ? matched : candidates;
}

static string? ReadRootName(string xmlPath)
{
    try
    {
        return XDocument.Load(xmlPath).Root?.Name.LocalName;
    }
    catch
    {
        return null;
    }
}

static bool CanDeserializeWithAnyCandidate(string xmlPath, IReadOnlyList<XmlRootCheck> candidates, Assembly appAssembly, out string error)
{
    List<string> errors = new List<string>();
    foreach (XmlRootCheck candidate in candidates)
    {
        Type? type = appAssembly.GetType(candidate.TypeName);
        if (type == null)
        {
            errors.Add($"{candidate.TypeName}: type not found");
            continue;
        }

        try
        {
            using FileStream stream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            object? instance = new XmlSerializer(type).Deserialize(stream);
            if (instance != null)
            {
                error = "";
                return true;
            }

            errors.Add($"{candidate.TypeName}: deserialized instance was null");
        }
        catch (Exception exception)
        {
            errors.Add($"{candidate.TypeName}: {exception.GetBaseException().Message}");
        }
    }

    error = string.Join("; ", errors);
    return false;
}

static void ValidateExpectedValues(XmlRootCheck check, object instance, string phase, List<string> failures)
{
    foreach (ExpectedValue expectedValue in check.ExpectedValues ?? Array.Empty<ExpectedValue>())
    {
        object? actualValue = ReadMemberPath(instance, expectedValue.MemberPath);
        string actualText = Convert.ToString(actualValue, CultureInfo.InvariantCulture) ?? "";
        if (!string.Equals(actualText, expectedValue.ExpectedText, StringComparison.Ordinal))
        {
            failures.Add($"{check.TypeName}.{expectedValue.MemberPath} ({phase}): expected {expectedValue.ExpectedText}, actual {actualText}");
        }
    }
}

static object? ReadMemberPath(object instance, string memberPath)
{
    object? current = instance;
    foreach (string memberName in memberPath.Split('.'))
    {
        if (current == null)
        {
            return null;
        }

        Type type = current.GetType();
        PropertyInfo? property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
        if (property != null)
        {
            current = property.GetValue(current);
            continue;
        }

        FieldInfo? field = type.GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            current = field.GetValue(current);
            continue;
        }

        return null;
    }

    return current;
}

internal sealed record XmlRootCheck(
    string TypeName,
    string RootName,
    string? SampleXml = null,
    IReadOnlyList<ExpectedValue>? ExpectedValues = null);

internal sealed record ExpectedValue(string MemberPath, string ExpectedText);
