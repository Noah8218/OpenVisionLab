using System.IO;
using System.Collections;
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
Assembly? libOpenCvAssembly = LoadOptionalAssembly(outputDirectory, "Lib.OpenCV.dll");
Type? openCvPropertyBaseType = appAssembly.GetType("OpenVisionLab.Vision._1._Tools.OpenCV.OpenCvPropertyBase");

var checks = new[]
{
    new XmlRootCheck("OpenVisionLab.DataState", "CData"),
    new XmlRootCheck("OpenVisionLab.BlobProperty", "CPropertyBlob",
        ExpectedValues: new[] { new ExpectedValue("MIN_AREA", "200"), new ExpectedValue("MAX_AREA", "1000000") }),
    new XmlRootCheck("OpenVisionLab.ContourProperty", "CPropertyContour",
        """<?xml version="1.0" encoding="utf-8"?><CPropertyContour><NAME>Contour</NAME><PIXELPERMM>0.006</PIXELPERMM><USE_THRESHOLD>true</USE_THRESHOLD><USE_BITWISENOT>false</USE_BITWISENOT><THRESHOLD_TYPES>Binary</THRESHOLD_TYPES><THRESHOLD>127</THRESHOLD><USE_ADAPTIVE_THRESHOLD>false</USE_ADAPTIVE_THRESHOLD><ADAPTIVE_THRESHOLD>0</ADAPTIVE_THRESHOLD><ADAPTIVE_THRESHOLD_TYPES>Binary</ADAPTIVE_THRESHOLD_TYPES><ADAPTIVE_THRESHOLD_ALGORITHM>GaussianC</ADAPTIVE_THRESHOLD_ALGORITHM><BlockSize>25</BlockSize><Weight>5</Weight><USE_ROI>true</USE_ROI><USE_MULTI_ROI>false</USE_MULTI_ROI><CvROI><X>10</X><Y>20</Y><Width>640</Width><Height>480</Height><Top>20</Top><Left>10</Left><Location><X>10</X><Y>20</Y></Location><Size><Width>640</Width><Height>480</Height></Size></CvROI><CvROIS /><CvMASKS /><USE_APPROXPOLYDP>false</USE_APPROXPOLYDP><USE_DRAW_IMAGE>true</USE_DRAW_IMAGE><ApproximationModes>ApproxSimple</ApproximationModes><DetectMode>List</DetectMode><EPSILON>0.01</EPSILON><MIN_AREA>200</MIN_AREA><MAX_AREA>1000000</MAX_AREA><DrawThickness>2</DrawThickness><ClrGridHtml>#0000ff</ClrGridHtml></CPropertyContour>""",
        new[]
        {
            new ExpectedValue("CvROI.X", "10"),
            new ExpectedValue("CvROI.Y", "20"),
            new ExpectedValue("CvROI.Width", "640"),
            new ExpectedValue("CvROI.Height", "480"),
            new ExpectedValue("MIN_AREA", "200"),
            new ExpectedValue("MAX_AREA", "1000000")
        }),
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
    new XmlRootCheck("Lib.OpenCV.Pipeline.VisionPipeline", "VisionPipeline",
        """<?xml version="1.0" encoding="utf-8"?><VisionPipeline><Name>Inspection</Name><Steps><Step><Name>Threshold1</Name><ToolType>Threshold</ToolType><InputLayer>Main</InputLayer><OutputLayer>Binary</OutputLayer><Parameters><Parameter><Key>Threshold</Key><Value>127</Value></Parameter><Parameter><Key>ThresholdType</Key><Value>Binary</Value></Parameter></Parameters></Step></Steps></VisionPipeline>""",
        new[]
        {
            new ExpectedValue("Name", "Inspection"),
            new ExpectedValue("Steps.Count", "1"),
            new ExpectedValue("Steps.0.Parameters.Threshold", "127"),
            new ExpectedValue("Steps.0.Parameters.ThresholdType", "Binary")
        }),
};

var failures = new List<string>();
foreach (XmlRootCheck check in checks)
{
    Type? type = ResolveCheckType(check.TypeName, appAssembly, libOpenCvAssembly);
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

        if (openCvPropertyBaseType != null && openCvPropertyBaseType.IsAssignableFrom(type))
        {
            object? helperReloaded = SaveWithSerializeHelperAsBase(appAssembly, openCvPropertyBaseType, type, instance);
            if (helperReloaded == null)
            {
                failures.Add($"{check.TypeName}: SerializeHelper base-typed save reload returned null");
                continue;
            }

            ValidateExpectedValues(check, helperReloaded, "serialize-helper-base-save", failures);
        }
    }
    catch (Exception exception)
    {
        failures.Add($"{check.TypeName} ({check.RootName}): {exception.GetBaseException().Message}");
    }
}

