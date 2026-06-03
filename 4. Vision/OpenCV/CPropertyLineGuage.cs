using System.ComponentModel;
using System.IO;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using static Lib.Common.CFormula;

namespace OpenVisionLab
{
    [CategoryOrder("Edge", 9)]
    [CategoryOrder("Fit Line", 10)]
    [CategoryOrder("Vertical Line", 11)]

    [CategoryOrder("Filter", 12)]
    [CategoryOrder("Draw", 13)]    
    public class CPropertyLineGuage : COpenCVPropertyBase, IOpenCVPropertyLineGuage
    {        
        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("Edge"), DescriptionAttribute("엣지 검출 타입을 결정합니다."), DisplayNameAttribute("POLARITY")]
        public PROJECTION_POLARITY PRJ_PORALITY { get; set; } = PROJECTION_POLARITY.BTOW;

        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Edge"), DescriptionAttribute("엣지 검출 방향을 결정합니다."), DisplayNameAttribute("DIRECTION")]        
        public PROJECTION_DIR PRJ_DIR { get; set; } = PROJECTION_DIR.X_LTOR;

        [PropertyOrder(3)]
        [Browsable(true)]
        [CategoryAttribute("Edge"), DescriptionAttribute("픽셀간의 차이이며, 값이 30일시 현재 픽셀과 이전 픽셀의 차이가 30이 차이면 엣지로 결정합니다."), DisplayNameAttribute("CONTRAST")]
        public double CONTRAST { get; set; } = 30;

        [PropertyOrder(4)]
        [Browsable(true)]
        [CategoryAttribute("Edge"), DescriptionAttribute("연속성 검사 파라미터이며, 값이 10일시 10개가 연속해서 CONTRAST차이가 나면 엣ㅅ지로 결정합니다."), DisplayNameAttribute("THICKNESS")]        
        public double THICKNESS { get; set; } = 5;

        [PropertyOrder(5)]
        [Browsable(true)]
        [CategoryAttribute("Edge"), DescriptionAttribute("엣지의 간격 파라미터이며, 10개일시 10픽셀마다 엣지를 검출합니다."), DisplayNameAttribute("STEP")]
        public double SAMPLING_STEP { get; set; } = 10;

        [Browsable(true)]
        [CategoryAttribute("Vertical Line"), DescriptionAttribute("수직선 생성 방향을 결정합니다."), DisplayNameAttribute("VERTICAL DIRECTION")]
        public PROJECTION_DIR VER_PRJ_DIR { get; set; } = PROJECTION_DIR.X_LTOR;

        [Browsable(true)]
        [CategoryAttribute("Vertical Line"), DescriptionAttribute("수직선의 각도를 결정합니다. 값이 10일시 10개의 픽셀의 각도로 수직선을 생성합니다."), DisplayNameAttribute("POINT RANGE")]
        public int POINT_RANGE { get; set; } = 10;

        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [CategoryAttribute("Vertical Line"), DescriptionAttribute("True시 임의의 각도로 수직선을 생성합니다."), DisplayNameAttribute("USE MANUAL ANGLE")]
        public bool USE_MANUAL_ANGLE { get; set; }

        [Browsable(true)]
        [CategoryAttribute("Vertical Line"), DescriptionAttribute(""), DisplayNameAttribute("MANUAL_ANGLE_VALUE")]
        public double MANUAL_ANGLE_VALUE { get; set; } = 0;

        [Browsable(true)]
        [CategoryAttribute("Fit Line"), DescriptionAttribute("사용시 Fitting Line에 길이를 늘리는 옵션 적용합니다."), DisplayNameAttribute("USE EXTEND FIT LINE")]
        public bool USE_EXTEND_FIT_LINE { get; set; } = false;

        [Browsable(true)]
        [CategoryAttribute("Fit Line"), DescriptionAttribute("해당 값(pixel)만큼 Fitting Line에 길이를 늘립니다. "), DisplayNameAttribute("EXTEND FIT LINE VALUE")]
        public int EXTEND_FIT_LINE_VALUE { get; set; } = 100;

        [Browsable(true)]
        [CategoryAttribute("Filter"), DescriptionAttribute("평균 엣지 필터링 차이값입니다. 평균값과 엣지 값의 차이가 설정값이상 나온다면 필터링 합니다."), DisplayNameAttribute("AVERAGE_Diff")]
        public double AVERAGE_Diff { get; set; } = 100;

        [Browsable(true)]
        [CategoryAttribute("Filter"), DescriptionAttribute("평균값으로 엣지 필터링을 사용할지 결정합니다."), DisplayNameAttribute("USE AVERAGE FILTER")]
        public bool USE_AVERAGE_FILTER { get; set; } = false;

        [Browsable(true)]
        [CategoryAttribute("Filter"), DescriptionAttribute("필터링할 엣지 타입을 결정합니다.(X/Y)"), DisplayNameAttribute("AVERAGE FILTER TYPE")]
        public AVERAGE_FILTER_TYPES AVERAGE_FILTER_TYPE { get; set; } = AVERAGE_FILTER_TYPES.Y;

        public enum AVERAGE_FILTER_TYPES
        {
            X,
            Y
        }

        [Browsable(true)]
        [CategoryAttribute("Draw"), DescriptionAttribute("검사 Draw시 수직선 라인을 Draw할지 결정합니다."), DisplayNameAttribute("SHOW VERTICAL LINE")]
        public bool SHOW_VERTICAL_LINE { get; set; } = true;

        [Browsable(true)]
        [CategoryAttribute("Draw"), DescriptionAttribute("검사 Draw시 엣지를 Draw할지 결정합니다."), DisplayNameAttribute("SHOW EDGE")]
        public bool SHOW_EDGE { get; set; } = true;

        [Browsable(true)]
        [CategoryAttribute("Draw"), DescriptionAttribute("검사 Draw시 엣지들의 연결을 Draw할지 결정합니다."), DisplayNameAttribute("SHOW CONTOUR LINE")]
        public bool SHOW_CONTOUR { get; set; } = true;

        [Browsable(true)]
        [CategoryAttribute("Draw"), DescriptionAttribute("검사 Draw시 핏팅 라인을 Draw할지 결정합니다."), DisplayNameAttribute("SHOW FIT LINE")]
        public bool SHOW_FITLINE { get; set; } = true;

        public CPropertyLineGuage() : base() { }
        public CPropertyLineGuage(string strName) : base(strName) { }

        public CPropertyLineGuage DeepCopy() => (CPropertyLineGuage)this.MemberwiseClone();        

        public CPropertyLineGuage LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\VISION\\" + NAME + ".xml";
            CPropertyLineGuage newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyLineGuage>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public CPropertyLineGuage LoadTestConfig(string path)
        {
            string strPath = path;
            CPropertyLineGuage newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyLineGuage>(strPath);
                if (newData != null)
                {
                    return newData;
                }

            }
            this.SaveTestConfig(path);
            return newData = this.LoadTestConfig(path);
        }
    }
}
