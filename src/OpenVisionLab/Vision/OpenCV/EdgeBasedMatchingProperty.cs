using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using OpenVisionLab.Vision2D.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    [CategoryOrder("Parameter", 0)]
    [CategoryOrder("Matching", 1)]
    [CategoryOrder("Auto MPoint", 2)]
    [CategoryOrder("Edge Model", 3)]
    [CategoryOrder("Angle", 4)]
    [CategoryOrder("Scale", 5)]
    [CategoryOrder("Search", 6)]
    [CategoryOrder("ROI", 7)]
    [CategoryOrder("Threshold", 8)]
    [CategoryOrder("Image Process", 9)]
    [System.Xml.Serialization.XmlRoot("CPropertyEdgeBasedMatching")]
    public class EdgeBasedMatchingProperty : OpenCvPropertyBase, IOpenCVPropertyEdgeBasedTemplateMatching, IOpenCvConfigurableProperty<EdgeBasedMatchingProperty>
    {
        [PropertyOrder(0)]
        [PropertyEditor(typeof(WpgMatchEditor))]
        [Category("Matching"), Description("Registered reference template for edge matching. A rotated ROI template is saved as an upright 0-degree template before matching."), DisplayName("Pattern path")]
        public string PATTERN_PATH { get; set; } = "";

        [PropertyOrder(1)]
        [Category("Matching"), Description("Minimum edge-direction score accepted as OK. Higher values reduce false matches, but can miss weak or noisy targets. Start around 0.7 to 0.9."), DisplayName("Min score")]
        public double SCORE_MIN { get; set; } = 0.75D;

        [PropertyOrder(2)]
        [Category("Matching"), Description("Maximum number of accepted matches."), DisplayName("Match count")]
        public int NUM_MATCH { get; set; } = 1;

        [PropertyOrder(4)]
        [Category("Matching"), Description("Require exactly one spatially distinct candidate. This opt-in mode keeps internal Top-K candidates even though Match count must be 1, and returns Ambiguous with no match when the score-margin gate fails."), DisplayName("Require unique match")]
        public bool USE_UNIQUE_MATCH_VALIDATION { get; set; } = false;

        [PropertyOrder(5)]
        [Category("Matching"), Description("Minimum normalized 0..1 score difference between the selected candidate and the strongest spatially distinct candidate. Used only when Require unique match is enabled."), DisplayName("Min unique margin")]
        public double UNIQUE_MATCH_MIN_SCORE_MARGIN { get; set; } = 0.03D;

        [PropertyOrder(3)]
        [Category("Matching"), Description("Opt-in global contrast-reversal support. When enabled, one candidate must match either every taught edge direction or the globally reversed direction; local edge directions are never ignored. Missing recipe/XML keys keep the legacy Same-only behavior."), DisplayName("Allow global polarity reversal")]
        public bool ALLOW_GLOBAL_POLARITY_REVERSAL { get; set; } = false;

        [PropertyOrder(6)]
        [Category("Matching"), Description("Draw the detected template outline and center point on the output image. Non-zero angle results must be drawn as rotated outlines."), DisplayName("Draw result")]
        public bool USE_DRAW_IMAGE { get; set; } = true;

        [PropertyOrder(0)]
        [Category("Auto MPoint"), Description("Restrict automatic pattern candidate analysis to the reviewed analysis ROI. This teaching option never runs matching Preview by itself."), DisplayName("Use analysis ROI")]
        public bool AUTO_MPOINT_USE_ANALYSIS_ROI { get; set; } = false;

        [PropertyOrder(1)]
        [PropertyEditor(typeof(WpgROIEditor))]
        [Category("Auto MPoint"), Description("Optional source-image region in which Auto MPoint searches for distinctive pattern candidates."), DisplayName("Analysis ROI")]
        public Rect AUTO_MPOINT_ANALYSIS_ROI { get; set; } = new Rect();

        [PropertyOrder(2)]
        [Category("Auto MPoint"), Description("Width in pixels of each candidate pattern window."), DisplayName("Pattern width")]
        public int AUTO_MPOINT_PATTERN_WIDTH { get; set; } = 96;

        [PropertyOrder(3)]
        [Category("Auto MPoint"), Description("Height in pixels of each candidate pattern window."), DisplayName("Pattern height")]
        public int AUTO_MPOINT_PATTERN_HEIGHT { get; set; } = 96;

        [PropertyOrder(4)]
        [Category("Auto MPoint"), Description("Grid interval in pixels between candidate windows. Smaller values inspect more positions and take longer."), DisplayName("Candidate stride")]
        public int AUTO_MPOINT_STRIDE { get; set; } = 16;

        [PropertyOrder(5)]
        [Category("Auto MPoint"), Description("Maximum number of accepted suggestions shown for operator review."), DisplayName("Max suggestions")]
        public int AUTO_MPOINT_MAX_RESULTS { get; set; } = 5;

        [PropertyOrder(6)]
        [Category("Auto MPoint"), Description("Minimum normalized contrast, edge coverage, and directional-balance quality required before exact matching checks."), DisplayName("Min feature quality")]
        public double AUTO_MPOINT_MIN_FEATURE_QUALITY { get; set; } = 0.15D;

        [PropertyOrder(7)]
        [Category("Auto MPoint"), Description("Minimum score margin between the taught location and the strongest competing location. Raise this for repetitive images."), DisplayName("Min uniqueness")]
        public double AUTO_MPOINT_MIN_UNIQUENESS { get; set; } = 0.05D;

        [PropertyOrder(8)]
        [Category("Auto MPoint"), Description("Maximum synthetic relocation error in pixels allowed for a suggested pattern."), DisplayName("Max position error")]
        public double AUTO_MPOINT_MAX_POSITION_ERROR { get; set; } = 2.5D;

        [PropertyOrder(9)]
        [Category("Auto MPoint"), Description("Minimum number of additional representative images required when Auto MPoint ranks candidates by actual cross-image matching performance."), DisplayName("Min representative images")]
        public int AUTO_MPOINT_MIN_REPRESENTATIVE_IMAGES { get; set; } = 3;

        [PropertyOrder(10)]
        [Category("Auto MPoint"), Description("Minimum 0..1 matching success rate across the selected representative images. Candidates below this rate are rejected."), DisplayName("Min representative success")]
        public double AUTO_MPOINT_MIN_REPRESENTATIVE_SUCCESS_RATE { get; set; } = 0.75D;

        [PropertyOrder(0)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(0, 255, 1, 0, nameof(CANNY_LOW), nameof(CANNY_HIGH))]
        [Category("Edge Model"), Description("Canny low/high thresholds used to build the template edge model. Raise them when noise creates too many edges; lower them when the taught edge breaks."), DisplayName("Canny range")]
        public int CANNY_LOW { get; set; } = 30;

        [PropertyOrder(1)]
        [Browsable(true)]
        [Category("Edge Model"), Description("Upper endpoint for the Canny range. It is edited through the Canny range row and kept for recipe/XML compatibility."), DisplayName("Canny high")]
        public int CANNY_HIGH { get; set; } = 90;

        [PropertyOrder(2)]
        [Category("Edge Model"), Description("Canny kernel size. Runtime normalizes to 3, 5, or 7. Keep 3 unless noise requires a wider gradient window."), DisplayName("Aperture size")]
        public int CANNY_APERTURE_SIZE { get; set; } = 3;

        [PropertyOrder(3)]
        [Category("Edge Model"), Description("Use the more precise L2 gradient calculation in Canny. Usually keep this enabled for stable edge direction scoring."), DisplayName("Use L2 gradient")]
        public bool USE_L2_GRADIENT { get; set; } = true;

        [PropertyOrder(4)]
        [Category("Edge Model"), Description("How template contours are collected from the edge image. External is the usual starting point for outline-style patterns."), DisplayName("Retrieval mode")]
        public RetrievalModes CONTOUR_RETRIEVAL_MODE { get; set; } = RetrievalModes.External;

        [PropertyOrder(5)]
        [Category("Edge Model"), Description("How much contour points are simplified before building the edge model. More simplification can be faster but may weaken detailed shapes."), DisplayName("Approximation")]
        public ContourApproximationModes CONTOUR_APPROXIMATION_MODE { get; set; } = ContourApproximationModes.ApproxNone;

        [PropertyOrder(6)]
        [Category("Edge Model"), Description("Maximum template edge points used for scoring. More points preserve shape detail but increase runtime."), DisplayName("Max template points")]
        public int MAX_TEMPLATE_POINTS { get; set; } = 300;

        [PropertyOrder(7)]
        [Category("Edge Model"), Description("Ignore very weak source gradients while scoring. Increase this when background noise produces unstable edge matches."), DisplayName("Min gradient magnitude")]
        public double MIN_GRADIENT_MAGNITUDE { get; set; } = 1.0D;

        [PropertyOrder(0)]
        [Category("Angle"), Description("Search rotated edge templates. Enable this only when the target can appear at different angles because it multiplies runtime."), DisplayName("Use angle search")]
        public bool USE_FIND_ANGLE { get; set; } = false;

        [PropertyOrder(1)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(-180, 180, 1, 0, nameof(FIND_ANGLE_MIN), nameof(FIND_ANGLE_MAX))]
        [Category("Angle"), Description("Rotation angle range to search. Wider ranges are slower; narrow this to the expected production rotation whenever possible."), DisplayName("Angle range")]
        public int FIND_ANGLE_MIN { get; set; } = -10;

        [PropertyOrder(2)]
        [Category("Angle"), Description("Angle interval in degrees. Smaller values are more precise but increase runtime."), DisplayName("Angle step")]
        public double FIND_ANGLE { get; set; } = 1.0D;

        [PropertyOrder(3)]
        [Browsable(true)]
        [Category("Angle"), Description("Upper endpoint for the angle range. It is edited through the Angle range row and kept for recipe/XML compatibility."), DisplayName("Max angle")]
        public int FIND_ANGLE_MAX { get; set; } = 10;

        [PropertyOrder(4)]
        [Category("Angle"), Description("First scan the full angle range with a larger step, then recheck only the best angles with the fine Angle step. Use for wide angle ranges after sample validation."), DisplayName("Coarse angle search")]
        public bool USE_COARSE_TO_FINE_ANGLE_SEARCH { get; set; } = false;

        [PropertyOrder(5)]
        [Category("Angle"), Description("First-pass angle interval in degrees for coarse-to-fine search."), DisplayName("Coarse angle step")]
        public double COARSE_ANGLE_STEP { get; set; } = 5.0D;

        [PropertyOrder(6)]
        [Category("Angle"), Description("Number of best coarse angles to refine. Higher values reduce miss risk but increase search time."), DisplayName("Coarse top K")]
        public int COARSE_ANGLE_TOP_K { get; set; } = 3;

        [PropertyOrder(0)]
        [Category("Scale"), Description("Search templates at multiple sizes. Use when the target can appear larger or smaller than the taught template. Keep the range narrow because every scale adds matching work."), DisplayName("Use scale search")]
        public bool USE_FIND_SCALE { get; set; } = false;

        [PropertyOrder(1)]
        [Category("Scale"), Description("Smallest template scale to search. 1.0 is the taught template size; 0.9 means 90% of the taught size."), DisplayName("Min scale")]
        public double FIND_SCALE_MIN { get; set; } = 0.9D;

        [PropertyOrder(2)]
        [Category("Scale"), Description("Largest template scale to search. 1.0 is the taught template size; 1.1 means 110% of the taught size."), DisplayName("Max scale")]
        public double FIND_SCALE_MAX { get; set; } = 1.1D;

        [PropertyOrder(3)]
        [Category("Scale"), Description("Scale interval. Smaller values are more precise but multiply runtime. Start around 0.02 to 0.05 for teaching checks."), DisplayName("Scale step")]
        public double FIND_SCALE_STEP { get; set; } = 0.05D;

        [PropertyOrder(0)]
        [Category("Search"), Description("Pixel interval for candidate center search. 1 is most accurate. Larger values can be faster, but may skip the true position on small or repetitive patterns."), DisplayName("Search step")]
        public int SEARCH_STEP { get; set; } = 2;

        [PropertyOrder(1)]
        [Category("Search"), Description("After coarse center search, recheck the best center neighborhoods with a 1px step. Use only after sample validation because large Search step values can miss the correct coarse candidate."), DisplayName("Refine position")]
        public bool USE_POSITION_REFINE { get; set; } = false;

        [PropertyOrder(2)]
        [Category("Search"), Description("Refine the final center with a local 3x3 score peak fit. This improves displayed center stability without changing the edge score threshold."), DisplayName("Subpixel refine")]
        public bool USE_SUBPIXEL_REFINE { get; set; } = false;

        [PropertyOrder(3)]
        [Category("Search"), Description("How quickly unlikely candidates are abandoned during scoring. Higher values can be faster, but may reject weak patterns. Start around 0.8 to 0.9."), DisplayName("Greediness")]
        public double GREEDINESS { get; set; } = 0.9D;

        [PropertyOrder(4)]
        [Category("Search"), Description("Speed option for large images/templates. It finds candidate positions on a 1/2-scale image, verifies them on the original image, and falls back to full search when the proposal is weak. Scale search uses the full edge search path."), DisplayName("Pyramid proposal")]
        public bool USE_PYRAMID_POSITION_PROPOSAL { get; set; } = false;

        [PropertyOrder(5)]
        [Category("Search"), Description("Number of 1/2-scale candidates passed to original-resolution verification. Keep 6 as the safe starting point; lower values can miss repeated patterns."), DisplayName("Pyramid proposal top N")]
        public int PYRAMID_POSITION_TOP_N { get; set; } = 6;

        [PropertyOrder(6)]
        [Category("Search"), Description("Original-resolution edge score needed before accepting the pyramid proposal without fallback. Lower values can be faster but increase false-match risk."), DisplayName("Pyramid proposal min score")]
        public double PYRAMID_POSITION_MIN_SCORE { get; set; } = 0.70D;

        [PropertyOrder(7)]
        [Category("Search"), Description("Safety option for repeated or similar edge shapes. Re-ranks the best edge candidates with image-template similarity while keeping the public score as the edge score."), DisplayName("Hybrid verify")]
        public bool USE_HYBRID_VERIFY { get; set; } = false;

        [PropertyOrder(8)]
        [Category("Search"), Description("Number of top edge candidates rechecked with image similarity. Higher values reduce false-match risk but increase runtime."), DisplayName("Hybrid top N")]
        public int HYBRID_VERIFY_TOP_N { get; set; } = 5;

        [PropertyOrder(9)]
        [Category("Search"), Description("Image similarity weight used only for selecting among top edge candidates. The reported score remains the edge score so SCORE_MIN recipes stay compatible."), DisplayName("Hybrid image weight")]
        public double HYBRID_VERIFY_IMAGE_WEIGHT { get; set; } = 0.35D;

        internal Mat ImageTemplate { get; set; } = new Mat();

        public EdgeBasedMatchingProperty() : base()
        {
            USE_THRESHOLD = false;
        }

        public EdgeBasedMatchingProperty(string strName) : base(strName)
        {
            USE_THRESHOLD = false;
        }

        public EdgeBasedMatchingProperty DeepCopy()
        {
            return (EdgeBasedMatchingProperty)MemberwiseClone();
        }

        public EdgeBasedMatchingProperty LoadConfig(string recipeName)
        {
            return LoadConfigFile<EdgeBasedMatchingProperty>(recipeName, LoadTemplateImage);
        }

        public EdgeBasedMatchingProperty LoadTestConfig(string path)
        {
            return LoadTestConfigFile<EdgeBasedMatchingProperty>(path);
        }

        private static void LoadTemplateImage(EdgeBasedMatchingProperty property)
        {
            property.ReloadTemplateImage();
        }

        public void ReloadTemplateImage()
        {
            ImageTemplate?.Dispose();
            ImageTemplate = new Mat();

            if (System.IO.File.Exists(PATTERN_PATH))
            {
                ImageTemplate = Cv2.ImRead(PATTERN_PATH);
            }
        }
    }
}
