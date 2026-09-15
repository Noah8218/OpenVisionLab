using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

internal static class PipelineXmlRoundTripCompatibilityContract
{
    private static readonly string[] Cultures = { "ko-KR", "en-US", "de-DE" };
    private static readonly string[] BlobDefaultKeys =
    {
        "Name",
        "PIXELPERMM",
        "USE_THRESHOLD",
        "USE_BITWISENOT",
        "THRESHOLD_TYPES",
        "THRESHOLD",
        "USE_ADAPTIVE_THRESHOLD",
        "ADAPTIVE_THRESHOLD",
        "ADAPTIVE_THRESHOLD_TYPES",
        "ADAPTIVE_THRESHOLD_ALGORITHM",
        "BlockSize",
        "Weight",
        "USE_ROI",
        "USE_MULTI_ROI",
        "USE_MASKING",
        "MIN_AREA",
        "MAX_AREA",
        "MIN_WIDTH",
        "MAX_WIDTH",
        "MIN_HEIGHT",
        "MAX_HEIGHT"
    };

    public static int Run(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        RunCase(
            "ko-KR/en-US/de-DE save-reopen keeps typed fields, parameters, unit, locale text, and validation",
            () => CheckCultureRoundTrip(outputDirectory),
            passed,
            failed);
        RunCase(
            "UTF-16 XML with Korean identity and template path reopens without semantic loss",
            () => CheckUtf16RoundTrip(outputDirectory),
            passed,
            failed);
        RunCase(
            "omitted Blob defaults match explicit default parameters through the existing mapper",
            () => CheckOmittedDefaultsRoundTrip(outputDirectory),
            passed,
            failed);
        RunCase(
            "comma-decimal and invalid enum values fail invariant validation under de-DE",
            () => CheckInvalidCultureValues(outputDirectory),
            passed,
            failed);

        string reportPath = Path.Combine(outputDirectory, "pipeline-xml-roundtrip-compatibility-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failed.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-018 current/default/unit/locale Pipeline XML semantic round-trip",
                "Owner: SerializeHelper + VisionPipelineStepBuilder/VisionPipelineAppToolFactory + VisionPipelineValidator",
                "Cultures: ko-KR, en-US, de-DE",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|pipeline-xml-roundtrip-compatibility|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void CheckCultureRoundTrip(string outputDirectory)
    {
        VisionPipeline baseline = CreateRepresentativePipeline();
        string expectedProjection = ProjectPipeline(baseline);
        ValidationSnapshot expectedValidation = CaptureValidation(baseline);
        Require(expectedValidation.Success, "Representative Pipeline is not valid before round-trip: " + expectedValidation.Errors);
        byte[]? referenceBytes = null;

        foreach (string cultureName in Cultures)
        {
            using CultureScope _ = new CultureScope(cultureName);
            string path = Path.Combine(outputDirectory, "culture-" + cultureName + ".pipeline.xml");
            Require(SerializeHelper.SaveXmlFile(path, baseline), "Could not save the representative XML under " + cultureName + ".");
            byte[] bytes = File.ReadAllBytes(path);
            referenceBytes ??= bytes;
            Require(referenceBytes.SequenceEqual(bytes), "XML bytes changed with CurrentCulture=" + cultureName + ".");
            Require(SerializeHelper.TryLoadFromXmlFile(path, out VisionPipeline loaded),
                "Representative XML could not be reopened under " + cultureName + ".");
            Require(string.Equals(expectedProjection, ProjectPipeline(loaded), StringComparison.Ordinal),
                "Pipeline semantic projection changed under " + cultureName + ".");
            ValidationSnapshot actualValidation = CaptureValidation(loaded);
            Require(expectedValidation.Equals(actualValidation),
                "Pipeline validation outcome changed under " + cultureName + ".");
            Require(loaded.Steps[0].Parameters.TryGetValue("TemplatePath", out string? templatePath)
                && string.Equals(templatePath, "C:\\테스트\\템플릿.png", StringComparison.Ordinal),
                "TemplatePath was not preserved under " + cultureName + ".");
        }
    }

    private static void CheckUtf16RoundTrip(string outputDirectory)
    {
        VisionPipeline baseline = CreateRepresentativePipeline();
        byte[] utf16Bytes;
        using (MemoryStream stream = new MemoryStream())
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode, 1024, leaveOpen: true))
            {
                new XmlSerializer(typeof(VisionPipeline)).Serialize(writer, baseline);
            }

            utf16Bytes = stream.ToArray();
        }
        string utf16Path = Path.Combine(outputDirectory, "한글-utf16.pipeline.xml");
        File.WriteAllBytes(utf16Path, utf16Bytes);

