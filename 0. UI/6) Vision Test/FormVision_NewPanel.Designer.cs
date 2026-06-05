namespace OpenVisionLab
{
    partial class FormVision_NewPanel
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
                        this.tbNewPanel = new RJCodeUI_M1.RJControls.RJTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNewCancel = new RJCodeUI_M1.RJControls.RJButton();
            this.btnNewCreate = new RJCodeUI_M1.RJControls.RJButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlClientArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.btnNewCancel);
            this.pnlClientArea.Controls.Add(this.btnNewCreate);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(398, 138);
            // 
            // 
            // 
            // tbNewPanel
            // 
            this.tbNewPanel._Customizable = true;
            this.tbNewPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbNewPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbNewPanel.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbNewPanel.BorderRadius = 0;
            this.tbNewPanel.BorderSize = 1;
            this.tbNewPanel.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbNewPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbNewPanel.Location = new System.Drawing.Point(108, 63);
            this.tbNewPanel.MultiLine = false;
            this.tbNewPanel.Name = "tbNewPanel";
            this.tbNewPanel.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbNewPanel.PasswordChar = false;
            this.tbNewPanel.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbNewPanel.PlaceHolderText = "3";
            this.tbNewPanel.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbNewPanel.Size = new System.Drawing.Size(277, 31);
            this.tbNewPanel.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbNewPanel.TabIndex = 2152;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.label1.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(22, 69);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 25);
            this.label1.TabIndex = 2151;
            this.label1.Text = "Name";
            // 
            // btnNewCancel
            // 
            this.btnNewCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.btnNewCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.btnNewCancel.BorderRadius = 10;
            this.btnNewCancel.BorderSize = 1;
            this.btnNewCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewCancel.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnNewCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnNewCancel.FlatAppearance.BorderSize = 0;
            this.btnNewCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(103)))), ((int)(((byte)(125)))));
            this.btnNewCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(96)))), ((int)(((byte)(117)))));
            this.btnNewCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewCancel.ForeColor = System.Drawing.Color.White;
            this.btnNewCancel.IconChar = FontAwesome.Sharp.IconChar.TimesCircle;
            this.btnNewCancel.IconColor = System.Drawing.Color.White;
            this.btnNewCancel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewCancel.IconSize = 24;
            this.btnNewCancel.Location = new System.Drawing.Point(294, 100);
            this.btnNewCancel.Name = "btnNewCancel";
            this.btnNewCancel.Size = new System.Drawing.Size(90, 35);
            this.btnNewCancel.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnNewCancel.TabIndex = 2153;
            this.btnNewCancel.Text = "Cancel";
            this.btnNewCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNewCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNewCancel.UseVisualStyleBackColor = false;
            this.btnNewCancel.Click += new System.EventHandler(this.btnNewCancel_Click);
            // 
            // btnNewCreate
            // 
            this.btnNewCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnNewCreate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnNewCreate.BorderRadius = 10;
            this.btnNewCreate.BorderSize = 1;
            this.btnNewCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewCreate.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnNewCreate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnNewCreate.FlatAppearance.BorderSize = 0;
            this.btnNewCreate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(149)))), ((int)(((byte)(106)))));
            this.btnNewCreate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(139)))), ((int)(((byte)(99)))));
            this.btnNewCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewCreate.ForeColor = System.Drawing.Color.White;
            this.btnNewCreate.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnNewCreate.IconColor = System.Drawing.Color.White;
            this.btnNewCreate.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewCreate.IconSize = 24;
            this.btnNewCreate.Location = new System.Drawing.Point(204, 100);
            this.btnNewCreate.Name = "btnNewCreate";
            this.btnNewCreate.Size = new System.Drawing.Size(90, 35);
            this.btnNewCreate.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnNewCreate.TabIndex = 2154;
            this.btnNewCreate.Text = "Add";
            this.btnNewCreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNewCreate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNewCreate.UseVisualStyleBackColor = false;
            this.btnNewCreate.Click += new System.EventHandler(this.btnNewCreate_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormVision_NewPanel
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "New Panel";
            this.ClientSize = new System.Drawing.Size(400, 180);
            this.Controls.Add(this.tbNewPanel);
            this.Controls.Add(this.label1);
            this.Name = "FormVision_NewPanel";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "New Panel";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.tbNewPanel, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private RJCodeUI_M1.RJControls.RJTextBox tbNewPanel;
        private System.Windows.Forms.Label label1;
        private RJCodeUI_M1.RJControls.RJButton btnNewCancel;
        private RJCodeUI_M1.RJControls.RJButton btnNewCreate;
        private System.Windows.Forms.Timer timer1;
    }
}
