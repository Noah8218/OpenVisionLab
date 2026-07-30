using System;
using System.IO;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolRecentSelectionStore
    {
        private const string DisableEnvironmentVariable = "OPENVISIONLAB_DISABLE_TOOL_RECENT_STORE";
        private const string FileName = "recent-native-tool.txt";

        public static bool TryRead(out VISION_MENU menu)
        {
            menu = default;
            if (IsDisabled())
            {
                return false;
            }

            try
            {
                string path = GetStorePath();
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    return false;
                }

                string text = File.ReadAllText(path)?.Trim();
                return Enum.TryParse(text, ignoreCase: true, out menu);
            }
            catch
            {
                menu = default;
                return false;
            }
        }

        public static void Save(VISION_MENU menu)
        {
            if (IsDisabled())
            {
                return;
            }

            try
            {
                string path = GetStorePath();
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, menu.ToString());
            }
            catch
            {
                // Recent-tool persistence is only a startup hint; never let it affect tool execution.
            }
        }

        private static bool IsDisabled()
        {
            return string.Equals(
                Environment.GetEnvironmentVariable(DisableEnvironmentVariable),
                "1",
                StringComparison.Ordinal);
        }

        private static string GetStorePath()
        {
            return Path.Combine(
                AppPathService.ConfigRootDirectory,
                "UI",
                FileName);
        }
    }
}
