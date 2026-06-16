using Lib.Common;
using Lib.OpenCV.Pipeline;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using OpenVisionLab.MessageDialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineSamples : Form
    {
        private readonly string recipeName;
        private readonly string pipelineName;
        private readonly IDisplayManager displayManager;
        private List<VisionPipelineSampleCatalogItem> catalogSamples = new List<VisionPipelineSampleCatalogItem>();
        private string catalogCoverageSummaryText = "Catalog coverage: -";
        private readonly Dictionary<string, CatalogSampleCheckResult> catalogCheckResults =
            new Dictionary<string, CatalogSampleCheckResult>(StringComparer.OrdinalIgnoreCase);

        private TabControl tabSamples;
        private TabPage tabCatalog;
        private TabPage tabSaved;
        private ListBox catalogList;
        private TextBox catalogDetailsText;
        private ListBox sampleList;
        private TextBox detailsText;
        private Button btnOpenCatalog;
        private Button btnCheckCatalog;
        private Button btnSaveCurrent;
        private Button btnLoad;
        private Button btnDelete;
        private Button btnRefresh;

        public VisionPipelineSampleCatalogItem SelectedCatalogSample { get; private set; }

        public FormVisionPipelineSamples()
            : this("Default", "Pipeline", DisplayManagerService.Default)
        {
        }

        public FormVisionPipelineSamples(string recipeName, string pipelineName, IDisplayManager displayManager)
        {
            this.recipeName = string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName;
            this.pipelineName = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName;
            this.displayManager = displayManager ?? DisplayManagerService.Default;

            InitializeComponent();
            VisionPipelineDialogStyle.Apply(this);
            VisionPipelineDialogStyle.StyleButton(btnOpenCatalog, primary: true);
            VisionPipelineDialogStyle.StyleButton(btnCheckCatalog, primary: false);
            VisionPipelineDialogStyle.StyleButton(btnSaveCurrent, primary: true);
            if (!VisionPipelineDesignTime.IsActive)
            {
                RefreshCatalogSamples();
                RefreshSamples();
            }
        }

        private void RefreshCatalogSamples()
        {
            catalogSamples = VisionPipelineSampleCatalogItem.LoadRunnable();
            catalogCoverageSummaryText = BuildCatalogCoverageSummary();
            catalogList.DataSource = null;
            catalogList.DisplayMember = nameof(VisionPipelineSampleCatalogItem.CatalogListText);
            catalogList.DataSource = catalogSamples;
            if (catalogSamples.Count > 0)
            {
                catalogList.SelectedIndex = 0;
            }
            else
            {
                BindCatalogSample(null);
            }

            UpdateButtonStates();
        }

        private void RefreshSamples()
        {
            List<VisionPipelineSampleSetInfo> samples = VisionPipelineSampleSetStorage.List(recipeName, pipelineName);
            sampleList.DataSource = samples;
            if (samples.Count > 0)
            {
                sampleList.SelectedIndex = 0;
            }
            else
            {
                BindSample(null);
            }

            UpdateButtonStates();
        }

        private void OnTabChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void OnCatalogSampleSelected(object sender, EventArgs e)
        {
            BindCatalogSample(catalogList.SelectedItem as VisionPipelineSampleCatalogItem);
        }

        private void OnSampleSelected(object sender, EventArgs e)
        {
            BindSample(sampleList.SelectedItem as VisionPipelineSampleSetInfo);
        }

        private void BindCatalogSample(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                SetCatalogHeader("No sample", "EMPTY", false, "Select a recipe sample to load its image, pipeline, and expected result.", "Learn: -", "Expected: -");
                SetCatalogPreview(null);
                catalogDetailsText.Text = "No catalog sample.";
                UpdateButtonStates();
                return;
            }

            string imageState = sample.CanOpen ? "Ready" : "Missing file";
            CatalogSampleCheckResult checkResult = GetCatalogCheckResult(sample);
            string statusText = checkResult == null
                ? sample.CanOpen ? "READY" : "MISSING"
                : checkResult.Success ? "OK" : "NG";
            SetCatalogHeader(
                sample.SampleName,
                statusText,
                sample.CanOpen && (checkResult == null || checkResult.Success),
                sample.Goal,
                BuildCatalogLearningText(sample),
                BuildCatalogExpectedHeader(sample, checkResult));
            SetCatalogPreview(sample);

            string checkText = checkResult == null
                ? "Last check: Not run"
                : $"Last check: {checkResult.Status} | {checkResult.MetricText} | Final: {checkResult.FinalLayerText} | Overlays: {checkResult.OverlayCountText} | {checkResult.TotalMilliseconds:0.0} ms | {checkResult.CheckedAt:HH:mm:ss}";
            string failureText = checkResult == null || string.IsNullOrWhiteSpace(checkResult.FailedStepText)
                ? string.Empty
                : $"{Environment.NewLine}Failed step: {checkResult.FailedStepText}";
            string actionText = checkResult == null || string.IsNullOrWhiteSpace(checkResult.ActionSummaryText)
                ? string.Empty
                : $"{Environment.NewLine}Action: {checkResult.ActionSummaryText}";
            string stepText = checkResult == null || string.IsNullOrWhiteSpace(checkResult.StepSummaryText)
                ? string.Empty
                : $"{Environment.NewLine}Step flow: {checkResult.StepSummaryText}";
            string metricReviewText = checkResult == null || string.IsNullOrWhiteSpace(checkResult.MetricReviewText)
                ? "Metric review: Not run"
                : checkResult.MetricReviewText;

            catalogDetailsText.Text =
                $"{checkText}{failureText}{actionText}{stepText}{Environment.NewLine}" +
                $"{metricReviewText}{Environment.NewLine}" +
                $"{catalogCoverageSummaryText}{Environment.NewLine}" +
                $"Expected metric: {sample.ExpectedText}{Environment.NewLine}" +
                $"Recipe guide: {sample.RecipeGuideText}{Environment.NewLine}" +
                $"Category: {sample.Category}{Environment.NewLine}" +
                $"{BuildCatalogLearningText(sample)}{Environment.NewLine}" +
                $"Goal: {sample.Goal}{Environment.NewLine}" +
                $"Image: {sample.ImagePath} ({sample.Width} x {sample.Height}){Environment.NewLine}" +
                $"Reference: {BuildReferenceText(sample)}{Environment.NewLine}" +
                $"Pipeline: {sample.BaselinePipeline}{Environment.NewLine}" +
                $"Action: Open + Preview loads this image and recipe, then runs preview in Pipeline.{Environment.NewLine}" +
                $"Validation: {sample.ValidationMode}{Environment.NewLine}" +
                $"State: {imageState}{Environment.NewLine}" +
                $"{Environment.NewLine}{sample.Notes}" +
                (checkResult == null || string.IsNullOrWhiteSpace(checkResult.Message)
                    ? string.Empty
                    : $"{Environment.NewLine}{Environment.NewLine}Check message: {checkResult.Message}");

            UpdateButtonStates();
        }

        private static string BuildCatalogCoverageSummary()
        {
            List<VisionPipelineSampleFolderCoverageItem> coverage = VisionPipelineSampleCatalogItem.LoadFolderCoverage();
            if (coverage.Count == 0)
            {
                return "Catalog coverage: -";
            }

            int coveredCount = coverage.Count(item => item.IsCovered);
            List<string> backlogFolders = coverage
                .Where(item => !item.IsCovered && !string.Equals(item.Folder, ".", StringComparison.OrdinalIgnoreCase))
                .Select(item => $"{item.Folder}({item.ImageCount})")
                .ToList();

            string backlogText = backlogFolders.Count == 0
                ? "none"
                : string.Join(", ", backlogFolders);
            return $"Catalog coverage: {coveredCount}/{coverage.Count} folders covered | Backlog: {backlogText}";
        }

        private static string BuildCatalogExpectedHeader(
            VisionPipelineSampleCatalogItem sample,
            CatalogSampleCheckResult checkResult)
        {
            if (sample == null)
            {
                return "Expected: -";
            }

            if (checkResult == null)
            {
                return $"Expected: {sample.ExpectedText} | Pipeline: {Path.GetFileName(sample.BaselinePipeline)}";
            }

            return $"Expected: {sample.ExpectedText} | Last: {checkResult.Status} {checkResult.MetricText} | Final: {checkResult.FinalLayerText}";
        }

        private void SetCatalogHeader(string title, string state, bool ready, string goal, string learning, string expected)
        {
            if (catalogTitleLabel != null)
            {
                catalogTitleLabel.Text = string.IsNullOrWhiteSpace(title) ? "Sample" : title.Trim();
            }

            if (catalogStatusLabel != null)
            {
                catalogStatusLabel.Text = string.IsNullOrWhiteSpace(state) ? "-" : state.Trim();
                catalogStatusLabel.BackColor = ready
                    ? Color.FromArgb(0, 146, 92)
                    : Color.FromArgb(173, 96, 0);
            }

            if (catalogGoalLabel != null)
            {
                catalogGoalLabel.Text = string.IsNullOrWhiteSpace(goal) ? "-" : goal.Trim();
            }

            if (catalogLearningLabel != null)
            {
                catalogLearningLabel.Text = string.IsNullOrWhiteSpace(learning) ? "Learn: -" : learning.Trim();
            }

            if (catalogExpectedLabel != null)
            {
                catalogExpectedLabel.Text = string.IsNullOrWhiteSpace(expected) ? "Expected: -" : expected.Trim();
            }
        }

        private static string BuildCatalogLearningText(VisionPipelineSampleCatalogItem sample)
        {
            return sample?.LearningText ?? "Learn: -";
        }

        private void SetCatalogPreview(VisionPipelineSampleCatalogItem sample)
        {
            if (catalogPreviewBox == null || catalogReferenceBox == null)
            {
                return;
            }

            ClearPictureBoxImage(catalogPreviewBox);
            ClearPictureBoxImage(catalogReferenceBox);

            LoadPictureBoxImage(catalogPreviewBox, sample?.ImageFullPath);
            bool hasReferenceImage = LoadPictureBoxImage(catalogReferenceBox, sample?.ReferenceImageFullPath);
            UpdateReferencePreviewEmptyState(sample, hasReferenceImage);
        }

        private void UpdateReferencePreviewEmptyState(VisionPipelineSampleCatalogItem sample, bool hasReferenceImage)
        {
            if (catalogReferenceEmptyLabel == null)
            {
                return;
            }

            if (catalogReferenceBox != null)
            {
                catalogReferenceBox.Visible = hasReferenceImage;
                if (hasReferenceImage)
                {
                    catalogReferenceBox.BringToFront();
                }
            }

            catalogReferenceEmptyLabel.Visible = !hasReferenceImage;
            if (!catalogReferenceEmptyLabel.Visible)
            {
                catalogReferenceEmptyLabel.SendToBack();
                return;
            }

            catalogReferenceEmptyLabel.Text = sample == null
                ? "Select a sample"
                : string.IsNullOrWhiteSpace(sample.ReferenceImagePath)
                    ? "No expected result yet"
                    : "Expected result not found";
            catalogReferenceEmptyLabel.BringToFront();
        }

        private static bool LoadPictureBoxImage(PictureBox pictureBox, string path)
        {
            if (pictureBox == null
                || string.IsNullOrWhiteSpace(path)
                || !File.Exists(path))
            {
                return false;
            }

            try
            {
                using (Image image = Image.FromFile(path))
                {
                    pictureBox.Image = new Bitmap(image);
                }

                return true;
            }
            catch
            {
                pictureBox.Image = null;
                return false;
            }
        }

        private static void ClearPictureBoxImage(PictureBox pictureBox)
        {
            if (pictureBox == null)
            {
                return;
            }

            Image previous = pictureBox.Image;
            pictureBox.Image = null;
            previous?.Dispose();
        }

        private static string BuildReferenceText(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null || string.IsNullOrWhiteSpace(sample.ReferenceImagePath))
            {
                return "not defined";
            }

            return File.Exists(sample.ReferenceImageFullPath)
                ? sample.ReferenceImagePath
                : $"{sample.ReferenceImagePath} (missing)";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearPictureBoxImage(catalogPreviewBox);
            ClearPictureBoxImage(catalogReferenceBox);
            base.OnFormClosed(e);
        }

        private void BindSample(VisionPipelineSampleSetInfo sample)
        {
            if (sample == null)
            {
                detailsText.Text = "No sample set.";
                UpdateButtonStates();
                return;
            }

            detailsText.Text =
                $"Name: {sample.Name}{Environment.NewLine}" +
                $"Layers: {sample.LayerCount}{Environment.NewLine}" +
                $"Saved: {sample.SavedAt:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}" +
                $"Path: {sample.DirectoryPath}";

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool catalogActive = tabSamples?.SelectedTab == tabCatalog;
            bool savedActive = tabSamples?.SelectedTab == tabSaved;
            VisionPipelineSampleCatalogItem catalog = catalogList?.SelectedItem as VisionPipelineSampleCatalogItem;
            VisionPipelineSampleSetInfo sample = sampleList?.SelectedItem as VisionPipelineSampleSetInfo;

            if (btnOpenCatalog != null)
            {
                btnOpenCatalog.Visible = catalogActive;
                btnOpenCatalog.Left = 0;
                btnOpenCatalog.Enabled = catalogActive && catalog?.CanOpen == true;
            }

            if (btnCheckCatalog != null)
            {
                btnCheckCatalog.Visible = catalogActive;
                btnCheckCatalog.Left = 132;
                btnCheckCatalog.Enabled = catalogActive && catalog?.CanOpen == true;
            }

            if (btnSaveCurrent != null)
            {
                btnSaveCurrent.Visible = savedActive;
                btnSaveCurrent.Left = 0;
                btnSaveCurrent.Enabled = savedActive;
            }

            if (btnLoad != null)
            {
                btnLoad.Visible = savedActive;
                btnLoad.Left = 116;
                btnLoad.Enabled = savedActive && sample != null;
            }

            if (btnDelete != null)
            {
                btnDelete.Visible = savedActive;
                btnDelete.Left = 204;
                btnDelete.Enabled = savedActive && sample != null;
            }
        }

        private async void OnCheckCatalogClicked(object sender, EventArgs e)
        {
            if (!(catalogList.SelectedItem is VisionPipelineSampleCatalogItem sample) || !sample.CanOpen)
            {
                return;
            }

            btnCheckCatalog.Enabled = false;
            btnOpenCatalog.Enabled = false;
            string originalText = btnCheckCatalog.Text;
            btnCheckCatalog.Text = "Checking...";
            SetCatalogHeader(
                sample.SampleName,
                "RUN",
                true,
                sample.Goal,
                BuildCatalogLearningText(sample),
                $"Expected: {sample.ExpectedText} | Pipeline: {Path.GetFileName(sample.BaselinePipeline)}");

            try
            {
                CatalogSampleCheckResult result = await System.Threading.Tasks.Task.Run(() => RunCatalogSampleCheck(sample));
                catalogCheckResults[sample.SampleName] = result;
                BindCatalogSample(sample);
            }
            catch (Exception ex)
            {
                catalogCheckResults[sample.SampleName] = new CatalogSampleCheckResult
                {
                    Status = "ERROR",
                    Success = false,
                    Message = ex.GetBaseException().Message,
                    MetricText = "-",
                    MetricReviewText = "Metric review: check failed before metric evaluation.",
                    FinalLayerText = "-",
                    OverlayCountText = "-",
                    FailedStepText = "-",
                    ActionSummaryText = ex.GetBaseException().Message,
                    StepSummaryText = string.Empty,
                    CheckedAt = DateTime.Now
                };
                BindCatalogSample(sample);
            }
            finally
            {
                btnCheckCatalog.Text = originalText;
                UpdateButtonStates();
            }
        }

        private static CatalogSampleCheckResult RunCatalogSampleCheck(VisionPipelineSampleCatalogItem sample)
        {
            DateTime checkedAt = DateTime.Now;
            using (Bitmap bitmap = new Bitmap(sample.ImageFullPath))
            using (Mat source = BitmapImageConverter.ToMat(bitmap))
            using (VisionRecipeRunResult result = new VisionRecipeRunner()
                .RunAsync(sample.PipelineFullPath, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds)
                .GetAwaiter()
                .GetResult())
            {
                List<string> messages = new List<string>();
                if (!result.Success && !string.IsNullOrWhiteSpace(result.Message))
                {
                    messages.Add(result.Message);
                }

                if (sample.Width > 0
                    && sample.Height > 0
                    && (bitmap.Width != sample.Width || bitmap.Height != sample.Height))
                {
                    messages.Add($"Image size {bitmap.Width} x {bitmap.Height} does not match catalog {sample.Width} x {sample.Height}.");
                }

                bool success = result.Success;
                string metricText = "no metric gate";
                string metricReviewText = "Metric review: no metric gate";

                IReadOnlyList<VisionPipelineSampleExpectedMetric> expectedMetrics = sample.ExpectedMetrics;
                if (expectedMetrics.Count > 0)
                {
                    List<string> metricParts = new List<string>();
                    List<string> metricReviewLines = new List<string>();
                    foreach (VisionPipelineSampleExpectedMetric expectedMetric in expectedMetrics)
                    {
                        if (string.IsNullOrWhiteSpace(expectedMetric.Name))
                        {
                            continue;
                        }

                        string expectedRangeText = BuildExpectedMetricRangeText(expectedMetric);
                        if (!TryFindMetric(result, expectedMetric.Name, out double metricValue))
                        {
                            messages.Add($"Expected metric '{expectedMetric.Name}' was not produced.");
                            metricParts.Add($"{expectedMetric.Name}=missing");
                            metricReviewLines.Add($"{expectedMetric.Name}: expected {expectedRangeText}, actual missing, judgment MISSING");
                            continue;
                        }

                        metricParts.Add($"{expectedMetric.Name}={metricValue:0.###}");
                        bool metricPassed = true;
                        if (TryParseDouble(expectedMetric.Minimum, out double minimum) && metricValue < minimum)
                        {
                            messages.Add($"{expectedMetric.Name} {metricValue:0.###} < {minimum:0.###}.");
                            metricPassed = false;
                        }

                        if (TryParseDouble(expectedMetric.Maximum, out double maximum) && metricValue > maximum)
                        {
                            messages.Add($"{expectedMetric.Name} {metricValue:0.###} > {maximum:0.###}.");
                            metricPassed = false;
                        }

                        metricReviewLines.Add(
                            $"{expectedMetric.Name}: expected {expectedRangeText}, actual {metricValue:0.###}, judgment {(metricPassed ? "OK" : "NG")}");
                    }

                    metricText = metricParts.Count == 0 ? "no metric gate" : string.Join("; ", metricParts);
                    metricReviewText = metricReviewLines.Count == 0
                        ? "Metric review: no metric gate"
                        : "Metric review:" + Environment.NewLine + " - " + string.Join(Environment.NewLine + " - ", metricReviewLines);
                }

                success = success && messages.Count == 0;
                string message = messages.Count == 0
                    ? result.Message
                    : string.Join(" ", messages);

                return new CatalogSampleCheckResult
                {
                    Status = success ? "OK" : "NG",
                    Success = success,
                    Message = message,
                    MetricText = metricText,
                    MetricReviewText = metricReviewText,
                    FinalLayerText = string.IsNullOrWhiteSpace(result.FinalLayer) ? "-" : result.FinalLayer,
                    OverlayCountText = ResolveOverlayCountText(result),
                    FailedStepText = ResolveFailedStepText(result),
                    ActionSummaryText = result.ActionSummaryText,
                    StepSummaryText = result.StepSummaryText,
                    TotalMilliseconds = result.TotalMilliseconds,
                    CheckedAt = checkedAt
                };
            }
        }

        private static string BuildExpectedMetricRangeText(VisionPipelineSampleExpectedMetric expectedMetric)
        {
            if (expectedMetric == null)
            {
                return "-";
            }

            string minimum = string.IsNullOrWhiteSpace(expectedMetric.Minimum) ? string.Empty : expectedMetric.Minimum.Trim();
            string maximum = string.IsNullOrWhiteSpace(expectedMetric.Maximum) ? string.Empty : expectedMetric.Maximum.Trim();
            if (!string.IsNullOrWhiteSpace(minimum) && !string.IsNullOrWhiteSpace(maximum))
            {
                return string.Equals(minimum, maximum, StringComparison.OrdinalIgnoreCase)
                    ? minimum
                    : $"{minimum}..{maximum}";
            }

            if (!string.IsNullOrWhiteSpace(minimum))
            {
                return $">= {minimum}";
            }

            if (!string.IsNullOrWhiteSpace(maximum))
            {
                return $"<= {maximum}";
            }

            return "-";
        }

        private static string ResolveOverlayCountText(VisionRecipeRunResult result)
        {
            VisionRecipeStepRunSummary finalStep = result?.Steps?.LastOrDefault();
            if (finalStep == null)
            {
                return "-";
            }

            return finalStep.OverlayCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string ResolveFailedStepText(VisionRecipeRunResult result)
        {
            VisionRecipeStepRunSummary failedStep = result?.Steps?.FirstOrDefault(step => !step.Success);
            if (failedStep == null)
            {
                return string.Empty;
            }

            string message = string.IsNullOrWhiteSpace(failedStep.Message) ? string.Empty : $" - {failedStep.Message}";
            return $"{failedStep.Index:00} {failedStep.Name} [{failedStep.Status}]{message}";
        }

        private CatalogSampleCheckResult GetCatalogCheckResult(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null || string.IsNullOrWhiteSpace(sample.SampleName))
            {
                return null;
            }

            return catalogCheckResults.TryGetValue(sample.SampleName, out CatalogSampleCheckResult result)
                ? result
                : null;
        }

        private static bool TryFindMetric(VisionRecipeRunResult result, string metricName, out double value)
        {
            value = 0;
            foreach (VisionRecipeStepRunSummary step in result?.Steps?.AsEnumerable().Reverse() ?? Enumerable.Empty<VisionRecipeStepRunSummary>())
            {
                if (step.Metrics != null && step.Metrics.TryGetValue(metricName, out value))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseDouble(string text, out double value)
        {
            return double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value);
        }

        private void OnOpenCatalogClicked(object sender, EventArgs e)
        {
            if (!(catalogList.SelectedItem is VisionPipelineSampleCatalogItem sample) || !sample.CanOpen)
            {
                return;
            }

            SelectedCatalogSample = sample;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnSaveCurrentClicked(object sender, EventArgs e)
        {
            string defaultName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string sampleName = FormVisionPipelineTextPrompt.Show(this, "Save Sample", "Sample Name", defaultName);
            if (string.IsNullOrWhiteSpace(sampleName))
            {
                return;
            }

            try
            {
                int count = VisionPipelineSampleSetStorage.Save(recipeName, pipelineName, sampleName.Trim(), displayManager);
                RefreshSamples();
                SelectSample(sampleName.Trim());
                VisionMessageBox.Info(this, "Pipeline Samples", $"Sample saved. Layers: {count}");
            }
            catch (Exception ex)
            {
                VisionMessageBox.Error(this, "Pipeline Samples", ex.GetBaseException().Message, ex.ToString());
            }
        }

        private void OnLoadClicked(object sender, EventArgs e)
        {
            if (!(sampleList.SelectedItem is VisionPipelineSampleSetInfo sample))
            {
                return;
            }

            try
            {
                int count = VisionPipelineSampleSetStorage.Load(recipeName, pipelineName, sample.Name, displayManager);
                VisionMessageBox.Info(this, "Pipeline Samples", $"Sample loaded. Layers: {count}");
            }
            catch (Exception ex)
            {
                VisionMessageBox.Error(this, "Pipeline Samples", ex.GetBaseException().Message, ex.ToString());
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (!(sampleList.SelectedItem is VisionPipelineSampleSetInfo sample))
            {
                return;
            }

            DialogResult answer = VisionMessageBox.Confirm(
                this,
                "Pipeline Samples",
                $"Delete sample '{sample.Name}'?");
            if (answer != DialogResult.Yes)
            {
                return;
            }

            try
            {
                VisionPipelineSampleSetStorage.Delete(sample);
                RefreshSamples();
            }
            catch (Exception ex)
            {
                VisionMessageBox.Error(this, "Pipeline Samples", ex.GetBaseException().Message, ex.ToString());
            }
        }

        private void OnRefreshClicked(object sender, EventArgs e)
        {
            RefreshCatalogSamples();
            RefreshSamples();
        }

        private void SelectSample(string sampleName)
        {
            for (int i = 0; i < sampleList.Items.Count; i++)
            {
                if (sampleList.Items[i] is VisionPipelineSampleSetInfo sample
                    && string.Equals(sample.Name, sampleName, StringComparison.OrdinalIgnoreCase))
                {
                    sampleList.SelectedIndex = i;
                    return;
                }
            }
        }

        private sealed class CatalogSampleCheckResult
        {
            public string Status { get; set; } = string.Empty;
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string MetricText { get; set; } = string.Empty;
            public string MetricReviewText { get; set; } = string.Empty;
            public string FinalLayerText { get; set; } = string.Empty;
            public string OverlayCountText { get; set; } = string.Empty;
            public string FailedStepText { get; set; } = string.Empty;
            public string ActionSummaryText { get; set; } = string.Empty;
            public string StepSummaryText { get; set; } = string.Empty;
            public double TotalMilliseconds { get; set; }
            public DateTime CheckedAt { get; set; }
        }
    }
}
