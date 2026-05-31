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
using System.Threading;

namespace KtemVisionSystem
{
    public partial class CLIGHTCON_JSL_200 : Component
    {
        private const string Light = "Light";

        [CategoryAttribute(Light), DescriptionAttribute(""), DisplayNameAttribute("Channel1Last")]
        public int Channel1Last { get; set; } = 100;
        [CategoryAttribute(Light), DescriptionAttribute(""), DisplayNameAttribute("Channel2Last")]
        public int Channel2Last { get; set; } = 100;
        [CategoryAttribute(Light), DescriptionAttribute(""), DisplayNameAttribute("Channel3Last")]
        public int Channel3Last { get; set; } = 100;
        [CategoryAttribute(Light), DescriptionAttribute(""), DisplayNameAttribute("Channel4Last")]
        public int Channel4Last { get; set; } = 100;
        [CategoryAttribute(Light), DescriptionAttribute(""), DisplayNameAttribute("PortName")]
        public string PortName { get; set; } = "COM1";
        [CategoryAttribute(Light), DescriptionAttribute(""), DisplayNameAttribute("BaudRate")]
        public int BaudRate { get; set; } = 19200;
        [CategoryAttribute(Light), DescriptionAttribute(""), DisplayNameAttribute("MaxChannleCount")]
        public int MaxChannleCount { get; set; } = 4;
        public bool IsOpen { get; set; } = false;

        public CLIGHTCON_JSL_200()
        {
            InitializeComponent();
        }

