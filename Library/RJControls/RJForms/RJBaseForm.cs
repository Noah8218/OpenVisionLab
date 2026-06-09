using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using RJCodeUI_M1.RJControls;
using RJCodeUI_M1.Settings;
using System.ComponentModel;

namespace RJCodeUI_M1.RJForms
{
    public class RJBaseForm : Form
    {
        /// This class inherits from the <see cref="Form"/> class of the <see cref="System.Windows.Forms"/> library
        /// 
        ///<summary>
        ///     A borderless form loses the functionality of resizing, aereo snap, maximize-restore and minimizing from the taskbar,
        ///     so in this class these functionalities will be re-implemented by overriding
        ///     the processing Windows messages, creation parameters methods at the operating system level
        ///     and adding the necessary buttons and methods.
        ///</summary>

        #region -> Fields

        /// Fields
        private IContainer components = null; //Container for components that are not child of the form. 
        ///Allows all added components to be removed with the Dispose method by the components container
        ///<see cref="protected override void Dispose(bool disposing)"/>

        private bool isPrimaryForm; //Gets or sets if it is a primary form: Is a base form for all application forms (e.g Login form and main form)      
        private bool isResizable = true;//Gets or sets if form is resizable from the border of the form
        private bool minimizeButton = true; // Gets or sets if the minimize button is shown.
        private bool maximizeButton = true; // Gets or sets if the maximize button is shown.
        private bool isMaximized;//Gets or setss if the form is maximized: Switch to stop code execution cycles in the resize event       
        private bool snapWindow;//Gets or sets if winsnap of the form is activated(This is a simple imitation of the window aero snap function)
        private Size tempSize;//Gets or sets the temporary size of the form to restore size after having activated WinSnap       
        private Point tempLocation;//Gets or sets the temporary Location of the form to restore size after having activated WinSnap  
        private int borderSize;//Gets or sets form Border Width
        private Color borderColor;//Gets or sets form borde color       
        //Private Controls
        private Timer timerFadeIn;//Used to display the form gradually-> Fade In Effect

        /// Constant             
        private const int resizeAreaSize = 10; //Determines the size of the resize area of ​​the form
        private const int WS_MINIMIZEBOX = 0x20000;//Native Methods: Represents a window style that has a minimize button
        private const int WM_NCHITTEST = 0x0084;//Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.

        #endregion

        #region -> Constructor

        /// Constructor
        public RJBaseForm()
        {
            /*Initialize the components for the form design*/
            components = new System.ComponentModel.Container();//initialize container

            btnMinimize = new Button();
            btnMaximize = new Button();
            btnClose = new Button();
            miSnapLeft = new FontAwesome.Sharp.IconMenuItem();
            miSnapRight = new FontAwesome.Sharp.IconMenuItem();
            miExitSnap = new FontAwesome.Sharp.IconMenuItem();
            timerFadeIn = new Timer(components);//Add to component container

            #region -Control Box Buttons
            // 
            // Button: Close
            //           
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Image = CreateCloseImage(Color.White);
            btnClose.Location = new Point(175, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(35, 40);
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += new System.EventHandler(Close_Click);
            // 
            // Button: Maximize
            // 
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatStyle = FlatStyle.Flat;
            btnMaximize.Image = CreateMaximizeImage(Color.White);
            btnMaximize.Location = new Point(140, 0);
            btnMaximize.Name = "btnMaximize";
            btnMaximize.Size = new Size(35, 40);
            btnMaximize.UseVisualStyleBackColor = true;
            btnMaximize.Click += new System.EventHandler(MaximizeRestore_Click);
            // 
            // Button: Minimize
            // 
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.Image = CreateMinimizeImage(Color.White);
            btnMinimize.Location = new Point(105, 0);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(35, 40);
            btnMinimize.UseVisualStyleBackColor = true;
            btnMinimize.Click += new System.EventHandler(Minimize_Click);
            #endregion

            #region -Snap Window
            // 
            // MenuItem: Window Snap Left (FontAwesome.Sharp library)
            //            
            miSnapLeft.Name = "miSnapLeft";
            miSnapLeft.Text = "Snap Window Left";
            miSnapLeft.IconSize = 21;
            miSnapLeft.IconChar = IconChar.SignInAlt;
            miSnapLeft.Rotation = -180;
            miSnapLeft.IconColor = RJColors.FantasyColorScheme1;
            miSnapLeft.Click += new System.EventHandler(SnapWindowLeft_Click);
            // 
            // MenuItem: Window Snap Right (FontAwesome.Sharp library)
            // 
            miSnapRight.Name = "miSnapRight";
            miSnapRight.Text = "Snap Window Right";
            miSnapRight.IconSize = 21;
            miSnapRight.IconChar = IconChar.SignInAlt;
            miSnapRight.IconColor = RJColors.FantasyColorScheme2;
            miSnapRight.Click += new System.EventHandler(SnapWindowRight_Click);
            // 
            // MenuItem: Exit Window Snap (FontAwesome.Sharp library)
            // 
            miExitSnap.Name = "miExitSnap";
            miExitSnap.Text = "Exit Snap Window";
            miExitSnap.IconSize = 21;
            miExitSnap.IconChar = IconChar.CropAlt;
            miExitSnap.IconColor = RJColors.FantasyColorScheme3;
            miExitSnap.Click += new System.EventHandler(ExitSnapWindow_Click);
            #endregion

            #region -Others
            //Timer
            timerFadeIn.Interval = 1;
            timerFadeIn.Tick += new System.EventHandler(this.timerFadeIn_Tick);
            #endregion
            //
            //RJBaseForm
            //           
            this.Font = new Font("Microsoft Sans Serif", UIAppearance.TextSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));//Default font                      
            this.Resize += new EventHandler(Form_Resize);//Event when the form is resized

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);

        }

