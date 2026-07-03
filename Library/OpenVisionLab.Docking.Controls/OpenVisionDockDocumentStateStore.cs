using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentStateStore
    {
        private readonly string documentIdsPath;
        private readonly string layoutPath;
        private readonly List<string> loadedDocumentIds = new List<string>();
        private bool loaded;

        public OpenVisionDockDocumentStateStore(string documentIdsPath, string layoutPath)
        {
            this.documentIdsPath = string.IsNullOrWhiteSpace(documentIdsPath)
                ? throw new ArgumentException("Document id state path is required.", nameof(documentIdsPath))
                : documentIdsPath;
            this.layoutPath = string.IsNullOrWhiteSpace(layoutPath)
                ? throw new ArgumentException("Document layout state path is required.", nameof(layoutPath))
                : layoutPath;
        }

        public IReadOnlyList<string> LoadedDocumentIds => loadedDocumentIds;

        public bool HasLoadedDocumentIds => loadedDocumentIds.Count > 0;

        public void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }

            loaded = true;
            loadedDocumentIds.Clear();
            if (!File.Exists(documentIdsPath))
            {
                return;
            }

            try
            {
                loadedDocumentIds.AddRange(NormalizeIds(File.ReadAllLines(documentIdsPath)));
            }
            catch
            {
                loadedDocumentIds.Clear();
            }
        }

        public void ClearLoadedDocumentIds()
        {
            loadedDocumentIds.Clear();
        }

        public void SaveDocumentIds(IEnumerable<string> documentIds)
        {
            List<string> distinctIds = NormalizeIds(documentIds).ToList();
            loaded = true;
            loadedDocumentIds.Clear();
            loadedDocumentIds.AddRange(distinctIds);

            try
            {
                if (distinctIds.Count == 0)
                {
                    DeleteIfExists(documentIdsPath);
                    DeleteIfExists(layoutPath);
                    return;
                }

                EnsureParentDirectory(documentIdsPath);
                File.WriteAllLines(documentIdsPath, distinctIds);
            }
            catch
            {
            }
        }

        public IReadOnlyList<OpenVisionDockDocumentLayoutEntry> LoadPaneLayout(ICollection<string> activeDocumentIds)
        {
            if (!File.Exists(layoutPath) || activeDocumentIds == null || activeDocumentIds.Count == 0)
            {
                return new List<OpenVisionDockDocumentLayoutEntry>();
            }

            try
            {
                return File.ReadAllLines(layoutPath)
                    .Select(ParseLayoutLine)
                    .Where(item => !string.IsNullOrWhiteSpace(item.DocumentId)
                        && activeDocumentIds.Contains(item.DocumentId, StringComparer.OrdinalIgnoreCase))
                    .GroupBy(item => item.DocumentId, StringComparer.OrdinalIgnoreCase)
                    .Select(group =>
                    {
                        (string DocumentId, int PaneIndex, string LayoutPath) item = group.First();
                        return new OpenVisionDockDocumentLayoutEntry(
                            item.DocumentId,
                            Math.Max(0, item.PaneIndex),
                            string.IsNullOrWhiteSpace(item.LayoutPath)
                                ? CreateFlatLayoutPath(item.PaneIndex)
                                : item.LayoutPath);
                    })
                    .ToList();
            }
            catch
            {
                return new List<OpenVisionDockDocumentLayoutEntry>();
            }
        }

        public void SavePaneLayout(IEnumerable<OpenVisionDockDocumentLayoutEntry> paneLayout)
        {
            try
            {
                List<string> lines = (paneLayout ?? Enumerable.Empty<OpenVisionDockDocumentLayoutEntry>())
                    .Where(item => !string.IsNullOrWhiteSpace(item.LayerTitle))
                    .Select(item =>
                        item.LayerTitle.Trim()
                        + "\t"
                        + Math.Max(0, item.PaneIndex).ToString(CultureInfo.InvariantCulture)
                        + "\t"
                        + (string.IsNullOrWhiteSpace(item.LayoutPath)
                            ? CreateFlatLayoutPath(item.PaneIndex)
                            : item.LayoutPath.Trim()))
                    .ToList();

                if (lines.Count == 0)
                {
                    DeleteIfExists(layoutPath);
                    return;
                }

                EnsureParentDirectory(layoutPath);
                File.WriteAllLines(layoutPath, lines);
            }
            catch
            {
            }
        }

        private static IEnumerable<string> NormalizeIds(IEnumerable<string> documentIds)
        {
            return (documentIds ?? Enumerable.Empty<string>())
                .Select(id => id?.Trim())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static (string DocumentId, int PaneIndex, string LayoutPath) ParseLayoutLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return (string.Empty, 0, string.Empty);
            }

            // Persisted docking files only describe state; native document moves stay in the docking workspace.
            string[] parts = line.Split(new[] { '\t' }, 3);
            string documentId = parts[0]?.Trim() ?? string.Empty;
            int paneIndex = 0;
            if (parts.Length > 1)
            {
                int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out paneIndex);
            }

            string layoutPath = parts.Length > 2
                ? parts[2]?.Trim() ?? string.Empty
                : CreateFlatLayoutPath(paneIndex);
            return (documentId, paneIndex, layoutPath);
        }

        private static string CreateFlatLayoutPath(int paneIndex)
        {
            return "H" + Math.Max(0, paneIndex).ToString(CultureInfo.InvariantCulture);
        }

        private static void EnsureParentDirectory(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
