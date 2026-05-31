using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Concurrent;
using System.Xml.Linq;
using OpenCvSharp;
using System.Collections.Generic;
using System.Xml.Serialization;
using static OpenVisionLab.DEFINE;
using OpenVisionLab._2._Common;
using System.ComponentModel;
using System.Drawing;
using OpenVisionLab._3._Device.DB;
using OpenCvSharp.Aruco;
using Lib.Common;

namespace OpenVisionLab
{   
    public class CData
    {
        // 이미지 큐
        // 해당 큐에 검사 이미지를 넣어서 검사를 진행        
        [XmlIgnore] public ConcurrentQueue<CGrabBuffer> GrabQueue = new ConcurrentQueue<CGrabBuffer>();

        // 해당 디펙값을 넣고 스레드에서 검사
        // 라벨러 관리는 어떻게?
        // 리스트 딕셔너리?
        // 관리 클래스를 하나 만들어도 괜찮음
        [XmlIgnore] public List<List<CAttatchLabelling>> AttachesTemp = new List<List<CAttatchLabelling>>();
        [XmlIgnore] public List<List<CAttatchLabelling>> Attaches = new List<List<CAttatchLabelling>>();
        [XmlIgnore] public List<CAttatchLabelling> Already_AttachedList = new List<CAttatchLabelling>();
        //[XmlIgnore] public List<CAttatchLabelling> AttatchLabels_No1 = new List<CAttatchLabelling>();
        //[XmlIgnore] public List<CAttatchLabelling> AttatchLabels_No2 = new List<CAttatchLabelling>();
        //[XmlIgnore] public List<CAttatchLabelling> AttatchLabels_No3 = new List<CAttatchLabelling>();
        //[XmlIgnore] public List<CAttatchLabelling> AttatchLabels_No4 = new List<CAttatchLabelling>();

        // 스팩관련 프로퍼티
        [XmlIgnore] public CPropertySpec SPEC = new CPropertySpec("SPEC");
        [XmlIgnore] public CPropertySetting SETTING = new CPropertySetting("SETTING");

        [XmlIgnore] public List<CPropertyParam> ParamList_Bolt = new List<CPropertyParam>();
        [XmlIgnore] public List<CPropertyParam> ParamList_Pin = new List<CPropertyParam>();
        [XmlIgnore] public List<CPropertyParam> ParamList_Connector = new List<CPropertyParam>();
        [XmlIgnore] public List<CPropertyParam> ParamList_Thermal = new List<CPropertyParam>();

        public int ParamList_Bolt_Count { get; set; } = 0;
        public int ParamList_Pin_Count { get; set; } = 0;
        public int ParamList_Connector_Count { get; set; } = 0;
        public int ParamList_Thermal_Count { get; set; } = 0;

        // 그래프 관리 리스트
        [XmlIgnore] public List<CMvcGraph> GraphList { get; set; } = new List<CMvcGraph>();

        [XmlIgnore] public bool runInsp_1 = false;

        [XmlIgnore] public bool runInsp_2 = false;
        public bool IsSaveImage { get; set; } = false;
        public int AcqusitionFrameCount { get; set; } = 0;

        // 프로그램 종료할 때 1회 저장함        
        public double RealTimeEncoder { get; set; } = 0;

        [XmlIgnore] public int TestEncoder { get; set; } = 0;

        #region COUNT
        public int CountOK { get; set; } = 0;
        public int CountNG_Label { get; set; } = 0;        
        public int CountBypass { get; set; } = 0;
        public int CountTotal { get; set; } = 0;
        [XmlIgnore] public double Total_Dist_PerMM { get; set; } = 0;
        [XmlIgnore] public double Total_Encoder { get; set; } = 0;
        #endregion
        [XmlIgnore] public string LotName { get; set; } = "";
        [XmlIgnore] public Dictionary<string, CDefectSummary> defectSummaries { get; set; } = new Dictionary<string, CDefectSummary>();
        public CData() { CUtil.InitDirectory("DATA"); }
   
        public void ResetCount()
        {
            CountOK = 0;
            CountNG_Label = 0;            
            CountBypass = 0;
            CountTotal = 0;
        }

        /// <summary>
        /// 레시피가 변경될 때마다 프로퍼티값들을 다시 load
        /// </summary>
        /// <param name="RecipeName"></param>
        public void LoadProperty(string RecipeName)
        {
            // 직렬화는 아래와 같이 객체를 Load후 넘겨줘야 한다.
            SPEC = SPEC.LoadConfig(RecipeName);
            SETTING = SETTING.LoadConfig(RecipeName);

            for (int i = 0; i < SETTING.LabelCount; i++) { Attaches.Add(new List<CAttatchLabelling>()); }
            for (int i = 0; i < SETTING.LabelCount; i++) { AttachesTemp.Add(new List<CAttatchLabelling>()); }


            for (int i = 0; i < ParamList_Bolt_Count; i++) { ParamList_Bolt.Add(new CPropertyParam($"{PROCESS_TYPES.Bolt_tightened}-{ParamList_Bolt.Count + 1}")); }
            for (int i = 0; i < ParamList_Bolt.Count; i++)
            {
                ParamList_Bolt[i] = ParamList_Bolt[i].LoadConfig(RecipeName);
            }

        }

        public CData LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\" + "VISION" + ".xml";
            CData newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CData>(strPath);
                if (newData != null)
                {
                    newData.LoadProperty(RecipeName);
                    return newData;
                }
                    
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public void SaveConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\" + "VISION" + ".xml";
            SerializeHelper.ToXmlFile(strPath, this);
        }
    }

    public class CGrabBuffer
    {
        public int Index = 0;                
        public Mat ImageGrab = new Mat();
        public double TotalEncoder = 0;
        public bool TestImage = false;
        public CGrabBuffer(Mat image, int nIndex, double totalEncoder, bool testImage)
        {
            Index = nIndex;
            // 나중에 메모리 증가 사유일수 있음(Deep/Slow 확인 필요)
            image.CopyTo(ImageGrab);
            TotalEncoder = totalEncoder;
            TestImage = testImage;
        }
    }
}
