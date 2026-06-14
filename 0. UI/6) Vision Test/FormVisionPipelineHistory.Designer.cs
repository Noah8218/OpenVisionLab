using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineHistory
    {
        private TableLayoutPanel rootLayout;
        private TableLayoutPanel centerLayout;
        private Panel footerPanel;
        private DataGridViewTextBoxColumn stepIndexColumn;
        private DataGridViewTextBoxColumn stepStatusColumn;
        private DataGridViewTextBoxColumn stepAcceptanceColumn;
        private DataGridViewTextBoxColumn stepNameColumn;
        private DataGridViewTextBoxColumn stepToolColumn;
        private DataGridViewTextBoxColumn stepElapsedColumn;
        private DataGridViewTextBoxColumn stepImageColumn;
        private DataGridViewTextBoxColumn stepOverlayColumn;
        private DataGridViewTextBoxColumn stepMetricColumn;
        private DataGridViewTextBoxColumn stepMessageColumn;
        private DataGridViewTextBoxColumn stepAcceptanceMessageColumn;
        private DataGridViewTextBoxColumn metricNameColumn;
        private DataGridViewTextBoxColumn metricValueColumn;

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            runList = new ListBox();
            centerLayout = new TableLayoutPanel();
            detailsText = new TextBox();
            stepGrid = new DataGridView();
            metricGrid = new DataGridView();
            stepIndexColumn = new DataGridViewTextBoxColumn();
            stepStatusColumn = new DataGridViewTextBoxColumn();
            stepAcceptanceColumn = new DataGridViewTextBoxColumn();
            stepNameColumn = new DataGridViewTextBoxColumn();
            stepToolColumn = new DataGridViewTextBoxColumn();
            stepElapsedColumn = new DataGridViewTextBoxColumn();
            stepImageColumn = new DataGridViewTextBoxColumn();
            stepOverlayColumn = new DataGridViewTextBoxColumn();
            stepMetricColumn = new DataGridViewTextBoxColumn();
            stepMessageColumn = new DataGridViewTextBoxColumn();
            stepAcceptanceMessageColumn = new DataGridViewTextBoxColumn();
            metricNameColumn = new DataGridViewTextBoxColumn();
            metricValueColumn = new DataGridViewTextBoxColumn();
            previewBox = new PictureBox();
            footerPanel = new Panel();
            btnRefresh = new Button();
            btnOpenFolder = new Button();
            rootLayout.SuspendLayout();
            centerLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)stepGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)metricGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)previewBox).BeginInit();
            footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 3;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46F));
            rootLayout.Controls.Add(runList, 0, 0);
            rootLayout.Controls.Add(centerLayout, 1, 0);
            rootLayout.Controls.Add(previewBox, 2, 0);
            rootLayout.Controls.Add(footerPanel, 0, 1);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(12);
            rootLayout.RowCount = 2;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            rootLayout.Size = new Size(1060, 680);
            rootLayout.TabIndex = 0;
            rootLayout.SetColumnSpan(footerPanel, 3);
            // 
            // runList
            // 
            runList.Dock = DockStyle.Fill;
            runList.FormattingEnabled = true;
            runList.IntegralHeight = false;
            runList.ItemHeight = 15;
            runList.Location = new Point(15, 15);
            runList.Name = "runList";
            runList.Size = new Size(244, 608);
            runList.TabIndex = 0;
            runList.SelectedIndexChanged += OnRunSelected;
            // 
            // centerLayout
            // 
            centerLayout.ColumnCount = 1;
            centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            centerLayout.Controls.Add(detailsText, 0, 0);
            centerLayout.Controls.Add(stepGrid, 0, 1);
            centerLayout.Controls.Add(metricGrid, 0, 2);
            centerLayout.Dock = DockStyle.Fill;
            centerLayout.Location = new Point(265, 12);
            centerLayout.Name = "centerLayout";
            centerLayout.Padding = new Padding(8, 0, 8, 0);
            centerLayout.RowCount = 3;
            centerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            centerLayout.Size = new Size(414, 614);
            centerLayout.TabIndex = 1;
            // 
            // detailsText
            // 
            detailsText.Dock = DockStyle.Fill;
            detailsText.Location = new Point(11, 3);
            detailsText.Multiline = true;
            detailsText.Name = "detailsText";
            detailsText.ReadOnly = true;
            detailsText.ScrollBars = ScrollBars.Vertical;
            detailsText.Size = new Size(392, 104);
            detailsText.TabIndex = 0;
            // 
            // stepGrid
            // 
            stepGrid.AllowUserToAddRows = false;
            stepGrid.AllowUserToDeleteRows = false;
            stepGrid.AutoGenerateColumns = false;
            stepGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            stepGrid.Columns.AddRange(new DataGridViewColumn[] { stepIndexColumn, stepStatusColumn, stepAcceptanceColumn, stepNameColumn, stepToolColumn, stepElapsedColumn, stepImageColumn, stepOverlayColumn, stepMetricColumn, stepMessageColumn, stepAcceptanceMessageColumn });
            stepGrid.Dock = DockStyle.Fill;
            stepGrid.Location = new Point(11, 113);
            stepGrid.MultiSelect = false;
            stepGrid.Name = "stepGrid";
            stepGrid.ReadOnly = true;
            stepGrid.RowHeadersVisible = false;
            stepGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            stepGrid.Size = new Size(392, 296);
            stepGrid.TabIndex = 1;
            stepGrid.SelectionChanged += OnStepSelected;
            // 
            // metricGrid
            // 
            metricGrid.AllowUserToAddRows = false;
            metricGrid.AllowUserToDeleteRows = false;
            metricGrid.AutoGenerateColumns = false;
            metricGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            metricGrid.Columns.AddRange(new DataGridViewColumn[] { metricNameColumn, metricValueColumn });
            metricGrid.Dock = DockStyle.Fill;
            metricGrid.Location = new Point(11, 415);
            metricGrid.Name = "metricGrid";
            metricGrid.ReadOnly = true;
            metricGrid.RowHeadersVisible = false;
            metricGrid.Size = new Size(392, 196);
            metricGrid.TabIndex = 2;
            // 
            // stepIndexColumn
            // 
            stepIndexColumn.DataPropertyName = "Index";
            stepIndexColumn.HeaderText = "No";
            stepIndexColumn.Name = "stepIndexColumn";
            stepIndexColumn.Width = 46;
            // 
            // stepStatusColumn
            // 
            stepStatusColumn.DataPropertyName = "Status";
            stepStatusColumn.HeaderText = "Status";
            stepStatusColumn.Name = "stepStatusColumn";
            stepStatusColumn.Width = 70;
            // 
            // stepAcceptanceColumn
            // 
            stepAcceptanceColumn.DataPropertyName = "AcceptancePassed";
            stepAcceptanceColumn.HeaderText = "Accept";
            stepAcceptanceColumn.Name = "stepAcceptanceColumn";
            stepAcceptanceColumn.Width = 64;
            // 
            // stepNameColumn
            // 
            stepNameColumn.DataPropertyName = "Name";
            stepNameColumn.HeaderText = "Name";
            stepNameColumn.Name = "stepNameColumn";
            stepNameColumn.Width = 130;
            // 
            // stepToolColumn
            // 
            stepToolColumn.DataPropertyName = "ToolType";
            stepToolColumn.HeaderText = "Tool";
            stepToolColumn.Name = "stepToolColumn";
            stepToolColumn.Width = 100;
            // 
            // stepElapsedColumn
            // 
            stepElapsedColumn.DataPropertyName = "ElapsedMilliseconds";
            stepElapsedColumn.HeaderText = "ms";
            stepElapsedColumn.Name = "stepElapsedColumn";
            stepElapsedColumn.Width = 70;
            // 
            // stepImageColumn
            // 
            stepImageColumn.DataPropertyName = "ResultImageSize";
            stepImageColumn.HeaderText = "Image";
            stepImageColumn.Name = "stepImageColumn";
            stepImageColumn.Width = 92;
            // 
            // stepOverlayColumn
            // 
            stepOverlayColumn.DataPropertyName = "OverlayCount";
            stepOverlayColumn.HeaderText = "Overlay";
            stepOverlayColumn.Name = "stepOverlayColumn";
            stepOverlayColumn.Width = 68;
            // 
            // stepMetricColumn
            // 
            stepMetricColumn.DataPropertyName = "MetricCount";
            stepMetricColumn.HeaderText = "Metric";
            stepMetricColumn.Name = "stepMetricColumn";
            stepMetricColumn.Width = 64;
            // 
            // stepMessageColumn
            // 
            stepMessageColumn.DataPropertyName = "Message";
            stepMessageColumn.HeaderText = "Message";
            stepMessageColumn.Name = "stepMessageColumn";
            stepMessageColumn.Width = 220;
            // 
            // stepAcceptanceMessageColumn
            // 
            stepAcceptanceMessageColumn.DataPropertyName = "AcceptanceMessage";
            stepAcceptanceMessageColumn.HeaderText = "Acceptance";
            stepAcceptanceMessageColumn.Name = "stepAcceptanceMessageColumn";
            stepAcceptanceMessageColumn.Width = 220;
            // 
            // metricNameColumn
            // 
            metricNameColumn.DataPropertyName = "Name";
            metricNameColumn.HeaderText = "Metric";
            metricNameColumn.Name = "metricNameColumn";
            metricNameColumn.Width = 160;
            // 
            // metricValueColumn
            // 
            metricValueColumn.DataPropertyName = "Value";
            metricValueColumn.HeaderText = "Value";
            metricValueColumn.Name = "metricValueColumn";
            metricValueColumn.Width = 110;
            // 
            // previewBox
            // 
            previewBox.BackColor = Color.Black;
            previewBox.BorderStyle = BorderStyle.FixedSingle;
            previewBox.Dock = DockStyle.Fill;
            previewBox.Location = new Point(685, 15);
            previewBox.Name = "previewBox";
            previewBox.Size = new Size(360, 608);
            previewBox.SizeMode = PictureBoxSizeMode.Zoom;
            previewBox.TabIndex = 2;
            previewBox.TabStop = false;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(btnRefresh);
            footerPanel.Controls.Add(btnOpenFolder);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.Location = new Point(15, 629);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(1030, 36);
            footerPanel.TabIndex = 3;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(250, 252, 253);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.ForeColor = Color.FromArgb(35, 85, 132);
            btnRefresh.Location = new Point(0, 7);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(88, 28);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += OnRefreshClicked;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.BackColor = Color.FromArgb(250, 252, 253);
            btnOpenFolder.FlatStyle = FlatStyle.Flat;
            btnOpenFolder.ForeColor = Color.FromArgb(35, 85, 132);
            btnOpenFolder.Location = new Point(92, 7);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(88, 28);
            btnOpenFolder.TabIndex = 1;
            btnOpenFolder.Text = "Open Folder";
            btnOpenFolder.UseVisualStyleBackColor = false;
            btnOpenFolder.Click += OnOpenFolderClicked;
            // 
            // FormVisionPipelineHistory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            ClientSize = new Size(1060, 680);
            Controls.Add(rootLayout);
            MinimumSize = new Size(920, 560);
            Name = "FormVisionPipelineHistory";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pipeline Run History";
            rootLayout.ResumeLayout(false);
            centerLayout.ResumeLayout(false);
            centerLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)stepGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)metricGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)previewBox).EndInit();
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
