using OpenVisionLab.Pipeline.Controls;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DrawingBitmap = System.Drawing.Bitmap;
using WpfImage = System.Windows.Controls.Image;
using WpfPoint = System.Windows.Point;

namespace OpenVisionLab
{
    public partial class OpenVisionPipelineReviewView
    {
        private void OnPipelineFlowStepSelected(object sender, PipelineFlowStepSelectedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            StepSelected?.Invoke(this, e);
        }

        private void BtnReturnToRecipe_Click(object sender, RoutedEventArgs e)
        {
            ReturnToRecipeRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnPreviousStep_Click(object sender, RoutedEventArgs e)
        {
            PreviousStepRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnNextStep_Click(object sender, RoutedEventArgs e)
        {
            NextStepRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnFirstIssueStep_Click(object sender, RoutedEventArgs e)
        {
            FirstIssueStepRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnOpenSelectedToolLearn_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedToolLearnRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnEditSelectedStep_Click(object sender, RoutedEventArgs e)
        {
            EditSelectedStepRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnUseSelectedMatchingPose_Click(object sender, RoutedEventArgs e)
        {
            UseSelectedMatchingPoseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnEditFixtureProducer_Click(object sender, RoutedEventArgs e)
        {
            EditFixtureProducerRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnEditFixtureMeasurement_Click(object sender, RoutedEventArgs e)
        {
            EditFixtureMeasurementRequested?.Invoke(this, EventArgs.Empty);
        }

        private void FixtureConsumerGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suppressFixtureConsumerSelection
                || fixtureConsumerGrid.SelectedItem is not OpenVisionPipelineReviewFixtureConsumerRow row)
            {
                return;
            }

            FixtureConsumerSelected?.Invoke(
                this,
                new OpenVisionPipelineReviewFixtureConsumerSelectedEventArgs(row.StepIndex));
        }

        private void BtnOpenPairSample_Click(object sender, RoutedEventArgs e)
        {
            OpenPairSampleRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnRunReview_Click(object sender, RoutedEventArgs e)
        {
            RunReviewRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ScalePoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suppressScaleCalibrationSelection)
            {
                return;
            }

            RefreshScaleCalibrationPreview();
            SetScaleCalibrationStatus(null);
        }

        private void BtnCalculateScaleCalibration_Click(object sender, RoutedEventArgs e)
        {
            VisionPipelineGeometryFeatureResult pointA = cmbScalePointA.SelectedItem as VisionPipelineGeometryFeatureResult;
            VisionPipelineGeometryFeatureResult pointB = cmbScalePointB.SelectedItem as VisionPipelineGeometryFeatureResult;
            if (pointA == null || pointB == null)
            {
                SetScaleCalibrationStatus(T("PipelineReview.ScaleCalibration.InvalidPoint", "Select two valid points."));
                return;
            }

            if (!OpenVisionPipelineReviewViewRenderService.TryParsePositiveDouble(txtScaleKnownDistance.Text, out double knownDistance))
            {
                SetScaleCalibrationStatus(T("PipelineReview.ScaleCalibration.InvalidKnownDistance", "Known distance must be a positive number."));
                return;
            }

            VisionScaleCalibrationUnit unit = VisionScaleCalibrationUnit.Millimeter;
            if (cmbScaleUnit.SelectedItem is VisionScaleCalibrationUnitOption unitOption)
            {
                unit = unitOption.Unit;
            }

            ScaleCalibrationRequested?.Invoke(
                this,
                new VisionScaleCalibrationRequestedEventArgs(
                    pointA.Identity,
                    pointB.Identity,
                    knownDistance,
                    unit));
        }

        private void BtnApplyScaleCalibration_Click(object sender, RoutedEventArgs e)
        {
            if (cmbScaleTargetStep.SelectedItem is not VisionPipelineScaleTargetOption selectedTarget)
            {
                SetScaleCalibrationStatus(T("PipelineReview.ScaleCalibration.SelectTarget", "Select one compatible target Step."));
                return;
            }

            ScaleCalibrationApplyRequested?.Invoke(
                this,
                new VisionScaleCalibrationApplyRequestedEventArgs(selectedTarget.StepIndex));
        }

        private void ImgOutputPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not WpfImage image)
            {
                return;
            }

            if (!TryTranslateImageClickToImagePoint(image, objectResultBaseImage, e.GetPosition(image), out double x, out double y))
            {
                return;
            }

            if (sender == imgObjectResultPreview)
            {
                SelectObjectAtImagePointForTest(x, y);
                e.Handled = true;
                return;
            }

            SelectObjectAt(x, y);
            e.Handled = true;
        }

        private void ImgGeometryResultPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image image)
            {
                return;
            }

            if (!TryTranslateImageClickToImagePoint(image, objectResultBaseImage, e.GetPosition(image), out double x, out double y))
            {
                return;
            }

            SelectGeometryAtImagePointForTest(x, y);
            e.Handled = true;
        }

        private void ImgCircleEvidencePreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image image
                || !TryTranslateImageClickToImagePoint(
                    image,
                    objectResultBaseImage,
                    e.GetPosition(image),
                    out double x,
                    out double y))
            {
                return;
            }

