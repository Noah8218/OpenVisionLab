using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineSampleCatalogItem
    {
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
        public string ImageFullPath { get; set; } = string.Empty;
        public string PipelineFullPath { get; set; } = string.Empty;

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

                string minimum = string.IsNullOrWhiteSpace(ExpectedMetricMinimum) ? string.Empty : $" min {ExpectedMetricMinimum}";
                string maximum = string.IsNullOrWhiteSpace(ExpectedMetricMaximum) ? string.Empty : $" max {ExpectedMetricMaximum}";
                return $"{ExpectedMetricName}{minimum}{maximum}";
            }
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Category)
                ? SampleName
                : $"{SampleName}  [{Category}]";
        }

        public static List<VisionPipelineSampleCatalogItem> LoadRunnable()
        {
            string catalogPath = ResolveWorkspacePath("docs", "samples", "OpenVisionLab.SampleCatalog.csv");
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
                    Notes = GetValue(row, "Notes")
                };

                if (string.IsNullOrWhiteSpace(item.BaselinePipeline))
                {
                    continue;
                }

                item.ImageFullPath = ResolveRelativePath(repoRoot, item.ImagePath);
                item.PipelineFullPath = ResolveRelativePath(repoRoot, item.BaselinePipeline);
                items.Add(item);
            }

            return items;
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
    }
}
