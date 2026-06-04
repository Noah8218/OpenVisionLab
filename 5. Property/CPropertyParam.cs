using Lib.Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using static OpenVisionLab._2._Common.CParameterManager;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public class CPropertyParam
    {
        public string NAME { get; set; } = "TEST";        

        // 검사 영역의 ROI 위치(기본 볼트검사)
        public List<CRectangle> ROIs = new List<CRectangle>();
        // 검사 영역의 스팩 위치(핀검사)
        public List<CSpecAreas> SpecAreas = new List<CSpecAreas>();
        public List<CSpecDistance> SpecDistance = new List<CSpecDistance>();

        // 커넥터 체결 유무를 볼 라인 1
        [XmlIgnore] public CPropertyLineGuage Line_1 { get; set; } = new CPropertyLineGuage();

        public CConnectorParm CConnectorParm_Line1 { get; set; } = new CConnectorParm();
        public CConnectorParm CConnectorParm_Line2 { get; set; } = new CConnectorParm();
        public CConnectorParm CConnectorParm_Mean { get; set; } = new CConnectorParm();

        // 커넥터 체결 유무를 볼 라인 2
        [XmlIgnore] public CPropertyLineGuage Line_2 { get; set; } = new CPropertyLineGuage();
        
        // 커넥터 체결 유무를 볼 Mean 프로퍼티
        [XmlIgnore] public CPropertyMean Mean { get; set; } = new CPropertyMean();

        // 검사 영역마다 상대좌표 계산을 할 매칭 파라미터값
        [XmlIgnore] public CPropertyMatching Matching { get; set; } = new CPropertyMatching();

        // 기본 Blob 파라미터 설정
        [XmlIgnore] public CPropertyBlob Blob { get; set; } = new CPropertyBlob();

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

        public CPropertyParam(string strName) { NAME = strName; }    
        public CPropertyParam() { }
    
        public CPropertyParam DeepCopy()
        {
            CPropertyParam temp = (CPropertyParam)this.MemberwiseClone();
            return temp;
        }

        #region CONFIG BY XML              
        public CPropertyParam LoadConfig(string strName)
        {
            CUtil.InitDirectory($"RECIPE\\{strName}");

            string strPath = Application.StartupPath + "\\RECIPE\\" + strName + "\\" + NAME + ".xml";
            CPropertyParam newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyParam>(strPath);
                if (newData != null)
                {
                    newData.Matching.NAME = $"{NAME}-Matching";
                    newData.Matching = (CPropertyMatching)newData.Matching.LoadConfig(strName);

                    newData.Blob.NAME = $"{NAME}-Blob";
                    newData.Blob = newData.Blob.LoadConfig(strName);

                    newData.Line_1.NAME = $"{NAME}-Line_1";
                    newData.Line_1 = (CPropertyLineGuage)newData.Line_1.LoadConfig(strName);

                    newData.Line_2.NAME = $"{NAME}-Line_2";
                    newData.Line_2 = (CPropertyLineGuage)newData.Line_2.LoadConfig(strName);

                    newData.Mean.NAME = $"{NAME}-Mean";
                    newData.Mean = newData.Mean.LoadConfig(strName);

                    return newData;
                }

                else newData = new CPropertyParam(NAME);
            }
            else
            {
                SaveConfig(strName);
                this.LoadConfig(strName);
            }

            this.SaveConfig(strName);
            return newData = this.LoadConfig(strName);
        }

        public void SaveConfig(string strName)
        {
            CUtil.InitDirectory($"RECIPE\\{strName}");

            string strPath = Application.StartupPath + "\\RECIPE\\" + strName + "\\" + NAME + ".xml";
            SerializeHelper.ToXmlFile(strPath, this);

            Matching.NAME = $"{NAME}-Matching";
            Matching.SaveConfig(strName);

            Blob.NAME = $"{NAME}-Blob";
            Blob.SaveConfig(strName);

            Line_1.NAME = $"{NAME}-Line_1";
            Line_1.SaveConfig(strName);

            Line_2.NAME = $"{NAME}-Line_2";
            Line_2.SaveConfig(strName);

            Mean.NAME = $"{NAME}-Mean";
            Mean.SaveConfig(strName);

            //LoadConfig(strName);
        }
        #endregion
    }
}
