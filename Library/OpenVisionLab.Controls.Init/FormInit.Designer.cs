namespace OpenVisionLab
{
    partial class FormInit
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
            this.contentPanel = new System.Windows.Forms.Panel();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lbName = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.progressLayout = new System.Windows.Forms.TableLayoutPanel();
            this.progressCanvas = new System.Windows.Forms.Panel();
            this.lbTackTime = new System.Windows.Forms.Label();
            this.circularProgressBar5 = new CircularProgressBar.CircularProgressBar();
            this.lbProcedure = new System.Windows.Forms.Label();
            this.timerTackTime = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contentPanel.SuspendLayout();
            this.mainLayout.SuspendLayout();
            this.progressLayout.SuspendLayout();
            this.progressCanvas.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.contentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.contentPanel.Controls.Add(this.mainLayout);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(18, 30);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(896, 502);
            this.contentPanel.TabIndex = 0;
            // 
            // mainLayout
            // 
            this.mainLayout.BackColor = System.Drawing.Color.Transparent;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.lbName, 0, 0);
            this.mainLayout.Controls.Add(this.lbVersion, 0, 1);
            this.mainLayout.Controls.Add(this.progressLayout, 0, 2);
            this.mainLayout.Controls.Add(this.lbProcedure, 0, 3);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(28, 14, 28, 14);
            this.mainLayout.RowCount = 4;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.mainLayout.Size = new System.Drawing.Size(894, 500);
            this.mainLayout.TabIndex = 0;
            // 
            // lbName
            // 
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbName.Font = new System.Drawing.Font("Microsoft YaHei UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(193)))));
            this.lbName.Location = new System.Drawing.Point(31, 14);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(832, 76);
            this.lbName.TabIndex = 21;
            this.lbName.Text = "Vision Lab";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbVersion
            // 
            this.lbVersion.BackColor = System.Drawing.Color.Transparent;
            this.lbVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbVersion.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(193)))));
            this.lbVersion.Location = new System.Drawing.Point(31, 90);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.lbVersion.Size = new System.Drawing.Size(832, 36);
            this.lbVersion.TabIndex = 22;
            this.lbVersion.Text = "VERSION : 2.0.0 - 2021/09/19 18:20 Noah";
            this.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // progressLayout
            // 
            this.progressLayout.BackColor = System.Drawing.Color.Transparent;
            this.progressLayout.ColumnCount = 1;
            this.progressLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.progressLayout.Controls.Add(this.progressCanvas, 0, 0);
            this.progressLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressLayout.Location = new System.Drawing.Point(31, 129);
            this.progressLayout.Name = "progressLayout";
            this.progressLayout.RowCount = 1;
            this.progressLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.progressLayout.Size = new System.Drawing.Size(832, 304);
            this.progressLayout.TabIndex = 24;
            // 
            // progressCanvas
            // 
            this.progressCanvas.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.progressCanvas.BackColor = System.Drawing.Color.Transparent;
            this.progressCanvas.Controls.Add(this.lbTackTime);
            this.progressCanvas.Controls.Add(this.circularProgressBar5);
            this.progressCanvas.Location = new System.Drawing.Point(266, 2);
            this.progressCanvas.Margin = new System.Windows.Forms.Padding(0);
            this.progressCanvas.Name = "progressCanvas";
            this.progressCanvas.Size = new System.Drawing.Size(300, 300);
            this.progressCanvas.TabIndex = 0;
            // 
            // lbTackTime
            // 
            this.lbTackTime.BackColor = System.Drawing.Color.Transparent;
            this.lbTackTime.Font = new System.Drawing.Font("Consolas", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTackTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(193)))));
            this.lbTackTime.Location = new System.Drawing.Point(0, 171);
            this.lbTackTime.Name = "lbTackTime";
            this.lbTackTime.Size = new System.Drawing.Size(300, 40);
            this.lbTackTime.TabIndex = 12;
            this.lbTackTime.Text = "00:00";
            this.lbTackTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // circularProgressBar5
            // 
            this.circularProgressBar5.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.circularProgressBar5.AnimationSpeed = 100;
            this.circularProgressBar5.BackColor = System.Drawing.Color.Transparent;
            this.circularProgressBar5.Font = new System.Drawing.Font("Arial", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.circularProgressBar5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.circularProgressBar5.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.circularProgressBar5.InnerMargin = 0;
            this.circularProgressBar5.InnerWidth = 0;
            this.circularProgressBar5.Location = new System.Drawing.Point(0, 0);
            this.circularProgressBar5.MarqueeAnimationSpeed = 2000;
            this.circularProgressBar5.Name = "circularProgressBar5";
            this.circularProgressBar5.OuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.circularProgressBar5.OuterMargin = -10;
            this.circularProgressBar5.OuterWidth = 7;
            this.circularProgressBar5.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(193)))));
            this.circularProgressBar5.ProgressWidth = 14;
            this.circularProgressBar5.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 4.125F);
            this.circularProgressBar5.Size = new System.Drawing.Size(300, 300);
            this.circularProgressBar5.StartAngle = 270;
            this.circularProgressBar5.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.circularProgressBar5.SubscriptColor = System.Drawing.Color.Transparent;
            this.circularProgressBar5.SubscriptMargin = new System.Windows.Forms.Padding(0);
            this.circularProgressBar5.SubscriptText = "";
            this.circularProgressBar5.SuperscriptColor = System.Drawing.Color.Transparent;
            this.circularProgressBar5.SuperscriptMargin = new System.Windows.Forms.Padding(0);
            this.circularProgressBar5.SuperscriptText = "";
            this.circularProgressBar5.TabIndex = 11;
            this.circularProgressBar5.Text = "Loading...";
            this.circularProgressBar5.TextMargin = new System.Windows.Forms.Padding(0);
            this.circularProgressBar5.Value = 67;
            // 
            // lbProcedure
            // 
            this.lbProcedure.BackColor = System.Drawing.Color.Transparent;
            this.lbProcedure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbProcedure.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProcedure.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(193)))));
            this.lbProcedure.Location = new System.Drawing.Point(31, 436);
            this.lbProcedure.Name = "lbProcedure";
            this.lbProcedure.Size = new System.Drawing.Size(832, 50);
            this.lbProcedure.TabIndex = 23;
            this.lbProcedure.Text = "1. Load the Config of Initialize...";
            this.lbProcedure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerTackTime
            // 
            this.timerTackTime.Enabled = true;
            this.timerTackTime.Interval = 50;
            this.timerTackTime.Tick += new System.EventHandler(this.timerTackTime_Tick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormInit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(932, 550);
            this.Controls.Add(this.contentPanel);
            this.DisplayHeader = false;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.MinimumSize = new System.Drawing.Size(932, 550);
            this.Name = "FormInit";
            this.Opacity = 0.92D;
            this.Padding = new System.Windows.Forms.Padding(18, 30, 18, 18);
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "INIT";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormInit_Load);
            this.Shown += new System.EventHandler(this.FormInit_Shown);
            this.contentPanel.ResumeLayout(false);
            this.mainLayout.ResumeLayout(false);
            this.progressLayout.ResumeLayout(false);
            this.progressCanvas.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.TableLayoutPanel progressLayout;
        private System.Windows.Forms.Panel progressCanvas;
        private CircularProgressBar.CircularProgressBar circularProgressBar5;
        private System.Windows.Forms.Label lbTackTime;
        private System.Windows.Forms.Timer timerTackTime;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbVersion;
        private System.Windows.Forms.Label lbProcedure;
        private System.Windows.Forms.Timer timer1;
    }
}
