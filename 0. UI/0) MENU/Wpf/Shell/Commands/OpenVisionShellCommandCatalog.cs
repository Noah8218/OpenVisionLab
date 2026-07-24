using MahApps.Metro.IconPacks;
using OpenVisionLab.Mvvm;
using OpenVisionLab.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellPreviewViewModel : ObservableObject, IDisposable
    {
        private static readonly Brush ToolInputMissingBrush = new SolidColorBrush(Color.FromRgb(244, 197, 66));
        private static readonly Brush ToolConfigurableBrush = new SolidColorBrush(Color.FromRgb(105, 214, 154));
        private OpenVisionShellNavItem selectedItem;
        private OpenVisionLanguageOption selectedLanguageOption;
        private string selectedLayerResultName = "Main";
        private IReadOnlyList<string> layerOptions;
        private bool hasWorkspaceImage;
        private bool arithmeticRequiresInputLayerB;
        private bool hasArithmeticInputLayerB;
        private bool lineScaleNeedsReview = true;
        private double lineScaleMmPerPixel;
        private string toolSearchText = string.Empty;
        private readonly HashSet<VISION_MENU> templateReadyTools = new HashSet<VISION_MENU>();

        public OpenVisionShellPreviewViewModel(IReadOnlyList<OpenVisionShellNavGroup> navigationGroups)
        {
            LanguageOptions = OpenVisionLanguageService.GetLanguageOptions();
            selectedLanguageOption = ResolveLanguageOption(OpenVisionLanguageService.CurrentLanguage);
            NavigationGroups = navigationGroups ?? Array.Empty<OpenVisionShellNavGroup>();
            layerOptions = new[] { "Main" };
            ApplyToolReadiness();
            ApplyToolFilter();
            SelectToolCommand = new RelayCommand<object>(parameter => SelectTool(parameter as OpenVisionShellNavItem));
            SelectLayerResultCommand = new RelayCommand<object>(SelectLayerResult);
            ClearToolSearchCommand = new RelayCommand<object>(_ => ToolSearchText = string.Empty);
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;

            // Startup is workspace-first: tool windows open only after the operator chooses a tool.
            ClearActiveTools();
        }


        public IReadOnlyList<OpenVisionShellNavGroup> NavigationGroups { get; private set; }
        public IReadOnlyList<OpenVisionLanguageOption> LanguageOptions { get; }
        public IReadOnlyList<string> LayerOptions => layerOptions;
        public ICommand SelectToolCommand { get; }
        public ICommand SelectLayerResultCommand { get; }
        public ICommand ClearToolSearchCommand { get; }
        public string ToolSearchText
        {
            get => toolSearchText;
            set
            {
                string next = value ?? string.Empty;
                if (string.Equals(toolSearchText, next, StringComparison.Ordinal))
                {
                    return;
                }

                toolSearchText = next;
                OnPropertyChanged(nameof(ToolSearchText));
                OnPropertyChanged(nameof(IsToolSearchActive));
                ApplyToolFilter();
            }
        }
        public bool IsToolSearchActive => !string.IsNullOrWhiteSpace(toolSearchText);
        public string ToolSearchHintText => T("Shell.ToolSearch.Hint", "Search tool, intent, parameter, or result");
        public string ToolSearchClearToolTipText => T("Shell.ToolSearch.ClearToolTip", "Clear tool search");
        public string ToolSearchLearnToolTipText => T("Shell.ToolSearch.OpenLearnToolTip", "Open this tool's Learn topic");
        public string ToolSearchSamplesToolTipText => T("Shell.ToolSearch.OpenSamplesToolTip", "Open samples for this tool");
        public string ToolSearchGuidedSetupToolTipText => T("Shell.ToolSearch.OpenGuidedSetupToolTip", "Open Guided Setup for this tool");
        public int VisibleToolCount => NavigationGroups.SelectMany(group => group.Items).Count(item => item.IsVisible);
        public string VisibleToolCommandIds => string.Join(",", NavigationGroups
            .SelectMany(group => group.Items)
            .Where(item => item.IsVisible)
            .Select(item => item.CommandId));
        public string ToolSearchSummaryText => string.IsNullOrWhiteSpace(toolSearchText)
            ? TF("Shell.ToolSearch.AllFormat", "All {0} tools", VisibleToolCount)
            : TF("Shell.ToolSearch.ResultFormat", "{0} tools found", VisibleToolCount);
        public string SelectedLayerOption
        {
            get => selectedLayerResultName;
            set => SelectLayerResult(value);
        }
        public OpenVisionLanguageOption SelectedLanguageOption
        {
            get => selectedLanguageOption;
            set
            {
                if (value == null || selectedLanguageOption.Language == value.Language) { return; }
                selectedLanguageOption = value;
                OnPropertyChanged(nameof(SelectedLanguageOption));

                bool languageChanged = OpenVisionLanguageService.CurrentLanguage != value.Language;
                OpenVisionLanguageService.SetLanguage(value.Language);
                if (!languageChanged)
                {
                    RefreshLocalization();
                }
            }
        }

        public OpenVisionShellNavItem SelectedItem
        {
            get => selectedItem;
            private set
            {
                if (ReferenceEquals(selectedItem, value))
                {
                    // Re-clicking the selected tool should still let the host reopen a
                    // manually closed floating tool window.
                    OnPropertyChanged(nameof(SelectedItem));
                    return;
                }

                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(SelectedToolPreviewTitle));
                OnPropertyChanged(nameof(SelectedInputPreviewText));
                OnPropertyChanged(nameof(SelectedOutputPreviewText));
                OnPropertyChanged(nameof(SelectedDirectRunText));
                OnPropertyChanged(nameof(SelectedRouteText));
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public void SetLayerOptions(IEnumerable<string> layerNames, string selectedLayer)
        {
            IReadOnlyList<string> nextOptions = (layerNames ?? Array.Empty<string>())
                .Where(layer => !string.IsNullOrWhiteSpace(layer))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (nextOptions.Count == 0)
            {
                nextOptions = new[] { "Main" };
            }

            bool optionsChanged = !layerOptions.SequenceEqual(nextOptions, StringComparer.OrdinalIgnoreCase);
            layerOptions = nextOptions;
            if (optionsChanged)
            {
                OnPropertyChanged(nameof(LayerOptions));
            }

            string nextSelected = !string.IsNullOrWhiteSpace(selectedLayer)
                && nextOptions.Contains(selectedLayer, StringComparer.OrdinalIgnoreCase)
                    ? selectedLayer
                    : nextOptions[0];
            if (!string.Equals(selectedLayerResultName, nextSelected, StringComparison.OrdinalIgnoreCase))
            {
                selectedLayerResultName = nextSelected;
                RaiseLayerResultSelectionChanged();
            }
            else if (optionsChanged)
            {
                OnPropertyChanged(nameof(SelectedLayerOption));
            }
        }

        public void SetToolReadiness(
            bool workspaceImageReady,
            bool arithmeticInputLayerBRequired,
            bool arithmeticInputLayerBReady,
            VisionToolRepository repository)
        {
            hasWorkspaceImage = workspaceImageReady;
            arithmeticRequiresInputLayerB = arithmeticInputLayerBRequired;
            hasArithmeticInputLayerB = arithmeticInputLayerBReady;
            ResolveLineScale(repository, out lineScaleNeedsReview, out lineScaleMmPerPixel);
            templateReadyTools.Clear();

            MatchingProperty matching = repository?.Matchings.FirstOrDefault();
            if (matching != null
                && VisionToolPropertySummaryViewModel.CreateTemplateStatus(matching.PATTERN_PATH, matching.ImageTemplate).IsReady)
            {
                templateReadyTools.Add(VISION_MENU.Matching);
            }

            var edgeBasedMatching = repository?.EdgeBasedMatchings.FirstOrDefault();
            if (edgeBasedMatching != null
                && VisionToolPropertySummaryViewModel.CreateTemplateStatus(edgeBasedMatching.PATTERN_PATH, edgeBasedMatching.ImageTemplate).IsReady)
            {
                templateReadyTools.Add(VISION_MENU.EdgeBasedMatching);
            }

            var featureMatching = repository?.Features.FirstOrDefault();
            if (featureMatching != null
                && VisionToolPropertySummaryViewModel.CreateTemplateStatus(featureMatching.PATTERN_PATH, featureMatching.ImageTemplate).IsReady)
            {
                templateReadyTools.Add(VISION_MENU.FeatureMatching);
            }

            ApplyToolReadiness();
        }

        public string SelectedToolPreviewTitle => SelectedItem == null
            ? T("Shell.ToolPreview", "Tool Preview")
            : TF("Shell.ToolPreviewFormat", "{0} Preview", SelectedItem.Title);
        public string SelectedInputPreviewText => SelectedItem == null
            ? T("Pipeline.PreviewMode.Input", "Input")
            : TF("Shell.InputPreviewFormat", "{0} Input", SelectedItem.Title);
        public string SelectedOutputPreviewText => SelectedItem == null
            ? T("Pipeline.PreviewMode.Output", "Output")
            : TF("Shell.OutputPreviewFormat", "{0} Output", SelectedItem.Title);
        public string SelectedDirectRunText => SelectedItem == null
            ? T("Shell.NoToolSelected", LocalText("도구를 선택하세요", "Select a tool"))
            : TF("Shell.SelectedToolFormat", "{0} (selected)", SelectedItem.Title);
        public string SelectedRouteText => SelectedItem == null
            ? T("Shell.RouteNoTool", LocalText("도구를 선택하면 예상 경로가 표시됩니다.", "Select a tool to preview a route."))
            : TF("Shell.RouteFormat", "Expected: Main -> {0}", SelectedItem.CommandId + "_Preview");
        public string SubtitleText => T("App.Subtitle", "Rule-based Vision Workbench");
        public string ImageProcessingText => T("Menu.ImageProcessing", "Image Processing");
        public string AlgorithmText => T("Menu.Algorithm", "Algorithm");
        public string LearnText => T("Menu.Learn", "Learn");
        public string LearnToolTipText => T("Menu.Learn.ToolTip", "Open OpenVisionLab Learn.");
        public string GuideText => T("Menu.Guide", "Guide");
        public string GuideToolTipText => T("Menu.Guide.ToolTip", "Open the OpenVisionLab tutorial.");
        public string WorkbenchText => T("Shell.Workbench", "Workbench");
        public string CameraText => T("Shell.Camera", "Camera");
        public string LayerText => T("Main.Layer", "Layer");
        public string LayerInputText => T("Main.LayerInput", "Layer Input");
        public string SelectText => T("Shell.Select", "Select");
        public string AllText => T("Shell.All", "All");
        public string StopText => T("Main.Stop", "Stop");
        public string SettingsToolTipText => T("Menu.Settings.ToolTip", "Change language and edit translation text.");
        public string ExportToolTipText => T("Common.Export", "Export");
        public string MinimizeToolTipText => T("Common.Minimize", "Minimize");
        public string MaximizeToolTipText => T("Common.Maximize", "Maximize");
        public string CloseToolTipText => T("Common.Close", "Close");
        public string MainLayerText => T("Shell.MainLayer", "Main Layer");
        public string SelectedCanvasLayerText => TF("Shell.SelectedLayerCanvasFormat", "{0} Layer", selectedLayerResultName);
        public string SelectedCanvasLayerStatusText => TF("Shell.CanvasLayerStatusFormat", "Layer {0} | {1}", selectedLayerResultName, ResolveLayerResultSize(selectedLayerResultName));
        public string ResultPreviewText => T("Shell.ResultPreview", "Result Preview");
        public string DirectResultText => T("Shell.DirectResult", "Direct Result");
        public string DirectBadgeReadyText => T("Shell.DirectBadgeReady", "Ready");
        public string DirectStatusReadyDetailText => T("Shell.DirectStatusReadyDetail", "Run the tool to show a result.");
        public string DirectStatusNoToolText => T("Shell.DirectStatusSelectTool", LocalText("왼쪽 목록에서 검사 도구를 선택하세요.", "Select an inspection tool from the left list."));
        public string DirectStatusPassedText => T("Shell.StatusPassed", "Status: Passed");
        public string DirectElapsedSampleText => T("Shell.ElapsedSample", "Elapsed: 14.2 ms");
        public string LayersResultsText => T("Shell.LayersResults", "Layers / Results");
        public string SelectedLayerDetailText => T("Shell.SelectedLayerDetail", "Selected Layer");
        public string LayerResultMainText => TF("Shell.LayerResultMainFormat", "02  Main  {0}", T("PipelineBatch.Filter.Passed", "Passed"));
        public string LayerResultNoneText => TF("Shell.LayerResultNoneFormat", "03  NewPanel_LPV  {0}", T("Shell.LayerStateNone", "none"));
        public string LayerResultDisplayText => TF("Shell.LayerResultDisplayFormat", "04  NewPanel_sbA  {0}", T("Shell.LayerStateDisplay", "display"));
        public bool IsMainLayerResultSelected => string.Equals(selectedLayerResultName, "Main", StringComparison.OrdinalIgnoreCase);
        public bool IsLpvLayerResultSelected => string.Equals(selectedLayerResultName, "NewPanel_LPV", StringComparison.OrdinalIgnoreCase);
        public bool IsSbALayerResultSelected => string.Equals(selectedLayerResultName, "NewPanel_sbA", StringComparison.OrdinalIgnoreCase);
        public string PipelineText => T("VisionMenu.Pipeline", "Pipeline");
        public string PipelineFilterPassedText => FormatPipelineStep("01", "VisionMenu.Filter", "Filter", T("PipelineBatch.Filter.Passed", "Passed"));
        public string PipelineMorphologyPassedText => FormatPipelineStep("02", "VisionMenu.Morphology", "Morphology", T("PipelineBatch.Filter.Passed", "Passed"));
        public string PipelineContourNeedsPreviewText => FormatPipelineStep("03", "VisionMenu.Contour", "Contour", T("Pipeline.State.NeedsPreview", "Needs Preview"));
        public string RunLogText => T("Pipeline.RunLog", "Run Log");
        public string StatusText => SelectedItem == null
            ? T("Shell.ReadyFoundation", "Ready")
            : TF("Shell.ReadySelectedFormat", "Ready | Selected: {0}", SelectedItem.Title);

        public static OpenVisionShellPreviewViewModel CreatePreview()
        {
            OpenVisionLanguageService.Load();
            return new OpenVisionShellPreviewViewModel(OpenVisionShellCommandCatalog.CreateNavigationGroups());
        }

        public void Dispose()
        {
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            OpenVisionLanguageOption currentOption = ResolveLanguageOption(OpenVisionLanguageService.CurrentLanguage);
            if (selectedLanguageOption.Language != currentOption.Language)
            {
                selectedLanguageOption = currentOption;
                OnPropertyChanged(nameof(SelectedLanguageOption));
            }

            RefreshLocalization();
        }

        private void RefreshLocalization()
        {
            VISION_MENU? selectedMenu = SelectedItem?.Menu;
            NavigationGroups = OpenVisionShellCommandCatalog.CreateNavigationGroups();
            ApplyToolReadiness();
            ApplyToolFilter();
            OnPropertyChanged(nameof(NavigationGroups));
            RaiseLocalizedTextChanged();

            if (selectedMenu.HasValue)
            {
                OpenVisionShellNavItem nextItem = NavigationGroups
                    .SelectMany(group => group.Items)
                    .FirstOrDefault(item => item.Menu == selectedMenu.Value);
                RestoreSelectedToolForLocalization(nextItem);
                return;
            }

            ClearActiveTools();
        }

        private OpenVisionLanguageOption ResolveLanguageOption(OpenVisionLanguage language)
        {
            return LanguageOptions.FirstOrDefault(option => option.Language == language)
                ?? LanguageOptions.First();
        }

        private void SelectTool(OpenVisionShellNavItem item)
        {
            if (item == null) { return; }

            foreach (OpenVisionShellNavItem command in NavigationGroups.SelectMany(group => group.Items))
            {
                command.SetActive(ReferenceEquals(command, item));
            }

            SelectedItem = item;
        }

        private void RestoreSelectedToolForLocalization(OpenVisionShellNavItem item)
        {
            if (item == null)
            {
                ClearActiveTools();
                selectedItem = null;
                RaiseSelectedToolTextChanged();
                return;
            }

            foreach (OpenVisionShellNavItem command in NavigationGroups.SelectMany(group => group.Items))
            {
                command.SetActive(ReferenceEquals(command, item));
            }

            selectedItem = item;
            RaiseSelectedToolTextChanged();
        }

        private void RaiseSelectedToolTextChanged()
        {
            OnPropertyChanged(nameof(SelectedToolPreviewTitle));
            OnPropertyChanged(nameof(SelectedInputPreviewText));
            OnPropertyChanged(nameof(SelectedOutputPreviewText));
            OnPropertyChanged(nameof(SelectedDirectRunText));
            OnPropertyChanged(nameof(SelectedRouteText));
            OnPropertyChanged(nameof(StatusText));
        }

        private void ClearActiveTools()
        {
            foreach (OpenVisionShellNavItem command in NavigationGroups.SelectMany(group => group.Items))
            {
                command.SetActive(false);
            }
        }

        private void ApplyToolReadiness()
        {
            foreach (OpenVisionShellNavItem item in NavigationGroups.SelectMany(group => group.Items))
            {
                if (!item.RequiresWorkspaceImage)
                {
                    continue;
                }

                if (!hasWorkspaceImage)
                {
                    item.SetReadiness(
                        T("Shell.ToolReadiness.InputMissingBadge", "No input"),
                        ToolInputMissingBrush,
                        T("Shell.ToolReadiness.InputMissingDescription", "Load a Main image before configuring this tool. Preview/Run will not start automatically."));
                    continue;
                }

                if (item.RequiresTemplate && !templateReadyTools.Contains(item.Menu))
                {
                    item.SetReadiness(
                        T("Shell.ToolReadiness.TemplateMissingBadge", "Template needed"),
                        ToolInputMissingBrush,
                        T("Shell.ToolReadiness.TemplateMissingDescription", "The Main image is ready, but this matching tool has no registered template. Open the tool, register a template, then run Preview explicitly."));
                    continue;
                }

                if (item.Menu == VISION_MENU.Arithmetic
                    && arithmeticRequiresInputLayerB
                    && !hasArithmeticInputLayerB)
                {
                    item.SetReadiness(
                        T("Shell.ToolReadiness.InputBMissingBadge", "B needed"),
                        ToolInputMissingBrush,
                        T("Shell.ToolReadiness.InputBMissingDescription", "This Arithmetic setting needs another image layer for Input B. Load or create that layer, then run Preview explicitly."));
                    continue;
                }

                if (item.Menu == VISION_MENU.Line)
                {
                    if (lineScaleNeedsReview)
                    {
                        item.SetReadiness(
                            T("Shell.ToolReadiness.LineScaleReviewBadge", "Check scale"),
                            ToolInputMissingBrush,
                            T("Shell.ToolReadiness.LineScaleReviewDescription", "Line A/B Pixel / mm values are missing, invalid, or inconsistent. Review both values before using mm results, then run Preview explicitly."));
                        continue;
                    }

                    if (lineScaleMmPerPixel <= 0D)
                    {
                        item.SetReadiness(
                            T("Shell.ToolReadiness.LineScalePixelOnlyBadge", "PX only"),
                            ToolConfigurableBrush,
                            T("Shell.ToolReadiness.LineScalePixelOnlyDescription", "Line A/B Pixel / mm is 0. Pixel results can be configured, but mm results must not be used. Run Preview explicitly."));
                        continue;
                    }

                    string scaleText = lineScaleMmPerPixel.ToString("0.######", CultureInfo.InvariantCulture);
                    item.SetReadiness(
                        TF("Shell.ToolReadiness.LineScaleConfiguredBadgeFormat", "mm {0}", scaleText),
                        ToolConfigurableBrush,
                        TF("Shell.ToolReadiness.LineScaleConfiguredDescriptionFormat", "Line A/B Pixel / mm values match at {0} mm/px. Verify the real calibration evidence before trusting mm results, then run Preview explicitly.", scaleText));
                    continue;
                }

                item.SetReadiness(
                    T("Shell.ToolReadiness.ConfigureBadge", "Configure"),
                    ToolConfigurableBrush,
                    T("Shell.ToolReadiness.ConfigureDescription", "Main image is ready. Open the tool, configure parameters, then run Preview explicitly."));
            }
        }

        private void ApplyToolFilter()
        {
            foreach (OpenVisionShellNavGroup group in NavigationGroups)
            {
                bool hasVisibleItem = false;
                foreach (OpenVisionShellNavItem item in group.Items)
                {
                    bool isVisible = item.MatchesSearch(toolSearchText);
                    item.SetVisible(isVisible);
                    hasVisibleItem |= isVisible;
                }

                group.SetVisible(hasVisibleItem);
            }

            OnPropertyChanged(nameof(VisibleToolCount));
            OnPropertyChanged(nameof(VisibleToolCommandIds));
            OnPropertyChanged(nameof(ToolSearchSummaryText));
        }

        private static void ResolveLineScale(
            VisionToolRepository repository,
            out bool needsReview,
            out double millimetersPerPixel)
        {
            LineGaugeProperty left = repository?.Lines_L.FirstOrDefault();
            LineGaugeProperty right = repository?.Lines_R.FirstOrDefault();
            double leftScale = left?.PIXELPERMM ?? double.NaN;
            double rightScale = right?.PIXELPERMM ?? double.NaN;
            if (double.IsNaN(leftScale)
                || double.IsInfinity(leftScale)
                || leftScale < 0D
                || double.IsNaN(rightScale)
                || double.IsInfinity(rightScale)
                || rightScale < 0D)
            {
                needsReview = true;
                millimetersPerPixel = 0D;
                return;
            }

            double tolerance = Math.Max(
                1e-12D,
                Math.Max(Math.Abs(leftScale), Math.Abs(rightScale)) * 1e-6D);
            if (Math.Abs(leftScale - rightScale) > tolerance)
            {
                needsReview = true;
                millimetersPerPixel = 0D;
                return;
            }

            needsReview = false;
            millimetersPerPixel = (leftScale + rightScale) / 2D;
        }

        private void SelectLayerResult(object parameter)
        {
            string layerName = Convert.ToString(parameter, CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(layerName) || string.Equals(selectedLayerResultName, layerName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            selectedLayerResultName = layerName;
            RaiseLayerResultSelectionChanged();
            OnPropertyChanged(nameof(StatusText));
        }

        private void RaiseLayerResultSelectionChanged()
        {
            OnPropertyChanged(nameof(SelectedCanvasLayerText));
            OnPropertyChanged(nameof(SelectedCanvasLayerStatusText));
            OnPropertyChanged(nameof(SelectedLayerOption));
            OnPropertyChanged(nameof(IsMainLayerResultSelected));
            OnPropertyChanged(nameof(IsLpvLayerResultSelected));
            OnPropertyChanged(nameof(IsSbALayerResultSelected));
        }


        private void RaiseLocalizedTextChanged()
        {
            OnPropertyChanged(nameof(SubtitleText));
            OnPropertyChanged(nameof(ImageProcessingText));
            OnPropertyChanged(nameof(AlgorithmText));
            OnPropertyChanged(nameof(LearnText));
            OnPropertyChanged(nameof(LearnToolTipText));
            OnPropertyChanged(nameof(GuideText));
            OnPropertyChanged(nameof(GuideToolTipText));
            OnPropertyChanged(nameof(WorkbenchText));
            OnPropertyChanged(nameof(CameraText));
            OnPropertyChanged(nameof(LayerText));
            OnPropertyChanged(nameof(LayerInputText));
            OnPropertyChanged(nameof(SelectText));
            OnPropertyChanged(nameof(AllText));
            OnPropertyChanged(nameof(StopText));
            OnPropertyChanged(nameof(SettingsToolTipText));
            OnPropertyChanged(nameof(ExportToolTipText));
            OnPropertyChanged(nameof(MinimizeToolTipText));
            OnPropertyChanged(nameof(MaximizeToolTipText));
            OnPropertyChanged(nameof(CloseToolTipText));
            OnPropertyChanged(nameof(MainLayerText));
            OnPropertyChanged(nameof(SelectedCanvasLayerText));
            OnPropertyChanged(nameof(SelectedCanvasLayerStatusText));
            OnPropertyChanged(nameof(ResultPreviewText));
            OnPropertyChanged(nameof(DirectResultText));
            OnPropertyChanged(nameof(DirectBadgeReadyText));
            OnPropertyChanged(nameof(DirectStatusReadyDetailText));
            OnPropertyChanged(nameof(DirectStatusNoToolText));
            OnPropertyChanged(nameof(DirectStatusPassedText));
            OnPropertyChanged(nameof(DirectElapsedSampleText));
            OnPropertyChanged(nameof(LayersResultsText));
            OnPropertyChanged(nameof(SelectedLayerDetailText));
            OnPropertyChanged(nameof(LayerResultMainText));
            OnPropertyChanged(nameof(LayerResultNoneText));
            OnPropertyChanged(nameof(LayerResultDisplayText));
            OnPropertyChanged(nameof(PipelineText));
            OnPropertyChanged(nameof(PipelineFilterPassedText));
            OnPropertyChanged(nameof(PipelineMorphologyPassedText));
            OnPropertyChanged(nameof(PipelineContourNeedsPreviewText));
            OnPropertyChanged(nameof(RunLogText));
            OnPropertyChanged(nameof(SelectedToolPreviewTitle));
            OnPropertyChanged(nameof(SelectedInputPreviewText));
            OnPropertyChanged(nameof(SelectedOutputPreviewText));
            OnPropertyChanged(nameof(SelectedDirectRunText));
            OnPropertyChanged(nameof(SelectedRouteText));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(ToolSearchHintText));
            OnPropertyChanged(nameof(ToolSearchClearToolTipText));
            OnPropertyChanged(nameof(ToolSearchLearnToolTipText));
            OnPropertyChanged(nameof(ToolSearchSamplesToolTipText));
            OnPropertyChanged(nameof(ToolSearchGuidedSetupToolTipText));
            OnPropertyChanged(nameof(ToolSearchSummaryText));
        }

        private static string T(string key, string fallback)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallback
                : value;
        }

        private static string TF(string key, string fallback, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, T(key, fallback), args);
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                ? korean
                : english;
        }

        private static string FormatPipelineStep(string index, string toolKey, string fallbackToolName, string status)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0} {1}  {2}",
                index,
                T(toolKey, fallbackToolName),
                status);
        }

        private static string ResolveLayerResultSize(string layerName)
        {
            if (string.Equals(layerName, "NewPanel_sbA", StringComparison.OrdinalIgnoreCase))
            {
                return "768x576";
            }

            if (string.Equals(layerName, "NewPanel_LPV", StringComparison.OrdinalIgnoreCase))
            {
                return "-";
            }

            return "512x512";
        }

    }

    public static class OpenVisionShellCommandCatalog
    {
        public static IReadOnlyList<OpenVisionShellNavGroup> CreateNavigationGroups()
        {
            return new[]
            {
                new OpenVisionShellNavGroup(
                    T("Menu.ImageProcessing", "IMAGE PROCESSING"),
                    new[]
                    {
                        WpfTool(VISION_MENU.Threshold, PackIconMaterialKind.ImageFilterBlackWhite),
                        WpfTool(VISION_MENU.Filter, PackIconMaterialKind.Filter),
                        WpfTool(VISION_MENU.Morphology, PackIconMaterialKind.Shape),
                        WpfTool(VISION_MENU.Arithmetic, PackIconMaterialKind.CalculatorVariant),
                        WpfTool(VISION_MENU.EdgeDetection, PackIconMaterialKind.ImageFilterHdr),
                        WpfTool(VISION_MENU.RotateAndScale, PackIconMaterialKind.Rotate3dVariant),
                        WpfTool(VISION_MENU.AffineTransform, PackIconMaterialKind.Rotate3dVariant),
                        WpfTool(VISION_MENU.Histogram, PackIconMaterialKind.ChartHistogram),
                        WpfTool(VISION_MENU.HSV, PackIconMaterialKind.Palette),
                        WpfTool(VISION_MENU.Mean, PackIconMaterialKind.FunctionVariant)
                    }),
                new OpenVisionShellNavGroup(
                    T("Menu.Algorithm", "ALGORITHM"),
                    new[]
                    {
                        WpfTool(VISION_MENU.Blob, PackIconMaterialKind.VectorCircle),
                        WpfTool(VISION_MENU.Contour, PackIconMaterialKind.VectorSquare),
                        WpfTool(VISION_MENU.Line, PackIconMaterialKind.VectorLine),
                        WpfTool(VISION_MENU.Matching, PackIconMaterialKind.CrosshairsGps),
                        WpfTool(VISION_MENU.EdgeBasedMatching, PackIconMaterialKind.CrosshairsGps),
                        WpfTool(VISION_MENU.FeatureMatching, PackIconMaterialKind.ImageSearch),
                        PipelineTool(VISION_MENU.Pipeline, PackIconMaterialKind.ChartTimelineVariant)
                    })
            };
        }

        private static OpenVisionShellNavItem WpfTool(
            VISION_MENU menu,
            PackIconMaterialKind iconKind,
            bool isActive = false)
        {
            return OpenVisionShellNavItem.Create(
                menu,
                DisplayName(menu),
                iconKind,
                LocalText("도구", "Tool"),
                StatusBrushes.Tool,
                LocalText("도구 화면 사용 중", "Tool view active"),
                isActive,
                requiresWorkspaceImage: true,
                requiresTemplate: menu == VISION_MENU.Matching
                    || menu == VISION_MENU.EdgeBasedMatching
                    || menu == VISION_MENU.FeatureMatching,
                searchTerms: SearchTerms(menu));
        }

        private static OpenVisionShellNavItem PendingAlgorithmTool(
            VISION_MENU menu,
            PackIconMaterialKind iconKind)
        {
            return OpenVisionShellNavItem.Create(
                menu,
                DisplayName(menu),
                iconKind,
                T("Shell.PendingTool.NavStatus", "Pending"),
                StatusBrushes.Pending,
                T("Shell.PendingTool.NavDescription", "Tool view is being prepared"),
                isActive: false,
                searchTerms: SearchTerms(menu));
        }

        private static OpenVisionShellNavItem PendingWpfTool(
            VISION_MENU menu,
            PackIconMaterialKind iconKind)
        {
            return OpenVisionShellNavItem.Create(
                menu,
                DisplayName(menu),
                iconKind,
                LocalText("속성", "PG"),
                StatusBrushes.Pending,
                LocalText("속성 편집기는 유지됨", "PropertyGrid editor is intentionally preserved"),
                isActive: false,
                searchTerms: SearchTerms(menu));
        }

        private static OpenVisionShellNavItem PipelineTool(
            VISION_MENU menu,
            PackIconMaterialKind iconKind)
        {
            return OpenVisionShellNavItem.Create(
                menu,
                DisplayName(menu),
                iconKind,
                LocalText("흐름", "Flow"),
                StatusBrushes.Pipeline,
                LocalText("파이프라인 작업 화면", "Pipeline workbench surface"),
                isActive: false,
                searchTerms: SearchTerms(menu));
        }

        private static string SearchTerms(VISION_MENU menu)
        {
            switch (menu)
            {
                case VISION_MENU.Threshold:
                    return "이진화 임계값 흑백 밝기 grayscale gv binary binarize threshold maxvalue binaryinv";
                case VISION_MENU.Filter:
                    return "필터 블러 가우시안 미디언 양방향 샤프닝 잡음 blur gaussian median bilateral sharpen noise kernel sigma";
                case VISION_MENU.Morphology:
                    return "모폴로지 침식 팽창 열기 닫기 morphology erode dilate open close kernel iterations";
                case VISION_MENU.Arithmetic:
                    return "산술 논리 연산 합성 차이 arithmetic add subtract multiply divide bitwise and or xor not inputlayerb";
                case VISION_MENU.EdgeDetection:
                    return "에지 경계선 캐니 미분 edge canny sobel scharr laplacian hough gradient";
                case VISION_MENU.RotateAndScale:
                    return "회전 크기 변환 기하학 rotate scale geometry angle scalex scaley";
                case VISION_MENU.AffineTransform:
                    return "어파인 행렬 3점 좌표 보정 기하 변환 affine matrix three point transform shear translation warp";
                case VISION_MENU.Histogram:
                    return "히스토그램 명암비 밝기 평활화 histogram contrast brightness equalize clahe normalize";
                case VISION_MENU.HSV:
                    return "색상 컬러 범위 마스크 hsv color mask hue saturation value huemin huemax";
                case VISION_MENU.Mean:
                    return "평균 밝기 표준편차 gv mean average brightness stddev meanvalueavg";
                case VISION_MENU.Blob:
                    return "블랍 레이블링 개수 면적 위치 결함 유무 blob labeling count area bounds presence defect resultcount areaavg boundswidth";
                case VISION_MENU.Contour:
                    return "외곽선 윤곽 형상 면적 둘레 contour outline shape perimeter area bounds resultcount areamax";
                case VISION_MENU.Line:
                    return "라인 핀 거리 교점 길이 간격 피치 측정 보정 line pin distance intersection gauge edge length gap pitch clearance measurement linedistance distancemmavg distancemmrange pixelpermm roi contrast";
                case VISION_MENU.Matching:
                    return "템플릿 매칭 패턴 위치 검출 점수 matching template pattern presence locate score scoremax resultcount";
                case VISION_MENU.EdgeBasedMatching:
                    return "에지 기반 매칭 윤곽 템플릿 점수 edge based matching outline shape template score canny";
                case VISION_MENU.FeatureMatching:
                    return "특징점 매칭 키포인트 기술자 호모그래피 feature sift keypoint descriptor homography matching ransac";
                case VISION_MENU.Pipeline:
                    return "파이프라인 흐름 레시피 스텝 레이어 리뷰 pipeline flow recipe step layer review result merge";
                default:
                    return string.Empty;
            }
        }

        private static string DisplayName(VISION_MENU menu)
        {
            return T("VisionMenu." + menu, menu.ToString());
        }

        private static string T(string key, string fallback)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallback
                : value;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                ? korean
                : english;
        }

        private static class StatusBrushes
        {
            public static readonly Brush Tool = new SolidColorBrush(Color.FromRgb(138, 215, 218));
            public static readonly Brush Pending = new SolidColorBrush(Color.FromRgb(244, 197, 66));
            public static readonly Brush Pipeline = new SolidColorBrush(Color.FromRgb(105, 214, 154));
        }
    }

    public sealed class OpenVisionShellNavGroup : ObservableObject
    {
        private bool isVisible = true;

        public OpenVisionShellNavGroup(string title, IReadOnlyList<OpenVisionShellNavItem> items)
        {
            Title = title ?? string.Empty;
            Items = items ?? Array.Empty<OpenVisionShellNavItem>();
        }

        public string Title { get; }
        public IReadOnlyList<OpenVisionShellNavItem> Items { get; }
        public bool IsVisible
        {
            get => isVisible;
            private set => SetProperty(ref isVisible, value);
        }

        public void SetVisible(bool value)
        {
            IsVisible = value;
        }
    }

    public sealed class OpenVisionShellNavItem : ObservableObject
    {
        private static readonly Brush ActiveTextBrush = Brushes.White;
        private static readonly Brush InactiveTextBrush = new SolidColorBrush(Color.FromRgb(215, 236, 238));
        private bool isActive;
        private string statusLabel;
        private Brush statusBrush;
        private string description;
        private string toolTip;
        private readonly string searchText;
        private bool isVisible = true;

        private OpenVisionShellNavItem(
            VISION_MENU menu,
            string title,
            PackIconMaterialKind iconKind,
            string statusLabel,
            Brush statusBrush,
            string description,
            bool isActive,
            bool requiresWorkspaceImage,
            bool requiresTemplate,
            string searchTerms)
        {
            Menu = menu;
            CommandId = menu.ToString();
            Title = title ?? string.Empty;
            IconKind = iconKind;
            this.statusLabel = statusLabel ?? string.Empty;
            this.statusBrush = statusBrush ?? Brushes.Transparent;
            this.description = description ?? string.Empty;
            toolTip = string.IsNullOrWhiteSpace(this.description)
                ? Title
                : string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} - {1}", Title, this.description);
            RequiresWorkspaceImage = requiresWorkspaceImage;
            RequiresTemplate = requiresTemplate;
            HasGuidedSetup = OpenVisionGuidedSetupCatalog.TryResolveTemplate(menu, out _);
            searchText = string.Join(" ", Title, CommandId, searchTerms ?? string.Empty);
            IsActive = isActive;
        }


        public VISION_MENU Menu { get; }
        public string CommandId { get; }
        public string Title { get; }
        public PackIconMaterialKind IconKind { get; }
        public string StatusLabel => statusLabel;
        public Brush StatusBrush => statusBrush;
        public string Description => description;
        public string ToolTip => toolTip;
        public bool RequiresWorkspaceImage { get; }
        public bool RequiresTemplate { get; }
        public bool HasGuidedSetup { get; }
        public bool IsVisible
        {
            get => isVisible;
            private set => SetProperty(ref isVisible, value);
        }
        public bool IsActive
        {
            get => isActive;
            private set
            {
                if (isActive == value) { return; }
                isActive = value;
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(TextBrush));
                OnPropertyChanged(nameof(FontWeight));
            }
        }

        public Brush TextBrush => IsActive ? ActiveTextBrush : InactiveTextBrush;
        public FontWeight FontWeight => IsActive ? FontWeights.SemiBold : FontWeights.Normal;

        public static OpenVisionShellNavItem Create(
            VISION_MENU menu,
            string title,
            PackIconMaterialKind iconKind,
            string statusLabel,
            Brush statusBrush,
            string description,
            bool isActive,
            bool requiresWorkspaceImage = false,
            bool requiresTemplate = false,
            string searchTerms = "")
        {
            return new OpenVisionShellNavItem(menu, title, iconKind, statusLabel, statusBrush, description, isActive, requiresWorkspaceImage, requiresTemplate, searchTerms);
        }

        public void SetActive(bool value)
        {
            IsActive = value;
        }

        public bool MatchesSearch(string query)
        {
            string[] tokens = (query ?? string.Empty).Split(
                new[] { ' ', '\t', '\r', '\n', ',', '/', '|' },
                StringSplitOptions.RemoveEmptyEntries);
            return tokens.All(token => searchText.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public void SetVisible(bool value)
        {
            IsVisible = value;
        }

        public void SetReadiness(string label, Brush brush, string readinessDescription)
        {
            statusLabel = label ?? string.Empty;
            statusBrush = brush ?? Brushes.Transparent;
            description = readinessDescription ?? string.Empty;
            toolTip = string.IsNullOrWhiteSpace(description)
                ? Title
                : string.Format(CultureInfo.CurrentCulture, "{0} - {1}", Title, description);
            OnPropertyChanged(nameof(StatusLabel));
            OnPropertyChanged(nameof(StatusBrush));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(ToolTip));
        }

    }
}
