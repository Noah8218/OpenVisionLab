using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using static OpenVisionLab.CPropertyGridEditor;
using System.Windows.Controls.WpfPropertyGrid;
using OpenCvSharp;
using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Property;

namespace OpenVisionLab.Vision._1._Tools.OpenCV
{
    [CategoryOrder("Parameter", 0)]
    [CategoryOrder("Threshold", 1)]
    [CategoryOrder("ROI", 2)]
    [CategoryOrder("Image Process", 5)]
    public class COpenCVPropertyBase : DependencyObject, IOpenCVPropertyBase
    {
        public COpenCVPropertyBase(string strName) : base()
        {
            NAME = strName;
        }

        public COpenCVPropertyBase() { }

        [Browsable(true)]
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        [PropertyOrder(0)]
        public string NAME { get; set; } = "";

        [Browsable(true)]
        [PropertyOrder(1)]
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("PIXEL/MM")]
        public double PIXELPERMM { get; set; } = 0.006D;

        [PropertyOrder(0)]
        [CategoryAttribute("Threshold"), DescriptionAttribute("검사를 하기전 이진화처리를 하고 검사를 할지 결정합니다."), DisplayNameAttribute("USE_THRESHOLD")]
        public virtual bool USE_THRESHOLD { get; set; } = true;

        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Threshold"), DescriptionAttribute("True시 이미지를 반전합니다. White -> Black / Black -> White"), DisplayNameAttribute("USE_BITWISENOT")]
        public bool USE_BITWISENOT { get; set; } = false;

        [PropertyOrder(3)]
        [CategoryAttribute("Threshold"), DescriptionAttribute("이진화의 알고리즘의 타입입니다. 옵션에 따라 이진화 결과가 틀립니다."), DisplayNameAttribute("THRESHOLD_TYPE")]
        public virtual ThresholdTypes THRESHOLD_TYPES { get; set; } = ThresholdTypes.Binary;

        [PropertyOrder(4)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [CategoryAttribute("Threshold"), DescriptionAttribute(""), DisplayNameAttribute("THRESHOLD")]
        public virtual double THRESHOLD { get; set; }
        
        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("Threshold"), DescriptionAttribute("검사를 하기전 적응형 이진화처리를 하고 검사를 할지 결정합니다."), DisplayNameAttribute("USE_ADAPTIVE_THRESHOLD")]
        public bool USE_ADAPTIVE_THRESHOLD { get; set; } = false;

        [PropertyOrder(6)]
        [PropertyEditor(typeof(WpgSliderEditor))]
        [CategoryAttribute("Threshold"), DescriptionAttribute(""), DisplayNameAttribute("ADAPTIVE_THRESHOLD")]
        public virtual double ADAPTIVE_THRESHOLD { get; set; }

        [PropertyOrder(7)]
        [CategoryAttribute("Threshold"), DescriptionAttribute("이진화의 알고리즘의 타입입니다. 옵션에 따라 이진화 결과가 틀립니다."), DisplayNameAttribute("ADAPTIVE_THRESHOLD_TYPES")]
        public virtual ThresholdTypes ADAPTIVE_THRESHOLD_TYPES { get; set; } = ThresholdTypes.Binary;

        [PropertyOrder(8)]
        [Browsable(true)]
        [CategoryAttribute("Threshold"), DescriptionAttribute("적응형 이진화의 알고리즘의 타입입니다. 옵션에 따라 이진화 결과가 틀립니다."), DisplayNameAttribute("ADAPTIVE_THRESHOLD_ALGORITHM")]
        public AdaptiveThresholdTypes ADAPTIVE_THRESHOLD_ALGORITHM { get; set; } = AdaptiveThresholdTypes.GaussianC;

        [PropertyOrder(9)]
        [Browsable(true)]
        [CategoryAttribute("Threshold"), DescriptionAttribute("적응형 이진화의 사이즈(픽셀)입니다. 홀수만 가능합니다."), DisplayNameAttribute("BlockSize")]
        public int BlockSize { get; set; } = 25;

        [PropertyOrder(10)]
        [Browsable(true)]
        [CategoryAttribute("Threshold"), DescriptionAttribute("적응형 이진화의 가중치값 입니다. BlockSize에서 추출한 값에 가중치값을 더합니다."), DisplayNameAttribute("Weight")]
        public int Weight { get; set; } = 5;

        [PropertyOrder(0)]
        [Browsable(true)]
        [CategoryAttribute("ROI"), DescriptionAttribute("사용시 지정한 ROI로 검사하며, 미사용시 이미지 전체를 대상으로 검사합니다."), DisplayNameAttribute("USE_ROI")]
        public bool USE_ROI { get; set; } = true;

        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("ROI"), DescriptionAttribute("사용시 여러개의 ROI로 검사하며, 미사용시 ROI 1개로 검사합니다."), DisplayNameAttribute("USE_MULTI_ROI")]
        public bool USE_MULTI_ROI { get; set; } = false;

        [PropertyOrder(2)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgROIEditor))]
        [CategoryAttribute("ROI"), DescriptionAttribute(""), DisplayNameAttribute("ROI")]
        public OpenCvSharp.Rect CvROI { get; set; } = new OpenCvSharp.Rect();

        [PropertyOrder(3)]
        [Browsable(true)]
        [PropertyEditor(typeof(WpgMultiROIEditor))]
        [TypeConverter(typeof(ListTypeConverter))]// 
        [CategoryAttribute("ROI"), DescriptionAttribute(""), DisplayNameAttribute("ROI 리스트")]
        public List<OpenCvSharp.Rect> CvROIS { get; set; } = new List<OpenCvSharp.Rect>();

        [PropertyOrder(4)]
        [PropertyEditor(typeof(WpgMultiROIEditor))]
        [TypeConverter(typeof(ListTypeConverter))]// 
        [CategoryAttribute("ROI"), DescriptionAttribute("마스킹 영역입니다. 해당 영역에 이물이 검출시 필터링 됩니다."), DisplayNameAttribute("마스킹")]
        public List<OpenCvSharp.Rect> CvMASKS { get; set; } = new List<OpenCvSharp.Rect>();

        #region CONFIG BY XML              
        public void SaveConfig(string RecipeName)
        {
            string strPath = System.Windows.Forms.Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\VISION\\" + NAME + ".xml";
            SerializeHelper.ToXmlFile(strPath, this);
        }

        public void SaveTestConfig(string Path)
        {
            CUtil.InitDirectory("TEST");
            string strPath = Path;
            SerializeHelper.ToXmlFile(strPath, this);
        }
        #endregion
    }
}
