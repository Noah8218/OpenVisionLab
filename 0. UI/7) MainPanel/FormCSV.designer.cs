using System.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormCSV
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCSV));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxDelimiter = new System.Windows.Forms.GroupBox();
            this.radioButtonSemicolon = new System.Windows.Forms.RadioButton();
            this.radioButtonComma = new System.Windows.Forms.RadioButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonReset = new RJCodeUI_M1.RJControls.RJButton();
            this.dataGridViewCSV = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.menuStrip1.SuspendLayout();
            this.groupBoxDelimiter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCSV)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(5, 3);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(543, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem.Text = "New (select path)";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save As...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Comma separated values (*.csv) | *.csv";
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "Select csv file";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Enabled = false;
            this.textBoxPath.Location = new System.Drawing.Point(4, 60);
            this.textBoxPath.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(358, 21);
            this.textBoxPath.TabIndex = 2;
            this.textBoxPath.Text = "Select the path using File > New or Drag and Drop";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 47);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "File path:";
            // 
            // groupBoxDelimiter
            // 
            this.groupBoxDelimiter.Controls.Add(this.radioButtonSemicolon);
            this.groupBoxDelimiter.Controls.Add(this.radioButtonComma);
            this.groupBoxDelimiter.Location = new System.Drawing.Point(4, 7);
            this.groupBoxDelimiter.Margin = new System.Windows.Forms.Padding(1);
            this.groupBoxDelimiter.Name = "groupBoxDelimiter";
            this.groupBoxDelimiter.Padding = new System.Windows.Forms.Padding(1);
            this.groupBoxDelimiter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBoxDelimiter.Size = new System.Drawing.Size(205, 38);
            this.groupBoxDelimiter.TabIndex = 7;
            this.groupBoxDelimiter.TabStop = false;
            this.groupBoxDelimiter.Text = "Delimiter:";
            // 
            // radioButtonSemicolon
            // 
            this.radioButtonSemicolon.AutoSize = true;
            this.radioButtonSemicolon.Location = new System.Drawing.Point(70, 18);
            this.radioButtonSemicolon.Margin = new System.Windows.Forms.Padding(1);
            this.radioButtonSemicolon.Name = "radioButtonSemicolon";
            this.radioButtonSemicolon.Size = new System.Drawing.Size(109, 16);
            this.radioButtonSemicolon.TabIndex = 1;
            this.radioButtonSemicolon.Text = "Semicolon ( ; )";
            this.radioButtonSemicolon.UseVisualStyleBackColor = true;
            this.radioButtonSemicolon.CheckedChanged += new System.EventHandler(this.radioButtonSemicolon_CheckedChanged);
            // 
            // radioButtonComma
            // 
            this.radioButtonComma.AutoSize = true;
            this.radioButtonComma.Checked = true;
            this.radioButtonComma.Location = new System.Drawing.Point(70, 0);
            this.radioButtonComma.Margin = new System.Windows.Forms.Padding(1);
            this.radioButtonComma.Name = "radioButtonComma";
            this.radioButtonComma.Size = new System.Drawing.Size(98, 16);
            this.radioButtonComma.TabIndex = 0;
            this.radioButtonComma.TabStop = true;
            this.radioButtonComma.Text = "Comma ( , ) ";
            this.radioButtonComma.UseVisualStyleBackColor = true;
            this.radioButtonComma.CheckedChanged += new System.EventHandler(this.radioButtonComma_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(5, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonReset);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxPath);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxDelimiter);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewCSV);
            this.splitContainer1.Size = new System.Drawing.Size(543, 336);
            this.splitContainer1.SplitterDistance = 105;
            this.splitContainer1.TabIndex = 3;
            // 
            // buttonReset
            // 
            this.buttonReset.BackColor = System.Drawing.Color.White;
            this.buttonReset.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.buttonReset.BorderRadius = 15;
            this.buttonReset.BorderSize = 3;
            this.buttonReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonReset.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.buttonReset.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.buttonReset.FlatAppearance.BorderSize = 3;
            this.buttonReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.buttonReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.buttonReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.buttonReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.buttonReset.IconChar = FontAwesome.Sharp.IconChar.Redo;
            this.buttonReset.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.buttonReset.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.buttonReset.IconSize = 80;
            this.buttonReset.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonReset.Location = new System.Drawing.Point(416, 0);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(127, 105);
            this.buttonReset.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.buttonReset.TabIndex = 2139;
            this.buttonReset.Text = "CLEAR";
            this.buttonReset.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonReset.UseVisualStyleBackColor = false;
            this.buttonReset.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridViewCSV
            // 
            this.dataGridViewCSV.AllowDrop = true;
            this.dataGridViewCSV.AllowUserToAddRows = false;
            this.dataGridViewCSV.AllowUserToOrderColumns = true;
            this.dataGridViewCSV.AllowUserToResizeRows = false;
            this.dataGridViewCSV.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dataGridViewCSV.AlternatingRowsColorApply = false;
            this.dataGridViewCSV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewCSV.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dataGridViewCSV.BorderRadius = 13;
            this.dataGridViewCSV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewCSV.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridViewCSV.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dataGridViewCSV.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewCSV.ColumnHeaderHeight = 40;
            this.dataGridViewCSV.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCSV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewCSV.ColumnHeadersHeight = 40;
            this.dataGridViewCSV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewCSV.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dataGridViewCSV.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewCSV.Customizable = false;
            this.dataGridViewCSV.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dataGridViewCSV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCSV.EnableHeadersVisualStyles = false;
            this.dataGridViewCSV.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewCSV.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewCSV.Name = "dataGridViewCSV";
            this.dataGridViewCSV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGridViewCSV.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridViewCSV.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewCSV.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewCSV.RowHeadersVisible = false;
            this.dataGridViewCSV.RowHeadersWidth = 30;
            this.dataGridViewCSV.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewCSV.RowHeight = 40;
            this.dataGridViewCSV.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            this.dataGridViewCSV.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewCSV.RowsTextColor = System.Drawing.Color.Gray;
            this.dataGridViewCSV.RowTemplate.Height = 40;
            this.dataGridViewCSV.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dataGridViewCSV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCSV.SelectionTextColor = System.Drawing.Color.Gray;
            this.dataGridViewCSV.Size = new System.Drawing.Size(543, 227);
            this.dataGridViewCSV.TabIndex = 2108;
            this.dataGridViewCSV.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridViewCSV_DragDrop);
            this.dataGridViewCSV.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridViewCSV_DragEnter);
            // 
            // FormCSV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(553, 366);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MinimumSize = new System.Drawing.Size(569, 318);
            this.Name = "FormCSV";
            this.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.Text = " CSV Editor";
            this.VisibleChanged += new System.EventHandler(this.FormLayerDisplay_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBoxDelimiter.ResumeLayout(false);
            this.groupBoxDelimiter.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCSV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private OpenFileDialog openFileDialog1;
        private TextBox textBoxPath;
        private Label label1;
        private SaveFileDialog saveFileDialog1;
        private GroupBox groupBoxDelimiter;
        private RadioButton radioButtonSemicolon;
        private RadioButton radioButtonComma;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private RJCodeUI_M1.RJControls.RJDataGridView dataGridViewCSV;
        private RJCodeUI_M1.RJControls.RJButton buttonReset;
        private SplitContainer splitContainer1;
    }
}