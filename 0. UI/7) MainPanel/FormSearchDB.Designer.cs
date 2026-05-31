using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormSearchDB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSearchDB));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.rjButton6 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton5 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton4 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton3 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton2 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjPanel2 = new RJCodeUI_M1.RJControls.RJPanel();
            this.dgvSearchDB = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.btnSearchData = new RJCodeUI_M1.RJControls.RJButton();
            this.EndPickerTime = new System.Windows.Forms.DateTimePicker();
            this.StartPickerTime = new System.Windows.Forms.DateTimePicker();
            this.rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
            this.EndPicker = new RJCodeUI_M1.RJControls.RJDatePicker();
            this.rjLabel12 = new RJCodeUI_M1.RJControls.RJLabel();
            this.StartPicker = new RJCodeUI_M1.RJControls.RJDatePicker();
            this.btnRestoreDB = new RJCodeUI_M1.RJControls.RJButton();
            this.btnBackupDB = new RJCodeUI_M1.RJControls.RJButton();
            this.btnSearchLot = new RJCodeUI_M1.RJControls.RJButton();
            this.tbSearch = new RJCodeUI_M1.RJControls.RJTextBox();
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.cbPeriod = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnSaveCsv = new RJCodeUI_M1.RJControls.RJButton();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.rjPanel1.SuspendLayout();
            this.rjPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchDB)).BeginInit();
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
            this.rjPanel1.Controls.Add(this.btnSaveCsv);
            this.rjPanel1.Controls.Add(this.rjButton6);
            this.rjPanel1.Controls.Add(this.rjButton5);
            this.rjPanel1.Controls.Add(this.rjButton4);
            this.rjPanel1.Controls.Add(this.rjButton3);
            this.rjPanel1.Controls.Add(this.rjButton2);
            this.rjPanel1.Controls.Add(this.rjPanel2);
            this.rjPanel1.Controls.Add(this.btnSearchData);
            this.rjPanel1.Controls.Add(this.EndPickerTime);
            this.rjPanel1.Controls.Add(this.StartPickerTime);
            this.rjPanel1.Controls.Add(this.rjLabel4);
            this.rjPanel1.Controls.Add(this.EndPicker);
            this.rjPanel1.Controls.Add(this.rjLabel12);
            this.rjPanel1.Controls.Add(this.StartPicker);
            this.rjPanel1.Controls.Add(this.btnRestoreDB);
            this.rjPanel1.Controls.Add(this.btnBackupDB);
            this.rjPanel1.Controls.Add(this.btnSearchLot);
            this.rjPanel1.Controls.Add(this.tbSearch);
            this.rjPanel1.Controls.Add(this.rjLabel3);
            this.rjPanel1.Controls.Add(this.rjLabel1);
            this.rjPanel1.Controls.Add(this.cbPeriod);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rjPanel1.Location = new System.Drawing.Point(0, 0);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(704, 695);
            this.rjPanel1.TabIndex = 2144;
            // 
            // rjButton6
            // 
            this.rjButton6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton6.BorderRadius = 10;
            this.rjButton6.BorderSize = 1;
            this.rjButton6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton6.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.rjButton6.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton6.FlatAppearance.BorderSize = 0;
            this.rjButton6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.rjButton6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.rjButton6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton6.ForeColor = System.Drawing.Color.White;
            this.rjButton6.IconChar = FontAwesome.Sharp.IconChar.None;
            this.rjButton6.IconColor = System.Drawing.Color.White;
            this.rjButton6.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton6.IconSize = 24;
            this.rjButton6.Location = new System.Drawing.Point(129, 605);
            this.rjButton6.Name = "rjButton6";
            this.rjButton6.Size = new System.Drawing.Size(114, 40);
            this.rjButton6.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.rjButton6.TabIndex = 2201;
            this.rjButton6.Text = "+1 Day";
            this.rjButton6.UseVisualStyleBackColor = false;
            this.rjButton6.Click += new System.EventHandler(this.btnDayEdit);
            // 
            // rjButton5
            // 
            this.rjButton5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton5.BorderRadius = 10;
            this.rjButton5.BorderSize = 1;
            this.rjButton5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton5.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.rjButton5.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton5.FlatAppearance.BorderSize = 0;
            this.rjButton5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.rjButton5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.rjButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton5.ForeColor = System.Drawing.Color.White;
            this.rjButton5.IconChar = FontAwesome.Sharp.IconChar.None;
            this.rjButton5.IconColor = System.Drawing.Color.White;
            this.rjButton5.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton5.IconSize = 24;
            this.rjButton5.Location = new System.Drawing.Point(12, 605);
            this.rjButton5.Name = "rjButton5";
            this.rjButton5.Size = new System.Drawing.Size(114, 40);
            this.rjButton5.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.rjButton5.TabIndex = 2200;
            this.rjButton5.Text = "-1 Day";
            this.rjButton5.UseVisualStyleBackColor = false;
            this.rjButton5.Click += new System.EventHandler(this.btnDayEdit);
            // 
            // rjButton4
            // 
            this.rjButton4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton4.BorderRadius = 10;
            this.rjButton4.BorderSize = 1;
            this.rjButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton4.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.rjButton4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton4.FlatAppearance.BorderSize = 0;
            this.rjButton4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.rjButton4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton4.ForeColor = System.Drawing.Color.White;
            this.rjButton4.IconChar = FontAwesome.Sharp.IconChar.None;
            this.rjButton4.IconColor = System.Drawing.Color.White;
            this.rjButton4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton4.IconSize = 24;
            this.rjButton4.Location = new System.Drawing.Point(249, 559);
            this.rjButton4.Name = "rjButton4";
            this.rjButton4.Size = new System.Drawing.Size(114, 40);
            this.rjButton4.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.rjButton4.TabIndex = 2199;
            this.rjButton4.Text = "Month";
            this.rjButton4.UseVisualStyleBackColor = false;
            this.rjButton4.Click += new System.EventHandler(this.btnDayEdit);
            // 
            // rjButton3
            // 
            this.rjButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton3.BorderRadius = 10;
            this.rjButton3.BorderSize = 1;
            this.rjButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton3.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.rjButton3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton3.FlatAppearance.BorderSize = 0;
            this.rjButton3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.rjButton3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton3.ForeColor = System.Drawing.Color.White;
            this.rjButton3.IconChar = FontAwesome.Sharp.IconChar.None;
            this.rjButton3.IconColor = System.Drawing.Color.White;
            this.rjButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton3.IconSize = 24;
            this.rjButton3.Location = new System.Drawing.Point(129, 559);
            this.rjButton3.Name = "rjButton3";
            this.rjButton3.Size = new System.Drawing.Size(114, 40);
            this.rjButton3.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.rjButton3.TabIndex = 2198;
            this.rjButton3.Text = "Week";
            this.rjButton3.UseVisualStyleBackColor = false;
            this.rjButton3.Click += new System.EventHandler(this.btnDayEdit);
            // 
            // rjButton2
            // 
            this.rjButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton2.BorderRadius = 10;
            this.rjButton2.BorderSize = 1;
            this.rjButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton2.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.rjButton2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton2.FlatAppearance.BorderSize = 0;
            this.rjButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.rjButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton2.ForeColor = System.Drawing.Color.White;
            this.rjButton2.IconChar = FontAwesome.Sharp.IconChar.None;
            this.rjButton2.IconColor = System.Drawing.Color.White;
            this.rjButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton2.IconSize = 24;
            this.rjButton2.Location = new System.Drawing.Point(9, 559);
            this.rjButton2.Name = "rjButton2";
            this.rjButton2.Size = new System.Drawing.Size(114, 40);
            this.rjButton2.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.rjButton2.TabIndex = 2197;
            this.rjButton2.Text = "Today";
            this.rjButton2.UseVisualStyleBackColor = false;
            this.rjButton2.Click += new System.EventHandler(this.btnDayEdit);
            // 
            // rjPanel2
            // 
            this.rjPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel2.BorderRadius = 0;
            this.rjPanel2.Controls.Add(this.dgvSearchDB);
            this.rjPanel2.Customizable = false;
            this.rjPanel2.Location = new System.Drawing.Point(12, 74);
            this.rjPanel2.Name = "rjPanel2";
            this.rjPanel2.Size = new System.Drawing.Size(674, 369);
            this.rjPanel2.TabIndex = 2196;
            // 
            // dgvSearchDB
            // 
            this.dgvSearchDB.AllowUserToAddRows = false;
            this.dgvSearchDB.AllowUserToDeleteRows = false;
            this.dgvSearchDB.AllowUserToResizeRows = false;
            this.dgvSearchDB.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dgvSearchDB.AlternatingRowsColorApply = false;
            this.dgvSearchDB.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSearchDB.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvSearchDB.BorderRadius = 13;
            this.dgvSearchDB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSearchDB.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvSearchDB.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dgvSearchDB.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSearchDB.ColumnHeaderHeight = 40;
            this.dgvSearchDB.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchDB.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearchDB.ColumnHeadersHeight = 40;
            this.dgvSearchDB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSearchDB.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dgvSearchDB.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSearchDB.Customizable = false;
            this.dgvSearchDB.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvSearchDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSearchDB.EnableHeadersVisualStyles = false;
            this.dgvSearchDB.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvSearchDB.Location = new System.Drawing.Point(0, 0);
            this.dgvSearchDB.Name = "dgvSearchDB";
            this.dgvSearchDB.ReadOnly = true;
            this.dgvSearchDB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvSearchDB.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dgvSearchDB.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchDB.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSearchDB.RowHeadersVisible = false;
            this.dgvSearchDB.RowHeadersWidth = 30;
            this.dgvSearchDB.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvSearchDB.RowHeight = 40;
            this.dgvSearchDB.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            this.dgvSearchDB.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSearchDB.RowsTextColor = System.Drawing.Color.Gray;
            this.dgvSearchDB.RowTemplate.Height = 40;
            this.dgvSearchDB.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dgvSearchDB.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSearchDB.SelectionTextColor = System.Drawing.Color.Gray;
            this.dgvSearchDB.Size = new System.Drawing.Size(674, 369);
            this.dgvSearchDB.TabIndex = 9;
            // 
            // btnSearchData
            // 
            this.btnSearchData.BackColor = System.Drawing.Color.White;
            this.btnSearchData.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSearchData.BorderRadius = 15;
            this.btnSearchData.BorderSize = 3;
            this.btnSearchData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchData.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSearchData.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSearchData.FlatAppearance.BorderSize = 3;
            this.btnSearchData.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSearchData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSearchData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchData.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSearchData.IconChar = FontAwesome.Sharp.IconChar.Search;
            this.btnSearchData.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSearchData.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSearchData.IconSize = 80;
            this.btnSearchData.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSearchData.Location = new System.Drawing.Point(415, 465);
            this.btnSearchData.Name = "btnSearchData";
            this.btnSearchData.Size = new System.Drawing.Size(277, 88);
            this.btnSearchData.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSearchData.TabIndex = 2195;
            this.btnSearchData.Text = "Search Data";
            this.btnSearchData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearchData.UseVisualStyleBackColor = false;
            this.btnSearchData.Click += new System.EventHandler(this.btnSearchData_Click);
            // 
            // EndPickerTime
            // 
            this.EndPickerTime.CalendarFont = new System.Drawing.Font("굴림", 9F);
            this.EndPickerTime.CalendarMonthBackground = System.Drawing.SystemColors.WindowFrame;
            this.EndPickerTime.CustomFormat = "yyyy-MM-dd-HH";
            this.EndPickerTime.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.EndPickerTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.EndPickerTime.Location = new System.Drawing.Point(218, 521);
            this.EndPickerTime.Name = "EndPickerTime";
            this.EndPickerTime.ShowUpDown = true;
            this.EndPickerTime.Size = new System.Drawing.Size(191, 32);
            this.EndPickerTime.TabIndex = 2131;
            // 
            // StartPickerTime
            // 
            this.StartPickerTime.CalendarFont = new System.Drawing.Font("굴림", 9F);
            this.StartPickerTime.CalendarMonthBackground = System.Drawing.SystemColors.WindowFrame;
            this.StartPickerTime.CustomFormat = "yyyy-MM-dd-HH";
            this.StartPickerTime.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.StartPickerTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.StartPickerTime.Location = new System.Drawing.Point(218, 465);
            this.StartPickerTime.Name = "StartPickerTime";
            this.StartPickerTime.ShowUpDown = true;
            this.StartPickerTime.Size = new System.Drawing.Size(191, 32);
            this.StartPickerTime.TabIndex = 2130;
            // 
            // rjLabel4
            // 
            this.rjLabel4.AutoSize = true;
            this.rjLabel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel4.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel4.LinkLabel = false;
            this.rjLabel4.Location = new System.Drawing.Point(6, 502);
            this.rjLabel4.Name = "rjLabel4";
            this.rjLabel4.Size = new System.Drawing.Size(31, 16);
            this.rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel4.TabIndex = 2129;
            this.rjLabel4.Text = "End";
            // 
            // EndPicker
            // 
            this.EndPicker.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.EndPicker.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.EndPicker.BorderRadius = 10;
            this.EndPicker.BorderSize = 1;
            this.EndPicker.CustomFormat = null;
            this.EndPicker.Customizable = false;
            this.EndPicker.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.EndPicker.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.EndPicker.Location = new System.Drawing.Point(6, 521);
            this.EndPicker.MinimumSize = new System.Drawing.Size(120, 25);
            this.EndPicker.Name = "EndPicker";
            this.EndPicker.Padding = new System.Windows.Forms.Padding(2);
            this.EndPicker.Size = new System.Drawing.Size(209, 32);
            this.EndPicker.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.EndPicker.TabIndex = 2128;
            this.EndPicker.Value = new System.DateTime(2020, 12, 27, 13, 40, 35, 574);
            // 
            // rjLabel12
            // 
            this.rjLabel12.AutoSize = true;
            this.rjLabel12.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel12.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel12.LinkLabel = false;
            this.rjLabel12.Location = new System.Drawing.Point(3, 446);
            this.rjLabel12.Name = "rjLabel12";
            this.rjLabel12.Size = new System.Drawing.Size(41, 16);
            this.rjLabel12.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel12.TabIndex = 2127;
            this.rjLabel12.Text = "Start";
            // 
            // StartPicker
            // 
            this.StartPicker.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.StartPicker.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.StartPicker.BorderRadius = 10;
            this.StartPicker.BorderSize = 1;
            this.StartPicker.CustomFormat = null;
            this.StartPicker.Customizable = false;
            this.StartPicker.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.StartPicker.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.StartPicker.Location = new System.Drawing.Point(3, 465);
            this.StartPicker.MinimumSize = new System.Drawing.Size(120, 25);
            this.StartPicker.Name = "StartPicker";
            this.StartPicker.Padding = new System.Windows.Forms.Padding(2);
            this.StartPicker.Size = new System.Drawing.Size(209, 32);
            this.StartPicker.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.StartPicker.TabIndex = 2125;
            this.StartPicker.Value = new System.DateTime(2020, 12, 27, 13, 40, 35, 574);
            // 
            // btnRestoreDB
            // 
            this.btnRestoreDB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnRestoreDB.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnRestoreDB.BorderRadius = 10;
            this.btnRestoreDB.BorderSize = 1;
            this.btnRestoreDB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestoreDB.Design = RJCodeUI_M1.RJControls.ButtonDesign.Delete;
            this.btnRestoreDB.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnRestoreDB.FlatAppearance.BorderSize = 0;
            this.btnRestoreDB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(74)))), ((int)(((byte)(77)))));
            this.btnRestoreDB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(69)))), ((int)(((byte)(72)))));
            this.btnRestoreDB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestoreDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestoreDB.ForeColor = System.Drawing.Color.White;
            this.btnRestoreDB.IconChar = FontAwesome.Sharp.IconChar.History;
            this.btnRestoreDB.IconColor = System.Drawing.Color.White;
            this.btnRestoreDB.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnRestoreDB.IconSize = 24;
            this.btnRestoreDB.Location = new System.Drawing.Point(554, 564);
            this.btnRestoreDB.Name = "btnRestoreDB";
            this.btnRestoreDB.Size = new System.Drawing.Size(138, 35);
            this.btnRestoreDB.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnRestoreDB.TabIndex = 2124;
            this.btnRestoreDB.Text = "Restore";
            this.btnRestoreDB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRestoreDB.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRestoreDB.UseVisualStyleBackColor = false;
            this.btnRestoreDB.Click += new System.EventHandler(this.btnRestoreDB_Click);
            // 
            // btnBackupDB
            // 
            this.btnBackupDB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnBackupDB.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnBackupDB.BorderRadius = 10;
            this.btnBackupDB.BorderSize = 1;
            this.btnBackupDB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackupDB.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnBackupDB.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnBackupDB.FlatAppearance.BorderSize = 0;
            this.btnBackupDB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(149)))), ((int)(((byte)(106)))));
            this.btnBackupDB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(139)))), ((int)(((byte)(99)))));
            this.btnBackupDB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackupDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackupDB.ForeColor = System.Drawing.Color.White;
            this.btnBackupDB.IconChar = FontAwesome.Sharp.IconChar.CloudUploadAlt;
            this.btnBackupDB.IconColor = System.Drawing.Color.White;
            this.btnBackupDB.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBackupDB.IconSize = 24;
            this.btnBackupDB.Location = new System.Drawing.Point(415, 564);
            this.btnBackupDB.Name = "btnBackupDB";
            this.btnBackupDB.Size = new System.Drawing.Size(138, 35);
            this.btnBackupDB.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnBackupDB.TabIndex = 2123;
            this.btnBackupDB.Text = "Backup";
            this.btnBackupDB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBackupDB.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBackupDB.UseVisualStyleBackColor = false;
            this.btnBackupDB.Click += new System.EventHandler(this.btnBackupDB_Click);
            // 
            // btnSearchLot
            // 
            this.btnSearchLot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnSearchLot.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnSearchLot.BorderRadius = 0;
            this.btnSearchLot.BorderSize = 1;
            this.btnSearchLot.Design = RJCodeUI_M1.RJControls.ButtonDesign.IconButton;
            this.btnSearchLot.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSearchLot.FlatAppearance.BorderSize = 0;
            this.btnSearchLot.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.btnSearchLot.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.btnSearchLot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchLot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchLot.ForeColor = System.Drawing.Color.White;
            this.btnSearchLot.IconChar = FontAwesome.Sharp.IconChar.Search;
            this.btnSearchLot.IconColor = System.Drawing.Color.White;
            this.btnSearchLot.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSearchLot.IconSize = 24;
            this.btnSearchLot.Location = new System.Drawing.Point(637, 35);
            this.btnSearchLot.Name = "btnSearchLot";
            this.btnSearchLot.Size = new System.Drawing.Size(49, 31);
            this.btnSearchLot.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnSearchLot.TabIndex = 17;
            this.btnSearchLot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearchLot.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearchLot.UseVisualStyleBackColor = false;
            this.btnSearchLot.Click += new System.EventHandler(this.btnSearchLot_Click);
            // 
            // tbSearch
            // 
            this.tbSearch._Customizable = false;
            this.tbSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbSearch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbSearch.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(162)))), ((int)(((byte)(247)))));
            this.tbSearch.BorderRadius = 0;
            this.tbSearch.BorderSize = 1;
            this.tbSearch.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbSearch.Location = new System.Drawing.Point(218, 35);
            this.tbSearch.MultiLine = false;
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbSearch.PasswordChar = false;
            this.tbSearch.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbSearch.PlaceHolderText = "Search";
            this.tbSearch.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbSearch.Size = new System.Drawing.Size(419, 31);
            this.tbSearch.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteBorder;
            this.tbSearch.TabIndex = 16;
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(215, 16);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(265, 16);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel3.TabIndex = 15;
            this.rjLabel3.Text = "Search by product name or description";
            // 
            // rjLabel1
            // 
            this.rjLabel1.AutoSize = true;
            this.rjLabel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel1.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel1.LinkLabel = false;
            this.rjLabel1.Location = new System.Drawing.Point(9, 16);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(89, 16);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel1.TabIndex = 12;
            this.rjLabel1.Text = "DefectType:";
            // 
            // cbPeriod
            // 
            this.cbPeriod.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbPeriod.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbPeriod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbPeriod.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbPeriod.BorderRadius = 0;
            this.cbPeriod.BorderSize = 1;
            this.cbPeriod.Customizable = false;
            this.cbPeriod.DataSource = null;
            this.cbPeriod.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cbPeriod.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbPeriod.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbPeriod.Items.AddRange(new object[] {
            "Today",
            "This Week",
            "Last 7 Days",
            "This Month",
            "Last 30 Days",
            "This Year",
            "Custom"});
            this.cbPeriod.Location = new System.Drawing.Point(12, 35);
            this.cbPeriod.Name = "cbPeriod";
            this.cbPeriod.Padding = new System.Windows.Forms.Padding(1);
            this.cbPeriod.SelectedIndex = -1;
            this.cbPeriod.Size = new System.Drawing.Size(200, 33);
            this.cbPeriod.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbPeriod.TabIndex = 11;
            this.cbPeriod.Texts = "";
            // 
            // btnSaveCsv
            // 
            this.btnSaveCsv.BackColor = System.Drawing.Color.White;
            this.btnSaveCsv.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveCsv.BorderRadius = 15;
            this.btnSaveCsv.BorderSize = 3;
            this.btnSaveCsv.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveCsv.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSaveCsv.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSaveCsv.FlatAppearance.BorderSize = 3;
            this.btnSaveCsv.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSaveCsv.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSaveCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCsv.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveCsv.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveCsv.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveCsv.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveCsv.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveCsv.IconSize = 80;
            this.btnSaveCsv.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveCsv.Location = new System.Drawing.Point(415, 604);
            this.btnSaveCsv.Name = "btnSaveCsv";
            this.btnSaveCsv.Size = new System.Drawing.Size(277, 88);
            this.btnSaveCsv.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSaveCsv.TabIndex = 2202;
            this.btnSaveCsv.Text = "Save As CSV";
            this.btnSaveCsv.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveCsv.UseVisualStyleBackColor = false;
            this.btnSaveCsv.Click += new System.EventHandler(this.btnSaveCsv_Click);
            // 
            // FormSearchDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(704, 695);
            this.Controls.Add(this.rjPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormSearchDB";
            this.Text = "Search DB";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.Enter += new System.EventHandler(this.FormSearchDB_Enter);
            this.rjPanel1.ResumeLayout(false);
            this.rjPanel1.PerformLayout();
            this.rjPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchDB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJComboBox cbPeriod;
        private RJCodeUI_M1.RJControls.RJButton btnSearchLot;
        private RJCodeUI_M1.RJControls.RJTextBox tbSearch;
        private RJCodeUI_M1.RJControls.RJButton btnBackupDB;
        private System.Windows.Forms.DateTimePicker EndPickerTime;
        private System.Windows.Forms.DateTimePicker StartPickerTime;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJDatePicker EndPicker;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel12;
        private RJCodeUI_M1.RJControls.RJDatePicker StartPicker;
        private RJCodeUI_M1.RJControls.RJButton btnSearchData;
        private RJCodeUI_M1.RJControls.RJButton btnRestoreDB;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel2;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvSearchDB;
        private RJCodeUI_M1.RJControls.RJButton rjButton4;
        private RJCodeUI_M1.RJControls.RJButton rjButton3;
        private RJCodeUI_M1.RJControls.RJButton rjButton2;
        private RJCodeUI_M1.RJControls.RJButton rjButton6;
        private RJCodeUI_M1.RJControls.RJButton rjButton5;
        private RJCodeUI_M1.RJControls.RJButton btnSaveCsv;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}