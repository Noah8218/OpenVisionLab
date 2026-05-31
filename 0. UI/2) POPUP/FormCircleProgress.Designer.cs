
namespace KtemVisionSystem
{
    partial class FormCircleProgress
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
            this.SuspendLayout();
            // 
            // circularProgressBar5
            // 
            this.circularProgressBar5.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.circularProgressBar5.AnimationSpeed = 500;
            this.circularProgressBar5.BackColor = System.Drawing.Color.Transparent;
            this.circularProgressBar5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.circularProgressBar5.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.circularProgressBar5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.circularProgressBar5.InnerColor = System.Drawing.Color.Black;
            this.circularProgressBar5.InnerMargin = 0;
            this.circularProgressBar5.InnerWidth = 0;
            this.circularProgressBar5.Location = new System.Drawing.Point(0, 0);
            this.circularProgressBar5.MarqueeAnimationSpeed = 2000;
            this.circularProgressBar5.Name = "circularProgressBar5";
            this.circularProgressBar5.OuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.circularProgressBar5.OuterMargin = -11;
            this.circularProgressBar5.OuterWidth = 8;
            this.circularProgressBar5.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.circularProgressBar5.ProgressWidth = 14;
            this.circularProgressBar5.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 4.125F);
            this.circularProgressBar5.Size = new System.Drawing.Size(600, 450);
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
            this.lbTackTime.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTackTime.ForeColor = System.Drawing.Color.DarkCyan;
            this.lbTackTime.Location = new System.Drawing.Point(246, 279);
            this.lbTackTime.Name = "lbTackTime";
            this.lbTackTime.Size = new System.Drawing.Size(107, 37);
            this.lbTackTime.TabIndex = 12;
            this.lbTackTime.Text = "00:00";
            // 
            // timerTackTime
            // 
            this.timerTackTime.Enabled = true;
            this.timerTackTime.Tick += new System.EventHandler(this.timerTackTime_Tick);
            // 
            // FormCircleProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(600, 450);
            this.Controls.Add(this.lbTackTime);
            this.Controls.Add(this.circularProgressBar5);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCircleProgress";
            this.Opacity = 0.7D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormCircleProgress";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormCircleProgress_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CircularProgressBar.CircularProgressBar circularProgressBar5;
        private System.Windows.Forms.Label lbTackTime;
        private System.Windows.Forms.Timer timerTackTime;
    }
}