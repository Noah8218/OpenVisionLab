using Cyotek.Windows.Forms;

namespace OpenVisionLab
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSystem));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.rjButton3 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton2 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjDataGridView1 = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.dgvResult2 = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.dgvResult1 = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.btnBlobRun = new RJCodeUI_M1.RJControls.RJButton();
            this.rjPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rjDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult1)).BeginInit();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            // 
            // rjPanel1
            // 
            this.rjPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel1.BorderRadius = 5;
            this.rjPanel1.Controls.Add(this.rjButton3);
            this.rjPanel1.Controls.Add(this.rjButton2);
            this.rjPanel1.Controls.Add(this.rjDataGridView1);
            this.rjPanel1.Controls.Add(this.dgvResult2);
            this.rjPanel1.Controls.Add(this.dgvResult1);
            this.rjPanel1.Controls.Add(this.btnBlobRun);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.rjPanel1.Location = new System.Drawing.Point(0, 0);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(745, 519);
            this.rjPanel1.TabIndex = 2144;
            this.rjPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.rjPanel1_Paint);
            // 
            // rjButton3
            // 
            this.rjButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rjButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton3.BorderRadius = 15;
            this.rjButton3.BorderSize = 3;
            this.rjButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton3.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.rjButton3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton3.FlatAppearance.BorderSize = 0;
            this.rjButton3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.rjButton3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.rjButton3.ForeColor = System.Drawing.Color.White;
            this.rjButton3.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.rjButton3.IconColor = System.Drawing.Color.White;
            this.rjButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton3.IconSize = 80;
            this.rjButton3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rjButton3.Location = new System.Drawing.Point(568, 324);
            this.rjButton3.Name = "rjButton3";
            this.rjButton3.Size = new System.Drawing.Size(165, 116);
            this.rjButton3.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.rjButton3.TabIndex = 2154;
            this.rjButton3.Text = "칩 매핑";
            this.rjButton3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rjButton3.UseVisualStyleBackColor = false;
            this.rjButton3.Visible = false;
            this.rjButton3.Click += new System.EventHandler(this.rjButton2_Click);
            // 
            // rjButton2
            // 
            this.rjButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rjButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton2.BorderRadius = 15;
            this.rjButton2.BorderSize = 3;
            this.rjButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton2.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.rjButton2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton2.FlatAppearance.BorderSize = 0;
            this.rjButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.rjButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.rjButton2.ForeColor = System.Drawing.Color.White;
            this.rjButton2.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.rjButton2.IconColor = System.Drawing.Color.White;
            this.rjButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton2.IconSize = 80;
            this.rjButton2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rjButton2.Location = new System.Drawing.Point(568, 168);
            this.rjButton2.Name = "rjButton2";
            this.rjButton2.Size = new System.Drawing.Size(165, 116);
            this.rjButton2.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.rjButton2.TabIndex = 2153;
            this.rjButton2.Text = "제거 후 칩 검사";
            this.rjButton2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rjButton2.UseVisualStyleBackColor = false;
            this.rjButton2.Visible = false;
            this.rjButton2.Click += new System.EventHandler(this.rjButton1_Click);
            // 
            // rjDataGridView1
            // 
            this.rjDataGridView1.AllowUserToAddRows = false;
            this.rjDataGridView1.AllowUserToDeleteRows = false;
            this.rjDataGridView1.AllowUserToResizeRows = false;
            this.rjDataGridView1.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.rjDataGridView1.AlternatingRowsColorApply = false;
            this.rjDataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjDataGridView1.BorderRadius = 13;
            this.rjDataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rjDataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.rjDataGridView1.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.rjDataGridView1.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjDataGridView1.ColumnHeaderHeight = 40;
            this.rjDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.rjDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.rjDataGridView1.ColumnHeadersHeight = 40;
            this.rjDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.rjDataGridView1.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.rjDataGridView1.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.rjDataGridView1.Customizable = false;
            this.rjDataGridView1.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjDataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.rjDataGridView1.EnableHeadersVisualStyles = false;
            this.rjDataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rjDataGridView1.Location = new System.Drawing.Point(3, 324);
            this.rjDataGridView1.Name = "rjDataGridView1";
            this.rjDataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rjDataGridView1.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.rjDataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.rjDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.rjDataGridView1.RowHeadersVisible = false;
            this.rjDataGridView1.RowHeadersWidth = 30;
            this.rjDataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.rjDataGridView1.RowHeight = 40;
            this.rjDataGridView1.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            this.rjDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.rjDataGridView1.RowsTextColor = System.Drawing.Color.Gray;
            this.rjDataGridView1.RowTemplate.Height = 40;
            this.rjDataGridView1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.rjDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.rjDataGridView1.SelectionTextColor = System.Drawing.Color.Gray;
            this.rjDataGridView1.Size = new System.Drawing.Size(490, 150);
            this.rjDataGridView1.TabIndex = 2152;
            // 
            // dgvResult2
            // 
            this.dgvResult2.AllowUserToAddRows = false;
            this.dgvResult2.AllowUserToDeleteRows = false;
            this.dgvResult2.AllowUserToResizeRows = false;
            this.dgvResult2.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dgvResult2.AlternatingRowsColorApply = false;
            this.dgvResult2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvResult2.BorderRadius = 13;
            this.dgvResult2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvResult2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvResult2.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dgvResult2.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvResult2.ColumnHeaderHeight = 40;
            this.dgvResult2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvResult2.ColumnHeadersHeight = 40;
            this.dgvResult2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvResult2.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dgvResult2.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvResult2.Customizable = false;
            this.dgvResult2.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvResult2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvResult2.EnableHeadersVisualStyles = false;
            this.dgvResult2.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvResult2.Location = new System.Drawing.Point(3, 168);
            this.dgvResult2.Name = "dgvResult2";
            this.dgvResult2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvResult2.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dgvResult2.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResult2.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvResult2.RowHeadersVisible = false;
            this.dgvResult2.RowHeadersWidth = 30;
            this.dgvResult2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvResult2.RowHeight = 40;
            this.dgvResult2.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Gray;
            this.dgvResult2.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvResult2.RowsTextColor = System.Drawing.Color.Gray;
            this.dgvResult2.RowTemplate.Height = 40;
            this.dgvResult2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dgvResult2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResult2.SelectionTextColor = System.Drawing.Color.Gray;
            this.dgvResult2.Size = new System.Drawing.Size(490, 150);
            this.dgvResult2.TabIndex = 2151;
            this.dgvResult2.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSeletecList_CellClick2);
            // 
            // dgvResult1
            // 
            this.dgvResult1.AllowUserToAddRows = false;
            this.dgvResult1.AllowUserToDeleteRows = false;
            this.dgvResult1.AllowUserToResizeRows = false;
            this.dgvResult1.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dgvResult1.AlternatingRowsColorApply = false;
            this.dgvResult1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvResult1.BorderRadius = 13;
            this.dgvResult1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvResult1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvResult1.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dgvResult1.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvResult1.ColumnHeaderHeight = 40;
            this.dgvResult1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvResult1.ColumnHeadersHeight = 40;
            this.dgvResult1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvResult1.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dgvResult1.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvResult1.Customizable = false;
            this.dgvResult1.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvResult1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvResult1.EnableHeadersVisualStyles = false;
            this.dgvResult1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvResult1.Location = new System.Drawing.Point(3, 12);
            this.dgvResult1.Name = "dgvResult1";
            this.dgvResult1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvResult1.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dgvResult1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResult1.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvResult1.RowHeadersVisible = false;
            this.dgvResult1.RowHeadersWidth = 30;
            this.dgvResult1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvResult1.RowHeight = 40;
            this.dgvResult1.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Gray;
            this.dgvResult1.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvResult1.RowsTextColor = System.Drawing.Color.Gray;
            this.dgvResult1.RowTemplate.Height = 40;
            this.dgvResult1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dgvResult1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResult1.SelectionTextColor = System.Drawing.Color.Gray;
            this.dgvResult1.Size = new System.Drawing.Size(490, 150);
            this.dgvResult1.TabIndex = 2150;
            this.dgvResult1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSeletecList_CellClick);
            // 
            // btnBlobRun
            // 
            this.btnBlobRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBlobRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnBlobRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnBlobRun.BorderRadius = 15;
            this.btnBlobRun.BorderSize = 3;
            this.btnBlobRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBlobRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnBlobRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnBlobRun.FlatAppearance.BorderSize = 0;
            this.btnBlobRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.btnBlobRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.btnBlobRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBlobRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.btnBlobRun.ForeColor = System.Drawing.Color.White;
            this.btnBlobRun.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.btnBlobRun.IconColor = System.Drawing.Color.White;
            this.btnBlobRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBlobRun.IconSize = 80;
            this.btnBlobRun.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBlobRun.Location = new System.Drawing.Point(568, 12);
            this.btnBlobRun.Name = "btnBlobRun";
            this.btnBlobRun.Size = new System.Drawing.Size(165, 116);
            this.btnBlobRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnBlobRun.TabIndex = 2147;
            this.btnBlobRun.Text = "제거 전 칩 검사";
            this.btnBlobRun.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBlobRun.UseVisualStyleBackColor = false;
            this.btnBlobRun.Visible = false;
            this.btnBlobRun.Click += new System.EventHandler(this.btnBlobRun_Click);
            // 
            // FormSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(745, 519);
            this.Controls.Add(this.rjPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormSystem";
            this.Text = "System";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.rjPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rjDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private RJCodeUI_M1.RJControls.RJButton btnBlobRun;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvResult1;
        private RJCodeUI_M1.RJControls.RJDataGridView rjDataGridView1;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvResult2;
        private RJCodeUI_M1.RJControls.RJButton rjButton3;
        private RJCodeUI_M1.RJControls.RJButton rjButton2;
    }
}