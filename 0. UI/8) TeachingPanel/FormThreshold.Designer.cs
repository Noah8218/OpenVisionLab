using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormThreshold
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormThreshold));
			timePixelData = new System.Windows.Forms.Timer(components);
			panel1 = new System.Windows.Forms.Panel();
			rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
			label4 = new System.Windows.Forms.Label();
			tbWeight = new RJCodeUI_M1.RJControls.RJTextBox();
			label5 = new System.Windows.Forms.Label();
			tbBlockSize = new RJCodeUI_M1.RJControls.RJTextBox();
			trbAdaptiveThreshold = new RJCodeUI_M1.RJControls.RJTrackBar();
			rjLabel2 = new RJCodeUI_M1.RJControls.RJLabel();
			cbAdaptiveType = new RJCodeUI_M1.RJControls.RJComboBox();
			rjLabel6 = new RJCodeUI_M1.RJControls.RJLabel();
			cbAdaptiveThresholdMenu = new RJCodeUI_M1.RJControls.RJComboBox();
			rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
			trbDoubleThresholdMin = new RJCodeUI_M1.RJControls.RJTrackBar();
			rjLabel5 = new RJCodeUI_M1.RJControls.RJLabel();
			trbDoubleThresholdMax = new RJCodeUI_M1.RJControls.RJTrackBar();
			rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
			trbThreshold = new RJCodeUI_M1.RJControls.RJTrackBar();
			cbThresholdMenu = new RJCodeUI_M1.RJControls.RJComboBox();
			rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
			panel1.SuspendLayout();
			rjPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trbAdaptiveThreshold).BeginInit();
			((System.ComponentModel.ISupportInitialize)trbDoubleThresholdMin).BeginInit();
			((System.ComponentModel.ISupportInitialize)trbDoubleThresholdMax).BeginInit();
			((System.ComponentModel.ISupportInitialize)trbThreshold).BeginInit();
			SuspendLayout();
			// 
			// timePixelData
			// 
			timePixelData.Enabled = true;
			timePixelData.Interval = 10;
			// 
			// panel1
			// 
			panel1.AutoScroll = true;
			panel1.BackColor = System.Drawing.Color.FromArgb(240, 245, 249);
			panel1.Controls.Add(rjPanel1);
			panel1.Controls.Add(trbAdaptiveThreshold);
			panel1.Controls.Add(rjLabel2);
			panel1.Controls.Add(cbAdaptiveType);
			panel1.Controls.Add(rjLabel6);
			panel1.Controls.Add(cbAdaptiveThresholdMenu);
			panel1.Controls.Add(rjLabel3);
			panel1.Controls.Add(trbDoubleThresholdMin);
			panel1.Controls.Add(rjLabel5);
			panel1.Controls.Add(trbDoubleThresholdMax);
			panel1.Controls.Add(rjLabel1);
			panel1.Controls.Add(trbThreshold);
			panel1.Controls.Add(cbThresholdMenu);
			panel1.Controls.Add(rjLabel4);
			panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			panel1.Location = new System.Drawing.Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(612, 456);
			panel1.TabIndex = 1952;
			// 
			// rjPanel1
			// 
			rjPanel1.BackColor = System.Drawing.Color.FromArgb(250, 252, 253);
			rjPanel1.BorderRadius = 5;
			rjPanel1.Controls.Add(label4);
			rjPanel1.Controls.Add(tbWeight);
			rjPanel1.Controls.Add(label5);
			rjPanel1.Controls.Add(tbBlockSize);
			rjPanel1.Customizable = false;
			rjPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			rjPanel1.Location = new System.Drawing.Point(0, 385);
			rjPanel1.Name = "rjPanel1";
			rjPanel1.Size = new System.Drawing.Size(595, 123);
			rjPanel1.TabIndex = 2195;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Font = new System.Drawing.Font("Verdana", 9.5F);
			label4.ForeColor = System.Drawing.Color.Black;
			label4.Location = new System.Drawing.Point(12, 6);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(73, 16);
			label4.TabIndex = 2136;
			label4.Text = "Block Size";
			// 
			// tbWeight
			// 
			tbWeight._Customizable = true;
			tbWeight.BackColor = System.Drawing.Color.WhiteSmoke;
			tbWeight.BorderColor = System.Drawing.Color.FromArgb(132, 129, 132);
			tbWeight.BorderFocusColor = System.Drawing.Color.FromArgb(108, 120, 218);
			tbWeight.BorderRadius = 0;
			tbWeight.BorderSize = 1;
			tbWeight.Font = new System.Drawing.Font("Verdana", 9.5F);
			tbWeight.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132);
			tbWeight.Location = new System.Drawing.Point(11, 78);
			tbWeight.MultiLine = false;
			tbWeight.Name = "tbWeight";
			tbWeight.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
			tbWeight.PasswordChar = false;
			tbWeight.PlaceHolderColor = System.Drawing.Color.DarkGray;
			tbWeight.PlaceHolderText = "5";
			tbWeight.ScrollBars = System.Windows.Forms.ScrollBars.None;
			tbWeight.Size = new System.Drawing.Size(184, 31);
			tbWeight.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
			tbWeight.TabIndex = 2141;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Font = new System.Drawing.Font("Verdana", 9.5F);
			label5.ForeColor = System.Drawing.Color.Black;
			label5.Location = new System.Drawing.Point(12, 59);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(52, 16);
			label5.TabIndex = 2137;
			label5.Text = "Weight";
			// 
			// tbBlockSize
			// 
			tbBlockSize._Customizable = true;
			tbBlockSize.BackColor = System.Drawing.Color.WhiteSmoke;
			tbBlockSize.BorderColor = System.Drawing.Color.FromArgb(132, 129, 132);
			tbBlockSize.BorderFocusColor = System.Drawing.Color.FromArgb(108, 120, 218);
			tbBlockSize.BorderRadius = 0;
			tbBlockSize.BorderSize = 1;
			tbBlockSize.Font = new System.Drawing.Font("Verdana", 9.5F);
			tbBlockSize.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132);
			tbBlockSize.Location = new System.Drawing.Point(9, 27);
			tbBlockSize.MultiLine = false;
			tbBlockSize.Name = "tbBlockSize";
			tbBlockSize.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
			tbBlockSize.PasswordChar = false;
			tbBlockSize.PlaceHolderColor = System.Drawing.Color.DarkGray;
			tbBlockSize.PlaceHolderText = "25";
			tbBlockSize.ScrollBars = System.Windows.Forms.ScrollBars.None;
			tbBlockSize.Size = new System.Drawing.Size(184, 31);
			tbBlockSize.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
			tbBlockSize.TabIndex = 2140;
			// 
			// trbAdaptiveThreshold
			// 
			trbAdaptiveThreshold.AutoSize = false;
			trbAdaptiveThreshold.ChannelColor = System.Drawing.Color.LightGray;
			trbAdaptiveThreshold.Customizable = true;
			trbAdaptiveThreshold.Dock = System.Windows.Forms.DockStyle.Top;
			trbAdaptiveThreshold.LargeChange = 1;
			trbAdaptiveThreshold.Location = new System.Drawing.Point(0, 339);
			trbAdaptiveThreshold.Maximum = 255;
			trbAdaptiveThreshold.Minimum = 1;
			trbAdaptiveThreshold.Name = "trbAdaptiveThreshold";
			trbAdaptiveThreshold.ShowValue = true;
			trbAdaptiveThreshold.Size = new System.Drawing.Size(595, 46);
			trbAdaptiveThreshold.SliderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			trbAdaptiveThreshold.TabIndex = 2194;
			trbAdaptiveThreshold.TextColor = System.Drawing.Color.FromArgb(162, 160, 162);
			trbAdaptiveThreshold.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			trbAdaptiveThreshold.Value = 1;
			trbAdaptiveThreshold.Scroll += trbAdaptiveThreshold_Scroll;
			// 
			// rjLabel2
			// 
			rjLabel2.AutoSize = true;
			rjLabel2.Dock = System.Windows.Forms.DockStyle.Top;
			rjLabel2.Font = new System.Drawing.Font("Verdana", 9.5F);
			rjLabel2.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132);
			rjLabel2.LinkLabel = false;
			rjLabel2.Location = new System.Drawing.Point(0, 323);
			rjLabel2.Name = "rjLabel2";
			rjLabel2.Size = new System.Drawing.Size(134, 16);
			rjLabel2.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel2.TabIndex = 2193;
			rjLabel2.Text = "Adaptive Threshold";
			// 
			// cbAdaptiveType
			// 
			cbAdaptiveType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbAdaptiveType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbAdaptiveType.BackColor = System.Drawing.Color.FromArgb(240, 245, 249);
			cbAdaptiveType.BorderColor = System.Drawing.Color.FromArgb(132, 129, 132);
			cbAdaptiveType.BorderRadius = 0;
			cbAdaptiveType.BorderSize = 2;
			cbAdaptiveType.Customizable = false;
			cbAdaptiveType.DataSource = null;
			cbAdaptiveType.Dock = System.Windows.Forms.DockStyle.Top;
			cbAdaptiveType.DropDownBackColor = System.Drawing.Color.FromArgb(250, 252, 253);
			cbAdaptiveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbAdaptiveType.DropDownTextColor = System.Drawing.Color.FromArgb(132, 129, 132);
			cbAdaptiveType.Font = new System.Drawing.Font("Verdana", 15F);
			cbAdaptiveType.ForeColor = System.Drawing.Color.DimGray;
			cbAdaptiveType.IconColor = System.Drawing.Color.FromArgb(90, 146, 246);
			cbAdaptiveType.Location = new System.Drawing.Point(0, 288);
			cbAdaptiveType.MinimumSize = new System.Drawing.Size(100, 30);
			cbAdaptiveType.Name = "cbAdaptiveType";
			cbAdaptiveType.Padding = new System.Windows.Forms.Padding(2);
			cbAdaptiveType.SelectedIndex = -1;
			cbAdaptiveType.Size = new System.Drawing.Size(595, 35);
			cbAdaptiveType.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbAdaptiveType.TabIndex = 2192;
			cbAdaptiveType.Texts = "";
			// 
			// rjLabel6
			// 
			rjLabel6.AutoSize = true;
			rjLabel6.Dock = System.Windows.Forms.DockStyle.Top;
			rjLabel6.Font = new System.Drawing.Font("Verdana", 9.5F);
			rjLabel6.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132);
			rjLabel6.LinkLabel = false;
			rjLabel6.Location = new System.Drawing.Point(0, 272);
			rjLabel6.Name = "rjLabel6";
			rjLabel6.Size = new System.Drawing.Size(165, 16);
			rjLabel6.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel6.TabIndex = 2191;
			rjLabel6.Text = "AdaptiveThreshold Type";
			// 
			// cbAdaptiveThresholdMenu
			// 
			cbAdaptiveThresholdMenu.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbAdaptiveThresholdMenu.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbAdaptiveThresholdMenu.BackColor = System.Drawing.Color.FromArgb(240, 245, 249);
			cbAdaptiveThresholdMenu.BorderColor = System.Drawing.Color.FromArgb(132, 129, 132);
			cbAdaptiveThresholdMenu.BorderRadius = 0;
			cbAdaptiveThresholdMenu.BorderSize = 2;
			cbAdaptiveThresholdMenu.Customizable = false;
			cbAdaptiveThresholdMenu.DataSource = null;
			cbAdaptiveThresholdMenu.Dock = System.Windows.Forms.DockStyle.Top;
			cbAdaptiveThresholdMenu.DropDownBackColor = System.Drawing.Color.FromArgb(250, 252, 253);
			cbAdaptiveThresholdMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbAdaptiveThresholdMenu.DropDownTextColor = System.Drawing.Color.FromArgb(132, 129, 132);
			cbAdaptiveThresholdMenu.Font = new System.Drawing.Font("Verdana", 15F);
			cbAdaptiveThresholdMenu.ForeColor = System.Drawing.Color.DimGray;
			cbAdaptiveThresholdMenu.IconColor = System.Drawing.Color.FromArgb(90, 146, 246);
			cbAdaptiveThresholdMenu.Location = new System.Drawing.Point(0, 237);
			cbAdaptiveThresholdMenu.MinimumSize = new System.Drawing.Size(100, 30);
			cbAdaptiveThresholdMenu.Name = "cbAdaptiveThresholdMenu";
			cbAdaptiveThresholdMenu.Padding = new System.Windows.Forms.Padding(2);
			cbAdaptiveThresholdMenu.SelectedIndex = -1;
			cbAdaptiveThresholdMenu.Size = new System.Drawing.Size(595, 35);
			cbAdaptiveThresholdMenu.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbAdaptiveThresholdMenu.TabIndex = 2190;
			cbAdaptiveThresholdMenu.Texts = "";
			// 
			// rjLabel3
			// 
			rjLabel3.AutoSize = true;
			rjLabel3.Dock = System.Windows.Forms.DockStyle.Top;
			rjLabel3.Font = new System.Drawing.Font("Verdana", 9.5F);
			rjLabel3.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132);
			rjLabel3.LinkLabel = false;
			rjLabel3.Location = new System.Drawing.Point(0, 221);
			rjLabel3.Name = "rjLabel3";
			rjLabel3.Size = new System.Drawing.Size(108, 16);
			rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel3.TabIndex = 2189;
			rjLabel3.Text = "Threshold Type";
			// 
			// trbDoubleThresholdMin
			// 
			trbDoubleThresholdMin.AutoSize = false;
			trbDoubleThresholdMin.ChannelColor = System.Drawing.Color.LightGray;
			trbDoubleThresholdMin.Customizable = true;
			trbDoubleThresholdMin.Dock = System.Windows.Forms.DockStyle.Top;
			trbDoubleThresholdMin.LargeChange = 1;
			trbDoubleThresholdMin.Location = new System.Drawing.Point(0, 175);
			trbDoubleThresholdMin.Maximum = 255;
			trbDoubleThresholdMin.Minimum = 1;
			trbDoubleThresholdMin.Name = "trbDoubleThresholdMin";
			trbDoubleThresholdMin.ShowValue = true;
			trbDoubleThresholdMin.Size = new System.Drawing.Size(595, 46);
			trbDoubleThresholdMin.SliderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			trbDoubleThresholdMin.TabIndex = 2186;
			trbDoubleThresholdMin.TextColor = System.Drawing.Color.FromArgb(162, 160, 162);
			trbDoubleThresholdMin.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			trbDoubleThresholdMin.Value = 1;
			trbDoubleThresholdMin.Scroll += trbDoubleThresholdMax_Scroll;
			// 
			// rjLabel5
			// 
			rjLabel5.AutoSize = true;
			rjLabel5.Dock = System.Windows.Forms.DockStyle.Top;
			rjLabel5.Font = new System.Drawing.Font("Verdana", 9.5F);
			rjLabel5.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132);
			rjLabel5.LinkLabel = false;
			rjLabel5.Location = new System.Drawing.Point(0, 159);
			rjLabel5.Name = "rjLabel5";
			rjLabel5.Size = new System.Drawing.Size(147, 16);
			rjLabel5.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel5.TabIndex = 2185;
			rjLabel5.Text = "Double Threshold Min";
			// 
			// trbDoubleThresholdMax
			// 
			trbDoubleThresholdMax.AutoSize = false;
			trbDoubleThresholdMax.ChannelColor = System.Drawing.Color.LightGray;
			trbDoubleThresholdMax.Customizable = true;
			trbDoubleThresholdMax.Dock = System.Windows.Forms.DockStyle.Top;
			trbDoubleThresholdMax.LargeChange = 1;
			trbDoubleThresholdMax.Location = new System.Drawing.Point(0, 113);
			trbDoubleThresholdMax.Maximum = 255;
			trbDoubleThresholdMax.Minimum = 1;
			trbDoubleThresholdMax.Name = "trbDoubleThresholdMax";
			trbDoubleThresholdMax.ShowValue = true;
			trbDoubleThresholdMax.Size = new System.Drawing.Size(595, 46);
			trbDoubleThresholdMax.SliderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			trbDoubleThresholdMax.TabIndex = 2184;
			trbDoubleThresholdMax.TextColor = System.Drawing.Color.FromArgb(162, 160, 162);
			trbDoubleThresholdMax.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			trbDoubleThresholdMax.Value = 1;
			trbDoubleThresholdMax.Scroll += trbDoubleThresholdMax_Scroll;
			// 
			// rjLabel1
			// 
			rjLabel1.AutoSize = true;
			rjLabel1.Dock = System.Windows.Forms.DockStyle.Top;
			rjLabel1.Font = new System.Drawing.Font("Verdana", 9.5F);
			rjLabel1.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132);
			rjLabel1.LinkLabel = false;
			rjLabel1.Location = new System.Drawing.Point(0, 97);
			rjLabel1.Name = "rjLabel1";
			rjLabel1.Size = new System.Drawing.Size(151, 16);
			rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel1.TabIndex = 2183;
			rjLabel1.Text = "Double Threshold Max";
			// 
			// trbThreshold
			// 
			trbThreshold.AutoSize = false;
			trbThreshold.ChannelColor = System.Drawing.Color.LightGray;
			trbThreshold.Customizable = true;
			trbThreshold.Dock = System.Windows.Forms.DockStyle.Top;
			trbThreshold.LargeChange = 1;
			trbThreshold.Location = new System.Drawing.Point(0, 51);
			trbThreshold.Maximum = 255;
			trbThreshold.Minimum = 1;
			trbThreshold.Name = "trbThreshold";
			trbThreshold.ShowValue = true;
			trbThreshold.Size = new System.Drawing.Size(595, 46);
			trbThreshold.SliderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			trbThreshold.TabIndex = 2182;
			trbThreshold.TextColor = System.Drawing.Color.FromArgb(162, 160, 162);
			trbThreshold.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			trbThreshold.Value = 1;
			trbThreshold.Scroll += trbThreshold_Scroll;
			// 
			// cbThresholdMenu
			// 
			cbThresholdMenu.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbThresholdMenu.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbThresholdMenu.BackColor = System.Drawing.Color.FromArgb(240, 245, 249);
			cbThresholdMenu.BorderColor = System.Drawing.Color.FromArgb(132, 129, 132);
			cbThresholdMenu.BorderRadius = 0;
			cbThresholdMenu.BorderSize = 2;
			cbThresholdMenu.Customizable = false;
			cbThresholdMenu.DataSource = null;
			cbThresholdMenu.Dock = System.Windows.Forms.DockStyle.Top;
			cbThresholdMenu.DropDownBackColor = System.Drawing.Color.FromArgb(250, 252, 253);
			cbThresholdMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbThresholdMenu.DropDownTextColor = System.Drawing.Color.FromArgb(132, 129, 132);
			cbThresholdMenu.Font = new System.Drawing.Font("Verdana", 15F);
			cbThresholdMenu.ForeColor = System.Drawing.Color.DimGray;
			cbThresholdMenu.IconColor = System.Drawing.Color.FromArgb(90, 146, 246);
			cbThresholdMenu.Location = new System.Drawing.Point(0, 16);
			cbThresholdMenu.MinimumSize = new System.Drawing.Size(100, 30);
			cbThresholdMenu.Name = "cbThresholdMenu";
			cbThresholdMenu.Padding = new System.Windows.Forms.Padding(2);
			cbThresholdMenu.SelectedIndex = -1;
			cbThresholdMenu.Size = new System.Drawing.Size(595, 35);
			cbThresholdMenu.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbThresholdMenu.TabIndex = 2181;
			cbThresholdMenu.Texts = "";
			// 
			// rjLabel4
			// 
			rjLabel4.AutoSize = true;
			rjLabel4.Dock = System.Windows.Forms.DockStyle.Top;
			rjLabel4.Font = new System.Drawing.Font("Verdana", 9.5F);
			rjLabel4.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132);
			rjLabel4.LinkLabel = false;
			rjLabel4.Location = new System.Drawing.Point(0, 0);
			rjLabel4.Name = "rjLabel4";
			rjLabel4.Size = new System.Drawing.Size(108, 16);
			rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel4.TabIndex = 2180;
			rjLabel4.Text = "Threshold Type";
			// 
			// FormThreshold
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			AutoScroll = true;
			ClientSize = new System.Drawing.Size(612, 456);
			Controls.Add(panel1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			Name = "FormThreshold";
			Text = "Threshold";
			Load += Form_Load;
			VisibleChanged += Form_VisibleChanged;
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			rjPanel1.ResumeLayout(false);
			rjPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trbAdaptiveThreshold).EndInit();
			((System.ComponentModel.ISupportInitialize)trbDoubleThresholdMin).EndInit();
			((System.ComponentModel.ISupportInitialize)trbDoubleThresholdMax).EndInit();
			((System.ComponentModel.ISupportInitialize)trbThreshold).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.Panel panel1;
        public RJCodeUI_M1.RJControls.RJTrackBar trbDoubleThresholdMin;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel5;
        public RJCodeUI_M1.RJControls.RJTrackBar trbDoubleThresholdMax;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        public RJCodeUI_M1.RJControls.RJTrackBar trbThreshold;
        private RJCodeUI_M1.RJControls.RJComboBox cbThresholdMenu;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJComboBox cbAdaptiveType;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel6;
        private RJCodeUI_M1.RJControls.RJComboBox cbAdaptiveThresholdMenu;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        public RJCodeUI_M1.RJControls.RJTrackBar trbAdaptiveThreshold;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel2;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private System.Windows.Forms.Label label4;
        private RJCodeUI_M1.RJControls.RJTextBox tbWeight;
        private System.Windows.Forms.Label label5;
        private RJCodeUI_M1.RJControls.RJTextBox tbBlockSize;
    }
}