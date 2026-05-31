using Cyotek.Windows.Forms;

namespace KtemVisionSystem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBlob));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.fpnImageList = new System.Windows.Forms.FlowLayoutPanel();
            this.dgvDefect = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.btnBlobRun = new RJCodeUI_M1.RJControls.RJButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefect)).BeginInit();
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
            this.splitContainer1.Panel2.Controls.Add(this.fpnImageList);
            this.splitContainer1.Panel2.Controls.Add(this.btnBlobRun);
            this.splitContainer1.Size = new System.Drawing.Size(612, 531);
            this.splitContainer1.SplitterDistance = 262;
            this.splitContainer1.TabIndex = 1953;
            // 
            // fpnImageList
            // 
            this.fpnImageList.AutoScroll = true;
            this.fpnImageList.BackColor = System.Drawing.Color.Black;
            this.fpnImageList.Location = new System.Drawing.Point(3, 3);
            this.fpnImageList.Name = "fpnImageList";
            this.fpnImageList.Size = new System.Drawing.Size(421, 562);
            this.fpnImageList.TabIndex = 2139;
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
            this.dgvDefect.Size = new System.Drawing.Size(612, 262);
            this.dgvDefect.TabIndex = 2137;
            this.dgvDefect.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSeletecList_CellClick);
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
            this.btnBlobRun.Location = new System.Drawing.Point(462, -1);
            this.btnBlobRun.Name = "btnBlobRun";
            this.btnBlobRun.Size = new System.Drawing.Size(150, 115);
            this.btnBlobRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnBlobRun.TabIndex = 2138;
            this.btnBlobRun.Text = "단일 검사";
            this.btnBlobRun.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBlobRun.UseVisualStyleBackColor = false;
            this.btnBlobRun.Click += new System.EventHandler(this.btnBlobRun_Click);
            // 
            // FormBlob
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(612, 531);
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
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private RJCodeUI_M1.RJControls.RJButton btnBlobRun;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvDefect;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel fpnImageList;
    }
}