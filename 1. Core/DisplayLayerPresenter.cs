using OpenVisionLab.ImageSpace.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab._1._Core
{
    internal sealed class DisplayLayerPresenter : IDisplayHostBinder
    {
        private readonly object displaySync = new object();
        private readonly IDisplayHost host;
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
            host = new DisplayDockHost();
            layers = new DisplayLayerStore();
            imageSync = new DisplayImageSyncService(imageSpace, layers);
        }

        public int LayerCount => layers.Count;

        public void SetForm(Form form) => host.SetOwner(form);

        public void SetDockPanel(DockPanel dockPanel) => host.SetDockPanel(dockPanel);

        public IReadOnlyList<DisplayLayerInfo> GetLayerInfos()
        {
            lock (displaySync)
            {
                return layers.GetInfos();
            }
        }

        public string GetLayerTitle(int index)
        {
            return layers.GetTitle(index);
        }

        public FormLayerDisplay GetLayerDisplayOrNull(string title)
        {
            lock (displaySync)
            {
                return layers.GetByTitleOrFirst(title);
            }
        }

        public int FindIndex(string title)
        {
            return layers.FindIndex(title);
        }

        public int FindSelectedIndex()
        {
            return layers.FindIndex(selectedItemAccessor());
        }

        public void CreatePanel(ImageSpaceFrame frame = null)
        {
            host.InvokeOnUiThread(() =>
            {
                FormVision_NewPanel formVisionNewPanel = new FormVision_NewPanel(layers.Count);
                if (formVisionNewPanel.ShowDialog() == DialogResult.OK)
                {
                    Bitmap image = frame?.Image ?? new Bitmap(10, 10);
                    CreateLayerDisplay(ImageSpaceFrameAdapter.FromBitmap(image), formVisionNewPanel.PanelName, true);
                }
            });
        }

        public void CreateLayerDisplay(ImageSpaceFrame frame, string title, bool useClose = true)
        {
            if (frame?.Image == null) return;

            ClearEmptyDisplay();

            host.InvokeOnUiThread(() =>
            {
                lock (displaySync)
                {
                    int displayIndex = FindIndex(title);
                    FormLayerDisplay existingLayer = layers.GetOrNull(displayIndex);
                    bool existsLayer = existingLayer != null && existingLayer.Text == title;

                    if (!existsLayer)
                    {
                        AddLayerDisplay(frame.Image, title, useClose);
                        return;
                    }

                    UpdateLayerDisplay(displayIndex, frame.Image, title);
                }
            });
        }

        public void SetLayerImage(int index, Bitmap image)
        {
            imageSync.SetImage(index, image);
        }

        public void RefreshLayer(int index)
        {
            FormLayerDisplay display = layers.GetOrNull(index);
            display?.RefreshViewer();
        }

        public void ActivateLayer(string title)
        {
            ActivateLayer(FindIndex(title));
        }

        public void ActivateLayer(int index)
        {
            FormLayerDisplay display = layers.GetOrNull(index);
            if (display == null) { return; }

            display.Activate();
            displayManager.FocusItem = display.Text;
        }

        public void ZoomLayerToFit(string title)
        {
            ZoomLayerToFit(FindIndex(title));
        }

        public void ZoomLayerToFit(int index)
        {
            FormLayerDisplay display = layers.GetOrNull(index);
            display?.ZoomToFit();
        }

        public void AcceptLayerImageChanged(string title)
        {
            imageSync.AcceptImageChanged(title, FindIndex(title));
        }

        private void ClearEmptyDisplay()
        {
            host.InvokeOnUiThread(() =>
            {
                lock (displaySync)
                {
                    layers.RemoveEmpty();
                }
            });
        }

        private void AddLayerDisplay(Bitmap imageSource, string title, bool useClose)
        {
            if (host.DockPanel == null) return;

			FormLayerDisplay display = layers.Create(null, useClose, title, displayManager);
			display.Show(host.DockPanel, DockState.Document);
			display.SetImage(imageSource);
			imageSpace.SetImage(display.nIndex, title, display.GetCurrentImage());
        }

        private void UpdateLayerDisplay(int displayIndex, Bitmap imageSource, string title)
        {
            FormLayerDisplay display = layers.GetOrNull(displayIndex);
            if (display == null) return;

			display.SetImage(imageSource);
			imageSpace.SetImage(displayIndex, title, display.GetCurrentImage());

			if (host.ActiveDocumentTitle != title)
            {
                display.Activate();
            }
        }
    }
}
