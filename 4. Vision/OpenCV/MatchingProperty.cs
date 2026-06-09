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
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("MATCH_MODE")]
        public TemplateMatchModes MATCH_MODE { get; set; } = TemplateMatchModes.CCoeffNormed;

        [PropertyOrder(1)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("SCORE_MIN")]
        public double SCORE_MIN { get; set; } = 0.6D;

        [PropertyOrder(2)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("MAGNIFICATION")]
        public double MAGNIFIATION { get; set; } = 1.0D;

        [PropertyOrder(3)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("FIND PATTERN COUNT")]
        public int NUM_MATCH { get; set; } = 3;

        [PropertyOrder(4)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("USE FIND ANGLE")]
        public bool USE_FIND_ANGLE { get; set; } = true;

        [PropertyOrder(5)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("FIND ANGLE")]
        public double FIND_ANGLE { get; set; } = 0.1D;

        [PropertyOrder(6)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("FIND ANGLE MAX")]
        public int FIND_ANGLE_MAX { get; set; } = 10;

        [PropertyOrder(7)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("FIND ANGLE MIN")]
        public int FIND_ANGLE_MIN { get; set; } = -10;

        [PropertyOrder(8)]
        [PropertyEditor(typeof(WpgMatchEditor))]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("PATTERN PATH")]
        public string PATTERN_PATH { get; set; } = "";

        [PropertyOrder(0)]
        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("USE CANNY")]
        public bool USE_CANNY { get; set; } = false;
        [PropertyOrder(1)]
        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("CANNY HIGH")]
        public int CANNY_HIGH { get; set; } = 60;
        [PropertyOrder(2)]
        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("CANNY LOW")]
         public int CANNY_LOW { get; set; } = 30;
        
        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("USE PADDING COLOR WHITE")]
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
            return LoadTestConfigFile<MatchingProperty>(path, LoadTemplateImage);
        }

        private static void LoadTemplateImage(MatchingProperty property)
        {
            if (System.IO.File.Exists(property.PATTERN_PATH))
            {
                property.ImageTemplate = Cv2.ImRead(property.PATTERN_PATH);
            }
        }
        #endregion

    }
}
