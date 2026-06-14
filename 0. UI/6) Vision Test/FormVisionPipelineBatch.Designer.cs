using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineBatch
    {
        private TableLayoutPanel rootLayout;
        private TableLayoutPanel samplePanel;
        private FlowLayoutPanel sampleButtonsPanel;
        private TableLayoutPanel centerPanel;
        private Panel batchControlPanel;
        private TableLayoutPanel previewPanel;
        private FlowLayoutPanel footerPanel;
        private Label sampleSetsLabel;
        private Label batchResultsLabel;
        private Label stepPreviewLabel;
        private DataGridViewTextBoxColumn resultSampleColumn;
        private DataGridViewTextBoxColumn resultStatusColumn;
        private DataGridViewTextBoxColumn resultSuccessColumn;
        private DataGridViewTextBoxColumn resultElapsedColumn;
        private DataGridViewTextBoxColumn resultFailedStepColumn;
        private DataGridViewTextBoxColumn resultMessageColumn;
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
            samplePanel = new TableLayoutPanel();
            sampleSetsLabel = new Label();
            sampleList = new CheckedListBox();
            sampleButtonsPanel = new FlowLayoutPanel();
            btnSelectAll = new Button();
            btnClear = new Button();
            btnRefresh = new Button();
            centerPanel = new TableLayoutPanel();
            batchResultsLabel = new Label();
            batchControlPanel = new Panel();
            cbResultFilter = new ComboBox();
            batchStatusLabel = new Label();
            progressBar = new ProgressBar();
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
            previewPanel = new TableLayoutPanel();
            stepPreviewLabel = new Label();
            previewBox = new PictureBox();
            footerPanel = new FlowLayoutPanel();
            btnCancel = new Button();
            btnRunFailed = new Button();
            btnRunAll = new Button();
            btnRunSelected = new Button();
            btnOpenSummary = new Button();
            btnHistory = new Button();
            rootLayout.SuspendLayout();
            samplePanel.SuspendLayout();
            sampleButtonsPanel.SuspendLayout();
            centerPanel.SuspendLayout();
            batchControlPanel.SuspendLayout();
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
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F));
            rootLayout.Controls.Add(samplePanel, 0, 0);
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
            // samplePanel
            // 
            samplePanel.ColumnCount = 1;
            samplePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            samplePanel.Controls.Add(sampleSetsLabel, 0, 0);
            samplePanel.Controls.Add(sampleList, 0, 1);
            samplePanel.Controls.Add(sampleButtonsPanel, 0, 2);
            samplePanel.Dock = DockStyle.Fill;
            samplePanel.Location = new Point(12, 12);
            samplePanel.Margin = new Padding(0);
            samplePanel.Name = "samplePanel";
            samplePanel.Padding = new Padding(0, 0, 8, 0);
            samplePanel.RowCount = 3;
            samplePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            samplePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            samplePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));
            samplePanel.Size = new Size(250, 650);
            samplePanel.TabIndex = 0;
            // 
            // sampleSetsLabel
            // 
            sampleSetsLabel.Dock = DockStyle.Fill;
            sampleSetsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            sampleSetsLabel.ForeColor = Color.FromArgb(35, 85, 132);
            sampleSetsLabel.Location = new Point(3, 0);
            sampleSetsLabel.Name = "sampleSetsLabel";
            sampleSetsLabel.Size = new Size(236, 24);
            sampleSetsLabel.TabIndex = 0;
            sampleSetsLabel.Text = "Sample Sets";
            sampleSetsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // sampleList
            // 
            sampleList.CheckOnClick = true;
            sampleList.Dock = DockStyle.Fill;
            sampleList.FormattingEnabled = true;
            sampleList.IntegralHeight = false;
            sampleList.Location = new Point(3, 27);
            sampleList.Name = "sampleList";
            sampleList.Size = new Size(236, 544);
            sampleList.TabIndex = 1;
            // 
            // sampleButtonsPanel
            // 
            sampleButtonsPanel.Controls.Add(btnSelectAll);
            sampleButtonsPanel.Controls.Add(btnClear);
            sampleButtonsPanel.Controls.Add(btnRefresh);
            sampleButtonsPanel.Dock = DockStyle.Fill;
            sampleButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
            sampleButtonsPanel.Location = new Point(3, 577);
            sampleButtonsPanel.Name = "sampleButtonsPanel";
            sampleButtonsPanel.Size = new Size(236, 70);
            sampleButtonsPanel.TabIndex = 2;
            sampleButtonsPanel.WrapContents = true;
            // 
            // btnSelectAll
            // 
            btnSelectAll.BackColor = Color.FromArgb(250, 252, 253);
            btnSelectAll.FlatStyle = FlatStyle.Flat;
            btnSelectAll.ForeColor = Color.FromArgb(35, 85, 132);
            btnSelectAll.Margin = new Padding(4, 8, 4, 4);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(68, 28);
            btnSelectAll.TabIndex = 0;
            btnSelectAll.Text = "All";
            btnSelectAll.UseVisualStyleBackColor = false;
            btnSelectAll.Click += OnSelectAllClicked;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.FromArgb(250, 252, 253);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.ForeColor = Color.FromArgb(35, 85, 132);
            btnClear.Margin = new Padding(4, 8, 4, 4);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(68, 28);
            btnClear.TabIndex = 1;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += OnClearClicked;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(250, 252, 253);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.ForeColor = Color.FromArgb(35, 85, 132);
            btnRefresh.Margin = new Padding(4, 8, 4, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(82, 28);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += OnRefreshClicked;
            // 
            // centerPanel
            // 
            centerPanel.ColumnCount = 1;
            centerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            centerPanel.Controls.Add(batchResultsLabel, 0, 0);
            centerPanel.Controls.Add(batchControlPanel, 0, 1);
            centerPanel.Controls.Add(resultGrid, 0, 2);
            centerPanel.Controls.Add(detailsText, 0, 3);
            centerPanel.Controls.Add(stepGrid, 0, 4);
            centerPanel.Controls.Add(metricGrid, 0, 5);
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.Location = new Point(265, 15);
            centerPanel.Name = "centerPanel";
            centerPanel.Padding = new Padding(0, 0, 8, 0);
            centerPanel.RowCount = 6;
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 62F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 38F));
            centerPanel.Size = new Size(504, 644);
            centerPanel.TabIndex = 1;
            // 
            // batchResultsLabel
            // 
            batchResultsLabel.Dock = DockStyle.Fill;
            batchResultsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            batchResultsLabel.ForeColor = Color.FromArgb(35, 85, 132);
            batchResultsLabel.Location = new Point(3, 0);
            batchResultsLabel.Name = "batchResultsLabel";
            batchResultsLabel.Size = new Size(490, 24);
            batchResultsLabel.TabIndex = 0;
            batchResultsLabel.Text = "Batch Results";
            batchResultsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // batchControlPanel
            // 
            batchControlPanel.Controls.Add(cbResultFilter);
            batchControlPanel.Controls.Add(batchStatusLabel);
            batchControlPanel.Controls.Add(progressBar);
            batchControlPanel.Dock = DockStyle.Fill;
            batchControlPanel.Location = new Point(3, 27);
            batchControlPanel.Name = "batchControlPanel";
            batchControlPanel.Size = new Size(490, 40);
            batchControlPanel.TabIndex = 1;
            // 
            // cbResultFilter
            // 
            cbResultFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cbResultFilter.FormattingEnabled = true;
            cbResultFilter.Items.AddRange(new object[] { "All", "Failed", "Passed", "Invalid", "Error", "Timeout", "Cancel" });
            cbResultFilter.Location = new Point(0, 10);
            cbResultFilter.Name = "cbResultFilter";
            cbResultFilter.SelectedIndex = 0;
            cbResultFilter.Size = new Size(116, 23);
            cbResultFilter.TabIndex = 0;
            cbResultFilter.SelectedIndexChanged += OnResultFilterChanged;
            // 
            // batchStatusLabel
            // 
            batchStatusLabel.Location = new Point(128, 2);
            batchStatusLabel.Name = "batchStatusLabel";
            batchStatusLabel.Size = new Size(420, 20);
            batchStatusLabel.TabIndex = 1;
            batchStatusLabel.Text = "Ready";
            batchStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(128, 24);
            progressBar.Maximum = 1;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(420, 16);
            progressBar.TabIndex = 2;
            // 
            // resultGrid
            // 
            resultGrid.AllowUserToAddRows = false;
            resultGrid.AllowUserToDeleteRows = false;
            resultGrid.AutoGenerateColumns = false;
            resultGrid.Columns.AddRange(new DataGridViewColumn[] { resultSampleColumn, resultStatusColumn, resultSuccessColumn, resultElapsedColumn, resultFailedStepColumn, resultMessageColumn });
            resultGrid.DataSource = resultBinding;
            resultGrid.Dock = DockStyle.Fill;
            resultGrid.Location = new Point(3, 73);
            resultGrid.MultiSelect = false;
            resultGrid.Name = "resultGrid";
            resultGrid.ReadOnly = true;
            resultGrid.RowHeadersVisible = false;
            resultGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            resultGrid.Size = new Size(490, 144);
            resultGrid.TabIndex = 2;
            resultGrid.SelectionChanged += OnResultSelected;
            // 
            // detailsText
            // 
            detailsText.Dock = DockStyle.Fill;
            detailsText.Location = new Point(3, 223);
            detailsText.Multiline = true;
            detailsText.Name = "detailsText";
            detailsText.ReadOnly = true;
            detailsText.ScrollBars = ScrollBars.Vertical;
            detailsText.Size = new Size(490, 72);
            detailsText.TabIndex = 3;
            // 
            // stepGrid
            // 
            stepGrid.AllowUserToAddRows = false;
            stepGrid.AllowUserToDeleteRows = false;
            stepGrid.AutoGenerateColumns = false;
            stepGrid.Columns.AddRange(new DataGridViewColumn[] { stepIndexColumn, stepStatusColumn, stepAcceptanceColumn, stepNameColumn, stepToolColumn, stepElapsedColumn, stepImageColumn, stepOverlayColumn, stepMetricColumn, stepMessageColumn, stepAcceptanceMessageColumn });
            stepGrid.Dock = DockStyle.Fill;
            stepGrid.Location = new Point(3, 301);
            stepGrid.MultiSelect = false;
            stepGrid.Name = "stepGrid";
            stepGrid.ReadOnly = true;
            stepGrid.RowHeadersVisible = false;
            stepGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            stepGrid.Size = new Size(490, 209);
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
            metricGrid.Location = new Point(3, 516);
            metricGrid.MultiSelect = false;
            metricGrid.Name = "metricGrid";
            metricGrid.ReadOnly = true;
            metricGrid.RowHeadersVisible = false;
            metricGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            metricGrid.Size = new Size(490, 125);
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
            previewPanel.Location = new Point(778, 12);
            previewPanel.Margin = new Padding(0);
            previewPanel.Name = "previewPanel";
            previewPanel.RowCount = 2;
            previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            previewPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            previewPanel.Size = new Size(390, 650);
            previewPanel.TabIndex = 2;
            // 
            // stepPreviewLabel
            // 
            stepPreviewLabel.Dock = DockStyle.Fill;
            stepPreviewLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            stepPreviewLabel.ForeColor = Color.FromArgb(35, 85, 132);
            stepPreviewLabel.Location = new Point(3, 0);
            stepPreviewLabel.Name = "stepPreviewLabel";
            stepPreviewLabel.Size = new Size(384, 24);
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
            previewBox.Size = new Size(384, 620);
            previewBox.SizeMode = PictureBoxSizeMode.Zoom;
            previewBox.TabIndex = 1;
            previewBox.TabStop = false;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(btnCancel);
            footerPanel.Controls.Add(btnRunFailed);
            footerPanel.Controls.Add(btnRunAll);
            footerPanel.Controls.Add(btnRunSelected);
            footerPanel.Controls.Add(btnOpenSummary);
            footerPanel.Controls.Add(btnHistory);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.FlowDirection = FlowDirection.RightToLeft;
            footerPanel.Location = new Point(15, 665);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(1150, 40);
            footerPanel.TabIndex = 3;
            footerPanel.WrapContents = false;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(250, 252, 253);
            btnCancel.Enabled = false;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.FromArgb(35, 85, 132);
            btnCancel.Margin = new Padding(4, 8, 4, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 28);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += OnCancelClicked;
            // 
            // btnRunFailed
            // 
            btnRunFailed.BackColor = Color.FromArgb(250, 252, 253);
            btnRunFailed.FlatStyle = FlatStyle.Flat;
            btnRunFailed.ForeColor = Color.FromArgb(35, 85, 132);
            btnRunFailed.Margin = new Padding(4, 8, 4, 4);
            btnRunFailed.Name = "btnRunFailed";
            btnRunFailed.Size = new Size(104, 28);
            btnRunFailed.TabIndex = 1;
            btnRunFailed.Text = "Run Failed";
            btnRunFailed.UseVisualStyleBackColor = false;
            btnRunFailed.Click += OnRunFailedClicked;
            // 
            // btnRunAll
            // 
            btnRunAll.BackColor = Color.FromArgb(250, 252, 253);
            btnRunAll.FlatStyle = FlatStyle.Flat;
            btnRunAll.ForeColor = Color.FromArgb(35, 85, 132);
            btnRunAll.Margin = new Padding(4, 8, 4, 4);
            btnRunAll.Name = "btnRunAll";
            btnRunAll.Size = new Size(88, 28);
            btnRunAll.TabIndex = 2;
            btnRunAll.Text = "Run All";
            btnRunAll.UseVisualStyleBackColor = false;
            btnRunAll.Click += OnRunAllClicked;
            // 
            // btnRunSelected
            // 
            btnRunSelected.BackColor = Color.FromArgb(250, 252, 253);
            btnRunSelected.FlatStyle = FlatStyle.Flat;
            btnRunSelected.ForeColor = Color.FromArgb(35, 85, 132);
            btnRunSelected.Margin = new Padding(4, 8, 4, 4);
            btnRunSelected.Name = "btnRunSelected";
            btnRunSelected.Size = new Size(104, 28);
            btnRunSelected.TabIndex = 3;
            btnRunSelected.Text = "Run Checked";
            btnRunSelected.UseVisualStyleBackColor = false;
            btnRunSelected.Click += OnRunSelectedClicked;
            // 
            // btnOpenSummary
            // 
            btnOpenSummary.BackColor = Color.FromArgb(250, 252, 253);
            btnOpenSummary.Enabled = false;
            btnOpenSummary.FlatStyle = FlatStyle.Flat;
            btnOpenSummary.ForeColor = Color.FromArgb(35, 85, 132);
            btnOpenSummary.Margin = new Padding(4, 8, 4, 4);
            btnOpenSummary.Name = "btnOpenSummary";
            btnOpenSummary.Size = new Size(116, 28);
            btnOpenSummary.TabIndex = 4;
            btnOpenSummary.Text = "Open Summary";
            btnOpenSummary.UseVisualStyleBackColor = false;
            btnOpenSummary.Click += OnOpenSummaryClicked;
            // 
            // btnHistory
            // 
            btnHistory.BackColor = Color.FromArgb(250, 252, 253);
            btnHistory.FlatStyle = FlatStyle.Flat;
            btnHistory.ForeColor = Color.FromArgb(35, 85, 132);
            btnHistory.Margin = new Padding(4, 8, 4, 4);
            btnHistory.Name = "btnHistory";
            btnHistory.Size = new Size(88, 28);
            btnHistory.TabIndex = 5;
            btnHistory.Text = "History";
            btnHistory.UseVisualStyleBackColor = false;
            btnHistory.Click += OnHistoryClicked;
            // 
            // FormVisionPipelineBatch
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            ClientSize = new Size(1180, 720);
            Controls.Add(rootLayout);
            MinimumSize = new Size(1040, 620);
            Name = "FormVisionPipelineBatch";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pipeline Batch Run";
            resultBinding.DataSource = visibleBatchResults;
            rootLayout.ResumeLayout(false);
            samplePanel.ResumeLayout(false);
            sampleButtonsPanel.ResumeLayout(false);
            centerPanel.ResumeLayout(false);
            centerPanel.PerformLayout();
            batchControlPanel.ResumeLayout(false);
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
