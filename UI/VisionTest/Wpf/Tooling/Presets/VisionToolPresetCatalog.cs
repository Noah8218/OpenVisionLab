using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal static class VisionToolPresetCatalog
    {
        public static IReadOnlyList<VisionToolPreset<TProperty>> GetPropertyGridPresets<TProperty>()
        {
            Type propertyType = typeof(TProperty);
            if (propertyType == typeof(BlobProperty))
            {
                return CastPresets<TProperty, BlobProperty>(CreateBlobPresets());
            }

            if (propertyType == typeof(ContourProperty))
            {
                return CastPresets<TProperty, ContourProperty>(CreateContourPresets());
            }

            return Array.Empty<VisionToolPreset<TProperty>>();
        }

        public static IReadOnlyList<VisionToolPreset<TProperty>> GetMatchingPresets<TProperty>()
        {
            Type propertyType = typeof(TProperty);
            if (propertyType == typeof(MatchingProperty))
            {
                return CastPresets<TProperty, MatchingProperty>(CreateTemplateMatchingPresets());
            }

            if (propertyType == typeof(EdgeBasedMatchingProperty))
            {
                return CastPresets<TProperty, EdgeBasedMatchingProperty>(CreateEdgeBasedMatchingPresets());
            }

            if (propertyType == typeof(FeatureMatchingProperty))
            {
                return CastPresets<TProperty, FeatureMatchingProperty>(CreateFeatureMatchingPresets());
            }

            return Array.Empty<VisionToolPreset<TProperty>>();
        }

        public static IReadOnlyList<VisionToolPreset<LineGaugeProperty>> GetLinePresets()
        {
            return CreateLinePresets();
        }

        private static IReadOnlyList<VisionToolPreset<BlobProperty>> CreateBlobPresets()
        {
            return new[]
            {
                new VisionToolPreset<BlobProperty>(
                    "basic",
                    "VisionTool.Preset.Basic",
                    "Basic",
                    "VisionTool.Preset.Blob.Basic.Description",
                    "Start with thresholded bright/dark regions and a medium area filter.",
                    property =>
                    {
                        property.USE_THRESHOLD = true;
                        property.USE_ADAPTIVE_THRESHOLD = false;
                        property.THRESHOLD_TYPES = ThresholdTypes.Binary;
                        property.THRESHOLD = 100D;
                        property.USE_BITWISENOT = false;
                        property.MIN_AREA = 200;
                        property.MAX_AREA = 1000000;
                        property.USE_MASKING = false;
                    }),
                new VisionToolPreset<BlobProperty>(
                    "fast",
                    "VisionTool.Preset.Fast",
                    "Fast",
                    "VisionTool.Preset.Blob.Fast.Description",
                    "Ignore small noise with a larger area floor and simple thresholding.",
                    property =>
                    {
                        property.USE_THRESHOLD = true;
                        property.USE_ADAPTIVE_THRESHOLD = false;
                        property.THRESHOLD_TYPES = ThresholdTypes.Binary;
                        property.THRESHOLD = 120D;
                        property.USE_BITWISENOT = false;
                        property.MIN_AREA = 500;
                        property.MAX_AREA = 1000000;
                        property.USE_MASKING = false;
                    }),
                new VisionToolPreset<BlobProperty>(
                    "precise",
                    "VisionTool.Preset.Precise",
                    "Precise",
                    "VisionTool.Preset.Blob.Precise.Description",
                    "Keep smaller regions and enable adaptive thresholding for uneven lighting.",
                    property =>
                    {
                        property.USE_THRESHOLD = false;
                        property.USE_ADAPTIVE_THRESHOLD = true;
                        property.ADAPTIVE_THRESHOLD_TYPES = ThresholdTypes.Binary;
                        property.ADAPTIVE_THRESHOLD_ALGORITHM = AdaptiveThresholdTypes.GaussianC;
                        property.BlockSize = 25;
                        property.Weight = 5;
                        property.USE_BITWISENOT = false;
                        property.MIN_AREA = 80;
                        property.MAX_AREA = 1000000;
                        property.USE_MASKING = false;
                    })
            };
        }

        private static IReadOnlyList<VisionToolPreset<ContourProperty>> CreateContourPresets()
        {
            return new[]
            {
                new VisionToolPreset<ContourProperty>(
                    "basic",
                    "VisionTool.Preset.Basic",
                    "Basic",
                    "VisionTool.Preset.Contour.Basic.Description",
                    "External outlines with a medium area filter for first contour checks.",
                    property =>
                    {
                        property.USE_THRESHOLD = true;
                        property.USE_ADAPTIVE_THRESHOLD = false;
                        property.THRESHOLD_TYPES = ThresholdTypes.Binary;
                        property.THRESHOLD = 100D;
                        property.USE_BITWISENOT = false;
                        property.DetectMode = RetrievalModes.External;
                        property.ApproximationModes = ContourApproximationModes.ApproxSimple;
                        property.USE_APPROXPOLYDP = false;
                        property.DrawMode = ContourDrawMode.Outline;
                        property.DrawThickness = 2;
                        property.MIN_AREA = 200;
                        property.MAX_AREA = 1000000;
                        property.USE_MASKING = false;
                    }),
                new VisionToolPreset<ContourProperty>(
                    "fast",
                    "VisionTool.Preset.Fast",
                    "Fast",
                    "VisionTool.Preset.Contour.Fast.Description",
                    "External simple contours with a larger area floor to skip noise quickly.",
                    property =>
                    {
                        property.USE_THRESHOLD = true;
                        property.USE_ADAPTIVE_THRESHOLD = false;
                        property.THRESHOLD_TYPES = ThresholdTypes.Binary;
                        property.THRESHOLD = 120D;
                        property.USE_BITWISENOT = false;
                        property.DetectMode = RetrievalModes.External;
                        property.ApproximationModes = ContourApproximationModes.ApproxSimple;
                        property.USE_APPROXPOLYDP = false;
                        property.DrawMode = ContourDrawMode.Outline;
                        property.DrawThickness = 2;
                        property.MIN_AREA = 500;
                        property.MAX_AREA = 1000000;
                        property.USE_MASKING = false;
                    }),
                new VisionToolPreset<ContourProperty>(
                    "precise",
                    "VisionTool.Preset.Precise",
                    "Precise",
                    "VisionTool.Preset.Contour.Precise.Description",
                    "Smaller contours with polygon approximation for final shape tuning.",
                    property =>
                    {
                        property.USE_THRESHOLD = true;
                        property.USE_ADAPTIVE_THRESHOLD = false;
                        property.THRESHOLD_TYPES = ThresholdTypes.Binary;
                        property.THRESHOLD = 90D;
                        property.USE_BITWISENOT = false;
                        property.DetectMode = RetrievalModes.External;
                        property.ApproximationModes = ContourApproximationModes.ApproxSimple;
                        property.USE_APPROXPOLYDP = true;
                        property.EPSILON = 0.005D;
                        property.DrawMode = ContourDrawMode.Outline;
                        property.DrawThickness = 2;
                        property.MIN_AREA = 80;
                        property.MAX_AREA = 1000000;
                        property.USE_MASKING = false;
                    })
            };
        }

        private static IReadOnlyList<VisionToolPreset<LineGaugeProperty>> CreateLinePresets()
        {
            return new[]
            {
                new VisionToolPreset<LineGaugeProperty>(
                    "basic",
                    "VisionTool.Preset.Basic",
                    "Basic",
                    "VisionTool.Preset.Line.Basic.Description",
                    "Balanced contrast and scan interval for the selected Line A/B.",
                    property =>
                    {
                        ApplyLineCommon(property);
                        property.CONTRAST = 30D;
                        property.THICKNESS = 5D;
                        property.SAMPLING_STEP = 10D;
                        property.POINT_RANGE = 10;
                        property.USE_MANUAL_ANGLE = false;
                        property.USE_EXTEND_FIT_LINE = false;
                        property.USE_AVERAGE_FILTER = false;
                    }),
                new VisionToolPreset<LineGaugeProperty>(
                    "fast",
                    "VisionTool.Preset.Fast",
                    "Fast",
                    "VisionTool.Preset.Line.Fast.Description",
                    "Higher contrast and wider scan interval for quick edge checks.",
                    property =>
                    {
                        ApplyLineCommon(property);
                        property.CONTRAST = 45D;
                        property.THICKNESS = 4D;
                        property.SAMPLING_STEP = 16D;
                        property.POINT_RANGE = 16;
                        property.USE_MANUAL_ANGLE = false;
                        property.USE_EXTEND_FIT_LINE = false;
                        property.USE_AVERAGE_FILTER = false;
                    }),
                new VisionToolPreset<LineGaugeProperty>(
                    "precise",
                    "VisionTool.Preset.Precise",
                    "Precise",
                    "VisionTool.Preset.Line.Precise.Description",
                    "Lower contrast, denser scan lines, fit-line extension, and average filtering for final tuning.",
                    property =>
                    {
                        ApplyLineCommon(property);
                        property.CONTRAST = 20D;
                        property.THICKNESS = 3D;
                        property.SAMPLING_STEP = 4D;
                        property.POINT_RANGE = 4;
                        property.USE_MANUAL_ANGLE = false;
                        property.USE_EXTEND_FIT_LINE = true;
                        property.EXTEND_FIT_LINE_VALUE = 150;
                        property.USE_AVERAGE_FILTER = true;
                        property.AVERAGE_Diff = 80D;
                        property.AVERAGE_FILTER_TYPE = LineGaugeProperty.AVERAGE_FILTER_TYPES.Y;
                    })
            };
        }

        private static void ApplyLineCommon(LineGaugeProperty property)
        {
            if (property == null)
            {
                return;
            }

            property.USE_THRESHOLD = false;
            property.USE_ADAPTIVE_THRESHOLD = false;
            property.USE_BITWISENOT = false;
            property.SHOW_VERTICAL_LINE = true;
            property.SHOW_EDGE = true;
            property.SHOW_CONTOUR = true;
            property.SHOW_FITLINE = true;
        }

        private static IReadOnlyList<VisionToolPreset<TProperty>> CastPresets<TProperty, TSource>(IReadOnlyList<VisionToolPreset<TSource>> presets)
        {
            VisionToolPreset<TProperty>[] result = new VisionToolPreset<TProperty>[presets.Count];
            for (int i = 0; i < presets.Count; i++)
            {
                result[i] = (VisionToolPreset<TProperty>)(object)presets[i];
            }

            return result;
        }

        private static IReadOnlyList<VisionToolPreset<MatchingProperty>> CreateTemplateMatchingPresets()
        {
            return new[]
            {
                new VisionToolPreset<MatchingProperty>(
                    "basic",
                    "VisionTool.Preset.Basic",
                    "Basic",
                    "VisionTool.Preset.Matching.Basic.Description",
                    "One best match, no angle or scale search. Good first check for fixed parts.",
                    property =>
                    {
                        property.AUTO_PREVIEW = false;
                        property.MATCH_MODE = TemplateMatchModes.CCoeffNormed;
                        property.SCORE_MIN = 0.6D;
                        property.NUM_MATCH = 1;
                        property.MAGNIFIATION = 1.0D;
                        property.USE_FIND_ANGLE = false;
                        property.USE_COARSE_TO_FINE_ANGLE_SEARCH = false;
                        property.USE_FIND_SCALE = false;
                        property.USE_PYRAMID_POSITION_PROPOSAL = false;
                        property.USE_CANNY = false;
                    }),
                new VisionToolPreset<MatchingProperty>(
                    "fast",
                    "VisionTool.Preset.Fast",
                    "Fast",
                    "VisionTool.Preset.Matching.Fast.Description",
                    "Lower work scale and one strict match. Use for quick screening before tuning.",
                    property =>
                    {
                        property.AUTO_PREVIEW = false;
                        property.MATCH_MODE = TemplateMatchModes.CCoeffNormed;
                        property.SCORE_MIN = 0.7D;
                        property.NUM_MATCH = 1;
                        property.MAGNIFIATION = 0.5D;
                        property.USE_FIND_ANGLE = false;
                        property.USE_COARSE_TO_FINE_ANGLE_SEARCH = false;
                        property.USE_FIND_SCALE = false;
                        property.USE_PYRAMID_POSITION_PROPOSAL = false;
                        property.USE_CANNY = false;
                    }),
                new VisionToolPreset<MatchingProperty>(
                    "precise",
                    "VisionTool.Preset.Precise",
                    "Precise",
                    "VisionTool.Preset.Matching.Precise.Description",
                    "Three candidates with a narrow angle search. Use after template position is stable.",
                    property =>
                    {
                        property.AUTO_PREVIEW = false;
                        property.MATCH_MODE = TemplateMatchModes.CCoeffNormed;
                        property.SCORE_MIN = 0.6D;
                        property.NUM_MATCH = 3;
                        property.MAGNIFIATION = 1.0D;
                        property.USE_FIND_ANGLE = true;
                        property.FIND_ANGLE_MIN = -10;
                        property.FIND_ANGLE_MAX = 10;
                        property.FIND_ANGLE = 0.5D;
                        property.USE_COARSE_TO_FINE_ANGLE_SEARCH = false;
                        property.USE_FIND_SCALE = false;
                        property.USE_PYRAMID_POSITION_PROPOSAL = false;
                        property.USE_CANNY = false;
                    })
            };
        }

        private static IReadOnlyList<VisionToolPreset<EdgeBasedMatchingProperty>> CreateEdgeBasedMatchingPresets()
        {
            return new[]
            {
                new VisionToolPreset<EdgeBasedMatchingProperty>(
                    "basic",
                    "VisionTool.Preset.Basic",
                    "Basic",
                    "VisionTool.Preset.EdgeMatching.Basic.Description",
                    "One edge match with safe Canny and 2 px search step.",
                    property =>
                    {
                        property.SCORE_MIN = 0.75D;
                        property.NUM_MATCH = 1;
                        property.CANNY_LOW = 30;
                        property.CANNY_HIGH = 90;
                        property.MAX_TEMPLATE_POINTS = 300;
                        property.USE_FIND_ANGLE = false;
                        property.USE_FIND_SCALE = false;
                        property.SEARCH_STEP = 2;
                        property.USE_POSITION_REFINE = false;
                        property.USE_SUBPIXEL_REFINE = false;
                        property.GREEDINESS = 0.9D;
                        property.USE_PYRAMID_POSITION_PROPOSAL = false;
                        property.USE_HYBRID_VERIFY = false;
                        property.USE_UNIQUE_MATCH_VALIDATION = false;
                    }),
                new VisionToolPreset<EdgeBasedMatchingProperty>(
                    "fast",
                    "VisionTool.Preset.Fast",
                    "Fast",
                    "VisionTool.Preset.EdgeMatching.Fast.Description",
                    "Fewer model points, coarse search, and strict score for quick screening.",
                    property =>
                    {
                        property.SCORE_MIN = 0.8D;
                        property.NUM_MATCH = 1;
                        property.CANNY_LOW = 35;
                        property.CANNY_HIGH = 100;
                        property.MAX_TEMPLATE_POINTS = 180;
                        property.USE_FIND_ANGLE = false;
                        property.USE_FIND_SCALE = false;
                        property.SEARCH_STEP = 4;
                        property.USE_POSITION_REFINE = true;
                        property.USE_SUBPIXEL_REFINE = false;
                        property.GREEDINESS = 0.93D;
                        property.USE_PYRAMID_POSITION_PROPOSAL = true;
                        property.PYRAMID_POSITION_TOP_N = 4;
                        property.USE_HYBRID_VERIFY = false;
                        property.USE_UNIQUE_MATCH_VALIDATION = false;
                    }),
                new VisionToolPreset<EdgeBasedMatchingProperty>(
                    "precise",
                    "VisionTool.Preset.Precise",
                    "Precise",
                    "VisionTool.Preset.EdgeMatching.Precise.Description",
                    "More edge points, 1 px search, narrow angle scan, and hybrid verification.",
                    property =>
                    {
                        property.SCORE_MIN = 0.7D;
                        property.NUM_MATCH = 3;
                        property.CANNY_LOW = 25;
                        property.CANNY_HIGH = 80;
                        property.MAX_TEMPLATE_POINTS = 500;
                        property.USE_FIND_ANGLE = true;
                        property.FIND_ANGLE_MIN = -10;
                        property.FIND_ANGLE_MAX = 10;
                        property.FIND_ANGLE = 0.5D;
                        property.USE_FIND_SCALE = false;
                        property.SEARCH_STEP = 1;
                        property.USE_POSITION_REFINE = true;
                        property.USE_SUBPIXEL_REFINE = true;
                        property.GREEDINESS = 0.85D;
                        property.USE_PYRAMID_POSITION_PROPOSAL = false;
                        property.USE_HYBRID_VERIFY = true;
                        property.HYBRID_VERIFY_TOP_N = 5;
                        property.USE_UNIQUE_MATCH_VALIDATION = false;
                    })
            };
        }

        private static IReadOnlyList<VisionToolPreset<FeatureMatchingProperty>> CreateFeatureMatchingPresets()
        {
            return new[]
            {
                new VisionToolPreset<FeatureMatchingProperty>(
                    "basic",
                    "VisionTool.Preset.Basic",
                    "Basic",
                    "VisionTool.Preset.FeatureMatching.Basic.Description",
                    "Balanced ratio and RANSAC tolerance for first verification.",
                    property =>
                    {
                        property.SCORE_MIN = 0.6D;
                        property.RANSAC_REPROJ_THRESHOLD = 3D;
                    }),
                new VisionToolPreset<FeatureMatchingProperty>(
                    "fast",
                    "VisionTool.Preset.Fast",
                    "Fast",
                    "VisionTool.Preset.FeatureMatching.Fast.Description",
                    "Looser feature ratio with wider geometry tolerance for quick checks.",
                    property =>
                    {
                        property.SCORE_MIN = 0.7D;
                        property.RANSAC_REPROJ_THRESHOLD = 4D;
                    }),
                new VisionToolPreset<FeatureMatchingProperty>(
                    "precise",
                    "VisionTool.Preset.Precise",
                    "Precise",
                    "VisionTool.Preset.FeatureMatching.Precise.Description",
                    "Stricter feature ratio with tighter geometry validation for final tuning.",
                    property =>
                    {
                        property.SCORE_MIN = 0.55D;
                        property.RANSAC_REPROJ_THRESHOLD = 2D;
                    })
            };
        }
    }
}
