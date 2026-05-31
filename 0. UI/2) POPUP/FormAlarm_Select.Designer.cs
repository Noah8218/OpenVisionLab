
namespace KtemVisionSystem
{
    partial class FormAlarm_Select
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
            this.lbName = new System.Windows.Forms.Label();
            this.btnSkip = new MetroFramework.Controls.MetroButton();
            this.btnReject = new MetroFramework.Controls.MetroButton();
            this.btnRetry = new MetroFramework.Controls.MetroButton();
            this.btnBuzzerOff = new MetroFramework.Controls.MetroButton();
            this.label7 = new System.Windows.Forms.Label();
            this.lbAlarmTime = new System.Windows.Forms.Label();
            this.lbAlarmDesc = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbAlarmCode = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            this.btnSkip.Location = new System.Drawing.Point(20, 179);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(168, 50);
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
            this.btnReject.Location = new System.Drawing.Point(360, 180);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(168, 50);
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
            this.btnRetry.Location = new System.Drawing.Point(190, 180);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(168, 50);
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
            this.btnBuzzerOff.Location = new System.Drawing.Point(529, 180);
            this.btnBuzzerOff.Name = "btnBuzzerOff";
            this.btnBuzzerOff.Size = new System.Drawing.Size(168, 50);
            this.btnBuzzerOff.Style = MetroFramework.MetroColorStyle.Orange;
            this.btnBuzzerOff.TabIndex = 1083;
            this.btnBuzzerOff.Text = "BUZZER OFF";
            this.btnBuzzerOff.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnBuzzerOff.UseSelectable = true;
            this.btnBuzzerOff.Click += new System.EventHandler(this.btnBuzzerOff_Click);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Red;
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(360, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 52);
            this.label7.TabIndex = 1089;
            this.label7.Text = "TIME";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAlarmTime
            // 
            this.lbAlarmTime.BackColor = System.Drawing.Color.Black;
            this.lbAlarmTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAlarmTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbAlarmTime.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAlarmTime.ForeColor = System.Drawing.Color.Red;
            this.lbAlarmTime.Location = new System.Drawing.Point(448, 73);
            this.lbAlarmTime.Name = "lbAlarmTime";
            this.lbAlarmTime.Size = new System.Drawing.Size(248, 52);
            this.lbAlarmTime.TabIndex = 1088;
            this.lbAlarmTime.Text = "0000/00/00 00:00:00";
            this.lbAlarmTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAlarmDesc
            // 
            this.lbAlarmDesc.BackColor = System.Drawing.Color.Black;
            this.lbAlarmDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAlarmDesc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbAlarmDesc.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAlarmDesc.ForeColor = System.Drawing.Color.Red;
            this.lbAlarmDesc.Location = new System.Drawing.Point(109, 126);
            this.lbAlarmDesc.Name = "lbAlarmDesc";
            this.lbAlarmDesc.Size = new System.Drawing.Size(587, 52);
            this.lbAlarmDesc.TabIndex = 1087;
            this.lbAlarmDesc.Text = "......";
            this.lbAlarmDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Red;
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(21, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 52);
            this.label4.TabIndex = 1086;
            this.label4.Text = "DESC";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAlarmCode
            // 
            this.lbAlarmCode.BackColor = System.Drawing.Color.Black;
            this.lbAlarmCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAlarmCode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbAlarmCode.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAlarmCode.ForeColor = System.Drawing.Color.Red;
            this.lbAlarmCode.Location = new System.Drawing.Point(109, 73);
            this.lbAlarmCode.Name = "lbAlarmCode";
            this.lbAlarmCode.Size = new System.Drawing.Size(249, 52);
            this.lbAlarmCode.TabIndex = 1085;
            this.lbAlarmCode.Text = "[1] 000";
            this.lbAlarmCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Red;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(21, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 52);
            this.label1.TabIndex = 1084;
            this.label1.Text = "CODE";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormAlarm_Select
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 273);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbAlarmTime);
            this.Controls.Add(this.lbAlarmDesc);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbAlarmCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBuzzerOff);
            this.Controls.Add(this.btnRetry);
            this.Controls.Add(this.btnReject);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.lbName);
            this.DisplayHeader = false;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "FormAlarm_Select";
            this.Opacity = 0.99D;
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Style = MetroFramework.MetroColorStyle.Red;
            this.Text = "1";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormAlarm_Select_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbName;
        private MetroFramework.Controls.MetroButton btnSkip;
        private MetroFramework.Controls.MetroButton btnReject;
        private MetroFramework.Controls.MetroButton btnRetry;
        private MetroFramework.Controls.MetroButton btnBuzzerOff;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbAlarmTime;
        private System.Windows.Forms.Label lbAlarmDesc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbAlarmCode;
        private System.Windows.Forms.Label label1;
    }
}