using OpenVisionLab.Logging.Controls.ViewModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenVisionLab.Logging.Controls.View
{
    public partial class LogPanelView : UserControl
    {
        private readonly LogPanelViewModel viewModel;
        private DispatcherOperation pendingScroll;
        private bool unloaded;

        public LogPanelView()
        {
            InitializeComponent();
            viewModel = new LogPanelViewModel(Clipboard.SetText);
            DataContext = viewModel;
            viewModel.FilteredLogs.CollectionChanged += FilteredLogs_CollectionChanged;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            SizeChanged += LogPanelView_SizeChanged;
            Unloaded += LogPanelView_Unloaded;
        }

        private void LogPanelView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            viewModel.IsCompactLayout = ActualHeight < 190 || ActualWidth < 720;
        }

        private void FilteredLogs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (unloaded || !viewModel.AutoScroll || LogList.Items.Count == 0)
            {
                CancelPendingScroll();
                return;
            }
            if (pendingScroll?.Status == DispatcherOperationStatus.Pending) { return; }
            pendingScroll = Dispatcher.BeginInvoke(new System.Action(() =>
            {
                pendingScroll = null;
                if (unloaded || !viewModel.AutoScroll || LogList.Items.Count == 0) { return; }
                LogList.ScrollIntoView(LogList.Items[LogList.Items.Count - 1]);
            }), DispatcherPriority.Background);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.AutoScroll) && !viewModel.AutoScroll) { CancelPendingScroll(); }
        }

        private void CancelPendingScroll()
        {
            pendingScroll?.Abort();
            pendingScroll = null;
        }

        private void LogPanelView_Unloaded(object sender, RoutedEventArgs e)
        {
            unloaded = true;
            CancelPendingScroll();
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            viewModel.FilteredLogs.CollectionChanged -= FilteredLogs_CollectionChanged;
            SizeChanged -= LogPanelView_SizeChanged;
            viewModel.Dispose();
        }
    }
}
