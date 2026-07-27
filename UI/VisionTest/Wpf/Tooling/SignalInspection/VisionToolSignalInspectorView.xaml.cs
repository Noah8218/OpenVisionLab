using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    public partial class VisionToolSignalInspectorView : UserControl
    {
        private VisionToolSignalEvidence evidence;

        public VisionToolSignalInspectorView()
        {
            InitializeComponent();
            plotSurface.CursorValueChanged += PlotSurface_CursorValueChanged;
            plotSurface.MarkerValueChangeRequested += PlotSurface_MarkerValueChangeRequested;
            ApplyLocalization();
        }

        internal event EventHandler<VisionToolSignalMarkerValueChangedEventArgs> MarkerValueChangeRequested = delegate { };

        internal bool HasEvidence => evidence != null;

        internal string EvidenceId => evidence?.EvidenceId ?? string.Empty;

        internal string SourceSha256 => evidence?.SourceSha256 ?? string.Empty;

        internal int SeriesCount => evidence?.Series.Count ?? 0;

        internal int MarkerCount => evidence?.Markers.Count ?? 0;

        internal double GetMarkerValue(string markerId)
        {
            VisionToolSignalMarker marker = evidence?.Markers
                .FirstOrDefault(item => string.Equals(item.Id, markerId, StringComparison.Ordinal));
            return marker?.X ?? double.NaN;
        }

        internal void ShowEvidence(VisionToolSignalEvidence value)
        {
            evidence = value ?? throw new ArgumentNullException(nameof(value));
            plotSurface.SetEvidence(evidence);
            RebuildLegend();
            UpdateProvenance();
            Visibility = Visibility.Visible;
        }

        internal void ClearEvidence()
        {
            evidence = null;
            plotSurface.SetEvidence(null);
            legendPanel.Children.Clear();
            provenanceText.Text = string.Empty;
            Visibility = Visibility.Collapsed;
        }

        internal void ApplyLocalization()
        {
            bool korean = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean;
            titleText.Text = korean ? "신호 검사기" : "Signal Inspector";
            evidenceBadgeText.Text = korean ? "현재 미리보기 근거" : "Current Preview evidence";
            ((TextBlock)resetButton.Content).Text = korean ? "초기화" : "Reset";
            resetButton.ToolTip = korean ? "신호 확대/이동 상태 초기화" : "Reset signal zoom and pan";
            exportButton.ToolTip = korean ? "신호 근거를 TSV로 내보내기" : "Export signal evidence as TSV";
            cursorText.Text = korean
                ? "플롯에서 포인터를 움직여 값을 확인하십시오."
                : "Move pointer over plot to inspect values.";
            UpdateProvenance();
        }

        internal void ResetViewForTest()
        {
            plotSurface.ResetView();
        }

        internal bool ExerciseNavigationForTest()
        {
            return plotSurface.ExerciseNavigationForTest();
        }

        internal void CommitMarkerForTest(string markerId, double value)
        {
            plotSurface.CommitMarkerForTest(markerId, value);
        }

        internal void ExportForTest(string path)
        {
            if (evidence == null)
            {
                throw new InvalidOperationException("No current signal evidence is available.");
            }

            VisionToolSignalEvidenceExporter.ExportTsv(evidence, path);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            plotSurface.ResetView();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (evidence == null)
            {
                return;
            }

            bool korean = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean;
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = korean ? "신호 근거 내보내기" : "Export signal evidence",
                Filter = "Tab-separated values (*.tsv)|*.tsv|All files (*.*)|*.*",
                DefaultExt = ".tsv",
                AddExtension = true,
                FileName = "signal_evidence_" + evidence.EvidenceId.Substring(0, 12) + ".tsv"
            };
            if (dialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                VisionToolSignalEvidenceExporter.ExportTsv(evidence, dialog.FileName);
            }
        }

        private void PlotSurface_CursorValueChanged(object sender, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                cursorText.Text = value;
                return;
            }

            cursorText.Text = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                ? "플롯에서 포인터를 움직여 값을 확인하십시오."
                : "Move pointer over plot to inspect values.";
        }

        private void PlotSurface_MarkerValueChangeRequested(
            object sender,
            VisionToolSignalMarkerValueChangedEventArgs e)
        {
            MarkerValueChangeRequested(this, e);
        }

        private void RebuildLegend()
        {
            legendPanel.Children.Clear();
            foreach (VisionToolSignalSeries series in evidence.Series)
            {
                StackPanel item = new StackPanel
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center
                };
                item.Children.Add(new Border
                {
                    Width = 12,
                    Height = 3,
                    Margin = new Thickness(0, 0, 4, 0),
                    Background = (Brush)new BrushConverter().ConvertFromString(series.ColorHex),
                    VerticalAlignment = VerticalAlignment.Center
                });
                item.Children.Add(new TextBlock
                {
                    Text = series.Name,
                    FontSize = 10.5,
                    Foreground = new SolidColorBrush(Color.FromRgb(36, 48, 64)),
                    VerticalAlignment = VerticalAlignment.Center
                });
                legendPanel.Children.Add(item);
            }
        }

        private void UpdateProvenance()
        {
            if (evidence == null)
            {
                return;
            }

            bool korean = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean;
            string shortSourceHash = evidence.SourceSha256.Length > 12
                ? evidence.SourceSha256.Substring(0, 12)
                : evidence.SourceSha256;
            provenanceText.Text = korean
                ? $"도구 {evidence.ToolIdentity}  |  입력 {evidence.InputLayer}  |  영역 {evidence.RegionDescription}  |  원본 SHA-256 {shortSourceHash}…\n매개변수 {evidence.ParameterSummary}"
                : $"Tool {evidence.ToolIdentity}  |  Input {evidence.InputLayer}  |  Region {evidence.RegionDescription}  |  Source SHA-256 {shortSourceHash}…\nParameters {evidence.ParameterSummary}";
            if (!string.IsNullOrWhiteSpace(evidence.Guidance))
            {
                provenanceText.Text += Environment.NewLine + evidence.Guidance;
            }
        }
    }
}
