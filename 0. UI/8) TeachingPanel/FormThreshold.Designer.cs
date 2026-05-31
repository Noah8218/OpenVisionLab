using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormThreshold
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormThreshold));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.tbWeight = new RJCodeUI_M1.RJControls.RJTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbBlockSize = new RJCodeUI_M1.RJControls.RJTextBox();
            this.trbAdaptiveThreshold = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel2 = new RJCodeUI_M1.RJControls.RJLabel();
            this.cbAdaptiveType = new RJCodeUI_M1.RJControls.RJComboBox();
            this.rjLabel6 = new RJCodeUI_M1.RJControls.RJLabel();
            this.cbAdaptiveThresholdMenu = new RJCodeUI_M1.RJControls.RJComboBox();
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.trbDoubleThresholdMin = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel5 = new RJCodeUI_M1.RJControls.RJLabel();
            this.trbDoubleThresholdMax = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.trbThreshold = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.cbThresholdMenu = new RJCodeUI_M1.RJControls.RJComboBox();
            this.rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
            this.panel1.SuspendLayout();
            this.rjPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbAdaptiveThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbDoubleThresholdMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbDoubleThresholdMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.panel1.Controls.Add(this.rjPanel1);
            this.panel1.Controls.Add(this.trbAdaptiveThreshold);
            this.panel1.Controls.Add(this.rjLabel2);
            this.panel1.Controls.Add(this.cbAdaptiveType);
            this.panel1.Controls.Add(this.rjLabel6);
            this.panel1.Controls.Add(this.cbAdaptiveThresholdMenu);
            this.panel1.Controls.Add(this.rjLabel3);
            this.panel1.Controls.Add(this.trbDoubleThresholdMin);
            this.panel1.Controls.Add(this.rjLabel5);
            this.panel1.Controls.Add(this.trbDoubleThresholdMax);
            this.panel1.Controls.Add(this.rjLabel1);
            this.panel1.Controls.Add(this.trbThreshold);
            this.panel1.Controls.Add(this.cbThresholdMenu);
            this.panel1.Controls.Add(this.rjLabel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(612, 456);
            this.panel1.TabIndex = 1952;
            // 
            // rjPanel1
            // 
            this.rjPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel1.BorderRadius = 5;
            this.rjPanel1.Controls.Add(this.label4);
            this.rjPanel1.Controls.Add(this.tbWeight);
            this.rjPanel1.Controls.Add(this.label5);
            this.rjPanel1.Controls.Add(this.tbBlockSize);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjPanel1.Location = new System.Drawing.Point(0, 385);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(595, 123);
            this.rjPanel1.TabIndex = 2195;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 16);
            this.label4.TabIndex = 2136;
            this.label4.Text = "Block Size";
            // 
            // tbWeight
            // 
            this.tbWeight._Customizable = true;
            this.tbWeight.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbWeight.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbWeight.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbWeight.BorderRadius = 0;
            this.tbWeight.BorderSize = 1;
            this.tbWeight.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbWeight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbWeight.Location = new System.Drawing.Point(11, 78);
            this.tbWeight.MultiLine = false;
            this.tbWeight.Name = "tbWeight";
            this.tbWeight.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbWeight.PasswordChar = false;
            this.tbWeight.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbWeight.PlaceHolderText = "5";
            this.tbWeight.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbWeight.Size = new System.Drawing.Size(184, 31);
            this.tbWeight.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbWeight.TabIndex = 2141;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(12, 59);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 16);
            this.label5.TabIndex = 2137;
            this.label5.Text = "Weight";
            // 
            // tbBlockSize
            // 
            this.tbBlockSize._Customizable = true;
            this.tbBlockSize.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbBlockSize.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbBlockSize.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbBlockSize.BorderRadius = 0;
            this.tbBlockSize.BorderSize = 1;
            this.tbBlockSize.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbBlockSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbBlockSize.Location = new System.Drawing.Point(9, 27);
            this.tbBlockSize.MultiLine = false;
            this.tbBlockSize.Name = "tbBlockSize";
            this.tbBlockSize.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbBlockSize.PasswordChar = false;
            this.tbBlockSize.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbBlockSize.PlaceHolderText = "25";
            this.tbBlockSize.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbBlockSize.Size = new System.Drawing.Size(184, 31);
            this.tbBlockSize.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbBlockSize.TabIndex = 2140;
            // 
            // trbAdaptiveThreshold
            // 
            this.trbAdaptiveThreshold.AutoSize = false;
            this.trbAdaptiveThreshold.ChannelColor = System.Drawing.Color.LightGray;
            this.trbAdaptiveThreshold.Customizable = true;
            this.trbAdaptiveThreshold.Dock = System.Windows.Forms.DockStyle.Top;
            this.trbAdaptiveThreshold.LargeChange = 1;
            this.trbAdaptiveThreshold.Location = new System.Drawing.Point(0, 339);
            this.trbAdaptiveThreshold.Maximum = 255;
            this.trbAdaptiveThreshold.Minimum = 1;
            this.trbAdaptiveThreshold.Name = "trbAdaptiveThreshold";
            this.trbAdaptiveThreshold.ShowValue = true;
            this.trbAdaptiveThreshold.Size = new System.Drawing.Size(595, 46);
            this.trbAdaptiveThreshold.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbAdaptiveThreshold.TabIndex = 2194;
            this.trbAdaptiveThreshold.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbAdaptiveThreshold.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbAdaptiveThreshold.Value = 1;
            this.trbAdaptiveThreshold.Scroll += new System.EventHandler(this.trbAdaptiveThreshold_Scroll);
            // 
            // rjLabel2
            // 
            this.rjLabel2.AutoSize = true;
            this.rjLabel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjLabel2.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel2.LinkLabel = false;
            this.rjLabel2.Location = new System.Drawing.Point(0, 323);
            this.rjLabel2.Name = "rjLabel2";
            this.rjLabel2.Size = new System.Drawing.Size(134, 16);
            this.rjLabel2.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel2.TabIndex = 2193;
            this.rjLabel2.Text = "Adaptive Threshold";
            // 
            // cbAdaptiveType
            // 
            this.cbAdaptiveType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbAdaptiveType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbAdaptiveType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbAdaptiveType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbAdaptiveType.BorderRadius = 0;
            this.cbAdaptiveType.BorderSize = 2;
            this.cbAdaptiveType.Customizable = false;
            this.cbAdaptiveType.DataSource = null;
            this.cbAdaptiveType.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbAdaptiveType.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbAdaptiveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdaptiveType.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbAdaptiveType.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbAdaptiveType.ForeColor = System.Drawing.Color.DimGray;
            this.cbAdaptiveType.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbAdaptiveType.Location = new System.Drawing.Point(0, 288);
            this.cbAdaptiveType.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbAdaptiveType.Name = "cbAdaptiveType";
            this.cbAdaptiveType.Padding = new System.Windows.Forms.Padding(2);
            this.cbAdaptiveType.SelectedIndex = -1;
            this.cbAdaptiveType.Size = new System.Drawing.Size(595, 35);
            this.cbAdaptiveType.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbAdaptiveType.TabIndex = 2192;
            this.cbAdaptiveType.Texts = "";
            // 
            // rjLabel6
            // 
            this.rjLabel6.AutoSize = true;
            this.rjLabel6.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjLabel6.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel6.LinkLabel = false;
            this.rjLabel6.Location = new System.Drawing.Point(0, 272);
            this.rjLabel6.Name = "rjLabel6";
            this.rjLabel6.Size = new System.Drawing.Size(165, 16);
            this.rjLabel6.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel6.TabIndex = 2191;
            this.rjLabel6.Text = "AdaptiveThreshold Type";
            // 
            // cbAdaptiveThresholdMenu
            // 
            this.cbAdaptiveThresholdMenu.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbAdaptiveThresholdMenu.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbAdaptiveThresholdMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbAdaptiveThresholdMenu.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbAdaptiveThresholdMenu.BorderRadius = 0;
            this.cbAdaptiveThresholdMenu.BorderSize = 2;
            this.cbAdaptiveThresholdMenu.Customizable = false;
            this.cbAdaptiveThresholdMenu.DataSource = null;
            this.cbAdaptiveThresholdMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbAdaptiveThresholdMenu.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbAdaptiveThresholdMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdaptiveThresholdMenu.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbAdaptiveThresholdMenu.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbAdaptiveThresholdMenu.ForeColor = System.Drawing.Color.DimGray;
            this.cbAdaptiveThresholdMenu.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbAdaptiveThresholdMenu.Location = new System.Drawing.Point(0, 237);
            this.cbAdaptiveThresholdMenu.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbAdaptiveThresholdMenu.Name = "cbAdaptiveThresholdMenu";
            this.cbAdaptiveThresholdMenu.Padding = new System.Windows.Forms.Padding(2);
            this.cbAdaptiveThresholdMenu.SelectedIndex = -1;
            this.cbAdaptiveThresholdMenu.Size = new System.Drawing.Size(595, 35);
            this.cbAdaptiveThresholdMenu.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbAdaptiveThresholdMenu.TabIndex = 2190;
            this.cbAdaptiveThresholdMenu.Texts = "";
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjLabel3.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(0, 221);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(108, 16);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel3.TabIndex = 2189;
            this.rjLabel3.Text = "Threshold Type";
            // 
            // trbDoubleThresholdMin
            // 
            this.trbDoubleThresholdMin.AutoSize = false;
            this.trbDoubleThresholdMin.ChannelColor = System.Drawing.Color.LightGray;
            this.trbDoubleThresholdMin.Customizable = true;
            this.trbDoubleThresholdMin.Dock = System.Windows.Forms.DockStyle.Top;
            this.trbDoubleThresholdMin.LargeChange = 1;
            this.trbDoubleThresholdMin.Location = new System.Drawing.Point(0, 175);
            this.trbDoubleThresholdMin.Maximum = 255;
            this.trbDoubleThresholdMin.Minimum = 1;
            this.trbDoubleThresholdMin.Name = "trbDoubleThresholdMin";
            this.trbDoubleThresholdMin.ShowValue = true;
            this.trbDoubleThresholdMin.Size = new System.Drawing.Size(595, 46);
            this.trbDoubleThresholdMin.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbDoubleThresholdMin.TabIndex = 2186;
            this.trbDoubleThresholdMin.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbDoubleThresholdMin.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbDoubleThresholdMin.Value = 1;
            this.trbDoubleThresholdMin.Scroll += new System.EventHandler(this.trbDoubleThresholdMax_Scroll);
            // 
            // rjLabel5
            // 
            this.rjLabel5.AutoSize = true;
            this.rjLabel5.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjLabel5.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel5.LinkLabel = false;
            this.rjLabel5.Location = new System.Drawing.Point(0, 159);
            this.rjLabel5.Name = "rjLabel5";
            this.rjLabel5.Size = new System.Drawing.Size(147, 16);
            this.rjLabel5.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel5.TabIndex = 2185;
            this.rjLabel5.Text = "Double Threshold Min";
            // 
            // trbDoubleThresholdMax
            // 
            this.trbDoubleThresholdMax.AutoSize = false;
            this.trbDoubleThresholdMax.ChannelColor = System.Drawing.Color.LightGray;
            this.trbDoubleThresholdMax.Customizable = true;
            this.trbDoubleThresholdMax.Dock = System.Windows.Forms.DockStyle.Top;
            this.trbDoubleThresholdMax.LargeChange = 1;
            this.trbDoubleThresholdMax.Location = new System.Drawing.Point(0, 113);
            this.trbDoubleThresholdMax.Maximum = 255;
            this.trbDoubleThresholdMax.Minimum = 1;
            this.trbDoubleThresholdMax.Name = "trbDoubleThresholdMax";
            this.trbDoubleThresholdMax.ShowValue = true;
            this.trbDoubleThresholdMax.Size = new System.Drawing.Size(595, 46);
            this.trbDoubleThresholdMax.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbDoubleThresholdMax.TabIndex = 2184;
            this.trbDoubleThresholdMax.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbDoubleThresholdMax.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbDoubleThresholdMax.Value = 1;
            this.trbDoubleThresholdMax.Scroll += new System.EventHandler(this.trbDoubleThresholdMax_Scroll);
            // 
            // rjLabel1
            // 
            this.rjLabel1.AutoSize = true;
            this.rjLabel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjLabel1.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel1.LinkLabel = false;
            this.rjLabel1.Location = new System.Drawing.Point(0, 97);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(151, 16);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel1.TabIndex = 2183;
            this.rjLabel1.Text = "Double Threshold Max";
            // 
            // trbThreshold
            // 
            this.trbThreshold.AutoSize = false;
            this.trbThreshold.ChannelColor = System.Drawing.Color.LightGray;
            this.trbThreshold.Customizable = true;
            this.trbThreshold.Dock = System.Windows.Forms.DockStyle.Top;
            this.trbThreshold.LargeChange = 1;
            this.trbThreshold.Location = new System.Drawing.Point(0, 51);
            this.trbThreshold.Maximum = 255;
            this.trbThreshold.Minimum = 1;
            this.trbThreshold.Name = "trbThreshold";
            this.trbThreshold.ShowValue = true;
            this.trbThreshold.Size = new System.Drawing.Size(595, 46);
            this.trbThreshold.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbThreshold.TabIndex = 2182;
            this.trbThreshold.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbThreshold.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbThreshold.Value = 1;
            this.trbThreshold.Scroll += new System.EventHandler(this.trbThreshold_Scroll);
            // 
            // cbThresholdMenu
            // 
            this.cbThresholdMenu.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbThresholdMenu.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbThresholdMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbThresholdMenu.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbThresholdMenu.BorderRadius = 0;
            this.cbThresholdMenu.BorderSize = 2;
            this.cbThresholdMenu.Customizable = false;
            this.cbThresholdMenu.DataSource = null;
            this.cbThresholdMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbThresholdMenu.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbThresholdMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbThresholdMenu.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbThresholdMenu.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbThresholdMenu.ForeColor = System.Drawing.Color.DimGray;
            this.cbThresholdMenu.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbThresholdMenu.Location = new System.Drawing.Point(0, 16);
            this.cbThresholdMenu.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbThresholdMenu.Name = "cbThresholdMenu";
            this.cbThresholdMenu.Padding = new System.Windows.Forms.Padding(2);
            this.cbThresholdMenu.SelectedIndex = -1;
            this.cbThresholdMenu.Size = new System.Drawing.Size(595, 35);
            this.cbThresholdMenu.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbThresholdMenu.TabIndex = 2181;
            this.cbThresholdMenu.Texts = "";
            // 
            // rjLabel4
            // 
            this.rjLabel4.AutoSize = true;
            this.rjLabel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjLabel4.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel4.LinkLabel = false;
            this.rjLabel4.Location = new System.Drawing.Point(0, 0);
            this.rjLabel4.Name = "rjLabel4";
            this.rjLabel4.Size = new System.Drawing.Size(108, 16);
            this.rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel4.TabIndex = 2180;
            this.rjLabel4.Text = "Threshold Type";
            // 
            // FormThreshold
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(612, 456);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormThreshold";
            this.Text = "Threshold";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.rjPanel1.ResumeLayout(false);
            this.rjPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbAdaptiveThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbDoubleThresholdMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbDoubleThresholdMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbThreshold)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.Panel panel1;
        public RJCodeUI_M1.RJControls.RJTrackBar trbDoubleThresholdMin;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel5;
        public RJCodeUI_M1.RJControls.RJTrackBar trbDoubleThresholdMax;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        public RJCodeUI_M1.RJControls.RJTrackBar trbThreshold;
        private RJCodeUI_M1.RJControls.RJComboBox cbThresholdMenu;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJComboBox cbAdaptiveType;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel6;
        private RJCodeUI_M1.RJControls.RJComboBox cbAdaptiveThresholdMenu;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        public RJCodeUI_M1.RJControls.RJTrackBar trbAdaptiveThreshold;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel2;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private System.Windows.Forms.Label label4;
        private RJCodeUI_M1.RJControls.RJTextBox tbWeight;
        private System.Windows.Forms.Label label5;
        private RJCodeUI_M1.RJControls.RJTextBox tbBlockSize;
    }
}