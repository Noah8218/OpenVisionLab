using System.ComponentModel;
using System.IO;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Property;

namespace OpenVisionLab
{
    [CategoryOrder("Blob Parameter", 6)]
    public class CPropertyBlob : COpenCVPropertyBase, IOpenCVPropertyBlob
    {
        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("Area(가로*세로) 최소 사이즈입니다. 그 이하는 필터링 됩니다."), DisplayNameAttribute("MIN AREA")]
        public  int MIN_AREA { get; set; } = 200;

        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Blob Parameter"), DescriptionAttribute("Area(가로*세로) 최대 사이즈입니다. 그 이상는 필터링 됩니다."), DisplayNameAttribute("MAX AREA")]
        public int MAX_AREA { get; set; } = 1000000;

        public CPropertyBlob(string strName)
        {
            NAME = strName;
        }

        public CPropertyBlob() { }
    
        public CPropertyBlob DeepCopy() => (CPropertyBlob)this.MemberwiseClone();
        
        public CPropertyBlob LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\VISION\\" + NAME + ".xml";
            CPropertyBlob newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyBlob>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public CPropertyBlob LoadTestConfig(string path)
        {
            string strPath = path;
            CPropertyBlob newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyBlob>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveTestConfig(path);
            return newData = this.LoadTestConfig(path);
        }
    }
}
