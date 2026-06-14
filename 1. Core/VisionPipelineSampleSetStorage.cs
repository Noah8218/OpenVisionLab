using OpenVisionLab._1._Core;
using Lib.Common;
using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineSampleSetInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DirectoryPath { get; set; } = string.Empty;
        public int LayerCount { get; set; }
        public DateTime SavedAt { get; set; }

        public override string ToString()
        {
            return $"{Name} ({LayerCount})";
        }
    }

    internal static class VisionPipelineSampleSetStorage
    {
        private const string ManifestFileName = "layers.tsv";

        public static List<VisionPipelineSampleSetInfo> List(string recipeName, string pipelineName)
        {
            string rootDirectory = RecipeWorkspaceService.GetVisionPipelineSampleSetRootDirectory(recipeName, pipelineName);
            if (!Directory.Exists(rootDirectory))
            {
                return new List<VisionPipelineSampleSetInfo>();
            }

            return Directory.EnumerateDirectories(rootDirectory)
                .Select(directory => CreateInfo(directory))
                .Where(info => info != null)
                .OrderByDescending(info => info.SavedAt)
                .ToList();
        }

        public static int Save(string recipeName, string pipelineName, string sampleSetName, IDisplayManager displayManager)
        {
            string directory = RecipeWorkspaceService.GetVisionPipelineSampleSetDirectory(recipeName, pipelineName, sampleSetName);
            List<string> manifestLines = new List<string>();
            Dictionary<string, int> usedFileNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> savedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                string title = displayManager.GetLayerTitle(i);
                Bitmap image = displayManager.GetLayerImage(i);
                if (image == null || string.IsNullOrWhiteSpace(title))
                {
                    continue;
                }

                string fileName = GetUniqueImageFileName(SanitizeFileName(title), usedFileNames);
                savedFileNames.Add(fileName);
                using (Bitmap clone = new Bitmap(image))
                {
                    clone.Save(Path.Combine(directory, fileName), ImageFormat.Png);
                }

                manifestLines.Add($"{EscapeManifestValue(title)}\t{EscapeManifestValue(fileName)}");
            }

            File.WriteAllLines(GetManifestPath(directory), manifestLines);
            DeleteStaleImages(directory, savedFileNames);
            return manifestLines.Count;
        }

        public static int Load(string recipeName, string pipelineName, string sampleSetName, IDisplayManager displayManager)
        {
            string directory = RecipeWorkspaceService.GetVisionPipelineSampleSetDirectory(recipeName, pipelineName, sampleSetName);
            string manifestPath = GetManifestPath(directory);
            if (!File.Exists(manifestPath))
            {
                return 0;
            }

            int loadCount = 0;
            foreach (string line in File.ReadAllLines(manifestPath))
            {
                string[] parts = line.Split('\t');
                if (parts.Length != 2)
                {
                    continue;
                }

                string title = UnescapeManifestValue(parts[0]);
                string fileName = UnescapeManifestValue(parts[1]);
                string imagePath = Path.Combine(directory, fileName);
                if (string.IsNullOrWhiteSpace(title) || !File.Exists(imagePath))
                {
                    continue;
                }

                using (Bitmap loaded = new Bitmap(imagePath))
                {
                    Bitmap layerImage = new Bitmap(loaded);
                    int index = displayManager.FindIndex(title);
                    if (index >= 0)
                    {
                        displayManager.SetLayerImage(index, layerImage);
                        displayManager.RefreshLayer(index);
                    }
                    else
                    {
                        displayManager.CreateLayerDisplay(layerImage, title, true);
                    }
                }

                loadCount++;
            }

            return loadCount;
        }

        public static List<string> GetLayerTitles(VisionPipelineSampleSetInfo sampleSet)
        {
            return ReadLayerEntries(sampleSet)
                .Select(entry => entry.Title)
                .Where(title => !string.IsNullOrWhiteSpace(title))
                .ToList();
        }

        public static VisionPipelineContext CreateContext(VisionPipelineSampleSetInfo sampleSet)
        {
            if (sampleSet == null)
            {
                throw new ArgumentNullException(nameof(sampleSet));
            }

            VisionPipelineContext context = new VisionPipelineContext();
            foreach (SampleLayerEntry entry in ReadLayerEntries(sampleSet))
            {
                string imagePath = Path.Combine(sampleSet.DirectoryPath, entry.FileName);
                if (string.IsNullOrWhiteSpace(entry.Title) || !File.Exists(imagePath))
                {
                    continue;
                }

                using (Bitmap loaded = new Bitmap(imagePath))
                using (OpenCvSharp.Mat mat = BitmapImageConverter.ToMat(loaded))
                {
                    context.SetLayer(entry.Title, mat);
                }
            }

            return context;
        }

        public static void Delete(VisionPipelineSampleSetInfo sampleSet)
        {
            if (sampleSet == null || string.IsNullOrWhiteSpace(sampleSet.DirectoryPath) || !Directory.Exists(sampleSet.DirectoryPath))
            {
                return;
            }

            Directory.Delete(sampleSet.DirectoryPath, recursive: true);
        }

        private static VisionPipelineSampleSetInfo CreateInfo(string directory)
        {
            string manifestPath = GetManifestPath(directory);
            if (!File.Exists(manifestPath))
            {
                return null;
            }

            return new VisionPipelineSampleSetInfo
            {
                Name = Path.GetFileName(directory),
                DirectoryPath = directory,
                LayerCount = File.ReadLines(manifestPath).Count(),
                SavedAt = File.GetLastWriteTime(manifestPath)
            };
        }

        private static List<SampleLayerEntry> ReadLayerEntries(VisionPipelineSampleSetInfo sampleSet)
        {
            if (sampleSet == null || string.IsNullOrWhiteSpace(sampleSet.DirectoryPath))
            {
                return new List<SampleLayerEntry>();
            }

            string manifestPath = GetManifestPath(sampleSet.DirectoryPath);
            if (!File.Exists(manifestPath))
            {
                return new List<SampleLayerEntry>();
            }

            List<SampleLayerEntry> entries = new List<SampleLayerEntry>();
            foreach (string line in File.ReadAllLines(manifestPath))
            {
                string[] parts = line.Split('\t');
                if (parts.Length != 2)
                {
                    continue;
                }

                entries.Add(new SampleLayerEntry
                {
                    Title = UnescapeManifestValue(parts[0]),
                    FileName = UnescapeManifestValue(parts[1])
                });
            }

            return entries;
        }

        private sealed class SampleLayerEntry
        {
            public string Title { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;
        }

        private static string GetManifestPath(string directory)
        {
            return Path.Combine(directory, ManifestFileName);
        }

        private static string GetUniqueImageFileName(string baseName, IDictionary<string, int> usedFileNames)
        {
            string name = string.IsNullOrWhiteSpace(baseName) ? "Layer" : baseName;
            string fileName = $"{name}.png";

            if (!usedFileNames.TryGetValue(fileName, out int count))
            {
                usedFileNames[fileName] = 1;
                return fileName;
            }

            count++;
            usedFileNames[fileName] = count;
            return $"{name}_{count}.png";
        }

        private static void DeleteStaleImages(string directory, ISet<string> savedFileNames)
        {
            foreach (string path in Directory.EnumerateFiles(directory, "*.png"))
            {
                string fileName = Path.GetFileName(path);
                if (savedFileNames.Contains(fileName))
                {
                    continue;
                }

                try
                {
                    File.Delete(path);
                }
                catch
                {
                    // Sample image cleanup is best-effort.
                }
            }
        }

        private static string SanitizeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Layer";
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Layer" : sanitized;
        }

        private static string EscapeManifestValue(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\t", "\\t")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        private static string UnescapeManifestValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t")
                .Replace("\\\\", "\\");
        }
    }
}
