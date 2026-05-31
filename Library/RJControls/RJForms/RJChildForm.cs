using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Drawing;
using FontAwesome.Sharp;
using RJCodeUI_M1.Settings;
using RJCodeUI_M1.Utils;
using RJCodeUI_M1.RJControls;

namespace RJCodeUI_M1.RJForms
{
    public class RJChildForm : RJBaseForm
    {
        /// This class inherits from the <see cref="RJBaseForm"/> class
        /// 
        ///<summary>
        ///In This class, you set a default size of the form, remove the border of the form, 
        ///and add custom title bar and client area using panels, as well as add the buttons
        ///to maximize, minimize, close, and the Dropdown menu for the list of options 
        ///for the form. (Left-Right SnapWindow, Help).
        ///The default form size is equal to the Desktop Panel Size of the main form,
        ///you can change that by setting the _DesktopPanelSize property to false.
        ///</summary>

        #region -> Fields

        /// Fields      
        private IContainer components = null; //Container for components that are not child of the form. 
        ///Allows all added components to be removed with the Dispose method by the components container
        ///<see cref="protected override void Dispose(bool disposing)"/>
        private bool isChildForm; //Gets or sets if it is a child form
        private int markerPosition;//Gets or Sets Form menu button marker location
        private string helpMessage;//Gets or Sets Form help message to the user
        private IconChar formIcon;//Gets or Sets Form icon
        private bool disableFormOptions;//Disable or enable dropdown menu of windows Form Options
        private bool desktopPanelSize;//Gets or sets whether the size of the form is equal to the size of the desktop panel or is customizable from the size property of the designer properties box (The default is true)
        private Color supernovaColor = UIAppearance.Style == UIStyle.Supernova ? RJColors.GetSupernovaColor() : Color.CornflowerBlue;

        /// Controls  
        protected Panel pnlClientArea;
        private Panel pnlTitleBar;//Sets Form title bar
        private Label lblCaption;//Sets Form caption
        private RJDragControl dragControl;//Sets Form drag control (is component, the constructor accepts a parameter of type IContainer)
        private IconButton btnFormIcon;//Sets Form icon Button
        private RJDropdownMenu dmFormOptions;//Sets Dropdown menu of windows Form Options (is component, the constructor accepts a parameter of type IContainer)
        private IconMenuItem miHelp;//Sets Help MenuItem


        ///<Note>:ICON MENU ITEM, ICON BUTTON and ICON CHAR is provided by <see cref="FontAwesome.Sharp"/> library
        ///      Autor: mkoertgen
        ///      GitHub: https://github.com/awesome-inc/FontAwesome.Sharp
        ///      Nuget Package: https://www.nuget.org/packages/FontAwesome.Sharp </Note>
        #endregion

        #region -> Constructor

        /// Constructor
        public RJChildForm()
        {
            InitializeItems();
        }

