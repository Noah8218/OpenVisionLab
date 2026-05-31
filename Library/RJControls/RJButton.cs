using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;
using FontAwesome.Sharp;
using RJCodeUI_M1.Utils;

namespace RJCodeUI_M1.RJControls
{
    public class RJButton : IconButton
    {
        ///<summary>
        /// ------ This class inherits from the <see cref = "IconButton" /> class of the library <see cref = "FontAwesome.Sharp" />
        /// Author: mkoertgen
        /// GitHub: https://github.com/awesome-inc/FontAwesome.Sharp
        /// Nuget Package: https://www.nuget.org/packages/FontAwesome.Sharp
        /// This button implements 4 appearance properties:
        /// -> You can change the style of the button to Glass or Solid (<see cref = "ControlStyle" />).
        /// -> You can change the button design to Normal, Icon Button, Metro, Confirm, Cancel or Delete (<see cref = "ButtonDesign" />).
        /// -> You can set a rounded border, Property: <see cref = "int BorderRadius" />).
        /// -> You can change the button border size, Property: <see cref = "int BorderSize" />).
        /// Tutorial guide: https://www.youtube.com/watch?v=u8SL5g9QGcI&t
        /// </summary>

        #region -> Fields

        private ControlStyle style = ControlStyle.Solid; // Gets or sets the style of the button (glass or solid).
        private ButtonDesign design = ButtonDesign.Normal; // Gets or sets the button design (Normal, Icon Button, Metro, Confirm, Cancel, or Delete).
        private int borderRadius = 0; // Gets or sets the size of the border radius (rounded corners).
        private int borderSize = 0; // Gets or sets the size of the button border.
        private Color skinBorderColor = UIAppearance.StyleColor; // Gets or sets the border color if the style is GLASS, or the background color if the style is SOLID;
        private static readonly Color supernovaColor = UIAppearance.Style == UIStyle.Supernova ? RJColors.GetSupernovaColor() : Color.CornflowerBlue; // Sets and gets a supernova color if the style is supernova
        #endregion

        #region -> Constructor

