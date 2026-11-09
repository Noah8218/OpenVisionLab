using System;
using System.Windows.Automation;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class AutoMPointTeachingPanel : UserControl, IDisposable
    {
        private readonly VisionToolLanguageChangeController languageChangeController;
        private string statusKey = "VisionTool.AutoMPoint.Ready";
        private object[] statusArguments = Array.Empty<object>();
        private int representativeCount;

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

        internal void SetStatus(string key, params object[] arguments)
        {
            statusKey = key;
            statusArguments = arguments;
            RefreshStatus();
        }

        internal void SetRepresentativeCount(int count)
        {
            representativeCount = count;
            RepresentativeCountText.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, OpenVisionLanguageService.T("VisionTool.AutoMPoint.RepresentativeCountFormat"), count);
        }

        private void RefreshStatus() => StatusText.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, OpenVisionLanguageService.T(statusKey), statusArguments);

        private void ApplyLocalization()
        {
            if (TitleText.GetBindingExpression(TextBlock.TextProperty) == null) { TitleText.Text = OpenVisionLanguageService.T("VisionTool.AutoMPoint.Title"); }
            RefreshStatus();
            SetRepresentativeCount(representativeCount);
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