        #endregion

        #region -> Private icon helpers

        private static Image CreateCloseImage(Color color)
        {
            Bitmap bitmap = new Bitmap(14, 14);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(color, 2))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.DrawLine(pen, 3, 3, 11, 11);
                graphics.DrawLine(pen, 11, 3, 3, 11);
            }

            return bitmap;
        }

        private static Image CreateMaximizeImage(Color color)
        {
            Bitmap bitmap = new Bitmap(14, 14);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(color, 2))
            {
                graphics.DrawRectangle(pen, 3, 3, 8, 8);
            }

            return bitmap;
        }

        private static Image CreateMinimizeImage(Color color)
        {
            Bitmap bitmap = new Bitmap(14, 14);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(color, 2))
            {
                graphics.DrawLine(pen, 3, 10, 11, 10);
            }

            return bitmap;
        }

        #endregion

        #region -> Properties
        /// Controls       
        protected Button btnMinimize { get; private set; }//Minimize button
        protected Button btnMaximize { get; private set; }//Maximize button
        protected Button btnClose { get; private set; }//Close button        
        protected IconMenuItem miSnapLeft { get; private set; }///Dock window Left   [ Note:> ICON MENU ITEM is provided by <see cref="FontAwesome.Sharp"/> library]
        protected IconMenuItem miSnapRight { get; private set; }//Dock window Right  [ Autor: mkoertgen, GitHub: https://github.com/awesome-inc/FontAwesome.Sharp  ]   
        protected IconMenuItem miExitSnap { get; private set; }//Restore window to normal size  

        ///Value
        [Browsable(false)]
        protected bool PrimaryForm
        {//Gets or sets if it is a primary form: Is a base form for all application forms (e.g Login form and main form)    

            get { return isPrimaryForm; }
            set { isPrimaryForm = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets if form is resizable")]
        [DefaultValue(true)]
        public bool Resizable
        {//Gets or sets if form is resizable (See WndProc(ref Message message) method )
            get { return isResizable; }
            set { isResizable = value; }
        }

        /// Disign     
        [Category("RJ Code Advance")]
        [Description("Hide or show the minimize button")]
        [DefaultValue(true)]
        public bool DisplayMinimizeButton
        {
            get { return minimizeButton; }
            set
            {
                minimizeButton = value;
                btnMinimize.Visible = minimizeButton;
            }
        }

        [Category("RJ Code Advance")]
        [Description("Hide or show the maximize buttonr")]
        [DefaultValue(true)]
        public bool DisplayMaximizeButton
        {
            get { return maximizeButton; }
            set
            {
                maximizeButton = value;
                btnMaximize.Visible = maximizeButton;
            }
        }
        [Browsable(false)]
        public int BorderSize
        {//Gets or sets form Border Width

            get { return borderSize; }
            set
            {
                borderSize = value;
                this.Padding = new Padding(borderSize);//The form Border Width will be defined by the Padding property
            }
        }

        [Browsable(false)]
        public Color BorderColor
        {//Gets or sets form border color

            get { return borderColor; }
            set
            {
                borderColor = value;
                this.BackColor = value;//The form border color will be determined by the BackColor property
            }
        }
        [Browsable(false)]//Hide backcolor property
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
        protected override CreateParams CreateParams
        {//Override the property parameters form creation
            get
            {
                CreateParams param = base.CreateParams;
                param.Style |= WS_MINIMIZEBOX; //Sets a minimize box on the window style/ Will allow to minimize the form from the taskbar icon
                return param;
            }
        }

        #endregion

        #region -> Private methods

        private void FadeInEffect()
        {//Fade-In effect-> The form appears gradually
            //this.Opacity = 0.0;
            //timerFadeIn.Start();
        }
        private void ApplyAppearanceSettings()
        {
            if (UIAppearance.Style == UIStyle.Supernova)//if the style is supernova
            {
                btnClose.FlatAppearance.MouseOverBackColor = RJColors.FantasyColorScheme4;
                btnMaximize.FlatAppearance.MouseOverBackColor = RJColors.FantasyColorScheme1;
                btnMinimize.FlatAppearance.MouseOverBackColor = RJColors.Sky;
                if (isPrimaryForm)
                {
                    if (UIAppearance.Theme == UITheme.Light)//if the theme is LIGHT, set the maximize, minimize and close buttons to black.
                    {
                    }
                }
            }
        }

        #endregion

        #region -> Protected methods

        protected virtual void CloseWindow()
        {
            if (isPrimaryForm)//If it is a primary form (e.g MainForm or Login Form) Completely close the application.
            {
                //var result = RJMessageBox.Show("Are you sure to close the application?",
                //    "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                //if (result == System.Windows.Forms.DialogResult.Yes)
                //    Application.Exit();//Terminates all message loops and closes all windows
            }
            else//If it is a child form
                this.Close();//Close current form
        }
        protected void MinimizeWindow()
        {//minimize form
            this.WindowState = FormWindowState.Minimized;
        }
        protected void MaximizeWindow()
        {//Maximize or restore the form to normal size

            if (this.WindowState == FormWindowState.Normal)//Maximize the form
            {
                ///<Note>:When maximizing a borderless form, it covers the entire screen hiding the taskbar,
                ///for this you must specify a maximum size:</Note>               
                this.MaximumSize = Screen.FromHandle(this.Handle).WorkingArea.Size;//Sets the size of the desktop area as the maximum size of the form
                this.WindowState = FormWindowState.Maximized;
                BorderSize = 0;//Remove border in maximized status
                isMaximized = true;//Set maximized status 

                if (snapWindow == true)//If windows snap is activated, we set its normal size
                {
                    this.Size = tempSize;
                    snapWindow = false;
                }
            }
            else//Restore the form
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
        protected void SnapWindowLeft()
        {/*Dock the form to the left of the desktop: Widht = Half the width of the desktop area, Height = Height of the desktop area*/

            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;

            var screen = Screen.FromHandle(this.Handle);//Gets current system screen where the form is located
            var desktopArea = screen.WorkingArea.Size;//Gets the working area of the screen (Exclude the task bar)
            var newSize = new Size(desktopArea.Width / 2, desktopArea.Height);/*Sets the size of the form: 
                                                                                Width = half the width of the desktop area,
                                                                                Height = height of the desktop area*/
            if (this.Size != newSize)//save the current size of the form so that it can be reset when quit snap mode
                tempSize = this.Size;
            if (snapWindow == false)
                tempLocation = this.Location;

            this.Size = newSize;//Sets the new size of the form
            this.Location = new Point(screen.Bounds.X, screen.Bounds.Y);/*Sets form location (X and Y coordinates of the screen boundaries)
                                                                          Dock the form to the LEFT of the desktop*/
            snapWindow = true;//Sets snap state
        }
        protected void SnapWindowRight()
        {/*Dock the form to the right of the desktop: Widht= Half the width of the desktop area, Height= Height of the desktop area*/

            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;

            var screen = Screen.FromHandle(this.Handle);
            var desktopArea = screen.WorkingArea.Size;
            var newSize = new Size(desktopArea.Width / 2, desktopArea.Height);

            if (this.Size != newSize)
                tempSize = this.Size;
            if (snapWindow == false)
                tempLocation = this.Location;

            this.Size = newSize;
            this.Location = new Point(
                screen.Bounds.X + desktopArea.Width / 2, screen.Bounds.Y);/*Sets form location (X Coordinate=midpoint of desk area)
                                                                            Dock the form to the RIGHT of the desktop*/
            snapWindow = true;
        }
        protected void ExitSnapWindow()
        {//Restores the size of the form after docking the window

            if (snapWindow == true)
            {
                this.Size = tempSize;
                this.Location = tempLocation;
                snapWindow = false;
            }
        }
        #endregion

        #region -> Overrides

        protected override void WndProc(ref Message message)
        {//WindowProc function: Overriding processing Windows messages/OS level

            // Resize/WM_NCHITTEST values
            const int HTCLIENT = 1; //Represents the client area of the window
            const int HTLEFT = 10;  //Left border of a window, allows resize horizontally to the left
            const int HTRIGHT = 11; //Right border of a window, allows resize horizontally to the right
            const int HTTOP = 12;   //Upper-horizontal border of a window, allows resize vertically up
            const int HTTOPLEFT = 13;//Upper-left corner of a window border, allows resize diagonally to the left
            const int HTTOPRIGHT = 14;//Upper-right corner of a window border, allows resize diagonally to the right
            const int HTBOTTOM = 15; //Lower-horizontal border of a window, allows resize vertically down
            const int HTBOTTOMLEFT = 16;//Lower-left corner of a window border, allows resize diagonally to the left
            const int HTBOTTOMRIGHT = 17;//Lower-right corner of a window border, allows resize diagonally to the right
            ///<Doc> More Information: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest </Doc>

            if (isResizable) /*Allow the resizing of the form as long as it is resizable from the border*/
            {
                switch (message.Msg)
                {
                    case WM_NCHITTEST: //If the windows message is WM_NCHITTEST
                        base.WndProc(ref message);
                        if (this.WindowState == FormWindowState.Normal)//Resize the form if it is in normal state
                        {
                            if ((int)message.Result == HTCLIENT)//If the result of the message (mouse pointer) is in the client area of the window
                            {
                                Point screenPoint = new Point(message.LParam.ToInt32()); //Gets screen point coordinates(X and Y coordinate of the pointer)                           
                                Point clientPoint = this.PointToClient(screenPoint); //Computes the location of the screen point into client coordinates                          

                                if (clientPoint.Y <= resizeAreaSize)//If the pointer is at the top of the form (within the resize area- X coordinate)
                                {
                                    if (clientPoint.X <= resizeAreaSize) //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in 
                                        message.Result = (IntPtr)HTTOPLEFT; //Resize diagonally to the left
                                    else if (clientPoint.X < (this.Size.Width - resizeAreaSize))//If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                                        message.Result = (IntPtr)HTTOP; //Resize vertically up
                                    else //Resize diagonally to the right
                                        message.Result = (IntPtr)HTTOPRIGHT;
                                }
                                else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                                {
                                    if (clientPoint.X <= resizeAreaSize)//Resize horizontally to the left
                                        message.Result = (IntPtr)HTLEFT;
                                    else if (clientPoint.X > (this.Width - resizeAreaSize))//Resize horizontally to the right
                                        message.Result = (IntPtr)HTRIGHT;
                                }
                                else
                                {
                                    if (clientPoint.X <= resizeAreaSize)//Resize diagonally to the left
                                        message.Result = (IntPtr)HTBOTTOMLEFT;
                                    else if (clientPoint.X < (this.Size.Width - resizeAreaSize)) //Resize vertically down
                                        message.Result = (IntPtr)HTBOTTOM;
                                    else //Resize diagonally to the right
                                        message.Result = (IntPtr)HTBOTTOMRIGHT;
                                }
                            }
                        }
                        return;
                }
            }
            base.WndProc(ref message);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyAppearanceSettings();//Apply settings

            if (this.TopLevel == true) //(ToLevel = false, it means that the form is a control, the opacity property has no effect)
            {
                FadeInEffect();//Display the form gradually
            }
        }
        #endregion

        #region -> Event Methodss

        private void timerFadeIn_Tick(object sender, EventArgs e)
        {//Timer -> Fade-in effect

            if (this.Opacity < 1) this.Opacity += 0.1;//Increase opacity by 0.01
            else timerFadeIn.Stop();//Stop the timer when the opacity of the form is 1.
            this.Invalidate();            
        }

        private void Close_Click(object sender, EventArgs e) { CloseWindow(); }
        private void Minimize_Click(object sender, EventArgs e) { MinimizeWindow(); }
        private void MaximizeRestore_Click(object sender, EventArgs e) { MaximizeWindow(); }
        private void SnapWindowLeft_Click(object sender, EventArgs e) { SnapWindowLeft(); }
        private void SnapWindowRight_Click(object sender, EventArgs e) { SnapWindowRight(); }
        private void ExitSnapWindow_Click(object sender, EventArgs e) { ExitSnapWindow(); }
        private void Form_Resize(object sender, EventArgs e)
        {
            //Sets Border Width of form and update the maximized button when the form is restored to its normal size and the isMaximized switch is active
            if (this.WindowState == FormWindowState.Normal && isMaximized == true)
            {
                isMaximized = false;
                BorderSize = UIAppearance.FormBorderSize;
            }
            ///<Note>If the variable isMaximized does not exist, 
            ///the above code will always be executed when the form changes size, 
            ///location or dragged point by point, that could slow down a bit. 
            ///So with the above condition the code snippet will only be executed once</Note> 
        }
        #endregion
    }
}

