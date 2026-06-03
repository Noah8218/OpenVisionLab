using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using System.Xml.Linq;

namespace OpenVisionLab.Vision._1._Tools.OpenCV
{
    [CategoryOrder("Parameter", 0)]
    [CategoryOrder("Normalize", 1)]
    public class CPropertyVision
    {
        public CPropertyVision(string strName) : base()
        {
            NAME = strName;
        }

        public CPropertyVision() { }    

        [PropertyOrder(0)]
        [Browsable(true)]
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]        
        public string NAME { get; set; } = "";

        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("Normalize"), DescriptionAttribute("이미지에 노말라이즈를 적용 후 검사할지 결정합니다."), DisplayNameAttribute("USE NORMALIZE")]
        public bool USE_NORMALIZE { get; set; } = true;

        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Normalize"), DescriptionAttribute("노말라이즈 최소 gv값입니다."), DisplayNameAttribute("Alpha")]
        public int Alpha { get; set; } = 30;

        [PropertyOrder(3)]
        [Browsable(true)]
        [CategoryAttribute("Normalize"), DescriptionAttribute("노말라이즈 최대 gv값입니다."), DisplayNameAttribute("Beta")]
        public int Beta { get; set; } = 255;

        public CPropertyVision DeepCopy()
        {
            CPropertyVision temp = (CPropertyVision)this.MemberwiseClone();
            return temp;
        }

        #region CONFIG BY XML              
        public CPropertyVision LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\VISION\\" + NAME + ".xml";
            CPropertyVision newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyVision>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public void SaveConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\VISION\\" + NAME + ".xml";
            SerializeHelper.ToXmlFile(strPath, this);
        }
        #endregion
    }
}