        using CultureScope _ = new CultureScope("de-DE");
        Require(SerializeHelper.TryLoadFromXmlFile(utf16Path, out VisionPipeline loaded),
            "UTF-16 Pipeline XML could not be reopened under de-DE.");
        Require(string.Equals(ProjectPipeline(baseline), ProjectPipeline(loaded), StringComparison.Ordinal),
            "UTF-16 Pipeline semantic projection changed after reopen.");
        Require(loaded.Name.Contains("한글", StringComparison.Ordinal), "UTF-16 Korean Pipeline name was lost.");
    }

    private static void CheckOmittedDefaultsRoundTrip(string outputDirectory)
    {
        VisionPipelineStep omitted = new VisionPipelineStep
        {
            Name = "Omitted Blob",
            ToolType = "Blob",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "BlobOut"
        };
        VisionPipelineStep explicitDefaults = new VisionPipelineStep
        {
            Name = "Explicit Blob",
            ToolType = "Blob",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "BlobOut"
        };

        object omittedProperty = VisionPipelineStepPropertyMapper.CreateProperty(omitted)
            ?? throw new InvalidOperationException("The existing PropertyGrid mapper returned no Blob property.");
        foreach (string key in BlobDefaultKeys)
        {
            string propertyName = string.Equals(key, "Name", StringComparison.Ordinal)
                ? "NAME"
                : key;
            object value = ReadMember(omittedProperty, propertyName);
            explicitDefaults.Parameters[key] = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        VisionPipeline pipeline = new VisionPipeline { Name = "Defaults" };
        pipeline.Steps.Add(omitted);
        pipeline.Steps.Add(explicitDefaults);
        string path = Path.Combine(outputDirectory, "omitted-defaults.pipeline.xml");
        using CultureScope _ = new CultureScope("de-DE");
        Require(SerializeHelper.SaveXmlFile(path, pipeline), "Omitted/default fixture could not be saved.");
        Require(SerializeHelper.TryLoadFromXmlFile(path, out VisionPipeline loaded),
            "Omitted/default fixture could not be reopened.");

        object reloadedOmittedProperty = VisionPipelineStepPropertyMapper.CreateProperty(loaded.Steps[0])
            ?? throw new InvalidOperationException("The existing PropertyGrid mapper returned no reloaded omitted Blob property.");
        object reloadedExplicitProperty = VisionPipelineStepPropertyMapper.CreateProperty(loaded.Steps[1])
            ?? throw new InvalidOperationException("The existing PropertyGrid mapper returned no reloaded explicit Blob property.");
        foreach (string propertyName in BlobDefaultKeys.Select(key => string.Equals(key, "Name", StringComparison.Ordinal) ? "NAME" : key))
        {
            string omittedValue = FormatValue(ReadMember(reloadedOmittedProperty, propertyName));
            string explicitValue = FormatValue(ReadMember(reloadedExplicitProperty, propertyName));
            Require(string.Equals(omittedValue, explicitValue, StringComparison.Ordinal),
                $"Omitted/default mismatch for {propertyName}: {omittedValue} vs {explicitValue}.");
        }

        Require(loaded.Steps[0].Parameters.Count == 0,
            "The omitted/default round-trip rewrote omitted parameters unexpectedly.");
        Require(loaded.Steps[1].Parameters.ContainsKey("MIN_AREA"),
            "The explicit/default round-trip lost an authored parameter.");
    }

    private static void CheckInvalidCultureValues(string outputDirectory)
    {
        VisionPipeline baseline = CreateRepresentativePipeline();
        string baselineXmlPath = Path.Combine(outputDirectory, "invalid-culture-source.pipeline.xml");
        Require(SerializeHelper.SaveXmlFile(baselineXmlPath, baseline), "Could not create invalid-value source fixture.");
        XDocument source = XDocument.Load(baselineXmlPath);
        XElement thresholdValue = source.Descendants("Parameter")
            .Single(element => string.Equals(element.Element("Key")?.Value, "Threshold", StringComparison.Ordinal))
            .Element("Value")!;
        XElement thresholdTypeValue = source.Descendants("Parameter")
            .Single(element => string.Equals(element.Element("Key")?.Value, "ThresholdType", StringComparison.Ordinal))
            .Element("Value")!;

        using CultureScope cultureScope = new CultureScope("de-DE");
        thresholdValue.Value = "127,5";
        string invalidNumberXml = source.ToString(SaveOptions.DisableFormatting);
        string invalidNumberError;
        Require(SerializeHelper.TryLoadFromXmlText(invalidNumberXml, out VisionPipeline invalidNumber, out invalidNumberError),
            "Invalid culture separator should remain inspectable as source text. " + invalidNumberError);
        VisionPipelineValidationResult numberValidation = VisionPipelineValidator.Validate(invalidNumber, new[] { "Main" });
        Require(numberValidation.Errors.Any(error => error.Contains("Threshold", StringComparison.Ordinal)
            && error.Contains("numeric", StringComparison.OrdinalIgnoreCase)),
            "Comma-decimal Threshold was not rejected by invariant validation.");

        thresholdValue.Value = "125.0";
        thresholdTypeValue.Value = "NotAnEnum";
        string invalidEnumXml = source.ToString(SaveOptions.DisableFormatting);
        string invalidEnumError;
        Require(SerializeHelper.TryLoadFromXmlText(invalidEnumXml, out VisionPipeline invalidEnum, out invalidEnumError),
            "Invalid enum should remain inspectable as source text. " + invalidEnumError);
        VisionPipelineValidationResult enumValidation = VisionPipelineValidator.Validate(invalidEnum, new[] { "Main" });
        Require(enumValidation.Errors.Any(error => error.Contains("ThresholdType", StringComparison.Ordinal)
            && error.Contains("expects one of", StringComparison.OrdinalIgnoreCase)),
            "Invalid ThresholdType was not rejected by the shared validator.");

        using Mat sourceImage = new Mat(16, 16, MatType.CV_8UC1, Scalar.White);
        bool runBlocked = false;
        try
        {
            using VisionRecipeRunResult runResult = new VisionRecipeRunner().RunAsync(invalidEnum, sourceImage)
                .GetAwaiter()
                .GetResult();
        }
        catch (VisionPipelineValidationException)
        {
            runBlocked = true;
        }

        Require(runBlocked, "Invalid enum Pipeline was not blocked before execution.");
    }

    private static VisionPipeline CreateRepresentativePipeline()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "한글 기본값·단위" };
        VisionPipelineStep threshold = new VisionPipelineStep
        {
            Name = "한글 Threshold",
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "ThresholdOut",
            UseAcceptance = true,
            ExpectedSuccess = true,
            MaxElapsedMilliseconds = 1000,
            AcceptanceMetricName = "ResultCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 0D,
            UseAcceptanceMetricMaximum = true,
            AcceptanceMetricMaximum = 255D
        };
        threshold.Parameters["Mode"] = "Threshold";
        threshold.Parameters["Threshold"] = "1.25E+2";
        threshold.Parameters["MaxValue"] = "255";
        threshold.Parameters["ThresholdType"] = "Binary";
        threshold.Parameters["PIXELPERMM"] = "0.006";
        threshold.Parameters["USE_ROI"] = "false";
        threshold.Parameters["TemplatePath"] = "C:\\테스트\\템플릿.png";
        pipeline.Steps.Add(threshold);

        VisionPipelineStep contour = new VisionPipelineStep
        {
            Name = "Contour unit",
            ToolType = "Contour",
            Enabled = true,
            InputLayer = "ThresholdOut",
            OutputLayer = "ContourOut"
        };
        contour.Parameters["PIXELPERMM"] = "6E-3";
        contour.Parameters["USE_THRESHOLD"] = "false";
        contour.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        contour.Parameters["USE_BITWISENOT"] = "false";
        contour.Parameters["USE_ROI"] = "false";
        contour.Parameters["ApproximationModes"] = "ApproxSimple";
        contour.Parameters["DetectMode"] = "External";
        contour.Parameters["MIN_AREA"] = "200";
        contour.Parameters["MAX_AREA"] = "1000000";
        pipeline.Steps.Add(contour);
        return pipeline;
    }

    private static ValidationSnapshot CaptureValidation(VisionPipeline pipeline)
    {
        VisionPipelineValidationResult result = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
        return new ValidationSnapshot(
            result.Success,
            string.Join("\n", result.Errors),
            string.Join("\n", result.Warnings));
    }

    private static string ProjectPipeline(VisionPipeline pipeline)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(pipeline.Name ?? string.Empty);
        foreach (VisionPipelineStep step in pipeline.Steps)
        {
            builder.AppendLine(string.Join(
                "|",
                step.Name ?? string.Empty,
                step.ToolType ?? string.Empty,
                step.Enabled,
                step.InputLayer ?? string.Empty,
                step.OutputLayer ?? string.Empty,
                step.UseAcceptance,
                step.ExpectedSuccess,
                step.MaxElapsedMilliseconds,
                step.RequiredMessageText ?? string.Empty,
                step.AcceptanceMetricName ?? string.Empty,
                step.UseAcceptanceMetricMinimum,
                step.AcceptanceMetricMinimum.ToString("R", CultureInfo.InvariantCulture),
                step.UseAcceptanceMetricMaximum,
                step.AcceptanceMetricMaximum.ToString("R", CultureInfo.InvariantCulture)));
            foreach (KeyValuePair<string, string> parameter in step.Parameters.OrderBy(item => item.Key, StringComparer.Ordinal))
            {
                builder.AppendLine(parameter.Key + "=" + parameter.Value);
            }
        }

        return builder.ToString();
    }

    private static object ReadMember(object owner, string name)
    {
        PropertyInfo? property = owner.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
        {
            return property.GetValue(owner)!;
        }

        FieldInfo? field = owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            return field.GetValue(owner)!;
        }

        throw new InvalidOperationException($"Mapper property '{name}' was not found on {owner.GetType().FullName}.");
    }

    private static string FormatValue(object value)
    {
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static void RunCase(string name, Action action, List<string> passed, List<string> failed)
    {
        try
        {
            action();
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add(name + ": " + exception.GetBaseException().Message);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed record ValidationSnapshot(bool Success, string Errors, string Warnings);

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo previousCulture = CultureInfo.CurrentCulture;
        private readonly CultureInfo previousUiCulture = CultureInfo.CurrentUICulture;

        public CultureScope(string cultureName)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }
}
