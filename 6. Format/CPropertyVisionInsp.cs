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
using System.Xml.Serialization;

namespace KtemVisionSystem
{
    public class CPropertyVisionInsp
    {
        //[XmlIgnore]

        [CategoryAttribute("ETC"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "VISION_INSP_PARAM";
        [CategoryAttribute("ETC"), DescriptionAttribute(""), DisplayNameAttribute("픽셀 사이즈 (mm)")]
        public double DIST_PIXEL_SIZE_MM { get; set; } = 0.0062D;

        [CategoryAttribute("길이"), DescriptionAttribute(""), DisplayNameAttribute("이진화 임계값")]
        public int DIST_THRESHOLD { get; set; } = 100;

        [CategoryAttribute("길이"), DescriptionAttribute(""), DisplayNameAttribute("이진화 사용여부")]
        public bool USE_DIST_THRESHOLD { get; set; } = false;

        [CategoryAttribute("길이"), DescriptionAttribute(""), DisplayNameAttribute("길이 최대 (mm)")]
        public double DIST_MM_MIN { get; set; } = 6.3D;

        [CategoryAttribute("길이"), DescriptionAttribute(""), DisplayNameAttribute("길이 최대 (mm)")]
        public double DIST_MM_MAX { get; set; } = 8.5D;        

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("LEFT_ROI")]
        public OpenCvSharp.Rect LEFT_ROI { get; set; } = new OpenCvSharp.Rect();

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("RIGHT_ROI")]
        public OpenCvSharp.Rect RIGHT_ROI { get; set; } = new OpenCvSharp.Rect();

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("THICKNESS_ROI")]
        public OpenCvSharp.Rect THICKNESS_ROI{ get; set; } = new OpenCvSharp.Rect();

