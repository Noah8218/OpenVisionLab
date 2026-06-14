using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using OpenVisionLab.Logging;
using OpenVisionLab.MessageDialogs;
using OpenVisionLab.Pipeline.Controls;
using OpenVisionLab.PropertyGrid;
using RJCodeUI_M1.RJForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;

namespace OpenVisionLab
{
    public partial class FormVision_Pipeline : RJChildForm
    {
        private enum PipelineStepRunStatus
        {
            NotRun,
            Running,
            Passed,
            Failed,
            Timeout,
            Canceled,
            Skipped,
            Loaded
        }

        internal enum OverlayLabelMode
        {
            None,
            Number,
            Details
        }

        private enum StepPreviewImageMode
        {
            Summary,
            Input,
            Output,
            Overlay
        }

        private enum ResultGridRowAction
        {
            None,
            Metrics,
            Overlays,
            Overlay,
            PreviewInput,
            PreviewOutput,
            PreviewOverlay
        }

        private sealed class ResultGridRowTag
        {
            public ResultGridRowTag(ResultGridRowAction action, int overlayIndex = -1)
            {
                Action = action;
                OverlayIndex = overlayIndex;
            }

            public ResultGridRowAction Action { get; }
            public int OverlayIndex { get; }
        }

        private readonly IDisplayManager displayManager;
        private readonly string recipeName;
        private readonly PropertyGridEventBinder propertyGridVisibilityBinder;
        private readonly Dictionary<VisionPipelineStep, Bitmap> stepInputImages = new Dictionary<VisionPipelineStep, Bitmap>();
        private readonly Dictionary<VisionPipelineStep, Bitmap> stepResultImages = new Dictionary<VisionPipelineStep, Bitmap>();
        private readonly Dictionary<VisionPipelineStep, Bitmap> stepPreviewImages = new Dictionary<VisionPipelineStep, Bitmap>();
        private readonly Dictionary<VisionPipelineStep, PipelineStepRunStatus> stepStatuses = new Dictionary<VisionPipelineStep, PipelineStepRunStatus>();
        private readonly Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary> stepResultSummaries = new Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary>();
        private readonly Dictionary<VisionPipelineStep, List<VisionToolOverlay>> stepOverlays = new Dictionary<VisionPipelineStep, List<VisionToolOverlay>>();
        private readonly Dictionary<string, Bitmap> previewLayerImages = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);
        private const int StepTimeoutMilliseconds = 60000;
        private const string StepPreviewDirectoryName = "StepPreviews";
        private const string StepPreviewManifestFileName = "steps.tsv";
        private const string PipelineSummaryOutputLayerName = "Pipeline_Output";
        private VisionPipeline pipeline = new VisionPipeline { Name = "Pipeline" };
        private StepPreviewImageMode currentPreviewImageMode = StepPreviewImageMode.Summary;
        private Bitmap pipelineSummarySourceImage;
        private Bitmap pipelineSummaryPreviewImage;
        private readonly List<VisionToolOverlay> pipelineSummaryOverlays = new List<VisionToolOverlay>();
        private bool isUpdatingPreviewImageMode;

        private TextBox tbPipelineName;
        private ComboBox cbToolType;
        private ComboBox cbInputLayer;
        private TextBox tbOutputLayer;
        private TreeView treeSteps;
        private System.Windows.Forms.Integration.ElementHost pipelineFlowHost;
        private PipelineFlowView pipelineFlowView;
        private Label flowPreviewCaption;
        private TextBox tbFlowPreview;
        private System.Windows.Forms.Integration.ElementHost propertyGridHost;
        private IPropertyGridView propertyGridStep;
        private PictureBox previewBox;
        private Label previewCaption;
        private Panel previewOptionsPanel;
        private ComboBox cbPreviewImageMode;
        private Label previewModeLabel;
        private Label overlayLabelModeLabel;
        private ComboBox cbOverlayLabelMode;
        private Label overlayPointLimitLabel;
        private NumericUpDown nudOverlayPointLimit;
        private Button btnOpenPreview;
        private Label resultCaption;
        private DataGridView resultGrid;
        private TextBox tbRunLog;
        private Button btnAdd;
        private Button btnAiRecipe;
        private Button btnRemove;
        private Button btnUp;
        private Button btnDown;
        private Button btnLoad;
        private Button btnSave;
        private Button btnRun;
        private Button btnPublish;
        private Button btnCancel;
        private Button btnMore;
        private Button btnHistory;
        private Button btnSamples;
        private Button btnBatch;
        private Button btnImport;
        private Button btnValidate;
        private CheckBox chkPublishAllLayers;
        private ContextMenuStrip moreCommandMenu;
        private ContextMenuStrip stepContextMenu;
        private ToolTip pipelineToolTip;
        private ToolStripMenuItem menuAddAfter;
        private ToolStripMenuItem menuInsertBefore;
        private ToolStripMenuItem menuAcceptancePreset;
        private ToolStripMenuItem menuClearAcceptance;
        private ToolStripMenuItem menuChainStepInput;
        private ToolStripMenuItem menuMoreChainStepInput;
        private ToolStripMenuItem menuCopyStep;
        private ToolStripMenuItem menuPasteAfter;
        private ToolStripMenuItem menuDuplicateStep;
        private ToolStripMenuItem menuToggleStepEnabled;
        private ToolStripMenuItem menuRemoveStep;
        private VisionPipelineStep copiedStep;
        private object selectedStepProperty;
        private bool isBindingStepProperty;
        private bool isUpdatingHeaderOutputLayer;
        private bool userEditedHeaderOutputLayer;
        private string lastSuggestedHeaderOutputLayer = "Pipeline_Output";
        private bool isRunningPipeline;
        private CancellationTokenSource runCancellationSource;
        private TableLayoutPanel rootLayout;
        private Panel headerPanel;
        private Label nameLabel;
        private Label toolTypeLabel;
        private Label inputLayerLabel;
        private Label outputLayerLabel;
        private TableLayoutPanel bodyLayout;
        private TableLayoutPanel stepTreePanel;
        private Label stepTreeCaption;
        private TableLayoutPanel editorPanel;
        private Label propertiesCaption;
        private TableLayoutPanel stepIoPanel;
        private Label stepInputCaption;
        private ComboBox cbStepInputLayer;
        private Label stepFlowArrowLabel;
        private Label stepOutputCaption;
        private TextBox tbStepOutputLayer;
        private Button btnStepChainInput;
        private Label stepIoStatusLabel;
        private TableLayoutPanel runLogPanel;
        private Label runLogCaption;
        private TableLayoutPanel previewPanel;
        private Panel footerPanel;
        private bool isUpdatingStepIoPanel;

        public FormVision_Pipeline()
            : this(DisplayManagerService.Default, "Default")
        {
        }

        public FormVision_Pipeline(IDisplayManager displayManager, string recipeName)
        {
            this.displayManager = displayManager ?? DisplayManagerService.Default;
            this.recipeName = string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName;
            propertyGridVisibilityBinder = new PropertyGridEventBinder(() => this.displayManager);

            bool designTime = IsPipelineDesignTime();
            if (!designTime)
            {
                PropertyGridEditorFactory.SetRuntimeContext(() => this.displayManager);
                PropertyGridEditorFactory.SetRecipeNameContext(() => this.recipeName);
                VisionPipelineStepPropertyMapper.SetLayerNameContext(GetPipelineLayerTitles);
                string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(this.recipeName, VisionPipelineAppendService.DefaultPipelineName);
                pipeline = VisionPipelineStorage.Load(this.recipeName, activePipelineName);
            }

            InitializeComponent();
            if (designTime)
            {
                return;
            }

            InitializeStepContextMenu();
            InitializePipelineMoreMenu();
            InitializePipelinePropertyGridHost();
            InitializePipelineFlowHost();
            ConfigurePipelineToolTips();
            SetCurrentPreviewImageMode(StepPreviewImageMode.Summary, refresh: false);
            SetRunLogHint();
            LoadLayerItems();
            ResetHeaderOutputSuggestion();
            NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            BindPipeline();
            LoadPipelineImages(restoreLayerImages: false);
            AppendActivePipelineLog("OPEN");
            RunPipelineUiSmokeCheck();
        }


		protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearStepPreviewImages();
            base.OnFormClosed(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isRunningPipeline)
            {
                e.Cancel = true;
                AppendLog("CANCEL | Close requested. Waiting for the current run to stop.");
                runCancellationSource?.Cancel();
                btnCancel.Enabled = false;
                return;
            }

