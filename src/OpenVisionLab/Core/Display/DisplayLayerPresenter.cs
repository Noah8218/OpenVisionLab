using OpenVisionLab.ImageSpace.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab.Core
{
    internal sealed class DisplayLayerPresenter
    {
        private readonly object displaySync = new object();
        private readonly DisplayLayerStore layers;
        private readonly IImageSpace imageSpace;
        private readonly DisplayImageSyncService imageSync;
        private readonly IDisplayManager displayManager;
        private readonly Func<string> selectedItemAccessor;

        public DisplayLayerPresenter(IDisplayManager displayManager, IImageSpace imageSpace, Func<string> selectedItemAccessor)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.imageSpace = imageSpace ?? throw new ArgumentNullException(nameof(imageSpace));
            this.selectedItemAccessor = selectedItemAccessor ?? (() => string.Empty);
            layers = new DisplayLayerStore();
            imageSync = new DisplayImageSyncService(imageSpace, layers);
        }

        public int LayerCount => layers.Count;

        public IReadOnlyList<DisplayLayerInfo> GetLayerInfos()
        {
            lock (displaySync)
            {
                return layers.GetInfos();
            }
        }

        public string GetLayerTitle(int index)
        {
            lock (displaySync)
            {
                return layers.GetTitle(index);
            }
        }

        public int FindIndex(string title)
        {
            lock (displaySync)
            {
                return layers.FindIndex(title);
            }
        }

        public int FindSelectedIndex()
        {
            return FindIndex(selectedItemAccessor());
        }

        public void CreatePanel(ImageSpaceFrame frame = null)
        {
            Bitmap image = frame?.Image ?? new Bitmap(10, 10);
            CreateLayerDisplay(ImageSpaceFrameAdapter.FromBitmap(image), CreateNewLayerName(), true);
        }

        private string CreateNewLayerName()
        {
            int next = layers.Count + 1;
            string title;
            do
            {
                title = "NewPanel_" + next.ToString(System.Globalization.CultureInfo.InvariantCulture);
                next++;
            }
            while (FindIndex(title) >= 0);

            return title;
        }

        public void CreateLayerDisplay(ImageSpaceFrame frame, string title, bool useClose = true)
        {
            CreateLayerDisplay(frame, title, useClose, null);
        }

        public void CreateLayerDisplayAt(ImageSpaceFrame frame, string title, bool useClose, int index)
        {
            CreateLayerDisplay(frame, title, useClose, index);
        }

        private void CreateLayerDisplay(ImageSpaceFrame frame, string title, bool useClose, int? insertIndex)
        {
            if (frame?.Image == null) { return; }

            lock (displaySync)
            {
                layers.RemoveEmpty();
                int displayIndex = layers.FindIndex(title);
                if (displayIndex < 0)
                {
                    AddLayer(frame.Image, title, useClose, insertIndex);
                    return;
                }

                UpdateLayer(displayIndex, frame.Image, title);
            }
        }

        public void SetLayerImage(int index, Bitmap image)
        {
            lock (displaySync)
            {
                imageSync.SetImage(index, image);
            }
        }

        public void RemoveLayerDisplay(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            lock (displaySync)
            {
                layers.Remove(title);
                imageSpace.RemoveImage(title);
            }
        }

        public bool RenameLayerDisplay(string oldTitle, string newTitle)
        {
            if (string.IsNullOrWhiteSpace(oldTitle) || string.IsNullOrWhiteSpace(newTitle))
            {
                return false;
            }

            string normalizedNewTitle = newTitle.Trim();
            lock (displaySync)
            {
                int index = layers.FindIndex(oldTitle);
                if (index < 0
                    || string.Equals(layers.GetTitle(index), normalizedNewTitle, StringComparison.OrdinalIgnoreCase)
                    || layers.FindIndex(normalizedNewTitle) >= 0)
                {
                    return false;
                }

                Bitmap image = imageSpace.GetImage(index);
                if (!layers.Rename(oldTitle, normalizedNewTitle))
                {
                    return false;
                }

                imageSpace.SetImage(index, normalizedNewTitle, image);
                displayManager.FocusItem = normalizedNewTitle;
                displayManager.SelectedItem = normalizedNewTitle;
                imageSpace.SetActiveImage(image);
                return true;
            }
        }

        public void RefreshLayer(int index)
        {
            // WPF workspaces bind to ImageSpace state, so there is no per-form viewer to refresh.
        }

        public void ActivateLayer(string title)
        {
            ActivateLayer(FindIndex(title));
        }

        public void ActivateLayer(int index)
        {
            lock (displaySync)
            {
                string title = layers.GetTitle(index);
                if (string.IsNullOrWhiteSpace(title))
                {
                    return;
                }

                displayManager.FocusItem = title;
                displayManager.SelectedItem = title;
                imageSpace.SetActiveImage(imageSpace.GetImage(index));
            }
        }

        public void ZoomLayerToFit(string title)
        {
            ZoomLayerToFit(FindIndex(title));
        }

        public void ZoomLayerToFit(int index)
        {
            // The active WPF image workspace owns fit/zoom presentation.
        }

        public void AcceptLayerImageChanged(string title)
        {
            imageSync.AcceptImageChanged(title, FindIndex(title));
        }

        public bool GetLayerUseClose(int index)
        {
            lock (displaySync)
            {
                return layers.GetUseClose(index);
            }
        }

        private void AddLayer(Bitmap imageSource, string title, bool useClose, int? insertIndex)
        {
            int index = layers.Create(title, useClose, insertIndex);
            Bitmap layerImage = CloneBitmap(imageSource);
            imageSpace.InsertImage(index, title, layerImage);
            imageSpace.SetActiveImage(layerImage);
            displayManager.FocusItem = title;
            displayManager.SelectedItem = title;
        }

        private void UpdateLayer(int displayIndex, Bitmap imageSource, string title)
        {
            Bitmap layerImage = CloneBitmap(imageSource);
            imageSpace.SetImage(displayIndex, title, layerImage);
            imageSpace.SetActiveImage(layerImage);
            displayManager.FocusItem = title;
            displayManager.SelectedItem = title;
        }

        private static Bitmap CloneBitmap(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }

            try
            {
                return image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            }
            catch
            {
                return new Bitmap(image);
            }
        }
    }
}
