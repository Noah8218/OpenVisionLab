using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

using System.IO.Ports;
using OpenCvSharp;
using System.Threading;

namespace KtemVisionSystem
{
    public partial class ILightController_ATL : Component
    {
        enum ENPAGE { P1, P2, P3, P4, P5, P6, P7, P8, MAX }
        enum ENCHANNEL { CH1, CH2, CH3, CH4, CH5, CH6, CH7, CH8, MAX }
        public ILightController_ATL()
        {
            InitializeComponent();
        }

        public string PortName { get; set; } = "COM10";
        public int BaudRate { get; set; } = 19200;
        public bool IsOpen { get { return m_SerialPort.IsOpen; } }
        public int ChannelCount { get; set; } = (int)ENCHANNEL.MAX;
        public bool StartOn { get; set; } = true;

        public int[] nTimeDelay { get; set; } = new int[(int)ENCHANNEL.MAX];
        public int[] nTimeStrobe { get; set; } = new int[(int)ENCHANNEL.MAX];

        public bool Init()
        {
            try
            {
                LoadConfig();
                
                m_SerialPort = new SerialPort();
                m_SerialPort.PortName = PortName;
                m_SerialPort.BaudRate = BaudRate;
                m_SerialPort.DataBits = 8;
                m_SerialPort.Parity = Parity.None;
                
                m_SerialPort.Open();

                if (m_SerialPort.IsOpen)
                {
                    for (int i = 0; i < (int)ENCHANNEL.MAX; i++)
                    {
                        //SetDelayStrobeTime(0, i, nTimeDelay[i], nTimeStrobe[i]);
                        SetDelayTime(0, i, nTimeDelay[i]);
                        string str = m_SerialPort.ReadLine();
                    }
                    CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                if (m_SerialPort.IsOpen)
                {
                    m_SerialPort.DiscardInBuffer();
                    m_SerialPort.DiscardOutBuffer();

                    m_SerialPort.Close();
                    m_SerialPort.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

#region CONFIG BY XML              
        private string m_XMLName = "LIGHTCONTROLLER";
        public bool LoadConfig()
        {
            try
            {
                string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

                if (File.Exists(strPath))
                {
                    try
                    {
                        string strName;
                        string strDate;

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(strPath);

                        XmlNodeList xmlRoot = xmlDoc.SelectNodes("LIGHT");
                        strDate = xmlRoot[0].Attributes.GetNamedItem("DATE").Value;//2021-04-12 14:40:51.212
                        strName = xmlRoot[0].Name;//LIGHT
                        {
                            XmlNodeList nodeCh = xmlRoot[0].ChildNodes;// CH 1

                            for (int i = 0; i < nodeCh.Count; i++)
                            {
                                strName = nodeCh[i].Name;
                                int.TryParse(nodeCh[i].Attributes.GetNamedItem("DELAY_TIME").Value, out nTimeDelay[i]);
                                int.TryParse(nodeCh[i].Attributes.GetNamedItem("STROBE_TIME").Value, out nTimeStrobe[i]);
                            }
                        }
                    }
                    catch (XmlException ex)
                    {
                        CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    }
                    catch (FileNotFoundException ex)
                    {
                        CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    }
                    catch (System.Exception ex)
                    {
                        CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    }
                }
                else
                {
                    SaveConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveConfig()
        {
            string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

            try
            {
                XmlDocument xmlMain = new XmlDocument();
                XmlDeclaration xmlDec = xmlMain.CreateXmlDeclaration("1.0", "utf-8", null);// 선언
                xmlMain.AppendChild(xmlDec);// xmlMain의 자식 노드로 추가
                XmlElement root = xmlMain.CreateElement("LIGHT");
                root.SetAttribute("DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                {
                    for (int nCH = 0; nCH < (int)ENCHANNEL.MAX; nCH++)
                    {
                        XmlElement Element = xmlMain.CreateElement(Enum.GetName(typeof(ENCHANNEL), nCH));
                        Element.SetAttribute("DELAY_TIME", nTimeDelay[nCH].ToString());
                        Element.SetAttribute("STROBE_TIME", nTimeStrobe[nCH].ToString());
                        root.AppendChild(Element);
                    }
                }

                xmlMain.AppendChild(root);
                xmlMain.Save(strPath);
            }
            catch (XmlException ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            catch (System.Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public string ValuesToString(int[] list)
        {
            string strValues = "";

            try
            {
                for (int i = 0; i < list.Length; i++)
                {
                    if (list.Length - 1 != i)
                    {
                        strValues += list[i].ToString() + ",";
                    }
                    else
                    {
                        strValues += list[i].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return strValues;
            }

            return strValues;
        }
        #endregion

#region Light Control 함수
        private void m_SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = string.Empty;
        }
        /* DelayTime 시간이 흐른후에 ON
         * nPage : 0x00~0x07(0~7)
         * nCh : 0x00 ~ 0x07(0~7)
         * nTime : 시간 단위 : 0x0000~0x1770(0~6000)usec(마이크로초)(1 sec->1,000 msec->1,000,000 usec)
         */
        public bool SetDelayTime(int nPage, int nCh, int nTime)
        {
            try
            {
                if (nPage < 0 || 7 < nPage)
                    return false;
                if (nCh < 0 || 7 < nCh)
                    return false;
                if (nTime < 0 || 6000 < nTime)
                    return false;

                byte[] byData = new byte[9];
                byData[0] = 0x4C;// Header
                byData[1] = 0x12;// Command
                byData[2] = (byte)nPage;// Page : 0x00~0x07(0~7)
                byData[3] = (byte)nCh;// Channel : 0x00 ~ 0x07(0~7)
                byData[4] = BitConverter.GetBytes(nTime)[1];// Delay High Value : 0x00 ~ 0x17(0~23)
                byData[5] = BitConverter.GetBytes(nTime)[0];// Delay Low Value : 0x00 ~ 0xFF(0~255)
                byData[6] = (byte)(byData[1] ^ byData[2] ^ byData[3] ^ byData[4] ^ byData[5]);// checkSum : XOR
                byData[7] = 0x0D;// Tail1
                byData[8] = 0x0A;// Tail2

                if (m_SerialPort.IsOpen) m_SerialPort.Write(byData, 0, byData.Length);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }
        /* StrobeTime : ON이 유지되는 시간
         * nPage : 0x00~0x07(0~7)
         * nCh : 0x00 ~ 0x17(0~23)
         * 시간 단위 : 0~6000usec(마이크로초)(1 sec->1,000 msec->1,000,000 usec)
        */
        public bool SetStrobeTime(int nPage, int nCh, int nTime)
        {
            try
            {
                if (nPage < 0 || 7 < nPage)
                    return false;
                if (nCh < 0 || 7 < nCh)
                    return false;
                if (nTime < 0 || 6000 < nTime)
                    return false;

                byte[] byData = new byte[9];
                byData[0] = 0x4C;// Header
                byData[1] = 0x13;// Command
                byData[2] = (byte)nPage;// Page : 0x00~0x07(0~7)
                byData[3] = (byte)nCh;// Channel : 0x00 ~ 0x07(0~7)
                byData[4] = BitConverter.GetBytes(nTime)[1];// Delay High Value : 0x00 ~ 0x17(0~23)
                byData[5] = BitConverter.GetBytes(nTime)[0];// Delay Low Value : 0x00 ~ 0xFF(0~255)
                byData[6] = (byte)(byData[1] ^ byData[2] ^ byData[3] ^ byData[4] ^ byData[5]);// checkSum : XOR
                byData[7] = 0x0D;// Tail1
                byData[8] = 0x0A;// Tail2

                if (m_SerialPort.IsOpen) m_SerialPort.Write(byData, 0, byData.Length);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }
        /* DelayTime,StrobeTime : 동시에 설정
         * nPage : 0x00~0x07(0~7)
         * nCh : 0x00 ~ 0x17(0~23)
         * 시간 단위 : 0~6000usec(마이크로초)(1 sec->1,000 msec->1,000,000 usec)
        */
        public bool SetDelayStrobeTime(int nPage, int nCh, int nDelayTime, int nStrobeTime)
        {
            try
            {
                if (nPage < 0 || 7 < nPage)
                    return false;
                if (nCh < 0 || 7 < nCh)
                    return false;
                if (nDelayTime < 0 || 6000 < nDelayTime)
                    return false;
                if (nStrobeTime < 0 || 6000 < nStrobeTime)
                    return false;

                byte[] byData = new byte[11];
                byData[0] = 0x4C;// Header
                byData[1] = 0x14;// Command
                byData[2] = (byte)nPage;// Page : 0x00~0x07(0~7)
                byData[3] = (byte)nCh;// Channel : 0x00 ~ 0x07(0~7)
                byData[4] = BitConverter.GetBytes(nDelayTime)[1];// Delay High Value : 0x00 ~ 0x17(0~23)
                byData[5] = BitConverter.GetBytes(nDelayTime)[0];// Delay Low Value : 0x00 ~ 0xFF(0~255)
                byData[6] = BitConverter.GetBytes(nStrobeTime)[1];// Delay High Value : 0x00 ~ 0x17(0~23)
                byData[7] = BitConverter.GetBytes(nStrobeTime)[0];// Delay Low Value : 0x00 ~ 0xFF(0~255)
                byData[8] = (byte)(byData[1] ^ byData[2] ^ byData[3] ^ byData[4] ^ byData[5] ^ byData[6] ^ byData[7]);// checkSum : XOR
                byData[9] = 0x0D;// Tail1
                byData[10] = 0x0A;// Tail2

                if (m_SerialPort.IsOpen)
                    m_SerialPort.Write(byData, 0, byData.Length);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }
        #endregion Light Control 함수

    }

    public class ILightController_ATL_TCP
    {
        public EventHandler<StringEventArgs> EventRecvMsg;

        CTCPAsync TCPSocket = new CTCPAsync();

        public CPropertySocket PropertyTCP = new CPropertySocket("LightControl_ATL");

        public string strMessage = "";

        public ILightController_ATL_TCP()
        {

        }

        ~ILightController_ATL_TCP()
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

                PropertyTCP.LoadConfig();

                TCPSocket.SetAddress(PropertyTCP.IP, int.Parse(PropertyTCP.PORT));
                //MetalTray.SetAddress("192.168.189.2", 2003);
                TCPSocket.Connect();
                TCPSocket.SetCallbackReceive(OnControlClinetReceiveFunctionMetal);
                SetDelayTime(0, 0, 1000);
                SetStrobeTime(0, 0, 2000);
                SetDelayStrobeTime(0, 0, 4000, 4000);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);

                return false;
            }

            return true;
        }
        /* DelayTime 시간이 흐른후에 ON
         * nPage : 0x00~0x07(0~7)
         * nCh : 0x00 ~ 0x07(0~7)
         * nTime : 시간 단위 : 0x0000~0x1770(0~6000)usec(마이크로초)(1 sec->1,000 msec->1,000,000 usec)
         */
        public bool SetDelayTime(int nPage, int nCh, int nTime)
        {
            try
            {
                if (nPage < 0 || 7 < nPage)
                    return false;
                if (nCh < 0 || 7 < nCh)
                    return false;
                if (nTime < 0 || 6000 < nTime)
                    return false;

                byte[] byData = new byte[9];
                byData[0] = 0x4C;// Header
                byData[1] = 0x12;// Command
                byData[2] = (byte)nPage;// Page : 0x00~0x07(0~7)
                byData[3] = (byte)nCh;// Channel : 0x00 ~ 0x07(0~7)
                byData[4] = BitConverter.GetBytes(nTime)[1];// Delay High Value : 0x00 ~ 0x17(0~23)
                byData[5] = BitConverter.GetBytes(nTime)[0];// Delay Low Value : 0x00 ~ 0xFF(0~255)
                byData[6] = (byte)(byData[1] ^ byData[2] ^ byData[3] ^ byData[4] ^ byData[5]);// checkSum : XOR
                byData[7] = 0x0D;// Tail1
                byData[8] = 0x0A;// Tail2

                if (TCPSocket != null)
                    TCPSocket.Send(byData);
                byte[] byRecv;
                TCPSocket.GetByteData(out byRecv);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }
        /* StrobeTime : ON이 유지되는 시간
         * nPage : 0x00~0x07(0~7)
         * nCh : 0x00 ~ 0x17(0~23)
         * 시간 단위 : 0~6000usec(마이크로초)(1 sec->1,000 msec->1,000,000 usec)
        */
        public bool SetStrobeTime(int nPage, int nCh, int nTime)
        {
            try
            {
                if (nPage < 0 || 7 < nPage)
                    return false;
                if (nCh < 0 || 7 < nCh)
                    return false;
                if (nTime < 0 || 6000 < nTime)
                    return false;

                byte[] byData = new byte[9];
                byData[0] = 0x4C;// Header
                byData[1] = 0x13;// Command
                byData[2] = (byte)nPage;// Page : 0x00~0x07(0~7)
                byData[3] = (byte)nCh;// Channel : 0x00 ~ 0x07(0~7)
                byData[4] = BitConverter.GetBytes(nTime)[1];// Delay High Value : 0x00 ~ 0x17(0~23)
                byData[5] = BitConverter.GetBytes(nTime)[0];// Delay Low Value : 0x00 ~ 0xFF(0~255)
                byData[6] = (byte)(byData[1] ^ byData[2] ^ byData[3] ^ byData[4] ^ byData[5]);// checkSum : XOR
                byData[7] = 0x0D;// Tail1
                byData[8] = 0x0A;// Tail2

                if (TCPSocket != null)
                    TCPSocket.Send(byData);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }
        /* DelayTime,StrobeTime : 동시에 설정
         * nPage : 0x00~0x07(0~7)
         * nCh : 0x00 ~ 0x17(0~23)
         * 시간 단위 : 0~6000usec(마이크로초)(1 sec->1,000 msec->1,000,000 usec)
        */
        public bool SetDelayStrobeTime(int nPage, int nCh, int nDelayTime, int nStrobeTime)
        {
            try
            {
                if (nPage < 0 || 7 < nPage)
                    return false;
                if (nCh < 0 || 7 < nCh)
                    return false;
                if (nDelayTime < 0 || 6000 < nDelayTime)
                    return false;
                if (nStrobeTime < 0 || 6000 < nStrobeTime)
                    return false;

                byte[] byData = new byte[11];
                byData[0] = 0x4C;// Header
                byData[1] = 0x14;// Command
                byData[2] = (byte)nPage;// Page : 0x00~0x07(0~7)
                byData[3] = (byte)nCh;// Channel : 0x00 ~ 0x07(0~7)
                byData[4] = BitConverter.GetBytes(nDelayTime)[1];// Delay High Value : 0x00 ~ 0x17(0~23)
                byData[5] = BitConverter.GetBytes(nDelayTime)[0];// Delay Low Value : 0x00 ~ 0xFF(0~255)
                byData[6] = BitConverter.GetBytes(nStrobeTime)[1];// Delay High Value : 0x00 ~ 0x17(0~23)
                byData[7] = BitConverter.GetBytes(nStrobeTime)[0];// Delay Low Value : 0x00 ~ 0xFF(0~255)
                byData[8] = (byte)(byData[1] ^ byData[2] ^ byData[3] ^ byData[4] ^ byData[5] ^ byData[6] ^ byData[7]);// checkSum : XOR
                byData[9] = 0x0D;// Tail1
                byData[10] = 0x0A;// Tail2

                strMessage = "";
                if (TCPSocket != null) 
                    TCPSocket.Send(byData);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }
        /* 현재 설정되어 있는 모든 데이터를 저장합니다.
         */
        public bool SetSave()
        {
            try
            {
                byte[] byData = new byte[5];
                byData[0] = 0x4C;// Header
                byData[1] = 0x1B;// Command
                byData[8] = 0x19;// checkSum : XOR
                byData[9] = 0x0D;// Tail1
                byData[10] = 0x0A;// Tail2

                strMessage = "";
                if (TCPSocket != null)
                    TCPSocket.Send(byData);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
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
            while (TCPSocket.GetByteData(out byData))
            {
                string strData = Encoding.ASCII.GetString(byData, 0, byData.Length);
                if (strData != "\r\n" && strData != "") strMessage = Encoding.ASCII.GetString(byData, 0, byData.Length);

                if (EventRecvMsg != null)
                {
                    EventRecvMsg(this, new StringEventArgs(strMessage));
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
