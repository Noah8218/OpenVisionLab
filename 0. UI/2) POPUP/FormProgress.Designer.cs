
namespace KtemVisionSystem
{
    partial class FormProgress
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
            this.circularProgressBar5 = new CircularProgressBar.CircularProgressBar();
            this.lbTackTime = new System.Windows.Forms.Label();
            this.timerTackTime = new System.Windows.Forms.Timer(this.components);
            this.lbName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbAlarmCode = new System.Windows.Forms.Label();
            this.lbDesc = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbTime = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // circularProgressBar5
            // 
            this.circularProgressBar5.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.circularProgressBar5.AnimationSpeed = 500;
            this.circularProgressBar5.BackColor = System.Drawing.Color.Transparent;
            this.circularProgressBar5.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.circularProgressBar5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.circularProgressBar5.InnerColor = System.Drawing.Color.Black;
            this.circularProgressBar5.InnerMargin = 0;
            this.circularProgressBar5.InnerWidth = 0;
            this.circularProgressBar5.Location = new System.Drawing.Point(493, 248);
            this.circularProgressBar5.MarqueeAnimationSpeed = 2000;
            this.circularProgressBar5.Name = "circularProgressBar5";
            this.circularProgressBar5.OuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.circularProgressBar5.OuterMargin = -11;
            this.circularProgressBar5.OuterWidth = 8;
            this.circularProgressBar5.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.circularProgressBar5.ProgressWidth = 14;
            this.circularProgressBar5.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 4.125F);
            this.circularProgressBar5.Size = new System.Drawing.Size(218, 199);
            this.circularProgressBar5.StartAngle = 270;
            this.circularProgressBar5.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.circularProgressBar5.SubscriptColor = System.Drawing.Color.Transparent;
            this.circularProgressBar5.SubscriptMargin = new System.Windows.Forms.Padding(0);
            this.circularProgressBar5.SubscriptText = "";
            this.circularProgressBar5.SuperscriptColor = System.Drawing.Color.Transparent;
            this.circularProgressBar5.SuperscriptMargin = new System.Windows.Forms.Padding(0);
            this.circularProgressBar5.SuperscriptText = "";
            this.circularProgressBar5.TabIndex = 11;
            this.circularProgressBar5.Text = "Please Wait";
            this.circularProgressBar5.TextMargin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.circularProgressBar5.Value = 67;
            // 
            // lbTackTime
            // 
            this.lbTackTime.AutoSize = true;
            this.lbTackTime.Font = new System.Drawing.Font("Consolas", 20F, System.Drawing.FontStyle.Bold);
            this.lbTackTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbTackTime.Location = new System.Drawing.Point(560, 369);
            this.lbTackTime.Name = "lbTackTime";
            this.lbTackTime.Size = new System.Drawing.Size(90, 32);
            this.lbTackTime.TabIndex = 12;
            this.lbTackTime.Text = "00:00";
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
            this.lbName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbName.Location = new System.Drawing.Point(0, 7);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(255, 87);
            this.lbName.TabIndex = 21;
            this.lbName.Text = "PROGRESS";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
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
            this.lbAlarmCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbAlarmCode.Location = new System.Drawing.Point(111, 113);
            this.lbAlarmCode.Name = "lbAlarmCode";
            this.lbAlarmCode.Size = new System.Drawing.Size(249, 52);
            this.lbAlarmCode.TabIndex = 25;
            this.lbAlarmCode.Text = "[1] 000";
            this.lbAlarmCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDesc
            // 
            this.lbDesc.BackColor = System.Drawing.Color.Black;
            this.lbDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDesc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbDesc.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbDesc.Location = new System.Drawing.Point(111, 166);
            this.lbDesc.Name = "lbDesc";
            this.lbDesc.Size = new System.Drawing.Size(587, 52);
            this.lbDesc.TabIndex = 27;
            this.lbDesc.Text = "......";
            this.lbDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(23, 166);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 52);
            this.label4.TabIndex = 26;
            this.label4.Text = "DESC";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTime
            // 
            this.lbTime.BackColor = System.Drawing.Color.Black;
            this.lbTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbTime.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbTime.Location = new System.Drawing.Point(450, 113);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(248, 52);
            this.lbTime.TabIndex = 29;
            this.lbTime.Text = "0000/00/00 00:00:00";
            this.lbTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(362, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 52);
            this.label7.TabIndex = 30;
            this.label7.Text = "TIME";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnCancel.Highlight = true;
            this.btnCancel.Location = new System.Drawing.Point(27, 369);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(422, 73);
            this.btnCancel.Style = MetroFramework.MetroColorStyle.Red;
            this.btnCancel.TabIndex = 1078;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 480);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbTime);
            this.Controls.Add(this.lbDesc);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbAlarmCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.lbTackTime);
            this.Controls.Add(this.circularProgressBar5);
            this.DisplayHeader = false;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "FormProgress";
            this.Opacity = 0.7D;
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "PROGRESS";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormProgress_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CircularProgressBar.CircularProgressBar circularProgressBar5;
        private System.Windows.Forms.Label lbTackTime;
        private System.Windows.Forms.Timer timerTackTime;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbAlarmCode;
        private System.Windows.Forms.Label lbDesc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Label label7;
        private MetroFramework.Controls.MetroButton btnCancel;
    }
}