using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenVisionLab._1._Core
{
    internal sealed class DisplayLayerStore
    {
        private readonly List<FormLayerDisplay> layers = new List<FormLayerDisplay>();

        public int Count => layers.Count;

        public IReadOnlyList<DisplayLayerInfo> GetInfos()
        {
            return layers
                .Select((display, index) => new DisplayLayerInfo(index, display.Text))
                .ToList();
        }

        public string GetTitle(int index)
        {
            return GetOrNull(index)?.Text ?? string.Empty;
        }

        public int FindIndex(string title)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (string.Equals(layers[i].Text, title, System.StringComparison.OrdinalIgnoreCase)) return i;
            }

            return -1;
        }

        public FormLayerDisplay Create(Bitmap imageSource, bool useClose, string title, IDisplayManager displayManager)
        {
            FormLayerDisplay display = new FormLayerDisplay(imageSource, layers.Count, layers, useClose, title, false, displayManager);
            layers.Add(display);
            return display;
        }

        public void RemoveEmpty()
        {
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (layers[i].Text == string.Empty)
                {
                    layers.RemoveAt(i);
                }
            }
        }

        public FormLayerDisplay GetOrNull(int index)
        {
            if (index < 0 || index >= layers.Count) return null;
            return layers[index];
        }

        public FormLayerDisplay GetByTitleOrFirst(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                FormLayerDisplay match = layers.FirstOrDefault(display => display.Text == title && !display.IsDisposed);
                if (match != null)
                {
                    return match;
                }
            }

            return layers.FirstOrDefault(display => !display.IsDisposed);
        }
    }
}
