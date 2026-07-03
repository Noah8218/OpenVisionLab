using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    internal sealed class OpenVisionWorkspaceSamplePickerViewModel : ObservableObject
    {
        private readonly List<VisionPipelineSampleCatalogItem> samples;
        private readonly IReadOnlyList<OpenVisionWorkspaceSampleCatalogSourceOption> catalogSourceOptions;
        private readonly ICollectionView samplesView;
        private readonly RelayCommand openLearnDocumentCommand;
        private readonly RelayCommand selectCounterpartSampleCommand;
        private IReadOnlyList<OpenVisionWorkspaceSampleFocusOption> sampleFocusOptions;
        private IReadOnlyList<OpenVisionWorkspaceSampleLearnPathOption> learnPathOptions;
        private VisionPipelineSampleCatalogItem selectedSample;
        private OpenVisionWorkspaceSampleCatalogSourceOption selectedCatalogSourceOption;
        private OpenVisionWorkspaceSampleFocusOption selectedSampleFocusOption;
        private OpenVisionWorkspaceSampleLearnPathOption selectedLearnPathOption;
        private string searchText = string.Empty;

        public OpenVisionWorkspaceSamplePickerViewModel(IEnumerable<VisionPipelineSampleCatalogItem> samples)
        {
            this.samples = (samples ?? Enumerable.Empty<VisionPipelineSampleCatalogItem>())
                .Where(item => item != null && item.CanOpen)
                .ToList();
            catalogSourceOptions = OpenVisionWorkspaceSampleCatalogSourceOption.Create(this.samples);
            openLearnDocumentCommand = new RelayCommand(OpenLearnDocument, () => HasLearnDocument);
            selectCounterpartSampleCommand = new RelayCommand(SelectCounterpartSample, CanSelectCounterpartSample);
            selectedCatalogSourceOption = catalogSourceOptions
                .FirstOrDefault(option => option.SourceKind == VisionPipelineSampleCatalogSourceKind.Public)
                ?? catalogSourceOptions.FirstOrDefault();
            RebuildSampleFocusOptions(preferredFocusId: null);
            RebuildLearnPathOptions(preferredLearnPathId: null);

            samplesView = CollectionViewSource.GetDefaultView(this.samples);
            samplesView.Filter = MatchesSampleFilter;
            selectedSample = FirstVisibleSample();
        }

        public ICollectionView SamplesView => samplesView;

        public ICommand OpenLearnDocumentCommand => openLearnDocumentCommand;

        public ICommand SelectCounterpartSampleCommand => selectCounterpartSampleCommand;

        public IReadOnlyList<OpenVisionWorkspaceSampleCatalogSourceOption> CatalogSourceOptions => catalogSourceOptions;

        public IReadOnlyList<OpenVisionWorkspaceSampleFocusOption> SampleFocusOptions => sampleFocusOptions;

        public IReadOnlyList<OpenVisionWorkspaceSampleLearnPathOption> LearnPathOptions => learnPathOptions;

        public string DialogTitleText => LocalText("샘플 카탈로그", "Sample Catalog");

        public string HeaderText => LocalText("검증 샘플 선택", "Select Verification Sample");

        public string IntroText => LocalText(
            "목적, 도구 흐름, 기대 결과를 확인한 뒤 Main 이미지와 Sample_ 파이프라인만 준비합니다. Preview와 Run은 사용자가 직접 실행합니다.",
            "Review the goal, tool flow, and expected result. This only prepares the Main image and Sample_ pipeline; Preview and Run remain manual.");

        public string SearchLabelText => T("Localization.Search", "Search");

        public string CatalogSourceLabelText => T("PipelineSamples.CatalogSource", "Catalog Source");

        public string ActiveCatalogSourceText => SelectedCatalogSourceOption == null
            ? T("PipelineSamples.CatalogSourceEmpty", "No catalog source.")
            : SelectedCatalogSourceOption.Description;

        public string SampleFocusLabelText => LocalText("제품/툴 빠른 선택", "Product / Tool Focus");

        public string ActiveSampleFocusText => SelectedSampleFocusOption == null
            ? LocalText("현재 카탈로그의 모든 샘플을 표시합니다.", "Showing every sample in the selected catalog.")
            : SelectedSampleFocusOption.Description;

        public string LearnPathLabelText => LocalText("Learn 경로", "Learning Path");

        public string ActiveLearnPathText => SelectedLearnPathOption == null
            ? LocalText("모든 검증 샘플을 표시합니다.", "Showing all verification samples.")
            : SelectedLearnPathOption.Description;

        public string SearchHintText => LocalText("샘플명, 목적, 도구, 검증 기준 검색", "Search sample, goal, tool, or criterion");

        public string ListTitleText => LocalText("샘플 목록", "Samples");

        public string DetailTitleText => LocalText("선택 샘플 상세", "Selected Sample");

        public string BenchmarkLabelText => LocalText("검증 벤치마크", "Verification Benchmark");

        public string PairComparisonLabelText => LocalText("쌍 비교 기준", "Pair Comparison");

        public string PairDecisionLabelText => LocalText("Good/Bad 판정 가이드", "Good/Bad Decision Guide");

        public string SelectCounterpartSampleButtonText
        {
            get
            {
                if (SelectedSample != null && string.Equals(SelectedSample.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase))
                {
                    return LocalText("NG 기준 선택", "Select NG Reference");
                }

                if (SelectedSample != null
                    && (SelectedSample.ExpectsFailure
                        || string.Equals(SelectedSample.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase)))
                {
                    return LocalText("OK 기준 선택", "Select OK Reference");
                }

                return LocalText("반대 기준 선택", "Select Opposite Reference");
            }
        }

        public string LearnModeLabelText => LocalText("Learn 모드", "Learn Mode");

        public string RecommendedStartLabelText => LocalText("추천 시작점", "Recommended Start");

        public string ResultExplanationLabelText => LocalText("결과 해석", "Result Meaning");

        public string FailureCauseLabelText => LocalText("실패 원인", "Failure Causes");

        public string GoalLabelText => LocalText("목표", "Goal");

        public string FlowLabelText => LocalText("도구 흐름", "Tool Flow");

        public string ExpectedLabelText => LocalText("기대 결과", "Expected");

        public string CheckLabelText => LocalText("검증 포인트", "Check Point");

        public string FixLabelText => LocalText("NG 시 수정 포인트", "If NG");

        public string PairLabelText => LocalText("Good/Bad 쌍", "Good/Bad Pair");

        public string PipelineLabelText => LocalText("파이프라인", "Pipeline");

        public string ImageLabelText => LocalText("입력 이미지", "Input Image");

        public string OpenButtonText => LocalText("이 샘플 열기", "Open This Sample");

        public string OpenLearnAndSampleButtonText => LocalText("가이드와 샘플 열기", "Open Guide + Sample");

        public string CancelButtonText => T("Common.Cancel", "Cancel");

        public string ManualPreviewNoticeText => LocalText(
            "열기 후에는 Main 레이어와 Sample_ 파이프라인만 준비됩니다. 도구 검증은 Preview/Run을 직접 눌러 확인하십시오.",
            "After opening, only the Main layer and Sample_ pipeline are prepared. Use Preview/Run manually to verify the tool.");

        public string LearnDocumentLabelText => LocalText("따라하기 문서", "Step-by-step Guide");

        public string LearnDocumentTitleText =>
            OpenVisionWorkspaceLearnDocumentService.ResolveDocumentTitle(SelectedSample, SelectedLearnPathOption);

        public string LearnDocumentDescriptionText => HasLearnDocument
            ? string.Format(
                CultureInfo.CurrentCulture,
                LocalText("{0} 문서를 엽니다. 샘플 열기나 Preview/Run은 실행하지 않습니다.", "Open {0}. This does not open a sample or run Preview/Run."),
                LearnDocumentTitleText)
            : LocalText("연결된 Learn 문서를 찾을 수 없습니다.", "No linked Learn document found.");

        public string OpenLearnDocumentButtonText => LocalText("문서 열기", "Open Guide");

        public bool HasLearnDocument =>
            OpenVisionWorkspaceLearnDocumentService.TryResolveDocumentPath(SelectedSample, SelectedLearnPathOption, out _);

        public bool CanOpenLearnAndSample => CanSelect && HasLearnDocument;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (!SetProperty(ref searchText, value ?? string.Empty))
                {
                    return;
                }

                RefreshSampleFilter();
            }
        }

        public OpenVisionWorkspaceSampleCatalogSourceOption SelectedCatalogSourceOption
        {
            get => selectedCatalogSourceOption;
            set
            {
                OpenVisionWorkspaceSampleCatalogSourceOption next = value ?? catalogSourceOptions.FirstOrDefault();
                if (!SetProperty(ref selectedCatalogSourceOption, next))
                {
                    return;
                }

                string previousLearnPathId = selectedLearnPathOption?.Id;
                string previousFocusId = selectedSampleFocusOption?.Id;
                RebuildSampleFocusOptions(previousFocusId);
                RebuildLearnPathOptions(previousLearnPathId);
                RefreshSampleFilter();
                OnPropertyChanged(nameof(ActiveCatalogSourceText));
            }
        }

        public OpenVisionWorkspaceSampleFocusOption SelectedSampleFocusOption
        {
            get => selectedSampleFocusOption;
            set
            {
                OpenVisionWorkspaceSampleFocusOption next = value ?? sampleFocusOptions.FirstOrDefault();
                if (!SetProperty(ref selectedSampleFocusOption, next))
                {
                    return;
                }

                string previousLearnPathId = selectedLearnPathOption?.Id;
                RebuildLearnPathOptions(previousLearnPathId);
                RefreshSampleFilter();
                OnPropertyChanged(nameof(ActiveSampleFocusText));
            }
        }

        public OpenVisionWorkspaceSampleLearnPathOption SelectedLearnPathOption
        {
            get => selectedLearnPathOption;
            set
            {
                if (!SetProperty(ref selectedLearnPathOption, value ?? learnPathOptions.FirstOrDefault()))
                {
                    return;
                }

                RefreshSampleFilter();
                OnPropertyChanged(nameof(ActiveLearnPathText));
                NotifyLearnDocumentChanged();
            }
        }

        public VisionPipelineSampleCatalogItem SelectedSample
        {
            get => selectedSample;
            set
            {
                if (!SetProperty(ref selectedSample, value))
                {
                    return;
                }

                OnPropertyChanged(nameof(CanSelect));
                OnPropertyChanged(nameof(CanOpenLearnAndSample));
                OnPropertyChanged(nameof(SelectedImageSource));
                OnPropertyChanged(nameof(SelectionSummaryText));
                OnPropertyChanged(nameof(SelectedPipelineText));
                OnPropertyChanged(nameof(SelectedImageText));
                OnPropertyChanged(nameof(BenchmarkOutcomeText));
                OnPropertyChanged(nameof(BenchmarkSummaryText));
                OnPropertyChanged(nameof(BenchmarkPairText));
                OnPropertyChanged(nameof(HasPairComparison));
                OnPropertyChanged(nameof(PairComparisonVisibility));
                OnPropertyChanged(nameof(PairComparisonSummaryText));
                OnPropertyChanged(nameof(PairComparisonDetailText));
                OnPropertyChanged(nameof(PairDecisionVisibility));
                OnPropertyChanged(nameof(PairDecisionSummaryText));
                OnPropertyChanged(nameof(PairDecisionMetricText));
                OnPropertyChanged(nameof(PairDecisionChecklistText));
                OnPropertyChanged(nameof(PairDecisionNextActionText));
                OnPropertyChanged(nameof(PairDecisionQuickActionText));
                OnPropertyChanged(nameof(PairDecisionQuickWorkflowText));
                OnPropertyChanged(nameof(PairDecisionWorkflowText));
                OnPropertyChanged(nameof(SelectCounterpartSampleButtonText));
                OnPropertyChanged(nameof(LearnModeText));
                OnPropertyChanged(nameof(RecommendedStartText));
                OnPropertyChanged(nameof(ResultExplanationText));
                OnPropertyChanged(nameof(FailureCauseText));
                selectCounterpartSampleCommand.RaiseCanExecuteChanged();
                NotifyLearnDocumentChanged();
            }
        }

        public bool HasSamples => samples.Count > 0;

        public bool CanSelect => SelectedSample?.CanOpen == true;

        public int VisibleSampleCount => samplesView?.Cast<object>().Count() ?? 0;

        public ImageSource SelectedImageSource => LoadImageSource(SelectedSample?.ImageFullPath);

        public string ResultCountText
        {
            get
            {
                int visibleCount = VisibleSampleCount;
                int sourceCount = GetFocusFilteredSamples().Count;
                return string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("표시 {0} / 전체 {1}", "Visible {0} / Total {1}"),
                    visibleCount,
                    sourceCount);
            }
        }

        public string SelectionSummaryText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return LocalText("선택된 샘플 없음", "No sample selected");
                }

                string category = string.IsNullOrWhiteSpace(SelectedSample.Category) ? "-" : SelectedSample.Category.Trim();
                return SelectedSample.SampleName
                    + " / "
                    + SelectedSample.CatalogSourceDisplayName
                    + " / "
                    + category
                    + " / "
                    + SelectedSample.Width
                    + "x"
                    + SelectedSample.Height;
            }
        }

        public string SelectedPipelineText =>
            SelectedSample == null || string.IsNullOrWhiteSpace(SelectedSample.BaselinePipeline)
                ? "-"
                : SelectedSample.BaselinePipeline;

        public string SelectedImageText =>
            SelectedSample == null || string.IsNullOrWhiteSpace(SelectedSample.ImagePath)
                ? "-"
                : SelectedSample.ImagePath;

        public string BenchmarkOutcomeText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                if (SelectedSample.ExpectsFailure
                    || string.Equals(SelectedSample.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase))
                {
                    return LocalText("NG 기준 샘플", "NG reference");
                }

                if (string.Equals(SelectedSample.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase))
                {
                    return LocalText("OK 기준 샘플", "OK reference");
                }

                return LocalText("OK 기준", "OK criteria");
            }
        }

        public string BenchmarkSummaryText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                string expected = SelectedSample.ExpectedText;
                if (!string.IsNullOrWhiteSpace(expected) && expected != "-")
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("기준: {0}", "Criteria: {0}"),
                        expected);
                }

                string reason = SelectedSample.ExpectedReasonText;
                if (!string.IsNullOrWhiteSpace(reason) && reason != "-")
                {
                    return reason;
                }

                return LocalText("Pipeline Review에서 실행 결과를 검증합니다.", "Verify the run result in Pipeline Review.");
            }
        }

        public string BenchmarkPairText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                return SelectedSample.HasPair
                    ? SelectedSample.PairText
                    : LocalText("단일 샘플", "Single sample");
            }
        }

        public bool HasPairComparison => ResolvePairSamples().Count > 1;

        public Visibility PairComparisonVisibility => HasPairComparison ? Visibility.Visible : Visibility.Collapsed;

        public string PairComparisonSummaryText
        {
            get
            {
                List<VisionPipelineSampleCatalogItem> pairSamples = ResolvePairSamples();
                if (pairSamples.Count <= 1)
                {
                    return LocalText("비교 쌍 없음", "No comparison pair");
                }

                int okCount = pairSamples.Count(IsOkReference);
                int ngCount = pairSamples.Count(IsNgReference);
                return string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("OK {0} / NG {1} / 전체 {2}", "OK {0} / NG {1} / Total {2}"),
                    okCount,
                    ngCount,
                    pairSamples.Count);
            }
        }

        public string PairComparisonDetailText
        {
            get
            {
                List<VisionPipelineSampleCatalogItem> counterparts = ResolveCounterpartSamples();
                if (counterparts.Count == 0)
                {
                    return LocalText("같은 PairGroup의 반대 기준 샘플이 없습니다.", "No opposite reference in the same PairGroup.");
                }

                IEnumerable<string> summaries = counterparts.Take(3).Select(FormatPairReference);
                string detail = string.Join(", ", summaries);
                if (counterparts.Count > 3)
                {
                    detail += string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText(" 외 {0}개", " plus {0} more"),
                        counterparts.Count - 3);
                }

                return detail;
            }
        }

        public Visibility PairDecisionVisibility => PairDecisionGuide.HasGuide ? Visibility.Visible : Visibility.Collapsed;

        public string PairDecisionSummaryText => PairDecisionGuide.SummaryText;

        public string PairDecisionMetricText => PairDecisionGuide.MetricText;

        public string PairDecisionChecklistText => PairDecisionGuide.ChecklistText;

        public string PairDecisionNextActionText => PairDecisionGuide.NextActionText;

        public string PairDecisionQuickActionText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                string metricText = ResolveQuickDecisionMetricText();
                if (IsOkReference(SelectedSample))
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("\ub2e4\uc74c: NG \uae30\uc900 \uc5f4\uae30 -> \uac19\uc740 Pipeline\uc73c\ub85c {0} \ube44\uad50.", "Next: open NG reference -> compare {0} with the same pipeline."),
                        metricText);
                }

                if (IsNgReference(SelectedSample))
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("\ub2e4\uc74c: OK \uae30\uc900 \ud655\uc778 -> \uc774 NG\uc640 {0} \ube44\uad50.", "Next: verify OK reference -> compare {0} with this NG."),
                        metricText);
                }

                return string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("\ub2e4\uc74c: OK/NG\ub97c \uac19\uc740 Pipeline\uc73c\ub85c \uc2e4\ud589\ud574 {0} \ube44\uad50.", "Next: run OK/NG with the same pipeline and compare {0}."),
                    metricText);
            }
        }

        public string PairDecisionQuickWorkflowText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                if (IsOkReference(SelectedSample))
                {
                    return LocalText(
                        "검증 순서: 이 OK를 먼저 Preview/Run으로 확인하고, NG 기준을 같은 Pipeline으로 비교합니다.",
                        "Review order: verify this OK with Preview/Run, then compare the NG reference with the same pipeline.");
                }

                if (IsNgReference(SelectedSample))
                {
                    return LocalText(
                        "검증 순서: OK 기준을 먼저 확인한 뒤 이 NG와 metric 분리를 비교합니다.",
                        "Review order: check the OK reference first, then compare metric separation against this NG.");
                }

                return LocalText(
                    "검증 순서: OK/NG를 같은 Pipeline으로 실행해 metric 분리를 확인합니다.",
                    "Review order: run OK/NG with the same pipeline and check metric separation.");
            }
        }

        public string PairDecisionWorkflowText => PairDecisionGuide.WorkflowText;

        public string LearnModeText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                string flow = SelectedSample.ToolFlowText ?? string.Empty;
                string category = SelectedSample.Category ?? string.Empty;
                if (ContainsAny(flow, category, "EdgeBased"))
                {
                    return LocalText("Edge 기반 매칭 배우기", "Learn edge-based matching");
                }

                if (ContainsAny(flow, category, "Feature"))
                {
                    return LocalText("Feature 매칭 배우기", "Learn feature matching");
                }

                if (ContainsAny(flow, category, "Matching"))
                {
                    return LocalText("Matching 배우기", "Learn matching");
                }

                if (ContainsAny(flow, category, "Blob"))
                {
                    return LocalText("Blob으로 얼룩/입자 찾기", "Find stains or particles with Blob");
                }

                if (ContainsAny(flow, category, "Line", "LineGauge", "Distance"))
                {
                    return LocalText("Line으로 거리/각도 측정하기", "Measure distance or angle with Line");
                }

                if (ContainsAny(flow, category, "Contour"))
                {
                    return LocalText("Contour로 형상/개수 검출하기", "Detect shapes or counts with Contour");
                }

                if (ContainsAny(flow, category, "Mean"))
                {
                    return LocalText("Mean으로 밝기 변화 측정하기", "Measure brightness drift with Mean");
                }

                return LocalText("샘플 중심 검사 흐름 배우기", "Learn a sample-backed inspection flow");
            }
        }

        public string RecommendedStartText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                string flow = SelectedSample.ToolFlowText ?? string.Empty;
                string category = SelectedSample.Category ?? string.Empty;
                string startMode = SelectedSample.ExpectsFailure || string.Equals(SelectedSample.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase)
                    ? LocalText("OK 기준 확인 후 이 NG 기준과 비교합니다.", "Check the OK reference, then compare this NG reference.")
                    : LocalText("기본 검사로 시작하고 Preview/Run은 직접 실행합니다.", "Start basic; run Preview/Run manually.");

                if (ContainsAny(flow, category, "Matching", "Feature", "EdgeBased"))
                {
                    return startMode + " " + LocalText("템플릿/ROI/최소 점수부터 봅니다.", "Start with template, ROI, and minimum score.");
                }

                if (ContainsAny(flow, category, "Blob", "Contour"))
                {
                    return startMode + " " + LocalText("Threshold/ROI/면적/개수 기준부터 봅니다.", "Start with threshold, ROI, area, and count.");
                }

                if (ContainsAny(flow, category, "Line", "LineGauge", "Distance"))
                {
                    return startMode + " " + LocalText("ROI/극성/샘플링/길이·각도 기준부터 봅니다.", "Start with ROI, polarity, sampling, and length/angle.");
                }

                return startMode;
            }
        }

        public string ResultExplanationText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                IReadOnlyList<VisionPipelineSampleExpectedMetric> metrics = SelectedSample.ExpectedMetrics;
                if (metrics.Count == 0)
                {
                    return LocalText("Pipeline Review에서 실제 metric을 기준과 비교합니다.", "Compare actual metrics against criteria in Pipeline Review.");
                }

                return string.Join(" ", metrics.Take(4).Select(FormatMetricExplanation));
            }
        }

        public string FailureCauseText
        {
            get
            {
                if (SelectedSample == null)
                {
                    return "-";
                }

                string flow = SelectedSample.ToolFlowText ?? string.Empty;
                string category = SelectedSample.Category ?? string.Empty;
                if (ContainsAny(flow, category, "Matching", "Feature", "EdgeBased"))
                {
                    return LocalText("템플릿 과대/ROI 과대/점수 낮음/후보 과다/Edge 부족을 확인합니다.", "Check oversized template/ROI, low score, too many candidates, or weak edges.");
                }

                if (ContainsAny(flow, category, "Blob", "Contour"))
                {
                    return LocalText("Threshold 범위, ROI, 면적 제한, Morphology 크기를 확인합니다.", "Check threshold range, ROI, area limits, and morphology size.");
                }

                if (ContainsAny(flow, category, "Line", "LineGauge", "Distance"))
                {
                    return LocalText("Edge 부족, ROI/극성/샘플링 간격 불일치를 확인합니다.", "Check weak edges and ROI/polarity/sampling mismatch.");
                }

                string fixGuide = SelectedSample.FixGuideText;
                if (!string.IsNullOrWhiteSpace(fixGuide) && fixGuide != "-")
                {
                    return fixGuide;
                }

                return LocalText("입력 레이어, ROI, 기준 metric, 파라미터 범위를 확인합니다.", "Check input layer, ROI, target metrics, and parameter ranges.");
            }
        }

        private VisionPipelineSampleCatalogItem FirstVisibleSample()
        {
            return samplesView.Cast<VisionPipelineSampleCatalogItem>().FirstOrDefault();
        }

        private void RefreshSampleFilter()
        {
            samplesView.Refresh();
            if (SelectedSample == null || !MatchesSampleFilter(SelectedSample))
            {
                SelectedSample = FirstVisibleSample();
            }

            OnPropertyChanged(nameof(ResultCountText));
            OnPropertyChanged(nameof(VisibleSampleCount));
            selectCounterpartSampleCommand.RaiseCanExecuteChanged();
        }

        private void RebuildLearnPathOptions(string preferredLearnPathId)
        {
            List<VisionPipelineSampleCatalogItem> sourceSamples = GetFocusFilteredSamples();
            learnPathOptions = OpenVisionWorkspaceSampleLearnPathOption.Create(sourceSamples);
            selectedLearnPathOption = ResolveLearnPathOption(preferredLearnPathId);
            OnPropertyChanged(nameof(LearnPathOptions));
            OnPropertyChanged(nameof(SelectedLearnPathOption));
            OnPropertyChanged(nameof(ActiveLearnPathText));
            NotifyLearnDocumentChanged();
        }

        private void RebuildSampleFocusOptions(string preferredFocusId)
        {
            List<VisionPipelineSampleCatalogItem> sourceSamples = GetSourceFilteredSamples();
            sampleFocusOptions = OpenVisionWorkspaceSampleFocusOption.Create(sourceSamples);
            selectedSampleFocusOption = ResolveSampleFocusOption(preferredFocusId);
            OnPropertyChanged(nameof(SampleFocusOptions));
            OnPropertyChanged(nameof(SelectedSampleFocusOption));
            OnPropertyChanged(nameof(ActiveSampleFocusText));
        }

        private void NotifyLearnDocumentChanged()
        {
            OnPropertyChanged(nameof(HasLearnDocument));
            OnPropertyChanged(nameof(CanOpenLearnAndSample));
            OnPropertyChanged(nameof(LearnDocumentTitleText));
            OnPropertyChanged(nameof(LearnDocumentDescriptionText));
            openLearnDocumentCommand.RaiseCanExecuteChanged();
        }

        public void OpenLearnDocumentForSelection()
        {
            if (!HasLearnDocument)
            {
                return;
            }

            OpenLearnDocument();
        }

        private void OpenLearnDocument()
        {
            OpenVisionWorkspaceLearnDocumentService.OpenDocument(SelectedSample, SelectedLearnPathOption);
        }

        private bool CanSelectCounterpartSample()
        {
            return ResolveVisibleCounterpartSample() != null;
        }

        private void SelectCounterpartSample()
        {
            VisionPipelineSampleCatalogItem counterpart = ResolveVisibleCounterpartSample();
            if (counterpart == null)
            {
                return;
            }

            SelectedSample = counterpart;
            samplesView.MoveCurrentTo(counterpart);
        }

        private OpenVisionWorkspaceSampleLearnPathOption ResolveLearnPathOption(string preferredLearnPathId)
        {
            if (!string.IsNullOrWhiteSpace(preferredLearnPathId))
            {
                OpenVisionWorkspaceSampleLearnPathOption match = learnPathOptions
                    .FirstOrDefault(option => string.Equals(option.Id, preferredLearnPathId, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    return match;
                }
            }

            return learnPathOptions.FirstOrDefault();
        }

        private OpenVisionWorkspaceSampleFocusOption ResolveSampleFocusOption(string preferredFocusId)
        {
            if (!string.IsNullOrWhiteSpace(preferredFocusId))
            {
                OpenVisionWorkspaceSampleFocusOption match = sampleFocusOptions
                    .FirstOrDefault(option => string.Equals(option.Id, preferredFocusId, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    return match;
                }
            }

            return sampleFocusOptions.FirstOrDefault();
        }

        private List<VisionPipelineSampleCatalogItem> GetSourceFilteredSamples()
        {
            if (SelectedCatalogSourceOption == null)
            {
                return samples.ToList();
            }

            return samples
                .Where(item => SelectedCatalogSourceOption.Matches(item))
                .ToList();
        }

        private List<VisionPipelineSampleCatalogItem> GetFocusFilteredSamples()
        {
            IEnumerable<VisionPipelineSampleCatalogItem> sourceSamples = GetSourceFilteredSamples();
            if (SelectedSampleFocusOption == null)
            {
                return sourceSamples.ToList();
            }

            return sourceSamples
                .Where(item => SelectedSampleFocusOption.Matches(item))
                .ToList();
        }

        private List<VisionPipelineSampleCatalogItem> ResolvePairSamples()
        {
            if (SelectedSample == null || string.IsNullOrWhiteSpace(SelectedSample.PairGroup))
            {
                return new List<VisionPipelineSampleCatalogItem>();
            }

            string pairGroup = SelectedSample.PairGroup.Trim();
            return samples
                .Where(item => item != null
                    && item.CanOpen
                    && string.Equals(item.PairGroup?.Trim(), pairGroup, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => IsOkReference(item) ? 0 : 1)
                .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private List<VisionPipelineSampleCatalogItem> ResolveCounterpartSamples()
        {
            if (SelectedSample == null)
            {
                return new List<VisionPipelineSampleCatalogItem>();
            }

            bool selectedIsOk = IsOkReference(SelectedSample);
            bool selectedIsNg = IsNgReference(SelectedSample);
            return ResolvePairSamples()
                .Where(item => !ReferenceEquals(item, SelectedSample))
                .Where(item =>
                    selectedIsOk
                        ? IsNgReference(item)
                        : selectedIsNg
                            ? IsOkReference(item)
                            : true)
                .ToList();
        }

        private VisionPipelineSampleCatalogItem ResolveVisibleCounterpartSample()
        {
            return ResolveCounterpartSamples()
                .FirstOrDefault(MatchesSampleFilter);
        }

        private string ResolveQuickDecisionMetricText()
        {
            if (SelectedSample == null)
            {
                return LocalText("metric", "metrics");
            }

            HashSet<string> selectedNames = new HashSet<string>(
                SelectedSample.ExpectedMetrics
                    .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
                    .Select(metric => metric.Name.Trim()),
                StringComparer.OrdinalIgnoreCase);
            if (selectedNames.Count == 0)
            {
                return LocalText("metric", "metrics");
            }

            HashSet<string> counterpartNames = new HashSet<string>(
                ResolveCounterpartSamples()
                    .SelectMany(item => item.ExpectedMetrics)
                    .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
                    .Select(metric => metric.Name.Trim()),
                StringComparer.OrdinalIgnoreCase);

            List<string> commonNames = selectedNames
                .Where(counterpartNames.Contains)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .ToList();
            if (commonNames.Count == 0)
            {
                commonNames = selectedNames
                    .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                    .Take(2)
                    .ToList();
            }

            return commonNames.Count == 0
                ? LocalText("metric", "metrics")
                : string.Join(", ", commonNames);
        }

        private OpenVisionWorkspaceSamplePairDecisionGuide PairDecisionGuide =>
            OpenVisionWorkspaceSamplePairDecisionGuidePresenter.Create(SelectedSample, ResolvePairSamples());

        private static bool IsOkReference(VisionPipelineSampleCatalogItem item)
        {
            return item != null
                && !item.ExpectsFailure
                && string.Equals(item.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNgReference(VisionPipelineSampleCatalogItem item)
        {
            return item != null
                && (item.ExpectsFailure
                    || string.Equals(item.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase));
        }

        private static bool ContainsAny(string text, string secondText, params string[] tokens)
        {
            string first = text ?? string.Empty;
            string second = secondText ?? string.Empty;
            return tokens.Any(token =>
                first.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0
                || second.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private string FormatMetricExplanation(VisionPipelineSampleExpectedMetric metric)
        {
            if (metric == null || string.IsNullOrWhiteSpace(metric.Name))
            {
                return string.Empty;
            }

            string range = FormatMetricRange(metric);
            string name = metric.Name.Trim();
            if (string.Equals(name, "ScoreMax", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(CultureInfo.CurrentCulture, LocalText("점수 {0} OK.", "Score {0} OK."), range);
            }

            if (string.Equals(name, "ResultCount", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(CultureInfo.CurrentCulture, LocalText("후보 {0} OK.", "Count {0} OK."), range);
            }

            if (name.IndexOf("Angle", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return string.Format(CultureInfo.CurrentCulture, LocalText("각도 {0}.", "Angle {0}."), range);
            }

            if (name.IndexOf("Width", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("Area", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return string.Format(CultureInfo.CurrentCulture, LocalText("크기/면적 {0} 벗어나면 NG.", "Size/area outside {0} is NG."), range);
            }

            if (name.IndexOf("Edge", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return string.Format(CultureInfo.CurrentCulture, LocalText("Edge {0} 신뢰도.", "Edge {0} confidence."), range);
            }

            if (name.IndexOf("Mean", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return string.Format(CultureInfo.CurrentCulture, LocalText("평균 밝기 {0} 정상.", "Mean brightness {0} normal."), range);
            }

            return string.Format(CultureInfo.CurrentCulture, LocalText("{0} {1} 기준을 확인합니다.", "Check {0} {1}."), name, range);
        }

        private static string FormatMetricRange(VisionPipelineSampleExpectedMetric metric)
        {
            string minimum = metric.Minimum?.Trim() ?? string.Empty;
            string maximum = metric.Maximum?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(minimum) && !string.IsNullOrWhiteSpace(maximum))
            {
                return minimum == maximum ? minimum : minimum + "~" + maximum;
            }

            if (!string.IsNullOrWhiteSpace(minimum))
            {
                return ">= " + minimum;
            }

            if (!string.IsNullOrWhiteSpace(maximum))
            {
                return "<= " + maximum;
            }

            return "-";
        }

        private string FormatPairReference(VisionPipelineSampleCatalogItem item)
        {
            string role = IsNgReference(item)
                ? LocalText("NG", "NG")
                : IsOkReference(item)
                    ? LocalText("OK", "OK")
                    : LocalText("참조", "Reference");
            return role + ": " + item.SampleName;
        }

        private bool MatchesSampleFilter(object value)
        {
            if (value is not VisionPipelineSampleCatalogItem sample)
            {
                return false;
            }

            if (SelectedCatalogSourceOption != null && !SelectedCatalogSourceOption.Matches(sample))
            {
                return false;
            }

            if (SelectedSampleFocusOption != null && !SelectedSampleFocusOption.Matches(sample))
            {
                return false;
            }

            if (SelectedLearnPathOption != null && !SelectedLearnPathOption.Matches(sample))
            {
                return false;
            }

            string query = searchText?.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                return true;
            }

            string[] tokens = query.Split(new[] { ' ', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                return true;
            }

            string searchable = string.Join(
                " ",
                sample.SampleName,
                sample.Category,
                sample.Goal,
                sample.ToolFlowText,
                sample.ExpectedText,
                sample.PairText,
                sample.CatalogSourceDisplayName,
                sample.CheckGuideText,
                sample.FixGuideText);

            return tokens.All(token => searchable.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static ImageSource LoadImageSource(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }

            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.DecodePixelWidth = 420;
                image.UriSource = new Uri(path, UriKind.Absolute);
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }
    }
}
