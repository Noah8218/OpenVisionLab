using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLog));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mySqlCommand1 = new MySqlConnector.MySqlCommand();
            this.uiMillisecondTimer1 = new Sunny.UI.UIMillisecondTimer(this.components);
            this.cbLogItems = new Sunny.UI.UIComboBox();
            this.pnLog = new System.Windows.Forms.Panel();
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
            this.timer1.Enabled = true;
            // 
            // mySqlCommand1
            // 
            this.mySqlCommand1.CommandTimeout = 0;
            this.mySqlCommand1.Connection = null;
            this.mySqlCommand1.Transaction = null;
            this.mySqlCommand1.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // cbLogItems
            // 
            this.cbLogItems.DataSource = null;
            this.cbLogItems.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbLogItems.FillColor = System.Drawing.Color.White;
            this.cbLogItems.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.cbLogItems.Location = new System.Drawing.Point(0, 0);
            this.cbLogItems.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbLogItems.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbLogItems.Name = "cbLogItems";
            this.cbLogItems.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbLogItems.Size = new System.Drawing.Size(475, 29);
            this.cbLogItems.TabIndex = 2;
            this.cbLogItems.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbLogItems.Watermark = "";
            this.cbLogItems.SelectedIndexChanged += new System.EventHandler(this.cbLogItems_SelectedIndexChanged);
            // 
            // pnLog
            // 
            this.pnLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnLog.Location = new System.Drawing.Point(0, 29);
            this.pnLog.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.pnLog.Name = "pnLog";
            this.pnLog.Size = new System.Drawing.Size(475, 232);
            this.pnLog.TabIndex = 1952;
            // 
            // FormLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(475, 261);
            this.Controls.Add(this.pnLog);
            this.Controls.Add(this.cbLogItems);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormLog";
            this.Text = "Log";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.FormLayerDisplay_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private MySqlConnector.MySqlCommand mySqlCommand1;
        private Sunny.UI.UIMillisecondTimer uiMillisecondTimer1;
        private Sunny.UI.UIComboBox cbLogItems;
        private System.Windows.Forms.Panel pnLog;
    }
}