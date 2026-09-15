using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Property;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class LineToolPartialBoundaryContract
{
    internal static int Run(string requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        try
        {
            Require(
                string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
                "Line Tool Partial evidence must be on D:.");

            string viewSource = Read(repositoryRoot, @"src\OpenVisionLab\UI\VisionTest\Wpf\ToolViews\LineToolWpfView.xaml.cs");
            string adapterSource = Read(repositoryRoot, @"src\OpenVisionLab\UI\Menu\Wpf\Recipe\PropertyGrid\VisionPipelineLinePropertyAdapter.cs");
            string documentSource = Read(repositoryRoot, @"src\OpenVisionLab\UI\Menu\Wpf\NativeTools\Documents\OpenVisionNativeToolDocument.cs");
            string factorySource = Read(repositoryRoot, @"src\OpenVisionLab\UI\Menu\Wpf\NativeTools\Documents\OpenVisionNativeCustomToolFactory.cs");

            Require(
                Count(viewSource, "VisionPipelineLinePropertyAdapter.ApplySampleProperty") == 2
                    && viewSource.Contains("ApplySampleLinePair", StringComparison.Ordinal)
                    && !viewSource.Contains("CopyLineProperty", StringComparison.Ordinal)
                    && !viewSource.Contains("target.", StringComparison.Ordinal),
                "Line View should delegate both sample property projections and retain no copy body.");
            passed.Add("Line View sample path delegates both A/B projections to the existing Line adapter");

            string[] mappedMembers =
            {
                "PIXELPERMM", "USE_THRESHOLD", "USE_BITWISENOT", "THRESHOLD_TYPES", "THRESHOLD",
                "USE_ADAPTIVE_THRESHOLD", "ADAPTIVE_THRESHOLD", "ADAPTIVE_THRESHOLD_TYPES",
                "ADAPTIVE_THRESHOLD_ALGORITHM", "BlockSize", "Weight", "USE_ROI", "CvROI",
                "USE_MULTI_ROI", "CvROIS", "USE_MASKING", "CvMASKS", "PRJ_PORALITY", "PRJ_DIR",
                "CONTRAST", "THICKNESS", "SAMPLING_STEP", "VER_PRJ_DIR", "POINT_RANGE",
                "USE_MANUAL_ANGLE", "MANUAL_ANGLE_VALUE", "USE_EXTEND_FIT_LINE", "EXTEND_FIT_LINE_VALUE",
                "AVERAGE_Diff", "USE_AVERAGE_FILTER", "AVERAGE_FILTER_TYPE", "SHOW_VERTICAL_LINE",
                "SHOW_EDGE", "SHOW_CONTOUR", "SHOW_FITLINE"
            };
            Require(
                adapterSource.Contains("internal static void ApplySampleProperty(", StringComparison.Ordinal)
                    && mappedMembers.All(member => adapterSource.Contains("target." + member, StringComparison.Ordinal))
                    && adapterSource.Contains("new List<Rect>(source.CvROIS)", StringComparison.Ordinal)
                    && adapterSource.Contains("new List<Rect>(source.CvMASKS)", StringComparison.Ordinal),
                "The existing Line adapter should own the complete common/Line projection and defensive list copies.");
            passed.Add("Line adapter contains the complete sample projection and defensive ROI/mask copies");

            Require(
                documentSource.Contains("VisionPipelineLinePropertyAdapter.TryCreateLineGaugePair", StringComparison.Ordinal)
                    && documentSource.Contains("lineView.ApplySampleLinePair", StringComparison.Ordinal),
                "Native document should retain the existing Line sample dispatch path.");
            Require(
                factorySource.Contains("new LineToolPresenter(", StringComparison.Ordinal)
                    && factorySource.Contains("OpenVisionNativeToolPropertySessionStore.Save(\"Line(L)_1\"", StringComparison.Ordinal)
                    && factorySource.Contains("OpenVisionNativeToolPropertySessionStore.Save(\"Line(R)_1\"", StringComparison.Ordinal),
                "Line factory should retain the existing presenter, persistence, creation, and release composition.");
            passed.Add("Sample caller and composition/lifetime path remain unchanged");

            LineGaugeProperty source = CreateSourceProperty();
            LineGaugeProperty target = new LineGaugeProperty("Line_A");
            VisionPipelineLinePropertyAdapter.ApplySampleProperty(source, target);
            Require(
                target.PIXELPERMM == source.PIXELPERMM
                    && target.USE_THRESHOLD == source.USE_THRESHOLD
                    && target.THRESHOLD_TYPES == source.THRESHOLD_TYPES
                    && target.THRESHOLD == source.THRESHOLD
                    && target.ADAPTIVE_THRESHOLD_ALGORITHM == source.ADAPTIVE_THRESHOLD_ALGORITHM
                    && target.PRJ_DIR == source.PRJ_DIR
                    && target.VER_PRJ_DIR == source.VER_PRJ_DIR
                    && target.CONTRAST == source.CONTRAST
                    && target.MANUAL_ANGLE_VALUE == source.MANUAL_ANGLE_VALUE
                    && target.SHOW_FITLINE == source.SHOW_FITLINE,
                "The adapter should preserve representative common, threshold, scan, edge, and draw values.");
            Require(
                !ReferenceEquals(source.CvROIS, target.CvROIS)
                    && !ReferenceEquals(source.CvMASKS, target.CvMASKS)
                    && target.CvROIS.Count == 1
                    && target.CvMASKS.Count == 1,
                "The adapter should preserve defensive list ownership for ROI and mask values.");
            source.CvROIS.Add(new Rect(20, 21, 22, 23));
            source.CvMASKS.Clear();
            Require(
                target.CvROIS.Count == 1 && target.CvMASKS.Count == 1,
                "Later source list mutation must not alias the target property.");
            passed.Add("Runtime projection preserves values and list ownership without View state");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }

        string outputPath = Path.Combine(evidenceDirectory, "line-tool-partial-boundary-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: Line Tool View Partial sample-property boundary",
                "EvidenceDirectory: " + evidenceDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            "LINE_TOOL_PARTIAL_BOUNDARY_CONTRACT|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static LineGaugeProperty CreateSourceProperty()
    {
        return new LineGaugeProperty("Sample_Line")
        {
            PIXELPERMM = 2.5D,
            USE_THRESHOLD = false,
            USE_BITWISENOT = true,
            THRESHOLD_TYPES = OpenCvSharp.ThresholdTypes.BinaryInv,
            THRESHOLD = 42D,
            USE_ADAPTIVE_THRESHOLD = true,
            ADAPTIVE_THRESHOLD = 77D,
            ADAPTIVE_THRESHOLD_TYPES = OpenCvSharp.ThresholdTypes.BinaryInv,
            ADAPTIVE_THRESHOLD_ALGORITHM = OpenCvSharp.AdaptiveThresholdTypes.MeanC,
            BlockSize = 17,
            Weight = -4,
            USE_ROI = false,
            CvROI = new Rect(1, 2, 30, 40),
            USE_MULTI_ROI = true,
            CvROIS = new List<Rect> { new Rect(3, 4, 5, 6) },
            USE_MASKING = true,
            CvMASKS = new List<Rect> { new Rect(7, 8, 9, 10) },
            PRJ_PORALITY = FormulaUtil.PROJECTION_POLARITY.WTOB,
            PRJ_DIR = FormulaUtil.PROJECTION_DIR.X_RTOL,
            CONTRAST = 31D,
            THICKNESS = 6D,
            SAMPLING_STEP = 11D,
            VER_PRJ_DIR = FormulaUtil.PROJECTION_DIR.Y_BTOT,
            POINT_RANGE = 12,
            USE_MANUAL_ANGLE = true,
            MANUAL_ANGLE_VALUE = -15D,
            USE_EXTEND_FIT_LINE = true,
            EXTEND_FIT_LINE_VALUE = 140,
            AVERAGE_Diff = 101D,
            USE_AVERAGE_FILTER = true,
            AVERAGE_FILTER_TYPE = LineGaugeProperty.AVERAGE_FILTER_TYPES.X,
            SHOW_VERTICAL_LINE = false,
            SHOW_EDGE = false,
            SHOW_CONTOUR = true,
            SHOW_FITLINE = false
        };
    }

    private static int Count(string source, string value)
    {
        return source.Split(new[] { value }, StringSplitOptions.None).Length - 1;
    }

    private static string Read(string repositoryRoot, string relativePath)
    {
        return File.ReadAllText(Path.Combine(repositoryRoot, relativePath));
    }

    private static string ResolveRepositoryRoot()
    {
        DirectoryInfo? current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("OpenVisionLab repository root could not be located.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
