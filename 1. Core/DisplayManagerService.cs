using OpenVisionLab.ImageSpace.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab._1._Core
{
    public sealed class DisplayManagerService : IDisplayManager, IDisplayHostBinder
    {
        public static DisplayManagerService Default { get; } = new DisplayManagerService();

        private readonly IImageSpace imageSpace = new ImageSpaceService();
        private readonly VisionRuntimeState state = new VisionRuntimeState();
        private readonly DisplayLayerPresenter layerPresenter;

        public event EventHandler<EventArgs> UpdateParameter;
        public event EventHandler<EventArgs> UpdateResult;
        public event EventHandler<EventArgs> UpdateCam;
        public event EventHandler<VisionToolRunEventArgs> VisionToolRunUpdated;

        public DisplayManagerService()
        {
            layerPresenter = new DisplayLayerPresenter(this, imageSpace, () => SelectedItem);
        }

        public VisionRuntimeState State => state;
        public IImageSpace ImageSpace => imageSpace;
        public int LayerCount => layerPresenter.LayerCount;
        public string SelectedItem { get { return state.SelectedItem; } set { state.SelectedItem = value; } }
        public string FocusItem { get { return state.FocusItem; } set { state.FocusItem = value; } }
        public int CameraIndex { get { return state.CameraIndex; } set { state.CameraIndex = value; } }
        public string TackTime { get { return state.TackTime; } set { state.TackTime = value; } }

        public void SetForm(Form form) => layerPresenter.SetForm(form);
        public void SetDockPanel(DockPanel dockPanel) => layerPresenter.SetDockPanel(dockPanel);

        public void SetCameraIndex(int cameraIndex)
        {
            CameraIndex = cameraIndex;
            UpdateCam?.Invoke(null, EventArgs.Empty);
        }

        public void SetTackTime(string tackTime)
        {
            TackTime = tackTime;
            UpdateResult?.Invoke(null, EventArgs.Empty);
        }

        public void NotifyParameterChanged()
        {
            UpdateParameter?.Invoke(null, EventArgs.Empty);
        }

        public void NotifyVisionToolRunUpdated(VisionToolRunEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            VisionToolRunUpdated?.Invoke(this, args);
        }

        public void CreatePanel(ImageSpaceFrame frame = null)
        {
            layerPresenter.CreatePanel(frame);
        }

        public IReadOnlyList<DisplayLayerInfo> GetLayerInfos()
        {
            return layerPresenter.GetLayerInfos();
        }

        public string GetLayerTitle(int index)
        {
            return layerPresenter.GetLayerTitle(index);
        }

        internal FormLayerDisplay GetLayerDisplayOrNull(string title)
        {
            return layerPresenter.GetLayerDisplayOrNull(title);
        }

        public int FindIndex(string title)
        {
            return layerPresenter.FindIndex(title);
        }

        public int FindIndex()
        {
            return layerPresenter.FindSelectedIndex();
        }

        public void CreateLayerDisplay(ImageSpaceFrame frame, string title, bool useClose = true)
        {
            layerPresenter.CreateLayerDisplay(frame, title, useClose);
        }

        internal void SetLayerImage(int index, Bitmap image)
        {
            layerPresenter.SetLayerImage(index, image);
        }

        public void RefreshLayer(int index)
        {
            layerPresenter.RefreshLayer(index);
        }

        public void ActivateLayer(string title)
        {
            layerPresenter.ActivateLayer(title);
        }

        public void ActivateLayer(int index)
        {
            layerPresenter.ActivateLayer(index);
        }

        public void ZoomLayerToFit(string title)
        {
            layerPresenter.ZoomLayerToFit(title);
        }

        public void ZoomLayerToFit(int index)
        {
            layerPresenter.ZoomLayerToFit(index);
        }

        internal void AcceptLayerImageChanged(string title)
        {
            layerPresenter.AcceptLayerImageChanged(title);
        }

    }
}
