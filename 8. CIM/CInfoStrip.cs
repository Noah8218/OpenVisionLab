using System;
using System.Collections.Generic;
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
    public class CInfoStrip
    {        
        public CInfoStrip(string strName = "")
        {
            NAME = strName;            
        }

        public string NAME { get; set; } = "";

        public string LotID { get; set; } = "";

        public string MgzNo { get; set; } = "";

        public string SlotNo { get; set; } = "";

        public string SlotName { get; set; } = "";

        public string GroupName { get; set; } = "";

        public string DeviceName { get; set; } = "";

        private string m_strDeviceName = "";
        public string StripName { get; set; } = "";


        #region CONFIG BY XML              
        private string m_XMLName = "CInfoStrip";
        public bool LoadConfig(string strRecipeName)
        {
            try
            {
                string strPath = $"{Application.StartupPath}\\RECIPE\\{strRecipeName}\\{NAME}_{m_XMLName}.xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                string strName = xmlReader.Name;
                                if (!xmlReader.Read()) return false;
                                string strRead = xmlReader.Value;

                                CLOG.NORMAL( "CONFIG [{0}] ==> {1}", strName, strRead);

                                switch (xmlReader.Name)
                                {
                                    case "NAME": if (xmlReader.Read()) NAME = xmlReader.Value; break;
                                    case "LotID":
                                        if (!xmlReader.Read()) return false;
                                        if (strRead == "\r\n\t")
                                        {
                                            LotID = "";
                                        }
                                        else
                                        {
                                            LotID = strRead;
                                        }
                                        break;
                                    case "MgzNo":
                                        if (!xmlReader.Read()) return false;
                                        if (strRead == "\r\n\t")
                                        {
                                            MgzNo = "";
                                        }
                                        else
                                        {
                                            MgzNo = strRead;
                                        }
                                        break;
                                    case "SlotNo":
                                        if (!xmlReader.Read()) return false;
                                        if (strRead == "\r\n\t")
                                        {
                                            SlotNo = "";
                                        }
                                        else
                                        {
                                            SlotNo = strRead;
                                        }
                                        break;
                                    case "SlotName":
                                        if (!xmlReader.Read()) return false;
                                        if (strRead == "\r\n\t")
                                        {
                                            SlotName = "";
                                        }
                                        else
                                        {
                                            SlotName = strRead;
                                        }
                                        break;
                                    case "Strip_GroupName":
                                        if (!xmlReader.Read()) return false;
                                        if (strRead == "\r\n\t")
                                        {
                                            GroupName = "";
                                        }
                                        else
                                        {
                                            GroupName = strRead;
                                        }
                                        break;
                                    case "Strip_DeviceName":
                                        if (!xmlReader.Read()) return false;
                                        if (strRead == "\r\n\t")
                                        {
                                            DeviceName = "";
                                        }
                                        else
                                        {
                                            DeviceName = strRead;
                                        }
                                        break;
                                    case "StripName":
                                        if (!xmlReader.Read()) return false;
                                        if (strRead == "\r\n\t")
                                        {
                                            StripName = "";
                                        }
                                        else
                                        {
                                            StripName = strRead;
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
            string strPath = $"{Application.StartupPath}\\RECIPE\\{strRecipeName}\\{NAME}_{m_XMLName}.xml";

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

                xmlWriter.WriteElementString("NAME", NAME.ToString());
                xmlWriter.WriteElementString("LotID", LotID.ToString());
                xmlWriter.WriteElementString("MgzNo", MgzNo.ToString());
                xmlWriter.WriteElementString("SlotNo", SlotNo.ToString());
                xmlWriter.WriteElementString("SlotName", SlotName.ToString());
                xmlWriter.WriteElementString("Strip_GroupName", GroupName.ToString());
                xmlWriter.WriteElementString("Strip_DeviceName", DeviceName.ToString());
                xmlWriter.WriteElementString("StripName", StripName.ToString());

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
