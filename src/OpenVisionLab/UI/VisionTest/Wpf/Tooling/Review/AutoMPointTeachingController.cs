using OpenVisionLab.Common;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using Microsoft.Win32;
using OpenCvSharp;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using RoutedEventArgs = System.Windows.RoutedEventArgs;

namespace OpenVisionLab
{
    internal sealed class AutoMPointTeachingController : IDisposable
    {
        private readonly AutoMPointTeachingPanel panel;
        private readonly VisionToolSingleInputMatchingToolController<EdgeBasedMatchingProperty> toolController;
        private readonly List<string> representativeImagePaths = new List<string>();
        private Bitmap sourceBitmap;
        private int sourceRevision;
        private int analyzedSourceRevision = -1;
        private string analyzedDefinition = string.Empty;
        private string analyzedRepresentativeDefinition = string.Empty;
        private string appliedTemplatePath = string.Empty;
        private bool disposed;

        internal AutoMPointTeachingController(
            AutoMPointTeachingPanel panel,
            VisionToolSingleInputMatchingToolController<EdgeBasedMatchingProperty> toolController)
        {
            this.panel = panel ?? throw new ArgumentNullException(nameof(panel));
            this.toolController = toolController ?? throw new ArgumentNullException(nameof(toolController));

            panel.AnalyzeButton.Click += AnalyzeCandidates;
            panel.RepresentativeImagesButton.Click += SelectRepresentativeImages;
            panel.ReportButton.Click += ExportReport;
            panel.UsePatternButton.Click += UseSelectedPattern;
            panel.CandidateList.SelectionChanged += CandidateSelectionChanged;
            ClearAnalysis();
            UpdateRepresentativeCount();
        }

        internal int CandidateCount => panel.CandidateList.Items.Count;

        internal int RepresentativeImageCount => representativeImagePaths.Count;

        internal string AppliedTemplatePath => appliedTemplatePath;

        internal void SetInputPreview(Bitmap image)
        {
            sourceBitmap?.Dispose();
            sourceBitmap = image == null ? null : new Bitmap(image);
            representativeImagePaths.Clear();
            UpdateRepresentativeCount();
            sourceRevision++;
            ClearAnalysis();
        }

        internal void SetRepresentativeImages(IEnumerable<string> paths)
        {
            representativeImagePaths.Clear();
            representativeImagePaths.AddRange(
                (paths ?? Enumerable.Empty<string>())
                    .Where(path => !string.IsNullOrWhiteSpace(path))
                    .Select(Path.GetFullPath)
                    .Distinct(StringComparer.OrdinalIgnoreCase));
            ClearAnalysis();
            UpdateRepresentativeCount();
            panel.StatusText.Text = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeLoadedFormat"),
                representativeImagePaths.Count);
        }

        internal bool ExportSelectedReport(string reportPath)
        {
            if (sourceBitmap == null
                || SelectedCandidate is not AutoMPointCandidateResult candidate
                || candidate.RepresentativeImageCount <= 0)
            {
                panel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.NoSelection");
                return false;
            }

            EdgeBasedMatchingProperty matchingProperty = toolController.CreateProperty();
            string currentDefinition = CreateAnalysisDefinition(CreateAutoMPointProperty(matchingProperty));
            if (!IsCurrentAnalysis(currentDefinition))
            {
                panel.ReportButton.IsEnabled = false;
                panel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Changed");
                return false;
            }

            bool exported = AutoMPointHtmlReportExporter.TryExport(
                sourceBitmap,
                representativeImagePaths,
                candidate,
                analyzedDefinition,
                reportPath,
                out string error);
            panel.StatusText.Text = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T(exported
                    ? "VisionTool.AutoMPoint.ReportExportedFormat"
                    : "VisionTool.AutoMPoint.ReportExportFailedFormat"),
                exported ? Path.GetFullPath(reportPath) : error);
            return exported;
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;

