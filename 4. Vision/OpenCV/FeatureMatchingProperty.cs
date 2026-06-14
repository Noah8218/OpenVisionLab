using Lib.OpenCV.Blob;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using System.Xml.Linq;
using static OpenVisionLab.PropertyGridEditorFactory;
using System.Xml.Serialization;
using Lib.OpenCV.Property;

namespace OpenVisionLab.Vision._1._Tools.OpenCV
{
    [System.Xml.Serialization.XmlRoot("CPropertyFeatureMatching")]
    public class FeatureMatchingProperty : OpenCvPropertyBase, IOpenCVPropertyFeatureSIFT, IOpenCvConfigurableProperty<FeatureMatchingProperty>
    {
        [PropertyOrder(1)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("SCORE_MIN")]
        public double SCORE_MIN { get; set; } = 0.6D;

        [PropertyOrder(4)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("RANSAC_REPROJ_THRESHOLD")]
        public double RANSAC_REPROJ_THRESHOLD { get; set; } = 3D;

        [PropertyOrder(8)]
        [PropertyEditor(typeof(WpgMatchEditor))]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("PATTERN PATH")]
        public string PATTERN_PATH { get; set; } = "";

        [XmlIgnore] public Mat ImageTemplate = new Mat();

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
