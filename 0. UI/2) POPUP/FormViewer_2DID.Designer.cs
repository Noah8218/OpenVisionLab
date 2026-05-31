namespace IntelligentFactory
{
    partial class FormViewer_2DID
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timerView = new System.Windows.Forms.Timer(this.components);
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.dgvMetalTray = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvTopCover = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCameraOperator = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.btnSaveZigElevatorBufferCount = new MetroFramework.Controls.MetroTile();
            this.tbZigElevatorBufferCount = new MetroFramework.Controls.MetroTextBox();
            this.label39 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMetalTray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopCover)).BeginInit();
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
            // dgvMetalTray
            // 
            this.dgvMetalTray.AllowUserToAddRows = false;
            this.dgvMetalTray.AllowUserToDeleteRows = false;
            this.dgvMetalTray.AllowUserToResizeColumns = false;
            this.dgvMetalTray.AllowUserToResizeRows = false;
            this.dgvMetalTray.BackgroundColor = System.Drawing.Color.White;
            this.dgvMetalTray.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 12F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMetalTray.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvMetalTray.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMetalTray.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.Column2,
            this.Column4});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMetalTray.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvMetalTray.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvMetalTray.EnableHeadersVisualStyles = false;
            this.dgvMetalTray.GridColor = System.Drawing.Color.Black;
            this.dgvMetalTray.Location = new System.Drawing.Point(23, 126);
            this.dgvMetalTray.MultiSelect = false;
            this.dgvMetalTray.Name = "dgvMetalTray";
            this.dgvMetalTray.ReadOnly = true;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 12F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMetalTray.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvMetalTray.RowHeadersVisible = false;
            this.dgvMetalTray.RowHeadersWidth = 62;
            this.dgvMetalTray.RowTemplate.Height = 30;
            this.dgvMetalTray.RowTemplate.ReadOnly = true;
            this.dgvMetalTray.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvMetalTray.Size = new System.Drawing.Size(450, 831);
            this.dgvMetalTray.TabIndex = 103;
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "NO";
            this.ColumnName.MinimumWidth = 8;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.Width = 75;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "BCR";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 200;
            // 
            // Column4
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle6;
            this.Column4.HeaderText = "STATUS";
            this.Column4.MinimumWidth = 8;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 150;
            // 
            // dgvTopCover
            // 
            this.dgvTopCover.AllowUserToAddRows = false;
            this.dgvTopCover.AllowUserToDeleteRows = false;
            this.dgvTopCover.AllowUserToResizeColumns = false;
            this.dgvTopCover.AllowUserToResizeRows = false;
            this.dgvTopCover.BackgroundColor = System.Drawing.Color.White;
            this.dgvTopCover.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 12F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTopCover.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTopCover.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTopCover.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTopCover.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvTopCover.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvTopCover.EnableHeadersVisualStyles = false;
            this.dgvTopCover.GridColor = System.Drawing.Color.Black;
            this.dgvTopCover.Location = new System.Drawing.Point(472, 126);
            this.dgvTopCover.MultiSelect = false;
            this.dgvTopCover.Name = "dgvTopCover";
            this.dgvTopCover.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTopCover.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvTopCover.RowHeadersVisible = false;
            this.dgvTopCover.RowHeadersWidth = 62;
            this.dgvTopCover.RowTemplate.Height = 30;
            this.dgvTopCover.RowTemplate.ReadOnly = true;
            this.dgvTopCover.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTopCover.Size = new System.Drawing.Size(451, 831);
            this.dgvTopCover.TabIndex = 104;
            this.dgvTopCover.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "NO";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 75;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "BCR";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 200;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn3.HeaderText = "STATUS";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 150;
            // 
            // btnCameraOperator
            // 
            this.btnCameraOperator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.btnCameraOperator.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.btnCameraOperator.ForeColor = System.Drawing.Color.White;
            this.btnCameraOperator.Location = new System.Drawing.Point(23, 55);
            this.btnCameraOperator.Name = "btnCameraOperator";
            this.btnCameraOperator.Size = new System.Drawing.Size(449, 35);
            this.btnCameraOperator.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnCameraOperator.TabIndex = 1186;
            this.btnCameraOperator.Text = "METAL TRAY";
            this.btnCameraOperator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnCameraOperator.UseCustomBackColor = true;
            this.btnCameraOperator.UseCustomForeColor = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.ForeColor = System.Drawing.Color.White;
            this.metroLabel1.Location = new System.Drawing.Point(473, 55);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(449, 35);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel1.TabIndex = 1187;
            this.metroLabel1.Text = "TOP COVER";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
            // 
            // btnSaveZigElevatorBufferCount
            // 
            this.btnSaveZigElevatorBufferCount.ActiveControl = null;
            this.btnSaveZigElevatorBufferCount.BackColor = System.Drawing.Color.Transparent;
            this.btnSaveZigElevatorBufferCount.Location = new System.Drawing.Point(813, 91);
            this.btnSaveZigElevatorBufferCount.Name = "btnSaveZigElevatorBufferCount";
            this.btnSaveZigElevatorBufferCount.Size = new System.Drawing.Size(109, 35);
            this.btnSaveZigElevatorBufferCount.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSaveZigElevatorBufferCount.TabIndex = 1499;
            this.btnSaveZigElevatorBufferCount.Text = "SAVE";
            this.btnSaveZigElevatorBufferCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSaveZigElevatorBufferCount.TileImage = global::IntelligentFactory.Properties.Resources.save_50;
            this.btnSaveZigElevatorBufferCount.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveZigElevatorBufferCount.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.btnSaveZigElevatorBufferCount.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnSaveZigElevatorBufferCount.UseSelectable = true;
            this.btnSaveZigElevatorBufferCount.UseTileImage = true;
            this.btnSaveZigElevatorBufferCount.Click += new System.EventHandler(this.btnSaveZigElevatorBufferCount_Click);
            // 
            // tbZigElevatorBufferCount
            // 
            // 
            // 
            // 
            this.tbZigElevatorBufferCount.CustomButton.Image = null;
            this.tbZigElevatorBufferCount.CustomButton.Location = new System.Drawing.Point(146, 1);
            this.tbZigElevatorBufferCount.CustomButton.Name = "";
            this.tbZigElevatorBufferCount.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbZigElevatorBufferCount.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbZigElevatorBufferCount.CustomButton.TabIndex = 1;
            this.tbZigElevatorBufferCount.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbZigElevatorBufferCount.CustomButton.UseSelectable = true;
            this.tbZigElevatorBufferCount.CustomButton.Visible = false;
            this.tbZigElevatorBufferCount.DisplayIcon = true;
            this.tbZigElevatorBufferCount.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbZigElevatorBufferCount.Lines = new string[] {
        "00000000"};
            this.tbZigElevatorBufferCount.Location = new System.Drawing.Point(632, 91);
            this.tbZigElevatorBufferCount.MaxLength = 32767;
            this.tbZigElevatorBufferCount.Name = "tbZigElevatorBufferCount";
            this.tbZigElevatorBufferCount.PasswordChar = '\0';
            this.tbZigElevatorBufferCount.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbZigElevatorBufferCount.SelectedText = "";
            this.tbZigElevatorBufferCount.SelectionLength = 0;
            this.tbZigElevatorBufferCount.SelectionStart = 0;
            this.tbZigElevatorBufferCount.ShortcutsEnabled = true;
            this.tbZigElevatorBufferCount.Size = new System.Drawing.Size(180, 35);
            this.tbZigElevatorBufferCount.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbZigElevatorBufferCount.TabIndex = 1498;
            this.tbZigElevatorBufferCount.Text = "00000000";
            this.tbZigElevatorBufferCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbZigElevatorBufferCount.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbZigElevatorBufferCount.UseSelectable = true;
            this.tbZigElevatorBufferCount.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbZigElevatorBufferCount.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // label39
            // 
            this.label39.BackColor = System.Drawing.Color.Transparent;
            this.label39.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label39.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label39.Location = new System.Drawing.Point(473, 91);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(158, 35);
            this.label39.TabIndex = 1497;
            this.label39.Text = "BUFFER COUNT";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormViewer_2DID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 988);
            this.Controls.Add(this.btnSaveZigElevatorBufferCount);
            this.Controls.Add(this.tbZigElevatorBufferCount);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.btnCameraOperator);
            this.Controls.Add(this.dgvTopCover);
            this.Controls.Add(this.dgvMetalTray);
            this.Name = "FormViewer_2DID";
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "2D ID";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormViewer_2DID_FormClosing);
            this.Load += new System.EventHandler(this.FormViewer_2DID_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMetalTray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopCover)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerView;
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private System.Windows.Forms.DataGridView dgvTopCover;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridView dgvMetalTray;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel btnCameraOperator;
        private MetroFramework.Controls.MetroTile btnSaveZigElevatorBufferCount;
        private MetroFramework.Controls.MetroTextBox tbZigElevatorBufferCount;
        private System.Windows.Forms.Label label39;
    }
}