        /// Initialize Component
        private void InitializeItems()
        {
            //Initialize the components for the form design: add the title bar, buttons for maximized, minimized,
            //form options dropdown menu and the client area of the form*/
            components = new System.ComponentModel.Container();//initialize container

            #region -Control Instantiation

            pnlClientArea = new Panel();
            pnlTitleBar = new Panel();
            lblCaption = new Label();
            dragControl = new RJDragControl(pnlTitleBar, this, components);//Drag control, add to component container
            dmFormOptions = new RJDropdownMenu(components);//Add to component container
            btnFormIcon = new FontAwesome.Sharp.IconButton();
            miHelp = new FontAwesome.Sharp.IconMenuItem();

            pnlTitleBar.SuspendLayout();
            #endregion

            #region -Form Title Bar
            //
            //  Panel: Form Title Bar 
            //           
            pnlTitleBar.Name = "pnlTitleBar";
            pnlTitleBar.Location = new Point(0, 0);
            pnlTitleBar.Dock = DockStyle.Top;
            pnlTitleBar.Size = new Size(960, 40);
            pnlTitleBar.Controls.Add(btnFormIcon);//Add controls 
            pnlTitleBar.Controls.Add(lblCaption);
            pnlTitleBar.Controls.Add(this.btnMinimize);
            pnlTitleBar.Controls.Add(this.btnMaximize);
            pnlTitleBar.Controls.Add(this.btnClose);
            // 
            // Icon Button: Form Icon (FontAwesome.Sharp library)
            //            
            btnFormIcon.Name = "btnIcon";
            btnFormIcon.Cursor = Cursors.Hand;
            btnFormIcon.FlatStyle = FlatStyle.Flat;
            btnFormIcon.FlatAppearance.BorderSize = 0;
            btnFormIcon.Flip = FontAwesome.Sharp.FlipOrientation.Normal;
            btnFormIcon.IconChar = FontAwesome.Sharp.IconChar.Folder;
            btnFormIcon.IconColor = Color.WhiteSmoke;
            btnFormIcon.IconSize = 25;
            btnFormIcon.Rotation = 0D;
            btnFormIcon.Location = new Point(0, 0);
            btnFormIcon.Size = new Size(40, 40);
            btnFormIcon.UseVisualStyleBackColor = false;//Events            
            btnFormIcon.MouseEnter += new System.EventHandler(FormIcon_MouseEnter);
            btnFormIcon.MouseLeave += new System.EventHandler(FormIcon_MouseLeave);
            btnFormIcon.Click += new System.EventHandler(FormIcon_Click);
            FormIcon = IconChar.Folder;
            // 
            // Label: Form Caption
            // 
            lblCaption.Name = "lblCaption";
            lblCaption.AutoSize = true;
            lblCaption.Font = new Font("Montserrat", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            lblCaption.ForeColor = Color.WhiteSmoke;
            lblCaption.Location = new Point(40, 10);
            //
            // Button: Control box buttons
            // 
            this.btnClose.Dock = DockStyle.Right;
            this.btnMaximize.Dock = DockStyle.Right;
            this.btnMinimize.Dock = DockStyle.Right;

            #endregion

            #region -Form Options
            // 
            // Icon MenuItem: Help (FontAwesome.Sharp library)
            // 
            miHelp.Name = "miHelp";
            miHelp.Text = "Help";
            miHelp.IconSize = 21;
            miHelp.IconChar = IconChar.Question;
            miHelp.IconColor = RJColors.FantasyColorScheme4;
            miHelp.Click += new System.EventHandler(HelpMessage_Click);
            //        
            //  DropdownMenu: Form Options
            //
            dmFormOptions.Name = "dmFormOptions";
            dmFormOptions.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            //dmFormOptions.Items.AddRange(new ToolStripItem[] {//Add menu items
            //miSnapLeft,//Snap Window Left
            //miSnapRight,//Snap Window Right
            //miExitSnap,//Exit Snap Window
            //miHelp});
            dmFormOptions.Items.AddRange(new ToolStripItem[] {//Add menu items
            miSnapLeft,//Snap Window Left
            miSnapRight,//Snap Window Right
            miExitSnap//Exit Snap Window
            });

            dmFormOptions.OwnerIsMenuButton = false;
            dmFormOptions.VisibleChanged += new EventHandler(FormOptions_VisibleChanged);

            #endregion

            #region -Client Area

            // Panel: Client Area (Form Body)                      
            pnlClientArea.Dock = DockStyle.Fill;
            pnlClientArea.Location = new Point(0, 40);
            pnlClientArea.Name = "pnlClientArea";
            pnlClientArea.Size = new Size(960, 485);
            pnlClientArea.AutoScroll = true;
            #endregion

            #region -RJ Child Form Properties
            //
            // RJChildForm          
            //
            this.Name = "RJChildForm";
            this.Text = "RJ Child form";
            this.Controls.Add(pnlClientArea);
            this.Controls.Add(pnlTitleBar);
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.None;//Disable autoscale mode, to keep the form size set in the DefaultSize property
            this.FormBorderStyle = FormBorderStyle.None;//Borderless form    
            this.MinimumSize = new Size(400, 180);//Minimun form size 
            this.DoubleBuffered = true;
            this.Resize += new System.EventHandler(Form_Resize);
            this.Deactivate += new EventHandler(Form_Deactivated);//subscribe Deactivate event to change / opaque title bar color
            this.Activated += new EventHandler(Form_Activated);//Subscribe Activated event to retrieve title bar color   
            desktopPanelSize = true;//Set default value 
            pnlTitleBar.PerformLayout();
            pnlTitleBar.ResumeLayout();
            #endregion

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
        }

        #endregion

        #region -> Properties

        //Values
        [Browsable(false)]
        public bool IsChildForm
        { //Gets or sets if it is a child form
            get { return isChildForm; }
            set
            {
                isChildForm = value;
                if (isChildForm == true) // If the form is a child form, set as a non-resizable form.
                    this.Resizable = false;
                else // Otherwise, set as resizable form.
                    this.Resizable = true;
                ApplyApperanceSettings();//Reapply the settings when this property changes
            }
        }

        [Category("RJ Code Advance")]
        public bool _DesktopPanelSize
        {
            //Gets or sets whether the size of the form is equal to the size of the main form desktop panel 
            //or is customizable from the size property of the designer properties box
            get { return desktopPanelSize; }
            set
            {
                desktopPanelSize = value;//Set value
                if (value == true)//If the value is true set the desktop panel size as the form size + title bar height
                {
                    this.Size = new Size(960, 560);
                }
            }
            /*Note> Do not change the name, at least the script(_) below. The properties are run in 
             alphabetical order, so this property must run before the ClientSize property.*/
        }

        [Category("RJ Code Advance")]
        public bool DisableFormOptions
        {//Disable dropdown menu of Form Options
            get
            {
                return disableFormOptions;
            }

            set
            {
                disableFormOptions = value;
                if (value == true)
                {
                    btnFormIcon.Cursor = Cursors.Arrow;
                    btnFormIcon.FlatAppearance.MouseOverBackColor = UIAppearance.StyleColor;
                    btnFormIcon.FlatAppearance.MouseDownBackColor = UIAppearance.StyleColor;
                }
            }
        }

        [Browsable(false)]
        public int MarkerPosition
        {// Gets or sets the location of the menu button marker on the main form
            get { return markerPosition; }
            set { markerPosition = value; }
        }

        // Design
        public new Size ClientSize
        {
            //Hide client size so that the default size (Main Form Desktop Panel  Size) takes effect on derived forms
            //You can disable it by setting the _DesktopPanelSize property to false
            get { return base.ClientSize; }
            set
            {
                if (desktopPanelSize == false)
                {//If desktopPanelSize field is false, set value as form size
                    base.ClientSize = value;
                }
                else
                {
                    //Otherwise keep the default size set 
                }
            }
        }
        protected override Size DefaultSize
        {//Form default size
            get { return new Size(960, 560); }
            ///<Note>The default size of the form should be equal to the size of the desktop panel of the main form +Height Title Bar(40)
            ///this to avoid problems with the location and display of the controls when displaying the form on the desktop panel.
            ///In addition to having an exact and elegant design 
            ///where you can have more control over the control's dock and anchor properties</note>
        }
        public override string Text
        {//Overriden text property to extend functionality.
            get { return base.Text; }
            set
            {
                base.Text = value;
                lblCaption.Text = value;//Set form caption
            }
        }

        [Category("RJ Code Advance")]
        [TypeConverter(typeof(FontAwesome.Sharp.IconConverter))]
        public IconChar FormIcon
        {//Gets or Sets the Form icon
            get { return formIcon; }
            set
            {
                formIcon = value;
                btnFormIcon.IconChar = formIcon;
            }
        }

        [Category("RJ Code Advance")]
        public string Caption
        {//Gets or sets the Form Caption
            get { return this.Text; }
            set
            {
                this.Text = value;
                lblCaption.Text = value;
            }
        }

        [Category("RJ Code Advance")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string HelpMessage
        {//Form help message to the user
            get { return helpMessage; }
            set
            {
                helpMessage = value;
            }
        }
        #endregion

        #region  -> Private methods

        private void ApplyApperanceSettings()
        {//Apply theme settings

            this.pnlClientArea.BackColor = UIAppearance.BackgroundColor;//Set the background color of the form

            if (IsChildForm)//If it is a child form. That is, it is displayed on the desktop panel of the main form.
            {
                pnlTitleBar.Visible = false;//Hide title bar            
                this.BorderSize = 0;//Remove form border                          
            }
            else //If it is not a child form. That is, it is displayed outside the desktop pane of the main form
            {   //The form has the title bar and border
                if (!this.DesignMode)
                    this.CenterToScreen();//Center window
                if (UIAppearance.Style == UIStyle.Supernova)
                {
                    pnlTitleBar.BackColor = RJColors.DarkItemBackground;
                    btnFormIcon.IconColor = supernovaColor;
                }
                else
                {
                    pnlTitleBar.BackColor = UIAppearance.PrimaryStyleColor;//set title bar backcolor 
                    btnFormIcon.IconColor = Color.WhiteSmoke;
                }
                pnlTitleBar.Visible = true;//Show title bar
                lblCaption.Text = this.Text;//Set Form caption
                btnFormIcon.IconChar = FormIcon;//Set Form icon               
                this.BorderSize = UIAppearance.FormBorderSize;//The form Border Width will be equal to the border of the user settings
                this.BorderColor = UIAppearance.FormBorderColor;//Set form border color

            }
        }
        #endregion

        #region -> Overrides
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();//Dispose components
            }
            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (isChildForm == false) // If the IsChildform property is TRUE, the ApplyApperanceSettings method is called there, so it is not necessary to call it again.
                ApplyApperanceSettings();//Apply appearance settings in load event.           
        }
        #endregion

