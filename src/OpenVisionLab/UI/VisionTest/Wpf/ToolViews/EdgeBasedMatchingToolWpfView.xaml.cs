using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Result;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class EdgeBasedMatchingToolWpfView : VisionToolSingleInputPropertyToolViewBase
    {
        private readonly VisionToolSingleInputMatchingToolController<EdgeBasedMatchingProperty> toolController;
        private readonly AutoMPointTeachingPanel autoMPointPanel;
        private readonly AutoMPointTeachingController autoMPointController;
        private readonly UIElement verificationGuide;

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
            toolContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            toolContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.SetRow(verificationGuide, 0);
            toolContent.Children.Add(verificationGuide);
            autoMPointPanel.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(autoMPointPanel, 1);
            toolContent.Children.Add(autoMPointPanel);
            toolShell.ToolContent = toolContent;
            toolShell.ToolContentVisibility = Visibility.Visible;

            autoMPointPanel.DetailsExpander.Expanded += AutoMPointDetails_Expanded;
            autoMPointPanel.DetailsExpander.Collapsed += AutoMPointDetails_Collapsed;
            autoMPointController = new AutoMPointTeachingController(autoMPointPanel, toolController);
        }

        public string ResultReviewTextForTest => toolController.ResultReviewText;

        internal int AutoMPointCandidateCountForTest => autoMPointController.CandidateCount;

        internal int AutoMPointRepresentativeImageCountForTest =>
            autoMPointController.RepresentativeImageCount;

        internal string AutoMPointAppliedTemplatePathForTest =>
            autoMPointController.AppliedTemplatePath;

        internal bool IsTeachingPanelSeparatedFromGuideForTest
        {
            get
            {
                if (verificationGuide?.IsVisible != true || autoMPointPanel?.IsVisible != true)
                {
                    return false;
                }

                System.Windows.Point guideOrigin = verificationGuide.TranslatePoint(
                    new System.Windows.Point(0D, 0D),
                    toolShell);
                System.Windows.Point panelOrigin = autoMPointPanel.TranslatePoint(
                    new System.Windows.Point(0D, 0D),
                    toolShell);
                return panelOrigin.Y >= guideOrigin.Y + verificationGuide.RenderSize.Height - 0.5D;
            }
        }

        internal bool ExportAutoMPointReportForTest(string reportPath)
        {
            return autoMPointController.ExportSelectedReport(reportPath);
        }

        internal void SetAutoMPointRepresentativeImagesForTest(IEnumerable<string> paths)
        {
            autoMPointController.SetRepresentativeImages(paths);
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
            autoMPointController.SetInputPreview(image);
            base.SetInputPreview(image);
        }

        protected override void DisposeToolResources()
        {
            autoMPointController.Dispose();
            autoMPointPanel.DetailsExpander.Expanded -= AutoMPointDetails_Expanded;
            autoMPointPanel.DetailsExpander.Collapsed -= AutoMPointDetails_Collapsed;
            autoMPointPanel.Dispose();
        }

        private void AutoMPointDetails_Expanded(object sender, RoutedEventArgs e)
        {
            verificationGuide.Visibility = Visibility.Collapsed;
        }

        private void AutoMPointDetails_Collapsed(object sender, RoutedEventArgs e)
        {
            verificationGuide.Visibility = Visibility.Visible;
        }
    }
}
