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
			btnOpenPreview = new Button();
			previewBox = new PictureBox();
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
			pipelineToolTip = new ToolTip();
			pnlClientArea.SuspendLayout();
			rootLayout.SuspendLayout();
			headerPanel.SuspendLayout();
			bodyLayout.SuspendLayout();
			stepTreePanel.SuspendLayout();
			editorPanel.SuspendLayout();
			runLogPanel.SuspendLayout();
			previewPanel.SuspendLayout();
			((ISupportInitialize)nudOverlayPointLimit).BeginInit();
			((ISupportInitialize)previewBox).BeginInit();
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
			cbToolType.Items.AddRange(new object[] { "Threshold", "Morphology", "Filter", "EdgeDetection", "Blob", "Contour", "LineGauge", "Matching", "FeatureMatching", "Mean" });
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
			editorPanel.Controls.Add(propertyGridHost, 0, 2);
			editorPanel.Dock = DockStyle.Fill;
			editorPanel.Location = new DrawingPoint(343, 3);
			editorPanel.Name = "editorPanel";
			editorPanel.RowCount = 3;
			editorPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			editorPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 94F));
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
			btnStepChainInput.Text = "Chain";
			btnStepChainInput.UseVisualStyleBackColor = false;
			btnStepChainInput.Click += OnChainSelectedStepInputClicked;
			// 
			// stepIoStatusLabel
			// 
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
			// propertyGridHost
			// 
			propertyGridHost.BackColor = Color.White;
			propertyGridHost.Dock = DockStyle.Fill;
			propertyGridHost.Location = new DrawingPoint(3, 119);
			propertyGridHost.Name = "propertyGridHost";
			propertyGridHost.Size = new DrawingSize(516, 372);
			propertyGridHost.TabIndex = 2;
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
			previewPanel.Controls.Add(resultCaption, 0, 3);
			previewPanel.Controls.Add(resultGrid, 0, 4);
			previewPanel.Dock = DockStyle.Fill;
			previewPanel.Location = new DrawingPoint(871, 3);
			previewPanel.Name = "previewPanel";
			previewPanel.RowCount = 5;
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
			previewPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
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
			previewOptionsPanel.Controls.Add(btnOpenPreview);
			previewOptionsPanel.Dock = DockStyle.Fill;
			previewOptionsPanel.Location = new DrawingPoint(3, 25);
			previewOptionsPanel.Name = "previewOptionsPanel";
			previewOptionsPanel.Size = new DrawingSize(368, 24);
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
			cbPreviewImageMode.Size = new DrawingSize(96, 24);
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
			previewModeLabel.Size = new DrawingSize(96, 22);
			previewModeLabel.TabIndex = 0;
			previewModeLabel.Text = "Overlay";
			previewModeLabel.TextAlign = ContentAlignment.MiddleCenter;
			previewModeLabel.Visible = false;
			// 
			// overlayLabelModeLabel
			// 
			overlayLabelModeLabel.AutoSize = true;
			overlayLabelModeLabel.ForeColor = Color.FromArgb(35, 85, 132);
			overlayLabelModeLabel.Location = new DrawingPoint(104, 5);
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
			cbOverlayLabelMode.Location = new DrawingPoint(142, 2);
			cbOverlayLabelMode.Name = "cbOverlayLabelMode";
			cbOverlayLabelMode.Size = new DrawingSize(72, 24);
			cbOverlayLabelMode.TabIndex = 2;
			cbOverlayLabelMode.SelectedIndex = 1;
			cbOverlayLabelMode.SelectedIndexChanged += OnOverlayOptionChanged;
			// 
			// overlayPointLimitLabel
			// 
			overlayPointLimitLabel.AutoSize = true;
			overlayPointLimitLabel.ForeColor = Color.FromArgb(35, 85, 132);
			overlayPointLimitLabel.Location = new DrawingPoint(222, 5);
			overlayPointLimitLabel.Name = "overlayPointLimitLabel";
			overlayPointLimitLabel.Size = new DrawingSize(41, 16);
			overlayPointLimitLabel.TabIndex = 3;
			overlayPointLimitLabel.Text = "Points";
			// 
			// nudOverlayPointLimit
			// 
			nudOverlayPointLimit.Increment = new decimal(new int[] { 50, 0, 0, 0 });
			nudOverlayPointLimit.Location = new DrawingPoint(264, 2);
			nudOverlayPointLimit.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
			nudOverlayPointLimit.Name = "nudOverlayPointLimit";
			nudOverlayPointLimit.Size = new DrawingSize(54, 22);
			nudOverlayPointLimit.TabIndex = 4;
			nudOverlayPointLimit.Value = new decimal(new int[] { 300, 0, 0, 0 });
			nudOverlayPointLimit.ValueChanged += OnOverlayOptionChanged;
			// 
			// btnOpenPreview
			// 
			btnOpenPreview.BackColor = Color.FromArgb(250, 252, 253);
			btnOpenPreview.FlatStyle = FlatStyle.Flat;
			btnOpenPreview.ForeColor = Color.FromArgb(35, 85, 132);
			btnOpenPreview.Location = new DrawingPoint(320, 1);
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
			previewBox.Location = new DrawingPoint(3, 55);
			previewBox.Name = "previewBox";
			previewBox.Size = new DrawingSize(368, 286);
			previewBox.SizeMode = PictureBoxSizeMode.Zoom;
			previewBox.TabIndex = 2;
			previewBox.TabStop = false;
			previewBox.DoubleClick += OnOpenPreviewClicked;
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
			runLogPanel.ResumeLayout(false);
			runLogPanel.PerformLayout();
			previewPanel.ResumeLayout(false);
			previewOptionsPanel.ResumeLayout(false);
			previewOptionsPanel.PerformLayout();
			((ISupportInitialize)nudOverlayPointLimit).EndInit();
			((ISupportInitialize)previewBox).EndInit();
			((ISupportInitialize)resultGrid).EndInit();
			footerPanel.ResumeLayout(false);
			ConfigurePipelineButtonIcons();
			ResumeLayout(false);
		}
    }
}
