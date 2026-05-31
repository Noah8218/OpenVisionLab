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


using OpenCvSharp;

namespace OpenVisionLab
{
    public class CPropertyCamera
    {
        public enum CAM_TYPE : int { GIGE = 0, USB = 1, CAMERA_LINK = 2, CXP = 3 };

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("TYPE")]
        public CAM_TYPE TYPE { get; set; } = CAM_TYPE.GIGE;

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("IP")]
        public string IP { get; set; } = "192.168.100.101";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("SERIAL_NUMBER")]
        public string SERIAL_NUMBER { get; set; } = "";

        [CategoryAttribute("INFORMATION"), DescriptionAttribute(""), DisplayNameAttribute("INDEX")]
        public int INDEX { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("CAMERA_POSITION")]
        public string CAMERA_POSITION { get; set; } = "상부";
        public enum TRIGGER_MODE_TYPE : int { OFF = 0, ON_SW = 1, ON_HW = 2 };

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("TRIGGER_MODE")]
        public TRIGGER_MODE_TYPE TRIGGER_MODE { get; set; } = TRIGGER_MODE_TYPE.OFF;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("()도 만큼 이미지를 회전할지 정합니다.")]
        public bool USE_Process_Rotate { get; set; } = false;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("Process_Rotate_Angle")]
        public double Process_Rotate_Angle { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("USE_ROTATE")]
        public bool USE_ROTATE { get; set; } = false;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("ROTATE")]
        public RotateFlags ROTATE { get; set; } = RotateFlags.Rotate90Clockwise;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("USE_FLIP")]
        public bool USE_FLIP { get; set; } = false;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("FLIP")]
        public FlipMode FLIP { get; set; } = FlipMode.X;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("EXPOSURETIME_US")]
        public int EXPOSURETIME_US { get; set; } = 10000;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("GAIN")]
        public int GAIN { get; set; } = 15;

        [CategoryAttribute("SIZE"), DescriptionAttribute(""), DisplayNameAttribute("WIDTH")]
        public int WIDTH { get; set; } = 4024;

        [CategoryAttribute("SIZE"), DescriptionAttribute(""), DisplayNameAttribute("HEIGHT")]
        public int HEIGHT { get; set; } = 3036;

        [CategoryAttribute("SIZE"), DescriptionAttribute(""), DisplayNameAttribute("PIXEL/MM")]
        public double PIXELPERMM { get; set; } = 0.006D;

        public CPropertyCamera(string strName)
        {
            NAME = strName;                        
        }

        public CPropertyCamera()
        {

        }

        public CPropertyCamera LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\DEVICE\\" + NAME + ".xml";
            CPropertyCamera newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyCamera>(strPath);
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
