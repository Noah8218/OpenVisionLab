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
    public class CPropertySocket
    {
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";

        public enum CON_TYPE:int { TCPIP = 0, UDP = 1 };

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("TYPE")]
        public CON_TYPE TYPE { get; set; } = CON_TYPE.TCPIP;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("IP")]
        public string IP { get; set; } = "192.168.100.101";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("PORT")]
        public string PORT { get; set; } = "5000";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("INDEX")]
        public int INDEX { get; set; } = 0;


        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("EMPTY")]
        public string EMPTY { get; set; } = "EMPTY";

        public CPropertySocket(string strName)
        {
            NAME = strName;
        }

        #region CONFIG BY XML              
        private string m_XMLName = "PROPERTY_SOCKET";
        public bool LoadConfig()
        {
            try
            {
                string strPath = Application.StartupPath + "\\CONFIG\\DEVICE\\" + NAME + INDEX.ToString() + ".xml";

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
                                    case "TYPE": if (xmlReader.Read()) TYPE = (CON_TYPE)Enum.Parse(typeof(CON_TYPE), xmlReader.Value); break;
                                    case "IP": if (xmlReader.Read()) IP = xmlReader.Value; break;
                                    case "PORT": if (xmlReader.Read()) PORT = xmlReader.Value; break;
                                    case "INDEX": if (xmlReader.Read()) INDEX = int.Parse(xmlReader.Value); break;
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
            string strPath = Application.StartupPath + "\\CONFIG\\DEVICE\\" + NAME + INDEX.ToString() + ".xml";

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
                xmlWriter.WriteElementString("TYPE", TYPE.ToString());
                xmlWriter.WriteElementString("INDEX", INDEX.ToString());
                xmlWriter.WriteElementString("IP", IP.ToString());
                xmlWriter.WriteElementString("PORT", PORT.ToString());

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
