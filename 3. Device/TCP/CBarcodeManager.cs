using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace KtemVisionSystem.Device
{
    public class CBarcodeManager
    {
        public EventHandler<StringEventArgs> EventRecvMetalTrayBCR;
        public EventHandler<StringEventArgs> EventRecvTopCoverBCR;

        CTCPAsync BCR_MetalTray = new CTCPAsync();
        CTCPAsync BCR_TopCover = new CTCPAsync();

        public CPropertySocket PropertyMetalTray = new CPropertySocket("BCR_METAL_TRAY");
        public CPropertySocket PropertyTopCover = new CPropertySocket("BCR_TOP_COVER");

        public string MetalBCR = "";
        public string CoverBCR = "";

        public CBarcodeManager()
        {
            
        }

        ~ CBarcodeManager()
        {

        }

        public bool Init()
        {
            try
            {
                // IP, Port 관리필요
                //MetalTray.LoadConfig();
                //Test
                //BCR_MetalTray.CloseClient();
                //BCR_TopCover.CloseClient();

                LoadConfig();

                PropertyMetalTray.LoadConfig();
                PropertyTopCover.LoadConfig();

                BCR_MetalTray.SetAddress(PropertyMetalTray.IP, int.Parse(PropertyMetalTray.PORT));
                //MetalTray.SetAddress("192.168.189.2", 2003);
                BCR_MetalTray.Connect();
                BCR_MetalTray.SetCallbackReceive(OnControlClinetReceiveFunctionMetal);

                BCR_TopCover.SetAddress(PropertyTopCover.IP, int.Parse(PropertyTopCover.PORT));
                //CoverTray.SetAddress("192.168.188.2", 2001);
                BCR_TopCover.Connect();
                BCR_TopCover.SetCallbackReceive(OnControlClinetReceiveFunctionCover);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);

                return false;
            }

            return true;
        }

        /// <summary>
        /// 바코드 Read 트리거 신호, 트리거 후 자동으로 값을 받아서 콜벡처리
        /// </summary>
        public void StartReadMetalBcr()
        {
            try
            {
                MetalBCR = "";
                if (BCR_MetalTray != null) BCR_MetalTray.Send("<T>");

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void StartReadCoverBcr()
        {
            try
            {
                CoverBCR = "";
                if (BCR_TopCover != null) BCR_TopCover.Send("<T>");

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        /// <summary>
        /// 값이 들어오는 콜벡함수
        /// </summary>
        /// <param name="ar"></param>
        private void OnControlClinetReceiveFunctionMetal(IAsyncResult ar)
        {
            byte[] byData;
            string sMsg;

            // 만약 데이터가 있다면
            while (BCR_MetalTray.GetByteData(out byData))
            {
                string strData = Encoding.ASCII.GetString(byData, 0, byData.Length);
                if(strData != "\r\n" && strData != "") MetalBCR = Encoding.ASCII.GetString(byData, 0, byData.Length);

                if(EventRecvMetalTrayBCR != null)
                {
                    EventRecvMetalTrayBCR(this, new StringEventArgs(MetalBCR));
                }
            }
        }

        private void OnControlClinetReceiveFunctionCover(IAsyncResult ar)
        {
            byte[] byData;
            string sMsg;

            // 만약 데이터가 있다면
            while (BCR_TopCover.GetByteData(out byData))
            {
                string strData = Encoding.ASCII.GetString(byData, 0, byData.Length);
                if (strData != "\r\n" && strData != "") CoverBCR = Encoding.ASCII.GetString(byData, 0, byData.Length);

                if (EventRecvTopCoverBCR != null)
                {
                    EventRecvTopCoverBCR(this, new StringEventArgs(CoverBCR));
                }
            }
        }

        #region CONFIG BY XML              
        private string m_XMLName = "PROPERTY_BCR";
        public int BCR_RETRY_COUNT { get; set; } = 3;

        public bool LoadConfig()
        {
            try
            {
                string strPath = Application.StartupPath + "\\CONFIG\\DEVICE\\BCR.xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                switch (xmlReader.Name)
                                {
                                    case "BCR_RETRY_COUNT": if (xmlReader.Read()) BCR_RETRY_COUNT = int.Parse(xmlReader.Value); break;
                                }
                            }
                            else
                            {
                                if (xmlReader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (xmlReader.Name == m_XMLName) break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CLOG.ABNORMAL( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    SaveConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }
        public bool SaveConfig()
        {
            string strPath = Application.StartupPath + "\\CONFIG\\DEVICE\\BCR.xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("PROPERTY");
                xmlWriter.WriteElementString("BCR_RETRY_COUNT", BCR_RETRY_COUNT.ToString());

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return true;
        }
        #endregion
    }
}
