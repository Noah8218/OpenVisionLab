using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.Core
{
    internal sealed class DisplayLayerStore
    {
        private readonly List<DisplayLayerMetadata> layers = new List<DisplayLayerMetadata>();

        public int Count => layers.Count;

        public IReadOnlyList<DisplayLayerInfo> GetInfos()
        {
            return layers
                .Select((layer, index) => new DisplayLayerInfo(index, layer.Title))
                .ToList();
        }

        public string GetTitle(int index)
        {
            if (index < 0 || index >= layers.Count)
            {
                return string.Empty;
            }

            return layers[index].Title ?? string.Empty;
        }

        public int FindIndex(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return -1;
            }

            for (int i = 0; i < layers.Count; i++)
            {
                if (string.Equals(layers[i].Title, title, System.StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        public int Create(string title, bool useClose, int? insertIndex = null)
        {
            int index = insertIndex.GetValueOrDefault(layers.Count);
            if (index < 0 || index > layers.Count)
            {
                index = layers.Count;
            }

            layers.Insert(index, new DisplayLayerMetadata
            {
                Title = title ?? string.Empty,
                UseClose = useClose
            });
            return index;
        }

        public bool Rename(string oldTitle, string newTitle)
        {
            int index = FindIndex(oldTitle);
            if (index < 0 || string.IsNullOrWhiteSpace(newTitle) || FindIndex(newTitle) >= 0)
            {
                return false;
            }

            layers[index].Title = newTitle.Trim();
            return true;
        }

        public void Remove(string title)
        {
            int index = FindIndex(title);
            if (index >= 0)
            {
                layers.RemoveAt(index);
            }
        }

        public void RemoveEmpty()
        {
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layers[i].Title))
                {
                    layers.RemoveAt(i);
                }
            }
        }

        public bool GetUseClose(int index)
        {
            if (index < 0 || index >= layers.Count)
            {
                return true;
            }

            return layers[index].UseClose;
        }

        private sealed class DisplayLayerMetadata
        {
            public string Title { get; set; } = string.Empty;
            public bool UseClose { get; set; } = true;
        }
    }
}
