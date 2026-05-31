using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.ComponentModel;
using FontAwesome.Sharp;
using RJCodeUI_M1.RJControls;
using RJCodeUI_M1.Settings;
using RJCodeUI_M1.Utils;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Collections.Generic;

namespace RJCodeUI_M1.RJForms
{
    public class RJMainForm : RJBaseForm
    {
        /// This class inherits from the <see cref="RJBaseForm"/> class
        /// 
        ///<summary>
        ///In this class, a borderless window is set and add a custom title bar, 
        ///a side menu and a client area using panels, you set a method to 
        ///open the child forms within the area of the client (desktop), in addition
        ///to adding screenshot functions, printing and moving the child form to a new window. 
        ///As well as add the buttons to maximize, minimize, close and the Dropdown Menu for the
        ///list of options in the form.
        ///</summary>

        #region -> Fields

        /// Fields        
        private IContainer components = null; //Container for components that are not child of the form. 
        ///Allows all added components to be removed with the Dispose method by the components container
        ///<see cref="protected override void Dispose(bool disposing)"/>
        private RJChildForm activeChildForm;//Sets and Gets the currently active child form on the desktop panel
        private List<RJChildForm> listChildForms;//Stores child forms that have been opened and are active in the background (forms are hidden)
        private bool deactivateFormEvent;
        /// Control Fields      
        protected Panel pnlSideMenu;//Side menu    
        protected Panel pnlSideMenuHeader;//Side menu header    
        protected Panel pnlTitleBar;//Title bar 
        protected Panel pnlDesktop;//Desktop (Cliente area: Child form container)this panel will only store and display a single child form. The other deactivated child forms are stored in the generic list of forms.  
        protected Panel pnlDesktopHeader;//Desktop header 

        internal RJDropdownMenu dmFormOptions;//Dropdown menu of Form Options (is component, the constructor accepts a parameter of type IContainer)

        private RJLabel lblCaption;//Form Caption

        private Panel pnlSide;
        private Panel pnlMarker;//Current child form marker, places a marker on the menu button to indicate which is current / active form on the application desktop
        private RJMenuIcon biSideMenuButton;//Side menu button
        private RJMenuIcon biFormIcon;//Form icon button
        private IconButton btnMoveNewWindow;//Move child form to new window
        private IconButton btnScreenshot;//Form screenshot
        private IconButton btnPrint;//print child form
        private IconButton btnHelp;//Child form Help       
        private RJDragControl dragControl;//Form drag control, (is component, the constructor accepts a parameter of type IContainer)       
        private IconMenuItem miCloseChildForms;//menu item Close all child forms
        private ToolTip toolTip;//Displays a brief description of a control's (is component, the constructor accepts a parameter of type IContainer)        

        ///<Note>:ICON MENU ITEM and ICON BUTTON is provided by <see cref="FontAwesome.Sharp"/> library
        ///      Autor: mkoertgen
        ///      GitHub: https://github.com/awesome-inc/FontAwesome.Sharp
        ///      Nuget Package: https://www.nuget.org/packages/FontAwesome.Sharp </Note> 
        #endregion Fields

        #region -> Constructor

        /// Constructor      
        public RJMainForm()
        {
            RJColors.colorIndex = 0;//Reset superonva color index
            InitializeItems();
            listChildForms = new List<RJChildForm>();//Initialize generic list
        }

