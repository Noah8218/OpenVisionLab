using Cyotek.Windows.Forms;

namespace KtemVisionSystem
{
    partial class FormSystem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSystem));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.btnSaveVisionParam = new RJCodeUI_M1.RJControls.RJButton();
            this.btnGrab = new RJCodeUI_M1.RJControls.RJButton();
            this.btnLive = new RJCodeUI_M1.RJControls.RJButton();
            this.label4 = new System.Windows.Forms.Label();
            this.tbExposure = new RJCodeUI_M1.RJControls.RJTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbGain = new RJCodeUI_M1.RJControls.RJTextBox();
            this.rjPanel3 = new RJCodeUI_M1.RJControls.RJPanel();
            this.cbCamera = new RJCodeUI_M1.RJControls.RJComboBox();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.chkUseLayerImage = new RJCodeUI_M1.RJControls.RJCheckBox();
            this.rjLabel2 = new RJCodeUI_M1.RJControls.RJLabel();
            this.btnNewPanel = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.rjPanel1.SuspendLayout();
            this.rjPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // rjPanel1
            // 
            this.rjPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel1.BorderRadius = 5;
            this.rjPanel1.Controls.Add(this.btnSaveVisionParam);
            this.rjPanel1.Controls.Add(this.btnGrab);
            this.rjPanel1.Controls.Add(this.btnLive);
            this.rjPanel1.Controls.Add(this.label4);
            this.rjPanel1.Controls.Add(this.tbExposure);
            this.rjPanel1.Controls.Add(this.label5);
            this.rjPanel1.Controls.Add(this.tbGain);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjPanel1.Location = new System.Drawing.Point(0, 72);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(612, 123);
            this.rjPanel1.TabIndex = 2144;
            // 
            // btnSaveVisionParam
            // 
            this.btnSaveVisionParam.BackColor = System.Drawing.Color.White;
            this.btnSaveVisionParam.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveVisionParam.BorderRadius = 15;
            this.btnSaveVisionParam.BorderSize = 3;
            this.btnSaveVisionParam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveVisionParam.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSaveVisionParam.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSaveVisionParam.FlatAppearance.BorderSize = 3;
            this.btnSaveVisionParam.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSaveVisionParam.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSaveVisionParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveVisionParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveVisionParam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveVisionParam.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveVisionParam.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveVisionParam.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveVisionParam.IconSize = 80;
            this.btnSaveVisionParam.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveVisionParam.Location = new System.Drawing.Point(477, 6);
            this.btnSaveVisionParam.Name = "btnSaveVisionParam";
            this.btnSaveVisionParam.Size = new System.Drawing.Size(125, 99);
            this.btnSaveVisionParam.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSaveVisionParam.TabIndex = 2142;
            this.btnSaveVisionParam.Text = "SAVE";
            this.btnSaveVisionParam.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveVisionParam.UseVisualStyleBackColor = false;
            this.btnSaveVisionParam.Click += new System.EventHandler(this.btnSaveVisionParam_Click);
            // 
            // btnGrab
            // 
            this.btnGrab.BackColor = System.Drawing.Color.White;
            this.btnGrab.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnGrab.BorderRadius = 15;
            this.btnGrab.BorderSize = 3;
            this.btnGrab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGrab.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnGrab.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnGrab.FlatAppearance.BorderSize = 3;
            this.btnGrab.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnGrab.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnGrab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGrab.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnGrab.IconChar = FontAwesome.Sharp.IconChar.Camera;
            this.btnGrab.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnGrab.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnGrab.IconSize = 80;
            this.btnGrab.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnGrab.Location = new System.Drawing.Point(215, 6);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(125, 99);
            this.btnGrab.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnGrab.TabIndex = 2138;
            this.btnGrab.Text = "GRAB";
            this.btnGrab.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGrab.UseVisualStyleBackColor = false;
            this.btnGrab.Click += new System.EventHandler(this.OnClickCameraOperation);
            // 
            // btnLive
            // 
            this.btnLive.BackColor = System.Drawing.Color.White;
            this.btnLive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLive.BorderRadius = 15;
            this.btnLive.BorderSize = 3;
            this.btnLive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLive.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnLive.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnLive.FlatAppearance.BorderSize = 3;
            this.btnLive.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnLive.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLive.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLive.IconChar = FontAwesome.Sharp.IconChar.Youtube;
            this.btnLive.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLive.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLive.IconSize = 80;
            this.btnLive.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnLive.Location = new System.Drawing.Point(346, 6);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(125, 99);
            this.btnLive.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnLive.TabIndex = 2139;
            this.btnLive.Text = "LIVE";
            this.btnLive.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnLive.UseVisualStyleBackColor = false;
            this.btnLive.Click += new System.EventHandler(this.OnClickCameraOperation);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 16);
            this.label4.TabIndex = 2136;
            this.label4.Text = "GAIN";
            // 
            // tbExposure
            // 
            this.tbExposure._Customizable = true;
            this.tbExposure.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbExposure.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbExposure.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbExposure.BorderRadius = 0;
            this.tbExposure.BorderSize = 1;
            this.tbExposure.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbExposure.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbExposure.Location = new System.Drawing.Point(11, 78);
            this.tbExposure.MultiLine = false;
            this.tbExposure.Name = "tbExposure";
            this.tbExposure.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbExposure.PasswordChar = false;
            this.tbExposure.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbExposure.PlaceHolderText = "10";
            this.tbExposure.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbExposure.Size = new System.Drawing.Size(184, 31);
            this.tbExposure.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbExposure.TabIndex = 2141;
            this.tbExposure.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPress_KeyPress);
            this.tbExposure.MouseLeave += new System.EventHandler(this.tbGain_MouseLeave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(12, 59);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 16);
            this.label5.TabIndex = 2137;
            this.label5.Text = "EXPOSURE";
            // 
            // tbGain
            // 
            this.tbGain._Customizable = true;
            this.tbGain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbGain.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbGain.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbGain.BorderRadius = 0;
            this.tbGain.BorderSize = 1;
            this.tbGain.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbGain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbGain.Location = new System.Drawing.Point(9, 27);
            this.tbGain.MultiLine = false;
            this.tbGain.Name = "tbGain";
            this.tbGain.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbGain.PasswordChar = false;
            this.tbGain.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbGain.PlaceHolderText = "10";
            this.tbGain.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbGain.Size = new System.Drawing.Size(184, 31);
            this.tbGain.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbGain.TabIndex = 2140;
            this.tbGain.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPress_KeyPress);
            this.tbGain.MouseLeave += new System.EventHandler(this.tbGain_MouseLeave);
            // 
            // rjPanel3
            // 
            this.rjPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel3.BorderRadius = 5;
            this.rjPanel3.Controls.Add(this.cbCamera);
            this.rjPanel3.Controls.Add(this.cbLayerList);
            this.rjPanel3.Controls.Add(this.chkUseLayerImage);
            this.rjPanel3.Controls.Add(this.rjLabel2);
            this.rjPanel3.Controls.Add(this.btnNewPanel);
            this.rjPanel3.Customizable = false;
            this.rjPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjPanel3.Location = new System.Drawing.Point(0, 0);
            this.rjPanel3.Name = "rjPanel3";
            this.rjPanel3.Size = new System.Drawing.Size(612, 72);
            this.rjPanel3.TabIndex = 2143;
            // 
            // cbCamera
            // 
            this.cbCamera.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbCamera.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbCamera.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbCamera.BorderRadius = 0;
            this.cbCamera.BorderSize = 2;
            this.cbCamera.Customizable = false;
            this.cbCamera.DataSource = null;
            this.cbCamera.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCamera.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbCamera.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbCamera.ForeColor = System.Drawing.Color.DimGray;
            this.cbCamera.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbCamera.Location = new System.Drawing.Point(12, 32);
            this.cbCamera.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbCamera.Name = "cbCamera";
            this.cbCamera.Padding = new System.Windows.Forms.Padding(2);
            this.cbCamera.SelectedIndex = -1;
            this.cbCamera.Size = new System.Drawing.Size(181, 35);
            this.cbCamera.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbCamera.TabIndex = 2174;
            this.cbCamera.Texts = "";
            this.cbCamera.OnSelectedIndexChanged += new System.EventHandler(this.cbCamera_SelectedIndexChanged);
            // 
            // cbLayerList
            // 
            this.cbLayerList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.BorderRadius = 0;
            this.cbLayerList.BorderSize = 2;
            this.cbLayerList.Customizable = false;
            this.cbLayerList.DataSource = null;
            this.cbLayerList.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList.Location = new System.Drawing.Point(189, 32);
            this.cbLayerList.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList.Name = "cbLayerList";
            this.cbLayerList.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList.SelectedIndex = -1;
            this.cbLayerList.Size = new System.Drawing.Size(203, 35);
            this.cbLayerList.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList.TabIndex = 2173;
            this.cbLayerList.Texts = "";
            this.cbLayerList.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList_SelectedIndexChanged);
            // 
            // chkUseLayerImage
            // 
            this.chkUseLayerImage.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkUseLayerImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chkUseLayerImage.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.chkUseLayerImage.BorderSize = 1;
            this.chkUseLayerImage.Check = true;
            this.chkUseLayerImage.Checked = true;
            this.chkUseLayerImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseLayerImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkUseLayerImage.Customizable = false;
            this.chkUseLayerImage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.chkUseLayerImage.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.chkUseLayerImage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.chkUseLayerImage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(82)))), ((int)(((byte)(180)))));
            this.chkUseLayerImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkUseLayerImage.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseLayerImage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.chkUseLayerImage.IconColor = System.Drawing.Color.White;
            this.chkUseLayerImage.Location = new System.Drawing.Point(189, 4);
            this.chkUseLayerImage.MinimumSize = new System.Drawing.Size(0, 21);
            this.chkUseLayerImage.Name = "chkUseLayerImage";
            this.chkUseLayerImage.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.chkUseLayerImage.Size = new System.Drawing.Size(118, 26);
            this.chkUseLayerImage.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.chkUseLayerImage.TabIndex = 2169;
            this.chkUseLayerImage.Text = "Panel Source Image";
            this.chkUseLayerImage.UseVisualStyleBackColor = false;
            this.chkUseLayerImage.CheckedChanged += new System.EventHandler(this.chkUseLayerImage_CheckedChanged);
            // 
            // rjLabel2
            // 
            this.rjLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rjLabel2.AutoSize = true;
            this.rjLabel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel2.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel2.LinkLabel = false;
            this.rjLabel2.Location = new System.Drawing.Point(14, 9);
            this.rjLabel2.Name = "rjLabel2";
            this.rjLabel2.Size = new System.Drawing.Size(35, 16);
            this.rjLabel2.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel2.TabIndex = 2171;
            this.rjLabel2.Text = "Cam";
            // 
            // btnNewPanel
            // 
            this.btnNewPanel.BackColor = System.Drawing.Color.Transparent;
            this.btnNewPanel.BackIcon = true;
            this.btnNewPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPanel.Customizable = true;
            this.btnNewPanel.DropdownMenu = null;
            this.btnNewPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            this.btnNewPanel.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewPanel.IconSize = 30;
            this.btnNewPanel.Location = new System.Drawing.Point(341, 4);
            this.btnNewPanel.Name = "btnNewPanel";
            this.btnNewPanel.Size = new System.Drawing.Size(30, 30);
            this.btnNewPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnNewPanel.TabIndex = 2168;
            this.btnNewPanel.TabStop = false;
            this.btnNewPanel.Click += new System.EventHandler(this.btnNewPanel_Click);
            // 
            // FormSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(612, 381);
            this.Controls.Add(this.rjPanel1);
            this.Controls.Add(this.rjPanel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormSystem";
            this.Text = "System";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.rjPanel1.ResumeLayout(false);
            this.rjPanel1.PerformLayout();
            this.rjPanel3.ResumeLayout(false);
            this.rjPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private RJCodeUI_M1.RJControls.RJButton btnSaveVisionParam;
        private RJCodeUI_M1.RJControls.RJButton btnLive;
        private RJCodeUI_M1.RJControls.RJTextBox tbExposure;
        private RJCodeUI_M1.RJControls.RJButton btnGrab;
        private RJCodeUI_M1.RJControls.RJTextBox tbGain;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel3;
        private RJCodeUI_M1.RJControls.RJComboBox cbCamera;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
        private RJCodeUI_M1.RJControls.RJCheckBox chkUseLayerImage;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel2;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
    }
}