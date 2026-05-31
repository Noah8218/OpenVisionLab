using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RJCodeUI_M1.RJControls
{
    public class RJImageColorOverlay : Panel
    {
        /// <summary>
        /// This class enherits from the <see cref="Panel"/> class 
        /// This control incorporates the function of overlay color to images. This means that you cover 
        /// an image with a semi-transparent color box. This method is very popular on websites because
        /// it improves the appearance considerably and the elements on it are more readable, so I decided 
        /// to make one for windows forms.
        /// 
        /// the trick here is to draw a box with a semi-transparent color (you can set opacity) on the panel
        /// with an image background.To get a semi-transparent color, set the alpha color component.
        /// In Windows, the alpha component is a number between 0 (fully transparent) and 255 (fully opaque).
        /// </summary>
        /// 

        #region -> Fields

        private int opacity; // Set or get opacity (percentage of transparency, 0 = fully transparent and 100 fully opaque / solid)
        private int alpha; // Sets or gets the value of the alpha parameter
        private Color overlayColor; // Set or get the overlay color
        private int borderRadius; // Set or get the border radius
        private bool customizable; // Sets or gets if the control's appearance color is customizable.

        #endregion

        #region -> Constructor

        public RJImageColorOverlay()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | // Set control styles to avoid flickering
                                       ControlStyles.UserPaint |
                                       ControlStyles.OptimizedDoubleBuffer,
                                       true);
            //this.BackgroundImage = Properties.Resources.BackImage; // Set default image
            this.BackgroundImageLayout = ImageLayout.Stretch; // Set the default image layout
            this.TabStop = false;
            this.Size = new System.Drawing.Size(250, 250);
            Opacity = 50; // Set a default opacity of 50%
            OverlayColor = Color.MediumSlateBlue; // Set the default overlay color

        }
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        [Description("Gets or sets whether the control's appearance colors are customizable")]
        public bool Customizable
        {
            get { return customizable; }
            set { customizable = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Set or get opacity (percentage of transparency, 0 = fully transparent and 100 fully opaque / solid)")]
        public int Opacity
        {
            get { return opacity; }
            set
            {
                if (value < 0 || value > 100)
                    return;
                opacity = value; // Set value
                alpha = Convert.ToInt32(opacity / 100D * 255); // Convert the opacity value to a valid alpha value for windows.
                // Alpha = Opacity / 100% * 255 (On Windows, the maximum alpha value is 255, which is completely opaque)
                // The suffix D is of type double which would be equal to 100.00 or (double) 100.
                if (this.DesignMode) this.Invalidate(false); // Redraw the control to apply the changes (invokes the OnPaint event) -> preview in design mode
            }
        }

        [Category("RJ Code Advance")]
        [Description("Set or get the overlay color")]
        public Color OverlayColor
        {
            get { return overlayColor; }
            set
            {
                overlayColor = value; // Set value
                // Redraw the control to apply the changes (invokes the OnPaint event) -> preview in design mode
                if (this.DesignMode) this.Invalidate(false);
            }
        }

        [Category("RJ Code Advance")]
        [Description("Set or get the background image")]
        public Image Image
        {
            get { return this.BackgroundImage; }
            set { this.BackgroundImage = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Set or get the image layout")]
        public ImageLayout ImageMode
        {
            get { return this.BackgroundImageLayout; }
            set { this.BackgroundImageLayout = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Set or get the border radius")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                borderRadius = value;
                this.Invalidate(); // Redraw the control to update the appearance.
            }
        }

        #endregion

        #region -> Private methods
        private void ApplyAppearanceSettings()
        {// Apply the appearance settings to the control
            if (customizable == false)
            {
                overlayColor = Settings.UIAppearance.StyleColor;
            }
        }

        #endregion

        #region -> Overridden methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyAppearanceSettings();
        }
        protected override void OnPaint(PaintEventArgs e)
        {// Occurs when the control is redrawn

            base.OnPaint(e); // Draw the control normally
            Graphics graph = e.Graphics;
            // Create a solid color brush object from the overlay color parameter and the opacity set in the alpha parameter
            using (var brush = new SolidBrush(Color.FromArgb(alpha, overlayColor)))
            {
                graph.FillRectangle(brush, this.ClientRectangle); // Fill the inside of the panel with the solid brush created
                // Basically draw a transparent colored rectangle (based on the alpha value defined by opacity) on the panel.
                Utils.RoundedControl.RegionAndSmoothed(this, borderRadius, graph); // Apply Rounded Corners + border Smoothing.
            }
        }
        #endregion

    }
}