        /// Initialize Component
        private void InitializeItems()
        {
            /// Initialize the components (object instantiation) for the form design:
            /// add the title bar, side menu, buttons for maximized, minimized, form options-user options dropdown menu
            /// and the client area(Desktop) of the form  
            /// 
            components = new System.ComponentModel.Container();//initialize container

            #region -Control Instantiation
            pnlSideMenu = new Panel();
            pnlSideMenuHeader = new Panel();
            pnlTitleBar = new Panel();
            pnlDesktop = new Panel();
            pnlDesktopHeader = new Panel();
            pnlMarker = new Panel();
            pnlSide = new Panel();
            btnMoveNewWindow = new FontAwesome.Sharp.IconButton();///(FontAwesome.Sharp library)
            btnScreenshot = new FontAwesome.Sharp.IconButton();
            btnPrint = new FontAwesome.Sharp.IconButton();
            btnHelp = new FontAwesome.Sharp.IconButton();
            biSideMenuButton = new RJMenuIcon();
            biFormIcon = new RJMenuIcon();
            lblCaption = new RJLabel();
            dmFormOptions = new RJDropdownMenu(components);//Add to component container
            miCloseChildForms = new FontAwesome.Sharp.IconMenuItem();///(FontAwesome.Sharp library)
            toolTip = new ToolTip(components);//Add to component container
            dragControl = new RJDragControl(pnlTitleBar, this, components);///Drag Control, add to component container
            //Suspend layout           
            #endregion

            #region -Form Side Menu
            //
            //  Panel: Side Panel
            //
            pnlSide.Name = "pnlSide";
            pnlSide.Dock = DockStyle.Left;
            pnlSide.Location = new Point(0, 0);
            pnlSide.Size = new Size(220, 610);
            pnlSide.Controls.Add(pnlSideMenu);
            pnlSide.Controls.Add(pnlSideMenuHeader);
            //
            //  Panel: Side Menu      
            //
            pnlSideMenu.Name = "pnlSideMenu";
            pnlSideMenu.Padding = new Padding(0, 65, 0, 0);
            pnlSideMenu.BackColor = RJColors.SideMenuColor;
            pnlSideMenu.Dock = DockStyle.Fill;
            pnlSideMenu.Location = new Point(0, 60);
            pnlSideMenu.Size = new Size(220, 550);
            pnlSideMenu.Controls.Add(pnlMarker);//Add controls

            // 
            // Panel: Side Menu Header
            //
            pnlSideMenuHeader.Name = "pnlSideMenuHeader";
            pnlSideMenuHeader.Controls.Add(biSideMenuButton);//Add controls
            pnlSideMenuHeader.Size = new Size(220, 60);
            pnlSideMenuHeader.Dock = DockStyle.Top;
            pnlSideMenuHeader.Location = new Point(0, 0);
            // 
            // RJ BarIcon: Side Menu Button
            // 
            biSideMenuButton.Name = "biSideMenuButton";
            biSideMenuButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            biSideMenuButton.Customizable = true;
            biSideMenuButton.IconColor = Color.White;
            biSideMenuButton.BackColor = Color.Transparent;
            biSideMenuButton.Cursor = Cursors.Hand;
            biSideMenuButton.IconChar = FontAwesome.Sharp.IconChar.Bars;///(FontAwesome.Sharp library)
            biSideMenuButton.IconSize = 25;
            biSideMenuButton.Location = new Point(175, 20);
            biSideMenuButton.Size = new System.Drawing.Size(25, 25);
            biSideMenuButton.Click += new System.EventHandler(SideMenuButton_Click);//Event
            //
            // Panel: Child Form Marker
            //
            pnlMarker.Name = "pnlChildFormMarker";
            pnlMarker.Size = new Size(4, 55);
            pnlMarker.Location = new Point(0, 60);
            pnlMarker.Visible = false;
            #endregion

            #region -Form Title Bar
            //
            //  Panel: TitleBar
            //
            pnlTitleBar.Name = "pnlTitleBar";
            pnlTitleBar.BackColor = UIAppearance.StyleColor;
            pnlTitleBar.Size = new Size(960, 60);
            pnlTitleBar.Location = new Point(220, 0);
            pnlTitleBar.Dock = DockStyle.Top;
            pnlTitleBar.Controls.Add(lblCaption);//Add controls
            pnlTitleBar.Controls.Add(biFormIcon);
            pnlTitleBar.Controls.Add(this.btnMinimize);
            pnlTitleBar.Controls.Add(this.btnMaximize);
            pnlTitleBar.Controls.Add(this.btnClose);
            // 
            // Label: Form Caption
            // 
            lblCaption.Name = "lblCaption";
            lblCaption.AutoSize = true;
            lblCaption.Style = LabelStyle.BarCaption;
            lblCaption.Location = new Point(46, 20);
            lblCaption.TextAlign = ContentAlignment.MiddleLeft;
            lblCaption.Text = "Home";
            //
            // Button: Control box buttons
            //            
            this.btnClose.Size = new Size(35, 20);
            this.btnClose.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

            this.btnMaximize.Size = new Size(35, 20);
            this.btnMaximize.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

            this.btnMinimize.Size = new Size(35, 20);
            this.btnMinimize.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            // 
            // Bar Icon: Active Form Icon
            // 
            biFormIcon.Name = "biFormIcon";
            biFormIcon.BackColor = Color.Transparent;
            biFormIcon.Cursor = Cursors.Hand;
            biFormIcon.IconChar = FontAwesome.Sharp.IconChar.Home;///(FontAwesome.Sharp library)
            biFormIcon.IconSize = 25;
            biFormIcon.Location = new Point(19, 20);
            biFormIcon.Size = new Size(25, 25);
            biFormIcon.SizeMode = PictureBoxSizeMode.AutoSize;
            biFormIcon.Click += new System.EventHandler(this.FormIcon_Click);//Events
            biFormIcon.MouseEnter += new System.EventHandler(this.FormIcon_MouseHover);
            biFormIcon.MouseLeave += new System.EventHandler(this.FormIcon_MouseLeave);
            #endregion

            #region -Form Options
            //
            // Icon MenuItem: Close Child Forms (FontAwesome.Sharp library)
            //
            miCloseChildForms.Name = "miCloseChildForms";
            miCloseChildForms.Text = "Close Child Forms";
            miCloseChildForms.IconSize = 24;
            miCloseChildForms.IconChar = FontAwesome.Sharp.IconChar.CalendarTimes;
            miCloseChildForms.IconColor = RJColors.FantasyColorScheme4;
            miCloseChildForms.Click += new System.EventHandler(CloseAllChildForms_Click);//Event
            //
            //  DropdownMenu: Form Options -------------
            //
            dmFormOptions.Name = "dmFormOptions";
            dmFormOptions.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            dmFormOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            miSnapLeft,//Snap Window Left
            miSnapRight,//Snap Window Right
            miExitSnap,//Exit Snap Window
            miCloseChildForms});
            dmFormOptions.OwnerIsMenuButton = false;
            #endregion

