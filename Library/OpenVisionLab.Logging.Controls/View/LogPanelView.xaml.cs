using OpenVisionLab.Logging.Controls.ViewModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenVisionLab.Logging.Controls.View
{
    public partial class LogPanelView : UserControl
    {
        private readonly LogPanelViewModel viewModel;

        public LogPanelView()
        {
            InitializeComponent();
            viewModel = new LogPanelViewModel();
            DataContext = viewModel;
            viewModel.FilteredLogs.CollectionChanged += FilteredLogs_CollectionChanged;
            Unloaded += LogPanelView_Unloaded;
        }

        private void FilteredLogs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!viewModel.AutoScroll || LogList.Items.Count == 0)
            {
                return;
            }

            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                if (LogList.Items.Count == 0)
                {
                    return;
                }

                object lastItem = LogList.Items[LogList.Items.Count - 1];
                LogList.ScrollIntoView(lastItem);
            }), DispatcherPriority.Background);
        }

        private void LogPanelView_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.FilteredLogs.CollectionChanged -= FilteredLogs_CollectionChanged;
            viewModel.Dispose();
        }
    }
}
