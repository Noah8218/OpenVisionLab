using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace IntelligentFactory
{
    public class DIO_I7050D
    {
        private string m_strPortName = "COM8";

        public string PortName
        {
            get { return m_strPortName; }
            set { m_strPortName = value; }
        }

        private int m_nBaudRate = 9600;
        public int BaudRate
        {
            get { return m_nBaudRate; }
            set { m_nBaudRate = value; }
        }


        private bool m_bIsOpen = false;
        public bool IsOpen
        {
            get
            {
                return m_bIsOpen;
            }
            set
            {
                m_bIsOpen = value;
            }
        }

        private SerialPort Comport = new SerialPort();
        

        public DIO_I7050D()
        {

        }

        #region IO                

        public ISignal DI_00_TRIGGER = null;

        public ISignal DO_00_READY = null;
        public ISignal DO_01_RESULT_OK = null;
        public ISignal DO_02_RESULT_NG = null;
        public ISignal DO_03_ALARM = null;

        public List<ISignal> Inputs { get; set; } = new List<ISignal>();
        public List<ISignal> Outputs { get; set; } = new List<ISignal>();

        public bool Init()
        {
            try
            {
                if (LoadConfig()) { }
                else { }

                if (Open()) {}
                else { }

                Inputs.Clear();
                Outputs.Clear();

                DI_00_TRIGGER = new ISignal("DI_00_TRIGGER", "0", "0", ISignal.DEV_TYPE.LB);
                Inputs.Add(DI_00_TRIGGER);

                DO_00_READY = new ISignal("DO_00_READY", "0", "0", ISignal.DEV_TYPE.LB);
                Outputs.Add(DO_00_READY);
                DO_01_RESULT_OK = new ISignal("DO_01_RESULT_OK", "1", "0", ISignal.DEV_TYPE.LB);
                Outputs.Add(DO_01_RESULT_OK);
                DO_02_RESULT_NG = new ISignal("DO_02_RESULT_NG", "2", "0", ISignal.DEV_TYPE.LB);
                Outputs.Add(DO_02_RESULT_NG);
                DO_03_ALARM = new ISignal("DO_03_ALARM", "3", "0", ISignal.DEV_TYPE.LB);
                Outputs.Add(DO_03_ALARM);

                for (int i = 0; i < Outputs.Count; i++)
                {
                    Off(Outputs[i]);
                }

                StartThreadRead();

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }
        #endregion

        #region Thread
        private IThreadStatus m_ThreadStatusRead = new IThreadStatus();
        public IThreadStatus ThreadStatusRead
        {
            get { return m_ThreadStatusRead; }
        }

        private void StartThreadRead()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadRead));
            t.Start(m_ThreadStatusRead);
        }

        public void StopThreadRead()
        {
            if (!ThreadStatusRead.IsExit())
            {
                ThreadStatusRead.Stop(100);
            }
        }

        int m_nAliveTime = 0;
        private void ThreadRead(object ob)
        {
            IThreadStatus ThreadStatus = (IThreadStatus)ob;

            ThreadStatus.Start("Read Io");
            Logger.WriteLog(LOG.Normal, "Read the Io Signal");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    if ((Environment.TickCount - m_nAliveTime) > 1000)
                    {
                        m_nAliveTime = Environment.TickCount;

                    }

                    Thread.Sleep(100);

                    if (IsOpen)
                    {
                        try
                        {
                            Read();
                        }
                        catch (Exception Desc)
                        {
                            Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                        }
                    }
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

        }
        #endregion

        private string m_strReadBuffer = "";
        public void Read()
        {
            //char[] ReadPacket = new char[] { '@', '0', '1', (char)0X0D };
            string strSendPacket = $"@01" + (char)0x0D;
            byte[] buff = System.Text.ASCIIEncoding.Default.GetBytes(strSendPacket);
            Comport.Write(buff, 0, buff.Length);

            Thread.Sleep(10);

            string str = Comport.ReadExisting();

            if (str == "") return;

            if (str.Contains((char)0x0D))
            {
                string strRecvPacket = str.Substring(0, str.IndexOf((char)0x0D));

                string strResult = str.Substring(1, 4);

                int Dec = Convert.ToInt32(strResult, 16);
                string bin = Convert.ToString(Dec, 2);

                char[] arrInputs = bin.ToArray();
                Array.Reverse(arrInputs);
                // input  0 ~ 7
                // output 0 ~ 7 순서대로 진행

                DI_00_TRIGGER.Current = arrInputs[0] == '0' ? ISignal.SIGNAL_ON : ISignal.SIGNAL_OFF;
            }
        }
        public bool Open()
        {
            try
            {
                if (!Comport.IsOpen)
                {
                    Comport.PortName = "COM8";
                    Comport.BaudRate = 9600;
                    Comport.Open();

                    if (Comport.IsOpen)
                    {                        
                        Comport.DataReceived += M_sp_DataReceived;
                        GC.Collect();

                        IsOpen = true;

                        return true;
                    }
                    else
                    {
                        //Logger.WriteLog(LOG.ERR, "[Failed] Light Controller Init");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public void On(string strAddr)
        {
            string strSendPacket = $"#011{strAddr}01" + (char)0x0D;
            byte[] buff = System.Text.ASCIIEncoding.Default.GetBytes(strSendPacket);
            Comport.Write(buff, 0, buff.Length);
        }

        public void Off(string strAddr)
        {
            string strSendPacket = $"#011{strAddr}00" + (char)0x0D;
            byte[] buff = System.Text.ASCIIEncoding.Default.GetBytes(strSendPacket);
            Comport.Write(buff, 0, buff.Length);
        }

        public bool On(ISignal signal)
        {
            try
            {
                string strAddr = signal.Address;
                string strSendPacket = $"#011{strAddr}01" + (char)0x0D;
                byte[] buff = System.Text.ASCIIEncoding.Default.GetBytes(strSendPacket);
                Comport.Write(buff, 0, buff.Length);

                signal.Current = ISignal.SIGNAL_ON;

                Thread.Sleep(1);

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool Off(ISignal signal)
        {
            try
            {
                string strAddr = signal.Address;
                string strSendPacket = $"#011{strAddr}00" + (char)0x0D;
                byte[] buff = System.Text.ASCIIEncoding.Default.GetBytes(strSendPacket);
                Comport.Write(buff, 0, buff.Length);

                signal.Current = ISignal.SIGNAL_OFF;

                Thread.Sleep(1);

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        private void M_sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            IsOpen = true;
        }

        public bool Close()
        {
            try
            {
                if(Comport.IsOpen)
                {
                    IsOpen = false;
                    Comport.Close();
                }
                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }
        

        public string str2hex(string strData)
        {
            string resultHex = string.Empty;
            byte[] arr_byteStr = Encoding.Default.GetBytes(strData);

            foreach (byte byteStr in arr_byteStr)
                resultHex += string.Format("{0:X2}", byteStr);

            return resultHex;
        }

        public bool BitOnLower(ISignal signal)
        {
            try
            {
                // #AA1cDD

                List<string> CheckSum = new List<string>();                

                string strDelimiterChar = "#";
                CheckSum.Add(strDelimiterChar);
                string strAddress = signal.Address;
                for(int i = 0;  i< strAddress.Length;i++)
                {                    
                    CheckSum.Add(strAddress[i].ToString());
                }                
                string strLowerCommand = "1";
                CheckSum.Add(strLowerCommand);
                string strChannel = signal.Chaannel;
                CheckSum.Add(strChannel);
                string strData = "1";
                CheckSum.Add(strData);

                string strCheckSum = "";
                for (int i = 0; i <CheckSum.Count;i++)
                {                    
                     str2hex(CheckSum[i]);                    
                }

                // Comman Format : Leading Character + Module Address + Command + Chksum + CR
                // Response Format : Leading Character + Module Address + Data + Chksum + CR
                if (IsOpen)
                {

                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }


        #region CONFIG MANAGER
        private string m_strPath = Application.StartupPath + "\\" + "PlcConfig" + ".xml";
        private string m_XMLName = "PlcConfig";
        public bool LoadConfig()
        {
            try
            {
                m_strPath = Application.StartupPath + "\\" + DEFINE.CONFIG + "\\" + "PlcConfig" + ".xml";
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
                //Logger.WriteLog(ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveConfig()
        {
            m_strPath = Application.StartupPath + "\\" + DEFINE.CONFIG + "\\" + "PlcConfig" + ".xml";
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
                // Logger.WriteLog(LOG.ERR, "[FAILED] LightController Config Save");
                //Logger.WriteLog(ex.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            // Logger.WriteLog(LOG.SYS, "[SUCCESS] LightController Config Save ==> {0}", m_strPath);
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
                            m_strPortName = xmlReader.Value;
                            break;
                        case "BaudRate":
                            if (!xmlReader.Read()) return false;
                            m_nBaudRate = int.Parse(xmlReader.Value);
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
            return true;
        }

        public bool SaveConfigByXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Parameter");

            xmlWriter.WriteElementString("PortName", m_strPortName);
            xmlWriter.WriteElementString("BaudRate", m_nBaudRate.ToString());
            xmlWriter.WriteEndElement();

            return true;
        }

        #endregion      
    }
}
