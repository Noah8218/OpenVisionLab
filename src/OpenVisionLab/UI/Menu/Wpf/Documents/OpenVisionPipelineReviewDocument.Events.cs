using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using OpenVisionLab.Pipeline.Controls;
using OpenVisionLab.Vision2D.Pipeline;

namespace OpenVisionLab
{
    internal sealed partial class OpenVisionPipelineReviewDocument
    {
        private void OnStepSelected(object sender, PipelineFlowStepSelectedEventArgs e)
        {
            SelectStep(e.Index, e.Mode);
        }

        private async void OnRunReviewRequested(object sender, EventArgs e)
        {
            await RunReviewAsync();
        }

        private void InvokeOnViewDispatcher(Action action)
        {
            if (action == null)
            {
                return;
            }

            if (view.Dispatcher.CheckAccess())
            {
                action();
                return;
            }

            view.Dispatcher.Invoke(action);
        }

        private void OnPreviousStepRequested(object sender, EventArgs e)
        {
            if (selectedIndex > 0)
            {
                SelectStep(selectedIndex - 1, selectedMode);
            }
        }

        private void OnNextStepRequested(object sender, EventArgs e)
        {
            int stepCount = pipeline?.Steps?.Count ?? 0;
            if (selectedIndex >= 0 && selectedIndex < stepCount - 1)
            {
                SelectStep(selectedIndex + 1, selectedMode);
            }
        }

        private void OnFirstIssueStepRequested(object sender, EventArgs e)
        {
            SelectFirstIssueStep();
        }

        private void SelectFirstIssueStep()
        {
            int issueIndex = FindFirstIssueStepIndex();
            if (issueIndex >= 0)
            {
                SelectStep(issueIndex, selectedMode);
            }
        }

        private void OnOpenPairSampleRequested(object sender, EventArgs e)
        {
            RequestOpenPairSample();
        }

        private void OnUseSelectedMatchingPoseRequested(object sender, EventArgs e)
        {
            SaveSelectedMatchingPoseAsReference();
        }

        private void OnReturnToRecipeRequested(object sender, EventArgs e)
        {
            ReturnToRecipeRequested(this, EventArgs.Empty);
        }

        private void OnOpenSelectedToolLearnRequested(object sender, EventArgs e)
        {
            if (OpenVisionLearnTopicCatalog.TryResolveForToolType(SelectedToolType, out _))
            {
                OpenSelectedToolLearnRequested(this, EventArgs.Empty);
            }
        }

        private void OnEditSelectedStepRequested(object sender, EventArgs e)
        {
            EditSelectedStepRequested(this, EventArgs.Empty);
        }

        private void OnEditFixtureProducerRequested(object sender, EventArgs e)
        {
            RequestStepEdit(fixtureProducerIndex);
        }

        private void OnEditFixtureMeasurementRequested(object sender, EventArgs e)
        {
            RequestStepEdit(fixtureMeasurementIndex);
        }

        private void OnFixtureConsumerSelected(
            object sender,
            OpenVisionPipelineReviewFixtureConsumerSelectedEventArgs e)
        {
            if (e == null
                || e.StepIndex == fixtureMeasurementIndex
                || pipeline?.Steps?.ElementAtOrDefault(e.StepIndex) == null)
            {
                return;
            }

            fixtureMeasurementIndex = e.StepIndex;
            UpdateFixtureDesignerState();
        }

        private void OnScaleCalibrationRequested(object sender, VisionScaleCalibrationRequestedEventArgs e)
        {
            IReadOnlyList<VisionPipelineGeometryFeatureResult> points = executionController
                .GetCurrentGeometryFeatures()
                .Where(item => item.Kind == VisionPipelineGeometryKind.Point)
                .ToList();
            VisionPipelineGeometryFeatureResult pointA = points.FirstOrDefault(item =>
                string.Equals(item.Identity, e?.PointAIdentity, StringComparison.OrdinalIgnoreCase));
            VisionPipelineGeometryFeatureResult pointB = points.FirstOrDefault(item =>
                string.Equals(item.Identity, e?.PointBIdentity, StringComparison.OrdinalIgnoreCase));
            Bitmap coordinateImage = ResolveLayerPreviewImage(pointA?.CoordinateLayer);

            if (!VisionPipelineScaleCalibrationStorage.TryCalculate(
                    activePipelineName,
                    pointA,
                    pointB,
                    e?.KnownDistance ?? 0D,
                    e?.Unit ?? VisionScaleCalibrationUnit.Millimeter,
                    coordinateImage,
                    out VisionPipelineScaleCalibrationRecord record,
                    out string error)
                || !VisionPipelineScaleCalibrationStorage.TrySave(
                    recipeContext.Name,
                    record,
                    out string evidencePath,
                    out error))
            {
                view.SetScaleCalibrationStatus("Scale evidence was not saved: " + error);
                return;
            }

            UpdateScaleCalibrationState(
                "Saved exact two-point evidence: " + evidencePath + ". Apply remains explicit; no Preview/Run occurred.");
        }

