using Lib.OpenCV.Result;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolMatchingPropertyRuntime<TProperty> : IDisposable
    {
        private readonly VisionToolPropertyGridPresenter<TProperty> presenter;
        private readonly VisionToolPropertyGridHost propertyGridController;
        private readonly VisionToolPropertyChangeController propertyChangeController;
        private readonly VisionToolDebouncedPreviewScheduler autoPreviewScheduler;
        private readonly TextBlock summaryText;
        private readonly TextBlock templateStatusText;
        private readonly Control templateStatusIcon;
        private readonly VisionToolMatchingResultReviewPresenter resultReviewPresenter;
        private readonly VisionToolMatchingVerificationGuidePresenter verificationGuidePresenter;
        private readonly Action refreshOverlay;

        private VisionToolMatchingPropertyRuntime(
            FrameworkElement owner,
            Border propertyGridHost,
            VisionToolPropertyGridPresenter<TProperty> presenter,
            TextBlock summaryText,
            TextBlock templateStatusText,
            Control templateStatusIcon,
            VisionToolVerificationGuideView verificationGuideView,
            TextBlock resultReviewText,
            TextBlock resultGuidanceText,
            Panel resultReviewChips,
            Action requestPreview,
            Action refreshOverlay)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.summaryText = summaryText ?? throw new ArgumentNullException(nameof(summaryText));
            this.templateStatusText = templateStatusText ?? throw new ArgumentNullException(nameof(templateStatusText));
            this.templateStatusIcon = templateStatusIcon;
            verificationGuidePresenter = verificationGuideView == null
                ? null
                : new VisionToolMatchingVerificationGuidePresenter(verificationGuideView);
            resultReviewPresenter = OpenVisionToolOpenProfiler.Measure(
                "CreateMatchingResultReviewPresenter",
                () => new VisionToolMatchingResultReviewPresenter(
                    owner,
                    resultReviewText,
                    resultGuidanceText,
                    resultReviewChips,
                    verificationGuidePresenter));
            if (requestPreview != null)
            {
                // Matching variants share the same delayed preview path after PropertyGrid edits.
                autoPreviewScheduler = new VisionToolDebouncedPreviewScheduler(owner, requestPreview, 120);
            }

            this.refreshOverlay = refreshOverlay;
            propertyChangeController = OpenVisionToolOpenProfiler.Measure(
                "CreateMatchingPropertyChangeController",
                () => new VisionToolPropertyChangeController(
                    UpdateSummary,
                    ClearResultReview,
                    e =>
                    {
                        presenter.ReloadTemplateIfPatternChanged(e);
                        presenter.PersistSelectedObject();
                    },
                    refreshOverlay,
                    schedulePreview: ScheduleAutoPreview,
                    shouldSchedulePreview: VisionToolPropertyPreviewPolicy.ShouldScheduleAutoPreview));
            propertyGridController = OpenVisionToolOpenProfiler.Measure(
                "AttachMatchingPropertyGridHost",
                () => VisionToolPropertyGridHost.Attach(
                    propertyGridHost,
                    presenter.SelectedObject,
                    propertyChangeController.OnPropertyValueChanged));

            OpenVisionToolOpenProfiler.Measure("UpdateMatchingSummary", UpdateSummary);
            OpenVisionToolOpenProfiler.Measure("ClearMatchingResultReview", ClearResultReview);
        }

        public static VisionToolMatchingPropertyRuntime<TProperty> Attach(
            FrameworkElement owner,
            Border propertyGridHost,
            VisionToolPropertyGridPresenter<TProperty> presenter,
            TextBlock summaryText,
            TextBlock templateStatusText,
            Control templateStatusIcon,
            VisionToolVerificationGuideView verificationGuideView,
            TextBlock resultReviewText,
            TextBlock resultGuidanceText,
            Panel resultReviewChips,
            Action requestPreview,
            Action refreshOverlay = null)
        {
            return new VisionToolMatchingPropertyRuntime<TProperty>(
                owner,
                propertyGridHost,
                presenter,
                summaryText,
                templateStatusText,
                templateStatusIcon,
                verificationGuideView,
                resultReviewText,
                resultGuidanceText,
                resultReviewChips,
                requestPreview,
                refreshOverlay);
        }

        public TProperty CreateProperty()
        {
            if (propertyGridController.CommitPendingEdit())
            {
                presenter.PersistSelectedObject();
                UpdateSummary();
            }

            return presenter.CreateProperty();
        }

        public void SetTemplatePathForTest(string path)
        {
            presenter.ApplyTemplatePathForTest(path);
            presenter.PersistSelectedObject();
            propertyChangeController.RefreshAfterExternalUpdate(propertyGridController, applyVisibilityRules: true);
            // Template registration changes the actual matching input even when it comes from
            // an external editor/test hook rather than a direct WPG property-change event.
            if (IsAutoPreviewEnabled())
            {
                ScheduleAutoPreview();
                return;
            }

            ClearResultReview();
        }

        public void ConfigurePropertyForTest(Action<TProperty> configure)
        {
            if (configure == null)
            {
                return;
            }

            if (presenter.SelectedObject is TProperty property)
            {
                configure(property);
                presenter.PersistSelectedObject();
                propertyChangeController.RefreshAfterExternalUpdate(propertyGridController, applyVisibilityRules: true);
                UpdateSummary();
                ClearResultReview();
            }
        }

        public bool ApplyPreset(VisionToolPreset<TProperty> preset)
        {
            if (preset == null)
            {
                return false;
            }

            if (presenter.SelectedObject is not TProperty property)
            {
                return false;
            }

            preset.ApplyTo(property);
            presenter.PersistSelectedObject();
            propertyGridController.RefreshAndApplyVisibilityRules();
            UpdateSummary();
            refreshOverlay?.Invoke();
            ClearResultReview();
            return true;
        }

        public void RefreshSelectedObject()
        {
            propertyGridController.RefreshSelectedObject();
        }

        public void UpdateSummary()
        {
            // Matching template status is refreshed through the same path as the PropertyGrid summary.
            OpenVisionLab.Contracts.VisionToolTemplateStatus templateStatus = presenter.TemplateStatus;
            VisionToolTemplateStatusPresenter.Apply(templateStatusText, templateStatusIcon, templateStatus);
            summaryText.Text = CreateDisplaySummary();
            verificationGuidePresenter?.ShowTeachingState(templateStatus, CreateCriteria());
        }

        public void RefreshInputRoiOverlay(VisionToolInlinePreviewSlot inputPreview)
        {
            if (inputPreview != null && presenter.SelectedObject is OpenCvPropertyBase property)
            {
                inputPreview.SetOpenCvRoiOverlays(property);
            }
        }

        public void SetResultReview(string title, IEnumerable<MatchingResult> results, TimeSpan? tactTime = null)
        {
            resultReviewPresenter.Show(title, results, tactTime, CreateCriteria());
        }

        public void ClearResultReview()
        {
            resultReviewPresenter.Clear();
            verificationGuidePresenter?.ShowTeachingState(presenter.TemplateStatus, CreateCriteria());
        }

        public void Dispose()
        {
            autoPreviewScheduler?.Dispose();
            propertyGridController.Dispose();
        }

        private void ScheduleAutoPreview()
        {
            autoPreviewScheduler?.Schedule();
        }

        private bool IsAutoPreviewEnabled()
        {
            return !(presenter.SelectedObject is MatchingProperty matching) || matching.AUTO_PREVIEW;
        }

        private VisionToolMatchingResultReviewCriteria CreateCriteria()
        {
            if (presenter.SelectedObject is MatchingProperty matching)
            {
                return new VisionToolMatchingResultReviewCriteria(
                    matching.SCORE_MIN,
                    matching.NUM_MATCH,
                    matching.USE_FIND_ANGLE,
                    matching.USE_FIND_SCALE,
                    matching.USE_PYRAMID_POSITION_PROPOSAL,
                    "Score >=");
            }

            if (presenter.SelectedObject is EdgeBasedMatchingProperty edgeBasedMatching)
            {
                return new VisionToolMatchingResultReviewCriteria(
                    edgeBasedMatching.SCORE_MIN,
                    edgeBasedMatching.NUM_MATCH,
                    edgeBasedMatching.USE_FIND_ANGLE,
                    edgeBasedMatching.USE_FIND_SCALE,
                    edgeBasedMatching.USE_PYRAMID_POSITION_PROPOSAL,
                    "Score >=",
                    VisionToolMatchingVerificationKind.EdgeBasedMatching,
                    CreateEdgeRangeText(edgeBasedMatching),
                    CreateEdgeSearchText(edgeBasedMatching),
                    Math.Max(1, edgeBasedMatching.MAX_TEMPLATE_POINTS));
            }

            if (presenter.SelectedObject is FeatureMatchingProperty featureMatching)
            {
                return new VisionToolMatchingResultReviewCriteria(
                    null,
                    null,
                    false,
                    false,
                    false,
                    string.Empty,
                    VisionToolMatchingVerificationKind.FeatureMatching,
                    string.Empty,
                    string.Empty,
                    null,
                    CreateFeatureCriteriaText(featureMatching));
            }

            return default;
        }

        private string CreateDisplaySummary()
        {
            VisionToolMatchingResultReviewCriteria criteria = CreateCriteria();
            List<string> parts = new List<string>();
            string criteriaText = criteria.CreateSummary();
            if (!string.IsNullOrWhiteSpace(criteriaText))
            {
                parts.Add(criteriaText);
            }

            if (presenter.SelectedObject is OpenCvPropertyBase property)
            {
                parts.Add(CreateImageProcessSummary(property));
            }

            return parts.Count == 0
                ? presenter.Summary
                : string.Join(" / ", parts);
        }

        private static string CreateImageProcessSummary(OpenCvPropertyBase property)
        {
            if (property == null)
            {
                return string.Empty;
            }

            string threshold = property.USE_THRESHOLD
                ? VisionToolVerificationText.FormatThreshold(property.THRESHOLD)
                : property.USE_ADAPTIVE_THRESHOLD
                    ? VisionToolVerificationText.FormatAdaptiveThreshold(property.ADAPTIVE_THRESHOLD)
                    : VisionToolVerificationText.OriginalImage;
            string roi = property.USE_ROI
                ? property.USE_MULTI_ROI ? VisionToolVerificationText.MultiRoi : VisionToolVerificationText.RoiOn
                : VisionToolVerificationText.FullImage;

            return threshold + " / " + roi;
        }

        private static string CreateEdgeRangeText(EdgeBasedMatchingProperty property)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0}..{1}",
                property.CANNY_LOW,
                property.CANNY_HIGH);
        }

        private static string CreateEdgeSearchText(EdgeBasedMatchingProperty property)
        {
            string search = property.USE_POSITION_REFINE && property.SEARCH_STEP > 1
                ? string.Format(CultureInfo.CurrentCulture, "{0}+refine", property.SEARCH_STEP)
                : Math.Max(1, property.SEARCH_STEP).ToString(CultureInfo.CurrentCulture);
            search += string.Format(CultureInfo.CurrentCulture, " / Greedy {0:0.###}", property.GREEDINESS);

            if (property.USE_PYRAMID_POSITION_PROPOSAL)
            {
                search += string.Format(
                    CultureInfo.CurrentCulture,
                    " / Pyramid {0}",
                    Math.Max(1, property.PYRAMID_POSITION_TOP_N));
            }

            if (property.USE_HYBRID_VERIFY)
            {
                search += string.Format(
                    CultureInfo.CurrentCulture,
                    " / Hybrid {0}",
                    Math.Max(1, property.HYBRID_VERIFY_TOP_N));
            }

            if (property.USE_UNIQUE_MATCH_VALIDATION)
            {
                search += string.Format(
                    CultureInfo.CurrentCulture,
                    " / Unique {0:0.###}",
                    property.UNIQUE_MATCH_MIN_SCORE_MARGIN);
            }

            return search;
        }

        private static string CreateFeatureCriteriaText(FeatureMatchingProperty property)
        {
            if (property == null)
            {
                return string.Empty;
            }

            return string.Join(
                " / ",
                VisionToolVerificationText.FormatFeatureRatioCriteria(property.SCORE_MIN),
                VisionToolVerificationText.FormatRansacCriteria(property.RANSAC_REPROJ_THRESHOLD));
        }
    }
}
