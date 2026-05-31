
namespace OpenVisionLab
{
    partial class FormAlarm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbTackTime = new System.Windows.Forms.Label();
            this.timerTackTime = new System.Windows.Forms.Timer(this.components);
            this.lbName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbAlarmCode = new System.Windows.Forms.Label();
            this.lbAlarmDesc = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbAlarmTime = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSkip = new MetroFramework.Controls.MetroButton();
            this.btnReject = new MetroFramework.Controls.MetroButton();
            this.btnRetry = new MetroFramework.Controls.MetroButton();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.btnReset = new MetroFramework.Controls.MetroButton();
            this.gridAlarm = new MetroFramework.Controls.MetroGrid();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.lbAlarmPos = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarm)).BeginInit();
            this.SuspendLayout();
            // 
            // lbTackTime
            // 
            this.lbTackTime.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold);
            this.lbTackTime.ForeColor = System.Drawing.Color.Red;
            this.lbTackTime.Location = new System.Drawing.Point(876, 7);
            this.lbTackTime.Name = "lbTackTime";
            this.lbTackTime.Size = new System.Drawing.Size(90, 32);
            this.lbTackTime.TabIndex = 12;
            this.lbTackTime.Text = "00:00";
            this.lbTackTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerTackTime
            // 
            this.timerTackTime.Enabled = true;
            this.timerTackTime.Tick += new System.EventHandler(this.timerTackTime_Tick);
            // 
            // lbName
            // 
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.Font = new System.Drawing.Font("Microsoft YaHei UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.ForeColor = System.Drawing.Color.Red;
            this.lbName.Location = new System.Drawing.Point(0, 7);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(204, 87);
            this.lbName.TabIndex = 21;
            this.lbName.Text = "ALARM";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Red;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(23, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 52);
            this.label1.TabIndex = 23;
            this.label1.Text = "CODE";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAlarmCode
            // 
            this.lbAlarmCode.BackColor = System.Drawing.Color.Black;
            this.lbAlarmCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAlarmCode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbAlarmCode.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAlarmCode.ForeColor = System.Drawing.Color.Red;
            this.lbAlarmCode.Location = new System.Drawing.Point(111, 113);
            this.lbAlarmCode.Name = "lbAlarmCode";
            this.lbAlarmCode.Size = new System.Drawing.Size(249, 52);
            this.lbAlarmCode.TabIndex = 25;
            this.lbAlarmCode.Text = "[1] 000";
            this.lbAlarmCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAlarmDesc
            // 
            this.lbAlarmDesc.BackColor = System.Drawing.Color.Black;
            this.lbAlarmDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAlarmDesc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbAlarmDesc.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAlarmDesc.ForeColor = System.Drawing.Color.Red;
            this.lbAlarmDesc.Location = new System.Drawing.Point(111, 166);
            this.lbAlarmDesc.Name = "lbAlarmDesc";
            this.lbAlarmDesc.Size = new System.Drawing.Size(587, 52);
            this.lbAlarmDesc.TabIndex = 27;
            this.lbAlarmDesc.Text = "......";
            this.lbAlarmDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Red;
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(23, 166);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 52);
            this.label4.TabIndex = 26;
            this.label4.Text = "DESC";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAlarmTime
            // 
            this.lbAlarmTime.BackColor = System.Drawing.Color.Black;
            this.lbAlarmTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAlarmTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbAlarmTime.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAlarmTime.ForeColor = System.Drawing.Color.Red;
            this.lbAlarmTime.Location = new System.Drawing.Point(450, 113);
            this.lbAlarmTime.Name = "lbAlarmTime";
            this.lbAlarmTime.Size = new System.Drawing.Size(248, 52);
            this.lbAlarmTime.TabIndex = 29;
            this.lbAlarmTime.Text = "0000/00/00 00:00:00";
            this.lbAlarmTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Red;
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(362, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 52);
            this.label7.TabIndex = 30;
            this.label7.Text = "TIME";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSkip
            // 
            this.btnSkip.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnSkip.Highlight = true;
            this.btnSkip.Location = new System.Drawing.Point(426, 7);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(229, 73);
            this.btnSkip.Style = MetroFramework.MetroColorStyle.Red;
            this.btnSkip.TabIndex = 1079;
            this.btnSkip.Text = "SKIP";
            this.btnSkip.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnSkip.UseSelectable = true;
            this.btnSkip.Visible = false;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // btnReject
            // 
            this.btnReject.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnReject.Highlight = true;
            this.btnReject.Location = new System.Drawing.Point(704, 266);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(229, 73);
            this.btnReject.Style = MetroFramework.MetroColorStyle.Red;
            this.btnReject.TabIndex = 1081;
            this.btnReject.Text = "알람 스킵";
            this.btnReject.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnReject.UseSelectable = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRetry.Highlight = true;
            this.btnRetry.Location = new System.Drawing.Point(176, 7);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(229, 73);
            this.btnRetry.Style = MetroFramework.MetroColorStyle.Red;
            this.btnRetry.TabIndex = 1082;
            this.btnRetry.Text = "RETRY";
            this.btnRetry.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnRetry.UseSelectable = true;
            this.btnRetry.Visible = false;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // metroButton1
            // 
            this.metroButton1.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.metroButton1.Highlight = true;
            this.metroButton1.Location = new System.Drawing.Point(704, 113);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(229, 73);
            this.metroButton1.Style = MetroFramework.MetroColorStyle.Orange;
            this.metroButton1.TabIndex = 1083;
            this.metroButton1.Text = "소리 차단";
            this.metroButton1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton1.UseSelectable = true;
            this.metroButton1.Click += new System.EventHandler(this.btnBuzzerOff_Click);
            // 
            // btnReset
            // 
            this.btnReset.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnReset.Highlight = true;
            this.btnReset.Location = new System.Drawing.Point(704, 189);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(229, 73);
            this.btnReset.Style = MetroFramework.MetroColorStyle.Red;
            this.btnReset.TabIndex = 1084;
            this.btnReset.Text = "알람 리셋";
            this.btnReset.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnReset.UseSelectable = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // gridAlarm
            // 
            this.gridAlarm.AllowUserToResizeRows = false;
            this.gridAlarm.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.gridAlarm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridAlarm.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridAlarm.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(19)))), ((int)(((byte)(73)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridAlarm.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.gridAlarm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAlarm.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.Column1});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(19)))), ((int)(((byte)(73)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridAlarm.DefaultCellStyle = dataGridViewCellStyle8;
            this.gridAlarm.EnableHeadersVisualStyles = false;
            this.gridAlarm.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.gridAlarm.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.gridAlarm.Location = new System.Drawing.Point(23, 272);
            this.gridAlarm.MultiSelect = false;
            this.gridAlarm.Name = "gridAlarm";
            this.gridAlarm.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(19)))), ((int)(((byte)(73)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridAlarm.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.gridAlarm.RowHeadersWidth = 51;
            this.gridAlarm.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridAlarm.RowTemplate.Height = 23;
            this.gridAlarm.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAlarm.Size = new System.Drawing.Size(675, 224);
            this.gridAlarm.Style = MetroFramework.MetroColorStyle.Red;
            this.gridAlarm.TabIndex = 1308;
            this.gridAlarm.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "CODE";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "NAME";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 6;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 300;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "POS";
            this.Column1.Name = "Column1";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Red;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(23, 219);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 52);
            this.label2.TabIndex = 1309;
            this.label2.Text = "POS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAlarmPos
            // 
            this.lbAlarmPos.BackColor = System.Drawing.Color.Black;
            this.lbAlarmPos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAlarmPos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbAlarmPos.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAlarmPos.ForeColor = System.Drawing.Color.Red;
            this.lbAlarmPos.Location = new System.Drawing.Point(111, 219);
            this.lbAlarmPos.Name = "lbAlarmPos";
            this.lbAlarmPos.Size = new System.Drawing.Size(587, 52);
            this.lbAlarmPos.TabIndex = 1310;
            this.lbAlarmPos.Text = "......";
            this.lbAlarmPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormAlarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 519);
            this.Controls.Add(this.lbAlarmPos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gridAlarm);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.metroButton1);
            this.Controls.Add(this.btnRetry);
            this.Controls.Add(this.btnReject);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbAlarmTime);
            this.Controls.Add(this.lbAlarmDesc);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbAlarmCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.lbTackTime);
            this.DisplayHeader = false;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "FormAlarm";
            this.Opacity = 0.99D;
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Style = MetroFramework.MetroColorStyle.Red;
            this.Text = "ALARM";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.FormAlarm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarm)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbTackTime;
        private System.Windows.Forms.Timer timerTackTime;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbAlarmCode;
        private System.Windows.Forms.Label lbAlarmDesc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbAlarmTime;
        private System.Windows.Forms.Label label7;
        private MetroFramework.Controls.MetroButton btnSkip;
        private MetroFramework.Controls.MetroButton btnReject;
        private MetroFramework.Controls.MetroButton btnRetry;
        private MetroFramework.Controls.MetroButton metroButton1;
        private MetroFramework.Controls.MetroButton btnReset;
        private MetroFramework.Controls.MetroGrid gridAlarm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbAlarmPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}