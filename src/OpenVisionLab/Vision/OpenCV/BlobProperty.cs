using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using OpenVisionLab.Vision2D.Blob;
using OpenVisionLab.Vision2D.Property;
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
        [RangeEditor(0, 1000000, 100, 0, nameof(MIN_AREA), nameof(MAX_AREA))]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("Blob으로 인정할 Area(가로*세로) 최소 사이즈입니다."), DisplayNameAttribute("Min area")]
        public  int MIN_AREA { get; set; } = 200;

        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("Area(가로*세로) 최대 사이즈입니다. 그 이상는 필터링 됩니다."), DisplayNameAttribute("Max area")]
        public int MAX_AREA { get; set; } = 1000000;

        [PropertyOrder(3)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(0, 1000000, 10, 0, nameof(MIN_WIDTH), nameof(MAX_WIDTH))]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("개별 Blob의 축 정렬 바운딩 박스 최소 폭(px)입니다."), DisplayNameAttribute("Min bounding width")]
        public int MIN_WIDTH { get; set; } = 0;

        [PropertyOrder(4)]
        [Browsable(true)]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("개별 Blob의 축 정렬 바운딩 박스 최대 폭(px)입니다."), DisplayNameAttribute("Max bounding width")]
        public int MAX_WIDTH { get; set; } = 1000000;

        [PropertyOrder(5)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(0, 1000000, 10, 0, nameof(MIN_HEIGHT), nameof(MAX_HEIGHT))]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("개별 Blob의 축 정렬 바운딩 박스 최소 높이(px)입니다."), DisplayNameAttribute("Min bounding height")]
        public int MIN_HEIGHT { get; set; } = 0;

        [PropertyOrder(6)]
        [Browsable(true)]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("개별 Blob의 축 정렬 바운딩 박스 최대 높이(px)입니다."), DisplayNameAttribute("Max bounding height")]
        public int MAX_HEIGHT { get; set; } = 1000000;

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