            SelectCircleSampleAtImagePointForTest(x, y);
            e.Handled = true;
        }

        private void ObjectResultsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suppressObjectSelection)
            {
                return;
            }

            VisionPipelineObjectResult selected = objectResultsGrid?.SelectedItem as VisionPipelineObjectResult;
            suppressObjectSelection = true;
            ShowObjectHighlight(selected);
            UpdateObjectMetricSelection();
            suppressObjectSelection = false;
        }

        private void GeometryResultsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suppressGeometrySelection)
            {
                return;
            }

            VisionPipelineGeometryFeatureResult selected = geometryResultsGrid?.SelectedItem as VisionPipelineGeometryFeatureResult;
            suppressGeometrySelection = true;
            ShowGeometryHighlight(selected);
            suppressGeometrySelection = false;
        }

        private void InstanceResultsGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (suppressInstanceSelection)
            {
                return;
            }

            VisionPipelineInstanceResult selected =
                instanceResultsGrid?.SelectedItem as VisionPipelineInstanceResult;
            suppressInstanceSelection = true;
            ShowInstanceHighlight(selected);
            suppressInstanceSelection = false;
        }

        private void CircleSamplesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suppressCircleSelection)
            {
                return;
            }

            VisionPipelineCircleSampleEvidence selected =
                circleSamplesGrid?.SelectedItem as VisionPipelineCircleSampleEvidence;
            if (selected == null)
            {
                RestoreObjectResultPreview();
                RefreshCircleEvidencePlot();
                return;
            }

            ShowCircleSampleHighlight(selected);
            RefreshCircleEvidencePlot();
        }

        private void CircleEvidencePlot_SampleSelectionRequested(
            object sender,
            VisionToolSignalSampleSelectedEventArgs e)
        {
            if (showCircleProfile || circleSamples == null || circleSamples.Count == 0)
            {
                return;
            }

            int number = (int)Math.Round(e.X);
            VisionPipelineCircleSampleEvidence selected =
                circleSamples.OrderBy(item => Math.Abs(item.Number - number)).FirstOrDefault();
            if (selected != null)
            {
                SelectCircleSampleInternal(selected);
            }
        }

        private void ObjectMetricPlot_SampleSelectionRequested(
            object sender,
            VisionToolSignalSampleSelectedEventArgs e)
        {
            if (objectMetricDistribution == null || objectResults == null || objectResults.Count == 0)
            {
                return;
            }

            VisionPipelineObjectResult selected = objectResults
                .OrderBy(item => Math.Abs(objectMetricDistribution.GetValue(item) - e.X))
                .ThenBy(item => item.Number)
                .FirstOrDefault();
            if (selected != null)
            {
                SelectObjectAtInternal(selected);
            }
        }

        private void BtnObjectMetricArea_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectObjectMetricKindInternal(VisionPipelineObjectMetricKind.Area);
        }

        private void BtnObjectMetricWidth_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectObjectMetricKindInternal(VisionPipelineObjectMetricKind.BoundsWidth);
        }

        private void BtnObjectMetricHeight_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectObjectMetricKindInternal(VisionPipelineObjectMetricKind.BoundsHeight);
        }

        private void BtnCircleResidualPlot_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            showCircleProfile = false;
            RefreshCircleEvidencePlot();
        }

        private void BtnCircleProfilePlot_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            showCircleProfile = true;
            RefreshCircleEvidencePlot();
        }

        private void SelectObjectAt(double x, double y)
        {
            VisionPipelineObjectResult selected = objectResults
                .Select(item =>
                {
                    if (item == null)
                    {
                        return (item: item, distance: double.PositiveInfinity);
                    }

                    double dx = x - item.CenterX;
                    double dy = y - item.CenterY;
                    return (item, distance: Math.Sqrt(dx * dx + dy * dy));
                })
                .Where(entry => entry.item != null && !double.IsInfinity(entry.distance))
                .OrderBy(entry => entry.distance)
                .FirstOrDefault()
                .item;
            if (selected == null)
            {
                return;
            }

            SelectObjectAtInternal(selected);
        }

        internal void SelectObjectAtImagePointForTest(double x, double y)
        {
            SelectObjectAt(x, y);
        }

        private void SelectObjectAtInternal(VisionPipelineObjectResult item)
        {
            if (objectResultsGrid == null)
            {
                return;
            }

            suppressObjectSelection = true;
            objectResultsGrid.SelectedItem = item;
            objectResultsGrid.ScrollIntoView(item);
            ShowObjectHighlight(item);
            UpdateObjectMetricSelection();
            suppressObjectSelection = false;
        }

        internal bool SelectGeometryAtImagePointForTest(double x, double y)
        {
            if (geometryResults == null || geometryResults.Count == 0)
            {
                if (geometryResultsGrid != null)
                {
                    suppressGeometrySelection = true;
                    geometryResultsGrid.SelectedItem = null;
                    ShowGeometryHighlight(null);
                    suppressGeometrySelection = false;
                }

                return false;
            }

            double tolerance = objectResultBaseImage == null
                ? 8D
                : Math.Max(6D, Math.Min(objectResultBaseImage.Width, objectResultBaseImage.Height) / 80D);
            (VisionPipelineGeometryFeatureResult feature, double distance) best = geometryResults
                .Select(item => (item: item, distance: OpenVisionPipelineReviewViewRenderService.GeometryHitDistance(item, x, y)))
                .Where(entry => entry.item != null && entry.distance <= tolerance)
                .OrderBy(entry => entry.distance)
                .FirstOrDefault();

            if (geometryResultsGrid == null || best.feature == null)
            {
                return false;
            }

            suppressGeometrySelection = true;
            geometryResultsGrid.SelectedItem = best.feature;
            geometryResultsGrid.ScrollIntoView(best.feature);
            ShowGeometryHighlight(best.feature);
            suppressGeometrySelection = false;
            return true;
        }

        internal string SelectedGeometryIdentityForTest =>
            (geometryResultsGrid.SelectedItem as VisionPipelineGeometryFeatureResult)?.Identity ?? string.Empty;

        private void RefreshScaleCalibrationPreview()
        {
            if (scaleCalibrationBaseImage == null)
            {
                ViewModel.SetScaleCalibrationPreview(null);
                return;
            }

            VisionPipelineGeometryFeatureResult pointA = cmbScalePointA.SelectedItem as VisionPipelineGeometryFeatureResult;
            VisionPipelineGeometryFeatureResult pointB = cmbScalePointB.SelectedItem as VisionPipelineGeometryFeatureResult;
            if (pointA == null || pointB == null)
            {
                ViewModel.SetScaleCalibrationPreview(scaleCalibrationBaseImage);
                return;
            }

            DrawingBitmap preview = OpenVisionPipelineReviewViewRenderService.CreateScaleCalibrationPreview(
                scaleCalibrationBaseImage,
                pointA,
                pointB,
                out string previewText);
            if (preview == null)
            {
                ViewModel.SetScaleCalibrationPreview(scaleCalibrationBaseImage);
            }
            else
            {
                ViewModel.SetScaleCalibrationPreview(preview);
            }

            preview?.Dispose();

            if (!string.IsNullOrWhiteSpace(previewText))
            {
                lblScaleCalibrationPreview.Text = previewText;
            }
        }

        private void ShowObjectHighlight(VisionPipelineObjectResult item)
        {
            if (item == null || objectResultBaseImage == null)
            {
                RestoreObjectResultPreview();
                return;
            }

            DrawingBitmap preview = OpenVisionPipelineReviewViewRenderService.CreateObjectHighlight(objectResultBaseImage, item);
            if (preview == null)
            {
                RestoreObjectResultPreview();
                return;
            }

            ViewModel.SetHighlightedOutputPreview(preview);
            preview.Dispose();
            HasObjectHighlight = true;
        }

        private void ShowGeometryHighlight(VisionPipelineGeometryFeatureResult item)
        {
            if (item == null || objectResultBaseImage == null)
            {
                RestoreObjectResultPreview();
                return;
            }

            DrawingBitmap preview = OpenVisionPipelineReviewViewRenderService.CreateGeometryHighlight(objectResultBaseImage, item);
            if (preview == null)
            {
                RestoreObjectResultPreview();
                return;
            }

            ViewModel.SetHighlightedOutputPreview(preview);
            preview.Dispose();
            HasObjectHighlight = true;
        }

        private void ShowInstanceHighlight(VisionPipelineInstanceResult item)
        {
            if (item == null || objectResultBaseImage == null)
            {
                RestoreObjectResultPreview();
                return;
            }

            DrawingBitmap preview =
                OpenVisionPipelineReviewInstanceRenderService.CreateHighlight(
                    objectResultBaseImage,
                    item);
            if (preview == null)
            {
                RestoreObjectResultPreview();
                return;
            }

            ViewModel.SetHighlightedOutputPreview(preview);
            preview.Dispose();
            HasObjectHighlight = true;
        }

        private void ShowCircleSampleHighlight(VisionPipelineCircleSampleEvidence item)
        {
            if (item == null || objectResultBaseImage == null || circleEvidence == null)
            {
                RestoreObjectResultPreview();
                return;
            }

            DrawingBitmap preview =
                OpenVisionPipelineReviewCircleEvidenceRenderService.CreateSampleHighlight(
                    objectResultBaseImage,
                    circleEvidence,
                    item);
            if (preview == null)
            {
                RestoreObjectResultPreview();
                return;
            }

            ViewModel.SetHighlightedOutputPreview(preview);
            preview.Dispose();
            HasObjectHighlight = true;
        }

        private void RestoreObjectResultPreview()
        {
            ViewModel.SetHighlightedOutputPreview(objectResultBaseImage);
            HasObjectHighlight = false;
        }

        private static bool TryTranslateImageClickToImagePoint(
            WpfImage image,
            DrawingBitmap imageBitmap,
            WpfPoint controlPoint,
            out double imageX,
            out double imageY)
        {
            imageX = 0D;
            imageY = 0D;

            if (imageBitmap == null || image?.Source == null)
            {
                return false;
            }

            if (image.ActualWidth <= 0D
                || image.ActualHeight <= 0D
                || imageBitmap.Width <= 0
                || imageBitmap.Height <= 0)
            {
                return false;
            }

            Rect displayedImage = GetDisplayedImageRect(image, imageBitmap);
            if (!displayedImage.Contains(controlPoint) || displayedImage.Width <= 0D || displayedImage.Height <= 0D)
            {
                return false;
            }

            imageX = ((controlPoint.X - displayedImage.Left) / displayedImage.Width) * imageBitmap.Width;
            imageY = ((controlPoint.Y - displayedImage.Top) / displayedImage.Height) * imageBitmap.Height;

            imageX = Math.Max(0D, Math.Min(imageBitmap.Width - 1D, imageX));
            imageY = Math.Max(0D, Math.Min(imageBitmap.Height - 1D, imageY));
            return true;
        }

        private static Rect GetDisplayedImageRect(WpfImage image, DrawingBitmap imageBitmap)
        {
            if (image == null || imageBitmap == null || image.ActualWidth <= 0D || image.ActualHeight <= 0D)
            {
                return Rect.Empty;
            }

            double imageAspect = imageBitmap.Width / (double)imageBitmap.Height;
            double controlAspect = image.ActualWidth / image.ActualHeight;
            if (controlAspect > imageAspect)
            {
                double displayedWidth = image.ActualHeight * imageAspect;
                return new Rect(
                    (image.ActualWidth - displayedWidth) / 2D,
                    0D,
                    displayedWidth,
                    image.ActualHeight);
            }

            double displayedHeight = image.ActualWidth / imageAspect;
            return new Rect(
                0D,
                (image.ActualHeight - displayedHeight) / 2D,
                image.ActualWidth,
                displayedHeight);
        }
    }
}
