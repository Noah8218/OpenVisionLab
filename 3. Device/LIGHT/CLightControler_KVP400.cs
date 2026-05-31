using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

using OpenCvSharp;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using System.Xml;
using OpenCvSharp.XFeatures2D;
using System.Xml.Linq;
using Lib.Common;

namespace OpenVisionLab
{
    public class CLightControler_KVP400
    {
        private SerialPort m_Sp = new SerialPort();

        public string[] PortNm { get; set; } = SerialPort.GetPortNames(); 
        public string PortName { get; set; } = "COM3"; // 저장용 portname
        public int BaudRate { get; set; } = 19200;
        public bool IsOpen { get; set; }
        public int ChannelCount { get; set; } = 6;
        public bool StartOn { get; set; } = true;
        public int[] Values { get; set; } = new int[6];
        public bool[] OnOffCheck { get; set; } = new bool[6];        

        private int Light_Index { get; set; } = 0;

        private string STX = "@";
        private string CMD_SET = "S";
        private string CMD_GET = "G";
        private string CMD_INTENSITY = "I";
        private string CMD_DIVISION = "/";
        private string CMD_TRIGGER = "T";
        private string CMD_ON = "1";
        private string CMD_OFF = "0";        

        public event EventHandler<bool> Event_Connect = null;
        public event EventHandler<string> Event_Msg = null;
        public event EventHandler<bool> Event_OnOff = null;
        
        public CLightControler_KVP400() { }
        
        //string to byte
        private byte[] StringToByte(string str)
        {
            byte[] StrByte = Encoding.ASCII.GetBytes(str);
            return StrByte;
        }

        //Serial 연결
        public bool Connect(string RecipeName, int Index)
        {
            Light_Index = Index;

            ReadInitFile(RecipeName);

            if (!IsOpen)
            {
                if (!SerialPort.GetPortNames().ToList().Contains(PortName))
                {
                    CLOG.ABNORMAL($"COMPORT Does Not Exist. --> PORT : {PortName}");
                    return false;
                }

                m_Sp.WriteTimeout = 1000;
                m_Sp.ReadTimeout = 1000;
                m_Sp.PortName = PortName;
                m_Sp.BaudRate = BaudRate;
                m_Sp.DataBits = 8;
                m_Sp.StopBits = StopBits.One;
                m_Sp.Parity = Parity.None;
                m_Sp.Open();

                m_Sp.DataReceived += M_Sp_DataReceived;

                for(int i= 0; i < ChannelCount; i++)
                {
                    Write(i + 1, Values[i]);
                    Thread.Sleep(50);
                }

                AllOn();                
                return true;
            }
            else { return false; }           
        }

        private void M_Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort pt = (SerialPort)sender;

            try
            {
                string rcvMsg = pt.ReadLine();

                rcvMsg = rcvMsg.Replace("\r", "");
                rcvMsg = rcvMsg.Replace("\n", "");

                string result = "";
                try
                {
                    if (1 <= rcvMsg.Length)
                    {
                        result = rcvMsg.Substring(rcvMsg.Length - 1, 1);
                    }
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                }


                if (string.IsNullOrEmpty(rcvMsg) == false)
                {                    
                    CLOG.COMM( "SerialPortMgr::Received Msg[{0}]", rcvMsg);
                    if (this.Event_Msg != null)
                        this.Event_Msg(this, rcvMsg);

                    if(rcvMsg.Contains("ST"))
                    {
                        IsOpen = true;
                        switch(result)
                        {
                            case "0":
                                IsOpen = true;
                                CLOG.COMM($"LIGHT Comman OK");
                                CLOG.DEVICE($"Connected : LIGHT({Light_Index + 1})");
                                break;
                            case "1":
                                // Command Error
                                IsOpen = false;
                                CLOG.COMM($"LIGHT Command Error");
                                CLOG.DEVICE($"Disconnected : LIGHT({Light_Index + 1})");
                                break;
                            case "2":
                                // Data Range Error
                                CLOG.COMM($"LIGHT Data Range Error");
                                CLOG.DEVICE($"Disconnected : LIGHT({Light_Index + 1})");
                                IsOpen = false;
                                break;
                            case "3":
                                // Time Out Error
                                CLOG.COMM($"LIGHT Time Out Error");
                                CLOG.DEVICE($"Disconnected : LIGHT({Light_Index + 1})");
                                IsOpen = false;
                                break;
                        }                   
                    }                                    
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");                
            }
        }

