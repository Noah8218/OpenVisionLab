using FontAwesome.Sharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using RJCodeUI_M1.Settings;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace RJCodeUI_M1.RJControls
{
    [DefaultEvent("OnSelectedIndexChanged")] // Default event when double-clicking the control in the designer.
    public class RJComboBox : UserControl
    {
        /// <summary>
        /// This class inherits the <see cref = "UserControl" /> class.
        /// This control exposes most of the most used essential properties and events of a normal combo box, 
        /// you can add the functionality that you probably need and are missing here.
        /// On the other hand, you can change the style to solid or glass, also customize the drop down menu
        /// arrow icon and border color.
        /// Tutorial guide: https://youtu.be/5V6ZD2mAUw8 (Custom ComboBox - UserControl)
        /// </summary>

        #region -> Fields
        // Fields
        private ControlStyle style; // Style (solid or glass)
        private Color backgroundColor; // Gets or sets the background color.
        private Color iconColor; // Gets or sets the icon color.
        private Color borderColor; // Gets or sets the border color.
        private int borderSize = 1; // Gets or sets the border size.
        private int borderRadius = 0; // Gets or sets the border radius.
        private bool customizable; // Gets or sets if the control is customizable.
        private Color dropDownSelectedBackColor = Color.Empty;
        private Color dropDownSelectedTextColor = Color.White;
        // Controls
        private ComboBox comboList; // Gets or sets the Combo Box (not visible, but can show dropdown)
        private IconButton btnIcon; // Gets or sets the Dropdown Arrow Icon (button to show dropdown list)
        private Label label; // Gets or sets the Label (to display the text of the combo box)

        /// <Note>: ICON BUTTON is provided by the library <see cref = "FontAwesome.Sharp" />
        /// Author: mkoertgen
        /// GitHub: https://github.com/awesome-inc/FontAwesome.Sharp
        /// Nuget Package: https://www.nuget.org/packages/FontAwesome.Sharp </Note>
        #endregion

        #region -> Events

        [Category("RJ Code Advance")]
        public event EventHandler OnSelectedIndexChanged; // Main event of the combo box

        [Category("RJ Code Advance")]
        public event EventHandler DropDownOpening;

        #endregion

        #region -> Constructor

        public RJComboBox()
        {
            comboList = new ComboBox();
            btnIcon = new IconButton();
            label = new Label();

            this.SuspendLayout();
            //
            // ComboBox: Combined list
            //
            comboList.Font = new Font("Microsoft Sans Serif", 9.5F);
            comboList.FormattingEnabled = true;
            comboList.Size = new Size(170, 21);
            comboList.Visible = false;
            comboList.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged); // Subscribe the SelectedIndexChanged event of the control and attach it to the previously defined OnSelectedIndexChanged default event (see method definition).
            comboList.DropDownClosed += new EventHandler(ComboBox_DropDownClosed);
            comboList.TextChanged += new EventHandler(ComboBox_TextChanged);
            comboList.DrawMode = DrawMode.OwnerDrawFixed;
            comboList.ItemHeight = 24;
            comboList.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
            comboList.FlatStyle = FlatStyle.Flat;
            //
            // IconButton: Dropdown Arrow Icon
            //
            btnIcon.FlatStyle = FlatStyle.Flat;
            btnIcon.FlatAppearance.BorderSize = 0;
            btnIcon.Size = new Size(30, 30);
            btnIcon.Dock = DockStyle.Right;
            btnIcon.BackColor = Color.WhiteSmoke;
            btnIcon.IconChar = IconChar.AngleDown; // Set dropdown arrow icon
            btnIcon.IconColor = Color.MediumSlateBlue;
            btnIcon.IconSize = 22;
            btnIcon.Location = new Point(169, 1);
            btnIcon.Cursor = Cursors.Hand;
            btnIcon.Click += new EventHandler(BtnIcon_Click); // Subscribe to event BtnIcon.Click (To Show dropdown combo box)
            //
            // Label: ComboBox Label (Represents the visual part of the ComboBox, displays the text, and populates the user control.)
            //
            label.BackColor = Color.WhiteSmoke;
            label.Dock = DockStyle.Fill; // Set padding
            label.Location = new Point(1, 1);
            label.Padding = new Padding(8, 0, 0, 0);
            label.Size = new Size(168, 30);
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Text = "";
            label.Controls.Add(btnIcon); // Add the icon button to the label.
            label.Font = new Font(UIAppearance.TextFamilyName, UIAppearance.TextSize); // Font

            /* Since the label represents the visual part of the ComboBox and occupies most of the user control,
            It is necessary to attach the events that occur in the label (label) to the events of the user control (RJComboBox),
            That is, for example in the Click event, when the label is clicked, the click event of the user control (RJComboBox) must also be executed.
            See the definition of the event methods to understand better. */
            label.Click += new EventHandler(RJComboBox_Click);
            label.KeyDown += new KeyEventHandler(RJComboBox_KeyDown);
            label.KeyPress += new KeyPressEventHandler(RJComboBox_KeyPress);
            label.KeyUp += new KeyEventHandler(RJComboBox_KeyUp);
            label.MouseEnter += new EventHandler(RJComboBox_MouseEnter);
            // You can attach the events you want, if it doesn't exist, you can create it was done with the OnSelectedIndexChanged event.

            //
            // User control: RJComboBox
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.MediumSlateBlue;
            this.Padding = new Padding(1);
            this.Size = new Size(200, 32);
            BorderSize = borderSize; // Set border size to apply the necessary adjustments.
            // Add the controls
            this.Controls.Add(label); // Fill in the remaining space (display the text of the combo box)           
            this.Controls.Add(comboList); // It's in the background, behind the label (not visible, but shows the drop-down list).
            // This order is important, the last controls are added first (from bottom to top).
            comboList.SendToBack();
            label.BringToFront();
            this.ResumeLayout(false);
            SetComboComponentLocation();
        }
        #endregion

        #region -> Properties

        #region - Appearance Properties

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the style (glass or solid)")]
        public ControlStyle Style
        {
            get { return style; }
            set
            {
                style = value; // Set the value
                // Update appearance settings-> preview changes in design mode.
                if (this.DesignMode) ApplyAppearanceSettings();
            }
        }
        [Category("RJ Code - Appearance")]
        [Description("Gets or sets whether the control is customizable")]
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
                label.BackColor = backgroundColor;
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
                    SetComboComponentLocation(); // Update the location of the combobox.
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
                    SetComboComponentLocation(); // Update the location of the combobox.
                    this.Invalidate(); // Draw the border and radius (Redraw the control)
                }
            }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets icon color")]
        public Color IconColor
        {
            get { return btnIcon.IconColor; }
            set
            {
                iconColor = value;
                btnIcon.IconColor = iconColor;
            }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the dropdown style")]
        public ComboBoxStyle DropDownStyle
        {
            get { return comboList.DropDownStyle; }
            set
            {
                comboList.DropDownStyle = value;
                if (comboList.DropDownStyle == ComboBoxStyle.Simple)
                    btnIcon.Visible = false;
                else
                    btnIcon.Visible = true;
            }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the background color of the drop-down list")]
        public Color DropDownBackColor
        {
            get { return comboList.BackColor; }
            set { comboList.BackColor = value; }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the text color of the drop-down list")]
        public Color DropDownTextColor
        {
            get { return comboList.ForeColor; }
            set { comboList.ForeColor = value; }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the selected row background color of the drop-down list")]
        public Color DropDownSelectedBackColor
        {
            get { return dropDownSelectedBackColor; }
            set { dropDownSelectedBackColor = value; }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the selected row text color of the drop-down list")]
        public Color DropDownSelectedTextColor
        {
            get { return dropDownSelectedTextColor; }
            set { dropDownSelectedTextColor = value; }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the drop-down list item height")]
        public int DropDownItemHeight
        {
            get { return comboList.ItemHeight; }
            set
            {
                if (value >= 18)
                {
                    comboList.ItemHeight = value;
                }
            }
        }


        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the text color of the combo")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                label.ForeColor = value;
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
                label.Font = value;
            }
        }

        [Category("RJ Code - Appearance")]
        [Description("Gets or sets the text")]
        public string Texts
        {
            get { return label.Text; }
            set
            {
                label.Text = value;
            }
        }
        #endregion

        #region - Functional properties

        [Category("RJ Code - Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        [Description("Gets an object that represents the collection of elements contained in this ComboBox")]
        public ComboBox.ObjectCollection Items
        {
            get { return comboList.Items; }
            /*
              This property exposes all the functionality of the ITEMS property of the normal combobox (ComboBox.ObjectCollection),
              how to add a collection of strings from the designer, using the Methods Items.Add, Items.AddRange, Items.Remove,
              Items.Count, etc.
              In short, this property allows you to get a reference to the list of items that are currently stored in the ComboBox. 
              With this reference, you can add items, remove items, and get a count of the items in the collection.
              More information:
             https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.items?view=net-5.0#System_Windows_Forms_ComboBox_Items
             */
        }

        [Category("RJ Code - Data")]
        [AttributeProvider(typeof(IListSource))]//Fuente de lista
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Gets or sets the data source for this ComboBox.")]
        public object DataSource
        {
            get { return comboList.DataSource; }
            set { comboList.DataSource = value; }
        }

        [Category("RJ Code - Data")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design")]
        [Description("Gets or sets the property to display for this ListControl. (Inherited from ListControl)")]
        public string DisplayMember
        {
            get { return comboList.DisplayMember; }
            set { comboList.DisplayMember = value; }
        }

        [Category("RJ Code - Data")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
        [Description("Gets or sets the property path to be used as the actual value for items in ListControl. (Inherited from ListControl)")]
        public string ValueMember
        {
            get { return comboList.ValueMember; }
            set { comboList.ValueMember = value; }
        }

        [Browsable(false)] // Property not visible in property box
        [Description("Gets or sets the index that the currently selected item specifies.")]
        public int SelectedIndex
        {
            get { return comboList.SelectedIndex; }
            set
            {
                if (value >= 0)
                {
                    comboList.SelectedIndex = value;
                }
            }
        }
        [Browsable(false)] // Property not visible in property box
        public object SelectedValue
        {
            get { return this.comboList.SelectedValue; }
        }


        [Browsable(false)] // Property not visible in property box
        public object SelectedItem
        {
            get { return this.comboList.SelectedItem; }
        }

        // Combo Box Auto complete
        [Browsable(true)]
        [Category("RJ Code - Data AC")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return comboList.AutoCompleteCustomSource; }
            set { comboList.AutoCompleteCustomSource = value; }
        }

        [Browsable(true)]
        [Category("RJ Code - Data AC")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get { return comboList.AutoCompleteMode; }
            set { comboList.AutoCompleteMode = value; }
        }

        [Browsable(true)]
        [Category("RJ Code - Data AC")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteSource AutoCompleteSource
        {
            get { return comboList.AutoCompleteSource; }
            set { comboList.AutoCompleteSource = value; }
        }
        // You can add more properties of the normal combo box if you need
        #endregion

        #region - Overridden properties
        public new string Text
        {// Override the text property of the user control and set or get the text for the combo box label.
            get { return label.Text; }
            set
            {
                label.Text = value;
            }
        }
        #endregion

        #endregion

        #region -> Private methods

        private void ApplyAppearanceSettings()
        {// Apply appearance settings

            if (customizable == false)
            {
                comboList.ForeColor = UIAppearance.TextColor;
                comboList.BackColor = UIAppearance.ItemBackgroundColor;

                switch (style)
                {
                    case ControlStyle.Solid: // Solid style
                        BorderColor = UIAppearance.StyleColor;
                        BorderSize = 0;
                        label.ForeColor = Color.White;
                        btnIcon.IconColor = Color.White;
                        this.BackColor = UIAppearance.StyleColor;
                        break;

                    case ControlStyle.Glass: // Glass style

                        btnIcon.IconColor = UIAppearance.StyleColor;
                        label.ForeColor = UIAppearance.PrimaryTextColor;
                        this.BackColor = UIAppearance.BackgroundColor;

                        if (UIAppearance.Theme == UITheme.Dark)
                            BorderColor = Utils.ColorEditor.Darken(UIAppearance.TextColor, 10);
                        else BorderColor = UIAppearance.TextColor;
                        break;
                }
            }
        }
        private void SetComboComponentLocation()
        {
            //Update the size and location of the ComboBox
            comboList.Width = Math.Max(1, label.Width);
            comboList.DropDownWidth = comboList.Width;
            comboList.Location = new Point(label.Left, label.Bottom - comboList.Height);
            comboList.SendToBack();
            label.BringToFront();
        }

        private void ShowDropDownList()
        {
            DropDownOpening?.Invoke(this, EventArgs.Empty);
            SetComboComponentLocation();
            comboList.Visible = true;
            comboList.BringToFront();
            comboList.Select();
            comboList.DroppedDown = true;
        }
        #endregion

        #region -> Public methods

        public void Clear()
        {// Clear control
            this.label.Text = "";
            this.comboList.Items.Clear();
        }
        #endregion

        #region -> Event methods

        //-> Default event
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {// Attach the default OnSelectedIndexChanged event of the user control (RJComboBox)
            // to the SelectedIndexChanged event of the ComboBox (comboList).
            if (OnSelectedIndexChanged != null)
                OnSelectedIndexChanged.Invoke(comboList, e);
            // When an item is selected or the dropdown text changes, update the label text.
            label.Text = comboList.Text;
        }

        //-> Components actions
        private void RJComboBox_Click(object sender, EventArgs e)
        {// Attach the click event of the user control (RJComboBox) to this click event of the tag (dateText).
            this.OnClick(e);
            /* This event method is subscribed to the tag's click event (remember that the tag represents
             most of the user control), so when the tag is clicked,
             the click event of the user control will be executed.
             This scenario is the same as the default OnSelectedIndexChanged event created. */
            if (DropDownStyle == ComboBoxStyle.DropDownList) // When the style is DropDownList, the drop-down list must be opened when clicking anywhere in the control (In the same way as in a traditional ComboBox).
                ShowDropDownList(); // Set the DroppedDown property to true to display the dropdown list.
        }
        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            label.Text = comboList.Text;
            // When an item is selected or the dropdown text changes, update the label text.
        }

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Bounds.Width <= 0 || e.Bounds.Height <= 0)
            {
                return;
            }

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool disabled = (e.State & DrawItemState.Disabled) == DrawItemState.Disabled;
            Color backColor = selected ? ResolveDropDownSelectedBackColor() : comboList.BackColor;
            Color textColor = disabled
                ? SystemColors.GrayText
                : selected ? dropDownSelectedTextColor : comboList.ForeColor;

            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            string text = e.Index >= 0 && e.Index < comboList.Items.Count
                ? comboList.GetItemText(comboList.Items[e.Index])
                : comboList.Text;

            Rectangle textBounds = new Rectangle(
                e.Bounds.Left + 9,
                e.Bounds.Top,
                Math.Max(0, e.Bounds.Width - 18),
                e.Bounds.Height);

            TextRenderer.DrawText(
                e.Graphics,
                text,
                comboList.Font,
                textBounds,
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                using (Pen focusPen = new Pen(Utils.ColorEditor.Darken(backColor, selected ? (ushort)12 : (ushort)20)))
                {
                    Rectangle focusBounds = Rectangle.Inflate(e.Bounds, -1, -1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }
        }

        private Color ResolveDropDownSelectedBackColor()
        {
            if (!dropDownSelectedBackColor.IsEmpty)
            {
                return dropDownSelectedBackColor;
            }

            if (!borderColor.IsEmpty)
            {
                return borderColor;
            }

            if (!iconColor.IsEmpty)
            {
                return iconColor;
            }

            return Color.FromArgb(47, 111, 171);
        }
        private void BtnIcon_Click(object sender, EventArgs e)
        {// When the drop-down arrow icon button is clicked, display the drop-down combo box.

            ShowDropDownList(); // Set the DroppedDown property to true to display the dropdown list.
            if (customizable) // If it is customizable
            {
                switch (style) // Apply a highlight to the drop-down arrow icon button.
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
            else // If not customizable
            {
                switch (style) // Apply a highlight to the drop-down arrow icon button.
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
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {// It happens when the drop-down list is closed

            if (customizable)
            {
                // Disable the highlighting of the drop-down icon button.
                btnIcon.BackColor = backgroundColor;
                btnIcon.IconColor = iconColor;
                BorderColor = borderColor;
            }
            else
            {
                ApplyAppearanceSettings(); // Refresh the appearance to disable the highlighting of the flyout icon button.
            }

            comboList.Visible = false;
            comboList.SendToBack();
            label.BringToFront();
        }

        //-> Attach events: Label -> UserControl
        private void RJComboBox_MouseEnter(object sender, EventArgs e) // Attach the other necessary events in the same way.
        {
            this.OnMouseEnter(e);
        }
        private void RJComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }
        private void RJComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }
        private void RJComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }
        // You can keep attaching the events you need
        #endregion

        #region -> Overridden methods

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetComboComponentLocation();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Load event
            this.Visible = false; // Hide the control while applying the appearance settings, this prevents flickering when displaying the form.
            ApplyAppearanceSettings(); // Apply UI appearance settings
            this.Visible = true;
            SetComboComponentLocation();
            comboList.Visible = false;
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
                    Utils.RoundedControl.RegionOnly(label, borderRadius - borderSize);
                    if (comboList.DroppedDown == true) // If the drop-down list is open, draw a fill in the button area to remove visible surface from the background, you can comment the code to understand what I mean.
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
                    Utils.RoundedControl.RegionOnly(label, borderRadius - borderSize);
            }
        }
        #endregion

    }
}
