using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionToolSignalMarker
    {
        public VisionToolSignalMarker(
            string id,
            string name,
            double x,
            string colorHex,
            bool isEditable,
            double snapStep = 1d)
        {
            Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("A marker ID is required.", nameof(id)) : id.Trim();
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("A marker name is required.", nameof(name)) : name.Trim();
            X = double.IsFinite(x) ? x : throw new ArgumentOutOfRangeException(nameof(x), "Marker X must be finite.");
            ColorHex = string.IsNullOrWhiteSpace(colorHex) ? "#C0392B" : colorHex.Trim();
            IsEditable = isEditable;
            SnapStep = snapStep > 0 && double.IsFinite(snapStep)
                ? snapStep
                : throw new ArgumentOutOfRangeException(nameof(snapStep), "Marker snap step must be finite and positive.");
        }

        public string Id { get; }

        public string Name { get; }

        public double X { get; }

        public string ColorHex { get; }

        public bool IsEditable { get; }

        public double SnapStep { get; }
    }

    internal sealed class VisionToolSignalMarkerValueChangedEventArgs : EventArgs
    {
        public VisionToolSignalMarkerValueChangedEventArgs(string markerId, double value)
        {
            MarkerId = markerId ?? string.Empty;
            Value = value;
        }

        public string MarkerId { get; }

        public double Value { get; }
    }

    internal sealed class VisionToolSignalSeries
    {
        public VisionToolSignalSeries(
            string name,
            string colorHex,
            double xStart,
            double xStep,
            IEnumerable<double> values)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("A series name is required.", nameof(name)) : name.Trim();
            ColorHex = string.IsNullOrWhiteSpace(colorHex) ? "#157C86" : colorHex.Trim();
            XStart = xStart;
            XStep = xStep > 0 && double.IsFinite(xStep)
                ? xStep
                : throw new ArgumentOutOfRangeException(nameof(xStep), "X step must be finite and positive.");
            Values = Array.AsReadOnly((values ?? throw new ArgumentNullException(nameof(values))).ToArray());
            if (Values.Count == 0 || Values.Any(value => !double.IsFinite(value)))
            {
                throw new ArgumentException("A signal series requires finite values.", nameof(values));
            }
        }

        public string Name { get; }

        public string ColorHex { get; }

        public double XStart { get; }

        public double XStep { get; }

        public IReadOnlyList<double> Values { get; }

        public double XEnd => XStart + ((Values.Count - 1) * XStep);
    }

    internal sealed class VisionToolSignalEvidence
    {
        public VisionToolSignalEvidence(
            string evidenceId,
            string sourceSha256,
            string resultSha256,
            string toolIdentity,
            string inputLayer,
            string regionDescription,
            string parameterSummary,
            string xAxisLabel,
            string yAxisLabel,
            IEnumerable<VisionToolSignalSeries> series,
            IEnumerable<VisionToolSignalMarker> markers = null,
            string guidance = null)
        {
            EvidenceId = Require(evidenceId, nameof(evidenceId));
            SourceSha256 = Require(sourceSha256, nameof(sourceSha256));
            ResultSha256 = Require(resultSha256, nameof(resultSha256));
            ToolIdentity = Require(toolIdentity, nameof(toolIdentity));
            InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "(none)" : inputLayer.Trim();
            RegionDescription = string.IsNullOrWhiteSpace(regionDescription) ? "Full image" : regionDescription.Trim();
            ParameterSummary = string.IsNullOrWhiteSpace(parameterSummary) ? "(none)" : parameterSummary.Trim();
            XAxisLabel = Require(xAxisLabel, nameof(xAxisLabel));
            YAxisLabel = Require(yAxisLabel, nameof(yAxisLabel));
            Series = Array.AsReadOnly((series ?? throw new ArgumentNullException(nameof(series))).ToArray());
            if (Series.Count == 0)
            {
                throw new ArgumentException("At least one signal series is required.", nameof(series));
            }

            Markers = Array.AsReadOnly((markers ?? Array.Empty<VisionToolSignalMarker>()).ToArray());
            Guidance = guidance?.Trim() ?? string.Empty;
        }

        public string EvidenceId { get; }

        public string SourceSha256 { get; }

        public string ResultSha256 { get; }

        public string ToolIdentity { get; }

        public string InputLayer { get; }

        public string RegionDescription { get; }

        public string ParameterSummary { get; }

        public string XAxisLabel { get; }

        public string YAxisLabel { get; }

        public IReadOnlyList<VisionToolSignalSeries> Series { get; }

        public IReadOnlyList<VisionToolSignalMarker> Markers { get; }

        public string Guidance { get; }

        private static string Require(string value, string parameterName)
        {
            return string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("A non-empty value is required.", parameterName)
                : value.Trim();
        }
    }
}
