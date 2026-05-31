using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using static OpenVisionLab.CPropertyGridEditor;

namespace OpenVisionLab
{
    [CategoryOrder("Contour", 10)]
    public class CPropertyContour : COpenCVPropertyBase, IOpenCVPropertyContour
    {
        private Color m_DrawColor = Color.FromArgb(0, 0, 0);

        [PropertyOrder(0)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("USE_APPROXPOLYDP")]
        public bool USE_APPROXPOLYDP { get; set; } = false;

        [PropertyOrder(1)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("USE_DRAW_IMAGE")]
        public bool USE_DRAW_IMAGE { get; set; } = false;

        [PropertyOrder(2)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("ApproximationModes")]
        public ContourApproximationModes ApproximationModes { get; set; } = ContourApproximationModes.ApproxSimple;

        [PropertyOrder(3)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("DetectMode")]
        public RetrievalModes DetectMode { get; set; } = RetrievalModes.List;

        [PropertyOrder(4)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("EPSILON")]
        public double EPSILON { get; set; } = 0.01;

        [PropertyOrder(5)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("MIN_AREA")]
        public int MIN_AREA { get; set; } = 200;

        [PropertyOrder(6)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("MAX_AREA")]
        public int MAX_AREA { get; set; } = 1000000;

        [PropertyOrder(7)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("Draw_Color")]
        [PropertyEditor(typeof(WpgColorEditor))]
        [XmlIgnore]
        public System.Drawing.Color DrawColor
        {
            get { return m_DrawColor; }
            set { m_DrawColor = value; }
        }

        [PropertyOrder(8)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("Draw_Thickness")]
        public int DrawThickness { get; set; } = 2;

        [Browsable(false)]
        public string ClrGridHtml
        {
            get { return ColorTranslator.ToHtml(m_DrawColor); }
            set { DrawColor = ColorTranslator.FromHtml(value); }
        }

        public CPropertyContour() : base() { }
        public CPropertyContour(string strName) : base(strName) { }        

        public CPropertyContour DeepCopy()
        {
            CPropertyContour temp = (CPropertyContour)this.MemberwiseClone();
            return temp;
        }

        public CPropertyContour LoadConfig(string RecipeName)
        {
            string strPath = System.Windows.Forms.Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\VISION\\" + NAME + ".xml";
            CPropertyContour newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyContour>(strPath);
                if (newData != null)
                {
                    //newData.DrawColor = System.Drawing.Color.FromArgb(255, newData.R, newData.G, newData.B);
                    return newData;
                }

            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public CPropertyContour LoadTestConfig(string path)
        {
            string strPath = path;
            CPropertyContour newData = null;

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CPropertyContour>(strPath);
                if (newData != null)
                {
                    //newData.DrawColor = System.Drawing.Color.FromArgb(255, newData.R, newData.G, newData.B);
                    return newData;
                }

            }
            this.SaveTestConfig(path);
            return newData = this.LoadTestConfig(path);
        }
    }
}
