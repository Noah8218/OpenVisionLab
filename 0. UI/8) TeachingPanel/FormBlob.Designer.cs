using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormBlob
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
            Manina.Windows.Forms.ImageListViewColor imageListViewColor1 = new Manina.Windows.Forms.ImageListViewColor();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBlob));
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader1 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader2 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader3 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader4 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader5 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader6 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader7 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader8 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader9 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader imageListViewColumnHeader10 = new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader();
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvDefect = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnTotalRun = new RJCodeUI_M1.RJControls.RJButton();
            this.btnBlobRun = new RJCodeUI_M1.RJControls.RJButton();
            this.imageListView1 = new Manina.Windows.Forms.ImageListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.rendererToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.renderertoolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.colorToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.thumbnailsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.galleryToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.horizontalStripToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.verticalStripToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.clearThumbsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.x96ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x120ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x200ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefect)).BeginInit();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvDefect);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.splitContainer1.Panel2.Controls.Add(this.toolStripContainer1);
            this.splitContainer1.Size = new System.Drawing.Size(612, 395);
            this.splitContainer1.SplitterDistance = 101;
            this.splitContainer1.TabIndex = 1953;
            // 
            // dgvDefect
            // 
            this.dgvDefect.AllowUserToResizeRows = false;
            this.dgvDefect.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dgvDefect.AlternatingRowsColorApply = false;
            this.dgvDefect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDefect.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvDefect.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvDefect.BorderRadius = 13;
            this.dgvDefect.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDefect.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvDefect.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dgvDefect.ColumnHeaderFont = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvDefect.ColumnHeaderHeight = 40;
            this.dgvDefect.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDefect.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDefect.ColumnHeadersHeight = 40;
            this.dgvDefect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDefect.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dgvDefect.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDefect.Customizable = false;
            this.dgvDefect.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvDefect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDefect.EnableHeadersVisualStyles = false;
            this.dgvDefect.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvDefect.Location = new System.Drawing.Point(0, 0);
            this.dgvDefect.Name = "dgvDefect";
            this.dgvDefect.ReadOnly = true;
            this.dgvDefect.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvDefect.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dgvDefect.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDefect.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDefect.RowHeadersVisible = false;
            this.dgvDefect.RowHeadersWidth = 30;
            this.dgvDefect.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvDefect.RowHeight = 40;
            this.dgvDefect.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            this.dgvDefect.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDefect.RowsTextColor = System.Drawing.Color.Gray;
            this.dgvDefect.RowTemplate.Height = 40;
            this.dgvDefect.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dgvDefect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDefect.SelectionTextColor = System.Drawing.Color.Gray;
            this.dgvDefect.Size = new System.Drawing.Size(612, 101);
            this.dgvDefect.TabIndex = 2137;
            this.dgvDefect.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSeletecList_CellClick);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.StatusStrip);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.btnTotalRun);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.btnBlobRun);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.imageListView1);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(612, 243);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(612, 290);
            this.toolStripContainer1.TabIndex = 2139;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 0);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(612, 22);
            this.StatusStrip.TabIndex = 0;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(39, 17);
            this.StatusLabel.Text = "Ready";
            // 
            // btnTotalRun
            // 
            this.btnTotalRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTotalRun.BackColor = System.Drawing.Color.White;
            this.btnTotalRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTotalRun.BorderRadius = 15;
            this.btnTotalRun.BorderSize = 3;
            this.btnTotalRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTotalRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnTotalRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnTotalRun.FlatAppearance.BorderSize = 3;
            this.btnTotalRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnTotalRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnTotalRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTotalRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTotalRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTotalRun.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.btnTotalRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTotalRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTotalRun.IconSize = 80;
            this.btnTotalRun.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnTotalRun.Location = new System.Drawing.Point(463, 121);
            this.btnTotalRun.Name = "btnTotalRun";
            this.btnTotalRun.Size = new System.Drawing.Size(137, 116);
            this.btnTotalRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnTotalRun.TabIndex = 2139;
            this.btnTotalRun.Text = "전체 검사";
            this.btnTotalRun.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnTotalRun.UseVisualStyleBackColor = false;
            this.btnTotalRun.Click += new System.EventHandler(this.btnTotalRun_Click);
            // 
            // btnBlobRun
            // 
            this.btnBlobRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBlobRun.BackColor = System.Drawing.Color.White;
            this.btnBlobRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnBlobRun.BorderRadius = 15;
            this.btnBlobRun.BorderSize = 3;
            this.btnBlobRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBlobRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnBlobRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnBlobRun.FlatAppearance.BorderSize = 3;
            this.btnBlobRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnBlobRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnBlobRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBlobRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBlobRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnBlobRun.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.btnBlobRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnBlobRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBlobRun.IconSize = 80;
            this.btnBlobRun.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBlobRun.Location = new System.Drawing.Point(463, 3);
            this.btnBlobRun.Name = "btnBlobRun";
            this.btnBlobRun.Size = new System.Drawing.Size(137, 116);
            this.btnBlobRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnBlobRun.TabIndex = 2138;
            this.btnBlobRun.Text = "단일 검사";
            this.btnBlobRun.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBlobRun.UseVisualStyleBackColor = false;
            this.btnBlobRun.Click += new System.EventHandler(this.btnBlobRun_Click);
            // 
            // imageListView1
            // 
            this.imageListView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.imageListView1.CheckBoxAlignment = System.Drawing.ContentAlignment.TopLeft;
            imageListViewColor1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.ColumnHeaderBackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.ColumnHeaderBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.ColumnHeaderHoverColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.ColumnHeaderHoverColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.ColumnSelectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.ColumnSeparatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.ControlBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.DisabledColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.DisabledColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.HoverColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.HoverColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.PaneBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.PaneSeparatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.SelectedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.SelectedColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.SelectedColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.SelectionRectangleColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.SelectionRectangleColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.UnFocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.UnFocusedColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            imageListViewColor1.UnFocusedColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.imageListView1.Colors = imageListViewColor1;
            this.imageListView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.imageListView1.Location = new System.Drawing.Point(0, 0);
            this.imageListView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.imageListView1.Name = "imageListView1";
            this.imageListView1.PersistentCacheDirectory = "";
            this.imageListView1.PersistentCacheSize = ((long)(100));
            this.imageListView1.Size = new System.Drawing.Size(459, 243);
            this.imageListView1.TabIndex = 2138;
            this.imageListView1.UseWIC = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rendererToolStripLabel,
            this.renderertoolStripComboBox,
            this.colorToolStripComboBox,
            this.toolStripSeparator2,
            this.thumbnailsToolStripButton,
            this.galleryToolStripButton,
            this.horizontalStripToolStripButton,
            this.verticalStripToolStripButton,
            this.toolStripSeparator3,
            this.clearThumbsToolStripButton,
            this.toolStripDropDownButton1,
            this.toolStripSeparator4});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(248, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // rendererToolStripLabel
            // 
            this.rendererToolStripLabel.Name = "rendererToolStripLabel";
            this.rendererToolStripLabel.Size = new System.Drawing.Size(57, 22);
            this.rendererToolStripLabel.Text = "Renderer:";
            this.rendererToolStripLabel.Visible = false;
            // 
            // renderertoolStripComboBox
            // 
            this.renderertoolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.renderertoolStripComboBox.Name = "renderertoolStripComboBox";
            this.renderertoolStripComboBox.Size = new System.Drawing.Size(121, 25);
            this.renderertoolStripComboBox.Visible = false;
            this.renderertoolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.renderertoolStripComboBox_SelectedIndexChanged);
            // 
            // colorToolStripComboBox
            // 
            this.colorToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorToolStripComboBox.Name = "colorToolStripComboBox";
            this.colorToolStripComboBox.Size = new System.Drawing.Size(121, 25);
            this.colorToolStripComboBox.Visible = false;
            this.colorToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.colorToolStripComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // thumbnailsToolStripButton
            // 
            this.thumbnailsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.thumbnailsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("thumbnailsToolStripButton.Image")));
            this.thumbnailsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.thumbnailsToolStripButton.Name = "thumbnailsToolStripButton";
            this.thumbnailsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.thumbnailsToolStripButton.Text = "Thumbnails";
            this.thumbnailsToolStripButton.Click += new System.EventHandler(this.thumbnailsToolStripButton_Click);
            // 
            // galleryToolStripButton
            // 
            this.galleryToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.galleryToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("galleryToolStripButton.Image")));
            this.galleryToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.galleryToolStripButton.Name = "galleryToolStripButton";
            this.galleryToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.galleryToolStripButton.Text = "Gallery";
            this.galleryToolStripButton.Click += new System.EventHandler(this.galleryToolStripButton_Click);
            // 
            // horizontalStripToolStripButton
            // 
            this.horizontalStripToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.horizontalStripToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("horizontalStripToolStripButton.Image")));
            this.horizontalStripToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.horizontalStripToolStripButton.Name = "horizontalStripToolStripButton";
            this.horizontalStripToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.horizontalStripToolStripButton.Text = "Horizontal Strip";
            this.horizontalStripToolStripButton.Click += new System.EventHandler(this.horizontalStripToolStripButton_Click);
            // 
            // verticalStripToolStripButton
            // 
            this.verticalStripToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.verticalStripToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("verticalStripToolStripButton.Image")));
            this.verticalStripToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.verticalStripToolStripButton.Name = "verticalStripToolStripButton";
            this.verticalStripToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.verticalStripToolStripButton.Text = "Vertical Strip";
            this.verticalStripToolStripButton.Click += new System.EventHandler(this.verticalStripToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // clearThumbsToolStripButton
            // 
            this.clearThumbsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearThumbsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("clearThumbsToolStripButton.Image")));
            this.clearThumbsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearThumbsToolStripButton.Name = "clearThumbsToolStripButton";
            this.clearThumbsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.clearThumbsToolStripButton.Text = "Clear Thumbnail Cache";
            this.clearThumbsToolStripButton.Click += new System.EventHandler(this.clearThumbsToolStripButton_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.x96ToolStripMenuItem,
            this.x120ToolStripMenuItem,
            this.x200ToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(103, 22);
            this.toolStripDropDownButton1.Text = "Thumbnail Size";
            // 
            // x96ToolStripMenuItem
            // 
            this.x96ToolStripMenuItem.Name = "x96ToolStripMenuItem";
            this.x96ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.x96ToolStripMenuItem.Text = "96x96";
            this.x96ToolStripMenuItem.Click += new System.EventHandler(this.x96ToolStripMenuItem_Click);
            // 
            // x120ToolStripMenuItem
            // 
            this.x120ToolStripMenuItem.Name = "x120ToolStripMenuItem";
            this.x120ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.x120ToolStripMenuItem.Text = "120x120";
            // 
            // x200ToolStripMenuItem
            // 
            this.x200ToolStripMenuItem.Name = "x200ToolStripMenuItem";
            this.x200ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.x200ToolStripMenuItem.Text = "200x200";
            this.x200ToolStripMenuItem.Click += new System.EventHandler(this.x200ToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // imageListView
            // 
            this.imageListView.AllowDuplicateFileNames = true;
            imageListViewColumnHeader1.Comparer = null;
            imageListViewColumnHeader1.DisplayIndex = 0;
            imageListViewColumnHeader1.Grouper = null;
            imageListViewColumnHeader1.Key = "";
            imageListViewColumnHeader1.Type = Manina.Windows.Forms.ColumnType.Name;
            imageListViewColumnHeader2.Comparer = null;
            imageListViewColumnHeader2.DisplayIndex = 1;
            imageListViewColumnHeader2.Grouper = null;
            imageListViewColumnHeader2.Key = "";
            imageListViewColumnHeader2.Type = Manina.Windows.Forms.ColumnType.FileType;
            imageListViewColumnHeader3.Comparer = null;
            imageListViewColumnHeader3.DisplayIndex = 2;
            imageListViewColumnHeader3.Grouper = null;
            imageListViewColumnHeader3.Key = "";
            imageListViewColumnHeader3.Type = Manina.Windows.Forms.ColumnType.FileSize;
            imageListViewColumnHeader4.Comparer = null;
            imageListViewColumnHeader4.DisplayIndex = 3;
            imageListViewColumnHeader4.Grouper = null;
            imageListViewColumnHeader4.Key = "";
            imageListViewColumnHeader4.Type = Manina.Windows.Forms.ColumnType.DateModified;
            imageListViewColumnHeader5.Comparer = null;
            imageListViewColumnHeader5.DisplayIndex = 4;
            imageListViewColumnHeader5.Grouper = null;
            imageListViewColumnHeader5.Key = "";
            imageListViewColumnHeader5.Type = Manina.Windows.Forms.ColumnType.DateTaken;
            imageListViewColumnHeader6.Comparer = null;
            imageListViewColumnHeader6.DisplayIndex = 5;
            imageListViewColumnHeader6.Grouper = null;
            imageListViewColumnHeader6.Key = "";
            imageListViewColumnHeader6.Type = Manina.Windows.Forms.ColumnType.ExposureTime;
            imageListViewColumnHeader7.Comparer = null;
            imageListViewColumnHeader7.DisplayIndex = 6;
            imageListViewColumnHeader7.Grouper = null;
            imageListViewColumnHeader7.Key = "";
            imageListViewColumnHeader7.Type = Manina.Windows.Forms.ColumnType.FNumber;
            imageListViewColumnHeader8.Comparer = null;
            imageListViewColumnHeader8.DisplayIndex = 7;
            imageListViewColumnHeader8.Grouper = null;
            imageListViewColumnHeader8.Key = "";
            imageListViewColumnHeader8.Type = Manina.Windows.Forms.ColumnType.ISOSpeed;
            imageListViewColumnHeader9.Comparer = null;
            imageListViewColumnHeader9.DisplayIndex = 8;
            imageListViewColumnHeader9.Grouper = null;
            imageListViewColumnHeader9.Key = "";
            imageListViewColumnHeader9.Type = Manina.Windows.Forms.ColumnType.Rating;
            imageListViewColumnHeader10.Comparer = null;
            imageListViewColumnHeader10.DisplayIndex = 9;
            imageListViewColumnHeader10.Grouper = null;
            imageListViewColumnHeader10.Key = "Custom";
            imageListViewColumnHeader10.Type = Manina.Windows.Forms.ColumnType.Custom;
            this.imageListView.Columns.AddRange(new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader[] {
            imageListViewColumnHeader1,
            imageListViewColumnHeader2,
            imageListViewColumnHeader3,
            imageListViewColumnHeader4,
            imageListViewColumnHeader5,
            imageListViewColumnHeader6,
            imageListViewColumnHeader7,
            imageListViewColumnHeader8,
            imageListViewColumnHeader9,
            imageListViewColumnHeader10});
            this.imageListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageListView.Location = new System.Drawing.Point(0, 0);
            this.imageListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.imageListView.Name = "imageListView";
            this.imageListView.PersistentCacheDirectory = "";
            this.imageListView.PersistentCacheSize = ((long)(100));
            this.imageListView.Size = new System.Drawing.Size(407, 311);
            this.imageListView.TabIndex = 0;
            this.imageListView.UseWIC = true;
            // 
            // FormBlob
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(612, 395);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormBlob";
            this.Text = "Defect";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefect)).EndInit();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private RJCodeUI_M1.RJControls.RJButton btnBlobRun;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvDefect;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Manina.Windows.Forms.ImageListView imageListView1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private Manina.Windows.Forms.ImageListView imageListView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel rendererToolStripLabel;
        private System.Windows.Forms.ToolStripComboBox renderertoolStripComboBox;
        private System.Windows.Forms.ToolStripComboBox colorToolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton thumbnailsToolStripButton;
        private System.Windows.Forms.ToolStripButton galleryToolStripButton;
        private System.Windows.Forms.ToolStripButton horizontalStripToolStripButton;
        private System.Windows.Forms.ToolStripButton verticalStripToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton clearThumbsToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem x96ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x120ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x200ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private RJCodeUI_M1.RJControls.RJButton btnTotalRun;
    }
}