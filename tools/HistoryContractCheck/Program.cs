using System;
using System.Drawing;
using OpenVisionLab.History;

internal static class Program
{
    private static int Main()
    {
        try
        {
            CheckUndoRedoState();
            CheckPropertyChangeCommand();
            CheckSnapshotCommand();
            CheckPropertyGridUndoBinder();
            CheckBitmapHistorySnapshot();
            CheckImageSpaceInsertRemove();
            Console.WriteLine("HistoryContract=OK");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("HistoryContract=NG");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static void CheckUndoRedoState()
    {
        int value = 0;
        UndoRedoManager history = new UndoRedoManager(2);
        history.Execute(new SnapshotCommand<int>("A", 0, 1, next => value = next));
        history.Execute(new SnapshotCommand<int>("B", 1, 2, next => value = next));

        Assert(history.CanUndo, "Undo must be available after execute.");
        Assert(!history.CanRedo, "Redo must be cleared after execute.");
        Assert(history.UndoText == "B", "Latest command name must be shown as UndoText.");

        Assert(history.Undo(), "Undo must succeed.");
        Assert(value == 1, "Undo must restore previous state.");
        Assert(history.CanRedo, "Redo must be available after undo.");
        Assert(history.RedoText == "B", "RedoText must show undone command.");

        Assert(history.Redo(), "Redo must succeed.");
        Assert(value == 2, "Redo must restore next state.");
    }

    private static void CheckPropertyChangeCommand()
    {
        DummyTarget target = new DummyTarget { Count = 5, Mode = DummyMode.Normal };
        int afterApplyCount = 0;
        PropertyChangeCommand countCommand = new PropertyChangeCommand(
            target,
            nameof(DummyTarget.Count),
            5,
            "9",
            () => afterApplyCount++);

        countCommand.Redo();
        Assert(target.Count == 9, "PropertyChangeCommand must coerce string values to int.");
        countCommand.Undo();
        Assert(target.Count == 5, "PropertyChangeCommand must restore old int value.");

        PropertyChangeCommand modeCommand = new PropertyChangeCommand(
            target,
            nameof(DummyTarget.Mode),
            DummyMode.Normal,
            "Advanced",
            null);

        modeCommand.Redo();
        Assert(target.Mode == DummyMode.Advanced, "PropertyChangeCommand must coerce string values to enum.");
        modeCommand.Undo();
        Assert(target.Mode == DummyMode.Normal, "PropertyChangeCommand must restore old enum value.");
        Assert(afterApplyCount == 2, "AfterApply must run once per property apply.");
    }

    private static void CheckSnapshotCommand()
    {
        SnapshotState current = new SnapshotState("Before", 1);
        SnapshotCommand<SnapshotState> command = new SnapshotCommand<SnapshotState>(
            "Snapshot",
            new SnapshotState("Before", 1),
            new SnapshotState("After", 3),
            next => current = new SnapshotState(next.Name, next.Count));

        command.Redo();
        Assert(current.Name == "After" && current.Count == 3, "Snapshot redo must restore after state.");
        command.Undo();
        Assert(current.Name == "Before" && current.Count == 1, "Snapshot undo must restore before state.");
    }

    private static void CheckPropertyGridUndoBinder()
    {
        DummyTarget target = new DummyTarget { Count = 1 };
        FakePropertyGrid propertyGrid = new FakePropertyGrid { SelectedObject = target };
        UndoRedoManager history = new UndoRedoManager();
        int afterApplyCount = 0;

        using (new PropertyGridUndoBinder(propertyGrid, history, () => afterApplyCount++))
        {
            target.Count = 7;
            propertyGrid.RaisePropertyValueChanged(new FakeProperty(nameof(DummyTarget.Count)), target, 1, 7);
            Assert(history.CanUndo, "PropertyGridUndoBinder must record property changes.");

            Assert(history.Undo(), "PropertyGridUndoBinder undo must succeed.");
            Assert(target.Count == 1, "PropertyGridUndoBinder undo must restore old value.");
            Assert(afterApplyCount == 1, "PropertyGridUndoBinder undo must invoke afterApply.");

            Assert(history.Redo(), "PropertyGridUndoBinder redo must succeed.");
            Assert(target.Count == 7, "PropertyGridUndoBinder redo must restore new value.");
            Assert(afterApplyCount == 2, "PropertyGridUndoBinder redo must invoke afterApply.");

            propertyGrid.RaiseSelectedObjectsChanged();
            Assert(!history.CanUndo && !history.CanRedo, "PropertyGridUndoBinder must clear history when selected object changes.");
        }
    }

    private static void CheckBitmapHistorySnapshot()
    {
        using Bitmap bitmap = new Bitmap(4, 3);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Black);
            bitmap.SetPixel(2, 1, Color.FromArgb(10, 20, 30));
        }

        BitmapHistoryPolicy policy = new BitmapHistoryPolicy
        {
            MaxPixels = 100,
            MaxEncodedBytes = 1024 * 1024
        };

        Assert(BitmapHistorySnapshot.TryCapture(bitmap, policy, out BitmapHistorySnapshot snapshot, out string reason), $"Bitmap snapshot must be captured. {reason}");
        Assert(snapshot.HasImage, "Bitmap snapshot must contain image bytes.");
        using Bitmap restored = snapshot.ToBitmap();
        Assert(restored.Width == 4 && restored.Height == 3, "Bitmap snapshot restore must preserve size.");
        Color color = restored.GetPixel(2, 1);
        Assert(color.R == 10 && color.G == 20 && color.B == 30, "Bitmap snapshot restore must preserve pixel data.");

        Assert(BitmapHistorySnapshot.TryCapture(null, policy, out BitmapHistorySnapshot empty, out reason), $"Empty bitmap snapshot must be captured. {reason}");
        Assert(!empty.HasImage && empty.ToBitmap() == null, "Empty bitmap snapshot must restore to null.");

        BitmapHistoryPolicy smallPolicy = new BitmapHistoryPolicy { MaxPixels = 1, MaxEncodedBytes = 1024 * 1024 };
        Assert(!BitmapHistorySnapshot.TryCapture(bitmap, smallPolicy, out _, out reason), "Oversized bitmap snapshot must be rejected.");
        Assert(!string.IsNullOrWhiteSpace(reason), "Oversized bitmap snapshot must report rejection reason.");
    }

