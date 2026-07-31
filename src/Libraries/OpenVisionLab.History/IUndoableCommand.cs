using System;

namespace OpenVisionLab.History
{
    public interface IUndoableCommand
    {
        string Name { get; }
        void Undo();
        void Redo();
    }

    public sealed class UndoRedoStateChangedEventArgs : EventArgs
    {
        public UndoRedoStateChangedEventArgs(bool canUndo, bool canRedo, string undoText, string redoText)
        {
            CanUndo = canUndo;
            CanRedo = canRedo;
            UndoText = undoText ?? string.Empty;
            RedoText = redoText ?? string.Empty;
        }

        public bool CanUndo { get; }
        public bool CanRedo { get; }
        public string UndoText { get; }
        public string RedoText { get; }
    }
}
