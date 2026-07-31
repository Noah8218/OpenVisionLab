using System;
using System.Collections.Generic;

namespace OpenVisionLab.History
{
    public sealed class UndoRedoManager
    {
        private readonly Stack<IUndoableCommand> undoStack = new Stack<IUndoableCommand>();
        private readonly Stack<IUndoableCommand> redoStack = new Stack<IUndoableCommand>();
        private readonly int capacity;
        private bool isReplaying;

        public UndoRedoManager()
            : this(100)
        {
        }

        public UndoRedoManager(int capacity)
        {
            this.capacity = Math.Max(1, capacity);
        }

        public event EventHandler<UndoRedoStateChangedEventArgs> StateChanged;

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;
        public bool IsReplaying => isReplaying;
        public string UndoText => CanUndo ? undoStack.Peek().Name : string.Empty;
        public string RedoText => CanRedo ? redoStack.Peek().Name : string.Empty;

        public void Execute(IUndoableCommand command)
        {
            if (command == null || isReplaying)
            {
                return;
            }

            undoStack.Push(command);
            TrimUndoStack();
            redoStack.Clear();
            RaiseStateChanged();
        }

        public bool Undo()
        {
            if (!CanUndo)
            {
                return false;
            }

            IUndoableCommand command = undoStack.Pop();
            Replay(command.Undo);
            redoStack.Push(command);
            RaiseStateChanged();
            return true;
        }

        public bool Redo()
        {
            if (!CanRedo)
            {
                return false;
            }

            IUndoableCommand command = redoStack.Pop();
            Replay(command.Redo);
            undoStack.Push(command);
            RaiseStateChanged();
            return true;
        }

        public void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();
            RaiseStateChanged();
        }

        private void Replay(Action action)
        {
            isReplaying = true;
            try
            {
                action();
            }
            finally
            {
                isReplaying = false;
            }
        }

        private void TrimUndoStack()
        {
            if (undoStack.Count <= capacity)
            {
                return;
            }

            IUndoableCommand[] items = undoStack.ToArray();
            undoStack.Clear();
            for (int i = Math.Min(items.Length, capacity) - 1; i >= 0; i--)
            {
                undoStack.Push(items[i]);
            }
        }

        private void RaiseStateChanged()
        {
            StateChanged?.Invoke(this, new UndoRedoStateChangedEventArgs(CanUndo, CanRedo, UndoText, RedoText));
        }
    }
}
