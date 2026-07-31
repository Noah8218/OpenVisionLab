using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed class OpenVisionRecipeValidationSuiteScopeOption
    {
        public const string SelectedSampleKey = "SelectedSample";
        public const string LocalValidationSetKey = "LocalValidationSet";
        public const string GoodBadPairKey = "GoodBadPair";
        public const string CatalogKey = "Catalog";

        private OpenVisionRecipeValidationSuiteScopeOption(string key, string displayText, string detailText)
        {
            Key = key ?? string.Empty;
            DisplayText = displayText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
        }

        public string Key { get; }

        public string DisplayText { get; }

        public string DetailText { get; }

        public static IReadOnlyList<OpenVisionRecipeValidationSuiteScopeOption> CreateDefaults()
        {
            return new[]
            {
                new OpenVisionRecipeValidationSuiteScopeOption(
                    SelectedSampleKey,
                    OpenVisionRecipeText.Local("선택 샘플", "Selected sample"),
                    OpenVisionRecipeText.Local("현재 선택 샘플 1개를 실행하고 이력에 저장합니다.", "Run the selected sample and save it to history.")),
                new OpenVisionRecipeValidationSuiteScopeOption(
                    LocalValidationSetKey,
                    OpenVisionRecipeText.Local("로컬 세트", "Local set"),
                    OpenVisionRecipeText.Local("현재 레시피에 등록한 OK/NG 이미지 목록을 명시적으로 실행합니다.", "Explicitly run the recipe-local OK/NG image list.")),
                new OpenVisionRecipeValidationSuiteScopeOption(
                    GoodBadPairKey,
                    "Good/Bad",
                    OpenVisionRecipeText.Local("같은 PairGroup의 Good/Bad 샘플을 실행합니다.", "Run Good/Bad samples from the same PairGroup.")),
                new OpenVisionRecipeValidationSuiteScopeOption(
                    CatalogKey,
                    OpenVisionRecipeText.Local("카탈로그", "Catalog"),
                    OpenVisionRecipeText.Local("Product catalog 전체 benchmark를 실행합니다.", "Run the full Product catalog benchmark."))
            };
        }
    }

    public sealed class OpenVisionRecipeDependencyReviewRow
    {
        public OpenVisionRecipeDependencyReviewRow(
            string status,
            string stepName,
            string parameterName,
            string path,
            string action)
        {
            Status = string.IsNullOrWhiteSpace(status) ? "-" : status;
            StepName = string.IsNullOrWhiteSpace(stepName) ? "-" : stepName;
            ParameterName = string.IsNullOrWhiteSpace(parameterName) ? "-" : parameterName;
            Path = string.IsNullOrWhiteSpace(path) ? "-" : path;
            Action = string.IsNullOrWhiteSpace(action) ? "-" : action;
        }

        public string Status { get; }

        public string StepName { get; }

        public string ParameterName { get; }

        public string Path { get; }

        public string Action { get; }
    }

    public sealed class OpenVisionRecipeBranchOutputComparisonRow
    {
        public OpenVisionRecipeBranchOutputComparisonRow(
            string status,
            string stepName,
            string route,
            string action)
        {
            Status = string.IsNullOrWhiteSpace(status) ? "-" : status;
            StepName = string.IsNullOrWhiteSpace(stepName) ? "-" : stepName;
            Route = string.IsNullOrWhiteSpace(route) ? "-" : route;
            Action = string.IsNullOrWhiteSpace(action) ? "-" : action;
        }

        public string Status { get; }

        public string StepName { get; }

        public string Route { get; }

        public string Action { get; }
    }

    public sealed class OpenVisionRecipeManagerSummary
    {
        public static OpenVisionRecipeManagerSummary Empty { get; } = new OpenVisionRecipeManagerSummary(
            string.Empty,
            string.Empty,
            string.Empty,
            0,
            0,
            false,
            string.Empty,
            string.Empty,
            Array.Empty<OpenVisionRecipePipelineStepPreview>());

        public OpenVisionRecipeManagerSummary(
            string recipeName,
            string activePipelineName,
            string previewPipelineName,
            int pipelineCount,
            int stepCount,
            bool xmlValid,
            string detailText,
            string llmXmlValidationReport,
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> pipelinePreviewSteps)
        {
            RecipeName = recipeName ?? string.Empty;
            ActivePipelineName = activePipelineName ?? string.Empty;
            PreviewPipelineName = previewPipelineName ?? string.Empty;
            PipelineCount = pipelineCount;
            StepCount = stepCount;
            XmlValid = xmlValid;
            DetailText = detailText ?? string.Empty;
            LlmXmlValidationReport = llmXmlValidationReport ?? string.Empty;
            LlmXmlValidationIssues = OpenVisionRecipeValidationIssue.CreateRows(LlmXmlValidationReport, XmlValid);
            PipelinePreviewSteps = pipelinePreviewSteps ?? Array.Empty<OpenVisionRecipePipelineStepPreview>();
        }

        public string RecipeName { get; }

        public string ActivePipelineName { get; }

        public string PreviewPipelineName { get; }

        public int PipelineCount { get; }

        public int StepCount { get; }

        public bool XmlValid { get; }

        public string DetailText { get; }

        public string LlmXmlValidationReport { get; }

        public IReadOnlyList<OpenVisionRecipeValidationIssue> LlmXmlValidationIssues { get; }

        public IReadOnlyList<OpenVisionRecipePipelineStepPreview> PipelinePreviewSteps { get; }

        public string HeaderText =>
            OpenVisionRecipeText.Local("선택 레시피: ", "Selected recipe: ")
            + (string.IsNullOrWhiteSpace(RecipeName) ? "-" : RecipeName);

        public string ActivePipelineDisplay =>
            OpenVisionRecipeText.Local("활성: ", "Active: ")
            + (string.IsNullOrWhiteSpace(ActivePipelineName) ? "-" : ActivePipelineName);

        public string PipelineCountDisplay =>
            OpenVisionRecipeText.Local("파이프라인 ", "Pipelines ")
            + PipelineCount.ToString(CultureInfo.InvariantCulture);

        public string StepCountDisplay =>
            OpenVisionRecipeText.Local("단계 ", "Steps ")
            + StepCount.ToString(CultureInfo.InvariantCulture);

        public string XmlStatusDisplay => XmlValid ? "XML OK" : "XML NG";

        public string PipelinePreviewStepListDisplay =>
            OpenVisionRecipeText.Local("파이프라인 미리보기 단계 목록 (", "Pipeline preview step list (")
            + PipelinePreviewSteps.Count.ToString(CultureInfo.InvariantCulture)
            + ")";

        public string OperatorReviewText
        {
            get
            {
                if (!XmlValid)
                {
                    return OpenVisionRecipeText.Local("실행 전 XML 검토가 필요합니다.", "XML needs review before run.");
                }

                if (StepCount <= 0)
                {
                    return OpenVisionRecipeText.Local("검토 전에 파이프라인 단계를 추가하세요.", "Add a pipeline step before review.");
                }

                if (!string.Equals(ActivePipelineName, PreviewPipelineName, StringComparison.OrdinalIgnoreCase))
                {
                    return OpenVisionRecipeText.Local("이 파이프라인을 활성화하거나 활성 파이프라인을 선택하세요.", "Activate this pipeline or choose the active pipeline.");
                }

                return OpenVisionRecipeText.Local("검토 준비됨: 샘플 이미지로 실행한 뒤 출력 레이어를 확인하세요.", "Review ready: run with a sample image, then check output layers.");
            }
        }

        public string OperatorReviewChecklistText
        {
            get
            {
                if (!XmlValid)
                {
                    return string.Join(
                        Environment.NewLine,
                        OpenVisionRecipeText.Local("1. LLM XML 검증 보고서의 오류를 확인하세요.", "1. Review errors in the LLM XML validation report."),
                        OpenVisionRecipeText.Local("2. XML 경로, 레이어, ToolType, Parameters를 수정하세요.", "2. Fix XML paths, layers, ToolType, and Parameters."),
                        OpenVisionRecipeText.Local("3. XML OK 후 샘플 검사로 넘어가세요.", "3. Continue to sample check after XML OK."));
                }

                if (StepCount <= 0)
                {
                    return string.Join(
                        Environment.NewLine,
                        OpenVisionRecipeText.Local("1. 파이프라인 단계를 추가하거나 샘플에서 복제하세요.", "1. Add pipeline steps or duplicate from a sample."),
                        OpenVisionRecipeText.Local("2. 입력/출력 레이어 경로를 확인하세요.", "2. Check input/output layer routes."),
                        OpenVisionRecipeText.Local("3. 샘플 검사로 출력 레이어를 확인하세요.", "3. Run sample check to verify output layers."));
                }

                if (!string.Equals(ActivePipelineName, PreviewPipelineName, StringComparison.OrdinalIgnoreCase))
                {
                    return string.Join(
                        Environment.NewLine,
                        OpenVisionRecipeText.Local("1. 선택 파이프라인을 활성화하거나 활성 파이프라인을 선택하세요.", "1. Activate the selected pipeline or choose the active pipeline."),
                        OpenVisionRecipeText.Local("2. 활성 파이프라인 기준으로 샘플 검사를 실행하세요.", "2. Run sample check against the active pipeline."),
                        OpenVisionRecipeText.Local("3. 결과 레이어와 판정 기준을 비교하세요.", "3. Compare result layers and acceptance gates."));
                }

                return string.Join(
                    Environment.NewLine,
                    OpenVisionRecipeText.Local("1. 검사 실행으로 선택 샘플의 출력 레이어를 확인하세요.", "1. Run check to inspect selected sample output layers."),
                    OpenVisionRecipeText.Local("2. 쌍 검사로 Good/Bad 분리와 지표 기준을 확인하세요.", "2. Run pair check to verify Good/Bad separation and metric gates."),
                    OpenVisionRecipeText.Local("3. 실패 시 단계 목록에서 해당 출력/판정 기준을 조정하세요.", "3. On failure, tune the matching output or acceptance gate in the step list."));
            }
        }
    }

    public sealed class OpenVisionRecipeValidationIssue
    {
        private OpenVisionRecipeValidationIssue(
            string severity,
            string location,
            string explanation,
            string action)
        {
            Severity = severity ?? string.Empty;
            Location = location ?? string.Empty;
            Explanation = explanation ?? string.Empty;
            Action = action ?? string.Empty;
        }

        public string Severity { get; }

        public string Location { get; }

        public string Explanation { get; }

        public string Action { get; }

        internal static IReadOnlyList<OpenVisionRecipeValidationIssue> CreateRows(string report, bool xmlValid)
        {
            List<OpenVisionRecipeValidationIssue> rows = new List<OpenVisionRecipeValidationIssue>();
            foreach (string line in SplitLines(report))
            {
                string trimmed = line.Trim();
                if (TryStripPrefix(trimmed, "오류: ", "Error: ", out string error))
                {
                    rows.Add(new OpenVisionRecipeValidationIssue(
                        OpenVisionRecipeText.Local("오류", "Error"),
                        OpenVisionRecipeText.Local("XML/경로", "XML/route"),
                        error,
                        OpenVisionRecipeText.Local("ToolType, Layer, Parameter를 수정한 뒤 다시 검증하세요.", "Fix ToolType, Layer, and Parameters, then validate again.")));
                    continue;
                }

                if (TryStripPrefix(trimmed, "경고: ", "Warning: ", out string warning))
                {
                    rows.Add(new OpenVisionRecipeValidationIssue(
                        OpenVisionRecipeText.Local("경고", "Warning"),
                        OpenVisionRecipeText.Local("XML/레시피", "XML/recipe"),
                        warning,
                        OpenVisionRecipeText.Local("이름, 분기, 의존 경로가 의도와 맞는지 확인하세요.", "Check that names, branches, and dependency paths match the intent.")));
                    continue;
                }

                if (trimmed.IndexOf("XML load:", StringComparison.OrdinalIgnoreCase) >= 0
                    && trimmed.IndexOf("OK", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    rows.Add(new OpenVisionRecipeValidationIssue(
                        OpenVisionRecipeText.Local("오류", "Error"),
                        OpenVisionRecipeText.Local("XML 로드", "XML load"),
                        trimmed,
                        OpenVisionRecipeText.Local("VisionPipeline XML 형식으로 다시 생성하거나 가져오세요.", "Regenerate or import a valid VisionPipeline XML.")));
                }
            }

            if (rows.Count == 0)
            {
                rows.Add(new OpenVisionRecipeValidationIssue(
                    xmlValid ? "OK" : OpenVisionRecipeText.Local("검토", "Review"),
                    OpenVisionRecipeText.Local("검증 요약", "Validation summary"),
                    xmlValid
                        ? OpenVisionRecipeText.Local("LLM XML 구조와 경로 검증이 통과했습니다.", "LLM XML structure and route validation passed.")
                        : OpenVisionRecipeText.Local("검증 리포트를 확인해야 합니다.", "Review the validation report."),
                    xmlValid
                        ? OpenVisionRecipeText.Local("샘플 검사 또는 Pipeline Review로 진행하세요.", "Continue to sample check or Pipeline Review.")
                        : OpenVisionRecipeText.Local("오류/경고 라인을 확인한 뒤 다시 검증하세요.", "Review error/warning lines, then validate again.")));
            }

            return rows.Take(6).ToArray();
        }

        private static IEnumerable<string> SplitLines(string report)
        {
            return (report ?? string.Empty)
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool TryStripPrefix(string value, string koreanPrefix, string englishPrefix, out string stripped)
        {
            stripped = string.Empty;
            if (value.StartsWith(koreanPrefix, StringComparison.Ordinal))
            {
                stripped = value.Substring(koreanPrefix.Length).Trim();
                return true;
            }

            if (value.StartsWith(englishPrefix, StringComparison.OrdinalIgnoreCase))
            {
                stripped = value.Substring(englishPrefix.Length).Trim();
                return true;
            }

            return false;
        }
    }

    public sealed class OpenVisionRecipePipelineOption
    {
        private OpenVisionRecipePipelineOption(
            string pipelineName,
            bool isActive,
            int stepCount,
            bool xmlValid,
            bool routeValid,
            string statusText,
            VisionPipelinePersistenceState persistenceState)
        {
            PipelineName = pipelineName ?? string.Empty;
            IsActive = isActive;
            StepCount = stepCount;
            XmlValid = xmlValid;
            RouteValid = routeValid;
            StatusText = statusText ?? string.Empty;
            HasPersistenceStatus = persistenceState != null;
            HasPersistenceFailure =
                persistenceState?.IsFailure == true;
            PersistenceStatusText =
                OpenVisionRecipePersistenceStatusPresenter
                    .CreateCompactText(persistenceState);
            PersistenceHelpText =
                OpenVisionRecipePersistenceStatusPresenter
                    .CreateHelpText(persistenceState);
        }

        public string PipelineName { get; }

        public bool IsActive { get; }

        public int StepCount { get; }

        public bool XmlValid { get; }

        public bool RouteValid { get; }

        public string StatusText { get; }

        public bool HasPersistenceStatus { get; }

        public bool HasPersistenceFailure { get; }

        public string PersistenceStatusText { get; }

        public string PersistenceHelpText { get; }

        public string DisplayText =>
            (IsActive ? OpenVisionRecipeText.Local("[활성] ", "[ACTIVE] ") : string.Empty)
            + PipelineName;

        public string DetailText =>
            StepCount.ToString(CultureInfo.InvariantCulture)
            + OpenVisionRecipeText.Local(" 단계 | ", " step(s) | ")
            + StatusText;

        internal static OpenVisionRecipePipelineOption Create(
            string recipeName,
            string pipelineName,
            string activePipelineName)
        {
            string name = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName.Trim();
            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, name);
            bool isActive = string.Equals(name, activePipelineName, StringComparison.OrdinalIgnoreCase);
            VisionPipelineStorage.TryGetPersistenceState(
                recipeName,
                name,
                out VisionPipelinePersistenceState persistenceState);
            if (!VisionPipelineStorage.TryLoadFromFile(path, out VisionPipeline pipeline, out string message))
            {
                string loadStatus = persistenceState?.IsFailure == true
                    ? OpenVisionRecipeText.Local(
                        "저장 복원 검토 필요",
                        "Storage restoration review required")
                    : "XML NG - " + message;
                return new OpenVisionRecipePipelineOption(
                    name,
                    isActive,
                    0,
                    false,
                    false,
                    loadStatus,
                    persistenceState);
            }

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
            if (persistenceState?.IsFailure == true)
            {
                return new OpenVisionRecipePipelineOption(
                    name,
                    isActive,
                    pipeline?.Steps?.Count ?? 0,
                    false,
                    false,
                    OpenVisionRecipeText.Local(
                        "저장 복원 검토 필요",
                        "Storage restoration review required"),
                    persistenceState);
            }

            string status = validation.Success
                ? OpenVisionRecipeText.Local("XML OK / 경로 OK", "XML OK / Route OK")
                : OpenVisionRecipeText.Local("XML OK / 경로 NG ", "XML OK / Route NG ") + validation.Errors.Count.ToString(CultureInfo.InvariantCulture);
            return new OpenVisionRecipePipelineOption(
                name,
                isActive,
                pipeline?.Steps?.Count ?? 0,
                true,
                validation.Success,
                status,
                persistenceState);
        }
    }

    public sealed class OpenVisionRecipeLayerCard
    {
        public OpenVisionRecipeLayerCard(string layerName, string statusText, BitmapImage thumbnail, bool canNavigate)
        {
            LayerName = string.IsNullOrWhiteSpace(layerName) ? "-" : layerName.Trim();
            StatusText = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText.Trim();
            Thumbnail = thumbnail;
            CanNavigate = canNavigate;
        }

        public string LayerName { get; }

        public string StatusText { get; }

        public BitmapImage Thumbnail { get; }

        public bool CanNavigate { get; }

        public bool HasThumbnail => Thumbnail != null;

        public static OpenVisionRecipeLayerCard CreateMissing(string layerName)
        {
            return new OpenVisionRecipeLayerCard(
                layerName,
                OpenVisionRecipeText.Local("레이어 없음", "Layer missing"),
                null,
                false);
        }
    }

    public sealed class OpenVisionRecipePipelineStepPreview
    {
        internal OpenVisionRecipePipelineStepPreview(
            int index,
            VisionPipelineStep step,
            Func<string, OpenVisionRecipeLayerCard> layerCardProvider)
        {
            Index = index;
            Name = string.IsNullOrWhiteSpace(step?.Name) ? "Step " + index.ToString(CultureInfo.InvariantCulture) : step.Name.Trim();
            ToolType = string.IsNullOrWhiteSpace(step?.ToolType) ? "-" : step.ToolType.Trim();
            InputLayer = string.IsNullOrWhiteSpace(step?.InputLayer) ? "-" : step.InputLayer.Trim();
            OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer) ? "-" : step.OutputLayer.Trim();
            SourceLayers = ResolveListParameter(step, "SourceLayers");
            SourceSteps = ResolveListParameter(step, "SourceSteps");
            InputLayerCard = CreateLayerCard(layerCardProvider, InputLayer);
            OutputLayerCard = CreateLayerCard(layerCardProvider, OutputLayer);
            ParameterCount = step?.Parameters?.Count ?? 0;
            IsEnabled = step?.Enabled ?? false;
            AcceptanceText = ResolveAcceptanceText(step);
            RouteText = OpenVisionRecipeText.Local("레이어: ", "Layers: ") + Shorten(InputLayer, 34) + " -> " + Shorten(OutputLayer, 34);
            LayerRouteText = InputLayer + " -> " + OutputLayer;
            TableRouteText = Shorten(InputLayer, 8) + " -> " + Shorten(OutputLayer, 12);
            AcceptanceDetailText = ResolveAcceptanceDetailText(step);
            TableAcceptanceText = ResolveTableAcceptanceText(step);
            ParameterPreviewText = BuildParameterPreviewText(step);
            FullParameterText = BuildFullParameterText(step);
            RoiMetadataText = BuildRoiMetadataText(step);
            TemplateMetadataText = BuildTemplateMetadataText(step);
            EditorActionText = BuildEditorActionText(ToolType);
        }

        public int Index { get; }

        public string Name { get; }

        public string ToolType { get; }

        public string InputLayer { get; }

        public string OutputLayer { get; }

        public IReadOnlyList<string> SourceLayers { get; }

        public IReadOnlyList<string> SourceSteps { get; }

        public OpenVisionRecipeLayerCard InputLayerCard { get; }

        public OpenVisionRecipeLayerCard OutputLayerCard { get; }

        public int ParameterCount { get; }

        public bool IsEnabled { get; }

        public string AcceptanceText { get; }

        public string RouteText { get; }

        public string LayerRouteText { get; }

        public string TableRouteText { get; }

        public string AcceptanceDetailText { get; }

        public string TableAcceptanceText { get; }

        public string ParameterPreviewText { get; }

        public string FullParameterText { get; }

        public string RoiMetadataText { get; }

        public string TemplateMetadataText { get; }

        public string EditorActionText { get; }

        public string DisplayText =>
            Index.ToString(CultureInfo.InvariantCulture) + ". "
            + (IsEnabled ? OpenVisionRecipeText.Local("[사용] ", "[ON] ") : OpenVisionRecipeText.Local("[중지] ", "[OFF] "))
            + Shorten(Name, 42)
            + " / "
            + ToolType;

        public string DetailText =>
            Shorten(InputLayer, 32)
            + " -> "
            + Shorten(OutputLayer, 32)
            + OpenVisionRecipeText.Local(" | 파라미터 ", " | Params ")
            + ParameterCount.ToString(CultureInfo.InvariantCulture)
            + AcceptanceText;

        public string FullDetailText =>
            InputLayer
            + " -> "
            + OutputLayer
            + OpenVisionRecipeText.Local(" | 파라미터 ", " | Params ")
            + ParameterCount.ToString(CultureInfo.InvariantCulture)
            + AcceptanceText;

        private static OpenVisionRecipeLayerCard CreateLayerCard(
            Func<string, OpenVisionRecipeLayerCard> layerCardProvider,
            string layerName)
        {
            if (layerCardProvider == null)
            {
                return OpenVisionRecipeLayerCard.CreateMissing(layerName);
            }

            return layerCardProvider(layerName) ?? OpenVisionRecipeLayerCard.CreateMissing(layerName);
        }

        private static IReadOnlyList<string> ResolveListParameter(VisionPipelineStep step, string key)
        {
            if (step?.Parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return Array.Empty<string>();
            }

            string value = step.Parameters
                .Where(item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                .Select(item => item.Value)
                .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<string>();
            }

            return value
                .Split(new[] { ';', ',', '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string Shorten(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
            {
                return string.IsNullOrWhiteSpace(value) ? "-" : value;
            }

            return value.Substring(0, Math.Max(1, maxLength - 3)) + "...";
        }

        private static string ResolveAcceptanceText(VisionPipelineStep step)
        {
            if (step == null || !step.UseAcceptance)
            {
                return string.Empty;
            }

            string metric = string.IsNullOrWhiteSpace(step.AcceptanceMetricName) ? "result" : step.AcceptanceMetricName.Trim();
            List<string> gates = new List<string>();
            if (step.UseAcceptanceMetricMinimum)
            {
                gates.Add(">=" + step.AcceptanceMetricMinimum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                gates.Add("<=" + step.AcceptanceMetricMaximum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return gates.Count == 0
                ? OpenVisionRecipeText.Local(" | 판정 ", " | Accept ") + metric
                : OpenVisionRecipeText.Local(" | 판정 ", " | Accept ") + metric + " " + string.Join(" ", gates);
        }

        private static string ResolveTableAcceptanceText(VisionPipelineStep step)
        {
            if (step == null || !step.UseAcceptance)
            {
                return "-";
            }

            string metric = string.IsNullOrWhiteSpace(step.AcceptanceMetricName) ? "result" : step.AcceptanceMetricName.Trim();
            List<string> gates = new List<string>();
            if (step.UseAcceptanceMetricMinimum)
            {
                gates.Add(">=" + step.AcceptanceMetricMinimum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                gates.Add("<=" + step.AcceptanceMetricMaximum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return gates.Count == 0
                ? Shorten(metric, 12)
                : Shorten(metric, 12) + Environment.NewLine + string.Join(" ", gates);
        }

        private static string ResolveAcceptanceDetailText(VisionPipelineStep step)
        {
            if (step == null || !step.UseAcceptance)
            {
                return OpenVisionRecipeText.Local("판정 기준 없음", "No acceptance gate");
            }

            string metric = string.IsNullOrWhiteSpace(step.AcceptanceMetricName) ? "result" : step.AcceptanceMetricName.Trim();
            List<string> gates = new List<string>();
            if (step.UseAcceptanceMetricMinimum)
            {
                gates.Add(">=" + step.AcceptanceMetricMinimum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                gates.Add("<=" + step.AcceptanceMetricMaximum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return gates.Count == 0
                ? metric
                : metric + Environment.NewLine + string.Join(" ", gates);
        }

        private static string BuildParameterPreviewText(VisionPipelineStep step)
        {
            if (step?.Parameters == null || step.Parameters.Count == 0)
            {
                return OpenVisionRecipeText.Local("파라미터: 없음", "Params: none");
            }

            List<string> pairs = step.Parameters
                .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                .Take(4)
                .Select(parameter => parameter.Key + "=" + Shorten(parameter.Value, 18))
                .ToList();
            int remaining = Math.Max(0, step.Parameters.Count - pairs.Count);
            string suffix = remaining > 0 ? " +" + remaining.ToString(CultureInfo.InvariantCulture) : string.Empty;
            return OpenVisionRecipeText.Local("파라미터: ", "Params: ") + string.Join(", ", pairs) + suffix;
        }

        private static string BuildFullParameterText(VisionPipelineStep step)
        {
            if (step?.Parameters == null || step.Parameters.Count == 0)
            {
                return OpenVisionRecipeText.Local("파라미터 없음", "No parameters");
            }

            return string.Join(
                Environment.NewLine,
                step.Parameters
                    .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(parameter => parameter.Key + " = " + (parameter.Value ?? string.Empty)));
        }

        private static string BuildRoiMetadataText(VisionPipelineStep step)
        {
            IDictionary<string, string> parameters = step?.Parameters;
            if (parameters == null || parameters.Count == 0)
            {
                return OpenVisionRecipeText.Local("ROI: 파라미터 없음", "ROI: no parameters");
            }

            bool useRoi = GetBoolParameter(parameters, "USE_ROI");
            bool useMultiRoi = GetBoolParameter(parameters, "USE_MULTI_ROI");
            string roi = GetParameter(parameters, useMultiRoi ? "CvROIS" : "CvROI");
            if (string.IsNullOrWhiteSpace(roi))
            {
                roi = string.Join(
                    " | ",
                    parameters
                        .Where(parameter => parameter.Key?.IndexOf("ROI", StringComparison.OrdinalIgnoreCase) >= 0)
                        .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Key)
                            && !parameter.Key.StartsWith("USE_", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                        .Take(3)
                        .Select(parameter => parameter.Key + "=" + parameter.Value));
            }

            if (string.IsNullOrWhiteSpace(roi))
            {
                return useRoi
                    ? OpenVisionRecipeText.Local("ROI: 켜짐, 영역 값 없음", "ROI: enabled, no region value")
                    : OpenVisionRecipeText.Local("ROI: 전체 이미지", "ROI: full image");
            }

            string prefix = useMultiRoi
                ? OpenVisionRecipeText.Local("ROI: 다중 ", "ROI: multi ")
                : OpenVisionRecipeText.Local("ROI: ", "ROI: ");
            return prefix + Shorten(roi, 72);
        }

        private static string BuildTemplateMetadataText(VisionPipelineStep step)
        {
            IDictionary<string, string> parameters = step?.Parameters;
            if (parameters == null || parameters.Count == 0)
            {
                return OpenVisionRecipeText.Local("Template: 파라미터 없음", "Template: no parameters");
            }

            string templatePath = GetFirstParameter(parameters, "TemplatePath", "PATTERN_PATH", "PatternPath");
            if (string.IsNullOrWhiteSpace(templatePath))
            {
                return OpenVisionRecipeText.Local("Template: 없음", "Template: none");
            }

            List<string> parts = new List<string>
            {
                "Template: " + Shorten(Path.GetFileName(templatePath.Trim()), 42)
            };
            string score = GetParameter(parameters, "SCORE_MIN");
            if (!string.IsNullOrWhiteSpace(score))
            {
                parts.Add("score >= " + score.Trim());
            }

            string count = GetParameter(parameters, "NUM_MATCH");
            if (!string.IsNullOrWhiteSpace(count))
            {
                parts.Add("count " + count.Trim());
            }

            return string.Join(" | ", parts);
        }

        private static string BuildEditorActionText(string toolType)
        {
            string name = string.IsNullOrWhiteSpace(toolType) ? "Tool" : toolType.Trim();
            return OpenVisionRecipeText.Local("도구 열기: ", "Open tool: ") + name;
        }

        private static bool GetBoolParameter(IDictionary<string, string> parameters, string key)
        {
            string value = GetParameter(parameters, key);
            return bool.TryParse(value, out bool parsed) && parsed;
        }

        private static string GetFirstParameter(IDictionary<string, string> parameters, params string[] keys)
        {
            foreach (string key in keys ?? Array.Empty<string>())
            {
                string value = GetParameter(parameters, key);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private static string GetParameter(IDictionary<string, string> parameters, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            return parameters.TryGetValue(key, out string value) ? value ?? string.Empty : string.Empty;
        }
    }

    public sealed class OpenVisionRecipeOperatorValidationRow
    {
        private OpenVisionRecipeOperatorValidationRow(
            string itemText,
            string stateText,
            string evidenceText,
            string nextActionText)
        {
            ItemText = string.IsNullOrWhiteSpace(itemText) ? "-" : itemText.Trim();
            StateText = string.IsNullOrWhiteSpace(stateText) ? "WAIT" : stateText.Trim().ToUpperInvariant();
            EvidenceText = string.IsNullOrWhiteSpace(evidenceText) ? "-" : evidenceText.Trim();
            NextActionText = string.IsNullOrWhiteSpace(nextActionText) ? "-" : nextActionText.Trim();
        }

        public string ItemText { get; }

        public string StateText { get; }

        public string EvidenceText { get; }

        public string NextActionText { get; }

        public bool IsOk => string.Equals(StateText, "OK", StringComparison.OrdinalIgnoreCase);

        public bool IsNg => string.Equals(StateText, "NG", StringComparison.OrdinalIgnoreCase);

        public bool IsWait => !IsOk && !IsNg;

        public string DisplayText => ItemText + " | " + StateText + " | " + EvidenceText;

        public static OpenVisionRecipeOperatorValidationRow Create(
            string itemText,
            string stateText,
            string evidenceText,
            string nextActionText)
        {
            return new OpenVisionRecipeOperatorValidationRow(itemText, stateText, evidenceText, nextActionText);
        }
    }

    public sealed class OpenVisionRecipeOperatorResultChannelRow
    {
        private OpenVisionRecipeOperatorResultChannelRow(
            string channelText,
            string valueText,
            string sourceText,
            string useText)
        {
            ChannelText = string.IsNullOrWhiteSpace(channelText) ? "-" : channelText.Trim();
            ValueText = string.IsNullOrWhiteSpace(valueText) ? "-" : valueText.Trim();
            SourceText = string.IsNullOrWhiteSpace(sourceText) ? "-" : sourceText.Trim();
            UseText = string.IsNullOrWhiteSpace(useText) ? "-" : useText.Trim();
        }

        public string ChannelText { get; }

        public string ValueText { get; }

        public string SourceText { get; }

        public string UseText { get; }

        public bool IsOk => string.Equals(ValueText, "OK", StringComparison.OrdinalIgnoreCase);

        public bool IsNg => string.Equals(ValueText, "NG", StringComparison.OrdinalIgnoreCase);

        public bool IsWait => string.Equals(ValueText, "WAIT", StringComparison.OrdinalIgnoreCase);

        public string DisplayText => ChannelText + " | " + ValueText + " | " + SourceText;

        public static OpenVisionRecipeOperatorResultChannelRow Create(
            string channelText,
            string valueText,
            string sourceText,
            string useText)
        {
            return new OpenVisionRecipeOperatorResultChannelRow(channelText, valueText, sourceText, useText);
        }
    }

}
