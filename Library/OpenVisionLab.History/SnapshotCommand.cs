using System;

namespace OpenVisionLab.History
{
    public sealed class SnapshotCommand<TSnapshot> : IUndoableCommand
    {
        private readonly TSnapshot before;
        private readonly TSnapshot after;
        private readonly Action<TSnapshot> restore;

        public SnapshotCommand(string name, TSnapshot before, TSnapshot after, Action<TSnapshot> restore)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Command name is required.", nameof(name));
            }

            this.before = before;
            this.after = after;
            this.restore = restore ?? throw new ArgumentNullException(nameof(restore));
            Name = name;
        }

        public string Name { get; }

        public void Undo()
        {
            restore(before);
        }

        public void Redo()
        {
            restore(after);
        }
    }
}
