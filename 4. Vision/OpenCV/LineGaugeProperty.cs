using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using static Lib.Common.FormulaUtil;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    [CategoryOrder("Edge", 9)]
    [CategoryOrder("Scan Line", 10)]
    [CategoryOrder("Fit Line", 11)]
    [CategoryOrder("Filter", 12)]
    [CategoryOrder("Draw", 99)]
    [System.Xml.Serialization.XmlRoot("CPropertyLineGuage")]
    public class LineGaugeProperty : OpenCvPropertyBase, IOpenCvPropertyLineGauge, IOpenCvConfigurableProperty<LineGaugeProperty>
    {        
        [PropertyOrder(0)]
        [Browsable(true)]
        [CategoryAttribute("Edge"), DescriptionAttribute("엣지 검출 타입을 결정합니다."), DisplayNameAttribute("Polarity")]
        public PROJECTION_POLARITY PRJ_PORALITY { get; set; } = PROJECTION_POLARITY.BTOW;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        public PROJECTION_POLARITY PRJ_POLARITY
        {
            get => PRJ_PORALITY;
            set => PRJ_PORALITY = value;
        }

        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("Edge"), DescriptionAttribute("엣지 검출 방향을 결정합니다."), DisplayNameAttribute("Direction")]        
        public PROJECTION_DIR PRJ_DIR { get; set; } = PROJECTION_DIR.X_LTOR;

        [PropertyOrder(2)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [NumberRange(0, 255, 1, 0)]
        [CategoryAttribute("Edge"), DescriptionAttribute("픽셀간의 차이이며, 값이 30일시 현재 픽셀과 이전 픽셀의 차이가 30이 차이면 엣지로 결정합니다."), DisplayNameAttribute("Contrast")]
        public double CONTRAST { get; set; } = 30;

        [PropertyOrder(3)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [NumberRange(1, 50, 1, 0)]
        [CategoryAttribute("Edge"), DescriptionAttribute("연속성 검사 파라미터이며, 값이 10일시 10개가 연속해서 CONTRAST차이가 나면 엣ㅅ지로 결정합니다."), DisplayNameAttribute("Thickness")]        
        public double THICKNESS { get; set; } = 5;

        [PropertyOrder(4)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [NumberRange(1, 100, 1, 0)]
        [CategoryAttribute("Edge"), DescriptionAttribute("엣지의 간격 파라미터이며, 10개일시 10픽셀마다 엣지를 검출합니다."), DisplayNameAttribute("Sampling step")]
        public double SAMPLING_STEP { get; set; } = 10;

        [PropertyOrder(0)]
        [Browsable(true)]
        [CategoryAttribute("Scan Line"), DescriptionAttribute("스캔 라인 생성 방향을 결정합니다."), DisplayNameAttribute("Scan direction")]
        public PROJECTION_DIR VER_PRJ_DIR { get; set; } = PROJECTION_DIR.X_LTOR;

        [PropertyOrder(1)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [NumberRange(1, 100, 1, 0)]
        [CategoryAttribute("Scan Line"), DescriptionAttribute("스캔 라인 생성 간격을 결정합니다. 값이 10이면 10개 포인트 간격으로 스캔 라인을 생성합니다."), DisplayNameAttribute("Scan interval")]
        public int POINT_RANGE { get; set; } = 10;

        [RefreshProperties(RefreshProperties.All)]
        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Scan Line"), DescriptionAttribute("True시 지정한 각도로 스캔 라인을 생성합니다."), DisplayNameAttribute("Use scan angle")]
        public bool USE_MANUAL_ANGLE { get; set; }

        [PropertyOrder(3)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [NumberRange(-180, 180, 1, 0)]
        [CategoryAttribute("Scan Line"), DescriptionAttribute("스캔 라인을 생성할 각도입니다."), DisplayNameAttribute("Scan angle")]
        public double MANUAL_ANGLE_VALUE { get; set; } = 0;

        [PropertyOrder(0)]
        [Browsable(true)]
        [CategoryAttribute("Fit Line"), DescriptionAttribute("사용시 Fitting Line에 길이를 늘리는 옵션 적용합니다."), DisplayNameAttribute("Extend fit line")]
        public bool USE_EXTEND_FIT_LINE { get; set; } = false;

        [PropertyOrder(1)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [NumberRange(0, 1000, 10, 0)]
        [CategoryAttribute("Fit Line"), DescriptionAttribute("해당 값(pixel)만큼 Fitting Line에 길이를 늘립니다. "), DisplayNameAttribute("Extend length")]
        public int EXTEND_FIT_LINE_VALUE { get; set; } = 100;

        [PropertyOrder(2)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [NumberRange(0, 255, 1, 0)]
        [CategoryAttribute("Filter"), DescriptionAttribute("평균 엣지 필터링 차이값입니다. 평균값과 엣지 값의 차이가 설정값이상 나온다면 필터링 합니다."), DisplayNameAttribute("Average diff")]
        public double AVERAGE_Diff { get; set; } = 100;

        [PropertyOrder(0)]
        [Browsable(true)]
        [CategoryAttribute("Filter"), DescriptionAttribute("평균값으로 엣지 필터링을 사용할지 결정합니다."), DisplayNameAttribute("Use average filter")]
        public bool USE_AVERAGE_FILTER { get; set; } = false;

        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("Filter"), DescriptionAttribute("필터링할 엣지 타입을 결정합니다.(X/Y)"), DisplayNameAttribute("Average filter type")]
        public AVERAGE_FILTER_TYPES AVERAGE_FILTER_TYPE { get; set; } = AVERAGE_FILTER_TYPES.Y;

        public enum AVERAGE_FILTER_TYPES
        {
            X,
            Y
        }

        [PropertyOrder(0)]
        [Browsable(true)]
        [CategoryAttribute("Draw"), DescriptionAttribute("검사 Draw시 스캔 라인을 Draw할지 결정합니다."), DisplayNameAttribute("Show scan line")]
        public bool SHOW_VERTICAL_LINE { get; set; } = true;

        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("Draw"), DescriptionAttribute("검사 Draw시 엣지를 Draw할지 결정합니다."), DisplayNameAttribute("Show edge")]
        public bool SHOW_EDGE { get; set; } = true;

        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Draw"), DescriptionAttribute("검사 Draw시 엣지들의 연결을 Draw할지 결정합니다."), DisplayNameAttribute("Show contour line")]
        public bool SHOW_CONTOUR { get; set; } = true;

        [PropertyOrder(3)]
        [Browsable(true)]
        [CategoryAttribute("Draw"), DescriptionAttribute("검사 Draw시 핏팅 라인을 Draw할지 결정합니다."), DisplayNameAttribute("Show fit line")]
        public bool SHOW_FITLINE { get; set; } = true;

        public LineGaugeProperty() : base() { }
        public LineGaugeProperty(string strName) : base(strName) { }

        public LineGaugeProperty DeepCopy() => (LineGaugeProperty)this.MemberwiseClone();        

        public LineGaugeProperty LoadConfig(string RecipeName)
        {
            return LoadConfigFile<LineGaugeProperty>(RecipeName);
        }

        public LineGaugeProperty LoadTestConfig(string path)
        {
            return LoadTestConfigFile<LineGaugeProperty>(path);
        }
    }
}
