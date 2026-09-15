using DrawingBitmap = System.Drawing.Bitmap;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionPipelineReviewImageResourceOwner : IDisposable
    {
        private DrawingBitmap objectResultBaseImage;
        private DrawingBitmap objectMetricSourceImage;
        private DrawingBitmap circleSourceImage;
        private DrawingBitmap scaleCalibrationBaseImage;
        private OpenVisionPipelineReviewMatcherDiagnosticState matcherDiagnosticState;

        public DrawingBitmap ObjectResultBaseImage => objectResultBaseImage;
        public DrawingBitmap ObjectMetricSourceImage => objectMetricSourceImage;
        public DrawingBitmap CircleSourceImage => circleSourceImage;
        public DrawingBitmap ScaleCalibrationBaseImage => scaleCalibrationBaseImage;
        public OpenVisionPipelineReviewMatcherDiagnosticState MatcherDiagnosticState => matcherDiagnosticState;

        public void ReplaceObjectResultBaseImage(DrawingBitmap image)
        {
            ReplaceBitmap(ref objectResultBaseImage, image);
        }

        public void ReplaceObjectMetricSourceImage(DrawingBitmap image)
        {
            ReplaceBitmap(ref objectMetricSourceImage, image);
        }

        public void ReplaceCircleSourceImage(DrawingBitmap image)
        {
            ReplaceBitmap(ref circleSourceImage, image);
        }

        public void ReplaceScaleCalibrationBaseImage(DrawingBitmap image)
        {
            ReplaceBitmap(ref scaleCalibrationBaseImage, image);
        }

        public void ReplaceMatcherDiagnosticState(OpenVisionPipelineReviewMatcherDiagnosticState state)
        {
            matcherDiagnosticState?.Dispose();
            matcherDiagnosticState = state;
        }

        public void Dispose()
        {
            DisposeBitmap(ref objectResultBaseImage);
            DisposeBitmap(ref objectMetricSourceImage);
            DisposeBitmap(ref circleSourceImage);
            DisposeBitmap(ref scaleCalibrationBaseImage);
            matcherDiagnosticState?.Dispose();
            matcherDiagnosticState = null;
        }

        private static void ReplaceBitmap(ref DrawingBitmap target, DrawingBitmap source)
        {
            DisposeBitmap(ref target);
            target = source == null ? null : new DrawingBitmap(source);
        }

        private static void DisposeBitmap(ref DrawingBitmap bitmap)
        {
            bitmap?.Dispose();
            bitmap = null;
        }
    }
}
