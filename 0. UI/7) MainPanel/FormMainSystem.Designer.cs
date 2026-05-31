using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormMainSystem
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainSystem));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerDateTime = new System.Windows.Forms.Timer(this.components);
            this.timerConnection = new System.Windows.Forms.Timer(this.components);
            this.timerEncoder = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvLabelDefct = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.timerCountUp = new System.Windows.Forms.Timer(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgvAttachLabel = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.rjPanel9 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lbPlcMotionSpeed = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel21 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel8 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lbLotNO = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel19 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel5 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lbTotalDisM = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel18 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel7 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lbTotalDis = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel20 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.rjPanel6 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lbEncoderCam1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel16 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel2 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lbEncoderCam2 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel15 = new RJCodeUI_M1.RJControls.RJLabel();
            this.lbMenu = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel4 = new RJCodeUI_M1.RJControls.RJPanel();
            this.btnReset = new RJCodeUI_M1.RJControls.RJButton();
            this.rjLabel2 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel5 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel25 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel7 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel6 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel3 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lbEncoder = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel14 = new RJCodeUI_M1.RJControls.RJLabel();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLabelDefct)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttachLabel)).BeginInit();
            this.rjPanel9.SuspendLayout();
            this.rjPanel8.SuspendLayout();
            this.rjPanel5.SuspendLayout();
            this.rjPanel7.SuspendLayout();
            this.rjPanel1.SuspendLayout();
            this.rjPanel6.SuspendLayout();
            this.rjPanel2.SuspendLayout();
            this.rjPanel4.SuspendLayout();
            this.rjPanel3.SuspendLayout();
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
            // timerDateTime
            // 
            this.timerDateTime.Enabled = true;
            this.timerDateTime.Interval = 1000;
            this.timerDateTime.Tick += new System.EventHandler(this.timerDateTime_Tick);
            // 
            // timerConnection
            // 
            this.timerConnection.Enabled = true;
            this.timerConnection.Tick += new System.EventHandler(this.timerConnection_Tick);
            // 
            // timerEncoder
            // 
            this.timerEncoder.Enabled = true;
            this.timerEncoder.Interval = 200;
            this.timerEncoder.Tick += new System.EventHandler(this.timerEncoder_Tick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvLabelDefct);
            this.panel2.Location = new System.Drawing.Point(6, 361);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(474, 180);
            this.panel2.TabIndex = 2165;
            // 
            // dgvLabelDefct
            // 
            this.dgvLabelDefct.AllowUserToAddRows = false;
            this.dgvLabelDefct.AllowUserToDeleteRows = false;
            this.dgvLabelDefct.AllowUserToResizeRows = false;
            this.dgvLabelDefct.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dgvLabelDefct.AlternatingRowsColorApply = false;
            this.dgvLabelDefct.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLabelDefct.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvLabelDefct.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvLabelDefct.BorderRadius = 13;
            this.dgvLabelDefct.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLabelDefct.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLabelDefct.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dgvLabelDefct.ColumnHeaderFont = new System.Drawing.Font("Verdana", 9F);
            this.dgvLabelDefct.ColumnHeaderHeight = 40;
            this.dgvLabelDefct.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLabelDefct.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLabelDefct.ColumnHeadersHeight = 40;
            this.dgvLabelDefct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLabelDefct.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dgvLabelDefct.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLabelDefct.Customizable = false;
            this.dgvLabelDefct.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvLabelDefct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLabelDefct.EnableHeadersVisualStyles = false;
            this.dgvLabelDefct.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvLabelDefct.Location = new System.Drawing.Point(0, 0);
            this.dgvLabelDefct.Name = "dgvLabelDefct";
            this.dgvLabelDefct.ReadOnly = true;
            this.dgvLabelDefct.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvLabelDefct.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dgvLabelDefct.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLabelDefct.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLabelDefct.RowHeadersVisible = false;
            this.dgvLabelDefct.RowHeadersWidth = 30;
            this.dgvLabelDefct.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvLabelDefct.RowHeight = 40;
            this.dgvLabelDefct.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            this.dgvLabelDefct.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLabelDefct.RowsTextColor = System.Drawing.Color.Gray;
            this.dgvLabelDefct.RowTemplate.Height = 40;
            this.dgvLabelDefct.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dgvLabelDefct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLabelDefct.SelectionTextColor = System.Drawing.Color.Gray;
            this.dgvLabelDefct.Size = new System.Drawing.Size(474, 180);
            this.dgvLabelDefct.TabIndex = 2140;
            this.dgvLabelDefct.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAttachLabel_CellClick);
            this.dgvLabelDefct.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseDoubleClick);
            // 
            // timerCountUp
            // 
            this.timerCountUp.Interval = 50;
            this.timerCountUp.Tick += new System.EventHandler(this.timerCountUp_Tick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dgvAttachLabel);
            this.panel3.Location = new System.Drawing.Point(6, 547);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(474, 181);
            this.panel3.TabIndex = 2166;
            // 
            // dgvAttachLabel
            // 
            this.dgvAttachLabel.AllowUserToAddRows = false;
            this.dgvAttachLabel.AllowUserToDeleteRows = false;
            this.dgvAttachLabel.AllowUserToResizeRows = false;
            this.dgvAttachLabel.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dgvAttachLabel.AlternatingRowsColorApply = false;
            this.dgvAttachLabel.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAttachLabel.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvAttachLabel.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvAttachLabel.BorderRadius = 13;
            this.dgvAttachLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAttachLabel.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvAttachLabel.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dgvAttachLabel.ColumnHeaderFont = new System.Drawing.Font("Verdana", 9F);
            this.dgvAttachLabel.ColumnHeaderHeight = 40;
            this.dgvAttachLabel.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Verdana", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAttachLabel.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvAttachLabel.ColumnHeadersHeight = 40;
            this.dgvAttachLabel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvAttachLabel.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dgvAttachLabel.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAttachLabel.Customizable = false;
            this.dgvAttachLabel.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvAttachLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAttachLabel.EnableHeadersVisualStyles = false;
            this.dgvAttachLabel.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvAttachLabel.Location = new System.Drawing.Point(0, 0);
            this.dgvAttachLabel.Name = "dgvAttachLabel";
            this.dgvAttachLabel.ReadOnly = true;
            this.dgvAttachLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvAttachLabel.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dgvAttachLabel.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Verdana", 9F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAttachLabel.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvAttachLabel.RowHeadersVisible = false;
            this.dgvAttachLabel.RowHeadersWidth = 30;
            this.dgvAttachLabel.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvAttachLabel.RowHeight = 40;
            this.dgvAttachLabel.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Gray;
            this.dgvAttachLabel.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvAttachLabel.RowsTextColor = System.Drawing.Color.Gray;
            this.dgvAttachLabel.RowTemplate.Height = 40;
            this.dgvAttachLabel.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dgvAttachLabel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAttachLabel.SelectionTextColor = System.Drawing.Color.Gray;
            this.dgvAttachLabel.Size = new System.Drawing.Size(474, 181);
            this.dgvAttachLabel.TabIndex = 2141;
            this.dgvAttachLabel.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAttachLabel_CellClick);
            this.dgvAttachLabel.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAttachLabel_CellMouseDoubleClick);
            // 
            // rjPanel9
            // 
            this.rjPanel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel9.BorderRadius = 5;
            this.rjPanel9.Controls.Add(this.lbPlcMotionSpeed);
            this.rjPanel9.Controls.Add(this.rjLabel21);
            this.rjPanel9.Customizable = false;
            this.rjPanel9.Location = new System.Drawing.Point(323, 278);
            this.rjPanel9.Name = "rjPanel9";
            this.rjPanel9.Size = new System.Drawing.Size(160, 57);
            this.rjPanel9.TabIndex = 2168;
            // 
            // lbPlcMotionSpeed
            // 
            this.lbPlcMotionSpeed.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbPlcMotionSpeed.Font = new System.Drawing.Font("Verdana", 14F);
            this.lbPlcMotionSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.lbPlcMotionSpeed.LinkLabel = false;
            this.lbPlcMotionSpeed.Location = new System.Drawing.Point(5, 27);
            this.lbPlcMotionSpeed.Name = "lbPlcMotionSpeed";
            this.lbPlcMotionSpeed.Size = new System.Drawing.Size(152, 23);
            this.lbPlcMotionSpeed.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lbPlcMotionSpeed.TabIndex = 1;
            this.lbPlcMotionSpeed.Text = "-";
            this.lbPlcMotionSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel21
            // 
            this.rjLabel21.AutoSize = true;
            this.rjLabel21.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel21.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel21.LinkLabel = false;
            this.rjLabel21.Location = new System.Drawing.Point(29, 9);
            this.rjLabel21.Name = "rjLabel21";
            this.rjLabel21.Size = new System.Drawing.Size(117, 18);
            this.rjLabel21.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel21.TabIndex = 0;
            this.rjLabel21.Text = "Speed(m/min)";
            // 
            // rjPanel8
            // 
            this.rjPanel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel8.BorderRadius = 5;
            this.rjPanel8.Controls.Add(this.lbLotNO);
            this.rjPanel8.Controls.Add(this.rjLabel19);
            this.rjPanel8.Customizable = false;
            this.rjPanel8.Location = new System.Drawing.Point(3, 278);
            this.rjPanel8.Name = "rjPanel8";
            this.rjPanel8.Size = new System.Drawing.Size(316, 57);
            this.rjPanel8.TabIndex = 2167;
            // 
            // lbLotNO
            // 
            this.lbLotNO.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbLotNO.Font = new System.Drawing.Font("Verdana", 14F);
            this.lbLotNO.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.lbLotNO.LinkLabel = false;
            this.lbLotNO.Location = new System.Drawing.Point(3, 26);
            this.lbLotNO.Name = "lbLotNO";
            this.lbLotNO.Size = new System.Drawing.Size(306, 23);
            this.lbLotNO.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lbLotNO.TabIndex = 1;
            this.lbLotNO.Text = "-";
            this.lbLotNO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel19
            // 
            this.rjLabel19.AutoSize = true;
            this.rjLabel19.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel19.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel19.LinkLabel = false;
            this.rjLabel19.Location = new System.Drawing.Point(124, 8);
            this.rjLabel19.Name = "rjLabel19";
            this.rjLabel19.Size = new System.Drawing.Size(58, 18);
            this.rjLabel19.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel19.TabIndex = 0;
            this.rjLabel19.Text = "Lot No";
            // 
            // rjPanel5
            // 
            this.rjPanel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel5.BorderRadius = 5;
            this.rjPanel5.Controls.Add(this.lbTotalDisM);
            this.rjPanel5.Controls.Add(this.rjLabel18);
            this.rjPanel5.Customizable = false;
            this.rjPanel5.Location = new System.Drawing.Point(1, 215);
            this.rjPanel5.Name = "rjPanel5";
            this.rjPanel5.Size = new System.Drawing.Size(160, 57);
            this.rjPanel5.TabIndex = 2161;
            // 
            // lbTotalDisM
            // 
            this.lbTotalDisM.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbTotalDisM.Font = new System.Drawing.Font("Verdana", 14F);
            this.lbTotalDisM.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.lbTotalDisM.LinkLabel = false;
            this.lbTotalDisM.Location = new System.Drawing.Point(3, 26);
            this.lbTotalDisM.Name = "lbTotalDisM";
            this.lbTotalDisM.Size = new System.Drawing.Size(154, 23);
            this.lbTotalDisM.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lbTotalDisM.TabIndex = 1;
            this.lbTotalDisM.Text = "-";
            this.lbTotalDisM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel18
            // 
            this.rjLabel18.AutoSize = true;
            this.rjLabel18.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel18.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel18.LinkLabel = false;
            this.rjLabel18.Location = new System.Drawing.Point(38, 9);
            this.rjLabel18.Name = "rjLabel18";
            this.rjLabel18.Size = new System.Drawing.Size(92, 18);
            this.rjLabel18.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel18.TabIndex = 0;
            this.rjLabel18.Text = "생산 길이(M)";
            // 
            // rjPanel7
            // 
            this.rjPanel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel7.BorderRadius = 5;
            this.rjPanel7.Controls.Add(this.lbTotalDis);
            this.rjPanel7.Controls.Add(this.rjLabel20);
            this.rjPanel7.Customizable = false;
            this.rjPanel7.Location = new System.Drawing.Point(162, 215);
            this.rjPanel7.Name = "rjPanel7";
            this.rjPanel7.Size = new System.Drawing.Size(160, 57);
            this.rjPanel7.TabIndex = 2162;
            // 
            // lbTotalDis
            // 
            this.lbTotalDis.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbTotalDis.Font = new System.Drawing.Font("Verdana", 14F);
            this.lbTotalDis.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.lbTotalDis.LinkLabel = false;
            this.lbTotalDis.Location = new System.Drawing.Point(3, 26);
            this.lbTotalDis.Name = "lbTotalDis";
            this.lbTotalDis.Size = new System.Drawing.Size(154, 23);
            this.lbTotalDis.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lbTotalDis.TabIndex = 1;
            this.lbTotalDis.Text = "-";
            this.lbTotalDis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel20
            // 
            this.rjLabel20.AutoSize = true;
            this.rjLabel20.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel20.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel20.LinkLabel = false;
            this.rjLabel20.Location = new System.Drawing.Point(26, 9);
            this.rjLabel20.Name = "rjLabel20";
            this.rjLabel20.Size = new System.Drawing.Size(105, 18);
            this.rjLabel20.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel20.TabIndex = 0;
            this.rjLabel20.Text = "생산 길이(MM)";
            // 
            // rjPanel1
            // 
            this.rjPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel1.BorderRadius = 5;
            this.rjPanel1.Controls.Add(this.rjPanel6);
            this.rjPanel1.Controls.Add(this.rjPanel2);
            this.rjPanel1.Controls.Add(this.lbMenu);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Location = new System.Drawing.Point(0, 2);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(480, 75);
            this.rjPanel1.TabIndex = 2143;
            // 
            // rjPanel6
            // 
            this.rjPanel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel6.BorderRadius = 5;
            this.rjPanel6.Controls.Add(this.lbEncoderCam1);
            this.rjPanel6.Controls.Add(this.rjLabel16);
            this.rjPanel6.Customizable = false;
            this.rjPanel6.Location = new System.Drawing.Point(6, 3);
            this.rjPanel6.Name = "rjPanel6";
            this.rjPanel6.Size = new System.Drawing.Size(160, 57);
            this.rjPanel6.TabIndex = 2137;
            this.rjPanel6.Visible = false;
            // 
            // lbEncoderCam1
            // 
            this.lbEncoderCam1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbEncoderCam1.Font = new System.Drawing.Font("Verdana", 14F);
            this.lbEncoderCam1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.lbEncoderCam1.LinkLabel = false;
            this.lbEncoderCam1.Location = new System.Drawing.Point(3, 26);
            this.lbEncoderCam1.Name = "lbEncoderCam1";
            this.lbEncoderCam1.Size = new System.Drawing.Size(154, 23);
            this.lbEncoderCam1.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lbEncoderCam1.TabIndex = 1;
            this.lbEncoderCam1.Text = "-";
            this.lbEncoderCam1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel16
            // 
            this.rjLabel16.AutoSize = true;
            this.rjLabel16.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel16.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel16.LinkLabel = false;
            this.rjLabel16.Location = new System.Drawing.Point(11, 8);
            this.rjLabel16.Name = "rjLabel16";
            this.rjLabel16.Size = new System.Drawing.Size(127, 18);
            this.rjLabel16.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel16.TabIndex = 0;
            this.rjLabel16.Text = "Encoder(Cam)1";
            // 
            // rjPanel2
            // 
            this.rjPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel2.BorderRadius = 5;
            this.rjPanel2.Controls.Add(this.lbEncoderCam2);
            this.rjPanel2.Controls.Add(this.rjLabel15);
            this.rjPanel2.Customizable = false;
            this.rjPanel2.Location = new System.Drawing.Point(167, 3);
            this.rjPanel2.Name = "rjPanel2";
            this.rjPanel2.Size = new System.Drawing.Size(160, 57);
            this.rjPanel2.TabIndex = 2139;
            this.rjPanel2.Visible = false;
            // 
            // lbEncoderCam2
            // 
            this.lbEncoderCam2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbEncoderCam2.Font = new System.Drawing.Font("Verdana", 14F);
            this.lbEncoderCam2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.lbEncoderCam2.LinkLabel = false;
            this.lbEncoderCam2.Location = new System.Drawing.Point(3, 26);
            this.lbEncoderCam2.Name = "lbEncoderCam2";
            this.lbEncoderCam2.Size = new System.Drawing.Size(154, 23);
            this.lbEncoderCam2.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lbEncoderCam2.TabIndex = 1;
            this.lbEncoderCam2.Text = "-";
            this.lbEncoderCam2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel15
            // 
            this.rjLabel15.AutoSize = true;
            this.rjLabel15.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel15.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel15.LinkLabel = false;
            this.rjLabel15.Location = new System.Drawing.Point(21, 9);
            this.rjLabel15.Name = "rjLabel15";
            this.rjLabel15.Size = new System.Drawing.Size(127, 18);
            this.rjLabel15.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel15.TabIndex = 0;
            this.rjLabel15.Text = "Encoder(Cam)2";
            // 
            // lbMenu
            // 
            this.lbMenu.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbMenu.Font = new System.Drawing.Font("Consolas", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lbMenu.LinkLabel = false;
            this.lbMenu.Location = new System.Drawing.Point(12, 7);
            this.lbMenu.Name = "lbMenu";
            this.lbMenu.Size = new System.Drawing.Size(441, 68);
            this.lbMenu.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.lbMenu.TabIndex = 2126;
            this.lbMenu.Text = "Mode";
            this.lbMenu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjPanel4
            // 
            this.rjPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel4.BorderRadius = 5;
            this.rjPanel4.Controls.Add(this.btnReset);
            this.rjPanel4.Controls.Add(this.rjLabel2);
            this.rjPanel4.Controls.Add(this.rjLabel3);
            this.rjPanel4.Controls.Add(this.rjLabel5);
            this.rjPanel4.Controls.Add(this.rjLabel25);
            this.rjPanel4.Controls.Add(this.rjLabel4);
            this.rjPanel4.Controls.Add(this.rjLabel7);
            this.rjPanel4.Controls.Add(this.rjLabel1);
            this.rjPanel4.Controls.Add(this.rjLabel6);
            this.rjPanel4.Customizable = false;
            this.rjPanel4.Location = new System.Drawing.Point(1, 78);
            this.rjPanel4.Name = "rjPanel4";
            this.rjPanel4.Size = new System.Drawing.Size(482, 138);
            this.rjPanel4.TabIndex = 2136;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.White;
            this.btnReset.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnReset.BorderRadius = 15;
            this.btnReset.BorderSize = 3;
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnReset.FlatAppearance.BorderSize = 3;
            this.btnReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnReset.IconChar = FontAwesome.Sharp.IconChar.Redo;
            this.btnReset.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnReset.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnReset.IconSize = 65;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReset.Location = new System.Drawing.Point(357, 43);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(111, 88);
            this.btnReset.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnReset.TabIndex = 2141;
            this.btnReset.Text = "RESET";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // rjLabel2
            // 
            this.rjLabel2.AutoSize = true;
            this.rjLabel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel2.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel2.LinkLabel = false;
            this.rjLabel2.Location = new System.Drawing.Point(123, 6);
            this.rjLabel2.Name = "rjLabel2";
            this.rjLabel2.Size = new System.Drawing.Size(63, 34);
            this.rjLabel2.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel2.TabIndex = 2120;
            this.rjLabel2.Text = "000";
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(259, 6);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(63, 34);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel3.TabIndex = 2122;
            this.rjLabel3.Text = "000";
            // 
            // rjLabel5
            // 
            this.rjLabel5.AutoSize = true;
            this.rjLabel5.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel5.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel5.ForeColor = System.Drawing.Color.Red;
            this.rjLabel5.LinkLabel = false;
            this.rjLabel5.Location = new System.Drawing.Point(405, 6);
            this.rjLabel5.Name = "rjLabel5";
            this.rjLabel5.Size = new System.Drawing.Size(63, 34);
            this.rjLabel5.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel5.TabIndex = 2124;
            this.rjLabel5.Text = "000";
            // 
            // rjLabel25
            // 
            this.rjLabel25.AutoSize = true;
            this.rjLabel25.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel25.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel25.LinkLabel = false;
            this.rjLabel25.Location = new System.Drawing.Point(3, 6);
            this.rjLabel25.Name = "rjLabel25";
            this.rjLabel25.Size = new System.Drawing.Size(127, 34);
            this.rjLabel25.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel25.TabIndex = 2119;
            this.rjLabel25.Text = "TOTAL :";
            // 
            // rjLabel4
            // 
            this.rjLabel4.AutoSize = true;
            this.rjLabel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel4.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjLabel4.LinkLabel = false;
            this.rjLabel4.Location = new System.Drawing.Point(188, 6);
            this.rjLabel4.Name = "rjLabel4";
            this.rjLabel4.Size = new System.Drawing.Size(79, 34);
            this.rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel4.TabIndex = 2121;
            this.rjLabel4.Text = "OK :";
            // 
            // rjLabel7
            // 
            this.rjLabel7.AutoSize = true;
            this.rjLabel7.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel7.Font = new System.Drawing.Font("Consolas", 35.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjLabel7.LinkLabel = false;
            this.rjLabel7.Location = new System.Drawing.Point(235, 54);
            this.rjLabel7.Name = "rjLabel7";
            this.rjLabel7.Size = new System.Drawing.Size(76, 55);
            this.rjLabel7.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel7.TabIndex = 2126;
            this.rjLabel7.Text = "OK";
            // 
            // rjLabel1
            // 
            this.rjLabel1.AutoSize = true;
            this.rjLabel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel1.Font = new System.Drawing.Font("Consolas", 35.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel1.LinkLabel = false;
            this.rjLabel1.Location = new System.Drawing.Point(10, 53);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(232, 55);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel1.TabIndex = 2125;
            this.rjLabel1.Text = "VISION :";
            // 
            // rjLabel6
            // 
            this.rjLabel6.AutoSize = true;
            this.rjLabel6.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel6.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel6.ForeColor = System.Drawing.Color.Red;
            this.rjLabel6.LinkLabel = false;
            this.rjLabel6.Location = new System.Drawing.Point(331, 6);
            this.rjLabel6.Name = "rjLabel6";
            this.rjLabel6.Size = new System.Drawing.Size(79, 34);
            this.rjLabel6.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel6.TabIndex = 2123;
            this.rjLabel6.Text = "NG :";
            // 
            // rjPanel3
            // 
            this.rjPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel3.BorderRadius = 5;
            this.rjPanel3.Controls.Add(this.lbEncoder);
            this.rjPanel3.Controls.Add(this.rjLabel14);
            this.rjPanel3.Customizable = false;
            this.rjPanel3.Location = new System.Drawing.Point(323, 215);
            this.rjPanel3.Name = "rjPanel3";
            this.rjPanel3.Size = new System.Drawing.Size(160, 57);
            this.rjPanel3.TabIndex = 2138;
            // 
            // lbEncoder
            // 
            this.lbEncoder.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbEncoder.Font = new System.Drawing.Font("Verdana", 14F);
            this.lbEncoder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.lbEncoder.LinkLabel = false;
            this.lbEncoder.Location = new System.Drawing.Point(5, 27);
            this.lbEncoder.Name = "lbEncoder";
            this.lbEncoder.Size = new System.Drawing.Size(152, 23);
            this.lbEncoder.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lbEncoder.TabIndex = 1;
            this.lbEncoder.Text = "-";
            this.lbEncoder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel14
            // 
            this.rjLabel14.AutoSize = true;
            this.rjLabel14.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel14.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel14.LinkLabel = false;
            this.rjLabel14.Location = new System.Drawing.Point(54, 9);
            this.rjLabel14.Name = "rjLabel14";
            this.rjLabel14.Size = new System.Drawing.Size(68, 18);
            this.rjLabel14.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel14.TabIndex = 0;
            this.rjLabel14.Text = "Encoder";
            // 
            // FormMainSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(489, 861);
            this.Controls.Add(this.rjPanel9);
            this.Controls.Add(this.rjPanel8);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.rjPanel5);
            this.Controls.Add(this.rjPanel7);
            this.Controls.Add(this.rjPanel1);
            this.Controls.Add(this.rjPanel4);
            this.Controls.Add(this.rjPanel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormMainSystem";
            this.Text = "System";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLabelDefct)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttachLabel)).EndInit();
            this.rjPanel9.ResumeLayout(false);
            this.rjPanel9.PerformLayout();
            this.rjPanel8.ResumeLayout(false);
            this.rjPanel8.PerformLayout();
            this.rjPanel5.ResumeLayout(false);
            this.rjPanel5.PerformLayout();
            this.rjPanel7.ResumeLayout(false);
            this.rjPanel7.PerformLayout();
            this.rjPanel1.ResumeLayout(false);
            this.rjPanel6.ResumeLayout(false);
            this.rjPanel6.PerformLayout();
            this.rjPanel2.ResumeLayout(false);
            this.rjPanel2.PerformLayout();
            this.rjPanel4.ResumeLayout(false);
            this.rjPanel4.PerformLayout();
            this.rjPanel3.ResumeLayout(false);
            this.rjPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel4;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel2;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel5;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel25;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel7;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel6;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel3;
        private RJCodeUI_M1.RJControls.RJLabel lbEncoder;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel14;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel6;
        private RJCodeUI_M1.RJControls.RJLabel lbEncoderCam1;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel16;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel2;
        private RJCodeUI_M1.RJControls.RJLabel lbEncoderCam2;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel15;
        private System.Windows.Forms.Timer timerDateTime;
        private System.Windows.Forms.Timer timerConnection;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private RJCodeUI_M1.RJControls.RJLabel lbMenu;
        private System.Windows.Forms.Timer timerEncoder;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel5;
        private RJCodeUI_M1.RJControls.RJLabel lbTotalDisM;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel18;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel7;
        private RJCodeUI_M1.RJControls.RJLabel lbTotalDis;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel20;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Timer timerCountUp;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvLabelDefct;
        private System.Windows.Forms.Panel panel3;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvAttachLabel;
        private RJCodeUI_M1.RJControls.RJButton btnReset;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel8;
        private RJCodeUI_M1.RJControls.RJLabel lbLotNO;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel19;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel9;
        private RJCodeUI_M1.RJControls.RJLabel lbPlcMotionSpeed;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel21;
    }
}