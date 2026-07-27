using OpenCvSharp;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeHistogramSignalEvidenceFactory
    {
        public static VisionToolSignalEvidence Create(
            Mat source,
            Mat result,
            string toolIdentity,
            string inputLayer,
            string parameterSummary)
        {
            if (source == null || source.Empty())
            {
                throw new ArgumentException("A source image is required.", nameof(source));
            }

            if (result == null || result.Empty())
            {
                throw new ArgumentException("A result image is required.", nameof(result));
            }

            string sourceHash = OpenVisionNativeGraySignalEvidenceCalculator.ComputeImageSha256(source);
            string resultHash = OpenVisionNativeGraySignalEvidenceCalculator.ComputeImageSha256(result);
            string evidenceId = OpenVisionNativeGraySignalEvidenceCalculator.CreateEvidenceId(
                toolIdentity ?? string.Empty,
                inputLayer ?? string.Empty,
                "Full image",
                parameterSummary ?? string.Empty,
                sourceHash,
                resultHash);

            return new VisionToolSignalEvidence(
                evidenceId,
                sourceHash,
                resultHash,
                toolIdentity,
                inputLayer,
                "Full image",
                parameterSummary,
                "Gray level",
                "Pixel %",
                new[]
                {
                    new VisionToolSignalSeries("Source", "#1F77B4", 0, 1, OpenVisionNativeGraySignalEvidenceCalculator.CreateNormalizedHistogram(source)),
                    new VisionToolSignalSeries("Result", "#E07A2F", 0, 1, OpenVisionNativeGraySignalEvidenceCalculator.CreateNormalizedHistogram(result))
                });
        }
    }
}
