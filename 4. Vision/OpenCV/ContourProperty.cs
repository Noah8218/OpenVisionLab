using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    [CategoryOrder("Contour", 10)]
    [System.Xml.Serialization.XmlRoot("CPropertyContour")]
    public class ContourProperty : OpenCvPropertyBase, IOpenCVPropertyContour, IOpenCvConfigurableProperty<ContourProperty>
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

        public ContourProperty() : base() { }
        public ContourProperty(string strName) : base(strName) { }        

        public ContourProperty DeepCopy()
        {
            ContourProperty temp = (ContourProperty)this.MemberwiseClone();
            return temp;
        }

        public ContourProperty LoadConfig(string RecipeName)
        {
            return LoadConfigFile<ContourProperty>(RecipeName);
        }

        public ContourProperty LoadTestConfig(string path)
        {
            return LoadTestConfigFile<ContourProperty>(path);
        }
    }
}
