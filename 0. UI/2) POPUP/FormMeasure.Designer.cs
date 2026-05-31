
using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormMeasure
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.tbPixelPermm = new RJCodeUI_M1.RJControls.RJTextBox();
            this.ibSource = new Cyotek.Windows.Forms.ImageBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.xLocationToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.yLocationToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbVertical = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbContrast = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerDisplay = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(958, 726);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(1, 41);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ibSource);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(958, 726);
            this.splitContainer1.SplitterDistance = 39;
            this.splitContainer1.TabIndex = 1037;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.White;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(958, 39);
            this.splitContainer2.SplitterDistance = 660;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer3.Panel1.Controls.Add(this.label4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.tbPixelPermm);
            this.splitContainer3.Size = new System.Drawing.Size(294, 39);
            this.splitContainer3.SplitterDistance = 159;
            this.splitContainer3.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 25);
            this.label4.TabIndex = 2142;
            this.label4.Text = "Pixel Size";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbPixelPermm
            // 
            this.tbPixelPermm._Customizable = true;
            this.tbPixelPermm.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbPixelPermm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbPixelPermm.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbPixelPermm.BorderRadius = 0;
            this.tbPixelPermm.BorderSize = 1;
            this.tbPixelPermm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbPixelPermm.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbPixelPermm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbPixelPermm.Location = new System.Drawing.Point(0, 0);
            this.tbPixelPermm.MultiLine = false;
            this.tbPixelPermm.Name = "tbPixelPermm";
            this.tbPixelPermm.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbPixelPermm.PasswordChar = false;
            this.tbPixelPermm.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbPixelPermm.PlaceHolderText = "0.5";
            this.tbPixelPermm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbPixelPermm.Size = new System.Drawing.Size(131, 31);
            this.tbPixelPermm.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbPixelPermm.TabIndex = 2142;
            // 
            // ibSource
            // 
            this.ibSource.BackColor = System.Drawing.Color.Black;
            this.ibSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibSource.GridColor = System.Drawing.Color.Black;
            this.ibSource.GridColorAlternate = System.Drawing.Color.Black;
            this.ibSource.Location = new System.Drawing.Point(0, 0);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(958, 661);
            this.ibSource.TabIndex = 1038;
            this.ibSource.VirtualSize = new System.Drawing.Size(256, 256);
            this.ibSource.Paint += new System.Windows.Forms.PaintEventHandler(this.ibSource_Paint);
            this.ibSource.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pblImage_KeyDown);
            this.ibSource.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ibSource_KeyUp);
            this.ibSource.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ibSource_MouseDown);
            this.ibSource.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibSource_MouseMove);
            this.ibSource.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ibSource_MouseUp);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lbMode,
            this.toolStripStatusLabel2,
            this.xLocationToolStripStatusLabel,
            this.toolStripStatusLabel4,
            this.yLocationToolStripStatusLabel,
            this.toolStripStatusLabel5,
            this.lbVertical,
            this.toolStripStatusLabel3,
            this.lbContrast});
            this.statusStrip1.Location = new System.Drawing.Point(0, 661);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.statusStrip1.Size = new System.Drawing.Size(958, 22);
            this.statusStrip1.TabIndex = 1037;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(94, 17);
            this.toolStripStatusLabel1.Text = "Selected Mode :";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbMode
            // 
            this.lbMode.AutoSize = false;
            this.lbMode.BackColor = System.Drawing.Color.Transparent;
            this.lbMode.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lbMode.Name = "lbMode";
            this.lbMode.Size = new System.Drawing.Size(150, 17);
            this.lbMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(67, 17);
            this.toolStripStatusLabel2.Text = "X Location:";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // xLocationToolStripStatusLabel
            // 
            this.xLocationToolStripStatusLabel.AutoSize = false;
            this.xLocationToolStripStatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.xLocationToolStripStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.xLocationToolStripStatusLabel.Name = "xLocationToolStripStatusLabel";
            this.xLocationToolStripStatusLabel.Size = new System.Drawing.Size(60, 17);
            this.xLocationToolStripStatusLabel.Text = "0.0";
            this.xLocationToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(67, 17);
            this.toolStripStatusLabel4.Text = "Y Location:";
            this.toolStripStatusLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // yLocationToolStripStatusLabel
            // 
            this.yLocationToolStripStatusLabel.AutoSize = false;
            this.yLocationToolStripStatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.yLocationToolStripStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.yLocationToolStripStatusLabel.Name = "yLocationToolStripStatusLabel";
            this.yLocationToolStripStatusLabel.Size = new System.Drawing.Size(60, 17);
            this.yLocationToolStripStatusLabel.Text = "0.0";
            this.yLocationToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(60, 17);
            this.toolStripStatusLabel5.Text = "Distance :";
            this.toolStripStatusLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbVertical
            // 
            this.lbVertical.AutoSize = false;
            this.lbVertical.BackColor = System.Drawing.Color.Transparent;
            this.lbVertical.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lbVertical.Name = "lbVertical";
            this.lbVertical.Size = new System.Drawing.Size(150, 17);
            this.lbVertical.Text = "0.0";
            this.lbVertical.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(72, 17);
            this.toolStripStatusLabel3.Text = "Grey Value :";
            this.toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbContrast
            // 
            this.lbContrast.AutoSize = false;
            this.lbContrast.BackColor = System.Drawing.Color.Transparent;
            this.lbContrast.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lbContrast.Name = "lbContrast";
            this.lbContrast.Size = new System.Drawing.Size(150, 17);
            this.lbContrast.Text = "0.0";
            this.lbContrast.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timerDisplay
            // 
            this.timerDisplay.Enabled = true;
            this.timerDisplay.Interval = 30;
            this.timerDisplay.Tick += new System.EventHandler(this.timerDisplay_Tick);
            // 
            // FormMeasure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "MEASURE";
            this.ClientSize = new System.Drawing.Size(960, 768);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormMeasure";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "MEASURE";
            this.Load += new System.EventHandler(this.FormPoint_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ImageBox ibSource;
        private System.Windows.Forms.Timer timerDisplay;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lbMode;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel xLocationToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel yLocationToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel lbVertical;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel lbContrast;
        private System.Windows.Forms.Label label4;
        private RJCodeUI_M1.RJControls.RJTextBox tbPixelPermm;
    }
}