        public CLIGHTCON_JSL_200(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Close()
        {
            if (m_SP.IsOpen)
            {
                //Off();
                m_SP.Close();
            }
        }
        public bool Init()
        {
            try
            {                
                LoadConfig();

                if (m_SP.IsOpen) m_SP.Close();

                if (!m_SP.IsOpen)
                {
                    m_SP.PortName = "COM1";
                    m_SP.BaudRate = BaudRate;
                    m_SP.Open();

                    if (m_SP.IsOpen)
                    {
                        m_SP.DiscardOutBuffer();
                        m_SP.DiscardInBuffer();

                        IsOpen = true;

                        CheckStrobeData();

                        SetOnTime(1, 2000);
                        SetOnTime(2, 2000);

                        bool bConnection = false;
                        
                        // 아직 체크 소스 못만듬
                        

                        GC.Collect();
                        return true;
                    }
                    else
                    {
                     //   CLog.Error( "[Failed] Light Controller Init");
                        return false;
                    }

                }
                else if (m_SP.IsOpen)
                {
                  // CLog.Error( "[Failed] Light Controller Init");
                    m_SP.Close();
                    return false;
                }

                return true;
            }
            catch (Exception Desc)
            {
                //CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool CheckStrobeData()
        {
            try
            {
                string strSOH = ":";
                string strEqui = "00";
                string strOPCode = "R";
                string strCR = "\r";
                string strLF = "\n";

                string strSendMessage = $"{strSOH}{strEqui}{strOPCode}{strCR}{strLF}";

                //string strHex = stringToHex(strSendMessage);

                //string str = Encoding.ASCII.get(strSendMessage);

                m_SP.WriteLine(strSendMessage);

                int nRec = m_SP.BytesToRead;
                int nCnt = 0;
                int nSleepTime = 10;
                while (nRec <= 0)
                {
                    Thread.Sleep(nSleepTime);
                    nCnt += nSleepTime;
                    nRec = m_SP.BytesToRead;

                    if (nCnt == 2000)
                    {
                        return false;
                    }
                }

                byte[] btRead = new byte[nRec];

                m_SP.Read(btRead, 0, btRead.Length);

                string strReciveData = ByteToString(btRead);

                return true;
            }
            catch (Exception Desc)
            {
                //CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        private string ByteToString(byte[] strByte)
        {

            string str = Encoding.Default.GetString(strByte);
            return str; 
        }        


        /// <summary>
        /// 지정한 채널의 조명을 On합니다.
        /// </summary>
        /// <param name="nChannle"></param>
        public bool SetOnTime(int nChannle, int nValue)
        {
            // 1 ~ 4
            switch(nChannle)
            {
                case 1:
                    Channel1Last = nValue;
                    break;
                case 2:
                    Channel2Last = nValue;
                    break;
                case 3:
                    Channel3Last = nValue;
                    break;
                case 4:
                    Channel4Last = nValue;
                    break;
            }

            // 현재 남아있는 버퍼를 초기화 진행하여 통신간 잘못된 데이터값을 가져오지 않게 한다.
            if (m_SP.IsOpen)
            {                
                m_SP.DiscardOutBuffer();
                m_SP.DiscardInBuffer();
            }

            try
            {

                //(0번 장비, 최대 페이지 1, 1번 페이지, 스트로브횟수 1, 4채널 ON TIME 값 998)
                //: 00 B 01 01 1 998,998,998,998[CR][LF]

                string strSOH = ":";
                string strEqui = "00";
                string strOPCode = "B";
                string strMaxPage = "01";
                string strPage = "01";
                string strStrobeCount = "1";
                string strChannel = string.Format("{0},{1}", Channel1Last, Channel2Last);
                string strCR = "\r";
                string strLF = "\n";

                string strSendMessage = $"{strSOH}{strEqui}{strOPCode}{strMaxPage}{strPage}{strStrobeCount}{strChannel}{strCR}{strLF}";

                m_SP.WriteLine(strSendMessage);

                return true;
            }
            catch (Exception Desc)
            {
              //  CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }
       
        public static string stringToHex(string strData)
        {
            string resultHex = string.Empty;
            byte[] arr_byteStr = Encoding.Default.GetBytes(strData);

            foreach (byte byteStr in arr_byteStr)
                resultHex += string.Format("{0:X2}", byteStr);

            return resultHex;
        }


        public string ByteHexToHexString(byte[] hex)
        {
            string result = string.Empty;
            foreach (byte c in hex)
                result += c.ToString("x2").ToUpper();
            return result;
        }

        public byte[] HexToByte(string strHex)
        {
            byte[] bytes = new byte[strHex.Length / 2];
            try
            {
                for (int count = 0; count < strHex.Length; count += 2)
                {
                    bytes[count / 2] = System.Convert.ToByte(strHex.Substring(count, 2), 16);
                }
                return bytes;
            }
            catch
            {

            }

            return bytes;
        }

        #region File Manager
        private string m_strPath = Application.StartupPath + "\\CONFIG\\DEVICE\\" + "LIGHTCON" + ".xml";
        private string m_XMLName = "LightControllerConfig";
        public bool LoadConfig()
        {
            try
            {
                if (File.Exists(m_strPath))   //  xml 파일 존재 유무 검사
                {
                    XmlTextReader xmlReader = new XmlTextReader(m_strPath);    //  xml 파일 열기

                    try
                    {
                        LoadConfigByXML(xmlReader);
                    }
                    catch (Exception e)
                    {
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
             //   Logger.WriteLog(ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveConfig()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(m_strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();

                SaveConfigByXML(xmlWriter);

                xmlWriter.WriteEndDocument();
            }
            catch (Exception ex)
            {
               // CLog.Error( "[FAILED] LightController Config Save");                
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

         //   CLog.Error( "[SUCCESS] LightController Config Save ==> {0}", m_strPath);
            return true;
        }

        public bool LoadConfigByXML(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "PortName":
                            if (!xmlReader.Read()) return false;
                            PortName = xmlReader.Value;
                            break;
                        case "BaudRate":
                            if (!xmlReader.Read()) return false;
                            BaudRate = int.Parse(xmlReader.Value);
                            break;
                        case "Channel1Last":
                            if (!xmlReader.Read()) return false;
                            Channel1Last = int.Parse(xmlReader.Value);
                            break;
                        case "Channel2Last":
                            if (!xmlReader.Read()) return false;
                            Channel2Last = int.Parse(xmlReader.Value);
                            break;
                        case "Channel3Last":
                            if (!xmlReader.Read()) return false;
                            Channel3Last = int.Parse(xmlReader.Value);
                            break;
                        case "Channel4Last":
                            if (!xmlReader.Read()) return false;
                            Channel4Last = int.Parse(xmlReader.Value);
                            break;
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


        //    Logger.WriteLog(LOG.Normal, "[SUCCESS] LightController Config Load");
            return true;
        }

        public bool SaveConfigByXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Parameter");

            xmlWriter.WriteElementString("PortName", PortName);
            xmlWriter.WriteElementString("BaudRate", BaudRate.ToString());
            xmlWriter.WriteElementString("Channel1Last", Channel1Last.ToString());
            xmlWriter.WriteElementString("Channel2Last", Channel2Last.ToString());
            xmlWriter.WriteElementString("Channel3Last", Channel3Last.ToString());
            xmlWriter.WriteElementString("Channel4Last", Channel4Last.ToString());            

            xmlWriter.WriteEndElement();

            return true;
        }

        #endregion       
    }
}
