using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class PixelPerMmFiniteUnitContract
{
    public static int Run(string evidenceDirectory)
    {
        evidenceDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();

        RunCase("100 px at 0.01 mm/px converts every declared metric family", () =>
        {
            VisionPipelineStep step = CreateStep("0.01");
            Dictionary<string, double> metrics = VisionPipelineMetricEnrichmentService.CreateEnrichedMetrics(
                CreatePixelMetrics(),
                new[]
                {
                    new VisionToolOverlay
                    {
                        Kind = VisionToolOverlayKind.Rectangle,
                        Bounds = new RectangleF(0F, 0F, 100F, 100F)
                    },
                    new VisionToolOverlay
                    {
                        Kind = VisionToolOverlayKind.Line,
                        Start = new PointF(0F, 0F),
                        End = new PointF(100F, 0F)
                    }
                },
                step);

            AssertMetric(metrics, VisionPipelineKnownMetrics.BoundsWidthMmMin, 1D);
            AssertMetric(metrics, VisionPipelineKnownMetrics.BoundsHeightMmMax, 1D);
            AssertMetric(metrics, VisionPipelineKnownMetrics.LineLengthMmAvg, 1D);
            AssertMetric(metrics, VisionPipelineKnownMetrics.GeometryDistanceMm, 1D);
            AssertMetric(metrics, VisionPipelineKnownMetrics.GeometrySignedClearanceMm, -1D);
            AssertMetric(metrics, VisionPipelineKnownMetrics.CircleRadiusMm, 1D);
            AssertMetric(metrics, VisionPipelineKnownMetrics.CircleDiameterMm, 2D);
            AssertMetric(metrics, VisionPipelineKnownMetrics.DistanceMmAvg, 1D);
            AssertMetric(metrics, VisionPipelineKnownMetrics.CurveCenterArcLengthMm, 1D);
        }, passed, failed);

        RunCase("nonfinite, zero, and negative calibration publish no mm metrics", () =>
        {
            foreach (string scale in new[] { "NaN", "Infinity", "+Infinity", "-Infinity", "1E309", "0", "-0.01" })
            {
                Dictionary<string, double> metrics = VisionPipelineMetricEnrichmentService.CreateEnrichedMetrics(
                    CreatePixelMetrics(),
                    Array.Empty<VisionToolOverlay>(),
                    CreateStep(scale));
                Require(!metrics.Keys.Any(name => name.EndsWith("Mm", StringComparison.OrdinalIgnoreCase)
                    || name.Contains("Mm", StringComparison.OrdinalIgnoreCase)),
                    $"Invalid scale {scale} produced a millimeter metric.");
            }
        }, passed, failed);

        RunCase("finite scale overflow fails closed", () =>
        {
            Dictionary<string, double> metrics = VisionPipelineMetricEnrichmentService.CreateEnrichedMetrics(
                new Dictionary<string, double>
                {
                    [VisionPipelineKnownMetrics.GeometryDistancePx] = 2D,
                    [VisionPipelineKnownMetrics.DistancePxAvg] = 2D
                },
                Array.Empty<VisionToolOverlay>(),
                CreateStep(double.MaxValue.ToString("R", CultureInfo.InvariantCulture)));
            Require(!metrics.ContainsKey(VisionPipelineKnownMetrics.GeometryDistanceMm)
                && !metrics.ContainsKey(VisionPipelineKnownMetrics.DistanceMmAvg),
                "A finite scale whose multiplication overflows published an mm metric.");
        }, passed, failed);

        RunCase("validator rejects nonfinite or negative legacy scales", () =>
        {
            foreach (string scale in new[] { "NaN", "Infinity", "+Infinity", "-Infinity", "1E309", "-0.01" })
            {
                VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
                    CreatePipeline(scale),
                    new[] { "Main" });
                Require(validation.Errors.Any(error => error.Contains("PIXELPERMM", StringComparison.OrdinalIgnoreCase)),
                    $"Scale {scale} was not rejected by the shared validator.");
            }

            VisionPipelineValidationResult pixelOnly = VisionPipelineValidator.Validate(CreatePipeline("0"), new[] { "Main" });
            Require(!pixelOnly.Errors.Any(error => error.Contains("PIXELPERMM", StringComparison.OrdinalIgnoreCase)),
                "PIXELPERMM=0 no longer preserves the intentional pixel-only contract.");
        }, passed, failed);

        RunCase("legacy PIXELPERMM key survives XML round-trip", () =>
        {
            string path = Path.Combine(evidenceDirectory, "legacy-pixelpermm.xml");
            VisionPipeline pipeline = CreatePipeline("0.01");
            Require(SerializeHelper.SaveXmlFile(path, pipeline), "Legacy scale XML could not be saved.");
            Require(SerializeHelper.TryLoadFromXmlFile(path, out VisionPipeline loaded), "Legacy scale XML could not be reopened.");
            Require(loaded.Steps.Count == 1
                && loaded.Steps[0].Parameters.TryGetValue("PIXELPERMM", out string scale)
                && string.Equals(scale, "0.01", StringComparison.Ordinal),
                "The legacy PIXELPERMM key or value changed across XML round-trip.");
            Require(File.ReadAllText(path).Contains("PIXELPERMM", StringComparison.Ordinal),
                "Serialized XML did not retain the public legacy key.");
        }, passed, failed);

        string reportPath = Path.Combine(evidenceDirectory, "pixelpermm-finite-unit-contract.txt");
        File.WriteAllLines(
            reportPath,
            passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|pixelpermm-finite-unit|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static Dictionary<string, double> CreatePixelMetrics()
    {
        return new Dictionary<string, double>
        {
            [VisionPipelineKnownMetrics.GeometryDistancePx] = 100D,
            [VisionPipelineKnownMetrics.GeometrySignedClearancePx] = -100D,
            [VisionPipelineKnownMetrics.CircleRadiusPx] = 100D,
            [VisionPipelineKnownMetrics.CircleDiameterPx] = 200D,
            [VisionPipelineKnownMetrics.DistancePxMin] = 100D,
            [VisionPipelineKnownMetrics.DistancePxMax] = 100D,
            [VisionPipelineKnownMetrics.DistancePxAvg] = 100D,
            [VisionPipelineKnownMetrics.DistancePxRange] = 100D,
            [VisionPipelineKnownMetrics.CurveOuterArcLengthPx] = 100D,
            [VisionPipelineKnownMetrics.CurveInnerArcLengthPx] = 100D,
            [VisionPipelineKnownMetrics.CurveCenterArcLengthPx] = 100D
        };
    }

    private static VisionPipelineStep CreateStep(string scale)
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Scale contract",
            ToolType = "GeometryMeasure",
            InputLayer = "Main",
            OutputLayer = "Measured"
        };
        step.Parameters["PIXELPERMM"] = scale;
        return step;
    }

    private static VisionPipeline CreatePipeline(string scale)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "PixelPerMmContract" };
        pipeline.Steps.Add(CreateStep(scale));
        return pipeline;
    }

    private static void AssertMetric(IReadOnlyDictionary<string, double> metrics, string name, double expected)
    {
        Require(metrics.TryGetValue(name, out double actual), $"Metric {name} was not published.");
        Require(Math.Abs(actual - expected) <= 0.000000001D,
            $"Metric {name} expected {expected.ToString("R", CultureInfo.InvariantCulture)} but was {actual.ToString("R", CultureInfo.InvariantCulture)}.");
    }

    private static void RunCase(string name, Action action, List<string> passed, List<string> failed)
    {
        try
        {
            action();
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add($"{name}: {exception.GetBaseException().Message}");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
