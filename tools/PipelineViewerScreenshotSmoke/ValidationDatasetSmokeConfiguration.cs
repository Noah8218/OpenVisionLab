using System;
using System.Globalization;
using System.IO;

internal sealed class ValidationDatasetSmokeConfiguration
{
    private ValidationDatasetSmokeConfiguration(
        string datasetRoot,
        string okFolder,
        string ngFolder,
        string templatePath,
        string pipelinePath,
        string pipelineName,
        string pipelineXml,
        string suiteName,
        string boundary,
        int maximumPerRole,
        bool usesDefaultMatchingBaseline)
    {
        DatasetRoot = datasetRoot;
        OkFolder = okFolder;
        NgFolder = ngFolder;
        TemplatePath = templatePath;
        PipelinePath = pipelinePath;
        PipelineName = pipelineName;
        PipelineXml = pipelineXml;
        SuiteName = suiteName;
        Boundary = boundary;
        MaximumPerRole = maximumPerRole;
        UsesDefaultMatchingBaseline = usesDefaultMatchingBaseline;
    }

    internal string DatasetRoot { get; }

    internal string OkFolder { get; }

    internal string NgFolder { get; }

    internal string TemplatePath { get; }

    internal string PipelinePath { get; }

    internal string PipelineName { get; }

    internal string PipelineXml { get; }

    internal string SuiteName { get; }

    internal string Boundary { get; }

    internal int MaximumPerRole { get; }

    internal bool UsesDefaultMatchingBaseline { get; }

    internal static ValidationDatasetSmokeConfiguration LoadFromEnvironment(string repositoryRoot)
    {
        return Create(
            repositoryRoot,
            Environment.GetEnvironmentVariable("OPENVISIONLAB_VALIDATION_DATASET_ROOT"),
            Environment.GetEnvironmentVariable("OPENVISIONLAB_VALIDATION_PIPELINE_PATH"),
            Environment.GetEnvironmentVariable("OPENVISIONLAB_VALIDATION_PIPELINE_NAME"),
            Environment.GetEnvironmentVariable("OPENVISIONLAB_VALIDATION_SUITE_NAME"),
            Environment.GetEnvironmentVariable("OPENVISIONLAB_VALIDATION_BOUNDARY"),
            Environment.GetEnvironmentVariable("OPENVISIONLAB_VALIDATION_TEMPLATE_PATH"),
            Environment.GetEnvironmentVariable("OPENVISIONLAB_VALIDATION_MAX_PER_ROLE"));
    }

    internal static ValidationDatasetSmokeConfiguration Create(
        string repositoryRoot,
        string? datasetRoot,
        string? sourcePipelinePath,
        string? requestedPipelineName,
        string? suiteName,
        string? boundary,
        string? templatePath,
        string? maximumPerRoleText)
    {
        datasetRoot ??= string.Empty;
        sourcePipelinePath ??= string.Empty;
        requestedPipelineName ??= string.Empty;
        suiteName ??= string.Empty;
        boundary ??= string.Empty;
        templatePath ??= string.Empty;
        int maximumPerRole = int.TryParse(
            maximumPerRoleText,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out int requestedMaximum)
            ? Math.Max(1, requestedMaximum)
            : int.MaxValue;
        if (string.IsNullOrWhiteSpace(datasetRoot) || !Directory.Exists(datasetRoot))
        {
            throw new DirectoryNotFoundException(
                "Set OPENVISIONLAB_VALIDATION_DATASET_ROOT to a folder containing all_images\\OK and all_images\\NG.");
        }

        string okFolder = Directory.Exists(Path.Combine(datasetRoot, "OK"))
            ? Path.Combine(datasetRoot, "OK")
            : Path.Combine(datasetRoot, "all_images", "OK");
        string ngFolder = Directory.Exists(Path.Combine(datasetRoot, "NG"))
            ? Path.Combine(datasetRoot, "NG")
            : Path.Combine(datasetRoot, "all_images", "NG");
        if (!Directory.Exists(okFolder) || !Directory.Exists(ngFolder))
        {
            throw new DirectoryNotFoundException("Dataset does not contain OK and NG folders: " + datasetRoot);
        }

        bool usesDefaultMatchingBaseline = string.IsNullOrWhiteSpace(sourcePipelinePath);
        if (usesDefaultMatchingBaseline && string.IsNullOrWhiteSpace(templatePath))
        {
            templatePath = Path.Combine(repositoryRoot, "bin", "Debug", "EasyMatch", "Die Pad Model 1.bmp");
        }

        if (usesDefaultMatchingBaseline && !File.Exists(templatePath))
        {
            throw new FileNotFoundException("Validation Matching template was not found.", templatePath);
        }

        if (usesDefaultMatchingBaseline)
        {
            sourcePipelinePath = Path.Combine(repositoryRoot, "docs", "samples", "public", "Public_Matching_DiePad.pipeline.xml");
        }

        if (!File.Exists(sourcePipelinePath))
        {
            throw new FileNotFoundException("Validation pipeline was not found.", sourcePipelinePath);
        }

        string pipelineName = string.IsNullOrWhiteSpace(requestedPipelineName)
            ? usesDefaultMatchingBaseline
                ? "DiePad500_Matching_Baseline"
                : Path.GetFileNameWithoutExtension(sourcePipelinePath)
            : requestedPipelineName.Trim();
        string pipelineXml = File.ReadAllText(sourcePipelinePath);
        if (usesDefaultMatchingBaseline)
        {
            pipelineXml = pipelineXml
                .Replace("<Name>Public_Matching_DiePad</Name>", "<Name>" + pipelineName + "</Name>", StringComparison.Ordinal)
                .Replace("<Value>Public_Matching_DiePad</Value>", "<Value>" + pipelineName + "</Value>", StringComparison.Ordinal)
                .Replace(
                    "docs\\samples\\public\\templates\\Matching_DiePad_Synthetic_Template.png",
                    templatePath,
                    StringComparison.OrdinalIgnoreCase)
                .Replace(
                    "docs/samples/public/templates/Matching_DiePad_Synthetic_Template.png",
                    templatePath,
                    StringComparison.OrdinalIgnoreCase);
        }

        if (string.IsNullOrWhiteSpace(suiteName))
        {
            suiteName = usesDefaultMatchingBaseline ? "Die Pad 500 Matching baseline" : pipelineName;
        }

        if (string.IsNullOrWhiteSpace(boundary))
        {
            boundary = usesDefaultMatchingBaseline
                ? "Matching-only baseline; not a tuned multi-tool defect recipe or field qualification."
                : "Caller-supplied validation pipeline; interpret only within the supplied dataset split and gate.";
        }

        return new ValidationDatasetSmokeConfiguration(
            datasetRoot,
            okFolder,
            ngFolder,
            templatePath,
            sourcePipelinePath,
            pipelineName,
            pipelineXml,
            suiteName,
            boundary,
            maximumPerRole,
            usesDefaultMatchingBaseline);
    }
}
