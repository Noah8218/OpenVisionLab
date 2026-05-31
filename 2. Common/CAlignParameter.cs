using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public class CAlignParameter
    {
        public OpenCvSharp.Point FilmCaliPosL { get; set; } = new OpenCvSharp.Point();
        public OpenCvSharp.Point FilmCaliPosR { get; set; } = new OpenCvSharp.Point();
        public double FilmT { get; set; } =0.0;

        public OpenCvSharp.Point ReCFilmCaliPosL { get; set; } = new OpenCvSharp.Point();
        public OpenCvSharp.Point ReCFilmCaliPosR { get; set; } = new OpenCvSharp.Point();
        public double ReCFilmT { get; set; } = 0.0;

        public OpenCvSharp.Point PanelilmCaliPosL { get; set; } = new OpenCvSharp.Point();
        public OpenCvSharp.Point PanelilmCaliPosR { get; set; } = new OpenCvSharp.Point();
        public double PanelT { get; set; } = 0.0;

        public  CAlignParameter LoadConfig(string Name)
        {
            string savePath = $"{Application.StartupPath}\\RECIPE\\{Name}\\AlignParameter.xml";
            CAlignParameter newData = null;

            if (File.Exists(savePath))
            {
                newData = SerializeHelper.FromXmlFile<CAlignParameter>(savePath);
                if (newData != null)
                    return newData;
            }

            newData = new CAlignParameter();
            newData.SaveConfig(Name);
            return newData;
        }

        public void SaveConfig(string Name)
        {
            string savePath = $"{Application.StartupPath}\\RECIPE\\{Name}\\AlignParameter.xml";
            SerializeHelper.ToXmlFile(savePath, this);
        }
    }
}
