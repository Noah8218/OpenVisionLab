using System.Collections.Generic;
using System.Xml.Serialization;
using static OpenVisionLab.Common.ParameterManager;

namespace OpenVisionLab
{
    [System.Xml.Serialization.XmlRoot("CPropertyParam")]
    public class ParameterProperty
    {
        public string NAME { get; set; } = "TEST";

        // �˻� ������ ROI ��ġ(�⺻ ��Ʈ�˻�)
        public List<SpecRectangle> ROIs { get; set; } = new List<SpecRectangle>();
        // �˻� ������ ���� ��ġ(�ɰ˻�)
        public List<SpecAreas> SpecAreas { get; set; } = new List<SpecAreas>();
        public List<SpecDistance> SpecDistance { get; set; } = new List<SpecDistance>();

        // Ŀ���� ü�� ������ �� ���� 1
        [XmlIgnore] public LineGaugeProperty Line_1 { get; set; } = new LineGaugeProperty();

        [XmlElement("CConnectorParm_Line1")]
        public ConnectorParameter ConnectorParameterLine1 { get; set; } = new ConnectorParameter();

        [XmlElement("CConnectorParm_Line2")]
        public ConnectorParameter ConnectorParameterLine2 { get; set; } = new ConnectorParameter();

        [XmlElement("CConnectorParm_Mean")]
        public ConnectorParameter ConnectorParameterMean { get; set; } = new ConnectorParameter();

        // Ŀ���� ü�� ������ �� ���� 2
        [XmlIgnore] public LineGaugeProperty Line_2 { get; set; } = new LineGaugeProperty();

        // Ŀ���� ü�� ������ �� Mean ������Ƽ
        [XmlIgnore] public MeanProperty Mean { get; set; } = new MeanProperty();

        // �˻� �������� �����ǥ ����� �� ��Ī �Ķ���Ͱ�
        [XmlIgnore] public MatchingProperty Matching { get; set; } = new MatchingProperty();

        // �⺻ Blob �Ķ���� ����
        [XmlIgnore] public BlobProperty Blob { get; set; } = new BlobProperty();

        // ������ ��ġ(������ Ʋ������ Ʋ������ŭ �����ؼ� �̹����� ����
        public double Master_T { get; set; } = 0;

        // ������ ���� X
        // �ش� ��ġ�� ��ŭ ROI X ����
        public double CenterX { get; set; } = 0;

        // ������ ���� Y
        // �ش� ��ġ�� ��ŭ ROI Y ����
        public double CenterY { get; set; } = 0;

        // ���� ������
        // ex : 10�ȼ�
        public int SpecSize { get; set; } = 10;

        // Ŀ���� ü�� ���� �˻縦 ���� max/min�� �ϱ� ���� �Ķ����
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
