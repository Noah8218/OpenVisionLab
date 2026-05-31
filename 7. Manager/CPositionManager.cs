using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace KtemVisionSystem
{
    public class CPositionManager
    {
        private List<CPosition> m_Positions = new List<CPosition>();
        public List<CPosition> Positions
        {
            get { return m_Positions; }
            set { m_Positions = value; }
        }

        private int m_nMaxCount = 0;
        public int MaxCount
        {
            get { return m_nMaxCount; }
            set { m_nMaxCount = value; }
        }
        private string m_strRecipeName = "";
        public string RecipeName
        {
            get { return m_strRecipeName; }
            set { m_strRecipeName = value; }
        }

        #region Position

        public const int AXIS_X_POS_LOADER_READY = 0;
        public const int AXIS_X_POS_STAGE_CLAMPING = 1;
        public const int AXIS_X_POS_STAGE_BACK_READY = 2;
        public const int AXIS_X_POS_UNLOADER_READY = 3;        

        public const int AXIS_Z_POS_STAGE_READY = 5;
        public const int AXIS_Z_POS_VISION_FOCUS = 6;

        public const int AXIS_RAIL_POS_VISION_HOLD = 7;
        public const int AXIS_RAIL_POS_VISION_UNHOLD = 8;


        public CPosition POS_Y_POSITION_READY;
        public CPosition POS_Y_POSITION_END;
        #endregion

        public CPositionManager()
        {            
        }

        public bool InitIPosition(string strRecipeName)
        {
            try
            {
                m_Positions.Clear();

                m_strRecipeName = strRecipeName;
                LoadConfig();

                if(m_Positions.Count == 0)
                {
                    POS_Y_POSITION_READY = new CPosition( "Y", "POS_Y_POSITION_READY", 0);
                    m_Positions.Add(POS_Y_POSITION_READY);
                    POS_Y_POSITION_END = new CPosition( "Y", "POS_Y_POSITION_END", 0);
                    m_Positions.Add(POS_Y_POSITION_END);                
                }

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ALARM("[FAILED] {0}==>{1} Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        #region File Manager              
        private string m_XMLName = "Position";
        public bool LoadConfig()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.StartupPath + "\\Recipe\\" + RecipeName + "\\" + m_XMLName + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        ReadInitFileFromXMLPosition(xmlReader);
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
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1} Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
            return true;
        }

        public bool SaveConfig()
        {
            string strPath = System.Windows.Forms.Application.StartupPath + "\\Recipe\\" + RecipeName + "\\" + m_XMLName + ".xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();

                WriteInitFileToXMLPosition(xmlWriter);
                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1} Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();

                //ReadInitFileVisionStage();
            }

            CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool ParsingPosition(string strData)
        {
            try
            {
                string[] strParsing = strData.Split(',');

                CPosition pos = new CPosition();

                if(strParsing.Length == 3)
                {                    
                    pos.Axis = strParsing[0];
                    pos.Name = strParsing[1];
                    pos.Position = long.Parse(strParsing[2]);

                    m_Positions.Add(pos);
                }
                
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool ReadInitFileFromXMLPosition(XmlReader xmlReader)
        {
            try
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlReader.Name)
                        {
                            case "Position":
                                if (!xmlReader.Read()) return false;

                                string[] strParsing = xmlReader.Value.Split(';');

                                for (int i = 0; i < strParsing.Length; i++)
                                {
                                    ParsingPosition(strParsing[i]);                                    
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

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1} Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool WriteInitFileToXMLPosition(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Parameter");
            xmlWriter.WriteElementString("MaxCount", m_Positions.Count.ToString());

            string strPositionSum = "";
            for (int i = 0; i < Positions.Count; i++)
            {
                string strPosition = "";
                if(i == Positions.Count-1)
                {
                    strPosition = Positions[i].Axis.ToString() + "," + Positions[i].Name + "," + Positions[i].Position.ToString();
                }
                else
                {
                    strPosition = Positions[i].Axis.ToString() + "," + Positions[i].Name + "," + Positions[i].Position.ToString() + ";";
                }

                strPositionSum += strPosition;
            }

            xmlWriter.WriteElementString("Position", strPositionSum);

            xmlWriter.WriteEndElement();
            return true;
        }       
        #endregion


    }

}
