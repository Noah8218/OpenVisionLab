using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class VisionToolParameterGuideView : UserControl
    {
        private Action<string> focusRelatedProperty;

        public VisionToolParameterGuideView()
        {
            InitializeComponent();
            ShowPrompt();
        }

        internal event EventHandler ContentPresented = delegate { };

        internal string TitleForTest => txtTitle.Text ?? string.Empty;
        internal string IdentityForTest => txtIdentity.Text ?? string.Empty;
        internal string SummaryForTest => txtSummary.Text ?? string.Empty;
        internal string ImpactForTest => txtImpact.Text ?? string.Empty;
        internal string CheckForTest => txtCheck.Text ?? string.Empty;
        internal string CoverageForTest => txtCoverage.Text ?? string.Empty;
        internal string ApplicabilityForTest => txtApplicability.Text ?? string.Empty;
        internal bool IsExpandedForTest => guideExpander.IsExpanded;

        internal void ShowPrompt()
        {
            focusRelatedProperty = null;
            guideExpander.IsExpanded = false;
            txtHeader.Text = T("VisionTool.ParameterGuide.Header");
            txtCoverage.Text = string.Empty;
            txtTitle.Text = T("VisionTool.ParameterGuide.PromptTitle");
            txtIdentity.Text = string.Empty;
            txtApplicability.Text = string.Empty;
            txtApplicability.Visibility = Visibility.Collapsed;
            txtSummary.Text = T("VisionTool.ParameterGuide.PromptDetail");
            txtImpact.Text = string.Empty;
            txtBestWhen.Text = string.Empty;
            txtRisk.Text = string.Empty;
            txtCheck.Text = T("VisionTool.ParameterGuide.PromptCheck");
            impactSection.Visibility = Visibility.Collapsed;
            bestWhenSection.Visibility = Visibility.Collapsed;
            riskSection.Visibility = Visibility.Collapsed;
            checkSection.Visibility = Visibility.Visible;
            relatedSection.Visibility = Visibility.Collapsed;
            relatedButtons.Children.Clear();
            ApplyLabels();
            AutomationProperties.SetName(this, txtHeader.Text);
            AutomationProperties.SetHelpText(this, txtSummary.Text);
        }

        internal void ShowContent(
            VisionToolParameterGuideContent content,
            Action<string> focusRelatedProperty)
        {
            if (content == null)
            {
                ShowPrompt();
                return;
            }

            this.focusRelatedProperty = focusRelatedProperty;
            guideExpander.IsExpanded = true;
            txtHeader.Text = T("VisionTool.ParameterGuide.Header");
            txtHeader.Text += " \u00B7 " + content.Title;
            txtCoverage.Text = content.Coverage;
            txtTitle.Text = content.Title;
            txtIdentity.Text = content.Identity;
            txtApplicability.Text = content.Applicability;
            txtApplicability.Visibility = string.IsNullOrWhiteSpace(content.Applicability)
                ? Visibility.Collapsed
                : Visibility.Visible;
            txtSummary.Text = content.Summary;
            txtImpact.Text = content.Impact;
            txtBestWhen.Text = content.BestWhen;
            txtRisk.Text = content.Risk;
            txtCheck.Text = content.CheckAfterPreview;
            impactSection.Visibility = VisibilityFor(content.Impact);
            bestWhenSection.Visibility = VisibilityFor(content.BestWhen);
            riskSection.Visibility = VisibilityFor(content.Risk);
            checkSection.Visibility = VisibilityFor(content.CheckAfterPreview);
            PopulateRelatedButtons(content.RelatedPropertyNames);
            ApplyLabels();
            AutomationProperties.SetName(this, txtHeader.Text + ": " + content.Title);
            AutomationProperties.SetHelpText(this, content.Summary + " " + content.Impact);
            ContentPresented(this, EventArgs.Empty);
        }

        internal void SetCompactMode(bool compact)
        {
            guideScroll.MaxHeight = compact ? 116D : 184D;
        }

        internal void SetExpandedForTest(bool expanded)
        {
            guideExpander.IsExpanded = expanded;
        }

        private void PopulateRelatedButtons(IReadOnlyList<string> propertyNames)
        {
            relatedButtons.Children.Clear();
            if (propertyNames == null || propertyNames.Count == 0)
            {
                relatedSection.Visibility = Visibility.Collapsed;
                return;
            }

            foreach (string propertyName in propertyNames)
            {
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    continue;
                }

                Button button = new Button
                {
                    Content = propertyName,
                    Tag = propertyName,
                    Margin = new Thickness(0, 0, 5, 4),
                    Padding = new Thickness(6, 2, 6, 2),
                    MinHeight = 24D,
                    ToolTip = T("VisionTool.ParameterGuide.RelatedToolTip")
                };
                AutomationProperties.SetName(
                    button,
                    T("VisionTool.ParameterGuide.RelatedAutomationPrefix") + " " + propertyName);
                button.Click += RelatedButton_Click;
                relatedButtons.Children.Add(button);
            }

            relatedSection.Visibility = relatedButtons.Children.Count == 0
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void RelatedButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string propertyName)
            {
                focusRelatedProperty?.Invoke(propertyName);
            }
        }

        private void ApplyLabels()
        {
            lblSummary.Text = T("VisionTool.ParameterGuide.SummaryLabel");
            lblImpact.Text = T("VisionTool.ParameterGuide.ImpactLabel");
            lblBestWhen.Text = T("VisionTool.ParameterGuide.BestWhenLabel");
            lblRisk.Text = T("VisionTool.ParameterGuide.RiskLabel");
            lblCheck.Text = T("VisionTool.ParameterGuide.CheckLabel");
            lblRelated.Text = T("VisionTool.ParameterGuide.RelatedLabel");
        }

        private static Visibility VisibilityFor(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private static string T(string key)
        {
            return OpenVisionLanguageService.T(key);
        }
    }
}
