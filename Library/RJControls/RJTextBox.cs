using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;

namespace RJCodeUI_M1.RJControls
{
    [DefaultEvent("onTextChanged")]//Default event when double click on control in designer
    public class RJTextBox : UserControl
    {
        /// <summary>
        /// This class inherits from the <see cref="UserControl"/> class
        /// This custom text box allows you to change 2 main styles, with a 
        /// border around it or single border at the bottom (underlined).
        /// It also allows adding a watermark (Place Holder) and setting if
        /// it is a password field. In addition to customizing the border color 
        /// and size, border radius, background color, text color, and placeholder text color, 
        /// among others.
        /// Tutorial guide (Part 1): https://youtu.be/CkpUQYzYCC8
        /// Tutorial guide(Part 2): https://youtu.be/3wP6QqjiCCo
        /// </summary>
        /// 

        #region -> Fields
        // Fields
        private TextBoxStyle style; // Gets or sets the style of the text box (MatteBorder, FlaringBorder, MatteLine or FlaringLine)
        private Color placeHolderColor; // Gets or sets the color of the placeholder text
        private Color borderColor; // Gets or sets the border color.
        private Color borderFocusColor = Color.Black; // Gets or sets the border color in focused state
        private int borderSize = 2; // Gets or sets the width of the border.
        private int borderRadius = 0; // Gets or sets the border radius.
        private int maxLength = 32767; // Gets or sets the maximum number of characters in the text box.
        private string placeHolderText; // Gets or sets the placeholder text.
        private bool isPlaceHolder; // Gets or sets whether the text box is in placeholder state.
        private bool isPasswordChar; // Gets or sets whether it is a password character or a password field.
        private bool customizable; // Gets or sets whether the control's colors are customizable or assigned based on appearance settings
        private static int padFactor = 7; // Set the value for the padding (Padding)
        // Controls
        private TextBox textBox; // Set or get the text box
        #endregion

        #region -> Events

        [Category("RJ Code Advance")]
        public event EventHandler onTextChanged; // Default event of RJTextBox
        #endregion

