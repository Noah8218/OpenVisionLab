using FontAwesome.Sharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;
using System.Drawing.Drawing2D;

namespace RJCodeUI_M1.RJControls
{
    [DefaultEvent("OnValueChanged")] // Default event when double-clicking the control in the designer
    public class RJDatePicker : UserControl
    {
        /// <summary>
        /// This class inherits the <see cref = "UserControl" /> class.
        /// This control exposes most of the most used essential properties and events of a normal DateTimePicker, you can add the functionalities that you probably need and are missing here.
        /// On the other hand, you can change the style to solid or glass, also customize the drop down menu,
        /// calendar icon, border color and border radius (rounded corners only affect solid style).
        /// Tutorial guide: https://www.youtube.com/watch?v=IJM9SIX0pIs&t (Custom data picker - No UserControl)
        /// Tutorial guide 2: https://www.youtube.com/watch?v=CkpUQYzYCC8&t (Custon text box - UserControl)
        /// </summary>
        /// 

        #region -> Fields

        // Fields
        private int borderRadius; // Gets or sets the size of the border radius (rounded corners).
        private int borderSize = 1; // Gets or sets the border size
        private ControlStyle style; // Gets or sets the control style (Glass or Solid).
        private Color backgroundColor; // Gets or sets the Background Color.
        private Color borderColor; // Gets or sets the Border Color.
        private Color iconColor; // Gets or sets the Icon Color.
        private bool customizable; // Gets or sets if the control's appearance colors are customizable.
        private bool isDroppedDown;//Gets or sets whether the drop-down calendar is open.
        // Controls
        private Label dateText; // Gets or sets the Label of the DateTimePicker (to display the date text of the DateTimePicker).
        private IconButton btnIcon; // Gets or sets the Dropdown Calendar Icon (button to show the calendar).
        private DateTimePicker datePicker; // Gets or sets the Date Picker (not visible, but can show the dropdown calendar).

        /// <Note>: ICON BUTTON is provided by the <see cref = "FontAwesome.Sharp" /> library.
        /// Author: mkoertgen
        /// GitHub: https://github.com/awesome-inc/FontAwesome.Sharp
        /// Nuget Package: https://www.nuget.org/packages/FontAwesome.Sharp </Note>
        #endregion

        #region -> Events

        [Category("RJ Code Advance")]
        public event EventHandler OnValueChanged; // Default event of the control.      
        #endregion

        #region -> Constructor

