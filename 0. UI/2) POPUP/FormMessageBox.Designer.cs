namespace OpenVisionLab
{
    partial class FormMessageBox
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
            this.lbMessage = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnBackground = new System.Windows.Forms.Panel();
            this.lbHeader = new System.Windows.Forms.Label();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.pbIcon = new FontAwesome.Sharp.IconPictureBox();
            this.pnBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMessage
            // 
            this.lbMessage.BackColor = System.Drawing.Color.Transparent;
            this.lbMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lbMessage.ForeColor = System.Drawing.Color.Black;
            this.lbMessage.Location = new System.Drawing.Point(143, 53);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(833, 148);
            this.lbMessage.TabIndex = 20;
            this.lbMessage.Text = "Do you want to the Exit?";
            this.lbMessage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.form_MouseDown);
            this.lbMessage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.form_MouseMove);
            this.lbMessage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.form_MouseUp);
            // 
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(808, 204);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(168, 35);
            this.btnCancel.TabIndex = 1083;
            this.btnCancel.Text = "No";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            //
            this.btnOK.Location = new System.Drawing.Point(634, 204);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(168, 35);
            this.btnOK.TabIndex = 1084;
            this.btnOK.Text = "Yes";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnBackground
            // 
            this.pnBackground.Controls.Add(this.pbIcon);
            this.pnBackground.Controls.Add(this.lbHeader);
            this.pnBackground.Controls.Add(this.lbMessage);
            this.pnBackground.Controls.Add(this.btnOK);
            this.pnBackground.Controls.Add(this.btnCancel);
            this.pnBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnBackground.Location = new System.Drawing.Point(0, 0);
            this.pnBackground.Name = "pnBackground";
            this.pnBackground.Size = new System.Drawing.Size(987, 252);
            this.pnBackground.TabIndex = 1085;
            this.pnBackground.MouseDown += new System.Windows.Forms.MouseEventHandler(this.form_MouseDown);
            this.pnBackground.MouseMove += new System.Windows.Forms.MouseEventHandler(this.form_MouseMove);
            this.pnBackground.MouseUp += new System.Windows.Forms.MouseEventHandler(this.form_MouseUp);
            // 
            // lbHeader
            // 
            this.lbHeader.BackColor = System.Drawing.Color.Transparent;
            this.lbHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHeader.ForeColor = System.Drawing.Color.Black;
            this.lbHeader.Location = new System.Drawing.Point(22, 11);
            this.lbHeader.Name = "lbHeader";
            this.lbHeader.Size = new System.Drawing.Size(954, 38);
            this.lbHeader.TabIndex = 1085;
            this.lbHeader.Text = "EXIT";
            this.lbHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.form_MouseDown);
            this.lbHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.form_MouseMove);
            this.lbHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.form_MouseUp);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // pbIcon
            // 
            this.pbIcon.BackColor = System.Drawing.Color.White;
            this.pbIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(169)))), ((int)(((byte)(0)))));
            this.pbIcon.IconChar = FontAwesome.Sharp.IconChar.CommentDots;
            this.pbIcon.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(169)))), ((int)(((byte)(0)))));
            this.pbIcon.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.pbIcon.IconSize = 130;
            this.pbIcon.Location = new System.Drawing.Point(10, 64);
            this.pbIcon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(130, 130);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbIcon.TabIndex = 1954;
            this.pbIcon.TabStop = false;
            // 
            // FormMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(987, 252);
            this.ControlBox = false;
            this.Controls.Add(this.pnBackground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EXIT";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMessageBox_KeyDown);
            this.pnBackground.ResumeLayout(false);
            this.pnBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pnBackground;
        private System.Windows.Forms.Label lbHeader;
        private System.Windows.Forms.Timer timerRefresh;
        private FontAwesome.Sharp.IconPictureBox pbIcon;
    }
}
