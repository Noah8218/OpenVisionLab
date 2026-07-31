using Lib.OpenCV.Result;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Lib.Common.FormulaUtil;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeLineSignalAlternative
    {
        public OpenVisionNativeLineSignalAlternative(
            int scanPosition,
            OpenCvSharp.Point imagePoint,
            double response,
            bool isStable,
            string reason)
        {
            ScanPosition = scanPosition;
            ImagePoint = imagePoint;
            Response = response;
            IsStable = isStable;
            Reason = reason ?? string.Empty;
        }

        public int ScanPosition { get; }

        public OpenCvSharp.Point ImagePoint { get; }

        public double Response { get; }

        public bool IsStable { get; }

        public string Reason { get; }
    }

    internal sealed class OpenVisionNativeLineSignalProfile
    {
        private readonly IReadOnlyList<double> intensities;
        private readonly IReadOnlyList<double> responses;
        private readonly IReadOnlyList<OpenVisionNativeLineSignalAlternative> alternatives;
        private readonly IReadOnlyList<VisionToolSignalMarker> markers;
        private readonly string parameterSummary;
        private readonly string regionDescription;
        private readonly string guidance;

        public OpenVisionNativeLineSignalProfile(
            string lineName,
            OpenCvSharp.Point scanStart,
            OpenCvSharp.Point scanEnd,
            OpenCvSharp.Point selectedPoint,
            int selectedScanPosition,
            double selectedResponse,
            string parameterSummary,
            string regionDescription,
            string guidance,
            IReadOnlyList<double> intensities,
            IReadOnlyList<double> responses,
            IReadOnlyList<OpenVisionNativeLineSignalAlternative> alternatives,
            IReadOnlyList<VisionToolSignalMarker> markers)
        {
            LineName = string.IsNullOrWhiteSpace(lineName) ? "Line" : lineName.Trim();
            ScanStart = scanStart;
            ScanEnd = scanEnd;
            SelectedPoint = selectedPoint;
            SelectedScanPosition = selectedScanPosition;
            SelectedResponse = selectedResponse;
            this.parameterSummary = parameterSummary ?? string.Empty;
            this.regionDescription = regionDescription ?? string.Empty;
            this.guidance = guidance ?? string.Empty;
            this.intensities = intensities ?? throw new ArgumentNullException(nameof(intensities));
            this.responses = responses ?? throw new ArgumentNullException(nameof(responses));
            this.alternatives = alternatives ?? Array.Empty<OpenVisionNativeLineSignalAlternative>();
            this.markers = markers ?? Array.Empty<VisionToolSignalMarker>();
        }

        public string LineName { get; }

        public OpenCvSharp.Point ScanStart { get; }

        public OpenCvSharp.Point ScanEnd { get; }

        public OpenCvSharp.Point SelectedPoint { get; }

        public int SelectedScanPosition { get; }

        public double SelectedResponse { get; }

        public IReadOnlyList<OpenVisionNativeLineSignalAlternative> Alternatives => alternatives;

        public VisionToolSignalEvidence CreateEvidence(Mat source, Mat result, string inputLayer)
        {
            string sourceHash = OpenVisionNativeGraySignalEvidenceCalculator.ComputeImageSha256(source);
            string resultHash = OpenVisionNativeGraySignalEvidenceCalculator.ComputeImageSha256(result);
            string alternativeIdentity = string.Join(
                "|",
                alternatives.Select(item => string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}:{1},{2}:{3:0.###############}:{4}",
                    item.ScanPosition,
                    item.ImagePoint.X,
                    item.ImagePoint.Y,
                    item.Response,
                    item.Reason)));
            string evidenceId = OpenVisionNativeGraySignalEvidenceCalculator.CreateEvidenceId(
                "LineSignal/" + LineName,
                inputLayer ?? string.Empty,
                regionDescription,
                parameterSummary,
                FormatPoint(SelectedPoint),
                SelectedScanPosition.ToString(CultureInfo.InvariantCulture),
                SelectedResponse.ToString("0.###############", CultureInfo.InvariantCulture),
                alternativeIdentity,
                sourceHash,
                resultHash);

            return new VisionToolSignalEvidence(
                evidenceId,
                sourceHash,
                resultHash,
                "LineGauge/" + LineName,
                inputLayer,
                regionDescription,
                parameterSummary,
                "Scan distance (px)",
                "GV / signed \u0394GV",
                new[]
                {
                    new VisionToolSignalSeries("Intensity (GV)", "#1F77B4", 0, 1, intensities),
                    new VisionToolSignalSeries("Signed edge response (\u0394GV)", "#D35400", 0, 1, responses)
                },
                markers,
                guidance,
                new[]
                {
                    Attribute("CoordinateSpace", "Source image pixels"),
                    Attribute("ScanStart", FormatPoint(ScanStart)),
                    Attribute("ScanEnd", FormatPoint(ScanEnd)),
                    Attribute("SelectedImagePoint", FormatPoint(SelectedPoint)),
                    Attribute("SelectedScanPosition", SelectedScanPosition.ToString(CultureInfo.InvariantCulture)),
                    Attribute("SelectedResponse", SelectedResponse.ToString("0.###", CultureInfo.InvariantCulture)),
                    Attribute("RejectedAlternativeCount", alternatives.Count.ToString(CultureInfo.InvariantCulture)),
                    Attribute("RuntimeCorrespondence", "Matched LineGauge first-stable edge")
                });
        }

        private static KeyValuePair<string, string> Attribute(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value);
        }

        private static string FormatPoint(OpenCvSharp.Point point)
        {
            return string.Format(CultureInfo.InvariantCulture, "({0},{1})", point.X, point.Y);
        }
    }

    internal static class OpenVisionNativeLineSignalEvidenceFactory
    {
        private sealed class ScanSample
        {
            public ScanSample(OpenCvSharp.Point imagePoint, byte intensity)
            {
                ImagePoint = imagePoint;
                Intensity = intensity;
            }

            public OpenCvSharp.Point ImagePoint { get; }

            public byte Intensity { get; }
        }

        private sealed class Candidate
        {
            public Candidate(int scanPosition, OpenCvSharp.Point imagePoint, double response, bool isStable)
            {
                ScanPosition = scanPosition;
                ImagePoint = imagePoint;
                Response = response;
                IsStable = isStable;
            }

            public int ScanPosition { get; }

            public OpenCvSharp.Point ImagePoint { get; }

            public double Response { get; }

            public bool IsStable { get; }
        }

        public static bool TryCreate(
            Mat source,
            LineGaugeProperty property,
            IEnumerable<LineGaugeResult> results,
            string lineName,
            out OpenVisionNativeLineSignalProfile profile,
            out string failureReason)
        {
            profile = null;
            failureReason = string.Empty;
            try
            {
                profile = Create(source, property, results, lineName);
                return true;
            }
            catch (Exception ex)
            {
                failureReason = ex.Message;
                return false;
            }
        }

        private static OpenVisionNativeLineSignalProfile Create(
            Mat source,
            LineGaugeProperty property,
            IEnumerable<LineGaugeResult> results,
            string lineName)
        {
            if (source == null || source.Empty())
            {
                throw new ArgumentException("A source image is required.", nameof(source));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            LineGaugeResult result = (results ?? Array.Empty<LineGaugeResult>())
                .Where(item => item?.edgeList != null && item.edgeList.Count > 0)
                .OrderByDescending(item => item.edgeList.Count)
                .ThenBy(item => item.Index)
                .FirstOrDefault();
            if (result == null)
            {
                throw new InvalidOperationException("Line signal evidence requires a successful LineGauge result.");
            }

            Rect roi = ResolveRoi(source, property, result);
            using Mat prepared = CreatePreparedRoi(source, roi, property);
            OpenCvSharp.Point selectedPoint = result.edgeList[result.edgeList.Count / 2];
            OpenCvSharp.Point selectedLocalPoint = new OpenCvSharp.Point(
                selectedPoint.X - roi.X,
                selectedPoint.Y - roi.Y);
            List<ScanSample> samples = CreateScanSamples(prepared, roi, selectedLocalPoint, property.PRJ_DIR);
            int thickness = Math.Max(1, (int)property.THICKNESS);
            List<Candidate> candidates = CreateCandidates(samples, property.PRJ_DIR, property.PRJ_PORALITY, property.CONTRAST, thickness);
            Candidate selected = candidates.FirstOrDefault(item => item.IsStable);
            if (selected == null || selected.ImagePoint != selectedPoint)
            {
                throw new InvalidOperationException(
                    "Line signal replay did not match the runtime first-stable edge. "
                    + "Runtime=" + FormatPoint(selectedPoint)
                    + ", Replay=" + (selected == null ? "(none)" : FormatPoint(selected.ImagePoint)));
            }

            double[] intensities = samples.Select(item => (double)item.Intensity).ToArray();
            double[] responses = new double[intensities.Length];
            for (int index = 0; index < responses.Length - 1; index++)
            {
                responses[index] = intensities[index + 1] - intensities[index];
            }

            int distinctSeparation = Math.Max(2, thickness + 1);
            List<OpenVisionNativeLineSignalAlternative> alternatives = SelectDistinctAlternatives(
                    candidates,
                    selected,
                    distinctSeparation)
                .Select(item => new OpenVisionNativeLineSignalAlternative(
                    item.ScanPosition,
                    item.ImagePoint,
                    item.Response,
                    item.IsStable,
                    CreateAlternativeReason(item, selected)))
                .ToList();
            IReadOnlyList<VisionToolSignalMarker> markers = CreateMarkers(selected, alternatives);
            string parameterSummary = CreateParameterSummary(property);
            string regionDescription = string.Format(
                CultureInfo.InvariantCulture,
                "ROI {0},{1},{2},{3} / representative scan {4}->{5}",
                roi.X,
                roi.Y,
                roi.Width,
                roi.Height,
                FormatPoint(samples[0].ImagePoint),
                FormatPoint(samples[samples.Count - 1].ImagePoint));
            string guidance = CreateGuidance(property, selected, alternatives);

            return new OpenVisionNativeLineSignalProfile(
                lineName,
                samples[0].ImagePoint,
                samples[samples.Count - 1].ImagePoint,
                selected.ImagePoint,
                selected.ScanPosition,
                selected.Response,
                parameterSummary,
                regionDescription,
                guidance,
                intensities,
                responses,
                alternatives,
                markers);
        }

        private static IEnumerable<Candidate> SelectDistinctAlternatives(
            IEnumerable<Candidate> candidates,
            Candidate selected,
            int separation)
        {
            List<Candidate> remaining = candidates
                .Where(item => !ReferenceEquals(item, selected)
                    && Math.Abs(item.ScanPosition - selected.ScanPosition) > separation)
                .OrderBy(item => item.ScanPosition)
                .ToList();
            List<List<Candidate>> groups = new List<List<Candidate>>();
            foreach (Candidate candidate in remaining)
            {
                List<Candidate> group = groups.LastOrDefault();
                if (group == null
                    || candidate.ScanPosition - group[group.Count - 1].ScanPosition > separation)
                {
                    group = new List<Candidate>();
                    groups.Add(group);
                }

                group.Add(candidate);
            }

            foreach (List<Candidate> group in groups)
            {
                Candidate representative = group
                    .Where(item => item.IsStable)
                    .OrderBy(item => item.ScanPosition)
                    .FirstOrDefault()
                    ?? group.OrderByDescending(item => Math.Abs(item.Response)).First();
                yield return representative;
            }
        }

        private static Rect ResolveRoi(Mat source, LineGaugeProperty property, LineGaugeResult result)
        {
            Rect roi;
            if (property.USE_MULTI_ROI)
            {
                int index = Math.Max(0, result.Index - 1);
                if (property.CvROIS == null || index >= property.CvROIS.Count)
                {
                    throw new InvalidOperationException("The LineGauge result has no matching multi-ROI definition.");
                }

                roi = property.CvROIS[index];
            }
            else if (property.USE_ROI)
            {
                roi = property.CvROI;
            }
            else
            {
                roi = new Rect(0, 0, source.Width, source.Height);
            }

            int left = Math.Clamp(roi.X, 0, source.Width);
            int top = Math.Clamp(roi.Y, 0, source.Height);
            int right = Math.Clamp(roi.Right, left, source.Width);
            int bottom = Math.Clamp(roi.Bottom, top, source.Height);
            Rect clamped = new Rect(left, top, right - left, bottom - top);
            if (clamped.Width <= 0 || clamped.Height <= 0)
            {
                throw new InvalidOperationException("The reviewed LineGauge ROI is empty.");
            }

            return clamped;
        }

        private static Mat CreatePreparedRoi(Mat source, Rect roi, LineGaugeProperty property)
        {
            using Mat gray = OpenVisionNativeGraySignalEvidenceCalculator.CreateGray8Copy(source);
            Mat prepared = gray.SubMat(roi).Clone();
            if (property.USE_THRESHOLD)
            {
                Cv2.Threshold(prepared, prepared, property.THRESHOLD, 255, property.THRESHOLD_TYPES);
            }
            else if (property.USE_ADAPTIVE_THRESHOLD)
            {
                Cv2.AdaptiveThreshold(
                    prepared,
                    prepared,
                    property.ADAPTIVE_THRESHOLD,
                    property.ADAPTIVE_THRESHOLD_ALGORITHM,
                    property.ADAPTIVE_THRESHOLD_TYPES,
                    property.BlockSize,
                    property.Weight);
            }

            if (property.USE_BITWISENOT)
            {
                Cv2.BitwiseNot(prepared, prepared);
            }

            return prepared;
        }

        private static List<ScanSample> CreateScanSamples(
            Mat prepared,
            Rect roi,
            OpenCvSharp.Point selectedLocalPoint,
            PROJECTION_DIR direction)
        {
            if (selectedLocalPoint.X < 0
                || selectedLocalPoint.X >= prepared.Width
                || selectedLocalPoint.Y < 0
                || selectedLocalPoint.Y >= prepared.Height)
            {
                throw new InvalidOperationException("The selected LineGauge edge is outside the reviewed ROI.");
            }

            List<ScanSample> samples = new List<ScanSample>();
            switch (direction)
            {
                case PROJECTION_DIR.X_LTOR:
                    for (int x = 0; x < prepared.Width; x++)
                    {
                        samples.Add(CreateSample(prepared, roi, x, selectedLocalPoint.Y));
                    }
                    break;
                case PROJECTION_DIR.X_RTOL:
                    for (int x = prepared.Width - 1; x >= 0; x--)
                    {
                        samples.Add(CreateSample(prepared, roi, x, selectedLocalPoint.Y));
                    }
                    break;
                case PROJECTION_DIR.Y_TTOB:
                    for (int y = 0; y < prepared.Height; y++)
                    {
                        samples.Add(CreateSample(prepared, roi, selectedLocalPoint.X, y));
                    }
                    break;
                case PROJECTION_DIR.Y_BTOT:
                    for (int y = prepared.Height - 1; y >= 0; y--)
                    {
                        samples.Add(CreateSample(prepared, roi, selectedLocalPoint.X, y));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unsupported LineGauge projection direction: " + direction);
            }

            if (samples.Count < 3)
            {
                throw new InvalidOperationException("The representative scan requires at least three pixels.");
            }

            return samples;
        }

        private static ScanSample CreateSample(Mat prepared, Rect roi, int x, int y)
        {
            return new ScanSample(
                new OpenCvSharp.Point(roi.X + x, roi.Y + y),
                prepared.At<byte>(y, x));
        }

        private static List<Candidate> CreateCandidates(
            IReadOnlyList<ScanSample> samples,
            PROJECTION_DIR direction,
            PROJECTION_POLARITY polarity,
            double contrast,
            int thickness)
        {
            int start;
            int endExclusive;
            int stabilityStart;
            int stabilityEnd;
            switch (direction)
            {
                case PROJECTION_DIR.X_LTOR:
                    start = 1;
                    endExclusive = samples.Count - thickness - 2;
                    stabilityStart = 3;
                    stabilityEnd = thickness + 2;
                    break;
                case PROJECTION_DIR.X_RTOL:
                case PROJECTION_DIR.Y_BTOT:
                    start = 0;
                    endExclusive = samples.Count - thickness - 2;
                    stabilityStart = 2;
                    stabilityEnd = thickness + 1;
                    break;
                case PROJECTION_DIR.Y_TTOB:
                    start = 1;
                    endExclusive = samples.Count - thickness - 1;
                    stabilityStart = 2;
                    stabilityEnd = thickness;
                    break;
                default:
                    throw new InvalidOperationException("Unsupported LineGauge projection direction: " + direction);
            }

            List<Candidate> candidates = new List<Candidate>();
            for (int position = Math.Max(0, start); position < Math.Min(samples.Count - 1, endExclusive); position++)
            {
                double previous = samples[position].Intensity;
                double response = samples[position + 1].Intensity - previous;
                if (!MatchesPolarity(response, polarity, contrast))
                {
                    continue;
                }

                bool stable = true;
                for (int offset = stabilityStart; offset <= stabilityEnd; offset++)
                {
                    int index = position + offset;
                    if (index < 0
                        || index >= samples.Count
                        || !MatchesPolarity(samples[index].Intensity - previous, polarity, contrast))
                    {
                        stable = false;
                        break;
                    }
                }

                candidates.Add(new Candidate(position, samples[position].ImagePoint, response, stable));
            }

            return candidates;
        }

        private static bool MatchesPolarity(double response, PROJECTION_POLARITY polarity, double contrast)
        {
            switch (polarity)
            {
                case PROJECTION_POLARITY.BTOW:
                    return response > contrast;
                case PROJECTION_POLARITY.WTOB:
                    return -response > contrast;
                default:
                    return Math.Abs(response) > contrast;
            }
        }

        private static string CreateAlternativeReason(Candidate candidate, Candidate selected)
        {
            if (!candidate.IsStable)
            {
                return candidate.ScanPosition < selected.ScanPosition
                    ? "Rejected before selection: thickness continuity failed"
                    : "Rejected later candidate: thickness continuity failed";
            }

            return "Not selected: later stable edge after the first accepted edge";
        }

        private static IReadOnlyList<VisionToolSignalMarker> CreateMarkers(
            Candidate selected,
            IReadOnlyList<OpenVisionNativeLineSignalAlternative> alternatives)
        {
            List<VisionToolSignalMarker> markers = new List<VisionToolSignalMarker>();
            List<OpenVisionNativeLineSignalAlternative> visibleAlternatives = alternatives
                .OrderBy(item => item.ScanPosition < selected.ScanPosition ? 0 : 1)
                .ThenBy(item => item.IsStable ? 1 : 0)
                .ThenByDescending(item => Math.Abs(item.Response))
                .Take(4)
                .ToList();
            for (int index = 0; index < visibleAlternatives.Count; index++)
            {
                OpenVisionNativeLineSignalAlternative item = visibleAlternatives[index];
                markers.Add(new VisionToolSignalMarker(
                    "Alternative" + (index + 1).ToString(CultureInfo.InvariantCulture),
                    (item.IsStable ? "Later " : "Rejected ") + FormatPoint(item.ImagePoint),
                    item.ScanPosition,
                    item.IsStable ? "#7D3C98" : "#E67E22",
                    false));
            }

            markers.Add(new VisionToolSignalMarker(
                "SelectedEdge",
                "Selected " + FormatPoint(selected.ImagePoint),
                selected.ScanPosition,
                "#C0392B",
                false));
            return markers;
        }

        private static string CreateParameterSummary(LineGaugeProperty property)
        {
            string preprocessing = property.USE_THRESHOLD
                ? string.Format(CultureInfo.InvariantCulture, "Threshold {0:0.#}/{1}", property.THRESHOLD, property.THRESHOLD_TYPES)
                : property.USE_ADAPTIVE_THRESHOLD
                    ? string.Format(
                        CultureInfo.InvariantCulture,
                        "Adaptive {0}/{1}/Block {2}/C {3}",
                        property.ADAPTIVE_THRESHOLD_ALGORITHM,
                        property.ADAPTIVE_THRESHOLD_TYPES,
                        property.BlockSize,
                        property.Weight)
                    : "Grayscale";
            if (property.USE_BITWISENOT)
            {
                preprocessing += " + Invert";
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} / {1} / Min contrast {2:0.###} GV / Thickness {3:0} / Sampling {4:0.###} / {5}",
                property.PRJ_DIR,
                property.PRJ_PORALITY,
                property.CONTRAST,
                property.THICKNESS,
                property.SAMPLING_STEP,
                preprocessing);
        }

        private static string CreateGuidance(
            LineGaugeProperty property,
            Candidate selected,
            IReadOnlyList<OpenVisionNativeLineSignalAlternative> alternatives)
        {
            string rule = property.PRJ_PORALITY == PROJECTION_POLARITY.BTOW
                ? "signed response > +" + property.CONTRAST.ToString("0.###", CultureInfo.InvariantCulture) + " GV"
                : property.PRJ_PORALITY == PROJECTION_POLARITY.WTOB
                    ? "signed response < -" + property.CONTRAST.ToString("0.###", CultureInfo.InvariantCulture) + " GV"
                    : "|signed response| > " + property.CONTRAST.ToString("0.###", CultureInfo.InvariantCulture) + " GV";
            string alternativesText = alternatives.Count == 0
                ? "No other contrast candidates were present on this representative scan."
                : string.Join(
                    "; ",
                    alternatives.Take(6).Select(item => string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} at {1}, \u0394GV {2:0.###}: {3}",
                        item.IsStable ? "Later stable" : "Rejected",
                        FormatPoint(item.ImagePoint),
                        item.Response,
                        item.Reason)));
            if (alternatives.Count > 6)
            {
                alternativesText += string.Format(
                    CultureInfo.InvariantCulture,
                    "; +{0} more in TSV evidence",
                    alternatives.Count - 6);
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "Runtime replay matched the first stable edge at {0} (scan {1}, \u0394GV {2:0.###}). "
                + "Polarity gate: {3}. {4}",
                FormatPoint(selected.ImagePoint),
                selected.ScanPosition,
                selected.Response,
                rule,
                alternativesText);
        }

        private static string FormatPoint(OpenCvSharp.Point point)
        {
            return string.Format(CultureInfo.InvariantCulture, "({0},{1})", point.X, point.Y);
        }
    }
}
