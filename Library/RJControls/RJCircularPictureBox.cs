using RJCodeUI_M1.Settings;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RJCodeUI_M1.RJControls
{
    public class RJCircularPictureBox : PictureBox
    {
        /// <summary>
        /// This class inherits from the <see cref = "PictureBox" /> class
        /// This custom control creates a circular image box, where it allows to change the border size,
        /// border color, line style, cap style, enable border gradient color,
        /// and finally set the angle of the gradient.
        /// Tutorial guide: https://youtu.be/54347QjDisA
        /// </summary>
        ///

        #region -> Fields
        private int borderSize = 2; // Gets or sets the border size
        private Color borderColor = Color.RoyalBlue; // Gets or sets the border color.
        private Color borderColor2 = Color.HotPink; // Gets or sets the second border color to apply a gradient color.
        private DashStyle borderLineStyle = DashStyle.Solid; // Gets or sets the line style of the border.
        private DashCap borderCapStyle = DashCap.Flat; // Gets or sets the cap style of the border to the dash style (Dash).
        private bool gradientColor = false; // Gets or sets if the border color is gradient.
        private float gradientAngle = 50F; // Gets or sets the angle of the gradient.
        private bool customizable = true; // Gets or sets if the border color is customizable, or the color is equal to the color set in the appearance settings.
        #endregion

        #region -> Constructor
        public RJCircularPictureBox()
        {
            this.Size = new Size(100, 100);
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        #endregion

        #region -> Properties
        [Category("RJ Code Advance")]
        public bool Customizable
        {
            get { return customizable; }
            set { customizable = value; }
        }

        [Category("RJ Code Advance")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color BorderColor2
        {
            get { return borderColor2; }
            set
            {
                borderColor2 = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public DashStyle BorderLineStyle
        {
            get { return borderLineStyle; }
            set
            {
                borderLineStyle = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public DashCap BorderCapStyle
        {
            get { return borderCapStyle; }
            set
            {
                borderCapStyle = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public bool GradientColor
        {
            get { return gradientColor; }
            set
            {
                gradientColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public float GradientAngle
        {
            get { return gradientAngle; }
            set
            {
                gradientAngle = value;
                this.Invalidate();
            }
        }
        #endregion

        #region -> Private methods
        private void ApplyAppearanceSettings()
        {
            if (customizable == false) // If not customizable, apply appearance color settings.
            {
                borderColor = UIAppearance.StyleColor;
                borderColor2 = UIAppearance.PrimaryStyleColor;
            }
        }
        #endregion

        #region -> Overridden methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyAppearanceSettings(); // Apply UI appearance settings
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Set the same value for the width and height of the size so that the control has a perfect circular shape.
            this.Size = new Size(this.Width, this.Width);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            // Fields
            var graph = pe.Graphics; // Graphics object.
            var rectContourSmooth = Rectangle.Inflate(this.ClientRectangle, -1, -1); // Rectangle for the dimensions of the contour smoothing and rounded region of the control.
            var rectBorder = Rectangle.Inflate(rectContourSmooth, -borderSize, -borderSize); // Rectangle for the border dimensions.
            var rectBorderSmooth = RectangleF.Inflate(rectBorder, 0.5F, 0.5F);
            var smoothSize = borderSize > 0 ? borderSize : 1; // Smoothing size, If the border size is greater than 0, the Smoothing size is equal to the border size, otherwise the value is 1.
            using (var pathRegion = new GraphicsPath()) // Graphics path object for the control region.
            using (var penSmooth = new Pen(this.Parent.BackColor, smoothSize)) // Pen object to draw the contour smoothing.
            using (var penBorder = new Pen(borderColor, borderSize)) // Pen object to draw the border of the control.
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias; // Set the smoothing mode of the graphics object.
                penBorder.DashStyle = borderLineStyle; // Set the line style of the border.
                penBorder.DashCap = borderCapStyle; // Set the cap style of the line (Dash or Period).
                pathRegion.AddEllipse(rectContourSmooth); // Add an ellipse in the path of the region with the dimensions of the previously defined rectangle.
                // Set the rounded region of the control.
                this.Region = new Region(pathRegion);

                //He drew
                graph.DrawEllipse(penSmooth, rectContourSmooth); // Draw the contour smoothing.
                if (borderSize > 0)
                {
                    graph.DrawEllipse(penSmooth, rectBorderSmooth); // Draw the border smoothing
                    if (gradientColor) // Draw the gradient color border.
                    {
                        using (var bGradientColor = new LinearGradientBrush(rectBorder, borderColor, borderColor2, gradientAngle))
                        {
                            penBorder.Brush = bGradientColor;
                            graph.DrawEllipse(penBorder, rectBorder);
                        }
                    }
                    // Draw the solid color border, a single color.
                    else graph.DrawEllipse(penBorder, rectBorder);
                }
            }
        }
        #endregion

    }
}
