namespace Thermofisher_intern_CSV_viewer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewCSV = new System.Windows.Forms.DataGridView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxDelimiter = new System.Windows.Forms.GroupBox();
            this.radioButtonSemicolon = new System.Windows.Forms.RadioButton();
            this.radioButtonComma = new System.Windows.Forms.RadioButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.buttonReset = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCSV)).BeginInit();
            this.groupBoxDelimiter.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(10, 10);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1152, 45);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadCSVToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(80, 41);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(452, 48);
            this.newToolStripMenuItem.Text = "New (select path)";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadCSVToolStripMenuItem
            // 
            this.loadCSVToolStripMenuItem.Name = "loadCSVToolStripMenuItem";
            this.loadCSVToolStripMenuItem.Size = new System.Drawing.Size(452, 48);
            this.loadCSVToolStripMenuItem.Text = "Load (Repopulate table)";
            this.loadCSVToolStripMenuItem.Click += new System.EventHandler(this.loadCSVToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(452, 48);
            this.saveToolStripMenuItem.Text = "Save As...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(452, 48);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // dataGridViewCSV
            // 
            this.dataGridViewCSV.AllowDrop = true;
            this.dataGridViewCSV.AllowUserToOrderColumns = true;
            this.dataGridViewCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewCSV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewCSV.BackgroundColor = System.Drawing.SystemColors.InactiveCaption;
            this.dataGridViewCSV.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCSV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewCSV.ColumnHeadersHeight = 52;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewCSV.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewCSV.Location = new System.Drawing.Point(22, 246);
            this.dataGridViewCSV.Name = "dataGridViewCSV";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.HotTrack;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCSV.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewCSV.RowHeadersWidth = 92;
            this.dataGridViewCSV.RowTemplate.DividerHeight = 1;
            this.dataGridViewCSV.RowTemplate.Height = 45;
            this.dataGridViewCSV.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewCSV.Size = new System.Drawing.Size(1128, 553);
            this.dataGridViewCSV.TabIndex = 1;
            this.dataGridViewCSV.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridViewCSV_DragDrop);
            this.dataGridViewCSV.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridViewCSV_DragEnter);
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
            this.textBoxPath.Location = new System.Drawing.Point(153, 197);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(621, 43);
            this.textBoxPath.TabIndex = 2;
            this.textBoxPath.Text = "Select the path using File > New or Drag and Drop";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 197);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 37);
            this.label1.TabIndex = 3;
            this.label1.Text = "File path:";
            // 
            // groupBoxDelimiter
            // 
            this.groupBoxDelimiter.Controls.Add(this.radioButtonSemicolon);
            this.groupBoxDelimiter.Controls.Add(this.radioButtonComma);
            this.groupBoxDelimiter.Location = new System.Drawing.Point(22, 73);
            this.groupBoxDelimiter.Name = "groupBoxDelimiter";
            this.groupBoxDelimiter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBoxDelimiter.Size = new System.Drawing.Size(440, 118);
            this.groupBoxDelimiter.TabIndex = 7;
            this.groupBoxDelimiter.TabStop = false;
            this.groupBoxDelimiter.Text = "Delimiter:";
            // 
            // radioButtonSemicolon
            // 
            this.radioButtonSemicolon.AutoSize = true;
            this.radioButtonSemicolon.Location = new System.Drawing.Point(151, 57);
            this.radioButtonSemicolon.Name = "radioButtonSemicolon";
            this.radioButtonSemicolon.Size = new System.Drawing.Size(215, 41);
            this.radioButtonSemicolon.TabIndex = 1;
            this.radioButtonSemicolon.Text = "Semicolon ( ; )";
            this.radioButtonSemicolon.UseVisualStyleBackColor = true;
            this.radioButtonSemicolon.CheckedChanged += new System.EventHandler(this.radioButtonSemicolon_CheckedChanged);
            // 
            // radioButtonComma
            // 
            this.radioButtonComma.AutoSize = true;
            this.radioButtonComma.Checked = true;
            this.radioButtonComma.Location = new System.Drawing.Point(151, 0);
            this.radioButtonComma.Name = "radioButtonComma";
            this.radioButtonComma.Size = new System.Drawing.Size(191, 41);
            this.radioButtonComma.TabIndex = 0;
            this.radioButtonComma.TabStop = true;
            this.radioButtonComma.Text = "Comma ( , ) ";
            this.radioButtonComma.UseVisualStyleBackColor = true;
            this.radioButtonComma.CheckedChanged += new System.EventHandler(this.radioButtonComma_CheckedChanged);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(481, 86);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Padding = new System.Windows.Forms.Padding(10);
            this.buttonReset.Size = new System.Drawing.Size(293, 105);
            this.buttonReset.TabIndex = 8;
            this.buttonReset.Text = "Clear table";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1172, 821);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.groupBoxDelimiter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.dataGridViewCSV);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1200, 900);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = " CSV Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCSV)).EndInit();
            this.groupBoxDelimiter.ResumeLayout(false);
            this.groupBoxDelimiter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem loadCSVToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private DataGridView dataGridViewCSV;
        private OpenFileDialog openFileDialog1;
        private TextBox textBoxPath;
        private Label label1;
        private SaveFileDialog saveFileDialog1;
        private GroupBox groupBoxDelimiter;
        private RadioButton radioButtonSemicolon;
        private RadioButton radioButtonComma;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Button buttonReset;
    }
}