        //Serial 해제     
        public bool DisConnect()
        {
            AllOff();
            m_Sp.Close();
            m_Sp.Dispose();
            IsOpen = false;
            CLOG.DEVICE($"Disconnected : {Light_Index+1}");
            return false;
        }

        public bool Write(int Channel, int Value)
        {
            try
            {
                if(Channel <= 0)
                {
                    CLOG.DEVICE( "[FAILED] Channel--> {0}", Channel);
                    CLOG.DEVICE( "0보다 큰값을 입력하여 주십시오");                    
                    return false;
                }

                if (Value < 0 || Value > 256)
                {
                    CLOG.DEVICE( "[FAILED] Value--> {0}", Value);
                    CLOG.DEVICE( "0 ~ 255사이의 값을 입력하여 주십시오");
                    return false;
                }

                string Ch = Channel.ToString().PadLeft(2,'0');
                string V = Value.ToString().PadLeft(3,'0'); 
                string SendMessage = $"{STX}{CMD_SET}{CMD_INTENSITY}{Ch}{CMD_DIVISION}{V}\r\n";
                //"@SI01/001\r\n";
                m_Sp.WriteLine(SendMessage);

                Values[Channel - 1] = Value;

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool AllWrite(int Value)
        {
            try
            {
                if (Value < 0 || Value > 256)
                {
                    CLOG.DEVICE( "[FAILED] Value--> {0}", Value);
                    CLOG.DEVICE( "0 ~ 255사이의 값을 입력하여 주십시오");
                    return false;
                }

                string SendMessage = $"{STX}{CMD_SET}{CMD_INTENSITY}00{CMD_DIVISION}";
                for (int i = 0; i < ChannelCount; i++) { SendMessage += $"{Value}{CMD_DIVISION}"; }                
                SendMessage = SendMessage.Substring(0, SendMessage.Length - 1);
                SendMessage += "\r\n";

                m_Sp.WriteLine(SendMessage);

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool On(int Channel)
        {
            try
            {
                if (Channel <= 0)
                {
                    CLOG.DEVICE( "[FAILED] Channel--> {0}", Channel);
                    CLOG.DEVICE( "0보다 큰값을 입력하여 주십시오");
                    return false;
                }
                string Ch = Channel.ToString().PadLeft(2, '0');

                string SendMessage = $"{STX}{CMD_SET}{CMD_TRIGGER}{Ch}{CMD_DIVISION}{CMD_ON}\r\n";

                m_Sp.WriteLine(SendMessage);

                OnOffCheck[Channel - 1] = true;

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool Off(int Channel)
        {
            try
            {
                if (Channel <= 0)
                {
                    CLOG.DEVICE( "[FAILED] Channel--> {0}", Channel);
                    CLOG.DEVICE( "0보다 큰값을 입력하여 주십시오");
                    return false;
                }
                string Ch = Channel.ToString().PadLeft(2, '0');

                string SendMessage = $"{STX}{CMD_SET}{CMD_TRIGGER}{Ch}{CMD_DIVISION}{CMD_OFF}\r\n";

                m_Sp.WriteLine(SendMessage);

                OnOffCheck[Channel - 1] = false;

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool AllOn()
        {
            try
            {
                string SendMessage = $"{STX}{CMD_SET}{CMD_TRIGGER}00{CMD_DIVISION}";
                for (int i = 0; i < ChannelCount; i++)
                {
                    SendMessage += $"{CMD_ON}{CMD_DIVISION}"; 
                }
                SendMessage = SendMessage.Substring(0, SendMessage.Length - 1);
                SendMessage += "\r\n";

                m_Sp.WriteLine(SendMessage);

                for (int i = 0; i < ChannelCount; i++)
                {
                    OnOffCheck[i] = true;
                }

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool AllOff()
        {
            try
            {
                string SendMessage = $"{STX}{CMD_SET}{CMD_TRIGGER}00{CMD_DIVISION}";
                for (int i = 0; i < ChannelCount; i++)
                {
                    SendMessage += $"{CMD_OFF}{CMD_DIVISION}";
                }
                SendMessage = SendMessage.Substring(0, SendMessage.Length - 1);
                SendMessage += "\r\n";

                m_Sp.WriteLine(SendMessage);

                for (int i = 0; i < ChannelCount; i++)
                {
                    OnOffCheck[i] = false;
                }

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool ReadOnOff(int Channel)
        {
            try
            {
                if (Channel <= 0)
                {
                    CLOG.DEVICE( "[FAILED] Channel--> {0}", Channel);
                    CLOG.DEVICE( "0보다 큰값을 입력하여 주십시오");
                    return false;
                }
                string Ch = Channel.ToString().PadLeft(2, '0');

                string SendMessage = $"{STX}{CMD_GET}{CMD_TRIGGER}{Ch}\r\n";

                m_Sp.WriteLine(SendMessage);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool ReadChannelVal(int Channel)
        {
            try
            {
                if (Channel <= 0)
                {
                    CLOG.DEVICE("[FAILED] Channel--> {0}", Channel);
                    CLOG.DEVICE("0보다 큰값을 입력하여 주십시오");
                    return false;
                }
                string Ch = Channel.ToString().PadLeft(2, '0');

                string SendMessage = $"{STX}{CMD_GET}{CMD_INTENSITY}{Ch}\r\n";

                m_Sp.WriteLine(SendMessage);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        private string ByteToString(byte[] strByte)
        {

            string str = Encoding.Default.GetString(strByte);
            return str;
        }


        #region CONFIG BY XML
        //private string m_strPath = Application.StartupPath + "\\" + "LighControllerConfig" + ".xml";
        private string m_XMLName = "LIGHTCONTROLLER";
        public bool ReadInitFile(string RecipeName)
        {
            try
            {
                string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\DEVICE\\" + m_XMLName + (Light_Index+1) + ".xml";                

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);
                    
                    try
                    {
                        ReadInitFileFromXML(xmlReader);
                    }
                    catch (Exception Desc)
                    {
                        xmlReader.Close();
                    }
                    xmlReader.Close();
                }
                else
                {
                    WriteInitFile(RecipeName);
                    return false;
                }
            }
            catch (Exception Desc)
            {
                return false;
            }
            return true;
        }
        
        public bool WriteInitFile(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\DEVICE\\" + m_XMLName + (Light_Index + 1) + ".xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();
                
                WriteInitFileToXML(xmlWriter);

                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {
                
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }
            return true;
        }
        
        public bool ReadInitFileFromXML(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "PortName":
                            if (!xmlReader.Read()) return false;
                            //PortNm[0] = xmlReader.Value;
                            PortName = xmlReader.Value;
                            break;
                        case "BaudRate":
                            if (!xmlReader.Read()) return false;
                            BaudRate = int.Parse(xmlReader.Value);
                            break;
                        case "ChannelCount":
                            if (!xmlReader.Read()) return false;
                            ChannelCount = int.Parse(xmlReader.Value);
                            break;
                        case "StartOn":
                            if (!xmlReader.Read()) return false;
                            StartOn = bool.Parse(xmlReader.Value);
                            break;
                        case "Values":
                            if (!xmlReader.Read()) return false;
                            string strValue = xmlReader.Value;
                            string[] strValues = strValue.Split(',');

                            Values = new int[ChannelCount];
                            OnOffCheck = new bool[ChannelCount];
                            for (int i = 0; i < ChannelCount; i++)
                            {
                                if (i < Values.Length) Values[i] = int.Parse(strValues[i]);
                                if (i < Values.Length) OnOffCheck[i] = true;
                            }
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
        
        public bool WriteInitFileToXML(XmlWriter xmlWriter)
        {
            try
            {
                xmlWriter.WriteStartElement("LIGHTCONTROLLER");
                
                xmlWriter.WriteElementString("PortName", PortName);
                xmlWriter.WriteElementString("BaudRate", BaudRate.ToString());
                xmlWriter.WriteElementString("ChannelCount", ChannelCount.ToString());
                xmlWriter.WriteElementString("StartOn", StartOn.ToString());
                xmlWriter.WriteElementString("Values", ValuesToString(Values));
                
                xmlWriter.WriteEndElement();
            }
            catch (Exception Desc)
            {
               
            }

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
            catch (Exception Desc)
            {
                return strValues;
            }
            return strValues;
        }
        #endregion

    }

}
