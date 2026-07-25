using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.History;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab.Core
{
    public sealed class DisplayManagerService : IDisplayManager
    {
        public static DisplayManagerService Default { get; } = new DisplayManagerService();

        private readonly IImageSpace imageSpace = new ImageSpaceService();
        private readonly VisionRuntimeState state = new VisionRuntimeState();
        private readonly DisplayLayerPresenter layerPresenter;
        private readonly DisplayLayerImageHistoryService layerImageHistory;

        public event EventHandler<EventArgs> UpdateParameter;
        public event EventHandler<EventArgs> UpdateResult;
        public event EventHandler<EventArgs> UpdateCam;
        public event EventHandler<VisionToolRunEventArgs> VisionToolRunUpdated;
        public event EventHandler<UndoRedoStateChangedEventArgs> LayerImageHistoryChanged
        {
            add { layerImageHistory.StateChanged += value; }
            remove { layerImageHistory.StateChanged -= value; }
        }

        public DisplayManagerService()
        {
            layerPresenter = new DisplayLayerPresenter(this, imageSpace, () => SelectedItem);
            layerImageHistory = new DisplayLayerImageHistoryService(RestoreLayerImageSnapshot, RestoreLayerSnapshot);
        }

        public VisionRuntimeState State => state;
        public IImageSpace ImageSpace => imageSpace;
        public int LayerCount => layerPresenter.LayerCount;
        public string SelectedItem { get { return state.SelectedItem; } set { state.SelectedItem = value; } }
        public string FocusItem { get { return state.FocusItem; } set { state.FocusItem = value; } }
        public int CameraIndex { get { return state.CameraIndex; } set { state.CameraIndex = value; } }
        public string TackTime { get { return state.TackTime; } set { state.TackTime = value; } }
        public bool CanUndoLayerImage => layerImageHistory.CanUndo;
        public bool CanRedoLayerImage => layerImageHistory.CanRedo;
        public string UndoLayerImageText => layerImageHistory.UndoText;
        public string RedoLayerImageText => layerImageHistory.RedoText;
        public string LastLayerImageHistorySkippedReason => layerImageHistory.LastSkippedReason;

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
            int beforeIndex = FindIndex(title);
            DisplayLayerImageSnapshot beforeSnapshot = beforeIndex >= 0
                ? CaptureLayerImageSnapshot(beforeIndex, title)
                : null;
            DisplayLayerSnapshot beforeLayerSnapshot = beforeIndex >= 0
                ? CaptureLayerSnapshot(beforeIndex, title)
                : DisplayLayerSnapshot.Missing(beforeIndex, title);

            layerPresenter.CreateLayerDisplay(frame, title, useClose);

            if (beforeSnapshot != null)
            {
                int afterIndex = FindIndex(title);
                DisplayLayerImageSnapshot afterSnapshot = afterIndex >= 0
                    ? CaptureLayerImageSnapshot(afterIndex, title)
                    : null;
                layerImageHistory.Record($"Layer Image: {title}", beforeSnapshot, afterSnapshot);
            }
            else if (beforeLayerSnapshot != null)
            {
                int afterIndex = FindIndex(title);
                DisplayLayerSnapshot afterLayerSnapshot = afterIndex >= 0
                    ? CaptureLayerSnapshot(afterIndex, title)
                    : DisplayLayerSnapshot.Missing(afterIndex, title);
                layerImageHistory.RecordLayer($"Layer Create: {title}", beforeLayerSnapshot, afterLayerSnapshot);
            }
        }

        internal void SetLayerImage(int index, Bitmap image)
        {
            string title = GetLayerTitle(index);
            DisplayLayerImageSnapshot beforeSnapshot = CaptureLayerImageSnapshot(index, title);
            layerPresenter.SetLayerImage(index, image);
            DisplayLayerImageSnapshot afterSnapshot = CaptureLayerImageSnapshot(index, title);
            layerImageHistory.Record($"Layer Image: {title}", beforeSnapshot, afterSnapshot);
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

        public bool UndoLayerImage()
        {
            return layerImageHistory.Undo();
        }

        public bool RedoLayerImage()
        {
            return layerImageHistory.Redo();
        }

        public void ClearLayerImageHistory()
        {
            layerImageHistory.Clear();
        }

        internal void RemoveLayerDisplay(string title)
        {
            layerPresenter.RemoveLayerDisplay(title);
        }

        internal bool RenameLayerDisplay(string oldTitle, string newTitle)
        {
            return layerPresenter.RenameLayerDisplay(oldTitle, newTitle);
        }

        private DisplayLayerImageSnapshot CaptureLayerImageSnapshot(int index, string title)
        {
            if (layerImageHistory.IsReplaying)
            {
                return null;
            }

            Bitmap image = ImageSpace.GetImage(index);
            return layerImageHistory.Capture(index, title, image);
        }

        private DisplayLayerSnapshot CaptureLayerSnapshot(int index, string title)
        {
            if (layerImageHistory.IsReplaying)
            {
                return null;
            }

            Bitmap image = ImageSpace.GetImage(index);
            bool useClose = layerPresenter.GetLayerUseClose(index);
            Rectangle roi = ImageSpace.GetRoi(index);
            Rectangle trainRoi = ImageSpace.GetTrainRoi(index);
            return layerImageHistory.CaptureLayer(index, title, image, useClose, roi, trainRoi);
        }

        private void RestoreLayerImageSnapshot(DisplayLayerImageSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            int index = FindIndex(snapshot.Title);
            if (index < 0)
            {
                index = snapshot.Index;
            }

            if (index < 0)
            {
                return;
            }

            using Bitmap image = snapshot.Image?.ToBitmap();
            layerPresenter.SetLayerImage(index, image);
            RefreshLayer(index);
            ActivateLayer(index);
        }

        private void RestoreLayerSnapshot(DisplayLayerSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            if (!snapshot.Exists)
            {
                layerPresenter.RemoveLayerDisplay(snapshot.Title);
                return;
            }

            using Bitmap image = snapshot.Image?.ToBitmap();
            Bitmap restoreImage = image ?? new Bitmap(10, 10);
            int index = FindIndex(snapshot.Title);
            if (index >= 0)
            {
                layerPresenter.SetLayerImage(index, restoreImage);
            }
            else
            {
                layerPresenter.CreateLayerDisplayAt(ImageSpaceFrameAdapter.FromBitmap(restoreImage), snapshot.Title, snapshot.UseClose, snapshot.Index);
                index = FindIndex(snapshot.Title);
            }

            if (index >= 0)
            {
                ImageSpace.SetRoi(index, snapshot.Roi);
                ImageSpace.SetTrainRoi(index, snapshot.TrainRoi);
                RefreshLayer(index);
                ActivateLayer(index);
            }
        }

    }
}
