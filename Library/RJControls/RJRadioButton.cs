using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;

namespace RJCodeUI_M1.RJControls
{
    public class RJRadioButton : RadioButton
    {
        /// <summary>
        /// This class inherits from the <see cref = "RadioButton" /> class.
        /// this control completely overrides the paint event and a new layout / appearance is drawn
        ///
        /// Drawings occur in the following order:
        /// 1.- The background of the control surface is drawn.
        /// 2.- The text is drawn.
        /// 3.- The border of the radio button is drawn.
        /// 4.- The background of the radio button is drawn.
        /// 5.- The radio button check mark is drawn.
        /// Tutorial guide: https://www.youtube.com/watch?v=SAA5qDoiL4M
        /// </summary>
        /// 

        #region -> Fields
        private Color checkedColor = UIAppearance.StyleColor; // Gets or sets the color of the marked state.
        private Color unCheckedColor = RJColors.Cancel; // Gets or sets the color of the unchecked state.
        private Color fillColor = UIAppearance.ItemBackgroundColor; // Gets or sets the circular fill color of the radio button.
        private bool customizable = false; // Gets or sets if the colors of the control is customizable.
        #endregion

        #region -> Properties
        [Category("RJ Code Advance")]
        [Description("Gets or sets whether the control's colors are customizable.")]
        public bool Customizable
        {
            get { return customizable; }
            set
            {
                customizable = value;
                if (this.DesignMode)
                {
                    ApplyApperanceSettings();
                    this.Invalidate();
                }
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets whether the control's colors are customizable.")]
        public Color CheckedColor
        {
            get { return checkedColor; }
            set
            {
                checkedColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets whether the control's colors are customizable.")]
        public Color UnCheckedColor
        {
            get { return unCheckedColor; }
            set
            {
                unCheckedColor = value;
                this.Invalidate();
            }
        }
        #endregion

        #region -> Constructor
        public RJRadioButton()
        {
            this.MinimumSize = new Size(0, 21);
            this.Font = new Font("Verdana", UIAppearance.TextSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Cursor = Cursors.Hand;
            // Set a Padding of 10 to the left to increase the width of the control, so that the icon and text fit and display completely.
            this.Padding = new Padding(10, 0, 0, 0);
        }
        #endregion

        #region -> Private methods
        private void ApplyApperanceSettings()
        {
            if (customizable == false)
            {
                checkedColor = UIAppearance.StyleColor;
                unCheckedColor = RJColors.Cancel;
                this.ForeColor = UIAppearance.TextColor;
                if (UIAppearance.Theme == UITheme.Light)
                    fillColor = Utils.ColorEditor.Lighten(UIAppearance.BackgroundColor, 15);
                else fillColor = Utils.ColorEditor.Darken(UIAppearance.BackgroundColor, 3);
            }
        }
        #endregion

        #region -> Overridden methods
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyApperanceSettings();
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {// This method takes care of completely overriding the paint and drawing a new control with a custom appearance.

            // Fields
            Graphics graphics = pevent.Graphics; // Graphics object.
            int rbSize = 18; // Size of the radio button.
            int rbCheckSize = 12; // Size of the radio button check icon.
            Rectangle rectRadioButton = new Rectangle() // Dimensions of the radio button.
            {
                X = 1,
                Y = (this.Height - rbSize) / 2, // Centered
                Width = rbSize,
                Height = rbSize
            };
            Rectangle rectRbCheck = new Rectangle() // Dimensions of the radio button markup icon.
            {
                X = rectRadioButton.X + ((rectRadioButton.Width - rbCheckSize) / 2), // Centered
                Y = (this.Height - rbCheckSize) / 2, // Centered
                Width = rbCheckSize,
                Height = rbCheckSize
            };

            if (customizable && this.Parent != null)//Set the background color if the control is customizable.
            {
                if (UIAppearance.Theme == UITheme.Light)
                fillColor = Utils.ColorEditor.Lighten(this.Parent.BackColor, 10);
                else fillColor = Utils.ColorEditor.Darken(this.Parent.BackColor, 3);
            }

            // Drawing
            using (Pen penRbBorder = new Pen(checkedColor, 1.5F)) // Pen to draw the border.
            using (SolidBrush brushRbFill = new SolidBrush(fillColor)) // Brush to draw the circular fill.
            using (SolidBrush brushRbCheck = new SolidBrush(checkedColor)) // Brush to draw the markup icon.
            using (SolidBrush brushText = new SolidBrush(this.ForeColor)) // Brush to draw the text.
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias; // Set the smoothing mode.

                // Draw the control surface
                graphics.Clear(this.BackColor);
                // Draw the radio button
                if (this.Checked) // Checked state
                {
                    graphics.FillEllipse(brushRbFill, rectRadioButton); // Circular fill of the radio button
                    graphics.DrawEllipse(penRbBorder, rectRadioButton); // Circular border of the radio button
                    graphics.FillEllipse(brushRbCheck, rectRbCheck); // Circular marking of the radio button
                }
                else // Unchecked state
                {
                    penRbBorder.Color = unCheckedColor; // Change the color of the pen
                    graphics.FillEllipse(brushRbFill, rectRadioButton); // Circular fill of the radio button
                    graphics.DrawEllipse(penRbBorder, rectRadioButton); // Circular border of the radio button
                }
                // Draw the text
                graphics.DrawString(this.Text, this.Font, brushText,
                    rbSize + 8, (this.Height - TextRenderer.MeasureText(this.Text, this.Font).Height) / 2); // Y = Centered
            }
        }
        #endregion
    }
}
