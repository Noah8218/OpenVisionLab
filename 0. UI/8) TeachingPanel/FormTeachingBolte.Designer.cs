using Cyotek.Windows.Forms;

namespace KtemVisionSystem
{
    partial class FormTeachingBolte
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTeachingBolte));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.gridROI = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdoInspection = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoThreshold = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoROI = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.btnRoiTest = new RJCodeUI_M1.RJControls.RJButton();
            this.btnSaveVisionParam = new RJCodeUI_M1.RJControls.RJButton();
            this.btnRoiReleative = new RJCodeUI_M1.RJControls.RJButton();
            this.btnRoiSet = new RJCodeUI_M1.RJControls.RJButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnPinDistanceTest = new RJCodeUI_M1.RJControls.RJButton();
            this.btnPinDistanceSetPos = new RJCodeUI_M1.RJControls.RJButton();
            this.btnPinDistanceCalibration = new RJCodeUI_M1.RJControls.RJButton();
            this.btnSpecAreaTest = new RJCodeUI_M1.RJControls.RJButton();
            this.btnSpecAreaSetPos = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton1 = new RJCodeUI_M1.RJControls.RJButton();
            this.btnPinCalibration = new RJCodeUI_M1.RJControls.RJButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tbCalibrationSpec = new RJCodeUI_M1.RJControls.RJTextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ibTrainImage = new Cyotek.Windows.Forms.ImageBox();
            this.propertyGrid_Parameter = new System.Windows.Forms.PropertyGrid();
            this.btnTeachingRun = new RJCodeUI_M1.RJControls.RJButton();
            this.btnTeaching = new RJCodeUI_M1.RJControls.RJButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTeachingDelete = new RJCodeUI_M1.RJControls.RJButton();
            this.cbProcess = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnTeachingAdd = new RJCodeUI_M1.RJControls.RJButton();
            this.cbTeachingCount = new RJCodeUI_M1.RJControls.RJComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rdoBlobPara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoMatchingPara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rjPanel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.metroTabControl1.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridROI)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            // 
            // rjPanel1
            // 
            this.rjPanel1.AutoScroll = true;
            this.rjPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel1.BorderRadius = 5;
            this.rjPanel1.Controls.Add(this.panel5);
            this.rjPanel1.Controls.Add(this.panel3);
            this.rjPanel1.Controls.Add(this.panel4);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rjPanel1.Font = new System.Drawing.Font("Consolas", 24.75F);
            this.rjPanel1.Location = new System.Drawing.Point(0, 0);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(680, 861);
            this.rjPanel1.TabIndex = 2144;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.metroTabControl1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 425);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(680, 418);
            this.panel5.TabIndex = 2161;
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.metroTabPage1);
            this.metroTabControl1.Controls.Add(this.tabPage1);
            this.metroTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroTabControl1.Location = new System.Drawing.Point(0, 0);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 1;
            this.metroTabControl1.Size = new System.Drawing.Size(680, 418);
            this.metroTabControl1.TabIndex = 0;
            this.metroTabControl1.UseSelectable = true;
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.Controls.Add(this.gridROI);
            this.metroTabPage1.Controls.Add(this.panel1);
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(672, 376);
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "3. 검사 영역 && 검사 파라미터 설정";
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            this.metroTabPage1.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.VerticalScrollbarSize = 10;
            // 
            // gridROI
            // 
            this.gridROI.AllowUserToAddRows = false;
            this.gridROI.AllowUserToDeleteRows = false;
            this.gridROI.AllowUserToResizeRows = false;
            this.gridROI.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.gridROI.AlternatingRowsColorApply = false;
            this.gridROI.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.gridROI.BorderRadius = 13;
            this.gridROI.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridROI.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridROI.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.gridROI.ColumnHeaderFont = new System.Drawing.Font("Consolas", 24.75F);
            this.gridROI.ColumnHeaderHeight = 40;
            this.gridROI.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 24.75F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridROI.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridROI.ColumnHeadersHeight = 40;
            this.gridROI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridROI.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.gridROI.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.gridROI.Customizable = false;
            this.gridROI.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.gridROI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridROI.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridROI.EnableHeadersVisualStyles = false;
            this.gridROI.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gridROI.Location = new System.Drawing.Point(0, 130);
            this.gridROI.Name = "gridROI";
            this.gridROI.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridROI.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.gridROI.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 24.75F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridROI.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridROI.RowHeadersVisible = false;
            this.gridROI.RowHeadersWidth = 30;
            this.gridROI.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridROI.RowHeight = 40;
            this.gridROI.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            this.gridROI.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridROI.RowsTextColor = System.Drawing.Color.Gray;
            this.gridROI.RowTemplate.Height = 40;
            this.gridROI.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.gridROI.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridROI.SelectionTextColor = System.Drawing.Color.Gray;
            this.gridROI.Size = new System.Drawing.Size(672, 246);
            this.gridROI.TabIndex = 2165;
            this.gridROI.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSeletecList_CellClick);
            this.gridROI.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridROI_CellMouseDoubleClick);
            this.gridROI.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridROI_CellValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdoInspection);
            this.panel1.Controls.Add(this.rdoThreshold);
            this.panel1.Controls.Add(this.rdoROI);
            this.panel1.Controls.Add(this.btnRoiTest);
            this.panel1.Controls.Add(this.btnSaveVisionParam);
            this.panel1.Controls.Add(this.btnRoiReleative);
            this.panel1.Controls.Add(this.btnRoiSet);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(672, 130);
            this.panel1.TabIndex = 2165;
            // 
            // rdoInspection
            // 
            this.rdoInspection.AutoSize = true;
            this.rdoInspection.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoInspection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoInspection.Customizable = true;
            this.rdoInspection.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoInspection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoInspection.Location = new System.Drawing.Point(161, 100);
            this.rdoInspection.Margin = new System.Windows.Forms.Padding(0);
            this.rdoInspection.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoInspection.Name = "rdoInspection";
            this.rdoInspection.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoInspection.Size = new System.Drawing.Size(101, 18);
            this.rdoInspection.TabIndex = 2163;
            this.rdoInspection.Text = "Inspection";
            this.rdoInspection.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoInspection.UseVisualStyleBackColor = true;
            this.rdoInspection.CheckedChanged += new System.EventHandler(this.Onrdo_CheckedChanged);
            // 
            // rdoThreshold
            // 
            this.rdoThreshold.AutoSize = true;
            this.rdoThreshold.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoThreshold.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoThreshold.Customizable = true;
            this.rdoThreshold.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoThreshold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoThreshold.Location = new System.Drawing.Point(64, 100);
            this.rdoThreshold.Margin = new System.Windows.Forms.Padding(0);
            this.rdoThreshold.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoThreshold.Name = "rdoThreshold";
            this.rdoThreshold.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoThreshold.Size = new System.Drawing.Size(97, 18);
            this.rdoThreshold.TabIndex = 2165;
            this.rdoThreshold.Text = "Threshold";
            this.rdoThreshold.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoThreshold.UseVisualStyleBackColor = true;
            this.rdoThreshold.CheckedChanged += new System.EventHandler(this.Onrdo_CheckedChanged);
            // 
            // rdoROI
            // 
            this.rdoROI.AutoSize = true;
            this.rdoROI.Checked = true;
            this.rdoROI.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoROI.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoROI.Customizable = true;
            this.rdoROI.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoROI.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoROI.Location = new System.Drawing.Point(6, 100);
            this.rdoROI.Margin = new System.Windows.Forms.Padding(0);
            this.rdoROI.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoROI.Name = "rdoROI";
            this.rdoROI.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoROI.Size = new System.Drawing.Size(58, 18);
            this.rdoROI.TabIndex = 2164;
            this.rdoROI.TabStop = true;
            this.rdoROI.Text = "ROI";
            this.rdoROI.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoROI.UseVisualStyleBackColor = true;
            this.rdoROI.CheckedChanged += new System.EventHandler(this.Onrdo_CheckedChanged);
            // 
            // btnRoiTest
            // 
            this.btnRoiTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnRoiTest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiTest.BorderRadius = 15;
            this.btnRoiTest.BorderSize = 3;
            this.btnRoiTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRoiTest.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnRoiTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnRoiTest.FlatAppearance.BorderSize = 3;
            this.btnRoiTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnRoiTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRoiTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRoiTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnRoiTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiTest.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnRoiTest.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiTest.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnRoiTest.IconSize = 80;
            this.btnRoiTest.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRoiTest.Location = new System.Drawing.Point(312, 3);
            this.btnRoiTest.Name = "btnRoiTest";
            this.btnRoiTest.Size = new System.Drawing.Size(148, 87);
            this.btnRoiTest.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnRoiTest.TabIndex = 2162;
            this.btnRoiTest.Text = "ROI 검사 테스트";
            this.btnRoiTest.UseVisualStyleBackColor = false;
            this.btnRoiTest.Click += new System.EventHandler(this.btnRoiTest_Click_1);
            // 
            // btnSaveVisionParam
            // 
            this.btnSaveVisionParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnSaveVisionParam.IconSize = 70;
            this.btnSaveVisionParam.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveVisionParam.Location = new System.Drawing.Point(532, 3);
            this.btnSaveVisionParam.Name = "btnSaveVisionParam";
            this.btnSaveVisionParam.Size = new System.Drawing.Size(131, 93);
            this.btnSaveVisionParam.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSaveVisionParam.TabIndex = 2156;
            this.btnSaveVisionParam.Text = "SAVE";
            this.btnSaveVisionParam.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveVisionParam.UseVisualStyleBackColor = false;
            this.btnSaveVisionParam.Click += new System.EventHandler(this.btnSaveVisionParam_Click);
            // 
            // btnRoiReleative
            // 
            this.btnRoiReleative.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnRoiReleative.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiReleative.BorderRadius = 15;
            this.btnRoiReleative.BorderSize = 3;
            this.btnRoiReleative.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRoiReleative.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnRoiReleative.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnRoiReleative.FlatAppearance.BorderSize = 3;
            this.btnRoiReleative.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnRoiReleative.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRoiReleative.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRoiReleative.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnRoiReleative.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiReleative.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnRoiReleative.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiReleative.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnRoiReleative.IconSize = 80;
            this.btnRoiReleative.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRoiReleative.Location = new System.Drawing.Point(158, 3);
            this.btnRoiReleative.Name = "btnRoiReleative";
            this.btnRoiReleative.Size = new System.Drawing.Size(148, 87);
            this.btnRoiReleative.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnRoiReleative.TabIndex = 2160;
            this.btnRoiReleative.Text = "2) ROI 상대좌표";
            this.btnRoiReleative.UseVisualStyleBackColor = false;
            this.btnRoiReleative.Click += new System.EventHandler(this.btnRoiReleative_Click);
            // 
            // btnRoiSet
            // 
            this.btnRoiSet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnRoiSet.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiSet.BorderRadius = 15;
            this.btnRoiSet.BorderSize = 3;
            this.btnRoiSet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRoiSet.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnRoiSet.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnRoiSet.FlatAppearance.BorderSize = 3;
            this.btnRoiSet.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnRoiSet.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRoiSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRoiSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnRoiSet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiSet.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnRoiSet.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnRoiSet.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnRoiSet.IconSize = 80;
            this.btnRoiSet.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRoiSet.Location = new System.Drawing.Point(4, 3);
            this.btnRoiSet.Name = "btnRoiSet";
            this.btnRoiSet.Size = new System.Drawing.Size(148, 87);
            this.btnRoiSet.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnRoiSet.TabIndex = 2159;
            this.btnRoiSet.Text = "1) ROI SET";
            this.btnRoiSet.UseVisualStyleBackColor = false;
            this.btnRoiSet.Click += new System.EventHandler(this.btnRoiSet_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 38);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(672, 376);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "4. Calibration";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnPinDistanceTest);
            this.panel2.Controls.Add(this.btnPinDistanceSetPos);
            this.panel2.Controls.Add(this.btnPinDistanceCalibration);
            this.panel2.Controls.Add(this.btnSpecAreaTest);
            this.panel2.Controls.Add(this.btnSpecAreaSetPos);
            this.panel2.Controls.Add(this.rjButton1);
            this.panel2.Controls.Add(this.btnPinCalibration);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.tbCalibrationSpec);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(672, 376);
            this.panel2.TabIndex = 2167;
            // 
            // btnPinDistanceTest
            // 
            this.btnPinDistanceTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnPinDistanceTest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceTest.BorderRadius = 15;
            this.btnPinDistanceTest.BorderSize = 3;
            this.btnPinDistanceTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPinDistanceTest.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnPinDistanceTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnPinDistanceTest.FlatAppearance.BorderSize = 3;
            this.btnPinDistanceTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnPinDistanceTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnPinDistanceTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPinDistanceTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnPinDistanceTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceTest.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnPinDistanceTest.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceTest.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPinDistanceTest.IconSize = 80;
            this.btnPinDistanceTest.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnPinDistanceTest.Location = new System.Drawing.Point(513, 104);
            this.btnPinDistanceTest.Name = "btnPinDistanceTest";
            this.btnPinDistanceTest.Size = new System.Drawing.Size(148, 95);
            this.btnPinDistanceTest.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnPinDistanceTest.TabIndex = 2166;
            this.btnPinDistanceTest.Text = "검사 테스트";
            this.btnPinDistanceTest.UseVisualStyleBackColor = false;
            this.btnPinDistanceTest.Click += new System.EventHandler(this.btnPinDistanceTest_Click);
            // 
            // btnPinDistanceSetPos
            // 
            this.btnPinDistanceSetPos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnPinDistanceSetPos.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceSetPos.BorderRadius = 15;
            this.btnPinDistanceSetPos.BorderSize = 3;
            this.btnPinDistanceSetPos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPinDistanceSetPos.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnPinDistanceSetPos.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnPinDistanceSetPos.FlatAppearance.BorderSize = 3;
            this.btnPinDistanceSetPos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnPinDistanceSetPos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnPinDistanceSetPos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPinDistanceSetPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnPinDistanceSetPos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceSetPos.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnPinDistanceSetPos.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceSetPos.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPinDistanceSetPos.IconSize = 80;
            this.btnPinDistanceSetPos.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnPinDistanceSetPos.Location = new System.Drawing.Point(361, 104);
            this.btnPinDistanceSetPos.Name = "btnPinDistanceSetPos";
            this.btnPinDistanceSetPos.Size = new System.Drawing.Size(148, 95);
            this.btnPinDistanceSetPos.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnPinDistanceSetPos.TabIndex = 2165;
            this.btnPinDistanceSetPos.Text = "2) Distance 상대좌표";
            this.btnPinDistanceSetPos.UseVisualStyleBackColor = false;
            this.btnPinDistanceSetPos.Click += new System.EventHandler(this.btnPinDistanceSetPos_Click);
            // 
            // btnPinDistanceCalibration
            // 
            this.btnPinDistanceCalibration.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnPinDistanceCalibration.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceCalibration.BorderRadius = 15;
            this.btnPinDistanceCalibration.BorderSize = 3;
            this.btnPinDistanceCalibration.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPinDistanceCalibration.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnPinDistanceCalibration.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnPinDistanceCalibration.FlatAppearance.BorderSize = 3;
            this.btnPinDistanceCalibration.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnPinDistanceCalibration.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnPinDistanceCalibration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPinDistanceCalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnPinDistanceCalibration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceCalibration.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnPinDistanceCalibration.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinDistanceCalibration.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPinDistanceCalibration.IconSize = 80;
            this.btnPinDistanceCalibration.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnPinDistanceCalibration.Location = new System.Drawing.Point(207, 104);
            this.btnPinDistanceCalibration.Name = "btnPinDistanceCalibration";
            this.btnPinDistanceCalibration.Size = new System.Drawing.Size(148, 95);
            this.btnPinDistanceCalibration.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnPinDistanceCalibration.TabIndex = 2164;
            this.btnPinDistanceCalibration.Text = "1) Pin Distance Calibration";
            this.btnPinDistanceCalibration.UseVisualStyleBackColor = false;
            this.btnPinDistanceCalibration.Click += new System.EventHandler(this.btnPinDistanceCalibration_Click);
            // 
            // btnSpecAreaTest
            // 
            this.btnSpecAreaTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnSpecAreaTest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSpecAreaTest.BorderRadius = 15;
            this.btnSpecAreaTest.BorderSize = 3;
            this.btnSpecAreaTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSpecAreaTest.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSpecAreaTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSpecAreaTest.FlatAppearance.BorderSize = 3;
            this.btnSpecAreaTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSpecAreaTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSpecAreaTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSpecAreaTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnSpecAreaTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSpecAreaTest.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnSpecAreaTest.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSpecAreaTest.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSpecAreaTest.IconSize = 80;
            this.btnSpecAreaTest.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSpecAreaTest.Location = new System.Drawing.Point(515, 3);
            this.btnSpecAreaTest.Name = "btnSpecAreaTest";
            this.btnSpecAreaTest.Size = new System.Drawing.Size(148, 95);
            this.btnSpecAreaTest.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSpecAreaTest.TabIndex = 2163;
            this.btnSpecAreaTest.Text = "검사 테스트";
            this.btnSpecAreaTest.UseVisualStyleBackColor = false;
            this.btnSpecAreaTest.Click += new System.EventHandler(this.btnSpecAreaTest_Click);
            // 
            // btnSpecAreaSetPos
            // 
            this.btnSpecAreaSetPos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnSpecAreaSetPos.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSpecAreaSetPos.BorderRadius = 15;
            this.btnSpecAreaSetPos.BorderSize = 3;
            this.btnSpecAreaSetPos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSpecAreaSetPos.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSpecAreaSetPos.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSpecAreaSetPos.FlatAppearance.BorderSize = 3;
            this.btnSpecAreaSetPos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSpecAreaSetPos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSpecAreaSetPos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSpecAreaSetPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnSpecAreaSetPos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSpecAreaSetPos.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnSpecAreaSetPos.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSpecAreaSetPos.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSpecAreaSetPos.IconSize = 80;
            this.btnSpecAreaSetPos.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSpecAreaSetPos.Location = new System.Drawing.Point(361, 3);
            this.btnSpecAreaSetPos.Name = "btnSpecAreaSetPos";
            this.btnSpecAreaSetPos.Size = new System.Drawing.Size(148, 95);
            this.btnSpecAreaSetPos.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSpecAreaSetPos.TabIndex = 2162;
            this.btnSpecAreaSetPos.Text = "2) Spec Area 상대좌표";
            this.btnSpecAreaSetPos.UseVisualStyleBackColor = false;
            this.btnSpecAreaSetPos.Click += new System.EventHandler(this.rjButton2_Click);
            // 
            // rjButton1
            // 
            this.rjButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rjButton1.BackColor = System.Drawing.Color.White;
            this.rjButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.BorderRadius = 15;
            this.rjButton1.BorderSize = 3;
            this.rjButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton1.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.rjButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.rjButton1.FlatAppearance.BorderSize = 3;
            this.rjButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.rjButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.rjButton1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton1.IconSize = 70;
            this.rjButton1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rjButton1.Location = new System.Drawing.Point(533, 280);
            this.rjButton1.Name = "rjButton1";
            this.rjButton1.Size = new System.Drawing.Size(131, 93);
            this.rjButton1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.rjButton1.TabIndex = 2160;
            this.rjButton1.Text = "SAVE";
            this.rjButton1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rjButton1.UseVisualStyleBackColor = false;
            this.rjButton1.Click += new System.EventHandler(this.rjButton1_Click);
            // 
            // btnPinCalibration
            // 
            this.btnPinCalibration.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnPinCalibration.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinCalibration.BorderRadius = 15;
            this.btnPinCalibration.BorderSize = 3;
            this.btnPinCalibration.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPinCalibration.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnPinCalibration.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnPinCalibration.FlatAppearance.BorderSize = 3;
            this.btnPinCalibration.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnPinCalibration.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnPinCalibration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPinCalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnPinCalibration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinCalibration.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnPinCalibration.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnPinCalibration.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPinCalibration.IconSize = 80;
            this.btnPinCalibration.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnPinCalibration.Location = new System.Drawing.Point(207, 3);
            this.btnPinCalibration.Name = "btnPinCalibration";
            this.btnPinCalibration.Size = new System.Drawing.Size(148, 95);
            this.btnPinCalibration.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnPinCalibration.TabIndex = 2161;
            this.btnPinCalibration.Text = "1) Pin Spec Area Calibration";
            this.btnPinCalibration.UseVisualStyleBackColor = false;
            this.btnPinCalibration.Click += new System.EventHandler(this.btnPinCalibration_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(9, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 2141;
            this.label2.Text = "Spec";
            // 
            // tbCalibrationSpec
            // 
            this.tbCalibrationSpec._Customizable = true;
            this.tbCalibrationSpec.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbCalibrationSpec.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbCalibrationSpec.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbCalibrationSpec.BorderRadius = 0;
            this.tbCalibrationSpec.BorderSize = 1;
            this.tbCalibrationSpec.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbCalibrationSpec.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbCalibrationSpec.Location = new System.Drawing.Point(6, 37);
            this.tbCalibrationSpec.MultiLine = false;
            this.tbCalibrationSpec.Name = "tbCalibrationSpec";
            this.tbCalibrationSpec.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbCalibrationSpec.PasswordChar = false;
            this.tbCalibrationSpec.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbCalibrationSpec.PlaceHolderText = "10";
            this.tbCalibrationSpec.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbCalibrationSpec.Size = new System.Drawing.Size(184, 31);
            this.tbCalibrationSpec.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbCalibrationSpec.TabIndex = 2142;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 138);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(680, 287);
            this.panel3.TabIndex = 2159;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoBlobPara);
            this.groupBox2.Controls.Add(this.rdoMatchingPara);
            this.groupBox2.Controls.Add(this.ibTrainImage);
            this.groupBox2.Controls.Add(this.propertyGrid_Parameter);
            this.groupBox2.Controls.Add(this.btnTeachingRun);
            this.groupBox2.Controls.Add(this.btnTeaching);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(680, 287);
            this.groupBox2.TabIndex = 2156;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "2. 상대좌표 설정";
            // 
            // ibTrainImage
            // 
            this.ibTrainImage.Location = new System.Drawing.Point(6, 129);
            this.ibTrainImage.Name = "ibTrainImage";
            this.ibTrainImage.Size = new System.Drawing.Size(266, 154);
            this.ibTrainImage.TabIndex = 2165;
            // 
            // propertyGrid_Parameter
            // 
            this.propertyGrid_Parameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid_Parameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.propertyGrid_Parameter.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGrid_Parameter.Location = new System.Drawing.Point(296, 60);
            this.propertyGrid_Parameter.Name = "propertyGrid_Parameter";
            this.propertyGrid_Parameter.Size = new System.Drawing.Size(372, 223);
            this.propertyGrid_Parameter.TabIndex = 2164;
            // 
            // btnTeachingRun
            // 
            this.btnTeachingRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnTeachingRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTeachingRun.BorderRadius = 15;
            this.btnTeachingRun.BorderSize = 3;
            this.btnTeachingRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeachingRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnTeachingRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnTeachingRun.FlatAppearance.BorderSize = 3;
            this.btnTeachingRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnTeachingRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnTeachingRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeachingRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTeachingRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTeachingRun.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnTeachingRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTeachingRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTeachingRun.IconSize = 80;
            this.btnTeachingRun.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnTeachingRun.Location = new System.Drawing.Point(5, 36);
            this.btnTeachingRun.Name = "btnTeachingRun";
            this.btnTeachingRun.Size = new System.Drawing.Size(126, 87);
            this.btnTeachingRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnTeachingRun.TabIndex = 2154;
            this.btnTeachingRun.Text = "1) Run";
            this.btnTeachingRun.UseVisualStyleBackColor = false;
            this.btnTeachingRun.Click += new System.EventHandler(this.btnTeachingRun_Click);
            // 
            // btnTeaching
            // 
            this.btnTeaching.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnTeaching.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTeaching.BorderRadius = 15;
            this.btnTeaching.BorderSize = 3;
            this.btnTeaching.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeaching.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnTeaching.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnTeaching.FlatAppearance.BorderSize = 3;
            this.btnTeaching.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnTeaching.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnTeaching.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeaching.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnTeaching.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTeaching.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnTeaching.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTeaching.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTeaching.IconSize = 80;
            this.btnTeaching.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnTeaching.Location = new System.Drawing.Point(137, 36);
            this.btnTeaching.Name = "btnTeaching";
            this.btnTeaching.Size = new System.Drawing.Size(135, 87);
            this.btnTeaching.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnTeaching.TabIndex = 2153;
            this.btnTeaching.Text = "2) Teaching";
            this.btnTeaching.UseVisualStyleBackColor = false;
            this.btnTeaching.Click += new System.EventHandler(this.btnTeaching_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(680, 138);
            this.panel4.TabIndex = 2160;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.btnTeachingDelete);
            this.groupBox3.Controls.Add(this.cbProcess);
            this.groupBox3.Controls.Add(this.btnTeachingAdd);
            this.groupBox3.Controls.Add(this.cbTeachingCount);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(680, 138);
            this.groupBox3.TabIndex = 2158;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "1. 티칭 개수 설정";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(289, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 25);
            this.label1.TabIndex = 2179;
            this.label1.Text = "티칭 포인트 : ";
            // 
            // btnTeachingDelete
            // 
            this.btnTeachingDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTeachingDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnTeachingDelete.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnTeachingDelete.BorderRadius = 10;
            this.btnTeachingDelete.BorderSize = 1;
            this.btnTeachingDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeachingDelete.Design = RJCodeUI_M1.RJControls.ButtonDesign.Delete;
            this.btnTeachingDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnTeachingDelete.FlatAppearance.BorderSize = 0;
            this.btnTeachingDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(74)))), ((int)(((byte)(77)))));
            this.btnTeachingDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(69)))), ((int)(((byte)(72)))));
            this.btnTeachingDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeachingDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTeachingDelete.ForeColor = System.Drawing.Color.White;
            this.btnTeachingDelete.IconChar = FontAwesome.Sharp.IconChar.TrashAlt;
            this.btnTeachingDelete.IconColor = System.Drawing.Color.White;
            this.btnTeachingDelete.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTeachingDelete.IconSize = 24;
            this.btnTeachingDelete.Location = new System.Drawing.Point(523, 74);
            this.btnTeachingDelete.Name = "btnTeachingDelete";
            this.btnTeachingDelete.Size = new System.Drawing.Size(142, 44);
            this.btnTeachingDelete.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnTeachingDelete.TabIndex = 2153;
            this.btnTeachingDelete.Text = "Delete";
            this.btnTeachingDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTeachingDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTeachingDelete.UseVisualStyleBackColor = false;
            this.btnTeachingDelete.Click += new System.EventHandler(this.btnTeachingDelete_Click);
            // 
            // cbProcess
            // 
            this.cbProcess.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbProcess.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbProcess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbProcess.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbProcess.BorderRadius = 0;
            this.cbProcess.BorderSize = 2;
            this.cbProcess.Customizable = true;
            this.cbProcess.DataSource = null;
            this.cbProcess.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProcess.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbProcess.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbProcess.ForeColor = System.Drawing.Color.DimGray;
            this.cbProcess.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbProcess.Location = new System.Drawing.Point(64, 43);
            this.cbProcess.MinimumSize = new System.Drawing.Size(100, 20);
            this.cbProcess.Name = "cbProcess";
            this.cbProcess.Padding = new System.Windows.Forms.Padding(2);
            this.cbProcess.SelectedIndex = -1;
            this.cbProcess.Size = new System.Drawing.Size(218, 27);
            this.cbProcess.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbProcess.TabIndex = 2178;
            this.cbProcess.Texts = "";
            this.cbProcess.OnSelectedIndexChanged += new System.EventHandler(this.cbProcess_OnSelectedIndexChanged);
            // 
            // btnTeachingAdd
            // 
            this.btnTeachingAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTeachingAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnTeachingAdd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTeachingAdd.BorderRadius = 10;
            this.btnTeachingAdd.BorderSize = 1;
            this.btnTeachingAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeachingAdd.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnTeachingAdd.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnTeachingAdd.FlatAppearance.BorderSize = 0;
            this.btnTeachingAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(149)))), ((int)(((byte)(106)))));
            this.btnTeachingAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(139)))), ((int)(((byte)(99)))));
            this.btnTeachingAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeachingAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTeachingAdd.ForeColor = System.Drawing.Color.White;
            this.btnTeachingAdd.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnTeachingAdd.IconColor = System.Drawing.Color.White;
            this.btnTeachingAdd.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTeachingAdd.IconSize = 24;
            this.btnTeachingAdd.Location = new System.Drawing.Point(380, 74);
            this.btnTeachingAdd.Name = "btnTeachingAdd";
            this.btnTeachingAdd.Size = new System.Drawing.Size(142, 44);
            this.btnTeachingAdd.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnTeachingAdd.TabIndex = 2152;
            this.btnTeachingAdd.Text = "Add";
            this.btnTeachingAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTeachingAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTeachingAdd.UseVisualStyleBackColor = false;
            this.btnTeachingAdd.Click += new System.EventHandler(this.btnTeachingAdd_Click);
            // 
            // cbTeachingCount
            // 
            this.cbTeachingCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTeachingCount.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbTeachingCount.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbTeachingCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbTeachingCount.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.cbTeachingCount.BorderRadius = 0;
            this.cbTeachingCount.BorderSize = 2;
            this.cbTeachingCount.Customizable = true;
            this.cbTeachingCount.DataSource = null;
            this.cbTeachingCount.DropDownBackColor = System.Drawing.SystemColors.Window;
            this.cbTeachingCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTeachingCount.DropDownTextColor = System.Drawing.SystemColors.WindowText;
            this.cbTeachingCount.IconColor = System.Drawing.Color.CornflowerBlue;
            this.cbTeachingCount.Location = new System.Drawing.Point(441, 43);
            this.cbTeachingCount.Name = "cbTeachingCount";
            this.cbTeachingCount.Padding = new System.Windows.Forms.Padding(3);
            this.cbTeachingCount.SelectedIndex = -1;
            this.cbTeachingCount.Size = new System.Drawing.Size(218, 27);
            this.cbTeachingCount.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbTeachingCount.TabIndex = 2154;
            this.cbTeachingCount.Texts = "";
            this.cbTeachingCount.OnSelectedIndexChanged += new System.EventHandler(this.cbTeachingCount_OnSelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(4, 41);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 25);
            this.label4.TabIndex = 2157;
            this.label4.Text = "공정 :";
            // 
            // rdoBlobPara
            // 
            this.rdoBlobPara.AutoSize = true;
            this.rdoBlobPara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoBlobPara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoBlobPara.Customizable = true;
            this.rdoBlobPara.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoBlobPara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoBlobPara.Location = new System.Drawing.Point(387, 36);
            this.rdoBlobPara.Margin = new System.Windows.Forms.Padding(0);
            this.rdoBlobPara.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoBlobPara.Name = "rdoBlobPara";
            this.rdoBlobPara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoBlobPara.Size = new System.Drawing.Size(62, 18);
            this.rdoBlobPara.TabIndex = 2167;
            this.rdoBlobPara.Text = "Blob";
            this.rdoBlobPara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoBlobPara.UseVisualStyleBackColor = true;
            this.rdoBlobPara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoMatchingPara
            // 
            this.rdoMatchingPara.AutoSize = true;
            this.rdoMatchingPara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoMatchingPara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoMatchingPara.Customizable = true;
            this.rdoMatchingPara.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoMatchingPara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoMatchingPara.Location = new System.Drawing.Point(296, 36);
            this.rdoMatchingPara.Margin = new System.Windows.Forms.Padding(0);
            this.rdoMatchingPara.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoMatchingPara.Name = "rdoMatchingPara";
            this.rdoMatchingPara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoMatchingPara.Size = new System.Drawing.Size(91, 18);
            this.rdoMatchingPara.TabIndex = 2166;
            this.rdoMatchingPara.Text = "Matching";
            this.rdoMatchingPara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoMatchingPara.UseVisualStyleBackColor = true;
            this.rdoMatchingPara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // FormTeachingBolte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(680, 861);
            this.Controls.Add(this.rjPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormTeachingBolte";
            this.Text = "TeachingBolte";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.rjPanel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.metroTabControl1.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridROI)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private RJCodeUI_M1.RJControls.RJButton btnTeachingRun;
        private RJCodeUI_M1.RJControls.RJButton btnTeaching;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJComboBox cbTeachingCount;
        private RJCodeUI_M1.RJControls.RJButton btnSaveVisionParam;
        private RJCodeUI_M1.RJControls.RJComboBox cbProcess;
        private System.Windows.Forms.Label label4;
        private RJCodeUI_M1.RJControls.RJButton btnTeachingDelete;
        private RJCodeUI_M1.RJControls.RJButton btnTeachingAdd;
        private System.Windows.Forms.PropertyGrid propertyGrid_Parameter;
        private ImageBox ibTrainImage;
        private RJCodeUI_M1.RJControls.RJButton btnRoiTest;
        private RJCodeUI_M1.RJControls.RJButton btnRoiReleative;
        private RJCodeUI_M1.RJControls.RJButton btnRoiSet;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private RJCodeUI_M1.RJControls.RJDataGridView gridROI;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoInspection;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoThreshold;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoROI;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage metroTabPage1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private RJCodeUI_M1.RJControls.RJTextBox tbCalibrationSpec;
        private RJCodeUI_M1.RJControls.RJButton rjButton1;
        private RJCodeUI_M1.RJControls.RJButton btnPinCalibration;
        private RJCodeUI_M1.RJControls.RJButton btnSpecAreaSetPos;
        private RJCodeUI_M1.RJControls.RJButton btnSpecAreaTest;
        private RJCodeUI_M1.RJControls.RJButton btnPinDistanceTest;
        private RJCodeUI_M1.RJControls.RJButton btnPinDistanceSetPos;
        private RJCodeUI_M1.RJControls.RJButton btnPinDistanceCalibration;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoBlobPara;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoMatchingPara;
    }
}