using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineBatchHistory : Form
    {
        private readonly string recipeName;
        private readonly string pipelineName;
        private readonly List<VisionPipelineBatchSampleRunResult> visibleResults = new List<VisionPipelineBatchSampleRunResult>();
        private readonly BindingSource resultBinding = new BindingSource();

        private ListBox batchList;
        private DataGridView resultGrid;
        private DataGridView stepGrid;
        private DataGridView metricGrid;
        private TextBox detailsText;
        private PictureBox previewBox;
        private ComboBox cbResultFilter;
        private Button btnRefresh;
        private Button btnOpenFolder;
        private VisionPipelineBatchRunSummary currentSummary;
        private string currentBatchDirectory;
        private string currentReportDirectory;

        public FormVisionPipelineBatchHistory()
            : this("Default", "Pipeline")
        {
        }

        public FormVisionPipelineBatchHistory(string recipeName, string pipelineName)
        {
            this.recipeName = string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName;
            this.pipelineName = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName;

            InitializeComponent();
            VisionPipelineDialogStyle.Apply(this);
            if (!VisionPipelineDesignTime.IsActive)
            {
                RefreshBatches();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SetPreviewImage(null);
            base.OnFormClosed(e);
        }

        private void RefreshBatches()
        {
            List<VisionPipelineBatchRunSummaryStorage.BatchRunSummaryInfo> batches =
                VisionPipelineBatchRunSummaryStorage.List(recipeName, pipelineName);
            batchList.DataSource = batches;
            if (batches.Count > 0)
            {
                batchList.SelectedIndex = 0;
            }
            else
            {
                BindSummary(null, null);
            }
        }

        private void OnRefreshClicked(object sender, EventArgs e)
        {
            RefreshBatches();
        }

        private void OnOpenFolderClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentBatchDirectory) || !Directory.Exists(currentBatchDirectory))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = currentBatchDirectory,
                UseShellExecute = true
            });
        }

        private void OnResultFilterChanged(object sender, EventArgs e)
        {
            RefreshResultView();
        }

        private void OnBatchSelected(object sender, EventArgs e)
        {
            if (!(batchList.SelectedItem is VisionPipelineBatchRunSummaryStorage.BatchRunSummaryInfo summaryInfo))
            {
                BindSummary(null, null);
                return;
            }

            BindSummary(VisionPipelineBatchRunSummaryStorage.Load(summaryInfo.SummaryPath), summaryInfo.DirectoryPath);
        }

        private void BindSummary(VisionPipelineBatchRunSummary summary, string directory)
        {
            currentSummary = summary;
            currentBatchDirectory = directory;
            currentReportDirectory = null;
            stepGrid.DataSource = null;
            metricGrid.DataSource = null;
            SetPreviewImage(null);

            if (summary == null)
            {
                visibleResults.Clear();
                resultBinding.ResetBindings(false);
                detailsText.Text = "No batch summary.";
                return;
            }

            RefreshResultView();
            if (visibleResults.Count > 0)
            {
                resultGrid.Rows[0].Selected = true;
                BindSampleResult(visibleResults[0]);
            }
        }

        private void OnResultSelected(object sender, EventArgs e)
        {
            if (resultGrid.CurrentRow?.DataBoundItem is VisionPipelineBatchSampleRunResult result)
            {
                BindSampleResult(result);
            }
        }

        private void BindSampleResult(VisionPipelineBatchSampleRunResult sampleResult)
        {
            stepGrid.DataSource = null;
            metricGrid.DataSource = null;
            SetPreviewImage(null);

            if (sampleResult == null)
            {
                detailsText.Text = "No sample result.";
                return;
            }

            detailsText.Text =
                $"Sample: {sampleResult.SampleName}{Environment.NewLine}" +
                $"Status: {sampleResult.Status}   Success: {sampleResult.Success}   Total: {sampleResult.TotalMilliseconds:0.0} ms{Environment.NewLine}" +
                $"Message: {sampleResult.Message}{Environment.NewLine}" +
                $"Report: {sampleResult.ReportPath}";

            if (string.IsNullOrWhiteSpace(sampleResult.ReportPath) || !File.Exists(sampleResult.ReportPath))
            {
                currentReportDirectory = null;
                return;
            }

            currentReportDirectory = Path.GetDirectoryName(sampleResult.ReportPath);
            VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(sampleResult.ReportPath);
            if (report == null)
            {
                return;
            }

            stepGrid.DataSource = report.Steps;
            if (stepGrid.Rows.Count > 0)
            {
                stepGrid.Rows[0].Selected = true;
                BindStep(report.Steps[0]);
            }
        }

        private void OnStepSelected(object sender, EventArgs e)
        {
            if (stepGrid.CurrentRow?.DataBoundItem is VisionPipelineStepRunReport stepReport)
            {
                BindStep(stepReport);
            }
        }

        private void BindStep(VisionPipelineStepRunReport stepReport)
        {
            metricGrid.DataSource = stepReport?.Metrics;
            SetPreviewImage(null);

            if (stepReport == null
                || string.IsNullOrWhiteSpace(currentReportDirectory))
            {
                return;
            }

            string imageFile = ResolvePreviewImageFile(stepReport);
            if (string.IsNullOrWhiteSpace(imageFile))
            {
                return;
            }

            string path = Path.Combine(currentReportDirectory, imageFile);
            if (!File.Exists(path))
            {
                return;
            }

            using (Bitmap loaded = new Bitmap(path))
            {
                SetPreviewImage(new Bitmap(loaded));
            }
        }

        private static string ResolvePreviewImageFile(VisionPipelineStepRunReport stepReport)
        {
            if (!string.IsNullOrWhiteSpace(stepReport?.OverlayImageFile))
            {
                return stepReport.OverlayImageFile;
            }

            return stepReport?.ResultImageFile ?? string.Empty;
        }

        private void RefreshResultView()
        {
            visibleResults.Clear();
            if (currentSummary?.Results != null)
            {
                visibleResults.AddRange(currentSummary.Results.FindAll(MatchesResultFilter));
            }

            resultBinding.ResetBindings(false);
            stepGrid.DataSource = null;
            metricGrid.DataSource = null;
            SetPreviewImage(null);

            if (currentSummary == null)
            {
                detailsText.Text = "No batch summary.";
                return;
            }

            detailsText.Text =
                $"Pipeline: {currentSummary.PipelineName}{Environment.NewLine}" +
                $"Started: {currentSummary.StartedAt}{Environment.NewLine}" +
                $"Finished: {currentSummary.FinishedAt}{Environment.NewLine}" +
                $"Total: {currentSummary.TotalCount}   Pass: {currentSummary.PassCount}   Fail: {currentSummary.FailCount}   Elapsed: {currentSummary.TotalMilliseconds:0.0} ms{Environment.NewLine}" +
                BuildBatchSummaryText(visibleResults);
        }

        private bool MatchesResultFilter(VisionPipelineBatchSampleRunResult result)
        {
            if (result == null)
            {
                return false;
            }

            string filter = cbResultFilter?.SelectedItem?.ToString() ?? "All";
            switch (filter)
            {
                case "Failed":
                    return !result.Success;
                case "Passed":
                    return result.Success;
                case "Invalid":
                    return string.Equals(result.Status, "INVALID", StringComparison.OrdinalIgnoreCase);
                case "Error":
                    return string.Equals(result.Status, "ERROR", StringComparison.OrdinalIgnoreCase);
                case "Timeout":
                    return string.Equals(result.Status, "TIMEOUT", StringComparison.OrdinalIgnoreCase);
                case "Cancel":
                    return string.Equals(result.Status, "CANCEL", StringComparison.OrdinalIgnoreCase);
                default:
                    return true;
            }
        }

        private static string BuildBatchSummaryText(IEnumerable<VisionPipelineBatchSampleRunResult> results)
        {
            List<VisionPipelineBatchSampleRunResult> resultList = new List<VisionPipelineBatchSampleRunResult>(results ?? Array.Empty<VisionPipelineBatchSampleRunResult>());
            if (resultList.Count == 0)
            {
                return "Filtered Results: 0";
            }

            List<string> statusLines = new List<string>();
            foreach (IGrouping<string, VisionPipelineBatchSampleRunResult> group in resultList
                .GroupBy(result => string.IsNullOrWhiteSpace(result.Status) ? "UNKNOWN" : result.Status)
                .OrderBy(group => group.Key))
            {
                statusLines.Add($"{group.Key}: {group.Count()}");
            }

            List<string> failedStepLines = new List<string>();
            foreach (IGrouping<string, VisionPipelineBatchSampleRunResult> group in resultList
                .Where(result => !result.Success)
                .GroupBy(result => string.IsNullOrWhiteSpace(result.FailedStep) ? "(no step)" : result.FailedStep)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key))
            {
                failedStepLines.Add($"{group.Key}: {group.Count()}");
            }

            return
                $"Filtered Results: {resultList.Count}{Environment.NewLine}" +
                $"Status: {string.Join(", ", statusLines)}{Environment.NewLine}" +
                $"Failed Steps: {(failedStepLines.Count == 0 ? "None" : string.Join(", ", failedStepLines))}";
        }

        private void SetPreviewImage(Bitmap image)
        {
            Image previous = previewBox.Image;
            previewBox.Image = image;
            previous?.Dispose();
        }

    }
}
