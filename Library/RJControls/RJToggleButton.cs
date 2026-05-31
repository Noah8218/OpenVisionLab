using RJCodeUI_M1.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.RJControls
{
    public class RJToggleButton : CheckBox
    {
        /// <summary>
        /// This class enherits from <see cref="CheckBox"/> class.
        /// This control completely overrides the paint event and a new ToggleButton control
        /// design is drawn with the colors assigned in the appearance settings, allows to
        /// change the style to solid or glass.
        /// 
        /// The drawings occur in the following order:
        ///     1.- The background of the control surface is drawn.       
        ///     2.- The border(glass style) or background (solid style) of the toggle button is drawn.
        ///     3.- The toggle button lever is drawn (ON or OFF).
        ///     4.- The text is drawn.
        ///     
        /// It exposes all the functionalities (properties, methods and events of the checkbox control)
        /// and new properties are added to customize the activation lever control.
        /// Tutorial guide: https://www.youtube.com/watch?v=m7Iv6xfjnuw&t
        /// </summary>
        /// 

        #region -> Fields

        private ControlStyle style; // Gets or sets the CheckBox style (Glass or Solid)

        private Color onBackBorderColor = Color.MediumSlateBlue; // Gets or sets the background or border color in the on state.
        private Color onToggleColor = Color.White; // Gets or sets the toggle color in the on state.
        private Color onTextColor = Color.White; // Gets or sets the color of the text in the on state.
        private string onText; // Gets or sets the text in the on state.

        private Color offBackBorderColor = Color.LightGray; // Gets or sets the background or border color in the off state.
        private Color offToggleColor = Color.Gray; // Gets or sets the toggle color in the off state.
        private Color offTextColor = Color.Gray; // Gets or sets the color of the text in the off state.
        private string offText; // Gets or sets the text in the off state.

        private bool customizable; // Gets or sets whether the control color is customizable or the colors are set according to the appearance settings

        #endregion

        #region -> Constructor

        public RJToggleButton()
        {
            this.MinimumSize = new Size(50, 25);
            Activated = true;// Set initial value
            Style = ControlStyle.Glass;
        }
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        public string ON_Text
        {// Gets or sets the text for the on state
            get { return onText; }
            set
            {
                onText = value;
                this.Invalidate(); // Redraw the control (See the OnPaint (PaintEventArgs e) method)
            }
        }

        [Category("RJ Code Advance")]
        public string OFF_Text
        {// Gets or sets the text for the disabled state
            get { return offText; }
            set
            {
                offText = value;
                this.Invalidate(); // Redraw the control (See the OnPaint (PaintEventArgs e) method)
            }
        }

        [Category("RJ Code Advance")]
        public ControlStyle Style
        {// Sets or gets the toggle button style (glass or solid)
            get { return style; }
            set
            {
                style = value; // Set value
                ApplyAppearanceSettings(); // Update the appearance settings.
                this.Invalidate(); // Redraw the control (See the OnPaint (PaintEventArgs e) method)

            }
        }

        [Category("RJ Code Advance")]
        public bool Activated
        {// sets or gets if the toggle button is on or off.
            // this value is determined by the checked property of the checkbox
            // since they both have a single value: Checked or UnChecked - ON or OFF = true or false.
            get { return this.Checked; }
            set
            {
                this.Checked = value;
            }
        }

        [Category("RJ Code Advance")]
        public bool Customizable
        {// Gets or sets whether the control color is customizable or the colors are set by the appearance settings
            get { return customizable; }
            set { customizable = value; }
        }

        [Category("RJ Code Advance")]
        public Color ON_BackBorderColor
        {// Gets or sets the background or border color of the activated state.
            get { return onBackBorderColor; }
            set
            {
                onBackBorderColor = value;
                this.Invalidate(); // (See OnPaint (PaintEventArgs e) method)
            }
        }

        [Category("RJ Code Advance")]
        public Color ON_ToggleColor
        {// Gets or sets the toggle color for the on state.
            get { return onToggleColor; }
            set
            {
                onToggleColor = value;
                this.Invalidate(); // Redraw the control (See the OnPaint (PaintEventArgs e) method)
            }
        }

        [Category("RJ Code Advance")]
        public Color ON_TextColor
        {// Gets or sets the text color of the activated state.
            get { return onTextColor; }
            set
            {
                onTextColor = value;
                this.Invalidate(); // Redraw the control (See the OnPaint (PaintEventArgs e) method)
            }
        }

        [Category("RJ Code Advance")]
        public Color OFF_BackBorderColor
        {// Gets or sets the background or border color for the off state.
            get { return offBackBorderColor; }
            set
            {
                offBackBorderColor = value;
                this.Invalidate(); // Redraw the control (See the OnPaint (PaintEventArgs e) method)
            }
        }

        [Category("RJ Code Advance")]
        public Color OFF_ToggleColor
        {// Gets or sets the toggle color for the off state.
            get { return offToggleColor; }
            set
            {
                offToggleColor = value;
                this.Invalidate(); // Redraw the control (See the OnPaint (PaintEventArgs e) method)
            }
        }

        [Category("RJ Code Advance")]
        public Color OFF_TextColor
        {// Gets or sets the text color for the disabled state.
            get { return offTextColor; }
            set
            {
                offTextColor = value;
                this.Invalidate(); // Redraw the control (See the OnPaint (PaintEventArgs e) method)
            }
        }

        [Category("RJ Code Advance")]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        public override string Text
        {// Override text property
            get { return base.Text; }
            set { base.Text = "#"; }
        }

        #endregion

        #region -> Private methods
        private void ApplyAppearanceSettings()
        {// Apply appearance settings as long as they are not customizable

            if (customizable == false)
            {

                if (UIAppearance.Theme == UITheme.Light) // Set theme colors LIGHT
                {
                    if (style == ControlStyle.Solid) // Solid style.
                    {
                        onBackBorderColor = UIAppearance.StyleColor;
                        onToggleColor = Color.White;
                        onTextColor = Color.WhiteSmoke;

                        offBackBorderColor = Color.LightGray;
                        offToggleColor = Color.White;
                        offTextColor = Color.Gray;
                    }
                    else // Glass style
                    {

                        onBackBorderColor = UIAppearance.StyleColor;
                        onToggleColor = UIAppearance.StyleColor;

                        onTextColor = UIAppearance.TextColor;

                        offBackBorderColor = Color.FromArgb(171, 171, 171);
                        offToggleColor = Color.FromArgb(171, 171, 171);
                        offTextColor = Color.Gray;
                    }

                }
                else // Set DARK theme colors
                {

                    onBackBorderColor = UIAppearance.StyleColor;

                    if (style == ControlStyle.Solid)
                    {
                        onToggleColor = RJColors.DarkItemBackground;
                        onTextColor = Color.Gainsboro;
                    }
                    else
                    {
                        onToggleColor = UIAppearance.StyleColor;
                        onTextColor = Utils.ColorEditor.Lighten(UIAppearance.TextColor, 20);
                    }

                    offBackBorderColor = RJColors.DarkActiveBackground;
                    offToggleColor = Color.FromArgb(110, 104, 153);
                    offTextColor = UIAppearance.TextColor;
                }
            }
        }

        private GraphicsPath GetFigurePath(Rectangle rect)
        {// This method is responsible for making the path of the ToggleButton figure.
            int arcSize = rect.Height; // Size of the arc.
            Rectangle leftArc = new Rectangle(rect.X, rect.Y, arcSize, arcSize); // Left arc
            Rectangle rightArc = new Rectangle(rect.Width - arcSize, rect.Y, arcSize, arcSize); // Right arch

            GraphicsPath path = new GraphicsPath(); // GraphicsPath object
            path.StartFigure(); // Start the figure
            path.AddArc(leftArc, 90, 180); // Add the left arc at the 90 degree angle with a 180 degree travel.
            path.AddArc(rightArc, 270, 180); // Add the right arc at the 270 degree angle with a 180 degree path.
            path.CloseFigure(); // End the figure

            return path; // Return the path of the figure.
        }
        #endregion

        #region -> Overridden methods

        protected override void OnPaint(PaintEventArgs pevent)
        {// This method completely overrides the paint and redraws a new layout and appearance of the control.

            // FIELDS
            Graphics graph = pevent.Graphics; // Graphics object
            Rectangle tbRect = Rectangle.Inflate(this.ClientRectangle, -2, -2); // Dimensions for the toggle button.
            float toggleSize = tbRect.Height - 4.8F; // Toggle size.
            RectangleF rectOffToggle = new RectangleF() // Dimensions for the lever in the off state.
            {
                X = tbRect.X + ((tbRect.Height - toggleSize) / 2),
                Y = tbRect.Y + ((tbRect.Height - toggleSize) / 2),
                Width = toggleSize,
                Height = toggleSize
            };
            RectangleF rectOnToggle = new RectangleF() // Dimensions for the lever in the on state.
            {
                X = tbRect.Width - toggleSize - ((tbRect.Height - toggleSize) / 2),
                Y = tbRect.Y + ((tbRect.Height - toggleSize) / 2),
                Width = toggleSize,
                Height = toggleSize
            };

            //HE DREW
            using (GraphicsPath path = GetFigurePath(tbRect)) // Get the path of the figure
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias; // Set the smoothing mode.

                // Draw the surface
                graph.Clear(this.Parent.BackColor);

                // Draw the toggle button in the on state.
                if (this.Checked) // ON
                {
                    // Draw the background of the toggle button
                    if (style == ControlStyle.Solid)
                        graph.FillPath(new SolidBrush(onBackBorderColor), path);
                    else graph.DrawPath(new Pen(onBackBorderColor, 2), path);
                    // Draw the lever
                    graph.FillEllipse(new SolidBrush(onToggleColor), rectOnToggle);
                    // Draw the text - Centered
                    if (onText != "" || onText != null)
                        graph.DrawString(onText, this.Font, new SolidBrush(onTextColor),
                        ((tbRect.Right - TextRenderer.MeasureText(onText, this.Font).Width) / 2) - (toggleSize / 2),
                          ((this.Height - TextRenderer.MeasureText(onText, this.Font).Height) / 2));
                }
                // Draw the toggle button in the off state.
                else // OFF
                {
                    // Draw the background of the toggle button
                    if (style == ControlStyle.Solid)
                        graph.FillPath(new SolidBrush(offBackBorderColor), path);
                    else pevent.Graphics.DrawPath(new Pen(offBackBorderColor, 2), path);
                    // Draw the lever
                    graph.FillEllipse(new SolidBrush(offToggleColor), rectOffToggle);
                    // Draw the text - Centered
                    if (offText != "" || offText != null)
                        graph.DrawString(offText, this.Font, new SolidBrush(offTextColor),
                          ((tbRect.Right - TextRenderer.MeasureText(onText, this.Font).Width) / 2) + (toggleSize / 2),
                          ((this.Height - TextRenderer.MeasureText(onText, this.Font).Height) / 2));

                }
            }
        }
        #endregion

    }
}
