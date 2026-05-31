using Lib.Common;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace OpenVisionLab
{
    public class CSystem
    {
        public ManualResetEvent PauseWait = new ManualResetEvent(false);

        public EventHandler EventChangedMode = null;
        public EventHandler EventChangedAuthorization = null;
        public EventHandler EventChangedMenu = null;
        public EventHandler EventChangedUi = null;
        public EventHandler EventChangedNotice = null;
        public EventHandler<EventArgs> EventUpdateStyle;

        #region MODE       
        public enum MODE { NO_LICENSE, READY, AUTO, ALARM, SIMULATION };
        private MODE m_eModePrev = MODE.READY;
        private MODE m_eMode = MODE.READY;
        public MODE Mode
        {
            get { return m_eMode; }
            set
            {
                m_eModePrev = m_eMode;
                m_eMode = value;

                if (m_eMode != m_eModePrev)
                {
                    if (EventChangedMode != null) { EventChangedMode(null, null); }                   
                }
            }
        }
        #endregion

        #region MENU     
        public enum MENU { MAIN,VISION,MOTION};

        public MENU m_SelectedMenu = MENU.MAIN;
        public MENU Menu
        {
            get { return m_SelectedMenu; }
            set
            {
                m_SelectedMenu = value;
                if (EventChangedMenu != null) { EventChangedMenu(null, null); }               
            }
        }        
        #endregion

        #region RECIPE
        private string m_strLastRecipe = "SETUP001";
        public string LastRecipe
        {
            get => m_strLastRecipe;
            set
            {
                LastRecipeUpdateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                m_strLastRecipe = value;
            }
        }        
        public string LastRecipeUpdateTime { get; set; }        

        #endregion

        #region NOTICE
        private string m_strNotice = "-";
        public string Notice
        {
            get { return m_strNotice; }
            set
            {
                m_strNotice = value;

                if (m_strNotice != "")
                {
                    if (EventChangedNotice != null) { EventChangedNotice(null, null); }                   
                }
                CLOG.NORMAL(Notice);
            }
        }        
        #endregion

        #region LICENSE
#if USE_LICENSE
        private bool m_bUseLicense = true;
#else
        private bool m_bUseLicense = false;
#endif
        private string m_strLicense = "";
        public string License
        {
            get { return m_strLicense; }
            set
            {
                m_strLicense = value;

                if (m_bUseLicense)
                {
                    bool bCertificated = false;
                    //License 확인 후 

                    if (bCertificated) Mode = MODE.READY;
                    else Mode = MODE.NO_LICENSE;
                }
            }
        }
        #endregion

        #region RESULT
        public enum RESULT { IDLE, OK, NG };
        private RESULT m_eResult = RESULT.IDLE;
        public RESULT Result
        {
            get { return m_eResult; }
            set { m_eResult = value; }
        }
        #endregion

        #region UI
        private string m_strProcdure = "READY";
        public string PROCDURE
        {
            get => m_strProcdure;
            set
            {
                m_strProcdure = value;
                CLOG.SEQ( "PROCDURE : {0}", m_strProcdure);
            }
        }

        private int m_nStyleIndex = 6;
        public int StyleIndex
        {
            get => m_nStyleIndex;
            set
            {
                m_nStyleIndex = value;

                if (EventUpdateStyle != null)
                {
                    EventUpdateStyle(null, null);
                }
            }
        }

        #endregion

        #region IPC
        public IntPtr IF_Handle { get; set; } = IntPtr.Zero;
        #endregion

        #region PROPERTIES

        private DEFINE.AUTHORIZATION m_Authorization = DEFINE.AUTHORIZATION.OPERATOR;
        public DEFINE.AUTHORIZATION Authorization
        {
            get => m_Authorization;
            set
            {
                m_Authorization = value;

                if (EventChangedAuthorization != null)
                {
                    EventChangedAuthorization(this, new EventArgs());
                }
            }
        }

        // Key : ID, Value : Password
        public CAccountManager AccountManager { get; set; } = new CAccountManager();
        #endregion

        public CSystem()
        {
            CUtil.InitDirectory("IMAGE");
            CUtil.InitDirectory("SAVE_IMAGE");
            CUtil.InitDirectory("RECIPE");            
            CUtil.InitDirectory("CAPTURE");
            CUtil.InitDirectory("CONFIG");            

            LoadConfig();
            AccountManager.LoadConfig();
        }

        public void Close() { }        

        #region CONFIG BY XML              
        private string m_XMLName = "SYSTEM";
        public bool LoadConfig()
        {
            try
            {
                string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        LoadConfigFromXML(xmlReader);
                    } 
                    catch (Exception Desc)
                    {
                        CLOG.ABNORMAL( "SYSTEM Load Ex ==> {0}", Desc.Message);                        
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
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
            return true;
        }

        public bool LoadConfigFromXML(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    CLOG.NORMAL( "CONFIG [{0}] ==> {1}", xmlReader.Name, xmlReader.Value);
                    
                    switch (xmlReader.Name)
                    {
                        case "License":
                            if (!xmlReader.Read()) return false;
                            License = xmlReader.Value;
                            break;
                        case "LastRecipe":
                            if (!xmlReader.Read()) return false;
                            LastRecipe = xmlReader.Value;
                            break;
                        case "LastRecipeUpdateTime":
                            if (!xmlReader.Read()) return false;
                            LastRecipeUpdateTime = xmlReader.Value;
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

            CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
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
                SaveConfigToXML(xmlWriter);
                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            return true;
        }      

        public bool SaveConfigToXML(XmlWriter xmlWriter)
        {
            try
            {
                xmlWriter.WriteStartElement("SYSTEM");
                xmlWriter.WriteElementString("License", License);
                xmlWriter.WriteElementString("LastRecipe", LastRecipe);
                xmlWriter.WriteElementString("LastRecipeUpdateTime", LastRecipeUpdateTime);

                xmlWriter.WriteEndElement();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "SYSTEM Save Ex ==> {0}", Desc.Message);                                
            }            
            
            return true;
        }
        #endregion
    }
}
