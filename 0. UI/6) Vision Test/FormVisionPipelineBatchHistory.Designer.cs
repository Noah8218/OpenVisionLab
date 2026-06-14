using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineBatchHistory
    {
        private TableLayoutPanel rootLayout;
        private TableLayoutPanel batchPanel;
        private TableLayoutPanel centerPanel;
        private Panel resultControlPanel;
        private TableLayoutPanel previewPanel;
        private Panel footerPanel;
        private Label batchRunsLabel;
        private Label sampleResultsLabel;
        private Label stepPreviewLabel;
        private DataGridViewTextBoxColumn resultSampleColumn;
        private DataGridViewTextBoxColumn resultStatusColumn;
        private DataGridViewTextBoxColumn resultSuccessColumn;
        private DataGridViewTextBoxColumn resultElapsedColumn;
        private DataGridViewTextBoxColumn resultFailedStepColumn;
        private DataGridViewTextBoxColumn resultMessageColumn;
        private DataGridViewTextBoxColumn stepIndexColumn;
        private DataGridViewTextBoxColumn stepStatusColumn;
        private DataGridViewTextBoxColumn stepNameColumn;
        private DataGridViewTextBoxColumn stepToolColumn;
        private DataGridViewTextBoxColumn stepElapsedColumn;
        private DataGridViewTextBoxColumn stepImageColumn;
        private DataGridViewTextBoxColumn stepOverlayColumn;
        private DataGridViewTextBoxColumn stepMetricColumn;
        private DataGridViewTextBoxColumn stepMessageColumn;
        private DataGridViewTextBoxColumn metricNameColumn;
        private DataGridViewTextBoxColumn metricValueColumn;

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            batchPanel = new TableLayoutPanel();
            batchRunsLabel = new Label();
            batchList = new ListBox();
            centerPanel = new TableLayoutPanel();
            sampleResultsLabel = new Label();
            resultControlPanel = new Panel();
            cbResultFilter = new ComboBox();
            resultGrid = new DataGridView();
            detailsText = new TextBox();
            stepGrid = new DataGridView();
            metricGrid = new DataGridView();
            resultSampleColumn = new DataGridViewTextBoxColumn();
            resultStatusColumn = new DataGridViewTextBoxColumn();
            resultSuccessColumn = new DataGridViewTextBoxColumn();
            resultElapsedColumn = new DataGridViewTextBoxColumn();
            resultFailedStepColumn = new DataGridViewTextBoxColumn();
            resultMessageColumn = new DataGridViewTextBoxColumn();
            stepIndexColumn = new DataGridViewTextBoxColumn();
            stepStatusColumn = new DataGridViewTextBoxColumn();
            stepNameColumn = new DataGridViewTextBoxColumn();
            stepToolColumn = new DataGridViewTextBoxColumn();
            stepElapsedColumn = new DataGridViewTextBoxColumn();
            stepImageColumn = new DataGridViewTextBoxColumn();
            stepOverlayColumn = new DataGridViewTextBoxColumn();
            stepMetricColumn = new DataGridViewTextBoxColumn();
            stepMessageColumn = new DataGridViewTextBoxColumn();
            metricNameColumn = new DataGridViewTextBoxColumn();
            metricValueColumn = new DataGridViewTextBoxColumn();
            previewPanel = new TableLayoutPanel();
            stepPreviewLabel = new Label();
            previewBox = new PictureBox();
            footerPanel = new Panel();
            btnRefresh = new Button();
            btnOpenFolder = new Button();
            rootLayout.SuspendLayout();
            batchPanel.SuspendLayout();
            centerPanel.SuspendLayout();
            resultControlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resultGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)stepGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)metricGrid).BeginInit();
            previewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)previewBox).BeginInit();
            footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 3;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F));
            rootLayout.Controls.Add(batchPanel, 0, 0);
            rootLayout.Controls.Add(centerPanel, 1, 0);
            rootLayout.Controls.Add(previewPanel, 2, 0);
            rootLayout.Controls.Add(footerPanel, 0, 1);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(12);
            rootLayout.RowCount = 2;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            rootLayout.Size = new Size(1180, 720);
            rootLayout.TabIndex = 0;
            rootLayout.SetColumnSpan(footerPanel, 3);
            // 
            // batchPanel
            // 
            batchPanel.ColumnCount = 1;
            batchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            batchPanel.Controls.Add(batchRunsLabel, 0, 0);
            batchPanel.Controls.Add(batchList, 0, 1);
            batchPanel.Dock = DockStyle.Fill;
            batchPanel.Location = new Point(12, 12);
            batchPanel.Margin = new Padding(0);
            batchPanel.Name = "batchPanel";
            batchPanel.Padding = new Padding(0, 0, 8, 0);
            batchPanel.RowCount = 2;
            batchPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            batchPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            batchPanel.Size = new Size(260, 650);
            batchPanel.TabIndex = 0;
            // 
            // batchRunsLabel
            // 
            batchRunsLabel.Dock = DockStyle.Fill;
            batchRunsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            batchRunsLabel.ForeColor = Color.FromArgb(35, 85, 132);
            batchRunsLabel.Location = new Point(3, 0);
            batchRunsLabel.Name = "batchRunsLabel";
            batchRunsLabel.Size = new Size(246, 24);
            batchRunsLabel.TabIndex = 0;
            batchRunsLabel.Text = "Batch Runs";
            batchRunsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // batchList
            // 
            batchList.Dock = DockStyle.Fill;
            batchList.FormattingEnabled = true;
            batchList.IntegralHeight = false;
            batchList.ItemHeight = 15;
            batchList.Location = new Point(3, 27);
            batchList.Name = "batchList";
            batchList.Size = new Size(246, 620);
            batchList.TabIndex = 1;
            batchList.SelectedIndexChanged += OnBatchSelected;
            // 
            // centerPanel
            // 
            centerPanel.ColumnCount = 1;
            centerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            centerPanel.Controls.Add(sampleResultsLabel, 0, 0);
            centerPanel.Controls.Add(resultControlPanel, 0, 1);
            centerPanel.Controls.Add(resultGrid, 0, 2);
            centerPanel.Controls.Add(detailsText, 0, 3);
            centerPanel.Controls.Add(stepGrid, 0, 4);
            centerPanel.Controls.Add(metricGrid, 0, 5);
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.Location = new Point(275, 15);
            centerPanel.Name = "centerPanel";
            centerPanel.Padding = new Padding(0, 0, 8, 0);
            centerPanel.RowCount = 6;
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 62F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 38F));
            centerPanel.Size = new Size(499, 644);
            centerPanel.TabIndex = 1;
            // 
            // sampleResultsLabel
            // 
            sampleResultsLabel.Dock = DockStyle.Fill;
            sampleResultsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            sampleResultsLabel.ForeColor = Color.FromArgb(35, 85, 132);
            sampleResultsLabel.Location = new Point(3, 0);
            sampleResultsLabel.Name = "sampleResultsLabel";
            sampleResultsLabel.Size = new Size(485, 24);
            sampleResultsLabel.TabIndex = 0;
            sampleResultsLabel.Text = "Sample Results";
            sampleResultsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // resultControlPanel
            // 
            resultControlPanel.Controls.Add(cbResultFilter);
            resultControlPanel.Dock = DockStyle.Fill;
            resultControlPanel.Location = new Point(3, 27);
            resultControlPanel.Name = "resultControlPanel";
            resultControlPanel.Size = new Size(485, 36);
            resultControlPanel.TabIndex = 1;
            // 
            // cbResultFilter
            // 
            cbResultFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cbResultFilter.FormattingEnabled = true;
            cbResultFilter.Items.AddRange(new object[] { "All", "Failed", "Passed", "Invalid", "Error", "Timeout", "Cancel" });
            cbResultFilter.Location = new Point(0, 9);
            cbResultFilter.Name = "cbResultFilter";
            cbResultFilter.SelectedIndex = 0;
            cbResultFilter.Size = new Size(116, 23);
            cbResultFilter.TabIndex = 0;
            cbResultFilter.SelectedIndexChanged += OnResultFilterChanged;
            // 
            // resultGrid
            // 
            resultGrid.AllowUserToAddRows = false;
            resultGrid.AllowUserToDeleteRows = false;
            resultGrid.AutoGenerateColumns = false;
            resultGrid.Columns.AddRange(new DataGridViewColumn[] { resultSampleColumn, resultStatusColumn, resultSuccessColumn, resultElapsedColumn, resultFailedStepColumn, resultMessageColumn });
            resultGrid.DataSource = resultBinding;
            resultGrid.Dock = DockStyle.Fill;
            resultGrid.Location = new Point(3, 69);
            resultGrid.MultiSelect = false;
            resultGrid.Name = "resultGrid";
            resultGrid.ReadOnly = true;
            resultGrid.RowHeadersVisible = false;
            resultGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            resultGrid.Size = new Size(485, 144);
            resultGrid.TabIndex = 2;
            resultGrid.SelectionChanged += OnResultSelected;
            // 
            // detailsText
            // 
            detailsText.Dock = DockStyle.Fill;
            detailsText.Location = new Point(3, 219);
            detailsText.Multiline = true;
            detailsText.Name = "detailsText";
            detailsText.ReadOnly = true;
            detailsText.ScrollBars = ScrollBars.Vertical;
            detailsText.Size = new Size(485, 72);
            detailsText.TabIndex = 3;
            // 
            // stepGrid
            // 
            stepGrid.AllowUserToAddRows = false;
            stepGrid.AllowUserToDeleteRows = false;
            stepGrid.AutoGenerateColumns = false;
            stepGrid.Columns.AddRange(new DataGridViewColumn[] { stepIndexColumn, stepStatusColumn, stepNameColumn, stepToolColumn, stepElapsedColumn, stepImageColumn, stepOverlayColumn, stepMetricColumn, stepMessageColumn });
            stepGrid.Dock = DockStyle.Fill;
            stepGrid.Location = new Point(3, 297);
            stepGrid.MultiSelect = false;
            stepGrid.Name = "stepGrid";
            stepGrid.ReadOnly = true;
            stepGrid.RowHeadersVisible = false;
            stepGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            stepGrid.Size = new Size(485, 211);
            stepGrid.TabIndex = 4;
            stepGrid.SelectionChanged += OnStepSelected;
            // 
            // metricGrid
            // 
            metricGrid.AllowUserToAddRows = false;
            metricGrid.AllowUserToDeleteRows = false;
            metricGrid.AutoGenerateColumns = false;
            metricGrid.Columns.AddRange(new DataGridViewColumn[] { metricNameColumn, metricValueColumn });
            metricGrid.Dock = DockStyle.Fill;
            metricGrid.Location = new Point(3, 514);
            metricGrid.MultiSelect = false;
            metricGrid.Name = "metricGrid";
            metricGrid.ReadOnly = true;
            metricGrid.RowHeadersVisible = false;
            metricGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            metricGrid.Size = new Size(485, 127);
            metricGrid.TabIndex = 5;
            // 
            // resultSampleColumn
            // 
            resultSampleColumn.DataPropertyName = "SampleName";
            resultSampleColumn.HeaderText = "Sample";
            resultSampleColumn.Name = "resultSampleColumn";
            resultSampleColumn.Width = 150;
            // 
            // resultStatusColumn
            // 
            resultStatusColumn.DataPropertyName = "Status";
            resultStatusColumn.HeaderText = "Status";
            resultStatusColumn.Name = "resultStatusColumn";
            resultStatusColumn.Width = 74;
            // 
            // resultSuccessColumn
            // 
            resultSuccessColumn.DataPropertyName = "Success";
            resultSuccessColumn.HeaderText = "Pass";
            resultSuccessColumn.Name = "resultSuccessColumn";
            resultSuccessColumn.Width = 62;
            // 
            // resultElapsedColumn
            // 
            resultElapsedColumn.DataPropertyName = "TotalMilliseconds";
            resultElapsedColumn.HeaderText = "ms";
            resultElapsedColumn.Name = "resultElapsedColumn";
            resultElapsedColumn.Width = 74;
            // 
            // resultFailedStepColumn
            // 
            resultFailedStepColumn.DataPropertyName = "FailedStep";
            resultFailedStepColumn.HeaderText = "Failed Step";
            resultFailedStepColumn.Name = "resultFailedStepColumn";
            resultFailedStepColumn.Width = 120;
            // 
            // resultMessageColumn
            // 
            resultMessageColumn.DataPropertyName = "Message";
            resultMessageColumn.HeaderText = "Message";
            resultMessageColumn.Name = "resultMessageColumn";
            resultMessageColumn.Width = 260;
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
            stepStatusColumn.Width = 76;
            // 
            // stepNameColumn
            // 
            stepNameColumn.DataPropertyName = "Name";
            stepNameColumn.HeaderText = "Name";
            stepNameColumn.Name = "stepNameColumn";
            stepNameColumn.Width = 150;
            // 
            // stepToolColumn
            // 
            stepToolColumn.DataPropertyName = "ToolType";
            stepToolColumn.HeaderText = "Tool";
            stepToolColumn.Name = "stepToolColumn";
            stepToolColumn.Width = 110;
            // 
            // stepElapsedColumn
            // 
            stepElapsedColumn.DataPropertyName = "ElapsedMilliseconds";
            stepElapsedColumn.HeaderText = "ms";
            stepElapsedColumn.Name = "stepElapsedColumn";
            stepElapsedColumn.Width = 78;
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
            stepMessageColumn.Width = 260;
            // 
            // metricNameColumn
            // 
            metricNameColumn.DataPropertyName = "Name";
            metricNameColumn.HeaderText = "Metric";
            metricNameColumn.Name = "metricNameColumn";
            metricNameColumn.Width = 190;
            // 
            // metricValueColumn
            // 
            metricValueColumn.DataPropertyName = "Value";
            metricValueColumn.HeaderText = "Value";
            metricValueColumn.Name = "metricValueColumn";
            metricValueColumn.Width = 120;
            // 
            // previewPanel
            // 
            previewPanel.ColumnCount = 1;
            previewPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            previewPanel.Controls.Add(stepPreviewLabel, 0, 0);
            previewPanel.Controls.Add(previewBox, 0, 1);
            previewPanel.Dock = DockStyle.Fill;
            previewPanel.Location = new Point(780, 12);
            previewPanel.Margin = new Padding(0);
            previewPanel.Name = "previewPanel";
            previewPanel.RowCount = 2;
            previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            previewPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            previewPanel.Size = new Size(388, 650);
            previewPanel.TabIndex = 2;
            // 
            // stepPreviewLabel
            // 
            stepPreviewLabel.Dock = DockStyle.Fill;
            stepPreviewLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            stepPreviewLabel.ForeColor = Color.FromArgb(35, 85, 132);
            stepPreviewLabel.Location = new Point(3, 0);
            stepPreviewLabel.Name = "stepPreviewLabel";
            stepPreviewLabel.Size = new Size(382, 24);
            stepPreviewLabel.TabIndex = 0;
            stepPreviewLabel.Text = "Step Preview";
            stepPreviewLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // previewBox
            // 
            previewBox.BackColor = Color.Black;
            previewBox.BorderStyle = BorderStyle.FixedSingle;
            previewBox.Dock = DockStyle.Fill;
            previewBox.Location = new Point(3, 27);
            previewBox.Name = "previewBox";
            previewBox.Size = new Size(382, 620);
            previewBox.SizeMode = PictureBoxSizeMode.Zoom;
            previewBox.TabIndex = 1;
            previewBox.TabStop = false;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(btnRefresh);
            footerPanel.Controls.Add(btnOpenFolder);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.Location = new Point(15, 665);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(1150, 40);
            footerPanel.TabIndex = 3;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(250, 252, 253);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.ForeColor = Color.FromArgb(35, 85, 132);
            btnRefresh.Location = new Point(0, 8);
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
            btnOpenFolder.Location = new Point(96, 8);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(108, 28);
            btnOpenFolder.TabIndex = 1;
            btnOpenFolder.Text = "Open Folder";
            btnOpenFolder.UseVisualStyleBackColor = false;
            btnOpenFolder.Click += OnOpenFolderClicked;
            // 
            // FormVisionPipelineBatchHistory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            ClientSize = new Size(1180, 720);
            Controls.Add(rootLayout);
            MinimumSize = new Size(1040, 620);
            Name = "FormVisionPipelineBatchHistory";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pipeline Batch History";
            resultBinding.DataSource = visibleResults;
            rootLayout.ResumeLayout(false);
            batchPanel.ResumeLayout(false);
            centerPanel.ResumeLayout(false);
            centerPanel.PerformLayout();
            resultControlPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)resultGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)stepGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)metricGrid).EndInit();
            previewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)previewBox).EndInit();
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
