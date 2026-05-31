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
using OpenCvSharp;

namespace OpenVisionLab
{
    public class CPropertyPlcSetting
    {
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";    

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("HostIP")]
        public string HostIP { get; set; } = "192.168.10.111";

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("HostPort")]
        public int HostPort { get; set; } = 6000;

        public CPropertyPlcSetting(string strName)
        {
            NAME = strName;
        }

        public CPropertyPlcSetting()
        {
            
        }

        public CPropertyPlcSetting LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\DEVICE\\" + NAME + ".xml";
            CPropertyPlcSetting newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyPlcSetting>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public void SaveConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\DEVICE\\" + NAME + ".xml";
            SerializeHelper.ToXmlFile(strPath, this);
        }

    }
}
