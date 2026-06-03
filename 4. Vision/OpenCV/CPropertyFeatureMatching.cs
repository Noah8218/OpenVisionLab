using Lib.OpenCV.Blob;
using OpenCvSharp;
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
using static OpenVisionLab.CPropertyGridEditor;
using System.Xml.Serialization;
using Lib.OpenCV.Property;

namespace OpenVisionLab.Vision._1._Tools.OpenCV
{
    public class CPropertyFeatureMatching : COpenCVPropertyBase, IOpenCVPropertyFeatureSIFT
    {
        [PropertyOrder(1)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("SCORE_MIN")]
        public double SCORE_MIN { get; set; } = 0.6D;

        [PropertyOrder(4)]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("RANSAC_REPROJ_THRESHOLD")]
        public double RANSAC_REPROJ_THRESHOLD { get; set; } = 3D;

        [PropertyOrder(8)]
        [PropertyEditor(typeof(WpgMatchEditor))]
        [CategoryAttribute("Matching"), DescriptionAttribute(""), DisplayNameAttribute("PATTERN PATH")]
        public string PATTERN_PATH { get; set; } = "";

        [XmlIgnore] public Mat ImageTemplate = new Mat();

        public CPropertyFeatureMatching(string strName)
        {
            NAME = strName;
        }

        public CPropertyFeatureMatching() { }

        public CPropertyFeatureMatching DeepCopy() => (CPropertyFeatureMatching)this.MemberwiseClone();

        public CPropertyFeatureMatching LoadConfig(string RecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\VISION\\" + NAME + ".xml";
            CPropertyFeatureMatching newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyFeatureMatching>(strPath);
                if (File.Exists(newData.PATTERN_PATH))
                {
                    newData.ImageTemplate = Cv2.ImRead(newData.PATTERN_PATH);
                }
                return newData;
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public CPropertyFeatureMatching LoadTestConfig(string path)
        {
            string strPath = path;
            CPropertyFeatureMatching newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyFeatureMatching>(strPath);
                if (File.Exists(newData.PATTERN_PATH))
                {
                    newData.ImageTemplate = Cv2.ImRead(newData.PATTERN_PATH);
                }
                return newData;
            }
            this.SaveTestConfig(path);
            return newData = this.LoadTestConfig(path);
        }
    }
}