            base.OnFormClosing(e);
        }

        private void InitializePipelinePropertyGridHost()
        {
            if (propertyGridStep != null || propertyGridHost == null)
            {
                return;
            }

            var wpfPropertyGrid = new System.Windows.Controls.WpfPropertyGrid.PropertyGrid
            {
                Layout = new System.Windows.Controls.WpfPropertyGrid.Design.CategorizedLayout()
            };

            propertyGridHost.Child = wpfPropertyGrid;
            propertyGridStep = wpfPropertyGrid;
            propertyGridStep.ApplyDisplayOptions(PropertyGridDisplayOptions.Pipeline);
            propertyGridStep.SelectedObjectsChanged += propertyGridVisibilityBinder.Wpg_SelectedObjectsChanged;
            propertyGridStep.PropertyValueChanged += propertyGridVisibilityBinder.Wpg_PropertyValueChanged;
            propertyGridStep.PropertyValueChanged += OnStepPropertyValueChanged;
        }

        private void InitializePipelineFlowHost()
        {
            if (pipelineFlowView != null || pipelineFlowHost == null)
            {
                return;
            }

            pipelineFlowView = new PipelineFlowView();
            pipelineFlowView.StepSelected += OnPipelineFlowStepSelected;
            pipelineFlowHost.Child = pipelineFlowView;
        }

        private void OnPipelineFlowStepSelected(object sender, PipelineFlowStepSelectedEventArgs e)
        {
            if (e.Index < 0 || pipeline?.Steps == null || e.Index >= pipeline.Steps.Count)
            {
                return;
            }

            VisionPipelineStep step = pipeline.Steps[e.Index];
            SetCurrentPreviewImageMode(ToStepPreviewImageMode(e.Mode), refresh: false);
            SelectStep(step);
            if (ReferenceEquals(SelectedStep, step))
            {
                BindStepProperty(step);
                ShowStepPreview(step);
                RefreshPipelineFlowPreview();
            }
        }

        private void InitializeStepContextMenu()
        {
            if (treeSteps == null || stepContextMenu != null)
            {
                return;
            }

            stepContextMenu = new ContextMenuStrip();
            menuAddAfter = new ToolStripMenuItem("Add Step After...", null, OnAddAfterSelectedClicked);
            menuInsertBefore = new ToolStripMenuItem("Insert Step Before...", null, OnInsertBeforeSelectedClicked);
            menuAcceptancePreset = new ToolStripMenuItem("Acceptance Preset");
            foreach (VisionPipelineAcceptancePreset preset in VisionPipelineKnownMetrics.GetPresets())
            {
                VisionPipelineAcceptancePreset capturedPreset = preset;
                menuAcceptancePreset.DropDownItems.Add(new ToolStripMenuItem(
                    preset.Name,
                    null,
                    (sender, e) => ApplyAcceptancePreset(capturedPreset)));
            }

            menuClearAcceptance = new ToolStripMenuItem("Clear Acceptance", null, OnClearAcceptanceClicked);
            menuChainStepInput = new ToolStripMenuItem("Use Previous Output as Input", null, OnChainSelectedStepInputClicked);
            menuCopyStep = new ToolStripMenuItem("Copy Step", null, OnCopyStepClicked);
            menuPasteAfter = new ToolStripMenuItem("Paste Step After", null, OnPasteAfterSelectedClicked);
            menuDuplicateStep = new ToolStripMenuItem("Duplicate Step", null, OnDuplicateStepClicked);
            menuToggleStepEnabled = new ToolStripMenuItem("Disable Step", null, OnToggleStepEnabledClicked);
            menuRemoveStep = new ToolStripMenuItem("Remove Step", null, OnRemoveClicked);

            stepContextMenu.Items.AddRange(new ToolStripItem[]
            {
                menuAddAfter,
                menuInsertBefore,
                new ToolStripSeparator(),
                menuAcceptancePreset,
                menuClearAcceptance,
                new ToolStripSeparator(),
                menuChainStepInput,
                new ToolStripSeparator(),
                menuCopyStep,
                menuPasteAfter,
                menuDuplicateStep,
                new ToolStripSeparator(),
                menuToggleStepEnabled,
                menuRemoveStep
            });
            stepContextMenu.Opening += OnStepContextMenuOpening;
            treeSteps.ContextMenuStrip = stepContextMenu;
            treeSteps.NodeMouseClick += OnTreeStepNodeMouseClick;
        }

        private void InitializePipelineMoreMenu()
        {
            if (moreCommandMenu != null)
            {
                return;
            }

            moreCommandMenu = new ContextMenuStrip();
            moreCommandMenu.Items.Add(new ToolStripMenuItem("Run History", null, OnHistoryClicked));
            moreCommandMenu.Items.Add(new ToolStripMenuItem("Sample Images", null, OnSamplesClicked));
            moreCommandMenu.Items.Add(new ToolStripMenuItem("Batch Test", null, OnBatchClicked));
            moreCommandMenu.Items.Add(new ToolStripMenuItem("AI Recipe...", null, OnAiRecipeClicked));
            moreCommandMenu.Items.Add(new ToolStripSeparator());
            menuMoreChainStepInput = new ToolStripMenuItem("Use Previous Output as Input", null, OnChainSelectedStepInputClicked);
            moreCommandMenu.Items.Add(menuMoreChainStepInput);
            moreCommandMenu.Items.Add(new ToolStripSeparator());
            moreCommandMenu.Items.Add(new ToolStripMenuItem("Import XML...", null, OnImportClicked));
            moreCommandMenu.Items.Add(new ToolStripMenuItem("Load Saved...", null, OnLoadClicked));
            moreCommandMenu.Opening += OnMoreCommandMenuOpening;
        }

        private void FormVisionPipeline_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (isRunningPipeline)
                {
                    OnCancelClicked(sender, e);
                    e.Handled = true;
                    return;
                }

                Close();
            }
        }

        private void OnPipelineNameTextChanged(object sender, EventArgs e)
        {
            pipeline.Name = string.IsNullOrWhiteSpace(tbPipelineName.Text) ? "Pipeline" : tbPipelineName.Text.Trim();
        }

        private static bool IsPipelineDesignTime()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return true;
            }

            string processName = Process.GetCurrentProcess().ProcessName;
            return processName.IndexOf("devenv", StringComparison.OrdinalIgnoreCase) >= 0
                || processName.IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RunPipelineUiSmokeCheck()
        {
            List<string> failures = new List<string>();
            if (rootLayout == null || bodyLayout == null || footerPanel == null)
            {
                failures.Add("layout");
            }

            if (treeSteps == null || pipelineFlowHost == null || pipelineFlowView == null || propertyGridStep == null || previewBox == null || resultGrid == null)
            {
                failures.Add("main controls");
            }

            if (stepContextMenu == null || menuAcceptancePreset == null || menuAcceptancePreset.DropDownItems.Count == 0)
            {
                failures.Add("step context menu");
            }

            if (btnMore == null || moreCommandMenu == null || btnHistory == null || btnSamples == null || btnBatch == null || btnImport == null || btnValidate == null || btnRun == null || btnPublish == null || btnCancel == null || btnAiRecipe == null)
            {
                failures.Add("footer commands");
            }

            if (nameLabel == null || toolTypeLabel == null || inputLayerLabel == null || outputLayerLabel == null || cbToolType == null || cbToolType.Items.Count == 0 || cbInputLayer == null)
            {
                failures.Add("header inputs");
            }

            if (failures.Count > 0)
            {
                AppendLog($"SMOKE NG | {string.Join(", ", failures)}");
            }
        }

        private static Button CreateButton(string text, int x, int y, EventHandler click)
        {
            Button button = new Button
            {
                Text = text,
                Location = new DrawingPoint(x, y),
                Size = new DrawingSize(88, 28),
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            button.ForeColor = Color.FromArgb(35, 85, 132);
            button.BackColor = Color.FromArgb(250, 252, 253);
            button.Click += click;
            return button;
        }

        private void ConfigurePipelineButtonIcons()
        {
            Color actionColor = Color.FromArgb(35, 85, 132);
            Color dangerColor = Color.FromArgb(185, 52, 52);

            ApplyMaterialButtonIcon(btnAdd, "\u002B", "Add Step", actionColor, 128);
            ApplyMaterialButtonIcon(btnAiRecipe, "AI", "Recipe", actionColor, 112);
            ApplyMaterialButtonIcon(btnRemove, "\u2715", "Remove", dangerColor, 96);
            ApplyMaterialButtonIcon(btnUp, "\u25B2", "Up", actionColor, 72);
            ApplyMaterialButtonIcon(btnDown, "\u25BC", "Down", actionColor, 84);
            ApplyMaterialButtonIcon(btnMore, "...", "More", actionColor, 88);
            ApplyMaterialButtonIcon(btnHistory, "\u21BA", "History", actionColor, 92);
            ApplyMaterialButtonIcon(btnSamples, "\u25A7", "Samples", actionColor, 100);
            ApplyMaterialButtonIcon(btnBatch, "\u25A6", "Batch", actionColor, 84);
            ApplyMaterialButtonIcon(btnImport, "\u21E9", "Import", actionColor, 90);
            ApplyMaterialButtonIcon(btnValidate, "\u2713", "Check", actionColor, 84);
            ApplyMaterialButtonIcon(btnLoad, "\u25A3", "Load", actionColor, 78);
            ApplyMaterialButtonIcon(btnSave, "\u25A3", "Save Project", actionColor, 112);
            ApplyMaterialButtonIcon(btnRun, "\u25B6", "Run Preview", Color.FromArgb(0, 128, 72), 118);
            ApplyMaterialButtonIcon(btnPublish, "\u21E7", "Publish Result", actionColor, 126);
            ApplyMaterialButtonIcon(btnCancel, "\u2297", "Cancel", dangerColor, 92);

            LayoutFooterButtons();
        }

        private void ConfigurePipelineToolTips()
        {
            if (pipelineToolTip == null)
            {
                pipelineToolTip = new ToolTip();
            }

            pipelineToolTip.SetToolTip(cbToolType, "Choose the tool for the next step. Input and output are confirmed in Add Step.");
            pipelineToolTip.SetToolTip(cbInputLayer, "Default input used when Add Step opens.");
            pipelineToolTip.SetToolTip(tbOutputLayer, "Default output suggestion used when Add Step opens.");
            pipelineToolTip.SetToolTip(pipelineFlowHost, "Click a step to inspect its input, output, parameters, and preview.");
            pipelineToolTip.SetToolTip(stepIoPanel, "Selected step image flow. Change INPUT or OUTPUT here before editing tool parameters.");
            pipelineToolTip.SetToolTip(cbStepInputLayer, "Image layer used by the selected step.");
            pipelineToolTip.SetToolTip(tbStepOutputLayer, "Image layer created by the selected step.");
            pipelineToolTip.SetToolTip(btnStepChainInput, "Use the previous enabled step's output as this step's input.");
            pipelineToolTip.SetToolTip(stepIoStatusLabel, "Shows whether the selected step starts from a source image, follows the previous output, or branches from another layer.");
            pipelineToolTip.SetToolTip(btnAdd, "Open Add Step to confirm tool, step name, input, and output.");
            pipelineToolTip.SetToolTip(btnAiRecipe, "Create pipeline steps from an AI recipe.");
            pipelineToolTip.SetToolTip(cbPreviewImageMode, "Choose Summary for the full detection view, or Input/Output/Overlay for the selected step.");
            pipelineToolTip.SetToolTip(previewModeLabel, "Current preview mode. Click Step, IN, or OUT in Pipeline Flow to change it.");
            pipelineToolTip.SetToolTip(btnOpenPreview, "Open a zoomable preview. Mouse wheel zooms, drag pans.");
            pipelineToolTip.SetToolTip(previewBox, "Double-click to open the zoomable preview.");
            pipelineToolTip.SetToolTip(btnRun, "Run Preview executes inside this window. The main workspace is not updated.");
            pipelineToolTip.SetToolTip(btnPublish, "Publish Summary or the selected step result to the main workspace.");
            pipelineToolTip.SetToolTip(btnSave, "Save pipeline XML, workspace images, and step preview images.");
            pipelineToolTip.SetToolTip(chkPublishAllLayers, "Publish every cached step output instead of only the selected or latest result.");
        }

        private void SetRunLogHint(string hint = null)
        {
            if (runLogCaption == null)
            {
                return;
            }

            runLogCaption.Text = string.IsNullOrWhiteSpace(hint)
                ? "Run Log - Run Preview stays here; Publish Result updates the main workspace."
                : $"Run Log - {hint}";
        }

        private static void ApplyMaterialButtonIcon(Button button, string iconText, string labelText, Color iconColor, int width)
        {
            if (button == null)
            {
                return;
            }

            button.Size = new DrawingSize(width, button.Height);
            button.Image = null;
            button.Text = $"{iconText} {labelText}";
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.TextImageRelation = TextImageRelation.Overlay;
            button.Padding = new Padding(6, 0, 6, 0);
            button.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button.ForeColor = iconColor;
            button.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
        }

        private void LayoutFooterButtons()
        {
            if (footerPanel == null)
            {
                return;
            }

            int x = 0;
            PositionFooterButton(btnRemove, ref x, 4);
            PositionFooterButton(btnUp, ref x, 4);
            PositionFooterButton(btnDown, ref x, 10);

            if (chkPublishAllLayers != null)
            {
                chkPublishAllLayers.Location = new DrawingPoint(x, 14);
                x += chkPublishAllLayers.Width + 18;
            }

            Button[] rightButtons = new[] { btnMore, btnValidate, btnSave, btnRun, btnPublish, btnCancel };
            int rightWidth = rightButtons.Where(button => button != null).Sum(button => button.Width) + Math.Max(0, rightButtons.Count(button => button != null) - 1) * 4;
            int rightX = Math.Max(x + 10, footerPanel.ClientSize.Width - rightWidth);

            int actionX = rightX;
            PositionFooterButton(btnMore, ref actionX, 4);
            PositionFooterButton(btnValidate, ref actionX, 4);
            PositionFooterButton(btnSave, ref actionX, 4);
            PositionFooterButton(btnRun, ref actionX, 4);
            PositionFooterButton(btnPublish, ref actionX, 4);
            PositionFooterButton(btnCancel, ref actionX, 0);
        }

        private static void PositionFooterButton(Button button, ref int x, int gap)
        {
            if (button == null)
            {
                return;
            }

            button.Location = new DrawingPoint(x, 8);
            x += button.Width + gap;
        }

        private static Label CreateCaptionLabel(string text)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(35, 85, 132),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
            };
        }

        private static Control CreateFramedPanel(string title, Control content)
        {
            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0, 0, 8, 0)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.Controls.Add(CreateCaptionLabel(title), 0, 0);
            panel.Controls.Add(content, 0, 1);
            return panel;
        }

        private void LoadLayerItems()
        {
            string previousSelection = cbInputLayer.SelectedItem?.ToString();
            cbInputLayer.Items.Clear();
            foreach (string title in GetPipelineLayerTitles())
            {
                cbInputLayer.Items.Add(title);
            }

            SelectInputLayerItem(ResolveDefaultInputLayerForNewStep(previousSelection));
        }

        private void SelectInputLayerItem(string layerName)
        {
            if (cbInputLayer.Items.Count == 0)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(layerName))
            {
                for (int i = 0; i < cbInputLayer.Items.Count; i++)
                {
                    if (string.Equals(cbInputLayer.Items[i]?.ToString(), layerName, StringComparison.OrdinalIgnoreCase))
                    {
                        cbInputLayer.SelectedIndex = i;
                        return;
                    }
                }
            }

            cbInputLayer.SelectedIndex = 0;
        }

        private static void SelectComboItem(ComboBox comboBox, string value)
        {
            if (comboBox == null || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (string.Equals(comboBox.Items[i]?.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }
        }

        private void SelectToolTypeItem(string toolType)
        {
            if (cbToolType.Items.Count == 0 || string.IsNullOrWhiteSpace(toolType))
            {
                return;
            }

            for (int i = 0; i < cbToolType.Items.Count; i++)
            {
                if (string.Equals(cbToolType.Items[i]?.ToString(), toolType, StringComparison.OrdinalIgnoreCase))
                {
                    cbToolType.SelectedIndex = i;
                    return;
                }
            }
        }

        private List<string> GetAvailableToolTypes()
        {
            return cbToolType.Items
                .Cast<object>()
                .Select(item => item?.ToString())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private List<string> GetPipelineStepNames()
        {
            return pipeline?.Steps == null
                ? new List<string>()
                : pipeline.Steps
                    .Where(step => step != null && !string.IsNullOrWhiteSpace(step.Name))
                    .Select(step => step.Name.Trim())
                    .ToList();
        }

        private string ResolveDefaultInputLayerForNewStep(string fallbackLayer = null)
        {
            VisionPipelineStep lastProducer = pipeline?.Steps == null
                ? null
                : pipeline.Steps.LastOrDefault(step => step != null && step.Enabled && !string.IsNullOrWhiteSpace(step.OutputLayer));
            if (lastProducer != null)
            {
                return lastProducer.OutputLayer.Trim();
            }

            if (!string.IsNullOrWhiteSpace(fallbackLayer))
            {
                return fallbackLayer.Trim();
            }

            string mainLayer = GetPipelineLayerTitles()
                .FirstOrDefault(layer => string.Equals(layer, "Main", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(mainLayer))
            {
                return mainLayer;
            }

            return GetPipelineLayerTitles().FirstOrDefault() ?? "Main";
        }

        private string ResolveOutputLayerForNewStep(string toolType, string inputLayer)
        {
            string requested = string.IsNullOrWhiteSpace(tbOutputLayer.Text)
                ? string.Empty
                : tbOutputLayer.Text.Trim();
            if (userEditedHeaderOutputLayer
                && !string.IsNullOrWhiteSpace(requested)
                && !PipelineLayerExists(requested)
                && !string.Equals(requested, inputLayer, StringComparison.OrdinalIgnoreCase))
            {
                return requested;
            }

            return CreateUniqueLayerName(CreateSuggestedOutputLayerBase(toolType, inputLayer));
        }

        private void OnHeaderStepDefaultChanged(object sender, EventArgs e)
        {
            if (isUpdatingHeaderOutputLayer || userEditedHeaderOutputLayer)
            {
                return;
            }

            UpdateHeaderOutputSuggestion(force: false);
        }

        private void OnHeaderOutputLayerTextChanged(object sender, EventArgs e)
        {
            if (isUpdatingHeaderOutputLayer)
            {
                return;
            }

            userEditedHeaderOutputLayer = !string.Equals(tbOutputLayer.Text, lastSuggestedHeaderOutputLayer, StringComparison.Ordinal);
        }

        private void ResetHeaderOutputSuggestion(string toolType = null)
        {
            userEditedHeaderOutputLayer = false;
            UpdateHeaderOutputSuggestion(force: true, toolType);
        }

        private void UpdateHeaderOutputSuggestion(bool force, string toolType = null)
        {
            if (!force && userEditedHeaderOutputLayer)
            {
                return;
            }

            string selectedToolType = string.IsNullOrWhiteSpace(toolType)
                ? cbToolType.SelectedItem?.ToString() ?? "Threshold"
                : toolType.Trim();
            string selectedInputLayer = cbInputLayer.SelectedItem?.ToString() ?? ResolveDefaultInputLayerForNewStep();
            string suggestion = CreateUniqueLayerName(CreateSuggestedOutputLayerBase(selectedToolType, selectedInputLayer));

            isUpdatingHeaderOutputLayer = true;
            try
            {
                lastSuggestedHeaderOutputLayer = suggestion;
                tbOutputLayer.Text = suggestion;
            }
            finally
            {
                isUpdatingHeaderOutputLayer = false;
            }
        }

        private static string CreateSuggestedOutputLayerBase(string toolType, string inputLayer)
        {
            string toolToken = SanitizeLayerToken(toolType, "Step");
            string inputToken = SanitizeLayerToken(inputLayer, "Input");
            if (string.Equals(inputToken, "Main", StringComparison.OrdinalIgnoreCase)
                || string.Equals(inputToken, "Input", StringComparison.OrdinalIgnoreCase))
            {
                return $"{toolToken}_Output";
            }

            return $"{inputToken}_{toolToken}";
        }

        private static string SanitizeLayerToken(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            char[] chars = value.Trim()
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '_')
                .ToArray();
            string token = new string(chars).Trim('_');
            while (token.Contains("__", StringComparison.Ordinal))
            {
                token = token.Replace("__", "_");
            }

            return string.IsNullOrWhiteSpace(token) ? fallback : token;
        }

        private bool PipelineLayerExists(string layerName)
        {
            return !string.IsNullOrWhiteSpace(layerName)
                && GetPipelineLayerTitles().Any(title => string.Equals(title, layerName, StringComparison.OrdinalIgnoreCase));
        }

        private void PrepareNextAddDefaults(string toolType)
        {
            LoadLayerItems();
            ResetHeaderOutputSuggestion(toolType);
        }

        private void BindPipeline()
        {
            tbPipelineName.Text = pipeline.Name;
            treeSteps.BeginUpdate();
            treeSteps.Nodes.Clear();

            TreeNode root = new TreeNode(string.IsNullOrWhiteSpace(pipeline.Name) ? "Pipeline" : pipeline.Name)
            {
                Tag = null
            };
            treeSteps.Nodes.Add(root);

            TreeNode parent = root;
            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                TreeNode node = new TreeNode(FormatStepNodeText(step, i, GetStepStatus(step)))
                {
                    Tag = step,
                    ToolTipText = FormatStepNodeToolTip(step, i, GetStepStatus(step))
                };
                ApplyStepNodeStyle(node, GetStepStatus(step));
                parent.Nodes.Add(node);
                parent = node;
            }

            root.ExpandAll();
            treeSteps.EndUpdate();

            SelectFirstStepNode();
            if (pipeline.Steps.Count == 0)
            {
                propertyGridStep.SelectedObject = null;
                ShowStepPreview(null);
            }

            RefreshPipelineFlowPreview();
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            string toolType = cbToolType.SelectedItem?.ToString() ?? "Threshold";
            string inputLayer = cbInputLayer.SelectedItem?.ToString() ?? ResolveDefaultInputLayerForNewStep();
            string outputLayer = ResolveOutputLayerForNewStep(toolType, inputLayer);
            if (!TryShowAddStepDialog(
                toolType,
                inputLayer,
                outputLayer,
                "Append to pipeline",
                inputLayer,
                out string selectedToolType,
                out string selectedInputLayer,
                out string selectedOutputLayer,
                out string selectedStepName))
            {
                return;
            }

            VisionPipelineStep step = CreateDefaultStep(selectedToolType, selectedInputLayer, selectedOutputLayer);
            step.Name = selectedStepName;
            pipeline.Steps.Add(step);
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            SelectToolTypeItem(selectedToolType);
            PrepareNextAddDefaults(selectedToolType);
            BindPipeline();
            SelectStep(step);
            RefreshAfterPipelineNormalization(normalizedCount);
            AppendLog($"ADD | {step.Name} | {step.ToolType} | {step.InputLayer} -> {step.OutputLayer}");
        }

        private bool TryShowAddStepDialog(
            string defaultToolType,
            string defaultInputLayer,
            string defaultOutputLayer,
            string insertPositionText,
            string expectedInputLayer,
            out string selectedToolType,
            out string selectedInputLayer,
            out string selectedOutputLayer,
            out string selectedStepName)
        {
            UpdatePipelineFromView();
            LoadLayerItems();

            selectedToolType = string.Empty;
            selectedInputLayer = string.Empty;
            selectedOutputLayer = string.Empty;
            selectedStepName = string.Empty;

            string toolType = string.IsNullOrWhiteSpace(defaultToolType) ? "Threshold" : defaultToolType.Trim();
            string inputLayer = string.IsNullOrWhiteSpace(defaultInputLayer)
                ? ResolveDefaultInputLayerForNewStep()
                : defaultInputLayer.Trim();
            string outputLayer = string.IsNullOrWhiteSpace(defaultOutputLayer)
                ? ResolveOutputLayerForNewStep(toolType, inputLayer)
                : defaultOutputLayer.Trim();

            using (FormVisionPipelineAddStep form = new FormVisionPipelineAddStep(
                GetAvailableToolTypes(),
                GetPipelineLayerTitles().ToList(),
                toolType,
                inputLayer,
                outputLayer,
                CreateUniqueStepName(toolType),
                GetPipelineStepNames(),
                GetPipelineLayerTitles,
                insertPositionText,
                expectedInputLayer))
            {
                if (VisionPipelineDialogService.ShowDialog(form, this) != DialogResult.OK)
                {
                    return false;
                }

                selectedToolType = form.SelectedToolType;
                selectedInputLayer = form.SelectedInputLayer;
                selectedOutputLayer = form.SelectedOutputLayer;
                selectedStepName = form.SelectedStepName;
                return true;
            }
        }

        private void OnAiRecipeClicked(object sender, EventArgs e)
        {
            UpdatePipelineFromView();
            using (FormVisionPipelineLlmRecipe form = new FormVisionPipelineLlmRecipe(GetDisplayLayerTitles(), CreateContextFromLayers))
            {
                DialogResult result = VisionPipelineDialogService.ShowDialog(form, this);
                if (result != DialogResult.OK || form.ImportedPipeline == null)
                {
                    return;
                }

                Bitmap importedPreviewImage = form.TakeImportedPreviewImage();
                pipeline = form.ImportedPipeline;
                tbRunLog.Clear();
                ClearStepPreviewImages();
                ResetStepStatuses();
                if (importedPreviewImage != null)
                {
                    ApplyImportedPreviewImage(importedPreviewImage, "AI IMAGE");
                }

                int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
                LoadLayerItems();
                BindPipeline();
                RefreshAfterPipelineNormalization(normalizedCount);
                SelectReviewStepAfterAiApply();
                AppendLog($"AI APPLY | {pipeline.Name} | Steps={pipeline.Steps.Count}");
                VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, GetDisplayLayerTitles());
                LogValidationResult(validation, showSuccess: true);
                if (!validation.Success)
                {
                    FocusFirstValidationError(validation);
                    AppendLog("AI APPLY | Fix validation errors before running.");
                    return;
                }

                if (HasRunnablePipelineInputImage())
                {
                    AppendLog("AI APPLY | Preview run started.");
                    BeginInvoke(new Action(() => OnRunClicked(this, EventArgs.Empty)));
                }
                else
                {
                    AppendLog("AI APPLY | Load an input image, then Run.");
                }
            }
        }

        private void ApplyImportedPreviewImage(Bitmap image, string logPrefix = "IMAGE")
        {
            if (image == null)
            {
                return;
            }

            int index = displayManager.FindIndex("Main");
            if (index >= 0)
            {
                displayManager.SetLayerImage(index, image);
                displayManager.RefreshLayer(index);
            }
            else
            {
                displayManager.CreateLayerDisplay(image, "Main", true);
            }

            displayManager.ActivateLayer("Main");
            displayManager.ZoomLayerToFit("Main");
            displayManager.AcceptLayerImageChanged("Main");
            AppendLog($"{logPrefix} | Main | {image.Width} x {image.Height}");
        }

        private void SelectReviewStepAfterAiApply()
        {
            VisionPipelineStep reviewStep = pipeline.Steps.LastOrDefault(step => step?.Enabled == true)
                ?? pipeline.Steps.LastOrDefault();
            if (reviewStep != null)
            {
                SelectStep(reviewStep);
                ShowStepPreview(reviewStep);
                return;
            }

            ShowStepPreview(null);
        }

        private bool HasRunnablePipelineInputImage()
        {
            foreach (VisionPipelineStep step in pipeline.Steps.Where(step => step?.Enabled == true))
            {
                if (string.IsNullOrWhiteSpace(step.InputLayer))
                {
                    continue;
                }

                Bitmap image = displayManager.GetLayerImage(step.InputLayer);
                if (!DisplayManagerImageExtensions.IsPlaceholderBitmap(image))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnAddAfterSelectedClicked(object sender, EventArgs e)
        {
            string toolType = cbToolType.SelectedItem?.ToString() ?? "Threshold";
            int index = SelectedStepIndex;
            string inputLayer = index >= 0
                ? pipeline.Steps[index].OutputLayer
                : cbInputLayer.SelectedItem?.ToString() ?? "Main";
            string outputLayer = CreateUniqueLayerName(CreateSuggestedOutputLayerBase(toolType, inputLayer));
            if (!TryShowAddStepDialog(
                toolType,
                inputLayer,
                outputLayer,
                index >= 0 ? $"Add after {pipeline.Steps[index].Name}" : "Append to pipeline",
                inputLayer,
                out string selectedToolType,
                out string selectedInputLayer,
                out string selectedOutputLayer,
                out string selectedStepName))
            {
                return;
            }

            VisionPipelineStep step = CreateDefaultStep(selectedToolType, selectedInputLayer, selectedOutputLayer);
            step.Name = selectedStepName;

            int insertIndex = index < 0 ? pipeline.Steps.Count : index + 1;
            pipeline.Steps.Insert(insertIndex, step);
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            SelectToolTypeItem(selectedToolType);
            PrepareNextAddDefaults(selectedToolType);
            BindPipeline();
            SelectStep(step);
            RefreshAfterPipelineNormalization(normalizedCount);
            AppendLog($"ADD AFTER | {step.Name} | {step.ToolType} | {step.InputLayer} -> {step.OutputLayer}");
        }

        private void OnInsertBeforeSelectedClicked(object sender, EventArgs e)
        {
            int index = SelectedStepIndex;
            if (index < 0)
            {
                OnAddClicked(sender, e);
                return;
            }

            string toolType = cbToolType.SelectedItem?.ToString() ?? "Threshold";
            VisionPipelineStep current = pipeline.Steps[index];
            string outputLayer = CreateUniqueLayerName(CreateSuggestedOutputLayerBase(toolType, current.InputLayer));
            if (!TryShowAddStepDialog(
                toolType,
                current.InputLayer,
                outputLayer,
                $"Insert before {current.Name}",
                current.InputLayer,
                out string selectedToolType,
                out string selectedInputLayer,
                out string selectedOutputLayer,
                out string selectedStepName))
            {
                return;
            }

            VisionPipelineStep step = CreateDefaultStep(selectedToolType, selectedInputLayer, selectedOutputLayer);
            step.Name = selectedStepName;
            current.InputLayer = step.OutputLayer;

            pipeline.Steps.Insert(index, step);
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            SelectToolTypeItem(selectedToolType);
            PrepareNextAddDefaults(selectedToolType);
            BindPipeline();
            SelectStep(step);
            RefreshAfterPipelineNormalization(normalizedCount);
            AppendLog($"INSERT | {step.Name} | {step.ToolType} | {step.InputLayer} -> {step.OutputLayer}");
        }

        private void OnCopyStepClicked(object sender, EventArgs e)
        {
            if (SelectedStep == null) { return; }

            copiedStep = CloneStep(SelectedStep, makeUnique: false);
            AppendLog($"COPY | {SelectedStep.Name}");
        }

        private void OnPasteAfterSelectedClicked(object sender, EventArgs e)
        {
            if (copiedStep == null) { return; }

            int index = SelectedStepIndex;
            VisionPipelineStep step = CloneStep(copiedStep, makeUnique: true);
            if (index >= 0)
            {
                step.InputLayer = pipeline.Steps[index].OutputLayer;
            }

            int insertIndex = index < 0 ? pipeline.Steps.Count : index + 1;
            pipeline.Steps.Insert(insertIndex, step);
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            PrepareNextAddDefaults(step.ToolType);
            BindPipeline();
            SelectStep(step);
            RefreshAfterPipelineNormalization(normalizedCount);
        }

        private void OnDuplicateStepClicked(object sender, EventArgs e)
        {
            if (SelectedStep == null) { return; }

            int index = SelectedStepIndex;
            VisionPipelineStep step = CloneStep(SelectedStep, makeUnique: true);
            if (index >= 0 && !string.IsNullOrWhiteSpace(SelectedStep.OutputLayer))
            {
                string inputLayer = SelectedStep.OutputLayer.Trim();
                step.InputLayer = inputLayer;
                step.OutputLayer = CreateUniqueLayerName(CreateSuggestedOutputLayerBase(step.ToolType, inputLayer));
            }

            pipeline.Steps.Insert(index + 1, step);
            PrepareNextAddDefaults(step.ToolType);
            BindPipeline();
            SelectStep(step);
            AppendLog($"DUPLICATE | {step.Name} | {step.InputLayer} -> {step.OutputLayer}");
        }

        private void OnToggleStepEnabledClicked(object sender, EventArgs e)
        {
            if (SelectedStep == null) { return; }

            SelectedStep.Enabled = !SelectedStep.Enabled;
            UpdateSelectedTreeNodeText();
            BindStepProperty(SelectedStep);
            ShowStepPreview(SelectedStep);
        }

        private void ApplyAcceptancePreset(VisionPipelineAcceptancePreset preset)
        {
            if (SelectedStep == null || preset == null)
            {
                return;
            }

            VisionPipelineKnownMetrics.ApplyPreset(SelectedStep, preset);
            BindStepProperty(SelectedStep);
            UpdateSelectedTreeNodeText();
            AppendLog($"PRESET | {SelectedStep.Name} | {preset.Name}");
        }

        private void OnClearAcceptanceClicked(object sender, EventArgs e)
        {
            if (SelectedStep == null)
            {
                return;
            }

            VisionPipelineKnownMetrics.ClearAcceptance(SelectedStep);
            BindStepProperty(SelectedStep);
            UpdateSelectedTreeNodeText();
            AppendLog($"PRESET | {SelectedStep.Name} | Acceptance cleared.");
        }

        private void OnChainSelectedStepInputClicked(object sender, EventArgs e)
        {
            UpdatePipelineFromView();
            VisionPipelineStep step = SelectedStep;
            int index = SelectedStepIndex;
            if (step == null || index < 0 || !CanChainSelectedStepInput(out string previousOutput, out string currentInput))
            {
                SetRunLogHint("Select a step after the first one, then choose a previous output to chain.");
                AppendLog("CHAIN NG | No previous output is available for the selected step.");
                return;
            }

            step.InputLayer = previousOutput;
            string normalizationMessage = NormalizeChainedInspectionPreprocessing(step, index);
            InvalidateStepResultsFrom(index);
            LoadLayerItems();
            BindStepProperty(step);
            UpdateSelectedTreeNodeText();
            RefreshPipelineFlowPreview();
            ShowStepPreview(step);
            SetRunLogHint($"Selected step now uses previous output: {previousOutput}");
            AppendLog($"CHAIN | {step.Name} | Input {currentInput} -> {previousOutput}");
            if (!string.IsNullOrWhiteSpace(normalizationMessage))
            {
                AppendLog(normalizationMessage);
            }
        }

        private bool CanChainSelectedStepInput(out string previousOutput, out string currentInput)
        {
            previousOutput = string.Empty;
            currentInput = string.Empty;

            VisionPipelineStep step = SelectedStep;
            int index = SelectedStepIndex;
            if (step == null || index <= 0)
            {
                return false;
            }

            currentInput = string.IsNullOrWhiteSpace(step.InputLayer) ? string.Empty : step.InputLayer.Trim();
            if (!TryGetPreviousEnabledOutput(index, out previousOutput))
            {
                return false;
            }

            return !string.Equals(currentInput, previousOutput, StringComparison.OrdinalIgnoreCase);
        }

        private bool TryGetPreviousEnabledOutput(int stepIndex, out string previousOutput)
        {
            return VisionPipelineNormalizer.TryGetPreviousEnabledOutput(pipeline, stepIndex, out previousOutput);
        }

        private void InvalidateStepResultsFrom(int startIndex)
        {
            if (pipeline?.Steps == null)
            {
                return;
            }

            for (int i = Math.Max(0, startIndex); i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                RemoveStepPreview(step);
                SetStepStatus(step, PipelineStepRunStatus.NotRun);
            }
        }

        private string NormalizeChainedInspectionPreprocessing(VisionPipelineStep step, int stepIndex)
        {
            VisionPipelineNormalizationChange change = VisionPipelineNormalizer.NormalizeChainedInspectionPreprocessing(pipeline, step, stepIndex);
            return change == null ? string.Empty : change.Message;
        }

        private int NormalizeChainedInspectionPreprocessingForPipeline(bool appendLog, bool invalidateResults = true)
        {
            IReadOnlyList<VisionPipelineNormalizationChange> changes =
                VisionPipelineNormalizer.NormalizeChainedInspectionPreprocessing(pipeline);

            foreach (VisionPipelineNormalizationChange change in changes)
            {
                if (appendLog)
                {
                    AppendLog(change.Message);
                }
            }

            if (appendLog && changes.Count > 0)
            {
                AppendLog($"AUTO FIX | Normalized {changes.Count} chained inspection step(s). Save Project to keep these settings.");
            }

            if (invalidateResults && changes.Count > 0)
            {
                InvalidateStepResultsFrom(changes[0].StepIndex);
            }

            return changes.Count;
        }

        private void RefreshAfterPipelineNormalization(int changedCount)
        {
            if (changedCount <= 0)
            {
                return;
            }

            if (SelectedStep != null)
            {
                BindStepProperty(SelectedStep);
            }

            RefreshPipelineFlowPreview();
            ShowStepPreview(SelectedStep);
        }

        private void OnTreeStepNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Node != null)
            {
                treeSteps.SelectedNode = e.Node;
            }
        }

        private void OnStepContextMenuOpening(object sender, CancelEventArgs e)
        {
            bool hasStep = SelectedStep != null;
            bool canChain = CanChainSelectedStepInput(out string previousOutput, out _);
            menuInsertBefore.Enabled = hasStep;
            menuCopyStep.Enabled = hasStep;
            menuPasteAfter.Enabled = copiedStep != null;
            menuDuplicateStep.Enabled = hasStep;
            menuAcceptancePreset.Enabled = hasStep;
            menuClearAcceptance.Enabled = hasStep;
            menuChainStepInput.Enabled = canChain;
            menuChainStepInput.Text = canChain
                ? $"Use Previous Output as Input ({previousOutput})"
                : "Use Previous Output as Input";
            menuToggleStepEnabled.Enabled = hasStep;
            menuRemoveStep.Enabled = hasStep;
            menuToggleStepEnabled.Text = hasStep && SelectedStep.Enabled ? "Disable Step" : "Enable Step";
        }

        private void OnMoreCommandMenuOpening(object sender, CancelEventArgs e)
        {
            if (menuMoreChainStepInput == null)
            {
                return;
            }

            bool canChain = CanChainSelectedStepInput(out string previousOutput, out _);
            menuMoreChainStepInput.Enabled = canChain;
            menuMoreChainStepInput.Text = canChain
                ? $"Use Previous Output as Input ({previousOutput})"
                : "Use Previous Output as Input";
        }

        private static VisionPipelineStep CreateDefaultStep(string toolType, string inputLayer, string outputLayer)
        {
            VisionPipelineStep step = new VisionPipelineStep
            {
                Name = $"{toolType}_{DateTime.Now:HHmmss}",
                ToolType = toolType,
                InputLayer = inputLayer,
                OutputLayer = outputLayer
            };

            switch (NormalizeToolType(toolType))
            {
                case "threshold":
                    step.Parameters["Mode"] = "Threshold";
                    step.Parameters["Threshold"] = "127";
                    step.Parameters["MaxValue"] = "255";
                    step.Parameters["ThresholdType"] = "Binary";
                    break;
                case "morphology":
                    step.Parameters["Shape"] = "Rect";
                    step.Parameters["Operator"] = "Erode";
                    step.Parameters["KernelWidth"] = "3";
                    step.Parameters["KernelHeight"] = "3";
                    step.Parameters["Iterations"] = "1";
                    break;
                case "filter":
                    step.Parameters["FilterType"] = "Blur";
                    step.Parameters["KernelWidth"] = "3";
                    step.Parameters["KernelHeight"] = "3";
                    break;
                case "edgedetection":
                    step.Parameters["EdgeType"] = "Canny";
                    step.Parameters["CannyThresholdLow"] = "100";
                    step.Parameters["CannyThresholdHigh"] = "200";
                    step.Parameters["CannyApertureSize"] = "3";
                    step.Parameters["UseL2Gradient"] = "True";
                    break;
                case "blob":
                    step.Parameters["MIN_AREA"] = "200";
                    step.Parameters["MAX_AREA"] = "1000000";
                    break;
                case "contour":
                    step.Parameters["MIN_AREA"] = "200";
                    step.Parameters["MAX_AREA"] = "1000000";
                    step.Parameters["DetectMode"] = "List";
                    step.Parameters["ApproximationModes"] = "ApproxSimple";
                    break;
                case "linegauge":
                    step.Parameters["PRJ_PORALITY"] = "BTOW";
                    step.Parameters["PRJ_DIR"] = "X_LTOR";
                    step.Parameters["CONTRAST"] = "30";
                    break;
                case "mean":
                    step.Parameters["MEAN_TYPES"] = "Mean";
                    break;
            }

            return step;
        }

        private VisionPipelineStep CloneStep(VisionPipelineStep source, bool makeUnique)
        {
            VisionPipelineStep clone = new VisionPipelineStep
            {
                Name = makeUnique ? CreateUniqueStepName(source.Name) : source.Name,
                ToolType = source.ToolType,
                Enabled = source.Enabled,
                InputLayer = source.InputLayer,
                OutputLayer = makeUnique ? CreateUniqueLayerName(source.OutputLayer) : source.OutputLayer,
                UseAcceptance = source.UseAcceptance,
                ExpectedSuccess = source.ExpectedSuccess,
                MaxElapsedMilliseconds = source.MaxElapsedMilliseconds,
                RequiredMessageText = source.RequiredMessageText,
                AcceptanceMetricName = source.AcceptanceMetricName,
                UseAcceptanceMetricMinimum = source.UseAcceptanceMetricMinimum,
                AcceptanceMetricMinimum = source.AcceptanceMetricMinimum,
                UseAcceptanceMetricMaximum = source.UseAcceptanceMetricMaximum,
                AcceptanceMetricMaximum = source.AcceptanceMetricMaximum
            };

            foreach (KeyValuePair<string, string> parameter in source.Parameters)
            {
                clone.Parameters[parameter.Key] = parameter.Value;
            }

            return clone;
        }

        private string CreateUniqueStepName(string baseName)
        {
            string name = string.IsNullOrWhiteSpace(baseName) ? "Step" : baseName.Trim();
            if (!pipeline.Steps.Any(step => string.Equals(step.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                return name;
            }

            for (int index = 2; index < 10000; index++)
            {
                string candidate = $"{name}_{index}";
                if (!pipeline.Steps.Any(step => string.Equals(step.Name, candidate, StringComparison.OrdinalIgnoreCase)))
                {
                    return candidate;
                }
            }

            return $"{name}_{DateTime.Now:HHmmss}";
        }

        private string CreateUniqueLayerName(string baseName)
        {
            string name = string.IsNullOrWhiteSpace(baseName) ? "Pipeline_Output" : baseName.Trim();
            HashSet<string> existing = new HashSet<string>(GetPipelineLayerTitles(), StringComparer.OrdinalIgnoreCase);
            if (!existing.Contains(name))
            {
                return name;
            }

            for (int index = 2; index < 10000; index++)
            {
                string candidate = $"{name}_{index}";
                if (!existing.Contains(candidate))
                {
                    return candidate;
                }
            }

            return $"{name}_{DateTime.Now:HHmmss}";
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            int index = SelectedStepIndex;
            if (index < 0) { return; }

            RemoveStepPreview(pipeline.Steps[index]);
            pipeline.Steps.RemoveAt(index);
            LoadLayerItems();
            BindPipeline();
            SelectStepAt(Math.Min(index, pipeline.Steps.Count - 1));
        }

        private void OnUpClicked(object sender, EventArgs e)
        {
            int index = SelectedStepIndex;
            if (index <= 0) { return; }

            VisionPipelineStep step = pipeline.Steps[index];
            pipeline.Steps.RemoveAt(index);
            pipeline.Steps.Insert(index - 1, step);
            LoadLayerItems();
            BindPipeline();
            SelectStep(step);
        }

        private void OnDownClicked(object sender, EventArgs e)
        {
            int index = SelectedStepIndex;
            if (index < 0 || index >= pipeline.Steps.Count - 1) { return; }

            VisionPipelineStep step = pipeline.Steps[index];
            pipeline.Steps.RemoveAt(index);
            pipeline.Steps.Insert(index + 1, step);
            LoadLayerItems();
            BindPipeline();
            SelectStep(step);
        }

        private void OnMoreClicked(object sender, EventArgs e)
        {
            if (moreCommandMenu == null || btnMore == null)
            {
                return;
            }

            moreCommandMenu.Show(btnMore, new DrawingPoint(0, btnMore.Height));
        }

        private void OnLoadClicked(object sender, EventArgs e)
        {
            UpdatePipelineFromView();
            string requestedPipelineName = string.IsNullOrWhiteSpace(tbPipelineName.Text) ? "Pipeline" : tbPipelineName.Text.Trim();
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, requestedPipelineName);
            if (!File.Exists(pipelinePath))
            {
                AppendLog($"LOAD NG | Saved pipeline was not found: {requestedPipelineName}");
                VisionMessageBox.Warning(
                    this,
                    "Pipeline Load",
                    $"Saved pipeline was not found.\r\n\r\n{pipelinePath}");
                return;
            }

            pipeline = VisionPipelineStorage.Load(recipeName, requestedPipelineName);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            tbRunLog.Clear();
            ClearStepPreviewImages();
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            BindPipeline();
            LoadPipelineImages(restoreLayerImages: false);
            LoadLayerItems();
            RefreshAfterPipelineNormalization(normalizedCount);
            bool roundTripValid = VisionPipelineStorage.TryValidateRoundTrip(recipeName, pipeline, out string roundTripMessage);
            AppendActivePipelineLog("LOAD");
            AppendLog($"{(roundTripValid ? "LOAD VALID" : "LOAD WARN")} | {roundTripMessage}");
        }

        private void OnImportClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Import Pipeline XML";
                dialog.Filter = "Pipeline XML (*.xml;*.pipeline.xml)|*.xml;*.pipeline.xml|All files (*.*)|*.*";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;

                string initialDirectory = GetPipelineImportInitialDirectory();
                if (!string.IsNullOrWhiteSpace(initialDirectory) && Directory.Exists(initialDirectory))
                {
                    dialog.InitialDirectory = initialDirectory;
                }

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                ImportPipeline(dialog.FileName);
            }
        }

        private bool ImportPipeline(string path)
        {
            return ImportPipeline(path, promptToRun: true);
        }

        private bool ImportPipeline(string path, bool promptToRun)
        {
            UpdatePipelineFromView();
            if (!SerializeHelper.TryLoadFromXmlFile(path, out VisionPipeline imported) || imported == null)
            {
                VisionMessageBox.Warning(this, "Pipeline Import", "Pipeline XML could not be loaded.");
                AppendLog($"IMPORT NG | {path}");
                return false;
            }

            pipeline = imported;
            if (string.IsNullOrWhiteSpace(pipeline.Name))
            {
                pipeline.Name = Path.GetFileNameWithoutExtension(path);
            }

            tbRunLog.Clear();
            ClearStepPreviewImages();
            ResetStepStatuses();
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            LoadLayerItems();
            BindPipeline();
            RefreshAfterPipelineNormalization(normalizedCount);
            ShowStepPreview(SelectedStep);

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, GetDisplayLayerTitles());
            LogValidationResult(validation, showSuccess: true);
            AppendLog($"IMPORT | {Path.GetFileName(path)} | Steps={pipeline.Steps.Count}");

            if (!validation.Success)
            {
                FocusFirstValidationError(validation);
                VisionMessageBox.Warning(this, "Pipeline Import", validation.FormatErrors());
                return false;
            }

            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            bool roundTripValid = VisionPipelineStorage.TryValidateRoundTrip(recipeName, pipeline, out string roundTripMessage);
            AppendLog($"IMPORT SAVE | {pipeline.Name} | Pipeline XML saved for Load.");
            AppendActivePipelineLog("IMPORT");
            AppendLog($"{(roundTripValid ? "IMPORT VALID" : "IMPORT WARN")} | {roundTripMessage}");

            if (promptToRun)
            {
                DialogResult answer = VisionMessageBox.Confirm(
                    this,
                    "Pipeline Import",
                    "Pipeline XML imported and validated.\r\nRun this pipeline now?");
                if (answer == DialogResult.Yes)
                {
                    BeginInvoke(new Action(() => OnRunClicked(this, EventArgs.Empty)));
                }
            }

            return true;
        }

        private void OnValidateClicked(object sender, EventArgs e)
        {
            ValidateCurrentPipeline(showSuccessMessage: true);
        }

        private bool ValidateCurrentPipeline(bool showSuccessMessage)
        {
            UpdatePipelineFromView();
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            RefreshAfterPipelineNormalization(normalizedCount);
            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, GetDisplayLayerTitles());
            LogValidationResult(validation, showSuccess: true);
            if (!validation.Success)
            {
                SetRunLogHint("Check failed. Fix the selected step before Run Preview.");
                FocusFirstValidationError(validation);
                VisionMessageBox.Warning(this, "Pipeline Check", validation.FormatErrors());
                return false;
            }

            if (showSuccessMessage)
            {
                if (validation.Warnings.Count == 0)
                {
                    SetRunLogHint("Check passed. Run Preview will not update the main workspace.");
                    VisionMessageBox.Info(
                        this,
                        "Pipeline Check",
                        "Check passed.\r\n\r\nRun Preview executes inside the Pipeline window. Publish Result updates the main workspace.");
                }
                else
                {
                    FocusFirstValidationWarning(validation);
                    SetRunLogHint("Check passed. Review the selected item before Run Preview.");
                    VisionMessageBox.Warning(
                        this,
                        "Pipeline Check",
                        $"Check passed, but review is recommended.\r\n\r\n{validation.FormatWarnings()}\r\n\r\nRun Preview stays in this window. Publish Result updates the main workspace.");
                }
            }

            return true;
        }

        private static string GetPipelineImportInitialDirectory()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            foreach (string candidate in EnumeratePipelineImportDirectories(baseDirectory))
            {
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }
            }

            return baseDirectory;
        }

        private static IEnumerable<string> EnumeratePipelineImportDirectories(string baseDirectory)
        {
            HashSet<string> roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(baseDirectory))
            {
                roots.Add(Path.GetFullPath(baseDirectory));
            }

            roots.Add(Path.GetFullPath(Directory.GetCurrentDirectory()));

            foreach (string root in roots)
            {
                foreach (string candidate in EnumeratePipelineImportDirectoriesFromRoot(root))
                {
                    yield return candidate;
                }
            }
        }

        private static IEnumerable<string> EnumeratePipelineImportDirectoriesFromRoot(string root)
        {
            string current = string.IsNullOrWhiteSpace(root)
                ? Directory.GetCurrentDirectory()
                : Path.GetFullPath(root);

            for (int i = 0; i < 6 && !string.IsNullOrWhiteSpace(current); i++)
            {
                yield return Path.Combine(current, "docs", "samples");
                yield return Path.Combine(current, "RECIPE");
                yield return Path.Combine(current, "Sample");

                DirectoryInfo parent = Directory.GetParent(current);
                if (parent == null)
                {
                    break;
                }

                current = parent.FullName;
            }
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            UpdatePipelineFromView();
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            RefreshAfterPipelineNormalization(normalizedCount);
            LogValidationResult(VisionPipelineValidator.Validate(pipeline, GetDisplayLayerTitles()), showSuccess: false);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            SavePipelineImages();
            bool roundTripValid = VisionPipelineStorage.TryValidateRoundTrip(recipeName, pipeline, out string roundTripMessage);
            SetRunLogHint("Pipeline XML, workspace images, and previews saved.");
            AppendActivePipelineLog("SAVE");
            AppendLog($"{(roundTripValid ? "SAVE VALID" : "SAVE WARN")} | {roundTripMessage}");
            if (roundTripValid)
            {
                VisionMessageBox.Info(this, "Pipeline", "Pipeline project saved.");
            }
            else
            {
                VisionMessageBox.Warning(this, "Pipeline", $"Pipeline project saved, but validation reported:\r\n{roundTripMessage}");
            }
        }

        private void OnHistoryClicked(object sender, EventArgs e)
        {
            UpdatePipelineFromView();
            using (FormVisionPipelineHistory historyForm = new FormVisionPipelineHistory(recipeName, pipeline.Name))
            {
                VisionPipelineDialogService.ShowDialog(historyForm, this);
            }
        }

        private void OnSamplesClicked(object sender, EventArgs e)
        {
            UpdatePipelineFromView();
            using (FormVisionPipelineSamples sampleForm = new FormVisionPipelineSamples(recipeName, pipeline.Name, displayManager))
            {
                DialogResult result = VisionPipelineDialogService.ShowDialog(sampleForm, this);
                if (result == DialogResult.OK && sampleForm.SelectedCatalogSample != null)
                {
                    ApplyCatalogSample(sampleForm.SelectedCatalogSample);
                }
            }

            LoadLayerItems();
        }

        private void ApplyCatalogSample(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(sample.ImageFullPath) || !File.Exists(sample.ImageFullPath))
                {
                    VisionMessageBox.Warning(this, "Pipeline Samples", $"Sample image was not found.\r\n\r\n{sample.ImageFullPath}");
                    AppendLog($"SAMPLE NG | Missing image | {sample.SampleName}");
                    return;
                }

                if (string.IsNullOrWhiteSpace(sample.PipelineFullPath) || !File.Exists(sample.PipelineFullPath))
                {
                    VisionMessageBox.Warning(this, "Pipeline Samples", $"Sample pipeline was not found.\r\n\r\n{sample.PipelineFullPath}");
                    AppendLog($"SAMPLE NG | Missing pipeline | {sample.SampleName}");
                    return;
                }

                using (Bitmap loaded = new Bitmap(sample.ImageFullPath))
                {
                    ApplyImportedPreviewImage(new Bitmap(loaded), "SAMPLE IMAGE");
                }

                bool imported = ImportPipeline(sample.PipelineFullPath, promptToRun: false);
                if (!imported)
                {
                    return;
                }

                SetRunLogHint($"Sample ready. Expected {sample.ExpectedText}. Run Preview stays in this window.");
                AppendLog($"SAMPLE OPEN | {sample.SampleName} | {Path.GetFileName(sample.ImageFullPath)} | {Path.GetFileName(sample.PipelineFullPath)} | Expected {sample.ExpectedText}");
                if (HasRunnablePipelineInputImage())
                {
                    BeginInvoke(new Action(() => OnRunClicked(this, EventArgs.Empty)));
                }
            }
            catch (Exception ex)
            {
                VisionMessageBox.Error(this, "Pipeline Samples", ex.GetBaseException().Message, ex.ToString());
                AppendLog($"SAMPLE NG | {sample.SampleName} | {ex.GetBaseException().Message}");
            }
        }

        private void OnBatchClicked(object sender, EventArgs e)
        {
            UpdatePipelineFromView();
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            RefreshAfterPipelineNormalization(normalizedCount);
            using (FormVisionPipelineBatch batchForm = new FormVisionPipelineBatch(recipeName, pipeline))
            {
                VisionPipelineDialogService.ShowDialog(batchForm, this);
            }
        }

        private async void OnRunClicked(object sender, EventArgs e)
        {
            if (isRunningPipeline)
            {
                AppendLog("RUN | Pipeline is already running.");
                return;
            }

            UpdatePipelineFromView();
            tbRunLog.Clear();
            int normalizedCount = NormalizeChainedInspectionPreprocessingForPipeline(appendLog: true);
            RefreshAfterPipelineNormalization(normalizedCount);
            SetRunLogHint("Preview is running. Main workspace is unchanged.");
            ClearStepPreviewImages();
            ResetStepStatuses();
            ShowStepPreview(SelectedStep);
            AppendLog("PREVIEW START | Results stay in Pipeline until Publish Result.");
            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, GetDisplayLayerTitles());
            LogValidationResult(validation, showSuccess: true);
            if (!validation.Success)
            {
                SetRunLogHint("Check failed. Fix the selected step before Run Preview.");
                FocusFirstValidationError(validation);
                VisionMessageBox.Warning(this, "Pipeline", validation.FormatErrors());
                return;
            }

            DateTime startedAt = DateTime.Now;
            Stopwatch stopwatch = Stopwatch.StartNew();
            isRunningPipeline = true;
            SetPipelineRunningState(true);
            runCancellationSource = new CancellationTokenSource();
            Cursor previousCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            VisionPipelineRunResult result;
            bool runCanceled = false;
            try
            {
                using (VisionPipelineContext context = CreateContextFromLayers())
                {
                    result = await RunPipelineAsync(context, runCancellationSource.Token);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                AppendLog($"Pipeline failed: {ex.GetBaseException().Message}");
                VisionMessageBox.Error(this, "Pipeline", ex.GetBaseException().Message, ex.ToString());
                return;
            }
            finally
            {
                runCanceled = runCancellationSource?.IsCancellationRequested == true;
                isRunningPipeline = false;
                SetPipelineRunningState(false);
                runCancellationSource?.Dispose();
                runCancellationSource = null;
                Cursor.Current = previousCursor;
            }

            stopwatch.Stop();
            DateTime finishedAt = DateTime.Now;
            displayManager.SetTackTime($"{stopwatch.Elapsed.TotalSeconds:0.000}s");
            RefreshPipelineSummaryPreviewImage();
            ShowStepPreview(SelectedStep);

            SaveRunReport(result, startedAt, finishedAt, publishAllOutputs: false);

            VisionPipelineStepResult last = result.StepResults.LastOrDefault();
            if (runCanceled)
            {
                SetRunLogHint("Preview canceled. Main workspace was not changed.");
                AppendLog($"PREVIEW CANCELED | Main workspace unchanged. | {stopwatch.Elapsed.TotalMilliseconds:0.0} ms");
                return;
            }

            if (last != null && !IsStepResultPassed(last))
            {
                string message = string.IsNullOrWhiteSpace(last.ToolResult?.Message)
                    ? last.AcceptanceMessage
                    : last.ToolResult.Message;
                SetRunLogHint("Preview failed. Check the selected step details.");
                AppendLog($"PREVIEW NG | {last.Step?.Name} | {message}");
                return;
            }

            SetCurrentPreviewImageMode(StepPreviewImageMode.Summary, refresh: true);
            SetRunLogHint("Preview ready. Summary shows all detections; Publish Result writes it to the main workspace.");
            AppendLog($"PREVIEW OK | Summary ready. Main workspace unchanged until Publish Result. | {stopwatch.Elapsed.TotalMilliseconds:0.0} ms");
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            if (runCancellationSource == null)
            {
                return;
            }

            AppendLog("CANCEL | Stop requested.");
            runCancellationSource.Cancel();
            btnCancel.Enabled = false;
        }

        private void OnPublishClicked(object sender, EventArgs e)
        {
            if (isRunningPipeline)
            {
                SetRunLogHint("Cannot publish while Preview is running.");
                AppendLog("PUBLISH NG | Pipeline is running.");
                return;
            }

            if (stepResultImages.Count == 0)
            {
                SetRunLogHint("Run Preview before Publish Result.");
                AppendLog("PUBLISH NG | Run Preview before publishing results.");
                VisionMessageBox.Info(
                    this,
                    "Pipeline Publish",
                    "Run Preview first, then Publish Result to update the output layer in the main workspace.");
                return;
            }

            if (chkPublishAllLayers.Checked)
            {
                int publishCount = 0;
                foreach (VisionPipelineStep step in pipeline.Steps)
                {
                    if (PublishStepOutput(step, showMessageOnBlocked: false))
                    {
                        publishCount++;
                    }
                }

                AppendLog($"PUBLISH OK | All cached outputs published. Count={publishCount}");
                if (publishCount == 0)
                {
                    SetRunLogHint("Nothing was published. Check each output layer.");
                    VisionMessageBox.Warning(
                        this,
                        "Pipeline Publish",
                        "No output layer could be published. Check each step output layer.");
                }
                else
                {
                    SetRunLogHint($"Publish complete. {publishCount} output layer(s) updated in the main workspace.");
                }

                return;
            }

            if (GetCurrentPreviewImageMode() == StepPreviewImageMode.Summary)
            {
                PublishPipelineSummaryOutput(showMessageOnBlocked: true);
                return;
            }

            VisionPipelineStep publishStep = ResolvePublishStep();
            if (publishStep == null)
            {
                SetRunLogHint("No cached result to publish.");
                AppendLog("PUBLISH NG | No cached step result was found.");
                return;
            }

            PublishStepOutput(publishStep, showMessageOnBlocked: true);
        }

        private bool PublishPipelineSummaryOutput(bool showMessageOnBlocked)
        {
            if (pipelineSummaryPreviewImage == null)
            {
                SetRunLogHint("Run Preview before publishing Summary.");
                AppendLog("PUBLISH NG | Summary preview is not ready.");
                if (showMessageOnBlocked)
                {
                    VisionMessageBox.Info(this, "Pipeline Publish", "Run Preview first, then publish the Summary result.");
                }

                return false;
            }

            string layerName = PipelineSummaryOutputLayerName;
            Bitmap publishImage = new Bitmap(pipelineSummaryPreviewImage);
            int index = displayManager.FindIndex(layerName);
            if (index >= 0)
            {
                displayManager.SetLayerImage(index, publishImage);
                displayManager.RefreshLayer(index);
                displayManager.ActivateLayer(index);
            }
            else
            {
                displayManager.CreateLayerDisplay(publishImage, layerName, true);
                displayManager.ActivateLayer(layerName);
            }

            displayManager.AcceptLayerImageChanged(layerName);
            SetRunLogHint($"Publish complete. Summary was updated in '{layerName}'.");
            AppendLog($"PUBLISH OK | Summary -> {layerName} | {pipelineSummaryPreviewImage.Width} x {pipelineSummaryPreviewImage.Height} | Overlays={pipelineSummaryOverlays.Count}");
            return true;
        }

        private void SetPipelineRunningState(bool running)
        {
            btnRun.Enabled = !running;
            btnCancel.Enabled = running;
            btnAdd.Enabled = !running;
            btnRemove.Enabled = !running;
            btnUp.Enabled = !running;
            btnDown.Enabled = !running;
            btnMore.Enabled = !running;
            btnImport.Enabled = !running;
            btnValidate.Enabled = !running;
            btnLoad.Enabled = !running;
            btnSave.Enabled = !running;
            btnHistory.Enabled = !running;
            btnSamples.Enabled = !running;
            btnBatch.Enabled = !running;
            btnAiRecipe.Enabled = !running;
            btnPublish.Enabled = !running;
            cbToolType.Enabled = !running;
            cbInputLayer.Enabled = !running;
            tbOutputLayer.Enabled = !running;
            tbPipelineName.Enabled = !running;
            treeSteps.Enabled = !running;
            propertyGridHost.Enabled = !running;
            chkPublishAllLayers.Enabled = !running;
        }

        private async Task<VisionPipelineRunResult> RunPipelineAsync(VisionPipelineContext context, CancellationToken cancellationToken)
        {
            return await VisionPipelineExecutionService.RunAsync(
                pipeline,
                context,
                StepTimeoutMilliseconds,
                cancellationToken,
                OnPipelineStepExecutionUpdated);
        }

        private void OnPipelineStepExecutionUpdated(VisionPipelineStepExecutionUpdate update)
        {
            if (update == null)
            {
                return;
            }

            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() => OnPipelineStepExecutionUpdated(update)));
                }
                catch (InvalidOperationException)
                {
                }

                return;
            }

            VisionPipelineStep step = update.Step;
            if (string.Equals(update.Status, "RUN", StringComparison.OrdinalIgnoreCase))
            {
                SetStepStatus(step, PipelineStepRunStatus.Running);
                CacheStepInputPreview(step);
                AppendLog($"START | {step?.Name} | {step?.InputLayer} -> {step?.OutputLayer}");
                return;
            }

            if (string.Equals(update.Status, "CANCEL", StringComparison.OrdinalIgnoreCase)
                && update.StepResult == null)
            {
                AppendLog($"CANCELED | {update.Message}");
                return;
            }

            if (update.StepResult == null)
            {
                AppendLog($"{update.Status} | {step?.Name} | {update.Message}");
                return;
            }

            SetStepStatus(step, GetStepRunStatus(update.StepResult));
            AppendStepLog(update.StepResult);
            CacheStepResultSummary(update.StepResult);
            CacheStepOverlays(update.StepResult);
            CacheStepPreview(update.StepResult);
            if (ReferenceEquals(SelectedStep, step))
            {
                ShowStepPreview(step);
            }
        }

        private VisionPipelineContext CreateContextFromLayers()
        {
            VisionPipelineContext context = new VisionPipelineContext();
            ClearPreviewLayerImages();

            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                string title = displayManager.GetLayerTitle(i);
                Bitmap image = displayManager.GetLayerImage(i);
                if (image == null) { continue; }

                using (Mat mat = BitmapImageConverter.ToMat(image))
                {
                    context.SetLayer(title, mat);
                }

                SetPreviewLayerImage(title, image);
            }

            return context;
        }

        private List<string> GetDisplayLayerTitles()
        {
            List<string> titles = new List<string>();
            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                string title = displayManager.GetLayerTitle(i);
                if (!string.IsNullOrWhiteSpace(title))
                {
                    titles.Add(title);
                }
            }

            return titles;
        }

        private IEnumerable<string> GetPipelineLayerTitles()
        {
            HashSet<string> titles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string title in GetDisplayLayerTitles())
            {
                if (titles.Add(title))
                {
                    yield return title;
                }
            }

            if (titles.Add("Main"))
            {
                yield return "Main";
            }

            foreach (VisionPipelineStep step in pipeline.Steps)
            {
                if (!string.IsNullOrWhiteSpace(step.InputLayer) && titles.Add(step.InputLayer))
                {
                    yield return step.InputLayer;
                }

                if (!string.IsNullOrWhiteSpace(step.OutputLayer) && titles.Add(step.OutputLayer))
                {
                    yield return step.OutputLayer;
                }
            }

            if (titles.Add("Pipeline_Output"))
            {
                yield return "Pipeline_Output";
            }
        }

        private VisionPipelineStep ResolvePublishStep()
        {
            VisionPipelineStep selected = SelectedStep;
            if (selected != null && stepResultImages.ContainsKey(selected))
            {
                return selected;
            }

            return pipeline.Steps
                .LastOrDefault(step => step != null
                    && stepResultImages.ContainsKey(step)
                    && GetStepStatus(step) == PipelineStepRunStatus.Passed)
                ?? pipeline.Steps.LastOrDefault(step => step != null && stepResultImages.ContainsKey(step));
        }

        private bool PublishStepOutput(VisionPipelineStep step, bool showMessageOnBlocked)
        {
            if (step == null || !stepResultImages.TryGetValue(step, out Bitmap image) || image == null)
            {
                return false;
            }

            string layerName = string.IsNullOrWhiteSpace(step.OutputLayer) ? "Pipeline_Output" : step.OutputLayer.Trim();
            string blockMessage = ValidatePublishTarget(step, layerName);
            if (!string.IsNullOrWhiteSpace(blockMessage))
            {
                AppendLog($"PUBLISH NG | {step.Name} | {blockMessage}");
                if (showMessageOnBlocked)
                {
                    VisionMessageBox.Warning(this, "Pipeline Publish", blockMessage);
                }

                return false;
            }

            Bitmap publishImage = new Bitmap(image);
            int index = displayManager.FindIndex(layerName);
            if (index >= 0)
            {
                displayManager.SetLayerImage(index, publishImage);
                displayManager.RefreshLayer(index);
                displayManager.ActivateLayer(index);
            }
            else
            {
                displayManager.CreateLayerDisplay(publishImage, layerName, true);
                displayManager.ActivateLayer(layerName);
            }

            displayManager.AcceptLayerImageChanged(layerName);
            SetRunLogHint($"Publish complete. '{layerName}' was updated in the main workspace.");
            AppendLog($"PUBLISH OK | {step.Name} -> {layerName} | {image.Width} x {image.Height}");
            return true;
        }

        private static string ValidatePublishTarget(VisionPipelineStep step, string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                return "Output layer is empty.";
            }

            if (string.Equals(layerName, "Main", StringComparison.OrdinalIgnoreCase))
            {
                return "Output layer 'Main' is the source image layer. Change Output Layer before publishing.";
            }

            if (string.Equals(layerName, step?.InputLayer, StringComparison.OrdinalIgnoreCase))
            {
                return $"Output layer '{layerName}' is the same as the input layer. Change Output Layer before publishing.";
            }

            return string.Empty;
        }

        private void SaveRunReport(VisionPipelineRunResult result, DateTime startedAt, DateTime finishedAt, bool publishAllOutputs)
        {
            try
            {
                string reportPath = VisionPipelineRunReportStorage.Save(
                    recipeName,
                    pipeline,
                    result,
                    startedAt,
                    finishedAt,
                    publishAllOutputs);
                AppendLog($"REPORT | {reportPath}");
            }
            catch (Exception ex)
            {
                AppendLog($"REPORT NG | {ex.GetBaseException().Message}");
            }
        }

        private static bool IsStepResultPassed(VisionPipelineStepResult stepResult)
        {
            return VisionPipelineResultSummaryService.IsPassed(stepResult);
        }

        private void SavePipelineImages()
        {
            string directory = RecipeWorkspaceService.GetVisionPipelineImageDirectory(recipeName, pipeline.Name);
            List<string> manifestLines = new List<string>();
            Dictionary<string, int> usedFileNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> savedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                string title = displayManager.GetLayerTitle(i);
                Bitmap image = displayManager.GetLayerImage(i);
                if (image == null || string.IsNullOrWhiteSpace(title)) { continue; }

                string fileName = GetUniqueImageFileName(SanitizeFileName(title), usedFileNames);
                savedFileNames.Add(fileName);
                string path = Path.Combine(directory, fileName);
                using (Bitmap clone = new Bitmap(image))
                {
                    clone.Save(path, ImageFormat.Png);
                }

                manifestLines.Add($"{EscapeManifestValue(title)}\t{EscapeManifestValue(fileName)}");
            }

            File.WriteAllLines(GetPipelineImageManifestPath(directory), manifestLines);
            DeleteStalePipelineImages(directory, savedFileNames);
            SavePipelineStepPreviewImages(directory);
            SavePipelineProjectManifest(directory);
            AppendLog($"Images saved: {manifestLines.Count}");
        }

        private void SavePipelineProjectManifest(string pipelineImageDirectory)
        {
            try
            {
                string manifestPath = VisionPipelineProjectManifestStorage.Save(
                    recipeName,
                    pipeline,
                    displayManager,
                    pipelineImageDirectory);
                AppendLog($"Project manifest saved: {Path.GetFileName(manifestPath)}");
            }
            catch (Exception ex)
            {
                AppendLog($"Project manifest NG | {ex.GetBaseException().Message}");
            }
        }

        private void LoadPipelineImages(bool restoreLayerImages)
        {
            string directory = RecipeWorkspaceService.GetVisionPipelineImageDirectory(recipeName, pipeline.Name);
            string manifestPath = GetPipelineImageManifestPath(directory);

            int loadCount = 0;
            if (restoreLayerImages && File.Exists(manifestPath))
            {
                foreach (string line in File.ReadAllLines(manifestPath))
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length != 2) { continue; }

                    string title = UnescapeManifestValue(parts[0]);
                    string fileName = UnescapeManifestValue(parts[1]);
                    string imagePath = Path.Combine(directory, fileName);
                    if (string.IsNullOrWhiteSpace(title) || !File.Exists(imagePath)) { continue; }

                    using (Bitmap loaded = new Bitmap(imagePath))
                    {
                        Bitmap layerImage = new Bitmap(loaded);
                        int index = displayManager.FindIndex(title);
                        if (index >= 0)
                        {
                            displayManager.SetLayerImage(index, layerImage);
                            displayManager.RefreshLayer(index);
                        }
                        else
                        {
                            displayManager.CreateLayerDisplay(layerImage, title, true);
                        }
                    }

                    loadCount++;
                }
            }
            else if (!restoreLayerImages && File.Exists(manifestPath))
            {
                AppendLog("Images kept: saved pipeline images were not restored.");
            }

            if (loadCount > 0)
            {
                AppendLog($"Images loaded: {loadCount}");
            }

            int previewCount = LoadPipelineStepPreviewImages(directory);
            if (previewCount > 0)
            {
                AppendLog($"Step previews loaded: {previewCount}");
                ShowStepPreview(SelectedStep);
            }
        }

        private void SavePipelineStepPreviewImages(string pipelineImageDirectory)
        {
            string previewDirectory = GetStepPreviewDirectory(pipelineImageDirectory);
            Directory.CreateDirectory(previewDirectory);

            List<string> manifestLines = new List<string>();
            Dictionary<string, int> usedFileNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> savedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                if (step == null || !stepResultImages.TryGetValue(step, out Bitmap bitmap) || bitmap == null)
                {
                    continue;
                }

                string baseFileName = $"{i + 1:000}_{SanitizeFileName(step.Name)}_{SanitizeFileName(step.OutputLayer)}";
                string fileName = GetUniqueImageFileName(baseFileName, usedFileNames);
                string path = Path.Combine(previewDirectory, fileName);
                using (Bitmap clone = new Bitmap(bitmap))
                {
                    clone.Save(path, ImageFormat.Png);
                }

                string inputFileName = string.Empty;
                if (stepInputImages.TryGetValue(step, out Bitmap inputBitmap) && inputBitmap != null)
                {
                    inputFileName = GetUniqueImageFileName(baseFileName + "_input", usedFileNames);
                    string inputPath = Path.Combine(previewDirectory, inputFileName);
                    using (Bitmap clone = new Bitmap(inputBitmap))
                    {
                        clone.Save(inputPath, ImageFormat.Png);
                    }

                    savedFileNames.Add(inputFileName);
                }

                string overlayFileName = Path.GetFileNameWithoutExtension(fileName) + ".overlays.tsv";
                SaveStepOverlayFile(Path.Combine(previewDirectory, overlayFileName), step);
                savedFileNames.Add(fileName);
                savedFileNames.Add(overlayFileName);
                manifestLines.Add(string.Join(
                    "\t",
                    i.ToString(CultureInfo.InvariantCulture),
                    EscapeManifestValue(step.Name),
                    EscapeManifestValue(step.OutputLayer),
                    EscapeManifestValue(fileName),
                    EscapeManifestValue(overlayFileName),
                    EscapeManifestValue(inputFileName)));
            }

            File.WriteAllLines(GetStepPreviewManifestPath(previewDirectory), manifestLines);
            DeleteStalePipelineImages(previewDirectory, savedFileNames);
            if (manifestLines.Count > 0)
            {
                AppendLog($"Step previews saved: {manifestLines.Count}");
            }
        }

        private int LoadPipelineStepPreviewImages(string pipelineImageDirectory)
        {
            string previewDirectory = GetStepPreviewDirectory(pipelineImageDirectory);
            string manifestPath = GetStepPreviewManifestPath(previewDirectory);
            if (!File.Exists(manifestPath))
            {
                return 0;
            }

            int loadCount = 0;
            foreach (string line in File.ReadAllLines(manifestPath))
            {
                string[] parts = line.Split('\t');
                if (parts.Length < 4)
                {
                    continue;
                }

                int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int stepIndex);
                string stepName = UnescapeManifestValue(parts[1]);
                string outputLayer = UnescapeManifestValue(parts[2]);
                string fileName = UnescapeManifestValue(parts[3]);
                string overlayFileName = parts.Length >= 5 ? UnescapeManifestValue(parts[4]) : string.Empty;
                string inputFileName = parts.Length >= 6 ? UnescapeManifestValue(parts[5]) : string.Empty;
                string imagePath = Path.Combine(previewDirectory, fileName);
                string inputImagePath = string.IsNullOrWhiteSpace(inputFileName) ? string.Empty : Path.Combine(previewDirectory, inputFileName);
                VisionPipelineStep step = FindStepForPreview(stepIndex, stepName, outputLayer);
                if (step == null || !File.Exists(imagePath))
                {
                    continue;
                }

                using (Bitmap loaded = new Bitmap(imagePath))
                {
                    RemoveStepPreview(step);
                    stepResultImages[step] = new Bitmap(loaded);
                    SetPreviewLayerImage(step.OutputLayer, loaded);
                    LoadStepOverlayFile(Path.Combine(previewDirectory, overlayFileName), step);
                    UpdateStepPreviewImage(step);
                }

                if (!string.IsNullOrWhiteSpace(inputImagePath) && File.Exists(inputImagePath))
                {
                    using (Bitmap loadedInput = new Bitmap(inputImagePath))
                    {
                        RemoveStepInputImage(step);
                        stepInputImages[step] = new Bitmap(loadedInput);
                    }
                }

                SetStepStatus(step, PipelineStepRunStatus.Loaded);
                loadCount++;
            }

            return loadCount;
        }

        private VisionPipelineStep FindStepForPreview(int stepIndex, string stepName, string outputLayer)
        {
            VisionPipelineStep exact = pipeline.Steps.FirstOrDefault(step =>
                string.Equals(step?.Name, stepName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(step?.OutputLayer, outputLayer, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                return exact;
            }

            VisionPipelineStep byName = pipeline.Steps.FirstOrDefault(step =>
                string.Equals(step?.Name, stepName, StringComparison.OrdinalIgnoreCase));
            if (byName != null)
            {
                return byName;
            }

            return stepIndex >= 0 && stepIndex < pipeline.Steps.Count
                ? pipeline.Steps[stepIndex]
                : null;
        }

        private void SaveStepOverlayFile(string path, VisionPipelineStep step)
        {
            if (string.IsNullOrWhiteSpace(path) || step == null)
            {
                return;
            }

            stepOverlays.TryGetValue(step, out List<VisionToolOverlay> overlays);
            List<string> lines = new List<string>();
            foreach (VisionToolOverlay overlay in overlays ?? new List<VisionToolOverlay>())
            {
                if (overlay == null)
                {
                    continue;
                }

                lines.Add(string.Join(
                    "\t",
                    overlay.Kind,
                    EscapeManifestValue(overlay.Label),
                    overlay.Bounds.X.ToString(CultureInfo.InvariantCulture),
                    overlay.Bounds.Y.ToString(CultureInfo.InvariantCulture),
                    overlay.Bounds.Width.ToString(CultureInfo.InvariantCulture),
                    overlay.Bounds.Height.ToString(CultureInfo.InvariantCulture),
                    overlay.Center.X.ToString(CultureInfo.InvariantCulture),
                    overlay.Center.Y.ToString(CultureInfo.InvariantCulture),
                    overlay.Start.X.ToString(CultureInfo.InvariantCulture),
                    overlay.Start.Y.ToString(CultureInfo.InvariantCulture),
                    overlay.End.X.ToString(CultureInfo.InvariantCulture),
                    overlay.End.Y.ToString(CultureInfo.InvariantCulture),
                    overlay.Angle.ToString(CultureInfo.InvariantCulture),
                    EscapeManifestValue(FormatOverlayPoints(overlay.Points))));
            }

            File.WriteAllLines(path, lines);
        }

        private void LoadStepOverlayFile(string path, VisionPipelineStep step)
        {
            if (step == null || string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();
            foreach (string line in File.ReadAllLines(path))
            {
                string[] parts = line.Split('\t');
                if (parts.Length < 14)
                {
                    continue;
                }

                if (!Enum.TryParse(parts[0], out VisionToolOverlayKind kind))
                {
                    continue;
                }

                VisionToolOverlay overlay = new VisionToolOverlay
                {
                    Kind = kind,
                    Label = UnescapeManifestValue(parts[1]),
                    Bounds = new RectangleF(
                        ParseFloat(parts[2]),
                        ParseFloat(parts[3]),
                        ParseFloat(parts[4]),
                        ParseFloat(parts[5])),
                    Center = new PointF(ParseFloat(parts[6]), ParseFloat(parts[7])),
                    Start = new PointF(ParseFloat(parts[8]), ParseFloat(parts[9])),
                    End = new PointF(ParseFloat(parts[10]), ParseFloat(parts[11])),
                    Angle = ParseDouble(parts[12])
                };
                foreach (PointF point in ParseOverlayPoints(UnescapeManifestValue(parts[13])))
                {
                    overlay.Points.Add(point);
                }

                overlays.Add(overlay);
            }

            stepOverlays[step] = overlays;
        }

        private static string FormatOverlayPoints(IEnumerable<PointF> points)
        {
            return string.Join(
                ";",
                (points ?? Enumerable.Empty<PointF>()).Select(point =>
                    point.X.ToString(CultureInfo.InvariantCulture) + "," + point.Y.ToString(CultureInfo.InvariantCulture)));
        }

        private static IEnumerable<PointF> ParseOverlayPoints(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                yield break;
            }

            foreach (string item in text.Split(';'))
            {
                string[] parts = item.Split(',');
                if (parts.Length != 2)
                {
                    continue;
                }

                yield return new PointF(ParseFloat(parts[0]), ParseFloat(parts[1]));
            }
        }

        private static float ParseFloat(string text)
        {
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float value) ? value : 0F;
        }

        private static double ParseDouble(string text)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ? value : 0D;
        }

        private void OnTreeStepSelected(object sender, TreeViewEventArgs e)
        {
            VisionPipelineStep step = e.Node?.Tag as VisionPipelineStep;
            BindStepProperty(step);
            ShowStepPreview(step);
            RefreshPipelineFlowPreview();
        }

        private void OnStepPropertyValueChanged(object sender, PropertyGridPropertyValueChangedEventArgs e)
        {
            if (isBindingStepProperty)
            {
                return;
            }

            OnStepPropertyChanged();
            ApplyPipelinePropertyGridVisibility();
        }

        private void OnStepPropertyChanged()
        {
            ApplySelectedStepProperty();
            UpdatePipelineFromView();
            UpdateSelectedTreeNodeText();
            RefreshSelectedStepIoPanel();
            ShowStepPreview(SelectedStep);
            RefreshPipelineFlowPreview();
        }

        private void OnStepIoChanged(object sender, EventArgs e)
        {
            if (isUpdatingStepIoPanel)
            {
                return;
            }

            ApplySelectedStepIoChanges();
        }

        private void OnOverlayOptionChanged(object sender, EventArgs e)
        {
            if (isUpdatingPreviewImageMode)
            {
                return;
            }

            if (ReferenceEquals(sender, cbPreviewImageMode))
            {
                currentPreviewImageMode = ParsePreviewMode(cbPreviewImageMode?.SelectedItem?.ToString());
            }

            ApplyPreviewModeState();
            RefreshCachedStepPreviewImages();
            RefreshPipelineSummaryPreviewImage();
            ShowStepPreview(SelectedStep);
        }

        private void SetCurrentPreviewImageMode(StepPreviewImageMode mode, bool refresh = true)
        {
            currentPreviewImageMode = mode;
            if (cbPreviewImageMode != null)
            {
                string modeText = FormatPreviewMode(mode);
                if (!string.Equals(cbPreviewImageMode.SelectedItem?.ToString(), modeText, StringComparison.OrdinalIgnoreCase))
                {
                    isUpdatingPreviewImageMode = true;
                    try
                    {
                        cbPreviewImageMode.SelectedItem = modeText;
                    }
                    finally
                    {
                        isUpdatingPreviewImageMode = false;
                    }
                }
            }

            ApplyPreviewModeState();
            pipelineFlowView?.SelectStep(SelectedStepIndex, ToPipelineFlowPreviewMode(mode));
            if (refresh)
            {
                RefreshCachedStepPreviewImages();
                RefreshPipelineSummaryPreviewImage();
                ShowStepPreview(SelectedStep);
            }
        }

        private void ApplyPreviewModeState()
        {
            bool showOverlay = GetCurrentPreviewImageMode() == StepPreviewImageMode.Overlay
                || GetCurrentPreviewImageMode() == StepPreviewImageMode.Summary;
            if (previewModeLabel != null)
            {
                previewModeLabel.Text = FormatPreviewMode(GetCurrentPreviewImageMode());
                previewModeLabel.BackColor = ResolvePreviewModeBackColor(GetCurrentPreviewImageMode());
            }

            if (overlayLabelModeLabel != null)
            {
                overlayLabelModeLabel.Enabled = showOverlay;
            }

            if (cbOverlayLabelMode != null)
            {
                cbOverlayLabelMode.Enabled = showOverlay;
            }

            if (overlayPointLimitLabel != null)
            {
                overlayPointLimitLabel.Enabled = showOverlay;
            }

            if (nudOverlayPointLimit != null)
            {
                nudOverlayPointLimit.Enabled = showOverlay;
            }
        }

        private void OnOpenPreviewClicked(object sender, EventArgs e)
        {
            OpenSelectedStepPreview();
        }

        private void OnResultGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || resultGrid?.Rows[e.RowIndex]?.Tag == null)
            {
                OpenSelectedStepPreview();
                return;
            }

            if (resultGrid.Rows[e.RowIndex].Tag is ResultGridRowTag tag)
            {
                OpenResultGridAction(tag, openViewer: true);
            }
        }

        private void OnResultGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || resultGrid?.Rows[e.RowIndex]?.Tag is not ResultGridRowTag tag)
            {
                return;
            }

            if (IsPreviewResultGridAction(tag.Action))
            {
                OpenResultGridAction(tag, openViewer: false);
            }
        }

        private void OpenResultGridAction(ResultGridRowTag tag, bool openViewer)
        {
            if (tag == null)
            {
                if (openViewer)
                {
                    OpenSelectedStepPreview();
                }

                return;
            }

            switch (tag.Action)
            {
                case ResultGridRowAction.Metrics:
                    if (openViewer)
                    {
                        ShowSelectedStepMetrics();
                    }

                    break;
                case ResultGridRowAction.Overlays:
                    SwitchSelectedStepPreviewMode(StepPreviewImageMode.Overlay, openViewer);
                    break;
                case ResultGridRowAction.Overlay:
                    SwitchSelectedStepPreviewMode(StepPreviewImageMode.Overlay, openViewer, tag.OverlayIndex);
                    break;
                case ResultGridRowAction.PreviewInput:
                    SwitchSelectedStepPreviewMode(StepPreviewImageMode.Input, openViewer);
                    break;
                case ResultGridRowAction.PreviewOutput:
                    SwitchSelectedStepPreviewMode(StepPreviewImageMode.Output, openViewer);
                    break;
                case ResultGridRowAction.PreviewOverlay:
                    SwitchSelectedStepPreviewMode(StepPreviewImageMode.Overlay, openViewer);
                    break;
                default:
                    if (openViewer)
                    {
                        OpenSelectedStepPreview();
                    }

                    break;
            }
        }

        private static bool IsPreviewResultGridAction(ResultGridRowAction action)
        {
            return action == ResultGridRowAction.PreviewInput
                || action == ResultGridRowAction.PreviewOutput
                || action == ResultGridRowAction.PreviewOverlay
                || action == ResultGridRowAction.Overlay
                || action == ResultGridRowAction.Overlays;
        }

        private void SwitchSelectedStepPreviewMode(StepPreviewImageMode mode, bool openViewer, int overlayIndex = -1)
        {
            SetCurrentPreviewImageMode(mode, refresh: true);
            if (openViewer)
            {
                OpenSelectedStepPreview(mode == StepPreviewImageMode.Overlay ? overlayIndex : -1);
            }
        }

        private void OpenSelectedStepPreview(int initialOverlayIndex = -1)
        {
            if (GetCurrentPreviewImageMode() == StepPreviewImageMode.Summary)
            {
                OpenPipelineSummaryPreview();
                return;
            }

            VisionPipelineStep step = SelectedStep;
            if (step == null)
            {
                AppendLog("VIEW | Select a step before opening preview.");
                return;
            }

            StepPreviewImageMode mode = GetCurrentPreviewImageMode();
            if (!TryGetStepPreviewSource(step, mode, out Bitmap source) || source == null)
            {
                AppendLog($"VIEW | {step.Name} has no {FormatPreviewMode(mode)} image.");
                return;
            }

            List<VisionToolOverlay> overlays = null;
            if (mode == StepPreviewImageMode.Overlay)
            {
                stepOverlays.TryGetValue(step, out overlays);
                if (stepResultImages.TryGetValue(step, out Bitmap rawSource) && rawSource != null)
                {
                    source = rawSource;
                }
            }

            stepResultSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary);
            using (Bitmap clone = new Bitmap(source))
            using (FormVisionPipelineImageViewer viewer = new FormVisionPipelineImageViewer(
                $"{step.Name} - {FormatPreviewMode(mode)}",
                clone,
                overlays,
                summary,
                GetCurrentOverlayLabelMode(),
                nudOverlayPointLimit == null ? 300 : (int)nudOverlayPointLimit.Value,
                mode == StepPreviewImageMode.Overlay ? initialOverlayIndex : -1))
            {
                VisionPipelineDialogService.ShowDialog(viewer, this);
            }
        }

        private void OpenPipelineSummaryPreview()
        {
            if (pipelineSummaryPreviewImage == null || pipelineSummarySourceImage == null)
            {
                AppendLog("VIEW | Summary has no preview image. Run Preview first.");
                return;
            }

            using (Bitmap clone = new Bitmap(pipelineSummarySourceImage))
            using (FormVisionPipelineImageViewer viewer = new FormVisionPipelineImageViewer(
                "Pipeline Summary - All detections",
                clone,
                pipelineSummaryOverlays,
                null,
                GetCurrentOverlayLabelMode(),
                nudOverlayPointLimit == null ? 300 : (int)nudOverlayPointLimit.Value,
                -1))
            {
                VisionPipelineDialogService.ShowDialog(viewer, this);
            }
        }

        private void ShowSelectedStepMetrics()
        {
            VisionPipelineStep step = SelectedStep;
            if (step == null)
            {
                AppendLog("VIEW | Select a step before opening metrics.");
                return;
            }

            if (!stepResultSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary)
                || summary?.Metrics == null
                || summary.Metrics.Count == 0)
            {
                VisionMessageBox.Info(this, "Pipeline Metrics", "No metrics are available. Run Preview first.");
                return;
            }

            List<string> detailLines = new List<string>
            {
                $"Step: {step.Name}",
                $"Tool: {step.ToolType}",
                $"Status: {FormatStepResultStatus(GetStepStatus(step))}",
                $"Flow: {FormatPipelineFlow(step.InputLayer, step.OutputLayer)}",
                $"Result: {BuildStepResultHeadline(step, summary)}"
            };

            if (summary.ElapsedMilliseconds > 0)
            {
                detailLines.Add($"Elapsed: {summary.ElapsedMilliseconds:0.0} ms");
            }

            if (step.UseAcceptance)
            {
                string acceptanceText = BuildStepMetricBadgeText(step, summary);
                if (string.IsNullOrWhiteSpace(acceptanceText))
                {
                    acceptanceText = FormatAcceptanceConfiguration(step);
                }

                detailLines.Add($"Acceptance: {acceptanceText}");
            }

            detailLines.Add(string.Empty);
            detailLines.Add("Metrics");
            foreach (KeyValuePair<string, double> metric in OrderResultMetrics(step, summary.Metrics))
            {
                detailLines.Add($"{FormatMetricRowName(metric.Key)}: {FormatMetricValue(metric.Value)}");
            }

            VisionMessageBox.Show(this, new VisionMessageOptions
            {
                Title = "Pipeline Metrics",
                Message = $"{step.Name}\r\n{BuildStepResultHeadline(step, summary)}",
                Details = string.Join(Environment.NewLine, detailLines),
                Kind = VisionMessageKind.Info,
                Buttons = MessageBoxButtons.OK
            });
        }

        private void UpdatePipelineFromView()
        {
            ApplySelectedStepProperty();
            pipeline.Name = string.IsNullOrWhiteSpace(tbPipelineName.Text) ? "Pipeline" : tbPipelineName.Text.Trim();
            if (treeSteps.Nodes.Count > 0)
            {
                treeSteps.Nodes[0].Text = pipeline.Name;
            }

            RefreshPipelineFlowPreview();
        }

        private void BindStepProperty(VisionPipelineStep step)
        {
            isBindingStepProperty = true;
            try
            {
                selectedStepProperty = VisionPipelineStepPropertyMapper.CreateProperty(step);
                propertyGridStep.SelectedObject = selectedStepProperty;
                ApplyPipelinePropertyGridVisibility();
                propertiesCaption.Text = step == null ? "Properties" : $"Properties - {step.Name}";
                RefreshSelectedStepIoPanel();
            }
            finally
            {
                isBindingStepProperty = false;
            }
        }

        private void RefreshSelectedStepIoPanel()
        {
            if (stepIoPanel == null || cbStepInputLayer == null || tbStepOutputLayer == null || btnStepChainInput == null)
            {
                return;
            }

            VisionPipelineStep step = SelectedStep;
            isUpdatingStepIoPanel = true;
            try
            {
                bool hasStep = step != null;
                stepIoPanel.Enabled = hasStep;
                cbStepInputLayer.Items.Clear();

                if (hasStep)
                {
                    List<string> layerNames = GetPipelineLayerTitles()
                        .Where(layer => !string.IsNullOrWhiteSpace(layer))
                        .Select(layer => layer.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    string inputLayer = string.IsNullOrWhiteSpace(step.InputLayer) ? "Main" : step.InputLayer.Trim();
                    if (!layerNames.Any(layer => string.Equals(layer, inputLayer, StringComparison.OrdinalIgnoreCase)))
                    {
                        layerNames.Insert(0, inputLayer);
                    }

                    foreach (string layerName in layerNames)
                    {
                        cbStepInputLayer.Items.Add(layerName);
                    }

                    SelectComboItem(cbStepInputLayer, inputLayer);
                    if (cbStepInputLayer.SelectedIndex < 0 && cbStepInputLayer.Items.Count > 0)
                    {
                        cbStepInputLayer.SelectedIndex = 0;
                    }

                    tbStepOutputLayer.Text = step.OutputLayer ?? string.Empty;
                }
                else
                {
                    tbStepOutputLayer.Text = string.Empty;
                }

                bool canChain = CanChainSelectedStepInput(out string previousOutput, out _);
                bool isLinked = hasStep
                    && VisionPipelineNormalizer.IsLinkedToPreviousEnabledOutput(pipeline, SelectedStepIndex, step.InputLayer);
                btnStepChainInput.Enabled = canChain;
                btnStepChainInput.Tag = previousOutput;
                UpdateStepIoStateView(step, hasStep, previousOutput, canChain, isLinked);
            }
            finally
            {
                isUpdatingStepIoPanel = false;
            }
        }

        private void UpdateStepIoStateView(VisionPipelineStep step, bool hasStep, string previousOutput, bool canChain, bool isLinked)
        {
            if (btnStepChainInput == null)
            {
                return;
            }

            Color sourceBackColor = Color.FromArgb(236, 244, 252);
            Color sourceForeColor = Color.FromArgb(35, 85, 132);
            Color linkedBackColor = Color.FromArgb(232, 248, 239);
            Color linkedForeColor = Color.FromArgb(0, 112, 74);
            Color branchBackColor = Color.FromArgb(255, 245, 226);
            Color branchForeColor = Color.FromArgb(168, 92, 0);
            Color disabledBackColor = Color.FromArgb(244, 247, 250);
            Color disabledForeColor = Color.FromArgb(92, 98, 108);

            string inputLayer = step == null || string.IsNullOrWhiteSpace(step.InputLayer)
                ? "Input?"
                : step.InputLayer.Trim();

            if (!hasStep)
            {
                btnStepChainInput.Text = "Source";
                ApplyStepIoStateStyle(disabledBackColor, disabledForeColor);
                SetStepIoStatus("Select a step to edit image flow.", disabledBackColor, disabledForeColor);
                return;
            }

            if (!string.IsNullOrWhiteSpace(previousOutput) && isLinked)
            {
                btnStepChainInput.Text = "Linked";
                ApplyStepIoStateStyle(linkedBackColor, linkedForeColor);
                SetStepIoStatus($"Linked: INPUT follows previous OUTPUT '{previousOutput}'.", linkedBackColor, linkedForeColor);
                return;
            }

            if (canChain)
            {
                btnStepChainInput.Text = "Link Prev";
                ApplyStepIoStateStyle(branchBackColor, branchForeColor);
                SetStepIoStatus($"Branch: INPUT reads '{inputLayer}' instead of previous OUTPUT '{previousOutput}'.", branchBackColor, branchForeColor);
                return;
            }

            btnStepChainInput.Text = "Source";
            ApplyStepIoStateStyle(sourceBackColor, sourceForeColor);
            SetStepIoStatus($"Source: INPUT starts from '{inputLayer}'.", sourceBackColor, sourceForeColor);
        }

        private void ApplyStepIoStateStyle(Color backColor, Color foreColor)
        {
            if (btnStepChainInput == null)
            {
                return;
            }

            btnStepChainInput.BackColor = backColor;
            btnStepChainInput.ForeColor = foreColor;
            btnStepChainInput.FlatAppearance.BorderColor = foreColor;
        }

        private void SetStepIoStatus(string text, Color backColor, Color foreColor)
        {
            if (stepIoStatusLabel == null)
            {
                return;
            }

            stepIoStatusLabel.Text = text;
            stepIoStatusLabel.BackColor = backColor;
            stepIoStatusLabel.ForeColor = foreColor;
        }

        private void ApplyPipelinePropertyGridVisibility()
        {
            propertyGridVisibilityBinder.ApplyVisibilityRules(propertyGridStep);
            HidePipelineHandledPropertiesInPropertyGrid();
            HideLinkedInspectionPreprocessingInPropertyGrid();
        }

        private void HidePipelineHandledPropertiesInPropertyGrid()
        {
            SetPropertyGridPropertyBrowsable(propertyGridStep, "InputLayer", false);
            SetPropertyGridPropertyBrowsable(propertyGridStep, "OutputLayer", false);
        }

        private void HideLinkedInspectionPreprocessingInPropertyGrid()
        {
            VisionPipelineStep step = SelectedStep;
            if (step == null
                || !VisionPipelineNormalizer.IsOpenCvInspectionToolWithInternalThreshold(VisionPipelineNormalizer.NormalizeToolType(step.ToolType))
                || !VisionPipelineNormalizer.IsLinkedToPreviousEnabledOutput(pipeline, SelectedStepIndex, step.InputLayer))
            {
                return;
            }

            string[] propertyNames =
            {
                "USE_THRESHOLD",
                "USE_BITWISENOT",
                "THRESHOLD_TYPES",
                "THRESHOLD",
                "USE_ADAPTIVE_THRESHOLD",
                "ADAPTIVE_THRESHOLD",
                "ADAPTIVE_THRESHOLD_TYPES",
                "ADAPTIVE_THRESHOLD_ALGORITHM",
                "BlockSize",
                "Weight"
            };

            foreach (string propertyName in propertyNames)
            {
                SetPropertyGridPropertyBrowsable(propertyGridStep, propertyName, false);
            }
        }

        private static void SetPropertyGridPropertyBrowsable(IPropertyGridView propertyGrid, string propertyName, bool isBrowsable)
        {
            var property = propertyGrid?.Properties?[propertyName];
            if (property != null)
            {
                property.IsBrowsable = isBrowsable;
            }
        }

        private void ApplySelectedStepIoChanges()
        {
            VisionPipelineStep step = SelectedStep;
            int index = SelectedStepIndex;
            if (step == null || index < 0)
            {
                return;
            }

            string inputLayer = cbStepInputLayer.SelectedItem?.ToString()?.Trim() ?? step.InputLayer;
            string outputLayer = tbStepOutputLayer.Text?.Trim() ?? string.Empty;
            bool changed = !string.Equals(step.InputLayer, inputLayer, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(step.OutputLayer, outputLayer, StringComparison.OrdinalIgnoreCase);
            if (!changed)
            {
                RefreshSelectedStepIoPanel();
                return;
            }

            step.InputLayer = inputLayer;
            step.OutputLayer = outputLayer;
            string normalizationMessage = NormalizeChainedInspectionPreprocessing(step, index);
            InvalidateStepResultsFrom(index);
            LoadLayerItems();
            BindStepProperty(step);
            UpdateSelectedTreeNodeText();
            RefreshPipelineFlowPreview();
            ShowStepPreview(step);
            SetRunLogHint("Step I/O changed. Run Preview again.");
            if (!string.IsNullOrWhiteSpace(normalizationMessage))
            {
                AppendLog(normalizationMessage);
            }
        }

        private void ApplySelectedStepProperty()
        {
            if (isBindingStepProperty || selectedStepProperty == null || SelectedStep == null)
            {
                return;
            }

            VisionPipelineStepPropertyMapper.ApplyProperty(SelectedStep, selectedStepProperty);
        }

        private void CacheStepPreview(VisionPipelineStepResult stepResult)
        {
            if (stepResult?.Step == null || stepResult.ToolResult?.ResultImage == null || stepResult.ToolResult.ResultImage.Empty())
            {
                return;
            }

            RemoveStepPreviewImage(stepResult.Step);
            RemoveStepResultImage(stepResult.Step);
            using (Bitmap bitmap = BitmapImageConverter.ToBitmap(stepResult.ToolResult.ResultImage))
            {
                stepResultImages[stepResult.Step] = new Bitmap(bitmap);
            }

            if (stepResultImages.TryGetValue(stepResult.Step, out Bitmap resultBitmap))
            {
                SetPreviewLayerImage(stepResult.Step.OutputLayer, resultBitmap);
            }

            UpdateStepPreviewImage(stepResult.Step);
        }

        private void CacheStepInputPreview(VisionPipelineStep step)
        {
            if (step == null || string.IsNullOrWhiteSpace(step.InputLayer))
            {
                return;
            }

            if (!previewLayerImages.TryGetValue(step.InputLayer.Trim(), out Bitmap input) || input == null)
            {
                return;
            }

            RemoveStepInputImage(step);
            stepInputImages[step] = new Bitmap(input);
        }

        private void SetPreviewLayerImage(string layerName, Bitmap image)
        {
            if (string.IsNullOrWhiteSpace(layerName) || image == null)
            {
                return;
            }

            string key = layerName.Trim();
            RemovePreviewLayerImage(key);
            previewLayerImages[key] = new Bitmap(image);
        }

        private void RemovePreviewLayerImage(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                return;
            }

            string key = layerName.Trim();
            if (previewLayerImages.TryGetValue(key, out Bitmap bitmap))
            {
                bitmap.Dispose();
                previewLayerImages.Remove(key);
            }
        }

        private void ClearPreviewLayerImages()
        {
            foreach (Bitmap bitmap in previewLayerImages.Values)
            {
                bitmap.Dispose();
            }

            previewLayerImages.Clear();
        }

        private void CacheStepResultSummary(VisionPipelineStepResult stepResult)
        {
            if (stepResult?.Step == null)
            {
                return;
            }

            int index = pipeline.Steps.IndexOf(stepResult.Step) + 1;
            stepResultSummaries[stepResult.Step] = VisionPipelineResultSummaryService.CreateStepSummary(index, stepResult);
        }

        private void CacheStepOverlays(VisionPipelineStepResult stepResult)
        {
            if (stepResult?.Step == null)
            {
                return;
            }

            stepOverlays[stepResult.Step] = stepResult.ToolResult?.Overlays != null
                ? stepResult.ToolResult.Overlays.ToList()
                : new List<VisionToolOverlay>();
        }

        private void RefreshPipelineSummaryPreviewImage()
        {
            ClearPipelineSummaryPreviewImage();
            pipelineSummaryOverlays.Clear();

            if (!TryGetPipelineSummaryBaseImage(out Bitmap source) || source == null)
            {
                return;
            }

            List<VisionPipelineStep> steps = pipeline?.Steps == null
                ? new List<VisionPipelineStep>()
                : pipeline.Steps.ToList();

            if (steps.Count == 0)
            {
                return;
            }

            pipelineSummarySourceImage = new Bitmap(source);
            Bitmap preview = new Bitmap(source);
            OverlayLabelMode labelMode = GetCurrentOverlayLabelMode();
            int pointLimit = nudOverlayPointLimit == null ? 300 : (int)nudOverlayPointLimit.Value;
            int overlayGroupCount = 0;

            using (Graphics graphics = Graphics.FromImage(preview))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                for (int i = 0; i < steps.Count; i++)
                {
                    VisionPipelineStep step = steps[i];
                    if (step == null
                        || !stepOverlays.TryGetValue(step, out List<VisionToolOverlay> overlays)
                        || overlays == null
                        || overlays.Count == 0)
                    {
                        continue;
                    }

                    List<VisionToolOverlay> visibleOverlays = overlays
                        .Where(overlay => overlay != null)
                        .Select(CloneOverlay)
                        .ToList();
                    if (visibleOverlays.Count == 0)
                    {
                        continue;
                    }

                    overlayGroupCount++;
                    pipelineSummaryOverlays.AddRange(visibleOverlays.Select(CloneOverlay));
                    PipelineStepRunStatus status = GetStepStatus(step);
                    Color overlayColor = ResolveSummaryOverlayColor(overlayGroupCount, status);
                    Color textBackColor = Color.FromArgb(
                        190,
                        Math.Max(0, overlayColor.R - 35),
                        Math.Max(0, overlayColor.G - 35),
                        Math.Max(0, overlayColor.B - 35));
                    DrawOverlays(graphics, visibleOverlays, preview.Size, labelMode, pointLimit, overlayColor, textBackColor);
                }

                DrawPipelineSummaryBadge(graphics, preview.Size);
            }

            pipelineSummaryPreviewImage = preview;
        }

        private bool TryGetPipelineSummaryBaseImage(out Bitmap bitmap)
        {
            bitmap = null;

            if (previewLayerImages.TryGetValue("Main", out bitmap) && bitmap != null)
            {
                return true;
            }

            VisionPipelineStep firstEnabledStep = pipeline?.Steps?
                .FirstOrDefault(step => step != null && step.Enabled);
            if (firstEnabledStep != null && TryResolveInputImageFromLayerCache(firstEnabledStep, out bitmap) && bitmap != null)
            {
                return true;
            }

            bitmap = stepInputImages.Values.FirstOrDefault(image => image != null)
                ?? stepResultImages.Values.FirstOrDefault(image => image != null);
            return bitmap != null;
        }

        private static VisionToolOverlay CloneOverlay(VisionToolOverlay overlay)
        {
            if (overlay == null)
            {
                return null;
            }

            VisionToolOverlay clone = new VisionToolOverlay
            {
                Kind = overlay.Kind,
                Label = overlay.Label,
                Bounds = overlay.Bounds,
                Center = overlay.Center,
                Start = overlay.Start,
                End = overlay.End,
                Angle = overlay.Angle
            };

            foreach (PointF point in overlay.Points ?? Enumerable.Empty<PointF>())
            {
                clone.Points.Add(point);
            }

            return clone;
        }

        private static Color ResolveSummaryOverlayColor(int groupIndex, PipelineStepRunStatus status)
        {
            if (IsFailureStatus(status))
            {
                return ResolveOverlayColor(status);
            }

            switch ((groupIndex - 1) % 4)
            {
                case 0:
                    return Color.FromArgb(0, 210, 120);
                case 1:
                    return Color.FromArgb(0, 180, 220);
                case 2:
                    return Color.FromArgb(255, 190, 40);
                default:
                    return Color.FromArgb(190, 120, 255);
            }
        }

        private void DrawPipelineSummaryBadge(Graphics graphics, DrawingSize imageSize)
        {
            if (imageSize.Width <= 12 || imageSize.Height <= 12)
            {
                return;
            }

            int overlayStepCount = stepOverlays.Count(pair => pair.Value != null && pair.Value.Count > 0);
            string text = $"SUMMARY | Steps {overlayStepCount} | Overlays {pipelineSummaryOverlays.Count}";
            using (Font font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (Brush backBrush = new SolidBrush(Color.FromArgb(190, 21, 128, 118)))
            using (StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            })
            {
                SizeF textSize = graphics.MeasureString(text, font);
                float width = Math.Min(Math.Max(150F, textSize.Width + 16F), Math.Max(12F, imageSize.Width - 12F));
                float height = Math.Min(Math.Max(22F, textSize.Height + 8F), Math.Max(12F, imageSize.Height - 12F));
                RectangleF bounds = new RectangleF(6F, 6F, width, height);
                graphics.FillRectangle(backBrush, bounds);
                graphics.DrawRectangle(Pens.White, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                graphics.DrawString(text, font, textBrush, new RectangleF(bounds.X + 8F, bounds.Y + 2F, bounds.Width - 12F, bounds.Height - 4F), format);
            }
        }

        private void ClearPipelineSummaryPreviewImage()
        {
            pipelineSummarySourceImage?.Dispose();
            pipelineSummarySourceImage = null;
            pipelineSummaryPreviewImage?.Dispose();
            pipelineSummaryPreviewImage = null;
        }

        private Bitmap RenderStepPreviewBitmap(VisionPipelineStep step, Bitmap source)
        {
            Bitmap preview = new Bitmap(source);
            PipelineStepRunStatus status = GetStepStatus(step);
            if (step == null)
            {
                return preview;
            }

            OverlayLabelMode labelMode = GetCurrentOverlayLabelMode();
            int pointLimit = nudOverlayPointLimit == null ? 300 : (int)nudOverlayPointLimit.Value;
            stepOverlays.TryGetValue(step, out List<VisionToolOverlay> overlays);
            using (Graphics graphics = Graphics.FromImage(preview))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Color overlayColor = ResolveOverlayColor(status);
                Color textBackColor = ResolveOverlayTextBackColor(status);
                if (overlays != null && overlays.Count > 0)
                {
                    DrawOverlays(graphics, overlays, preview.Size, labelMode, pointLimit, overlayColor, textBackColor);
                }

                if (labelMode != OverlayLabelMode.None)
                {
                    DrawStepResultBadge(graphics, step, preview.Size, status, textBackColor);
                }
            }

            return preview;
        }

        private void RefreshCachedStepPreviewImages()
        {
            foreach (VisionPipelineStep step in stepResultImages.Keys.ToList())
            {
                UpdateStepPreviewImage(step);
            }
        }

        private void UpdateStepPreviewImage(VisionPipelineStep step)
        {
            if (step == null || !stepResultImages.TryGetValue(step, out Bitmap source) || source == null)
            {
                return;
            }

            RemoveStepPreviewImage(step);
            stepPreviewImages[step] = RenderStepPreviewBitmap(step, source);
        }

        private static void DrawOverlays(
            Graphics graphics,
            IEnumerable<VisionToolOverlay> overlays,
            DrawingSize imageSize,
            OverlayLabelMode labelMode,
            int pointLimit,
            Color overlayColor,
            Color textBackColor)
        {
            using (Pen boxPen = new Pen(overlayColor, 2F))
            using (Pen centerPen = new Pen(Color.FromArgb(245, overlayColor), 2F))
            using (Brush pointBrush = new SolidBrush(Color.FromArgb(210, overlayColor)))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (Brush textBackBrush = new SolidBrush(textBackColor))
            using (Font font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                foreach (VisionToolOverlay overlay in overlays ?? Enumerable.Empty<VisionToolOverlay>())
                {
                    if (overlay == null)
                    {
                        continue;
                    }

                    switch (overlay.Kind)
                    {
                        case VisionToolOverlayKind.Rectangle:
                            DrawRectangleOverlay(graphics, overlay, imageSize, boxPen, centerPen, textBrush, textBackBrush, font, labelMode);
                            break;
                        case VisionToolOverlayKind.Point:
                            DrawCenterMarker(graphics, overlay.Center, imageSize, centerPen, pointBrush);
                            if (labelMode != OverlayLabelMode.None)
                            {
                                DrawOverlayLabel(graphics, FormatOverlayLabel(overlay.Label, labelMode), overlay.Center, imageSize, textBrush, textBackBrush, font);
                            }
                            break;
                        case VisionToolOverlayKind.Points:
                            DrawPointOverlay(graphics, overlay, imageSize, pointBrush, pointLimit);
                            break;
                        case VisionToolOverlayKind.Line:
                            DrawLineOverlay(graphics, overlay, imageSize, boxPen, centerPen, textBrush, textBackBrush, font, labelMode);
                            break;
                    }
                }
            }
        }

        private OverlayLabelMode GetCurrentOverlayLabelMode()
        {
            string text = cbOverlayLabelMode?.SelectedItem?.ToString() ?? "No";
            if (string.Equals(text, "Details", StringComparison.OrdinalIgnoreCase))
            {
                return OverlayLabelMode.Details;
            }

            if (string.Equals(text, "None", StringComparison.OrdinalIgnoreCase))
            {
                return OverlayLabelMode.None;
            }

            return OverlayLabelMode.Number;
        }

        private StepPreviewImageMode GetCurrentPreviewImageMode()
        {
            return currentPreviewImageMode;
        }

        private static StepPreviewImageMode ParsePreviewMode(string text)
        {
            if (string.Equals(text, "Summary", StringComparison.OrdinalIgnoreCase))
            {
                return StepPreviewImageMode.Summary;
            }

            if (string.Equals(text, "Input", StringComparison.OrdinalIgnoreCase))
            {
                return StepPreviewImageMode.Input;
            }

            if (string.Equals(text, "Output", StringComparison.OrdinalIgnoreCase))
            {
                return StepPreviewImageMode.Output;
            }

            return StepPreviewImageMode.Overlay;
        }

        private static StepPreviewImageMode ToStepPreviewImageMode(PipelineFlowPreviewMode mode)
        {
            switch (mode)
            {
                case PipelineFlowPreviewMode.Input:
                    return StepPreviewImageMode.Input;
                case PipelineFlowPreviewMode.Output:
                    return StepPreviewImageMode.Output;
                default:
                    return StepPreviewImageMode.Overlay;
            }
        }

        private static PipelineFlowPreviewMode ToPipelineFlowPreviewMode(StepPreviewImageMode mode)
        {
            switch (mode)
            {
                case StepPreviewImageMode.Input:
                    return PipelineFlowPreviewMode.Input;
                case StepPreviewImageMode.Output:
                    return PipelineFlowPreviewMode.Output;
                default:
                    return PipelineFlowPreviewMode.Overlay;
            }
        }

        private static Color ResolvePreviewModeBackColor(StepPreviewImageMode mode)
        {
            switch (mode)
            {
                case StepPreviewImageMode.Summary:
                    return Color.FromArgb(21, 128, 118);
                case StepPreviewImageMode.Input:
                    return Color.FromArgb(18, 116, 76);
                case StepPreviewImageMode.Output:
                    return Color.FromArgb(39, 89, 145);
                default:
                    return Color.FromArgb(47, 111, 171);
            }
        }

        private static string FormatPreviewMode(StepPreviewImageMode mode)
        {
            switch (mode)
            {
                case StepPreviewImageMode.Summary:
                    return "Summary";
                case StepPreviewImageMode.Input:
                    return "Input";
                case StepPreviewImageMode.Output:
                    return "Output";
                default:
                    return "Overlay";
            }
        }

        private static void DrawRectangleOverlay(
            Graphics graphics,
            VisionToolOverlay overlay,
            DrawingSize imageSize,
            Pen boxPen,
            Pen centerPen,
            Brush textBrush,
            Brush textBackBrush,
            Font font,
            OverlayLabelMode labelMode)
        {
            RectangleF bounds = ClampRectangle(overlay.Bounds, imageSize);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            graphics.DrawRectangle(boxPen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            DrawCenterMarker(graphics, overlay.Center, imageSize, centerPen, null);
            if (labelMode != OverlayLabelMode.None)
            {
                DrawOverlayLabel(graphics, FormatOverlayLabel(overlay.Label, labelMode), new PointF(bounds.X, bounds.Y), imageSize, textBrush, textBackBrush, font);
            }
        }

        private static void DrawLineOverlay(
            Graphics graphics,
            VisionToolOverlay overlay,
            DrawingSize imageSize,
            Pen linePen,
            Pen centerPen,
            Brush textBrush,
            Brush textBackBrush,
            Font font,
            OverlayLabelMode labelMode)
        {
            PointF start = ClampPoint(overlay.Start, imageSize);
            PointF end = ClampPoint(overlay.End, imageSize);
            if (Math.Abs(start.X - end.X) < 0.000001 && Math.Abs(start.Y - end.Y) < 0.000001)
            {
                return;
            }

            graphics.DrawLine(linePen, start, end);
            PointF center = new PointF((start.X + end.X) / 2F, (start.Y + end.Y) / 2F);
            DrawCenterMarker(graphics, center, imageSize, centerPen, null);
            if (labelMode != OverlayLabelMode.None)
            {
                DrawOverlayLabel(graphics, FormatOverlayLabel(overlay.Label, labelMode), center, imageSize, textBrush, textBackBrush, font);
            }
        }

        internal static string FormatOverlayLabel(string label, OverlayLabelMode labelMode)
        {
            if (labelMode == OverlayLabelMode.None || string.IsNullOrWhiteSpace(label))
            {
                return string.Empty;
            }

            if (labelMode == OverlayLabelMode.Details)
            {
                return label;
            }

            string trimmed = label.Trim();
            int separator = trimmed.IndexOf(' ');
            return separator <= 0 ? trimmed : trimmed.Substring(0, separator);
        }

        private static void DrawPointOverlay(Graphics graphics, VisionToolOverlay overlay, DrawingSize imageSize, Brush pointBrush, int maxPointCount)
        {
            if (maxPointCount <= 0)
            {
                return;
            }

            int count = 0;
            foreach (PointF point in overlay.Points)
            {
                if (count++ >= maxPointCount)
                {
                    break;
                }

                PointF clamped = ClampPoint(point, imageSize);
                graphics.FillEllipse(pointBrush, clamped.X - 1.5F, clamped.Y - 1.5F, 3F, 3F);
            }
        }

        private void DrawStepResultBadge(Graphics graphics, VisionPipelineStep step, DrawingSize imageSize, PipelineStepRunStatus status, Color backColor)
        {
            string text = BuildStepResultBadgeText(step, status);
            if (string.IsNullOrWhiteSpace(text) || imageSize.Width <= 12 || imageSize.Height <= 12)
            {
                return;
            }

            text = TruncateText(text, 120);
            using (Font font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (Brush backBrush = new SolidBrush(backColor))
            using (StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            })
            {
                SizeF textSize = graphics.MeasureString(text, font);
                float width = Math.Min(Math.Max(96F, textSize.Width + 16F), Math.Max(12F, imageSize.Width - 12F));
                float height = Math.Min(Math.Max(22F, textSize.Height + 8F), Math.Max(12F, imageSize.Height - 12F));
                RectangleF bounds = new RectangleF(6F, 6F, width, height);
                graphics.FillRectangle(backBrush, bounds);
                graphics.DrawRectangle(Pens.White, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                graphics.DrawString(text, font, textBrush, new RectangleF(bounds.X + 8F, bounds.Y + 2F, bounds.Width - 12F, bounds.Height - 4F), format);
            }
        }

        private string BuildStepResultBadgeText(VisionPipelineStep step, PipelineStepRunStatus status)
        {
            if (step == null)
            {
                return string.Empty;
            }

            string statusText = FormatStepStatus(status);
            if (string.IsNullOrWhiteSpace(statusText))
            {
                return string.Empty;
            }

            List<string> parts = new List<string> { statusText };
            stepResultSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary);

            string metricText = BuildStepMetricBadgeText(step, summary);
            if (!string.IsNullOrWhiteSpace(metricText))
            {
                parts.Add(metricText);
            }

            if (IsFailureStatus(status) && !string.IsNullOrWhiteSpace(summary?.Message))
            {
                string message = TruncateText(summary.Message, 72);
                if (string.IsNullOrWhiteSpace(metricText)
                    || message.IndexOf(ResolveBadgeMetricName(step, summary) ?? string.Empty, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    parts.Add(message);
                }
            }

            return string.Join(" | ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
        }

        private static string BuildStepMetricBadgeText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            string metricName = ResolveBadgeMetricName(step, summary);
            if (string.IsNullOrWhiteSpace(metricName))
            {
                return string.Empty;
            }

            double metricValue = 0d;
            bool hasValue = summary?.Metrics != null && summary.Metrics.TryGetValue(metricName, out metricValue);
            string valueText = hasValue ? metricValue.ToString("0.###", CultureInfo.InvariantCulture) : "N/A";
            string criteriaText = BuildAcceptanceCriteriaText(step, metricName);
            return $"{metricName}={valueText}{criteriaText}";
        }

        private static string ResolveBadgeMetricName(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (!string.IsNullOrWhiteSpace(step?.AcceptanceMetricName))
            {
                return step.AcceptanceMetricName;
            }

            if (summary?.Metrics == null || summary.Metrics.Count == 0)
            {
                return string.Empty;
            }

            foreach (string metricName in VisionPipelineKnownMetrics.GetMetricNamesForTool(step?.ToolType))
            {
                if (summary.Metrics.ContainsKey(metricName))
                {
                    return metricName;
                }
            }

            return VisionPipelineKnownMetrics.OrderMetrics(summary.Metrics).FirstOrDefault().Key ?? string.Empty;
        }

        private static string BuildAcceptanceCriteriaText(VisionPipelineStep step, string metricName)
        {
            if (step == null
                || !step.UseAcceptance
                || string.IsNullOrWhiteSpace(metricName)
                || !string.Equals(step.AcceptanceMetricName, metricName, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            bool hasMinimum = step.UseAcceptanceMetricMinimum;
            bool hasMaximum = step.UseAcceptanceMetricMaximum;
            if (hasMinimum && hasMaximum)
            {
                if (Math.Abs(step.AcceptanceMetricMinimum - step.AcceptanceMetricMaximum) < 0.000001)
                {
                    return $" = {step.AcceptanceMetricMinimum:0.###}";
                }

                return $" [{step.AcceptanceMetricMinimum:0.###}..{step.AcceptanceMetricMaximum:0.###}]";
            }

            if (hasMinimum)
            {
                return $" >= {step.AcceptanceMetricMinimum:0.###}";
            }

            if (hasMaximum)
            {
                return $" <= {step.AcceptanceMetricMaximum:0.###}";
            }

            return string.Empty;
        }

        private static string FormatAcceptanceConfiguration(VisionPipelineStep step)
        {
            if (step == null || !step.UseAcceptance)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            if (!step.ExpectedSuccess)
            {
                parts.Add("ExpectedSuccess=False");
            }

            if (step.MaxElapsedMilliseconds > 0)
            {
                parts.Add($"Elapsed <= {step.MaxElapsedMilliseconds:0.0} ms");
            }

            if (!string.IsNullOrWhiteSpace(step.RequiredMessageText))
            {
                parts.Add($"Message contains '{step.RequiredMessageText}'");
            }

            if (parts.Count == 0)
            {
                parts.Add("Enabled");
            }

            return string.Join(", ", parts);
        }

        private static Color ResolveOverlayColor(PipelineStepRunStatus status)
        {
            switch (status)
            {
                case PipelineStepRunStatus.Passed:
                    return Color.FromArgb(0, 210, 120);
                case PipelineStepRunStatus.Failed:
                case PipelineStepRunStatus.Timeout:
                    return Color.FromArgb(235, 60, 60);
                case PipelineStepRunStatus.Running:
                    return Color.FromArgb(245, 160, 30);
                case PipelineStepRunStatus.Canceled:
                case PipelineStepRunStatus.Skipped:
                    return Color.FromArgb(145, 150, 158);
                case PipelineStepRunStatus.Loaded:
                    return Color.FromArgb(60, 140, 220);
                default:
                    return Color.FromArgb(255, 210, 0);
            }
        }

        private static Color ResolveOverlayTextBackColor(PipelineStepRunStatus status)
        {
            Color color = ResolveOverlayColor(status);
            return Color.FromArgb(190, Math.Max(0, color.R - 35), Math.Max(0, color.G - 35), Math.Max(0, color.B - 35));
        }

        private static bool IsFailureStatus(PipelineStepRunStatus status)
        {
            return status == PipelineStepRunStatus.Failed
                || status == PipelineStepRunStatus.Timeout
                || status == PipelineStepRunStatus.Canceled;
        }

        private static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text) || maxLength <= 0)
            {
                return string.Empty;
            }

            return text.Length <= maxLength
                ? text
                : text.Substring(0, Math.Max(0, maxLength - 3)) + "...";
        }

        private static void DrawCenterMarker(Graphics graphics, PointF center, DrawingSize imageSize, Pen pen, Brush brush)
        {
            PointF point = ClampPoint(center, imageSize);
            const float radius = 4F;
            graphics.DrawLine(pen, point.X - radius, point.Y, point.X + radius, point.Y);
            graphics.DrawLine(pen, point.X, point.Y - radius, point.X, point.Y + radius);
            brush ??= Brushes.Transparent;
            if (!ReferenceEquals(brush, Brushes.Transparent))
            {
                graphics.FillEllipse(brush, point.X - 2F, point.Y - 2F, 4F, 4F);
            }
        }

        private static void DrawOverlayLabel(
            Graphics graphics,
            string label,
            PointF anchor,
            DrawingSize imageSize,
            Brush textBrush,
            Brush textBackBrush,
            Font font)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return;
            }

            PointF point = ClampPoint(anchor, imageSize);
            SizeF textSize = graphics.MeasureString(label, font);
            float x = Math.Min(Math.Max(point.X, 0), Math.Max(0, imageSize.Width - textSize.Width - 4));
            float y = Math.Max(0, point.Y - textSize.Height - 4);
            RectangleF background = new RectangleF(x, y, textSize.Width + 4, textSize.Height + 2);
            graphics.FillRectangle(textBackBrush, background);
            graphics.DrawString(label, font, textBrush, x + 2, y + 1);
        }

        private static RectangleF ClampRectangle(RectangleF rectangle, DrawingSize imageSize)
        {
            float x = Math.Max(0, Math.Min(rectangle.X, imageSize.Width));
            float y = Math.Max(0, Math.Min(rectangle.Y, imageSize.Height));
            float right = Math.Max(0, Math.Min(rectangle.Right, imageSize.Width));
            float bottom = Math.Max(0, Math.Min(rectangle.Bottom, imageSize.Height));
            return new RectangleF(x, y, Math.Max(0, right - x), Math.Max(0, bottom - y));
        }

        private static PointF ClampPoint(PointF point, DrawingSize imageSize)
        {
            return new PointF(
                Math.Max(0, Math.Min(point.X, imageSize.Width)),
                Math.Max(0, Math.Min(point.Y, imageSize.Height)));
        }

        private void ShowStepPreview(VisionPipelineStep step)
        {
            SetPreviewImage(null);
            ShowStepResultDetails(step);
            UpdatePreviewActionState(step);

            if (GetCurrentPreviewImageMode() == StepPreviewImageMode.Summary)
            {
                previewCaption.Text = "Preview - SUMMARY | All detections";
                if (pipelineSummaryPreviewImage != null)
                {
                    SetPreviewImage(new Bitmap(pipelineSummaryPreviewImage));
                    return;
                }

                SetPreviewImage(CreatePreviewPlaceholder("Run Preview", "All detected overlays will be shown here on one image.", PipelineStepRunStatus.NotRun));
                return;
            }

            if (step == null)
            {
                previewCaption.Text = "Preview";
                SetPreviewImage(CreatePreviewPlaceholder("No step selected", "Select a pipeline step to view its result.", PipelineStepRunStatus.NotRun));
                return;
            }

            PipelineStepRunStatus status = GetStepStatus(step);
            string statusText = FormatStepResultStatus(status);
            StepPreviewImageMode mode = GetCurrentPreviewImageMode();
            previewCaption.Text = FormatPreviewCaption(step, mode, statusText);
            if (TryGetStepPreviewSource(step, mode, out Bitmap bitmap))
            {
                SetPreviewImage(new Bitmap(bitmap));
                return;
            }

            string detail = FormatMissingPreviewDetail(step, mode);
            SetPreviewImage(CreatePreviewPlaceholder(statusText, detail, status));
        }

        private void UpdatePreviewActionState(VisionPipelineStep step)
        {
            bool canOpen = TryGetStepPreviewSource(step, GetCurrentPreviewImageMode(), out Bitmap source) && source != null;

            if (btnOpenPreview != null)
            {
                btnOpenPreview.Enabled = canOpen;
                btnOpenPreview.ForeColor = canOpen
                    ? Color.FromArgb(35, 85, 132)
                    : Color.FromArgb(130, 140, 150);
                btnOpenPreview.FlatAppearance.BorderColor = canOpen
                    ? Color.FromArgb(47, 111, 171)
                    : Color.FromArgb(170, 178, 186);
                btnOpenPreview.BackColor = canOpen
                    ? Color.FromArgb(250, 252, 253)
                    : Color.FromArgb(232, 236, 240);
            }

            if (previewBox != null)
            {
                previewBox.Cursor = canOpen ? Cursors.Hand : Cursors.Default;
            }
        }

        private bool TryGetStepPreviewSource(VisionPipelineStep step, StepPreviewImageMode mode, out Bitmap bitmap)
        {
            bitmap = null;
            if (mode == StepPreviewImageMode.Summary)
            {
                bitmap = pipelineSummaryPreviewImage;
                return bitmap != null;
            }

            if (step == null)
            {
                return false;
            }

            switch (mode)
            {
                case StepPreviewImageMode.Input:
                    if (stepInputImages.TryGetValue(step, out bitmap) && bitmap != null)
                    {
                        return true;
                    }

                    return TryResolveInputImageFromLayerCache(step, out bitmap);

                case StepPreviewImageMode.Output:
                    return stepResultImages.TryGetValue(step, out bitmap) && bitmap != null;

                default:
                    return stepPreviewImages.TryGetValue(step, out bitmap) && bitmap != null;
            }
        }

        private bool TryResolveInputImageFromLayerCache(VisionPipelineStep step, out Bitmap bitmap)
        {
            bitmap = null;
            if (step == null || string.IsNullOrWhiteSpace(step.InputLayer))
            {
                return false;
            }

            return previewLayerImages.TryGetValue(step.InputLayer.Trim(), out bitmap) && bitmap != null;
        }

        private static string FormatMissingPreviewDetail(VisionPipelineStep step, StepPreviewImageMode mode)
        {
            if (step == null)
            {
                return string.Empty;
            }

            switch (mode)
            {
                case StepPreviewImageMode.Input:
                    return $"Run Preview to cache input layer '{step.InputLayer ?? "-"}'.";
                case StepPreviewImageMode.Output:
                    return $"Run Preview to create output layer '{step.OutputLayer ?? "-"}'.";
                default:
                    return $"Run Preview to create overlay for {step.InputLayer} -> {step.OutputLayer}.";
            }
        }

        private Bitmap CreatePreviewPlaceholder(string title, string detail, PipelineStepRunStatus status)
        {
            int width = Math.Max(320, previewBox?.Width ?? 420);
            int height = Math.Max(220, previewBox?.Height ?? 280);
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Brush background = new SolidBrush(Color.FromArgb(17, 21, 26)))
            using (Brush mutedBrush = new SolidBrush(Color.FromArgb(150, 166, 184)))
            using (Brush titleBrush = new SolidBrush(ResolveOverlayColor(status)))
            using (Font titleFont = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font detailFont = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Pixel))
            using (StringFormat center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.Clear(Color.Black);
                graphics.FillRectangle(background, 0, 0, width, height);
                RectangleF titleBounds = new RectangleF(18F, height / 2F - 34F, width - 36F, 34F);
                RectangleF detailBounds = new RectangleF(18F, height / 2F + 4F, width - 36F, 48F);
                graphics.DrawString(title ?? string.Empty, titleFont, titleBrush, titleBounds, center);
                graphics.DrawString(detail ?? string.Empty, detailFont, mutedBrush, detailBounds, center);
            }

            return bitmap;
        }

        private void ShowStepResultDetails(VisionPipelineStep step)
        {
            ClearResultGrid();

            if (GetCurrentPreviewImageMode() == StepPreviewImageMode.Summary)
            {
                ShowPipelineSummaryDetails();
                return;
            }

            if (step == null)
            {
                resultCaption.Text = "Result Details";
                return;
            }

            resultCaption.Text = $"Result - {step.Name}";
            PipelineStepRunStatus status = GetStepStatus(step);
            stepResultSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary);

            AddResultRow("Status", FormatStepResultStatus(status));
            AddResultRow("Viewing", FormatCurrentPreviewValue(step), CreatePreviewActionTag(GetCurrentPreviewImageMode()));
            AddResultRow("Result", BuildStepResultHeadline(step, summary));

            if (summary?.ElapsedMilliseconds > 0)
            {
                AddResultRow("Elapsed", $"{summary.ElapsedMilliseconds:0.0} ms");
            }

            AddResultRow("Flow", FormatPipelineFlow(step.InputLayer, step.OutputLayer));
            AddResultRow("Input image", FormatLayerImageValue(step.InputLayer, FormatStepPreviewImageState(step, StepPreviewImageMode.Input)), new ResultGridRowTag(ResultGridRowAction.PreviewInput));
            AddResultRow("Output image", FormatLayerImageValue(step.OutputLayer, FormatStepPreviewImageState(step, StepPreviewImageMode.Output)), new ResultGridRowTag(ResultGridRowAction.PreviewOutput));

            if (step.UseAcceptance)
            {
                string acceptanceText = BuildStepMetricBadgeText(step, summary);
                if (string.IsNullOrWhiteSpace(acceptanceText))
                {
                    acceptanceText = FormatAcceptanceConfiguration(step);
                }

                AddResultRow("Acceptance", acceptanceText);
            }

            if (summary == null)
            {
                AddResultRow("Next action", "Run Preview");
                AddResultRow("Tool", step.ToolType);
                AddResultRow("Parameters", (step.Parameters?.Count ?? 0).ToString(CultureInfo.InvariantCulture));
                return;
            }

            if (summary.OverlayCount > 0)
            {
                AddResultRow(
                    "Overlays",
                    summary.OverlayCount.ToString(CultureInfo.InvariantCulture),
                    new ResultGridRowTag(ResultGridRowAction.Overlays));
            }

            AddPrimaryMetricRows(step, summary);

            if (!string.IsNullOrWhiteSpace(summary.Message))
            {
                AddResultRow("Message", TruncateText(summary.Message, 96));
            }

            AddOverlayDetailRows(step);
            AddResultRow("Tool", step.ToolType);
            AddResultRow("Parameters", summary.ParameterCount.ToString(CultureInfo.InvariantCulture));
        }

        private void ShowPipelineSummaryDetails()
        {
            resultCaption.Text = "Result - Summary";
            int totalSteps = pipeline?.Steps?.Count ?? 0;
            int cachedSteps = stepResultSummaries.Count;
            int overlaySteps = stepOverlays.Count(pair => pair.Value != null && pair.Value.Count > 0);
            int overlayCount = pipelineSummaryOverlays.Count;

            AddResultRow("Status", pipelineSummaryPreviewImage == null ? "Run Preview required." : "Ready");
            AddResultRow("Viewing", $"Summary | {FormatBitmapSize(pipelineSummaryPreviewImage)}");
            AddResultRow("Overlay steps", $"{overlaySteps} / {totalSteps}");
            AddResultRow("Overlays", overlayCount.ToString(CultureInfo.InvariantCulture));
            AddResultRow("Cached steps", $"{cachedSteps} / {totalSteps}");
            AddResultRow("Publish target", PipelineSummaryOutputLayerName);

            foreach (VisionPipelineStep step in pipeline?.Steps ?? Enumerable.Empty<VisionPipelineStep>())
            {
                if (step == null
                    || !stepOverlays.TryGetValue(step, out List<VisionToolOverlay> overlays)
                    || overlays == null
                    || overlays.Count == 0)
                {
                    continue;
                }

                int index = pipeline.Steps.IndexOf(step) + 1;
                AddResultRow(
                    $"{index:00} {TruncateText(step.Name, 18)}",
                    $"{overlays.Count} overlays");
            }
        }

        private static string BuildStepResultHeadline(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary == null)
            {
                return "Run Preview required.";
            }

            if (!summary.Success && !string.IsNullOrWhiteSpace(summary.Message))
            {
                return TruncateText(summary.Message, 96);
            }

            if (summary.Metrics != null && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double resultCount))
            {
                string countText = FormatMetricValue(resultCount);
                return summary.OverlayCount > 0
                    ? $"{countText} results, {summary.OverlayCount} overlays"
                    : $"{countText} results";
            }

            if (summary.OverlayCount > 0)
            {
                return $"{summary.OverlayCount} overlays";
            }

            if (summary.HasResultImage)
            {
                return $"Image {summary.ResultImageSizeText}";
            }

            if (!string.IsNullOrWhiteSpace(summary.Message))
            {
                return TruncateText(summary.Message, 96);
            }

            return string.IsNullOrWhiteSpace(summary.Status) ? "-" : summary.Status;
        }

        private static string FormatPipelineFlow(string inputLayer, string outputLayer)
        {
            string input = string.IsNullOrWhiteSpace(inputLayer) ? "-" : inputLayer.Trim();
            string output = string.IsNullOrWhiteSpace(outputLayer) ? "-" : outputLayer.Trim();
            return $"{input} -> {output}";
        }

        private void AddPrimaryMetricRows(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary?.Metrics == null || summary.Metrics.Count == 0)
            {
                return;
            }

            const int maxRows = 5;
            int added = 0;
            foreach (KeyValuePair<string, double> metric in OrderResultMetrics(step, summary.Metrics))
            {
                if (added >= maxRows)
                {
                    break;
                }

                AddResultRow(FormatMetricRowName(metric.Key), FormatMetricValue(metric.Value));
                added++;
            }

            int hidden = Math.Max(0, summary.MetricCount - added);
            if (hidden > 0)
            {
                AddResultRow("More metrics", $"+{hidden}", new ResultGridRowTag(ResultGridRowAction.Metrics));
            }
        }

        private static IEnumerable<KeyValuePair<string, double>> OrderResultMetrics(VisionPipelineStep step, IDictionary<string, double> metrics)
        {
            if (metrics == null)
            {
                yield break;
            }

            HashSet<string> emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string metricName in VisionPipelineKnownMetrics.GetMetricNamesForTool(step?.ToolType))
            {
                if (metrics.TryGetValue(metricName, out double value) && emitted.Add(metricName))
                {
                    yield return new KeyValuePair<string, double>(metricName, value);
                }
            }

            foreach (KeyValuePair<string, double> metric in VisionPipelineKnownMetrics.OrderMetrics(metrics))
            {
                if (emitted.Add(metric.Key))
                {
                    yield return metric;
                }
            }
        }

        private static string FormatMetricRowName(string metricName)
        {
            if (string.IsNullOrWhiteSpace(metricName))
            {
                return "Metric";
            }

            switch (metricName.Trim())
            {
                case VisionPipelineKnownMetrics.ResultCount:
                    return "Result count";
                case VisionPipelineKnownMetrics.AreaMin:
                    return "Area min";
                case VisionPipelineKnownMetrics.AreaMax:
                    return "Area max";
                case VisionPipelineKnownMetrics.AreaAvg:
                    return "Area avg";
                case VisionPipelineKnownMetrics.AngleMin:
                    return "Angle min";
                case VisionPipelineKnownMetrics.AngleMax:
                    return "Angle max";
                case VisionPipelineKnownMetrics.AngleAvg:
                    return "Angle avg";
                case VisionPipelineKnownMetrics.ScoreMin:
                    return "Score min";
                case VisionPipelineKnownMetrics.ScoreMax:
                    return "Score max";
                case VisionPipelineKnownMetrics.ScoreAvg:
                    return "Score avg";
                case VisionPipelineKnownMetrics.MeanValueMin:
                    return "Mean min";
                case VisionPipelineKnownMetrics.MeanValueMax:
                    return "Mean max";
                case VisionPipelineKnownMetrics.MeanValueAvg:
                    return "Mean avg";
                case VisionPipelineKnownMetrics.EdgeCount:
                    return "Edge count";
                case VisionPipelineKnownMetrics.EdgePointCount:
                    return "Edge points";
                default:
                    return metricName;
            }
        }

        private static string FormatMetricValue(double value)
        {
            return Math.Abs(value - Math.Round(value)) < 0.000001
                ? Math.Round(value).ToString("0", CultureInfo.InvariantCulture)
                : value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private string FormatStepPreviewSummary(VisionPipelineStep step)
        {
            StepPreviewImageMode mode = GetCurrentPreviewImageMode();
            return $"{FormatPreviewMode(mode)} | {FormatStepPreviewImageState(step, mode)}";
        }

        private static ResultGridRowTag CreatePreviewActionTag(StepPreviewImageMode mode)
        {
            switch (mode)
            {
                case StepPreviewImageMode.Input:
                    return new ResultGridRowTag(ResultGridRowAction.PreviewInput);
                case StepPreviewImageMode.Output:
                    return new ResultGridRowTag(ResultGridRowAction.PreviewOutput);
                default:
                    return new ResultGridRowTag(ResultGridRowAction.PreviewOverlay);
            }
        }

        private string FormatCurrentPreviewValue(VisionPipelineStep step)
        {
            StepPreviewImageMode mode = GetCurrentPreviewImageMode();
            if (mode == StepPreviewImageMode.Summary)
            {
                return "Summary | All detections";
            }

            return $"{FormatPreviewMode(mode)} | {FormatPreviewTargetLayer(step, mode)}";
        }

        private static string FormatPreviewCaption(VisionPipelineStep step, StepPreviewImageMode mode, string statusText)
        {
            string target = FormatPreviewTargetLayer(step, mode);
            string modeText = FormatPreviewMode(mode).ToUpperInvariant();
            return $"Preview - {modeText} | {target} ({statusText})";
        }

        private static string FormatPreviewTargetLayer(VisionPipelineStep step, StepPreviewImageMode mode)
        {
            if (mode == StepPreviewImageMode.Summary)
            {
                return "All detections";
            }

            if (step == null)
            {
                return "-";
            }

            switch (mode)
            {
                case StepPreviewImageMode.Input:
                    return string.IsNullOrWhiteSpace(step.InputLayer) ? "Input -" : step.InputLayer.Trim();
                case StepPreviewImageMode.Output:
                    return string.IsNullOrWhiteSpace(step.OutputLayer) ? "Output -" : step.OutputLayer.Trim();
                default:
                    string output = string.IsNullOrWhiteSpace(step.OutputLayer) ? "-" : step.OutputLayer.Trim();
                    return output;
            }
        }

        private static string FormatLayerImageValue(string layerName, string imageState)
        {
            string layer = string.IsNullOrWhiteSpace(layerName) ? "-" : layerName.Trim();
            return $"{layer} | {imageState}";
        }

        private string FormatStepPreviewImageState(VisionPipelineStep step, StepPreviewImageMode mode)
        {
            if (TryGetStepPreviewSource(step, mode, out Bitmap bitmap) && bitmap != null)
            {
                return FormatBitmapSize(bitmap);
            }

            switch (mode)
            {
                case StepPreviewImageMode.Summary:
                    return "Needs Preview";
                case StepPreviewImageMode.Input:
                    return "Needs Preview";
                case StepPreviewImageMode.Output:
                    return "Not Created";
                default:
                    return "Not Created";
            }
        }

        private static string FormatStepResultStatus(PipelineStepRunStatus status)
        {
            return status == PipelineStepRunStatus.NotRun
                ? "Needs Preview"
                : FormatStepStatus(status);
        }

        private static string FormatBitmapSize(Bitmap bitmap)
        {
            return bitmap == null
                ? "-"
                : $"{bitmap.Width}x{bitmap.Height}";
        }

        private void AddOverlayDetailRows(VisionPipelineStep step)
        {
            if (step == null || !stepOverlays.TryGetValue(step, out List<VisionToolOverlay> overlays) || overlays == null || overlays.Count == 0)
            {
                return;
            }

            const int maxRows = 6;
            int row = 0;
            for (int overlayIndex = 0; overlayIndex < overlays.Count; overlayIndex++)
            {
                VisionToolOverlay overlay = overlays[overlayIndex];
                if (overlay == null || overlay.Kind != VisionToolOverlayKind.Rectangle)
                {
                    continue;
                }

                if (row >= maxRows)
                {
                    break;
                }

                row++;
                AddResultRow($"Box {row}", FormatOverlayBox(overlay), new ResultGridRowTag(ResultGridRowAction.Overlay, overlayIndex));
            }

            int hidden = overlays.Count(item => item != null && item.Kind == VisionToolOverlayKind.Rectangle) - row;
            if (hidden > 0)
            {
                AddResultRow("More boxes", $"+{hidden}", new ResultGridRowTag(ResultGridRowAction.Overlays));
            }
        }

        private static string FormatOverlayBox(VisionToolOverlay overlay)
        {
            RectangleF bounds = overlay.Bounds;
            string label = string.IsNullOrWhiteSpace(overlay.Label) ? string.Empty : $"{overlay.Label} | ";
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}x:{1:0} y:{2:0} w:{3:0} h:{4:0}",
                label,
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height);
        }

        private void ClearResultGrid()
        {
            EnsureResultGridColumns();
            resultGrid?.Rows.Clear();
        }

        private void AddResultRow(string name, string value, object tag = null)
        {
            if (resultGrid == null || string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            EnsureResultGridColumns();
            if (resultGrid.Columns.Count < 2)
            {
                return;
            }

            int rowIndex = resultGrid.Rows.Add(name, value ?? string.Empty);
            DataGridViewRow row = resultGrid.Rows[rowIndex];
            row.Tag = tag;
            row.Cells[0].ToolTipText = name;
            row.Cells[1].ToolTipText = value ?? string.Empty;
            if (string.Equals(name, "Status", StringComparison.OrdinalIgnoreCase))
            {
                row.DefaultCellStyle.Font = new Font(resultGrid.Font, FontStyle.Bold);
                row.DefaultCellStyle.ForeColor = ResolveResultRowColor(value);
            }
            else if (string.Equals(name, "Result", StringComparison.OrdinalIgnoreCase))
            {
                row.DefaultCellStyle.Font = new Font(resultGrid.Font, FontStyle.Bold);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 64, 103);
            }
            else if (string.Equals(name, "Viewing", StringComparison.OrdinalIgnoreCase))
            {
                row.DefaultCellStyle.Font = new Font(resultGrid.Font, FontStyle.Bold);
                row.DefaultCellStyle.ForeColor = ResolvePreviewModeBackColor(GetCurrentPreviewImageMode());
            }
            else if (string.Equals(name, "Next action", StringComparison.OrdinalIgnoreCase))
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(196, 113, 0);
            }
            else if (name.EndsWith("image", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Flow", StringComparison.OrdinalIgnoreCase))
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(64, 77, 92);
            }

            if (tag is ResultGridRowTag actionTag && actionTag.Action != ResultGridRowAction.None)
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(35, 85, 132);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(216, 235, 250);
                row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(22, 64, 103);
                row.Cells[0].ToolTipText = GetResultGridActionToolTip(actionTag);
                row.Cells[1].ToolTipText = GetResultGridActionToolTip(actionTag);
            }
        }

        private static string GetResultGridActionToolTip(ResultGridRowTag tag)
        {
            if (tag == null)
            {
                return string.Empty;
            }

            switch (tag.Action)
            {
                case ResultGridRowAction.Metrics:
                    return "Double-click to view all metrics.";
                case ResultGridRowAction.PreviewInput:
                    return "Click to show this step's input image. Double-click to open it.";
                case ResultGridRowAction.PreviewOutput:
                    return "Click to show this step's output image. Double-click to open it.";
                case ResultGridRowAction.PreviewOverlay:
                    return "Click to show this step's overlay preview. Double-click to open it.";
                case ResultGridRowAction.Overlay:
                    return "Click to show overlay preview. Double-click to open this overlay.";
                case ResultGridRowAction.Overlays:
                    return "Click to show overlay preview. Double-click to open it.";
                default:
                    return string.Empty;
            }
        }

        private void EnsureResultGridColumns()
        {
            if (resultGrid == null || resultGrid.Columns.Count >= 2)
            {
                return;
            }

            resultGrid.Columns.Clear();
            resultGrid.Columns.Add("Item", "Item");
            resultGrid.Columns.Add("Value", "Value");
            resultGrid.Columns[0].FillWeight = 34F;
            resultGrid.Columns[1].FillWeight = 66F;
        }

        private static Color ResolveResultRowColor(string statusText)
        {
            if (string.IsNullOrWhiteSpace(statusText))
            {
                return Color.FromArgb(32, 32, 32);
            }

            if (statusText.IndexOf("OK", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(0, 128, 72);
            }

            if (statusText.IndexOf("NG", StringComparison.OrdinalIgnoreCase) >= 0
                || statusText.IndexOf("TIMEOUT", StringComparison.OrdinalIgnoreCase) >= 0
                || statusText.IndexOf("CANCEL", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(190, 32, 32);
            }

            if (statusText.IndexOf("RUN", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(196, 113, 0);
            }

            return Color.FromArgb(35, 85, 132);
        }

        private void SetPreviewImage(Bitmap image)
        {
            Image previous = previewBox.Image;
            previewBox.Image = image;
            previous?.Dispose();
        }

        private void RemoveStepPreview(VisionPipelineStep step)
        {
            if (step == null) { return; }

            RemoveStepInputImage(step);
            RemoveStepPreviewImage(step);
            RemoveStepResultImage(step);
            stepResultSummaries.Remove(step);
            stepOverlays.Remove(step);

            if (ReferenceEquals(SelectedStep, step))
            {
                ShowStepResultDetails(step);
            }
        }

        private void RemoveStepPreviewImage(VisionPipelineStep step)
        {
            if (step == null) { return; }

            if (stepPreviewImages.TryGetValue(step, out Bitmap bitmap))
            {
                bitmap.Dispose();
                stepPreviewImages.Remove(step);
            }

            if (ReferenceEquals(SelectedStep, step))
            {
                SetPreviewImage(null);
            }
        }

        private void RemoveStepInputImage(VisionPipelineStep step)
        {
            if (step == null) { return; }

            if (stepInputImages.TryGetValue(step, out Bitmap bitmap))
            {
                bitmap.Dispose();
                stepInputImages.Remove(step);
            }
        }

        private void RemoveStepResultImage(VisionPipelineStep step)
        {
            if (step == null) { return; }

            if (stepResultImages.TryGetValue(step, out Bitmap bitmap))
            {
                bitmap.Dispose();
                stepResultImages.Remove(step);
            }

            RemovePreviewLayerImage(step.OutputLayer);
        }

        private void ClearStepPreviewImages()
        {
            SetPreviewImage(null);
            ClearPipelineSummaryPreviewImage();
            pipelineSummaryOverlays.Clear();
            foreach (Bitmap bitmap in stepInputImages.Values)
            {
                bitmap.Dispose();
            }

            foreach (Bitmap bitmap in stepPreviewImages.Values)
            {
                bitmap.Dispose();
            }

            foreach (Bitmap bitmap in stepResultImages.Values)
            {
                bitmap.Dispose();
            }

            foreach (Bitmap bitmap in previewLayerImages.Values)
            {
                bitmap.Dispose();
            }

            stepInputImages.Clear();
            stepPreviewImages.Clear();
            stepResultImages.Clear();
            previewLayerImages.Clear();
            stepResultSummaries.Clear();
            stepOverlays.Clear();
            ShowStepResultDetails(SelectedStep);
        }

        private void AppendStepLog(VisionPipelineStepResult stepResult)
        {
            if (stepResult == null) { return; }

            int index = stepResult.Step == null ? 0 : pipeline.Steps.IndexOf(stepResult.Step) + 1;
            VisionPipelineStepResultSummary summary = VisionPipelineResultSummaryService.CreateStepSummary(index, stepResult);
            string elapsed = summary.ElapsedMilliseconds <= 0 ? string.Empty : $"{summary.ElapsedMilliseconds:0.0} ms";
            string message = string.IsNullOrWhiteSpace(summary.Message) ? string.Empty : $" - {summary.Message}";
            string metricText = string.IsNullOrWhiteSpace(summary.MetricsText) ? string.Empty : $" | {summary.MetricsText}";
            AppendLog($"{summary.Status} | {summary.Name} | {summary.InputLayer} -> {summary.OutputLayer} | {elapsed}{message}{metricText}");
        }

        private void AppendLog(string message)
        {
            string logMessage = message ?? string.Empty;
            if (tbRunLog.TextLength > 0)
            {
                tbRunLog.AppendText(Environment.NewLine);
            }

            tbRunLog.AppendText(logMessage);
            tbRunLog.SelectionStart = tbRunLog.TextLength;
            tbRunLog.ScrollToCaret();
            OVLog.Write(LogCategory.Pipeline, ResolvePipelineLogLevel(logMessage), $"Pipeline | {logMessage}");
        }

        private void AppendActivePipelineLog(string action)
        {
            string activeName = string.IsNullOrWhiteSpace(pipeline?.Name) ? "Pipeline" : pipeline.Name;
            AppendLog($"{action} ACTIVE | Pipeline={activeName} | Steps={pipeline?.Steps?.Count ?? 0}");
        }

        private static LogLevel ResolvePipelineLogLevel(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return LogLevel.Info;
            }

            if (message.StartsWith("FAILED", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("CHECK ERROR", StringComparison.OrdinalIgnoreCase)
                || message.Contains(" failed", StringComparison.OrdinalIgnoreCase)
                || message.Contains(" NG", StringComparison.OrdinalIgnoreCase))
            {
                return LogLevel.Error;
            }

            if (message.StartsWith("WARN", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("CHECK WARN", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("CHECK REVIEW", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("SAVE WARN", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("LOAD WARN", StringComparison.OrdinalIgnoreCase)
                || message.Contains(" CANCELED", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("CANCELED", StringComparison.OrdinalIgnoreCase)
                || message.StartsWith("CANCEL", StringComparison.OrdinalIgnoreCase))
            {
                return LogLevel.Warning;
            }

            return LogLevel.Info;
        }

        private void ResetStepStatuses()
        {
            stepStatuses.Clear();
            foreach (VisionPipelineStep step in pipeline.Steps)
            {
                SetStepStatus(step, PipelineStepRunStatus.NotRun);
            }
        }

        private void SetStepStatus(VisionPipelineStep step, PipelineStepRunStatus status)
        {
            if (step == null)
            {
                return;
            }

            stepStatuses[step] = status;
            TreeNode node = FindStepNode(treeSteps.Nodes, step);
            if (node != null)
            {
                int index = pipeline.Steps.IndexOf(step);
                node.Text = FormatStepNodeText(step, index, status);
                node.ToolTipText = FormatStepNodeToolTip(step, index, status);
                ApplyStepNodeStyle(node, status);
                node.EnsureVisible();
            }

            RefreshPipelineFlowPreview();
        }

        private PipelineStepRunStatus GetStepStatus(VisionPipelineStep step)
        {
            return step != null && stepStatuses.TryGetValue(step, out PipelineStepRunStatus status)
                ? status
                : PipelineStepRunStatus.NotRun;
        }

        private static PipelineStepRunStatus GetStepRunStatus(VisionPipelineStepResult stepResult)
        {
            switch (VisionPipelineResultSummaryService.ResolveStatus(stepResult))
            {
                case "OK":
                    return PipelineStepRunStatus.Passed;
                case "SKIP":
                    return PipelineStepRunStatus.Skipped;
                case "TIMEOUT":
                    return PipelineStepRunStatus.Timeout;
                case "CANCEL":
                    return PipelineStepRunStatus.Canceled;
                default:
                    return PipelineStepRunStatus.Failed;
            }
        }

        private static void ApplyStepNodeStyle(TreeNode node, PipelineStepRunStatus status)
        {
            if (node == null)
            {
                return;
            }

            switch (status)
            {
                case PipelineStepRunStatus.Running:
                    node.ForeColor = Color.FromArgb(196, 113, 0);
                    break;
                case PipelineStepRunStatus.Passed:
                    node.ForeColor = Color.FromArgb(0, 128, 72);
                    break;
                case PipelineStepRunStatus.Failed:
                case PipelineStepRunStatus.Timeout:
                    node.ForeColor = Color.FromArgb(190, 32, 32);
                    break;
                case PipelineStepRunStatus.Canceled:
                case PipelineStepRunStatus.Skipped:
                    node.ForeColor = Color.FromArgb(92, 98, 108);
                    break;
                case PipelineStepRunStatus.Loaded:
                    node.ForeColor = Color.FromArgb(35, 85, 132);
                    break;
                default:
                    node.ForeColor = Color.FromArgb(32, 32, 32);
                    break;
            }
        }

        private void RefreshPipelineFlowPreview()
        {
            if (tbFlowPreview == null)
            {
                return;
            }

            List<VisionPipelineStep> steps = pipeline?.Steps == null
                ? new List<VisionPipelineStep>()
                : pipeline.Steps.ToList();
            RefreshPipelineFlowControl(steps);

            if (steps.Count == 0)
            {
                if (flowPreviewCaption != null)
                {
                    flowPreviewCaption.Text = "Step Flow";
                    flowPreviewCaption.ForeColor = Color.FromArgb(35, 85, 132);
                }

                tbFlowPreview.ForeColor = Color.FromArgb(92, 98, 108);
                tbFlowPreview.Text = "Main -> (Add Step)";
                return;
            }

            List<string> lines = new List<string>();
            string previousEnabledOutput = null;
            bool hasBranch = false;

            for (int i = 0; i < steps.Count; i++)
            {
                VisionPipelineStep step = steps[i];
                if (step == null)
                {
                    continue;
                }

                string inputLayer = FormatFlowLayerName(step.InputLayer, "Input?");
                string outputLayer = FormatFlowLayerName(step.OutputLayer, "Output?");
                bool isBranch = step.Enabled
                    && !string.IsNullOrWhiteSpace(previousEnabledOutput)
                    && !string.Equals(inputLayer, previousEnabledOutput, StringComparison.OrdinalIgnoreCase);
                hasBranch |= isBranch;

                string marker = ReferenceEquals(step, SelectedStep) ? ">" : " ";
                string state = ResolveFlowStepState(step, isBranch, previousEnabledOutput);
                string toolType = string.IsNullOrWhiteSpace(step.ToolType) ? "Step" : step.ToolType.Trim();
                lines.Add($"{marker}{i + 1:00} {toolType} | {inputLayer} -> {outputLayer} [{state}]");

                if (step.Enabled && !string.IsNullOrWhiteSpace(step.OutputLayer))
                {
                    previousEnabledOutput = step.OutputLayer.Trim();
                }
            }

            if (flowPreviewCaption != null)
            {
                flowPreviewCaption.Text = hasBranch ? "Step Flow - Branch" : "Step Flow";
                flowPreviewCaption.ForeColor = hasBranch
                    ? Color.FromArgb(168, 92, 0)
                    : Color.FromArgb(35, 85, 132);
            }

            tbFlowPreview.ForeColor = hasBranch
                ? Color.FromArgb(145, 80, 0)
                : Color.FromArgb(35, 85, 132);
            tbFlowPreview.Text = string.Join(Environment.NewLine, lines);
        }

        private void RefreshPipelineFlowControl(IReadOnlyList<VisionPipelineStep> steps)
        {
            if (pipelineFlowView == null)
            {
                return;
            }

            List<PipelineFlowStepItem> items = new List<PipelineFlowStepItem>();
            string previousEnabledOutput = null;
            for (int i = 0; i < (steps?.Count ?? 0); i++)
            {
                VisionPipelineStep step = steps[i];
                if (step == null)
                {
                    continue;
                }

                PipelineStepRunStatus status = GetStepStatus(step);
                bool hasInputImage = TryGetStepPreviewSource(step, StepPreviewImageMode.Input, out Bitmap inputBitmap) && inputBitmap != null;
                bool hasOutputImage = TryGetStepPreviewSource(step, StepPreviewImageMode.Output, out Bitmap outputBitmap) && outputBitmap != null;
                string inputLayer = FormatFlowLayerName(step.InputLayer, "Input?");
                bool isBranch = step.Enabled
                    && !string.IsNullOrWhiteSpace(previousEnabledOutput)
                    && !string.Equals(inputLayer, previousEnabledOutput, StringComparison.OrdinalIgnoreCase);
                items.Add(new PipelineFlowStepItem
                {
                    Index = i,
                    Name = step.Name,
                    ToolType = step.ToolType,
                    InputLayer = step.InputLayer,
                    OutputLayer = step.OutputLayer,
                    ExpectedInputLayer = previousEnabledOutput,
                    IsBranch = isBranch,
                    FlowStateText = ResolveFlowRelationText(step, isBranch, previousEnabledOutput),
                    IsEnabled = step.Enabled,
                    Status = ToPipelineFlowStatus(step, status),
                    StatusText = FormatPipelineFlowStatusText(step, status),
                    HasInputImage = hasInputImage,
                    HasOutputImage = hasOutputImage
                });

                if (step.Enabled && !string.IsNullOrWhiteSpace(step.OutputLayer))
                {
                    previousEnabledOutput = step.OutputLayer.Trim();
                }
            }

            pipelineFlowView.SetSteps(items);
            pipelineFlowView.SelectStep(SelectedStepIndex, ToPipelineFlowPreviewMode(GetCurrentPreviewImageMode()));
        }

        private static string ResolveFlowRelationText(VisionPipelineStep step, bool isBranch, string expectedInputLayer)
        {
            if (step == null)
            {
                return string.Empty;
            }

            if (!step.Enabled)
            {
                return "Disabled step";
            }

            string inputLayer = FormatFlowLayerName(step.InputLayer, "Input?");
            if (string.IsNullOrWhiteSpace(expectedInputLayer))
            {
                return $"Source input: {inputLayer}";
            }

            return isBranch
                ? $"Branch input: {inputLayer} (previous output: {expectedInputLayer})"
                : $"Linked input: previous output {expectedInputLayer}";
        }

        private static PipelineFlowStepStatus ToPipelineFlowStatus(VisionPipelineStep step, PipelineStepRunStatus status)
        {
            if (step != null && !step.Enabled)
            {
                return PipelineFlowStepStatus.Skipped;
            }

            switch (status)
            {
                case PipelineStepRunStatus.Running:
                    return PipelineFlowStepStatus.Running;
                case PipelineStepRunStatus.Passed:
                    return PipelineFlowStepStatus.Passed;
                case PipelineStepRunStatus.Failed:
                    return PipelineFlowStepStatus.Failed;
                case PipelineStepRunStatus.Timeout:
                    return PipelineFlowStepStatus.Timeout;
                case PipelineStepRunStatus.Canceled:
                    return PipelineFlowStepStatus.Canceled;
                case PipelineStepRunStatus.Skipped:
                    return PipelineFlowStepStatus.Skipped;
                case PipelineStepRunStatus.Loaded:
                    return PipelineFlowStepStatus.Loaded;
                default:
                    return PipelineFlowStepStatus.Waiting;
            }
        }

        private static string FormatPipelineFlowStatusText(VisionPipelineStep step, PipelineStepRunStatus status)
        {
            if (step != null && !step.Enabled)
            {
                return "OFF";
            }

            string statusText = FormatStepStatus(status);
            return string.IsNullOrWhiteSpace(statusText) ? "WAIT" : statusText;
        }

        private string ResolveFlowStepState(VisionPipelineStep step, bool isBranch, string expectedInputLayer)
        {
            if (step == null)
            {
                return "NULL";
            }

            if (!step.Enabled)
            {
                return "OFF";
            }

            if (isBranch)
            {
                return string.IsNullOrWhiteSpace(expectedInputLayer)
                    ? "BRANCH"
                    : $"BRANCH expected {expectedInputLayer}";
            }

            string status = FormatStepStatus(GetStepStatus(step));
            return string.IsNullOrWhiteSpace(status) ? "WAIT" : status;
        }

        private static string FormatFlowLayerName(string layerName, string fallback)
        {
            return string.IsNullOrWhiteSpace(layerName)
                ? fallback
                : layerName.Trim();
        }

        private void LogValidationResult(VisionPipelineValidationResult validation, bool showSuccess)
        {
            if (validation == null) { return; }

            foreach (string warning in validation.Warnings)
            {
                AppendLog($"CHECK REVIEW | {warning}");
            }

            foreach (string error in validation.Errors)
            {
                AppendLog($"CHECK ERROR | {error}");
            }

            if (showSuccess && validation.Success)
            {
                AppendLog(validation.Warnings.Count == 0
                    ? "CHECK OK | Pipeline flow is valid."
                    : $"CHECK OK | Pipeline flow is valid. Review item(s): {validation.Warnings.Count}.");
            }
        }

        private void FocusFirstValidationError(VisionPipelineValidationResult validation)
        {
            if (validation == null)
            {
                return;
            }

            foreach (string error in validation.Errors)
            {
                if (TryParseStepIndex(error, out int stepIndex))
                {
                    SelectStepAt(stepIndex);
                    AppendLog($"FOCUS | Step {stepIndex + 1} selected for validation error.");
                    return;
                }
            }
        }

        private void FocusFirstValidationWarning(VisionPipelineValidationResult validation)
        {
            if (validation == null)
            {
                return;
            }

            foreach (string warning in validation.Warnings)
            {
                if (TryParseStepIndex(warning, out int stepIndex))
                {
                    SelectStepAt(stepIndex);
                    AppendLog($"FOCUS | Step {stepIndex + 1} selected for validation warning.");
                    return;
                }
            }
        }

        private static bool TryParseStepIndex(string message, out int stepIndex)
        {
            stepIndex = -1;
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            const string prefix = "Step ";
            int start = message.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return false;
            }

            int numberStart = start + prefix.Length;
            int numberEnd = numberStart;
            while (numberEnd < message.Length && char.IsDigit(message[numberEnd]))
            {
                numberEnd++;
            }

            if (numberEnd == numberStart)
            {
                return false;
            }

            if (!int.TryParse(message.Substring(numberStart, numberEnd - numberStart), out int oneBasedIndex))
            {
                return false;
            }

            stepIndex = oneBasedIndex - 1;
            return stepIndex >= 0;
        }

        private void SelectFirstStepNode()
        {
            TreeNode first = FindFirstStepNode(treeSteps.Nodes);
            treeSteps.SelectedNode = first ?? (treeSteps.Nodes.Count > 0 ? treeSteps.Nodes[0] : null);

            if (first?.Tag is VisionPipelineStep step)
            {
                BindStepProperty(step);
                ShowStepPreview(step);
                pipelineFlowView?.SelectStep(pipeline?.Steps?.IndexOf(step) ?? -1, ToPipelineFlowPreviewMode(GetCurrentPreviewImageMode()));
            }
        }

        private void SelectStepAt(int index)
        {
            if (index < 0 || index >= pipeline.Steps.Count)
            {
                treeSteps.SelectedNode = treeSteps.Nodes.Count > 0 ? treeSteps.Nodes[0] : null;
                return;
            }

            SelectStep(pipeline.Steps[index]);
        }

        private void SelectStep(VisionPipelineStep step)
        {
            TreeNode node = FindStepNode(treeSteps.Nodes, step);
            if (node != null)
            {
                treeSteps.SelectedNode = node;
                node.EnsureVisible();
            }

            pipelineFlowView?.SelectStep(pipeline?.Steps?.IndexOf(step) ?? -1, ToPipelineFlowPreviewMode(GetCurrentPreviewImageMode()));
        }

        private static TreeNode FindFirstStepNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is VisionPipelineStep)
                {
                    return node;
                }

                TreeNode child = FindFirstStepNode(node.Nodes);
                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }

        private static TreeNode FindStepNode(TreeNodeCollection nodes, VisionPipelineStep step)
        {
            foreach (TreeNode node in nodes)
            {
                if (ReferenceEquals(node.Tag, step))
                {
                    return node;
                }

                TreeNode child = FindStepNode(node.Nodes, step);
                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }

        private void UpdateSelectedTreeNodeText()
        {
            if (!(treeSteps.SelectedNode?.Tag is VisionPipelineStep step)) { return; }

            int index = pipeline.Steps.IndexOf(step);
            PipelineStepRunStatus status = GetStepStatus(step);
            treeSteps.SelectedNode.Text = FormatStepNodeText(step, index, status);
            treeSteps.SelectedNode.ToolTipText = FormatStepNodeToolTip(step, index, status);
            ApplyStepNodeStyle(treeSteps.SelectedNode, status);
        }

        private int SelectedStepIndex
        {
            get
            {
                VisionPipelineStep step = SelectedStep;
                return step == null ? -1 : pipeline.Steps.IndexOf(step);
            }
        }

        private VisionPipelineStep SelectedStep
        {
            get { return treeSteps.SelectedNode?.Tag as VisionPipelineStep; }
        }

        private static string FormatStepNodeText(VisionPipelineStep step, int index, PipelineStepRunStatus status)
        {
            if (step == null)
            {
                return string.Empty;
            }

            string stepName = FormatStepTreeName(step.Name, index);
            string enabledText = !step.Enabled ? " [OFF]" : string.Empty;
            string statusText = status == PipelineStepRunStatus.NotRun ? string.Empty : $" [{FormatStepStatus(status)}]";
            return $"{stepName}{enabledText}{statusText}";
        }

        private static string FormatStepNodeToolTip(VisionPipelineStep step, int index, PipelineStepRunStatus status)
        {
            if (step == null)
            {
                return string.Empty;
            }

            string stepName = string.IsNullOrWhiteSpace(step.Name) ? "Step" : step.Name.Trim();
            string toolType = string.IsNullOrWhiteSpace(step.ToolType) ? "Unknown" : step.ToolType.Trim();
            string inputLayer = string.IsNullOrWhiteSpace(step.InputLayer) ? "Input?" : step.InputLayer.Trim();
            string outputLayer = string.IsNullOrWhiteSpace(step.OutputLayer) ? "Output?" : step.OutputLayer.Trim();
            string statusText = FormatStepStatus(status);
            if (string.IsNullOrWhiteSpace(statusText))
            {
                statusText = "WAIT";
            }

            string stepNo = index >= 0 ? $"{index + 1:00}" : "--";
            return $"Step {stepNo}: {stepName}{Environment.NewLine}Tool: {toolType}{Environment.NewLine}Flow: {inputLayer} -> {outputLayer}{Environment.NewLine}Status: {statusText}";
        }

        private static string FormatStepTreeName(string stepName, int index)
        {
            string name = string.IsNullOrWhiteSpace(stepName) ? "Step" : stepName.Trim();
            if (HasStepNumberPrefix(name))
            {
                return name;
            }

            return index >= 0 ? $"{index + 1:00} {name}" : name;
        }

        private static bool HasStepNumberPrefix(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 2)
            {
                return false;
            }

            if (!char.IsDigit(text[0]) || !char.IsDigit(text[1]))
            {
                return false;
            }

            return text.Length == 2 || char.IsWhiteSpace(text[2]) || text[2] == '.' || text[2] == '_' || text[2] == '-';
        }

        private static string FormatStepStatus(PipelineStepRunStatus status)
        {
            switch (status)
            {
                case PipelineStepRunStatus.Running:
                    return "RUN";
                case PipelineStepRunStatus.Passed:
                    return "OK";
                case PipelineStepRunStatus.Failed:
                    return "NG";
                case PipelineStepRunStatus.Timeout:
                    return "TIMEOUT";
                case PipelineStepRunStatus.Canceled:
                    return "CANCEL";
                case PipelineStepRunStatus.Skipped:
                    return "SKIP";
                case PipelineStepRunStatus.Loaded:
                    return "LOADED";
                default:
                    return string.Empty;
            }
        }

        private static string NormalizeToolType(string toolType)
        {
            return (toolType ?? string.Empty).Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        }

        private static string GetPipelineImageManifestPath(string directory)
        {
            return Path.Combine(directory, "layers.tsv");
        }

        private static string GetStepPreviewDirectory(string pipelineImageDirectory)
        {
            return Path.Combine(pipelineImageDirectory, StepPreviewDirectoryName);
        }

        private static string GetStepPreviewManifestPath(string stepPreviewDirectory)
        {
            return Path.Combine(stepPreviewDirectory, StepPreviewManifestFileName);
        }

        private static string GetUniqueImageFileName(string baseName, IDictionary<string, int> usedFileNames)
        {
            string name = string.IsNullOrWhiteSpace(baseName) ? "Layer" : baseName;
            string fileName = $"{name}.png";

            if (!usedFileNames.TryGetValue(fileName, out int count))
            {
                usedFileNames[fileName] = 1;
                return fileName;
            }

            count++;
            usedFileNames[fileName] = count;
            return $"{name}_{count}.png";
        }

        private static void DeleteStalePipelineImages(string directory, ISet<string> savedFileNames)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return;
            }

            foreach (string path in Directory.EnumerateFiles(directory, "*.png")
                .Concat(Directory.EnumerateFiles(directory, "*.overlays.tsv")))
            {
                string fileName = Path.GetFileName(path);
                if (savedFileNames.Contains(fileName))
                {
                    continue;
                }

                try
                {
                    File.Delete(path);
                }
                catch
                {
                    // Stale cleanup should not block saving the pipeline itself.
                }
            }
        }

        private static string SanitizeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Layer";
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Layer" : sanitized;
        }

        private static string EscapeManifestValue(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\t", "\\t")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        private static string UnescapeManifestValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t")
                .Replace("\\\\", "\\");
        }

        private sealed class PipelineStepPropertyAdapter : ICustomTypeDescriptor
        {
            private static readonly Dictionary<string, Type> ParameterTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["Mode"] = typeof(ThresholdToolMode),
                ["ThresholdType"] = typeof(ThresholdTypes),
                ["AdaptiveThresholdType"] = typeof(ThresholdTypes),
                ["THRESHOLD_TYPES"] = typeof(ThresholdTypes),
                ["ADAPTIVE_THRESHOLD_TYPES"] = typeof(ThresholdTypes),
                ["AdaptiveType"] = typeof(AdaptiveThresholdTypes),
                ["ADAPTIVE_THRESHOLD_ALGORITHM"] = typeof(AdaptiveThresholdTypes),
                ["Shape"] = typeof(MorphShapes),
                ["Operator"] = typeof(MorphTypes),
                ["FilterType"] = typeof(FilterToolType),
                ["BorderType"] = typeof(BorderTypes),
                ["EdgeType"] = typeof(EdgeDetectionToolType),
                ["ApproximationModes"] = typeof(ContourApproximationModes),
                ["DetectMode"] = typeof(RetrievalModes),
                ["PRJ_PORALITY"] = typeof(FormulaUtil.PROJECTION_POLARITY),
                ["PRJ_DIR"] = typeof(FormulaUtil.PROJECTION_DIR),
                ["VER_PRJ_DIR"] = typeof(FormulaUtil.PROJECTION_DIR),
                ["MATCH_MODE"] = typeof(TemplateMatchModes),
                ["MEAN_TYPES"] = typeof(MeanType)
            };

            private static readonly HashSet<string> IntegerParameters = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "MIN_AREA",
                "MAX_AREA",
                "DrawThickness",
                "POINT_RANGE",
                "EXTEND_FIT_LINE_VALUE",
                "NUM_MATCH",
                "FIND_ANGLE_MAX",
                "FIND_ANGLE_MIN",
                "CANNY_HIGH",
                "CANNY_LOW",
                "MEAN_MAX",
                "MEAN_MIN",
                "RangeMin",
                "RangeMax",
                "BlockSize",
                "Weight",
                "KernelWidth",
                "KernelHeight",
                "Iterations",
                "MedianKernelSize",
                "Diameter",
                "SigmaColor",
                "SigmaSpace",
                "CannyThresholdLow",
                "CannyThresholdHigh",
                "CannyApertureSize",
                "SobelDegreeX",
                "SobelDegreeY",
                "SobelKernelSize",
                "ScharrDegreeX",
                "ScharrDegreeY",
                "LaplacianKernelSize"
            };

            private static readonly HashSet<string> DoubleParameters = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "PIXELPERMM",
                "THRESHOLD",
                "ADAPTIVE_THRESHOLD",
                "EPSILON",
                "CONTRAST",
                "THICKNESS",
                "SAMPLING_STEP",
                "MANUAL_ANGLE_VALUE",
                "SCORE_MIN",
                "MAGNIFIATION",
                "FIND_ANGLE",
                "RANSAC_REPROJ_THRESHOLD",
                "Threshold",
                "MaxValue"
            };

            private readonly VisionPipelineStep step;
            private readonly Action changed;

            public PipelineStepPropertyAdapter(VisionPipelineStep step, Action changed)
            {
                this.step = step ?? throw new ArgumentNullException(nameof(step));
                this.changed = changed;
            }

            public AttributeCollection GetAttributes() => AttributeCollection.Empty;
            public string GetClassName() => "Pipeline Step";
            public string GetComponentName() => step.Name;
            public TypeConverter GetConverter() => new TypeConverter();
            public EventDescriptor GetDefaultEvent() => null;
            public PropertyDescriptor GetDefaultProperty() => null;
            public object GetEditor(Type editorBaseType) => null;
            public EventDescriptorCollection GetEvents(Attribute[] attributes) => EventDescriptorCollection.Empty;
            public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;
            public object GetPropertyOwner(PropertyDescriptor pd) => this;

            public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();

            public PropertyDescriptorCollection GetProperties()
            {
                List<PropertyDescriptor> properties = new List<PropertyDescriptor>
                {
                    new StepFieldDescriptor("Name", "Step", adapter => adapter.step.Name, (adapter, value) => adapter.step.Name = value),
                    new StepFieldDescriptor("ToolType", "Step", adapter => adapter.step.ToolType, (adapter, value) => adapter.step.ToolType = value),
                    new StepFieldDescriptor("InputLayer", "Layer", adapter => adapter.step.InputLayer, (adapter, value) => adapter.step.InputLayer = value),
                    new StepFieldDescriptor("OutputLayer", "Layer", adapter => adapter.step.OutputLayer, (adapter, value) => adapter.step.OutputLayer = value)
                };

                foreach (string key in step.Parameters.Keys.OrderBy(item => item))
                {
                    properties.Add(new StepParameterDescriptor(key, ResolveParameterType(key)));
                }

                return new PropertyDescriptorCollection(properties.ToArray());
            }

            private static Type ResolveParameterType(string key)
            {
                if (VisionPipelineStepParameterSchema.TryGetParameterType(key, out Type schemaType))
                {
                    return schemaType;
                }

                if (ParameterTypes.TryGetValue(key, out Type mappedType))
                {
                    return mappedType;
                }

                if (IntegerParameters.Contains(key))
                {
                    return typeof(int);
                }

                if (DoubleParameters.Contains(key))
                {
                    return typeof(double);
                }

                if (key.StartsWith("USE_", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("SHOW_", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "Invert", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "UseL2Gradient", StringComparison.OrdinalIgnoreCase))
                {
                    return typeof(bool);
                }

                return typeof(string);
            }

            private void NotifyChanged()
            {
                changed?.Invoke();
            }

            private sealed class StepFieldDescriptor : PropertyDescriptor
            {
                private readonly string category;
                private readonly Func<PipelineStepPropertyAdapter, string> getter;
                private readonly Action<PipelineStepPropertyAdapter, string> setter;

                public StepFieldDescriptor(
                    string name,
                    string category,
                    Func<PipelineStepPropertyAdapter, string> getter,
                    Action<PipelineStepPropertyAdapter, string> setter)
                    : base(name, null)
                {
                    this.category = category;
                    this.getter = getter;
                    this.setter = setter;
                }

                public override string Category => category;
                public override Type ComponentType => typeof(PipelineStepPropertyAdapter);
                public override bool IsReadOnly => false;
                public override Type PropertyType => typeof(string);
                public override bool CanResetValue(object component) => false;
                public override object GetValue(object component) => getter((PipelineStepPropertyAdapter)component);
                public override void ResetValue(object component) { }
                public override void SetValue(object component, object value)
                {
                    PipelineStepPropertyAdapter adapter = (PipelineStepPropertyAdapter)component;
                    setter(adapter, Convert.ToString(value) ?? string.Empty);
                    adapter.NotifyChanged();
                }

                public override bool ShouldSerializeValue(object component) => false;
            }

            private sealed class StepParameterDescriptor : PropertyDescriptor
            {
                private readonly string key;
                private readonly Type propertyType;

                public StepParameterDescriptor(string key, Type propertyType)
                    : base($"Parameter_{key}", null)
                {
                    this.key = key;
                    this.propertyType = propertyType ?? typeof(string);
                }

                public override string Category => "Parameters";
                public override string DisplayName => key;
                public override Type ComponentType => typeof(PipelineStepPropertyAdapter);
                public override bool IsReadOnly => false;
                public override Type PropertyType => propertyType;
                public override bool CanResetValue(object component) => false;
                public override object GetValue(object component)
                {
                    PipelineStepPropertyAdapter adapter = (PipelineStepPropertyAdapter)component;
                    adapter.step.Parameters.TryGetValue(key, out string value);
                    return ConvertFromText(value);
                }

                public override void ResetValue(object component) { }

                public override void SetValue(object component, object value)
                {
                    PipelineStepPropertyAdapter adapter = (PipelineStepPropertyAdapter)component;
                    adapter.step.Parameters[key] = ConvertToText(value);
                    adapter.NotifyChanged();
                }

                public override bool ShouldSerializeValue(object component) => false;

                private object ConvertFromText(string value)
                {
                    if (propertyType == typeof(string))
                    {
                        return value ?? string.Empty;
                    }

                    if (propertyType == typeof(bool))
                    {
                        return bool.TryParse(value, out bool boolValue) && boolValue;
                    }

                    if (propertyType == typeof(int))
                    {
                        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue)
                            ? intValue
                            : 0;
                    }

                    if (propertyType == typeof(double))
                    {
                        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue)
                            ? doubleValue
                            : 0d;
                    }

                    if (propertyType.IsEnum)
                    {
                        try
                        {
                            return string.IsNullOrWhiteSpace(value)
                                ? Enum.GetValues(propertyType).GetValue(0)
                                : Enum.Parse(propertyType, value, true);
                        }
                        catch
                        {
                            return Enum.GetValues(propertyType).GetValue(0);
                        }
                    }

                    return value ?? string.Empty;
                }

                private string ConvertToText(object value)
                {
                    if (value == null)
                    {
                        return string.Empty;
                    }

                    if (value is double doubleValue)
                    {
                        return doubleValue.ToString(CultureInfo.InvariantCulture);
                    }

                    if (value is float floatValue)
                    {
                        return floatValue.ToString(CultureInfo.InvariantCulture);
                    }

                    if (value is IFormattable formattable && !(value is Enum))
                    {
                        return formattable.ToString(null, CultureInfo.InvariantCulture);
                    }

                    return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
                }
            }
        }
    }
}




