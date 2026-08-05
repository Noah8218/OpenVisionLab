using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using System.Xml.Serialization;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using OpenVisionLab.Vision2D.Property;
using OpenCvSharp;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    [CategoryOrder("Parameter", 0)]
    [CategoryOrder("Matching", 1)]
    [CategoryOrder("Scale", 2)]
    [CategoryOrder("Search", 3)]
    [CategoryOrder("ROI", 4)]
    [CategoryOrder("Threshold", 5)]
    [CategoryOrder("Image Process", 6)]
    [System.Xml.Serialization.XmlRoot("CPropertyMatching")]
    public class MatchingProperty : OpenCvPropertyBase, IOpenCVPropertyMatching, IOpenCvConfigurableProperty<MatchingProperty>
    {
        [PropertyOrder(0)]
        [PropertyEditor(typeof(WpgMatchEditor))]
        [CategoryAttribute("Matching"), DescriptionAttribute("입력 레이어에서 등록한 템플릿 이미지 경로입니다."), DisplayNameAttribute("Pattern path")]
        public string PATTERN_PATH { get; set; } = "";

        [PropertyOrder(1)]
        [CategoryAttribute("Matching"), DescriptionAttribute("파라미터를 바꿀 때마다 자동으로 미리보기를 실행할지 결정합니다. 각도 탐색처럼 오래 걸릴 수 있는 조건에서는 끄고 수동으로 미리보기를 실행하는 것이 안전합니다."), DisplayNameAttribute("Auto preview")]
        public bool AUTO_PREVIEW { get; set; } = false;

        [PropertyOrder(2)]
        [CategoryAttribute("Matching"), DescriptionAttribute("템플릿 매칭 점수 계산 방식입니다. 조명 변화가 있으면 정규화 계열을 우선 비교합니다."), DisplayNameAttribute("Match mode")]
        public TemplateMatchModes MATCH_MODE { get; set; } = TemplateMatchModes.CCoeffNormed;

        [PropertyOrder(3)]
        [CategoryAttribute("Matching"), DescriptionAttribute("매칭 결과로 인정할 최소 점수입니다. 높을수록 약한 후보를 더 엄격하게 제외합니다."), DisplayNameAttribute("Min score")]
        public double SCORE_MIN { get; set; } = 0.6D;

        [PropertyOrder(4)]
        [CategoryAttribute("Matching"), DescriptionAttribute("검출할 최대 매칭 개수입니다. 1이면 가장 좋은 위치 1개만 사용합니다."), DisplayNameAttribute("Match count")]
        public int NUM_MATCH { get; set; } = 3;

        [PropertyOrder(5)]
        [CategoryAttribute("Matching"), DescriptionAttribute("계산용 작업 이미지를 줄여 검색하는 기존 피라미드 배율입니다. 대상 크기 변화 보정은 Scale 카테고리의 scale search를 사용합니다."), DisplayNameAttribute("Magnification")]
        public double MAGNIFIATION { get; set; } = 1.0D;

        [PropertyOrder(6)]
        [CategoryAttribute("Matching"), DescriptionAttribute("회전된 템플릿까지 탐색할지 결정합니다. 사용하면 각도 범위와 각도 간격 기준으로 반복 검색합니다."), DisplayNameAttribute("Use angle search")]
        public bool USE_FIND_ANGLE { get; set; } = true;

        [PropertyOrder(8)]
        [CategoryAttribute("Matching"), DescriptionAttribute("각도 탐색 시 후보 각도를 증가시키는 간격입니다. 작을수록 정밀하지만 검색 시간이 늘어납니다."), DisplayNameAttribute("Angle step")]
        public double FIND_ANGLE { get; set; } = 0.1D;

        [PropertyOrder(9)]
        [Browsable(true)]
        [CategoryAttribute("Matching"), DescriptionAttribute("각도 탐색의 상한값입니다."), DisplayNameAttribute("Max angle")]
        public int FIND_ANGLE_MAX { get; set; } = 10;

        [PropertyOrder(7)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(-180, 180, 1, 0, nameof(FIND_ANGLE_MIN), nameof(FIND_ANGLE_MAX))]
        [CategoryAttribute("Matching"), DescriptionAttribute("패턴 매칭에서 탐색할 회전 각도 범위입니다."), DisplayNameAttribute("Angle range")]
        public int FIND_ANGLE_MIN { get; set; } = -10;

        [PropertyOrder(10)]
        [CategoryAttribute("Matching"), DescriptionAttribute("넓은 각도 범위를 모두 세밀하게 검사하지 않고, 먼저 큰 간격으로 후보 각도를 찾은 뒤 상위 후보 주변만 Angle step으로 다시 검사합니다. -10~180도처럼 범위가 넓고 Angle step이 작을 때 켜면 유용합니다."), DisplayNameAttribute("Coarse angle search")]
        public bool USE_COARSE_TO_FINE_ANGLE_SEARCH { get; set; } = false;

        [PropertyOrder(11)]
        [CategoryAttribute("Matching"), DescriptionAttribute("Coarse angle search의 1차 탐색 간격입니다. 예: 5이면 전체 범위를 5도 단위로 먼저 검사하고, 선택된 후보 주변만 Angle step으로 정밀 검사합니다."), DisplayNameAttribute("Coarse angle step")]
        public double COARSE_ANGLE_STEP { get; set; } = 5.0D;

        [PropertyOrder(12)]
        [CategoryAttribute("Matching"), DescriptionAttribute("1차 탐색에서 정밀 재검사할 상위 후보 각도 개수입니다. 값이 클수록 놓칠 가능성은 줄지만 검사 시간이 늘어납니다."), DisplayNameAttribute("Coarse top K")]
        public int COARSE_ANGLE_TOP_K { get; set; } = 3;

        [PropertyOrder(0)]
        [CategoryAttribute("Scale"), DescriptionAttribute("등록한 템플릿보다 대상이 작거나 커질 수 있을 때 여러 크기의 템플릿을 검색합니다. 범위가 넓을수록 시간이 늘어납니다."), DisplayNameAttribute("Use scale search")]
        public bool USE_FIND_SCALE { get; set; } = false;

        [PropertyOrder(1)]
        [CategoryAttribute("Scale"), DescriptionAttribute("검색할 가장 작은 대상 크기입니다. 1.0은 등록 템플릿 원본 크기, 0.9는 90% 크기입니다."), DisplayNameAttribute("Min scale")]
        public double FIND_SCALE_MIN { get; set; } = 0.9D;

        [PropertyOrder(2)]
        [CategoryAttribute("Scale"), DescriptionAttribute("검색할 가장 큰 대상 크기입니다. 1.1은 등록 템플릿보다 110% 큰 대상을 의미합니다."), DisplayNameAttribute("Max scale")]
        public double FIND_SCALE_MAX { get; set; } = 1.1D;

        [PropertyOrder(3)]
        [CategoryAttribute("Scale"), DescriptionAttribute("스케일 검색 간격입니다. 작을수록 정밀하지만 검색 시간이 늘어납니다. 티칭 확인은 0.02~0.05부터 시작합니다."), DisplayNameAttribute("Scale step")]
        public double FIND_SCALE_STEP { get; set; } = 0.05D;

        [PropertyOrder(0)]
        [CategoryAttribute("Search"), DescriptionAttribute("Scale search가 느릴 때 작은 이미지에서 먼저 위치 후보를 찾고, 원본 해상도에서는 후보 주변만 검증합니다. 현재는 각도 탐색이 꺼진 scale search에서 사용합니다."), DisplayNameAttribute("Pyramid proposal")]
        public bool USE_PYRAMID_POSITION_PROPOSAL { get; set; } = false;

        [PropertyOrder(1)]
        [CategoryAttribute("Search"), DescriptionAttribute("스케일별로 원본 검증까지 가져갈 위치 후보 개수입니다. 값이 클수록 안정적이지만 검증 시간이 늘어납니다."), DisplayNameAttribute("Proposal top N")]
        public int PYRAMID_POSITION_TOP_N { get; set; } = 8;

        [PropertyOrder(2)]
        [CategoryAttribute("Search"), DescriptionAttribute("작은 이미지에서 후보로 인정할 최소 점수입니다. 후보가 약하면 기존 전체 검색으로 fallback합니다."), DisplayNameAttribute("Proposal min score")]
        public double PYRAMID_POSITION_MIN_SCORE { get; set; } = 0.70D;

        [PropertyOrder(0)]
        [CategoryAttribute("Image Process"), DescriptionAttribute("매칭 전에 Canny 엣지 이미지를 사용할지 결정합니다."), DisplayNameAttribute("Use canny")]
        public bool USE_CANNY { get; set; } = false;
        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Image Process"), DescriptionAttribute("Canny 전처리의 High 임계값입니다."), DisplayNameAttribute("Canny high")]
        public int CANNY_HIGH { get; set; } = 60;
        [PropertyOrder(1)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(0, 255, 1, 0, nameof(CANNY_LOW), nameof(CANNY_HIGH))]
        [CategoryAttribute("Image Process"), DescriptionAttribute("Canny 전처리에 사용할 Low/High 임계값 범위입니다."), DisplayNameAttribute("Canny range")]
        public int CANNY_LOW { get; set; } = 30;

        [PropertyOrder(3)]
        [CategoryAttribute("Image Process"), DescriptionAttribute("회전/전처리 과정에서 생기는 외곽 영역을 흰색으로 채울지 결정합니다."), DisplayNameAttribute("Use white padding")]
        public bool USE_PADDING_COLOR_WHITE { get; set; } = false;

        internal Mat ImageTemplate { get; set; } = new Mat();

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
            return LoadTestConfigFile<MatchingProperty>(path);
        }

        private static void LoadTemplateImage(MatchingProperty property)
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
        #endregion

    }
}
