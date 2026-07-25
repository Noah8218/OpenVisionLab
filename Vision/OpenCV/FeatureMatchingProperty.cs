using Lib.OpenCV.Blob;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.WpfPropertyGrid;
using System.Xml.Linq;
using static OpenVisionLab.PropertyGridEditorFactory;
using System.Xml.Serialization;
using Lib.OpenCV.Property;

namespace OpenVisionLab.Vision._1._Tools.OpenCV
{
    [CategoryOrder("Parameter", 0)]
    [CategoryOrder("Matching", 1)]
    [CategoryOrder("ROI", 2)]
    [CategoryOrder("Threshold", 3)]
    [CategoryOrder("Image Process", 4)]
    [System.Xml.Serialization.XmlRoot("CPropertyFeatureMatching")]
    public class FeatureMatchingProperty : OpenCvPropertyBase, IOpenCVPropertyFeatureSIFT, IOpenCvConfigurableProperty<FeatureMatchingProperty>
    {
        [PropertyOrder(1)]
        [CategoryAttribute("Matching"), DescriptionAttribute("Lowe ratio threshold used to keep distinctive descriptor matches. Smaller values are stricter; larger values keep more candidates."), DisplayNameAttribute("Ratio threshold")]
        public double SCORE_MIN { get; set; } = 0.6D;

        [PropertyOrder(2)]
        [CategoryAttribute("Matching"), DescriptionAttribute("RANSAC reprojection tolerance in pixels. Higher values accept more geometric variation."), DisplayNameAttribute("RANSAC tolerance")]
        public double RANSAC_REPROJ_THRESHOLD { get; set; } = 3D;

        [PropertyOrder(0)]
        [PropertyEditor(typeof(WpgMatchEditor))]
        [CategoryAttribute("Matching"), DescriptionAttribute("Feature template image path used for teaching and matching."), DisplayNameAttribute("Feature template path")]
        public string PATTERN_PATH { get; set; } = "";

        internal Mat ImageTemplate { get; set; } = new Mat();

        public FeatureMatchingProperty(string strName)
        {
            NAME = strName;
        }

        public FeatureMatchingProperty() { }

        public FeatureMatchingProperty DeepCopy() => (FeatureMatchingProperty)this.MemberwiseClone();

        public FeatureMatchingProperty LoadConfig(string RecipeName)
        {
            return LoadConfigFile<FeatureMatchingProperty>(RecipeName, LoadTemplateImage);
        }

        public FeatureMatchingProperty LoadTestConfig(string path)
        {
            return LoadTestConfigFile<FeatureMatchingProperty>(path);
        }

        private static void LoadTemplateImage(FeatureMatchingProperty property)
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
