using System.ComponentModel;
using System.IO;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV;
using Lib.OpenCV.Property;

namespace OpenVisionLab
{
    [CategoryOrder("Mean", 6)]
    public class CPropertyMean : COpenCVPropertyBase, IOpenCVPropertyMean
    {        
        [CategoryAttribute("Mean"), DescriptionAttribute(""), DisplayNameAttribute("MEAN_MAX")]
        public int MEAN_MAX { get; set; } = 240;
        [CategoryAttribute("Mean"), DescriptionAttribute(""), DisplayNameAttribute("MEAN_MIN")]
        public int MEAN_MIN { get; set; } = 100;

        [CategoryAttribute("Mean"), DescriptionAttribute(""), DisplayNameAttribute("MEAN_TYPES")]
        public MeanType MEAN_TYPES { get; set; } = MeanType.Mean;
        
        public CPropertyMean(string strName) { NAME = strName; }        
        public CPropertyMean() { }

        public CPropertyMean DeepCopy() => (CPropertyMean)this.MemberwiseClone();
    
        #region CONFIG BY XML              
        public CPropertyMean LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\VISION\\" + NAME + ".xml";
            CPropertyMean newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyMean>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public CPropertyMean LoadTestConfig(string path)
        {
            string strPath = path;
            CPropertyMean newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyMean>(strPath);
                if (newData != null)
                {
                    return newData;
                }
            }
            this.SaveTestConfig(path);
            return newData = this.LoadTestConfig(path);
        }
        #endregion

    }
}
