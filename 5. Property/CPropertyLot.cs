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
using Lib.Common;
using OpenCvSharp;

namespace OpenVisionLab
{
    public class CPropertyLot
    {
        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("LOT END 예상 시간")]
        public DateTime LOT_END_ESTIMATED_TIME { get; set; } = new DateTime();

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("OPEN_TIME")]
        public string OPEN_TIME { get; set; } = "";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("ESTIMATED_TIME")] 
        public string ESTIMATED_TIME { get; set; } = "";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("END_TIME")]
        public string END_TIME { get; set; } = "";        

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("WORKER")]
        public string WORKER { get; set; } = "";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("LOT_NO")]
        public string LOT_NO { get; set; } = "";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("BASE_METAL_DISTANCE")]
        public double BASE_METAL_DISTANCE_M { get; set; } = 10;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("TAPE_DISTANCE")]
        public double TAPE_DISTANCE_M { get; set; } = 10;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("TAKEUP_DISTANCE")]
        public double TAKEUP_DISTANCE_M { get; set; } = 10;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("TAKEUP_START_DEL_DISTTANCE")]
        public double TAKEUP_START_DEL_DISTANCE_M { get; set; } = 7;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("LOT_MIN_TAPE_DISTANCE_M")]
        public double LOT_MIN_TAPE_DISTANCE_M { get; set; } = 10;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("THICKNESS_DELAY_MINIT")]
        public double THICKNESS_DELAY_MINIT { get; set; } = 1;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("BASEMETAL_T_MM")]
        public double BASEMETAL_T_MM { get; set; } = 1;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("TAPE_T_MM")]
        public double TAPE_T_MM { get; set; } = 0.2;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("TAPE_TYPE")]
        public DEFINE.TapeType TAPE_TYPE { get; set; } = DEFINE.TapeType.동;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("EMPTY")]
        public string EMPTY { get; set; } = "EMPTY";

        public Stopwatch THICKNESS_DELAY_TIME { get; set; } = new Stopwatch();

        public bool IS_COMPLETE_LOT_OPEN { get; set; } = false;
        public bool IS_DOOR_OPEN { get; set; } = false;
        public bool IS_LOT_END { get; set; } = false;
        public bool USE_THICKNESS_ALARM { get; set; } = false;

        public double TAKEUP_USAGE_MM { get; set; } = 0.0;        

        public EventHandler<EventArgs> EventLotEnd;

        public CPropertyLot(string strName)
        {
            NAME = strName;
        }

        public void LotEnd()
        {
            try
            {
                this.END_TIME = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                CLOG.LOT("");
                CLOG.LOT("==================== [LOT END] ====================");
                CLOG.LOT($"WORKER : {this.WORKER}");
                CLOG.LOT($"LOT_NO : {this.LOT_NO}");                
                CLOG.LOT($"OPEN DATETIME : {this.OPEN_TIME}");
                CLOG.LOT($"END DATETIME : {this.END_TIME}");
                CLOG.LOT("==================== [--------] ====================");
                CLOG.LOT("");

                IS_LOT_END = true;
                IS_COMPLETE_LOT_OPEN = false;

                if(EventLotEnd != null)
                {
                    EventLotEnd(this, new EventArgs());
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }


        #region CONFIG BY XML              
        private string m_XMLName = "PROPERTY_IMAGEVIEW";
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
                                    case "WORKER": if (xmlReader.Read()) WORKER = xmlReader.Value; break;                                    
                                    case "LOT_NO": if (xmlReader.Read()) LOT_NO = xmlReader.Value; break;
                                    case "BASE_METAL_DISTANCE_M": if (xmlReader.Read()) BASE_METAL_DISTANCE_M = double.Parse(xmlReader.Value); break;
                                    case "TAPE_DISTANCE_M": if (xmlReader.Read()) TAPE_DISTANCE_M = double.Parse(xmlReader.Value); break;
                                    case "TAKEUP_DISTANCE_M": if (xmlReader.Read()) TAKEUP_DISTANCE_M = double.Parse(xmlReader.Value); break;
                                    case "TAKEUP_START_DEL_DISTANCE_M": if (xmlReader.Read()) TAKEUP_START_DEL_DISTANCE_M = double.Parse(xmlReader.Value); break;
                                    case "LOT_MIN_TAPE_DISTANCE_M": if (xmlReader.Read()) LOT_MIN_TAPE_DISTANCE_M = double.Parse(xmlReader.Value); break;
                                    case "THICKNESS_DELAY_MINIT": if (xmlReader.Read()) THICKNESS_DELAY_MINIT = double.Parse(xmlReader.Value); break;
                                    case "BASEMETAL_T_MM": if (!xmlReader.Read()) return false; BASEMETAL_T_MM = double.Parse(xmlReader.Value); break;
                                    case "TAPE_T_MM": if (!xmlReader.Read()) return false; TAPE_T_MM = double.Parse(xmlReader.Value); break;
                                    case "TAPE_TYPE": if (!xmlReader.Read()) return false; TAPE_TYPE = CUtil.ParseEnum<DEFINE.TapeType>(xmlReader.Value); break;
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
                    SaveConfig(strRecipeName);
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
                xmlWriter.WriteElementString("WORKER", WORKER.ToString().Replace("\r\n\t", ""));
                xmlWriter.WriteElementString("LOT_NO", LOT_NO.ToString().Replace("\r\n\t", ""));
                xmlWriter.WriteElementString("BASE_METAL_DISTANCE_M", BASE_METAL_DISTANCE_M.ToString());
                xmlWriter.WriteElementString("TAPE_DISTANCE_M", TAPE_DISTANCE_M.ToString());
                xmlWriter.WriteElementString("TAKEUP_DISTANCE_M", TAKEUP_DISTANCE_M.ToString());
                xmlWriter.WriteElementString("TAKEUP_START_DEL_DISTANCE_M", TAKEUP_START_DEL_DISTANCE_M.ToString());
                xmlWriter.WriteElementString("LOT_MIN_TAPE_DISTANCE_M", LOT_MIN_TAPE_DISTANCE_M.ToString());
                xmlWriter.WriteElementString("THICKNESS_DELAY_MINIT", THICKNESS_DELAY_MINIT.ToString());
                xmlWriter.WriteElementString("BASEMETAL_T_MM", BASEMETAL_T_MM.ToString());
                xmlWriter.WriteElementString("TAPE_T_MM", TAPE_T_MM.ToString());
                xmlWriter.WriteElementString("TAPE_TYPE", TAPE_TYPE.ToString());

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
        #endregion

    }
}
