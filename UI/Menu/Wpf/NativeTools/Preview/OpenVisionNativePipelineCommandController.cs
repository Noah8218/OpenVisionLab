using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativePipelineCommandController
    {
        private readonly string toolName;
        private readonly Func<string> resolveInputLayer;
        private readonly Func<string> resolveOutputLayer;
        private readonly Func<string, string, VisionPipelineStep> createSingleInputStep;
        private readonly Func<VisionPipelineStep> createArithmeticStep;
        private readonly Action<string> setStatus;
        private readonly Func<OpenVisionRecipeContext> recipeContextProvider;

        public OpenVisionNativePipelineCommandController(
            string toolName,
            Func<string> resolveInputLayer,
            Func<string> resolveOutputLayer,
            Func<string, string, VisionPipelineStep> createSingleInputStep,
            Func<VisionPipelineStep> createArithmeticStep,
            Action<string> setStatus,
            Func<OpenVisionRecipeContext> recipeContextProvider = null)
        {
            this.toolName = string.IsNullOrWhiteSpace(toolName) ? "Tool" : toolName;
            this.resolveInputLayer = resolveInputLayer;
            this.resolveOutputLayer = resolveOutputLayer;
            this.createSingleInputStep = createSingleInputStep;
            this.createArithmeticStep = createArithmeticStep;
            this.setStatus = setStatus ?? throw new ArgumentNullException(nameof(setStatus));
            this.recipeContextProvider = recipeContextProvider ?? (() => null);
        }

        public VisionPipelineStep AddSingleInputStep()
        {
            if (createSingleInputStep == null)
            {
                return ReportUnavailable();
            }

            string inputLayer = resolveInputLayer?.Invoke();
            string outputLayer = resolveOutputLayer?.Invoke();
            return AddCreatedStep(() => createSingleInputStep(inputLayer, outputLayer));
        }

        public VisionPipelineStep AddArithmeticStep()
        {
            if (createArithmeticStep == null)
            {
                return ReportUnavailable();
            }

            return AddCreatedStep(createArithmeticStep);
        }

        private VisionPipelineStep AddCreatedStep(Func<VisionPipelineStep> createStep)
        {
            try
            {
                // Pipeline creation is centralized so tool views do not duplicate append/status behavior.
                VisionPipelineStep step = createStep();
                if (step == null)
                {
                    return ReportUnavailable();
                }

                OpenVisionRecipeContext recipeContext = recipeContextProvider();
                VisionPipelineStep addedStep =
                    VisionPipelineAppendService.AddStep(step, recipeContext);
                string savedContext = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T(
                        "VisionTool.Status.PipelineSavedContextFormat"),
                    addedStep.Name,
                    recipeContext.Name,
                    recipeContext.PipelineName);
                setStatus(string.Format(
                    CultureInfo.CurrentCulture,
                    "Pipeline added / {0}",
                    savedContext));
                return addedStep;
            }
            catch (Exception ex)
            {
                setStatus("Pipeline add NG / " + ex.GetBaseException().Message);
                return null;
            }
        }

        private VisionPipelineStep ReportUnavailable()
        {
            setStatus("Pipeline add unavailable / " + toolName);
            return null;
        }
    }
}
