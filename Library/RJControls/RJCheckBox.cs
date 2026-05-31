using FontAwesome.Sharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;
using System.Drawing.Drawing2D;

namespace RJCodeUI_M1.RJControls
{
    public class RJCheckBox : CheckBox
    {
        /// <summary>
        /// This class inherits from the <see cref = "CheckBox" /> class
        /// You can change the style of the button to glassy or solid (<see cref = "ControlStyle Style" />).
        /// In addition to being able to customize the colors of the background, border size, border radius, icon, etc.
        /// as long as the customizable property is set to true.
        /// Tutorial guide: https://www.youtube.com/watch?v=SAA5qDoiL4M
        /// </summary>       
        /// 

        #region -> Fields

        private bool customizable = false; // Gets or sets if the control is customizable.
        private ControlStyle style = ControlStyle.Glass; // Gets or sets the CheckBox style (Glass or Solid).
        private int borderSize = 1; // Gets or sets the border size.
        private Color borderColor = UIAppearance.StyleColor; // Gets or sets the border color.
        private IconPictureBox CheckIcon; // Gets or sets the checked checkbox icon.

        /// <Note>: ICON PICTURE BOX is provided by the library <see cref = "FontAwesome.Sharp" />
        /// Author: mkoertgen
        /// GitHub: https://github.com/awesome-inc/FontAwesome.Sharp
        /// Nuget Package: https://www.nuget.org/packages/FontAwesome.Sharp </Note>
        #endregion

        #region -> Constructor

        public RJCheckBox()
        {
            CheckIcon = new IconPictureBox(); // Initialize check icon. 
            CheckIcon.IconChar = IconChar.Check; // Set icon.
            CheckIcon.IconSize = 19;
            CheckIcon.IconColor = UIAppearance.StyleColor;
            this.Cursor = Cursors.Hand;
            this.Checked = true;
            this.Font = new Font(UIAppearance.TextFamilyName, UIAppearance.TextSize);
            this.ForeColor = UIAppearance.TextColor;
            this.MinimumSize = new Size(0, 21);
            // Set a Padding of 10 to the left to increase the width of the control, so that the icon and text fit and display completely.
            this.Padding = new Padding(10, 0, 0, 0);
        }
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        [Description("Gets or Sets the CheckBox style (Glass or Solid)")]
        public ControlStyle Style
        {
            get { return style; }
            set
            {
                style = value; // Set value
                this.Image = null; // Remove check icon (when style change, checkIcon image changes, then image is stale, see next method codes)
                ApplyAppearanceSettings(); // Update or apply appearance settings.
                this.Invalidate(); // Redraw the control to update the appearance.
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the width of the border")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                if (value > 0) // The value must be greater than 0.
                    borderSize = value;
                this.Invalidate(); // Redraw the control to update the appearance.
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets to sets if checkbox is checked")]
        public bool Check
        {
            get { return this.Checked; }
            set { this.Checked = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets whether the control's appearance colors are customizable, otherwise the appearance color is set according to the appearance settings.")]
        public bool Customizable
        {
            get { return customizable; }
            set { customizable = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the border color.")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                this.Invalidate(); // Redraw the control to update the appearance.
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the color of the check icon.")]
        public Color IconColor
        {
            get { return CheckIcon.IconColor; }
            set
            {
                CheckIcon.IconColor = value;
                this.Invalidate(); // Redraw the control to update the appearance.
            }
        }

        #endregion

        #region -> Private methods

        private void ApplyAppearanceSettings()
        {// Apply appearance settings as long as the customizable property is set to false.
            if (customizable == false)
            {
                borderColor = UIAppearance.StyleColor;
                this.ForeColor = UIAppearance.TextColor;
                if (style == ControlStyle.Solid)
                    IconColor = Color.White;
                else
                    IconColor = UIAppearance.StyleColor;
            }
        }
        #endregion

        #region -> Overridden methods

        protected override void OnPaint(PaintEventArgs pevent)
        {// This method takes care of completely overriding the paint and drawing a new control with a custom appearance.

            // Fields
            Graphics graphics = pevent.Graphics; // Graphics object.
            int cbSize = 18; // Size of the checkbox.
            int checkIconSize = CheckIcon.IconSize; // Checkbox icon size.
            Rectangle rectCheckBox = new Rectangle() // Checkbox dimensions.
            {
                X = 1,
                Y = (this.Height - cbSize) / 2, // Centered
                Width = cbSize,
                Height = cbSize
            };
            Rectangle rectCheckIcon = new Rectangle() // Dimensions of the checkbox icon.
            {
                X = rectCheckBox.X + ((rectCheckBox.Width - checkIconSize) / 2) + 1, // Centered
                Y = ((this.Height - checkIconSize) / 2) + 2, // Centered
                Width = checkIconSize,
                Height = checkIconSize
            };

            //He drew
            using (Pen penRbBorder = new Pen(borderColor, borderSize)) // Pen to draw the border.
            using (SolidBrush brushRbFill = new SolidBrush(borderColor)) // Brush to draw the fill of the box.
            using (SolidBrush brushText = new SolidBrush(this.ForeColor)) // Brush to draw the text.
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias; // Set the smoothing mode.
                // Draw the control surface
                graphics.Clear(this.BackColor);
                // Draw the checkbox
                if (this.Checked) // Checked state.
                {
                    if (style == ControlStyle.Solid)
                        graphics.FillRectangle(brushRbFill, rectCheckBox); // Fill the box
                    graphics.DrawRectangle(penRbBorder, rectCheckBox); // Border of the box
                    graphics.DrawImage(CheckIcon.Image, rectCheckIcon); // Checkbox icon
                }
                else // State not checked.
                {
                    graphics.DrawRectangle(penRbBorder, rectCheckBox); // Border of the box
                }
                // Draw the text
                graphics.DrawString(this.Text, this.Font, brushText,
                    cbSize + 8, (this.Height - TextRenderer.MeasureText(this.Text, this.Font).Height) / 2); // Y = Centered
            }
        }
        #endregion   

    }
}