        public RJDatePicker()
        {
            dateText = new Label();
            datePicker = new DateTimePicker();
            btnIcon = new IconButton();
            this.SuspendLayout();
            //
            // IconButton: Dropdown Calendar Icon
            //
            btnIcon.Dock = DockStyle.Right;
            btnIcon.FlatAppearance.BorderSize = 0;
            btnIcon.FlatStyle = FlatStyle.Flat;
            btnIcon.Flip = FlipOrientation.Normal;
            btnIcon.IconChar = IconChar.CalendarAlt; // Set calendar icon.
            btnIcon.IconColor = Color.White;
            btnIcon.IconSize = 22;
            btnIcon.Location = new Point(189, 1);
            btnIcon.Name = "dropdownArrowIcon";
            btnIcon.Rotation = 0D;
            btnIcon.Size = new Size(30, 30);
            btnIcon.UseVisualStyleBackColor = false;
            btnIcon.Cursor = Cursors.Hand;
            btnIcon.Click += new EventHandler(ButtonIcon_Click); // Subscribe to event click (To show the calendar)

            //
            // Label: Date Text (Represents the visual part of the DateTimePicker, displays the text of the date, and FILLS in the user control)
            //
            dateText.Dock = DockStyle.Fill; // Set padding as dock style
            dateText.FlatStyle = FlatStyle.Flat;
            dateText.Font = new Font("Microsoft Sans Serif", 10F);
            dateText.ForeColor = Color.Gray;
            dateText.ImageAlign = ContentAlignment.MiddleLeft;
            dateText.Location = new Point(1, 1);
            dateText.Name = "dateText";
            dateText.Padding = new Padding(8, 0, 0, 0);
            dateText.TextAlign = ContentAlignment.MiddleLeft;
            dateText.Controls.Add(btnIcon); // Add the icon button to the label.

            /* Since the (dateText) tag represents the visual part of the ComboBox and takes up most of the user control,
             It is necessary to attach the events that occur in the tag (dateText) to the events of the user control (RJDatePicker),
             That is, for example in the Click event, when the label is clicked, the click event of the user control (RJDatePicker) must also be executed.
             See the definition of the event methods to understand better. */
            dateText.Click += new EventHandler(RJDatePicker_Click);
            dateText.KeyDown += new KeyEventHandler(RJDatePicker_KeyDown);
            dateText.KeyPress += new KeyPressEventHandler(RJDatePicker_KeyPress);
            dateText.KeyUp += new KeyEventHandler(RJDatePicker_KeyUp);
            dateText.MouseEnter += new EventHandler(RJDatePicker_MouseEnter);
            // You can attach whatever events you want, if it doesn't exist, you can create it like you did with the OnValueChanged event.
            
            //
            // DateTimePicker: Date Picker
            //
            datePicker.Name = "datePicker";
            datePicker.Size = new Size(100, 20);
            datePicker.Location = new Point(2, 2);
            datePicker.DropDownAlign = LeftRightAlignment.Right;
            datePicker.CloseUp += new EventHandler(DatePicker_CloseUp); // Subscribe to the CloseUp event (It happens when the calendar is hidden / closed)
            datePicker.ValueChanged += new EventHandler(DatePicker_ValueChanged); // Subscribe the ValueChanged event of the control and attach it to the previously defined OnValueChanged event (see method definition).

            //
            // User control: RJDatePicker
            //

            // Add controls
            this.Controls.Add(dateText); // Fill the remaining space (display the text of the date picker)          
            this.Controls.Add(datePicker); // It's in the background, behind the label (not visible, but shows the drop-down calendar).
            // This order is important, the last controls are added first (from bottom to top)

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.MediumSlateBlue;
            this.MinimumSize = new Size(120, 25);
            this.Padding = new Padding(1);
            this.Size = new Size(260, 32);
            BorderSize = borderSize; // Set border size to apply the necessary adjustments.
            this.ResumeLayout(false);

        }
        #endregion

        #region -> Properties

        #region - Design Properties

