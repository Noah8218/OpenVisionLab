using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    [CategoryOrder("Mean", 6)]
    [System.Xml.Serialization.XmlRoot("CPropertyMean")]
    public class MeanProperty : OpenCvPropertyBase, IOpenCVPropertyMean, IOpenCvConfigurableProperty<MeanProperty>
    {        
        [PropertyOrder(1)]
        [Browsable(false)]
        [CategoryAttribute("Mean"), DescriptionAttribute(""), DisplayNameAttribute("Max mean")]
        public int MEAN_MAX { get; set; } = 240;

        [PropertyOrder(0)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(0, 255, 1, 0, nameof(MEAN_MIN), nameof(MEAN_MAX))]
        [CategoryAttribute("Mean"), DescriptionAttribute("Mean 판정에 사용할 GV 범위입니다."), DisplayNameAttribute("Mean range")]
        public int MEAN_MIN { get; set; } = 100;

        [PropertyOrder(2)]
        [CategoryAttribute("Mean"), DescriptionAttribute(""), DisplayNameAttribute("Mean type")]
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
