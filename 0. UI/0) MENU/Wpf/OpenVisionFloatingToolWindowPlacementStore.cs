using System;
using System.Globalization;
using System.IO;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionFloatingToolWindowPlacementStore
    {
        private static readonly string PlacementPath = Path.Combine(
            AppPathService.EnsureDirectory("CONFIG", "UI"),
            "FloatingToolWindow.bounds");

        public bool TryLoad(out Rect bounds)
        {
            bounds = Rect.Empty;
            try
            {
                if (!File.Exists(PlacementPath))
                {
                    return false;
                }

                string[] parts = File.ReadAllText(PlacementPath)
                    .Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 4)
                {
                    return false;
                }

                double left = Parse(parts[0]);
                double top = Parse(parts[1]);
                double width = Parse(parts[2]);
                double height = Parse(parts[3]);
                if (width <= 0 || height <= 0)
                {
                    return false;
                }

                bounds = new Rect(left, top, width, height);
                return true;
            }
            catch
            {
                bounds = Rect.Empty;
                return false;
            }
        }

        public void Save(Window window)
        {
            if (window == null || !window.IsVisible || window.WindowState != WindowState.Normal)
            {
                return;
            }

            try
            {
                string directory = Path.GetDirectoryName(PlacementPath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string text = string.Join(
                    "\t",
                    Format(window.Left),
                    Format(window.Top),
                    Format(window.Width),
                    Format(window.Height));
                File.WriteAllText(PlacementPath, text);
            }
            catch
            {
            }
        }

        private static double Parse(string text)
        {
            return double.Parse(text, CultureInfo.InvariantCulture);
        }

        private static string Format(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}
