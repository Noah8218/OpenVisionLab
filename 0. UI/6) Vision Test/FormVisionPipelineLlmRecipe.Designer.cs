using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineLlmRecipe
    {
        private TableLayoutPanel rootLayout;
        private Panel toolbarPanel;
        private TableLayoutPanel bodyLayout;
        private TableLayoutPanel rightLayout;
        private Panel footerPanel;
        private TableLayoutPanel xmlFrameLayout;
        private TableLayoutPanel logFrameLayout;
        private Label xmlCaptionLabel;
        private Label logCaptionLabel;
        private Label validationCaptionLabel;
        private Label previewCaptionLabel;
        private Label runResultCaptionLabel;
        private DataGridViewTextBoxColumn validationTypeColumn;
        private DataGridViewTextBoxColumn validationMessageColumn;
        private DataGridViewTextBoxColumn stepNoColumn;
        private DataGridViewTextBoxColumn stepNameColumn;
        private DataGridViewTextBoxColumn stepStatusColumn;
        private DataGridViewTextBoxColumn stepElapsedColumn;
        private DataGridViewTextBoxColumn stepMetricsColumn;

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            toolbarPanel = new Panel();
            btnPaste = new Button();
            btnOpenXml = new Button();
            btnSample = new Button();
            btnPrompt = new Button();
            btnValidate = new Button();
            btnLoadImage = new Button();
            btnUseCurrent = new Button();
            btnRunPreview = new Button();
            imageStatusLabel = new Label();
            bodyLayout = new TableLayoutPanel();
            xmlFrameLayout = new TableLayoutPanel();
            xmlCaptionLabel = new Label();
            tbXml = new TextBox();
            rightLayout = new TableLayoutPanel();
            validationCaptionLabel = new Label();
            validationGrid = new DataGridView();
            validationTypeColumn = new DataGridViewTextBoxColumn();
            validationMessageColumn = new DataGridViewTextBoxColumn();
            previewCaptionLabel = new Label();
            previewBox = new PictureBox();
            runResultCaptionLabel = new Label();
            stepGrid = new DataGridView();
            stepNoColumn = new DataGridViewTextBoxColumn();
            stepNameColumn = new DataGridViewTextBoxColumn();
            stepStatusColumn = new DataGridViewTextBoxColumn();
            stepElapsedColumn = new DataGridViewTextBoxColumn();
            stepMetricsColumn = new DataGridViewTextBoxColumn();
            logFrameLayout = new TableLayoutPanel();
            logCaptionLabel = new Label();
            tbLog = new TextBox();
            footerPanel = new Panel();
            btnApply = new Button();
            btnClose = new Button();
            rootLayout.SuspendLayout();
            toolbarPanel.SuspendLayout();
            bodyLayout.SuspendLayout();
            xmlFrameLayout.SuspendLayout();
            rightLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)validationGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)previewBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)stepGrid).BeginInit();
            logFrameLayout.SuspendLayout();
            footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(toolbarPanel, 0, 0);
            rootLayout.Controls.Add(bodyLayout, 0, 1);
            rootLayout.Controls.Add(logFrameLayout, 0, 2);
            rootLayout.Controls.Add(footerPanel, 0, 3);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(12);
            rootLayout.RowCount = 4;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            rootLayout.Size = new Size(1180, 760);
            rootLayout.TabIndex = 0;
            // 
            // toolbarPanel
            // 
            toolbarPanel.Controls.Add(btnPaste);
            toolbarPanel.Controls.Add(btnOpenXml);
            toolbarPanel.Controls.Add(btnSample);
            toolbarPanel.Controls.Add(btnPrompt);
            toolbarPanel.Controls.Add(btnValidate);
            toolbarPanel.Controls.Add(btnLoadImage);
            toolbarPanel.Controls.Add(btnUseCurrent);
            toolbarPanel.Controls.Add(btnRunPreview);
            toolbarPanel.Controls.Add(imageStatusLabel);
            toolbarPanel.Dock = DockStyle.Fill;
            toolbarPanel.Location = new Point(15, 15);
            toolbarPanel.Name = "toolbarPanel";
            toolbarPanel.Size = new Size(1150, 32);
            toolbarPanel.TabIndex = 0;
            // 
            // btnPaste
            // 
            btnPaste.BackColor = Color.FromArgb(250, 252, 253);
            btnPaste.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnPaste.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnPaste.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnPaste.FlatStyle = FlatStyle.Flat;
            btnPaste.ForeColor = Color.FromArgb(35, 85, 132);
            btnPaste.Location = new Point(0, 5);
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new Size(72, 28);
            btnPaste.TabIndex = 0;
            btnPaste.Text = "Paste";
            btnPaste.UseVisualStyleBackColor = false;
            btnPaste.Click += OnPasteClicked;
            // 
            // btnOpenXml
            // 
            btnOpenXml.BackColor = Color.FromArgb(250, 252, 253);
            btnOpenXml.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnOpenXml.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnOpenXml.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnOpenXml.FlatStyle = FlatStyle.Flat;
            btnOpenXml.ForeColor = Color.FromArgb(35, 85, 132);
            btnOpenXml.Location = new Point(78, 5);
            btnOpenXml.Name = "btnOpenXml";
            btnOpenXml.Size = new Size(92, 28);
            btnOpenXml.TabIndex = 1;
            btnOpenXml.Text = "Open XML";
            btnOpenXml.UseVisualStyleBackColor = false;
            btnOpenXml.Click += OnOpenXmlClicked;
            // 
            // btnSample
            // 
            btnSample.BackColor = Color.FromArgb(250, 252, 253);
            btnSample.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnSample.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnSample.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnSample.FlatStyle = FlatStyle.Flat;
            btnSample.ForeColor = Color.FromArgb(35, 85, 132);
            btnSample.Location = new Point(176, 5);
            btnSample.Name = "btnSample";
            btnSample.Size = new Size(84, 28);
            btnSample.TabIndex = 2;
            btnSample.Text = "Sample";
            btnSample.UseVisualStyleBackColor = false;
            btnSample.Click += OnSampleClicked;
            // 
            // btnPrompt
            // 
            btnPrompt.BackColor = Color.FromArgb(250, 252, 253);
            btnPrompt.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnPrompt.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnPrompt.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnPrompt.FlatStyle = FlatStyle.Flat;
            btnPrompt.ForeColor = Color.FromArgb(35, 85, 132);
            btnPrompt.Location = new Point(266, 5);
            btnPrompt.Name = "btnPrompt";
            btnPrompt.Size = new Size(86, 28);
            btnPrompt.TabIndex = 3;
            btnPrompt.Text = "Prompt";
            btnPrompt.UseVisualStyleBackColor = false;
            btnPrompt.Click += OnPromptClicked;
            // 
            // btnValidate
            // 
            btnValidate.BackColor = Color.FromArgb(250, 252, 253);
            btnValidate.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnValidate.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnValidate.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnValidate.FlatStyle = FlatStyle.Flat;
            btnValidate.ForeColor = Color.FromArgb(35, 85, 132);
            btnValidate.Location = new Point(358, 5);
            btnValidate.Name = "btnValidate";
            btnValidate.Size = new Size(88, 28);
            btnValidate.TabIndex = 4;
            btnValidate.Text = "Validate";
            btnValidate.UseVisualStyleBackColor = false;
            btnValidate.Click += OnValidateClicked;
            // 
            // btnLoadImage
            // 
            btnLoadImage.BackColor = Color.FromArgb(250, 252, 253);
            btnLoadImage.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnLoadImage.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnLoadImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnLoadImage.FlatStyle = FlatStyle.Flat;
            btnLoadImage.ForeColor = Color.FromArgb(35, 85, 132);
            btnLoadImage.Location = new Point(452, 5);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new Size(98, 28);
            btnLoadImage.TabIndex = 5;
            btnLoadImage.Text = "Load Image";
            btnLoadImage.UseVisualStyleBackColor = false;
            btnLoadImage.Click += OnLoadImageClicked;
            // 
            // btnUseCurrent
            // 
            btnUseCurrent.BackColor = Color.FromArgb(250, 252, 253);
            btnUseCurrent.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnUseCurrent.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnUseCurrent.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnUseCurrent.FlatStyle = FlatStyle.Flat;
            btnUseCurrent.ForeColor = Color.FromArgb(35, 85, 132);
            btnUseCurrent.Location = new Point(556, 5);
            btnUseCurrent.Name = "btnUseCurrent";
            btnUseCurrent.Size = new Size(118, 28);
            btnUseCurrent.TabIndex = 6;
            btnUseCurrent.Text = "Current Layers";
            btnUseCurrent.UseVisualStyleBackColor = false;
            btnUseCurrent.Click += OnUseCurrentClicked;
            // 
            // btnRunPreview
            // 
            btnRunPreview.BackColor = Color.FromArgb(250, 252, 253);
            btnRunPreview.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnRunPreview.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnRunPreview.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnRunPreview.FlatStyle = FlatStyle.Flat;
            btnRunPreview.ForeColor = Color.FromArgb(35, 85, 132);
            btnRunPreview.Location = new Point(680, 5);
            btnRunPreview.Name = "btnRunPreview";
            btnRunPreview.Size = new Size(108, 28);
            btnRunPreview.TabIndex = 7;
            btnRunPreview.Text = "Run Preview";
            btnRunPreview.UseVisualStyleBackColor = false;
            btnRunPreview.Click += OnRunPreviewClicked;
            // 
            // imageStatusLabel
            // 
            imageStatusLabel.AutoSize = false;
            imageStatusLabel.ForeColor = Color.FromArgb(35, 85, 132);
            imageStatusLabel.Location = new Point(800, 8);
            imageStatusLabel.Name = "imageStatusLabel";
            imageStatusLabel.Size = new Size(322, 22);
            imageStatusLabel.TabIndex = 8;
            imageStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // bodyLayout
            // 
            bodyLayout.ColumnCount = 2;
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            bodyLayout.Controls.Add(xmlFrameLayout, 0, 0);
            bodyLayout.Controls.Add(rightLayout, 1, 0);
            bodyLayout.Dock = DockStyle.Fill;
            bodyLayout.Location = new Point(15, 53);
            bodyLayout.Name = "bodyLayout";
            bodyLayout.RowCount = 1;
            bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bodyLayout.Size = new Size(1150, 546);
            bodyLayout.TabIndex = 1;
            // 
            // xmlFrameLayout
            // 
            xmlFrameLayout.ColumnCount = 1;
            xmlFrameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            xmlFrameLayout.Controls.Add(xmlCaptionLabel, 0, 0);
            xmlFrameLayout.Controls.Add(tbXml, 0, 1);
            xmlFrameLayout.Dock = DockStyle.Fill;
            xmlFrameLayout.Location = new Point(3, 3);
            xmlFrameLayout.Name = "xmlFrameLayout";
            xmlFrameLayout.RowCount = 2;
            xmlFrameLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            xmlFrameLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            xmlFrameLayout.Size = new Size(626, 540);
            xmlFrameLayout.TabIndex = 0;
            // 
            // xmlCaptionLabel
            // 
            xmlCaptionLabel.Dock = DockStyle.Fill;
            xmlCaptionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            xmlCaptionLabel.ForeColor = Color.FromArgb(35, 85, 132);
            xmlCaptionLabel.Location = new Point(0, 0);
            xmlCaptionLabel.Margin = new Padding(0);
            xmlCaptionLabel.Name = "xmlCaptionLabel";
            xmlCaptionLabel.Size = new Size(626, 22);
            xmlCaptionLabel.TabIndex = 0;
            xmlCaptionLabel.Text = "LLM XML";
            xmlCaptionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tbXml
            // 
            tbXml.AcceptsReturn = true;
            tbXml.AcceptsTab = true;
            tbXml.BorderStyle = BorderStyle.FixedSingle;
            tbXml.Dock = DockStyle.Fill;
            tbXml.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            tbXml.Location = new Point(0, 22);
            tbXml.Margin = new Padding(0);
            tbXml.Multiline = true;
            tbXml.Name = "tbXml";
            tbXml.ScrollBars = ScrollBars.Both;
            tbXml.Size = new Size(626, 518);
            tbXml.TabIndex = 0;
            tbXml.WordWrap = false;
            tbXml.TextChanged += OnXmlTextChanged;
            // 
            // rightLayout
            // 
            rightLayout.ColumnCount = 1;
            rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rightLayout.Controls.Add(validationCaptionLabel, 0, 0);
            rightLayout.Controls.Add(validationGrid, 0, 1);
            rightLayout.Controls.Add(previewCaptionLabel, 0, 2);
            rightLayout.Controls.Add(previewBox, 0, 3);
            rightLayout.Controls.Add(runResultCaptionLabel, 0, 4);
            rightLayout.Controls.Add(stepGrid, 0, 5);
            rightLayout.Dock = DockStyle.Fill;
            rightLayout.Location = new Point(635, 3);
            rightLayout.Name = "rightLayout";
            rightLayout.Padding = new Padding(8, 0, 0, 0);
            rightLayout.RowCount = 6;
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 124F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 142F));
            rightLayout.Size = new Size(512, 540);
            rightLayout.TabIndex = 1;
            // 
            // validationCaptionLabel
            // 
            validationCaptionLabel.Dock = DockStyle.Fill;
            validationCaptionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            validationCaptionLabel.ForeColor = Color.FromArgb(35, 85, 132);
            validationCaptionLabel.Location = new Point(11, 0);
            validationCaptionLabel.Name = "validationCaptionLabel";
            validationCaptionLabel.Size = new Size(498, 22);
            validationCaptionLabel.TabIndex = 0;
            validationCaptionLabel.Text = "Validation";
            validationCaptionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // validationGrid
            // 
            validationGrid.AllowUserToAddRows = false;
            validationGrid.AllowUserToDeleteRows = false;
            validationGrid.AllowUserToResizeRows = false;
            validationGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 248, 252);
            validationGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            validationGrid.BackgroundColor = Color.White;
            validationGrid.BorderStyle = BorderStyle.FixedSingle;
            validationGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            validationGrid.Columns.AddRange(new DataGridViewColumn[] { validationTypeColumn, validationMessageColumn });
            validationGrid.Columns[0].FillWeight = 22F;
            validationGrid.Columns[1].FillWeight = 78F;
            validationGrid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            validationGrid.Dock = DockStyle.Fill;
            validationGrid.Location = new Point(11, 25);
            validationGrid.MultiSelect = false;
            validationGrid.Name = "validationGrid";
            validationGrid.ReadOnly = true;
            validationGrid.RowHeadersVisible = false;
            validationGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            validationGrid.Size = new Size(498, 118);
            validationGrid.TabIndex = 1;
            // 
            // validationTypeColumn
            // 
            validationTypeColumn.HeaderText = "Type";
            validationTypeColumn.Name = "validationTypeColumn";
            // 
            // validationMessageColumn
            // 
            validationMessageColumn.HeaderText = "Message";
            validationMessageColumn.Name = "validationMessageColumn";
            // 
            // previewCaptionLabel
            // 
            previewCaptionLabel.Dock = DockStyle.Fill;
            previewCaptionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            previewCaptionLabel.ForeColor = Color.FromArgb(35, 85, 132);
            previewCaptionLabel.Location = new Point(11, 146);
            previewCaptionLabel.Name = "previewCaptionLabel";
            previewCaptionLabel.Size = new Size(498, 22);
            previewCaptionLabel.TabIndex = 2;
            previewCaptionLabel.Text = "Preview";
            previewCaptionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // previewBox
            // 
            previewBox.BackColor = Color.Black;
            previewBox.BorderStyle = BorderStyle.FixedSingle;
            previewBox.Dock = DockStyle.Fill;
            previewBox.Location = new Point(11, 171);
            previewBox.Name = "previewBox";
            previewBox.Size = new Size(498, 202);
            previewBox.SizeMode = PictureBoxSizeMode.Zoom;
            previewBox.TabIndex = 3;
            previewBox.TabStop = false;
            previewBox.DoubleClick += OnPreviewBoxDoubleClick;
            // 
            // runResultCaptionLabel
            // 
            runResultCaptionLabel.Dock = DockStyle.Fill;
            runResultCaptionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            runResultCaptionLabel.ForeColor = Color.FromArgb(35, 85, 132);
            runResultCaptionLabel.Location = new Point(11, 376);
            runResultCaptionLabel.Name = "runResultCaptionLabel";
            runResultCaptionLabel.Size = new Size(498, 22);
            runResultCaptionLabel.TabIndex = 4;
            runResultCaptionLabel.Text = "Run Result";
            runResultCaptionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // stepGrid
            // 
            stepGrid.AllowUserToAddRows = false;
            stepGrid.AllowUserToDeleteRows = false;
            stepGrid.AllowUserToResizeRows = false;
            stepGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 248, 252);
            stepGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            stepGrid.BackgroundColor = Color.White;
            stepGrid.BorderStyle = BorderStyle.FixedSingle;
            stepGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            stepGrid.Columns.AddRange(new DataGridViewColumn[] { stepNoColumn, stepNameColumn, stepStatusColumn, stepElapsedColumn, stepMetricsColumn });
            stepGrid.Columns[0].FillWeight = 12F;
            stepGrid.Columns[1].FillWeight = 30F;
            stepGrid.Columns[2].FillWeight = 16F;
            stepGrid.Columns[3].FillWeight = 18F;
            stepGrid.Columns[4].FillWeight = 42F;
            stepGrid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            stepGrid.Dock = DockStyle.Fill;
            stepGrid.Location = new Point(11, 401);
            stepGrid.MultiSelect = false;
            stepGrid.Name = "stepGrid";
            stepGrid.ReadOnly = true;
            stepGrid.RowHeadersVisible = false;
            stepGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            stepGrid.Size = new Size(498, 136);
            stepGrid.TabIndex = 5;
            // 
            // stepNoColumn
            // 
            stepNoColumn.HeaderText = "No";
            stepNoColumn.Name = "stepNoColumn";
            // 
            // stepNameColumn
            // 
            stepNameColumn.HeaderText = "Step";
            stepNameColumn.Name = "stepNameColumn";
            // 
            // stepStatusColumn
            // 
            stepStatusColumn.HeaderText = "Status";
            stepStatusColumn.Name = "stepStatusColumn";
            // 
            // stepElapsedColumn
            // 
            stepElapsedColumn.HeaderText = "Elapsed";
            stepElapsedColumn.Name = "stepElapsedColumn";
            // 
            // stepMetricsColumn
            // 
            stepMetricsColumn.HeaderText = "Metrics";
            stepMetricsColumn.Name = "stepMetricsColumn";
            // 
            // logFrameLayout
            // 
            logFrameLayout.ColumnCount = 1;
            logFrameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            logFrameLayout.Controls.Add(logCaptionLabel, 0, 0);
            logFrameLayout.Controls.Add(tbLog, 0, 1);
            logFrameLayout.Dock = DockStyle.Fill;
            logFrameLayout.Location = new Point(15, 605);
            logFrameLayout.Name = "logFrameLayout";
            logFrameLayout.RowCount = 2;
            logFrameLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            logFrameLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            logFrameLayout.Size = new Size(1150, 110);
            logFrameLayout.TabIndex = 2;
            // 
            // logCaptionLabel
            // 
            logCaptionLabel.Dock = DockStyle.Fill;
            logCaptionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            logCaptionLabel.ForeColor = Color.FromArgb(35, 85, 132);
            logCaptionLabel.Location = new Point(0, 0);
            logCaptionLabel.Margin = new Padding(0);
            logCaptionLabel.Name = "logCaptionLabel";
            logCaptionLabel.Size = new Size(1150, 22);
            logCaptionLabel.TabIndex = 0;
            logCaptionLabel.Text = "Log";
            logCaptionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tbLog
            // 
            tbLog.BorderStyle = BorderStyle.FixedSingle;
            tbLog.Dock = DockStyle.Fill;
            tbLog.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            tbLog.Location = new Point(0, 22);
            tbLog.Margin = new Padding(0);
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ReadOnly = true;
            tbLog.ScrollBars = ScrollBars.Vertical;
            tbLog.Size = new Size(1144, 85);
            tbLog.TabIndex = 2;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(btnApply);
            footerPanel.Controls.Add(btnClose);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.Location = new Point(15, 721);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(1150, 24);
            footerPanel.TabIndex = 3;
            // 
            // btnApply
            // 
            btnApply.BackColor = Color.FromArgb(250, 252, 253);
            btnApply.Enabled = false;
            btnApply.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnApply.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnApply.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.ForeColor = Color.FromArgb(35, 85, 132);
            btnApply.Location = new Point(0, 5);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(138, 28);
            btnApply.TabIndex = 0;
            btnApply.Text = "Apply to Pipeline";
            btnApply.UseVisualStyleBackColor = false;
            btnApply.Click += OnApplyClicked;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(250, 252, 253);
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.ForeColor = Color.FromArgb(35, 85, 132);
            btnClose.Location = new Point(146, 5);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(82, 28);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += OnCloseClicked;
            // 
            // FormVisionPipelineLlmRecipe
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            ClientSize = new Size(1180, 760);
            Controls.Add(rootLayout);
            MinimumSize = new Size(980, 640);
            Name = "FormVisionPipelineLlmRecipe";
            StartPosition = FormStartPosition.CenterParent;
            Text = "AI Recipe Import";
            rootLayout.ResumeLayout(false);
            toolbarPanel.ResumeLayout(false);
            bodyLayout.ResumeLayout(false);
            xmlFrameLayout.ResumeLayout(false);
            xmlFrameLayout.PerformLayout();
            rightLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)validationGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)previewBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)stepGrid).EndInit();
            logFrameLayout.ResumeLayout(false);
            logFrameLayout.PerformLayout();
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