            panel.AnalyzeButton.Click -= AnalyzeCandidates;
            panel.RepresentativeImagesButton.Click -= SelectRepresentativeImages;
            panel.ReportButton.Click -= ExportReport;
            panel.UsePatternButton.Click -= UseSelectedPattern;
            panel.CandidateList.SelectionChanged -= CandidateSelectionChanged;
            sourceBitmap?.Dispose();
            sourceBitmap = null;
        }

        private AutoMPointCandidateResult SelectedCandidate =>
            panel.CandidateList.SelectedItem is ListBoxItem item
                ? item.Tag as AutoMPointCandidateResult
                : null;

        private void AnalyzeCandidates(object sender, RoutedEventArgs e)
        {
            if (sourceBitmap == null)
            {
                panel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.NoImage");
                return;
            }

            panel.AnalyzeButton.IsEnabled = false;
            panel.UsePatternButton.IsEnabled = false;
            Mouse.OverrideCursor = Cursors.Wait;
            panel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Analyzing");
            panel.CandidateList.Items.Clear();

            VisionToolResult execution = null;
            AutoMPointTool tool = null;
            List<Mat> representativeImages = new List<Mat>();
            try
            {
                EdgeBasedMatchingProperty matchingProperty = toolController.CreateProperty();
                AutoMPointToolProperty autoProperty = CreateAutoMPointProperty(matchingProperty);
                string definition = CreateAnalysisDefinition(autoProperty);
                using Mat source = OpenVisionLab.Common.BitmapImageConverter.ToMat(sourceBitmap);
                foreach (string path in representativeImagePaths)
                {
                    representativeImages.Add(Cv2.ImRead(path, ImreadModes.Unchanged));
                }

                tool = new AutoMPointTool();
                tool.SetProperty(autoProperty);
                execution = representativeImages.Count > 0
                    ? tool.Execute(source, representativeImages)
                    : tool.Execute(source);

                foreach (AutoMPointCandidateResult candidate in tool.results)
                {
                    panel.CandidateList.Items.Add(new ListBoxItem
                    {
                        Tag = candidate,
                        Content = FormatCandidate(candidate)
                    });
                }

                analyzedSourceRevision = sourceRevision;
                analyzedDefinition = definition;
                analyzedRepresentativeDefinition = CreateRepresentativeDefinition();
                if (panel.CandidateList.Items.Count > 0)
                {
                    panel.CandidateList.SelectedIndex = 0;
                }

                if (execution?.ResultImage != null && !execution.ResultImage.Empty())
                {
                    using Bitmap resultBitmap = OpenVisionLab.Common.BitmapImageConverter.ToBitmap(execution.ResultImage);
                    toolController.SetOutputPreview(resultBitmap);
                }

                AutoMPointCandidateResult best = tool.results.FirstOrDefault();
                panel.StatusText.Text = execution?.Success == true
                    && best?.RepresentativeImageCount > 0
                    ? string.Format(
                        CultureInfo.CurrentCulture,
                        OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeSuccessFormat"),
                        best.RepresentativeImageCount,
                        best.RepresentativeSuccessCount,
                        best.RepresentativeSuccessRate * 100d,
                        best.Rank)
                    : execution?.Success == true
                        ? string.Format(
                            CultureInfo.CurrentCulture,
                            OpenVisionLanguageService.T("VisionTool.AutoMPoint.SuccessFormat"),
                            panel.CandidateList.Items.Count)
                        : string.Format(
                            CultureInfo.CurrentCulture,
                            OpenVisionLanguageService.T("VisionTool.AutoMPoint.FailedFormat"),
                            execution?.ErrorName ?? VisionToolErrorCode.Unknown.ToString(),
                            execution?.Message ?? string.Empty);
            }
            catch (Exception exception)
            {
                analyzedSourceRevision = -1;
                analyzedDefinition = string.Empty;
                analyzedRepresentativeDefinition = string.Empty;
                panel.StatusText.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("VisionTool.AutoMPoint.FailedFormat"),
                    exception.GetType().Name,
                    exception.GetBaseException().Message);
            }
            finally
            {
                execution?.ResultImage?.Dispose();
                tool?.imageSource?.Dispose();
                tool?.imageResult?.Dispose();
                tool?.imageTemplate?.Dispose();
                foreach (Mat image in representativeImages)
                {
                    image.Dispose();
                }

                panel.AnalyzeButton.IsEnabled = true;
                panel.UsePatternButton.IsEnabled = SelectedCandidate != null;
                UpdateReportButton();
                Mouse.OverrideCursor = null;
            }
        }

        private void CandidateSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panel.UsePatternButton.IsEnabled = SelectedCandidate != null;
            UpdateReportButton();
        }

        private void ExportReport(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = OpenVisionLanguageService.T("VisionTool.AutoMPoint.ExportReport"),
                Filter = "HTML report|*.html",
                DefaultExt = ".html",
                AddExtension = true,
                FileName = "OpenVisionLab_AutoMPoint_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                    + ".html"
            };
            if (dialog.ShowDialog() == true)
            {
                ExportSelectedReport(dialog.FileName);
            }
        }

        private void UseSelectedPattern(object sender, RoutedEventArgs e)
        {
            if (sourceBitmap == null || SelectedCandidate is not AutoMPointCandidateResult candidate)
            {
                panel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.NoSelection");
                return;
            }

            EdgeBasedMatchingProperty matchingProperty = toolController.CreateProperty();
            string currentDefinition = CreateAnalysisDefinition(CreateAutoMPointProperty(matchingProperty));
            if (!IsCurrentAnalysis(currentDefinition))
            {
                panel.UsePatternButton.IsEnabled = false;
                panel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Changed");
                return;
            }

            try
            {
                using Mat source = OpenVisionLab.Common.BitmapImageConverter.ToMat(sourceBitmap);
                string path = PropertyGridEditorFactory.SaveTemplateImageForTeaching(source, candidate.PatternRoi);
                if (string.IsNullOrWhiteSpace(path))
                {
                    panel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.SaveFailed");
                    return;
                }

                toolController.ConfigurePropertyForTest(property =>
                {
                    property.PATTERN_PATH = path;
                    property.USE_THRESHOLD = false;
                    property.USE_ADAPTIVE_THRESHOLD = false;
                    property.ReloadTemplateImage();
                });
                appliedTemplatePath = path;
                panel.StatusText.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("VisionTool.AutoMPoint.AppliedFormat"),
                    candidate.Rank,
                    path);
            }
            catch (Exception exception)
            {
                panel.StatusText.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("VisionTool.AutoMPoint.FailedFormat"),
                    exception.GetType().Name,
                    exception.GetBaseException().Message);
            }
        }

        private void SelectRepresentativeImages(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeImages"),
                Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|All files|*.*",
                Multiselect = true,
                CheckFileExists = true
            };
            if (dialog.ShowDialog() == true)
            {
                SetRepresentativeImages(dialog.FileNames);
            }
        }

        private bool IsCurrentAnalysis(string currentDefinition)
        {
            return sourceRevision == analyzedSourceRevision
                && string.Equals(currentDefinition, analyzedDefinition, StringComparison.Ordinal)
                && string.Equals(
                    CreateRepresentativeDefinition(),
                    analyzedRepresentativeDefinition,
                    StringComparison.Ordinal);
        }

        private void ClearAnalysis()
        {
            analyzedSourceRevision = -1;
            analyzedDefinition = string.Empty;
            analyzedRepresentativeDefinition = string.Empty;
            appliedTemplatePath = string.Empty;
            panel.CandidateList.Items.Clear();
            panel.UsePatternButton.IsEnabled = false;
            panel.ReportButton.IsEnabled = false;
            panel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Ready");
        }

        private void UpdateRepresentativeCount()
        {
            panel.RepresentativeCountText.Text = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeCountFormat"),
                representativeImagePaths.Count);
        }

        private void UpdateReportButton()
        {
            AutoMPointCandidateResult candidate = SelectedCandidate;
            panel.ReportButton.IsEnabled = candidate != null
                && candidate.RepresentativeImageCount > 0
                && candidate.RepresentativeImageCount == representativeImagePaths.Count;
        }

        private string CreateRepresentativeDefinition()
        {
            return string.Join(
                "|",
                representativeImagePaths.Select(path =>
                {
                    FileInfo file = new FileInfo(path);
                    return string.Join(
                        ":",
                        file.FullName,
                        file.Exists ? file.Length : -1L,
                        file.Exists ? file.LastWriteTimeUtc.Ticks : 0L);
                }));
        }

        private static AutoMPointToolProperty CreateAutoMPointProperty(EdgeBasedMatchingProperty property)
        {
            return new AutoMPointToolProperty
            {
                UseAnalysisRoi = property.AUTO_MPOINT_USE_ANALYSIS_ROI,
                AnalysisRoi = property.AUTO_MPOINT_ANALYSIS_ROI,
                CandidateMode = AutoMPointCandidateMode.Grid,
                PatternWidth = property.AUTO_MPOINT_PATTERN_WIDTH,
                PatternHeight = property.AUTO_MPOINT_PATTERN_HEIGHT,
                CandidateStride = property.AUTO_MPOINT_STRIDE,
                MaximumFinalists = Math.Max(8, property.AUTO_MPOINT_MAX_RESULTS),
                MaximumResults = property.AUTO_MPOINT_MAX_RESULTS,
                MinimumFeatureQuality = property.AUTO_MPOINT_MIN_FEATURE_QUALITY,
                CannyLow = property.CANNY_LOW,
                CannyHigh = property.CANNY_HIGH,
                MatchingMinimumScore = property.SCORE_MIN,
                MinimumUniquenessMargin = property.AUTO_MPOINT_MIN_UNIQUENESS,
                MaximumTemplatePoints = property.MAX_TEMPLATE_POINTS,
                SearchStep = property.SEARCH_STEP,
                UsePositionRefine = property.USE_POSITION_REFINE,
                UseSubpixelRefine = property.USE_SUBPIXEL_REFINE,
                UsePyramidPositionProposal = property.USE_PYRAMID_POSITION_PROPOSAL,
                UseHybridVerify = property.USE_HYBRID_VERIFY,
                UseAngleSearch = property.USE_FIND_ANGLE,
                AngleMinimum = property.FIND_ANGLE_MIN,
                AngleMaximum = property.FIND_ANGLE_MAX,
                AngleStep = property.FIND_ANGLE,
                UseScaleSearch = property.USE_FIND_SCALE,
                ScaleMinimum = property.FIND_SCALE_MIN,
                ScaleMaximum = property.FIND_SCALE_MAX,
                ScaleStep = property.FIND_SCALE_STEP,
                MaximumPositionErrorPixels = property.AUTO_MPOINT_MAX_POSITION_ERROR,
                MaximumAngleErrorDegrees = Math.Max(1.5D, property.FIND_ANGLE),
                MaximumScaleErrorRatio = 0.03D,
                MinimumRepresentativeImageCount = property.AUTO_MPOINT_MIN_REPRESENTATIVE_IMAGES,
                MinimumRepresentativeSuccessRate =
                    property.AUTO_MPOINT_MIN_REPRESENTATIVE_SUCCESS_RATE
            };
        }

        private static string CreateAnalysisDefinition(AutoMPointToolProperty property)
        {
            return string.Join(
                "|",
                property.UseAnalysisRoi,
                property.AnalysisRoi.X,
                property.AnalysisRoi.Y,
                property.AnalysisRoi.Width,
                property.AnalysisRoi.Height,
                property.CandidateMode,
                property.PatternWidth,
                property.PatternHeight,
                property.CandidateStride,
                property.MaximumFinalists,
                property.MaximumResults,
                property.MinimumFeatureQuality,
                property.CannyLow,
                property.CannyHigh,
                property.MatchingMinimumScore,
                property.MinimumUniquenessMargin,
                property.MaximumTemplatePoints,
                property.SearchStep,
                property.UsePositionRefine,
                property.UseSubpixelRefine,
                property.UsePyramidPositionProposal,
                property.UseHybridVerify,
                property.UseAngleSearch,
                property.AngleMinimum,
                property.AngleMaximum,
                property.AngleStep,
                property.UseScaleSearch,
                property.ScaleMinimum,
                property.ScaleMaximum,
                property.ScaleStep,
                property.MaximumPositionErrorPixels,
                property.MaximumAngleErrorDegrees,
                property.MaximumScaleErrorRatio,
                property.MinimumRepresentativeImageCount,
                property.MinimumRepresentativeSuccessRate);
        }

        private static string FormatCandidate(AutoMPointCandidateResult candidate)
        {
            Rect roi = candidate.PatternRoi;
            if (candidate.RepresentativeImageCount > 0)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "#{0} BEST | ROI {1},{2},{3},{4} | Samples {5}/{6} ({7:0.0}%) | Avg {8:0.0} | Umin {9:0.000}",
                    candidate.Rank,
                    roi.X,
                    roi.Y,
                    roi.Width,
                    roi.Height,
                    candidate.RepresentativeSuccessCount,
                    candidate.RepresentativeImageCount,
                    candidate.RepresentativeSuccessRate * 100d,
                    candidate.RepresentativeMeanScore,
                    candidate.RepresentativeMinimumUniquenessMargin);
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "#{0} | ROI {1},{2},{3},{4} | U {5:0.000} | Err {6:0.00}px | P95 {7:0.0}ms",
                candidate.Rank,
                roi.X,
                roi.Y,
                roi.Width,
                roi.Height,
                candidate.UniquenessMargin,
                candidate.PositionErrorMaxPixels,
                candidate.RuntimeP95Milliseconds);
        }
    }
}