        public RJButton()
        {
            //Initialize properties
            this.FlatAppearance.BorderSize = 0;
            this.FlatStyle = FlatStyle.Flat;
            this.BackColor = UIAppearance.StyleColor;
            this.Flip = FlipOrientation.Normal;
            this.Font = new Font("Microsoft Sans Serif", UIAppearance.TextSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = Color.White;
            this.IconSize = 24;
            this.IconColor = Color.White;
            this.Rotation = 0D;
            this.Size = new Size(170, 40);
            SetButtonDesign();
        }


        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        [Description("Gets or sets the button design (Normal, Icon Button, Subway, Confirm, Cancel or Delete)")]
        public ButtonDesign Design
        {
            get { return design; }
            set
            {
                design = value;
                if (design != ButtonDesign.Custom) // If the design is not custom,
                    SetButtonDesign(); // Set the specified design.
                // It is not necessary to call the invalidate () method, since when changing the background or text color the method is invoked automatically.
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the button style (glass or solid)")]
        public ControlStyle Style
        {
            get { return style; }
            set
            {
                style = value; // Assign the specified value.
                SetButtonStyle(); // And set the specified style.
                // It is not necessary to call the invalidate () method, since when changing the background or text color the method is invoked automatically.               
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the border color if the style is GLASS, or the background color if the style is SOLID")]
        public Color BorderColor
        {
            get { return skinBorderColor; }
            set
            {
                skinBorderColor = value;
                this.Invalidate(); // Redraw the control to update the appearance.
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the button border size.")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                this.Invalidate(); // Redraw the control to update the appearance.
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the size of the border radius (rounded corners).")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                if (value >= 0)
                {
                    borderRadius = value;
                    this.Invalidate(); // Redraw the control to update the appearance.
                }
            }
        }

        [DefaultValue(typeof(Color), "Color.Black")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }
        #endregion

        #region -> Private methods

        private void SetButtonStyle()
        {// This method is in charge of setting the style of the button.
            if (design != ButtonDesign.Custom)
            {
                if (style == ControlStyle.Solid) // Solid Style
                {
                    this.BackColor = skinBorderColor;
                    this.ForeColor = Color.White;
                    this.IconColor = Color.White;
                }
                else // Glass Style.
                {
                    if (borderSize < 1) borderSize = 1;
                    this.ForeColor = skinBorderColor;
                    this.IconColor = skinBorderColor;
                    this.BackColor = Color.Empty;
                }
                // It is not necessary to call the invalidate () method, since when changing the background or text color the method is invoked automatically.
            }
        }

        private void SetButtonDesign()
        {// This method is responsible for setting the button design.
            switch (design)
            {
                case ButtonDesign.Normal: // Normal Button (no icon)
                    SetNormalButton(); // Set the properties of a button without an icon.
                    ApplyAppearanceSettings(); // Get and set the border or background color of the appearance settings.
                    break;

                case ButtonDesign.IconButton: // Icon Button
                    SetIconButton(); // Set the properties of an icon button.
                    // If it has no icon or the icon is to confirm, cancel or delete, set a default icon.
                    if (this.IconChar == FontAwesome.Sharp.IconChar.None || this.IconChar == FontAwesome.Sharp.IconChar.Check ||
                          this.IconChar == FontAwesome.Sharp.IconChar.TrashAlt || this.IconChar == FontAwesome.Sharp.IconChar.TimesCircle)
                        this.IconChar = FontAwesome.Sharp.IconChar.Magento; // Default icon.
                    ApplyAppearanceSettings(); // Get and set the border or background color of the appearance settings.
                    break;

                case ButtonDesign.Metro: // Metro Button
                    SetMetroButton(); // Set the properties of a metro button.
                    // If it has no icon or the icon is confirm, cancel or delete, set a default icon.
                    if (this.IconChar == FontAwesome.Sharp.IconChar.None || this.IconChar == FontAwesome.Sharp.IconChar.Check ||
                         this.IconChar == FontAwesome.Sharp.IconChar.TrashAlt || this.IconChar == FontAwesome.Sharp.IconChar.TimesCircle)
                        this.IconChar = FontAwesome.Sharp.IconChar.Magento; // Default icon.
                    ApplyAppearanceSettings(); // Get and set the border or background color of the appearance settings.
                    break;

                case ButtonDesign.Confirm: // Confirm button
                    SetIconButton(); // Set the properties of an icon button.
                    this.IconChar = FontAwesome.Sharp.IconChar.Check; // Set confirm icon
                    this.Text = "Confirm"; // Set text
                    skinBorderColor = RJColors.Confirm; // Border or background color.
                    break;

                case ButtonDesign.Delete: // Delete button
                    SetIconButton(); // Set the properties of an icon button.
                    this.IconChar = FontAwesome.Sharp.IconChar.TrashAlt; // Set delete icon
                    this.Text = "Delete"; // Set text
                    skinBorderColor = RJColors.Delete; // Border or background color.
                    break;

                case ButtonDesign.Cancel: // Cancel button
                    SetIconButton(); // Set the properties of an icon button.
                    this.IconChar = FontAwesome.Sharp.IconChar.TimesCircle; // Set cancel icon
                    this.Text = "Cancel"; // Set text
                    skinBorderColor = RJColors.Cancel; // Border or background color.
                    break;
            }
            SetButtonStyle();
        }
        private void SetNormalButton()
        {// This method is responsible for setting the properties of the normal button without icon.

            this.TextImageRelation = TextImageRelation.Overlay;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.IconChar = FontAwesome.Sharp.IconChar.None;
            if (this.DesignMode)
                this.Size = new Size(160, 40); // Reset the specified size
        }
        private void SetIconButton()
        {// This method is responsible for setting the icon button properties.

            this.TextAlign = ContentAlignment.MiddleRight;
            this.ImageAlign = ContentAlignment.MiddleCenter;
            this.TextImageRelation = TextImageRelation.ImageBeforeText;
            if (this.DesignMode) // Reset specified size and icon only at design time.
            {
                this.Size = new Size(160, 40);
                this.IconSize = 24;
            }
        }
        private void SetMetroButton()
        {// This method is in charge of setting the metro button properties.

            this.TextAlign = ContentAlignment.BottomCenter;
            this.ImageAlign = ContentAlignment.MiddleCenter;
            this.TextImageRelation = TextImageRelation.ImageAboveText;
            if (this.DesignMode) // Set the specified size and icon for meter (only at design time)
            {
                this.Size = new Size(110, 110);
                this.IconSize = 50;
            }
        }
        private void ApplyAppearanceSettings()
        {// This method is in charge of setting the border or background color of the appearance settings.
            if (design == ButtonDesign.Normal || design == ButtonDesign.IconButton || design == ButtonDesign.Metro)
            {
                if (UIAppearance.Style == UIStyle.Supernova) skinBorderColor = supernovaColor;
                else skinBorderColor = UIAppearance.StyleColor;
            }
            this.FlatAppearance.MouseOverBackColor = ColorEditor.Darken(this.BackColor, 12);
            this.FlatAppearance.MouseDownBackColor = ColorEditor.Darken(this.BackColor, 6);
        }
        #endregion

        #region -> Overridden methods

        protected override void OnPaint(PaintEventArgs pevent)
        {// This method is in charge of establishing the region of the button and drawing the border.
            base.OnPaint(pevent);
            Graphics graph = pevent.Graphics;

            if (style == ControlStyle.Glass) // Glass Style
                // Apply rounded corners to the region and smooth + draw the border.
                RoundedControl.RegionAndBorder(this, borderRadius, graph, skinBorderColor, borderSize);
            else // Solid style
                // Apply rounded corners to the region and smooth the border.
                RoundedControl.RegionAndSmoothed(this, borderRadius, graph);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Check to validate the radius. (The maximum radius is half the value of the top of the control)
            int maxRadius = this.Height / 2;
            if (borderRadius > maxRadius)
                borderRadius = maxRadius;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyAppearanceSettings(); // Apply the appearance settings.
        }

        #endregion

    }
}

