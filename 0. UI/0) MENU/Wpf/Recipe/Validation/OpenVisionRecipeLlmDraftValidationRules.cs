using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using static OpenVisionLab.OpenVisionRecipeLlmIntent;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeLlmDraftValidationRules
    {
        internal static bool AppendResultChannelValidation(
            VisionPipeline pipeline,
            string xmlText,
            ICollection<string> validationLines)
        {
            List<VisionPipelineStep> enabledSteps = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .ToList();
            validationLines.Add(OpenVisionRecipeText.Local("판정 출력 채널: Inspection.Status / Inspection.FailedStep / Inspection.Evidence / Inspection.Benchmark / Inspection.NextAction", "Result channels: Inspection.Status / Inspection.FailedStep / Inspection.Evidence / Inspection.Benchmark / Inspection.NextAction"));

            if (enabledSteps.Count == 0)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.Status를 만들 수 없습니다. 사용 중인 Step이 없습니다.", "Error: Inspection.Status cannot be derived because there are no enabled steps."));
                return false;
            }

            bool hasOutputLayer = enabledSteps.Any(step => !string.IsNullOrWhiteSpace(step.OutputLayer));
            if (!hasOutputLayer)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.Evidence를 만들 수 없습니다. 사용 중인 Step의 OutputLayer가 없습니다.", "Error: Inspection.Evidence cannot be derived because enabled steps have no OutputLayer."));
                return false;
            }

            bool hasSeparateOutput = enabledSteps.Any(step =>
                !string.IsNullOrWhiteSpace(step.OutputLayer)
                && !string.Equals(step.InputLayer, step.OutputLayer, StringComparison.OrdinalIgnoreCase));
            if (!hasSeparateOutput)
            {
                validationLines.Add(OpenVisionRecipeText.Local("경고: 모든 출력이 입력과 같습니다. 입력 보존과 Evidence 추적을 위해 별도 OutputLayer를 권장합니다.", "Warning: all outputs match their inputs. Prefer separate OutputLayer values for input preservation and evidence tracing."));
            }

            bool hasGateParameter = enabledSteps.Any(HasJudgementParameter);
            validationLines.Add(hasGateParameter
                ? OpenVisionRecipeText.Local("Inspection.Evidence: OK - 명시적 판정 기준이 있습니다.", "Inspection.Evidence: OK - explicit judgement criteria are present.")
                : OpenVisionRecipeText.Local("경고: 판정 기준이 명확하지 않습니다. Acceptance metric/range 또는 SCORE_MIN, MIN/MAX, THRESHOLD, AREA, DISTANCE, MEAN 계열 값을 추가하세요.", "Warning: judgement criteria are not explicit. Add an acceptance metric/range or SCORE_MIN, MIN/MAX, THRESHOLD, AREA, DISTANCE, or MEAN style values."));

            if ((xmlText ?? string.Empty).IndexOf("Inspection.", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.* 이름은 XML 노드가 아니라 OpenVisionLab 리뷰 채널입니다. 사용자 정의 XML 노드나 파라미터를 제거하세요.", "Error: Inspection.* names are review channels, not XML nodes. Remove custom XML nodes or parameters."));
                return false;
            }

            validationLines.Add(OpenVisionRecipeText.Local("Inspection.Status: OK - XML 검증과 명시적 샘플/Good-Bad 실행 결과에서 파생됩니다.", "Inspection.Status: OK - derived from XML validation and explicit sample/Good-Bad runs."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.FailedStep: OK - Step 이름과 경로로 실패 위치를 추적할 수 있습니다.", "Inspection.FailedStep: OK - failures can be traced through step names and routes."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.Benchmark: WAIT - 가져오기 후 카탈로그/이력 비교 실행이 필요합니다.", "Inspection.Benchmark: WAIT - run catalog/history comparison after import."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.NextAction: OK - 검증 리포트와 작업자 리포트에 다음 조치가 표시됩니다.", "Inspection.NextAction: OK - validation and operator reports expose the next action."));
            return true;
        }

        internal static bool AppendIntentContractValidation(VisionPipeline pipeline, string template, ICollection<string> validationLines)
        {
            template = template ?? string.Empty;
            if (IsLineDistanceTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "LineDistance",
                    "Pin gap / edge distance",
                    "Use ToolType=LineDistance for pin-to-pin, edge-to-edge, gap, pitch, width, or clearance measurement. Do not substitute Contour or Blob.");
            }

            if (IsContourTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "Contour",
                    "Shape boundary",
                    "Use ToolType=Contour for boundary, chip, scratch, shape, or region outline checks.");
            }

            if (IsBlobTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "Blob",
                    "Threshold + Blob",
                    "Use ToolType=Blob after Threshold for connected-object count, area, position, or foreground presence checks.");
            }

            if (IsEdgeBasedTemplate(template))
            {
                return AppendEdgeBasedIntentContractValidation(pipeline, validationLines);
            }

            if (IsFeatureMatchingTemplate(template))
            {
                return AppendFeatureMatchingIntentContractValidation(pipeline, validationLines);
            }

            validationLines.Add("Intent contract: SKIP - selected intent has no strict tool-family gate.");
            return true;
        }

        private static bool AppendEdgeBasedIntentContractValidation(
            VisionPipeline pipeline,
            ICollection<string> validationLines)
        {
            bool toolReady = AppendRequiredLlmIntentToolValidation(
                pipeline,
                validationLines,
                "EdgeBasedMatching",
                "Edge Based Matching",
                "Use ToolType=EdgeBasedMatching for the selected Edge Based Matching intent.");
            VisionPipelineStep edgeStep = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .FirstOrDefault(step => step != null
                    && step.Enabled
                    && string.Equals(step.ToolType, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase));
            if (edgeStep == null)
            {
                return false;
            }

            bool hasScoreMinimum = edgeStep.Parameters != null
                && edgeStep.Parameters.Keys.Any(key => string.Equals(key, "SCORE_MIN", StringComparison.OrdinalIgnoreCase));
            bool hasSearchCount = edgeStep.Parameters != null
                && edgeStep.Parameters.Keys.Any(key => string.Equals(key, "NUM_MATCH", StringComparison.OrdinalIgnoreCase));
            bool hasCannyLow = edgeStep.Parameters != null
                && edgeStep.Parameters.Keys.Any(key => string.Equals(key, "CANNY_LOW", StringComparison.OrdinalIgnoreCase));
            bool hasCannyHigh = edgeStep.Parameters != null
                && edgeStep.Parameters.Keys.Any(key => string.Equals(key, "CANNY_HIGH", StringComparison.OrdinalIgnoreCase));
            bool hasFullImageScope = edgeStep.Parameters != null
                && edgeStep.Parameters.Any(pair => string.Equals(pair.Key, "USE_ROI", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(pair.Value, "false", StringComparison.OrdinalIgnoreCase));
            bool hasScoreMaxGate = edgeStep.UseAcceptance
                && edgeStep.UseAcceptanceMetricMinimum
                && string.Equals(edgeStep.AcceptanceMetricName, VisionPipelineKnownMetrics.ScoreMax, StringComparison.OrdinalIgnoreCase);

            if (hasScoreMinimum && hasSearchCount && hasCannyLow && hasCannyHigh && hasFullImageScope && hasScoreMaxGate)
            {
                validationLines.Add("Edge Based Matching contract: OK - score, search count, Canny, full-image scope, and ScoreMax minimum gate are present.");
                return toolReady;
            }

            validationLines.Add("Error: Edge Based Matching requires SCORE_MIN, NUM_MATCH, CANNY_LOW/HIGH, USE_ROI=false, and a ScoreMax minimum acceptance gate.");
            validationLines.Add("Next: keep ResultCount as review evidence; add the missing EdgeBasedMatching score, Canny, scope, or ScoreMax gate before importing.");
            return false;
        }

        private static bool AppendFeatureMatchingIntentContractValidation(
            VisionPipeline pipeline,
            ICollection<string> validationLines)
        {
            bool toolReady = AppendRequiredLlmIntentToolValidation(
                pipeline,
                validationLines,
                "FeatureMatching",
                "Feature Matching",
                "Use ToolType=FeatureMatching for the selected Feature Matching intent.");
            VisionPipelineStep featureStep = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .FirstOrDefault(step => step != null
                    && step.Enabled
                    && string.Equals(step.ToolType, "FeatureMatching", StringComparison.OrdinalIgnoreCase));
            if (featureStep == null)
            {
                return false;
            }

            bool hasRatioMinimum = featureStep.Parameters != null
                && featureStep.Parameters.Keys.Any(key => string.Equals(key, "SCORE_MIN", StringComparison.OrdinalIgnoreCase));
            bool hasRansacThreshold = featureStep.Parameters != null
                && featureStep.Parameters.Keys.Any(key => string.Equals(key, "RANSAC_REPROJ_THRESHOLD", StringComparison.OrdinalIgnoreCase));
            bool hasScoreMaxGate = featureStep.UseAcceptance
                && featureStep.UseAcceptanceMetricMinimum
                && string.Equals(featureStep.AcceptanceMetricName, VisionPipelineKnownMetrics.ScoreMax, StringComparison.OrdinalIgnoreCase);

            if (hasRatioMinimum && hasRansacThreshold && hasScoreMaxGate)
            {
                validationLines.Add("Feature Matching contract: OK - SCORE_MIN, RANSAC_REPROJ_THRESHOLD, and ScoreMax minimum gate are present.");
                return toolReady;
            }

            validationLines.Add("Error: Feature Matching requires SCORE_MIN, RANSAC_REPROJ_THRESHOLD, and a ScoreMax minimum acceptance gate.");
            validationLines.Add("Next: keep ResultCount as review evidence; add the missing FeatureMatching ratio, RANSAC, or ScoreMax gate before importing.");
            return false;
        }

        private static bool AppendRequiredLlmIntentToolValidation(
            VisionPipeline pipeline,
            ICollection<string> validationLines,
            string requiredToolType,
            string intentName,
            string nextAction)
        {
            List<string> enabledToolTypes = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .Select(step => step.ToolType ?? string.Empty)
                .Where(toolType => !string.IsNullOrWhiteSpace(toolType))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(toolType => toolType, StringComparer.OrdinalIgnoreCase)
                .ToList();

            bool hasRequiredTool = enabledToolTypes.Any(toolType =>
                string.Equals(toolType, requiredToolType, StringComparison.OrdinalIgnoreCase)
                || IsAcceptedToolAlias(requiredToolType, toolType));

            if (hasRequiredTool)
            {
                validationLines.Add("Intent contract: OK - " + intentName + " uses ToolType=" + requiredToolType + ".");
                return true;
            }

            validationLines.Add("Error: Intent contract mismatch. Selected intent '" + intentName + "' requires ToolType=" + requiredToolType + ".");
            validationLines.Add("Draft enabled ToolTypes: " + (enabledToolTypes.Count == 0 ? "-" : string.Join(", ", enabledToolTypes)));
            validationLines.Add("Next: " + nextAction);
            return false;
        }

        private static bool IsAcceptedToolAlias(string requiredToolType, string actualToolType)
        {
            if (string.Equals(requiredToolType, "LineDistance", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(actualToolType, "LineDistanceGauge", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool HasJudgementParameter(VisionPipelineStep step)
        {
            if (step == null)
            {
                return false;
            }

            if (step.UseAcceptance
                && !string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                && (step.UseAcceptanceMetricMinimum || step.UseAcceptanceMetricMaximum))
            {
                return true;
            }

            if (step.Parameters == null || step.Parameters.Count == 0)
            {
                return false;
            }

            return step.Parameters.Keys.Any(key =>
            {
                string value = key ?? string.Empty;
                return value.IndexOf("SCORE", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("THRESH", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MIN", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MAX", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("AREA", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("DISTANCE", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MEAN", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("RATIO", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("CONTRAST", StringComparison.OrdinalIgnoreCase) >= 0;
            });
        }

        internal static bool TryValidateXmlSyntax(string xmlText, ICollection<string> validationLines)
        {
            try
            {
                XDocument.Parse(xmlText, LoadOptions.SetLineInfo);
                validationLines.Add(OpenVisionRecipeText.Local("XML 구문: OK", "XML syntax: OK"));
                return true;
            }
            catch (XmlException ex)
            {
                validationLines.Clear();
                validationLines.Add(OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG"));
                validationLines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("XML 구문: NG, 줄 {0}, 위치 {1}: {2}", "XML syntax: NG at line {0}, position {1}: {2}"),
                    ex.LineNumber,
                    ex.LinePosition,
                    ex.Message));
                validationLines.Add(OpenVisionRecipeText.Local("다음: 보고된 줄/위치의 잘못된 XML을 수정한 뒤 다시 검증하세요.", "Next: Fix malformed XML at the reported line/position, then validate again."));
                return false;
            }
        }

    }
}
