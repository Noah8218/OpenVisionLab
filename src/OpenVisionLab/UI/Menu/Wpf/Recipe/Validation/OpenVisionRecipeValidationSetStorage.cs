using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    [XmlRoot("OpenVisionValidationSets")]
    public sealed class OpenVisionRecipeValidationSetDocument
    {
        [XmlAttribute]
        public int SchemaVersion { get; set; } = 1;

        [XmlElement("Set")]
        public List<OpenVisionRecipeValidationSet> Sets { get; set; } = new List<OpenVisionRecipeValidationSet>();
    }

    public sealed class OpenVisionRecipeValidationSet
    {
        [XmlAttribute]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute]
        public string PipelineName { get; set; } = string.Empty;

        [XmlAttribute]
        public string PipelineDefinitionSha256 { get; set; } = string.Empty;

        [XmlAttribute]
        public string ImageSetSha256 { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        [XmlArray("Dependencies")]
        [XmlArrayItem("Dependency")]
        public List<OpenVisionRecipeValidationSetDependency> Dependencies { get; set; } =
            new List<OpenVisionRecipeValidationSetDependency>();

        [XmlElement("Image")]
        public List<OpenVisionRecipeValidationSetImage> Images { get; set; } = new List<OpenVisionRecipeValidationSetImage>();

        [XmlIgnore]
        public bool IsIdentityLocked => !string.IsNullOrWhiteSpace(PipelineDefinitionSha256);
    }

    public sealed class OpenVisionRecipeValidationSetDependency
    {
        [XmlAttribute]
        public string Path { get; set; } = string.Empty;

        [XmlAttribute]
        public string Sha256 { get; set; } = string.Empty;
    }

    public sealed class OpenVisionRecipeValidationSetImage
    {
        public const string ExpectedOk = "OK";
        public const string ExpectedNg = "NG";

        [XmlAttribute]
        public string Expected { get; set; } = ExpectedOk;

        [XmlAttribute]
        public string Path { get; set; } = string.Empty;

        [XmlAttribute]
        public string Sha256 { get; set; } = string.Empty;

        [XmlAttribute]
        public string VariantId { get; set; } = string.Empty;

        [XmlAttribute]
        public string ExpectedMetricName { get; set; } = string.Empty;

        [XmlAttribute]
        public string ExpectedMetricMinimum { get; set; } = string.Empty;

        [XmlAttribute]
        public string ExpectedMetricMaximum { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        [XmlIgnore]
        public bool IsExpectedNg => string.Equals(Expected, ExpectedNg, StringComparison.OrdinalIgnoreCase);

        [XmlIgnore]
        public bool Exists => !string.IsNullOrWhiteSpace(Path) && File.Exists(Path);
    }

    internal static class OpenVisionRecipeValidationSetStorage
    {
        internal const int CurrentSchemaVersion = 1;
        private const int MaximumSetCount = 64;
        private const int MaximumImageCount = 5000;
        private const string FileName = "validation-sets.xml";
        private static readonly HashSet<string> SupportedImageExtensions = new HashSet<string>(
            new[] { ".bmp", ".jpg", ".jpeg", ".png", ".tif", ".tiff" },
            StringComparer.OrdinalIgnoreCase);

        public static string GetPath(string recipeName)
        {
            RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
            string path = RecipeWorkspaceService.GetContainedStoragePath(
                RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName),
                Path.Combine("VISION", "ValidationSets", FileName),
                "Validation-set storage path");
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            return path;
        }

        public static bool TryLoad(
            string recipeName,
            out OpenVisionRecipeValidationSetDocument document,
            out string error)
        {
            string path = GetPath(recipeName);
            if (!File.Exists(path))
            {
                document = CreateEmpty();
                error = string.Empty;
                return true;
            }

            if (!SerializeHelper.TryLoadFromXmlFile(path, out document) || document == null)
            {
                document = CreateEmpty();
                error = "Validation set XML could not be loaded: " + path;
                return false;
            }

            if (document.SchemaVersion != CurrentSchemaVersion)
            {
                int version = document.SchemaVersion;
                document = CreateEmpty();
                error = "Unsupported validation set schema: " + version;
                return false;
            }

            Normalize(document);
            foreach (OpenVisionRecipeValidationSetImage image in document.Sets
                .SelectMany(set => set.Images ?? new List<OpenVisionRecipeValidationSetImage>()))
            {
                if (!TryValidateVariantContract(image, out string contractError))
                {
                    error = contractError;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool TrySave(
            string recipeName,
            OpenVisionRecipeValidationSetDocument document,
            out string error)
        {
            if (document == null)
            {
                error = "Validation set document is missing.";
                return false;
            }

            Normalize(document);
            foreach (OpenVisionRecipeValidationSetImage image in document.Sets
                .SelectMany(set => set.Images ?? new List<OpenVisionRecipeValidationSetImage>()))
            {
                if (!TryValidateVariantContract(image, out string contractError))
                {
                    error = contractError;
                    return false;
                }
            }

            if (document.Sets.Count > MaximumSetCount)
            {
                error = "Validation set limit exceeded: " + MaximumSetCount;
                return false;
            }

            if (document.Sets.Sum(set => set.Images.Count) > MaximumImageCount)
            {
                error = "Validation image limit exceeded: " + MaximumImageCount;
                return false;
            }

            try
            {
                document.SchemaVersion = CurrentSchemaVersion;
                SerializeHelper.SaveXmlFile(GetPath(recipeName), document);
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }
        }

        public static bool IsValidSetName(string name)
        {
            string value = name?.Trim() ?? string.Empty;
            return value.Length > 0
                && value.Length <= 80
                && !value.Any(char.IsControl);
        }

        public static int AddOrUpdateImages(
            OpenVisionRecipeValidationSet set,
            IEnumerable<string> paths,
            string expected,
            string notes,
            out int updatedCount,
            out int skippedCount)
        {
            return AddOrUpdateImages(
                set,
                paths,
                expected,
                notes,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                out updatedCount,
                out skippedCount);
        }

        public static int AddOrUpdateImages(
            OpenVisionRecipeValidationSet set,
            IEnumerable<string> paths,
            string expected,
            string notes,
            string variantId,
            string expectedMetricName,
            string expectedMetricMinimum,
            string expectedMetricMaximum,
            out int updatedCount,
            out int skippedCount)
        {
            updatedCount = 0;
            skippedCount = 0;
            if (set == null || set.IsIdentityLocked)
            {
                return 0;
            }

            set.Images ??= new List<OpenVisionRecipeValidationSetImage>();
            string normalizedExpected = NormalizeExpected(expected);
            string normalizedNotes = notes?.Trim() ?? string.Empty;
            string normalizedVariantId = NormalizeVariantId(variantId);
            string normalizedMetricName = expectedMetricName?.Trim() ?? string.Empty;
            string normalizedMetricMinimum = expectedMetricMinimum?.Trim() ?? string.Empty;
            string normalizedMetricMaximum = expectedMetricMaximum?.Trim() ?? string.Empty;
            OpenVisionRecipeValidationSetImage contractProbe = new OpenVisionRecipeValidationSetImage
            {
                VariantId = normalizedVariantId,
                ExpectedMetricName = normalizedMetricName,
                ExpectedMetricMinimum = normalizedMetricMinimum,
                ExpectedMetricMaximum = normalizedMetricMaximum
            };
            if (!TryValidateVariantContract(contractProbe, out _))
            {
                skippedCount = (paths ?? Enumerable.Empty<string>()).Count();
                return 0;
            }

            int addedCount = 0;

            foreach (string sourcePath in paths ?? Enumerable.Empty<string>())
            {
                if (!TryNormalizeImagePath(sourcePath, requireExisting: true, out string imagePath))
                {
                    skippedCount++;
                    continue;
                }

                OpenVisionRecipeValidationSetImage existing = set.Images.FirstOrDefault(item =>
                    string.Equals(item.Path, imagePath, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    existing.Expected = normalizedExpected;
                    existing.Notes = normalizedNotes;
                    CopyVariantContract(contractProbe, existing);
                    updatedCount++;
                    continue;
                }

                if (set.Images.Count >= MaximumImageCount)
                {
                    skippedCount++;
                    continue;
                }

                set.Images.Add(new OpenVisionRecipeValidationSetImage
                {
                    Expected = normalizedExpected,
                    Path = imagePath,
                    Notes = normalizedNotes,
                    VariantId = normalizedVariantId,
                    ExpectedMetricName = normalizedMetricName,
                    ExpectedMetricMinimum = normalizedMetricMinimum,
                    ExpectedMetricMaximum = normalizedMetricMaximum
                });
                addedCount++;
            }

            return addedCount;
        }

        public static bool TryApplyVariantContract(
            OpenVisionRecipeValidationSet set,
            OpenVisionRecipeValidationSetImage image,
            string variantId,
            string expectedMetricName,
            string expectedMetricMinimum,
            string expectedMetricMaximum,
            out string error)
        {
            if (set?.IsIdentityLocked == true)
            {
                error = "Hash-locked validation images cannot be edited.";
                return false;
            }

            if (set?.Images == null
                || image == null
                || !set.Images.Any(item => ReferenceEquals(item, image)))
            {
                error = "The selected validation image is no longer available.";
                return false;
            }

            OpenVisionRecipeValidationSetImage normalized = new OpenVisionRecipeValidationSetImage
            {
                VariantId = NormalizeVariantId(variantId),
                ExpectedMetricName = expectedMetricName?.Trim() ?? string.Empty,
                ExpectedMetricMinimum = expectedMetricMinimum?.Trim() ?? string.Empty,
                ExpectedMetricMaximum = expectedMetricMaximum?.Trim() ?? string.Empty
            };
            if (!TryValidateVariantContract(normalized, out error))
            {
                return false;
            }

            CopyVariantContract(normalized, image);
            error = string.Empty;
            return true;
        }

        public static bool TryValidateVariantContract(
            OpenVisionRecipeValidationSetImage image,
            out string error)
        {
            string variantId = NormalizeVariantId(image?.VariantId);
            if (variantId.Length > 80 || variantId.Any(char.IsControl))
            {
                error = "Variant ID must be 80 characters or fewer and contain no control characters.";
                return false;
            }

            string metricNamesText = image?.ExpectedMetricName?.Trim() ?? string.Empty;
            string minimumsText = image?.ExpectedMetricMinimum?.Trim() ?? string.Empty;
            string maximumsText = image?.ExpectedMetricMaximum?.Trim() ?? string.Empty;
            if (metricNamesText.Length > 500 || metricNamesText.Any(char.IsControl))
            {
                error = "Expected metric names must be 500 characters or fewer and contain no control characters.";
                return false;
            }

            string[] metricNames = SplitMetricContractParts(metricNamesText);
            string[] minimums = SplitMetricContractParts(minimumsText);
            string[] maximums = SplitMetricContractParts(maximumsText);
            if (metricNames.Length == 0)
            {
                if (minimums.Length > 0 || maximums.Length > 0)
                {
                    error = "Expected metric name is required when a minimum or maximum is entered.";
                    return false;
                }

                error = string.Empty;
                return true;
            }

            if (!IsMetricContractPartCountValid(minimums, metricNames.Length)
                || !IsMetricContractPartCountValid(maximums, metricNames.Length))
            {
                error = "Expected metric bounds must contain either one value or one value per metric.";
                return false;
            }

            for (int index = 0; index < metricNames.Length; index++)
            {
                string metricName = metricNames[index];
                if (string.IsNullOrWhiteSpace(metricName)
                    || metricName.Length > 100
                    || metricName.Any(char.IsControl))
                {
                    error = "Each expected metric name must be 100 characters or fewer and contain no control characters.";
                    return false;
                }

                string minimum = ResolveMetricContractPart(minimums, index);
                string maximum = ResolveMetricContractPart(maximums, index);
                bool hasMinimum = !string.IsNullOrWhiteSpace(minimum);
                bool hasMaximum = !string.IsNullOrWhiteSpace(maximum);
                if (!hasMinimum && !hasMaximum)
                {
                    error = "At least one expected metric bound is required for " + metricName + ".";
                    return false;
                }

                double minimumValue = double.NaN;
                double maximumValue = double.NaN;
                if (hasMinimum && !TryParseFinite(minimum, out minimumValue))
                {
                    error = "Expected metric minimum must be a finite number for " + metricName + ".";
                    return false;
                }

                if (hasMaximum && !TryParseFinite(maximum, out maximumValue))
                {
                    error = "Expected metric maximum must be a finite number for " + metricName + ".";
                    return false;
                }

                if (hasMinimum && hasMaximum && minimumValue > maximumValue)
                {
                    error = "Expected metric minimum cannot exceed the maximum for " + metricName + ".";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static string NormalizeVariantId(string value)
        {
            return value?.Trim() ?? string.Empty;
        }

        public static string GetVariantDisplayId(OpenVisionRecipeValidationSetImage image)
        {
            string value = NormalizeVariantId(image?.VariantId);
            return string.IsNullOrWhiteSpace(value) ? "Default" : value;
        }

        public static string BuildExpectedMetricText(OpenVisionRecipeValidationSetImage image)
        {
            if (string.IsNullOrWhiteSpace(image?.ExpectedMetricName))
            {
                return string.Empty;
            }

            string[] metricNames = SplitMetricContractParts(image.ExpectedMetricName);
            string[] minimums = SplitMetricContractParts(image.ExpectedMetricMinimum);
            string[] maximums = SplitMetricContractParts(image.ExpectedMetricMaximum);
            return string.Join(
                "; ",
                metricNames.Select((metricName, index) =>
                    metricName
                    + " ["
                    + (string.IsNullOrWhiteSpace(ResolveMetricContractPart(minimums, index))
                        ? "-∞"
                        : ResolveMetricContractPart(minimums, index))
                    + ".."
                    + (string.IsNullOrWhiteSpace(ResolveMetricContractPart(maximums, index))
                        ? "+∞"
                        : ResolveMetricContractPart(maximums, index))
                    + "]"));
        }

        public static bool TryRepairMissingImagePath(
            OpenVisionRecipeValidationSet set,
            OpenVisionRecipeValidationSetImage image,
            string replacementPath,
            out string repairedPath,
            out string error)
        {
            repairedPath = string.Empty;
            error = string.Empty;
            if (set?.IsIdentityLocked == true)
            {
                error = "Hash-locked validation images cannot be repaired.";
                return false;
            }

            if (set?.Images == null || image == null || !set.Images.Any(item => ReferenceEquals(item, image)))
            {
                error = "The selected validation image is no longer available.";
                return false;
            }

            if (image.Exists)
            {
                error = "Only a missing validation image can be repaired.";
                return false;
            }

            if (!TryNormalizeImagePath(replacementPath, requireExisting: true, out string normalizedReplacementPath))
            {
                error = "Select an existing supported image file.";
                return false;
            }

            if (set.Images.Any(item => !ReferenceEquals(item, image)
                && string.Equals(item?.Path, normalizedReplacementPath, StringComparison.OrdinalIgnoreCase)))
            {
                error = "The replacement image is already registered in this validation set.";
                return false;
            }

            repairedPath = normalizedReplacementPath;
            image.Path = normalizedReplacementPath;
            return true;
        }

        public static bool TryGetTopLevelImagePaths(
            string folderPath,
            out IReadOnlyList<string> paths,
            out string error)
        {
            paths = Array.Empty<string>();
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                error = "Validation image folder is missing.";
                return false;
            }

            string directory;
            try
            {
                directory = Path.GetFullPath(folderPath.Trim());
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                error = ex.GetBaseException().Message;
                return false;
            }

            if (!Directory.Exists(directory))
            {
                error = "Validation image folder does not exist: " + directory;
                return false;
            }

            try
            {
                List<string> imagePaths = new List<string>();
                foreach (string candidate in Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly))
                {
                    if (!TryNormalizeImagePath(candidate, requireExisting: true, out string imagePath))
                    {
                        continue;
                    }

                    imagePaths.Add(imagePath);
                    if (imagePaths.Count > MaximumImageCount)
                    {
                        error = "Validation image folder limit exceeded: " + MaximumImageCount;
                        return false;
                    }
                }

                paths = imagePaths
                    .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(path => path, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                return true;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
            {
                paths = Array.Empty<string>();
                error = ex.GetBaseException().Message;
                return false;
            }
        }

        public static OpenVisionRecipeValidationSetDocument CreateEmpty()
        {
            return new OpenVisionRecipeValidationSetDocument
            {
                SchemaVersion = CurrentSchemaVersion
            };
        }

        public static string ComputeFileSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            return Convert.ToHexString(SHA256.HashData(stream));
        }

        public static string ComputeTextSha256(string value)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty)));
        }

        public static string ComputeImageSetSha256(IEnumerable<OpenVisionRecipeValidationSetImage> images)
        {
            StringBuilder canonical = new StringBuilder();
            int index = 0;
            foreach (OpenVisionRecipeValidationSetImage image in images ?? Enumerable.Empty<OpenVisionRecipeValidationSetImage>())
            {
                canonical
                    .Append(index++.ToString("D6", System.Globalization.CultureInfo.InvariantCulture))
                    .Append('|')
                    .Append(Path.GetFullPath(image?.Path ?? string.Empty))
                    .Append('|')
                    .Append((image?.Sha256 ?? string.Empty).Trim().ToUpperInvariant())
                    .Append('|')
                    .Append(GetVariantDisplayId(image))
                    .Append('|')
                    .Append((image?.ExpectedMetricName ?? string.Empty).Trim())
                    .Append('|')
                    .Append((image?.ExpectedMetricMinimum ?? string.Empty).Trim())
                    .Append('|')
                    .Append((image?.ExpectedMetricMaximum ?? string.Empty).Trim())
                    .AppendLine();
            }

            return ComputeTextSha256(canonical.ToString());
        }

        public static bool TryValidateFrozenIdentity(
            OpenVisionRecipeValidationSet set,
            string pipelineName,
            string pipelineXml,
            out string error)
        {
            error = string.Empty;
            if (set == null || !set.IsIdentityLocked)
            {
                return true;
            }

            if (!string.Equals(set.PipelineName, pipelineName, StringComparison.Ordinal))
            {
                error = $"Frozen validation Pipeline mismatch. Expected '{set.PipelineName}', actual '{pipelineName}'.";
                return false;
            }

            string definitionSha256 = ComputeTextSha256(pipelineXml);
            if (!string.Equals(
                    set.PipelineDefinitionSha256,
                    definitionSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                error = "Frozen validation Pipeline definition SHA-256 mismatch.";
                return false;
            }

            foreach (OpenVisionRecipeValidationSetDependency dependency in
                set.Dependencies ?? Enumerable.Empty<OpenVisionRecipeValidationSetDependency>())
            {
                if (string.IsNullOrWhiteSpace(dependency?.Path)
                    || !File.Exists(dependency.Path)
                    || !IsFileSha256Match(dependency.Path, dependency.Sha256))
                {
                    error = "Frozen validation dependency SHA-256 mismatch: " + (dependency?.Path ?? string.Empty);
                    return false;
                }
            }

            foreach (OpenVisionRecipeValidationSetImage image in
                set.Images ?? Enumerable.Empty<OpenVisionRecipeValidationSetImage>())
            {
                if (string.IsNullOrWhiteSpace(image?.Sha256))
                {
                    error = "Frozen validation image SHA-256 is missing: " + (image?.Path ?? string.Empty);
                    return false;
                }

                if (!image.Exists || !IsFileSha256Match(image.Path, image.Sha256))
                {
                    error = "Frozen validation image SHA-256 mismatch: " + (image?.Path ?? string.Empty);
                    return false;
                }
            }

            string imageSetSha256 = ComputeImageSetSha256(set.Images);
            if (!string.Equals(set.ImageSetSha256, imageSetSha256, StringComparison.OrdinalIgnoreCase))
            {
                error = "Frozen validation image-set SHA-256 mismatch.";
                return false;
            }

            return true;
        }

        private static void Normalize(OpenVisionRecipeValidationSetDocument document)
        {
            document.Sets ??= new List<OpenVisionRecipeValidationSet>();
            List<OpenVisionRecipeValidationSet> normalizedSets = new List<OpenVisionRecipeValidationSet>();
            HashSet<string> setNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (OpenVisionRecipeValidationSet set in document.Sets.Where(item => item != null))
            {
                string name = set.Name?.Trim() ?? string.Empty;
                if (!IsValidSetName(name) || !setNames.Add(name))
                {
                    continue;
                }

                set.Name = name;
                set.PipelineName = set.PipelineName?.Trim() ?? string.Empty;
                set.PipelineDefinitionSha256 = NormalizeSha256(set.PipelineDefinitionSha256);
                set.ImageSetSha256 = NormalizeSha256(set.ImageSetSha256);
                set.Notes = set.Notes?.Trim() ?? string.Empty;
                set.Dependencies ??= new List<OpenVisionRecipeValidationSetDependency>();
                set.Dependencies = set.Dependencies
                    .Where(item => item != null && !string.IsNullOrWhiteSpace(item.Path))
                    .Select(item =>
                    {
                        item.Path = NormalizeExistingOrDeclaredPath(item.Path);
                        item.Sha256 = NormalizeSha256(item.Sha256);
                        return item;
                    })
                    .Where(item => !string.IsNullOrWhiteSpace(item.Path))
                    .GroupBy(item => item.Path, StringComparer.OrdinalIgnoreCase)
                    .Select(group => group.First())
                    .ToList();
                set.Images ??= new List<OpenVisionRecipeValidationSetImage>();
                HashSet<string> imagePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                List<OpenVisionRecipeValidationSetImage> images = new List<OpenVisionRecipeValidationSetImage>();
                foreach (OpenVisionRecipeValidationSetImage image in set.Images.Where(item => item != null))
                {
                    if (!TryNormalizeImagePath(image.Path, requireExisting: false, out string path)
                        || !imagePaths.Add(path))
                    {
                        continue;
                    }

                    image.Path = path;
                    image.Expected = NormalizeExpected(image.Expected);
                    image.Sha256 = NormalizeSha256(image.Sha256);
                    image.VariantId = NormalizeVariantId(image.VariantId);
                    image.ExpectedMetricName = image.ExpectedMetricName?.Trim() ?? string.Empty;
                    image.ExpectedMetricMinimum = image.ExpectedMetricMinimum?.Trim() ?? string.Empty;
                    image.ExpectedMetricMaximum = image.ExpectedMetricMaximum?.Trim() ?? string.Empty;
                    image.Notes = image.Notes?.Trim() ?? string.Empty;
                    images.Add(image);
                }

                set.Images = images;
                normalizedSets.Add(set);
            }

            document.Sets = normalizedSets;
        }

        private static bool TryNormalizeImagePath(string path, bool requireExisting, out string normalized)
        {
            normalized = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                normalized = System.IO.Path.GetFullPath(path.Trim());
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                normalized = string.Empty;
                return false;
            }

            return SupportedImageExtensions.Contains(System.IO.Path.GetExtension(normalized))
                && (!requireExisting || File.Exists(normalized));
        }

        private static string NormalizeExpected(string expected)
        {
            return string.Equals(expected?.Trim(), OpenVisionRecipeValidationSetImage.ExpectedNg, StringComparison.OrdinalIgnoreCase)
                ? OpenVisionRecipeValidationSetImage.ExpectedNg
                : OpenVisionRecipeValidationSetImage.ExpectedOk;
        }

        private static string NormalizeExistingOrDeclaredPath(string path)
        {
            try
            {
                return Path.GetFullPath(path?.Trim() ?? string.Empty);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                return string.Empty;
            }
        }

        private static string NormalizeSha256(string value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static void CopyVariantContract(
            OpenVisionRecipeValidationSetImage source,
            OpenVisionRecipeValidationSetImage target)
        {
            target.VariantId = source?.VariantId ?? string.Empty;
            target.ExpectedMetricName = source?.ExpectedMetricName ?? string.Empty;
            target.ExpectedMetricMinimum = source?.ExpectedMetricMinimum ?? string.Empty;
            target.ExpectedMetricMaximum = source?.ExpectedMetricMaximum ?? string.Empty;
        }

        private static bool TryParseFinite(string value, out double parsed)
        {
            return double.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out parsed)
                && !double.IsNaN(parsed)
                && !double.IsInfinity(parsed);
        }

        private static string[] SplitMetricContractParts(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? Array.Empty<string>()
                : value.Split(new[] { ';' }, StringSplitOptions.None)
                    .Select(part => part.Trim())
                    .ToArray();
        }

        private static bool IsMetricContractPartCountValid(string[] values, int metricCount)
        {
            return values.Length == 0 || values.Length == 1 || values.Length == metricCount;
        }

        private static string ResolveMetricContractPart(string[] values, int index)
        {
            if (values == null || values.Length == 0)
            {
                return string.Empty;
            }

            if (index >= 0 && index < values.Length)
            {
                return values[index]?.Trim() ?? string.Empty;
            }

            return values.Length == 1 ? values[0]?.Trim() ?? string.Empty : string.Empty;
        }

        private static bool IsFileSha256Match(string path, string expected)
        {
            return !string.IsNullOrWhiteSpace(expected)
                && string.Equals(ComputeFileSha256(path), expected, StringComparison.OrdinalIgnoreCase);
        }
    }

    public sealed class OpenVisionRecipeValidationSetOption
    {
        internal OpenVisionRecipeValidationSetOption(OpenVisionRecipeValidationSet set)
        {
            Set = set ?? new OpenVisionRecipeValidationSet();
        }

        internal OpenVisionRecipeValidationSet Set { get; }

        public string Name => Set.Name ?? string.Empty;

        public int ImageCount => Set.Images?.Count ?? 0;

        public int ReadyCount => Set.Images?.Count(image => image != null && image.Exists) ?? 0;

        public int MissingCount => Math.Max(0, ImageCount - ReadyCount);

        public int OkCount => Set.Images?.Count(image => image != null && !image.IsExpectedNg) ?? 0;

        public int NgCount => Set.Images?.Count(image => image != null && image.IsExpectedNg) ?? 0;

        public bool IsIdentityLocked => Set.IsIdentityLocked;

        public string PipelineName => Set.PipelineName ?? string.Empty;

        public string PipelineDefinitionSha256 => Set.PipelineDefinitionSha256 ?? string.Empty;

        public string ImageSetSha256 => Set.ImageSetSha256 ?? string.Empty;

        public string DisplayText => Name
            + " | "
            + ReadyCount
            + "/"
            + ImageCount
            + (MissingCount > 0
                ? " | " + OpenVisionRecipeText.Local("누락 ", "Missing ") + MissingCount
                : string.Empty)
            + (Set.IsIdentityLocked
                ? " | " + OpenVisionRecipeText.Local("해시 잠금", "Hash locked")
                : string.Empty);
    }

    public sealed class OpenVisionRecipeValidationSetImageRow
    {
        internal OpenVisionRecipeValidationSetImageRow(OpenVisionRecipeValidationSetImage image)
        {
            Image = image ?? new OpenVisionRecipeValidationSetImage();
        }

        internal OpenVisionRecipeValidationSetImage Image { get; }

        public string Expected => Image.Expected ?? OpenVisionRecipeValidationSetImage.ExpectedOk;

        public bool IsMissing => !Image.Exists;

        public string StateText => Image.Exists
            ? OpenVisionRecipeText.Local("준비", "Ready")
            : OpenVisionRecipeText.Local("누락", "Missing");

        public string FileName => string.IsNullOrWhiteSpace(Image.Path) ? "-" : System.IO.Path.GetFileName(Image.Path);

        public string Path => Image.Path ?? string.Empty;

        public string Notes => Image.Notes ?? string.Empty;

        public string VariantId => OpenVisionRecipeValidationSetStorage.GetVariantDisplayId(Image);

        public string ExpectedMetricText => OpenVisionRecipeValidationSetStorage.BuildExpectedMetricText(Image);

        public string DisplayText => Expected
            + " | "
            + StateText
            + " | "
            + FileName
            + " | "
            + VariantId
            + (string.IsNullOrWhiteSpace(ExpectedMetricText) ? string.Empty : " | " + ExpectedMetricText)
            + (string.IsNullOrWhiteSpace(Notes) ? string.Empty : " | " + Notes);
    }
}
