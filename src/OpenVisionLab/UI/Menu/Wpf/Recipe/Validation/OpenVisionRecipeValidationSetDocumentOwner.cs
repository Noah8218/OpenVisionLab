using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Owns the mutable Validation Set document and its persistence boundary.
    internal sealed class OpenVisionRecipeValidationSetDocumentOwner
    {
        private OpenVisionRecipeValidationSetDocument document =
            OpenVisionRecipeValidationSetStorage.CreateEmpty();

        internal bool StorageReady { get; private set; } = true;

        internal bool TryLoad(string recipeName, out string error)
        {
            bool loaded = OpenVisionRecipeValidationSetStorage.TryLoad(
                recipeName,
                out OpenVisionRecipeValidationSetDocument loadedDocument,
                out error);
            document = loadedDocument ?? OpenVisionRecipeValidationSetStorage.CreateEmpty();
            StorageReady = loaded;
            return loaded;
        }

        internal OpenVisionRecipeValidationSetOptionSelection BuildSelection(
            string preferredName,
            string preferredTrainName,
            string preferredValidationName,
            string preferredTestName)
        {
            return OpenVisionRecipeValidationSetPresenter.BuildOptionSelection(
                document,
                preferredName,
                preferredTrainName,
                preferredValidationName,
                preferredTestName);
        }

        internal OpenVisionRecipeValidationSetImageSelection BuildImageSelection(
            OpenVisionRecipeValidationSetOption option,
            string previousPath)
        {
            return OpenVisionRecipeValidationSetPresenter.BuildImageSelection(
                option,
                previousPath);
        }

        internal bool ContainsSet(string name)
        {
            return document.Sets?.Any(set => string.Equals(
                       set?.Name,
                       name,
                       StringComparison.OrdinalIgnoreCase)) == true;
        }

        internal bool TryCreateSet(string name)
        {
            string normalizedName = name?.Trim() ?? string.Empty;
            if (!OpenVisionRecipeValidationSetStorage.IsValidSetName(normalizedName)
                || ContainsSet(normalizedName))
            {
                return false;
            }

            document.Sets ??= new List<OpenVisionRecipeValidationSet>();
            document.Sets.Add(new OpenVisionRecipeValidationSet { Name = normalizedName });
            return true;
        }

        internal bool TryDeleteSet(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || document.Sets == null)
            {
                return false;
            }

            return document.Sets.RemoveAll(set => string.Equals(
                       set?.Name,
                       name,
                       StringComparison.OrdinalIgnoreCase)) > 0;
        }

        internal OpenVisionRecipeCatalogPairValidationSetImportResult ImportCatalogPair(
            VisionPipelineSampleCatalogItem selectedSample,
            IEnumerable<VisionPipelineSampleCatalogItem> availableSamples,
            string pipelineName)
        {
            return OpenVisionRecipeCatalogPairValidationSetService.Import(
                document,
                selectedSample,
                availableSamples,
                pipelineName);
        }

        internal bool TryAddImages(
            string setName,
            IEnumerable<string> paths,
            string expected,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum,
            out int added,
            out int updated,
            out int skipped,
            out string error)
        {
            added = 0;
            updated = 0;
            skipped = 0;
            error = string.Empty;
            OpenVisionRecipeValidationSet set = FindSet(setName);
            if (set == null || set.IsIdentityLocked)
            {
                return false;
            }

            OpenVisionRecipeValidationSetImage contract = new OpenVisionRecipeValidationSetImage
            {
                VariantId = variantId,
                ExpectedMetricName = metricName,
                ExpectedMetricMinimum = metricMinimum,
                ExpectedMetricMaximum = metricMaximum
            };
            if (!OpenVisionRecipeValidationSetStorage.TryValidateVariantContract(
                    contract,
                    out error))
            {
                return false;
            }

            added = OpenVisionRecipeValidationSetStorage.AddOrUpdateImages(
                set,
                paths,
                expected,
                notes,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum,
                out updated,
                out skipped);
            return added > 0 || updated > 0;
        }

        internal bool TryApplyVariantContract(
            string setName,
            string imagePath,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum,
            out string error)
        {
            if (!TryFindImage(
                    setName,
                    imagePath,
                    out OpenVisionRecipeValidationSet set,
                    out OpenVisionRecipeValidationSetImage image))
            {
                error = "The selected validation image is no longer available.";
                return false;
            }

            return OpenVisionRecipeValidationSetStorage.TryApplyVariantContract(
                set,
                image,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum,
                out error);
        }

        internal bool TryRepairMissingImagePath(
            string setName,
            string imagePath,
            string replacementPath,
            out string repairedPath,
            out string error)
        {
            repairedPath = string.Empty;
            if (!TryFindImage(
                    setName,
                    imagePath,
                    out OpenVisionRecipeValidationSet set,
                    out OpenVisionRecipeValidationSetImage image))
            {
                error = "The selected validation image is no longer available.";
                return false;
            }

            return OpenVisionRecipeValidationSetStorage.TryRepairMissingImagePath(
                set,
                image,
                replacementPath,
                out repairedPath,
                out error);
        }

        internal bool TryRemoveImage(string setName, string imagePath)
        {
            if (!TryFindImage(
                    setName,
                    imagePath,
                    out OpenVisionRecipeValidationSet set,
                    out OpenVisionRecipeValidationSetImage image))
            {
                return false;
            }

            return set.Images.RemoveAll(item => ReferenceEquals(item, image)
                    || string.Equals(item?.Path, imagePath, StringComparison.OrdinalIgnoreCase)) > 0;
        }

        internal bool TrySave(string recipeName, out string error)
        {
            return OpenVisionRecipeValidationSetStorage.TrySave(recipeName, document, out error);
        }

        internal string CreateUniqueSetName()
        {
            const string baseName = "Local_Validation_Set";
            HashSet<string> names = (document.Sets ?? new List<OpenVisionRecipeValidationSet>())
                .Where(set => set != null && !string.IsNullOrWhiteSpace(set.Name))
                .Select(set => set.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!names.Contains(baseName))
            {
                return baseName;
            }

            int suffix = 2;
            while (names.Contains(baseName + "_" + suffix.ToString(CultureInfo.InvariantCulture)))
            {
                suffix++;
            }

            return baseName + "_" + suffix.ToString(CultureInfo.InvariantCulture);
        }

        private OpenVisionRecipeValidationSet FindSet(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? null
                : document.Sets?.FirstOrDefault(set => string.Equals(
                    set?.Name,
                    name,
                    StringComparison.OrdinalIgnoreCase));
        }

        private bool TryFindImage(
            string setName,
            string imagePath,
            out OpenVisionRecipeValidationSet set,
            out OpenVisionRecipeValidationSetImage image)
        {
            set = FindSet(setName);
            image = set?.Images?.FirstOrDefault(item => string.Equals(
                item?.Path,
                imagePath,
                StringComparison.OrdinalIgnoreCase));
            return set != null && image != null;
        }
    }
}
