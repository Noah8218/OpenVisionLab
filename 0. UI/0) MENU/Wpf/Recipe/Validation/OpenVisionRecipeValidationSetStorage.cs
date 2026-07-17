using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string Notes { get; set; } = string.Empty;

        [XmlElement("Image")]
        public List<OpenVisionRecipeValidationSetImage> Images { get; set; } = new List<OpenVisionRecipeValidationSetImage>();
    }

    public sealed class OpenVisionRecipeValidationSetImage
    {
        public const string ExpectedOk = "OK";
        public const string ExpectedNg = "NG";

        [XmlAttribute]
        public string Expected { get; set; } = ExpectedOk;

        [XmlAttribute]
        public string Path { get; set; } = string.Empty;

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
            string directory = Path.Combine(
                RecipeWorkspaceService.GetRecipeWorkspaceDirectory(recipeName),
                "VISION",
                "ValidationSets");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, FileName);
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
            updatedCount = 0;
            skippedCount = 0;
            if (set == null)
            {
                return 0;
            }

            set.Images ??= new List<OpenVisionRecipeValidationSetImage>();
            string normalizedExpected = NormalizeExpected(expected);
            string normalizedNotes = notes?.Trim() ?? string.Empty;
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
                    Notes = normalizedNotes
                });
                addedCount++;
            }

            return addedCount;
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
                set.Notes = set.Notes?.Trim() ?? string.Empty;
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

        public string DisplayText => Name
            + " | "
            + ReadyCount
            + "/"
            + ImageCount
            + (MissingCount > 0
                ? " | " + OpenVisionRecipeText.Local("누락 ", "Missing ") + MissingCount
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

        public string DisplayText => Expected
            + " | "
            + StateText
            + " | "
            + FileName
            + (string.IsNullOrWhiteSpace(Notes) ? string.Empty : " | " + Notes);
    }
}
