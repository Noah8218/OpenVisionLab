using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;

namespace OpenVisionLab
{
    public partial class FormVision_Pipeline
    {
		private void InitializeComponent()
		{
			rootLayout = new TableLayoutPanel();
			headerPanel = new Panel();
			nameLabel = new Label();
			toolTypeLabel = new Label();
			inputLayerLabel = new Label();
			outputLayerLabel = new Label();
			sampleContextLabel = new Label();
			tbPipelineName = new TextBox();
			cbToolType = new ComboBox();
			cbInputLayer = new ComboBox();
			tbOutputLayer = new TextBox();
			btnAdd = new Button();
			btnAiRecipe = new Button();
			bodyLayout = new TableLayoutPanel();
			stepTreePanel = new TableLayoutPanel();
			stepTreeCaption = new Label();
			treeSteps = new TreeView();
			pipelineFlowHost = new System.Windows.Forms.Integration.ElementHost();
			flowPreviewCaption = new Label();
			tbFlowPreview = new TextBox();
			editorPanel = new TableLayoutPanel();
			propertiesCaption = new Label();
			stepIoPanel = new TableLayoutPanel();
			stepInputCaption = new Label();
			cbStepInputLayer = new ComboBox();
			stepFlowArrowLabel = new Label();
			stepOutputCaption = new Label();
			tbStepOutputLayer = new TextBox();
			btnStepChainInput = new Button();
			stepIoStatusLabel = new Label();
			stepAcceptancePanel = new TableLayoutPanel();
			stepAcceptanceCaption = new Label();
			stepAcceptanceStatusLabel = new Label();
			cbStepAcceptancePreset = new ComboBox();
			btnApplyStepAcceptancePreset = new Button();
			btnClearStepAcceptance = new Button();
			propertyGridHost = new System.Windows.Forms.Integration.ElementHost();
			runLogPanel = new TableLayoutPanel();
			runLogCaption = new Label();
			tbRunLog = new TextBox();
			previewPanel = new TableLayoutPanel();
			previewCaption = new Label();
			previewOptionsPanel = new Panel();
			cbPreviewImageMode = new ComboBox();
			previewModeLabel = new Label();
			overlayLabelModeLabel = new Label();
			cbOverlayLabelMode = new ComboBox();
			overlayPointLimitLabel = new Label();
			nudOverlayPointLimit = new NumericUpDown();
			chkOverlayRoi = new CheckBox();
			btnOpenPreview = new Button();
			previewBox = new PictureBox();
			matchingReviewPanel = new Panel();
			matchingTemplateTitle = new Label();
			matchingDetectedTitle = new Label();
			matchingReviewTitle = new Label();
			matchingTemplateBox = new PictureBox();
			matchingDetectedBox = new PictureBox();
			matchingReviewSummary = new Label();
			resultCaption = new Label();
			resultGrid = new DataGridView();
			footerPanel = new Panel();
			btnRemove = new Button();
			btnUp = new Button();
			btnDown = new Button();
			btnLoad = new Button();
			btnSave = new Button();
			btnRun = new Button();
			btnPublish = new Button();
			btnCancel = new Button();
			btnMore = new Button();
			btnHistory = new Button();
			btnSamples = new Button();
			btnBatch = new Button();
			btnImport = new Button();
			btnValidate = new Button();
			chkPublishAllLayers = new CheckBox();
			workflowHintLabel = new Label();
			pipelineToolTip = new ToolTip();
			pnlClientArea.SuspendLayout();
			rootLayout.SuspendLayout();
			headerPanel.SuspendLayout();
			bodyLayout.SuspendLayout();
			stepTreePanel.SuspendLayout();
			editorPanel.SuspendLayout();
			stepAcceptancePanel.SuspendLayout();
			runLogPanel.SuspendLayout();
			previewPanel.SuspendLayout();
			((ISupportInitialize)nudOverlayPointLimit).BeginInit();
			((ISupportInitialize)previewBox).BeginInit();
			matchingReviewPanel.SuspendLayout();
			((ISupportInitialize)matchingTemplateBox).BeginInit();
			((ISupportInitialize)matchingDetectedBox).BeginInit();
			((ISupportInitialize)resultGrid).BeginInit();
			footerPanel.SuspendLayout();
			SuspendLayout();
			// 
			// pnlClientArea
			// 
			pnlClientArea.BackColor = Color.FromArgb(238, 242, 246);
			pnlClientArea.Controls.Add(rootLayout);
			pnlClientArea.Location = new DrawingPoint(1, 41);
			pnlClientArea.Size = new DrawingSize(1278, 738);
			// 
			// rootLayout
			// 
			rootLayout.BackColor = Color.FromArgb(238, 242, 246);
			rootLayout.ColumnCount = 1;
			rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			rootLayout.Controls.Add(headerPanel, 0, 0);
			rootLayout.Controls.Add(bodyLayout, 0, 1);
			rootLayout.Controls.Add(runLogPanel, 0, 2);
			rootLayout.Controls.Add(footerPanel, 0, 3);
			rootLayout.Dock = DockStyle.Fill;
			rootLayout.Location = new DrawingPoint(0, 0);
			rootLayout.Name = "rootLayout";
			rootLayout.Padding = new Padding(12);
			rootLayout.RowCount = 4;
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
			rootLayout.Size = new DrawingSize(1278, 738);
			rootLayout.TabIndex = 0;
			// 
			// headerPanel
			// 
			headerPanel.Controls.Add(nameLabel);
			headerPanel.Controls.Add(toolTypeLabel);
			headerPanel.Controls.Add(inputLayerLabel);
			headerPanel.Controls.Add(outputLayerLabel);
			headerPanel.Controls.Add(sampleContextLabel);
			headerPanel.Controls.Add(tbPipelineName);
			headerPanel.Controls.Add(cbToolType);
			headerPanel.Controls.Add(cbInputLayer);
			headerPanel.Controls.Add(tbOutputLayer);
			headerPanel.Controls.Add(btnAdd);
			headerPanel.Controls.Add(btnAiRecipe);
			headerPanel.Dock = DockStyle.Fill;
			headerPanel.Location = new DrawingPoint(15, 15);
			headerPanel.Name = "headerPanel";
			headerPanel.Size = new DrawingSize(1248, 54);
			headerPanel.TabIndex = 0;
			// 
			// nameLabel
			// 
			nameLabel.AutoSize = true;
			nameLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
			nameLabel.ForeColor = Color.FromArgb(35, 85, 132);
			nameLabel.Location = new DrawingPoint(0, 1);
			nameLabel.Name = "nameLabel";
			nameLabel.Size = new DrawingSize(58, 15);
			nameLabel.TabIndex = 0;
			nameLabel.Text = "Pipeline";
			// 
			// toolTypeLabel
			// 
			toolTypeLabel.AutoSize = true;
			toolTypeLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
			toolTypeLabel.ForeColor = Color.FromArgb(35, 85, 132);
			toolTypeLabel.Location = new DrawingPoint(320, 1);
			toolTypeLabel.Name = "toolTypeLabel";
			toolTypeLabel.Size = new DrawingSize(89, 15);
			toolTypeLabel.TabIndex = 1;
			toolTypeLabel.Text = "New Step Tool";
			// 
			// inputLayerLabel
			// 
			inputLayerLabel.AutoSize = true;
			inputLayerLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
			inputLayerLabel.ForeColor = Color.FromArgb(35, 85, 132);
			inputLayerLabel.Location = new DrawingPoint(0, 0);
			inputLayerLabel.Name = "inputLayerLabel";
			inputLayerLabel.Size = new DrawingSize(67, 15);
			inputLayerLabel.TabIndex = 2;
			inputLayerLabel.Text = "Input Layer";
			inputLayerLabel.Visible = false;
			// 
			// outputLayerLabel
			// 
			outputLayerLabel.AutoSize = true;
			outputLayerLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
			outputLayerLabel.ForeColor = Color.FromArgb(35, 85, 132);
			outputLayerLabel.Location = new DrawingPoint(0, 0);
			outputLayerLabel.Name = "outputLayerLabel";
			outputLayerLabel.Size = new DrawingSize(76, 15);
			outputLayerLabel.TabIndex = 3;
			outputLayerLabel.Text = "Output Layer";
			outputLayerLabel.Visible = false;
			// 
			// sampleContextLabel
			// 
			sampleContextLabel.AutoEllipsis = true;
			sampleContextLabel.BackColor = Color.FromArgb(224, 238, 251);
			sampleContextLabel.BorderStyle = BorderStyle.FixedSingle;
			sampleContextLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
			sampleContextLabel.ForeColor = Color.FromArgb(35, 85, 132);
			sampleContextLabel.Location = new DrawingPoint(704, 21);
			sampleContextLabel.Name = "sampleContextLabel";
			sampleContextLabel.Padding = new Padding(8, 0, 8, 0);
			sampleContextLabel.Size = new DrawingSize(424, 28);
			sampleContextLabel.TabIndex = 10;
			sampleContextLabel.TextAlign = ContentAlignment.MiddleLeft;
			sampleContextLabel.Visible = false;
			// 
			// tbPipelineName
			// 
			tbPipelineName.Location = new DrawingPoint(0, 23);
			tbPipelineName.Name = "tbPipelineName";
			tbPipelineName.Size = new DrawingSize(280, 22);
			tbPipelineName.TabIndex = 4;
			tbPipelineName.Text = "Pipeline";
			tbPipelineName.TextChanged += OnPipelineNameTextChanged;
			// 
			// cbToolType
			// 
			cbToolType.DropDownStyle = ComboBoxStyle.DropDownList;
			cbToolType.FormattingEnabled = true;
			cbToolType.Items.AddRange(new object[] { "Threshold", "Morphology", "Filter", "EdgeDetection", "Blob", "Contour", "LineGauge", "RotateScale", "Matching", "FeatureMatching", "Mean", "OverlayMerge" });
			cbToolType.Location = new DrawingPoint(320, 22);
			cbToolType.Name = "cbToolType";
			cbToolType.Size = new DrawingSize(220, 24);
			cbToolType.TabIndex = 5;
			cbToolType.SelectedIndex = 0;
			cbToolType.SelectedIndexChanged += OnHeaderStepDefaultChanged;
			// 
			// cbInputLayer
			// 
			cbInputLayer.DropDownStyle = ComboBoxStyle.DropDownList;
			cbInputLayer.FormattingEnabled = true;
			cbInputLayer.Location = new DrawingPoint(0, 0);
			cbInputLayer.Name = "cbInputLayer";
			cbInputLayer.Size = new DrawingSize(190, 24);
			cbInputLayer.TabIndex = 6;
			cbInputLayer.Visible = false;
			cbInputLayer.SelectedIndexChanged += OnHeaderStepDefaultChanged;
			// 
			// tbOutputLayer
			// 
			tbOutputLayer.Location = new DrawingPoint(0, 0);
			tbOutputLayer.Name = "tbOutputLayer";
			tbOutputLayer.Size = new DrawingSize(220, 22);
			tbOutputLayer.TabIndex = 7;
			tbOutputLayer.Text = "Pipeline_Output";
			tbOutputLayer.Visible = false;
			tbOutputLayer.TextChanged += OnHeaderOutputLayerTextChanged;
			// 
			// btnAdd
			// 
			btnAdd.BackColor = Color.FromArgb(250, 252, 253);
			btnAdd.FlatStyle = FlatStyle.Flat;
			btnAdd.ForeColor = Color.FromArgb(35, 85, 132);
			btnAdd.Location = new DrawingPoint(556, 21);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new DrawingSize(128, 28);
			btnAdd.TabIndex = 8;
			btnAdd.Text = "Add Step";
			btnAdd.UseVisualStyleBackColor = false;
			btnAdd.Click += OnAddClicked;
			// 
			// btnAiRecipe
			// 
			btnAiRecipe.BackColor = Color.FromArgb(250, 252, 253);
			btnAiRecipe.FlatStyle = FlatStyle.Flat;
			btnAiRecipe.ForeColor = Color.FromArgb(35, 85, 132);
			btnAiRecipe.Location = new DrawingPoint(1044, 21);
			btnAiRecipe.Name = "btnAiRecipe";
			btnAiRecipe.Size = new DrawingSize(112, 28);
			btnAiRecipe.TabIndex = 9;
			btnAiRecipe.Text = "AI Recipe";
			btnAiRecipe.UseVisualStyleBackColor = false;
			btnAiRecipe.Visible = false;
			btnAiRecipe.Click += OnAiRecipeClicked;
			// 
			// bodyLayout
			// 
			bodyLayout.ColumnCount = 3;
			bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
			bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
			bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
			bodyLayout.Controls.Add(stepTreePanel, 0, 0);
			bodyLayout.Controls.Add(editorPanel, 1, 0);
			bodyLayout.Controls.Add(previewPanel, 2, 0);
			bodyLayout.Dock = DockStyle.Fill;
			bodyLayout.Location = new DrawingPoint(15, 75);
			bodyLayout.Name = "bodyLayout";
			bodyLayout.RowCount = 1;
			bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			bodyLayout.Size = new DrawingSize(1248, 482);
			bodyLayout.TabIndex = 1;
			// 
			// stepTreePanel
			// 
			stepTreePanel.ColumnCount = 1;
			stepTreePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			stepTreePanel.Controls.Add(stepTreeCaption, 0, 0);
			stepTreePanel.Controls.Add(treeSteps, 0, 1);
			stepTreePanel.Controls.Add(pipelineFlowHost, 0, 1);
			stepTreePanel.Controls.Add(flowPreviewCaption, 0, 2);
			stepTreePanel.Controls.Add(tbFlowPreview, 0, 3);
			stepTreePanel.Dock = DockStyle.Fill;
			stepTreePanel.Location = new DrawingPoint(3, 3);
			stepTreePanel.Name = "stepTreePanel";
			stepTreePanel.Padding = new Padding(0, 0, 8, 0);
			stepTreePanel.RowCount = 4;
			stepTreePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			stepTreePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			stepTreePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0F));
			stepTreePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0F));
			stepTreePanel.Size = new DrawingSize(334, 494);
			stepTreePanel.TabIndex = 0;
			// 
			// stepTreeCaption
			// 
			stepTreeCaption.Dock = DockStyle.Fill;
			stepTreeCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			stepTreeCaption.ForeColor = Color.FromArgb(35, 85, 132);
			stepTreeCaption.Location = new DrawingPoint(3, 0);
			stepTreeCaption.Name = "stepTreeCaption";
			stepTreeCaption.Size = new DrawingSize(320, 22);
			stepTreeCaption.TabIndex = 0;
			stepTreeCaption.Text = "Pipeline Flow";
			stepTreeCaption.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// treeSteps
			// 
			treeSteps.BackColor = Color.White;
			treeSteps.BorderStyle = BorderStyle.FixedSingle;
			treeSteps.Dock = DockStyle.Fill;
			treeSteps.FullRowSelect = true;
			treeSteps.HideSelection = false;
			treeSteps.Location = new DrawingPoint(3, 25);
			treeSteps.Name = "treeSteps";
			treeSteps.ShowNodeToolTips = true;
			treeSteps.Size = new DrawingSize(320, 328);
			treeSteps.TabIndex = 1;
			treeSteps.Visible = false;
			treeSteps.AfterSelect += OnTreeStepSelected;
			// 
			// pipelineFlowHost
			// 
			pipelineFlowHost.BackColor = Color.White;
			pipelineFlowHost.Dock = DockStyle.Fill;
			pipelineFlowHost.Location = new DrawingPoint(3, 25);
			pipelineFlowHost.Name = "pipelineFlowHost";
			pipelineFlowHost.Size = new DrawingSize(320, 328);
			pipelineFlowHost.TabIndex = 1;
			pipelineFlowHost.Text = "pipelineFlowHost";
			// 
			// flowPreviewCaption
			// 
			flowPreviewCaption.Dock = DockStyle.Fill;
			flowPreviewCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			flowPreviewCaption.ForeColor = Color.FromArgb(35, 85, 132);
			flowPreviewCaption.Location = new DrawingPoint(3, 356);
			flowPreviewCaption.Name = "flowPreviewCaption";
			flowPreviewCaption.Size = new DrawingSize(320, 22);
			flowPreviewCaption.TabIndex = 2;
			flowPreviewCaption.Text = "Step Flow";
			flowPreviewCaption.TextAlign = ContentAlignment.MiddleLeft;
			flowPreviewCaption.Visible = false;
			// 
			// tbFlowPreview
			// 
			tbFlowPreview.BackColor = Color.White;
			tbFlowPreview.BorderStyle = BorderStyle.FixedSingle;
			tbFlowPreview.Dock = DockStyle.Fill;
			tbFlowPreview.Font = new Font("Consolas", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
			tbFlowPreview.Location = new DrawingPoint(3, 381);
			tbFlowPreview.Multiline = true;
			tbFlowPreview.Name = "tbFlowPreview";
			tbFlowPreview.ReadOnly = true;
			tbFlowPreview.ScrollBars = ScrollBars.Vertical;
			tbFlowPreview.Size = new DrawingSize(320, 110);
			tbFlowPreview.TabIndex = 2;
			tbFlowPreview.WordWrap = true;
			tbFlowPreview.Visible = false;
			// 
			// editorPanel
			// 
			editorPanel.ColumnCount = 1;
			editorPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			editorPanel.Controls.Add(propertiesCaption, 0, 0);
			editorPanel.Controls.Add(stepIoPanel, 0, 1);
			editorPanel.Controls.Add(stepAcceptancePanel, 0, 2);
			editorPanel.Controls.Add(propertyGridHost, 0, 3);
			editorPanel.Dock = DockStyle.Fill;
			editorPanel.Location = new DrawingPoint(343, 3);
			editorPanel.Name = "editorPanel";
			editorPanel.RowCount = 4;
			editorPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			editorPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 94F));
			editorPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 86F));
			editorPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			editorPanel.Size = new DrawingSize(522, 494);
			editorPanel.TabIndex = 1;
			// 
			// propertiesCaption
			// 
			propertiesCaption.Dock = DockStyle.Fill;
			propertiesCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			propertiesCaption.ForeColor = Color.FromArgb(35, 85, 132);
			propertiesCaption.Location = new DrawingPoint(3, 0);
			propertiesCaption.Name = "propertiesCaption";
			propertiesCaption.Size = new DrawingSize(619, 22);
			propertiesCaption.TabIndex = 0;
			propertiesCaption.Text = "Properties";
			propertiesCaption.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// stepIoPanel
			// 
			stepIoPanel.BackColor = Color.FromArgb(247, 250, 253);
			stepIoPanel.ColumnCount = 5;
			stepIoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F));
			stepIoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24F));
			stepIoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F));
			stepIoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8F));
			stepIoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 104F));
			stepIoPanel.Controls.Add(stepInputCaption, 0, 0);
			stepIoPanel.Controls.Add(cbStepInputLayer, 0, 1);
			stepIoPanel.Controls.Add(stepFlowArrowLabel, 1, 1);
			stepIoPanel.Controls.Add(stepOutputCaption, 2, 0);
			stepIoPanel.Controls.Add(tbStepOutputLayer, 2, 1);
			stepIoPanel.Controls.Add(btnStepChainInput, 4, 1);
			stepIoPanel.Controls.Add(stepIoStatusLabel, 0, 2);
			stepIoPanel.Dock = DockStyle.Fill;
			stepIoPanel.Location = new DrawingPoint(3, 25);
			stepIoPanel.Name = "stepIoPanel";
			stepIoPanel.Padding = new Padding(8, 6, 8, 6);
			stepIoPanel.RowCount = 3;
			stepIoPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			stepIoPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
			stepIoPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
			stepIoPanel.SetColumnSpan(stepIoStatusLabel, 5);
			stepIoPanel.Size = new DrawingSize(516, 88);
			stepIoPanel.TabIndex = 1;
			// 
			// stepInputCaption
			// 
			stepInputCaption.Dock = DockStyle.Fill;
			stepInputCaption.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
			stepInputCaption.ForeColor = Color.FromArgb(35, 85, 132);
			stepInputCaption.Location = new DrawingPoint(11, 6);
			stepInputCaption.Name = "stepInputCaption";
			stepInputCaption.Size = new DrawingSize(181, 22);
			stepInputCaption.TabIndex = 0;
			stepInputCaption.Text = "INPUT SOURCE";
			stepInputCaption.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbStepInputLayer
			// 
			cbStepInputLayer.Dock = DockStyle.Fill;
			cbStepInputLayer.DropDownStyle = ComboBoxStyle.DropDownList;
			cbStepInputLayer.FlatStyle = FlatStyle.Flat;
			cbStepInputLayer.FormattingEnabled = true;
			cbStepInputLayer.Location = new DrawingPoint(11, 31);
			cbStepInputLayer.Name = "cbStepInputLayer";
			cbStepInputLayer.Size = new DrawingSize(181, 24);
			cbStepInputLayer.TabIndex = 1;
			cbStepInputLayer.SelectedIndexChanged += OnStepIoChanged;
			// 
			// stepFlowArrowLabel
			// 
			stepFlowArrowLabel.Dock = DockStyle.Fill;
			stepFlowArrowLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			stepFlowArrowLabel.ForeColor = Color.FromArgb(35, 85, 132);
			stepFlowArrowLabel.Location = new DrawingPoint(198, 28);
			stepFlowArrowLabel.Name = "stepFlowArrowLabel";
			stepFlowArrowLabel.Size = new DrawingSize(18, 32);
			stepFlowArrowLabel.TabIndex = 2;
			stepFlowArrowLabel.Text = ">";
			stepFlowArrowLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// stepOutputCaption
			// 
			stepOutputCaption.Dock = DockStyle.Fill;
			stepOutputCaption.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
			stepOutputCaption.ForeColor = Color.FromArgb(35, 85, 132);
			stepOutputCaption.Location = new DrawingPoint(222, 6);
			stepOutputCaption.Name = "stepOutputCaption";
			stepOutputCaption.Size = new DrawingSize(181, 22);
			stepOutputCaption.TabIndex = 3;
			stepOutputCaption.Text = "OUTPUT RESULT";
			stepOutputCaption.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// tbStepOutputLayer
			// 
			tbStepOutputLayer.BorderStyle = BorderStyle.FixedSingle;
			tbStepOutputLayer.Dock = DockStyle.Fill;
			tbStepOutputLayer.Location = new DrawingPoint(222, 31);
			tbStepOutputLayer.Name = "tbStepOutputLayer";
			tbStepOutputLayer.Size = new DrawingSize(181, 22);
			tbStepOutputLayer.TabIndex = 4;
			tbStepOutputLayer.TextChanged += OnStepIoChanged;
			// 
			// btnStepChainInput
			// 
			btnStepChainInput.BackColor = Color.FromArgb(250, 252, 253);
			btnStepChainInput.Dock = DockStyle.Fill;
			btnStepChainInput.FlatStyle = FlatStyle.Flat;
			btnStepChainInput.ForeColor = Color.FromArgb(35, 85, 132);
			btnStepChainInput.Location = new DrawingPoint(417, 31);
			btnStepChainInput.Name = "btnStepChainInput";
			btnStepChainInput.Size = new DrawingSize(88, 26);
			btnStepChainInput.TabIndex = 5;
			btnStepChainInput.Text = "Link Prev";
			btnStepChainInput.UseVisualStyleBackColor = false;
			btnStepChainInput.Click += OnChainSelectedStepInputClicked;
			// 
			// stepIoStatusLabel
			// 
			stepIoStatusLabel.AutoEllipsis = true;
			stepIoStatusLabel.BackColor = Color.FromArgb(236, 244, 252);
			stepIoStatusLabel.Dock = DockStyle.Fill;
			stepIoStatusLabel.Font = new Font("Segoe UI", 8.3F, FontStyle.Bold, GraphicsUnit.Point);
			stepIoStatusLabel.ForeColor = Color.FromArgb(35, 85, 132);
			stepIoStatusLabel.Location = new DrawingPoint(11, 58);
			stepIoStatusLabel.Margin = new Padding(3, 0, 3, 0);
			stepIoStatusLabel.Name = "stepIoStatusLabel";
			stepIoStatusLabel.Padding = new Padding(8, 0, 8, 0);
			stepIoStatusLabel.Size = new DrawingSize(494, 24);
			stepIoStatusLabel.TabIndex = 6;
			stepIoStatusLabel.Text = "Select a step to edit image flow.";
			stepIoStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// stepAcceptancePanel
			// 
			stepAcceptancePanel.BackColor = Color.FromArgb(247, 250, 253);
			stepAcceptancePanel.ColumnCount = 4;
			stepAcceptancePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			stepAcceptancePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 86F));
			stepAcceptancePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8F));
			stepAcceptancePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72F));
			stepAcceptancePanel.Controls.Add(stepAcceptanceCaption, 0, 0);
			stepAcceptancePanel.Controls.Add(cbStepAcceptancePreset, 0, 1);
			stepAcceptancePanel.Controls.Add(btnApplyStepAcceptancePreset, 1, 1);
			stepAcceptancePanel.Controls.Add(btnClearStepAcceptance, 3, 1);
			stepAcceptancePanel.Controls.Add(stepAcceptanceStatusLabel, 0, 2);
			stepAcceptancePanel.Dock = DockStyle.Fill;
			stepAcceptancePanel.Location = new DrawingPoint(3, 119);
			stepAcceptancePanel.Name = "stepAcceptancePanel";
			stepAcceptancePanel.Padding = new Padding(8, 5, 8, 5);
			stepAcceptancePanel.RowCount = 3;
			stepAcceptancePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			stepAcceptancePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
			stepAcceptancePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			stepAcceptancePanel.SetColumnSpan(stepAcceptanceCaption, 4);
			stepAcceptancePanel.SetColumnSpan(stepAcceptanceStatusLabel, 4);
			stepAcceptancePanel.Size = new DrawingSize(516, 80);
			stepAcceptancePanel.TabIndex = 2;
			// 
			// stepAcceptanceCaption
			// 
			stepAcceptanceCaption.Dock = DockStyle.Fill;
			stepAcceptanceCaption.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
			stepAcceptanceCaption.ForeColor = Color.FromArgb(35, 85, 132);
			stepAcceptanceCaption.Location = new DrawingPoint(11, 5);
			stepAcceptanceCaption.Name = "stepAcceptanceCaption";
			stepAcceptanceCaption.Size = new DrawingSize(494, 22);
			stepAcceptanceCaption.TabIndex = 0;
			stepAcceptanceCaption.Text = "RECOMMENDED ACCEPTANCE";
			stepAcceptanceCaption.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// stepAcceptanceStatusLabel
			// 
			stepAcceptanceStatusLabel.AutoEllipsis = true;
			stepAcceptanceStatusLabel.Dock = DockStyle.Fill;
			stepAcceptanceStatusLabel.Font = new Font("Segoe UI", 8.3F, FontStyle.Bold, GraphicsUnit.Point);
			stepAcceptanceStatusLabel.ForeColor = Color.FromArgb(92, 98, 108);
			stepAcceptanceStatusLabel.Location = new DrawingPoint(11, 59);
			stepAcceptanceStatusLabel.Name = "stepAcceptanceStatusLabel";
			stepAcceptanceStatusLabel.Size = new DrawingSize(494, 22);
			stepAcceptanceStatusLabel.TabIndex = 1;
			stepAcceptanceStatusLabel.Text = "No acceptance";
			stepAcceptanceStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbStepAcceptancePreset
			// 
			cbStepAcceptancePreset.Dock = DockStyle.Fill;
			cbStepAcceptancePreset.DropDownStyle = ComboBoxStyle.DropDownList;
			cbStepAcceptancePreset.FlatStyle = FlatStyle.Flat;
			cbStepAcceptancePreset.FormattingEnabled = true;
			cbStepAcceptancePreset.Location = new DrawingPoint(11, 30);
			cbStepAcceptancePreset.Name = "cbStepAcceptancePreset";
			cbStepAcceptancePreset.Size = new DrawingSize(336, 24);
			cbStepAcceptancePreset.TabIndex = 2;
			cbStepAcceptancePreset.SelectedIndexChanged += OnStepAcceptancePresetSelectionChanged;
			// 
			// btnApplyStepAcceptancePreset
			// 
			btnApplyStepAcceptancePreset.BackColor = Color.FromArgb(250, 252, 253);
			btnApplyStepAcceptancePreset.Dock = DockStyle.Fill;
			btnApplyStepAcceptancePreset.FlatStyle = FlatStyle.Flat;
			btnApplyStepAcceptancePreset.ForeColor = Color.FromArgb(35, 85, 132);
			btnApplyStepAcceptancePreset.Location = new DrawingPoint(353, 30);
			btnApplyStepAcceptancePreset.Name = "btnApplyStepAcceptancePreset";
			btnApplyStepAcceptancePreset.Size = new DrawingSize(80, 26);
			btnApplyStepAcceptancePreset.TabIndex = 3;
			btnApplyStepAcceptancePreset.Text = "Apply";
			btnApplyStepAcceptancePreset.UseVisualStyleBackColor = false;
			btnApplyStepAcceptancePreset.Click += OnApplyStepAcceptancePresetClicked;
			// 
			// btnClearStepAcceptance
			// 
			btnClearStepAcceptance.BackColor = Color.FromArgb(250, 252, 253);
			btnClearStepAcceptance.Dock = DockStyle.Fill;
			btnClearStepAcceptance.FlatStyle = FlatStyle.Flat;
			btnClearStepAcceptance.ForeColor = Color.FromArgb(92, 98, 108);
			btnClearStepAcceptance.Location = new DrawingPoint(447, 30);
			btnClearStepAcceptance.Name = "btnClearStepAcceptance";
			btnClearStepAcceptance.Size = new DrawingSize(58, 26);
			btnClearStepAcceptance.TabIndex = 4;
			btnClearStepAcceptance.Text = "Clear";
			btnClearStepAcceptance.UseVisualStyleBackColor = false;
			btnClearStepAcceptance.Click += OnClearAcceptanceClicked;
			// 
			// propertyGridHost
			// 
			propertyGridHost.BackColor = Color.White;
			propertyGridHost.Dock = DockStyle.Fill;
			propertyGridHost.Location = new DrawingPoint(3, 205);
			propertyGridHost.Name = "propertyGridHost";
			propertyGridHost.Size = new DrawingSize(516, 286);
			propertyGridHost.TabIndex = 3;
			propertyGridHost.Text = "propertyGridHost";
			// 
			// runLogPanel
			// 
			runLogPanel.ColumnCount = 1;
			runLogPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			runLogPanel.Controls.Add(runLogCaption, 0, 0);
			runLogPanel.Controls.Add(tbRunLog, 0, 1);
			runLogPanel.Dock = DockStyle.Fill;
			runLogPanel.Location = new DrawingPoint(15, 563);
			runLogPanel.Name = "runLogPanel";
			runLogPanel.RowCount = 2;
			runLogPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			runLogPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			runLogPanel.Size = new DrawingSize(1248, 114);
			runLogPanel.TabIndex = 2;
			// 
			// runLogCaption
			// 
			runLogCaption.Dock = DockStyle.Fill;
			runLogCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			runLogCaption.ForeColor = Color.FromArgb(35, 85, 132);
			runLogCaption.Location = new DrawingPoint(3, 0);
			runLogCaption.Name = "runLogCaption";
			runLogCaption.Size = new DrawingSize(1242, 22);
			runLogCaption.TabIndex = 0;
			runLogCaption.Text = "Run Log - Run Preview stays here; Publish Result updates workspace.";
			runLogCaption.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// tbRunLog
			// 
			tbRunLog.BackColor = Color.White;
			tbRunLog.BorderStyle = BorderStyle.FixedSingle;
			tbRunLog.Dock = DockStyle.Fill;
			tbRunLog.Location = new DrawingPoint(3, 25);
			tbRunLog.Multiline = true;
			tbRunLog.Name = "tbRunLog";
			tbRunLog.ReadOnly = true;
			tbRunLog.ScrollBars = ScrollBars.Vertical;
			tbRunLog.Size = new DrawingSize(1242, 86);
			tbRunLog.TabIndex = 1;
			// 
			// previewPanel
			// 
			previewPanel.ColumnCount = 1;
			previewPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			previewPanel.Controls.Add(previewCaption, 0, 0);
			previewPanel.Controls.Add(previewOptionsPanel, 0, 1);
			previewPanel.Controls.Add(previewBox, 0, 2);
			previewPanel.Controls.Add(matchingReviewPanel, 0, 3);
			previewPanel.Controls.Add(resultCaption, 0, 4);
			previewPanel.Controls.Add(resultGrid, 0, 5);
			previewPanel.Dock = DockStyle.Fill;
			previewPanel.Location = new DrawingPoint(871, 3);
			previewPanel.Name = "previewPanel";
			previewPanel.RowCount = 6;
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0F));
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 132F));
			previewPanel.Size = new DrawingSize(374, 494);
			previewPanel.TabIndex = 2;
			// 
			// previewCaption
			// 
			previewCaption.Dock = DockStyle.Fill;
			previewCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			previewCaption.ForeColor = Color.FromArgb(35, 85, 132);
			previewCaption.Location = new DrawingPoint(3, 0);
			previewCaption.Name = "previewCaption";
			previewCaption.Size = new DrawingSize(368, 22);
			previewCaption.TabIndex = 0;
			previewCaption.Text = "Preview";
			previewCaption.TextAlign = ContentAlignment.MiddleLeft;
			previewCaption.AutoEllipsis = true;
			// 
			// previewOptionsPanel
			// 
			previewOptionsPanel.BackColor = Color.FromArgb(238, 242, 246);
			previewOptionsPanel.Controls.Add(cbPreviewImageMode);
			previewOptionsPanel.Controls.Add(previewModeLabel);
			previewOptionsPanel.Controls.Add(overlayLabelModeLabel);
			previewOptionsPanel.Controls.Add(cbOverlayLabelMode);
			previewOptionsPanel.Controls.Add(overlayPointLimitLabel);
			previewOptionsPanel.Controls.Add(nudOverlayPointLimit);
			previewOptionsPanel.Controls.Add(chkOverlayRoi);
			previewOptionsPanel.Controls.Add(btnOpenPreview);
			previewOptionsPanel.Dock = DockStyle.Fill;
			previewOptionsPanel.Location = new DrawingPoint(3, 25);
			previewOptionsPanel.Name = "previewOptionsPanel";
			previewOptionsPanel.Size = new DrawingSize(368, 50);
			previewOptionsPanel.TabIndex = 1;
			// 
			// cbPreviewImageMode
			//
			cbPreviewImageMode.DropDownStyle = ComboBoxStyle.DropDownList;
			cbPreviewImageMode.FormattingEnabled = true;
			cbPreviewImageMode.FlatStyle = FlatStyle.Flat;
			cbPreviewImageMode.BackColor = Color.FromArgb(250, 252, 253);
			cbPreviewImageMode.ForeColor = Color.FromArgb(35, 85, 132);
			cbPreviewImageMode.Items.AddRange(new object[] { "Summary", "Input", "Output", "Overlay" });
			cbPreviewImageMode.Location = new DrawingPoint(0, 2);
			cbPreviewImageMode.Name = "cbPreviewImageMode";
			cbPreviewImageMode.Size = new DrawingSize(128, 24);
			cbPreviewImageMode.TabIndex = 0;
			cbPreviewImageMode.SelectedIndex = 0;
			cbPreviewImageMode.SelectedIndexChanged += OnOverlayOptionChanged;
			cbPreviewImageMode.Visible = true;
			// 
			// previewModeLabel
			// 
			previewModeLabel.BackColor = Color.FromArgb(47, 111, 171);
			previewModeLabel.BorderStyle = BorderStyle.FixedSingle;
			previewModeLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
			previewModeLabel.ForeColor = Color.White;
			previewModeLabel.Location = new DrawingPoint(0, 2);
			previewModeLabel.Name = "previewModeLabel";
			previewModeLabel.Size = new DrawingSize(128, 22);
			previewModeLabel.TabIndex = 0;
			previewModeLabel.Text = "Overlay";
			previewModeLabel.TextAlign = ContentAlignment.MiddleCenter;
			previewModeLabel.Visible = false;
			// 
			// overlayLabelModeLabel
			// 
			overlayLabelModeLabel.AutoSize = true;
			overlayLabelModeLabel.ForeColor = Color.FromArgb(35, 85, 132);
			overlayLabelModeLabel.Location = new DrawingPoint(0, 31);
			overlayLabelModeLabel.Name = "overlayLabelModeLabel";
			overlayLabelModeLabel.Size = new DrawingSize(36, 16);
			overlayLabelModeLabel.TabIndex = 1;
			overlayLabelModeLabel.Text = "Label";
			// 
			// cbOverlayLabelMode
			// 
			cbOverlayLabelMode.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOverlayLabelMode.FormattingEnabled = true;
			cbOverlayLabelMode.Items.AddRange(new object[] { "None", "No", "Details" });
			cbOverlayLabelMode.Location = new DrawingPoint(44, 28);
			cbOverlayLabelMode.Name = "cbOverlayLabelMode";
			cbOverlayLabelMode.Size = new DrawingSize(76, 24);
			cbOverlayLabelMode.TabIndex = 2;
			cbOverlayLabelMode.SelectedIndex = 1;
			cbOverlayLabelMode.SelectedIndexChanged += OnOverlayOptionChanged;
			// 
			// overlayPointLimitLabel
			// 
			overlayPointLimitLabel.AutoSize = true;
			overlayPointLimitLabel.ForeColor = Color.FromArgb(35, 85, 132);
			overlayPointLimitLabel.Location = new DrawingPoint(130, 31);
			overlayPointLimitLabel.Name = "overlayPointLimitLabel";
			overlayPointLimitLabel.Size = new DrawingSize(41, 16);
			overlayPointLimitLabel.TabIndex = 3;
			overlayPointLimitLabel.Text = "Points";
			// 
			// nudOverlayPointLimit
			// 
			nudOverlayPointLimit.Increment = new decimal(new int[] { 50, 0, 0, 0 });
			nudOverlayPointLimit.Location = new DrawingPoint(178, 28);
			nudOverlayPointLimit.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
			nudOverlayPointLimit.Name = "nudOverlayPointLimit";
			nudOverlayPointLimit.Size = new DrawingSize(58, 22);
			nudOverlayPointLimit.TabIndex = 4;
			nudOverlayPointLimit.Value = new decimal(new int[] { 300, 0, 0, 0 });
			nudOverlayPointLimit.ValueChanged += OnOverlayOptionChanged;
			// 
			// chkOverlayRoi
			// 
			chkOverlayRoi.AutoSize = true;
			chkOverlayRoi.Checked = true;
			chkOverlayRoi.CheckState = CheckState.Checked;
			chkOverlayRoi.ForeColor = Color.FromArgb(35, 85, 132);
			chkOverlayRoi.Location = new DrawingPoint(246, 31);
			chkOverlayRoi.Name = "chkOverlayRoi";
			chkOverlayRoi.Size = new DrawingSize(46, 20);
			chkOverlayRoi.TabIndex = 5;
			chkOverlayRoi.Text = "ROI";
			chkOverlayRoi.UseVisualStyleBackColor = true;
			chkOverlayRoi.CheckedChanged += OnOverlayOptionChanged;
			// 
			// btnOpenPreview
			// 
			btnOpenPreview.BackColor = Color.FromArgb(250, 252, 253);
			btnOpenPreview.FlatStyle = FlatStyle.Flat;
			btnOpenPreview.ForeColor = Color.FromArgb(35, 85, 132);
			btnOpenPreview.Location = new DrawingPoint(304, 2);
			btnOpenPreview.Name = "btnOpenPreview";
			btnOpenPreview.Size = new DrawingSize(32, 24);
			btnOpenPreview.TabIndex = 5;
			btnOpenPreview.Text = "...";
			btnOpenPreview.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			btnOpenPreview.UseVisualStyleBackColor = false;
			btnOpenPreview.Click += OnOpenPreviewClicked;
			// 
			// previewBox
			// 
			previewBox.BackColor = Color.Black;
			previewBox.BorderStyle = BorderStyle.FixedSingle;
			previewBox.Dock = DockStyle.Fill;
			previewBox.Location = new DrawingPoint(3, 81);
			previewBox.Name = "previewBox";
			previewBox.Size = new DrawingSize(368, 260);
			previewBox.SizeMode = PictureBoxSizeMode.Zoom;
			previewBox.TabIndex = 2;
			previewBox.TabStop = false;
			previewBox.DoubleClick += OnOpenPreviewClicked;
			// 
			// matchingReviewPanel
			// 
			matchingReviewPanel.BackColor = Color.FromArgb(247, 250, 253);
			matchingReviewPanel.BorderStyle = BorderStyle.FixedSingle;
			matchingReviewPanel.Controls.Add(matchingTemplateTitle);
			matchingReviewPanel.Controls.Add(matchingDetectedTitle);
			matchingReviewPanel.Controls.Add(matchingReviewTitle);
			matchingReviewPanel.Controls.Add(matchingTemplateBox);
			matchingReviewPanel.Controls.Add(matchingDetectedBox);
			matchingReviewPanel.Controls.Add(matchingReviewSummary);
			matchingReviewPanel.Dock = DockStyle.Fill;
			matchingReviewPanel.Location = new DrawingPoint(3, 347);
			matchingReviewPanel.Name = "matchingReviewPanel";
			matchingReviewPanel.Size = new DrawingSize(368, 0);
			matchingReviewPanel.TabIndex = 3;
			matchingReviewPanel.Visible = false;
			// 
			// matchingTemplateTitle
			// 
			matchingTemplateTitle.AutoSize = true;
			matchingTemplateTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
			matchingTemplateTitle.ForeColor = Color.FromArgb(35, 85, 132);
			matchingTemplateTitle.Location = new DrawingPoint(8, 6);
			matchingTemplateTitle.Name = "matchingTemplateTitle";
			matchingTemplateTitle.Size = new DrawingSize(56, 13);
			matchingTemplateTitle.TabIndex = 0;
			matchingTemplateTitle.Text = "Template";
			// 
			// matchingDetectedTitle
			// 
			matchingDetectedTitle.AutoSize = true;
			matchingDetectedTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
			matchingDetectedTitle.ForeColor = Color.FromArgb(35, 85, 132);
			matchingDetectedTitle.Location = new DrawingPoint(102, 6);
			matchingDetectedTitle.Name = "matchingDetectedTitle";
			matchingDetectedTitle.Size = new DrawingSize(79, 13);
			matchingDetectedTitle.TabIndex = 1;
			matchingDetectedTitle.Text = "Detected Crop";
			// 
			// matchingReviewTitle
			// 
			matchingReviewTitle.AutoSize = true;
			matchingReviewTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
			matchingReviewTitle.ForeColor = Color.FromArgb(35, 85, 132);
			matchingReviewTitle.Location = new DrawingPoint(198, 6);
			matchingReviewTitle.Name = "matchingReviewTitle";
			matchingReviewTitle.Size = new DrawingSize(43, 13);
			matchingReviewTitle.TabIndex = 2;
			matchingReviewTitle.Text = "Review";
			// 
			// matchingTemplateBox
			// 
			matchingTemplateBox.BackColor = Color.FromArgb(18, 20, 24);
			matchingTemplateBox.BorderStyle = BorderStyle.FixedSingle;
			matchingTemplateBox.Location = new DrawingPoint(8, 25);
			matchingTemplateBox.Name = "matchingTemplateBox";
			matchingTemplateBox.Size = new DrawingSize(84, 78);
			matchingTemplateBox.SizeMode = PictureBoxSizeMode.Zoom;
			matchingTemplateBox.TabIndex = 3;
			matchingTemplateBox.TabStop = false;
			matchingTemplateBox.Cursor = Cursors.Hand;
			matchingTemplateBox.DoubleClick += OnMatchingReviewImageDoubleClick;
			// 
			// matchingDetectedBox
			// 
			matchingDetectedBox.BackColor = Color.FromArgb(18, 20, 24);
			matchingDetectedBox.BorderStyle = BorderStyle.FixedSingle;
			matchingDetectedBox.Location = new DrawingPoint(102, 25);
			matchingDetectedBox.Name = "matchingDetectedBox";
			matchingDetectedBox.Size = new DrawingSize(84, 78);
			matchingDetectedBox.SizeMode = PictureBoxSizeMode.Zoom;
			matchingDetectedBox.TabIndex = 4;
			matchingDetectedBox.TabStop = false;
			matchingDetectedBox.Cursor = Cursors.Hand;
			matchingDetectedBox.DoubleClick += OnMatchingReviewImageDoubleClick;
			// 
			// matchingReviewSummary
			// 
			matchingReviewSummary.BackColor = Color.White;
			matchingReviewSummary.BorderStyle = BorderStyle.FixedSingle;
			matchingReviewSummary.Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point);
			matchingReviewSummary.ForeColor = Color.FromArgb(42, 54, 68);
			matchingReviewSummary.Location = new DrawingPoint(198, 25);
			matchingReviewSummary.Name = "matchingReviewSummary";
			matchingReviewSummary.Padding = new Padding(5);
			matchingReviewSummary.Size = new DrawingSize(136, 78);
			matchingReviewSummary.TabIndex = 5;
			matchingReviewSummary.Text = "Template: -\r\nCrop: -";
			// 
			// resultCaption
			// 
			resultCaption.Dock = DockStyle.Fill;
			resultCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			resultCaption.ForeColor = Color.FromArgb(35, 85, 132);
			resultCaption.Location = new DrawingPoint(3, 344);
			resultCaption.Name = "resultCaption";
			resultCaption.Size = new DrawingSize(368, 22);
			resultCaption.TabIndex = 3;
			resultCaption.Text = "Result Details";
			resultCaption.TextAlign = ContentAlignment.MiddleLeft;
			resultCaption.AutoEllipsis = true;
			// 
			// resultGrid
			// 
			resultGrid.AllowUserToAddRows = false;
			resultGrid.AllowUserToDeleteRows = false;
			resultGrid.AllowUserToResizeRows = false;
			resultGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			resultGrid.BackgroundColor = Color.White;
			resultGrid.BorderStyle = BorderStyle.FixedSingle;
			resultGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resultGrid.Dock = DockStyle.Fill;
			resultGrid.Location = new DrawingPoint(3, 365);
			resultGrid.MultiSelect = false;
			resultGrid.Name = "resultGrid";
			resultGrid.ReadOnly = true;
			resultGrid.RowHeadersVisible = false;
			resultGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			resultGrid.Size = new DrawingSize(368, 126);
			resultGrid.TabIndex = 4;
			resultGrid.Columns.Add("Item", "Item");
			resultGrid.Columns.Add("Value", "Value");
			resultGrid.Columns[0].FillWeight = 28F;
			resultGrid.Columns[1].FillWeight = 72F;
			resultGrid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
			resultGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 248, 252);
			resultGrid.CellClick += OnResultGridCellClick;
			resultGrid.CellDoubleClick += OnResultGridCellDoubleClick;
			// 
			// footerPanel
			// 
			footerPanel.Controls.Add(btnRemove);
			footerPanel.Controls.Add(btnUp);
			footerPanel.Controls.Add(btnDown);
			footerPanel.Controls.Add(chkPublishAllLayers);
			footerPanel.Controls.Add(workflowHintLabel);
			footerPanel.Controls.Add(btnMore);
			footerPanel.Controls.Add(btnValidate);
			footerPanel.Controls.Add(btnSave);
			footerPanel.Controls.Add(btnRun);
			footerPanel.Controls.Add(btnPublish);
			footerPanel.Controls.Add(btnCancel);
			footerPanel.Dock = DockStyle.Fill;
			footerPanel.Location = new DrawingPoint(15, 683);
			footerPanel.Name = "footerPanel";
			footerPanel.Size = new DrawingSize(1248, 40);
			footerPanel.TabIndex = 2;
			footerPanel.Resize += (sender, e) => LayoutFooterButtons();
			// 
			// btnRemove
			// 
			btnRemove.BackColor = Color.FromArgb(250, 252, 253);
			btnRemove.FlatStyle = FlatStyle.Flat;
			btnRemove.ForeColor = Color.FromArgb(35, 85, 132);
			btnRemove.Location = new DrawingPoint(0, 8);
			btnRemove.Name = "btnRemove";
			btnRemove.Size = new DrawingSize(76, 28);
			btnRemove.TabIndex = 0;
			btnRemove.Text = "Remove";
			btnRemove.UseVisualStyleBackColor = false;
			btnRemove.Click += OnRemoveClicked;
			// 
			// btnUp
			// 
			btnUp.BackColor = Color.FromArgb(250, 252, 253);
			btnUp.FlatStyle = FlatStyle.Flat;
			btnUp.ForeColor = Color.FromArgb(35, 85, 132);
			btnUp.Location = new DrawingPoint(82, 8);
			btnUp.Name = "btnUp";
			btnUp.Size = new DrawingSize(70, 28);
			btnUp.TabIndex = 1;
			btnUp.Text = "Up";
			btnUp.UseVisualStyleBackColor = false;
			btnUp.Click += OnUpClicked;
			// 
			// btnDown
			// 
			btnDown.BackColor = Color.FromArgb(250, 252, 253);
			btnDown.FlatStyle = FlatStyle.Flat;
			btnDown.ForeColor = Color.FromArgb(35, 85, 132);
			btnDown.Location = new DrawingPoint(158, 8);
			btnDown.Name = "btnDown";
			btnDown.Size = new DrawingSize(76, 28);
			btnDown.TabIndex = 2;
			btnDown.Text = "Down";
			btnDown.UseVisualStyleBackColor = false;
			btnDown.Click += OnDownClicked;
			// 
			// chkPublishAllLayers
			// 
			chkPublishAllLayers.AutoSize = true;
			chkPublishAllLayers.Location = new DrawingPoint(242, 14);
			chkPublishAllLayers.Name = "chkPublishAllLayers";
			chkPublishAllLayers.Size = new DrawingSize(114, 20);
			chkPublishAllLayers.TabIndex = 3;
			chkPublishAllLayers.Text = "Publish all";
			chkPublishAllLayers.UseVisualStyleBackColor = true;
			// 
			// workflowHintLabel
			// 
			workflowHintLabel.AutoEllipsis = true;
			workflowHintLabel.BackColor = Color.FromArgb(232, 241, 250);
			workflowHintLabel.BorderStyle = BorderStyle.FixedSingle;
			workflowHintLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
			workflowHintLabel.ForeColor = Color.FromArgb(35, 85, 132);
			workflowHintLabel.Location = new DrawingPoint(362, 9);
			workflowHintLabel.Name = "workflowHintLabel";
			workflowHintLabel.Padding = new Padding(8, 0, 8, 0);
			workflowHintLabel.Size = new DrawingSize(530, 26);
			workflowHintLabel.TabIndex = 14;
			workflowHintLabel.Text = "Preview only -> Publish workspace";
			workflowHintLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// btnMore
			// 
			btnMore.BackColor = Color.FromArgb(250, 252, 253);
			btnMore.FlatStyle = FlatStyle.Flat;
			btnMore.ForeColor = Color.FromArgb(35, 85, 132);
			btnMore.Location = new DrawingPoint(390, 8);
			btnMore.Name = "btnMore";
			btnMore.Size = new DrawingSize(82, 28);
			btnMore.TabIndex = 4;
			btnMore.Text = "More";
			btnMore.UseVisualStyleBackColor = false;
			btnMore.Click += OnMoreClicked;
			// 
			// btnHistory
			// 
			btnHistory.BackColor = Color.FromArgb(250, 252, 253);
			btnHistory.FlatStyle = FlatStyle.Flat;
			btnHistory.ForeColor = Color.FromArgb(35, 85, 132);
			btnHistory.Location = new DrawingPoint(390, 8);
			btnHistory.Name = "btnHistory";
			btnHistory.Size = new DrawingSize(72, 28);
			btnHistory.TabIndex = 4;
			btnHistory.Text = "History";
			btnHistory.UseVisualStyleBackColor = false;
			btnHistory.Click += OnHistoryClicked;
			// 
			// btnSamples
			// 
			btnSamples.BackColor = Color.FromArgb(250, 252, 253);
			btnSamples.FlatStyle = FlatStyle.Flat;
			btnSamples.ForeColor = Color.FromArgb(35, 85, 132);
			btnSamples.Location = new DrawingPoint(468, 8);
			btnSamples.Name = "btnSamples";
			btnSamples.Size = new DrawingSize(76, 28);
			btnSamples.TabIndex = 5;
			btnSamples.Text = "Samples";
			btnSamples.UseVisualStyleBackColor = false;
			btnSamples.Click += OnSamplesClicked;
			// 
			// btnBatch
			// 
			btnBatch.BackColor = Color.FromArgb(250, 252, 253);
			btnBatch.FlatStyle = FlatStyle.Flat;
			btnBatch.ForeColor = Color.FromArgb(35, 85, 132);
			btnBatch.Location = new DrawingPoint(550, 8);
			btnBatch.Name = "btnBatch";
			btnBatch.Size = new DrawingSize(70, 28);
			btnBatch.TabIndex = 6;
			btnBatch.Text = "Batch";
			btnBatch.UseVisualStyleBackColor = false;
			btnBatch.Click += OnBatchClicked;
			// 
			// btnImport
			// 
			btnImport.BackColor = Color.FromArgb(250, 252, 253);
			btnImport.FlatStyle = FlatStyle.Flat;
			btnImport.ForeColor = Color.FromArgb(35, 85, 132);
			btnImport.Location = new DrawingPoint(626, 8);
			btnImport.Name = "btnImport";
			btnImport.Size = new DrawingSize(76, 28);
			btnImport.TabIndex = 7;
			btnImport.Text = "Import";
			btnImport.UseVisualStyleBackColor = false;
			btnImport.Click += OnImportClicked;
			// 
			// btnValidate
			// 
			btnValidate.BackColor = Color.FromArgb(250, 252, 253);
			btnValidate.FlatStyle = FlatStyle.Flat;
			btnValidate.ForeColor = Color.FromArgb(35, 85, 132);
			btnValidate.Location = new DrawingPoint(708, 8);
			btnValidate.Name = "btnValidate";
			btnValidate.Size = new DrawingSize(76, 28);
			btnValidate.TabIndex = 8;
			btnValidate.Text = "Check";
			btnValidate.UseVisualStyleBackColor = false;
			btnValidate.Click += OnValidateClicked;
			// 
			// btnLoad
			// 
			btnLoad.BackColor = Color.FromArgb(250, 252, 253);
			btnLoad.FlatStyle = FlatStyle.Flat;
			btnLoad.ForeColor = Color.FromArgb(35, 85, 132);
			btnLoad.Location = new DrawingPoint(790, 8);
			btnLoad.Name = "btnLoad";
			btnLoad.Size = new DrawingSize(68, 28);
			btnLoad.TabIndex = 9;
			btnLoad.Text = "Load";
			btnLoad.UseVisualStyleBackColor = false;
			btnLoad.Click += OnLoadClicked;
			// 
			// btnSave
			// 
			btnSave.BackColor = Color.FromArgb(250, 252, 253);
			btnSave.FlatStyle = FlatStyle.Flat;
			btnSave.ForeColor = Color.FromArgb(35, 85, 132);
			btnSave.Location = new DrawingPoint(864, 8);
			btnSave.Name = "btnSave";
			btnSave.Size = new DrawingSize(68, 28);
			btnSave.TabIndex = 10;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = false;
			btnSave.Click += OnSaveClicked;
			// 
			// btnRun
			// 
			btnRun.BackColor = Color.FromArgb(250, 252, 253);
			btnRun.FlatStyle = FlatStyle.Flat;
			btnRun.ForeColor = Color.FromArgb(35, 85, 132);
			btnRun.Location = new DrawingPoint(938, 8);
			btnRun.Name = "btnRun";
			btnRun.Size = new DrawingSize(68, 28);
			btnRun.TabIndex = 11;
			btnRun.Text = "Run";
			btnRun.UseVisualStyleBackColor = false;
			btnRun.Click += OnRunClicked;
			// 
			// btnPublish
			// 
			btnPublish.BackColor = Color.FromArgb(250, 252, 253);
			btnPublish.FlatStyle = FlatStyle.Flat;
			btnPublish.ForeColor = Color.FromArgb(35, 85, 132);
			btnPublish.Location = new DrawingPoint(1012, 8);
			btnPublish.Name = "btnPublish";
			btnPublish.Size = new DrawingSize(86, 28);
			btnPublish.TabIndex = 12;
			btnPublish.Text = "Publish";
			btnPublish.UseVisualStyleBackColor = false;
			btnPublish.Click += OnPublishClicked;
			// 
			// btnCancel
			// 
			btnCancel.BackColor = Color.FromArgb(250, 252, 253);
			btnCancel.Enabled = false;
			btnCancel.FlatStyle = FlatStyle.Flat;
			btnCancel.ForeColor = Color.FromArgb(35, 85, 132);
			btnCancel.Location = new DrawingPoint(1104, 8);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new DrawingSize(76, 28);
			btnCancel.TabIndex = 13;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = false;
			btnCancel.Click += OnCancelClicked;
			// 
			// FormVision_Pipeline
			// 
			_DesktopPanelSize = false;
			BackColor = Color.FromArgb(238, 242, 246);
			BorderColor = Color.FromArgb(90, 146, 246);
			BorderSize = 1;
			Caption = "Pipeline";
			ClientSize = new DrawingSize(1280, 780);
			KeyPreview = true;
			MinimumSize = new DrawingSize(1180, 720);
			Name = "FormVision_Pipeline";
			Padding = new Padding(1);
			StartPosition = FormStartPosition.CenterParent;
			Text = "Pipeline";
			KeyDown += FormVisionPipeline_KeyDown;
			pnlClientArea.ResumeLayout(false);
			rootLayout.ResumeLayout(false);
			headerPanel.ResumeLayout(false);
			headerPanel.PerformLayout();
			bodyLayout.ResumeLayout(false);
			stepTreePanel.ResumeLayout(false);
			editorPanel.ResumeLayout(false);
			stepAcceptancePanel.ResumeLayout(false);
			runLogPanel.ResumeLayout(false);
			runLogPanel.PerformLayout();
			previewPanel.ResumeLayout(false);
			previewOptionsPanel.ResumeLayout(false);
			previewOptionsPanel.PerformLayout();
			((ISupportInitialize)nudOverlayPointLimit).EndInit();
			((ISupportInitialize)previewBox).EndInit();
			matchingReviewPanel.ResumeLayout(false);
			matchingReviewPanel.PerformLayout();
			((ISupportInitialize)matchingTemplateBox).EndInit();
			((ISupportInitialize)matchingDetectedBox).EndInit();
			((ISupportInitialize)resultGrid).EndInit();
			footerPanel.ResumeLayout(false);
			ConfigurePipelineButtonIcons();
			ResumeLayout(false);
		}
    }
}