        public CPropertyVisionInsp()
        {
        }
        public bool Init()
        {
            try
            {
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }
        public CPropertyVisionInsp( string strName)
        {
            NAME = strName;
        }
        #region CONFIG BY XML           

        public CPropertyVisionInsp LoadConfig(string strRecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";
            CPropertyVisionInsp newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyVisionInsp>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(strRecipeName);
            return newData;
        }

        public void SaveConfig(string strRecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";
            SerializeHelper.ToXmlFile(strPath, this);
        }

        private string m_XMLName = "PROPERTY_VISION_INSP";
        //public bool LoadConfigCamera(string strRecipeName)
        //{
        //    try
        //    {
        //        string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";

        //        if (File.Exists(strPath))
        //        {
        //            XmlTextReader xmlReader = new XmlTextReader(strPath);

        //            try
        //            {
        //                while (xmlReader.Read())
        //                {
        //                    if (xmlReader.NodeType == XmlNodeType.Element)
        //                    {
        //                        switch (xmlReader.Name)
        //                        {
        //                            case "THICKNESS_THRESHOLD": if (xmlReader.Read()) THICKNESS_THRESHOLD = int.Parse(xmlReader.Value); break;
        //                            case "USE_THICKNESS_THRESHOLD": if (xmlReader.Read()) USE_THICKNESS_THRESHOLD = bool.Parse(xmlReader.Value); break;
        //                            case "THICKNESS_MM_MAX": if (xmlReader.Read()) THICKNESS_MM_MAX = double.Parse(xmlReader.Value); break;                                    

        //                            case "HEIGHT_THRESHOLD": if (xmlReader.Read()) HEIGHT_THRESHOLD = int.Parse(xmlReader.Value); break;
        //                            case "USE_HEIGHT_THRESHOLD": if (xmlReader.Read()) USE_HEIGHT_THRESHOLD = bool.Parse(xmlReader.Value); break;
        //                            case "HEIGHT_MM_MIN": if (xmlReader.Read()) HEIGHT_MM_MIN = double.Parse(xmlReader.Value); break;
        //                            case "HEIGHT_MM_MAX": if (xmlReader.Read()) HEIGHT_MM_MAX = double.Parse(xmlReader.Value); break;                                    
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (xmlReader.NodeType == XmlNodeType.EndElement)
        //                        {
        //                            if (xmlReader.Name == m_XMLName) break;
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                CLog.Error( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //                xmlReader.Close();
        //            }

        //            xmlReader.Close();
        //        }
        //        else
        //        {
        //            SaveConfig(strRecipeName);
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CLog.Error( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return false;
        //    }
        //    return true;
        //}
        //public bool LoadConfig(string strRecipeName)
        //{
        //    try
        //    {
        //        string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";

        //        if (File.Exists(strPath))
        //        {
        //            XmlTextReader xmlReader = new XmlTextReader(strPath);

        //            try
        //            {
        //                while (xmlReader.Read())
        //                {
        //                    if (xmlReader.NodeType == XmlNodeType.Element)
        //                    {
        //                        switch (xmlReader.Name)
        //                        {
        //                            case "THICKNESS_THRESHOLD":     if (xmlReader.Read()) THICKNESS_THRESHOLD = int.Parse(xmlReader.Value); break;
        //                            case "USE_THICKNESS_THRESHOLD": if (xmlReader.Read()) USE_THICKNESS_THRESHOLD = bool.Parse(xmlReader.Value); break;
        //                            case "THICKNESS_MM_MAX":        if (xmlReader.Read()) THICKNESS_MM_MAX = double.Parse(xmlReader.Value); break;                                    

        //                            case "HEIGHT_THRESHOLD":        if (xmlReader.Read()) HEIGHT_THRESHOLD = int.Parse(xmlReader.Value); break;
        //                            case "USE_HEIGHT_THRESHOLD":    if (xmlReader.Read()) USE_HEIGHT_THRESHOLD = bool.Parse(xmlReader.Value); break;
        //                            case "HEIGHT_MM_MIN":           if (xmlReader.Read()) HEIGHT_MM_MIN = double.Parse(xmlReader.Value); break;
        //                            case "HEIGHT_MM_MAX":           if (xmlReader.Read()) HEIGHT_MM_MAX = double.Parse(xmlReader.Value); break;                                    
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (xmlReader.NodeType == XmlNodeType.EndElement)
        //                        {
        //                            if (xmlReader.Name == m_XMLName) break;
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                CLog.Error( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //                xmlReader.Close();
        //            }

        //            xmlReader.Close();
        //        }
        //        else
        //        {
        //            SaveConfig(strRecipeName);
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CLog.Error( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return false;
        //    }
        //    return true;
        //}
        //public bool SaveConfig(string strRecipeName)
        //{
        //    string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";

        //    XmlWriterSettings settings = new XmlWriterSettings();
        //    settings.Indent = true;
        //    settings.NewLineOnAttributes = true;
        //    settings.IndentChars = "\t";
        //    settings.NewLineChars = "\r\n";
        //    XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
        //    try
        //    {
        //        xmlWriter.WriteStartDocument();
        //        xmlWriter.WriteStartElement("PROPERTY");

        //        xmlWriter.WriteElementString("THICKNESS_THRESHOLD", THICKNESS_THRESHOLD.ToString());
        //        xmlWriter.WriteElementString("USE_THICKNESS_THRESHOLD", USE_THICKNESS_THRESHOLD.ToString());
        //        xmlWriter.WriteElementString("THICKNESS_MM_MAX", THICKNESS_MM_MAX.ToString());
        //        xmlWriter.WriteElementString("THICKNESS_PIXEL_SIZE_MM", THICKNESS_PIXEL_SIZE_MM.ToString());

        //        xmlWriter.WriteElementString("HEIGHT_THRESHOLD", HEIGHT_THRESHOLD.ToString());
        //        xmlWriter.WriteElementString("USE_HEIGHT_THRESHOLD", USE_HEIGHT_THRESHOLD.ToString());
        //        xmlWriter.WriteElementString("HEIGHT_MM_MIN", HEIGHT_MM_MIN.ToString());
        //        xmlWriter.WriteElementString("HEIGHT_MM_MAX", HEIGHT_MM_MAX.ToString());

        //        xmlWriter.WriteEndElement();
        //        xmlWriter.WriteEndDocument();
        //    }
        //    catch (Exception ex)
        //    {
        //        CLog.Error( "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
        //    }
        //    finally
        //    {
        //        xmlWriter.Flush();
        //        xmlWriter.Close();
        //    }

        //    return true;
        //}
        #endregion
    }    
}
