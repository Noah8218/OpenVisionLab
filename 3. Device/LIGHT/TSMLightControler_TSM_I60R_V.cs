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

namespace KtemVisionSystem
{
    public class TSMLightControler_TSM_I60R_V
    {
        private SerialPort m_Sp = new SerialPort();

        public string[] PortNm { get; set; } = SerialPort.GetPortNames(); 
        public string PortName { get; set; } = "COM3"; // 저장용 portname
        public int BaudRate { get; set; } = 19200;
        public bool IsOpen { get { return m_Sp.IsOpen; } }
        public int ChannelCount { get; set; } = 6;
        public bool StartOn { get; set; } = true;
        public int[] Values { get; set; } = new int[6];

        //Group
        private byte m_G_First { get; set; } = 0x31;
        private byte m_G_Sencond { get; set; } = 0x32;

        //common
        private byte m_Stx { get; set; } = 0x02;
        private byte m_Etx { get; set; } = 0x03;
        private byte m_Zero { get; set; } = 0x30;
        // on off
        private byte m_On { get; set; } = 0x31;
        private byte m_Off { get; set; } = 0x30;
        //command 
        private byte m_A { get; set; } = 0x41;
        private byte m_L { get; set; } = 0x4C;
        private byte m_O { get; set; } = 0x4F;
        private byte m_N { get; set; } = 0x4E;
        private byte m_W { get; set; } = 0x57;
        private byte m_R { get; set; } = 0x52;

        public bool SerialOpen = true;

        public TSMLightControler_TSM_I60R_V()
        {
            
        }

        //string to byte
        private byte[] StringToByte(string str)
        {
            byte[] StrByte = Encoding.ASCII.GetBytes(str);
            return StrByte;
        }

        //Serial 연결
        public bool Connect()
        {
            ReadInitFile();

            if (!SerialOpen)
            {
                m_Sp.PortName = PortName;
                m_Sp.BaudRate = BaudRate;
                m_Sp.DataBits = 8;
                m_Sp.StopBits = StopBits.One;
                m_Sp.Parity = Parity.None;
                
                m_Sp.Open();
                
                SerialOpen = true;

                return true;
            }
            else
            {
                return false;
            }
        }
        
        //Serial 해제     
        public bool DisConnect()
        {
            if (SerialOpen)
            {
                m_Sp.DiscardInBuffer();
                m_Sp.DiscardOutBuffer();
            
                m_Sp.Close();
                m_Sp.Dispose();

                SerialOpen = false;

                return true;
            }
            return false;
        }             

        public bool AllOn()
        {
            try
            {
                if (SerialOpen)
                {                    
                    byte[] Arr_On = new byte[7];

                    Arr_On[0] = m_Stx;
                    Arr_On[1] = m_Zero;
                    Arr_On[2] = m_G_First;
                    Arr_On[3] = m_O;
                    Arr_On[4] = m_L;
                    Arr_On[5] = m_On;
                    Arr_On[6] = m_Etx;

                    m_Sp.Write(Arr_On, 0, Arr_On.Length);

                    CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }


        public bool AllOff()
        {
            try
            {
                if (SerialOpen)
                {                    
                    byte[] Arr_Off = new byte[7];

                    Arr_Off[0] = m_Stx;
                    Arr_Off[1] = m_Zero;
                    Arr_Off[2] = m_G_First;
                    Arr_Off[3] = m_O;
                    Arr_Off[4] = m_L;
                    Arr_Off[5] = m_Off;
                    Arr_Off[6] = m_Etx;

                    m_Sp.Write(Arr_Off, 0, Arr_Off.Length);

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

        public bool SetValue(int Channel, int Value)
        {
            try
            {
                //Convert(Channel, Values[(Channel - 1)]);

                // 채널 byte로 변형
                string stchannel = Channel.ToString();
                byte[] Arr_Channel = StringToByte(stchannel);

                //// 조명 byte 변형
                //string strVal = Value;
                //Values[(Channel - 1)].ToString();
                string strPad = "";

                if (Value.ToString().Length < 3) { strPad = Value.ToString().PadLeft(3, '0'); }               
                else { strPad = Value.ToString(); }                
                byte[] Stval = StringToByte(strPad);

                byte[] control = new byte[11];

                control[0] = m_Stx;
                control[1] = m_Zero;
                control[2] = m_G_First;
                control[3] = m_W;
                control[4] = m_L;
                control[5] = m_Zero;
                control[6] = Arr_Channel[0];                
                control[5] = Stval[0];
                control[6] = Stval[1];
                control[7] = Stval[2];
                control[8] = m_Etx;

                m_Sp.Write(control, 0, control.Length);
                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }
        
        #region CONFIG BY XML
        //private string m_strPath = Application.StartupPath + "\\" + "LighControllerConfig" + ".xml";
        private string m_XMLName = "LIGHTCONTROLLER";
        public bool ReadInitFile()
        {
            try
            {
                string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);
                    
                    try
                    {
                        ReadInitFileFromXML(xmlReader);
                    }
                    catch (Exception ex)
                    {
                        xmlReader.Close();
                    }
                    xmlReader.Close();
                }
                else
                {
                    WriteInitFile();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        
        public bool WriteInitFile()
        {
            string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";
     
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
            catch (Exception ex)
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
                           
                            for (int i = 0; i < strValues.Length; i++)
                            {
                                if (i < Values.Length) Values[i] = int.Parse(strValues[i]);
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
        
        public bool SaveConfig()
        {
            string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

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
            catch (Exception ex)
            {
                
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return strValues;
            }
            return strValues;
        }
        #endregion

    }

}