        private void OnScaleCalibrationApplyRequested(object sender, VisionScaleCalibrationApplyRequestedEventArgs e)
        {
            if (pipeline?.Steps == null || e == null || e.StepIndex < 0 || e.StepIndex >= pipeline.Steps.Count)
            {
                view.SetScaleCalibrationStatus("Select one compatible target Step.");
                return;
            }

            if (!VisionPipelineScaleCalibrationStorage.TryLoad(
                    recipeContext.Name,
                    activePipelineName,
                    out VisionPipelineScaleCalibrationRecord record,
                    out string error))
            {
                view.SetScaleCalibrationStatus("Scale was not applied: " + error);
                return;
            }

            VisionPipelineStep target = pipeline.Steps[e.StepIndex];
            Bitmap coordinateImage = ResolveLayerPreviewImage(record.CoordinateLayer);
            if (!VisionPipelineScaleCalibrationStorage.TryApply(record, coordinateImage, target, out error))
            {
                view.SetScaleCalibrationStatus("Scale was not applied: " + error);
                return;
            }

            try
            {
                VisionPipelineStorage.Save(recipeContext.Name, pipeline);
                if (!VisionPipelineStorage.TryValidateRoundTrip(recipeContext.Name, pipeline, out string roundTripMessage))
                {
                    view.SetScaleCalibrationStatus("Scale pipeline save did not verify: " + roundTripMessage);
                    return;
                }

                if (!VisionPipelineScaleCalibrationStorage.TrySave(
                        recipeContext.Name,
                        record,
                        out string evidencePath,
                        out error))
                {
                    view.SetScaleCalibrationStatus("Scale was applied, but its applied-Step audit did not save: " + error);
                    return;
                }

                validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
                view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
                SelectStep(selectedIndex, selectedMode);
                view.SetScaleCalibrationStatus(
                    $"Applied {record.MillimetersPerPixel:0.############} mm/px to '{target.Name}' only. Pipeline and {evidencePath} round-tripped; no Preview/Run occurred.");
            }
            catch (Exception ex)
            {
                view.SetScaleCalibrationStatus("Scale apply failed: " + ex.GetBaseException().Message);
            }
        }

        private void RequestStepEdit(int index)
        {
            if (pipeline?.Steps == null || index < 0 || index >= pipeline.Steps.Count)
            {
                return;
            }

            SelectStep(index, PipelineFlowPreviewMode.Overlay);
            EditSelectedStepRequested(this, EventArgs.Empty);
        }

        public bool OpenPairSampleForTest()
        {
            return RequestOpenPairSample();
        }

        private bool RequestOpenPairSample()
        {
            if (activePairCounterpartSample?.CanOpen != true
                || string.IsNullOrWhiteSpace(activePairCounterpartSample.SampleName))
            {
                return false;
            }

            OpenWorkspaceSampleRequested(
                this,
                new OpenVisionPipelineReviewSampleOpenRequestedEventArgs(activePairCounterpartSample.SampleName));
            return true;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (disposed)
            {
                return;
            }

            if (!view.Dispatcher.CheckAccess())
            {
                view.Dispatcher.Invoke(RefreshLocalizedDisplay);
                return;
            }

            RefreshLocalizedDisplay();
        }

        private void RefreshLocalizedDisplay()
        {
            activePipelineName = ResolveActivePipelineName();
            int stepCount = pipeline?.Steps?.Count ?? 0;
            RefreshActiveSamplePairGuide(activePipelineName);
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            view.SetRecipeContext(recipeContext.Name);
            view.SetPipelineHeader(activePipelineName, stepCount);
            view.SetReviewProgress(FormatReviewProgressText());
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            RefreshReadiness();

            if (stepCount == 0)
            {
                selectedIndex = -1;
                view.SetEmptyState(activePipelineName);
                view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
                return;
            }

            int preservedIndex = view.SelectedFlowIndex >= 0 ? view.SelectedFlowIndex : selectedIndex;
            PipelineFlowPreviewMode preservedMode = selectedMode;
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            selectedMode = preservedMode;
            if (preservedIndex >= 0 && preservedIndex < stepCount)
            {
                selectedIndex = preservedIndex;
            }
            else if (selectedIndex < 0 || selectedIndex >= stepCount)
            {
                selectedIndex = 0;
            }

            SelectStep(selectedIndex, selectedMode);
        }
    }
}
