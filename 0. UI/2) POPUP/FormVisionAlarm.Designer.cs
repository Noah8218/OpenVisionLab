
using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormVisionAlarm
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
            //ImageGlass.DefaultGifAnimator defaultGifAnimator1 = new ImageGlass.DefaultGifAnimator();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbName = new System.Windows.Forms.Label();
            this.btnSkip = new MetroFramework.Controls.MetroButton();
            this.btnReject = new MetroFramework.Controls.MetroButton();
            this.btnRetry = new MetroFramework.Controls.MetroButton();
            this.btnBuzzerOff = new MetroFramework.Controls.MetroButton();
            this.lbStatusLoadingVisionInsp = new System.Windows.Forms.Label();
            this.ibSource = new ImageBox();
            this.dgvResultMap = new System.Windows.Forms.DataGridView();
            this.btnViewOriginal = new MetroFramework.Controls.MetroLabel();
            this.btnViewBinary = new MetroFramework.Controls.MetroLabel();
            this.lbThreshold = new MetroFramework.Controls.MetroLabel();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.trbThreshold = new MetroFramework.Controls.MetroTrackBar();
            this.lbAreaMin = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnReInsp = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultMap)).BeginInit();
            this.SuspendLayout();
            // 
            // lbName
            // 
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.Font = new System.Drawing.Font("Microsoft YaHei UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.ForeColor = System.Drawing.Color.Red;
            this.lbName.Location = new System.Drawing.Point(10, 10);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(204, 57);
            this.lbName.TabIndex = 21;
            this.lbName.Text = "ALARM";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSkip
            // 
            this.btnSkip.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnSkip.Highlight = true;
            this.btnSkip.Location = new System.Drawing.Point(367, 537);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(161, 50);
            this.btnSkip.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSkip.TabIndex = 1079;
            this.btnSkip.Text = "SKIP";
            this.btnSkip.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnSkip.UseSelectable = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // btnReject
            // 
            this.btnReject.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnReject.Highlight = true;
            this.btnReject.Location = new System.Drawing.Point(691, 537);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(161, 50);
            this.btnReject.Style = MetroFramework.MetroColorStyle.Red;
            this.btnReject.TabIndex = 1081;
            this.btnReject.Text = "REJECT";
            this.btnReject.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnReject.UseSelectable = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRetry.Highlight = true;
            this.btnRetry.Location = new System.Drawing.Point(529, 537);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(161, 50);
            this.btnRetry.Style = MetroFramework.MetroColorStyle.Red;
            this.btnRetry.TabIndex = 1082;
            this.btnRetry.Text = "RETRY";
            this.btnRetry.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnRetry.UseSelectable = true;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnBuzzerOff
            // 
            this.btnBuzzerOff.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnBuzzerOff.Highlight = true;
            this.btnBuzzerOff.Location = new System.Drawing.Point(854, 537);
            this.btnBuzzerOff.Name = "btnBuzzerOff";
            this.btnBuzzerOff.Size = new System.Drawing.Size(109, 50);
            this.btnBuzzerOff.Style = MetroFramework.MetroColorStyle.Orange;
            this.btnBuzzerOff.TabIndex = 1083;
            this.btnBuzzerOff.Text = "BUZZER OFF";
            this.btnBuzzerOff.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnBuzzerOff.UseSelectable = true;
            this.btnBuzzerOff.Click += new System.EventHandler(this.btnBuzzerOff_Click);
            // 
            // lbStatusLoadingVisionInsp
            // 
            this.lbStatusLoadingVisionInsp.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusLoadingVisionInsp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusLoadingVisionInsp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusLoadingVisionInsp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbStatusLoadingVisionInsp.ForeColor = System.Drawing.Color.White;
            this.lbStatusLoadingVisionInsp.Location = new System.Drawing.Point(367, 69);
            this.lbStatusLoadingVisionInsp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbStatusLoadingVisionInsp.Name = "lbStatusLoadingVisionInsp";
            this.lbStatusLoadingVisionInsp.Size = new System.Drawing.Size(596, 24);
            this.lbStatusLoadingVisionInsp.TabIndex = 1308;
            this.lbStatusLoadingVisionInsp.Text = "RESULT";
            this.lbStatusLoadingVisionInsp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ibSource
            // 
            //this.ibSource.Animator = defaultGifAnimator1;
            this.ibSource.Location = new System.Drawing.Point(367, 93);
            this.ibSource.Margin = new System.Windows.Forms.Padding(4);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(596, 371);
            this.ibSource.TabIndex = 1307;
            this.ibSource.Zoom = 100;
            // 
            // dgvResultMap
            // 
            this.dgvResultMap.AllowUserToAddRows = false;
            this.dgvResultMap.AllowUserToDeleteRows = false;
            this.dgvResultMap.AllowUserToResizeColumns = false;
            this.dgvResultMap.AllowUserToResizeRows = false;
            this.dgvResultMap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResultMap.BackgroundColor = System.Drawing.Color.DimGray;
            this.dgvResultMap.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Aquamarine;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Aquamarine;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResultMap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvResultMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Aquamarine;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResultMap.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvResultMap.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvResultMap.GridColor = System.Drawing.Color.White;
            this.dgvResultMap.Location = new System.Drawing.Point(15, 69);
            this.dgvResultMap.Margin = new System.Windows.Forms.Padding(2);
            this.dgvResultMap.Name = "dgvResultMap";
            this.dgvResultMap.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Aquamarine;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Aquamarine;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResultMap.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvResultMap.RowHeadersVisible = false;
            this.dgvResultMap.RowTemplate.Height = 23;
            this.dgvResultMap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvResultMap.Size = new System.Drawing.Size(351, 518);
            this.dgvResultMap.TabIndex = 1238;
            this.dgvResultMap.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResultMap_CellClick);
            // 
            // btnViewOriginal
            // 
            this.btnViewOriginal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.btnViewOriginal.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.btnViewOriginal.ForeColor = System.Drawing.Color.White;
            this.btnViewOriginal.Location = new System.Drawing.Point(367, 465);
            this.btnViewOriginal.Name = "btnViewOriginal";
            this.btnViewOriginal.Size = new System.Drawing.Size(108, 35);
            this.btnViewOriginal.Style = MetroFramework.MetroColorStyle.Silver;
            this.btnViewOriginal.TabIndex = 1757;
            this.btnViewOriginal.Text = "ORIGINAL";
            this.btnViewOriginal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnViewOriginal.UseCustomBackColor = true;
            this.btnViewOriginal.UseCustomForeColor = true;
            this.btnViewOriginal.Click += new System.EventHandler(this.OnClickViewMode);
            // 
            // btnViewBinary
            // 
            this.btnViewBinary.BackColor = System.Drawing.Color.DimGray;
            this.btnViewBinary.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.btnViewBinary.ForeColor = System.Drawing.Color.White;
            this.btnViewBinary.Location = new System.Drawing.Point(476, 465);
            this.btnViewBinary.Name = "btnViewBinary";
            this.btnViewBinary.Size = new System.Drawing.Size(108, 35);
            this.btnViewBinary.Style = MetroFramework.MetroColorStyle.Silver;
            this.btnViewBinary.TabIndex = 1758;
            this.btnViewBinary.Text = "BINARY";
            this.btnViewBinary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnViewBinary.UseCustomBackColor = true;
            this.btnViewBinary.UseCustomForeColor = true;
            this.btnViewBinary.Click += new System.EventHandler(this.OnClickViewMode);
            // 
            // lbThreshold
            // 
            this.lbThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbThreshold.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbThreshold.ForeColor = System.Drawing.Color.White;
            this.lbThreshold.Location = new System.Drawing.Point(888, 465);
            this.lbThreshold.Name = "lbThreshold";
            this.lbThreshold.Size = new System.Drawing.Size(77, 35);
            this.lbThreshold.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbThreshold.TabIndex = 1761;
            this.lbThreshold.Text = "000";
            this.lbThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbThreshold.UseCustomBackColor = true;
            this.lbThreshold.UseCustomForeColor = true;
            // 
            // metroLabel8
            // 
            this.metroLabel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel8.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel8.ForeColor = System.Drawing.Color.White;
            this.metroLabel8.Location = new System.Drawing.Point(585, 465);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(92, 35);
            this.metroLabel8.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel8.TabIndex = 1760;
            this.metroLabel8.Text = "Threshold";
            this.metroLabel8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel8.UseCustomBackColor = true;
            this.metroLabel8.UseCustomForeColor = true;
            // 
            // trbThreshold
            // 
            this.trbThreshold.BackColor = System.Drawing.Color.Transparent;
            this.trbThreshold.Location = new System.Drawing.Point(678, 465);
            this.trbThreshold.Maximum = 254;
            this.trbThreshold.Minimum = 1;
            this.trbThreshold.Name = "trbThreshold";
            this.trbThreshold.Size = new System.Drawing.Size(210, 35);
            this.trbThreshold.TabIndex = 1759;
            this.trbThreshold.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.trbThreshold.Scroll += new System.Windows.Forms.ScrollEventHandler(this.trbThreshold_Scroll);
            // 
            // lbAreaMin
            // 
            this.lbAreaMin.BackColor = System.Drawing.Color.Transparent;
            this.lbAreaMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAreaMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbAreaMin.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.lbAreaMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbAreaMin.Location = new System.Drawing.Point(485, 501);
            this.lbAreaMin.Name = "lbAreaMin";
            this.lbAreaMin.Size = new System.Drawing.Size(96, 35);
            this.lbAreaMin.TabIndex = 1763;
            this.lbAreaMin.Text = "000000";
            this.lbAreaMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label12.Location = new System.Drawing.Point(367, 501);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(117, 35);
            this.label12.TabIndex = 1762;
            this.label12.Text = "디바이스 최소 넓이";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label1.Location = new System.Drawing.Point(700, 501);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 35);
            this.label1.TabIndex = 1765;
            this.label1.Text = "000000";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label2.Location = new System.Drawing.Point(582, 501);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 35);
            this.label2.TabIndex = 1764;
            this.label2.Text = "디바이스 최대 넓이";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReInsp
            // 
            this.btnReInsp.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnReInsp.Highlight = true;
            this.btnReInsp.Location = new System.Drawing.Point(797, 501);
            this.btnReInsp.Name = "btnReInsp";
            this.btnReInsp.Size = new System.Drawing.Size(168, 35);
            this.btnReInsp.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnReInsp.TabIndex = 1766;
            this.btnReInsp.Text = "RE-INSP";
            this.btnReInsp.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnReInsp.UseSelectable = true;
            this.btnReInsp.Click += new System.EventHandler(this.btnReInsp_Click);
            // 
            // FormVisionAlarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 610);
            this.Controls.Add(this.btnReInsp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbAreaMin);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lbThreshold);
            this.Controls.Add(this.metroLabel8);
            this.Controls.Add(this.trbThreshold);
            this.Controls.Add(this.btnViewBinary);
            this.Controls.Add(this.btnViewOriginal);
            this.Controls.Add(this.lbStatusLoadingVisionInsp);
            this.Controls.Add(this.ibSource);
            this.Controls.Add(this.btnBuzzerOff);
            this.Controls.Add(this.btnRetry);
            this.Controls.Add(this.btnReject);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.dgvResultMap);
            this.DisplayHeader = false;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "FormVisionAlarm";
            this.Opacity = 0.99D;
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Style = MetroFramework.MetroColorStyle.Red;
            this.Text = "1";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormVisionAlarm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbName;
        private MetroFramework.Controls.MetroButton btnSkip;
        private MetroFramework.Controls.MetroButton btnReject;
        private MetroFramework.Controls.MetroButton btnRetry;
        private MetroFramework.Controls.MetroButton btnBuzzerOff;
        private System.Windows.Forms.Label lbStatusLoadingVisionInsp;
        private ImageBox ibSource;
        private System.Windows.Forms.DataGridView dgvResultMap;
        private MetroFramework.Controls.MetroLabel btnViewOriginal;
        private MetroFramework.Controls.MetroLabel btnViewBinary;
        private MetroFramework.Controls.MetroLabel lbThreshold;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private MetroFramework.Controls.MetroTrackBar trbThreshold;
        private System.Windows.Forms.Label lbAreaMin;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private MetroFramework.Controls.MetroButton btnReInsp;
    }
}