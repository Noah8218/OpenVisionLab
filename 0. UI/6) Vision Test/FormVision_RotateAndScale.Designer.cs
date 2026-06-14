namespace OpenVisionLab
{
    partial class FormVision_RotateAndScale
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
                        this.ibSource = new OpenVisionLab.VisionTestImageCanvas();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new OpenVisionLab.VisionTestImageCanvas();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.transformPreviewTimer = new System.Windows.Forms.Timer(this.components);
            this.trbRotate = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
            this.tbRotate = new RJCodeUI_M1.RJControls.RJTextBox();
            this.trbScaleX = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabelScaleX = new RJCodeUI_M1.RJControls.RJLabel();
            this.tbScaleX = new RJCodeUI_M1.RJControls.RJTextBox();
            this.trbScaleY = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabelScaleY = new RJCodeUI_M1.RJControls.RJLabel();
            this.tbScaleY = new RJCodeUI_M1.RJControls.RJTextBox();
            this.rjButton1 = new RJCodeUI_M1.RJControls.RJButton();
            this.pnlClientArea.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbRotate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbScaleX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbScaleY)).BeginInit();
            this.SuspendLayout();            
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.rjButton1);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(918, 613);
            // 
            // 
            // 
            // ibSource
            // 
            this.ibSource.Location = new System.Drawing.Point(8, 20);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(374, 220);
            this.ibSource.TabIndex = 2149;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox3.Controls.Add(this.cbLayerList);
            this.groupBox3.Controls.Add(this.ibSource);
            this.groupBox3.Location = new System.Drawing.Point(16, 58);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(390, 285);
            this.groupBox3.TabIndex = 2154;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Input Layer";
            // 
            // cbLayerList
            // 
            this.cbLayerList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.BorderRadius = 3;
            this.cbLayerList.BorderSize = 2;
            this.cbLayerList.Customizable = false;
            this.cbLayerList.DataSource = null;
            this.cbLayerList.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbLayerList.Location = new System.Drawing.Point(8, 248);
            this.cbLayerList.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList.Name = "cbLayerList";
            this.cbLayerList.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList.SelectedIndex = -1;
            this.cbLayerList.Size = new System.Drawing.Size(374, 32);
            this.cbLayerList.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList.TabIndex = 2158;
            this.cbLayerList.Texts = "";
            this.cbLayerList.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox4.Controls.Add(this.cbLayerList2);
            this.groupBox4.Controls.Add(this.btnNewPanel_Desty);
            this.groupBox4.Controls.Add(this.ibDestination);
            this.groupBox4.Location = new System.Drawing.Point(16, 360);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(390, 285);
            this.groupBox4.TabIndex = 2155;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output Layer";
            // 
            // cbLayerList2
            // 
            this.cbLayerList2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.BorderRadius = 3;
            this.cbLayerList2.BorderSize = 2;
            this.cbLayerList2.Customizable = false;
            this.cbLayerList2.DataSource = null;
            this.cbLayerList2.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList2.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList2.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbLayerList2.Location = new System.Drawing.Point(8, 248);
            this.cbLayerList2.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList2.Name = "cbLayerList2";
            this.cbLayerList2.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList2.SelectedIndex = -1;
            this.cbLayerList2.Size = new System.Drawing.Size(338, 32);
            this.cbLayerList2.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList2.TabIndex = 2159;
            this.cbLayerList2.Texts = "";
            this.cbLayerList2.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList2_SelectedIndexChanged);
            // 
            // btnNewPanel_Desty
            // 
            this.btnNewPanel_Desty.BackColor = System.Drawing.Color.Transparent;
            this.btnNewPanel_Desty.BackIcon = true;
            this.btnNewPanel_Desty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPanel_Desty.Customizable = true;
            this.btnNewPanel_Desty.DropdownMenu = null;
            this.btnNewPanel_Desty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel_Desty.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            this.btnNewPanel_Desty.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel_Desty.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewPanel_Desty.IconSize = 30;
            this.btnNewPanel_Desty.Location = new System.Drawing.Point(354, 250);
            this.btnNewPanel_Desty.Name = "btnNewPanel_Desty";
            this.btnNewPanel_Desty.Size = new System.Drawing.Size(28, 28);
            this.btnNewPanel_Desty.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.btnNewPanel_Desty.TabIndex = 2157;
            this.btnNewPanel_Desty.TabStop = false;
            this.btnNewPanel_Desty.Click += new System.EventHandler(this.btnNewPanel_Desty_Click);
            // 
            // ibDestination
            // 
            this.ibDestination.Location = new System.Drawing.Point(8, 20);
            this.ibDestination.Name = "ibDestination";
            this.ibDestination.Size = new System.Drawing.Size(374, 220);
            this.ibDestination.TabIndex = 2149;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // transformPreviewTimer
            // 
            this.transformPreviewTimer.Interval = 80;
            this.transformPreviewTimer.Tick += new System.EventHandler(this.transformPreviewTimer_Tick);
            // 
            // 
            // trbRotate
            // 
            this.trbRotate.AutoSize = false;
            this.trbRotate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbRotate.ChannelColor = System.Drawing.Color.LightGray;
            this.trbRotate.Customizable = true;
            this.trbRotate.LargeChange = 1;
            this.trbRotate.Location = new System.Drawing.Point(423, 89);
            this.trbRotate.Maximum = 180;
            this.trbRotate.Minimum = -180;
            this.trbRotate.Name = "trbRotate";
            this.trbRotate.ShowValue = true;
            this.trbRotate.Size = new System.Drawing.Size(360, 46);
            this.trbRotate.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbRotate.TabIndex = 2156;
            this.trbRotate.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbRotate.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbRotate.Scroll += new System.EventHandler(this.trbRotate_Scroll);
            // 
            // rjLabel4
            // 
            this.rjLabel4.AutoSize = true;
            this.rjLabel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel4.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel4.LinkLabel = false;
            this.rjLabel4.Location = new System.Drawing.Point(423, 64);
            this.rjLabel4.Name = "rjLabel4";
            this.rjLabel4.Size = new System.Drawing.Size(51, 16);
            this.rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel4.TabIndex = 2157;
            this.rjLabel4.Text = "Rotate";
            // 
            // tbRotate
            // 
            this.tbRotate._Customizable = false;
            this.tbRotate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbRotate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbRotate.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(162)))), ((int)(((byte)(247)))));
            this.tbRotate.BorderRadius = 3;
            this.tbRotate.BorderSize = 2;
            this.tbRotate.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbRotate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbRotate.Location = new System.Drawing.Point(793, 89);
            this.tbRotate.MultiLine = false;
            this.tbRotate.Name = "tbRotate";
            this.tbRotate.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbRotate.PasswordChar = false;
            this.tbRotate.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbRotate.PlaceHolderText = null;
            this.tbRotate.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbRotate.Size = new System.Drawing.Size(118, 31);
            this.tbRotate.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteBorder;
            this.tbRotate.TabIndex = 0;
            // 
            // trbScaleX
            // 
            this.trbScaleX.AutoSize = false;
            this.trbScaleX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbScaleX.ChannelColor = System.Drawing.Color.LightGray;
            this.trbScaleX.Customizable = true;
            this.trbScaleX.LargeChange = 1;
            this.trbScaleX.Location = new System.Drawing.Point(423, 189);
            this.trbScaleX.Maximum = 300;
            this.trbScaleX.Minimum = 10;
            this.trbScaleX.Name = "trbScaleX";
            this.trbScaleX.ShowValue = true;
            this.trbScaleX.Size = new System.Drawing.Size(360, 46);
            this.trbScaleX.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbScaleX.TabIndex = 2158;
            this.trbScaleX.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbScaleX.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbScaleX.Value = 100;
            this.trbScaleX.Scroll += new System.EventHandler(this.trbScale_Scroll);
            // 
            // rjLabelScaleX
            // 
            this.rjLabelScaleX.AutoSize = true;
            this.rjLabelScaleX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabelScaleX.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabelScaleX.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabelScaleX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabelScaleX.LinkLabel = false;
            this.rjLabelScaleX.Location = new System.Drawing.Point(423, 164);
            this.rjLabelScaleX.Name = "rjLabelScaleX";
            this.rjLabelScaleX.Size = new System.Drawing.Size(74, 16);
            this.rjLabelScaleX.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabelScaleX.TabIndex = 2159;
            this.rjLabelScaleX.Text = "Scale X (%)";
            // 
            // tbScaleX
            // 
            this.tbScaleX._Customizable = false;
            this.tbScaleX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbScaleX.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbScaleX.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(162)))), ((int)(((byte)(247)))));
            this.tbScaleX.BorderRadius = 3;
            this.tbScaleX.BorderSize = 2;
            this.tbScaleX.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbScaleX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbScaleX.Location = new System.Drawing.Point(793, 189);
            this.tbScaleX.MultiLine = false;
            this.tbScaleX.Name = "tbScaleX";
            this.tbScaleX.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbScaleX.PasswordChar = false;
            this.tbScaleX.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbScaleX.PlaceHolderText = null;
            this.tbScaleX.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbScaleX.Size = new System.Drawing.Size(118, 31);
            this.tbScaleX.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteBorder;
            this.tbScaleX.TabIndex = 2;
            // 
            // trbScaleY
            // 
            this.trbScaleY.AutoSize = false;
            this.trbScaleY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbScaleY.ChannelColor = System.Drawing.Color.LightGray;
            this.trbScaleY.Customizable = true;
            this.trbScaleY.LargeChange = 1;
            this.trbScaleY.Location = new System.Drawing.Point(423, 289);
            this.trbScaleY.Maximum = 300;
            this.trbScaleY.Minimum = 10;
            this.trbScaleY.Name = "trbScaleY";
            this.trbScaleY.ShowValue = true;
            this.trbScaleY.Size = new System.Drawing.Size(360, 46);
            this.trbScaleY.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbScaleY.TabIndex = 2161;
            this.trbScaleY.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbScaleY.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbScaleY.Value = 100;
            this.trbScaleY.Scroll += new System.EventHandler(this.trbScale_Scroll);
            // 
            // rjLabelScaleY
            // 
            this.rjLabelScaleY.AutoSize = true;
            this.rjLabelScaleY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabelScaleY.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabelScaleY.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabelScaleY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabelScaleY.LinkLabel = false;
            this.rjLabelScaleY.Location = new System.Drawing.Point(423, 264);
            this.rjLabelScaleY.Name = "rjLabelScaleY";
            this.rjLabelScaleY.Size = new System.Drawing.Size(74, 16);
            this.rjLabelScaleY.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabelScaleY.TabIndex = 2162;
            this.rjLabelScaleY.Text = "Scale Y (%)";
            // 
            // tbScaleY
            // 
            this.tbScaleY._Customizable = false;
            this.tbScaleY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbScaleY.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbScaleY.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(162)))), ((int)(((byte)(247)))));
            this.tbScaleY.BorderRadius = 3;
            this.tbScaleY.BorderSize = 2;
            this.tbScaleY.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbScaleY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbScaleY.Location = new System.Drawing.Point(793, 289);
            this.tbScaleY.MultiLine = false;
            this.tbScaleY.Name = "tbScaleY";
            this.tbScaleY.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbScaleY.PasswordChar = false;
            this.tbScaleY.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbScaleY.PlaceHolderText = null;
            this.tbScaleY.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbScaleY.Size = new System.Drawing.Size(118, 31);
            this.tbScaleY.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteBorder;
            this.tbScaleY.TabIndex = 3;
            // 
            // rjButton1
            // 
            this.rjButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(171)))));
            this.rjButton1.BorderRadius = 3;
            this.rjButton1.BorderSize = 1;
            this.rjButton1.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.rjButton1.FlatAppearance.BorderSize = 1;
            this.rjButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(241)))), ((int)(((byte)(247)))));
            this.rjButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(246)))), ((int)(((byte)(251)))));
            this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.rjButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.rjButton1.IconChar = FontAwesome.Sharp.IconChar.None;
            this.rjButton1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.rjButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton1.IconSize = 1;
            this.rjButton1.Location = new System.Drawing.Point(423, 563);
            this.rjButton1.Name = "rjButton1";
            this.rjButton1.Size = new System.Drawing.Size(488, 40);
            this.rjButton1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.rjButton1.TabIndex = 1;
            this.rjButton1.Text = "Run Rotate / Scale";
            this.rjButton1.UseVisualStyleBackColor = false;
            this.rjButton1.Click += new System.EventHandler(this.rjButton1_Click);
            // 
            // FormVision_RotateAndScale
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Rotate / Scale";
            this.ClientSize = new System.Drawing.Size(920, 655);
            this.Controls.Add(this.tbScaleY);
            this.Controls.Add(this.rjLabelScaleY);
            this.Controls.Add(this.trbScaleY);
            this.Controls.Add(this.tbScaleX);
            this.Controls.Add(this.rjLabelScaleX);
            this.Controls.Add(this.trbScaleX);
            this.Controls.Add(this.tbRotate);
            this.Controls.Add(this.rjLabel4);
            this.Controls.Add(this.trbRotate);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_RotateAndScale";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Rotate / Scale";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.trbRotate, 0);
            this.Controls.SetChildIndex(this.rjLabel4, 0);
            this.Controls.SetChildIndex(this.tbRotate, 0);
            this.Controls.SetChildIndex(this.trbScaleX, 0);
            this.Controls.SetChildIndex(this.rjLabelScaleX, 0);
            this.Controls.SetChildIndex(this.tbScaleX, 0);
            this.Controls.SetChildIndex(this.trbScaleY, 0);
            this.Controls.SetChildIndex(this.rjLabelScaleY, 0);
            this.Controls.SetChildIndex(this.tbScaleY, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbRotate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbScaleX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbScaleY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private VisionTestImageCanvas ibSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private VisionTestImageCanvas ibDestination;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel_Desty;
        private System.Windows.Forms.ToolTip toolTip1;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList2;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
        private System.Windows.Forms.Timer transformPreviewTimer;
        private RJCodeUI_M1.RJControls.RJTrackBar trbRotate;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJButton rjButton1;
        private RJCodeUI_M1.RJControls.RJTextBox tbRotate;
        private RJCodeUI_M1.RJControls.RJTrackBar trbScaleX;
        private RJCodeUI_M1.RJControls.RJLabel rjLabelScaleX;
        private RJCodeUI_M1.RJControls.RJTextBox tbScaleX;
        private RJCodeUI_M1.RJControls.RJTrackBar trbScaleY;
        private RJCodeUI_M1.RJControls.RJLabel rjLabelScaleY;
        private RJCodeUI_M1.RJControls.RJTextBox tbScaleY;
    }
}



