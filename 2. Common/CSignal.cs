using Lib.Common;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace OpenVisionLab
{
    public class CSignal
    {
        public enum DEV_TYPE { LB, LW };
        private DEV_TYPE m_DevType = DEV_TYPE.LB;
        public DEV_TYPE DevType
        {
            get { return m_DevType; }
            set { m_DevType = value; }
        }

        public enum SIGNAL_TYPE:int { OFF = 0, ON = 1, NONE = 2 };

        private const short STATION_NO = 255;

        public const int SIGNAL_OFF = 0;
        public const int SIGNAL_ON = 1;
        public const int SIGNAL_NONE = 2;
        public const int MAX_SIGNAL_STATUS = 3;

        public int ON_KEEP_TIME = 50;
        public bool IS_UPDATE_CHECKING = false;
        public int UPDATE_CHECKING_TIME = 0;

        public bool IS_CONTACT_A { get; set; } = true;
        public int DELAY_BEFORE { get; set; } = 0;
        public int DELAY_AFTER { get; set; } = 0;

        private string m_strAddress;
        public string Address
        {
            get
            {
                return m_strAddress;
            }
            set
            {
                m_strAddress = value;
            }
        }

        public bool IsOn 
        {
            get
            { 
                if(IS_CONTACT_A) return Current == SIGNAL_ON; 
                else return !(Current == SIGNAL_ON);
            }
        }


        private int m_nWordCount = 1;
        public int WordCount
        {
            get { return m_nWordCount; }
            set { m_nWordCount = value; }
        }

        private double m_dCurrentActual;
        public double CurrentActual
        {
            get
            {
                //Double nValue = Current;
                double dValue = (double)Current * m_dFactor;
                return dValue;
            }
            set { m_dCurrentActual = value; }
        }

        private string m_strUnit = "";
        public string Unit
        {
            get { return m_strUnit; }
            set { m_strUnit = value; }
        }

        private double m_dFactor = 1.0D;
        public double Factor
        {
            get { return m_dFactor; }
            set { m_dFactor = value; }
        }

        public string DisplayData
        {
            get
            {
                return string.Format("{0} {1}", ((double)Current * m_dFactor).ToString("F2"), m_strUnit);
            }
        }

        private PlcDeviceType m_PlcType = PlcDeviceType.W;
        public enum PlcDeviceType
        {
            // PLC用デバイス
            M = 0x90
          , SM = 0x91
          , L = 0x92
          , F = 0x93
          , V = 0x94
          , S = 0x98
          , X = 0x9C
          , Y = 0x9D
          , B = 0xA0
          , SB = 0xA1
          , DX = 0xA2
          , DY = 0xA3
          , D = 0xA8
          , SD = 0xA9
          , R = 0xAF
          , ZR = 0xB0
          , W = 0xB4
          , SW = 0xB5
          , TC = 0xC0
          , TS = 0xC1
          , TN = 0xC2
          , CC = 0xC3
          , CS = 0xC4
          , CN = 0xC5
          , SC = 0xC6
          , SS = 0xC7
          , SN = 0xC8
          , Z = 0xCC
          , TT
          , TM
          , CT
          , CM
          , A
          , Max
        }

        private int m_nPrevious;
        private int m_nCurrent;
        public int Current
        {
            get
            {
                return m_nCurrent;
            }

            set
            {
                m_nCurrent = value;
                if (!IsDisplay)
                    IsDisplay = (m_nPrevious != m_nCurrent);
                m_nPrevious = m_nCurrent;

                if (IsDisplay)
                {
                    if (EventUpdateSignal != null)
                    {
                        EventUpdateSignal(this, new EventArgs());
                    }
                }
            }
        }

        private bool m_bDisplay;
        public bool IsDisplay
        {
            get
            {
                return m_bDisplay;
            }

            set
            {
                m_bDisplay = value;
            }
        }

        private string m_strName;

        public string Name
        {
            get
            {
                return m_strName;
            }
        }

        private string m_strChannel = "";
        public string Chaannel
        {
            get => m_strChannel;
            set => m_strChannel = value;
        }

        public string Section { get; set; } = "";

        public EventHandler<EventArgs> EventUpdateSignal;

        public CSignal(string strName, string strAddr, DEV_TYPE devType, int nWordCount = 1, string strUnit = "", double dFactor = 1.0D)
        {
            this.m_strName = strName;
            this.m_strAddress = strAddr;            
            this.m_DevType = devType;
            this.m_nWordCount = nWordCount;
            this.m_strUnit = strUnit;
            this.m_dFactor = dFactor;

            LoadConfig();
        }

        public CSignal(string strName, string strAddr, bool bIsContactA = true)
        {
            this.m_strName = strName;
            this.m_strAddress = strAddr;
            this.IS_CONTACT_A = bIsContactA;
        }

        public CSignal(string strSection, string strName, string strAddr, bool isDio = true,  bool bIsContactA = true)
        {
            this.Section = strSection;
            this.m_strName = strName;
            this.m_strAddress = strAddr;
            this.IS_CONTACT_A = bIsContactA;

            LoadConfig();
            
        }

        private string m_XMLName = "PROPERTY_IO";
        public bool LoadConfig()
        {
            try
            {
                string strPath = $"{Application.StartupPath}\\CONFIG\\DEVICE\\IO\\" + m_strName + ".xml";

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
                                    case "IS_CONTACT_A": if (xmlReader.Read()) IS_CONTACT_A = bool.Parse(xmlReader.Value); break;
                                    case "DELAY_BEFORE": if (xmlReader.Read()) DELAY_BEFORE = int.Parse(xmlReader.Value); break;
                                    case "DELAY_AFTER": if (xmlReader.Read()) DELAY_AFTER = int.Parse(xmlReader.Value); break;
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
                    catch (Exception Desc)
                    {
                        CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
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
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }
            return true;
        }

        public bool SaveConfig()
        {
            CUtil.InitDirectory("CONFIG");
            CUtil.InitDirectory("CONFIG\\DEVICE\\IO");

            string strPath = $"{Application.StartupPath}\\CONFIG\\DEVICE\\IO\\" + m_strName + ".xml";

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
                xmlWriter.WriteElementString("IS_CONTACT_A", IS_CONTACT_A.ToString());
                xmlWriter.WriteElementString("DELAY_BEFORE", DELAY_BEFORE.ToString());
                xmlWriter.WriteElementString("DELAY_AFTER", DELAY_AFTER.ToString());

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return true;
        }
    }
}