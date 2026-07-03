using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal enum VisionPipelineSampleCatalogSourceKind
    {
        Unknown = 0,
        Public = 1,
        LocalLegacy = 2,
        Product = 3
    }

    internal sealed class VisionPipelineSampleCatalogItem
    {
        private const string PublicCatalogFileName = "OpenVisionLab.PublicSampleCatalog.csv";
        private const string LocalLegacyCatalogFileName = "OpenVisionLab.SampleCatalog.csv";
        private const string ProductCatalogFileName = "OpenVisionLab.ProductSampleCatalog.csv";
        private string cachedToolFlowText;
        private string pipelineFullPath = string.Empty;

        public string SampleName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public string BaselinePipeline { get; set; } = string.Empty;
        public string ValidationMode { get; set; } = string.Empty;
        public string ExpectedMetricName { get; set; } = string.Empty;
        public string ExpectedMetricMinimum { get; set; } = string.Empty;
        public string ExpectedMetricMaximum { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string ReferenceImagePath { get; set; } = string.Empty;
        public string PairGroup { get; set; } = string.Empty;
        public string PairRole { get; set; } = string.Empty;
        public string ImageFullPath { get; set; } = string.Empty;
        public string PipelineFullPath
        {
            get => pipelineFullPath;
            set
            {
                pipelineFullPath = value ?? string.Empty;
                cachedToolFlowText = null;
            }
        }
        public string ReferenceImageFullPath { get; set; } = string.Empty;
        public VisionPipelineSampleCatalogSourceKind CatalogSourceKind { get; set; } = VisionPipelineSampleCatalogSourceKind.Unknown;
        public string CatalogSourcePath { get; set; } = string.Empty;

        public string CatalogSourceId
        {
            get
            {
                return CatalogSourceKind switch
                {
                    VisionPipelineSampleCatalogSourceKind.Public => "public",
                    VisionPipelineSampleCatalogSourceKind.LocalLegacy => "local-legacy",
                    VisionPipelineSampleCatalogSourceKind.Product => "product",
                    _ => "unknown"
                };
            }
        }

        public string CatalogSourceDisplayName
        {
            get
            {
                return CatalogSourceKind switch
                {
                    VisionPipelineSampleCatalogSourceKind.Public => LocalText("공개 샘플", "Public"),
                    VisionPipelineSampleCatalogSourceKind.LocalLegacy => LocalText("로컬 Legacy", "Local Legacy"),
                    VisionPipelineSampleCatalogSourceKind.Product => LocalText("제품군 샘플", "Product"),
                    _ => LocalText("출처 알 수 없음", "Unknown")
                };
            }
        }

        public string CatalogSourceDescription
        {
            get
            {
                return CatalogSourceKind switch
                {
                    VisionPipelineSampleCatalogSourceKind.Public => LocalText(
                        "GitHub/튜토리얼/포트폴리오에 사용할 수 있는 synthetic 샘플입니다.",
                        "GitHub-safe synthetic samples."),
                    VisionPipelineSampleCatalogSourceKind.LocalLegacy => LocalText(
                        "로컬 SDK/Sample 폴더 참조입니다. 사용 권리 확인 전에는 공개 문서에 쓰지 않습니다.",
                        "Local SDK/sample-folder references. Do not use for public docs until cleared."),
                    VisionPipelineSampleCatalogSourceKind.Product => LocalText(
                        "제품군 Good/Bad 샘플과 현장형 Explore 샘플입니다.",
                        "Product-domain Good/Bad samples plus field-style Explore samples for recipe setup."),
                    _ => LocalText("카탈로그 출처를 확인할 수 없습니다.", "Catalog source is unknown.")
                };
            }
        }

        public string CatalogSourceBadgeText => CatalogSourceDisplayName;

        public IReadOnlyList<VisionPipelineSampleExpectedMetric> ExpectedMetrics => BuildExpectedMetrics();

        public bool HasPair =>
            !string.IsNullOrWhiteSpace(PairGroup)
            || !string.IsNullOrWhiteSpace(PairRole);

        public bool ExpectsFailure =>
            string.Equals(ValidationMode?.Trim(), "ExpectedFailure", StringComparison.OrdinalIgnoreCase);

        public string PairText
        {
            get
            {
                if (!HasPair)
                {
                    return "-";
                }

                string group = string.IsNullOrWhiteSpace(PairGroup) ? "Pair" : PairGroup.Trim();
                string role = string.IsNullOrWhiteSpace(PairRole) ? "Sample" : PairRole.Trim();
                return $"{group} / {role}";
            }
        }

        public bool CanOpen => !string.IsNullOrWhiteSpace(ImageFullPath)
            && File.Exists(ImageFullPath)
            && !string.IsNullOrWhiteSpace(PipelineFullPath)
            && File.Exists(PipelineFullPath);

        public string ExpectedText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExpectedMetricName))
                {
                    return "-";
                }

                List<VisionPipelineSampleExpectedMetric> metrics = BuildExpectedMetrics();
                return metrics.Count == 0
                    ? "-"
                    : string.Join("; ", metrics.Select(metric => metric.Text));
            }
        }

        public string CatalogListText
        {
            get
            {
                string state = CanOpen ? "Ready" : "Missing";
                string pair = HasPair ? $" | {PairRole.Trim()}" : string.Empty;
                return $"{SampleName}{pair} | {CatalogSourceBadgeText} | {state}";
            }
        }

        public string ToolFlowText
        {
            get
            {
                if (cachedToolFlowText == null)
                {
                    string flow = ResolvePipelineToolFlow(PipelineFullPath);
                    cachedToolFlowText = string.IsNullOrWhiteSpace(flow) ? "-" : flow;
                }

                return cachedToolFlowText;
            }
        }

        public string LearningText
        {
            get
            {
                string category = string.IsNullOrWhiteSpace(Category) ? "Recipe" : Category.Trim();
                string flow = ToolFlowText;
                string pair = HasPair ? $" | Pair: {PairText}" : string.Empty;
                return flow == "-"
                    ? $"Learn: {category}{pair}"
                    : $"Learn: {category}{pair} | Flow: {flow}";
            }
        }

        public string ExpectedReasonText
        {
            get
            {
                IReadOnlyList<VisionPipelineSampleExpectedMetric> metrics = ExpectedMetrics;
                if (metrics.Count == 0)
                {
                    return "-";
                }

                HashSet<string> metricNames = new HashSet<string>(
                    metrics.Select(metric => metric.Name ?? string.Empty),
                    StringComparer.OrdinalIgnoreCase);
                bool isBadReference = string.Equals(PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase) || ExpectsFailure;
                List<string> reasons = new List<string>();

                if (ExpectsFailure)
                {
                    reasons.Add("This negative reference is expected to return controlled NG/no-result.");
                }
                else if (HasPair)
                {
                    string role = string.IsNullOrWhiteSpace(PairRole) ? "sample" : PairRole.Trim();
                    reasons.Add($"This {role} reference defines the acceptance margin for {PairGroup}.");
                }

                if (metricNames.Contains("ScoreMax"))
                {
                    reasons.Add(isBadReference
                        ? "Low ScoreMax separates wrong/no-target hypotheses from real matches."
                        : "High ScoreMax confirms that the requested template or feature target is present.");
                }

                if (metricNames.Contains("ResultCount"))
                {
                    reasons.Add(isBadReference
                        ? "ResultCount is used to prove the recipe rejects or limits unwanted candidates."
                        : "ResultCount confirms the expected number of detected candidates.");
                }

                if (metricNames.Contains("MeanValueAvg"))
                {
                    reasons.Add(isBadReference
                        ? "MeanValueAvg captures brightness drift from the normal reference band."
                        : "MeanValueAvg stays inside the normal brightness reference band.");
                }

                if (metricNames.Contains("LineAngleAvg") || metricNames.Contains("LineLengthMax") || metricNames.Contains("EdgeCount"))
                {
                    reasons.Add(isBadReference
                        ? "Line/edge metrics expose angle, length, or edge-count drift."
                        : "Line/edge metrics stay inside the expected geometry band.");
                }

                if (metricNames.Contains("AreaMax") || metricNames.Contains("AreaAvg") || metricNames.Contains("BoundsWidthMax") || metricNames.Contains("BoundsWidthMmMax"))
                {
                    reasons.Add(isBadReference
                        ? "Area/width metrics expose defect or contamination growth."
                        : "Area/width metrics remain inside the normal object geometry band.");
                }

                if (metricNames.Contains("MergeOverlayCount") || metricNames.Contains("MergeSourceCount"))
                {
                    reasons.Add("Merged overlay metrics confirm that branch results are visible in one review image.");
                }

                return reasons.Count == 0
                    ? ExpectedText
                    : string.Join(" ", reasons.Distinct(StringComparer.OrdinalIgnoreCase));
            }
        }

        public string RecipeGuideText
        {
            get
            {
                List<string> parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Goal))
                {
                    parts.Add($"Goal: {Goal.Trim()}");
                }

                string flow = ToolFlowText;
                if (flow != "-")
                {
                    parts.Add($"Flow: {flow}");
                }

                if (!string.IsNullOrWhiteSpace(ExpectedText) && ExpectedText != "-")
                {
                    parts.Add($"Expected: {ExpectedText}");
                }

                string expectedReason = ExpectedReasonText;
                if (!string.IsNullOrWhiteSpace(expectedReason) && expectedReason != "-")
                {
                    parts.Add($"Reason: {expectedReason}");
                }

                if (HasPair)
                {
                    parts.Add($"Pair: {PairText}");
                }

                if (ExpectsFailure)
                {
                    parts.Add("Expected outcome: no result / controlled NG");
                }

                string checkGuide = CheckGuideText;
                if (!string.IsNullOrWhiteSpace(checkGuide) && checkGuide != "-")
                {
                    parts.Add(checkGuide);
                }

                string fixGuide = FixGuideText;
                if (!string.IsNullOrWhiteSpace(fixGuide) && fixGuide != "-")
                {
                    parts.Add(fixGuide);
                }

                return parts.Count == 0 ? "-" : string.Join(" | ", parts);
            }
        }

        public string FixGuideText
        {
            get
            {
                HashSet<string> metricNames = new HashSet<string>(
                    ExpectedMetrics.Select(metric => metric.Name ?? string.Empty),
                    StringComparer.OrdinalIgnoreCase);
                string flow = ToolFlowText ?? string.Empty;

                if (ExpectsFailure)
                {
                    return "Fix point: if this sample passes, tighten ROI/score/acceptance so no-target images stay NG.";
                }

                if (metricNames.Contains("ScoreMax") || flow.IndexOf("Matching", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "Fix point: check template crop, ROI, score threshold, input layer, angle/scale options.";
                }

                if (metricNames.Contains("ResultCount") && flow.IndexOf("Blob", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "Fix point: tune threshold, morphology, MIN_AREA/MAX_AREA, and ROI before changing acceptance.";
                }

                if (metricNames.Contains("LineAngleAvg") || metricNames.Contains("LineLengthMax") || metricNames.Contains("EdgeCount"))
                {
                    return "Fix point: check ROI, projection direction, polarity, contrast, and sampling step.";
                }

                if (metricNames.Contains("MeanValueAvg"))
                {
                    return "Fix point: verify source layer, ROI, threshold off/on state, and lighting drift range.";
                }

                if (metricNames.Contains("AreaMax") || metricNames.Contains("BoundsWidthMax") || metricNames.Contains("BoundsWidthMmMax"))
                {
                    return "Fix point: inspect threshold/morphology/ROI first, then adjust metric bounds.";
                }

                return "-";
            }
        }

        public string CheckGuideText
        {
            get
            {
                IReadOnlyList<VisionPipelineSampleExpectedMetric> metrics = ExpectedMetrics;
                if (metrics.Count == 0)
                {
                    return "-";
                }

                List<string> checks = new List<string>();
                HashSet<string> names = new HashSet<string>(
                    metrics.Select(metric => metric.Name ?? string.Empty),
                    StringComparer.OrdinalIgnoreCase);

                if (names.Contains("MergeOverlayCount") || names.Contains("MergeSourceCount"))
                {
                    checks.Add("final merged review overlay");
                }

                if (names.Contains("ResultCount"))
                {
                    checks.Add("detected object count");
                }

                if (names.Contains("AreaMax") || names.Contains("AreaAvg"))
                {
                    checks.Add("defect/object area range");
                }

                if (names.Contains("BoundsWidthMax") || names.Contains("BoundsWidthMmMax") || names.Contains("BoundsWidthAvg"))
                {
                    checks.Add("object width in px/mm");
                }

                if (names.Contains("EdgeCount") || names.Contains("EdgePointCount"))
                {
                    checks.Add("edge point count");
                }

                if (names.Contains("LineLengthMax") || names.Contains("LineLengthMmMax") || names.Contains("LineAngleAvg"))
                {
                    checks.Add("fitted line length/angle");
                }

                if (names.Contains("ScoreMax"))
                {
                    checks.Add("matching score and result count");
                }

                if (names.Contains("MeanValueAvg"))
                {
                    checks.Add("mean brightness value");
                }

                if (ExpectsFailure)
                {
                    checks.Add("expected no-result/failure behavior");
                }

                if (names.Contains("ResultImageWidth") || names.Contains("ResultImageHeight"))
                {
                    checks.Add("output image size");
                }

                if (checks.Count == 0)
                {
                    checks.Add(string.Join(", ", metrics.Select(metric => metric.Name).Where(name => !string.IsNullOrWhiteSpace(name))));
                }

                return checks.Count == 0
                    ? "-"
                    : $"Check: {string.Join(", ", checks.Distinct(StringComparer.OrdinalIgnoreCase))}";
            }
        }

        public override string ToString()
        {
            return CatalogListText;
        }

        public static List<VisionPipelineSampleCatalogItem> LoadRunnable()
        {
            List<VisionPipelineSampleCatalogItem> items = new List<VisionPipelineSampleCatalogItem>();
            items.AddRange(LoadRunnable(VisionPipelineSampleCatalogSourceKind.Public));
            items.AddRange(LoadRunnable(VisionPipelineSampleCatalogSourceKind.Product));
            items.AddRange(LoadRunnable(VisionPipelineSampleCatalogSourceKind.LocalLegacy));
            return items;
        }

        public static List<VisionPipelineSampleCatalogItem> LoadRunnable(VisionPipelineSampleCatalogSourceKind sourceKind)
        {
            string catalogFileName = ResolveCatalogFileName(sourceKind);
            if (string.IsNullOrWhiteSpace(catalogFileName))
            {
                return new List<VisionPipelineSampleCatalogItem>();
            }

            string catalogPath = ResolveWorkspacePath("docs", "samples", catalogFileName);
            if (string.IsNullOrWhiteSpace(catalogPath) || !File.Exists(catalogPath))
            {
                return new List<VisionPipelineSampleCatalogItem>();
            }

            string repoRoot = ResolveRepoRoot(Path.GetDirectoryName(catalogPath));
            string[] lines = File.ReadAllLines(catalogPath);
            if (lines.Length <= 1)
            {
                return new List<VisionPipelineSampleCatalogItem>();
            }

            List<string> headers = ParseCsvLine(lines[0]);
            List<VisionPipelineSampleCatalogItem> items = new List<VisionPipelineSampleCatalogItem>();
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                Dictionary<string, string> row = CreateRow(headers, ParseCsvLine(lines[i]));
                VisionPipelineSampleCatalogItem item = new VisionPipelineSampleCatalogItem
                {
                    SampleName = GetValue(row, "SampleName"),
                    ImagePath = GetValue(row, "ImagePath"),
                    Width = ParseInt(GetValue(row, "Width")),
                    Height = ParseInt(GetValue(row, "Height")),
                    Category = GetValue(row, "Category"),
                    Goal = GetValue(row, "Goal"),
                    BaselinePipeline = GetValue(row, "BaselinePipeline"),
                    ValidationMode = GetValue(row, "ValidationMode"),
                    ExpectedMetricName = GetValue(row, "ExpectedMetricName"),
                    ExpectedMetricMinimum = GetValue(row, "ExpectedMetricMinimum"),
                    ExpectedMetricMaximum = GetValue(row, "ExpectedMetricMaximum"),
                    Notes = GetValue(row, "Notes"),
                    ReferenceImagePath = GetValue(row, "ReferenceImagePath"),
                    PairGroup = GetValue(row, "PairGroup"),
                    PairRole = GetValue(row, "PairRole"),
                    CatalogSourceKind = sourceKind,
                    CatalogSourcePath = catalogPath
                };

                if (string.IsNullOrWhiteSpace(item.BaselinePipeline))
                {
                    continue;
                }

                item.ImageFullPath = ResolveRelativePath(repoRoot, item.ImagePath);
                item.PipelineFullPath = ResolveRelativePath(repoRoot, item.BaselinePipeline);
                item.ReferenceImageFullPath = ResolveRelativePath(repoRoot, item.ReferenceImagePath);
                items.Add(item);
            }

            return items;
        }

        public static List<VisionPipelineSampleFolderCoverageItem> LoadFolderCoverage()
        {
            string catalogPath = ResolveWorkspacePath("docs", "samples", LocalLegacyCatalogFileName);
            if (string.IsNullOrWhiteSpace(catalogPath) || !File.Exists(catalogPath))
            {
                return new List<VisionPipelineSampleFolderCoverageItem>();
            }

            string repoRoot = ResolveRepoRoot(Path.GetDirectoryName(catalogPath));
            string sampleRoot = Path.Combine(repoRoot, "Sample");
            if (!Directory.Exists(sampleRoot))
            {
                return new List<VisionPipelineSampleFolderCoverageItem>();
            }

            HashSet<string> catalogTopFolders = new HashSet<string>(
                LoadRunnable(VisionPipelineSampleCatalogSourceKind.LocalLegacy)
                    .Select(item => GetCatalogTopFolder(item.ImagePath))
                    .Where(folder => !string.IsNullOrWhiteSpace(folder)),
                StringComparer.OrdinalIgnoreCase);

            string[] imageExtensions =
            {
                ".bmp",
                ".jpg",
                ".jpeg",
                ".png",
                ".tif",
                ".tiff"
            };

            return Directory
                .EnumerateFiles(sampleRoot, "*.*", SearchOption.AllDirectories)
                .Where(file => imageExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                .GroupBy(file => GetSampleTopFolder(sampleRoot, file), StringComparer.OrdinalIgnoreCase)
                .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
                .Select(group =>
                {
                    bool covered = catalogTopFolders.Contains(group.Key);
                    return new VisionPipelineSampleFolderCoverageItem
                    {
                        Folder = group.Key,
                        ImageCount = group.Count(),
                        IsCovered = covered
                    };
                })
                .ToList();
        }

        private static Dictionary<string, string> CreateRow(List<string> headers, List<string> values)
        {
            Dictionary<string, string> row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < headers.Count; i++)
            {
                row[headers[i]] = i < values.Count ? values[i] : string.Empty;
            }

            return row;
        }

        private static string GetValue(Dictionary<string, string> row, string key)
        {
            return row.TryGetValue(key, out string value) ? value?.Trim() ?? string.Empty : string.Empty;
        }

        private static string ResolveCatalogFileName(VisionPipelineSampleCatalogSourceKind sourceKind)
        {
            return sourceKind switch
            {
                VisionPipelineSampleCatalogSourceKind.Public => PublicCatalogFileName,
                VisionPipelineSampleCatalogSourceKind.LocalLegacy => LocalLegacyCatalogFileName,
                VisionPipelineSampleCatalogSourceKind.Product => ProductCatalogFileName,
                _ => string.Empty
            };
        }

        private List<VisionPipelineSampleExpectedMetric> BuildExpectedMetrics()
        {
            string[] names = SplitMetricParts(ExpectedMetricName);
            string[] minimums = SplitMetricParts(ExpectedMetricMinimum);
            string[] maximums = SplitMetricParts(ExpectedMetricMaximum);
            List<VisionPipelineSampleExpectedMetric> metrics = new List<VisionPipelineSampleExpectedMetric>();

            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i]?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                metrics.Add(new VisionPipelineSampleExpectedMetric
                {
                    Name = name,
                    Minimum = ResolveMetricPart(minimums, i),
                    Maximum = ResolveMetricPart(maximums, i)
                });
            }

            return metrics;
        }

        private static string[] SplitMetricParts(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? Array.Empty<string>()
                : value.Split(new[] { ';' }, StringSplitOptions.None)
                    .Select(part => part.Trim())
                    .ToArray();
        }

        private static string ResolveMetricPart(string[] values, int index)
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

        private static int ParseInt(string value)
        {
            return int.TryParse(value, out int parsed) ? parsed : 0;
        }

        private static List<string> ParseCsvLine(string line)
        {
            List<string> values = new List<string>();
            if (line == null)
            {
                return values;
            }

            bool quoted = false;
            System.Text.StringBuilder value = new System.Text.StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char current = line[i];
                if (current == '"')
                {
                    if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        value.Append('"');
                        i++;
                    }
                    else
                    {
                        quoted = !quoted;
                    }

                    continue;
                }

                if (current == ',' && !quoted)
                {
                    values.Add(value.ToString());
                    value.Clear();
                    continue;
                }

                value.Append(current);
            }

            values.Add(value.ToString());
            return values;
        }

        private static string ResolvePipelineToolFlow(string pipelinePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pipelinePath) || !File.Exists(pipelinePath))
                {
                    return string.Empty;
                }

                string xml = File.ReadAllText(pipelinePath);
                if (!SerializeHelper.TryLoadFromXmlText(xml, out VisionPipeline pipeline, out _)
                    || pipeline?.Steps == null
                    || pipeline.Steps.Count == 0)
                {
                    return string.Empty;
                }

                return string.Join(" -> ", pipeline.Steps
                    .Where(step => step != null && !string.IsNullOrWhiteSpace(step.ToolType))
                    .Select(step => step.ToolType.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase));
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetCatalogTopFolder(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)
                || !imagePath.StartsWith("Sample\\", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            string relativePath = imagePath.Substring("Sample\\".Length)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            int separatorIndex = relativePath.IndexOf(Path.DirectorySeparatorChar);
            return separatorIndex < 0 ? "." : relativePath.Substring(0, separatorIndex);
        }

        private static string GetSampleTopFolder(string sampleRoot, string imagePath)
        {
            string relativePath = Path.GetRelativePath(sampleRoot, imagePath)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            int separatorIndex = relativePath.IndexOf(Path.DirectorySeparatorChar);
            return separatorIndex < 0 ? "." : relativePath.Substring(0, separatorIndex);
        }

        private static string ResolveRelativePath(string repoRoot, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(path))
            {
                return Path.GetFullPath(path);
            }

            return Path.GetFullPath(Path.Combine(repoRoot, path));
        }

        private static string ResolveWorkspacePath(params string[] parts)
        {
            foreach (string root in EnumerateSearchRoots())
            {
                string candidate = Path.Combine(new[] { root }.Concat(parts).ToArray());
                if (File.Exists(candidate) || Directory.Exists(candidate))
                {
                    return Path.GetFullPath(candidate);
                }
            }

            return string.Empty;
        }

        private static string ResolveRepoRoot(string startDirectory)
        {
            DirectoryInfo directory = new DirectoryInfo(string.IsNullOrWhiteSpace(startDirectory)
                ? Directory.GetCurrentDirectory()
                : startDirectory);

            while (directory != null)
            {
                if (Directory.Exists(Path.Combine(directory.FullName, "docs", "samples"))
                    && Directory.Exists(Path.Combine(directory.FullName, "Sample")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            return Directory.GetCurrentDirectory();
        }

        private static IEnumerable<string> EnumerateSearchRoots()
        {
            HashSet<string> roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Directory.GetCurrentDirectory()
            };

            foreach (string root in roots.ToArray())
            {
                DirectoryInfo directory = new DirectoryInfo(root);
                for (int i = 0; i < 8 && directory != null; i++)
                {
                    yield return directory.FullName;
                    directory = directory.Parent;
                }
            }
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }
    }

    internal sealed class VisionPipelineSampleFolderCoverageItem
    {
        public string Folder { get; set; } = string.Empty;
        public int ImageCount { get; set; }
        public bool IsCovered { get; set; }

        public string StatusText => IsCovered ? "Covered" : "Backlog";
    }

    internal sealed class VisionPipelineSampleExpectedMetric
    {
        public string Name { get; set; } = string.Empty;
        public string Minimum { get; set; } = string.Empty;
        public string Maximum { get; set; } = string.Empty;

        public string Text
        {
            get
            {
                string minimum = string.IsNullOrWhiteSpace(Minimum) ? string.Empty : $" min {Minimum}";
                string maximum = string.IsNullOrWhiteSpace(Maximum) ? string.Empty : $" max {Maximum}";
                return $"{Name}{minimum}{maximum}";
            }
        }
    }
}
