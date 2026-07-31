using System;
using System.Windows.Input;

namespace OpenVisionLab.Logging.Controls.Infrastructure
{
    public sealed class UiCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public UiCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
