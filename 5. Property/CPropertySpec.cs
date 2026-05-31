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
    public class CPropertySpec
    {
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";    

        [CategoryAttribute("Length"), DescriptionAttribute(""), DisplayNameAttribute("DIST_MIN_MM")]
        public double DIST_MIN_MM { get; set; } = 0.5;

        [CategoryAttribute("Length"), DescriptionAttribute(""), DisplayNameAttribute("DIST_MAX_MM")]
        public double DIST_MAX_MM { get; set; } = 2;

        public CPropertySpec(string strName)
        {
            NAME = strName;
        }

        public CPropertySpec()
        {
            
        }

        public CPropertySpec LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\" + NAME + ".xml";
            CPropertySpec newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertySpec>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public void SaveConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\" + NAME + ".xml";
            SerializeHelper.ToXmlFile(strPath, this);
        }

    }
}
