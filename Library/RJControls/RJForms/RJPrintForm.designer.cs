namespace RJCodeUI_M1.RJForms
{
    partial class RJPrintForm
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
            this.pnlPrintMenu = new System.Windows.Forms.Panel();
            this.btnCancel = new FontAwesome.Sharp.IconButton();
            this.btnLandscape = new FontAwesome.Sharp.IconButton();
            this.btnPortrait = new FontAwesome.Sharp.IconButton();
            this.btnPrint = new FontAwesome.Sharp.IconButton();
            this.pnlDocument = new System.Windows.Forms.Panel();
            this.pbContent = new System.Windows.Forms.PictureBox();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCompany = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.printDoc = new System.Drawing.Printing.PrintDocument();
            this.pnlPrintMenu.SuspendLayout();
            this.pnlDocument.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbContent)).BeginInit();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPrintMenu
            // 
            this.pnlPrintMenu.Controls.Add(this.btnCancel);
            this.pnlPrintMenu.Controls.Add(this.btnLandscape);
            this.pnlPrintMenu.Controls.Add(this.btnPortrait);
            this.pnlPrintMenu.Controls.Add(this.btnPrint);
            this.pnlPrintMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPrintMenu.Location = new System.Drawing.Point(1, 41);
            this.pnlPrintMenu.Name = "pnlPrintMenu";
            this.pnlPrintMenu.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pnlPrintMenu.Size = new System.Drawing.Size(958, 25);
            this.pnlPrintMenu.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnCancel.FlatAppearance.BorderSize= 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.IconChar = FontAwesome.Sharp.IconChar.TimesCircle;
            this.btnCancel.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(101)))), ((int)(((byte)(243)))));
            this.btnCancel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCancel.IconSize = 20;
            this.btnCancel.Location = new System.Drawing.Point(115, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(35, 25);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnLandscape
            // 
            this.btnLandscape.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLandscape.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnLandscape.FlatAppearance.BorderSize= 0;
            this.btnLandscape.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLandscape.IconChar = FontAwesome.Sharp.IconChar.File;
            this.btnLandscape.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(132)))), ((int)(((byte)(235)))));
            this.btnLandscape.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLandscape.IconSize = 20;
            this.btnLandscape.Location = new System.Drawing.Point(80, 0);
            this.btnLandscape.Name = "btnLandscape";
            this.btnLandscape.Rotation = -90D;
            this.btnLandscape.Size = new System.Drawing.Size(35, 25);
            this.btnLandscape.TabIndex = 0;
            this.btnLandscape.UseVisualStyleBackColor = true;
            this.btnLandscape.Click += new System.EventHandler(this.btnLandscape_Click);
            // 
            // btnPortrait
            // 
            this.btnPortrait.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPortrait.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnPortrait.FlatAppearance.BorderSize= 0;
            this.btnPortrait.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPortrait.IconChar = FontAwesome.Sharp.IconChar.File;
            this.btnPortrait.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(168)))), ((int)(((byte)(210)))));
            this.btnPortrait.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPortrait.IconSize = 20;
            this.btnPortrait.Location = new System.Drawing.Point(45, 0);
            this.btnPortrait.Name = "btnPortrait";
            this.btnPortrait.Size = new System.Drawing.Size(35, 25);
            this.btnPortrait.TabIndex = 1;
            this.btnPortrait.UseVisualStyleBackColor = true;
            this.btnPortrait.Click += new System.EventHandler(this.btnPortrait_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnPrint.FlatAppearance.BorderSize= 0;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.IconChar = FontAwesome.Sharp.IconChar.Print;
            this.btnPrint.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(85)))), ((int)(((byte)(230)))));
            this.btnPrint.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPrint.IconSize = 20;
            this.btnPrint.Location = new System.Drawing.Point(10, 0);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(35, 25);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // pnlDocument
            // 
            this.pnlDocument.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pnlDocument.BackColor = System.Drawing.Color.White;
            this.pnlDocument.Controls.Add(this.pbContent);
            this.pnlDocument.Controls.Add(this.pnlHeader);
            this.pnlDocument.Location = new System.Drawing.Point(70, 90);
            this.pnlDocument.Name = "pnlDocument";
            this.pnlDocument.Padding = new System.Windows.Forms.Padding(20);
            this.pnlDocument.Size = new System.Drawing.Size(794, 445);
            this.pnlDocument.TabIndex = 1;
            // 
            // pbContent
            // 
            this.pbContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbContent.Location = new System.Drawing.Point(20, 100);
            this.pbContent.Name = "pbContent";
            this.pbContent.Size = new System.Drawing.Size(754, 322);
            this.pbContent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbContent.TabIndex = 1;
            this.pbContent.TabStop = false;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblCompany);
            this.pnlHeader.Controls.Add(this.lblDate);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(20, 20);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(754, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(28, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(77, 20);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Doc Title";
            // 
            // lblCompany
            // 
            this.lblCompany.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCompany.AutoSize = true;
            this.lblCompany.ForeColor = System.Drawing.Color.White;
            this.lblCompany.Location = new System.Drawing.Point(624, 10);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.Size = new System.Drawing.Size(118, 16);
            this.lblCompany.TabIndex = 1;
            this.lblCompany.Text = "RJ Code Advance";
            // 
            // lblDate
            // 
            this.lblDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDate.ForeColor = System.Drawing.Color.White;
            this.lblDate.Location = new System.Drawing.Point(329, 30);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(415, 20);
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "Date";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // printDoc
            // 
            this.printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_PrintPage);
            // 
            // RJPrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.Caption = "Print";
            this.ClientSize = new System.Drawing.Size(960, 565);
            this.pnlClientArea.Controls.Add(this.pnlPrintMenu);
            this.pnlClientArea.Controls.Add(this.pnlDocument);
            this.FormIcon = FontAwesome.Sharp.IconChar.Parking;
            this.Name = "RJPrintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print";         
            this.pnlPrintMenu.ResumeLayout(false);
            this.pnlDocument.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbContent)).EndInit();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlDocument;
        private System.Windows.Forms.Panel pnlPrintMenu;
        private System.Windows.Forms.PictureBox pbContent;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblCompany;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblTitle;

        private FontAwesome.Sharp.IconButton btnPortrait;//Move child form to new window
        private FontAwesome.Sharp.IconButton btnLandscape;//Form screenshot
        private FontAwesome.Sharp.IconButton btnPrint;//print child form
        private FontAwesome.Sharp.IconButton btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Drawing.Printing.PrintDocument printDoc;
    }
}