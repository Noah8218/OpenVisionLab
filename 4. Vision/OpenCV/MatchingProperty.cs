using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using System.Xml.Serialization;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    [CategoryOrder("Matching", 3)]
    [System.Xml.Serialization.XmlRoot("CPropertyMatching")]
    public class MatchingProperty : OpenCvPropertyBase, IOpenCVPropertyMatching, IOpenCvConfigurableProperty<MatchingProperty>
    {
        [PropertyOrder(0)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("Match mode")]
        public TemplateMatchModes MATCH_MODE { get; set; } = TemplateMatchModes.CCoeffNormed;

        [PropertyOrder(1)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("Min score")]
        public double SCORE_MIN { get; set; } = 0.6D;

        [PropertyOrder(2)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("Magnification")]
        public double MAGNIFIATION { get; set; } = 1.0D;

        [PropertyOrder(3)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("Match count")]
        public int NUM_MATCH { get; set; } = 3;

        [PropertyOrder(4)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("Use angle search")]
        public bool USE_FIND_ANGLE { get; set; } = true;

        [PropertyOrder(5)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("Angle step")]
        public double FIND_ANGLE { get; set; } = 0.1D;

        [PropertyOrder(7)]
        [Browsable(false)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("Max angle")]
        public int FIND_ANGLE_MAX { get; set; } = 10;

        [PropertyOrder(6)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(-180, 180, 1, 0, nameof(FIND_ANGLE_MIN), nameof(FIND_ANGLE_MAX))]
        [CategoryAttribute("Matching"), DescriptionAttribute("패턴 매칭에서 탐색할 회전 각도 범위입니다."), DisplayNameAttribute("Angle range")]
        public int FIND_ANGLE_MIN { get; set; } = -10;

        [PropertyOrder(8)]
        [PropertyEditor(typeof(WpgMatchEditor))]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("Pattern path")]
        public string PATTERN_PATH { get; set; } = "";

        [PropertyOrder(0)]
        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("Use canny")]
        public bool USE_CANNY { get; set; } = false;
        [PropertyOrder(2)]
        [Browsable(false)]
        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("Canny high")]
        public int CANNY_HIGH { get; set; } = 60;
        [PropertyOrder(1)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(0, 255, 1, 0, nameof(CANNY_LOW), nameof(CANNY_HIGH))]
        [CategoryAttribute("Image Process"), DescriptionAttribute("Canny 전처리에 사용할 Low/High 임계값 범위입니다."), DisplayNameAttribute("Canny range")]
        public int CANNY_LOW { get; set; } = 30;
        
        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("Use white padding")]
        public bool USE_PADDING_COLOR_WHITE { get; set; } = false;

        [XmlIgnore] public Mat ImageTemplate = new Mat();

        public MatchingProperty() : base() { }
        public MatchingProperty(string strName) : base(strName) { }

        public MatchingProperty DeepCopy()
        {
            MatchingProperty temp = (MatchingProperty)this.MemberwiseClone();
            return temp;
        }

        #region CONFIG BY XML              
        public MatchingProperty LoadConfig(string RecipeName)
        {
            return LoadConfigFile<MatchingProperty>(RecipeName, LoadTemplateImage);
        }

        public MatchingProperty LoadTestConfig(string path)
        {
            return LoadTestConfigFile<MatchingProperty>(path);
        }

        private static void LoadTemplateImage(MatchingProperty property)
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
        #endregion

    }
}
