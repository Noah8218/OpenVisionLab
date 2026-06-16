namespace OpenVisionLab
{
    partial class FormVision_FeatureMatching
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
                        this.ibSource = new OpenVisionLab.VisionTestImageCanvas();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.metroTabControl2 = new System.Windows.Forms.TabControl();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new OpenVisionLab.VisionTestImageCanvas();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnRun = new RJCodeUI_M1.RJControls.RJButton();
            this.pnParameter = new RJCodeUI_M1.RJControls.RJPanel();
            this.groupBoxFeatureReview = new System.Windows.Forms.GroupBox();
            this.pbTemplate = new System.Windows.Forms.PictureBox();
            this.pbDetectedCrop = new System.Windows.Forms.PictureBox();
            this.lblTemplateTitle = new System.Windows.Forms.Label();
            this.lblDetectedTitle = new System.Windows.Forms.Label();
            this.lblFeatureSummaryTitle = new System.Windows.Forms.Label();
            this.lblFeatureSummary = new System.Windows.Forms.Label();
            this.pnlClientArea.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).BeginInit();
            this.groupBoxFeatureReview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDetectedCrop)).BeginInit();
            this.SuspendLayout();            
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.groupBoxFeatureReview);
            this.pnlClientArea.Controls.Add(this.pnParameter);
            this.pnlClientArea.Controls.Add(this.btnRun);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(918, 613);
            // 
            // 
            // 
            // ibSource
            // 
            this.ibSource.Location = new System.Drawing.Point(8, 20);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(374, 220);
            this.ibSource.TabIndex = 2149;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox3.Controls.Add(this.metroTabControl2);
            this.groupBox3.Controls.Add(this.cbLayerList);
            this.groupBox3.Controls.Add(this.ibSource);
            this.groupBox3.Location = new System.Drawing.Point(16, 58);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(390, 285);
            this.groupBox3.TabIndex = 2154;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Input Layer";
            // 
            // metroTabControl2
            // 
            this.metroTabControl2.Location = new System.Drawing.Point(70, 133);
            this.metroTabControl2.Name = "metroTabControl2";
            this.metroTabControl2.Size = new System.Drawing.Size(8, 8);
            this.metroTabControl2.TabIndex = 2159;
            // 
            // cbLayerList
            // 
            this.cbLayerList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.BorderRadius = 3;
            this.cbLayerList.BorderSize = 2;
            this.cbLayerList.Customizable = false;
            this.cbLayerList.DataSource = null;
            this.cbLayerList.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbLayerList.Location = new System.Drawing.Point(8, 248);
            this.cbLayerList.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList.Name = "cbLayerList";
            this.cbLayerList.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList.SelectedIndex = -1;
            this.cbLayerList.Size = new System.Drawing.Size(374, 32);
            this.cbLayerList.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList.TabIndex = 2158;
            this.cbLayerList.Texts = "";
            this.cbLayerList.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox4.Controls.Add(this.cbLayerList2);
            this.groupBox4.Controls.Add(this.btnNewPanel_Desty);
            this.groupBox4.Controls.Add(this.ibDestination);
            this.groupBox4.Location = new System.Drawing.Point(16, 360);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(390, 285);
            this.groupBox4.TabIndex = 2155;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output Layer";
            // 
            // cbLayerList2
            // 
            this.cbLayerList2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.BorderRadius = 3;
            this.cbLayerList2.BorderSize = 2;
            this.cbLayerList2.Customizable = false;
            this.cbLayerList2.DataSource = null;
            this.cbLayerList2.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList2.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList2.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbLayerList2.Location = new System.Drawing.Point(8, 248);
            this.cbLayerList2.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList2.Name = "cbLayerList2";
            this.cbLayerList2.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList2.SelectedIndex = -1;
            this.cbLayerList2.Size = new System.Drawing.Size(338, 32);
            this.cbLayerList2.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList2.TabIndex = 2159;
            this.cbLayerList2.Texts = "";
            this.cbLayerList2.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList2_SelectedIndexChanged);
            // 
            // btnNewPanel_Desty
            // 
            this.btnNewPanel_Desty.BackColor = System.Drawing.Color.Transparent;
            this.btnNewPanel_Desty.BackIcon = true;
            this.btnNewPanel_Desty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPanel_Desty.Customizable = true;
            this.btnNewPanel_Desty.DropdownMenu = null;
            this.btnNewPanel_Desty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel_Desty.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            this.btnNewPanel_Desty.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel_Desty.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewPanel_Desty.IconSize = 30;
            this.btnNewPanel_Desty.Location = new System.Drawing.Point(354, 250);
            this.btnNewPanel_Desty.Name = "btnNewPanel_Desty";
            this.btnNewPanel_Desty.Size = new System.Drawing.Size(28, 28);
            this.btnNewPanel_Desty.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.btnNewPanel_Desty.TabIndex = 2157;
            this.btnNewPanel_Desty.TabStop = false;
            this.btnNewPanel_Desty.Click += new System.EventHandler(this.btnNewPanel_Desty_Click);
            // 
            // ibDestination
            // 
            this.ibDestination.Location = new System.Drawing.Point(8, 20);
            this.ibDestination.Name = "ibDestination";
            this.ibDestination.Size = new System.Drawing.Size(374, 220);
            this.ibDestination.TabIndex = 2149;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.btnRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(171)))));
            this.btnRun.BorderRadius = 3;
            this.btnRun.BorderSize = 1;
            this.btnRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(171)))));
            this.btnRun.FlatAppearance.BorderSize = 1;
            this.btnRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(241)))), ((int)(((byte)(247)))));
            this.btnRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(246)))), ((int)(((byte)(251)))));
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.btnRun.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.btnRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnRun.IconSize = 1;
            this.btnRun.Location = new System.Drawing.Point(423, 563);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(488, 40);
            this.btnRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnRun.TabIndex = 2153;
            this.btnRun.Text = "Run";
            this.btnRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRun.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // pnParameter
            // 
            this.pnParameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.pnParameter.BorderRadius = 5;
            this.pnParameter.Customizable = false;
            this.pnParameter.Location = new System.Drawing.Point(423, 17);
            this.pnParameter.Name = "pnParameter";
            this.pnParameter.Size = new System.Drawing.Size(488, 327);
            this.pnParameter.TabIndex = 2162;
            // 
            // groupBoxFeatureReview
            // 
            this.groupBoxFeatureReview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBoxFeatureReview.Controls.Add(this.lblFeatureSummary);
            this.groupBoxFeatureReview.Controls.Add(this.lblFeatureSummaryTitle);
            this.groupBoxFeatureReview.Controls.Add(this.lblDetectedTitle);
            this.groupBoxFeatureReview.Controls.Add(this.lblTemplateTitle);
            this.groupBoxFeatureReview.Controls.Add(this.pbDetectedCrop);
            this.groupBoxFeatureReview.Controls.Add(this.pbTemplate);
            this.groupBoxFeatureReview.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupBoxFeatureReview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.groupBoxFeatureReview.Location = new System.Drawing.Point(423, 350);
            this.groupBoxFeatureReview.Name = "groupBoxFeatureReview";
            this.groupBoxFeatureReview.Size = new System.Drawing.Size(488, 197);
            this.groupBoxFeatureReview.TabIndex = 2163;
            this.groupBoxFeatureReview.TabStop = false;
            this.groupBoxFeatureReview.Text = "Feature Review";
            // 
            // pbTemplate
            // 
            this.pbTemplate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(24)))));
            this.pbTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbTemplate.Location = new System.Drawing.Point(13, 45);
            this.pbTemplate.Name = "pbTemplate";
            this.pbTemplate.Size = new System.Drawing.Size(136, 139);
            this.pbTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbTemplate.TabIndex = 0;
            this.pbTemplate.TabStop = false;
            // 
            // pbDetectedCrop
            // 
            this.pbDetectedCrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(24)))));
            this.pbDetectedCrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbDetectedCrop.Location = new System.Drawing.Point(164, 45);
            this.pbDetectedCrop.Name = "pbDetectedCrop";
            this.pbDetectedCrop.Size = new System.Drawing.Size(136, 139);
            this.pbDetectedCrop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbDetectedCrop.TabIndex = 1;
            this.pbDetectedCrop.TabStop = false;
            // 
            // lblTemplateTitle
            // 
            this.lblTemplateTitle.AutoSize = true;
            this.lblTemplateTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTemplateTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.lblTemplateTitle.Location = new System.Drawing.Point(13, 23);
            this.lblTemplateTitle.Name = "lblTemplateTitle";
            this.lblTemplateTitle.Size = new System.Drawing.Size(98, 13);
            this.lblTemplateTitle.TabIndex = 2;
            this.lblTemplateTitle.Text = "Feature Template";
            // 
            // lblDetectedTitle
            // 
            this.lblDetectedTitle.AutoSize = true;
            this.lblDetectedTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDetectedTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.lblDetectedTitle.Location = new System.Drawing.Point(164, 23);
            this.lblDetectedTitle.Name = "lblDetectedTitle";
            this.lblDetectedTitle.Size = new System.Drawing.Size(82, 13);
            this.lblDetectedTitle.TabIndex = 3;
            this.lblDetectedTitle.Text = "Detected Crop";
            // 
            // lblFeatureSummaryTitle
            // 
            this.lblFeatureSummaryTitle.AutoSize = true;
            this.lblFeatureSummaryTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFeatureSummaryTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.lblFeatureSummaryTitle.Location = new System.Drawing.Point(316, 23);
            this.lblFeatureSummaryTitle.Name = "lblFeatureSummaryTitle";
            this.lblFeatureSummaryTitle.Size = new System.Drawing.Size(80, 13);
            this.lblFeatureSummaryTitle.TabIndex = 4;
            this.lblFeatureSummaryTitle.Text = "Feature Result";
            // 
            // lblFeatureSummary
            // 
            this.lblFeatureSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.lblFeatureSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFeatureSummary.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFeatureSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.lblFeatureSummary.Location = new System.Drawing.Point(316, 45);
            this.lblFeatureSummary.Name = "lblFeatureSummary";
            this.lblFeatureSummary.Padding = new System.Windows.Forms.Padding(6);
            this.lblFeatureSummary.Size = new System.Drawing.Size(157, 139);
            this.lblFeatureSummary.TabIndex = 5;
            this.lblFeatureSummary.Text = "Template: -\r\nCrop: -\r\nOverlay: Output";
            // 
            // FormVision_FeatureMatching
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "FeatureMatching";
            this.ClientSize = new System.Drawing.Size(920, 655);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_FeatureMatching";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "FeatureMatching";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).EndInit();
            this.groupBoxFeatureReview.ResumeLayout(false);
            this.groupBoxFeatureReview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDetectedCrop)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private VisionTestImageCanvas ibSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private VisionTestImageCanvas ibDestination;
        private RJCodeUI_M1.RJControls.RJButton btnRun;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel_Desty;
        private System.Windows.Forms.ToolTip toolTip1;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList2;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
        private System.Windows.Forms.TabControl metroTabControl2;
        private RJCodeUI_M1.RJControls.RJPanel pnParameter;
        private System.Windows.Forms.GroupBox groupBoxFeatureReview;
        private System.Windows.Forms.PictureBox pbTemplate;
        private System.Windows.Forms.PictureBox pbDetectedCrop;
        private System.Windows.Forms.Label lblTemplateTitle;
        private System.Windows.Forms.Label lblDetectedTitle;
        private System.Windows.Forms.Label lblFeatureSummaryTitle;
        private System.Windows.Forms.Label lblFeatureSummary;
    }
}



