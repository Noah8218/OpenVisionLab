using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace OpenVisionLab
{
    internal static class RecipeRuntimeStorage
    {
        public static bool Load(
            string recipeName,
            Func<DataState> dataAccessor,
            Action<DataState> dataSetter,
            Func<VisionToolRepository> visionToolAccessor)
        {
            VisionToolRepository visionTools = GetVisionTools(visionToolAccessor);
            if (!visionTools.LoadTools(recipeName))
            {
                return false;
            }

            DataState data = GetData(dataAccessor).LoadConfig(recipeName);
            dataSetter(data);
            return true;
        }

        public static bool Save(
            string recipeName,
            Func<DataState> dataAccessor,
            Func<VisionToolRepository> visionToolAccessor)
        {
            if (!GetVisionTools(visionToolAccessor).SaveTools(recipeName))
            {
                return false;
            }

            GetData(dataAccessor).SaveConfig(recipeName);
            return true;
        }

        // Persist the legacy r0 baseline before any Main pipeline/tool publication. A later
        // partial save can then be retried after reopen without treating it as unchanged.
        internal static void PrepareSavedRevision(string recipeName, DataState data)
        {
            if (!string.IsNullOrEmpty(data.SavedContentHash)) { return; }
            string path = RecipeWorkspaceService.GetVisionDataPath(recipeName);
            string description = SerializeHelper.TryLoadFromXmlFile(path, out DataState saved, out _) ? saved.Description : data.Description;
            string baseline = CaptureSavedContent(recipeName, description);
            data.SavedContentHash = baseline;
            try { data.SaveConfig(recipeName); }
            catch { data.SavedContentHash = string.Empty; throw; }
        }

        internal static bool SaveWithRevision(string recipeName, Func<DataState> dataAccessor, Func<VisionToolRepository> visionToolAccessor)
        {
            DataState data = GetData(dataAccessor);
            PrepareSavedRevision(recipeName, data);
            if (!GetVisionTools(visionToolAccessor).SaveTools(recipeName)) { return false; }
            string content = CaptureSavedContent(recipeName, data.Description);
            long previousRevision = data.SavedRevision;
            string previousHash = data.SavedContentHash;
            try
            {
                if (!string.Equals(content, previousHash, StringComparison.Ordinal)) { data.SavedRevision = checked(previousRevision + 1); }
                data.SavedContentHash = content;
                data.SaveConfig(recipeName);
                return true;
            }
            catch
            {
                data.SavedRevision = previousRevision;
                data.SavedContentHash = previousHash;
                throw;
            }
        }

        internal static void InitializeCreatedRevision(string recipeName, DataState data)
        {
            data.SavedRevision = 1;
            data.SavedContentHash = CaptureSavedContent(recipeName, data.Description);
            data.SaveConfig(recipeName);
        }

        private static string CaptureSavedContent(string recipeName, string description)
        {
            string directory = Path.GetDirectoryName(RecipeWorkspaceService.GetVisionConfigPath(recipeName, "__revision__"));
            StringBuilder content = new StringBuilder();
            content.Append(description?.Length ?? 0).Append(':').Append(description).Append('\n');
            foreach (string path in Directory.EnumerateFiles(directory, "*.xml", SearchOption.TopDirectoryOnly)
                .Where(path => !string.Equals(Path.GetFileName(path), "pipeline.active.xml", StringComparison.OrdinalIgnoreCase) && !Path.GetFileName(path).Contains(".invalid-", StringComparison.OrdinalIgnoreCase))
                .OrderBy(Path.GetFileName, StringComparer.Ordinal))
            {
                // Only recipe configuration XML participates: no runs, images, templates or backups.
                string xml = XDocument.Load(path).ToString(SaveOptions.DisableFormatting);
                content.Append(Path.GetFileName(path)).Append(':').Append(xml.Length).Append(':').Append(xml).Append('\n');
            }
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content.ToString())));
        }

        private static DataState GetData(Func<DataState> dataAccessor)
        {
            if (dataAccessor == null)
            {
                throw new InvalidOperationException("Recipe data runtime is not configured.");
            }

            return dataAccessor();
        }

        private static VisionToolRepository GetVisionTools(Func<VisionToolRepository> visionToolAccessor)
        {
            if (visionToolAccessor == null)
            {
                throw new InvalidOperationException("Vision tool runtime is not configured.");
            }

            return visionToolAccessor();
        }
    }
}
