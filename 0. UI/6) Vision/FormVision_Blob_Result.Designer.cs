namespace KtemVisionSystem
{
    partial class FormVision_Blob_Result
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
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.btnNewCancel = new RJCodeUI_M1.RJControls.RJButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.dgvDefect = new RJCodeUI_M1.RJControls.RJDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.rjPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefect)).BeginInit();
            this.SuspendLayout();
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroStyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // btnNewCancel
            // 
            this.btnNewCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.btnNewCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.btnNewCancel.BorderRadius = 10;
            this.btnNewCancel.BorderSize = 1;
            this.btnNewCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewCancel.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnNewCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnNewCancel.FlatAppearance.BorderSize = 0;
            this.btnNewCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(103)))), ((int)(((byte)(125)))));
            this.btnNewCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(96)))), ((int)(((byte)(117)))));
            this.btnNewCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewCancel.ForeColor = System.Drawing.Color.White;
            this.btnNewCancel.IconChar = FontAwesome.Sharp.IconChar.TimesCircle;
            this.btnNewCancel.IconColor = System.Drawing.Color.White;
            this.btnNewCancel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewCancel.IconSize = 24;
            this.btnNewCancel.Location = new System.Drawing.Point(355, 463);
            this.btnNewCancel.Name = "btnNewCancel";
            this.btnNewCancel.Size = new System.Drawing.Size(90, 35);
            this.btnNewCancel.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnNewCancel.TabIndex = 2153;
            this.btnNewCancel.Text = "Cancel";
            this.btnNewCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNewCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNewCancel.UseVisualStyleBackColor = false;
            this.btnNewCancel.Click += new System.EventHandler(this.btnNewCancel_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;            
            // 
            // rjPanel1
            // 
            this.rjPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel1.BorderRadius = 0;
            this.rjPanel1.Controls.Add(this.dgvDefect);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Location = new System.Drawing.Point(5, 63);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(440, 394);
            this.rjPanel1.TabIndex = 2154;
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
            this.dgvDefect.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvDefect.ColumnHeaderHeight = 40;
            this.dgvDefect.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.dgvDefect.Size = new System.Drawing.Size(440, 394);
            this.dgvDefect.TabIndex = 16;
            // 
            // FormVision_Blob_Result
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 502);
            this.Controls.Add(this.rjPanel1);
            this.Controls.Add(this.btnNewCancel);
            this.Name = "FormVision_Blob_Result";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.White;
            this.Text = "Result";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.rjPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefect)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private RJCodeUI_M1.RJControls.RJButton btnNewCancel;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvDefect;
    }
}