namespace RJCodeUI_M1.RJForms.Private
{
    partial class RJMessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RJMessageForm));
            this.pnlTittleBar = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.pnlIcon = new System.Windows.Forms.Panel();
            this.pbIcon = new FontAwesome.Sharp.IconPictureBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.pnlMessageText = new System.Windows.Forms.Panel();
            this.lblMessageText = new System.Windows.Forms.Label();
            this.rjDragControl1 = new RJCodeUI_M1.RJControls.RJDragControl(this.components);
            this.pnlTittleBar.SuspendLayout();
            this.pnlIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.pnlMessageText.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTittleBar
            // 
            this.pnlTittleBar.BackColor = System.Drawing.Color.SeaGreen;
            this.pnlTittleBar.Controls.Add(this.btnExit);
            this.pnlTittleBar.Controls.Add(this.lblCaption);
            this.pnlTittleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTittleBar.Location = new System.Drawing.Point(1, 1);
            this.pnlTittleBar.Name = "pnlTittleBar";
            this.pnlTittleBar.Size = new System.Drawing.Size(403, 40);
            this.pnlTittleBar.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.Location = new System.Drawing.Point(363, 0);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(40, 40);
            this.btnExit.TabIndex = 4;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaption.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblCaption.Location = new System.Drawing.Point(13, 11);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(56, 17);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "Caption";
            // 
            // pnlIcon
            // 
            this.pnlIcon.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlIcon.Controls.Add(this.pbIcon);
            this.pnlIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlIcon.Location = new System.Drawing.Point(1, 41);
            this.pnlIcon.Name = "pnlIcon";
            this.pnlIcon.Size = new System.Drawing.Size(50, 63);
            this.pnlIcon.TabIndex = 17;
            // 
            // pbIcon
            // 
            this.pbIcon.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pbIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(169)))), ((int)(((byte)(0)))));
            this.pbIcon.IconChar = FontAwesome.Sharp.IconChar.CommentDots;
            this.pbIcon.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(169)))), ((int)(((byte)(0)))));
            this.pbIcon.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.pbIcon.IconSize = 40;
            this.pbIcon.Location = new System.Drawing.Point(7, 20);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(40, 40);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbIcon.TabIndex = 5;
            this.pbIcon.TabStop = false;
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(1, 104);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(403, 60);
            this.pnlButtons.TabIndex = 4;
            // 
            // pnlMessageText
            // 
            this.pnlMessageText.AutoScroll = true;
            this.pnlMessageText.BackColor = System.Drawing.SystemColors.Control;
            this.pnlMessageText.Controls.Add(this.lblMessageText);
            this.pnlMessageText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMessageText.Location = new System.Drawing.Point(51, 41);
            this.pnlMessageText.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMessageText.Name = "pnlMessageText";
            this.pnlMessageText.Size = new System.Drawing.Size(353, 63);
            this.pnlMessageText.TabIndex = 16;
            // 
            // lblMessageText
            // 
            this.lblMessageText.AutoSize = true;
            this.lblMessageText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessageText.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblMessageText.Location = new System.Drawing.Point(0, 0);
            this.lblMessageText.MaximumSize = new System.Drawing.Size(345, 0);
            this.lblMessageText.Name = "lblMessageText";
            this.lblMessageText.Padding = new System.Windows.Forms.Padding(5, 22, 0, 22);
            this.lblMessageText.Size = new System.Drawing.Size(54, 61);
            this.lblMessageText.TabIndex = 1;
            this.lblMessageText.Text = "lblText";
            // 
            // rjDragControl1
            // 
            this.rjDragControl1.DragControl = this.pnlTittleBar;
            // 
            // RJMessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SeaGreen;
            this.ClientSize = new System.Drawing.Size(405, 165);
            this.Controls.Add(this.pnlMessageText);
            this.Controls.Add(this.pnlIcon);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlTittleBar);
            this.MaximumSize = new System.Drawing.Size(800, 650);
            this.MinimumSize = new System.Drawing.Size(405, 165);
            this.Name = "RJMessageForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RJMessageBox";
            this.Load += new System.EventHandler(this.RJMessageForm_Load);
            this.pnlTittleBar.ResumeLayout(false);
            this.pnlTittleBar.PerformLayout();
            this.pnlIcon.ResumeLayout(false);
            this.pnlIcon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.pnlMessageText.ResumeLayout(false);
            this.pnlMessageText.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlIcon;
        private System.Windows.Forms.Panel pnlTittleBar;
        private System.Windows.Forms.Panel pnlButtons;
        private FontAwesome.Sharp.IconPictureBox pbIcon;
        private System.Windows.Forms.Label lblCaption;
        public System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel pnlMessageText;
        private System.Windows.Forms.Label lblMessageText;
        private RJControls.RJDragControl rjDragControl1;
    }
}