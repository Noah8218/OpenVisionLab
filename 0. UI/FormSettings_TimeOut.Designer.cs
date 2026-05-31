namespace IntelligentFactory
{
    partial class FormSetting_TimeOut
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timerView = new System.Windows.Forms.Timer(this.components);
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.btnCancel = new MetroFramework.Controls.MetroTile();
            this.btnSave = new MetroFramework.Controls.MetroTile();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.dgvTimeOut = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.tbTimeOut = new MetroFramework.Controls.MetroTextBox();
            this.btnActionContinue = new System.Windows.Forms.Label();
            this.btnActionAlarm = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnApply = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimeOut)).BeginInit();
            this.SuspendLayout();
            // 
            // timerView
            // 
            this.timerView.Enabled = true;
            this.timerView.Interval = 1000;
            this.timerView.Tick += new System.EventHandler(this.timerView_Tick);
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Lime;
            // 
            // btnCancel
            // 
            this.btnCancel.ActiveControl = null;
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.Location = new System.Drawing.Point(512, 559);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(196, 58);
            this.btnCancel.Style = MetroFramework.MetroColorStyle.Red;
            this.btnCancel.TabIndex = 1750;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.TileImage = global::IntelligentFactory.Properties.Resources.delete_50;
            this.btnCancel.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnCancel.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnCancel.UseSelectable = true;
            this.btnCancel.UseTileImage = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.ActiveControl = null;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.Location = new System.Drawing.Point(316, 559);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(195, 58);
            this.btnSave.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSave.TabIndex = 1749;
            this.btnSave.Text = "SAVE";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.TileImage = global::IntelligentFactory.Properties.Resources.save_50;
            this.btnSave.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnSave.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnSave.UseSelectable = true;
            this.btnSave.UseTileImage = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // metroLabel1
            // 
            this.metroLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.ForeColor = System.Drawing.Color.White;
            this.metroLabel1.Location = new System.Drawing.Point(23, 60);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(674, 35);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel1.TabIndex = 1658;
            this.metroLabel1.Text = "LIST";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
            // 
            // dgvTimeOut
            // 
            this.dgvTimeOut.AllowUserToAddRows = false;
            this.dgvTimeOut.AllowUserToDeleteRows = false;
            this.dgvTimeOut.AllowUserToResizeColumns = false;
            this.dgvTimeOut.AllowUserToResizeRows = false;
            this.dgvTimeOut.BackgroundColor = System.Drawing.Color.White;
            this.dgvTimeOut.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 12F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTimeOut.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvTimeOut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimeOut.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTimeOut.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvTimeOut.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvTimeOut.EnableHeadersVisualStyles = false;
            this.dgvTimeOut.GridColor = System.Drawing.Color.Black;
            this.dgvTimeOut.Location = new System.Drawing.Point(23, 168);
            this.dgvTimeOut.MultiSelect = false;
            this.dgvTimeOut.Name = "dgvTimeOut";
            this.dgvTimeOut.ReadOnly = true;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 12F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTimeOut.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvTimeOut.RowHeadersVisible = false;
            this.dgvTimeOut.RowHeadersWidth = 62;
            this.dgvTimeOut.RowTemplate.Height = 30;
            this.dgvTimeOut.RowTemplate.ReadOnly = true;
            this.dgvTimeOut.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTimeOut.Size = new System.Drawing.Size(674, 389);
            this.dgvTimeOut.TabIndex = 1751;
            this.dgvTimeOut.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTimeOut_CellClick);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(454, 96);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 35);
            this.label9.TabIndex = 1754;
            this.label9.Text = "ms";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbName
            // 
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lbName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbName.Location = new System.Drawing.Point(23, 96);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(336, 35);
            this.lbName.TabIndex = 1752;
            this.lbName.Text = "-";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbTimeOut
            // 
            // 
            // 
            // 
            this.tbTimeOut.CustomButton.Image = null;
            this.tbTimeOut.CustomButton.Location = new System.Drawing.Point(60, 1);
            this.tbTimeOut.CustomButton.Name = "";
            this.tbTimeOut.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbTimeOut.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbTimeOut.CustomButton.TabIndex = 1;
            this.tbTimeOut.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbTimeOut.CustomButton.UseSelectable = true;
            this.tbTimeOut.CustomButton.Visible = false;
            this.tbTimeOut.DisplayIcon = true;
            this.tbTimeOut.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbTimeOut.Lines = new string[] {
        "00000"};
            this.tbTimeOut.Location = new System.Drawing.Point(360, 96);
            this.tbTimeOut.MaxLength = 32767;
            this.tbTimeOut.Name = "tbTimeOut";
            this.tbTimeOut.PasswordChar = '\0';
            this.tbTimeOut.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbTimeOut.SelectedText = "";
            this.tbTimeOut.SelectionLength = 0;
            this.tbTimeOut.SelectionStart = 0;
            this.tbTimeOut.ShortcutsEnabled = true;
            this.tbTimeOut.Size = new System.Drawing.Size(94, 35);
            this.tbTimeOut.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbTimeOut.TabIndex = 1753;
            this.tbTimeOut.Text = "00000";
            this.tbTimeOut.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbTimeOut.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbTimeOut.UseSelectable = true;
            this.tbTimeOut.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbTimeOut.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // btnActionContinue
            // 
            this.btnActionContinue.BackColor = System.Drawing.Color.DimGray;
            this.btnActionContinue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btnActionContinue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActionContinue.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActionContinue.ForeColor = System.Drawing.Color.White;
            this.btnActionContinue.Location = new System.Drawing.Point(511, 96);
            this.btnActionContinue.Name = "btnActionContinue";
            this.btnActionContinue.Size = new System.Drawing.Size(93, 35);
            this.btnActionContinue.TabIndex = 1755;
            this.btnActionContinue.Text = "CONTINUE";
            this.btnActionContinue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnActionContinue.Click += new System.EventHandler(this.btnActionContinue_Click);
            // 
            // btnActionAlarm
            // 
            this.btnActionAlarm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.btnActionAlarm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btnActionAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActionAlarm.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActionAlarm.ForeColor = System.Drawing.Color.White;
            this.btnActionAlarm.Location = new System.Drawing.Point(605, 96);
            this.btnActionAlarm.Name = "btnActionAlarm";
            this.btnActionAlarm.Size = new System.Drawing.Size(92, 35);
            this.btnActionAlarm.TabIndex = 1756;
            this.btnActionAlarm.Text = "ALARM";
            this.btnActionAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnActionAlarm.Click += new System.EventHandler(this.btnActionAlarm_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "NAME";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 335;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "TIMEOUT (ms)";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 150;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn3.HeaderText = "ACTION";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 150;
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.btnApply.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(360, 132);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(337, 35);
            this.btnApply.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnApply.TabIndex = 1757;
            this.btnApply.Text = "APPLY";
            this.btnApply.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnApply.UseCustomBackColor = true;
            this.btnApply.UseCustomForeColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // FormSetting_TimeOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 640);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnActionAlarm);
            this.Controls.Add(this.btnActionContinue);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.tbTimeOut);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.dgvTimeOut);
            this.Name = "FormSetting_TimeOut";
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "TIME OUT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSetting_TimeOut_FormClosing);
            this.Load += new System.EventHandler(this.FormSetting_TimeOut_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimeOut)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerView;
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroTile btnCancel;
        private MetroFramework.Controls.MetroTile btnSave;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.DataGridView dgvTimeOut;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbName;
        private MetroFramework.Controls.MetroTextBox tbTimeOut;
        private System.Windows.Forms.Label btnActionAlarm;
        private System.Windows.Forms.Label btnActionContinue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private MetroFramework.Controls.MetroLabel btnApply;
    }
}