        [Category("RJ Code Advance")]
        [Description("Set or get the style (glass or solid)")]
        public ControlStyle Style
        {
            get { return style; }
            set
            {
                style = value; // Set value.
                BorderRadius = borderRadius; // Update border radius.
                if (this.DesignMode)
                    ApplyAppearanceSettings(); // update appearance settings-> preview changes in design mode.
            }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets if the control is customizable")]
        public bool Customizable
        {
            get { return customizable; }
            set { customizable = value; }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the background color")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                backgroundColor = value;
                dateText.BackColor = backgroundColor;
                btnIcon.BackColor = backgroundColor;
            }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the border color")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                this.Invalidate(); //Draw the border (Redraw control) 
            }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets border size")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                if (value >= 1)
                {
                    borderSize = value;
                    this.Padding = new Padding(borderSize + 1); // Set the Padding property to apply a space and draw the border.
                    SetDatePickerLocation(); // Update the location of the combobox.
                    this.Invalidate(); // Draw the border (Redraw the control)
                }
            }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the border radius")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                if (value >= 0)
                {
                    borderRadius = value;
                    SetDatePickerLocation(); // Update the location of the combobox.
                    this.Invalidate(); // Draw the border and radius (Redraw the control)
                }
            }
        }

        [Category("RJ Code Advance")]
        [Description("Set or keep the icon color.")]
        public Color IconColor
        {
            get { return btnIcon.IconColor; }
            set
            {
                iconColor = value;
                btnIcon.IconColor = iconColor;
            }
        }

        [Category("RJ Code Advance")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                dateText.ForeColor = value;
            }
        }
        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the font")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                dateText.Font = value;                
            }
        }
        #endregion

        #region - Functional properties

        [Category("RJ Code Advance")]
        [Description("Gets or sets the format of the date and time displayed in the control")]
        public DateTimePickerFormat Format
        {
            get { return datePicker.Format; }
            set
            {
                datePicker.Format = value;
                dateText.Text = datePicker.Text; // Update date text
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the custom date / time format string")]
        public string CustomFormat
        {
            get { return datePicker.CustomFormat; }
            set
            {
                datePicker.CustomFormat = value;
                dateText.Text = datePicker.Text; // Update date text
            }

        }
        [Category("RJ Code Advance")]
        [Description("Gets or sets the date / time value assigned to the control")]
        public DateTime Value
        {
            get { return datePicker.Value; }
            set
            {
                datePicker.Value = value;
                dateText.Text = datePicker.Text; // Update date text
            }
        }

        [Browsable(false)] // Property not visible in the properties box
        public new string Text // Override the text property of the user control
        {
            get { return datePicker.Text; } // Return the text of the date picker.

        }
        #endregion

        #endregion

        #region -> Private methods

        private void ApplyAppearanceSettings()
        {// Apply appearance settings
            if (customizable == false)
            {
                switch (style)
                {
                    case ControlStyle.Solid: // Solid style.
                        BorderColor = UIAppearance.StyleColor;
                        BorderSize = 0;
                        dateText.ForeColor = Color.White;
                        btnIcon.IconColor = Color.White;
                        this.BackColor = UIAppearance.StyleColor;
                        break;
                    case ControlStyle.Glass://Glass style.

                        btnIcon.IconColor = UIAppearance.StyleColor;
                        dateText.ForeColor = UIAppearance.PrimaryTextColor;
                        this.BackColor = UIAppearance.BackgroundColor;

                        if (UIAppearance.Theme == UITheme.Dark)
                            BorderColor = Utils.ColorEditor.Darken(UIAppearance.TextColor, 10);
                        else BorderColor = UIAppearance.TextColor;
                        break;
                }
            }
        }

        private void SetDatePickerLocation()
        {// // Set the location of the datePicker to the lower right corner.
            if (borderRadius > 2)
                datePicker.Location = new Point(this.Width - datePicker.Width - this.Padding.Right - (borderRadius / 2) - 2, dateText.Bottom - datePicker.Height);
            else
                datePicker.Location = new Point(this.Width - datePicker.Width - this.Padding.Right, dateText.Bottom - datePicker.Height);
        }
        #endregion

        #region -> Event methods

        //-> Default event
        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {// Attach the default event created OnValueChanged of the user control (RJDatePicker)
            // to the main event ValueChanged of the datepicker (datePicker).

            if (OnValueChanged != null)
                OnValueChanged.Invoke(sender, e);

            dateText.Text = datePicker.Text; // When a date is selected from the drop-down calendar, update the label text.
        }

        //-> Component actions
        private void ButtonIcon_Click(object sender, EventArgs e)
        {// When the calendar icon button is clicked, the drop-down calendar will be displayed

            isDroppedDown = true;
            this.datePicker.Select(); // As a precaution, the datepicker needs to be focused before, some problems happen sometimes.
            SendKeys.Send("%{DOWN}"); // This control does not have the DroppedDown property to display the calendar.
            // therefore we will use the operating system shortcut keys to display the calendar: Alt key + down arrow is pressed
            // Alt key =%
            // Down arrow key = DOWN
            // So we send keystrokes when the calendar icon is clicked.
            if (customizable)
            {
                switch (style) // Apply a highlight to the icon button
                {
                    case ControlStyle.Solid:
                        btnIcon.BackColor = Utils.ColorEditor.Darken(backgroundColor, 10);
                        btnIcon.IconColor = Color.White;
                        break;
                    case ControlStyle.Glass:
                        btnIcon.BackColor = borderColor;
                        btnIcon.IconColor = Color.White;
                        BorderColor = borderColor;
                        break;
                }
            }
            else
            {
                switch (style) // Apply a highlight to the icon button
                {
                    case ControlStyle.Solid:
                        btnIcon.BackColor = Utils.ColorEditor.Darken(UIAppearance.StyleColor, 10);
                        btnIcon.IconColor = Color.White;
                        break;
                    case ControlStyle.Glass:
                        btnIcon.BackColor = UIAppearance.StyleColor;
                        btnIcon.IconColor = Color.White;
                        BorderColor = UIAppearance.StyleColor;
                        break;
                }
            }
        }
        private void DatePicker_CloseUp(object sender, EventArgs e)
        {// Occurs when the dropdown calendar closes
            isDroppedDown = false;
            if (customizable)
            {// Disable icon button highlighting.

                btnIcon.BackColor = backgroundColor;
                btnIcon.IconColor = iconColor;
                BorderColor = borderColor;
            }
            else
            {               
                ApplyAppearanceSettings(); // Refresh appearance to disable icon button highlighting.
            }
        }

        //->Attach events: Label -> UserControl
        private void RJDatePicker_Click(object sender, EventArgs e)
        {// Attach the click event of the user control (RJComboBox) to this click event of the tag (dateText).
            this.OnClick(e);
            /* This event method is subscribed to the tag's click event (remember that the tag represents 
             most of the user control), so when the tag is clicked, the click event of the user control will be executed.
             This scenario is the same as the default OnValueChanged event created. */
        }
        private void RJDatePicker_MouseEnter(object sender, EventArgs e) // Attach the other events in the same way.
        {
            this.OnMouseEnter(e);
        }
        private void RJDatePicker_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }
        private void RJDatePicker_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }
        private void RJDatePicker_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }
        // You can keep adding the events you need
        #endregion

        #region -> Overridden methods
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Load event
            dateText.Text = datePicker.Text; // Update the text of the user control.

            this.Visible = false; // Hide the control while applying the appearance settings, this prevents flickering when displaying the form.
            ApplyAppearanceSettings(); // Apply UI appearance settings
            SetDatePickerLocation(); // Update the location of datePicker
            this.Visible = true;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetDatePickerLocation(); // Update the location of datePicker
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graph = e.Graphics;

            // GLASS style
            if (style == ControlStyle.Glass)
            {
                // Apply rounded corners (If applicable) to the region + anti-aliasing + border drawing of the user control.
                Utils.RoundedControl.RegionAndBorder(this, borderRadius, graph, borderColor, borderSize);
                if (borderRadius > 1) // If the radius is greater than 1, apply rounded corners to the label.
                {
                    Utils.RoundedControl.RegionOnly(dateText, borderRadius - borderSize);
                    if (isDroppedDown == true) // If the drop-down list is open, draw a fill in the button area to remove visible surface from the background, you can comment the code to understand what I mean.
                    {
                        using (SolidBrush brush = new SolidBrush(borderColor))
                        {
                            graph.SmoothingMode = SmoothingMode.AntiAlias;
                            var rect = new Rectangle(btnIcon.Left + 5, btnIcon.Top + 0, btnIcon.Width, this.Height);
                            GraphicsPath path = Utils.RoundedControl.GetRoundedGPath(rect, borderRadius - borderSize);
                            graph.FillPath(new SolidBrush(borderColor), path);
                        }
                    }
                }
            }
            // SOLID style
            else
            {
                // Just apply rounded corners (If that's the case) to the region + smoothing of the user control.
                Utils.RoundedControl.RegionAndSmoothed(this, borderRadius, graph);
                if (borderRadius > 1) // If the radius is greater than 1, apply rounded corners to the label.
                    Utils.RoundedControl.RegionOnly(dateText, borderRadius - borderSize);
            }
        }

        #endregion

    }
}