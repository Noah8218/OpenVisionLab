using Lib.OpenCV.Pipeline;
using OpenVisionLab.MessageDialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab
{
	internal sealed partial class FormVisionPipelineBatch : Form
	{
		private const int StepTimeoutMilliseconds = 60000;

		private readonly string recipeName;
		private readonly VisionPipeline pipeline;
		private readonly List<VisionPipelineSampleSetInfo> sampleSets = new List<VisionPipelineSampleSetInfo>();
		private readonly List<VisionPipelineBatchSampleRunResult> batchResults = new List<VisionPipelineBatchSampleRunResult>();
		private readonly List<VisionPipelineBatchSampleRunResult> visibleBatchResults = new List<VisionPipelineBatchSampleRunResult>();
		private readonly BindingSource resultBinding = new BindingSource();

		private CheckedListBox sampleList;
		private DataGridView resultGrid;
		private DataGridView stepGrid;
		private DataGridView metricGrid;
		private TextBox detailsText;
		private PictureBox previewBox;
		private ComboBox cbResultFilter;
		private Label batchStatusLabel;
		private ProgressBar progressBar;
		private Button btnRefresh;
		private Button btnSelectAll;
		private Button btnClear;
		private Button btnRunSelected;
		private Button btnRunAll;
		private Button btnRunFailed;
		private Button btnCancel;
		private Button btnOpenSummary;
		private Button btnHistory;
		private CancellationTokenSource batchCancellationSource;
		private string currentSummaryPath;
		private string currentReportDirectory;

		public FormVisionPipelineBatch()
			: this("Default", new VisionPipeline { Name = "Pipeline" })
		{
		}

		public FormVisionPipelineBatch(string recipeName, VisionPipeline pipeline)
		{
			this.recipeName = string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName;
			this.pipeline = pipeline ?? new VisionPipeline { Name = "Pipeline" };

			InitializeComponent();
			VisionPipelineDialogStyle.Apply(this);
			VisionPipelineDialogStyle.StyleButton(btnRunSelected, primary: true);
			if (!VisionPipelineDesignTime.IsActive)
			{
				RefreshSamples();
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			batchCancellationSource?.Cancel();
			batchCancellationSource?.Dispose();
			SetPreviewImage(null);
			base.OnFormClosed(e);
		}

		private void RefreshSamples()
		{
			sampleSets.Clear();
			sampleSets.AddRange(VisionPipelineSampleSetStorage.List(recipeName, pipeline.Name));

			sampleList.Items.Clear();
			foreach (VisionPipelineSampleSetInfo sample in sampleSets)
			{
				sampleList.Items.Add(sample, true);
			}

			detailsText.Text = sampleSets.Count == 0
				? "No sample sets. Save sample sets from the Pipeline Samples window first."
				: $"Samples: {sampleSets.Count}";
		}

		private void OnRefreshClicked(object sender, EventArgs e)
		{
			RefreshSamples();
		}

		private void OnSelectAllClicked(object sender, EventArgs e)
		{
			for (int i = 0; i < sampleList.Items.Count; i++)
			{
				sampleList.SetItemChecked(i, true);
			}
		}

		private void OnClearClicked(object sender, EventArgs e)
		{
			for (int i = 0; i < sampleList.Items.Count; i++)
			{
				sampleList.SetItemChecked(i, false);
			}
		}

		private async void OnRunSelectedClicked(object sender, EventArgs e)
		{
			await RunBatchAsync(GetCheckedSamples());
		}

		private async void OnRunAllClicked(object sender, EventArgs e)
		{
			await RunBatchAsync(sampleSets.ToList());
		}

		private async void OnRunFailedClicked(object sender, EventArgs e)
		{
			HashSet<string> failedSampleNames = new HashSet<string>(
				batchResults.Where(result => !result.Success).Select(result => result.SampleName),
				StringComparer.OrdinalIgnoreCase);
			List<VisionPipelineSampleSetInfo> targets = sampleSets
				.Where(sample => failedSampleNames.Contains(sample.Name))
				.ToList();

			if (targets.Count == 0)
			{
				VisionMessageBox.Info(this, "Pipeline Batch", "No failed sample to run.");
				return;
			}

			await RunBatchAsync(targets);
		}

		private void OnResultFilterChanged(object sender, EventArgs e)
		{
			RefreshResultView();
		}

		private void OnCancelClicked(object sender, EventArgs e)
		{
			batchCancellationSource?.Cancel();
			detailsText.Text = "Cancel requested. Waiting for the current step to stop.";
			btnCancel.Enabled = false;
		}

		private void OnOpenSummaryClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(currentSummaryPath))
			{
				return;
			}

			string directory = Path.GetDirectoryName(currentSummaryPath);
			if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
			{
				return;
			}

			Process.Start(new ProcessStartInfo
			{
				FileName = directory,
				UseShellExecute = true
			});
		}

		private void OnHistoryClicked(object sender, EventArgs e)
		{
			using (FormVisionPipelineBatchHistory historyForm = new FormVisionPipelineBatchHistory(recipeName, pipeline.Name))
			{
				VisionPipelineDialogService.ShowDialog(historyForm, this);
			}
		}

		private async Task RunBatchAsync(List<VisionPipelineSampleSetInfo> targets)
		{
			if (targets == null || targets.Count == 0)
			{
				VisionMessageBox.Info(this, "Pipeline Batch", "No sample set selected.");
				return;
			}

			batchResults.Clear();
			RefreshResultView();
			stepGrid.DataSource = null;
			metricGrid.DataSource = null;
			SetPreviewImage(null);
			currentSummaryPath = null;
			currentReportDirectory = null;
			btnOpenSummary.Enabled = false;
			progressBar.Minimum = 0;
			progressBar.Maximum = Math.Max(1, targets.Count);
			progressBar.Value = 0;
			batchStatusLabel.Text = $"Ready to run {targets.Count} sample(s).";

			SetRunState(true);
			DateTime batchStartedAt = DateTime.Now;
			Stopwatch batchStopwatch = Stopwatch.StartNew();
			batchCancellationSource = new CancellationTokenSource();
			int completedCount = 0;

			try
			{
				foreach (VisionPipelineSampleSetInfo sample in targets)
				{
					if (batchCancellationSource.IsCancellationRequested)
					{
						break;
					}

					detailsText.Text = $"RUN | {sample.Name}";
					batchStatusLabel.Text = $"RUN | {sample.Name}";
					VisionPipelineBatchSampleRunResult result = await RunSampleAsync(sample, batchCancellationSource.Token);
					batchResults.Add(result);
					completedCount++;
					progressBar.Value = Math.Min(progressBar.Maximum, completedCount);
					RefreshResultView();
					SelectLastResult();
				}
			}
			finally
			{
				batchStopwatch.Stop();
				DateTime batchFinishedAt = DateTime.Now;
				batchCancellationSource?.Dispose();
				batchCancellationSource = null;
				SetRunState(false);

				try
				{
					currentSummaryPath = VisionPipelineBatchRunSummaryStorage.Save(
						recipeName,
						pipeline.Name,
						batchStartedAt,
						batchFinishedAt,
						batchResults);
					btnOpenSummary.Enabled = true;
					batchStatusLabel.Text = $"Finished | Pass {batchResults.Count(result => result.Success)} / {batchResults.Count}";
					detailsText.Text =
						BuildBatchSummaryText(batchResults) + Environment.NewLine +
						$"Batch finished. Total: {batchResults.Count}, Pass: {batchResults.Count(result => result.Success)}, Fail: {batchResults.Count(result => !result.Success)}{Environment.NewLine}" +
						$"Elapsed: {batchStopwatch.Elapsed.TotalMilliseconds:0.0} ms{Environment.NewLine}" +
						$"Summary: {currentSummaryPath}";
				}
				catch (Exception ex)
				{
					batchStatusLabel.Text = "Summary save failed.";
					detailsText.Text = $"Batch summary save failed: {ex.GetBaseException().Message}";
				}
			}
		}

		private async Task<VisionPipelineBatchSampleRunResult> RunSampleAsync(VisionPipelineSampleSetInfo sample, CancellationToken cancellationToken)
		{
			VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
				pipeline,
				VisionPipelineSampleSetStorage.GetLayerTitles(sample));
			if (!validation.Success)
			{
				return new VisionPipelineBatchSampleRunResult
				{
					SampleName = sample?.Name ?? string.Empty,
					Status = "INVALID",
					Success = false,
					Message = validation.FormatErrors()
				};
			}

			DateTime startedAt = DateTime.Now;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				using (VisionPipelineContext context = VisionPipelineSampleSetStorage.CreateContext(sample))
				{
					VisionPipelineRunResult runResult = await VisionPipelineExecutionService.RunAsync(
						pipeline,
						context,
						StepTimeoutMilliseconds,
						cancellationToken,
						update => UpdateStepProgress(sample, update));

					stopwatch.Stop();
					DateTime finishedAt = DateTime.Now;
					bool canceled = cancellationToken.IsCancellationRequested;
					bool success = !canceled && runResult.StepResults.Count > 0 && runResult.Success;
					VisionPipelineStepResult failedStep = VisionPipelineResultSummaryService.FindFirstFailedStep(runResult);
					VisionPipelineStepResultSummary failedSummary = VisionPipelineResultSummaryService.CreateStepSummary(0, failedStep);
					string reportPath = VisionPipelineRunReportStorage.Save(
						recipeName,
						pipeline,
						runResult,
						startedAt,
						finishedAt,
						publishAllOutputs: false,
						runLabel: sample?.Name);

					return new VisionPipelineBatchSampleRunResult
					{
						SampleName = sample?.Name ?? string.Empty,
						Status = canceled ? "CANCEL" : success ? "OK" : failedSummary.Status,
						Success = success,
						TotalMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
						FailedStep = failedStep?.Step?.Name ?? string.Empty,
						Message = canceled ? "Batch canceled." : failedSummary.Message,
						ReportPath = reportPath
					};
				}
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				return new VisionPipelineBatchSampleRunResult
				{
					SampleName = sample?.Name ?? string.Empty,
					Status = cancellationToken.IsCancellationRequested ? "CANCEL" : "ERROR",
					Success = false,
					TotalMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
					Message = ex.GetBaseException().Message
				};
			}
		}

		private void OnResultSelected(object sender, EventArgs e)
		{
			if (!(resultGrid.CurrentRow?.DataBoundItem is VisionPipelineBatchSampleRunResult result))
			{
				BindReport(null, null, null);
				return;
			}

			string reportPath = result.ReportPath;
			if (string.IsNullOrWhiteSpace(reportPath) || !File.Exists(reportPath))
			{
				BindReport(null, null, result);
				return;
			}

			BindReport(VisionPipelineRunReportStorage.Load(reportPath), Path.GetDirectoryName(reportPath), result);
		}

		private void BindReport(VisionPipelineRunReport report, string directory, VisionPipelineBatchSampleRunResult sampleResult)
		{
			currentReportDirectory = directory;
			stepGrid.DataSource = null;
			metricGrid.DataSource = null;
			SetPreviewImage(null);

			if (sampleResult == null)
			{
				detailsText.Text = "No batch result selected.";
				return;
			}

			detailsText.Text =
				$"Sample: {sampleResult.SampleName}{Environment.NewLine}" +
				$"Status: {sampleResult.Status}   Success: {sampleResult.Success}   Total: {sampleResult.TotalMilliseconds:0.0} ms{Environment.NewLine}" +
				$"Message: {sampleResult.Message}{Environment.NewLine}" +
				$"Report: {sampleResult.ReportPath}";

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

		private void UpdateStepProgress(VisionPipelineSampleSetInfo sample, VisionPipelineStepExecutionUpdate update)
		{
			if (IsDisposed)
			{
				return;
			}

			if (InvokeRequired)
			{
				BeginInvoke(new Action(() => UpdateStepProgress(sample, update)));
				return;
			}

			string sampleName = sample?.Name ?? string.Empty;
			string stepName = update?.Step?.Name ?? string.Empty;
			string status = update?.Status ?? string.Empty;
			batchStatusLabel.Text = $"{status} | {sampleName} | {stepName}";
			if (!string.IsNullOrWhiteSpace(update?.Message))
			{
				detailsText.Text = $"{status} | {sampleName} | {stepName}{Environment.NewLine}{update.Message}";
			}
		}

		private void RefreshResultView()
		{
			string selectedSample = (resultGrid.CurrentRow?.DataBoundItem as VisionPipelineBatchSampleRunResult)?.SampleName;

			visibleBatchResults.Clear();
			visibleBatchResults.AddRange(batchResults.Where(MatchesResultFilter));
			resultBinding.ResetBindings(false);
			UpdateResultSummaryText();

			if (!string.IsNullOrWhiteSpace(selectedSample))
			{
				SelectResultBySampleName(selectedSample);
			}
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

		private void UpdateResultSummaryText()
		{
			if (batchResults.Count == 0 || batchCancellationSource != null)
			{
				return;
			}

			detailsText.Text = BuildBatchSummaryText(batchResults);
		}

		private static string BuildBatchSummaryText(IEnumerable<VisionPipelineBatchSampleRunResult> results)
		{
			List<VisionPipelineBatchSampleRunResult> resultList = (results ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>()).ToList();
			if (resultList.Count == 0)
			{
				return "No batch result.";
			}

			IEnumerable<string> statusLines = resultList
				.GroupBy(result => string.IsNullOrWhiteSpace(result.Status) ? "UNKNOWN" : result.Status)
				.OrderBy(group => group.Key)
				.Select(group => $"{group.Key}: {group.Count()}");
			IEnumerable<string> failedStepLines = resultList
				.Where(result => !result.Success)
				.GroupBy(result => string.IsNullOrWhiteSpace(result.FailedStep) ? "(no step)" : result.FailedStep)
				.OrderByDescending(group => group.Count())
				.ThenBy(group => group.Key)
				.Select(group => $"{group.Key}: {group.Count()}");

			string failedSteps = string.Join(", ", failedStepLines);
			if (string.IsNullOrWhiteSpace(failedSteps))
			{
				failedSteps = "None";
			}

			return
				$"Summary: Total {resultList.Count}, Pass {resultList.Count(result => result.Success)}, Fail {resultList.Count(result => !result.Success)}{Environment.NewLine}" +
				$"Status: {string.Join(", ", statusLines)}{Environment.NewLine}" +
				$"Failed Steps: {failedSteps}";
		}

		private List<VisionPipelineSampleSetInfo> GetCheckedSamples()
		{
			return sampleList.CheckedItems
				.OfType<VisionPipelineSampleSetInfo>()
				.ToList();
		}

		private void SelectLastResult()
		{
			if (resultGrid.Rows.Count == 0)
			{
				return;
			}

			resultGrid.ClearSelection();
			int index = resultGrid.Rows.Count - 1;
			resultGrid.Rows[index].Selected = true;
			resultGrid.CurrentCell = resultGrid.Rows[index].Cells[0];
		}

		private void SelectResultBySampleName(string sampleName)
		{
			for (int i = 0; i < resultGrid.Rows.Count; i++)
			{
				if (resultGrid.Rows[i].DataBoundItem is VisionPipelineBatchSampleRunResult result
					&& string.Equals(result.SampleName, sampleName, StringComparison.OrdinalIgnoreCase))
				{
					resultGrid.ClearSelection();
					resultGrid.Rows[i].Selected = true;
					resultGrid.CurrentCell = resultGrid.Rows[i].Cells[0];
					return;
				}
			}
		}

		private void SetRunState(bool running)
		{
			sampleList.Enabled = !running;
			btnRefresh.Enabled = !running;
			btnSelectAll.Enabled = !running;
			btnClear.Enabled = !running;
			btnRunSelected.Enabled = !running;
			btnRunAll.Enabled = !running;
			btnRunFailed.Enabled = !running;
			cbResultFilter.Enabled = !running;
			btnHistory.Enabled = !running;
			btnCancel.Enabled = running;
			Cursor = running ? Cursors.WaitCursor : Cursors.Default;
		}

		private void SetPreviewImage(Bitmap image)
		{
			Image previous = previewBox.Image;
			previewBox.Image = image;
			previous?.Dispose();
		}

	}
}
