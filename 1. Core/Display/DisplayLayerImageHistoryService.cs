using OpenVisionLab.History;
using System;
using System.Drawing;

namespace OpenVisionLab._1._Core
{
    internal sealed class DisplayLayerImageHistoryService
    {
        private readonly UndoRedoManager history;
        private readonly BitmapHistoryPolicy policy;
        private readonly Action<DisplayLayerImageSnapshot> restoreImage;
        private readonly Action<DisplayLayerSnapshot> restoreLayer;

        public DisplayLayerImageHistoryService(
            Action<DisplayLayerImageSnapshot> restoreImage,
            Action<DisplayLayerSnapshot> restoreLayer)
        {
            this.restoreImage = restoreImage ?? throw new ArgumentNullException(nameof(restoreImage));
            this.restoreLayer = restoreLayer ?? throw new ArgumentNullException(nameof(restoreLayer));
            history = new UndoRedoManager(20);
            policy = new BitmapHistoryPolicy
            {
                MaxPixels = 25_000_000,
                MaxEncodedBytes = 64 * 1024 * 1024
            };
            history.StateChanged += (sender, e) => StateChanged?.Invoke(this, e);
        }

        public event EventHandler<UndoRedoStateChangedEventArgs> StateChanged;

        public bool CanUndo => history.CanUndo;
        public bool CanRedo => history.CanRedo;
        public bool IsReplaying => history.IsReplaying;
        public string UndoText => history.UndoText;
        public string RedoText => history.RedoText;
        public string LastSkippedReason { get; private set; } = string.Empty;

        public DisplayLayerImageSnapshot Capture(int index, string title, Bitmap image)
        {
            if (!BitmapHistorySnapshot.TryCapture(image, policy, out BitmapHistorySnapshot snapshot, out string reason))
            {
                LastSkippedReason = reason;
                return null;
            }

            return new DisplayLayerImageSnapshot
            {
                Index = index,
                Title = title ?? string.Empty,
                Image = snapshot
            };
        }

        public DisplayLayerSnapshot CaptureLayer(
            int index,
            string title,
            Bitmap image,
            bool useClose,
            Rectangle roi,
            Rectangle trainRoi)
        {
            if (!BitmapHistorySnapshot.TryCapture(image, policy, out BitmapHistorySnapshot snapshot, out string reason))
            {
                LastSkippedReason = reason;
                return null;
            }

            return new DisplayLayerSnapshot
            {
                Exists = true,
                Index = index,
                Title = title ?? string.Empty,
                UseClose = useClose,
                Roi = roi,
                TrainRoi = trainRoi,
                Image = snapshot
            };
        }

        public void Record(string actionName, DisplayLayerImageSnapshot before, DisplayLayerImageSnapshot after)
        {
            if (IsReplaying || before == null || after == null)
            {
                return;
            }

            if (before.Image?.HasSameContent(after.Image) == true)
            {
                return;
            }

            string title = string.IsNullOrWhiteSpace(after.Title) ? before.Title : after.Title;
            string name = string.IsNullOrWhiteSpace(actionName)
                ? $"Layer Image: {title}"
                : actionName;

            history.Execute(new SnapshotCommand<DisplayLayerImageSnapshot>(name, before, after, restoreImage));
        }

        public void RecordLayer(string actionName, DisplayLayerSnapshot before, DisplayLayerSnapshot after)
        {
            if (IsReplaying || before == null || after == null)
            {
                return;
            }

            if (HasSameLayerState(before, after))
            {
                return;
            }

            string title = !string.IsNullOrWhiteSpace(after.Title) ? after.Title : before.Title;
            string name = string.IsNullOrWhiteSpace(actionName)
                ? $"Layer: {title}"
                : actionName;

            history.Execute(new SnapshotCommand<DisplayLayerSnapshot>(name, before, after, restoreLayer));
        }

        public bool Undo()
        {
            return history.Undo();
        }

        public bool Redo()
        {
            return history.Redo();
        }

        public void Clear()
        {
            history.Clear();
            LastSkippedReason = string.Empty;
        }

        private static bool HasSameLayerState(DisplayLayerSnapshot before, DisplayLayerSnapshot after)
        {
            if (before.Exists != after.Exists
                || before.Index != after.Index
                || !string.Equals(before.Title, after.Title, StringComparison.Ordinal)
                || before.UseClose != after.UseClose
                || before.Roi != after.Roi
                || before.TrainRoi != after.TrainRoi)
            {
                return false;
            }

            return before.Image?.HasSameContent(after.Image) == true;
        }
    }
}
