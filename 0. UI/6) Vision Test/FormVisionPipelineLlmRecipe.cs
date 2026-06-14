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
        private Button btnApply;
        private Button btnClose;
        private Bitmap testImage;
        private Bitmap importedPreviewImage;
        private Bitmap previewRawImage;
        private List<VisionToolOverlay> previewOverlays = new List<VisionToolOverlay>();
        private VisionPipelineStepResultSummary previewSummary;
        private string previewTitle = "AI Preview";
        private VisionPipeline parsedPipeline;

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
                    AppendLog("PROMPT | Copied AI Recipe request to clipboard.");
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
            UpdateImageStatus();
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
            ClearPreviewResult();
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

                AppendLog($"{(runResult?.Success == true ? "RUN OK" : "RUN NG")} | {BuildRunSummary(runResult)}");
            }
            catch (Exception ex)
            {
                AppendLog($"RUN NG | {ex.GetBaseException().Message}");
            }
            finally
            {
                DisposeRunResultImages(runResult);
                SetBusy(false);
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
            btnApply.Enabled = false;
        }

        private bool ValidateXml(bool showLog)
        {
            validationGrid.Rows.Clear();
            parsedPipeline = null;
            btnApply.Enabled = false;

            string xml = ExtractXmlPayload(tbXml.Text);
            if (!SerializeHelper.TryLoadFromXmlText(xml, out VisionPipeline pipeline, out string loadError) || pipeline == null)
            {
                AddValidationRow("Error", loadError);
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

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, GetValidationSourceLayers());
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
            btnApply.Enabled = validation.Success;
            if (showLog)
            {
                AppendLog($"{(validation.Success ? "VALIDATE OK" : "VALIDATE NG")} | {pipeline.Name} | Steps={pipeline.Steps.Count} | Errors={validation.Errors.Count} | Warnings={validation.Warnings.Count}");
            }

            return validation.Success;
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
            foreach (VisionPipelineStepResultSummary summary in VisionPipelineResultSummaryService.CreateStepSummaries(runResult))
            {
                int rowIndex = stepGrid.Rows.Add(
                    summary.Index.ToString(CultureInfo.InvariantCulture),
                    $"{summary.Name} [{summary.ToolType}]",
                    summary.Status,
                    summary.ElapsedMilliseconds <= 0 ? "-" : $"{summary.ElapsedMilliseconds:0.0} ms",
                    summary.MetricsText);
                DataGridViewRow row = stepGrid.Rows[rowIndex];
                row.DefaultCellStyle.ForeColor = ResolveStatusColor(summary.Status);
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

            btnPaste.Enabled = !busy;
            btnOpenXml.Enabled = !busy;
            btnSample.Enabled = !busy;
            btnPrompt.Enabled = !busy;
            btnValidate.Enabled = !busy;
            btnLoadImage.Enabled = !busy;
            btnUseCurrent.Enabled = !busy;
            btnRunPreview.Enabled = !busy;
            btnApply.Enabled = !busy && parsedPipeline != null && ValidateXml(showLog: false);
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
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

            return string.Join(
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
                "- Contour_TextSymbols: keypad text, numbers, symbols, and small printed shapes. Preferred chain: Threshold -> Morphology -> Contour.",
                "- Rice_Particle: particle-count tuning with BinaryInv threshold, morphology open, and contour count acceptance.",
                "- Pins_Feature: repeated small pin feature count with threshold, morphology close, and contour count acceptance.",
                "- BentPin_Large: first bent-pin defect baseline using large contour regions; line/angle checks may be needed for final defect judgment.",
                "- DiePad*_Surface: die-pad surface contour benchmarks; ROI, area, edge, and shape checks are the next refinement.",
                "- Use these examples as first-pass recipe patterns, not as fixed final parameters.",
                "",
                "Supported ToolType values:",
                "- Threshold",
                "- Morphology",
                "- Filter",
                "- EdgeDetection",
                "- Blob",
                "- Contour",
                "- LineGauge",
                "- Matching",
                "- Mean",
                "- FeatureMatching",
                "",
                "OpenVisionLab XML rules:",
                "- Output a complete VisionPipeline XML document.",
                "- Every Step must include Name, ToolType, Enabled, InputLayer, OutputLayer, and Parameters.",
                "- The first step should normally read from Main.",
                "- Each later step should normally read from the previous step output.",
                "- Do not branch back to Main or another older layer unless the user's goal explicitly requires an independent branch.",
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
                "- Use EdgeCount or EdgePointCount for LineGauge.",
                "- Do not make acceptance too tight in the first recipe.",
                "",
                "Return only:",
                "1. Recipe summary",
                "2. Complete VisionPipeline XML",
                "3. Tuning checklist with 3 to 5 concrete parameters");
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
            UpdateImageStatus();
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
