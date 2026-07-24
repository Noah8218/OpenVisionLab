using System;
using System.Windows.Automation;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class AutoMPointTeachingPanel : UserControl, IDisposable
    {
        private readonly VisionToolLanguageChangeController languageChangeController;

        public AutoMPointTeachingPanel()
        {
            InitializeComponent();
            languageChangeController = VisionToolLanguageChangeController.Attach(ApplyLocalization);
            ApplyLocalization();
        }

        internal Button AnalyzeButton => analyzeButton;

        internal Button RepresentativeImagesButton => representativeImagesButton;

        internal Button ReportButton => reportButton;

        internal Expander DetailsExpander => detailsExpander;

        internal Button UsePatternButton => usePatternButton;

        internal ListBox CandidateList => candidateList;

        internal TextBlock TitleText => titleText;

        internal TextBlock SuggestedText => suggestedText;

        internal TextBlock AnalyzeText => analyzeText;

        internal TextBlock RepresentativeImagesText => representativeImagesText;

        internal TextBlock RepresentativeCountText => representativeCountText;

        internal TextBlock ReportText => reportText;

        internal TextBlock UsePatternText => usePatternText;

        internal TextBlock StatusText => statusText;

        public void Dispose()
        {
            languageChangeController.Dispose();
        }

        private void ApplyLocalization()
        {
            TitleText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Title");
            SuggestedText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Suggested");
            AnalyzeText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Analyze");
            RepresentativeImagesText.Text =
                OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeImages");
            ReportText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.ExportReport");
            UsePatternText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.UsePattern");
            AnalyzeButton.ToolTip = OpenVisionLanguageService.T("VisionTool.AutoMPoint.AnalyzeToolTip");
            RepresentativeImagesButton.ToolTip =
                OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeImagesToolTip");
            ReportButton.ToolTip = OpenVisionLanguageService.T("VisionTool.AutoMPoint.ExportReportToolTip");
            UsePatternButton.ToolTip = OpenVisionLanguageService.T("VisionTool.AutoMPoint.UsePatternToolTip");
            AutomationProperties.SetName(AnalyzeButton, AnalyzeText.Text);
            AutomationProperties.SetName(RepresentativeImagesButton, RepresentativeImagesText.Text);
            AutomationProperties.SetName(ReportButton, ReportText.Text);
            AutomationProperties.SetName(UsePatternButton, UsePatternText.Text);
            AutomationProperties.SetName(
                CandidateList,
                OpenVisionLanguageService.T("VisionTool.AutoMPoint.CandidateList"));
        }
    }
}
