using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeCatalogPairValidationSetImportResult
    {
        private OpenVisionRecipeCatalogPairValidationSetImportResult(
            bool success,
            string setName,
            int okCount,
            int ngCount,
            bool updated,
            string error)
        {
            Success = success;
            SetName = setName ?? string.Empty;
            OkCount = okCount;
            NgCount = ngCount;
            Updated = updated;
            Error = error ?? string.Empty;
        }

        public bool Success { get; }
        public string SetName { get; }
        public int OkCount { get; }
        public int NgCount { get; }
        public bool Updated { get; }
        public string Error { get; }

        public static OpenVisionRecipeCatalogPairValidationSetImportResult Succeeded(
            string setName,
            int okCount,
            int ngCount,
            bool updated)
        {
            return new OpenVisionRecipeCatalogPairValidationSetImportResult(
                true,
                setName,
                okCount,
                ngCount,
                updated,
                string.Empty);
        }

        public static OpenVisionRecipeCatalogPairValidationSetImportResult Failed(string error)
        {
            return new OpenVisionRecipeCatalogPairValidationSetImportResult(
                false,
                string.Empty,
                0,
                0,
                false,
                error);
        }
    }

    internal static class OpenVisionRecipeCatalogPairValidationSetService
    {
        private const string CatalogMarkerPrefix = "CatalogPair:";

        public static bool CanImport(
            VisionPipelineSampleCatalogItem selectedSample,
            IEnumerable<VisionPipelineSampleCatalogItem> availableSamples)
        {
            return TryPreparePair(
                selectedSample,
                availableSamples,
                out _,
                out _,
                out _);
        }

        public static OpenVisionRecipeCatalogPairValidationSetImportResult Import(
            OpenVisionRecipeValidationSetDocument document,
            VisionPipelineSampleCatalogItem selectedSample,
            IEnumerable<VisionPipelineSampleCatalogItem> availableSamples,
            string pipelineName)
        {
            if (document == null)
            {
                return OpenVisionRecipeCatalogPairValidationSetImportResult.Failed(
                    "Validation set document is missing.");
            }

            if (!TryPreparePair(
                    selectedSample,
                    availableSamples,
                    out List<VisionPipelineSampleCatalogItem> pairSamples,
                    out int okCount,
                    out int ngCount))
            {
                return OpenVisionRecipeCatalogPairValidationSetImportResult.Failed(
                    "Select a catalog pair that contains at least one existing OK image and one existing NG image.");
            }

            try
            {
                document.Sets ??= new List<OpenVisionRecipeValidationSet>();
                string marker = CreateCatalogMarker(selectedSample);
                string preferredName = CreatePreferredSetName(selectedSample.PairGroup);
                OpenVisionRecipeValidationSet existingCatalogSet = document.Sets
                    .FirstOrDefault(set => IsSameCatalogPairSet(set, marker));
                bool updated = existingCatalogSet != null;
                OpenVisionRecipeValidationSet target = existingCatalogSet
                    ?? new OpenVisionRecipeValidationSet
                    {
                        Name = CreateAvailableSetName(document, preferredName)
                    };
                if (target.IsIdentityLocked)
                {
                    return OpenVisionRecipeCatalogPairValidationSetImportResult.Failed(
                        "The matching catalog validation set is hash-locked. Create a new working set instead of overwriting frozen evidence.");
                }

                List<OpenVisionRecipeValidationSetImage> images = pairSamples
                    .Select(CreateValidationImage)
                    .ToList();
                foreach (OpenVisionRecipeValidationSetImage image in images)
                {
                    if (!OpenVisionRecipeValidationSetStorage.TryValidateVariantContract(image, out string contractError))
                    {
                        return OpenVisionRecipeCatalogPairValidationSetImportResult.Failed(
                            "Catalog metric contract is invalid for "
                            + Path.GetFileName(image.Path)
                            + ": "
                            + contractError);
                    }
                }

                target.PipelineName = pipelineName?.Trim() ?? string.Empty;
                target.PipelineDefinitionSha256 = string.Empty;
                target.Dependencies = new List<OpenVisionRecipeValidationSetDependency>();
                target.Images = images;
                target.ImageSetSha256 = string.Empty;
                target.Notes = marker
                    + " | Imported from "
                    + selectedSample.CatalogSourceId
                    + " sample catalog. Import does not execute Preview, Run, or the validation suite.";

                if (!updated)
                {
                    document.Sets.Add(target);
                }

                return OpenVisionRecipeCatalogPairValidationSetImportResult.Succeeded(
                    target.Name,
                    okCount,
                    ngCount,
                    updated);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                return OpenVisionRecipeCatalogPairValidationSetImportResult.Failed(
                    ex.GetBaseException().Message);
            }
        }

        private static bool TryPreparePair(
            VisionPipelineSampleCatalogItem selectedSample,
            IEnumerable<VisionPipelineSampleCatalogItem> availableSamples,
            out List<VisionPipelineSampleCatalogItem> pairSamples,
            out int okCount,
            out int ngCount)
        {
            pairSamples = new List<VisionPipelineSampleCatalogItem>();
            okCount = 0;
            ngCount = 0;
            if (selectedSample == null
                || string.IsNullOrWhiteSpace(selectedSample.PairGroup))
            {
                return false;
            }

            pairSamples = (availableSamples ?? Enumerable.Empty<VisionPipelineSampleCatalogItem>())
                .Where(sample => sample != null
                    && sample.CatalogSourceKind == selectedSample.CatalogSourceKind
                    && string.Equals(
                        sample.PairGroup?.Trim(),
                        selectedSample.PairGroup.Trim(),
                        StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(sample.ImageFullPath)
                    && File.Exists(sample.ImageFullPath))
                .OrderBy(sample => IsExpectedNg(sample) ? 1 : 0)
                .ThenBy(sample => sample.SampleName, StringComparer.OrdinalIgnoreCase)
                .ToList();
            okCount = pairSamples.Count(sample => !IsExpectedNg(sample));
            ngCount = pairSamples.Count(IsExpectedNg);
            return pairSamples.Count >= 2 && okCount > 0 && ngCount > 0;
        }

        private static OpenVisionRecipeValidationSetImage CreateValidationImage(
            VisionPipelineSampleCatalogItem sample)
        {
            string path = Path.GetFullPath(sample.ImageFullPath);
            return new OpenVisionRecipeValidationSetImage
            {
                Expected = IsExpectedNg(sample)
                    ? OpenVisionRecipeValidationSetImage.ExpectedNg
                    : OpenVisionRecipeValidationSetImage.ExpectedOk,
                Path = path,
                Sha256 = OpenVisionRecipeValidationSetStorage.ComputeFileSha256(path),
                VariantId = Truncate(sample.SampleName?.Trim(), 80),
                ExpectedMetricName = sample.ExpectedMetricName?.Trim() ?? string.Empty,
                ExpectedMetricMinimum = sample.ExpectedMetricMinimum?.Trim() ?? string.Empty,
                ExpectedMetricMaximum = sample.ExpectedMetricMaximum?.Trim() ?? string.Empty,
                Notes = "Catalog "
                    + sample.CatalogSourceId
                    + " | "
                    + (sample.PairRole?.Trim() ?? string.Empty)
                    + (string.IsNullOrWhiteSpace(sample.Notes) ? string.Empty : " | " + sample.Notes.Trim())
            };
        }

        private static bool IsExpectedNg(VisionPipelineSampleCatalogItem sample)
        {
            string role = sample?.PairRole?.Trim() ?? string.Empty;
            return sample?.ExpectsFailure == true
                || string.Equals(role, "Bad", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "NG", StringComparison.OrdinalIgnoreCase);
        }

        private static string CreateCatalogMarker(VisionPipelineSampleCatalogItem sample)
        {
            return CatalogMarkerPrefix
                + sample.CatalogSourceId
                + ":"
                + sample.PairGroup.Trim();
        }

        private static bool IsSameCatalogPairSet(
            OpenVisionRecipeValidationSet set,
            string marker)
        {
            return set != null
                && (set.Notes ?? string.Empty).StartsWith(marker, StringComparison.OrdinalIgnoreCase);
        }

        private static string CreatePreferredSetName(string pairGroup)
        {
            string name = "Catalog_" + (pairGroup?.Trim() ?? "Pair");
            return Truncate(name, 80);
        }

        private static string CreateAvailableSetName(
            OpenVisionRecipeValidationSetDocument document,
            string preferredName)
        {
            HashSet<string> names = (document.Sets ?? new List<OpenVisionRecipeValidationSet>())
                .Where(set => set != null && !string.IsNullOrWhiteSpace(set.Name))
                .Select(set => set.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!names.Contains(preferredName))
            {
                return preferredName;
            }

            int suffix = 2;
            while (true)
            {
                string suffixText = "_" + suffix;
                string candidate = Truncate(preferredName, 80 - suffixText.Length) + suffixText;
                if (!names.Contains(candidate))
                {
                    return candidate;
                }

                suffix++;
            }
        }

        private static string Truncate(string value, int maximumLength)
        {
            string text = value ?? string.Empty;
            return text.Length <= maximumLength ? text : text.Substring(0, maximumLength);
        }
    }
}
