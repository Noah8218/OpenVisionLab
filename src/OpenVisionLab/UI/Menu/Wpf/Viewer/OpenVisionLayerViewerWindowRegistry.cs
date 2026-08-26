using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionLayerViewerWindowRegistry
    {
        private readonly List<OpenVisionFloatingToolWindow> windows = new List<OpenVisionFloatingToolWindow>();

        public int Count => windows.Count;

        public string Titles => string.Join("|", windows.Select(window => window.Title));

        public IReadOnlyList<OpenVisionFloatingToolWindow> Windows => windows.ToList();

        public void Add(OpenVisionFloatingToolWindow window)
        {
            if (window == null)
            {
                return;
            }

            windows.Add(window);
            window.Closed += Window_Closed;
        }

        public void CloseAll()
        {
            foreach (OpenVisionFloatingToolWindow window in windows.ToList())
            {
                window.Closed -= Window_Closed;
                window.ClearHostedContent(disposeContent: true);
                window.Close();
            }

            windows.Clear();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (sender is OpenVisionFloatingToolWindow window)
            {
                window.Closed -= Window_Closed;
                window.ClearHostedContent(disposeContent: true);
                windows.Remove(window);
            }
        }
    }
}
