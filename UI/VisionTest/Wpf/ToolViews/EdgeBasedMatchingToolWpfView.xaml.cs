using Lib.OpenCV;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenVisionLab.Contracts;
using OpenCvSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab
{
    public partial class EdgeBasedMatchingToolWpfView : VisionToolSingleInputPropertyToolViewBase
    {
        private readonly VisionToolSingleInputMatchingToolController<EdgeBasedMatchingProperty> toolController;
        private readonly AutoMPointTeachingPanel autoMPointPanel;
        private readonly UIElement verificationGuide;
        private Bitmap autoMPointSourceBitmap;
        private readonly List<string> autoMPointRepresentativeImagePaths = new List<string>();
        private int sourceRevision;
        private int analyzedSourceRevision = -1;
        private string analyzedDefinition = string.Empty;
        private string analyzedRepresentativeDefinition = string.Empty;
        private string appliedTemplatePath = string.Empty;

        internal EdgeBasedMatchingToolWpfView(VisionToolPropertyGridPresenter<EdgeBasedMatchingProperty> presenter)
        {
            OpenVisionToolOpenProfiler.Measure("EdgeBasedMatchingInitializeComponent", InitializeComponent);
            autoMPointPanel = toolShell.ToolContent as AutoMPointTeachingPanel
                ?? throw new InvalidOperationException("Edge Based Matching must provide the Auto MPoint teaching panel.");
            toolController = OpenVisionToolOpenProfiler.Measure(
                "EdgeBasedMatchingAttachController",
                () => VisionToolSingleInputMatchingToolController<EdgeBasedMatchingProperty>.Attach(
                    this,
                    presenter,
                    "VisionMenu.EdgeBasedMatching",
                    "Edge Match"));
            AttachPropertyToolController(toolController);

            verificationGuide = toolShell.ToolContent as UIElement
                ?? throw new InvalidOperationException("Edge Based Matching runtime must provide its verification guide.");
            Grid toolContent = new Grid();
            toolContent.Children.Add(verificationGuide);
            autoMPointPanel.VerticalAlignment = VerticalAlignment.Top;
            Panel.SetZIndex(autoMPointPanel, 1);
            toolContent.Children.Add(autoMPointPanel);
            toolShell.ToolContent = toolContent;
            toolShell.ToolContentVisibility = Visibility.Visible;

            autoMPointPanel.AnalyzeButton.Click += AnalyzeAutoMPoint_Click;
            autoMPointPanel.RepresentativeImagesButton.Click += SelectAutoMPointRepresentativeImages_Click;
            autoMPointPanel.ReportButton.Click += ExportAutoMPointReport_Click;
            autoMPointPanel.UsePatternButton.Click += UseAutoMPointPattern_Click;
            autoMPointPanel.CandidateList.SelectionChanged += AutoMPointCandidates_SelectionChanged;
            autoMPointPanel.DetailsExpander.Expanded += AutoMPointDetails_Expanded;
            autoMPointPanel.DetailsExpander.Collapsed += AutoMPointDetails_Collapsed;
            ClearAutoMPointAnalysis();
            UpdateAutoMPointRepresentativeCount();
        }

        public string ResultReviewTextForTest => toolController.ResultReviewText;

        internal int AutoMPointCandidateCountForTest => autoMPointPanel.CandidateList.Items.Count;

        internal int AutoMPointRepresentativeImageCountForTest =>
            autoMPointRepresentativeImagePaths.Count;

        internal string AutoMPointAppliedTemplatePathForTest => appliedTemplatePath;

        internal bool ExportAutoMPointReportForTest(string reportPath)
        {
            return TryExportSelectedAutoMPointReport(reportPath);
        }

        internal void SetAutoMPointRepresentativeImagesForTest(IEnumerable<string> paths)
        {
            autoMPointRepresentativeImagePaths.Clear();
            autoMPointRepresentativeImagePaths.AddRange(
                (paths ?? Enumerable.Empty<string>())
                    .Where(path => !string.IsNullOrWhiteSpace(path))
                    .Select(Path.GetFullPath)
                    .Distinct(StringComparer.OrdinalIgnoreCase));
            ClearAutoMPointAnalysis();
            UpdateAutoMPointRepresentativeCount();
            autoMPointPanel.StatusText.Text = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeLoadedFormat"),
                autoMPointRepresentativeImagePaths.Count);
        }

        public EdgeBasedMatchingProperty CreateProperty()
        {
            return toolController.CreateProperty();
        }

        public void SetTemplatePathForTest(string path)
        {
            toolController.SetTemplatePathForTest(path);
        }

        public void ConfigurePropertyForTest(Action<EdgeBasedMatchingProperty> configure)
        {
            toolController.ConfigurePropertyForTest(configure);
        }

        public void SetResultReview(IEnumerable<MatchingResult> results)
        {
            toolController.SetResultReview(results);
        }

        public void SetResultReview(IEnumerable<MatchingResult> results, TimeSpan? tactTime)
        {
            toolController.SetResultReview(results, tactTime);
        }

        public override void SetInputPreview(Bitmap image)
        {
            autoMPointSourceBitmap?.Dispose();
            autoMPointSourceBitmap = image == null ? null : new Bitmap(image);
            autoMPointRepresentativeImagePaths.Clear();
            UpdateAutoMPointRepresentativeCount();
            sourceRevision++;
            ClearAutoMPointAnalysis();
            base.SetInputPreview(image);
        }

        protected override void DisposeToolResources()
        {
            autoMPointPanel.Dispose();
            autoMPointPanel.AnalyzeButton.Click -= AnalyzeAutoMPoint_Click;
            autoMPointPanel.RepresentativeImagesButton.Click -= SelectAutoMPointRepresentativeImages_Click;
            autoMPointPanel.ReportButton.Click -= ExportAutoMPointReport_Click;
            autoMPointPanel.UsePatternButton.Click -= UseAutoMPointPattern_Click;
            autoMPointPanel.CandidateList.SelectionChanged -= AutoMPointCandidates_SelectionChanged;
            autoMPointPanel.DetailsExpander.Expanded -= AutoMPointDetails_Expanded;
            autoMPointPanel.DetailsExpander.Collapsed -= AutoMPointDetails_Collapsed;
            autoMPointSourceBitmap?.Dispose();
            autoMPointSourceBitmap = null;
        }

        private void AutoMPointDetails_Expanded(object sender, RoutedEventArgs e)
        {
            verificationGuide.Visibility = Visibility.Collapsed;
        }

        private void AutoMPointDetails_Collapsed(object sender, RoutedEventArgs e)
        {
            verificationGuide.Visibility = Visibility.Visible;
        }

        private void AnalyzeAutoMPoint_Click(object sender, RoutedEventArgs e)
        {
            if (autoMPointSourceBitmap == null)
            {
                autoMPointPanel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.NoImage");
                return;
            }

            autoMPointPanel.AnalyzeButton.IsEnabled = false;
            autoMPointPanel.UsePatternButton.IsEnabled = false;
            Mouse.OverrideCursor = Cursors.Wait;
            autoMPointPanel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Analyzing");
            autoMPointPanel.CandidateList.Items.Clear();

            VisionToolResult execution = null;
            AutoMPointTool tool = null;
            List<Mat> representativeImages = new List<Mat>();
            try
            {
                EdgeBasedMatchingProperty matchingProperty = toolController.CreateProperty();
                AutoMPointToolProperty autoProperty = CreateAutoMPointProperty(matchingProperty);
                string definition = CreateAnalysisDefinition(autoProperty);
                using Mat source = Lib.Common.BitmapImageConverter.ToMat(autoMPointSourceBitmap);
                foreach (string path in autoMPointRepresentativeImagePaths)
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
                    autoMPointPanel.CandidateList.Items.Add(new ListBoxItem
                    {
                        Tag = candidate,
                        Content = FormatCandidate(candidate)
                    });
                }

                analyzedSourceRevision = sourceRevision;
                analyzedDefinition = definition;
                analyzedRepresentativeDefinition = CreateRepresentativeDefinition();
                if (autoMPointPanel.CandidateList.Items.Count > 0)
                {
                    autoMPointPanel.CandidateList.SelectedIndex = 0;
                }

                if (execution?.ResultImage != null && !execution.ResultImage.Empty())
                {
                    using Bitmap resultBitmap = Lib.Common.BitmapImageConverter.ToBitmap(execution.ResultImage);
                    toolController.SetOutputPreview(resultBitmap);
                }

                AutoMPointCandidateResult best = tool.results.FirstOrDefault();
                autoMPointPanel.StatusText.Text = execution?.Success == true
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
                        autoMPointPanel.CandidateList.Items.Count)
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
                autoMPointPanel.StatusText.Text = string.Format(
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
                autoMPointPanel.AnalyzeButton.IsEnabled = true;
                autoMPointPanel.UsePatternButton.IsEnabled = autoMPointPanel.CandidateList.SelectedItem != null;
                UpdateAutoMPointReportButton();
                Mouse.OverrideCursor = null;
            }
        }

        private void AutoMPointCandidates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            autoMPointPanel.UsePatternButton.IsEnabled = autoMPointPanel.CandidateList.SelectedItem is ListBoxItem item
                && item.Tag is AutoMPointCandidateResult;
            UpdateAutoMPointReportButton();
        }

        private void ExportAutoMPointReport_Click(object sender, RoutedEventArgs e)
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
                TryExportSelectedAutoMPointReport(dialog.FileName);
            }
        }

        private bool TryExportSelectedAutoMPointReport(string reportPath)
        {
            if (autoMPointSourceBitmap == null
                || autoMPointPanel.CandidateList.SelectedItem is not ListBoxItem selectedItem
                || selectedItem.Tag is not AutoMPointCandidateResult candidate
                || candidate.RepresentativeImageCount <= 0)
            {
                autoMPointPanel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.NoSelection");
                return false;
            }

            EdgeBasedMatchingProperty matchingProperty = toolController.CreateProperty();
            string currentDefinition = CreateAnalysisDefinition(CreateAutoMPointProperty(matchingProperty));
            if (sourceRevision != analyzedSourceRevision
                || !string.Equals(currentDefinition, analyzedDefinition, StringComparison.Ordinal)
                || !string.Equals(
                    CreateRepresentativeDefinition(),
                    analyzedRepresentativeDefinition,
                    StringComparison.Ordinal))
            {
                autoMPointPanel.ReportButton.IsEnabled = false;
                autoMPointPanel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Changed");
                return false;
            }

            bool exported = AutoMPointHtmlReportExporter.TryExport(
                autoMPointSourceBitmap,
                autoMPointRepresentativeImagePaths,
                candidate,
                analyzedDefinition,
                reportPath,
                out string error);
            autoMPointPanel.StatusText.Text = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T(exported
                    ? "VisionTool.AutoMPoint.ReportExportedFormat"
                    : "VisionTool.AutoMPoint.ReportExportFailedFormat"),
                exported ? Path.GetFullPath(reportPath) : error);
            return exported;
        }

        private void UseAutoMPointPattern_Click(object sender, RoutedEventArgs e)
        {
            if (autoMPointSourceBitmap == null
                || autoMPointPanel.CandidateList.SelectedItem is not ListBoxItem selectedItem
                || selectedItem.Tag is not AutoMPointCandidateResult candidate)
            {
                autoMPointPanel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.NoSelection");
                return;
            }

            EdgeBasedMatchingProperty matchingProperty = toolController.CreateProperty();
            string currentDefinition = CreateAnalysisDefinition(CreateAutoMPointProperty(matchingProperty));
            if (sourceRevision != analyzedSourceRevision
                || !string.Equals(currentDefinition, analyzedDefinition, StringComparison.Ordinal)
                || !string.Equals(
                    CreateRepresentativeDefinition(),
                    analyzedRepresentativeDefinition,
                    StringComparison.Ordinal))
            {
                autoMPointPanel.UsePatternButton.IsEnabled = false;
                autoMPointPanel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Changed");
                return;
            }

            try
            {
                using Mat source = Lib.Common.BitmapImageConverter.ToMat(autoMPointSourceBitmap);
                string path = PropertyGridEditorFactory.SaveTemplateImageForTeaching(source, candidate.PatternRoi);
                if (string.IsNullOrWhiteSpace(path))
                {
                    autoMPointPanel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.SaveFailed");
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
                autoMPointPanel.StatusText.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("VisionTool.AutoMPoint.AppliedFormat"),
                    candidate.Rank,
                    path);
            }
            catch (Exception exception)
            {
                autoMPointPanel.StatusText.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("VisionTool.AutoMPoint.FailedFormat"),
                    exception.GetType().Name,
                    exception.GetBaseException().Message);
            }
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
            OpenCvSharp.Rect roi = candidate.PatternRoi;
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

        private void ClearAutoMPointAnalysis()
        {
            analyzedSourceRevision = -1;
            analyzedDefinition = string.Empty;
            analyzedRepresentativeDefinition = string.Empty;
            appliedTemplatePath = string.Empty;
            autoMPointPanel.CandidateList.Items.Clear();
            autoMPointPanel.UsePatternButton.IsEnabled = false;
            autoMPointPanel.ReportButton.IsEnabled = false;
            autoMPointPanel.StatusText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Ready");
        }

        private void SelectAutoMPointRepresentativeImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeImages"),
                Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|All files|*.*",
                Multiselect = true,
                CheckFileExists = true
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            SetAutoMPointRepresentativeImagesForTest(dialog.FileNames);
        }

        private void UpdateAutoMPointRepresentativeCount()
        {
            autoMPointPanel.RepresentativeCountText.Text = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeCountFormat"),
                autoMPointRepresentativeImagePaths.Count);
        }

        private void UpdateAutoMPointReportButton()
        {
            autoMPointPanel.ReportButton.IsEnabled =
                autoMPointPanel.CandidateList.SelectedItem is ListBoxItem item
                && item.Tag is AutoMPointCandidateResult candidate
                && candidate.RepresentativeImageCount > 0
                && candidate.RepresentativeImageCount == autoMPointRepresentativeImagePaths.Count;
        }

        private string CreateRepresentativeDefinition()
        {
            return string.Join(
                "|",
                autoMPointRepresentativeImagePaths.Select(path =>
                {
                    FileInfo file = new FileInfo(path);
                    return string.Join(
                        ":",
                        file.FullName,
                        file.Exists ? file.Length : -1L,
                        file.Exists ? file.LastWriteTimeUtc.Ticks : 0L);
                }));
        }

    }
}