ValidatePipelineStepBuilder(appAssembly, libOpenCvAssembly, failures);
ValidatePipelineNormalizer(appAssembly, libOpenCvAssembly, failures);

int recipeXmlFileCount = 0;
if (Directory.Exists(recipeRootDirectory))
{
    recipeXmlFileCount = ValidateRecipeXmlFiles(recipeRootDirectory, checks, appAssembly, libOpenCvAssembly, failures);
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

static Assembly? LoadOptionalAssembly(string outputDirectory, string assemblyFileName)
{
    string assemblyPath = Path.Combine(outputDirectory, assemblyFileName);
    return File.Exists(assemblyPath) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath) : null;
}

static Type? ResolveCheckType(string typeName, params Assembly?[] assemblies)
{
    foreach (Assembly? assembly in assemblies)
    {
        Type? type = assembly?.GetType(typeName);
        if (type != null)
        {
            return type;
        }
    }

    return Type.GetType(typeName);
}

static void ValidatePipelineStepBuilder(Assembly appAssembly, Assembly? libOpenCvAssembly, List<string> failures)
{
    Type? builderType = appAssembly.GetType("OpenVisionLab.VisionPipelineStepBuilder");
    if (builderType == null)
    {
        failures.Add("OpenVisionLab.VisionPipelineStepBuilder: type not found");
        return;
    }

    try
    {
        ValidateBlobPipelineStepBuilder(appAssembly, builderType, failures);
        ValidateThresholdPipelineStepBuilder(libOpenCvAssembly, builderType, failures);
    }
    catch (Exception exception)
    {
        failures.Add($"VisionPipelineStepBuilder: {exception.GetBaseException().Message}");
    }
}

