using Lib.Common;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineLlmRecipe : Form
    {
        private readonly List<string> currentSourceLayers;
        private readonly Func<VisionPipelineContext> currentContextFactory;
        private TextBox tbOverview;
        private TextBox tbFlow;
        private TextBox tbFeedback;
        private TextBox tbPatch;
        private TextBox tbXml;
        private TextBox tbLog;
        private DataGridView validationGrid;
        private DataGridView stepGrid;
        private PictureBox previewBox;
        private Label imageStatusLabel;
        private Button btnPaste;
        private Button btnOpenXml;
        private Button btnSample;
        private Button btnPrompt;
        private Button btnValidate;
        private Button btnLoadImage;
        private Button btnUseCurrent;
        private Button btnRunPreview;
        private Button btnCopyFeedback;
        private Button btnCopyPatch;
        private Button btnApply;
        private Button btnClose;
        private Bitmap testImage;
        private Bitmap importedPreviewImage;
        private Bitmap previewRawImage;
        private List<VisionToolOverlay> previewOverlays = new List<VisionToolOverlay>();
        private VisionPipelineStepResultSummary previewSummary;
        private List<VisionPipelineStepResultSummary> latestRunSummaries = new List<VisionPipelineStepResultSummary>();
        private string previewTitle = "AI Preview";
        private VisionPipeline parsedPipeline;
        private IReadOnlyList<VisionPipelineNormalizationChange> latestNormalizationChanges = Array.Empty<VisionPipelineNormalizationChange>();
        private VisionPipelineValidationResult latestValidationResult = new VisionPipelineValidationResult();
        private string latestFeedbackText = string.Empty;
        private bool latestFeedbackHasRunResult;
        private bool latestValidationSuccess;
        private bool isBusy;

        public VisionPipeline ImportedPipeline { get; private set; }

        public FormVisionPipelineLlmRecipe()
            : this(new[] { "Main" }, () => new VisionPipelineContext())
        {
        }

        public FormVisionPipelineLlmRecipe(
            IEnumerable<string> sourceLayers,
            Func<VisionPipelineContext> currentContextFactory)
        {
            this.currentSourceLayers = NormalizeSourceLayers(sourceLayers);
            this.currentContextFactory = currentContextFactory;

            InitializeComponent();
            UpdateImageStatus();
            UpdateRecipeGuide();
            AppendLog("Paste an LLM-generated VisionPipeline XML, then Validate.");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            testImage?.Dispose();
            testImage = null;
            if (DialogResult != DialogResult.OK)
            {
                importedPreviewImage?.Dispose();
                importedPreviewImage = null;
            }

            SetPreviewImage(null);
            ClearPreviewResult();
            base.OnFormClosed(e);
        }

        public Bitmap TakeImportedPreviewImage()
        {
            Bitmap image = importedPreviewImage;
            importedPreviewImage = null;
            return image;
        }

        private void OnPasteClicked(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
            {
                AppendLog("PASTE NG | Clipboard has no text.");
                return;
            }

            tbXml.Text = Clipboard.GetText();
            MoveCaretToStart(tbXml);
            AppendLog("PASTE | Clipboard text loaded.");
        }

        private void OnOpenXmlClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Open LLM Recipe XML";
                dialog.Filter = "Pipeline XML (*.xml;*.pipeline.xml)|*.xml;*.pipeline.xml|All files (*.*)|*.*";
                dialog.CheckFileExists = true;
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                tbXml.Text = File.ReadAllText(dialog.FileName);
                MoveCaretToStart(tbXml);
                AppendLog($"OPEN XML | {Path.GetFileName(dialog.FileName)}");
            }
        }

        private void OnSampleClicked(object sender, EventArgs e)
        {
            string xmlPath = FindWorkspaceFile("docs", "samples", "Contour_TextSymbols.pipeline.xml");
            if (string.IsNullOrWhiteSpace(xmlPath) || !File.Exists(xmlPath))
            {
                AppendLog("SAMPLE NG | Contour_TextSymbols.pipeline.xml was not found.");
                return;
            }

            tbXml.Text = File.ReadAllText(xmlPath);
            MoveCaretToStart(tbXml);
            AppendLog($"SAMPLE | {Path.GetFileName(xmlPath)}");

            string imagePath = FindWorkspaceFile("Sample", "Contour.jpg");
            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                LoadPreviewImage(imagePath, "SAMPLE IMAGE");
            }

            ValidateXml(showLog: true);
        }

        private void OnPromptClicked(object sender, EventArgs e)
        {
            string defaultGoal = "Detect target objects and return boxes, metrics, and OK/NG criteria.";
            string goal = FormVisionPipelineTextPrompt.Show(this, "AI Recipe Prompt", "Inspection Goal", defaultGoal);
            if (string.IsNullOrWhiteSpace(goal))
            {
                return;
            }

            string prompt = BuildLlmPrompt(goal.Trim());
            using (FormVisionPipelinePromptPreview preview = new FormVisionPipelinePromptPreview(prompt))
            {
                if (VisionPipelineDialogService.ShowDialog(preview, this) == DialogResult.OK)
                {
                    Clipboard.SetText(preview.PromptText);
                    string feedbackNote = latestFeedbackHasRunResult ? " Latest Run Preview feedback included." : string.Empty;
                    AppendLog($"PROMPT | Copied AI Recipe request to clipboard.{feedbackNote}");
                }
                else
                {
                    AppendLog("PROMPT | Preview closed.");
                }
            }
        }

        private void OnValidateClicked(object sender, EventArgs e)
        {
            ValidateXml(showLog: true);
        }

        private void OnLoadImageClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Load Preview Image";
                dialog.Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All files (*.*)|*.*";
                dialog.CheckFileExists = true;
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    LoadPreviewImage(dialog.FileName, "IMAGE");
                }
                catch (Exception ex)
                {
                    AppendLog($"IMAGE NG | {ex.GetBaseException().Message}");
                }
            }
        }

        private void OnUseCurrentClicked(object sender, EventArgs e)
        {
            testImage?.Dispose();
            testImage = null;
            ClearPreviewResult();
            stepGrid.Rows.Clear();
            latestRunSummaries.Clear();
            UpdateImageStatus();
            UpdateRecipeGuide();
            AppendLog("IMAGE | Current display layers will be used.");
        }

        private async void OnRunPreviewClicked(object sender, EventArgs e)
        {
            if (!ValidateXml(showLog: true))
            {
                return;
            }

            SetBusy(true);
            stepGrid.Rows.Clear();
            latestRunSummaries.Clear();
            ClearPreviewResult();
            UpdateRecipeGuide();
            AppendLog("RUN | Preview started.");

            VisionPipelineRunResult runResult = null;
            try
            {
                using (VisionPipelineContext context = CreateRunContext())
                using (CancellationTokenSource cancellation = new CancellationTokenSource())
                {
                    runResult = await VisionPipelineExecutionService.RunAsync(
                        parsedPipeline,
                        context,
                        VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                        cancellation.Token);

                    if (IsUiClosing)
                    {
                        return;
                    }

                    PopulateRunResult(runResult);
                    CachePreviewResult(context, runResult);
                }

                latestFeedbackText = BuildLlmFeedback(parsedPipeline, runResult, latestValidationResult, latestNormalizationChanges, string.Empty);
                latestFeedbackHasRunResult = true;
                UpdateFeedbackButton();
                UpdateRecipeGuide(runResult, runResult?.Success == true ? "Run Preview OK" : "Run Preview NG");
                AppendLog($"{(runResult?.Success == true ? "RUN OK" : "RUN NG")} | {BuildRunSummary(runResult)}");
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                latestFeedbackText = BuildLlmFeedback(parsedPipeline, runResult, latestValidationResult, latestNormalizationChanges, message);
                latestFeedbackHasRunResult = true;
                UpdateFeedbackButton();
                UpdateRecipeGuide(runResult, "Run Preview ERROR");
                AppendLog($"RUN NG | {message}");
            }
            finally
            {
                if (!IsUiClosing && runResult != null)
                {
                    UpdateRecipeGuide(runResult, runResult.Success ? "Run Preview OK" : "Run Preview NG");
                    RefreshPatchPreview();
                }

                DisposeRunResultImages(runResult);
                SetBusy(false);
                UpdateFeedbackButton();
            }
        }

        private void OnApplyClicked(object sender, EventArgs e)
        {
            if (!ValidateXml(showLog: false))
            {
                return;
            }

            ImportedPipeline = ClonePipeline(parsedPipeline);
            importedPreviewImage?.Dispose();
            importedPreviewImage = testImage == null ? null : new Bitmap(testImage);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnCopyFeedbackClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(latestFeedbackText))
            {
                latestFeedbackText = BuildLlmFeedback(parsedPipeline, null, latestValidationResult, latestNormalizationChanges, "Run Preview has not been executed yet.");
            }

            try
            {
                Clipboard.SetText(latestFeedbackText);
                AppendLog("FEEDBACK | Copied AI tuning feedback to clipboard.");
            }
            catch (Exception ex)
            {
                AppendLog($"FEEDBACK NG | {ex.GetBaseException().Message}");
            }
        }

        private void OnCopyPatchClicked(object sender, EventArgs e)
        {
            string patchRequest = BuildPatchRequestText(GetPatchTargetSummary());
            if (string.IsNullOrWhiteSpace(patchRequest))
            {
                AppendLog("PATCH NG | Run Preview before copying a patch request.");
                return;
            }

            try
            {
                Clipboard.SetText(patchRequest);
                VisionPipelineStepResultSummary summary = GetPatchTargetSummary();
                string stepText = summary == null ? "-" : $"{summary.Index:00} {summary.Name}";
                AppendLog($"PATCH | Copied XML patch request. Step={stepText}");
            }
            catch (Exception ex)
            {
                AppendLog($"PATCH NG | {ex.GetBaseException().Message}");
            }
        }

        private void OnPreviewBoxDoubleClick(object sender, EventArgs e)
        {
            if (previewRawImage == null)
            {
                AppendLog("VIEW | Run Preview before opening overlay details.");
                return;
            }

            using (Bitmap clone = new Bitmap(previewRawImage))
            using (FormVisionPipelineImageViewer viewer = new FormVisionPipelineImageViewer(
                previewTitle,
                clone,
                previewOverlays,
                previewSummary,
                FormVision_Pipeline.OverlayLabelMode.Details,
                300))
            {
                VisionPipelineDialogService.ShowDialog(viewer, this);
            }
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void OnXmlTextChanged(object sender, EventArgs e)
        {
            parsedPipeline = null;
            ImportedPipeline = null;
            importedPreviewImage?.Dispose();
            importedPreviewImage = null;
            ClearPreviewResult();
            latestValidationSuccess = false;
            latestValidationResult = new VisionPipelineValidationResult();
            latestNormalizationChanges = Array.Empty<VisionPipelineNormalizationChange>();
            latestFeedbackText = string.Empty;
            latestFeedbackHasRunResult = false;
            latestRunSummaries.Clear();
            btnApply.Enabled = false;
            UpdateFeedbackButton();
            UpdateRecipeGuide();
        }

        private void OnStepGridSelectionChanged(object sender, EventArgs e)
        {
            RefreshPatchPreview();
        }

        private void RefreshPatchPreview()
        {
            if (IsUiClosing || tbPatch == null || latestRunSummaries == null || latestRunSummaries.Count == 0)
            {
                return;
            }

            tbPatch.Text = BuildPatchPreviewText();
            MoveCaretToStart(tbPatch);
            UpdateFeedbackButton();
        }

        private bool ValidateXml(bool showLog)
        {
            validationGrid.Rows.Clear();
            parsedPipeline = null;
            btnApply.Enabled = false;
            latestValidationSuccess = false;
            latestValidationResult = new VisionPipelineValidationResult();
            latestNormalizationChanges = Array.Empty<VisionPipelineNormalizationChange>();
            latestFeedbackText = string.Empty;
            latestFeedbackHasRunResult = false;
            latestRunSummaries.Clear();
            UpdateFeedbackButton();
            UpdateRecipeGuide();

            string xml = ExtractXmlPayload(tbXml.Text);
            if (!SerializeHelper.TryLoadFromXmlText(xml, out VisionPipeline pipeline, out string loadError) || pipeline == null)
            {
                AddValidationRow("Error", loadError);
                latestValidationResult.Errors.Add(loadError);
                latestFeedbackText = BuildLlmFeedback(null, null, latestValidationResult, latestNormalizationChanges, loadError);
                latestFeedbackHasRunResult = false;
                UpdateFeedbackButton();
                UpdateRecipeGuide(null, "Validation NG");
                if (showLog)
                {
                    AppendLog($"VALIDATE NG | {loadError}");
                }

                return false;
            }

            if (string.IsNullOrWhiteSpace(pipeline.Name))
            {
                pipeline.Name = $"AI_Recipe_{DateTime.Now:HHmmss}";
            }

            IReadOnlyList<VisionPipelineNormalizationChange> normalizationChanges = NormalizePipelineFlow(pipeline);
            latestNormalizationChanges = normalizationChanges.ToList();
            foreach (VisionPipelineNormalizationChange change in normalizationChanges)
            {
                AddValidationRow("Auto Fix", change.Message);
            }

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, GetValidationSourceLayers());
            latestValidationResult = validation;
            foreach (string error in validation.Errors)
            {
                AddValidationRow("Error", error);
            }

            foreach (string warning in validation.Warnings)
            {
                AddValidationRow("Warning", warning);
            }

            if (validation.Errors.Count == 0 && validation.Warnings.Count == 0)
            {
                AddValidationRow("OK", $"Pipeline '{pipeline.Name}' is valid. Steps={pipeline.Steps.Count}");
            }

            parsedPipeline = pipeline;
            latestValidationSuccess = validation.Success;
            latestFeedbackText = BuildLlmFeedback(pipeline, null, validation, latestNormalizationChanges, string.Empty);
            latestFeedbackHasRunResult = false;
            UpdateFeedbackButton();
            UpdateRecipeGuide(null, validation.Success ? "Validation OK" : "Validation NG");
            btnApply.Enabled = validation.Success;
            if (showLog)
            {
                foreach (VisionPipelineNormalizationChange change in normalizationChanges)
                {
                    AppendLog(change.Message);
                }

                AppendLog($"{(validation.Success ? "VALIDATE OK" : "VALIDATE NG")} | {pipeline.Name} | Steps={pipeline.Steps.Count} | Errors={validation.Errors.Count} | Warnings={validation.Warnings.Count}");
            }

            return validation.Success;
        }

        private static IReadOnlyList<VisionPipelineNormalizationChange> NormalizePipelineFlow(VisionPipeline pipeline)
        {
            return VisionPipelineNormalizer.NormalizeForRun(pipeline);
        }

        private VisionPipelineContext CreateRunContext()
        {
            if (testImage != null)
            {
                VisionPipelineContext context = new VisionPipelineContext();
                using (Mat mat = BitmapImageConverter.ToMat(testImage))
                {
                    context.SetLayer("Main", mat);
                }

                return context;
            }

            return currentContextFactory?.Invoke() ?? new VisionPipelineContext();
        }

        private IEnumerable<string> GetValidationSourceLayers()
        {
            if (testImage != null)
            {
                return new[] { "Main" };
            }

            return currentSourceLayers;
        }

        private void PopulateRunResult(VisionPipelineRunResult runResult)
        {
            stepGrid.Rows.Clear();
            latestRunSummaries = VisionPipelineResultSummaryService.CreateStepSummaries(runResult);
            int firstFailedRow = -1;
            foreach (VisionPipelineStepResultSummary summary in latestRunSummaries)
            {
                int rowIndex = stepGrid.Rows.Add(
                    summary.Index.ToString(CultureInfo.InvariantCulture),
                    $"{summary.Name} [{summary.ToolType}]",
                    summary.Status,
                    summary.ElapsedMilliseconds <= 0 ? "-" : $"{summary.ElapsedMilliseconds:0.0} ms",
                    summary.MetricsText);
                DataGridViewRow row = stepGrid.Rows[rowIndex];
                row.DefaultCellStyle.ForeColor = ResolveStatusColor(summary.Status);
                if (!summary.Success && firstFailedRow < 0)
                {
                    firstFailedRow = rowIndex;
                }

                if (!summary.Success)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 244, 238);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(206, 84, 64);
                    row.DefaultCellStyle.Font = new Font(stepGrid.Font, FontStyle.Bold);
                }
            }

            if (firstFailedRow >= 0 && firstFailedRow < stepGrid.Rows.Count)
            {
                stepGrid.ClearSelection();
                stepGrid.Rows[firstFailedRow].Selected = true;
                stepGrid.CurrentCell = stepGrid.Rows[firstFailedRow].Cells[0];
                stepGrid.FirstDisplayedScrollingRowIndex = firstFailedRow;
                VisionPipelineStepResultSummary failedSummary = latestRunSummaries[firstFailedRow];
                RefreshPatchPreview();
                AppendLog($"FOCUS | First failed step {failedSummary.Index:00} selected. Review AI Feedback for the suggested fix.");
            }
            else
            {
                RefreshPatchPreview();
            }
        }

        private void AddValidationRow(string type, string message)
        {
            int rowIndex = validationGrid.Rows.Add(type ?? string.Empty, message ?? string.Empty);
            DataGridViewRow row = validationGrid.Rows[rowIndex];
            row.DefaultCellStyle.ForeColor = ResolveStatusColor(type);
        }

        private static Color ResolveStatusColor(string text)
        {
            string value = text ?? string.Empty;
            if (value.IndexOf("OK", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(0, 128, 72);
            }

            if (value.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("NG", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(190, 32, 32);
            }

            if (value.IndexOf("WARN", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(196, 113, 0);
            }

            return Color.FromArgb(35, 85, 132);
        }

        private void CachePreviewResult(VisionPipelineContext context, VisionPipelineRunResult runResult)
        {
            VisionPipelineStepResult stepResult = ResolvePreviewStepResult(runResult);
            if (stepResult == null)
            {
                AppendLog("VIEW | Preview result has no step image.");
                return;
            }

            using (Mat resultImage = ResolveStepResultImage(context, stepResult))
            {
                if (resultImage == null || resultImage.Empty())
                {
                    AppendLog("VIEW | Preview result has no image.");
                    return;
                }

                using (Bitmap raw = BitmapImageConverter.ToBitmap(resultImage))
                {
                    previewRawImage = new Bitmap(raw);
                }
            }

            int index = Math.Max(0, runResult.StepResults.IndexOf(stepResult)) + 1;
            previewSummary = VisionPipelineResultSummaryService.CreateStepSummary(index, stepResult);
            previewTitle = string.IsNullOrWhiteSpace(stepResult.Step?.Name)
                ? "AI Preview"
                : stepResult.Step.Name;
            previewOverlays = CloneOverlays(stepResult.ToolResult?.Overlays);
            SetPreviewImage(RenderPreviewBitmap(previewRawImage, previewOverlays));
            AppendLog($"VIEW | {previewTitle} | Overlays={previewOverlays.Count}");
        }

        private static VisionPipelineStepResult ResolvePreviewStepResult(VisionPipelineRunResult runResult)
        {
            List<VisionPipelineStepResult> results = runResult?.StepResults ?? new List<VisionPipelineStepResult>();
            return results.LastOrDefault(result => result?.ToolResult?.ResultImage != null && !result.ToolResult.ResultImage.Empty())
                ?? results.LastOrDefault(result => result?.ToolResult?.Overlays?.Count > 0)
                ?? results.LastOrDefault();
        }

        private static Mat ResolveStepResultImage(VisionPipelineContext context, VisionPipelineStepResult stepResult)
        {
            string outputLayer = stepResult?.Step?.OutputLayer;
            if (!string.IsNullOrWhiteSpace(outputLayer))
            {
                Mat layerImage = context.GetLayer(outputLayer);
                if (layerImage != null && !layerImage.Empty())
                {
                    return layerImage;
                }

                layerImage?.Dispose();
            }

            Mat resultImage = stepResult?.ToolResult?.ResultImage;
            return resultImage != null && !resultImage.Empty()
                ? resultImage.Clone()
                : null;
        }

        private static List<VisionToolOverlay> CloneOverlays(IEnumerable<VisionToolOverlay> overlays)
        {
            List<VisionToolOverlay> clones = new List<VisionToolOverlay>();
            foreach (VisionToolOverlay overlay in overlays ?? Enumerable.Empty<VisionToolOverlay>())
            {
                if (overlay == null)
                {
                    continue;
                }

                VisionToolOverlay clone = new VisionToolOverlay
                {
                    Kind = overlay.Kind,
                    Label = overlay.Label,
                    Bounds = overlay.Bounds,
                    Center = overlay.Center,
                    Start = overlay.Start,
                    End = overlay.End,
                    Angle = overlay.Angle
                };
                clone.Points.AddRange(overlay.Points);
                clones.Add(clone);
            }

            return clones;
        }

        private static Bitmap RenderPreviewBitmap(Bitmap source, IEnumerable<VisionToolOverlay> overlays)
        {
            Bitmap preview = new Bitmap(source);
            List<VisionToolOverlay> overlayList = (overlays ?? Enumerable.Empty<VisionToolOverlay>()).Where(item => item != null).ToList();
            if (overlayList.Count == 0)
            {
                return preview;
            }

            using (Graphics graphics = Graphics.FromImage(preview))
            using (Pen boxPen = new Pen(Color.FromArgb(0, 210, 120), 2F))
            using (Pen centerPen = new Pen(Color.FromArgb(20, 185, 235), 2F))
            using (Brush pointBrush = new SolidBrush(Color.FromArgb(210, 0, 210, 120)))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (Brush textBackBrush = new SolidBrush(Color.FromArgb(210, 0, 120, 72)))
            using (Font font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                foreach (VisionToolOverlay overlay in overlayList)
                {
                    switch (overlay.Kind)
                    {
                        case VisionToolOverlayKind.Rectangle:
                            DrawRectangleOverlay(graphics, overlay, preview.Size, boxPen, centerPen, textBrush, textBackBrush, font);
                            break;
                        case VisionToolOverlayKind.Point:
                            DrawCenterMarker(graphics, overlay.Center, preview.Size, centerPen, pointBrush);
                            DrawOverlayLabel(graphics, FormVision_Pipeline.FormatOverlayLabel(overlay.Label, FormVision_Pipeline.OverlayLabelMode.Details), overlay.Center, preview.Size, textBrush, textBackBrush, font);
                            break;
                        case VisionToolOverlayKind.Points:
                            DrawPointOverlay(graphics, overlay, preview.Size, pointBrush, 300);
                            break;
                        case VisionToolOverlayKind.Line:
                            DrawLineOverlay(graphics, overlay, preview.Size, boxPen, centerPen, textBrush, textBackBrush, font);
                            break;
                    }
                }
            }

            return preview;
        }

        private static void DrawRectangleOverlay(
            Graphics graphics,
            VisionToolOverlay overlay,
            DrawingSize imageSize,
            Pen boxPen,
            Pen centerPen,
            Brush textBrush,
            Brush textBackBrush,
            Font font)
        {
            RectangleF bounds = ClampRectangle(overlay.Bounds, imageSize);
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                graphics.DrawRectangle(boxPen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }

            DrawCenterMarker(graphics, overlay.Center, imageSize, centerPen, null);
            DrawOverlayLabel(graphics, FormVision_Pipeline.FormatOverlayLabel(overlay.Label, FormVision_Pipeline.OverlayLabelMode.Details), new PointF(bounds.X, bounds.Y), imageSize, textBrush, textBackBrush, font);
        }

        private static void DrawLineOverlay(
            Graphics graphics,
            VisionToolOverlay overlay,
            DrawingSize imageSize,
            Pen linePen,
            Pen centerPen,
            Brush textBrush,
            Brush textBackBrush,
            Font font)
        {
            PointF start = ClampPoint(overlay.Start, imageSize);
            PointF end = ClampPoint(overlay.End, imageSize);
            graphics.DrawLine(linePen, start, end);
            PointF center = overlay.Center.IsEmpty
                ? new PointF((start.X + end.X) / 2F, (start.Y + end.Y) / 2F)
                : ClampPoint(overlay.Center, imageSize);
            DrawCenterMarker(graphics, center, imageSize, centerPen, null);
            DrawOverlayLabel(graphics, FormVision_Pipeline.FormatOverlayLabel(overlay.Label, FormVision_Pipeline.OverlayLabelMode.Details), center, imageSize, textBrush, textBackBrush, font);
        }

        private static void DrawPointOverlay(Graphics graphics, VisionToolOverlay overlay, DrawingSize imageSize, Brush pointBrush, int maxPointCount)
        {
            int count = 0;
            foreach (PointF point in overlay.Points)
            {
                if (count++ >= maxPointCount)
                {
                    break;
                }

                PointF clamped = ClampPoint(point, imageSize);
                graphics.FillEllipse(pointBrush, clamped.X - 1.5F, clamped.Y - 1.5F, 3F, 3F);
            }
        }

        private static void DrawCenterMarker(Graphics graphics, PointF center, DrawingSize imageSize, Pen pen, Brush brush)
        {
            PointF point = ClampPoint(center, imageSize);
            const float radius = 4F;
            graphics.DrawLine(pen, point.X - radius, point.Y, point.X + radius, point.Y);
            graphics.DrawLine(pen, point.X, point.Y - radius, point.X, point.Y + radius);
            if (brush != null)
            {
                graphics.FillEllipse(brush, point.X - 2F, point.Y - 2F, 4F, 4F);
            }
        }

        private static void DrawOverlayLabel(
            Graphics graphics,
            string label,
            PointF anchor,
            DrawingSize imageSize,
            Brush textBrush,
            Brush textBackBrush,
            Font font)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return;
            }

            PointF point = ClampPoint(anchor, imageSize);
            SizeF textSize = graphics.MeasureString(label, font);
            float x = Math.Min(Math.Max(point.X, 0), Math.Max(0, imageSize.Width - textSize.Width - 4));
            float y = Math.Max(0, point.Y - textSize.Height - 4);
            RectangleF background = new RectangleF(x, y, textSize.Width + 4, textSize.Height + 2);
            graphics.FillRectangle(textBackBrush, background);
            graphics.DrawString(label, font, textBrush, x + 2, y + 1);
        }

        private static RectangleF ClampRectangle(RectangleF rectangle, DrawingSize imageSize)
        {
            float x = Math.Max(0, Math.Min(rectangle.X, imageSize.Width));
            float y = Math.Max(0, Math.Min(rectangle.Y, imageSize.Height));
            float right = Math.Max(0, Math.Min(rectangle.Right, imageSize.Width));
            float bottom = Math.Max(0, Math.Min(rectangle.Bottom, imageSize.Height));
            return new RectangleF(x, y, Math.Max(0, right - x), Math.Max(0, bottom - y));
        }

        private static PointF ClampPoint(PointF point, DrawingSize imageSize)
        {
            return new PointF(
                Math.Max(0, Math.Min(point.X, imageSize.Width)),
                Math.Max(0, Math.Min(point.Y, imageSize.Height)));
        }

        private static void DisposeRunResultImages(VisionPipelineRunResult runResult)
        {
            foreach (VisionPipelineStepResult stepResult in runResult?.StepResults ?? Enumerable.Empty<VisionPipelineStepResult>())
            {
                stepResult?.ToolResult?.ResultImage?.Dispose();
            }
        }

        private static string BuildRunSummary(VisionPipelineRunResult runResult)
        {
            if (runResult == null || runResult.StepResults.Count == 0)
            {
                return "No step result.";
            }

            VisionPipelineStepResult failed = VisionPipelineResultSummaryService.FindFirstFailedStep(runResult);
            if (failed == null)
            {
                return $"Steps={runResult.StepResults.Count}";
            }

            string name = failed.Step?.Name ?? "Step";
            string message = VisionPipelineResultSummaryService.ResolveMessage(failed);
            return string.IsNullOrWhiteSpace(message) ? name : $"{name} | {message}";
        }

        private void SetPreviewImage(Bitmap image)
        {
            if (IsUiClosing || previewBox.IsDisposed)
            {
                image?.Dispose();
                return;
            }

            Image old = previewBox.Image;
            previewBox.Image = image;
            old?.Dispose();
        }

        private void ClearPreviewResult()
        {
            previewRawImage?.Dispose();
            previewRawImage = null;
            previewOverlays = new List<VisionToolOverlay>();
            previewSummary = null;
            previewTitle = "AI Preview";
            SetPreviewImage(null);
        }

        private void SetBusy(bool busy)
        {
            if (IsUiClosing)
            {
                return;
            }

            isBusy = busy;
            btnPaste.Enabled = !busy;
            btnOpenXml.Enabled = !busy;
            btnSample.Enabled = !busy;
            btnPrompt.Enabled = !busy;
            btnValidate.Enabled = !busy;
            btnLoadImage.Enabled = !busy;
            btnUseCurrent.Enabled = !busy;
            btnRunPreview.Enabled = !busy;
            btnCopyFeedback.Enabled = !busy && !string.IsNullOrWhiteSpace(latestFeedbackText);
            btnCopyPatch.Enabled = !busy && CanCopyPatchRequest();
            btnApply.Enabled = !busy && parsedPipeline != null && latestValidationSuccess;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void UpdateFeedbackButton()
        {
            if (IsUiClosing)
            {
                return;
            }

            if (btnCopyFeedback != null)
            {
                btnCopyFeedback.Enabled = !isBusy && !string.IsNullOrWhiteSpace(latestFeedbackText);
            }

            if (btnPrompt != null)
            {
                btnPrompt.Text = latestFeedbackHasRunResult ? "Retry Prompt" : "Build Prompt";
            }

            if (btnCopyPatch != null)
            {
                btnCopyPatch.Enabled = !isBusy && CanCopyPatchRequest();
            }
        }

        private void UpdateImageStatus()
        {
            if (testImage != null)
            {
                imageStatusLabel.Text = $"Preview image: Main ({testImage.Width} x {testImage.Height})";
                return;
            }

            imageStatusLabel.Text = $"Preview source: current layers ({string.Join(", ", currentSourceLayers.Take(4))})";
        }

        private void UpdateRecipeGuide(VisionPipelineRunResult runResult = null, string statusText = "")
        {
            if (IsUiClosing || tbOverview == null || tbFlow == null || tbFeedback == null || tbPatch == null)
            {
                return;
            }

            tbOverview.Text = BuildOverviewText(parsedPipeline, latestValidationResult, runResult, statusText);
            tbFlow.Text = BuildFlowText(parsedPipeline);
            tbFeedback.Text = BuildFeedbackPreviewText();
            tbPatch.Text = BuildPatchPreviewText();
            MoveCaretToStart(tbOverview);
            MoveCaretToStart(tbFlow);
            MoveCaretToStart(tbFeedback);
            MoveCaretToStart(tbPatch);
            UpdateFeedbackButton();
        }

        private static void MoveCaretToStart(TextBox textBox)
        {
            if (textBox == null || textBox.IsDisposed)
            {
                return;
            }

            textBox.SelectionStart = 0;
            textBox.SelectionLength = 0;
        }

        private string BuildOverviewText(
            VisionPipeline pipeline,
            VisionPipelineValidationResult validation,
            VisionPipelineRunResult runResult,
            string statusText)
        {
            string source = testImage == null
                ? $"Current layers: {string.Join(", ", currentSourceLayers.Take(4))}"
                : $"Preview image: Main ({testImage.Width} x {testImage.Height})";

            if (pipeline == null)
            {
                return string.Join(
                    Environment.NewLine,
                    "Status: Waiting for recipe XML",
                    source,
                    "",
                    "Use: Paste/Open XML or load Sample.",
                    "Next: Validate -> Run Preview -> Copy Feedback.");
            }

            string validationText = validation == null
                ? "-"
                : validation.Success
                    ? "OK"
                    : $"NG ({validation.Errors.Count} error)";
            string runText = runResult == null
                ? "-"
                : runResult.Success ? "OK" : "NG";
            string autoFixText = latestNormalizationChanges == null || latestNormalizationChanges.Count == 0
                ? "0"
                : latestNormalizationChanges.Count.ToString(CultureInfo.InvariantCulture);

            return string.Join(
                Environment.NewLine,
                $"Pipeline: {pipeline.Name}",
                $"Status: {(string.IsNullOrWhiteSpace(statusText) ? "Validated" : statusText)}",
                source,
                $"Steps: {pipeline.Steps.Count}",
                $"Validation: {validationText}",
                $"Run Preview: {runText}",
                $"Auto Fixes: {autoFixText}");
        }

        private string BuildFlowText(VisionPipeline pipeline)
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return string.Join(
                    Environment.NewLine,
                    "Validate a recipe to see the step chain.",
                    "",
                    "Expected flow:",
                    "01 Threshold | Main -> Binary",
                    "02 Morphology | Binary -> Clean",
                    "03 Contour | Clean -> Result");
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                string status = ResolveFlowStatus(i);
                builder.AppendLine($"{i + 1:00}. {status} {step?.Name ?? "Step"}");
                builder.AppendLine($"    {step?.ToolType ?? "-"} | {step?.InputLayer ?? "-"} -> {step?.OutputLayer ?? "-"}");
            }

            return builder.ToString().TrimEnd();
        }

        private string ResolveFlowStatus(int index)
        {
            if (latestRunSummaries == null || index < 0 || index >= latestRunSummaries.Count)
            {
                return "--";
            }

            string status = latestRunSummaries[index]?.Status ?? string.Empty;
            return string.IsNullOrWhiteSpace(status) ? "--" : status;
        }

        private string BuildFeedbackPreviewText()
        {
            if (string.IsNullOrWhiteSpace(latestFeedbackText))
            {
                return string.Join(
                    Environment.NewLine,
                    "Run Preview creates AI tuning feedback.",
                    "Copy Feedback sends validation, auto-fix, metrics, and failed-step context to the clipboard.",
                    "",
                    "Prompt will include the latest Run Preview feedback after a preview has run.");
            }

            string header = latestFeedbackHasRunResult
                ? "Ready for LLM retry:"
                : "Validation feedback:";
            List<string> priorityLines = BuildFeedbackPriorityLines(latestFeedbackText);
            return string.Join(Environment.NewLine, new[] { header }.Concat(priorityLines));
        }

        private static List<string> BuildFeedbackPriorityLines(string feedbackText)
        {
            List<string> sourceLines = (feedbackText ?? string.Empty)
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(line => line.TrimEnd())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
            List<string> lines = new List<string>();

            AddMatchingFeedbackLines(lines, sourceLines, "Preview Result:");
            AddMatchingFeedbackLines(lines, sourceLines, "First Failed Step:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Status:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Flow:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Direct Dependents:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Message:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Diagnostic:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Suggested Fix:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Patch Proposal:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Change Scope:");
            AddMatchingFeedbackLines(lines, sourceLines, "Final Review Contract:");
            AddMatchingFeedbackLines(lines, sourceLines, "- NG:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Final layer:");
            AddMatchingFeedbackLines(lines, sourceLines, "- MergeOverlayCount:");
            AddMatchingFeedbackLines(lines, sourceLines, "- MergeSourceCount:");
            AddMatchingFeedbackLines(lines, sourceLines, "Suggested Next LLM Request:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Change scope:");
            AddMatchingFeedbackLines(lines, sourceLines, "- Fix step ");
            AddMatchingFeedbackLines(lines, sourceLines, "- Add or repair a final OverlayMerge");

            if (lines.Count == 0)
            {
                lines.AddRange(sourceLines.Take(14));
                return lines;
            }

            lines.Add(string.Empty);
            lines.Add("Full feedback is copied by Copy AI Feedback.");
            return lines.Take(28).ToList();
        }

        private static void AddMatchingFeedbackLines(List<string> target, List<string> source, string prefix)
        {
            foreach (string line in source)
            {
                string trimmed = line.TrimStart();
                if (!trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                    || target.Any(existing => string.Equals(existing, line, StringComparison.Ordinal)))
                {
                    continue;
                }

                target.Add(line);
            }
        }

        private string BuildPatchPreviewText()
        {
            VisionPipelineStepResultSummary summary = GetPatchTargetSummary();
            if (summary == null)
            {
                return string.Join(
                    Environment.NewLine,
                    "Run Preview creates an XML Patch Request.",
                    "Select a failed step to focus the request.",
                    "Copy Patch Request asks the LLM to return a full VisionPipeline XML.");
            }

            List<string> lines = new List<string>
            {
                "XML Patch Request target:",
                FormatPatchStepTitle(summary),
                $"Status: {summary.Status}",
                $"Flow: {summary.InputLayer} -> {summary.OutputLayer}",
                $"Patch: {BuildRetryPatchProposal(summary)}",
                "",
                "Copy Patch Request includes the current step XML and asks for a full VisionPipeline XML."
            };

            return string.Join(Environment.NewLine, lines);
        }

        private string BuildPatchRequestText(VisionPipelineStepResultSummary summary)
        {
            if (parsedPipeline == null || summary == null)
            {
                return string.Empty;
            }

            VisionPipelineStep step = GetStepBySummary(summary);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("OpenVisionLab AI Recipe XML Patch Request");
            builder.AppendLine($"Pipeline: {parsedPipeline.Name}");
            builder.AppendLine($"Target Step: {FormatPatchStepTitle(summary)}");
            builder.AppendLine($"Status: {summary.Status}");
            builder.AppendLine($"Flow: {summary.InputLayer} -> {summary.OutputLayer}");
            builder.AppendLine($"Message: {(string.IsNullOrWhiteSpace(summary.Message) ? "-" : summary.Message)}");
            builder.AppendLine($"Diagnostic: {(string.IsNullOrWhiteSpace(summary.DiagnosticHint) ? "-" : summary.DiagnosticHint)}");
            builder.AppendLine($"Suggested Fix: {(string.IsNullOrWhiteSpace(summary.SuggestedFix) ? "-" : summary.SuggestedFix)}");
            builder.AppendLine($"Patch Proposal: {BuildRetryPatchProposal(summary)}");
            if (!string.IsNullOrWhiteSpace(summary.MetricsText))
            {
                builder.AppendLine($"Metrics: {summary.MetricsText}");
            }

            builder.AppendLine();
            builder.AppendLine("Patch Scope:");
            if (summary.Success)
            {
                builder.AppendLine("- This selected step is currently OK. Modify it only if the visual result has false positives or misses.");
            }
            else
            {
                builder.AppendLine("- Modify the target failed step and directly dependent steps only.");
            }

            builder.AppendLine("- Preserve successful previous steps and stable output layer names unless the layer flow is wrong.");
            builder.AppendLine("- Keep Main as the source image. Do not use Main as an output layer unless explicitly requested.");
            builder.AppendLine("- If this step is after preprocessing, verify whether InputLayer should be the previous OutputLayer instead of Main.");
            builder.AppendLine("- Do not remove final OverlayMerge review layers when branch detections must be visible together.");
            builder.AppendLine();
            builder.AppendLine("Current Step XML Reference:");
            builder.AppendLine(BuildStepXmlReference(step, summary));
            builder.AppendLine();
            builder.AppendLine("Return Format:");
            builder.AppendLine("- Return the full <VisionPipeline> XML that OpenVisionLab can paste/import directly.");
            builder.AppendLine("- Do not return only the step fragment.");
            builder.AppendLine("- Keep unsupported fields out of the XML.");
            builder.AppendLine("- After generating the XML, explain which fields changed and why in 3 lines or fewer.");
            return builder.ToString().TrimEnd();
        }

        private static string FormatPatchStepTitle(VisionPipelineStepResultSummary summary)
        {
            if (summary == null)
            {
                return "-";
            }

            string indexText = summary.Index.ToString("00", CultureInfo.InvariantCulture);
            string name = string.IsNullOrWhiteSpace(summary.Name) ? "Step" : summary.Name.Trim();
            if (name.StartsWith(indexText + " ", StringComparison.Ordinal)
                || name.StartsWith(indexText + ".", StringComparison.Ordinal))
            {
                name = name.Substring(indexText.Length).TrimStart(' ', '.', '-');
            }

            return $"{indexText}. {name} [{summary.ToolType}]";
        }

        private VisionPipelineStepResultSummary GetPatchTargetSummary()
        {
            if (latestRunSummaries == null || latestRunSummaries.Count == 0)
            {
                return null;
            }

            int selectedIndex = stepGrid?.CurrentRow?.Index ?? -1;
            if (selectedIndex >= 0 && selectedIndex < latestRunSummaries.Count)
            {
                return latestRunSummaries[selectedIndex];
            }

            return latestRunSummaries.FirstOrDefault(summary => summary != null && !summary.Success)
                ?? latestRunSummaries.LastOrDefault();
        }

        private bool CanCopyPatchRequest()
        {
            return latestFeedbackHasRunResult
                && parsedPipeline != null
                && GetPatchTargetSummary() != null;
        }

        private VisionPipelineStep GetStepBySummary(VisionPipelineStepResultSummary summary)
        {
            int index = (summary?.Index ?? 0) - 1;
            if (parsedPipeline?.Steps == null || index < 0 || index >= parsedPipeline.Steps.Count)
            {
                return null;
            }

            return parsedPipeline.Steps[index];
        }

        private static string BuildStepXmlReference(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<VisionPipelineStep>");
            AppendXmlElement(builder, 1, "Name", step?.Name ?? summary?.Name ?? string.Empty);
            AppendXmlElement(builder, 1, "ToolType", step?.ToolType ?? summary?.ToolType ?? string.Empty);
            AppendXmlElement(builder, 1, "Enabled", FormatXmlBool(step?.Enabled ?? true));
            AppendXmlElement(builder, 1, "InputLayer", step?.InputLayer ?? summary?.InputLayer ?? string.Empty);
            AppendXmlElement(builder, 1, "OutputLayer", step?.OutputLayer ?? summary?.OutputLayer ?? string.Empty);
            AppendXmlElement(builder, 1, "UseAcceptance", FormatXmlBool(step?.UseAcceptance ?? false));
            AppendXmlElement(builder, 1, "ExpectedSuccess", FormatXmlBool(step?.ExpectedSuccess ?? true));
            AppendXmlElement(builder, 1, "AcceptanceMetricName", step?.AcceptanceMetricName ?? string.Empty);
            AppendXmlElement(builder, 1, "UseAcceptanceMetricMinimum", FormatXmlBool(step?.UseAcceptanceMetricMinimum ?? false));
            AppendXmlElement(builder, 1, "AcceptanceMetricMinimum", (step?.AcceptanceMetricMinimum ?? 0).ToString("0.###", CultureInfo.InvariantCulture));
            AppendXmlElement(builder, 1, "UseAcceptanceMetricMaximum", FormatXmlBool(step?.UseAcceptanceMetricMaximum ?? false));
            AppendXmlElement(builder, 1, "AcceptanceMetricMaximum", (step?.AcceptanceMetricMaximum ?? 0).ToString("0.###", CultureInfo.InvariantCulture));
            builder.AppendLine("\t<Parameters>");
            foreach (KeyValuePair<string, string> parameter in (step?.Parameters ?? new Dictionary<string, string>()).OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
            {
                builder.AppendLine($"\t\t<Parameter Key=\"{EscapeXml(parameter.Key)}\" Value=\"{EscapeXml(parameter.Value)}\" />");
            }

            builder.AppendLine("\t</Parameters>");
            builder.AppendLine("</VisionPipelineStep>");
            return builder.ToString().TrimEnd();
        }

        private static void AppendXmlElement(StringBuilder builder, int indent, string name, string value)
        {
            builder.Append('\t', indent);
            builder.Append('<');
            builder.Append(name);
            builder.Append('>');
            builder.Append(EscapeXml(value));
            builder.Append("</");
            builder.Append(name);
            builder.AppendLine(">");
        }

        private static string EscapeXml(string value)
        {
            return System.Security.SecurityElement.Escape(value ?? string.Empty) ?? string.Empty;
        }

        private static string FormatXmlBool(bool value)
        {
            return value ? "true" : "false";
        }

        private void AppendLog(string message)
        {
            if (IsUiClosing || tbLog.IsDisposed)
            {
                return;
            }

            if (tbLog.TextLength > 0)
            {
                tbLog.AppendText(Environment.NewLine);
            }

            tbLog.AppendText(message ?? string.Empty);
            tbLog.SelectionStart = tbLog.TextLength;
            tbLog.ScrollToCaret();
        }

        private static string BuildLlmFeedback(
            VisionPipeline pipeline,
            VisionPipelineRunResult runResult,
            VisionPipelineValidationResult validation,
            IEnumerable<VisionPipelineNormalizationChange> normalizationChanges,
            string exceptionMessage)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("OpenVisionLab AI Recipe Feedback");
            builder.AppendLine($"Pipeline: {pipeline?.Name ?? "-"}");

            if (runResult != null)
            {
                builder.AppendLine($"Preview Result: {(runResult.Success ? "OK" : "NG")}");
                builder.AppendLine($"Executed Steps: {runResult.StepResults?.Count ?? 0}");
            }
            else if (validation != null)
            {
                builder.AppendLine($"Validation Result: {(validation.Success ? "OK" : "NG")}");
            }

            if (!string.IsNullOrWhiteSpace(exceptionMessage))
            {
                builder.AppendLine($"Exception: {exceptionMessage}");
            }

            AppendValidationFeedback(builder, validation);
            AppendNormalizationFeedback(builder, normalizationChanges);
            AppendPipelineFlowFeedback(builder, pipeline);
            AppendRunFeedback(builder, runResult);
            AppendFinalReviewFeedback(builder, runResult);
            AppendNextInstruction(builder, pipeline, runResult, validation, exceptionMessage);
            return builder.ToString().TrimEnd();
        }

        private static void AppendValidationFeedback(StringBuilder builder, VisionPipelineValidationResult validation)
        {
            if (validation == null)
            {
                return;
            }

            if (validation.Errors.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Validation Errors:");
                foreach (string error in validation.Errors)
                {
                    builder.AppendLine($"- {error}");
                }
            }

            if (validation.Warnings.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Validation Warnings:");
                foreach (string warning in validation.Warnings)
                {
                    builder.AppendLine($"- {warning}");
                }
            }
        }

        private static void AppendFinalReviewFeedback(StringBuilder builder, VisionPipelineRunResult runResult)
        {
            if (runResult == null)
            {
                return;
            }

            List<VisionPipelineStepResultSummary> summaries = VisionPipelineResultSummaryService.CreateStepSummaries(runResult);
            List<VisionPipelineStepResultSummary> reviewSteps = summaries
                .Where(step => IsOverlayReviewTool(step.ToolType) && step.OverlayCount > 0)
                .ToList();
            List<VisionPipelineStepResultSummary> mergeSteps = summaries
                .Where(step => VisionPipelineOverlayMergeService.IsMergeTool(step.ToolType))
                .ToList();

            builder.AppendLine();
            builder.AppendLine("Final Review Contract:");
            if (mergeSteps.Count == 0)
            {
                if (reviewSteps.Count >= 2)
                {
                    builder.AppendLine("- NG: Multiple inspection result steps produced overlays, but no final OverlayMerge step exists.");
                    builder.AppendLine("- Fix: keep the branch steps, then add a final OverlayMerge layer so the user can review all detections in one image.");
                }
                else if (reviewSteps.Count == 1)
                {
                    VisionPipelineStepResultSummary reviewStep = reviewSteps[0];
                    builder.AppendLine($"- OK: Single inspection result step '{reviewStep.Name}' writes '{reviewStep.OutputLayer}'. OverlayMerge is optional.");
                }
                else
                {
                    builder.AppendLine("- Review: no overlay-producing inspection result was detected in this preview.");
                }

                return;
            }

            VisionPipelineStepResultSummary finalMerge = mergeSteps.LastOrDefault();
            if (finalMerge == null)
            {
                return;
            }

            string mergeCount = finalMerge.Metrics.TryGetValue(VisionPipelineKnownMetrics.MergeOverlayCount, out double overlayCount)
                ? overlayCount.ToString("0", CultureInfo.InvariantCulture)
                : "-";
            string sourceCount = finalMerge.Metrics.TryGetValue(VisionPipelineKnownMetrics.MergeSourceCount, out double mergeSourceCount)
                ? mergeSourceCount.ToString("0", CultureInfo.InvariantCulture)
                : "-";
            builder.AppendLine($"- Final layer: {finalMerge.OutputLayer}");
            builder.AppendLine($"- MergeOverlayCount: {mergeCount}");
            builder.AppendLine($"- MergeSourceCount: {sourceCount}");

            if (!finalMerge.Success || !finalMerge.HasResultImage || finalMerge.OverlayCount <= 0)
            {
                builder.AppendLine("- NG: Final OverlayMerge did not produce a usable review image with overlays.");
                builder.AppendLine("- Fix: check SourceLayers, branch output layer names, and branch contour/blob/matching outputs.");
            }
            else if (summaries.LastOrDefault() != finalMerge)
            {
                builder.AppendLine("- Review: OverlayMerge produced a result, but it is not the final enabled step. Put the final review layer last when it is the user-facing answer.");
            }
            else
            {
                builder.AppendLine("- OK: Final OverlayMerge produced one review image for combined visual confirmation.");
            }
        }

        private static void AppendNormalizationFeedback(
            StringBuilder builder,
            IEnumerable<VisionPipelineNormalizationChange> normalizationChanges)
        {
            List<VisionPipelineNormalizationChange> changes = (normalizationChanges ?? Enumerable.Empty<VisionPipelineNormalizationChange>()).ToList();
            if (changes.Count == 0)
            {
                return;
            }

            builder.AppendLine();
            builder.AppendLine("OpenVisionLab Auto Fixes Applied:");
            foreach (VisionPipelineNormalizationChange change in changes)
            {
                builder.AppendLine($"- {change.Message}");
            }
        }

        private static void AppendPipelineFlowFeedback(StringBuilder builder, VisionPipeline pipeline)
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return;
            }

            builder.AppendLine();
            builder.AppendLine("Recipe Flow:");
            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                builder.AppendLine($"{i + 1:00}. {step?.Name ?? "Step"} [{step?.ToolType ?? "-"}] | {step?.InputLayer ?? "-"} -> {step?.OutputLayer ?? "-"}");
            }
        }

        private static void AppendRunFeedback(StringBuilder builder, VisionPipelineRunResult runResult)
        {
            if (runResult == null)
            {
                return;
            }

            List<VisionPipelineStepResultSummary> summaries = VisionPipelineResultSummaryService.CreateStepSummaries(runResult);
            builder.AppendLine();
            builder.AppendLine("Run Step Results:");
            foreach (VisionPipelineStepResultSummary summary in summaries)
            {
                string elapsed = summary.ElapsedMilliseconds <= 0
                    ? "-"
                    : $"{summary.ElapsedMilliseconds.ToString("0.0", CultureInfo.InvariantCulture)} ms";
                builder.AppendLine($"{summary.Index:00}. {summary.Status} | {summary.Name} [{summary.ToolType}] | {summary.InputLayer} -> {summary.OutputLayer} | {elapsed}");
                if (!string.IsNullOrWhiteSpace(summary.Message))
                {
                    builder.AppendLine($"    Message: {summary.Message}");
                }

                if (summary.IsToolError)
                {
                    builder.AppendLine($"    Tool Error: {summary.ErrorCode}:{summary.ErrorName} | ResultStatus={summary.ResultStatus}");
                }

                if (!string.IsNullOrWhiteSpace(summary.DiagnosticHint))
                {
                    builder.AppendLine($"    Diagnostic: {summary.DiagnosticHint}");
                }

                if (!string.IsNullOrWhiteSpace(summary.SuggestedFix))
                {
                    builder.AppendLine($"    Suggested Fix: {summary.SuggestedFix}");
                }

                if (!string.IsNullOrWhiteSpace(summary.MetricsText))
                {
                    builder.AppendLine($"    Metrics: {summary.MetricsText}");
                }

                if (summary.OverlayCount > 0 || summary.HasResultImage)
                {
                    builder.AppendLine($"    Visual: Overlays={summary.OverlayCount}, Image={summary.ResultImageSizeText}");
                }
            }

            VisionPipelineStepResult failed = VisionPipelineResultSummaryService.FindFirstFailedStep(runResult);
            if (failed != null)
            {
                VisionPipelineStepResultSummary failedSummary = VisionPipelineResultSummaryService.CreateStepSummary(
                    Math.Max(1, runResult.StepResults.IndexOf(failed) + 1),
                    failed);
                builder.AppendLine();
                builder.AppendLine("First Failed Step:");
                builder.AppendLine($"- {failedSummary.Index:00}. {failedSummary.Name} [{failedSummary.ToolType}]");
                builder.AppendLine($"- Status: {failedSummary.Status}");
                if (failedSummary.IsToolError)
                {
                    builder.AppendLine($"- Tool Error: {failedSummary.ErrorCode}:{failedSummary.ErrorName}");
                    builder.AppendLine($"- Result Status: {failedSummary.ResultStatus}");
                }
                builder.AppendLine($"- Flow: {failedSummary.InputLayer} -> {failedSummary.OutputLayer}");
                builder.AppendLine($"- Direct Dependents: {BuildDirectDependentStepText(summaries, failedSummary)}");
                builder.AppendLine($"- Message: {(string.IsNullOrWhiteSpace(failedSummary.Message) ? "-" : failedSummary.Message)}");
                builder.AppendLine($"- Diagnostic: {(string.IsNullOrWhiteSpace(failedSummary.DiagnosticHint) ? "-" : failedSummary.DiagnosticHint)}");
                builder.AppendLine($"- Suggested Fix: {(string.IsNullOrWhiteSpace(failedSummary.SuggestedFix) ? "-" : failedSummary.SuggestedFix)}");
                builder.AppendLine($"- Patch Proposal: {BuildRetryPatchProposal(failedSummary)}");
                if (!string.IsNullOrWhiteSpace(failedSummary.MetricsText))
                {
                    builder.AppendLine($"- Metrics: {failedSummary.MetricsText}");
                }

                builder.AppendLine("- Change Scope: keep successful previous steps unchanged. Modify this failed step and directly dependent steps only unless the flow itself is wrong.");
            }
        }

        private static void AppendNextInstruction(
            StringBuilder builder,
            VisionPipeline pipeline,
            VisionPipelineRunResult runResult,
            VisionPipelineValidationResult validation,
            string exceptionMessage)
        {
            builder.AppendLine();
            builder.AppendLine("Suggested Next LLM Request:");

            if (!string.IsNullOrWhiteSpace(exceptionMessage))
            {
                builder.AppendLine("- Fix the XML or step parameters so OpenVisionLab can run the preview without throwing an exception.");
                return;
            }

            if (validation?.Success == false)
            {
                builder.AppendLine("- Fix the validation errors first. Keep the same goal, but correct layer names, supported tool names, and parameter names.");
                return;
            }

            if (runResult == null)
            {
                builder.AppendLine("- Validate the recipe first, then run preview. If the target is missed, return this feedback with the image goal and ask for revised thresholds, ROI, morphology, or contour area limits.");
                return;
            }

            if (NeedsFinalOverlayMerge(runResult))
            {
                builder.AppendLine("- Add or repair a final OverlayMerge step so all branch detections are visible in one final review image.");
                builder.AppendLine("- Preserve successful branch steps and stable output layer names. Only connect their final result layers through OverlayMerge SourceLayers.");
                return;
            }

            if (runResult.Success)
            {
                builder.AppendLine("- The recipe runs successfully. If there are false positives, tighten ROI, contour area range, aspect ratio, or acceptance metrics while preserving the current layer chain.");
                return;
            }

            List<VisionPipelineStepResultSummary> summaries = VisionPipelineResultSummaryService.CreateStepSummaries(runResult);
            VisionPipelineStepResult failed = VisionPipelineResultSummaryService.FindFirstFailedStep(runResult);
            VisionPipelineStepResultSummary failedSummary = failed == null
                ? null
                : VisionPipelineResultSummaryService.CreateStepSummary(
                    Math.Max(1, runResult.StepResults.IndexOf(failed) + 1),
                    failed);
            AppendRetryScopeInstruction(builder, summaries, failedSummary);
            if (!string.IsNullOrWhiteSpace(failedSummary?.SuggestedFix))
            {
                builder.AppendLine($"- Fix step {failedSummary.Index:00} '{failedSummary.Name}': {failedSummary.SuggestedFix}");
                return;
            }

            string toolType = failed?.Step?.ToolType ?? string.Empty;
            if (string.Equals(toolType, "Contour", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "Blob", StringComparison.OrdinalIgnoreCase))
            {
                builder.AppendLine("- Adjust contour/blob area limits, threshold polarity, morphology cleanup, ROI, and ResultCount acceptance based on the failed step metrics.");
            }
            else if (string.Equals(toolType, "Threshold", StringComparison.OrdinalIgnoreCase))
            {
                builder.AppendLine("- Revise threshold mode/value/range/adaptive settings. Keep the output layer readable and feed the next preprocessing step from that output.");
            }
            else
            {
                builder.AppendLine("- Fix the failed step while preserving the successful previous steps and the input/output layer chain.");
            }
        }

        private static void AppendRetryScopeInstruction(
            StringBuilder builder,
            IReadOnlyList<VisionPipelineStepResultSummary> summaries,
            VisionPipelineStepResultSummary failedSummary)
        {
            if (failedSummary == null)
            {
                return;
            }

            builder.AppendLine($"- Change scope: keep successful previous steps unchanged. Modify step {failedSummary.Index:00} '{failedSummary.Name}' and directly dependent steps only unless the input/output flow itself is wrong.");
            builder.AppendLine($"- Direct dependents: {BuildDirectDependentStepText(summaries, failedSummary)}");
            builder.AppendLine($"- Patch proposal: {BuildRetryPatchProposal(failedSummary)}");
            builder.AppendLine("- Keep stable output layer names so saved previews, reports, and downstream steps remain comparable.");
        }

        private static string BuildRetryPatchProposal(VisionPipelineStepResultSummary failedSummary)
        {
            if (failedSummary == null)
            {
                return "Review the failed step parameters and input/output layer flow.";
            }

            string toolType = VisionPipelineNormalizer.NormalizeToolType(failedSummary.ToolType);
            string stepText = $"step {failedSummary.Index:00} '{failedSummary.Name}'";
            string xmlFields = BuildRetryPatchFieldText(toolType);
            string metricContext = BuildRetryPatchMetricText(failedSummary);
            string acceptanceContext = BuildRetryAcceptanceText(failedSummary);
            string suffix = $" XML fields: {xmlFields}. {metricContext}{acceptanceContext}";
            if (toolType == "threshold")
            {
                return $"Tune threshold mode/value/range on {stepText}; keep input '{failedSummary.InputLayer}' and output '{failedSummary.OutputLayer}' stable unless the layer flow is wrong.{suffix}";
            }

            if (toolType == "morphology")
            {
                return $"Tune kernel size, iteration count, and operator on {stepText}; verify it reads the previous clean binary layer.{suffix}";
            }

            if (toolType == "contour" || toolType == "blob")
            {
                return $"Tune area/count acceptance, ROI, and preprocessing input for {stepText}; avoid rewriting successful preprocessing steps first.{suffix}";
            }

            if (toolType == "line" || toolType == "linegauge")
            {
                return $"Tune edge polarity, scan region, threshold, and length/angle acceptance on {stepText}.{suffix}";
            }

            if (toolType == "matching" || toolType == "templatematching" || toolType == "feature" || toolType == "featurematching")
            {
                return $"Tune score threshold, ROI, template/source layer, and expected match count on {stepText}.{suffix}";
            }

            if (toolType == "overlaymerge" || toolType == "resultmerge")
            {
                return $"Check SourceLayers and branch output layer names on {stepText}; do not merge old or empty layers.{suffix}";
            }

            return $"Tune parameters and layer flow on {stepText}; preserve successful previous steps.{suffix}";
        }

        private static string BuildRetryPatchFieldText(string normalizedToolType)
        {
            string common = "InputLayer, OutputLayer, AcceptanceMetricName, AcceptanceMetricMinimum, AcceptanceMetricMaximum";
            switch (normalizedToolType)
            {
                case "threshold":
                    return $"{common}, Parameters.Mode, Parameters.Threshold, Parameters.RangeMin, Parameters.RangeMax, Parameters.ThresholdType, Parameters.Invert, Parameters.BlockSize, Parameters.Weight";
                case "morphology":
                    return $"{common}, Parameters.Operator, Parameters.KernelWidth, Parameters.KernelHeight, Parameters.Iterations";
                case "filter":
                    return $"{common}, Parameters.FilterType, Parameters.KernelWidth, Parameters.KernelHeight, Parameters.MedianKernelSize";
                case "edgedetection":
                    return $"{common}, Parameters.EdgeType, Parameters.CannyThresholdLow, Parameters.CannyThresholdHigh, Parameters.CannyApertureSize, Parameters.UseL2Gradient";
                case "blob":
                case "contour":
                    return $"{common}, Parameters.MIN_AREA, Parameters.MAX_AREA, Parameters.USE_ROI, Parameters.CvROI, Parameters.USE_THRESHOLD, Parameters.THRESHOLD, Parameters.THRESHOLD_TYPES";
                case "line":
                case "linegauge":
                    return $"{common}, Parameters.PRJ_DIR, Parameters.PRJ_PORALITY, Parameters.CONTRAST, Parameters.THICKNESS, Parameters.SAMPLING_STEP, Parameters.USE_ROI, Parameters.CvROI";
                case "matching":
                case "templatematching":
                case "feature":
                case "featurematching":
                case "sift":
                    return $"{common}, Parameters.SCORE_MIN, Parameters.PATTERN_PATH, Parameters.MATCH_MODE, Parameters.USE_ROI, Parameters.CvROI";
                case "overlaymerge":
                case "resultmerge":
                case "mergeresult":
                    return $"{common}, Parameters.SourceLayers";
                case "rotatescale":
                    return $"{common}, Parameters.Angle, Parameters.ScaleXPercent, Parameters.ScaleYPercent, Parameters.Interpolation";
                case "mean":
                    return $"{common}, Parameters.USE_ROI, Parameters.CvROI";
                default:
                    return $"{common}, Parameters";
            }
        }

        private static string BuildRetryPatchMetricText(VisionPipelineStepResultSummary failedSummary)
        {
            if (failedSummary?.Metrics == null || failedSummary.Metrics.Count == 0)
            {
                return "Metric context: no metrics were produced, so first check input layer, ROI, and tool parameter validity.";
            }

            string metrics = string.Join(
                ", ",
                VisionPipelineKnownMetrics.OrderMetrics(failedSummary.Metrics)
                    .Take(6)
                    .Select(metric => $"{VisionPipelineKnownMetrics.GetDisplayName(metric.Key)}={metric.Value.ToString("0.###", CultureInfo.InvariantCulture)}"));
            return $"Metric context: {metrics}.";
        }

        private static string BuildRetryAcceptanceText(VisionPipelineStepResultSummary failedSummary)
        {
            if (failedSummary == null || !failedSummary.IsAcceptanceNg)
            {
                return string.Empty;
            }

            string message = failedSummary.Message ?? string.Empty;
            if (message.IndexOf("below target", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return " Acceptance hint: the measured value is too low; either tune detection to produce more/stronger results or lower the minimum only if the visual result is acceptable.";
            }

            if (message.IndexOf("above target", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return " Acceptance hint: the measured value is too high; tighten ROI/area/score/threshold filters or raise the maximum only if the visual result is acceptable.";
            }

            return " Acceptance hint: compare the failed metric with the visual overlay before changing pass/fail limits.";
        }

        private static bool NeedsFinalOverlayMerge(VisionPipelineRunResult runResult)
        {
            if (runResult == null)
            {
                return false;
            }

            List<VisionPipelineStepResultSummary> summaries = VisionPipelineResultSummaryService.CreateStepSummaries(runResult);
            bool hasMergeStep = summaries.Any(summary => VisionPipelineOverlayMergeService.IsMergeTool(summary.ToolType));
            if (hasMergeStep)
            {
                return false;
            }

            int reviewStepCount = summaries.Count(summary =>
                IsOverlayReviewTool(summary.ToolType)
                && summary.OverlayCount > 0);
            return reviewStepCount >= 2;
        }

        private static string BuildDirectDependentStepText(
            IReadOnlyList<VisionPipelineStepResultSummary> summaries,
            VisionPipelineStepResultSummary failedSummary)
        {
            if (summaries == null || failedSummary == null || string.IsNullOrWhiteSpace(failedSummary.OutputLayer))
            {
                return "none";
            }

            List<string> dependents = summaries
                .Where(summary => summary != null
                    && summary.Index > failedSummary.Index
                    && string.Equals(summary.InputLayer, failedSummary.OutputLayer, StringComparison.OrdinalIgnoreCase))
                .Select(summary => $"{summary.Index:00} {summary.Name} [{summary.ToolType}]")
                .ToList();

            return dependents.Count == 0 ? "none" : string.Join(", ", dependents);
        }

        private static bool IsOverlayReviewTool(string toolType)
        {
            string normalized = VisionPipelineNormalizer.NormalizeToolType(toolType);
            return normalized == "blob"
                || normalized == "contour"
                || normalized == "line"
                || normalized == "linegauge"
                || normalized == "matching"
                || normalized == "templatematching"
                || normalized == "feature"
                || normalized == "featurematching"
                || normalized == "sift";
        }

        private bool IsUiClosing => IsDisposed || Disposing;

        private string BuildLlmPrompt(string goal)
        {
            List<string> layers = GetValidationSourceLayers()
                .Where(layer => !string.IsNullOrWhiteSpace(layer))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            string layerText = layers.Count == 0 ? "Main" : string.Join(", ", layers);
            string imageText = testImage == null
                ? "Use the current OpenVisionLab display layers."
                : $"Preview image loaded as Main ({testImage.Width} x {testImage.Height}).";

            string prompt = string.Join(
                Environment.NewLine,
                "You are generating an OpenVisionLab VisionPipeline XML recipe.",
                "",
                "Goal:",
                $"- Detect: {goal}",
                "- Input layer: Main",
                "- Expected result: boxes, count, metrics, and conservative OK/NG acceptance when possible",
                "- Allowed false positives: medium for the first pass",
                "- Preferred chain: choose the simplest reliable rule-based chain",
                "",
                "Image context:",
                $"- Source: {imageText}",
                $"- Available source layers: {layerText}",
                "- ROI: full image unless the user explicitly provides a smaller region",
                "- Polarity: infer from the image if visible; otherwise choose conservative threshold values",
                "",
                "Reference sample catalog:",
                BuildSampleCatalogPromptText(),
                "- Required examples are stable validation contracts. Treat their expected metrics as the way OpenVisionLab decides OK/NG.",
                "- Explore examples are coverage examples. Use their chains as starting patterns, but do not claim semantic decoding/OCR unless the ToolType actually supports it.",
                "- For a new image, copy the closest sample's chain shape first, then tune threshold, morphology, ROI, area/count, score, or edge metrics for the new target.",
                "- Prefer sample-backed metric gates. If no close sample exists, keep acceptance loose and explain the expected tuning metric.",
                "",
                "Supported ToolType values:",
                "- Threshold",
                "- Morphology",
                "- Filter",
                "- EdgeDetection",
                "- Blob",
                "- Contour",
                "- LineGauge",
                "- RotateScale",
                "- Matching",
                "- Mean",
                "- FeatureMatching",
                "- OverlayMerge",
                "",
                "Unsupported pipeline ToolType guard:",
                "- Do not output HSV, Histogram, Arithmetic, Color, Barcode, QR, OCR, EasyBarCode, EasyQRCode, EasyOcr, or any form-only/demo feature as ToolType.",
                "- If the user's goal mentions a form-only/demo feature, describe it in the summary as future manual form work and generate only the closest supported rule-based pipeline.",
                "- Do not invent semantic decoding. If Barcode, QR, OCR, or color classification is needed, use current tools only for candidate region detection and state the decoder/classifier gap.",
                "",
                "OpenVisionLab XML rules:",
                "- Output a complete VisionPipeline XML document.",
                "- Every Step must include Name, ToolType, Enabled, InputLayer, OutputLayer, and Parameters.",
                "- The first step should normally read from Main.",
                "- Each later step should normally read from the previous step output.",
                "- Never overwrite Main unless the user explicitly requests it. Use a named output layer for every processing step.",
                "- If a later inspection must use the original image, mark it as an independent branch and explain why in the summary.",
                "- Do not branch back to Main or another older layer unless the user's goal explicitly requires an independent branch.",
                "- If independent branches are used, add a final OverlayMerge step that reads Main and writes one final review layer.",
                "- OverlayMerge SourceLayers must list the branch result output layers separated by semicolon.",
                "- The final OverlayMerge review layer must contain all branch detections in one image. Intermediate branch images are for tuning only.",
                "- Users should not need to inspect several separate branch images to know whether the recipe worked.",
                "- Do not use broad ROI-sized rectangles as final detections. Final overlays should be object-level boxes, lines, or points.",
                "- Name output layers so the flow is readable, for example TextSymbol_Binary, TextSymbol_Clean, TextSymbol_Contour.",
                "- Parameter values must use invariant culture.",
                "- Boolean values must be true or false.",
                "- Enum values must use C# enum names used by OpenVisionLab.",
                "- ROI values must use x,y,width,height.",
                "- Do not invent parameter names.",
                "- Do not embed image data.",
                "- Prefer conservative default acceptance. Use loose count/area bounds on the first pass and expect the user to tune them in OpenVisionLab.",
                "",
                "Acceptance guidance:",
                "- Use ResultCount for Blob/Contour count checks.",
                "- Use AreaMin, AreaMax, and AreaAvg for Blob/Contour sanity checks.",
                "- Use ScoreMax or ScoreAvg for Matching and FeatureMatching.",
                "- Use EdgeCount or EdgePointCount for LineGauge edge detection.",
                "- Use LineLengthMmMax or BoundsWidthMmMax when PIXELPERMM is available and the goal is distance or size measurement.",
                "- Use MergeOverlayCount for OverlayMerge.",
                "- Do not make acceptance too tight in the first recipe.",
                "",
                "OpenVisionLab validation loop:",
                "- After import, OpenVisionLab will validate the XML, run preview, and review step metrics, overlays, result image, overlay image, and raw log.",
                "- A usable recipe should be able to produce GateStatus=OK, ArtifactIssueCount=0, and MetadataIssueCount=0 in automated sample checks.",
                "- If preview is NG, use the first failed step, error code, diagnostic hint, suggested fix, and metrics to revise the smallest necessary part of the pipeline.",
                "- If a detection step reads Main after an image-processing step, check whether it should instead read the previous output layer.",
                "- If Run Preview reports Final Review Contract NG, add or repair a final OverlayMerge instead of leaving separate branch result images.",
                "- Preserve successful previous steps and stable output layer names unless the failed step proves the flow is wrong.",
                "- Change only the first failed step and directly dependent steps unless the layer flow itself is the root cause.",
                "",
                "Return only:",
                "1. Recipe summary",
                "2. Complete VisionPipeline XML",
                "3. Tuning checklist with 3 to 5 concrete parameters");

            if (latestFeedbackHasRunResult && !string.IsNullOrWhiteSpace(latestFeedbackText))
            {
                prompt = string.Join(
                    Environment.NewLine,
                    prompt,
                    "",
                    "Previous OpenVisionLab Run Preview feedback:",
                    "```text",
                    latestFeedbackText,
                    "```",
                    "",
                "Revision request:",
                "- Revise the VisionPipeline XML using the feedback above.",
                "- Preserve every successful step and stable output layer name.",
                "- Do not change Main. Keep output layers separate from input layers.",
                "- Change only the first failed step and directly dependent steps unless the layer flow itself is wrong.",
                "- Fix the first failed step first, then tune false positives/false negatives.",
                "- If several branches detect separate targets, keep the branch steps but return one final OverlayMerge review layer for the user.");
            }

            return prompt;
        }

        private static string BuildSampleCatalogPromptText()
        {
            List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
                .Where(sample => sample.CanOpen)
                .Where(sample => !string.Equals(sample.ValidationMode, "Reference", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (samples.Count == 0)
            {
                return string.Join(
                    Environment.NewLine,
                    "- Contour_TextSymbols: keypad text, numbers, symbols, and small printed shapes. Preferred chain: Threshold -> Morphology -> Contour.",
                    "- Use these examples as first-pass recipe patterns, not as fixed final parameters.");
            }

            List<VisionPipelineSampleCatalogItem> promptSamples = SelectSampleCatalogPromptItems(samples);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"- Catalog examples shown: {promptSamples.Count}/{samples.Count}. Required recipes are listed first; Explore recipes are representative only.");
            foreach (VisionPipelineSampleCatalogItem sample in promptSamples)
            {
                string goal = string.IsNullOrWhiteSpace(sample.Goal) ? sample.Category : sample.Goal;
                string flow = string.IsNullOrWhiteSpace(sample.ToolFlowText) ? "-" : sample.ToolFlowText;
                string expected = string.IsNullOrWhiteSpace(sample.ExpectedText) ? "-" : sample.ExpectedText;
                string notes = string.IsNullOrWhiteSpace(sample.Notes) ? string.Empty : $" {TruncatePromptText(sample.Notes, 110)}";
                string mode = string.IsNullOrWhiteSpace(sample.ValidationMode) ? "Sample" : sample.ValidationMode.Trim();
                builder.AppendLine($"- [{mode}] {sample.SampleName}: {TruncatePromptText(goal, 120)} Chain: {flow}. Expected gate: {expected}.{notes}");
            }

            builder.Append("- Use these examples as first-pass recipe patterns, not as fixed final parameters. Preserve their input/output layer clarity and final review image pattern. Use Good/Bad sample pairs to define conservative acceptance gates, not only detection. Use Feature_TemplateReview when a feature/template review path is needed.");
            return builder.ToString();
        }

        private static List<VisionPipelineSampleCatalogItem> SelectSampleCatalogPromptItems(List<VisionPipelineSampleCatalogItem> samples)
        {
            List<VisionPipelineSampleCatalogItem> selected = new List<VisionPipelineSampleCatalogItem>();
            HashSet<string> selectedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (VisionPipelineSampleCatalogItem sample in samples.Where(IsRequiredCatalogSample))
            {
                AddPromptSample(selected, selectedNames, sample);
            }

            HashSet<string> exploreGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (VisionPipelineSampleCatalogItem sample in samples.Where(IsExploreCatalogSample))
            {
                string group = ResolvePromptSampleGroup(sample);
                if (exploreGroups.Add(group))
                {
                    AddPromptSample(selected, selectedNames, sample);
                }

                if (selected.Count >= 20)
                {
                    break;
                }
            }

            return selected;
        }

        private static void AddPromptSample(
            List<VisionPipelineSampleCatalogItem> selected,
            HashSet<string> selectedNames,
            VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null || !selectedNames.Add(sample.SampleName))
            {
                return;
            }

            selected.Add(sample);
        }

        private static bool IsRequiredCatalogSample(VisionPipelineSampleCatalogItem sample)
        {
            return string.Equals(sample?.ValidationMode, "Required", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsExploreCatalogSample(VisionPipelineSampleCatalogItem sample)
        {
            return string.Equals(sample?.ValidationMode, "Explore", StringComparison.OrdinalIgnoreCase);
        }

        private static string ResolvePromptSampleGroup(VisionPipelineSampleCatalogItem sample)
        {
            string category = sample?.Category ?? string.Empty;
            int separatorIndex = category.IndexOf('/');
            if (separatorIndex >= 0)
            {
                return category.Substring(0, separatorIndex).Trim();
            }

            return string.IsNullOrWhiteSpace(category) ? sample?.SampleName ?? string.Empty : category.Trim();
        }

        private static string TruncatePromptText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            string normalized = text.Replace("\r", " ").Replace("\n", " ").Trim();
            return normalized.Length <= maxLength
                ? normalized
                : normalized.Substring(0, Math.Max(0, maxLength - 3)) + "...";
        }

        private void LoadPreviewImage(string path, string logPrefix)
        {
            using (Bitmap loaded = new Bitmap(path))
            {
                testImage?.Dispose();
                testImage = new Bitmap(loaded);
            }

            ClearPreviewResult();
            stepGrid.Rows.Clear();
            latestRunSummaries.Clear();
            UpdateImageStatus();
            UpdateRecipeGuide();
            AppendLog($"{logPrefix} | {Path.GetFileName(path)} | {testImage.Width} x {testImage.Height}");
        }

        private static string ExtractXmlPayload(string text)
        {
            string value = text ?? string.Empty;
            int start = value.IndexOf("<VisionPipeline", StringComparison.OrdinalIgnoreCase);
            int end = value.LastIndexOf("</VisionPipeline>", StringComparison.OrdinalIgnoreCase);
            if (start >= 0 && end >= start)
            {
                end += "</VisionPipeline>".Length;
                return value.Substring(start, end - start);
            }

            return value.Trim();
        }

        private static string FindWorkspaceFile(params string[] relativeParts)
        {
            foreach (string root in EnumerateSearchRoots())
            {
                string candidate = Path.Combine(new[] { root }.Concat(relativeParts).ToArray());
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        private static IEnumerable<string> EnumerateSearchRoots()
        {
            HashSet<string> roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            AddSearchRoot(roots, Directory.GetCurrentDirectory());
            AddSearchRoot(roots, AppDomain.CurrentDomain.BaseDirectory);
            foreach (string root in roots.ToArray())
            {
                string current = root;
                for (int i = 0; i < 8 && !string.IsNullOrWhiteSpace(current); i++)
                {
                    yield return current;
                    DirectoryInfo parent = Directory.GetParent(current);
                    if (parent == null)
                    {
                        break;
                    }

                    current = parent.FullName;
                }
            }
        }

        private static void AddSearchRoot(HashSet<string> roots, string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                roots.Add(Path.GetFullPath(path));
            }
        }

        private static List<string> NormalizeSourceLayers(IEnumerable<string> sourceLayers)
        {
            HashSet<string> layers = new HashSet<string>(
                (sourceLayers ?? Enumerable.Empty<string>()).Where(item => !string.IsNullOrWhiteSpace(item)),
                StringComparer.OrdinalIgnoreCase);

            layers.Add("Main");
            return layers.OrderBy(item => string.Equals(item, "Main", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(item => item)
                .ToList();
        }

        private static VisionPipeline ClonePipeline(VisionPipeline source)
        {
            VisionPipeline clone = new VisionPipeline
            {
                Name = source?.Name ?? "AI_Recipe"
            };

            foreach (VisionPipelineStep step in source?.Steps ?? new List<VisionPipelineStep>())
            {
                clone.Steps.Add(CloneStep(step));
            }

            return clone;
        }

        private static VisionPipelineStep CloneStep(VisionPipelineStep source)
        {
            VisionPipelineStep clone = new VisionPipelineStep
            {
                Name = source?.Name ?? string.Empty,
                ToolType = source?.ToolType ?? string.Empty,
                Enabled = source?.Enabled ?? true,
                InputLayer = source?.InputLayer ?? string.Empty,
                OutputLayer = source?.OutputLayer ?? string.Empty,
                UseAcceptance = source?.UseAcceptance ?? false,
                ExpectedSuccess = source?.ExpectedSuccess ?? true,
                MaxElapsedMilliseconds = source?.MaxElapsedMilliseconds ?? 0,
                RequiredMessageText = source?.RequiredMessageText ?? string.Empty,
                AcceptanceMetricName = source?.AcceptanceMetricName ?? string.Empty,
                UseAcceptanceMetricMinimum = source?.UseAcceptanceMetricMinimum ?? false,
                AcceptanceMetricMinimum = source?.AcceptanceMetricMinimum ?? 0,
                UseAcceptanceMetricMaximum = source?.UseAcceptanceMetricMaximum ?? false,
                AcceptanceMetricMaximum = source?.AcceptanceMetricMaximum ?? 0
            };

            foreach (KeyValuePair<string, string> parameter in source?.Parameters ?? new Dictionary<string, string>())
            {
                clone.Parameters[parameter.Key] = parameter.Value;
            }

            return clone;
        }
    }
}
