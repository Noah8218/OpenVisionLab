using System;
using System.Collections.Generic;
using System.Linq;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    /// <summary>
    /// Learn window topic index map shared by OpenVisionLearnWindow and Tool Learn button entry points.
    /// Values map directly to OpenVisionLearnWindow topic switch indexes.
    /// Some tool areas intentionally share a topic (for example Mean and Histogram both use Brightness + histogram coverage).
    /// </summary>
    public enum OpenVisionLearnTopicIndex : int
    {
        Curriculum = 0,
        BrightnessAndHistogram = 1,
        Mean = BrightnessAndHistogram,
        Histogram = BrightnessAndHistogram,
        Threshold = 2,
        Filtering = 3,
        Morphology = 4,
        Blob = 5,
        Contour = 6,
        EdgeDetection = 7,
        LineDistance = 8,
        Matching = 9,
        FeatureMatching = 10,
        LayerRecipe = 11,
        EdgeBasedMatching = 12,
        MetricsAcceptance = 13,
        Arithmetic = 14,
        GeometryTransform = 15,
        ColorHsv = 16,
    }

    public sealed class OpenVisionLearnTopicMetadata
    {
        public OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex index, string document, string practicePathId, string practiceText, string title = null)
        {
            Index = index;
            Title = title ?? GetTopicTitle(index);
            Document = document;
            PracticePathId = practicePathId;
            PracticeText = practiceText;
        }

        private static string GetTopicTitle(OpenVisionLearnTopicIndex index)
        {
            return index switch
            {
                OpenVisionLearnTopicIndex.Curriculum => "Curriculum / Learn Overview",
                OpenVisionLearnTopicIndex.BrightnessAndHistogram => "Mean / Histogram",
                OpenVisionLearnTopicIndex.Threshold => "Threshold",
                OpenVisionLearnTopicIndex.Filtering => "Filtering",
                OpenVisionLearnTopicIndex.Morphology => "Morphology",
                OpenVisionLearnTopicIndex.Blob => "Blob",
                OpenVisionLearnTopicIndex.Contour => "Contour",
                OpenVisionLearnTopicIndex.EdgeDetection => "Edge Detection",
                OpenVisionLearnTopicIndex.LineDistance => "Line Distance",
                OpenVisionLearnTopicIndex.Matching => "Matching",
                OpenVisionLearnTopicIndex.FeatureMatching => "Feature Matching",
                OpenVisionLearnTopicIndex.LayerRecipe => "Layer / Pipeline / Recipe",
                OpenVisionLearnTopicIndex.EdgeBasedMatching => "EdgeBasedMatching",
                OpenVisionLearnTopicIndex.MetricsAcceptance => "Metrics / Acceptance",
                OpenVisionLearnTopicIndex.Arithmetic => "Arithmetic / Logic",
                OpenVisionLearnTopicIndex.GeometryTransform => "Geometry Transform",
                OpenVisionLearnTopicIndex.ColorHsv => "Color / HSV",
                _ => index.ToString()
            };
        }

        public OpenVisionLearnTopicIndex Index { get; }

        public string Title { get; }

        public string Document { get; }

        public string PracticePathId { get; }

        public string PracticeText { get; }
    }

    public static class OpenVisionLearnTopicCatalog
    {
        private static readonly IReadOnlyList<OpenVisionLearnTopicMetadata> All = new List<OpenVisionLearnTopicMetadata>
        {
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.Curriculum, "OPENVISIONLAB_LEARN_CURRICULUM.md", "all",
                "실습: Good/Bad 샘플을 하나씩 열어 입력 이미지, 처리 결과, 핵심 지표가 어떻게 연결되는지 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.BrightnessAndHistogram, "LEARN_MEAN.md", "mean",
                "실습: Mean과 Histogram 샘플에서 평균 GV와 명암 분포가 Good/Bad 사이에 어떻게 달라지는지 확인하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.Threshold, "LEARN_THRESHOLD.md", "threshold",
                "실습: Public_Threshold_BandPads_Good와 Public_Threshold_BandPads_Missing_Bad를 같은 Pipeline으로 열고, 명시적 Run Review에서 ResultCount 4와 1을 비교한 뒤 Threshold 기준 GV를 조정해 Preview 차이를 확인하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.Filtering, "LEARN_FILTER.md", "filter",
                "실습: Public_Filter_Denoise_Good와 Public_Filter_Denoise_Missing_Bad를 같은 Pipeline으로 열고, ResultCount 4는 OK, 2는 NG인지 비교하세요. Filter Tool에서 Median Kernel 5를 확인한 뒤 Preview 또는 Pipeline Review를 직접 실행해 노이즈 제거 전후를 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.Morphology, "LEARN_MORPHOLOGY.md", "morphology",
                "실습: Public_Morphology_Cleanup_Good와 Public_Morphology_Cleanup_Missing_Bad를 같은 Pipeline으로 열고, ResultCount 4는 OK, 2는 NG인지 비교하세요. Morphology Tool에서 Open, Rect, 5×5 Kernel을 확인한 뒤 Preview 또는 Pipeline Review를 직접 실행해 작은 노이즈와 최종 개수를 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.Blob, "LEARN_BLOB.md", "blob",
                "실습: Public_Blob_Particles_Good와 Public_Blob_Particles_Sparse_Bad를 같은 Pipeline으로 열고, ResultCount 8..14는 OK, 2..4는 NG인지 비교하세요. Blob Tool에서 MIN_AREA와 MAX_AREA를 확인한 뒤 Preview 또는 Pipeline Review를 직접 실행해 후보 box와 개수를 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.Contour, "LEARN_CONTOUR.md", "contour",
                "실습: Public_Contour_Shapes_Good와 Public_Contour_Shapes_Missing_Bad를 같은 Pipeline으로 열고, ResultCount 5는 OK, 2는 NG인지 비교하세요. Contour Tool에서 Retrieval mode, MIN_AREA, MAX_AREA, 컨투어 표시를 확인한 뒤 Preview 또는 Pipeline Review를 직접 실행해 외곽선과 개수를 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.EdgeDetection, "LEARN_EDGE_DETECTION.md", "edge-detection",
                "실습: Public_EdgeDetection_Shapes_Good와 Public_EdgeDetection_Shapes_Missing_Bad를 같은 Pipeline으로 열고, 최종 ResultCount 4는 OK, 2는 NG인지 비교하세요. EdgeDetection Tool에서 Canny Low/High와 L2 Gradient를 확인한 뒤 Preview 또는 Pipeline Review를 직접 실행해 edge map과 downstream Contour 결과를 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.LineDistance, "LEARN_LINE.md", "line",
                "실습: Public_Line_Pins_Good와 Public_Line_Pins_WidePin_Bad를 같은 Pipeline으로 열고, DistanceMmRange 0.03 이하를 먼저 확인한 뒤 DistanceMmAvg 0.20..0.25를 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.Matching, "LEARN_MATCHING.md", "template-matching",
                "실습: Matching 샘플에서 Template 위치, ScoreMax, ResultCount가 Good/Bad를 구분하는지 확인하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.FeatureMatching, "LEARN_FEATURE_MATCHING.md", "feature-matching",
                "실습: FeatureMatching 샘플에서 GoodMatches, ScoreMax, 오버레이 위치를 함께 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.LayerRecipe, "LEARN_PIPELINE_LAYER_ROUTING.md", "all",
                "실습: Pipeline Review에서 각 Step의 InputLayer와 OutputLayer를 따라가며 결과 이미지가 이어지는 순서를 확인하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.EdgeBasedMatching, "LEARN_EDGE_BASED_MATCHING.md", "edge-matching",
                "실습: EdgeBasedMatching 샘플에서 edge 오버레이, ScoreMax, ResultCount를 Good/Bad와 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.MetricsAcceptance, "LEARN_METRICS_ACCEPTANCE.md", "all",
                "실습: Metrics/Acceptance 기준이 Good은 통과시키고 Bad는 의도한 지표로 거부하는지 확인하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.Arithmetic, "LEARN_ARITHMETIC.md", "preprocess",
                "실습: Arithmetic에서 Add, Subtract, AbsDiff, Bitwise 연산의 입력 A/B와 결과 레이어를 비교하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.GeometryTransform, "LEARN_GEOMETRY_TRANSFORM.md", "geometry",
                "실습: RotateScale 또는 AffineTransform을 Preview하고 OutputSize, 변환 드로잉, valid-pixel ratio, ROI 좌표 변화를 확인하세요."),
            new OpenVisionLearnTopicMetadata(OpenVisionLearnTopicIndex.ColorHsv, "LEARN_COLOR_HSV.md", "color-hsv",
                "실습: HSV 색상 샘플에서 Hue/Saturation/Value 범위와 MaskPixelRatio가 Good/Bad를 구분하는지 확인하세요.")
        };

        public static IReadOnlyList<string> TopicTitles => All.Select(topic => topic.Title).ToArray();

        public static string ResolveTitle(int index)
        {
            return Resolve(index).Title;
        }

        public static OpenVisionLearnTopicMetadata Resolve(int index)
        {
            return Resolve((OpenVisionLearnTopicIndex)NormalizeTopicIndex(index));
        }

        public static OpenVisionLearnTopicMetadata Resolve(OpenVisionLearnTopicIndex index)
        {
            int idx = Math.Clamp((int)index, 0, All.Count - 1);
            OpenVisionLearnTopicMetadata topic = All[idx];
            return topic;
        }

        public static int NormalizeTopicIndex(int index)
        {
            int maxTopicIndex = (int)OpenVisionLearnTopicIndex.ColorHsv;
            return index >= (int)OpenVisionLearnTopicIndex.Curriculum && index <= maxTopicIndex ? index : (int)OpenVisionLearnTopicIndex.Threshold;
        }

        public static bool TryResolveForTool(VISION_MENU menu, out OpenVisionLearnTopicIndex topicIndex)
        {
            switch (menu)
            {
                case VISION_MENU.Morphology:
                    topicIndex = OpenVisionLearnTopicIndex.Morphology;
                    return true;
                case VISION_MENU.Filter:
                    topicIndex = OpenVisionLearnTopicIndex.Filtering;
                    return true;
                case VISION_MENU.Arithmetic:
                    topicIndex = OpenVisionLearnTopicIndex.Arithmetic;
                    return true;
                case VISION_MENU.EdgeDetection:
                    topicIndex = OpenVisionLearnTopicIndex.EdgeDetection;
                    return true;
                case VISION_MENU.Blob:
                    topicIndex = OpenVisionLearnTopicIndex.Blob;
                    return true;
                case VISION_MENU.Contour:
                    topicIndex = OpenVisionLearnTopicIndex.Contour;
                    return true;
                case VISION_MENU.Matching:
                    topicIndex = OpenVisionLearnTopicIndex.Matching;
                    return true;
                case VISION_MENU.EdgeBasedMatching:
                    topicIndex = OpenVisionLearnTopicIndex.EdgeBasedMatching;
                    return true;
                case VISION_MENU.Line:
                    topicIndex = OpenVisionLearnTopicIndex.LineDistance;
                    return true;
                case VISION_MENU.RotateAndScale:
                case VISION_MENU.AffineTransform:
                    topicIndex = OpenVisionLearnTopicIndex.GeometryTransform;
                    return true;
                case VISION_MENU.Histogram:
                    topicIndex = OpenVisionLearnTopicIndex.Histogram;
                    return true;
                case VISION_MENU.Mean:
                    topicIndex = OpenVisionLearnTopicIndex.Mean;
                    return true;
                case VISION_MENU.HSV:
                    topicIndex = OpenVisionLearnTopicIndex.ColorHsv;
                    return true;
                case VISION_MENU.FeatureMatching:
                    topicIndex = OpenVisionLearnTopicIndex.FeatureMatching;
                    return true;
                case VISION_MENU.Pipeline:
                    topicIndex = OpenVisionLearnTopicIndex.LayerRecipe;
                    return true;
                case VISION_MENU.Threshold:
                    topicIndex = OpenVisionLearnTopicIndex.Threshold;
                    return true;
                default:
                    topicIndex = OpenVisionLearnTopicIndex.Curriculum;
                    return false;
            }
        }

        public static bool TryResolveForToolType(string toolType, out OpenVisionLearnTopicIndex topicIndex)
        {
            switch ((toolType ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "threshold":
                    topicIndex = OpenVisionLearnTopicIndex.Threshold;
                    return true;
                case "filter":
                    topicIndex = OpenVisionLearnTopicIndex.Filtering;
                    return true;
                case "morphology":
                    topicIndex = OpenVisionLearnTopicIndex.Morphology;
                    return true;
                case "arithmetic":
                    topicIndex = OpenVisionLearnTopicIndex.Arithmetic;
                    return true;
                case "edgedetection":
                    topicIndex = OpenVisionLearnTopicIndex.EdgeDetection;
                    return true;
                case "blob":
                    topicIndex = OpenVisionLearnTopicIndex.Blob;
                    return true;
                case "contour":
                    topicIndex = OpenVisionLearnTopicIndex.Contour;
                    return true;
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                case "lineintersection":
                case "lineintersectiongauge":
                    topicIndex = OpenVisionLearnTopicIndex.LineDistance;
                    return true;
                case "matching":
                case "templatematching":
                    topicIndex = OpenVisionLearnTopicIndex.Matching;
                    return true;
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    topicIndex = OpenVisionLearnTopicIndex.EdgeBasedMatching;
                    return true;
                case "feature":
                case "featurematching":
                case "sift":
                    topicIndex = OpenVisionLearnTopicIndex.FeatureMatching;
                    return true;
                case "mean":
                case "histogram":
                    topicIndex = OpenVisionLearnTopicIndex.BrightnessAndHistogram;
                    return true;
                case "rotatescale":
                case "rotateandscale":
                case "affine":
                case "affinematrix":
                case "affinetransform":
                    topicIndex = OpenVisionLearnTopicIndex.GeometryTransform;
                    return true;
                case "hsv":
                    topicIndex = OpenVisionLearnTopicIndex.ColorHsv;
                    return true;
                default:
                    topicIndex = OpenVisionLearnTopicIndex.Curriculum;
                    return false;
            }
        }
    }
}
