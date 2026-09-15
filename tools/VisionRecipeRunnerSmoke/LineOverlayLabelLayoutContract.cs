using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Core.Geometry2D;
using System;
using System.Collections.Generic;
using System.IO;

internal static class LineOverlayLabelLayoutContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine("D:\\OpenVisionLab-TestData", "OpenVisionLab_Dev", "line-overlay-label-layout")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        Size imageSize = new Size(572, 420);
        LineSegment2D[] measurementLines = CreateMeasurementLines();
        Size valueTextSize = Cv2.GetTextSize("123456789.123 mm", HersheyFonts.HersheySimplex, 0.35, 1, out int valueBaseline);
        List<Rect> measurementLabels = new List<Rect>();
        foreach (LineSegment2D line in measurementLines)
        {
            if (VisionPipelineLineDistanceTool.TryResolveDistanceLabelBounds(
                imageSize,
                line,
                valueTextSize,
                valueBaseline,
                measurementLabels,
                out Rect bounds))
            {
                measurementLabels.Add(bounds);
            }
        }

        const string profileText = "Profile Line A with a deliberately long operator label";
        Size profileTextSize = Cv2.GetTextSize(profileText, HersheyFonts.HersheySimplex, 0.38, 1, out int profileBaseline);
        Rect profileBounds = OpenVisionNativeToolPreviewOverlayRenderer.ResolveLineSignalLabelBounds(
            imageSize,
            new Point(430, 230),
            new Point(560, 230),
            profileTextSize,
            profileBaseline);

        List<string> results = new List<string>();
        Check("At least one dense measurement label remains visible", measurementLabels.Count > 0, results);
        Check("Long measurement labels stay inside the source image", measurementLabels.TrueForAll(item => IsInside(item, imageSize)), results);
        Check("Dense measurement labels do not overlap each other", HaveNoOverlap(measurementLabels), results);
        Check("Profile label stays inside the source image", IsInside(profileBounds, imageSize), results);
        Check("Profile label is separated from every measurement label", measurementLabels.TrueForAll(item => !Intersects(item, profileBounds)), results);

        string evidencePath = Path.Combine(evidenceDirectory, "line-overlay-label-layout-contract.txt");
        results.Add("MeasurementLabelsPlaced: " + measurementLabels.Count);
        results.Add("ProfileBounds: " + profileBounds);
        File.WriteAllLines(evidencePath, results);
        bool passed = results.TrueForAll(line => !line.StartsWith("FAIL: ", StringComparison.Ordinal));
        Console.WriteLine("LINE_OVERLAY_LABEL_LAYOUT_CONTRACT=" + (passed ? "PASS" : "FAIL") + "|checks=5|labels=" + measurementLabels.Count);
        return passed ? 0 : 1;
    }

    private static LineSegment2D[] CreateMeasurementLines()
    {
        LineSegment2D[] lines = new LineSegment2D[24];
        for (int index = 0; index < lines.Length; index++)
        {
            int y = 118 + index * 10;
            lines[index] = new LineSegment2D(new Point(462, y), new Point(498, y));
        }

        return lines;
    }

    private static bool IsInside(Rect bounds, Size imageSize)
    {
        return bounds.X >= 0
            && bounds.Y >= 0
            && bounds.Right <= imageSize.Width
            && bounds.Bottom <= imageSize.Height;
    }

    private static bool HaveNoOverlap(IReadOnlyList<Rect> bounds)
    {
        for (int first = 0; first < bounds.Count; first++)
        {
            for (int second = first + 1; second < bounds.Count; second++)
            {
                if (Intersects(bounds[first], bounds[second]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool Intersects(Rect first, Rect second)
    {
        return first.X < second.Right
            && first.Right > second.X
            && first.Y < second.Bottom
            && first.Bottom > second.Y;
    }

    private static void Check(string name, bool condition, ICollection<string> results)
    {
        results.Add((condition ? "PASS: " : "FAIL: ") + name);
    }
}