    private static void CheckImageSpaceInsertRemove()
    {
        OpenVisionLab.ImageSpace.Core.ImageSpaceService imageSpace = new OpenVisionLab.ImageSpace.Core.ImageSpaceService();
        using Bitmap main = new Bitmap(4, 4);
        using Bitmap layer = new Bitmap(8, 8);

        imageSpace.SetImage(0, "Main", main);
        imageSpace.InsertImage(0, "Layer", layer);

        Assert(ReferenceEquals(imageSpace.GetImage("Layer"), layer), "Inserted layer image must be addressable by title.");
        Assert(ReferenceEquals(imageSpace.GetImage(0), layer), "Inserted layer image must occupy requested index.");
        Assert(ReferenceEquals(imageSpace.GetImage(1), main), "Existing image must shift after insert.");

        imageSpace.RemoveImage("Layer");

        Assert(ReferenceEquals(imageSpace.GetImage(0), main), "Existing image must shift back after remove.");
        Assert(imageSpace.GetImage("Layer") == null, "Removed layer title must not remain addressable.");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class DummyTarget
    {
        public int Count { get; set; }
        public DummyMode Mode { get; set; }
    }

    private sealed class FakePropertyGrid : OpenVisionLab.PropertyGrid.IPropertyGridView
    {
        public event EventHandler<OpenVisionLab.PropertyGrid.PropertyGridPropertyValueChangedEventArgs> PropertyValueChanged;
        public event EventHandler SelectedObjectsChanged;

        public object SelectedObject { get; set; }
        public bool HasCategories => true;
        public OpenVisionLab.PropertyGrid.IPropertyGridPropertyCollection Properties => null;

        public void ApplyDisplayOptions(OpenVisionLab.PropertyGrid.PropertyGridDisplayOptions options)
        {
        }

        public void SetPropertyBrowsable(string propertyName, bool isBrowsable)
        {
        }

        public void RefreshSelectedObject()
        {
        }

        public void RaisePropertyValueChanged(OpenVisionLab.PropertyGrid.IPropertyGridProperty property, object targetObject, object oldValue, object newValue)
        {
            PropertyValueChanged?.Invoke(this, new OpenVisionLab.PropertyGrid.PropertyGridPropertyValueChangedEventArgs(property, targetObject, oldValue, newValue));
        }

        public void RaiseSelectedObjectsChanged()
        {
            SelectedObjectsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private sealed class FakeProperty : OpenVisionLab.PropertyGrid.IPropertyGridProperty
    {
        public FakeProperty(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public bool IsBrowsable { get; set; }

        public void SetValue(object value)
        {
        }
    }

    private enum DummyMode
    {
        Normal,
        Advanced
    }

    private sealed class SnapshotState
    {
        public SnapshotState(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public string Name { get; }
        public int Count { get; }
    }
}