            #region -Desktop Header (Child Form Options)
            // 
            //  Panel: Desktop Header -------------
            //
            pnlDesktopHeader.Name = "pnlDesktopHeader";
            pnlDesktopHeader.BackColor = RJColors.LightBackground;
            pnlDesktopHeader.Dock = DockStyle.Top;
            pnlDesktopHeader.Location = new Point(220, 60);
            pnlDesktopHeader.Size = new Size(960, 25);
            pnlDesktopHeader.Padding = new Padding(14, 0, 0, 0);
            pnlDesktopHeader.Controls.Add(btnHelp);//Add controls
            pnlDesktopHeader.Controls.Add(btnMoveNewWindow);
            pnlDesktopHeader.Controls.Add(btnScreenshot);
            pnlDesktopHeader.Controls.Add(btnPrint);
            // 
            // Icon Button: Print Form (FontAwesome.Sharp library)
            // 
            btnPrint.Dock = DockStyle.Left;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.IconSize = 20;
            btnPrint.IconChar = FontAwesome.Sharp.IconChar.Print;
            btnPrint.IconColor = RJColors.FantasyColorScheme1;
            btnPrint.Location = new Point(0, 0);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(35, 25);
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Cursor = Cursors.Hand;
            btnPrint.Visible = false;
            btnPrint.Click += new EventHandler(Print_Click);
            // 
            // Icon Button: Form Screenshot (FontAwesome.Sharp library)
            // 
            btnScreenshot.Dock = DockStyle.Left;
            btnScreenshot.FlatAppearance.BorderSize = 0;
            btnScreenshot.FlatStyle = FlatStyle.Flat;
            btnScreenshot.IconSize = 20;
            btnScreenshot.IconChar = FontAwesome.Sharp.IconChar.Camera;
            btnScreenshot.IconColor = RJColors.FantasyColorScheme2;
            btnScreenshot.Location = new Point(0, 0);
            btnScreenshot.Name = "btnScreenShoot";
            btnScreenshot.Size = new Size(35, 25);
            btnScreenshot.UseVisualStyleBackColor = true;
            btnScreenshot.Cursor = Cursors.Hand;
            btnScreenshot.Visible = false;
            btnScreenshot.Click += new EventHandler(Screenshot_Click);
            // 
            // Icon Button: Move Child Form New Window (FontAwesome.Sharp library)
            // 
            btnMoveNewWindow.Name = "btnOpenNewWindow";
            btnMoveNewWindow.Dock = DockStyle.Left;
            btnMoveNewWindow.FlatAppearance.BorderSize = 0;
            btnMoveNewWindow.FlatStyle = FlatStyle.Flat;
            btnMoveNewWindow.IconSize = 20;
            btnMoveNewWindow.IconChar = FontAwesome.Sharp.IconChar.WindowMaximize;
            btnMoveNewWindow.IconColor = RJColors.FantasyColorScheme3;
            btnMoveNewWindow.Location = new Point(15, 0);
            btnMoveNewWindow.Size = new Size(35, 25);
            btnMoveNewWindow.UseVisualStyleBackColor = true;
            btnMoveNewWindow.Cursor = Cursors.Hand;
            btnMoveNewWindow.Visible = false;
            btnMoveNewWindow.Click += new EventHandler(MoveNewWindow_Click);//Event
            // 
            // Icon Button: User Help (FontAwesome.Sharp library)
            // 
            btnHelp.Dock = DockStyle.Left;
            btnHelp.FlatAppearance.BorderSize = 0;
            btnHelp.FlatStyle = FlatStyle.Flat;
            btnHelp.IconSize = 20;
            btnHelp.IconChar = FontAwesome.Sharp.IconChar.QuestionCircle;
            btnHelp.IconColor = RJColors.FantasyColorScheme4;
            btnHelp.Location = new Point(0, 0);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new Size(35, 25);
            btnHelp.UseVisualStyleBackColor = true;
            btnHelp.Cursor = Cursors.Hand;
            btnHelp.Visible = false;
            btnHelp.Click += new EventHandler(ChilFormHelp_Click);
            #endregion

            #region -Desktop (Cliente area - Child form container)
            //
            //  Panel: Desktop
            //
            pnlDesktop.Name = "pnlDesktop";
            pnlDesktop.BackColor = RJColors.LightBackground;
            pnlDesktop.Dock = DockStyle.Fill;
            pnlDesktop.Location = new Point(220, 85);
            pnlDesktop.Size = new Size(960, 525);
            #endregion