        #region -> Constructor
        public RJTextBox()
        {
            this.DoubleBuffered = true;
            // Initialize objects
            textBox = new TextBox();
            this.SuspendLayout();
            //
            // TextBox: (Occupies most of the RJTextBox user control)
            //
            textBox.Location = new Point(2, 3);
            textBox.Dock = DockStyle.Fill; // FILL the backBox control
            textBox.BackColor = UIAppearance.BackgroundColor;
            textBox.BorderStyle = BorderStyle.None; // Remove border
            textBox.Size = new System.Drawing.Size(230, 16);
            textBox.Enter += new EventHandler(TextBox_Enter); // Subscribe to the Enter event of the TextBox to highlight and remove the placeholder if it is the case.
            textBox.Leave += new EventHandler(TextBox_Leave); // subscribe to the Leave event of the TextBox to remove the highlight and reset the placeholder if it is the case.

            textBox.TextChanged += new EventHandler(TextBox_TextChanged); // Subscribe to the control's TextChanged event and attach it to the previously defined OnTextChanged default event (see method definition).

            /* The textBox control takes up most of the user control, therefore
                It is necessary to attach the events that occur in the text box (textBox) to the events of the user control (RJTextBox),
                That is, for example, in the Click event, when the text box is clicked, the click event of the user control (RJTextBox) must also be executed.
                See the definition of the event methods to understand better. */
            textBox.Click += new EventHandler(RJTextBox_Click);
            textBox.KeyDown += new KeyEventHandler(RJTextBox_KeyDown);
            textBox.KeyPress += new KeyPressEventHandler(RJTextBox_KeyPress);
            textBox.KeyUp += new KeyEventHandler(RJTextBox_KeyUp);
            textBox.MouseEnter += new EventHandler(RJTextBox_MouseEnter);
            // You can attach whatever events you want, if it doesn't exist, you can create it like you did with the onTextChanged event

            //
            // UserControl: RJTextBox
            //
            this.Controls.Add(textBox); // Add the textBox control

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = UIAppearance.StyleColor;
            this.Padding = new Padding(10, padFactor, 10, padFactor);
            this.Size = new System.Drawing.Size(250, 32);
            this.Font = new Font(UIAppearance.TextFamilyName, UIAppearance.TextSize);
            ForeColor = UIAppearance.TextColor;
            this.ResumeLayout(false);
        }

        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        [Description("Gets or sets the style of the text")]
        public TextBoxStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                if (this.DesignMode) // Preview changes in design mode
                {
                    ApplyAppearanceSettings();
                    this.Invalidate();
                }
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets whether the colors of the control are customizable or assigned based on appearance settings")]
        public bool _Customizable
        {
            get { return customizable; }
            set { customizable = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the background color")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                this.textBox.BackColor = value;
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the border color")]
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
        [Description("Gets or sets border color in focused state")]
        public Color BorderFocusColor
        {
            get { return borderFocusColor; }
            set { borderFocusColor = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the width of the border")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                if (value > 0 || value < 5)
                {
                    borderSize = value;
                    this.Invalidate();
                }
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the radius of the corners of the control")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                if (value >= 0)
                {
                    borderRadius = value;
                    this.Invalidate();
                }
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the color of the text")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                textBox.ForeColor = value;
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets whether it is a password character or a password field")]
        public bool PasswordChar
        {
            get { return isPasswordChar; }
            set
            {
                isPasswordChar = value;
                if (!isPlaceHolder)
                    textBox.UseSystemPasswordChar = value;
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets if multiline")]
        public bool MultiLine
        {
            get { return this.textBox.Multiline; }
            set { this.textBox.Multiline = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the color of the placeholder text")]
        public Color PlaceHolderColor
        {
            get { return placeHolderColor; }
            set
            {
                placeHolderColor = value;
                if (isPlaceHolder)
                    textBox.ForeColor = value;
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the placeholder text")]
        public string PlaceHolderText
        {
            get { return placeHolderText; }
            set
            {
                placeHolderText = value; // Set the value to the placeholder field.
                textBox.Text = ""; // Set Blank Text in the text box      
                SetPlaceholder(); // Set the placeholder to the text box.
            }
        }

        [Category("RJ Code Advance")]
        [DefaultValue(32767)]
        [Description("Gets or sets the maximum number of characters in the text box.")]
        public int MaxLength
        {
            get { return maxLength; }
            set
            {
                maxLength = value;
                textBox.MaxLength = maxLength;
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the font")]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                textBox.Font = value;
                if (this.DesignMode)
                    UpdateControlHeight(); // Set a suitable height of the user control and textBox.
            }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the text in the text box")]
        public new string Text
        {
            get
            {
                if (isPlaceHolder) return ""; // If the textbox is a placeholder, Return an empty string
                else return textBox.Text; // Otherwise return the text
            }
            set
            {
                RemovePlaceHolder(); //Remove placeholder if it is the case.
                textBox.Text = value; // Set value.
                SetPlaceholder(); // Set placeholder if it is the case.
            }
        }

        [Localizable(true)]
        [Category("RJ Code Advance")]
        [Description("Gets or sets scroll bars - should only be used when multiline")]
        public ScrollBars ScrollBars
        {
            get { return textBox.ScrollBars; }
            set { textBox.ScrollBars = value; }
        }

        [Browsable(false)]
        [Description("Override the Padding property to set a unique value")]
        public new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = new Padding(10, padFactor, 10, padFactor); }
        }

        #endregion

        #region -> Private methods

        private void SetPlaceholder()
        {// Set placeholder in text box whenever text box is empty or null.

            if (string.IsNullOrWhiteSpace(textBox.Text) && string.IsNullOrWhiteSpace(placeHolderText) == false)
            {
                isPlaceHolder = true; // Set the text box to the placeholder state.
                textBox.Text = placeHolderText; // Set the placeholder text as text in the text box.
                textBox.ForeColor = placeHolderColor; // Set the color of the placeholder text.
                if (isPasswordChar) // If it is a password field, remove the password character to display the placeholder text.
                    textBox.UseSystemPasswordChar = false;
            }
            else
            {
                isPlaceHolder = false;
                textBox.ForeColor = this.ForeColor;
            }
        }
        private void RemovePlaceHolder()
        {// This method takes care of removing the placeholder.
            if (isPlaceHolder && string.IsNullOrWhiteSpace(placeHolderText) == false)
            {
                isPlaceHolder = false; // remove the placeholder state from the text box
                textBox.Text = ""; // Empty text
                if (customizable) // If customizable, set the specified text color
                    textBox.ForeColor = this.ForeColor;
                else textBox.ForeColor = UIAppearance.PrimaryTextColor; // Otherwise, set the text color specified in appearance settings

                if (isPasswordChar) // If it is a password field, reset the password character to mask the characters.
                    textBox.UseSystemPasswordChar = true;
            }
        }

        private void UpdateControlHeight()
        {// This method is in charge of setting a suitable height for the user control (RJTextBox) and text box (textBox),
            // This to fully display the text and have a considerable stop in the user control.
            if (MultiLine == false) // Only update the height when the text box is not multiline.
            {
                int txtHeight = TextRenderer.MeasureText("Text", this.Font).Height + 1; // Get the height of the text and add 1.
                textBox.Multiline = true;
                textBox.MinimumSize = new Size(0, txtHeight); // Set the minimum height of the text box.
                textBox.Multiline = false;

                this.Height = textBox.Height + (padFactor * 2); // Set the height of the user control.
            }
        }

        private void ValidateControlRadius()
        {// This method validates if the radius has a maximum allowed value. The maximum radius is half the height of the user control.
            int maxRadius = this.Height / 2;
            if (borderRadius > maxRadius)
                borderRadius = maxRadius;
        }

        private void ApplyAppearanceSettings()
        {
            if (customizable == false)
            {

                BackColor = UIAppearance.BackgroundColor;//Set back color
                this.ForeColor = UIAppearance.PrimaryTextColor;//set text color
                BorderFocusColor = Utils.ColorEditor.Lighten(UIAppearance.StyleColor, 15);//set focus border color
                //Set placeholder color
                if (UIAppearance.Theme == UITheme.Dark)
                    PlaceHolderColor = Utils.ColorEditor.Darken(UIAppearance.TextColor, 5);
                else PlaceHolderColor = Color.DarkGray;

                //Set Border Color
                if (style == TextBoxStyle.MatteBorder || style == TextBoxStyle.MatteLine)
                {
                    if (UIAppearance.Theme == UITheme.Dark)
                        BorderColor = Utils.ColorEditor.Darken(UIAppearance.TextColor, 10);
                    else BorderColor = UIAppearance.TextColor;
                }
                else
                {
                    BorderColor = UIAppearance.StyleColor;
                }
            }
        }
        #endregion

        #region -> Public methods

        public void Clear()
        {
            Text = string.Empty;
        }
        #endregion

        #region -> Event methods

        //-> Default event
        private void TextBox_TextChanged(object sender, EventArgs e)
        {// Attach the default onTextChanged event of the user control (RJTextBox)
            // to the TextChanged event of the text box (textBox).
            if (onTextChanged != null)
                onTextChanged.Invoke(sender, e);

        }

        //-> Components actions
        private void TextBox_Enter(object sender, EventArgs e)
        {
            // when the cursor enters the textbox, remove the placeholder if it is the case
            // and highlight the border 
            this.Invalidate();
            RemovePlaceHolder();
        }
        private void TextBox_Leave(object sender, EventArgs e)
        {
            // when the cursor leaves the text box, reset the placeholder if it is the case and reset the border color
            this.Invalidate();
            SetPlaceholder();
        }

        //-> Attach events: TextBox -> UserControl
        private void RJTextBox_Click(object sender, EventArgs e)
        {// Attach the click event of the user control (RJTextBox) to this click event of the text box (textBox).
            this.OnClick(e);
            /* This event method is subscribed to the textbox click event, remember it represents most of the user control, so when the textbox is clicked,
       the click event of the user control will be executed.
       This scenario is the same as the default onTextChanged event created. */
        }
        private void RJTextBox_MouseEnter(object sender, EventArgs e) // Attach the other events in the same way
        {
            this.OnMouseEnter(e);
        }
        private void RJTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }
        private void RJTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }
        private void RJTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }
        // You can keep adding the events you need
        #endregion

        #region -> Overridden methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graph = e.Graphics;
            Color currentBorderColor;
            // Set the current border color, that is, border color focused or not.
            if (textBox.Focused)
                currentBorderColor = borderFocusColor;
            else currentBorderColor = borderColor;

            // Draw rectangular border in MatteBorder or FlaringBorder style
            if (style == TextBoxStyle.MatteBorder || style == TextBoxStyle.FlaringBorder)
            {
                // -> Apply rounded corners to user control-> REGION + SMOOTHING + EDGE
                Utils.RoundedControl.RegionAndBorder(this, borderRadius, graph, currentBorderColor, borderSize);

                // -> Apply rounded corners to text box-> REGION ONLY
                if (borderRadius > 15) // The border radius must be greater than 15, because if it is less it is not necessary to apply a rounded region.
                {
                    if (!MultiLine) // If it is not multiline, the border radius is equal to twice the border size
                        Utils.RoundedControl.RegionOnly(textBox, borderSize * 2);
                    else // Otherwise, the edge radius is at the set value minus
                        Utils.RoundedControl.RegionOnly(textBox, borderRadius - (borderSize * 2));
                }
            }
            // Draw underlined border in MatteLine or FlaringLine style
            else
            {
                using (Pen penBorder = new Pen(currentBorderColor, borderSize))
                {
                    // -> Apply rounded corners to user control-> REGION + SMOOTHING
                    Utils.RoundedControl.RegionAndSmoothed(this, borderRadius, graph);
                    // Draw a line at the bottom without anti-aliasing.
                    graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;                  
                    graph.DrawLine(penBorder, 0, this.Height - 1F, this.Width, this.Height - 1F);
                }
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.DesignMode)
                UpdateControlHeight();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyAppearanceSettings();
            UpdateControlHeight();
            ValidateControlRadius();
        }
        #endregion

    }
}
