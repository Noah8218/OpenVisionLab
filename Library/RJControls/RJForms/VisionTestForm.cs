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
using OpenVisionLab;
using OpenVisionLab._1._Core;
using Lib.Common;
using Lib.OpenCV;
using OpenCvSharp;
using System.Reflection;
using System.Diagnostics;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using OpenVisionLab._2._Common;
using OpenVisionLab.PropertyGrid;

namespace RJCodeUI_M1.RJForms
{
    public class VisionTestForm : RJBaseForm
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected readonly LayerViewerState source_1 = new LayerViewerState();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected readonly LayerViewerState source_2 = new LayerViewerState();
        protected IDisplayManager displayManager;
        private readonly PropertyGridEventBinder wpgEvent;

        #region Event Register        
        protected EventHandler<DockDisplayEventArgs> eventUpdateDisplay;
        #endregion

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected readonly LayerViewerState destination = new LayerViewerState();

        protected int source1_Index = 0;
        protected int source2_Index = 0;
        protected int destination_Index = 0;

        protected System.Windows.Forms.Integration.ElementHost host = null;
        protected IPropertyGridView wpg = null;

        public int GetDisplayIndex(string strTitle)
        {
            return displayManager.FindIndex(strTitle);
        }

        protected Bitmap GetLayerImage(string title)
        {
            return displayManager.ImageSpace.GetImage(title);
        }

        protected Bitmap GetLayerImage(int index)
        {
            return displayManager.ImageSpace.GetImage(index);
        }

        protected Rectangle GetLayerRoi(int index)
        {
            return displayManager.ImageSpace.GetRoi(index);
        }

        protected Rectangle GetLayerTrainRoi(int index)
        {
            return displayManager.ImageSpace.GetTrainRoi(index);
        }

        protected void SetLayerImage(int index, Bitmap image)
        {
            displayManager.SetLayerImage(index, image);
        }

        protected void InitializeLayerList(RJComboBox comboBox, int selectedIndex)
        {
            comboBox.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                comboBox.Items.Add(displayManager.GetLayerTitle(i));
            }

