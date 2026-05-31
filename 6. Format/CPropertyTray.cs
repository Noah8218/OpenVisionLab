using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace KtemVisionSystem
{
    public class CPropertyTray
    {
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "TRAY";

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("TRAY_HEIGHT")]
        public double TRAY_HEIGHT { get; set; } = 6;

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("MAX_TRAY_COUNT")]
        public int MAX_TRAY_COUNT { get; set; } = 65;

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("COLUMNS")]
        public int COLUMNS { get; set; } = 22;

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("ROWS")]
        public int ROWS { get; set; } = 7;

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("PITCH_X_MM")]
        public double PITCH_X_MM { get; set; } = 7.0D;

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("PITCH_Y_MM")]
        public double PITCH_Y_MM { get; set; } = 9.0D;

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("WINDOW_GAP_MM")]
        public double WINDOW_GAP_MM { get; set; } = 9.0D;

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("BOTTOM_WINDOW_INDEX")]
        public int BOTTOM_WINDOW_INDEX { get; set; } = 11;

        [CategoryAttribute("FORMAT"), DescriptionAttribute(""), DisplayNameAttribute("FIRST_INSP_POS")]
        public PointF FIRST_INSP_POS { get; set; } = new PointF();

        private PointF[,] m_TrayGabMap = null;
        public PointF[,] TrayGabMap
        {
            get => m_TrayGabMap;
        }

        private PointF[,] m_TrayMap = null;
        public PointF[,] TrayMap
        {
            get => m_TrayMap;
            set => m_TrayMap = value;
        }

        private List<PointF> m_ResultNaOkList = new List<PointF>();
        public List<PointF> ResultNaOkList
        {
            get => m_ResultNaOkList;
            set => m_ResultNaOkList = value;
        }

        private string m_strLoadResultNaOk = "";
        public string LoadResultNaOk
        {
            get => m_strLoadResultNaOk;
            set => m_strLoadResultNaOk = value;
        }

        private bool m_bIsReverseY = false;
        public CPropertyTray(string strName, bool bIsReverse = false)
        {
            this.NAME = strName;
            m_bIsReverseY = bIsReverse;
        }

        public bool Init()
        {
            try
            {
                int nCols = COLUMNS;
                int nRows = ROWS;

                double dStartX = FIRST_INSP_POS.X;
                double dStartY = FIRST_INSP_POS.Y;

                double dPitchX = PITCH_X_MM;
                double dPitchY = PITCH_Y_MM;

                PointF ptStart_LEFT_TOP = new PointF((float)dStartX, (float)dStartY);
                TrayMap = new PointF[nCols, nRows];

                for (int nX = 0; nX < nCols; nX++)
                {
                    for (int nY = 0; nY < nRows; nY++)
                    {
                        float fPosX = ptStart_LEFT_TOP.X - (float)(dPitchY * nY);
                        float fPosY = 0;
                        if (m_bIsReverseY)
                        {
                            fPosY = ptStart_LEFT_TOP.Y + (float)(dPitchX * nX);
                        }
                        else
                        {
                            fPosY = ptStart_LEFT_TOP.Y - (float)(dPitchX * nX);
                        }

                        TrayMap[nX, nY] = new PointF(fPosY, fPosX);
                    }
                }

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                CLOG.NORMAL( "X : {0} Y : {1}", COLUMNS, ROWS);
                CLOG.NORMAL( "Pitch X : {0} um Pitch Y : {1} um", PITCH_X_MM, PITCH_Y_MM);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        public PointF GetPos(int nX, int nY)
        {
            PointF ptPos = new PointF(-1, -1);

            try
            {
                ptPos = m_TrayMap[nX, nY];
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return ptPos;
            }

            return ptPos;
        }

        public CPropertyTray(string strName)
        {
            NAME = strName;
        }


        #region CONFIG BY XML              
        private string m_XMLName = "PROPERTY_TRAY";
        public bool LoadConfig(string strRecipeName)
        {
            try
            {
                string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";

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
                                    case "TRAY_HEIGHT": if (xmlReader.Read()) TRAY_HEIGHT = double.Parse(xmlReader.Value); break;
                                    case "MAX_TRAY_COUNT": if (xmlReader.Read()) MAX_TRAY_COUNT = int.Parse(xmlReader.Value); break;
                                    case "COLUMNS": if (xmlReader.Read()) COLUMNS = int.Parse(xmlReader.Value); break;
                                    case "ROWS": if (xmlReader.Read()) ROWS = int.Parse(xmlReader.Value); break;
                                    case "PITCH_X_MM": if (xmlReader.Read()) PITCH_X_MM = double.Parse(xmlReader.Value); break;
                                    case "PITCH_Y_MM": if (xmlReader.Read()) PITCH_Y_MM = double.Parse(xmlReader.Value); break;
                                    case "WINDOW_GAP_MM": if (xmlReader.Read()) WINDOW_GAP_MM = double.Parse(xmlReader.Value); break;
                                    case "BOTTOM_WINDOW_INDEX": if (xmlReader.Read()) BOTTOM_WINDOW_INDEX = int.Parse(xmlReader.Value); break;                                        
                                    case "FIRST_INSP_POS": if (xmlReader.Read())  FIRST_INSP_POS = CConverter.StringToPointF(xmlReader.Value); break;
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
                        Init();                        
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
                    SaveConfig(strRecipeName);
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
        public bool SaveConfig(string strRecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";

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
                xmlWriter.WriteElementString("TRAY_HEIGHT", TRAY_HEIGHT.ToString());
                xmlWriter.WriteElementString("MAX_TRAY_COUNT", MAX_TRAY_COUNT.ToString());
                xmlWriter.WriteElementString("COLUMNS", COLUMNS.ToString());
                xmlWriter.WriteElementString("ROWS", ROWS.ToString());
                xmlWriter.WriteElementString("PITCH_X_MM", PITCH_X_MM.ToString());
                xmlWriter.WriteElementString("PITCH_Y_MM", PITCH_Y_MM.ToString());
                xmlWriter.WriteElementString("WINDOW_GAP_MM", WINDOW_GAP_MM.ToString());
                xmlWriter.WriteElementString("BOTTOM_WINDOW_INDEX", BOTTOM_WINDOW_INDEX.ToString());
                xmlWriter.WriteElementString("FIRST_INSP_POS", CConverter.PointFToString(FIRST_INSP_POS));

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
