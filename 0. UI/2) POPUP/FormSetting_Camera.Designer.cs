namespace OpenVisionLab
{
    partial class FormSetting_Camera
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid_Cam = new System.Windows.Forms.PropertyGrid();
            this.cbCamera = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnSetCamera = new RJCodeUI_M1.RJControls.RJButton();
            this.tbCameraCount = new RJCodeUI_M1.RJControls.RJTextBox();
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.btnSaveParameter = new RJCodeUI_M1.RJControls.RJButton();
            this.pnlClientArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.splitContainer1);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(710, 615);
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
            this.splitContainer1.Panel1.Controls.Add(this.propertyGrid_Cam);
            this.splitContainer1.Panel1.Controls.Add(this.cbCamera);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnSetCamera);
            this.splitContainer1.Panel2.Controls.Add(this.tbCameraCount);
            this.splitContainer1.Panel2.Controls.Add(this.rjLabel3);
            this.splitContainer1.Panel2.Controls.Add(this.btnSaveParameter);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(710, 615);
            this.splitContainer1.SplitterDistance = 499;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 2134;
            // 
            // propertyGrid_Cam
            // 
            this.propertyGrid_Cam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid_Cam.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGrid_Cam.Location = new System.Drawing.Point(0, 32);
            this.propertyGrid_Cam.Name = "propertyGrid_Cam";
            this.propertyGrid_Cam.Size = new System.Drawing.Size(710, 467);
            this.propertyGrid_Cam.TabIndex = 2147;
            // 
            // cbCamera
            // 
            this.cbCamera.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbCamera.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbCamera.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.cbCamera.BorderRadius = 0;
            this.cbCamera.BorderSize = 2;
            this.cbCamera.Customizable = true;
            this.cbCamera.DataSource = null;
            this.cbCamera.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbCamera.DropDownBackColor = System.Drawing.SystemColors.Window;
            this.cbCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCamera.DropDownTextColor = System.Drawing.SystemColors.WindowText;
            this.cbCamera.IconColor = System.Drawing.Color.CornflowerBlue;
            this.cbCamera.Location = new System.Drawing.Point(0, 0);
            this.cbCamera.Name = "cbCamera";
            this.cbCamera.Padding = new System.Windows.Forms.Padding(3);
            this.cbCamera.SelectedIndex = -1;
            this.cbCamera.Size = new System.Drawing.Size(710, 32);
            this.cbCamera.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbCamera.TabIndex = 2146;
            this.cbCamera.Texts = "";
            this.cbCamera.OnSelectedIndexChanged += new System.EventHandler(this.cbCamera_OnSelectedIndexChanged);
            // 
            // btnSetCamera
            // 
            this.btnSetCamera.BackColor = System.Drawing.Color.White;
            this.btnSetCamera.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSetCamera.BorderRadius = 15;
            this.btnSetCamera.BorderSize = 3;
            this.btnSetCamera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSetCamera.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSetCamera.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSetCamera.FlatAppearance.BorderSize = 3;
            this.btnSetCamera.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSetCamera.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSetCamera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F);
            this.btnSetCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSetCamera.IconChar = FontAwesome.Sharp.IconChar.Edit;
            this.btnSetCamera.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSetCamera.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSetCamera.IconSize = 80;
            this.btnSetCamera.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSetCamera.Location = new System.Drawing.Point(283, 5);
            this.btnSetCamera.Name = "btnSetCamera";
            this.btnSetCamera.Size = new System.Drawing.Size(230, 105);
            this.btnSetCamera.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSetCamera.TabIndex = 2147;
            this.btnSetCamera.Text = "Count Change";
            this.btnSetCamera.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSetCamera.UseVisualStyleBackColor = false;
            this.btnSetCamera.Click += new System.EventHandler(this.btnSetCamera_Click);
            // 
            // tbCameraCount
            // 
            this.tbCameraCount._Customizable = true;
            this.tbCameraCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbCameraCount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbCameraCount.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbCameraCount.BorderRadius = 0;
            this.tbCameraCount.BorderSize = 1;
            this.tbCameraCount.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbCameraCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbCameraCount.Location = new System.Drawing.Point(169, 5);
            this.tbCameraCount.MultiLine = false;
            this.tbCameraCount.Name = "tbCameraCount";
            this.tbCameraCount.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbCameraCount.PasswordChar = false;
            this.tbCameraCount.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbCameraCount.PlaceHolderText = "10";
            this.tbCameraCount.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbCameraCount.Size = new System.Drawing.Size(108, 31);
            this.tbCameraCount.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbCameraCount.TabIndex = 2146;
            this.tbCameraCount.onTextChanged += new System.EventHandler(this.tbCameraCount_onTextChanged);
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(6, 8);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(164, 23);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel3.TabIndex = 2145;
            this.rjLabel3.Text = "Camera Count :";
            this.rjLabel3.Click += new System.EventHandler(this.rjLabel3_Click);
            // 
            // btnSaveParameter
            // 
            this.btnSaveParameter.BackColor = System.Drawing.Color.White;
            this.btnSaveParameter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.BorderRadius = 15;
            this.btnSaveParameter.BorderSize = 3;
            this.btnSaveParameter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveParameter.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSaveParameter.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSaveParameter.FlatAppearance.BorderSize = 3;
            this.btnSaveParameter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSaveParameter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSaveParameter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveParameter.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveParameter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveParameter.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveParameter.IconSize = 80;
            this.btnSaveParameter.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveParameter.Location = new System.Drawing.Point(515, 5);
            this.btnSaveParameter.Name = "btnSaveParameter";
            this.btnSaveParameter.Size = new System.Drawing.Size(180, 105);
            this.btnSaveParameter.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSaveParameter.TabIndex = 2144;
            this.btnSaveParameter.Text = "SAVE";
            this.btnSaveParameter.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveParameter.UseVisualStyleBackColor = false;
            this.btnSaveParameter.Click += new System.EventHandler(this.btnSaveParameter_Click);
            // 
            // FormSetting_Camera
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "CAMERA SETTINGS";
            this.ClientSize = new System.Drawing.Size(712, 657);
            this.Name = "FormSetting_Camera";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "CAMERA SETTINGS";
            this.Load += new System.EventHandler(this.Form_Load);
            this.pnlClientArea.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid propertyGrid_Cam;
        private RJCodeUI_M1.RJControls.RJComboBox cbCamera;
        private RJCodeUI_M1.RJControls.RJButton btnSetCamera;
        private RJCodeUI_M1.RJControls.RJTextBox tbCameraCount;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private RJCodeUI_M1.RJControls.RJButton btnSaveParameter;
    }
}