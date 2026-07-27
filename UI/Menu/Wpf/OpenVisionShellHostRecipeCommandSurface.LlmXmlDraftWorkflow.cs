using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void LoadLlmXmlDraft()
        {
            string path = selectImportPipelineXmlPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("LLM XML 초안 로드가 취소되었습니다.", "LLM XML draft load canceled.");
                return;
            }

            LoadLlmXmlDraftFromPath(path);
        }

        public bool LoadLlmXmlDraftFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                StatusText = LocalText("LLM XML 초안 파일을 찾을 수 없습니다.", "LLM XML draft file was not found.");
                return false;
            }

            if (IsReviewBundlePath(path))
            {
                return LoadReviewBundleForDryRun(path);
            }

            LlmXmlDraftText = File.ReadAllText(path);
            StatusText = LocalText("LLM XML 초안 로드됨: ", "Loaded LLM XML draft: ") + Path.GetFileName(path);
            return ValidateLlmXmlDraftText(false);
        }

        private bool LoadReviewBundleForDryRun(string path)
        {
            if (!OpenVisionRecipeReviewBundleInspector.TryInspect(path, out OpenVisionRecipeReviewBundleInspection inspection))
            {
                ClearLoadedReviewBundleContext();
                LlmXmlDraftValidationReport = inspection.IntegrityReport;
                LlmXmlDraftDependencyReport = inspection.PathReport;
                SetLlmXmlDraftDependencyPlaceholder(LocalText(
                    "번들 무결성 오류를 해결한 뒤 다시 선택하세요.",
                    "Fix the bundle integrity issue, then select it again."));
                StatusText = LocalText("검토 번들 dry-run NG. 가져오지 않았습니다.", "Review bundle dry-run NG. Nothing was imported.");
                openLlmXmlReview();
                return false;
            }

            LlmXmlDraftText = inspection.PipelineXml;
            loadedReviewBundleInspection = inspection;
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            bool xmlReady = ValidateLlmXmlDraftText(false);
            StatusText = xmlReady
                ? LocalText(
                    "검토 번들 dry-run OK. XML은 검토 화면에만 로드했으며 가져오기/Preview/Run은 실행하지 않았습니다.",
                    "Review bundle dry-run OK. XML was loaded for review only; import, Preview, and Run were not executed.")
                : LocalText(
                    "검토 번들 무결성은 OK지만 XML/의존성 검토는 NG입니다. 가져오기/Preview/Run은 실행하지 않았습니다.",
                    "Review bundle integrity is OK, but XML/dependency review is NG. Import, Preview, and Run were not executed.");
            openLlmXmlReview();
            return true;
        }

        private static bool IsReviewBundlePath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && (path.EndsWith(".review.zip", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(Path.GetExtension(path), ".zip", StringComparison.OrdinalIgnoreCase));
        }

        private void ClearLoadedReviewBundleContext()
        {
            if (loadedReviewBundleInspection == null)
            {
                return;
            }

            loadedReviewBundleInspection = null;
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            LlmXmlDraftValidationReport = LocalText(
                "검토 번들에서 로드한 XML이 변경되었습니다. 다시 검증하세요.",
                "XML loaded from the review bundle changed. Validate it again.");
            LlmXmlDraftDependencyReport = LocalText(
                "번들 경로 증거 연결이 해제되었습니다.",
                "Bundle path evidence was detached.");
            SetLlmXmlDraftDependencyPlaceholder(LocalText(
                "변경된 XML을 다시 검증하세요.",
                "Validate the changed XML again."));
        }

        private void ValidateLlmXmlDraft()
        {
            ValidateLlmXmlDraftText(false);
        }

        public bool ValidateLlmXmlDraftTextForTest()
        {
            return ValidateLlmXmlDraftText(false);
        }

        private void ImportLlmXmlDraft()
        {
            if (!TryBuildLlmDraftPipeline(copyDependencies: true, out VisionPipeline pipeline, out string validationReport, out string dependencyReport))
            {
                llmXmlDraftImportReady = false;
                LlmXmlDraftValidationReport = validationReport;
                LlmXmlDraftDependencyReport = dependencyReport;
                LlmXmlDraftReviewReport = LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
                LlmXmlDraftDiffReport = LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
                StatusText = LocalText("LLM XML 초안을 가져올 수 없습니다.", "LLM XML draft is not importable.");
                RefreshCommandState();
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string basePipelineName = string.IsNullOrWhiteSpace(pipeline.Name)
                ? "LLM_Draft_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                : pipeline.Name.Trim();
            pipeline.Name = CreateUniquePipelineName(recipeName, basePipelineName);
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                pipeline.Name))
            {
                return;
            }

            if (OpenVisionRecipeDependencyReviewService.TryCopyReferenceImageToRecipe(
                recipeName,
                pipeline.Name,
                LlmReferenceImagePath,
                out string copiedReferenceImagePath))
            {
                dependencyReport = string.IsNullOrWhiteSpace(dependencyReport)
                    ? LocalText("참조 이미지 복사됨: ", "Reference image copied: ") + copiedReferenceImagePath
                    : dependencyReport + Environment.NewLine + LocalText("참조 이미지 복사됨: ", "Reference image copied: ") + copiedReferenceImagePath;
            }
            LlmXmlDraftReviewReport = BuildLlmDraftReviewReport(pipeline);
            LlmXmlDraftDiffReport = BuildLlmDraftDiffReport(pipeline);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            StatusText = LocalText("LLM XML 초안 가져오기 완료: ", "Imported LLM XML draft: ") + pipeline.Name;
            RefreshPipelineOptions(pipeline.Name);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private void UseSelectedSampleReference()
        {
            if (SelectedSampleOption?.Sample == null || string.IsNullOrWhiteSpace(SelectedSampleOption.Sample.ImageFullPath))
            {
                StatusText = LocalText("선택된 샘플 이미지를 사용할 수 없습니다.", "No selected sample image is available.");
                return;
            }

            LlmReferenceImagePath = SelectedSampleOption.Sample.ImageFullPath;
            StatusText = LocalText("참조 이미지가 샘플에서 설정됨: ", "Reference image set from sample: ") + SelectedSampleOption.Sample.SampleName;
        }

        private bool ValidateLlmXmlDraftText(bool copyDependencies)
        {
            bool ok = TryBuildLlmDraftPipeline(copyDependencies, out VisionPipeline pipeline, out string validationReport, out string dependencyReport);
            if (loadedReviewBundleInspection != null)
            {
                validationReport = loadedReviewBundleInspection.IntegrityReport
                    + Environment.NewLine
                    + Environment.NewLine
                    + validationReport;
                dependencyReport = loadedReviewBundleInspection.PathReport
                    + Environment.NewLine
                    + Environment.NewLine
                    + dependencyReport;
            }

            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            LlmXmlDraftReviewReport = ok ? BuildLlmDraftReviewReport(pipeline) : LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
            LlmXmlDraftDiffReport = ok ? BuildLlmDraftDiffReport(pipeline) : LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
            llmXmlDraftImportReady = ok;
            StatusText = ok ? LocalText("LLM XML 초안 검증 OK.", "LLM XML draft validation OK.") : LocalText("LLM XML 초안 검증 NG.", "LLM XML draft validation NG.");
            RefreshCommandState();
            return ok;
        }

        private bool TryBuildLlmDraftPipeline(
            bool copyDependencies,
            out VisionPipeline pipeline,
            out string validationReport,
            out string dependencyReport)
        {
            OpenVisionRecipeLlmDraftValidationResult result =
                OpenVisionRecipeLlmDraftValidationService.Validate(
                    new OpenVisionRecipeLlmDraftValidationRequest(
                        LlmXmlDraftText,
                        NormalizeRecipeName(selectedRecipeName),
                        SelectedLlmToolTemplate,
                        LlmReferenceImagePath,
                        loadedReviewBundleInspection == null,
                        loadedReviewBundleInspection,
                        copyDependencies,
                        OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate)
                            ? CreatePinArrayGapIntentValidationContext()
                            : null,
                        OpenVisionRecipeLlmIntent.IsDarkBandGapTemplate(SelectedLlmToolTemplate)
                            ? new OpenVisionRecipeDarkBandGapIntentValidationContext(DarkBandGapIntentRoiText)
                            : null,
                        OpenVisionRecipeLlmIntent.IsHybridRelativeRoiGapTemplate(SelectedLlmToolTemplate)
                            ? CreateHybridRelativeRoiIntentValidationContext()
                            : null));

            pipeline = result.Pipeline;
            validationReport = result.ValidationReport;
            dependencyReport = result.DependencyReport;
            LlmXmlDraftDependencyRows = result.DependencyRows;
            return result.Success;
        }

        private OpenVisionRecipePinArrayGapIntentValidationContext CreatePinArrayGapIntentValidationContext()
        {
            int sourceWidth = 0;
            int sourceHeight = 0;
            string sourceImagePath = ResolvePinGapRoiSuggestionImagePath();
            if (!string.IsNullOrWhiteSpace(sourceImagePath))
            {
                try
                {
                    BitmapFrame frame = BitmapFrame.Create(
                        new Uri(sourceImagePath, UriKind.Absolute),
                        BitmapCreateOptions.DelayCreation,
                        BitmapCacheOption.OnLoad);
                    sourceWidth = frame.PixelWidth;
                    sourceHeight = frame.PixelHeight;
                }
                catch
                {
                    sourceWidth = 0;
                    sourceHeight = 0;
                }
            }

            return new OpenVisionRecipePinArrayGapIntentValidationContext(
                PinArrayGapRoiText,
                PinArrayGapPolarityText,
                PinArrayGapMeasurementText,
                PinArrayGapRangeMaxText,
                PinArrayGapDarkThresholdText,
                PinArrayGapMinDarkCoverageRatioText,
                PinArrayGapMinPinWidthText,
                PinArrayGapMaxPinBreakWidthText,
                PinArrayGapMinGapWidthText,
                sourceWidth,
                sourceHeight);
        }

        private OpenVisionRecipeHybridRelativeRoiIntentValidationContext CreateHybridRelativeRoiIntentValidationContext()
        {
            return new OpenVisionRecipeHybridRelativeRoiIntentValidationContext(
                LlmReferenceImagePath,
                MatchingIntentSearchRoiText,
                HybridRelativeRoiText,
                HybridReferencePoseText,
                MatchingIntentScoreMinText,
                HybridScoreMarginText,
                HybridAngleMinimumText,
                HybridAngleMaximumText,
                HybridScaleRatioMinimumText,
                HybridScaleRatioMaximumText,
                HybridMinimumValidPixelRatioText);
        }

        private void SetLlmXmlDraftDependencyPlaceholder(string action)
        {
            LlmXmlDraftDependencyRows = new[]
            {
                new OpenVisionRecipeDependencyReviewRow(
                    LocalText("대기", "Waiting"),
                    "-",
                    "-",
                    "-",
                    action)
            };
        }

        private bool CanUseLlmXmlDraft()
        {
            return CanUseSelectedRecipe()
                && !string.IsNullOrWhiteSpace(LlmXmlDraftText);
        }

        private bool CanImportLlmXmlDraft()
        {
            return CanUseLlmXmlDraft() && llmXmlDraftImportReady;
        }
    }
}
