using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using System.Xml;
using System.Xml.Serialization;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    public enum ContourDrawMode
    {
        Outline,
        BoundingBox
    }

    [CategoryOrder("Contour", 10)]
    [System.Xml.Serialization.XmlRoot("CPropertyContour")]
    public class ContourProperty : OpenCvPropertyBase, IOpenCVPropertyContour, IOpenCvConfigurableProperty<ContourProperty>
    {
        private Color m_DrawColor = Color.Aquamarine;

        [PropertyOrder(0)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("Use approx poly")]
        public bool USE_APPROXPOLYDP { get; set; } = false;

        [PropertyOrder(1)]
        [Browsable(false)]
        [CategoryAttribute("Contour"), DescriptionAttribute("Legacy compatibility flag. Native WPF contour previews always draw the configured overlay."), DisplayNameAttribute("Legacy draw result")]
        public bool USE_DRAW_IMAGE { get; set; } = false;

        [PropertyOrder(2)]
        [CategoryAttribute("Contour"), DescriptionAttribute("Result overlay shape for contour preview and run results."), DisplayNameAttribute("컨투어 표시")]
        public ContourDrawMode DrawMode { get; set; } = ContourDrawMode.Outline;

        [PropertyOrder(5)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("Approximation")]
        public ContourApproximationModes ApproximationModes { get; set; } = ContourApproximationModes.ApproxSimple;

        [PropertyOrder(6)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("Retrieval mode")]
        public RetrievalModes DetectMode { get; set; } = RetrievalModes.External;

        [PropertyOrder(7)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("Approx epsilon")]
        public double EPSILON { get; set; } = 0.01;

        [PropertyOrder(8)]
        [PropertyEditor(typeof(WpgRangeEditor))]
        [RangeEditor(0, 1000000, 100, 0, nameof(MIN_AREA), nameof(MAX_AREA))]
        [CategoryAttribute("Contour"), DescriptionAttribute("Contour로 인정할 Area(가로*세로) 최소 사이즈입니다."), DisplayNameAttribute("Min area")]
        public int MIN_AREA { get; set; } = 200;

        [PropertyOrder(9)]
        [Browsable(true)]
        [CategoryAttribute("Contour"), DescriptionAttribute(""), DisplayNameAttribute("Max area")]
        public int MAX_AREA { get; set; } = 1000000;

        [PropertyOrder(3)]
        [CategoryAttribute("Contour"), DescriptionAttribute("Contour overlay color."), DisplayNameAttribute("표시 색상")]
        [PropertyEditor(typeof(WpgColorEditor))]
        [XmlIgnore]
        public System.Drawing.Color DrawColor
        {
            get { return m_DrawColor; }
            set { m_DrawColor = value; }
        }

        [PropertyOrder(4)]
        [CategoryAttribute("Contour"), DescriptionAttribute("Contour overlay line thickness."), DisplayNameAttribute("선 두께")]
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
