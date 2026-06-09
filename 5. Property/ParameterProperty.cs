using System.Collections.Generic;
using System.Xml.Serialization;
using static OpenVisionLab._2._Common.ParameterManager;

namespace OpenVisionLab
{
    [System.Xml.Serialization.XmlRoot("CPropertyParam")]
    public class ParameterProperty
    {
        public string NAME { get; set; } = "TEST";        

        // 검사 영역의 ROI 위치(기본 볼트검사)
        public List<SpecRectangle> ROIs { get; set; } = new List<SpecRectangle>();
        // 검사 영역의 스팩 위치(핀검사)
        public List<SpecAreas> SpecAreas { get; set; } = new List<SpecAreas>();
        public List<SpecDistance> SpecDistance { get; set; } = new List<SpecDistance>();

        // 커넥터 체결 유무를 볼 라인 1
        [XmlIgnore] public LineGaugeProperty Line_1 { get; set; } = new LineGaugeProperty();

        [XmlElement("CConnectorParm_Line1")]
        public ConnectorParameter ConnectorParameterLine1 { get; set; } = new ConnectorParameter();

        [XmlElement("CConnectorParm_Line2")]
        public ConnectorParameter ConnectorParameterLine2 { get; set; } = new ConnectorParameter();

        [XmlElement("CConnectorParm_Mean")]
        public ConnectorParameter ConnectorParameterMean { get; set; } = new ConnectorParameter();

        // 커넥터 체결 유무를 볼 라인 2
        [XmlIgnore] public LineGaugeProperty Line_2 { get; set; } = new LineGaugeProperty();
        
        // 커넥터 체결 유무를 볼 Mean 프로퍼티
        [XmlIgnore] public MeanProperty Mean { get; set; } = new MeanProperty();

        // 검사 영역마다 상대좌표 계산을 할 매칭 파라미터값
        [XmlIgnore] public MatchingProperty Matching { get; set; } = new MatchingProperty();

        // 기본 Blob 파라미터 설정
        [XmlIgnore] public BlobProperty Blob { get; set; } = new BlobProperty();

        // 마스터 위치(각도가 틀어지면 틀어진만큼 보정해서 이미지를 생성
        public double Master_T { get; set; } = 0;

        // 마스터 센터 X 
        // 해당 위치값 만큼 ROI X 보정
        public double CenterX { get; set; } = 0;

        // 마스터 센터 Y
        // 해당 위치값 만큼 ROI Y 보정
        public double CenterY { get; set; } = 0;

        // 스팩 사이즈
        // ex : 10픽셀
        public int SpecSize { get; set; } = 10;

        // 커넥터 체결 유무 검사를 위해 max/min을 하기 위한 파라미터
        public int ScalarGv { get; set; } = 200;

        public bool UseUpptoBottom { get; set; } = true;

        [XmlIgnore]
        public bool UseUpToBottom
        {
            get => UseUpptoBottom;
            set => UseUpptoBottom = value;
        }

        public ParameterProperty(string strName) { NAME = strName; }    
        public ParameterProperty() { }
    
        public ParameterProperty DeepCopy()
        {
            ParameterProperty temp = (ParameterProperty)this.MemberwiseClone();
            return temp;
        }

        #region CONFIG BY XML              
        public ParameterProperty LoadConfig(string strName)
        {
            return ParameterPropertyStorage.Load(this, strName);
        }

        public void SaveConfig(string strName)
        {
            ParameterPropertyStorage.Save(this, strName);
        }
        #endregion
    }
}