            #region -Others
            //
            // ToolTip
            //   
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;
            toolTip.BackColor = Color.FromArgb(236, 82, 99);
            toolTip.ForeColor = Color.White;
            toolTip.OwnerDraw = true;
            toolTip.Draw += new DrawToolTipEventHandler(ToolTip_Draw);
            toolTip.SetToolTip(btnPrint, "Print child form content");
            toolTip.SetToolTip(btnScreenshot, "Take a screenshot");
            toolTip.SetToolTip(btnMoveNewWindow, "Move child form to new window");
            toolTip.SetToolTip(btnHelp, "Help message to user");
            #endregion

            #region -RJ Main Form Properties
            //
            //RJMainForm
            //
            deactivateFormEvent = true;
            this.Resizable = true;
            this.PrimaryForm = true;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.None;
            this.FormBorderStyle = FormBorderStyle.None;//Borderless form     
            this.MinimumSize = new Size(650, 400);//Minimun form size 
            this.Controls.Add(pnlDesktop);//Add controls
            this.Controls.Add(pnlDesktopHeader);
            this.Controls.Add(pnlTitleBar);
            this.Controls.Add(pnlSide);
            this.Deactivate += new EventHandler(Form_Deactivated);//subscribe Deactivate event to change / opaque title bar color
            this.Activated += new EventHandler(Form_Activated);//Subscribe Activated event to retrieve title bar color