        #region -> Events Methods

        private void FormIcon_MouseEnter(object sender, EventArgs e)
        {   //If the mouse pointer hovers over the form icon button, change form icon to options list
            //as long as the form options drop down is not disabled and the options drop down has not been shown
            if (disableFormOptions == false)
            {
                if (dmFormOptions.Visible == false)
                    btnFormIcon.IconChar = IconChar.ListUl;//Options list icon
            }
        }
        private void FormIcon_MouseLeave(object sender, EventArgs e)
        {   //If the mouse pointer leave the form icon button, set the form icon again
            //as long as the form options drop down is not disabled and has not been shown
            if (disableFormOptions == false)
            {
                if (dmFormOptions.Visible == false)
                    btnFormIcon.IconChar = FormIcon;//Form icon
            }
        }
        private void FormIcon_Click(object sender, EventArgs e)
        {//if the form icon is clicked and the form options dropdown menu is not disabled
            //Show drop-down menu from list of form options
            if (disableFormOptions == false)
                this.dmFormOptions.Show(pnlTitleBar, DropdownMenuPosition.LeftBottom);//Show at the bottom left of the form
        }
        private void FormOptions_VisibleChanged(object sender, EventArgs e)
        {//When the form options dropdown is shown or hidden

            if (dmFormOptions.Visible == true)//If menu is displayed
            {//keep button highlighted and set icon to options list                
                btnFormIcon.BackColor = ColorEditor.Darken(btnFormIcon.BackColor, 15);
                btnFormIcon.FlatAppearance.MouseOverBackColor = btnFormIcon.BackColor;
                btnFormIcon.IconChar = IconChar.ListUl;//Options list Icon
            }
            else // If menu is hidden
            {//Return the default color and icon
                if (UIAppearance.Style == UIStyle.Supernova)
                    btnFormIcon.BackColor = RJColors.DarkItemBackground;
                else btnFormIcon.BackColor = UIAppearance.PrimaryStyleColor;
                btnFormIcon.IconChar = FormIcon;
            }
        }
        private void HelpMessage_Click(object sender, EventArgs e)
        {//Show the form help message
            //if (helpMessage == "" || helpMessage == null)
            //    RJMessageBox.Show("No help message has been added for this form", "Message");
            //else
            //    RJMessageBox.Show(helpMessage, "Quick Help");
        }
        private void Form_Resize(object sender, EventArgs e)
        {
            if (this.DesignMode)//Only in design mode
            {
                if (desktopPanelSize)//if the desktopPanelSize field is true, simply allow to change the height of the form, the width should be kept equal to the width of the desktop panel
                    this.Size = new Size(960, this.Size.Height);
            }
            ///<Note>If the form is in design mode, it will not be possible to change the width of the form
            ///to always have the same width as the desktop panel of the main form, 
            ///this to have an exact and elegant design.
            ///However, if it is possible to change the height of the form and scroll down
            ///If you don't agree with this, you can remove this code</Note>
        }
        private void Form_Deactivated(object sender, EventArgs e)
        {//When the form goes into deactivated mode (loses focus) change the color of the title bar.
            pnlTitleBar.SuspendLayout();
            pnlTitleBar.BackColor = UIAppearance.DeactiveFormColor;//Set title bar backcolor  
            this.BorderColor = RJColors.DefaultFormBorderColor;//Set border color

            if (UIAppearance.Style == UIStyle.Supernova)//If the style is supernova, change the icon color to white
                btnFormIcon.IconColor = Color.WhiteSmoke;
            pnlTitleBar.ResumeLayout();
            pnlTitleBar.Update();//Force draw the title bar to avoid flickering when the background color is changed.

        }
        private void Form_Activated(object sender, EventArgs e)
        {//When the form enters activated mode (regains focus - form is redisplayed), 
            //Reset the color of the title bar and border of the form.
            if (UIAppearance.Style == UIStyle.Supernova)
            {
                pnlTitleBar.BackColor = RJColors.DarkItemBackground;//Set title bar backcolor    
                btnFormIcon.IconColor = supernovaColor;//Set icon color
            }
            else pnlTitleBar.BackColor = UIAppearance.PrimaryStyleColor;//Set title bar backcolor 

            this.BorderColor = UIAppearance.FormBorderColor;//Set border color

            pnlTitleBar.Update();//Force draw the title bar to avoid flickering when the background color is changed.
        }
        #endregion
    }
}
