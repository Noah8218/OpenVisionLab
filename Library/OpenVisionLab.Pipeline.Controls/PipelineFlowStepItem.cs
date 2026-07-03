using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenVisionLab.Pipeline.Controls
{
    public sealed class PipelineFlowStepItem : INotifyPropertyChanged
    {
        private bool isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Index { get; set; }

        public string Name { get; set; }

        public string ToolType { get; set; }

        public string InputLayer { get; set; }

        public string OutputLayer { get; set; }

        public string ExpectedInputLayer { get; set; }

        public string FlowStateText { get; set; }

        public bool IsBranch { get; set; }

        public string StatusText { get; set; }

        public PipelineFlowStepStatus Status { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool HasInputImage { get; set; }

        public bool HasOutputImage { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected == value)
                {
                    return;
                }

                isSelected = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
