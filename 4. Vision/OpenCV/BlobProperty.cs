using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Property;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    [CategoryOrder("Blob Parameter", 6)]
    [System.Xml.Serialization.XmlRoot("CPropertyBlob")]
    public class BlobProperty : OpenCvPropertyBase, IOpenCVPropertyBlob, IOpenCvConfigurableProperty<BlobProperty>
    {
        [PropertyOrder(1)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(0, 1000000, 10, 0, nameof(MIN_AREA), nameof(MAX_AREA))]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("Blob으로 인정할 Area(가로*세로) 범위입니다."), DisplayNameAttribute("Area range")]
        public  int MIN_AREA { get; set; } = 200;

        [PropertyOrder(2)]
        [Browsable(false)]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("Area(가로*세로) 최대 사이즈입니다. 그 이상는 필터링 됩니다."), DisplayNameAttribute("Max area")]
        public int MAX_AREA { get; set; } = 1000000;

        public BlobProperty(string strName)
        {
            NAME = strName;
        }

        public BlobProperty() { }
    
        public BlobProperty DeepCopy() => (BlobProperty)this.MemberwiseClone();
        
        public BlobProperty LoadConfig(string RecipeName)
        {
            return LoadConfigFile<BlobProperty>(RecipeName);
        }

        public BlobProperty LoadTestConfig(string path)
        {
            return LoadTestConfigFile<BlobProperty>(path);
        }
    }
}
