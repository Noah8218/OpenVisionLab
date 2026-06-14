using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineHistory : Form
    {
        private readonly string recipeName;
        private readonly string pipelineName;

        private ListBox runList;
        private DataGridView stepGrid;
        private DataGridView metricGrid;
        private TextBox detailsText;
        private PictureBox previewBox;
        private Button btnRefresh;
        private Button btnOpenFolder;
        private VisionPipelineRunReport currentReport;
        private string currentReportDirectory;

        public FormVisionPipelineHistory()
            : this("Default", "Pipeline")
        {
        }

        public FormVisionPipelineHistory(string recipeName, string pipelineName)
        {
            this.recipeName = string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName;
            this.pipelineName = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName;

            InitializeComponent();
            VisionPipelineDialogStyle.Apply(this);
            if (!VisionPipelineDesignTime.IsActive)
            {
                RefreshReports();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SetPreviewImage(null);
            base.OnFormClosed(e);
        }

        private void RefreshReports()
        {
            List<VisionPipelineRunReportStorage.RunReportInfo> reports = VisionPipelineRunReportStorage.List(recipeName, pipelineName);
            runList.DataSource = reports;
            if (reports.Count > 0)
            {
                runList.SelectedIndex = 0;
            }
            else
            {
                BindReport(null, null);
            }
        }

        private void OnRefreshClicked(object sender, EventArgs e)
        {
            RefreshReports();
        }

        private void OnOpenFolderClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentReportDirectory) || !Directory.Exists(currentReportDirectory))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = currentReportDirectory,
                UseShellExecute = true
            });
        }

        private void OnRunSelected(object sender, EventArgs e)
        {
            if (!(runList.SelectedItem is VisionPipelineRunReportStorage.RunReportInfo reportInfo))
            {
                BindReport(null, null);
                return;
            }

            BindReport(VisionPipelineRunReportStorage.Load(reportInfo.ReportPath), reportInfo.DirectoryPath);
        }

        private void BindReport(VisionPipelineRunReport report, string directory)
        {
            currentReport = report;
            currentReportDirectory = directory;
            SetPreviewImage(null);
            metricGrid.DataSource = null;

            if (report == null)
            {
                detailsText.Text = "No run report.";
                stepGrid.DataSource = null;
                return;
            }

            detailsText.Text =
                $"Pipeline: {report.PipelineName}{Environment.NewLine}" +
                $"Started: {report.StartedAt}{Environment.NewLine}" +
                $"Finished: {report.FinishedAt}{Environment.NewLine}" +
                $"Total: {report.TotalMilliseconds:0.0} ms   Success: {report.Success}   All Outputs: {report.PublishAllOutputs}";

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

            if (stepReport == null || string.IsNullOrWhiteSpace(currentReportDirectory))
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

        private void SetPreviewImage(Bitmap image)
        {
            Image previous = previewBox.Image;
            previewBox.Image = image;
            previous?.Dispose();
        }

    }
}
