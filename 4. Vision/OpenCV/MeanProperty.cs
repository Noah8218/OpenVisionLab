using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV;
using Lib.OpenCV.Property;

namespace OpenVisionLab
{
    [CategoryOrder("Mean", 6)]
    [System.Xml.Serialization.XmlRoot("CPropertyMean")]
    public class MeanProperty : OpenCvPropertyBase, IOpenCVPropertyMean, IOpenCvConfigurableProperty<MeanProperty>
    {        
        [CategoryAttribute("Mean"), DescriptionAttribute(""), DisplayNameAttribute("MEAN_MAX")]
        public int MEAN_MAX { get; set; } = 240;
        [CategoryAttribute("Mean"), DescriptionAttribute(""), DisplayNameAttribute("MEAN_MIN")]
        public int MEAN_MIN { get; set; } = 100;

        [CategoryAttribute("Mean"), DescriptionAttribute(""), DisplayNameAttribute("MEAN_TYPES")]
        public MeanType MEAN_TYPES { get; set; } = MeanType.Mean;
        
        public MeanProperty(string strName) { NAME = strName; }        
        public MeanProperty() { }

        public MeanProperty DeepCopy() => (MeanProperty)this.MemberwiseClone();
    
        #region CONFIG BY XML              
        public MeanProperty LoadConfig(string RecipeName)
        {
            return LoadConfigFile<MeanProperty>(RecipeName);
        }

        public MeanProperty LoadTestConfig(string path)
        {
            return LoadTestConfigFile<MeanProperty>(path);
        }
        #endregion

    }
}