static void ValidateBlobPipelineStepBuilder(Assembly appAssembly, Type builderType, List<string> failures)
{
    Type? blobType = appAssembly.GetType("OpenVisionLab.BlobProperty");
    if (blobType == null)
    {
        failures.Add("OpenVisionLab.BlobProperty: type not found for pipeline step builder check");
        return;
    }

    object? blob = Activator.CreateInstance(blobType, "Blob_1");
    blobType.GetProperty("MIN_AREA")?.SetValue(blob, 321);
    blobType.GetProperty("MAX_AREA")?.SetValue(blob, 654321);
    blobType.GetProperty("THRESHOLD")?.SetValue(blob, 123D);

    MethodInfo? method = builderType.GetMethod("FromProperty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    object? step = method?.Invoke(null, new[] { blob, "Main", "BlobOut" });
    if (step == null)
    {
        failures.Add("VisionPipelineStepBuilder.FromProperty: returned null");
        return;
    }

    ValidateBuilderValue(step, "ToolType", "Blob", "Blob builder", failures);
    ValidateBuilderValue(step, "InputLayer", "Main", "Blob builder", failures);
    ValidateBuilderValue(step, "OutputLayer", "BlobOut", "Blob builder", failures);
    ValidateBuilderValue(step, "Parameters.MIN_AREA", "321", "Blob builder", failures);
    ValidateBuilderValue(step, "Parameters.MAX_AREA", "654321", "Blob builder", failures);
    ValidateBuilderValue(step, "Parameters.THRESHOLD", "123", "Blob builder", failures);
}

static void ValidateThresholdPipelineStepBuilder(Assembly? libOpenCvAssembly, Type builderType, List<string> failures)
{
    Type? thresholdType = libOpenCvAssembly?.GetType("Lib.OpenCV.Property.ThresholdToolProperty");
    if (thresholdType == null)
    {
        failures.Add("Lib.OpenCV.Property.ThresholdToolProperty: type not found for pipeline step builder check");
        return;
    }

    object? threshold = Activator.CreateInstance(thresholdType);
    thresholdType.GetProperty("Threshold")?.SetValue(threshold, 77D);

    MethodInfo? method = builderType.GetMethod("FromThresholdProperty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    object? step = method?.Invoke(null, new[] { threshold, "Threshold1", "Main", "Binary" });
    if (step == null)
    {
        failures.Add("VisionPipelineStepBuilder.FromThresholdProperty: returned null");
        return;
    }

    ValidateBuilderValue(step, "ToolType", "Threshold", "Threshold builder", failures);
    ValidateBuilderValue(step, "Parameters.Threshold", "77", "Threshold builder", failures);
}

static void ValidatePipelineNormalizer(Assembly appAssembly, Assembly? libOpenCvAssembly, List<string> failures)
{
    Type? pipelineType = ResolveCheckType("Lib.OpenCV.Pipeline.VisionPipeline", appAssembly, libOpenCvAssembly);
    Type? stepType = ResolveCheckType("Lib.OpenCV.Pipeline.VisionPipelineStep", appAssembly, libOpenCvAssembly);
    Type? normalizerType = appAssembly.GetType("OpenVisionLab.VisionPipelineNormalizer");
    MethodInfo? normalizeMethod = normalizerType?
        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        .FirstOrDefault(method => method.Name == "NormalizeChainedInspectionPreprocessing"
            && method.GetParameters().Length == 1);

    if (pipelineType == null || stepType == null || normalizeMethod == null)
    {
        failures.Add("VisionPipelineNormalizer: required type or method was not found");
        return;
    }

    try
    {
        object pipeline = Activator.CreateInstance(pipelineType)
            ?? throw new InvalidOperationException("VisionPipeline instance could not be created.");
        pipelineType.GetProperty("Name")?.SetValue(pipeline, "NormalizerCheck");

        object? steps = pipelineType.GetProperty("Steps")?.GetValue(pipeline);
        MethodInfo? addMethod = steps?.GetType().GetMethod("Add");
        if (steps == null || addMethod == null)
        {
            failures.Add("VisionPipelineNormalizer: Steps collection could not be accessed");
            return;
        }

        object threshold = CreatePipelineStepForNormalizer(stepType, "01 Binary", "Threshold", "Main", "Binary", null);
        object morphology = CreatePipelineStepForNormalizer(stepType, "02 Clean", "Morphology", "Binary", "Clean", null);
        object contour = CreatePipelineStepForNormalizer(
            stepType,
            "03 Contour",
            "Contour",
            "Clean",
            "ContourOut",
            new Dictionary<string, string>
            {
                ["USE_THRESHOLD"] = "True",
                ["USE_ADAPTIVE_THRESHOLD"] = "True",
                ["USE_BITWISENOT"] = "True"
            });

        addMethod.Invoke(steps, new[] { threshold });
        addMethod.Invoke(steps, new[] { morphology });
        addMethod.Invoke(steps, new[] { contour });

        object? changes = normalizeMethod.Invoke(null, new[] { pipeline });
        int changeCount = CountEnumerable(changes);
        if (changeCount != 1)
        {
            failures.Add($"VisionPipelineNormalizer: expected 1 change, actual {changeCount}");
        }

        ValidateStepParameter(contour, "USE_THRESHOLD", "false", "VisionPipelineNormalizer", failures);
        ValidateStepParameter(contour, "USE_ADAPTIVE_THRESHOLD", "false", "VisionPipelineNormalizer", failures);
        ValidateStepParameter(contour, "USE_BITWISENOT", "false", "VisionPipelineNormalizer", failures);
    }
    catch (Exception exception)
    {
        failures.Add($"VisionPipelineNormalizer: {exception.GetBaseException().Message}");
    }
}

static object CreatePipelineStepForNormalizer(
    Type stepType,
    string name,
    string toolType,
    string inputLayer,
    string outputLayer,
    IDictionary<string, string>? parameters)
{
    object step = Activator.CreateInstance(stepType)
        ?? throw new InvalidOperationException("VisionPipelineStep instance could not be created.");
    stepType.GetProperty("Name")?.SetValue(step, name);
    stepType.GetProperty("ToolType")?.SetValue(step, toolType);
    stepType.GetProperty("Enabled")?.SetValue(step, true);
    stepType.GetProperty("InputLayer")?.SetValue(step, inputLayer);
    stepType.GetProperty("OutputLayer")?.SetValue(step, outputLayer);

    if (parameters != null && stepType.GetProperty("Parameters")?.GetValue(step) is IDictionary parameterBag)
    {
        foreach (KeyValuePair<string, string> parameter in parameters)
        {
            parameterBag[parameter.Key] = parameter.Value;
        }
    }

    return step;
}

static int CountEnumerable(object? value)
{
    if (value is ICollection collection)
    {
        return collection.Count;
    }

    if (value is IEnumerable enumerable)
    {
        int count = 0;
        foreach (object _ in enumerable)
        {
            count++;
        }

        return count;
    }

    return -1;
}

static void ValidateStepParameter(object step, string key, string expected, string checkName, List<string> failures)
{
    object? parameterBag = step.GetType().GetProperty("Parameters")?.GetValue(step);
    string actual = string.Empty;
    if (parameterBag is IDictionary dictionary && dictionary.Contains(key))
    {
        actual = Convert.ToString(dictionary[key], CultureInfo.InvariantCulture) ?? string.Empty;
    }

    if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
    {
        failures.Add($"{checkName}.{key}: expected {expected}, actual {actual}");
    }
}

static void ValidateBuilderValue(object step, string memberPath, string expected, string checkName, List<string> failures)
{
    string actual = Convert.ToString(ReadMemberPath(step, memberPath), CultureInfo.InvariantCulture) ?? string.Empty;
    if (!string.Equals(actual, expected, StringComparison.Ordinal))
    {
        failures.Add($"{checkName}.{memberPath}: expected {expected}, actual {actual}");
    }
}

static object? SaveWithSerializeHelperAsBase(Assembly appAssembly, Type baseType, Type actualType, object instance)
{
    Type? helperType = appAssembly.GetType("OpenVisionLab.SerializeHelper");
    MethodInfo? saveMethodDefinition = helperType?
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .FirstOrDefault(method => method.Name == "SaveXmlFile" && method.IsGenericMethodDefinition);

    if (saveMethodDefinition == null)
    {
        throw new InvalidOperationException("SerializeHelper.SaveXmlFile<T> was not found.");
    }

    string tempPath = Path.Combine(Path.GetTempPath(), $"OpenVisionLabXmlCheck_{Guid.NewGuid():N}.xml");
    try
    {
        MethodInfo saveMethod = saveMethodDefinition.MakeGenericMethod(baseType);
        saveMethod.Invoke(null, new[] { tempPath, instance });

        using FileStream readStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new XmlSerializer(actualType).Deserialize(readStream);
    }
    finally
    {
        if (File.Exists(tempPath))
        {
            File.Delete(tempPath);
        }
    }
}

static int ValidateRecipeXmlFiles(
    string recipeRootDirectory,
    IReadOnlyList<XmlRootCheck> checks,
    Assembly appAssembly,
    Assembly? libOpenCvAssembly,
    List<string> failures)
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
        if (!CanDeserializeWithAnyCandidate(xmlPath, fileCandidates, appAssembly, libOpenCvAssembly, out string error))
        {
            failures.Add($"{xmlPath}: {error}");
            continue;
        }

        if (string.Equals(rootName, "VisionPipeline", StringComparison.Ordinal)
            && !ValidateVisionPipelineSemantics(xmlPath, appAssembly, libOpenCvAssembly, out string validationError))
        {
            failures.Add($"{xmlPath}: {validationError}");
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

static bool CanDeserializeWithAnyCandidate(
    string xmlPath,
    IReadOnlyList<XmlRootCheck> candidates,
    Assembly appAssembly,
    Assembly? libOpenCvAssembly,
    out string error)
{
    List<string> errors = new List<string>();
    foreach (XmlRootCheck candidate in candidates)
    {
        Type? type = ResolveCheckType(candidate.TypeName, appAssembly, libOpenCvAssembly);
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

static bool ValidateVisionPipelineSemantics(
    string xmlPath,
    Assembly appAssembly,
    Assembly? libOpenCvAssembly,
    out string error)
{
    error = string.Empty;
    Type? pipelineType = ResolveCheckType("Lib.OpenCV.Pipeline.VisionPipeline", appAssembly, libOpenCvAssembly);
    Type? validatorType = appAssembly.GetType("OpenVisionLab.VisionPipelineValidator");
    MethodInfo? validateMethod = validatorType?.GetMethod(
        "Validate",
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    if (pipelineType == null || validateMethod == null)
    {
        return true;
    }

    try
    {
        using FileStream stream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        object? pipeline = new XmlSerializer(pipelineType).Deserialize(stream);
        if (pipeline == null)
        {
            error = "VisionPipeline semantic validation failed: deserialized pipeline was null.";
            return false;
        }

        object? validation = validateMethod.Invoke(null, new object[] { pipeline, new[] { "Main" } });
        bool success = validation?.GetType().GetProperty("Success")?.GetValue(validation) is bool value && value;
        if (success)
        {
            return true;
        }

        MethodInfo? formatErrorsMethod = validation?.GetType().GetMethod("FormatErrors", BindingFlags.Public | BindingFlags.Instance);
        string details = Convert.ToString(formatErrorsMethod?.Invoke(validation, null), CultureInfo.InvariantCulture) ?? string.Empty;
        error = string.IsNullOrWhiteSpace(details)
            ? "VisionPipeline semantic validation failed."
            : $"VisionPipeline semantic validation failed: {details}";
        return false;
    }
    catch (Exception exception)
    {
        error = $"VisionPipeline semantic validation failed: {exception.GetBaseException().Message}";
        return false;
    }
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

        if (current is IList list && int.TryParse(memberName, out int index))
        {
            current = index >= 0 && index < list.Count ? list[index] : null;
            continue;
        }

        if (current is IDictionary<string, string> stringDictionary && stringDictionary.TryGetValue(memberName, out string? stringValue))
        {
            current = stringValue;
            continue;
        }

        if (current is IDictionary dictionary && dictionary.Contains(memberName))
        {
            current = dictionary[memberName];
            continue;
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
