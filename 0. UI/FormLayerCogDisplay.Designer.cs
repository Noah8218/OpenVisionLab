namespace KtemVisionSystem
{
    partial class FormLayerCogDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLayerCogDisplay));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cogDisplay_Source = new Cognex.VisionPro.Display.CogDisplay();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay_Source)).BeginInit();
            this.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.cogDisplay_Source);
            this.splitContainer1.Size = new System.Drawing.Size(525, 313);
            this.splitContainer1.SplitterDistance = 284;
            this.splitContainer1.TabIndex = 0;
            // 
            // cogDisplay_Source
            // 
            this.cogDisplay_Source.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogDisplay_Source.ColorMapLowerRoiLimit = 0D;
            this.cogDisplay_Source.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogDisplay_Source.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogDisplay_Source.ColorMapUpperRoiLimit = 1D;
            this.cogDisplay_Source.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogDisplay_Source.DoubleTapZoomCycleLength = 2;
            this.cogDisplay_Source.DoubleTapZoomSensitivity = 2.5D;
            this.cogDisplay_Source.Location = new System.Drawing.Point(0, 0);
            this.cogDisplay_Source.Margin = new System.Windows.Forms.Padding(4);
            this.cogDisplay_Source.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogDisplay_Source.MouseWheelSensitivity = 1D;
            this.cogDisplay_Source.Name = "cogDisplay_Source";
            this.cogDisplay_Source.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogDisplay_Source.OcxState")));
            this.cogDisplay_Source.Size = new System.Drawing.Size(525, 284);
            this.cogDisplay_Source.TabIndex = 1762;
            // 
            // FormLayerDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(525, 313);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormLayerDisplay";
            this.Text = "LayerDisplay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LayerDisplay_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay_Source)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        internal Cognex.VisionPro.Display.CogDisplay cogDisplay_Source;
    }
}