            #endregion
        }
        #endregion

        #region -> Properties

        public new Size ClientSize
        {//Hide client size so that the default size takes effect on derived forms
            get { return base.ClientSize; }
            set { }
        }
        protected override Size DefaultSize
        {//Form default size
            get { return new Size(1180, 610); }
        }
        #endregion

        #region -> Protected methods

        ///Open Child Form
        protected void OpenChildForm<childForm>(Func<childForm> _delegate) where childForm : RJChildForm
        {
            ///Generic method with a generic delegate parameter (Func <TResult>) where the data type is RJChildForm.
            ///This method ALLOWS to open forms WITH or WITHOUT parameters within the desktop panel. On many occasions,
            ///the youtube tutorials used methods similar to this. However, it simply allowed forms to be opened 
            ///WITHOUT parameters ( e.g <see cref="new MyForm ()"/> )
            ///and it was impossible to open a form WITH parameters( e.g. <see cref="new MyForm (user:"John", mail:"john@gmail.com"/> )
            ///so this method solves this defect thanks to the generic delegate (Func <TResult>)       

            RJChildForm form = Application.OpenForms.OfType<childForm>().FirstOrDefault();//Checks if the child form is already open

            #region - Show Child Form as a New Instance

            if (form == null)//If there is no result, the form is not open, create the instance and display it on the desktop panel
            {
                deactivateFormEvent = false;//Don't deactivate form when child form opens

                form = _delegate();// Execute the delegate                
                form.IsChildForm = true;//Set form as child form     
                form.TopLevel = false;//Indicates that the  form is not TopLevel     
                form.Dock = DockStyle.Fill; //Sets fill dock style (Fill desktop panel)           
                form.MarkerPosition = 0;//Set the Marker Position to zero, because the form is not opening from a menu button.

                CleanDesktop(); //Remove current child form from desktop panel
                // If the multiple child forms option is disabled, only allow to open a single form within the desktop panel, then definitively close the previous one.
                if (UIAppearance.MultiChildForms == false) CloseActiveChildForm();//Close current child form - This does not affect if the secondary form is open in a new window.

                listChildForms.Add(form);//Add child form to form list
                pnlDesktop.Controls.Add(form);//Add child form to desktop panel
                pnlDesktop.Tag = form;//Store the form 

                form.Show();//Show form   
                form.BringToFront();
                form.Focus();//Focus form

                activeChildForm = form;//set FORM as active child form

                SetChildFormItems();//-> Set the Caption, Icon and Options of the child form in the layout of the main form

                deactivateFormEvent = true;//Reset value
            }
            #endregion

            #region - Redisplay Existing Form or Child Form
            else //If the form is already open, just show the form again
            {
                if (form.IsChildForm && form != activeChildForm)
                {//if the form is a child form and the form is different from the active child form add back to the panel and set as active child form

                    CleanDesktop();//Remove current child form from desktop panel
                    pnlDesktop.Controls.Add(form);//add child form to desktop panel
                    pnlDesktop.Tag = form;//Store the form 
                    activeChildForm = form;//set FORM as active child form
                    SetChildFormItems();//-> Set the Caption, Icon and Options of the child form in the layout of the main form
                }

                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;
                form.Show();//show the form on the desktop panel or outside in a new window if the form IS NOT A CHILD FORM
                form.BringToFront();//Bring the form to the front
                form.Focus();//Focus form
            }
            #endregion

            ///<Note>
            /// You can use the Func<TResult> delegate with anonymous methods or lambda expression,
            /// for example, we can call this method as follows: Suppose we are in the button click event method.
            /// With anonymous method:
            ///     <see cref="OpenChildForm( delegate () { return new MyForm('MyParameter'); });"/>    
            /// With lambda expression
            ///     <see cref="OpenChildForm( () => new MyForm('id', 'username'));"/>
            /// </Note>

        }//Here main comments
        protected void OpenChildForm<childForm>(Func<childForm> _delegate, object senderMenuButton) where childForm : RJChildForm
        {  ///Generic method with a Generic Delegate Parameter (Func <TResult>) and Menu Button Parameter where the data type is RJChildForm.

            if (senderMenuButton == null || senderMenuButton.GetType() != typeof(RJMenuButton))//Check if object is valid
            {
                RJMessageBox.Show("Please send valid object, null values ​​are not allowed");
                return;
            }
            RJMenuButton menuButton = (RJMenuButton)senderMenuButton;
            RJChildForm form = Application.OpenForms.OfType<childForm>().FirstOrDefault();//Checks if the child form is already open

            #region - Show Child Form as a New Instance
            if (form == null)//If there is no result, the form is not open, create the instance and display it on the desktop panel
            {
                deactivateFormEvent = false;//Don't deactivate form when child form opens

                form = _delegate();// Execute the delegate
                form.IsChildForm = true;
                form.TopLevel = false;//Indicates that the  form is not TopLevel     
                form.Dock = DockStyle.Fill;
                form.MarkerPosition = menuButton.Location.Y;//Set marker position   

                CleanDesktop(); //remove current child form from desktop panel
                if (UIAppearance.MultiChildForms == false) CloseActiveChildForm();//Close current child form if multiple child forms option is disabled- This does not affect if the secondary form is open in a new window.

                listChildForms.Add(form);//add child form to form list
                pnlDesktop.Controls.Add(form);//add child form to desktop panel
                pnlDesktop.Tag = form;

                form.Show();
                form.BringToFront();
                form.Focus();

                activeChildForm = form;//set FORM as active child form

                SetChildFormItems();//-> Set the Caption, Icon and Options of the child form in the layout of the main form
                menuButton.Activate(form); //-> Activate the menu button and associate to the form so that the button remains highlighted when the form is instantiated.

                deactivateFormEvent = true;//Reset value
            }
            #endregion

            #region - Redisplay Existing Form or Child Form
            else //If the form is already open, just show the form again
            {
                if (form.IsChildForm && form != activeChildForm)
                {//if the form is a child form and the form is different from the active child form add back to the panel and set as active child form

                    CleanDesktop();//Remove current child form from desktop panel
                    pnlDesktop.Controls.Add(form);//add child form to desktop panel
                    pnlDesktop.Tag = form;
                    activeChildForm = form;//set FORM as active child form
                    SetChildFormItems();//-> Set the Caption, Icon and Options of the child form in the layout of the main form
                }

                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;
                form.Show();//show the form on the desktop panel or outside in a new window if the form IS NOT A CHILD FORM
                form.BringToFront();
                form.Focus();

            }
            #endregion
        }
        protected void OpenChildForm<childForm>(Func<childForm> _delegate, object senderMenuItem, RJMenuButton ownerMenuButton) where childForm : RJChildForm
        { ///Generic method with a Generic Delegate (Func <TResult>), Menu Button and Menu Item Parameters where the data type is RJChildForm.

            if (ownerMenuButton == null || senderMenuItem == null || senderMenuItem.GetType() != typeof(ToolStripMenuItem))//validate parameters
            {
                RJMessageBox.Show("Please send valid object, null values ​​are not allowed");
                return;
            }

            RJChildForm form = Application.OpenForms.OfType<childForm>().FirstOrDefault();//Checks if the child form is already open

            #region - Show Child Form as a New Instance
            if (form == null)//If there is no result, the form is not open, create the instance and display it on the desktop panel
            {
                deactivateFormEvent = false;//Don't deactivate form when child form opens

                form = _delegate();// Execute the delegate
                form.IsChildForm = true;
                form.TopLevel = false;//Indicates that the  form is not TopLevel     
                form.Dock = DockStyle.Fill;
                form.MarkerPosition = ownerMenuButton.Location.Y;//Set marker position   

                CleanDesktop();//Remove current child form from desktop panel
                if (UIAppearance.MultiChildForms == false) CloseActiveChildForm();// Close current child form if multiple child forms option is disabled- This does not affect if the secondary form is open in a new window.

                listChildForms.Add(form);//add FORM to form list
                pnlDesktop.Controls.Add(form);//add FORM to desktop panel
                pnlDesktop.Tag = form;

                form.Show(); //show on desktop panel  
                form.BringToFront();
                form.Focus();

                activeChildForm = form;//set FORM as active child form

                SetChildFormItems();//-> Set the Caption, Icon and Options of the child form in the layout of the main form
                ownerMenuButton.Activate(form, (ToolStripMenuItem)senderMenuItem); //-> Activate the menu button and associate
                // a form for the button to remain highlighted until all forms are closed.
                // You can have many associated forms since the menu button shows a drop-down menu.
                // Also activate the menu item and associate it to the form so that the menu item remains
                // highlighted when the form is instantiated.

                deactivateFormEvent = true;//Reset value
            }
            #endregion

            #region - Redisplay Existing Form or Child Form
            else //If the form is already open, just show the form again
            {
                if (form.IsChildForm && form != activeChildForm)
                {//if the form is a child form and the form is different from the active child form add back to the panel and set as active child form

                    CleanDesktop();//Remove current child form from desktop panel
                    pnlDesktop.Controls.Add(form);//add FORM to desktop panel
                    pnlDesktop.Tag = form;
                    activeChildForm = form;//set FORM as active child form
                    SetChildFormItems();//-> Set the Caption, Icon and Options of the child form in the layout of the main form
                }

                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;
                form.Show();//show the form on the desktop panel or outside in a new window if the form IS NOT A CHILD FORM
                form.BringToFront();
                form.Focus();

            }
            #endregion
        }
        #endregion

        #region -> Private methods
        ///Load Design Parameters        
        private void SetChildFormItems()
        {
            //When a child form is displayed on the desktop, set the title, icon, marker and options of the child form in the layout of the main form

            biFormIcon.IconChar = activeChildForm.FormIcon;//Set form icon
            lblCaption.Text = activeChildForm.Text;//Set form caption
            pnlDesktopHeader.BackColor = UIAppearance.ItemBackgroundColor;//Set desktop header backcolor

            // Show child form option buttons
            btnHelp.Visible = true;
            btnMoveNewWindow.Visible = true;
            btnScreenshot.Visible = true;
            btnPrint.Visible = true;

            /// Set Child form marker, as long as the ChildFormMarker field of UIAppearance is set to true
            /// and the MarkerPosition field of the active form has a value other than 0 (In this case, Zero 
            /// is equal to a position not being set for the marker because the form was not opened from 
            /// a menu button) see <see cref="OpenChildForm<childForm>(Func<childForm> _delegate)"/> method.
            if (UIAppearance.ChildFormMarker == true && activeChildForm.MarkerPosition != 0)
            {
                pnlMarker.Location = new Point(0, activeChildForm.MarkerPosition);
                pnlMarker.Visible = true;
            }
        }
        private void ResetToDefaults()
        {
            //Reset main form to defaults

            activeChildForm = null; //Set current child form field to null
            lblCaption.Text = "Home";
            biFormIcon.IconChar = IconChar.Home;
            pnlMarker.Visible = false;//Hide child form marker
            pnlDesktopHeader.BackColor = UIAppearance.BackgroundColor;
            btnPrint.Visible = false;//Hide option buttons of child form
            btnScreenshot.Visible = false;
            btnMoveNewWindow.Visible = false;
            btnHelp.Visible = false;
        }
        private void UpdateFormLayout()
        {//This method is responsible for updating the parent form with the proper parameters,
            //either resetting the default values or displaying an older child form if that's the case.

            var childForm = listChildForms.LastOrDefault();//Check and get the last (previous) child form in the form list
            if (childForm == null) //If there is no result, there is no instance in the list of child forms
            {
                ResetToDefaults(); //Reset main form to defaults
            }
            else//if there is an instantiated child form in the list, add it back to the desktop panel, set it as an active child form and display it.
            {

                activeChildForm = childForm;//Set form as current child form (previous form) 
                pnlDesktop.Controls.Add(childForm);//Add to desktop panel
                pnlDesktop.Tag = childForm;//Store the form 

                childForm.Show();//Show child form
                childForm.BringToFront();

                SetChildFormItems();//Set the Caption, Icon and Options of the child form in the layout of the main form
            }
        }
        private void CleanDesktop()
        {//clean desktop panel

            if (activeChildForm != null)
            {
                activeChildForm.Hide();
                pnlDesktop.Controls.Remove(activeChildForm);
            }
            /*This method hides and removes the active child form from the desktop panel, so there will only be
            one child form open within the desktop panel, since adding a new form removes the old form and adds 
            the new one (check the OpenChildForm method) Inactive child forms are stored in a generic list.

            I created these methods to get rid of the doubts as many think (myself included) that having 20 or 
            50 forms added within the desktop affects performance, well I didn't realize that. In fact, it is 
            possible to add 10 thousand controls dynamically in a displayed form and there is no limit if it is
            added from the form's constructor, for experimenting, I added 100 thousand labels and 10 thousand 
            panels with color although it took more than 10 minutes to show ( pc: 8 ram, SixCore 3.50 GHz cpu) 
            and there is no performance problem (consumed 20mb ram) and scrolling the form works fine.

            Therefore, if these methods seem very tedious or difficult to understand, you can use the methods 
            of opening a child form within the Previous Projects panel (tutorials) that are shown on YouTube,
            where the secondary forms are stored within the desktop, they overlap one after the other and one 
            is displayed (form.bringtofront ();).

            However, it still doesn't seem appropriate to me to add so many forms within a panel considering 
            that a default form is top-level and I don't like the idea of ​​having so many controls in a panel (child form controls).

            As a result, I preferred to store the child forms in a generic list and add and display only a 
            single form on the desktop panel :) */
        }
        private void ApplyAppearanceSettings()
        {//Apply settings            
            pnlDesktop.BackColor = UIAppearance.BackgroundColor;//Set background color
            pnlDesktopHeader.BackColor = UIAppearance.BackgroundColor;//set desktop header backcolor
            this.BorderColor = UIAppearance.FormBorderColor;//Set border color
            this.BorderSize = UIAppearance.FormBorderSize;//Set form Border Width

            if (UIAppearance.Style == UIStyle.Supernova)//if the style is supernova
            {
                pnlTitleBar.BackColor = ColorEditor.Darken(UIAppearance.BackgroundColor, 7);//Set title bar backcolor
                pnlSideMenuHeader.BackColor = RJColors.SideMenuColor;//Set Side menu header backcolor
                pnlMarker.BackColor = Color.CornflowerBlue;//Set marker colo
            }
            else //any style that is not supernova
            {
                pnlTitleBar.BackColor = UIAppearance.PrimaryStyleColor;//Set title bar backcolor
                pnlSideMenuHeader.BackColor = ColorEditor.Darken(UIAppearance.PrimaryStyleColor, 6);//Set Side menu header backcolor
                pnlMarker.BackColor = UIAppearance.StyleColor;//Set marker color                
            }
        }

        /// Child Form Options
        private void MoveFormNewWindow()
        {//Move Active Child Form to New Window

            if (activeChildForm != null)
            {
                listChildForms.Remove(activeChildForm);//Remove current child form from list forms. 
                pnlDesktop.Controls.Remove(activeChildForm);//Remove current child form from desktop panel.                       
                activeChildForm.IsChildForm = false;//Set the form as normal form (Has a title bar and is resized).
                activeChildForm.TopLevel = true;//Set the form as top level.
                activeChildForm.Dock = DockStyle.None;

                UpdateFormLayout();//Refresh Form.

            }
        }
        private void CloseActiveChildForm()
        {//Close Active child Form

            if (activeChildForm != null)
            {
                listChildForms.Remove(activeChildForm);//Remove current child form from list forms. 
                pnlDesktop.Controls.Remove(activeChildForm);//Remove current child form from desktop panel .
                activeChildForm.Close();//Close current child form.             
            }
        }
        private void CloseAllChildForm()
        {//Close all secondary forms

            var childForms = Application.OpenForms.OfType<RJChildForm>().FirstOrDefault();// Check if there is a secondary form open 

            if (childForms != null)//If there is any form open, close all forms except this form and the main form of the application(e.g. Login form)
            {
                //Application.OpenForms.Cast<Form>().Except(new Form[] { this, Program.MainForm }).ToList().ForEach(x => x.Close());
                listChildForms.Clear();//Clear List forms
                pnlMarker.Visible = false;
                ResetToDefaults();
            }
            else //if there are no open forms show message
            {
                RJMessageBox.Show("No child form is open", "Message");
            }

        }
        private void Screenshot()
        {//Take form screenshot

            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);//Default save directory (Pictures)
            var saveScreenshot = new SaveFileDialog();//Instance SaveFileDialog
            //Filter image files
            saveScreenshot.Filter = "PNG Image|*.png|JPG Image|*.jpg|BMP IMAGE|*.bmp" +
                                    "|Image Files|*.png;*.jpg;*.BMP;*.gif " +
                                    "|All Files|*.*";
            saveScreenshot.DefaultExt = "png";//Set default extension.
            saveScreenshot.AddExtension = true;
            saveScreenshot.FileName = "RJCode #" + Path.GetRandomFileName();// Suggest a random name.
            saveScreenshot.InitialDirectory = initialDirectory;// Indicate the default directory/Initial.



            // Take the screenshot from the upper left corner to the right bottom corner
            Bitmap bmpScreenshot = new Bitmap(this.Size.Width, this.Size.Height, PixelFormat.Format32bppArgb); //Set the bitmap object to the size of the main form.
            Graphics graphic = Graphics.FromImage(bmpScreenshot);//Create a graphic object from the bitmap.
            graphic.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, this.Size);//Take the screenshot only the region of the main form (this.size).

            if (saveScreenshot.ShowDialog() == DialogResult.OK)//If user clicks save button.
            {
                bmpScreenshot.Save(saveScreenshot.FileName);//Save the screenshot to the path and filename chosen by the user.
                Process.Start(saveScreenshot.FileName);//Display the screenshot in the default image viewer application of the operating system.
            }
        }
        private void PrintCurrentChildForm()
        {//Print Current Child Form

            //Desktop panel screenshot
            Bitmap bmpScreenshot = new Bitmap(pnlDesktop.Width, pnlDesktop.Height, PixelFormat.Format32bppArgb);//Set the bitmap object to the size of the desktop panel.
            var screenPoint = this.PointToScreen(pnlDesktop.Location);//Computes the location of the screen point into desktop panel coordinates.
            Graphics graphic = Graphics.FromImage(bmpScreenshot);//Create a graphic object from the bitmap.
            graphic.CopyFromScreen(screenPoint.X, screenPoint.Y, 0, 0, pnlDesktop.Size);//Take the screenshot only the region of the  desktop panel.

            var printForm = new RJPrintForm(bmpScreenshot, activeChildForm.Caption);//Open the print form and send the screenshot and the form title.
            printForm.ShowDialog();//Show as modal window.
        }

        #endregion

        #region -> Overrides
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ApplyAppearanceSettings();
            pnlMarker.BringToFront();//Set marker to foreground
            //Control Box location
            btnClose.Location = new Point(this.pnlTitleBar.Width - btnClose.Width, 0);
            btnMaximize.Location = new Point(btnClose.Location.X - btnMaximize.Width, 0);
            btnMinimize.Location = new Point(btnMaximize.Location.X - btnMinimize.Width, 0);
        }
        #endregion

        #region -> Event Methods

        private void SideMenuButton_Click(object sender, EventArgs e)
        {

            if (this.pnlSide.Width == 220)//Collapse side menu
            {
                this.pnlSide.Width = 65;
                foreach (Control control in this.pnlSideMenuHeader.Controls)//Hide any header control except the side menu button 
                {
                    if (control != biSideMenuButton)
                    {
                        control.Visible = false;
                    }
                }

            }
            else //Expand side menu
            {
                this.pnlSide.Width = 220;
                foreach (Control control in this.pnlSideMenuHeader.Controls)//Display any header control 
                {
                    if (control != biSideMenuButton)
                    {
                        control.Visible = true;
                    }
                }
            }
        }
        private void FormIcon_Click(object sender, EventArgs e)
        {
            CloseActiveChildForm();//Close current child form of panel desktop
            UpdateFormLayout();//Refresh Form
        }
        private void FormIcon_MouseHover(object sender, EventArgs e)
        {//when the user hovers the mouse over the form icon change the icon to close

            if (this.activeChildForm != null)
            {
                this.biFormIcon.IconChar = IconChar.TimesCircle;//Close icon
            }
        }
        private void FormIcon_MouseLeave(object sender, EventArgs e)
        {//when the mouse pointer leaves the form icon re-set the current form icon

            if (this.activeChildForm != null)
            {
                this.biFormIcon.IconChar = activeChildForm.FormIcon;
            }
        }

        private void CloseAllChildForms_Click(object sender, EventArgs e)
        {
            CloseAllChildForm();//Close all secondary forms 
        }
        private void MoveNewWindow_Click(object sender, EventArgs e)
        {
            MoveFormNewWindow();//Move child form to new window          
        }
        private void ChilFormHelp_Click(object sender, EventArgs e)
        {//Show child form help message

            if (activeChildForm.HelpMessage == "" || activeChildForm.HelpMessage == null)//No message
                RJMessageBox.Show("No help message has been added for this form", "Message");
            else//Show help message
                RJMessageBox.Show(activeChildForm.HelpMessage, "Help");
        }
        private void Screenshot_Click(object sender, EventArgs e)
        {
            Screenshot();
        }
        private void Print_Click(object sender, EventArgs e)
        {
            PrintCurrentChildForm();
        }

        private void Form_Deactivated(object sender, EventArgs e)
        {//When the form goes into deactivated mode (loses focus) change the color of the title bar. 
            if (deactivateFormEvent == true)
            {
                this.BorderColor = RJColors.DefaultFormBorderColor;//Set border color
                if (UIAppearance.Style != UIStyle.Supernova)//If the style is not supernova, change the title bar color
                {

                    pnlTitleBar.BackColor = UIAppearance.DeactiveFormColor;//Set title bar backcolor
                    pnlSideMenuHeader.BackColor = ColorEditor.Darken(UIAppearance.DeactiveFormColor, 6);//Set Side menu header backcolor               
                    pnlTitleBar.Update();//Force draw the title bar to avoid flickering when the background color is changed.                    

                }
            }
        }
        private void Form_Activated(object sender, EventArgs e)
        {//When the form enters activated mode (regains focus - form is redisplayed), 
            //Set the title bar back to normal color.

            this.BorderColor = UIAppearance.FormBorderColor;//Set border color
            if (UIAppearance.Style != UIStyle.Supernova)
            {
                pnlTitleBar.BackColor = UIAppearance.PrimaryStyleColor;//Set title bar backcolor
                pnlSideMenuHeader.BackColor = ColorEditor.Darken(UIAppearance.PrimaryStyleColor, 6);//Set Side menu header backcolor               
                pnlTitleBar.Update();    //Force draw the title bar to avoid flickering when the background color is changed.            
            }

        }

        private void ToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {//draw the code provided in the properties
            e.DrawBackground();//BackColor
            e.DrawText();//Text
            //e.DrawBorder();//Border
        }
        #endregion
    }
}