            if (comboBox.Items.Count <= 0) { return; }
            comboBox.SelectedIndex = Math.Max(0, Math.Min(selectedIndex, comboBox.Items.Count - 1));
        }

        protected void InitializeSingleInputLayerList(RJComboBox sourceComboBox, RJComboBox destinationComboBox)
        {
            InitializeLayerList(sourceComboBox, source1_Index);
            InitializeLayerList(destinationComboBox, destination_Index);
        }

        protected bool RegisterEscapeClose()
        {
                        KeyPreview = true;
            KeyDown += CloseOnEscape;
            return true;
        
        }

        protected void ApplyVisionTestCompactStyle()
        {
            BackColor = Color.FromArgb(238, 242, 246);
            pnlClientArea.BackColor = Color.FromArgb(238, 242, 246);
            ApplyVisionTestCompactStyle(this);
        }

        private void ApplyVisionTestCompactStyle(Control parent)
        {
            if (parent == null) { return; }

            foreach (Control control in parent.Controls)
            {
                if (control is GroupBox groupBox)
                {
                    groupBox.BackColor = Color.FromArgb(238, 242, 246);
                    groupBox.ForeColor = Color.FromArgb(36, 48, 64);
                    groupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);

                    if (string.Equals(groupBox.Name, "groupBox3", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(groupBox.Text, "Input A", StringComparison.OrdinalIgnoreCase))
                    {
                        groupBox.Text = "Input Layer";
                    }
                    else if (string.Equals(groupBox.Name, "groupBox4", StringComparison.OrdinalIgnoreCase))
                    {
                        groupBox.Text = "Output Layer";
                    }
                }
                else if (control is RJPanel panel)
                {
                    panel.BackColor = Color.FromArgb(250, 252, 253);
                    panel.BorderRadius = 3;
                }
                else if (control is RJComboBox comboBox)
                {
                    comboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
                    comboBox.MinimumSize = new System.Drawing.Size(100, 28);
                    comboBox.BackColor = Color.FromArgb(250, 252, 253);
                    comboBox.ForeColor = Color.FromArgb(36, 48, 64);
                    comboBox.BorderColor = Color.FromArgb(148, 161, 178);
                    comboBox.DropDownTextColor = Color.FromArgb(36, 48, 64);
                    comboBox.IconColor = Color.FromArgb(47, 111, 171);
                    if (comboBox.Height > 32)
                    {
                        comboBox.Height = 32;
                    }
                }
                else if (control is RJButton button)
                {
                    button.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
                    button.Design = ButtonDesign.Custom;
                    button.Style = ControlStyle.Glass;
                    button.BorderRadius = 3;
                    button.BorderSize = 1;
                    button.BorderColor = Color.FromArgb(47, 111, 171);
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
                    button.BackColor = Color.FromArgb(250, 252, 253);
                    button.ForeColor = Color.FromArgb(35, 85, 132);

                    if (string.Equals(button.Name, "btnRun", StringComparison.OrdinalIgnoreCase)
                        && string.Equals(button.Text, "Run", StringComparison.OrdinalIgnoreCase))
                    {
                        button.Text = "Run";
                    }
                    else if (string.Equals(button.Name, "btnResult", StringComparison.OrdinalIgnoreCase))
                    {
                        button.Text = "Details";
                    }
                }

                ApplyVisionTestCompactStyle(control);
            }
        }

        protected void ActivateViewerLayer(int index, bool zoomToFit = false)
        {
            displayManager.ActivateLayer(index);
            Focus();
            TopLevel = true;
            TopMost = true;

            if (zoomToFit)
            {
                displayManager.ZoomLayerToFit(index);
            }
        }

        protected void ActivateSourceLayer(bool zoomToFit = false)
        {
            ActivateViewerLayer(source1_Index, zoomToFit);
        }

        protected void ActivateDestinationLayer(bool zoomToFit = false)
        {
            ActivateViewerLayer(destination_Index, zoomToFit);
        }

        protected void AcceptUserImageChange(RJComboBox comboBox, VisionTestImageCanvas viewer, Action<int> setIndex)
        {
            if (comboBox.SelectedIndex < 0 || viewer.DisplayBitmap == null) { return; }

            int index = comboBox.SelectedIndex;
            setIndex(index);
            SetLayerImage(index, viewer.DisplayBitmap);
        }

        protected void AcceptSourceImageChange(RJComboBox sourceComboBox, VisionTestImageCanvas sourceViewer)
        {
            AcceptUserImageChange(sourceComboBox, sourceViewer, index => source1_Index = index);
        }

        protected void AcceptDestinationImageChange(RJComboBox destinationComboBox, VisionTestImageCanvas destinationViewer)
        {
            AcceptUserImageChange(destinationComboBox, destinationViewer, index => destination_Index = index);
        }

        protected void SelectLayer(RJComboBox comboBox, LayerViewerState viewerState, VisionTestImageCanvas viewer, Action<int> setIndex, bool updateActiveImage = false)
        {
            if (comboBox.SelectedIndex < 0) { return; }

            int index = comboBox.SelectedIndex;
            setIndex(index);
            viewerState.Roi = GetLayerRoi(index);
            viewerState.TrainROI = GetLayerTrainRoi(index);

            Bitmap image = GetLayerImage(index);
            viewer.DisplayImage = image;

            if (updateActiveImage && image != null)
            {
                displayManager.SetImageSrc(BitmapImageConverter.ToMat(image));
            }
        }

        protected void SelectSourceLayer(RJComboBox sourceComboBox, VisionTestImageCanvas sourceViewer, bool updateActiveImage = false)
        {
            SelectLayer(sourceComboBox, source_1, sourceViewer, index => source1_Index = index, updateActiveImage);
        }

        protected void SelectDestinationLayer(RJComboBox destinationComboBox, VisionTestImageCanvas destinationViewer)
        {
            SelectLayer(destinationComboBox, destination, destinationViewer, index => destination_Index = index);
        }

        protected void CreateDestinationLayer(RJComboBox destinationComboBox, Action refreshLayerList, Action<int> setDestinationIndex)
        {
            displayManager.CreatePanel();
            refreshLayerList();

            int index = destinationComboBox.Items.Count - 1;
            if (index < 0) { return; }

            setDestinationIndex(index);
            destinationComboBox.SelectedIndex = index;
        }

        protected void CreateSingleInputDestinationLayer(RJComboBox destinationComboBox, Action refreshLayerList)
        {
            CreateDestinationLayer(destinationComboBox, refreshLayerList, index => destination_Index = index);
        }

        protected void PublishResult(RJComboBox destinationComboBox, VisionTestImageCanvas destinationViewer, Bitmap result, string elapsedText)
        {
            if (destinationComboBox?.SelectedItem == null || result == null) { return; }

            int displayIndex = GetDisplayIndex(destinationComboBox.SelectedItem.ToString());
            SetLayerImage(displayIndex, result);
            destinationViewer.DisplayImage = result;
            eventUpdateDisplay?.Invoke(null, new DockDisplayEventArgs(result, displayIndex, elapsedText));
        }

        protected Mat CreateRunSourceMat(VisionTestImageCanvas sourceViewer, out Bitmap resultBitmap)
        {
            if (sourceViewer?.DisplayBitmap == null)
            {
                throw new InvalidOperationException("Source image is not loaded.");
            }

            Mat sourceMat = BitmapImageConverter.ToMat(sourceViewer.DisplayBitmap).Clone();
            resultBitmap = BitmapDrawing.GetBitmapFormat24bppRgb(sourceViewer.DisplayBitmap);
            OpenCvHelper.SetImageChannel1(sourceMat);

            return sourceMat;
        }

        protected Bitmap CreateSingleInputResult(VisionTestImageCanvas sourceViewer, Action<Mat> processImage)
        {
            if (processImage == null)
            {
                throw new ArgumentNullException(nameof(processImage));
            }

            using (Mat sourceMat = CreateRunSourceMat(sourceViewer, out Bitmap initialResult))
            {
                initialResult.Dispose();
                return CreateSingleInputResult(sourceMat, processImage);
            }
        }

        protected Bitmap CreateSingleInputResult(VisionTestImageCanvas sourceViewer, Action<Mat, bool> processImage)
        {
            if (processImage == null)
            {
                throw new ArgumentNullException(nameof(processImage));
            }

            using (Mat sourceMat = CreateRunSourceMat(sourceViewer, out Bitmap initialResult))
            {
                initialResult.Dispose();
                return CreateSingleInputResult(sourceMat, processImage);
            }
        }

        protected Bitmap CreateSingleInputResult(Mat sourceMat, Action<Mat> processImage)
        {
            if (processImage == null)
            {
                throw new ArgumentNullException(nameof(processImage));
            }

            return CreateSingleInputResult(sourceMat, (image, isRoi) => processImage(image));
        }

        protected Bitmap CreateSingleInputResult(Mat sourceMat, Action<Mat, bool> processImage)
        {
            if (sourceMat == null)
            {
                throw new ArgumentNullException(nameof(sourceMat));
            }

            if (processImage == null)
            {
                throw new ArgumentNullException(nameof(processImage));
            }

            if (displayManager.IsLayerRoiEmpty(source1_Index))
            {
                processImage(sourceMat, false);
                return BitmapImageConverter.ToBitmap(sourceMat);
            }

            Rect roiRect = CommonConverter.RectangleToRect(GetLayerRoi(source1_Index));
            using (Mat imageRoi = sourceMat.SubMat(roiRect))
            {
                processImage(imageRoi, true);
                using (Bitmap sourceBitmap = BitmapImageConverter.ToBitmap(sourceMat))
                using (Bitmap roiBitmap = BitmapImageConverter.ToBitmap(imageRoi))
                {
                    return BitmapProcessing.OverlayImage(sourceBitmap, roiBitmap, roiRect.Left, roiRect.Top);
                }
            }
        }

        protected void InitializeSingleInputViewers(
            Action initializeLayerList,
            VisionTestImageCanvas sourceViewer,
            VisionTestImageCanvas destinationViewer,
            EventHandler sourceImageChanged,
            EventHandler destinationImageChanged,
            MouseEventHandler sourceMouseClick,
            MouseEventHandler destinationMouseClick,
            ToolTip toolTip = null,
            Control destinationNewPanelControl = null)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            RegisterEscapeClose();
            ApplyVisionTestCompactStyle();
            initializeLayerList();

            sourceViewer.UserImageChanged += sourceImageChanged;
            destinationViewer.UserImageChanged += destinationImageChanged;
            sourceViewer.MouseClick += sourceMouseClick;
            destinationViewer.MouseClick += destinationMouseClick;

            if (toolTip != null && destinationNewPanelControl != null)
            {
                toolTip.SetToolTip(destinationNewPanelControl, "Create Output Layer");
            }

            stopwatch.Stop();

            DeferInitialViewerLoad(() =>
            {
                sourceViewer.DisplayImage = GetLayerImage(DEFINE.Main);
                destinationViewer.DisplayImage = GetLayerImage(DEFINE.Main);
                sourceViewer.ZoomToFit();
                destinationViewer.ZoomToFit();
            }, "single input viewer image load");
        }

        protected void DeferInitialViewerLoad(Action action, string operationName)
        {
            if (action == null || IsDesignTime()) { return; }

            Action wrappedAction = () =>
            {
                if (IsDisposed) { return; }

                Stopwatch stopwatch = Stopwatch.StartNew();
                action();
                stopwatch.Stop();
            };

            if (IsHandleCreated)
            {
                BeginInvoke(wrappedAction);
                return;
            }

            EventHandler shownHandler = null;
            shownHandler = (sender, e) =>
            {
                Shown -= shownHandler;
                BeginInvoke(wrappedAction);
            };
            Shown += shownHandler;
        }

        private void CloseOnEscape(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != (int)Keys.Escape) { return; }

            DialogResult = DialogResult.Cancel;
            Close();
        }

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
        public VisionTestForm()
            : this(DisplayManagerService.Default)
        {
        }

        public VisionTestForm(IDisplayManager displayManager)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.displayManager = displayManager ?? DisplayManagerService.Default;
            wpgEvent = new PropertyGridEventBinder(() => this.displayManager);
            InitializeItems();
            stopwatch.Stop();
        }

        private void InitializePropertyGridHost()
        {
            if (host != null && wpg != null)
            {
                return;
            }

            host = new System.Windows.Forms.Integration.ElementHost { Dock = DockStyle.Fill };
            var propertyGrid = new System.Windows.Controls.WpfPropertyGrid.PropertyGrid
            {
                Layout = new System.Windows.Controls.WpfPropertyGrid.Design.CategorizedLayout()
            };
            host.Child = propertyGrid;
            wpg = propertyGrid;
            wpg.PropertyValueChanged += wpgEvent.Wpg_PropertyValueChanged;
            wpg.SelectedObjectsChanged += wpgEvent.Wpg_SelectedObjectsChanged;
        }

        protected Control EnsurePropertyGridHost()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            InitializePropertyGridHost();
            stopwatch.Stop();
            return host;
        }

        protected void AttachPropertyGrid(Control targetPanel, object selectedObject)
        {
            if (IsDesignTime() || targetPanel == null) { return; }

            Control propertyGridHost = EnsurePropertyGridHost();
            wpg.SelectedObject = selectedObject;

            if (propertyGridHost.Parent != targetPanel)
            {
                propertyGridHost.Parent?.Controls.Remove(propertyGridHost);
                targetPanel.Controls.Add(propertyGridHost);
            }
        }

        private static bool IsDesignTime()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return true;
            }

            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            return processName.IndexOf("devenv", StringComparison.OrdinalIgnoreCase) >= 0
                || processName.IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void SetDisplayManager(IDisplayManager manager)
        {
            displayManager = manager ?? DisplayManagerService.Default;
            source_1.SetDisplayManager(displayManager);
            source_2.SetDisplayManager(displayManager);
            destination.SetDisplayManager(displayManager);
            PropertyGridEditorFactory.SetRuntimeContext(() => displayManager);
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
            pnlTitleBar.Location = new System.Drawing.Point(0, 0);
            pnlTitleBar.Dock = DockStyle.Top;
            pnlTitleBar.Size = new System.Drawing.Size(960, 40);
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
            btnFormIcon.Location = new System.Drawing.Point(0, 0);
            btnFormIcon.Size = new System.Drawing.Size(40, 40);
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
            lblCaption.Location = new System.Drawing.Point(40, 10);
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
            pnlClientArea.Location = new System.Drawing.Point(0, 40);
            pnlClientArea.Name = "pnlClientArea";
            pnlClientArea.Size = new System.Drawing.Size(960, 485);
            pnlClientArea.AutoScroll = true;
            #endregion

            #region -RJ Child Form Properties
            //
            // RJChildForm          
            //
            this.Name = "VisionTestForm";
            this.Text = "RJ Child form";
            this.Controls.Add(pnlClientArea);
            this.Controls.Add(pnlTitleBar);
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.None;//Disable autoscale mode, to keep the form size set in the DefaultSize property
            this.FormBorderStyle = FormBorderStyle.None;//Borderless form    
            this.MinimumSize = new System.Drawing.Size(400, 180);//Minimun form size 
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
                    this.Size = new System.Drawing.Size(960, 560);
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
        public new System.Drawing.Size ClientSize
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
        protected override System.Drawing.Size DefaultSize
        {//Form default size
            get { return new System.Drawing.Size(960, 560); }
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
                btnFormIcon.BackColor = RJCodeUI_M1.Utils.ColorEditor.Darken(btnFormIcon.BackColor, 15);
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
                    this.Size = new System.Drawing.Size(960, this.Size.Height);
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

