using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormMainButton
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainButton));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.rjButton1 = new RJCodeUI_M1.RJControls.RJButton();
            this.btnManualTest = new RJCodeUI_M1.RJControls.RJButton();
            this.btnCross = new RJCodeUI_M1.RJControls.RJButton();
            this.btnLive = new RJCodeUI_M1.RJControls.RJButton();
            this.btnGrab = new RJCodeUI_M1.RJControls.RJButton();
            this.btnOperation = new RJCodeUI_M1.RJControls.RJButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.panel1.Controls.Add(this.rjButton1);
            this.panel1.Controls.Add(this.btnManualTest);
            this.panel1.Controls.Add(this.btnCross);
            this.panel1.Controls.Add(this.btnLive);
            this.panel1.Controls.Add(this.btnGrab);
            this.panel1.Controls.Add(this.btnOperation);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1430, 137);
            this.panel1.TabIndex = 2141;
            // 
            // rjButton1
            // 
            this.rjButton1.BackColor = System.Drawing.Color.White;
            this.rjButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.BorderRadius = 15;
            this.rjButton1.BorderSize = 3;
            this.rjButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton1.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.rjButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.rjButton1.FlatAppearance.BorderSize = 3;
            this.rjButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.rjButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.rjButton1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton1.IconSize = 95;
            this.rjButton1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rjButton1.Location = new System.Drawing.Point(750, 5);
            this.rjButton1.Name = "rjButton1";
            this.rjButton1.Size = new System.Drawing.Size(150, 120);
            this.rjButton1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.rjButton1.TabIndex = 2145;
            this.rjButton1.Text = "에이징 시작";
            this.rjButton1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rjButton1.UseVisualStyleBackColor = false;
            this.rjButton1.Click += new System.EventHandler(this.rjButton1_Click);
            // 
            // btnManualTest
            // 
            this.btnManualTest.BackColor = System.Drawing.Color.White;
            this.btnManualTest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnManualTest.BorderRadius = 15;
            this.btnManualTest.BorderSize = 3;
            this.btnManualTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnManualTest.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnManualTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnManualTest.FlatAppearance.BorderSize = 3;
            this.btnManualTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnManualTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnManualTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManualTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManualTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnManualTest.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.btnManualTest.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnManualTest.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnManualTest.IconSize = 95;
            this.btnManualTest.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnManualTest.Location = new System.Drawing.Point(600, 5);
            this.btnManualTest.Name = "btnManualTest";
            this.btnManualTest.Size = new System.Drawing.Size(150, 120);
            this.btnManualTest.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnManualTest.TabIndex = 2144;
            this.btnManualTest.Text = "단일 검사";
            this.btnManualTest.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnManualTest.UseVisualStyleBackColor = false;            
            // 
            // btnCross
            // 
            this.btnCross.BackColor = System.Drawing.Color.White;
            this.btnCross.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnCross.BorderRadius = 15;
            this.btnCross.BorderSize = 3;
            this.btnCross.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCross.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnCross.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnCross.FlatAppearance.BorderSize = 3;
            this.btnCross.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnCross.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCross.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCross.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCross.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnCross.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnCross.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnCross.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCross.IconSize = 95;
            this.btnCross.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCross.Location = new System.Drawing.Point(450, 5);
            this.btnCross.Name = "btnCross";
            this.btnCross.Size = new System.Drawing.Size(150, 120);
            this.btnCross.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnCross.TabIndex = 2143;
            this.btnCross.Text = "CROSS";
            this.btnCross.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCross.UseVisualStyleBackColor = false;
            this.btnCross.Click += new System.EventHandler(this.btnCross_Click);
            // 
            // btnLive
            // 
            this.btnLive.BackColor = System.Drawing.Color.White;
            this.btnLive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLive.BorderRadius = 15;
            this.btnLive.BorderSize = 3;
            this.btnLive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLive.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnLive.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnLive.FlatAppearance.BorderSize = 3;
            this.btnLive.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnLive.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLive.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLive.IconChar = FontAwesome.Sharp.IconChar.Youtube;
            this.btnLive.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLive.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLive.IconSize = 95;
            this.btnLive.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnLive.Location = new System.Drawing.Point(300, 5);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(150, 120);
            this.btnLive.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnLive.TabIndex = 2142;
            this.btnLive.Text = "LIVE";
            this.btnLive.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnLive.UseVisualStyleBackColor = false;
            this.btnLive.Click += new System.EventHandler(this.btnLive_Click);
            // 
            // btnGrab
            // 
            this.btnGrab.BackColor = System.Drawing.Color.White;
            this.btnGrab.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnGrab.BorderRadius = 15;
            this.btnGrab.BorderSize = 3;
            this.btnGrab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGrab.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnGrab.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnGrab.FlatAppearance.BorderSize = 3;
            this.btnGrab.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnGrab.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnGrab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGrab.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnGrab.IconChar = FontAwesome.Sharp.IconChar.Camera;
            this.btnGrab.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnGrab.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnGrab.IconSize = 95;
            this.btnGrab.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnGrab.Location = new System.Drawing.Point(150, 5);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(150, 120);
            this.btnGrab.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnGrab.TabIndex = 2141;
            this.btnGrab.Text = "GRAB";
            this.btnGrab.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGrab.UseVisualStyleBackColor = false;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // btnOperation
            // 
            this.btnOperation.BackColor = System.Drawing.Color.White;
            this.btnOperation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnOperation.BorderRadius = 15;
            this.btnOperation.BorderSize = 3;
            this.btnOperation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOperation.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnOperation.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnOperation.FlatAppearance.BorderSize = 3;
            this.btnOperation.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnOperation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOperation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOperation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnOperation.IconChar = FontAwesome.Sharp.IconChar.PlayCircle;
            this.btnOperation.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnOperation.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnOperation.IconSize = 95;
            this.btnOperation.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOperation.Location = new System.Drawing.Point(0, 5);
            this.btnOperation.Name = "btnOperation";
            this.btnOperation.Size = new System.Drawing.Size(150, 120);
            this.btnOperation.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnOperation.TabIndex = 2136;
            this.btnOperation.Text = "START";
            this.btnOperation.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOperation.UseVisualStyleBackColor = false;
            this.btnOperation.Click += new System.EventHandler(this.btnOperation_Click);
            // 
            // FormMainButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1430, 137);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormMainButton";
            this.Text = "Operation";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.FormLayerDisplay_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private RJCodeUI_M1.RJControls.RJButton btnManualTest;
        private RJCodeUI_M1.RJControls.RJButton btnCross;
        private RJCodeUI_M1.RJControls.RJButton btnLive;
        private RJCodeUI_M1.RJControls.RJButton btnGrab;
        private RJCodeUI_M1.RJControls.RJButton btnOperation;
        private RJCodeUI_M1.RJControls.RJButton rjButton1;
    }
}