using OpenVisionLab._1._Core;
using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using OpenVisionLab.Logging.Controls.View;
using OpenVisionLab.Logging.Controls.ViewModel;
using OpenVisionLab.Logging;
using OpenVisionLab.MessageDialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Xml.Linq;
using WeifenLuo.WinFormsUI.Docking;
using static OpenVisionLab.DEFINE;
using Cursors = System.Windows.Forms.Cursors;

namespace OpenVisionLab
{
    public partial class FormTeachingVision : Form
    {
        private readonly GlobalState Global;
        private readonly IDisplayManager displayManager;
        private readonly IDisplayHostBinder displayHostBinder;
        private DockPanel dockPanel;
        private SplitContainer workspaceSplitContainer;
        private SplitContainer workspaceMainSplitContainer;
        private Panel inlineLogPanel;
        private ElementHost inlineLogHost;
        private int PanelCount = 0;
        private Panel pnlToolbarActiveLayer;
        private Panel pnlToolbarSourceMode;
        private Panel pnlToolbarFlow;
        private Panel pnlToolbarRunSummary;
        private Panel pnlToolbarTackTime;
        private Label lblToolbarActiveLayer;
        private Label lblToolbarSourceMode;
        private System.Windows.Forms.Timer propertyGridWarmupTimer;
        private Label lblToolbarFlow;
        private Label lblToolbarRunSummary;
        private Label lblToolbarTackTime;
        private Label lblCameraCaption;
        private Label lblLayerCaption;
        private Button btnRunStep;
        private Button btnRunPipeline;
        private Button btnStopRun;
        private Button btnResetView;
        private TableLayoutPanel workspaceInsightLayout;
        private Label lblPipelineStepHeader;
        private Label lblLayerResultHeader;
        private Label lblLogFilterHeader;
        private Panel pnlStepSummaryCard;
        private Label lblStepCardHeader;
        private Label lblStepStatusBadge;
        private ListBox lstPipelineSteps;
        private ListBox lstLayerResults;
        private Label lblStepTitle;
        private Label lblStepStatus;
        private Label lblStepFlow;
        private Label lblStepTime;
        private Label lblStepMessage;
        private Button btnLogAll;
        private Button btnLogInfo;
        private Button btnLogPipeline;
        private Button btnLogError;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem showLogDockTitleMenuItem;
        private ToolStripMenuItem saveWorkspaceLayoutMenuItem;
        private ToolStripMenuItem loadWorkspaceLayoutMenuItem;
        private ToolStripMenuItem diagnoseWorkspaceLayoutMenuItem;
        private VisionPipeline currentPipeline = new VisionPipeline { Name = VisionPipelineAppendService.DefaultPipelineName };
        private VisionPipelineStep selectedPipelineStep;
        private VisionPipelineRunResult lastPipelineRunResult;
        private CancellationTokenSource mainPipelineCancellationSource;
        private readonly Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary> mainPipelineStepSummaries = new Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary>();
        private readonly Dictionary<string, VisionPipelineStepResultSummary> mainLayerResultSummaries = new Dictionary<string, VisionPipelineStepResultSummary>(StringComparer.OrdinalIgnoreCase);
        private VisionPipelineStepResultSummary lastStandaloneToolRunSummary;
        private string lastToolbarStatus = string.Empty;
        private string lastRunSummary = "대기 중";
        private string activeLogFilterLabel = "All";
        private bool showLogDockTitle = false;
        private bool isRunningMainPipeline;
        private bool mainPipelineStopRequested;
        private bool isBindingPipelineStepList;
        private int lastPipelineStepTooltipIndex = -1;
        private int lastLayerResultTooltipIndex = -1;
        private int inlineLogPanelHeight = DefaultInlineLogPanelHeight;
        private bool isApplyingWorkspaceLayout;
        private Form layoutOwnerForm;
        private const int DefaultInlineLogPanelHeight = 148;
        private const int DefaultToolbarHeight = 80;
        private const int MainWorkspaceInsightWidth = 360;
        private const int MainWorkspaceInsightMinWidth = 300;
        private const int MainWorkspaceInsightMaxWidth = 440;
        private const int MainWorkspaceInsightMinMainWidth = 620;
        private const int MainPipelineStepTimeoutMilliseconds = 60000;
        private const string WorkspaceLayoutDirectoryName = "LAYOUT";
        private const string WorkspaceLayoutFileName = "MainWorkspaceLayout.xml";
        private const string DockPanelLayoutFileName = "MainDockPanel.layout.xml";

        #region Event Register                
        private EventHandler<DockDisplayEventArgs> EventUpdateDisplay;
        #endregion

        private Dictionary<VISION_DOCK_FORM, object> Forms = new Dictionary<VISION_DOCK_FORM, object>();

        public FormTeachingVision()
            : this(ApplicationRuntimeContext.CreateDefault())
        {
        }

        public FormTeachingVision(ApplicationRuntimeContext runtimeContext)
        {
            ApplicationRuntimeContext context = runtimeContext ?? ApplicationRuntimeContext.CreateDefault();
            Global = context.Global;
            displayManager = context.DisplayManager;
            displayHostBinder = context.DisplayHostBinder;
            InitializeComponent();
        }
        
        private void FormTeachingVision_Load(object sender, EventArgs e)
        {            
            InitEvent();
            InitUi();
            InitCameraItem();
            InitLayListItem();

            toolTip1.SetToolTip(btnNewPanel, "새 레이어 만들기");
            toolTip1.SetToolTip(chkUseLayerImage, "선택한 레이어 이미지를 검사 입력으로 사용합니다.");
            toolTip1.SetToolTip(cbLayerList, "현재 보거나 검사 입력으로 사용할 레이어를 선택합니다.");
            displayManager.SetCameraIndex(0);
            if (cbLayerList.Items.Count > 0) { cbLayerList.SelectedIndex = 0; }
            RefreshToolbarStatus();
            OVLog.Write(LogCategory.Main, LogLevel.Info, "Vision workspace ready.");
            SchedulePropertyGridWarmup();
        }

        private void SchedulePropertyGridWarmup()
        {
            propertyGridWarmupTimer?.Stop();
            propertyGridWarmupTimer?.Dispose();
            propertyGridWarmupTimer = new System.Windows.Forms.Timer { Interval = 1200 };
            propertyGridWarmupTimer.Tick += (sender, args) =>
            {
                propertyGridWarmupTimer.Stop();
                propertyGridWarmupTimer.Dispose();
                propertyGridWarmupTimer = null;
                WarmupPropertyGrid();
            };
            propertyGridWarmupTimer.Start();
        }

        private void WarmupPropertyGrid()
        {
            if (IsDisposed) { return; }

            try
            {
                using ElementHost warmupHost = new ElementHost
                {
                    Width = 1,
                    Height = 1,
                    Visible = false
                };
                var propertyGrid = new System.Windows.Controls.WpfPropertyGrid.PropertyGrid
                {
                    Layout = new System.Windows.Controls.WpfPropertyGrid.Design.CategorizedLayout(),
                    SelectedObject = new PropertyGridWarmupObject()
                };

                warmupHost.Child = propertyGrid;
                Controls.Add(warmupHost);
                warmupHost.CreateControl();
                Controls.Remove(warmupHost);
                warmupHost.Child = null;
            }
            catch (Exception ex)
            {
                OVLog.Write(LogCategory.Main, LogLevel.Warning, $"Property grid warmup skipped. {ex.GetBaseException().Message}");
            }
        }

        private sealed class PropertyGridWarmupObject
        {
            public string Name { get; set; } = "Warmup";
            public int Value { get; set; } = 1;
        }

        private void InitCameraItem()
        {
            cbCamera.Items.Clear();
            cbCamera.Items.Add("Camera 1");
            cbCamera.Visible = false;
        }

        private void InitLayListItem()
        {
            string previousLayer = cbLayerList.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(previousLayer))
            {
                previousLayer = displayManager.SelectedItem;
            }

            cbLayerList.OnSelectedIndexChanged -= cbLayerList_SelectedIndexChanged;
            cbLayerList.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++) { cbLayerList.Items.Add(displayManager.GetLayerTitle(i)); }

            int selectedIndex = 0;
            if (!string.IsNullOrWhiteSpace(previousLayer) && cbLayerList.Items.Contains(previousLayer))
            {
                selectedIndex = cbLayerList.Items.IndexOf(previousLayer);
            }

            if (cbLayerList.Items.Count > 0)
            {
                cbLayerList.SelectedIndex = selectedIndex;
                displayManager.SelectedItem = cbLayerList.SelectedItem?.ToString() ?? displayManager.SelectedItem;
            }
            else
            {
                cbLayerList.Text = string.Empty;
            }

            cbLayerList.OnSelectedIndexChanged += cbLayerList_SelectedIndexChanged;
            RefreshLayerResultPanel();
            RefreshToolbarStatus();
        }

        private void btnNewPanel_Click(object sender, EventArgs e)
        {
            displayManager.CreatePanel();
            InitLayListItem();
            RefreshToolbarStatus("새 레이어 생성");
            OVLog.Write(LogCategory.Main, LogLevel.Info, $"Layer created. Layers={displayManager.LayerCount}");
        }

        private void chkUseLayerImage_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkUseLayerImage.Check) { displayManager.SetImageSrc(Lib.Common.BitmapImageConverter.ToMat(displayManager.GetLayerImage(DEFINE.Main)).Clone()); }
            else
            {
                if (cbLayerList.SelectedItem == null) { return; }
                displayManager.SelectedItem = cbLayerList.SelectedItem.ToString();
                displayManager.SetImageSrc(Lib.Common.BitmapImageConverter.ToMat(displayManager.GetLayerImage(displayManager.SelectedItem)).Clone());
            }

            RefreshToolbarStatus(chkUseLayerImage.Check ? "레이어 입력 켜짐" : "Main 입력 사용");
            OVLog.Write(LogCategory.Main, LogLevel.Info, $"Source mode changed. Mode={(chkUseLayerImage.Check ? "SelectedLayer" : "Main")}, Layer={displayManager.SelectedItem}");
        
        }

        private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCamera.SelectedIndex < 0)
            {
                return;
            }

            displayManager.SetCameraIndex(cbCamera.SelectedIndex);
            displayManager.NotifyParameterChanged();

        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLayerList.SelectedItem == null) { return; }

            displayManager.SelectedItem = cbLayerList.SelectedItem.ToString();
            displayManager.ActivateLayer(displayManager.SelectedItem);

            if (chkUseLayerImage.Check)
            {
                displayManager.SetImageSrc(Lib.Common.BitmapImageConverter.ToMat(displayManager.GetLayerImage(displayManager.SelectedItem)).Clone());
            }

            RefreshToolbarStatus($"{cbLayerList.SelectedItem} 선택");
            OVLog.Write(LogCategory.Main, LogLevel.Info, $"Layer selected. Layer={cbLayerList.SelectedItem}, UseLayerImage={chkUseLayerImage.Check}");
        }

        // 최상위 keys 명령어 이기 때문에 
        // Datagridview 같은곳에 editmode f2번같은게 먹지 않는다.        
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            Keys key = keyData & ~(Keys.Shift | Keys.Control);

            switch (key)
            {
                case Keys.Escape:
                    //if (AppCommon.ShowMessageBox("Notice", "창을 닫으시겠습니까?"))
                    //{
                    //    this.DialogResult = DialogResult.Cancel;
                    //    this.Close();
                    //}
                    return true;
                case Keys.F5:
                    return true;
                case Keys.F7:
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private bool InitEvent()
        {
            displayManager.UpdateResult += OnUpdateResult;
            displayManager.UpdateCam += OnDisplayContextChanged;
            displayManager.UpdateParameter += OnDisplayContextChanged;
            if (displayManager is DisplayManagerService displayManagerService)
            {
                displayManagerService.VisionToolRunUpdated += OnVisionToolRunUpdated;
            }

            EventUpdateDisplay += OnUpdateDisplay;
            Global.Recipe.EventChagedRecipe += OnChangedRecipe;
        
            return true;
        }
        private void InitUi()
        {
            Font font = new Font("Verdana", 12, FontStyle.Regular);

            ApplyLoadedWorkspaceLayoutState(LoadMainWorkspaceLayoutState());
            InitializeWorkspaceLayout();
            this.dockPanel = TeachingVisionDockPanelFactory.Create(TeachingPanel, font);
            InstallDockCaptionFactory();
            dockPanel.DockLeftPortion = GetLeftDockWidth();
            dockPanel.DockBottomPortion = 0.16;
            displayHostBinder.SetForm(this);
            displayHostBinder.SetDockPanel(dockPanel);
            PropertyGridEditorFactory.SetRuntimeContext(() => displayManager);
            PropertyGridEditorFactory.SetRecipeNameContext(() => Global.Recipe.Name);
            displayManager.CreateLayerDisplay(new Bitmap(10, 10), "Main", false);
            InitializeMainToolbar();

            
            Forms.Add(VISION_DOCK_FORM.THRESHOLD, new FormThreshold(displayManager));
            bool layoutLoaded = LoadDockPanelLayoutSafe();
            if (!layoutLoaded)
            {
                ShowVisionForms();
            }

            EnsureMainDocumentVisible();
            ApplyLogPanelPresentation();
            RegisterLayoutPersistenceEvents();
        }

        private void InitializeWorkspaceLayout()
        {
            if (workspaceSplitContainer != null) { return; }

            splitContainer1.Panel2.Controls.Remove(TeachingPanel);

            workspaceSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                FixedPanel = FixedPanel.Panel2,
                IsSplitterFixed = false,
                SplitterWidth = 3,
                Panel1MinSize = 280,
                Panel2MinSize = 96,
                BackColor = Color.FromArgb(24, 29, 39)
            };

            workspaceMainSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                FixedPanel = FixedPanel.Panel2,
                IsSplitterFixed = false,
                SplitterWidth = 3,
                Panel1MinSize = 25,
                Panel2MinSize = 25,
                BackColor = Color.FromArgb(24, 29, 39)
            };

            TeachingPanel.Dock = DockStyle.Fill;
            workspaceMainSplitContainer.Panel1.Controls.Add(TeachingPanel);
            workspaceSplitContainer.Panel1.Controls.Add(workspaceMainSplitContainer);
            InitializeWorkspaceInsightPanel();
            splitContainer1.Panel2.Controls.Add(workspaceSplitContainer);
            workspaceSplitContainer.Resize += (sender, e) => LayoutInlineLogPanel();
            workspaceSplitContainer.SplitterMoved += (sender, e) => RememberInlineLogPanelHeight();
            workspaceMainSplitContainer.Resize += (sender, e) => EnsureWorkspaceInsightWidth();
            BeginInvoke(new Action(EnsureWorkspaceInsightWidth));
        }

        private void EnsureWorkspaceInsightWidth()
        {
            if (workspaceMainSplitContainer == null || workspaceMainSplitContainer.IsDisposed)
            {
                return;
            }

            int totalWidth = workspaceMainSplitContainer.ClientSize.Width;
            if (totalWidth <= 0)
            {
                return;
            }

            int panel2MinSize = GetSafeWorkspacePanel2MinSize(totalWidth);
            int panel1MinSize = GetSafeWorkspacePanel1MinSize(totalWidth, panel2MinSize);
            if (panel1MinSize + panel2MinSize > totalWidth)
            {
                return;
            }

            int minDistance = panel1MinSize;
            int maxDistance = totalWidth - panel2MinSize;
            if (maxDistance < minDistance)
            {
                return;
            }

            int desiredInsightWidth = Math.Max(
                MainWorkspaceInsightMinWidth,
                Math.Min(MainWorkspaceInsightWidth, MainWorkspaceInsightMaxWidth));
            if (totalWidth < MainWorkspaceInsightWidth + MainWorkspaceInsightMinMainWidth)
            {
                desiredInsightWidth = Math.Max(
                    panel2MinSize,
                    totalWidth - MainWorkspaceInsightMinMainWidth);
            }

            int maxInsightWidth = totalWidth - panel1MinSize;
            desiredInsightWidth = Math.Max(
                panel2MinSize,
                Math.Min(Math.Min(desiredInsightWidth, MainWorkspaceInsightMaxWidth), maxInsightWidth));

            int targetDistance = totalWidth - desiredInsightWidth;
            targetDistance = Math.Max(minDistance, Math.Min(targetDistance, maxDistance));
            ApplyWorkspaceMainSplitConstraints(panel1MinSize, panel2MinSize, targetDistance);
        }

        private static int GetSafeWorkspacePanel2MinSize(int totalWidth)
        {
            if (totalWidth <= 80)
            {
                return 25;
            }

            return Math.Max(25, Math.Min(MainWorkspaceInsightMinWidth, totalWidth / 3));
        }

        private static int GetSafeWorkspacePanel1MinSize(int totalWidth, int panel2MinSize)
        {
            if (totalWidth <= panel2MinSize + 25)
            {
                return 25;
            }

            return Math.Max(25, Math.Min(420, totalWidth - panel2MinSize));
        }

        private void ApplyWorkspaceMainSplitConstraints(int panel1MinSize, int panel2MinSize, int targetDistance)
        {
            try
            {
                workspaceMainSplitContainer.Panel1MinSize = 25;
                workspaceMainSplitContainer.Panel2MinSize = 25;

                if (targetDistance > 0 && workspaceMainSplitContainer.SplitterDistance != targetDistance)
                {
                    workspaceMainSplitContainer.SplitterDistance = targetDistance;
                }

                workspaceMainSplitContainer.Panel1MinSize = panel1MinSize;
                workspaceMainSplitContainer.Panel2MinSize = panel2MinSize;
            }
            catch (InvalidOperationException ex)
            {
                OVLog.Write(LogCategory.Main, LogLevel.Warning, $"Workspace split layout skipped. Width={workspaceMainSplitContainer.ClientSize.Width}, MainMin={panel1MinSize}, InsightMin={panel2MinSize}, Distance={targetDistance}, Message={ex.Message}");
            }
        }

        private void InitializeWorkspaceInsightPanel()
        {
            if (workspaceMainSplitContainer == null || workspaceInsightLayout != null)
            {
                return;
            }

            Panel insightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(29, 35, 48),
                Padding = new Padding(10, 8, 10, 8)
            };

            workspaceInsightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                BackColor = Color.Transparent,
                ColumnCount = 1,
                RowCount = 0,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            workspaceInsightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            insightPanel.Controls.Add(workspaceInsightLayout);
            workspaceMainSplitContainer.Panel2.Controls.Add(insightPanel);

            lblPipelineStepHeader = CreateInsightHeader("검사 Step");
            AddInsightRow(lblPipelineStepHeader, 22);

            lstPipelineSteps = CreateInsightListBox();
            lstPipelineSteps.DrawItem += DrawPipelineStepListItem;
            lstPipelineSteps.SelectedIndexChanged += (sender, e) => OnPipelineStepSelectionChanged();
            lstPipelineSteps.MouseMove += (sender, e) => UpdatePipelineStepToolTip(e.Location);
            lstPipelineSteps.MouseLeave += (sender, e) => ClearPipelineStepToolTip();
            AddInsightRow(lstPipelineSteps, 136);

            pnlStepSummaryCard = CreateStepSummaryCard();
            AddInsightRow(pnlStepSummaryCard, 158);

            AddInsightGap(8);
            lblLayerResultHeader = CreateInsightHeader("레이어 / 결과");
            AddInsightRow(lblLayerResultHeader, 22);

            lstLayerResults = CreateInsightListBox();
            lstLayerResults.DrawItem += DrawLayerResultListItem;
            lstLayerResults.SelectedIndexChanged += (sender, e) => OnLayerResultSelectionChanged();
            lstLayerResults.MouseMove += (sender, e) => UpdateLayerResultToolTip(e.Location);
            lstLayerResults.MouseLeave += (sender, e) => ClearLayerResultToolTip();
            AddInsightRow(lstLayerResults, 136);

            AddInsightGap(8);
            lblLogFilterHeader = CreateInsightHeader("로그 필터");
            AddInsightRow(lblLogFilterHeader, 22);

            FlowLayoutPanel logFilterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = Padding.Empty,
                Padding = new Padding(0, 2, 0, 0)
            };

            btnLogAll = CreateInsightButton("All", 54);
            btnLogInfo = CreateInsightButton("Info", 54);
            btnLogPipeline = CreateInsightButton("Pipeline", 92);
            btnLogError = CreateInsightButton("Error", 54);
            SetMainToolTip(btnLogAll, "모든 로그를 시간순으로 표시합니다.");
            SetMainToolTip(btnLogInfo, "Info 레벨 로그만 표시합니다.");
            SetMainToolTip(btnLogPipeline, "Pipeline 영역 로그만 표시합니다.");
            SetMainToolTip(btnLogError, "Error 레벨 로그만 표시합니다.");
            btnLogAll.Click += (sender, e) => ApplyLogQuickFilter("Any", "Any", string.Empty, true, "All");
            btnLogInfo.Click += (sender, e) => ApplyLogQuickFilter("Any", "Info", string.Empty, false, "Info");
            btnLogPipeline.Click += (sender, e) => ApplyLogQuickFilter("Pipeline", "Any", string.Empty, false, "Pipeline");
            btnLogError.Click += (sender, e) => ApplyLogQuickFilter("Any", "Error", string.Empty, false, "Error");

            logFilterPanel.Controls.Add(btnLogAll);
            logFilterPanel.Controls.Add(btnLogInfo);
            logFilterPanel.Controls.Add(btnLogPipeline);
            logFilterPanel.Controls.Add(btnLogError);
            AddInsightRow(logFilterPanel, 36);
            UpdateLogFilterButtonStates();
            RefreshInsightHeaders();
        }

        private void AddInsightRow(Control control, int height)
        {
            int rowIndex = workspaceInsightLayout.RowCount;
            workspaceInsightLayout.RowCount++;
            workspaceInsightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            control.Dock = DockStyle.Fill;
            workspaceInsightLayout.Controls.Add(control, 0, rowIndex);
        }

        private void AddInsightGap(int height)
        {
            Panel gap = new Panel
            {
                BackColor = Color.Transparent,
                Margin = Padding.Empty
            };
            AddInsightRow(gap, height);
        }

        private Panel CreateStepSummaryCard()
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(32, 40, 55),
                Padding = new Padding(1),
                Margin = Padding.Empty
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(36, 45, 61),
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(7, 5, 7, 6),
                Margin = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblStepCardHeader = CreateInsightCardHeader("선택 Step 상세");
            lblStepStatusBadge = CreateStepStatusBadgeLabel();
            lblStepTitle = CreateInsightCardLabel("Step 없음", FontStyle.Bold);
            lblStepStatus = CreateInsightCardLabel("상태: -", FontStyle.Bold);
            lblStepFlow = CreateInsightCardLabel("흐름: -");
            lblStepTime = CreateInsightCardLabel("시간: -");
            lblStepMessage = CreateInsightCardLabel("메시지: -");

            AddInsightCardControl(layout, CreateStepSummaryHeaderPanel(), 0);
            AddInsightCardControl(layout, lblStepTitle, 1);
            AddInsightCardControl(layout, lblStepStatus, 2);
            AddInsightCardControl(layout, lblStepFlow, 3);
            AddInsightCardControl(layout, lblStepTime, 4);
            AddInsightCardControl(layout, lblStepMessage, 5);

            card.Controls.Add(layout);
            return card;
        }

        private TableLayoutPanel CreateStepSummaryHeaderPanel()
        {
            TableLayoutPanel headerPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58F));
            headerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            headerPanel.Controls.Add(lblStepCardHeader, 0, 0);
            headerPanel.Controls.Add(lblStepStatusBadge, 1, 0);
            return headerPanel;
        }

        private static void AddInsightCardControl(TableLayoutPanel layout, Control control, int row)
        {
            control.Dock = DockStyle.Fill;
            layout.Controls.Add(control, 0, row);
        }

        private static Label CreateInsightCardHeader(string text)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                ForeColor = Color.FromArgb(158, 211, 247),
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 8.4F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 2, 0),
                AutoEllipsis = true,
                UseMnemonic = false
            };
        }

        private static Label CreateStepStatusBadgeLabel()
        {
            return new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Text = "-",
                ForeColor = Color.FromArgb(225, 233, 244),
                BackColor = Color.FromArgb(74, 83, 100),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(4, 1, 0, 1),
                Padding = new Padding(2, 0, 2, 0),
                AutoEllipsis = true,
                BorderStyle = BorderStyle.FixedSingle,
                UseMnemonic = false
            };
        }

        private static Label CreateInsightCardLabel(string text, FontStyle style = FontStyle.Regular)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                ForeColor = Color.FromArgb(216, 225, 240),
                BackColor = Color.FromArgb(36, 44, 60),
                Font = new Font("Segoe UI", 8.35F, style, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 8, 0),
                AutoEllipsis = true,
                UseMnemonic = false
            };
        }

        private static Label CreateInsightHeader(string text)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                ForeColor = Color.FromArgb(214, 232, 252),
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 0, 0),
                AutoEllipsis = true,
                UseMnemonic = false
            };
        }

        private static Label CreateInsightValueLabel(string text, int height, FontStyle style = FontStyle.Regular)
        {
            return new Label
            {
                AutoSize = false,
                Height = height,
                Text = text,
                ForeColor = Color.FromArgb(216, 225, 240),
                BackColor = Color.FromArgb(36, 44, 60),
                Font = new Font("Segoe UI", 8.4F, style, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 8, 0),
                AutoEllipsis = true,
                BorderStyle = BorderStyle.FixedSingle,
                UseMnemonic = false
            };
        }

        private static ListBox CreateInsightListBox()
        {
            return new ListBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(17, 24, 34),
                ForeColor = Color.FromArgb(232, 239, 249),
                Font = new Font("Segoe UI", 8.4F, FontStyle.Regular, GraphicsUnit.Point),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 34,
                IntegralHeight = false,
                HorizontalScrollbar = false,
                Margin = new Padding(0, 0, 0, 0)
            };
        }

        private void DrawPipelineStepListItem(object sender, DrawItemEventArgs e)
        {
            if (!(sender is ListBox listBox) || e.Index < 0 || e.Index >= listBox.Items.Count)
            {
                return;
            }

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Rectangle bounds = e.Bounds;
            using (SolidBrush rowBrush = new SolidBrush(GetInsightRowBackColor(e.Index, selected)))
            {
                e.Graphics.FillRectangle(rowBrush, bounds);
            }

            if (!(listBox.Items[e.Index] is PipelineStepListItem item))
            {
                DrawInsightText(e.Graphics, listBox.Font, bounds, listBox.Items[e.Index]?.ToString() ?? string.Empty, selected);
                return;
            }

            VisionPipelineStep step = item.Step;
            mainPipelineStepSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary);
            PipelineStepVisualState visualState = ResolvePipelineStepVisualState(step, summary);
            string badge = visualState.Badge;
            Color badgeColor = visualState.AccentColor;
            string stepName = string.IsNullOrWhiteSpace(step?.Name) ? "Step" : step.Name;
            string toolType = string.IsNullOrWhiteSpace(step?.ToolType) ? "-" : step.ToolType;

            Rectangle accentBounds = new Rectangle(bounds.Left, bounds.Top, 4, bounds.Height);
            Rectangle indexBounds = new Rectangle(bounds.Left + 9, bounds.Top + 5, 30, 20);
            Rectangle badgeBounds = new Rectangle(bounds.Right - 58, bounds.Top + Math.Max(6, (bounds.Height - 18) / 2), 50, 18);
            Rectangle titleBounds = new Rectangle(bounds.Left + 42, bounds.Top + 4, bounds.Width - 110, 15);
            Rectangle detailBounds = new Rectangle(bounds.Left + 42, bounds.Top + 19, bounds.Width - 110, 13);

            using (SolidBrush accentBrush = new SolidBrush(badgeColor))
            {
                e.Graphics.FillRectangle(accentBrush, accentBounds);
            }
            DrawInsightBadge(e.Graphics, listBox.Font, badgeBounds, FormatInsightBadgeText(badge), badgeColor, selected);

            Color titleColor = visualState.Enabled
                ? Color.FromArgb(244, 249, 255)
                : Color.FromArgb(145, 154, 170);
            Color detailColor = selected
                ? Color.FromArgb(205, 225, 248)
                : Color.FromArgb(152, 173, 198);

            using (Font indexFont = new Font(listBox.Font, FontStyle.Bold))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    item.Index.ToString("00", CultureInfo.InvariantCulture),
                    indexFont,
                    indexBounds,
                    selected ? Color.FromArgb(202, 225, 251) : Color.FromArgb(128, 151, 179),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
            using (Font titleFont = new Font(listBox.Font, FontStyle.Bold))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    stepName,
                    titleFont,
                    titleBounds,
                    titleColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            TextRenderer.DrawText(
                e.Graphics,
                BuildPipelineStepListDetailText(visualState, summary, toolType),
                listBox.Font,
                detailBounds,
                detailColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (selected)
            {
                DrawInsightSelectionBorder(e.Graphics, bounds, badgeColor);
            }

            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                e.DrawFocusRectangle();
            }
        }

        private void DrawLayerResultListItem(object sender, DrawItemEventArgs e)
        {
            if (!(sender is ListBox listBox) || e.Index < 0 || e.Index >= listBox.Items.Count)
            {
                return;
            }

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Rectangle bounds = e.Bounds;
            using (SolidBrush rowBrush = new SolidBrush(GetInsightRowBackColor(e.Index, selected)))
            {
                e.Graphics.FillRectangle(rowBrush, bounds);
            }

            if (!(listBox.Items[e.Index] is LayerResultListItem item))
            {
                DrawInsightText(e.Graphics, listBox.Font, bounds, listBox.Items[e.Index]?.ToString() ?? string.Empty, selected);
                return;
            }

            bool active = string.Equals(item.Title, displayManager.FocusItem, StringComparison.OrdinalIgnoreCase)
                || string.Equals(item.Title, displayManager.SelectedItem, StringComparison.OrdinalIgnoreCase);
            Color resultColor = item.AccentColor;
            Color accentColor = active ? Color.FromArgb(107, 196, 238) : resultColor;
            using (SolidBrush accentBrush = new SolidBrush(accentColor))
            {
                e.Graphics.FillRectangle(accentBrush, new Rectangle(bounds.Left, bounds.Top, 4, bounds.Height));
            }

            Rectangle indexBounds = new Rectangle(bounds.Left + 9, bounds.Top + 5, 30, 20);
            Rectangle titleBounds = new Rectangle(bounds.Left + 42, bounds.Top + 4, bounds.Width - 110, 15);
            Rectangle detailBounds = new Rectangle(bounds.Left + 42, bounds.Top + 19, bounds.Width - 110, 13);
            Rectangle badgeBounds = new Rectangle(bounds.Right - 58, bounds.Top + Math.Max(6, (bounds.Height - 18) / 2), 50, 18);
            DrawInsightBadge(
                e.Graphics,
                listBox.Font,
                badgeBounds,
                GetLayerResultBadgeText(item, active),
                active ? Color.FromArgb(43, 92, 125) : resultColor,
                active || selected);

            using (Font indexFont = new Font(listBox.Font, FontStyle.Bold))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    item.Index.ToString("00", CultureInfo.InvariantCulture),
                    indexFont,
                    indexBounds,
                    active ? Color.FromArgb(174, 226, 250) : Color.FromArgb(128, 151, 179),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
            using (Font titleFont = new Font(listBox.Font, active ? FontStyle.Bold : FontStyle.Regular))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    item.Title,
                    titleFont,
                    titleBounds,
                    active ? Color.FromArgb(246, 251, 255) : Color.FromArgb(224, 234, 247),
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            TextRenderer.DrawText(
                e.Graphics,
                BuildLayerResultListDetailText(item, active),
                listBox.Font,
                detailBounds,
                active ? Color.FromArgb(145, 218, 246) : Color.FromArgb(146, 166, 193),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (active || selected)
            {
                DrawInsightSelectionBorder(e.Graphics, bounds, active ? Color.FromArgb(107, 196, 238) : resultColor);
            }

            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                e.DrawFocusRectangle();
            }
        }

        private static string BuildLayerResultListDetailText(LayerResultListItem item, bool active)
        {
            if (item == null)
            {
                return string.Empty;
            }

            string detail = FormatLayerResultDetailText(item);
            if (!string.IsNullOrWhiteSpace(item.SizeText)
                && item.SizeText != "-"
                && !string.Equals(item.SizeText, "이미지 없음", StringComparison.OrdinalIgnoreCase)
                && detail.IndexOf(item.SizeText, StringComparison.OrdinalIgnoreCase) < 0)
            {
                detail = $"{detail} · {item.SizeText}";
            }

            return active ? $"현재 표시 · {detail}" : detail;
        }

        private static string FormatLayerResultDetailText(LayerResultListItem item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            string badge = (item.Badge ?? string.Empty).Trim().ToUpperInvariant();
            if (badge == "MAIN")
            {
                return "기준 이미지";
            }

            if (badge == "LAYER")
            {
                return string.IsNullOrWhiteSpace(item.DetailText) ? "이미지 레이어" : item.DetailText;
            }

            if (badge == "EMPTY")
            {
                return string.IsNullOrWhiteSpace(item.DetailText) ? "이미지 없음" : item.DetailText;
            }

            return string.IsNullOrWhiteSpace(item.DetailText)
                ? $"검사 결과 · {FormatLayerResultBadgeText(item.Badge)}"
                : $"검사 결과 · {item.DetailText}";
        }

        private static string GetLayerResultBadgeText(LayerResultListItem item, bool active)
        {
            if (active)
            {
                return "표시";
            }

            return FormatLayerResultBadgeText(item?.Badge);
        }

        private static string FormatLayerResultBadgeText(string badge)
        {
            string value = (badge ?? string.Empty).Trim().ToUpperInvariant();
            switch (value)
            {
                case "MAIN":
                    return "기준";
                case "LAYER":
                    return "레이어";
                case "EMPTY":
                    return "없음";
                default:
                    return FormatInsightBadgeText(value);
            }
        }

        private static void DrawInsightText(Graphics graphics, Font font, Rectangle bounds, string text, bool selected)
        {
            TextRenderer.DrawText(
                graphics,
                text,
                font,
                new Rectangle(bounds.Left + 8, bounds.Top, bounds.Width - 16, bounds.Height),
                selected ? Color.White : Color.FromArgb(226, 235, 248),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void DrawInsightBadge(Graphics graphics, Font font, Rectangle bounds, string text, Color backColor, bool selected)
        {
            Color borderColor = selected ? Color.FromArgb(180, 238, 248, 255) : Color.FromArgb(84, 112, 142);
            using (SolidBrush badgeBrush = new SolidBrush(backColor))
            using (Font badgeFont = new Font(font, FontStyle.Bold))
            {
                graphics.FillRectangle(badgeBrush, bounds);
                ControlPaint.DrawBorder(graphics, bounds, borderColor, ButtonBorderStyle.Solid);
                TextRenderer.DrawText(
                    graphics,
                    text,
                    badgeFont,
                    bounds,
                    GetInsightBadgeForeColor(text),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private static void DrawInsightSelectionBorder(Graphics graphics, Rectangle bounds, Color accentColor)
        {
            ControlPaint.DrawBorder(
                graphics,
                bounds,
                LightenColor(accentColor, 0.35F),
                ButtonBorderStyle.Solid);
        }

        private static Color LightenColor(Color color, float amount)
        {
            amount = Math.Max(0F, Math.Min(1F, amount));
            return Color.FromArgb(
                color.R + (int)((255 - color.R) * amount),
                color.G + (int)((255 - color.G) * amount),
                color.B + (int)((255 - color.B) * amount));
        }

        private void SetMainToolTip(Control control, string text)
        {
            if (control == null || toolTip1 == null)
            {
                return;
            }

            toolTip1.SetToolTip(control, string.IsNullOrWhiteSpace(text) ? string.Empty : text);
        }

        private void SetInsightLabelText(Label label, string text, string toolTip = null)
        {
            if (label == null)
            {
                return;
            }

            label.Text = text ?? string.Empty;
            SetMainToolTip(label, toolTip ?? label.Text);
        }

        private void RefreshInsightHeaders()
        {
            int stepCount = currentPipeline?.Steps?.Count ?? 0;
            int layerCount = displayManager?.LayerCount ?? 0;

            SetInsightLabelText(
                lblPipelineStepHeader,
                $"검사 Step · {stepCount}개",
                "현재 Recipe의 검사 순서입니다. 선택한 Step은 아래 상태 카드에 자세히 표시됩니다.");
            SetInsightLabelText(
                lblLayerResultHeader,
                $"레이어 / 결과 · {layerCount}개",
                "현재 표시 가능한 이미지 레이어와 마지막 검사 결과 요약입니다.");
            SetInsightLabelText(
                lblLogFilterHeader,
                $"로그 필터 · {activeLogFilterLabel}",
                "하단 실행 로그에 적용할 빠른 필터입니다.");
        }

        private void UpdatePipelineStepToolTip(Point location)
        {
            if (lstPipelineSteps == null)
            {
                return;
            }

            int index = lstPipelineSteps.IndexFromPoint(location);
            if (index == lastPipelineStepTooltipIndex)
            {
                return;
            }

            lastPipelineStepTooltipIndex = index;
            if (index < 0 || index >= lstPipelineSteps.Items.Count || !(lstPipelineSteps.Items[index] is PipelineStepListItem item))
            {
                SetMainToolTip(lstPipelineSteps, string.Empty);
                return;
            }

            SetMainToolTip(lstPipelineSteps, BuildPipelineStepToolTip(item));
        }

        private void ClearPipelineStepToolTip()
        {
            lastPipelineStepTooltipIndex = -1;
            SetMainToolTip(lstPipelineSteps, string.Empty);
        }

        private string BuildPipelineStepToolTip(PipelineStepListItem item)
        {
            VisionPipelineStep step = item?.Step;
            if (step == null)
            {
                return string.Empty;
            }

            mainPipelineStepSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary);
            PipelineStepVisualState visualState = ResolvePipelineStepVisualState(step, summary);
            string status = visualState.StatusText;
            string flow = summary == null
                ? FormatFlowText(step.InputLayer, step.OutputLayer)
                : FormatSummaryFlowText(step, summary);
            string time = summary?.ElapsedMilliseconds > 0
                ? $"{summary.ElapsedMilliseconds:0.0} ms"
                : "-";
            string message = string.IsNullOrWhiteSpace(summary?.Message)
                ? visualState.Message
                : summary.Message;

            return $"{item.Index:00} {step.Name} [{step.ToolType}]\r\n" +
                $"상태: {status}\r\n" +
                $"흐름: {flow}\r\n" +
                $"시간: {time}\r\n" +
                $"메시지: {message}";
        }

        private void UpdateLayerResultToolTip(Point location)
        {
            if (lstLayerResults == null)
            {
                return;
            }

            int index = lstLayerResults.IndexFromPoint(location);
            if (index == lastLayerResultTooltipIndex)
            {
                return;
            }

            lastLayerResultTooltipIndex = index;
            if (index < 0 || index >= lstLayerResults.Items.Count || !(lstLayerResults.Items[index] is LayerResultListItem item))
            {
                SetMainToolTip(lstLayerResults, string.Empty);
                return;
            }

            SetMainToolTip(lstLayerResults, BuildLayerResultToolTip(item));
        }

        private void ClearLayerResultToolTip()
        {
            lastLayerResultTooltipIndex = -1;
            SetMainToolTip(lstLayerResults, string.Empty);
        }

        private string BuildLayerResultToolTip(LayerResultListItem item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            bool active = string.Equals(item.Title, displayManager.FocusItem, StringComparison.OrdinalIgnoreCase)
                || string.Equals(item.Title, displayManager.SelectedItem, StringComparison.OrdinalIgnoreCase);
            string visibleText = active ? "현재 표시 중" : "대기";
            return $"{item.Index:00} {item.Title}\r\n" +
                $"표시: {visibleText}\r\n" +
                $"상태: {GetLayerResultBadgeText(item, active)}\r\n" +
                $"구분: {FormatLayerResultDetailText(item)}\r\n" +
                $"크기: {item.SizeText}";
        }

        private static Color GetInsightRowBackColor(int index, bool selected)
        {
            if (selected)
            {
                return Color.FromArgb(48, 72, 108);
            }

            return index % 2 == 0
                ? Color.FromArgb(21, 29, 41)
                : Color.FromArgb(26, 35, 49);
        }

        private static string GetPipelineStepBadge(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            return ResolvePipelineStepVisualState(step, summary).Badge;
        }

        private static PipelineStepVisualState ResolvePipelineStepVisualState(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (step != null && !step.Enabled)
            {
                return new PipelineStepVisualState("OFF", "비활성", "실행 제외", "비활성 Step", false, Color.FromArgb(92, 101, 119));
            }

            if (summary == null)
            {
                return new PipelineStepVisualState("READY", "실행 대기", "대기", "실행 대기", true, Color.FromArgb(54, 106, 150));
            }

            string status = string.IsNullOrWhiteSpace(summary.Status)
                ? (summary.Success ? "OK" : "READY")
                : summary.Status.Trim().ToUpperInvariant();

            if (status.Contains("NG") || status.Contains("FAIL") || status.Contains("ERROR"))
            {
                return new PipelineStepVisualState("NG", "NG", "확인 필요", "검사 실패 또는 오류", true, Color.FromArgb(188, 82, 86), true);
            }

            if (status.Contains("RUN"))
            {
                return new PipelineStepVisualState("RUN", "실행 중", "실행 중", "검사 실행 중", true, Color.FromArgb(56, 142, 190));
            }

            if (status.Contains("SKIP"))
            {
                return new PipelineStepVisualState("SKIP", "건너뜀", "건너뜀", "조건에 의해 건너뜀", true, Color.FromArgb(129, 103, 62));
            }

            if (status.Contains("CANCEL") || status.Contains("STOP"))
            {
                return new PipelineStepVisualState("STOP", "중지됨", "중지", "실행 중지됨", true, Color.FromArgb(113, 124, 146));
            }

            if (status.Contains("OK") || summary.Success)
            {
                return new PipelineStepVisualState("OK", "정상", "완료", "정상 완료", true, Color.FromArgb(47, 141, 92));
            }

            return new PipelineStepVisualState(status, FormatStatusText(status), FormatInsightBadgeText(status), FormatStatusText(status), true, GetInsightStatusBackColor(status, true));
        }

        private static string BuildPipelineStepListDetailText(PipelineStepVisualState visualState, VisionPipelineStepResultSummary summary, string toolType)
        {
            string detail = visualState?.DetailText ?? "대기";
            string type = string.IsNullOrWhiteSpace(toolType) ? "검사" : toolType;
            if (summary?.ElapsedMilliseconds > 0)
            {
                return $"{detail} · {type} · {summary.ElapsedMilliseconds:0.0} ms";
            }

            return $"{detail} · {type}";
        }

        private static Color GetInsightStatusBackColor(string status, bool enabled)
        {
            if (!enabled)
            {
                return Color.FromArgb(92, 101, 119);
            }

            string value = status ?? string.Empty;
            if (value.IndexOf("NG", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(188, 82, 86);
            }

            if (value.IndexOf("STOP", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("CANCEL", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("중지", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(113, 124, 146);
            }

            if (value.IndexOf("RUN", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("실행", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(56, 142, 190);
            }

            if (value.IndexOf("OK", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(47, 141, 92);
            }

            if (value.IndexOf("SKIP", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(129, 103, 62);
            }

            return Color.FromArgb(54, 106, 150);
        }

        private static Color GetInsightBadgeForeColor(string status)
        {
            return Color.White;
        }

        private static string FormatInsightBadgeText(string status)
        {
            string value = (status ?? string.Empty).Trim().ToUpperInvariant();
            switch (value)
            {
                case "READY":
                    return "대기";
                case "RUN":
                    return "실행";
                case "OK":
                    return "OK";
                case "NG":
                    return "NG";
                case "OFF":
                    return "OFF";
                case "SKIP":
                    return "SKIP";
                case "STOP":
                case "CANCEL":
                    return "중지";
                default:
                    return string.IsNullOrWhiteSpace(value) ? "-" : value;
            }
        }

        private static Button CreateInsightButton(string text, int width)
        {
            Button button = new Button
            {
                AutoSize = false,
                Text = text,
                Width = width,
                Height = 28,
                Margin = new Padding(0, 0, 6, 0),
                BackColor = Color.FromArgb(45, 87, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point),
                UseVisualStyleBackColor = false
            };
            ApplyInsightButtonState(button, false);
            return button;
        }

        private static void ApplyInsightButtonState(Button button, bool selected)
        {
            if (button == null)
            {
                return;
            }

            button.BackColor = selected ? Color.FromArgb(38, 128, 143) : Color.FromArgb(45, 87, 107);
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderColor = selected ? Color.FromArgb(142, 222, 230) : Color.FromArgb(83, 133, 155);
            button.FlatAppearance.MouseOverBackColor = selected ? Color.FromArgb(47, 146, 161) : Color.FromArgb(57, 106, 132);
            button.FlatAppearance.MouseDownBackColor = selected ? Color.FromArgb(28, 102, 116) : Color.FromArgb(35, 74, 95);
        }

        private void UpdateLogFilterButtonStates()
        {
            ApplyInsightButtonState(btnLogAll, string.Equals(activeLogFilterLabel, "All", StringComparison.OrdinalIgnoreCase));
            ApplyInsightButtonState(btnLogInfo, string.Equals(activeLogFilterLabel, "Info", StringComparison.OrdinalIgnoreCase));
            ApplyInsightButtonState(btnLogPipeline, string.Equals(activeLogFilterLabel, "Pipeline", StringComparison.OrdinalIgnoreCase));
            ApplyInsightButtonState(btnLogError, string.Equals(activeLogFilterLabel, "Error", StringComparison.OrdinalIgnoreCase));
        }

        private void InitializeMainToolbar()
        {
            splitContainer1.Panel1MinSize = 80;
            splitContainer1.SplitterDistance = DefaultToolbarHeight;
            panel1.Height = DefaultToolbarHeight;
            panel1.BackColor = Color.FromArgb(55, 65, 99);
            panel1.Resize += (sender, e) => LayoutMainToolbar();

            StyleMenuStrip(menuStrip1);
            InitializeViewOptionsMenu();
            StyleCombo(cbLayerList);
            StyleToggle(chkUseLayerImage);
            cbCamera.Visible = false;
            chkUseLayerImage.Text = "레이어 입력";
            rjLabel3.Visible = false;
            lbTackTime.Visible = false;

            lblCameraCaption = CreateToolbarCaption("카메라");
            lblLayerCaption = CreateToolbarCaption("레이어");
            pnlToolbarActiveLayer = CreateStatusTile("활성 레이어", "Main | 1개", 170, out lblToolbarActiveLayer);
            pnlToolbarSourceMode = CreateStatusTile("입력 기준", "Main", 150, out lblToolbarSourceMode);
            pnlToolbarFlow = CreateStatusTile("실행 흐름", "Main -> -", 220, out lblToolbarFlow);
            pnlToolbarRunSummary = CreateStatusTile("작업 상태", "대기 중", 300, out lblToolbarRunSummary);
            pnlToolbarTackTime = CreateStatusTile("소요 시간", "-", 116, out lblToolbarTackTime);
            btnRunStep = CreateToolbarCommandButton("선택", 58, ToolbarCommandRole.Primary);
            btnRunPipeline = CreateToolbarCommandButton("전체", 58, ToolbarCommandRole.Success);
            btnStopRun = CreateToolbarCommandButton("중지", 58, ToolbarCommandRole.Danger);
            btnResetView = CreateToolbarCommandButton("맞춤", 58, ToolbarCommandRole.Neutral);

            toolTip1.SetToolTip(pnlToolbarActiveLayer, "현재 활성화되어 있거나 선택된 이미지 레이어입니다.");
            toolTip1.SetToolTip(pnlToolbarSourceMode, "검사/알고리즘 실행 시 입력으로 사용되는 원본 이미지입니다.");
            toolTip1.SetToolTip(pnlToolbarFlow, "현재 선택 Step 또는 마지막 실행의 입력/출력 레이어 흐름입니다.");
            toolTip1.SetToolTip(pnlToolbarRunSummary, "마지막 실행, 선택, 변경 작업을 표시합니다.");
            toolTip1.SetToolTip(pnlToolbarTackTime, "마지막 처리 또는 파이프라인 실행 시간입니다.");
            toolTip1.SetToolTip(btnRunStep, "선택한 검사 Step만 실행합니다.");
            toolTip1.SetToolTip(btnRunPipeline, "현재 파이프라인 전체를 실행합니다.");
            toolTip1.SetToolTip(btnStopRun, "실행 중인 파이프라인을 중지 요청합니다.");
            toolTip1.SetToolTip(btnResetView, "현재 레이어 보기 배율을 화면 맞춤으로 되돌립니다.");

            btnRunStep.Click += async (sender, e) => await RunPipelineFromMainAsync(selectedOnly: true);
            btnRunPipeline.Click += async (sender, e) => await RunPipelineFromMainAsync(selectedOnly: false);
            btnStopRun.Click += (sender, e) => StopMainPipelineRun();
            btnResetView.Click += (sender, e) => ResetActiveLayerView();

            panel1.Controls.Add(lblLayerCaption);
            panel1.Controls.Add(pnlToolbarActiveLayer);
            panel1.Controls.Add(pnlToolbarSourceMode);
            panel1.Controls.Add(pnlToolbarFlow);
            panel1.Controls.Add(pnlToolbarRunSummary);
            panel1.Controls.Add(pnlToolbarTackTime);
            panel1.Controls.Add(btnRunStep);
            panel1.Controls.Add(btnRunPipeline);
            panel1.Controls.Add(btnStopRun);
            panel1.Controls.Add(btnResetView);

            LayoutMainToolbar();
            RefreshPipelineWorkspace();
            SetMainPipelineRunningState(false);
            RefreshToolbarStatus("대기 중");
            UpdateMainRunCommandAvailability();
        }

        private void LayoutMainToolbar()
        {
            if (panel1 == null) { return; }
            if (lblCameraCaption == null || lblLayerCaption == null || pnlToolbarActiveLayer == null || pnlToolbarSourceMode == null || pnlToolbarFlow == null || pnlToolbarRunSummary == null || pnlToolbarTackTime == null) { return; }

            int panelWidth = panel1.ClientSize.Width;
            bool compact = panelWidth < 1320;
            bool narrow = panelWidth < 1080;
            ApplyToolbarCommandMode(compact, narrow);

            int y = 8;
            int margin = 14;
            int rowHeight = 28;
            int buttonGap = compact ? 5 : 6;

            int commandRight = panelWidth - margin;
            btnResetView.SetBounds(commandRight - btnResetView.Width, y, btnResetView.Width, rowHeight);
            commandRight = btnResetView.Left - buttonGap;
            btnStopRun.SetBounds(commandRight - btnStopRun.Width, y, btnStopRun.Width, rowHeight);
            commandRight = btnStopRun.Left - buttonGap;
            btnRunPipeline.SetBounds(commandRight - btnRunPipeline.Width, y, btnRunPipeline.Width, rowHeight);
            commandRight = btnRunPipeline.Left - buttonGap;
            btnRunStep.SetBounds(commandRight - btnRunStep.Width, y, btnRunStep.Width, rowHeight);
            commandRight = btnRunStep.Left - 8;
            btnNewPanel.SetBounds(commandRight - 30, y, 30, 30);

            int controlsRight = btnNewPanel.Left - 14;
            int x = margin;
            int availableTopWidth = Math.Max(0, controlsRight - x);
            int menuWidth = CalculateToolbarMenuWidth(availableTopWidth, compact, narrow);
            menuStrip1.SetBounds(x, y, menuWidth, 30);
            x = menuStrip1.Right + 12;

            int remaining = Math.Max(0, controlsRight - x);
            bool hideInputCaptions = remaining < 360;
            int layerCaptionWidth = hideInputCaptions ? 0 : 44;
            int checkWidth = narrow ? 104 : compact ? 124 : 142;
            int captionGap = hideInputCaptions ? 0 : 6;
            int comboArea = Math.Max(180, remaining - layerCaptionWidth - captionGap - 12 - checkWidth);
            int layerWidth = ClampInt(comboArea, 150, compact ? 220 : 260);

            lblCameraCaption.Visible = false;
            cbCamera.Visible = false;
            lblLayerCaption.Visible = !hideInputCaptions;

            lblLayerCaption.SetBounds(x, y + 5, layerCaptionWidth, 18);
            cbLayerList.Location = new Point(lblLayerCaption.Visible ? lblLayerCaption.Right + captionGap : x, y);
            cbLayerList.Size = new Size(layerWidth, rowHeight);
            x = cbLayerList.Right + 12;

            chkUseLayerImage.Location = new Point(x, y);
            chkUseLayerImage.Size = new Size(checkWidth, rowHeight);

            int statusY = 44;
            int statusX = margin;
            int statusGap = 8;
            int statusAvailable = Math.Max(0, panelWidth - margin * 2);
            int activeWidth = compact ? 154 : 170;
            int sourceWidth = compact ? 132 : 150;
            int flowWidth = compact ? 184 : 220;
            int tackWidth = compact ? 98 : 116;
            bool showSource = statusAvailable >= 720;
            bool showFlow = statusAvailable >= 900;
            bool showTackTime = statusAvailable >= 1080;

            pnlToolbarActiveLayer.Visible = true;
            pnlToolbarSourceMode.Visible = showSource;
            pnlToolbarFlow.Visible = showFlow;
            pnlToolbarTackTime.Visible = showTackTime;

            pnlToolbarActiveLayer.SetBounds(statusX, statusY, activeWidth, 30);
            statusX = pnlToolbarActiveLayer.Right + statusGap;
            if (showSource)
            {
                pnlToolbarSourceMode.SetBounds(statusX, statusY, sourceWidth, 30);
                statusX = pnlToolbarSourceMode.Right + statusGap;
            }

            if (showFlow)
            {
                pnlToolbarFlow.SetBounds(statusX, statusY, flowWidth, 30);
                statusX = pnlToolbarFlow.Right + statusGap;
            }

            int rightReserved = showTackTime ? tackWidth + statusGap : 0;
            int summaryWidth = Math.Max(180, panelWidth - margin - statusX - rightReserved);
            pnlToolbarRunSummary.SetBounds(statusX, statusY, summaryWidth, 30);
            if (showTackTime)
            {
                pnlToolbarTackTime.SetBounds(pnlToolbarRunSummary.Right + statusGap, statusY, tackWidth, 30);
            }
        }

        private static int CalculateToolbarMenuWidth(int availableWidth, bool compact, bool narrow)
        {
            if (availableWidth <= 0)
            {
                return 0;
            }

            int reservedForInputs = narrow ? 430 : compact ? 500 : 560;
            int preferredWidth = narrow ? 200 : compact ? 260 : 330;
            int minWidth = narrow ? 160 : compact ? 210 : 280;
            int maxWidth = availableWidth - reservedForInputs;
            if (maxWidth < minWidth)
            {
                return Math.Min(availableWidth, Math.Max(120, Math.Min(preferredWidth, availableWidth / 3)));
            }

            return ClampInt(preferredWidth, minWidth, maxWidth);
        }

        private static int ClampInt(int value, int min, int max)
        {
            if (max < min)
            {
                return min;
            }

            return Math.Max(min, Math.Min(value, max));
        }

        private void ApplyToolbarCommandMode(bool compact, bool narrow)
        {
            SetToolbarCommandButtonText(btnRunStep, "선택", 58);
            SetToolbarCommandButtonText(btnRunPipeline, "전체", 58);
            SetToolbarCommandButtonText(btnStopRun, mainPipelineStopRequested ? "요청" : "중지", 58);
            SetToolbarCommandButtonText(btnResetView, "맞춤", 58);

            if (btnNewPanel != null)
            {
                btnNewPanel.BackColor = Color.Transparent;
                btnNewPanel.ForeColor = Color.White;
                btnNewPanel.IconChar = FontAwesome.Sharp.IconChar.Plus;
                btnNewPanel.IconColor = Color.White;
                btnNewPanel.IconSize = 24;
                btnNewPanel.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            if (chkUseLayerImage != null)
            {
                chkUseLayerImage.Text = narrow ? "입력" : "레이어 입력";
            }
        }

        private Label CreateToolbarCaption(string text)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                ForeColor = Color.FromArgb(220, 232, 250),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
        }

        private Panel CreateStatusTile(string title, string value, int width, out Label valueLabel)
        {
            Panel panel = new Panel
            {
                AutoSize = false,
                Width = width,
                Height = 30,
                BackColor = Color.FromArgb(41, 50, 78),
                Padding = new Padding(0)
            };

            Panel accentPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 3,
                BackColor = Color.FromArgb(115, 165, 235)
            };

            Label titleLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 13,
                Text = title,
                ForeColor = Color.FromArgb(170, 197, 234),
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 7.2F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 6, 0),
                AutoEllipsis = true
            };

            valueLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Text = value,
                ForeColor = Color.FromArgb(250, 252, 255),
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 8.7F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 6, 1),
                AutoEllipsis = true
            };

            panel.Controls.Add(valueLabel);
            panel.Controls.Add(titleLabel);
            panel.Controls.Add(accentPanel);
            panel.Tag = new ToolbarStatusTileParts(accentPanel, titleLabel, valueLabel);
            return panel;
        }

        private sealed class ToolbarStatusTileParts
        {
            public ToolbarStatusTileParts(Panel accentPanel, Label titleLabel, Label valueLabel)
            {
                AccentPanel = accentPanel;
                TitleLabel = titleLabel;
                ValueLabel = valueLabel;
            }

            public Panel AccentPanel { get; }
            public Label TitleLabel { get; }
            public Label ValueLabel { get; }
        }

        private enum ToolbarTileMood
        {
            Neutral,
            Active,
            Running,
            Success,
            Warning,
            Error,
            Muted
        }

        private enum ToolbarCommandRole
        {
            Primary,
            Success,
            Danger,
            Neutral
        }

        private static Button CreateToolbarCommandButton(string text, int width, ToolbarCommandRole role)
        {
            Button button = new Button
            {
                AutoSize = false,
                Text = text,
                Width = width,
                Height = 28,
                BackColor = Color.FromArgb(42, 80, 108),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point),
                Margin = Padding.Empty,
                UseVisualStyleBackColor = false
            };
            ApplyToolbarCommandStyle(button, role, true);
            return button;
        }

        private static void SetToolbarCommandButtonText(Button button, string text, int width)
        {
            if (button == null) { return; }
            if (button.Text != text) { button.Text = text; }
            if (button.Width != width) { button.Width = width; }
        }

        private static void ApplyToolbarCommandStyle(Button button, ToolbarCommandRole role, bool enabled)
        {
            if (button == null) { return; }

            Color backColor;
            Color borderColor;
            Color hoverColor;
            Color downColor;
            switch (role)
            {
                case ToolbarCommandRole.Success:
                    backColor = Color.FromArgb(33, 112, 86);
                    borderColor = Color.FromArgb(81, 168, 128);
                    hoverColor = Color.FromArgb(43, 132, 101);
                    downColor = Color.FromArgb(24, 88, 69);
                    break;
                case ToolbarCommandRole.Danger:
                    backColor = Color.FromArgb(134, 64, 71);
                    borderColor = Color.FromArgb(194, 118, 124);
                    hoverColor = Color.FromArgb(156, 78, 85);
                    downColor = Color.FromArgb(104, 45, 52);
                    break;
                case ToolbarCommandRole.Neutral:
                    backColor = Color.FromArgb(68, 83, 113);
                    borderColor = Color.FromArgb(120, 139, 178);
                    hoverColor = Color.FromArgb(82, 99, 132);
                    downColor = Color.FromArgb(50, 62, 88);
                    break;
                default:
                    backColor = Color.FromArgb(42, 80, 108);
                    borderColor = Color.FromArgb(92, 124, 168);
                    hoverColor = Color.FromArgb(52, 97, 130);
                    downColor = Color.FromArgb(30, 64, 92);
                    break;
            }

            if (!enabled)
            {
                backColor = Color.FromArgb(48, 54, 72);
                borderColor = Color.FromArgb(68, 77, 101);
                hoverColor = backColor;
                downColor = backColor;
            }

            button.BackColor = backColor;
            button.ForeColor = enabled ? Color.White : Color.FromArgb(176, 185, 202);
            button.FlatAppearance.BorderColor = borderColor;
            button.FlatAppearance.MouseOverBackColor = enabled ? hoverColor : backColor;
            button.FlatAppearance.MouseDownBackColor = enabled ? downColor : backColor;
        }

        private void ApplyToolbarCommandEnabled(Button button, ToolbarCommandRole role, bool enabled)
        {
            if (button == null)
            {
                return;
            }

            button.Enabled = enabled;
            button.Cursor = enabled ? Cursors.Hand : Cursors.Default;
            ApplyToolbarCommandStyle(button, role, enabled);
        }

        private static void StyleMenuStrip(MenuStrip menuStrip)
        {
            menuStrip.AutoSize = false;
            menuStrip.Padding = new Padding(0);
            menuStrip.Margin = Padding.Empty;
            menuStrip.BackColor = Color.FromArgb(55, 65, 99);
            menuStrip.ForeColor = Color.WhiteSmoke;
            menuStrip.RenderMode = ToolStripRenderMode.Professional;
            menuStrip.Renderer = new WorkbenchMenuRenderer();
            menuStrip.ShowItemToolTips = true;

            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                StyleTopMenuItem(item);
            }
        }

        private void InitializeViewOptionsMenu()
        {
            if (viewToolStripMenuItem != null) { return; }

            imageProcessingToolStripMenuItem.Text = "이미지 처리";
            imageProcessingToolStripMenuItem.Click -= OnToolStripMenuItem_Click;
            algorithmToolStripMenuItem.Text = "알고리즘";
            ApplyVisionMenuLabels();

            viewToolStripMenuItem = new ToolStripMenuItem
            {
                Text = "보기",
                BackColor = Color.FromArgb(55, 65, 99),
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 9.3F, FontStyle.Bold, GraphicsUnit.Point)
            };

            showLogDockTitleMenuItem = new ToolStripMenuItem
            {
                Text = "로그 도킹 제목 표시",
                CheckOnClick = true,
                Checked = showLogDockTitle
            };
            showLogDockTitleMenuItem.CheckedChanged += (sender, e) =>
            {
                if (isApplyingWorkspaceLayout) { return; }
                SetLogDockTitleVisible(showLogDockTitleMenuItem.Checked, true);
            };

            saveWorkspaceLayoutMenuItem = new ToolStripMenuItem
            {
                Text = "레이아웃 저장"
            };
            saveWorkspaceLayoutMenuItem.Click += (sender, e) =>
            {
                bool saved = SaveWorkspaceLayout(true);
                RefreshToolbarStatus(saved ? "레이아웃 저장 완료" : "레이아웃 저장 실패");
            };

            loadWorkspaceLayoutMenuItem = new ToolStripMenuItem
            {
                Text = "레이아웃 불러오기"
            };
            loadWorkspaceLayoutMenuItem.Click += (sender, e) => LoadWorkspaceLayoutFromMenu();

            diagnoseWorkspaceLayoutMenuItem = new ToolStripMenuItem
            {
                Text = "레이아웃 상태 확인"
            };
            diagnoseWorkspaceLayoutMenuItem.Click += (sender, e) => ShowWorkspaceLayoutDiagnostics();

            viewToolStripMenuItem.DropDownItems.Add(saveWorkspaceLayoutMenuItem);
            viewToolStripMenuItem.DropDownItems.Add(loadWorkspaceLayoutMenuItem);
            viewToolStripMenuItem.DropDownItems.Add(diagnoseWorkspaceLayoutMenuItem);
            viewToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            viewToolStripMenuItem.DropDownItems.Add(showLogDockTitleMenuItem);
            menuStrip1.Items.Add(viewToolStripMenuItem);
            StyleMenuStrip(menuStrip1);
        }

        private void ApplyVisionMenuLabels()
        {
            ConfigureVisionMenuItem(morphologyToolStripMenuItem, VISION_MENU.Morphology);
            ConfigureVisionMenuItem(filterToolStripMenuItem, VISION_MENU.Filter);
            ConfigureVisionMenuItem(arithmeticToolStripMenuItem, VISION_MENU.Arithmetic);
            ConfigureVisionMenuItem(edgeDetectionToolStripMenuItem, VISION_MENU.EdgeDetection);
            ConfigureVisionMenuItem(rotateAndScaleToolStripMenuItem, VISION_MENU.RotateAndScale);
            ConfigureVisionMenuItem(histogramToolStripMenuItem, VISION_MENU.Histogram);
            ConfigureVisionMenuItem(hSVToolStripMenuItem, VISION_MENU.HSV);
            ConfigureVisionMenuItem(blobToolStripMenuItem, VISION_MENU.Blob);
            ConfigureVisionMenuItem(contourToolStripMenuItem, VISION_MENU.Contour);
            ConfigureVisionMenuItem(matchingToolStripMenuItem, VISION_MENU.Matching);
            ConfigureVisionMenuItem(featureMatchingToolStripMenuItem, VISION_MENU.FeatureMatching);
            ConfigureVisionMenuItem(lineToolStripMenuItem, VISION_MENU.Line);
            ConfigureVisionMenuItem(meanToolStripMenuItem, VISION_MENU.Mean);
            ConfigureVisionMenuItem(pipelineToolStripMenuItem, VISION_MENU.Pipeline);
        }

        private static void ConfigureVisionMenuItem(ToolStripMenuItem menuItem, VISION_MENU menu)
        {
            if (menuItem == null) { return; }

            string displayName = GetVisionMenuDisplayName(menu);
            menuItem.Tag = menu;
            menuItem.Text = displayName;
            menuItem.ToolTipText = $"{displayName} 도구 열기";
        }

        private static string GetVisionMenuDisplayName(VISION_MENU menu)
        {
            switch (menu)
            {
                case VISION_MENU.Morphology:
                    return "모폴로지";
                case VISION_MENU.Filter:
                    return "필터";
                case VISION_MENU.Arithmetic:
                    return "산술 연산";
                case VISION_MENU.EdgeDetection:
                    return "엣지 검출";
                case VISION_MENU.RotateAndScale:
                    return "회전 / 스케일";
                case VISION_MENU.Histogram:
                    return "히스토그램";
                case VISION_MENU.HSV:
                    return "HSV";
                case VISION_MENU.Blob:
                    return "블랍";
                case VISION_MENU.Contour:
                    return "컨투어";
                case VISION_MENU.Matching:
                    return "매칭";
                case VISION_MENU.FeatureMatching:
                    return "특징 매칭";
                case VISION_MENU.Line:
                    return "라인";
                case VISION_MENU.Mean:
                    return "평균";
                case VISION_MENU.Pipeline:
                    return "파이프라인";
                default:
                    return menu.ToString();
            }
        }

        private static void StyleTopMenuItem(ToolStripMenuItem item)
        {
            item.AutoSize = false;
            item.Height = 30;
            item.Padding = new Padding(12, 0, 12, 0);
            item.Margin = new Padding(0, 0, 4, 0);
            item.ForeColor = Color.FromArgb(246, 250, 255);
            item.BackColor = Color.FromArgb(61, 73, 112);
            item.Font = new Font("Segoe UI", 9.3F, FontStyle.Bold, GraphicsUnit.Point);
            item.DisplayStyle = ToolStripItemDisplayStyle.Text;

            int width = item.HasDropDownItems ? 104 : Math.Max(72, TextRenderer.MeasureText(item.Text, item.Font).Width + 30);
            item.Size = new Size(width, 30);

            foreach (ToolStripItem child in item.DropDownItems)
            {
                StyleDropDownItem(child);
            }
        }

        private static void StyleDropDownItem(ToolStripItem item)
        {
            item.ForeColor = Color.FromArgb(235, 241, 248);
            item.BackColor = Color.FromArgb(35, 43, 65);
            item.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            item.Padding = new Padding(10, 3, 12, 3);

            if (item is ToolStripMenuItem menuItem)
            {
                menuItem.AutoSize = false;
                menuItem.Size = new Size(Math.Max(128, TextRenderer.MeasureText(menuItem.Text, menuItem.Font).Width + 46), 28);
                menuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            }
        }

        private sealed class WorkbenchMenuRenderer : ToolStripProfessionalRenderer
        {
            private static readonly Color TopItemBack = Color.FromArgb(61, 73, 112);
            private static readonly Color TopItemHover = Color.FromArgb(76, 93, 141);
            private static readonly Color DropDownBack = Color.FromArgb(31, 38, 58);
            private static readonly Color DropDownHover = Color.FromArgb(54, 78, 116);
            private static readonly Color Border = Color.FromArgb(84, 101, 150);

            public WorkbenchMenuRenderer()
                : base(new WorkbenchMenuColorTable())
            {
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using (Pen pen = new Pen(Color.FromArgb(80, 96, 143)))
                {
                    Rectangle rect = new Rectangle(Point.Empty, e.ToolStrip.Size);
                    rect.Width -= 1;
                    rect.Height -= 1;
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                bool topLevel = e.Item.Owner is MenuStrip;
                Color fill = topLevel
                    ? (e.Item.Selected ? TopItemHover : TopItemBack)
                    : (e.Item.Selected ? DropDownHover : DropDownBack);

                if (topLevel)
                {
                    bounds.Inflate(-1, -3);
                }

                using (SolidBrush brush = new SolidBrush(fill))
                {
                    e.Graphics.FillRectangle(brush, bounds);
                }

                if (topLevel)
                {
                    using (Pen pen = new Pen(Border))
                    {
                        e.Graphics.DrawRectangle(pen, bounds);
                    }
                }
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = Color.FromArgb(230, 238, 250);
                base.OnRenderArrow(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                using (Pen pen = new Pen(Color.FromArgb(70, 82, 118)))
                {
                    int y = e.Item.Height / 2;
                    e.Graphics.DrawLine(pen, 8, y, e.Item.Width - 8, y);
                }
            }
        }

        private sealed class WorkbenchMenuColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground => Color.FromArgb(31, 38, 58);
            public override Color ImageMarginGradientBegin => Color.FromArgb(31, 38, 58);
            public override Color ImageMarginGradientMiddle => Color.FromArgb(31, 38, 58);
            public override Color ImageMarginGradientEnd => Color.FromArgb(31, 38, 58);
            public override Color MenuBorder => Color.FromArgb(84, 101, 150);
            public override Color MenuItemBorder => Color.FromArgb(92, 116, 166);
            public override Color MenuItemSelected => Color.FromArgb(54, 78, 116);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(54, 78, 116);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(54, 78, 116);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(76, 93, 141);
            public override Color MenuItemPressedGradientMiddle => Color.FromArgb(76, 93, 141);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(76, 93, 141);
            public override Color SeparatorDark => Color.FromArgb(70, 82, 118);
            public override Color SeparatorLight => Color.FromArgb(70, 82, 118);
        }

        private static string WorkspaceLayoutStatePath => Path.Combine(
            AppPathService.EnsureDirectory("CONFIG", WorkspaceLayoutDirectoryName),
            WorkspaceLayoutFileName);

        private static string DockPanelLayoutPath => Path.Combine(
            AppPathService.EnsureDirectory("CONFIG", WorkspaceLayoutDirectoryName),
            DockPanelLayoutFileName);

        private static int NormalizeInlineLogPanelHeight(int height)
        {
            if (height <= 0)
            {
                return DefaultInlineLogPanelHeight;
            }

            return Math.Max(96, Math.Min(560, height));
        }

        private void InstallDockCaptionFactory()
        {
            if (dockPanel?.Theme?.Extender == null)
            {
                return;
            }

            dockPanel.Theme.Extender.DockPaneCaptionFactory = new WorkbenchDockPaneCaptionFactory(() => showLogDockTitle);
        }

        private MainWorkspaceLayoutState LoadMainWorkspaceLayoutState()
        {
            MainWorkspaceLayoutState defaultState = new MainWorkspaceLayoutState
            {
                ShowLogDockTitle = false,
                InlineLogPanelHeight = DefaultInlineLogPanelHeight,
                ToolbarHeight = DefaultToolbarHeight
            };

            return SerializeHelper.LoadOrCreateXmlFile(WorkspaceLayoutStatePath, defaultState, out _);
        }

        private void ApplyLoadedWorkspaceLayoutState(MainWorkspaceLayoutState state)
        {
            if (state == null)
            {
                state = new MainWorkspaceLayoutState();
            }

            showLogDockTitle = state.ShowLogDockTitle;
            inlineLogPanelHeight = NormalizeInlineLogPanelHeight(state.InlineLogPanelHeight);

            if (showLogDockTitleMenuItem != null)
            {
                isApplyingWorkspaceLayout = true;
                try
                {
                    showLogDockTitleMenuItem.Checked = showLogDockTitle;
                }
                finally
                {
                    isApplyingWorkspaceLayout = false;
                }
            }

            if (panel1 != null)
            {
                panel1.Height = Math.Max(DefaultToolbarHeight, state.ToolbarHeight);
            }
        }

        private void RegisterLayoutPersistenceEvents()
        {
            Form owner = TopLevelControl as Form ?? FindForm();
            if (owner == null || owner == this || owner == layoutOwnerForm)
            {
                return;
            }

            if (layoutOwnerForm != null)
            {
                layoutOwnerForm.FormClosing -= LayoutOwnerForm_FormClosing;
            }

            layoutOwnerForm = owner;
            layoutOwnerForm.FormClosing += LayoutOwnerForm_FormClosing;
        }

        private void LayoutOwnerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveWorkspaceLayout(false);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveWorkspaceLayout(false);
            base.OnFormClosing(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            RegisterLayoutPersistenceEvents();
        }

        private void RememberInlineLogPanelHeight()
        {
            if (workspaceSplitContainer == null || workspaceSplitContainer.Panel2Collapsed)
            {
                return;
            }

            inlineLogPanelHeight = NormalizeInlineLogPanelHeight(workspaceSplitContainer.Panel2.Height);
        }

        private MainWorkspaceLayoutState CaptureWorkspaceLayoutState()
        {
            RememberInlineLogPanelHeight();

            return new MainWorkspaceLayoutState
            {
                ShowLogDockTitle = showLogDockTitle,
                InlineLogPanelHeight = inlineLogPanelHeight,
                ToolbarHeight = Math.Max(DefaultToolbarHeight, panel1?.Height ?? DefaultToolbarHeight)
            };
        }

        private bool SaveWorkspaceLayout()
        {
            return SaveWorkspaceLayout(false);
        }

        private bool SaveWorkspaceLayout(bool saveDockPanelLayout)
        {
            bool saved = true;
            try
            {
                SerializeHelper.SaveXmlFile(WorkspaceLayoutStatePath, CaptureWorkspaceLayoutState());
            }
            catch (Exception ex)
            {
                OVLog.Write(LogCategory.Main, LogLevel.Warning, $"Main workspace state save failed. {ex.GetBaseException().Message}");
                saved = false;
            }

            if (saveDockPanelLayout)
            {
                saved = SaveDockPanelLayoutSafe() && saved;
            }

            return saved;
        }

        private bool ShouldSaveDockPanelLayout()
        {
            if (!Forms.TryGetValue(VISION_DOCK_FORM.LOG, out object value) || !(value is DockContent logViewer) || logViewer.IsDisposed)
            {
                return true;
            }

            return logViewer.DockState != DockState.Hidden;
        }

        private bool LoadDockPanelLayoutSafe()
        {
            if (dockPanel == null || !File.Exists(DockPanelLayoutPath))
            {
                return false;
            }

            try
            {
                OVLog.Write(LogCategory.Main, LogLevel.Info, $"Main dock layout loading. Path={DockPanelLayoutPath} | Saved={ReadSavedDockLayoutSummary(DockPanelLayoutPath)}");
                bool applied = ApplySavedDockPanelLayout(DockPanelLayoutPath);
                OVLog.Write(
                    LogCategory.UI,
                    applied ? LogLevel.Info : LogLevel.Warning,
                    $"Main dock layout {(applied ? "applied" : "not applied")}. Runtime={CaptureRuntimeDockLayoutSummary()}");
                return applied;
            }
            catch (Exception ex)
            {
                BackupInvalidLayoutFile(DockPanelLayoutPath);
                OVLog.Write(LogCategory.Main, LogLevel.Warning, $"Main dock layout load failed. Default layout will be used. {ex.GetBaseException().Message}");
                return false;
            }
        }

        private void HideDockContentsForLayoutLoad()
        {
            List<DockContent> visibleContents = new List<DockContent>();
            foreach (IDockContent dockContent in dockPanel.Contents)
            {
                if (!(dockContent is DockContent content) || content.IsDisposed || content.DockPanel == null)
                {
                    continue;
                }

                if (content is FormLayerDisplay)
                {
                    continue;
                }

                if (content.DockState != DockState.Hidden)
                {
                    visibleContents.Add(content);
                }
            }

            foreach (DockContent content in visibleContents)
            {
                content.Hide();
            }
        }

        private bool ApplySavedDockPanelLayout(string path)
        {
            XDocument document = XDocument.Load(path);
            Dictionary<string, SavedDockContentLayout> layouts = ReadSavedDockContentLayouts(document);

            EnsureMainDocumentVisible();
            DockContent mainDocument = GetLayerDisplayContentOrNull("Main");
            if (mainDocument == null || mainDocument.Pane == null)
            {
                return false;
            }

            bool thresholdApplied = ApplySavedDockContentLayout(
                EnsureDockContent(VISION_DOCK_FORM.THRESHOLD, () => new FormThreshold(displayManager)),
                FindSavedLayout(layouts, persistString => persistString == typeof(FormThreshold).FullName),
                mainDocument);

            bool logApplied = ApplySavedDockContentLayout(
                EnsureLogDockContent(),
                FindSavedLayout(layouts, persistString => persistString == typeof(FormLogViewer).FullName),
                mainDocument);

            EnsureMainDocumentVisible();
            ApplyLogPanelPresentation();
            dockPanel.PerformLayout();
            return logApplied || thresholdApplied;
        }

        private bool ApplySavedDockContentLayout(DockContent content, SavedDockContentLayout layout, DockContent mainDocument)
        {
            if (content == null || content.IsDisposed || layout == null)
            {
                return false;
            }

            if (layout.IsHidden)
            {
                if (content.DockPanel != null && content.DockState != DockState.Hidden)
                {
                    content.Hide();
                }

                return true;
            }

            DockState dockState = ParseDockState(layout.PaneDockState, DockState.DockBottom);
            if (dockState == DockState.Float)
            {
                ShowFloatLayoutContent(content, layout);
                return true;
            }

            if (dockState == DockState.Document)
            {
                ShowDocumentLayoutContent(content, layout, mainDocument);
                return true;
            }

            if (content.DockPanel == null || content.DockState == DockState.Hidden)
            {
                content.Show(dockPanel, dockState);
            }
            else
            {
                content.DockState = dockState;
            }

            return true;
        }

        private void ShowFloatLayoutContent(DockContent content, SavedDockContentLayout layout)
        {
            HideDockContentBeforeMove(content);

            Rectangle bounds = layout.FloatBounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                bounds = new Rectangle(120, 120, 640, 420);
            }

            content.Show(dockPanel, bounds);
        }

        private void ShowDocumentLayoutContent(DockContent content, SavedDockContentLayout layout, DockContent mainDocument)
        {
            DockPane targetPane = mainDocument?.Pane;
            if (targetPane == null)
            {
                content.Show(dockPanel, DockState.Document);
                return;
            }

            if (string.IsNullOrWhiteSpace(layout.Alignment))
            {
                HideDockContentBeforeMove(content);
                content.Show(targetPane, null);
                return;
            }

            DockAlignment alignment = ParseDockAlignment(layout.Alignment, DockAlignment.Right);
            double proportion = NormalizeDockProportion(layout.Proportion);
            HideDockContentBeforeMove(content);

            content.Show(targetPane, alignment, proportion);
        }

        private static void HideDockContentBeforeMove(DockContent content)
        {
            if (content?.DockPanel != null && content.DockState != DockState.Hidden)
            {
                content.Hide();
            }
        }

        private static DockState ParseDockState(string value, DockState defaultValue)
        {
            return Enum.TryParse(value, out DockState dockState) ? dockState : defaultValue;
        }

        private static DockAlignment ParseDockAlignment(string value, DockAlignment defaultValue)
        {
            return Enum.TryParse(value, out DockAlignment alignment) ? alignment : defaultValue;
        }

        private static double NormalizeDockProportion(double proportion)
        {
            if (double.IsNaN(proportion) || double.IsInfinity(proportion) || proportion <= 0)
            {
                return 0.5;
            }

            return Math.Max(0.15, Math.Min(0.85, proportion));
        }

        private static SavedDockContentLayout FindSavedLayout(Dictionary<string, SavedDockContentLayout> layouts, Func<string, bool> isMatch)
        {
            return layouts
                .Where(pair => isMatch(pair.Key))
                .Select(pair => pair.Value)
                .FirstOrDefault();
        }

        private static Dictionary<string, SavedDockContentLayout> ReadSavedDockContentLayouts(XDocument document)
        {
            Dictionary<string, SavedDockContentLayout> layouts = new Dictionary<string, SavedDockContentLayout>();
            XElement contentsElement = document.Root?.Element("Contents");
            XElement panesElement = document.Root?.Element("Panes");
            XElement dockWindowsElement = document.Root?.Element("DockWindows");

            if (contentsElement == null || panesElement == null)
            {
                return layouts;
            }

            foreach (XElement content in contentsElement.Elements("Content"))
            {
                string contentId = (string)content.Attribute("ID");
                string persistString = (string)content.Attribute("PersistString");
                if (string.IsNullOrWhiteSpace(contentId) || string.IsNullOrWhiteSpace(persistString))
                {
                    continue;
                }

                XElement pane = FindPaneForContent(panesElement, contentId);
                string paneId = (string)pane?.Attribute("ID");
                XElement nestedPane = FindNestedPaneForPane(
                    dockWindowsElement,
                    document.Root?.Element("FloatWindows"),
                    paneId,
                    out XElement dockWindow,
                    out XElement floatWindow);

                layouts[persistString] = new SavedDockContentLayout
                {
                    PersistString = persistString,
                    PaneId = paneId,
                    PaneDockState = (string)pane?.Attribute("DockState") ?? string.Empty,
                    WindowDockState = (string)dockWindow?.Attribute("DockState") ?? string.Empty,
                    PrevPaneId = (string)nestedPane?.Attribute("PrevPane") ?? string.Empty,
                    Alignment = (string)nestedPane?.Attribute("Alignment") ?? string.Empty,
                    Proportion = ParseDouble((string)nestedPane?.Attribute("Proportion"), 0.5),
                    FloatBounds = ParseRectangle((string)floatWindow?.Attribute("Bounds")),
                    IsHidden = string.Equals((string)content.Attribute("IsHidden"), "True", StringComparison.OrdinalIgnoreCase),
                    IsFloat = string.Equals((string)content.Attribute("IsFloat"), "True", StringComparison.OrdinalIgnoreCase)
                };
            }

            return layouts;
        }

        private static XElement FindPaneForContent(XElement panesElement, string contentId)
        {
            List<XElement> candidates = panesElement?
                .Elements("Pane")
                .Where(element => element
                    .Element("Contents")?
                    .Elements("Content")
                    .Any(reference => (string)reference.Attribute("RefID") == contentId) == true)
                .ToList() ?? new List<XElement>();

            return candidates.FirstOrDefault(element => (string)element.Attribute("ActiveContent") == contentId)
                ?? candidates.FirstOrDefault(element => !string.Equals((string)element.Attribute("DockState"), "Float", StringComparison.OrdinalIgnoreCase))
                ?? candidates.FirstOrDefault();
        }

        private static XElement FindNestedPaneForPane(
            XElement dockWindowsElement,
            XElement floatWindowsElement,
            string paneId,
            out XElement dockWindow,
            out XElement floatWindow)
        {
            dockWindow = null;
            floatWindow = null;
            if (string.IsNullOrWhiteSpace(paneId))
            {
                return null;
            }

            foreach (XElement window in dockWindowsElement?.Elements("DockWindow") ?? Enumerable.Empty<XElement>())
            {
                XElement nestedPane = window
                    .Element("NestedPanes")?
                    .Elements("Pane")
                    .FirstOrDefault(reference => (string)reference.Attribute("RefID") == paneId);

                if (nestedPane == null)
                {
                    continue;
                }

                dockWindow = window;
                return nestedPane;
            }

            foreach (XElement window in floatWindowsElement?.Elements("FloatWindow") ?? Enumerable.Empty<XElement>())
            {
                XElement nestedPane = window
                    .Element("NestedPanes")?
                    .Elements("Pane")
                    .FirstOrDefault(reference => (string)reference.Attribute("RefID") == paneId);

                if (nestedPane == null)
                {
                    continue;
                }

                floatWindow = window;
                return nestedPane;
            }

            return null;
        }

        private static double ParseDouble(string value, double defaultValue)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ? result : defaultValue;
        }

        private static Rectangle ParseRectangle(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Rectangle.Empty;
            }

            string[] parts = value.Split(',');
            if (parts.Length != 4)
            {
                return Rectangle.Empty;
            }

            bool parsedX = int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int x);
            bool parsedY = int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int y);
            bool parsedWidth = int.TryParse(parts[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int width);
            bool parsedHeight = int.TryParse(parts[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int height);
            bool parsed = parsedX && parsedY && parsedWidth && parsedHeight;

            return parsed ? new Rectangle(x, y, width, height) : Rectangle.Empty;
        }

        private sealed class SavedDockContentLayout
        {
            public string PersistString { get; set; }

            public string PaneId { get; set; }

            public string PaneDockState { get; set; }

            public string WindowDockState { get; set; }

            public string PrevPaneId { get; set; }

            public string Alignment { get; set; }

            public double Proportion { get; set; }

            public Rectangle FloatBounds { get; set; }

            public bool IsHidden { get; set; }

            public bool IsFloat { get; set; }
        }

        private bool SaveDockPanelLayoutSafe()
        {
            if (dockPanel == null || dockPanel.IsDisposed)
            {
                return false;
            }

            try
            {
                EnsureMainDocumentVisible();
                dockPanel.PerformLayout();
                string runtimeSummary = CaptureRuntimeDockLayoutSummary();
                dockPanel.SaveAsXml(DockPanelLayoutPath);
                string savedSummary = ReadSavedDockLayoutSummary(DockPanelLayoutPath);
                bool hasLogLayout = SavedDockLayoutContains(typeof(FormLogViewer).FullName);
                bool savedLogMatchesRuntime = SavedDockStateMatchesRuntime(typeof(FormLogViewer).FullName, GetRuntimeDockState(VISION_DOCK_FORM.LOG));
                OVLog.Write(
                    LogCategory.UI,
                    hasLogLayout && savedLogMatchesRuntime ? LogLevel.Info : LogLevel.Warning,
                    $"Main dock layout saved. Path={DockPanelLayoutPath} | Runtime={runtimeSummary} | Saved={savedSummary}");
                return hasLogLayout && savedLogMatchesRuntime;
            }
            catch (Exception ex)
            {
                OVLog.Write(LogCategory.Main, LogLevel.Warning, $"Main dock layout save failed. {ex.GetBaseException().Message}");
                return false;
            }
        }

        private void ShowWorkspaceLayoutDiagnostics()
        {
            string runtimeSummary = CaptureRuntimeDockLayoutSummary();
            string savedSummary = ReadSavedDockLayoutSummary(DockPanelLayoutPath);
            string message =
                $"Layout file\r\n{DockPanelLayoutPath}\r\n\r\n" +
                $"Runtime\r\n{runtimeSummary}\r\n\r\n" +
                $"Saved XML\r\n{savedSummary}";

            OVLog.Write(LogCategory.Main, LogLevel.Info, $"Main dock layout diagnostics. Runtime={runtimeSummary} | Saved={savedSummary} | Path={DockPanelLayoutPath}");
            VisionMessageBox.Info(this, "Layout Diagnostics", message);
        }

        private string CaptureRuntimeDockLayoutSummary()
        {
            DockContent main = GetLayerDisplayContentOrNull("Main");
            DockContent threshold = Forms.TryGetValue(VISION_DOCK_FORM.THRESHOLD, out object thresholdValue)
                ? thresholdValue as DockContent
                : null;
            DockContent log = Forms.TryGetValue(VISION_DOCK_FORM.LOG, out object logValue)
                ? logValue as DockContent
                : null;

            return string.Join(
                " | ",
                SummarizeRuntimeDockContent("Main", main),
                SummarizeRuntimeDockContent("Threshold", threshold),
                SummarizeRuntimeDockContent("Log", log));
        }

        private string GetRuntimeDockState(VISION_DOCK_FORM key)
        {
            if (!Forms.TryGetValue(key, out object value) || !(value is DockContent content) || content.IsDisposed)
            {
                return string.Empty;
            }

            return content.DockState.ToString();
        }

        private static string SummarizeRuntimeDockContent(string name, DockContent content)
        {
            if (content == null)
            {
                return $"{name}:missing";
            }

            if (content.IsDisposed)
            {
                return $"{name}:disposed";
            }

            string paneState = content.Pane == null ? "-" : content.Pane.DockState.ToString();
            return $"{name}:DockState={content.DockState},Pane={paneState},Visible={content.Visible},HasPanel={content.DockPanel != null}";
        }

        private static bool SavedDockLayoutContains(string persistString)
        {
            if (!File.Exists(DockPanelLayoutPath))
            {
                return false;
            }

            try
            {
                XDocument document = XDocument.Load(DockPanelLayoutPath);
                return document.Root?
                    .Element("Contents")?
                    .Elements("Content")
                    .Any(content => string.Equals((string)content.Attribute("PersistString"), persistString, StringComparison.Ordinal)) == true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool SavedDockStateMatchesRuntime(string persistString, string runtimeDockState)
        {
            if (string.IsNullOrWhiteSpace(runtimeDockState))
            {
                return false;
            }

            string savedDockState = ReadSavedDockPaneState(DockPanelLayoutPath, persistString);
            return string.Equals(savedDockState, runtimeDockState, StringComparison.Ordinal);
        }

        private static string ReadSavedDockPaneState(string path, string persistString)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            try
            {
                XDocument document = XDocument.Load(path);
                XElement content = document.Root?
                    .Element("Contents")?
                    .Elements("Content")
                    .FirstOrDefault(element => string.Equals((string)element.Attribute("PersistString"), persistString, StringComparison.Ordinal));

                if (content == null)
                {
                    return string.Empty;
                }

                string contentId = (string)content.Attribute("ID");
                XElement pane = FindPaneForContent(document.Root?.Element("Panes"), contentId);

                return (string)pane?.Attribute("DockState") ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static string ReadSavedDockLayoutSummary(string path)
        {
            if (!File.Exists(path))
            {
                return "file-not-found";
            }

            try
            {
                XDocument document = XDocument.Load(path);
                return string.Join(
                    " | ",
                    SummarizeSavedDockContent(document, "Main", IsLayerDisplayPersistString),
                    SummarizeSavedDockContent(document, "Threshold", persistString => persistString == typeof(FormThreshold).FullName),
                    SummarizeSavedDockContent(document, "Log", persistString => persistString == typeof(FormLogViewer).FullName));
            }
            catch (Exception ex)
            {
                return $"xml-read-failed:{ex.GetBaseException().Message}";
            }
        }

        private static string SummarizeSavedDockContent(XDocument document, string name, Func<string, bool> isMatch)
        {
            XElement contentsElement = document.Root?.Element("Contents");
            XElement panesElement = document.Root?.Element("Panes");
            XElement dockWindowsElement = document.Root?.Element("DockWindows");
            XElement content = contentsElement?
                .Elements("Content")
                .FirstOrDefault(element => isMatch((string)element.Attribute("PersistString") ?? string.Empty));

            if (content == null)
            {
                return $"{name}:missing";
            }

            string contentId = (string)content.Attribute("ID");
            XElement pane = FindPaneForContent(panesElement, contentId);

            string paneId = (string)pane?.Attribute("ID");
            FindNestedPaneForPane(
                dockWindowsElement,
                document.Root?.Element("FloatWindows"),
                paneId,
                out XElement dockWindow,
                out XElement floatWindow);

            string paneState = (string)pane?.Attribute("DockState") ?? "-";
            string windowState = (string)dockWindow?.Attribute("DockState")
                ?? (floatWindow == null ? "-" : "Float");
            string hidden = (string)content.Attribute("IsHidden") ?? "-";
            string floating = (string)content.Attribute("IsFloat") ?? "-";
            return $"{name}:Pane={paneState},Window={windowState},Hidden={hidden},Float={floating}";
        }

        private IDockContent DeserializeDockContent(string persistString)
        {
            if (IsLayerDisplayPersistString(persistString))
            {
                return ResolveLayerDisplayContent(persistString);
            }

            if (persistString == typeof(FormThreshold).FullName)
            {
                return EnsureDockContent(VISION_DOCK_FORM.THRESHOLD, () => new FormThreshold(displayManager));
            }

            if (persistString == typeof(FormLogViewer).FullName)
            {
                return EnsureDockContent(VISION_DOCK_FORM.LOG, () => new FormLogViewer());
            }

            return null;
        }

        private static bool IsLayerDisplayPersistString(string persistString)
        {
            string layerPersistPrefix = typeof(FormLayerDisplay).FullName;
            return persistString == layerPersistPrefix
                || persistString?.StartsWith($"{layerPersistPrefix}:", StringComparison.Ordinal) == true;
        }

        private IDockContent ResolveLayerDisplayContent(string persistString)
        {
            string layerTitle = GetLayerTitleFromPersistString(persistString);

            if (displayManager is DisplayManagerService displayManagerService)
            {
                FormLayerDisplay layerDisplay = displayManagerService.GetLayerDisplayOrNull(layerTitle);
                if (layerDisplay != null)
                {
                    return layerDisplay;
                }
            }

            return null;
        }

        private DockContent GetLayerDisplayContentOrNull(string layerTitle)
        {
            if (displayManager is DisplayManagerService displayManagerService)
            {
                return displayManagerService.GetLayerDisplayOrNull(layerTitle) as DockContent;
            }

            return null;
        }

        private void EnsureMainDocumentVisible()
        {
            if (dockPanel == null || dockPanel.IsDisposed)
            {
                return;
            }

            DockContent mainDocument = GetLayerDisplayContentOrNull("Main");
            if (mainDocument == null || mainDocument.IsDisposed)
            {
                displayManager.CreateLayerDisplay(new Bitmap(10, 10), "Main", false);
                mainDocument = GetLayerDisplayContentOrNull("Main");
            }

            if (mainDocument == null || mainDocument.IsDisposed)
            {
                return;
            }

            if (mainDocument.DockPanel == null || mainDocument.DockState == DockState.Hidden || !mainDocument.Visible)
            {
                mainDocument.Show(dockPanel, DockState.Document);
            }

            if (dockPanel.ActiveDocument == null)
            {
                mainDocument.Activate();
            }
        }

        private static string GetLayerTitleFromPersistString(string persistString)
        {
            string layerPersistPrefix = typeof(FormLayerDisplay).FullName;
            if (persistString?.StartsWith($"{layerPersistPrefix}:", StringComparison.Ordinal) == true)
            {
                string title = persistString.Substring(layerPersistPrefix.Length + 1);
                if (!string.IsNullOrWhiteSpace(title))
                {
                    return title;
                }
            }

            return "Main";
        }

        private DockContent EnsureDockContent(VISION_DOCK_FORM key, Func<DockContent> factory)
        {
            if (!Forms.TryGetValue(key, out object value) || !(value is DockContent content) || content.IsDisposed)
            {
                content = factory();
                Forms[key] = content;
            }

            return content;
        }

        private void BackupInvalidLayoutFile(string path)
        {
            try
            {
                if (!File.Exists(path)) { return; }

                string directory = Path.GetDirectoryName(path);
                string name = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);
                string backupPath = Path.Combine(directory, $"{name}.invalid-{DateTime.Now:yyyyMMddHHmmssfff}{extension}");
                File.Move(path, backupPath);
            }
            catch (IOException)
            {
            }
        }

        private void LoadWorkspaceLayoutFromMenu()
        {
            ApplyLoadedWorkspaceLayoutState(LoadMainWorkspaceLayoutState());

            bool dockLayoutLoaded = LoadDockPanelLayoutSafe();
            if (!dockLayoutLoaded)
            {
                ShowVisionForms();
            }

            EnsureMainDocumentVisible();
            ApplyLogPanelPresentation();
            RefreshToolbarStatus(dockLayoutLoaded ? "레이아웃 불러옴" : "기본 레이아웃 적용");
            OVLog.Write(LogCategory.Main, LogLevel.Info, dockLayoutLoaded ? "Main workspace layout loaded." : "Default main workspace layout applied.");
        }

        private void SetLogDockTitleVisible(bool visible, bool persist)
        {
            showLogDockTitle = visible;

            if (showLogDockTitleMenuItem != null && showLogDockTitleMenuItem.Checked != visible)
            {
                isApplyingWorkspaceLayout = true;
                try
                {
                    showLogDockTitleMenuItem.Checked = visible;
                }
                finally
                {
                    isApplyingWorkspaceLayout = false;
                }
            }

            ApplyLogPanelPresentation();
            RefreshLogDockCaptionLayout();

            if (persist)
            {
                SaveWorkspaceLayout(false);
                RefreshToolbarStatus(visible ? "로그 도킹 제목 표시" : "로그 도킹 제목 숨김");
                OVLog.Write(LogCategory.Main, LogLevel.Info, $"Log panel presentation changed. DockTitleVisible={visible}");
            }
        }

        private void ApplyLogPanelPresentation()
        {
            if (workspaceSplitContainer == null || dockPanel == null) { return; }

            HideInlineLogPanel();
            DockContent logViewer = ShowLogDockContent();
            ApplyLogDockTitleVisibility(logViewer);
        }

        private void ShowInlineLogPanel()
        {
            EnsureInlineLogPanel();
            workspaceSplitContainer.Panel2Collapsed = false;
            inlineLogPanel.Visible = true;
            LayoutInlineLogPanel();
        }

        private void EnsureInlineLogPanel()
        {
            if (inlineLogPanel != null) { return; }

            inlineLogPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 22, 29),
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };

            inlineLogHost = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = new LogPanelView()
            };

            inlineLogPanel.Controls.Add(inlineLogHost);
            workspaceSplitContainer.Panel2.Controls.Add(inlineLogPanel);
        }

        private void HideInlineLogPanel()
        {
            if (inlineLogPanel != null)
            {
                inlineLogPanel.Visible = false;
            }

            if (workspaceSplitContainer != null)
            {
                workspaceSplitContainer.Panel2Collapsed = true;
            }
        }

        private void LayoutInlineLogPanel()
        {
            if (workspaceSplitContainer == null || workspaceSplitContainer.Panel2Collapsed) { return; }
            if (workspaceSplitContainer.Height <= workspaceSplitContainer.Panel2MinSize + workspaceSplitContainer.Panel1MinSize) { return; }

            int availableHeight = workspaceSplitContainer.Height - workspaceSplitContainer.Panel1MinSize - workspaceSplitContainer.SplitterWidth;
            int targetLogHeight = Math.Max(
                workspaceSplitContainer.Panel2MinSize,
                Math.Min(inlineLogPanelHeight, availableHeight));

            workspaceSplitContainer.SplitterDistance = Math.Max(
                workspaceSplitContainer.Panel1MinSize,
                workspaceSplitContainer.Height - targetLogHeight - workspaceSplitContainer.SplitterWidth);
        }

        private DockContent ShowLogDockContent()
        {
            DockContent logViewer = EnsureLogDockContent();
            if (logViewer.DockPanel == null)
            {
                logViewer.Show(this.dockPanel, DockState.DockBottom);
            }
            else if (logViewer.DockState == DockState.Hidden)
            {
                logViewer.Show();
            }

            ApplyLogDockTitleVisibility(logViewer);
            logViewer.Activate();
            return logViewer;
        }

        private void ApplyLogDockTitleVisibility(DockContent logViewer)
        {
            if (logViewer is FormLogViewer formLogViewer)
            {
                formLogViewer.SetDockTitleVisible(showLogDockTitle);
            }

            RefreshLogDockCaptionLayout(logViewer);
        }

        private void RefreshLogDockCaptionLayout()
        {
            if (!Forms.TryGetValue(VISION_DOCK_FORM.LOG, out object value) || !(value is DockContent logViewer))
            {
                return;
            }

            RefreshLogDockCaptionLayout(logViewer);
        }

        private void RefreshLogDockCaptionLayout(DockContent logViewer)
        {
            DockPane pane = logViewer?.Pane;
            if (pane == null)
            {
                return;
            }

            try
            {
                MethodInfo refreshChanges = typeof(DockPane).GetMethod(
                    "RefreshChanges",
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    Type.EmptyTypes,
                    null);
                refreshChanges?.Invoke(pane, null);
            }
            catch (TargetInvocationException)
            {
            }

            pane.PerformLayout();
            pane.Invalidate(true);
            dockPanel?.PerformLayout();
        }

        private DockContent EnsureLogDockContent()
        {
            if (!Forms.TryGetValue(VISION_DOCK_FORM.LOG, out object value) || !(value is DockContent logViewer) || logViewer.IsDisposed)
            {
                logViewer = new FormLogViewer();
                Forms[VISION_DOCK_FORM.LOG] = logViewer;
            }

            return logViewer;
        }

        private void HideLogDockContent()
        {
            if (!Forms.TryGetValue(VISION_DOCK_FORM.LOG, out object value) || !(value is DockContent logViewer))
            {
                return;
            }

            if (!logViewer.IsDisposed)
            {
                logViewer.Hide();
            }
        }

        private static void StyleCombo(RJCodeUI_M1.RJControls.RJComboBox combo)
        {
            combo.BackColor = Color.FromArgb(49, 58, 88);
            combo.BorderColor = Color.FromArgb(120, 132, 190);
            combo.DropDownBackColor = Color.FromArgb(49, 58, 88);
            combo.DropDownTextColor = Color.WhiteSmoke;
            combo.ForeColor = Color.WhiteSmoke;
            combo.IconColor = Color.WhiteSmoke;
            combo.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private static void StyleToggle(RJCodeUI_M1.RJControls.RJCheckBox checkBox)
        {
            checkBox.BackColor = Color.FromArgb(49, 58, 88);
            checkBox.FlatAppearance.CheckedBackColor = Color.FromArgb(47, 111, 171);
            checkBox.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 76, 118);
            checkBox.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private void RefreshPipelineWorkspace()
        {
            currentPipeline = LoadCurrentPipelineForMain();
            BindPipelineStepList();
            RefreshLayerResultPanel();
            UpdateSelectedStepSummary();
            UpdateMainRunCommandAvailability();
        }

        private VisionPipeline LoadCurrentPipelineForMain()
        {
            string recipeName = string.IsNullOrWhiteSpace(Global?.Recipe?.Name) ? "Default" : Global.Recipe.Name;
            try
            {
                return VisionPipelineStorage.Load(recipeName, VisionPipelineAppendService.DefaultPipelineName)
                    ?? new VisionPipeline { Name = VisionPipelineAppendService.DefaultPipelineName };
            }
            catch (Exception ex)
            {
                OVLog.Write(LogCategory.Pipeline, LogLevel.Warning, $"Pipeline load failed. Recipe={recipeName}, Message={ex.GetBaseException().Message}");
                return new VisionPipeline { Name = VisionPipelineAppendService.DefaultPipelineName };
            }
        }

        private void BindPipelineStepList()
        {
            if (lstPipelineSteps == null)
            {
                return;
            }

            string selectedName = selectedPipelineStep?.Name;
            isBindingPipelineStepList = true;
            try
            {
                lstPipelineSteps.BeginUpdate();
                lstPipelineSteps.Items.Clear();

                for (int i = 0; i < currentPipeline.Steps.Count; i++)
                {
                    VisionPipelineStep step = currentPipeline.Steps[i];
                    lstPipelineSteps.Items.Add(new PipelineStepListItem(i + 1, step));
                }

                if (lstPipelineSteps.Items.Count > 0)
                {
                    int selectedIndex = 0;
                    if (!string.IsNullOrWhiteSpace(selectedName))
                    {
                        for (int i = 0; i < lstPipelineSteps.Items.Count; i++)
                        {
                            if (lstPipelineSteps.Items[i] is PipelineStepListItem item
                                && string.Equals(item.Step?.Name, selectedName, StringComparison.OrdinalIgnoreCase))
                            {
                                selectedIndex = i;
                                break;
                            }
                        }
                    }

                    lstPipelineSteps.SelectedIndex = selectedIndex;
                    selectedPipelineStep = (lstPipelineSteps.Items[selectedIndex] as PipelineStepListItem)?.Step;
                }
                else
                {
                    selectedPipelineStep = null;
                }
            }
            finally
            {
                lstPipelineSteps.EndUpdate();
                isBindingPipelineStepList = false;
            }

            lstPipelineSteps.Invalidate();
            lastPipelineStepTooltipIndex = -1;
            RefreshInsightHeaders();
            UpdateMainRunCommandAvailability();
        }

        private void OnPipelineStepSelectionChanged()
        {
            if (isBindingPipelineStepList)
            {
                return;
            }

            selectedPipelineStep = (lstPipelineSteps?.SelectedItem as PipelineStepListItem)?.Step;
            UpdateSelectedStepSummary();
            lstPipelineSteps?.Invalidate();
            UpdateMainRunCommandAvailability();
            RefreshToolbarStatus(selectedPipelineStep == null ? null : $"{selectedPipelineStep.Name} 선택");
        }

        private void UpdateSelectedStepSummary()
        {
            if (lblStepTitle == null)
            {
                return;
            }

            if (selectedPipelineStep == null)
            {
                if (lastStandaloneToolRunSummary != null)
                {
                    SetInsightLabelText(lblStepCardHeader, "직접 실행 결과");
                    ApplySummaryToStepPanel(null, lastStandaloneToolRunSummary, true, "직접 실행");
                    return;
                }

                SetInsightLabelText(lblStepCardHeader, "선택 Step 상세");
                SetInsightLabelText(lblStepTitle, "Step 없음");
                SetInsightLabelText(lblStepStatus, "상태: Step 없음");
                SetInsightLabelText(lblStepFlow, "흐름: -");
                SetInsightLabelText(lblStepTime, "시간: -");
                SetInsightLabelText(lblStepMessage, "메시지: Pipeline 화면에서 Step을 추가하세요.");
                ApplySelectedStepVisualState("OFF", false);
                return;
            }

            string stepName = string.IsNullOrWhiteSpace(selectedPipelineStep.Name) ? "Step" : selectedPipelineStep.Name;
            string toolType = string.IsNullOrWhiteSpace(selectedPipelineStep.ToolType) ? "-" : selectedPipelineStep.ToolType;
            SetInsightLabelText(lblStepCardHeader, $"{GetPipelineStepDisplayIndex(selectedPipelineStep):00} Step 상세");
            SetInsightLabelText(lblStepTitle, $"{stepName} ({toolType})");

            if (mainPipelineStepSummaries.TryGetValue(selectedPipelineStep, out VisionPipelineStepResultSummary summary))
            {
                ApplySummaryToStepPanel(selectedPipelineStep, summary, selectedPipelineStep.Enabled, toolType);
            }
            else
            {
                PipelineStepVisualState visualState = ResolvePipelineStepVisualState(selectedPipelineStep, null);
                SetInsightLabelText(lblStepFlow, $"흐름: {FormatFlowText(selectedPipelineStep.InputLayer, selectedPipelineStep.OutputLayer)}");
                SetInsightLabelText(lblStepStatus, $"상태: {visualState.StatusText}");
                SetInsightLabelText(lblStepTime, "시간: -");
                SetInsightLabelText(lblStepMessage, $"메시지: {visualState.Message}");
                ApplySelectedStepVisualState(visualState.Badge, visualState.Enabled);
            }
        }

        private void ApplySummaryToStepPanel(VisionPipelineStep step, VisionPipelineStepResultSummary summary, bool enabled, string typeText)
        {
            if (summary == null)
            {
                return;
            }

            string name = string.IsNullOrWhiteSpace(summary.Name) ? step?.Name : summary.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "검사";
            }

            string toolType = string.IsNullOrWhiteSpace(typeText) ? summary.ToolType : typeText;
            if (string.IsNullOrWhiteSpace(toolType))
            {
                toolType = "검사";
            }

            PipelineStepVisualState visualState = ResolvePipelineStepVisualState(step, summary);
            SetInsightLabelText(lblStepTitle, $"{name} ({toolType})");
            SetInsightLabelText(lblStepFlow, $"흐름: {FormatSummaryFlowText(step, summary)}");
            SetInsightLabelText(lblStepStatus, $"상태: {visualState.StatusText}");
            SetInsightLabelText(lblStepTime, summary.ElapsedMilliseconds > 0
                ? $"시간: {summary.ElapsedMilliseconds:0.0} ms"
                : visualState.Badge == "RUN" ? "시간: 실행 중" : "시간: -");
            SetInsightLabelText(lblStepMessage, string.IsNullOrWhiteSpace(summary.Message)
                ? $"메시지: {visualState.Message}"
                : $"메시지: {summary.Message}");
            ApplySelectedStepVisualState(visualState.Badge, visualState.Enabled && enabled);
        }

        private int GetPipelineStepDisplayIndex(VisionPipelineStep step)
        {
            if (step == null || currentPipeline?.Steps == null)
            {
                return 0;
            }

            int index = currentPipeline.Steps.IndexOf(step);
            return index >= 0 ? index + 1 : 0;
        }

        private void ApplySelectedStepVisualState(string status, bool enabled)
        {
            Color statusColor = GetInsightStatusBackColor(status, enabled);
            Color normalText = Color.FromArgb(216, 225, 240);
            Color mutedText = Color.FromArgb(145, 154, 170);
            Color normalBack = Color.FromArgb(36, 44, 60);
            Color statusBack = enabled
                ? DarkenColor(statusColor, 0.48F)
                : Color.FromArgb(42, 48, 60);
            Color cardBack = enabled
                ? Color.FromArgb(36, 45, 61)
                : Color.FromArgb(34, 39, 50);

            if (pnlStepSummaryCard != null)
            {
                pnlStepSummaryCard.BackColor = enabled ? statusColor : Color.FromArgb(74, 83, 100);
                if (pnlStepSummaryCard.Controls.Count > 0)
                {
                    pnlStepSummaryCard.Controls[0].BackColor = cardBack;
                }
            }

            if (lblStepCardHeader != null)
            {
                lblStepCardHeader.ForeColor = enabled ? Color.FromArgb(158, 211, 247) : mutedText;
            }

            ApplyStepStatusBadge(status, enabled, statusColor);

            if (lblStepTitle != null)
            {
                lblStepTitle.ForeColor = enabled ? Color.FromArgb(246, 251, 255) : mutedText;
                lblStepTitle.BackColor = normalBack;
            }

            if (lblStepStatus != null)
            {
                lblStepStatus.ForeColor = enabled ? Color.FromArgb(248, 252, 255) : mutedText;
                lblStepStatus.BackColor = statusBack;
            }

            if (lblStepFlow != null)
            {
                lblStepFlow.ForeColor = enabled ? normalText : mutedText;
                lblStepFlow.BackColor = normalBack;
            }

            if (lblStepTime != null)
            {
                lblStepTime.ForeColor = enabled ? normalText : mutedText;
                lblStepTime.BackColor = normalBack;
            }

            if (lblStepMessage != null)
            {
                bool warning = (status ?? string.Empty).IndexOf("NG", StringComparison.OrdinalIgnoreCase) >= 0
                    || (status ?? string.Empty).IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) >= 0
                    || (status ?? string.Empty).IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0;
                lblStepMessage.ForeColor = warning ? Color.FromArgb(255, 190, 132) : enabled ? normalText : mutedText;
                lblStepMessage.BackColor = normalBack;
            }
        }

        private void ApplyStepStatusBadge(string status, bool enabled, Color statusColor)
        {
            if (lblStepStatusBadge == null)
            {
                return;
            }

            string badgeText = FormatInsightBadgeText(status);
            lblStepStatusBadge.Text = badgeText;
            lblStepStatusBadge.BackColor = enabled ? statusColor : Color.FromArgb(74, 83, 100);
            lblStepStatusBadge.ForeColor = enabled ? GetInsightBadgeForeColor(status) : Color.FromArgb(185, 195, 211);
            SetMainToolTip(lblStepStatusBadge, $"Step 상태: {badgeText}");
        }

        private static Color DarkenColor(Color color, float amount)
        {
            amount = Math.Max(0F, Math.Min(1F, amount));
            return Color.FromArgb(
                Math.Max(0, (int)(color.R * amount)),
                Math.Max(0, (int)(color.G * amount)),
                Math.Max(0, (int)(color.B * amount)));
        }

        private LayerResultVisualState ResolveLayerResultVisualState(string layerTitle, Bitmap image)
        {
            bool isPlaceholderImage = DisplayManagerImageExtensions.IsPlaceholderBitmap(image);
            if (isPlaceholderImage)
            {
                string detail = string.Equals(layerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                    ? "기준 이미지 없음"
                    : "이미지 없음";
                return new LayerResultVisualState("EMPTY", detail, "이미지 없음", Color.FromArgb(92, 108, 132));
            }

            string sizeText = image == null ? "-" : $"{image.Width}x{image.Height}";
            if (!string.IsNullOrWhiteSpace(layerTitle)
                && mainLayerResultSummaries.TryGetValue(layerTitle, out VisionPipelineStepResultSummary summary))
            {
                PipelineStepVisualState stepVisualState = ResolvePipelineStepVisualState(null, summary);
                string name = string.IsNullOrWhiteSpace(summary.Name) ? summary.ToolType : summary.Name;
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = "검사";
                }

                string time = summary.ElapsedMilliseconds > 0
                    ? $"{summary.ElapsedMilliseconds:0.0} ms"
                    : "-";
                string resultSize = summary.HasResultImage && summary.ResultImageWidth > 0 && summary.ResultImageHeight > 0
                    ? $"{summary.ResultImageWidth}x{summary.ResultImageHeight}"
                    : sizeText;
                return new LayerResultVisualState(stepVisualState.Badge, $"{name} · {stepVisualState.StatusText} · {time}", resultSize, stepVisualState.AccentColor);
            }

            if (string.Equals(layerTitle, "Main", StringComparison.OrdinalIgnoreCase))
            {
                return new LayerResultVisualState("MAIN", "기준 이미지", sizeText, Color.FromArgb(107, 196, 238));
            }

            return new LayerResultVisualState("LAYER", "레이어", sizeText, Color.FromArgb(82, 103, 135));
        }

        private void TrackLayerResultSummary(VisionPipelineStepResultSummary summary)
        {
            if (summary == null || string.IsNullOrWhiteSpace(summary.OutputLayer))
            {
                return;
            }

            mainLayerResultSummaries[summary.OutputLayer] = summary;
        }

        private void ClearPipelineLayerResultSummaries(VisionPipeline pipeline)
        {
            foreach (VisionPipelineStep step in pipeline?.Steps ?? Enumerable.Empty<VisionPipelineStep>())
            {
                if (!string.IsNullOrWhiteSpace(step.OutputLayer))
                {
                    mainLayerResultSummaries.Remove(step.OutputLayer);
                }
            }
        }

        private void RefreshLayerResultPanel()
        {
            if (lstLayerResults == null)
            {
                return;
            }

            string selectedTitle = (lstLayerResults.SelectedItem as LayerResultListItem)?.Title;
            lstLayerResults.BeginUpdate();
            try
            {
                lstLayerResults.Items.Clear();
                for (int i = 0; i < displayManager.LayerCount; i++)
                {
                    string title = displayManager.GetLayerTitle(i);
                    Bitmap image = displayManager.GetLayerImage(i);
                    LayerResultVisualState visualState = ResolveLayerResultVisualState(title, image);
                    lstLayerResults.Items.Add(new LayerResultListItem(i + 1, title, visualState.SizeText, visualState.DetailText, visualState.Badge, visualState.AccentColor));
                }

                if (!string.IsNullOrWhiteSpace(selectedTitle))
                {
                    for (int i = 0; i < lstLayerResults.Items.Count; i++)
                    {
                        if (lstLayerResults.Items[i] is LayerResultListItem item
                            && string.Equals(item.Title, selectedTitle, StringComparison.OrdinalIgnoreCase))
                        {
                            lstLayerResults.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            finally
            {
                lstLayerResults.EndUpdate();
            }

            lstLayerResults.Invalidate();
            lastLayerResultTooltipIndex = -1;
            RefreshInsightHeaders();
            UpdateMainRunCommandAvailability();
        }

        private void OnLayerResultSelectionChanged()
        {
            if (!(lstLayerResults?.SelectedItem is LayerResultListItem item)
                || string.IsNullOrWhiteSpace(item.Title))
            {
                return;
            }

            displayManager.ActivateLayer(item.Title);
            SelectLayerComboItem(item.Title);
            lstLayerResults.Invalidate();
            RefreshToolbarStatus($"{item.Title} 보기");
        }

        private void SelectLayerListItem(string layerTitle)
        {
            if (lstLayerResults == null || string.IsNullOrWhiteSpace(layerTitle))
            {
                return;
            }

            for (int i = 0; i < lstLayerResults.Items.Count; i++)
            {
                if (lstLayerResults.Items[i] is LayerResultListItem item
                    && string.Equals(item.Title, layerTitle, StringComparison.OrdinalIgnoreCase))
                {
                    lstLayerResults.SelectedIndex = i;
                    lstLayerResults.Invalidate();
                    return;
                }
            }
        }

        private void UpdateMainRunCommandAvailability()
        {
            MainRunCommandAvailability availability = EvaluateMainRunCommandAvailability();
            SetToolbarCommandButtonText(btnStopRun, mainPipelineStopRequested ? "요청" : "중지", 58);
            ApplyToolbarCommandEnabled(btnRunStep, ToolbarCommandRole.Primary, availability.CanRunSelected);
            ApplyToolbarCommandEnabled(btnRunPipeline, ToolbarCommandRole.Success, availability.CanRunAll);
            ApplyToolbarCommandEnabled(btnStopRun, ToolbarCommandRole.Danger, availability.CanStop);
            ApplyToolbarCommandEnabled(btnResetView, ToolbarCommandRole.Neutral, availability.CanResetView);

            SetMainToolTip(btnRunStep, availability.SelectedReason);
            SetMainToolTip(btnRunPipeline, availability.AllReason);
            SetMainToolTip(btnStopRun, availability.StopReason);
            SetMainToolTip(btnResetView, availability.ResetReason);
        }

        private MainRunCommandAvailability EvaluateMainRunCommandAvailability()
        {
            if (isRunningMainPipeline)
            {
                return new MainRunCommandAvailability(
                    false,
                    false,
                    !mainPipelineStopRequested,
                    false,
                    "이미 실행 중입니다.",
                    "이미 실행 중입니다.",
                    mainPipelineStopRequested ? "중지 요청을 처리하는 중입니다." : "실행 중인 파이프라인을 중지 요청합니다.",
                    "실행 중에는 화면 맞춤을 잠시 비활성화합니다.");
            }

            VisionPipeline pipeline = currentPipeline ?? LoadCurrentPipelineForMain();
            int stepCount = pipeline?.Steps?.Count ?? 0;
            if (stepCount == 0)
            {
                return new MainRunCommandAvailability(
                    false,
                    false,
                    false,
                    CanResetActiveLayerView(),
                    "실행할 검사 Step이 없습니다.",
                    "실행할 검사 Step이 없습니다.",
                    "실행 중인 파이프라인이 없습니다.",
                    "현재 레이어 보기 배율을 화면 맞춤으로 되돌립니다.");
            }

            VisionPipelineStep selectedStep = selectedPipelineStep
                ?? pipeline.Steps.FirstOrDefault(step => step?.Enabled == true)
                ?? pipeline.Steps.FirstOrDefault(step => step != null);
            RunInputAvailability selectedInput = EvaluateRunInputAvailability(new[] { selectedStep }, sequenceOutputs: false);
            RunInputAvailability allInput = EvaluateRunInputAvailability(pipeline.Steps, sequenceOutputs: true);

            bool canRunSelected = selectedStep != null && selectedStep.Enabled && selectedInput.Success;
            bool canRunAll = pipeline.Steps.Any(step => step?.Enabled == true) && allInput.Success;
            string selectedReason = canRunSelected
                ? "선택한 검사 Step만 실행합니다."
                : BuildRunDisabledReason(selectedStep, selectedInput, "선택한 검사 Step");
            string allReason = canRunAll
                ? "현재 파이프라인 전체를 실행합니다."
                : BuildRunDisabledReason(pipeline.Steps.FirstOrDefault(step => step?.Enabled == true), allInput, "전체 파이프라인");

            return new MainRunCommandAvailability(
                canRunSelected,
                canRunAll,
                false,
                CanResetActiveLayerView(),
                selectedReason,
                allReason,
                "실행 중인 파이프라인이 없습니다.",
                "현재 레이어 보기 배율을 화면 맞춤으로 되돌립니다.");
        }

        private RunInputAvailability EvaluateRunInputAvailability(IEnumerable<VisionPipelineStep> steps, bool sequenceOutputs)
        {
            HashSet<string> availableLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                string title = displayManager.GetLayerTitle(i);
                Bitmap image = displayManager.GetLayerImage(i);
                if (!string.IsNullOrWhiteSpace(title) && !DisplayManagerImageExtensions.IsPlaceholderBitmap(image))
                {
                    availableLayers.Add(title);
                }
            }

            foreach (VisionPipelineStep step in steps ?? Enumerable.Empty<VisionPipelineStep>())
            {
                if (step == null)
                {
                    continue;
                }

                if (!step.Enabled)
                {
                    if (!sequenceOutputs)
                    {
                        return RunInputAvailability.Fail("선택한 Step이 비활성 상태입니다.");
                    }

                    continue;
                }

                string inputLayer = string.IsNullOrWhiteSpace(step.InputLayer) ? "Main" : step.InputLayer;
                if (!availableLayers.Contains(inputLayer))
                {
                    return RunInputAvailability.Fail($"입력 이미지가 없습니다. Layer={inputLayer}");
                }

                if (sequenceOutputs && !string.IsNullOrWhiteSpace(step.OutputLayer))
                {
                    availableLayers.Add(step.OutputLayer);
                }
            }

            return RunInputAvailability.Ok();
        }

        private static string BuildRunDisabledReason(VisionPipelineStep step, RunInputAvailability inputAvailability, string targetName)
        {
            if (step == null)
            {
                return $"{targetName}이 없습니다.";
            }

            if (!step.Enabled)
            {
                return $"{targetName}이 비활성 상태입니다.";
            }

            return inputAvailability.Success
                ? $"{targetName}을 실행할 수 없습니다."
                : inputAvailability.Message;
        }

        private static string BuildPipelineLog(string eventName, params string[] fields)
        {
            string log = $"Event={SanitizeLogValue(eventName)}";
            foreach (string field in fields ?? Enumerable.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(field))
                {
                    log += $", {field}";
                }
            }

            return log;
        }

        private static string LogField(string name, object value)
        {
            return $"{name}={SanitizeLogValue(value)}";
        }

        private static string SanitizeLogValue(object value)
        {
            string text = value?.ToString() ?? "-";
            if (string.IsNullOrWhiteSpace(text))
            {
                return "-";
            }

            return text.Replace(Environment.NewLine, " | ").Replace("\r", " ").Replace("\n", " ").Trim();
        }

        private bool CanResetActiveLayerView()
        {
            string layerTitle = string.IsNullOrWhiteSpace(displayManager.FocusItem)
                ? displayManager.SelectedItem
                : displayManager.FocusItem;
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                layerTitle = "Main";
            }

            return displayManager.FindIndex(layerTitle) >= 0 || displayManager.FindIndex("Main") >= 0;
        }

        private async Task RunPipelineFromMainAsync(bool selectedOnly)
        {
            if (isRunningMainPipeline)
            {
                RefreshToolbarStatus("Pipeline 실행 중");
                OVLog.Write(
                    LogCategory.Pipeline, LogLevel.Warning,
                    BuildPipelineLog(
                        "PipelineRunIgnored",
                        LogField("Reason", "AlreadyRunning"),
                        LogField("SelectedOnly", selectedOnly)));
                return;
            }

            currentPipeline = LoadCurrentPipelineForMain();
            BindPipelineStepList();

            VisionPipeline runPipeline = BuildMainRunPipeline(selectedOnly);
            RunInputAvailability inputAvailability = EvaluateRunInputAvailability(runPipeline.Steps, sequenceOutputs: !selectedOnly);
            if (!inputAvailability.Success)
            {
                RefreshToolbarStatus("Pipeline 검증 실패");
                SetInsightLabelText(lblStepMessage, $"메시지: {inputAvailability.Message}");
                OVLog.Write(
                    LogCategory.Pipeline, LogLevel.Warning,
                    BuildPipelineLog(
                        "PipelineRunBlocked",
                        LogField("Reason", inputAvailability.Message),
                        LogField("SelectedOnly", selectedOnly),
                        LogField("Steps", runPipeline.Steps.Count)));
                UpdateMainRunCommandAvailability();
                return;
            }

            if (runPipeline.Steps.Count == 0)
            {
                RefreshToolbarStatus("Pipeline Step 없음");
                OVLog.Write(
                    LogCategory.Pipeline, LogLevel.Warning,
                    BuildPipelineLog(
                        "PipelineRunBlocked",
                        LogField("Reason", "NoSteps"),
                        LogField("SelectedOnly", selectedOnly)));
                UpdateSelectedStepSummary();
                UpdateMainRunCommandAvailability();
                return;
            }

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(runPipeline, GetDisplayLayerTitles());
            if (!validation.Success)
            {
                string message = validation.FormatErrors();
                RefreshToolbarStatus("Pipeline 검증 실패");
                lblStepMessage.Text = $"메시지: {message}";
                OVLog.Write(
                    LogCategory.Pipeline, LogLevel.Error,
                    BuildPipelineLog(
                        "PipelineValidationFailed",
                        LogField("Reason", message),
                        LogField("SelectedOnly", selectedOnly),
                        LogField("Steps", runPipeline.Steps.Count)));
                UpdateMainRunCommandAvailability();
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            mainPipelineStopRequested = false;
            SetMainPipelineRunningState(true);
            mainPipelineStepSummaries.Clear();
            ClearPipelineLayerResultSummaries(runPipeline);
            lastStandaloneToolRunSummary = null;
            lastPipelineRunResult = null;
            mainPipelineCancellationSource = new CancellationTokenSource();
            RefreshToolbarStatus(selectedOnly ? "Step 실행 중" : "Pipeline 실행 중");
            OVLog.Write(
                LogCategory.Pipeline, LogLevel.Info,
                BuildPipelineLog(
                    "PipelineRunStarted",
                    LogField("Mode", selectedOnly ? "Step" : "Pipeline"),
                    LogField("Steps", runPipeline.Steps.Count),
                    LogField("Recipe", Global.Recipe?.Name)));

            Cursor previousCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                using (VisionPipelineContext context = CreatePipelineContextFromDisplayLayers())
                {
                    lastPipelineRunResult = await VisionPipelineExecutionService.RunAsync(
                        runPipeline,
                        context,
                        MainPipelineStepTimeoutMilliseconds,
                        mainPipelineCancellationSource.Token,
                        OnMainPipelineStepExecutionUpdated);
                }

                PublishMainPipelineResults(lastPipelineRunResult);
                stopwatch.Stop();
                displayManager.SetTackTime($"{stopwatch.Elapsed.TotalSeconds:0.000}s");

                bool canceled = mainPipelineCancellationSource?.IsCancellationRequested == true;
                string summary = canceled
                    ? $"Pipeline 중지 ({stopwatch.Elapsed.TotalMilliseconds:0.0} ms)"
                    : $"Pipeline {(lastPipelineRunResult?.Success == true ? "OK" : "NG")} ({stopwatch.Elapsed.TotalMilliseconds:0.0} ms)";
                RefreshToolbarStatus(summary);
                OVLog.Write(
                    LogCategory.Pipeline,
                    lastPipelineRunResult?.Success == true ? LogLevel.Info : LogLevel.Warning,
                    BuildPipelineLog(
                        "PipelineRunCompleted",
                        LogField("Status", canceled ? "CANCEL" : lastPipelineRunResult?.Success == true ? "OK" : "NG"),
                        LogField("TimeMs", stopwatch.Elapsed.TotalMilliseconds.ToString("0.0")),
                        LogField("Steps", lastPipelineRunResult?.StepResults?.Count ?? 0),
                        LogField("PublishedLayers", displayManager.LayerCount)));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                string message = ex.GetBaseException().Message;
                RefreshToolbarStatus("Pipeline 실행 오류");
                lblStepMessage.Text = $"메시지: {message}";
                OVLog.Write(
                    LogCategory.Pipeline, LogLevel.Error,
                    BuildPipelineLog(
                        "PipelineRunFailed",
                        LogField("TimeMs", stopwatch.Elapsed.TotalMilliseconds.ToString("0.0")),
                        LogField("Error", message)));
            }
            finally
            {
                Cursor.Current = previousCursor;
                mainPipelineCancellationSource?.Dispose();
                mainPipelineCancellationSource = null;
                mainPipelineStopRequested = false;
                SetMainPipelineRunningState(false);
                RefreshLayerResultPanel();
                InitLayListItem();
                UpdateSelectedStepSummary();
            }
        }

        private VisionPipeline BuildMainRunPipeline(bool selectedOnly)
        {
            VisionPipeline runPipeline = new VisionPipeline
            {
                Name = currentPipeline?.Name ?? VisionPipelineAppendService.DefaultPipelineName
            };

            if (currentPipeline == null)
            {
                return runPipeline;
            }

            if (selectedOnly)
            {
                VisionPipelineStep step = selectedPipelineStep
                    ?? currentPipeline.Steps.FirstOrDefault(item => item?.Enabled == true)
                    ?? currentPipeline.Steps.FirstOrDefault();
                if (step != null)
                {
                    selectedPipelineStep = step;
                    runPipeline.Steps.Add(step);
                }

                return runPipeline;
            }

            foreach (VisionPipelineStep step in currentPipeline.Steps.Where(step => step != null))
            {
                runPipeline.Steps.Add(step);
            }

            return runPipeline;
        }

        private VisionPipelineContext CreatePipelineContextFromDisplayLayers()
        {
            VisionPipelineContext context = new VisionPipelineContext();
            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                string title = displayManager.GetLayerTitle(i);
                Bitmap image = displayManager.GetLayerImage(i);
                if (string.IsNullOrWhiteSpace(title) || DisplayManagerImageExtensions.IsPlaceholderBitmap(image))
                {
                    continue;
                }

                using (OpenCvSharp.Mat mat = BitmapImageConverter.ToMat(image))
                {
                    context.SetLayer(title, mat);
                }
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

        private void OnMainPipelineStepExecutionUpdated(VisionPipelineStepExecutionUpdate update)
        {
            if (update == null)
            {
                return;
            }

            this.UIThreadBeginInvoke(() => ApplyMainPipelineStepExecutionUpdate(update));
        }

        private void ApplyMainPipelineStepExecutionUpdate(VisionPipelineStepExecutionUpdate update)
        {
            VisionPipelineStep step = update.Step;
            if (step != null)
            {
                selectedPipelineStep = step;
                SelectPipelineStepInList(step);
            }

            if (string.Equals(update.Status, "RUN", StringComparison.OrdinalIgnoreCase))
            {
                if (step != null)
                {
                    SetInsightLabelText(lblStepCardHeader, $"{GetPipelineStepDisplayIndex(step):00} Step 상세");
                    string stepName = string.IsNullOrWhiteSpace(step.Name) ? "Step" : step.Name;
                    string toolType = string.IsNullOrWhiteSpace(step.ToolType) ? "-" : step.ToolType;
                    SetInsightLabelText(lblStepTitle, $"{stepName} ({toolType})");
                }

                SetInsightLabelText(lblStepStatus, "상태: 실행 중");
                SetInsightLabelText(lblStepFlow, $"흐름: {FormatFlowText(step?.InputLayer, step?.OutputLayer)}");
                SetInsightLabelText(lblStepTime, "시간: 실행 중");
                SetInsightLabelText(lblStepMessage, $"메시지: {update.Message}");
                ApplySelectedStepVisualState("RUN", step?.Enabled != false);
                RefreshToolbarStatus($"{step?.Name ?? "Step"} 실행 중");
                OVLog.Write(
                    LogCategory.Pipeline,
                    LogLevel.Info,
                    BuildPipelineLog(
                        "PipelineStepStarted",
                        LogField("Step", step?.Name),
                        LogField("Tool", step?.ToolType),
                        LogField("Input", step?.InputLayer),
                        LogField("Output", step?.OutputLayer)));
                return;
            }

            if (update.StepResult != null && step != null)
            {
                VisionPipelineStepResultSummary summary = VisionPipelineResultSummaryService.CreateStepSummary(0, update.StepResult);
                mainPipelineStepSummaries[step] = summary;
                TrackLayerResultSummary(summary);
                OVLog.Write(
                    LogCategory.Pipeline,
                    summary.Success ? LogLevel.Info : LogLevel.Warning,
                    BuildPipelineLog(
                        "PipelineStepCompleted",
                        LogField("Status", summary.Status),
                        LogField("Step", summary.Name),
                        LogField("Tool", summary.ToolType),
                        LogField("Input", summary.InputLayer),
                        LogField("Output", summary.OutputLayer),
                        LogField("TimeMs", summary.ElapsedMilliseconds.ToString("0.0")),
                        LogField("Message", summary.Message)));
                lstPipelineSteps?.Invalidate();
            }
            else
            {
                OVLog.Write(
                    LogCategory.Pipeline,
                    LogLevel.Info,
                    BuildPipelineLog(
                        "PipelineUpdate",
                        LogField("Status", update.Status),
                        LogField("Message", update.Message)));
            }

            UpdateSelectedStepSummary();
            RefreshToolbarStatus($"{step?.Name ?? "Pipeline"} {update.Status}");
        }

        private void SelectPipelineStepInList(VisionPipelineStep step)
        {
            if (lstPipelineSteps == null || step == null)
            {
                return;
            }

            for (int i = 0; i < lstPipelineSteps.Items.Count; i++)
            {
                if (lstPipelineSteps.Items[i] is PipelineStepListItem item && ReferenceEquals(item.Step, step))
                {
                    lstPipelineSteps.SelectedIndex = i;
                    return;
                }
            }
        }

        private void PublishMainPipelineResults(VisionPipelineRunResult result)
        {
            foreach (VisionPipelineStepResult stepResult in result?.StepResults ?? Enumerable.Empty<VisionPipelineStepResult>())
            {
                if (!VisionPipelineResultSummaryService.IsPassed(stepResult)
                    || stepResult.ToolResult?.ResultImage == null
                    || stepResult.ToolResult.ResultImage.Empty())
                {
                    continue;
                }

                PublishMainPipelineStepResult(stepResult);
            }
        }

        private void PublishMainPipelineStepResult(VisionPipelineStepResult stepResult)
        {
            string layerName = string.IsNullOrWhiteSpace(stepResult?.Step?.OutputLayer)
                ? "Pipeline_Output"
                : stepResult.Step.OutputLayer;
            VisionPipelineStepResultSummary summary = VisionPipelineResultSummaryService.CreateStepSummary(0, stepResult);
            if (summary != null && string.IsNullOrWhiteSpace(summary.OutputLayer))
            {
                summary.OutputLayer = layerName;
            }

            TrackLayerResultSummary(summary);

            using (Bitmap bitmap = BitmapImageConverter.ToBitmap(stepResult.ToolResult.ResultImage))
            {
                int index = displayManager.FindIndex(layerName);
                if (index >= 0)
                {
                    displayManager.SetLayerImage(index, bitmap);
                    displayManager.RefreshLayer(index);
                }
                else
                {
                    displayManager.CreateLayerDisplay(new Bitmap(bitmap), layerName, true);
                }
            }

            displayManager.ActivateLayer(layerName);
            SelectLayerComboItem(layerName);
            OVLog.Write(
                LogCategory.Pipeline, LogLevel.Info,
                BuildPipelineLog(
                    "PipelineResultPublished",
                    LogField("Layer", layerName),
                    LogField("Step", stepResult?.Step?.Name),
                    LogField("Tool", stepResult?.Step?.ToolType)));
        }

        private void SetMainPipelineRunningState(bool running)
        {
            isRunningMainPipeline = running;
            if (lstPipelineSteps != null) { lstPipelineSteps.Enabled = !running; }
            UpdateMainRunCommandAvailability();
        }

        private void StopMainPipelineRun()
        {
            if (mainPipelineCancellationSource == null)
            {
                RefreshToolbarStatus("중지할 실행 없음");
                return;
            }

            mainPipelineCancellationSource.Cancel();
            mainPipelineStopRequested = true;
            UpdateMainRunCommandAvailability();
            RefreshToolbarStatus("Pipeline 중지 요청");
            OVLog.Write(
                LogCategory.Pipeline,
                LogLevel.Warning,
                BuildPipelineLog(
                    "PipelineStopRequested",
                    LogField("Reason", "UserRequest")));
        }

        private void ResetActiveLayerView()
        {
            string layerTitle = string.IsNullOrWhiteSpace(displayManager.FocusItem)
                ? displayManager.SelectedItem
                : displayManager.FocusItem;
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                layerTitle = "Main";
            }

            if (displayManager.FindIndex(layerTitle) < 0)
            {
                layerTitle = "Main";
            }

            displayManager.ZoomLayerToFit(layerTitle);
            RefreshToolbarStatus($"{layerTitle} 화면 맞춤");
            OVLog.Write(LogCategory.Main, LogLevel.Info, $"View reset. Layer={layerTitle}");
        }

        private void ApplyLogQuickFilter(string type, string level, string searchText, bool showEntireStream, string label)
        {
            activeLogFilterLabel = string.IsNullOrWhiteSpace(label) ? "All" : label;
            UpdateLogFilterButtonStates();
            RefreshInsightHeaders();
            LogPanelViewModel.ApplyQuickFilter(type, level, searchText, showEntireStream);
            RefreshToolbarStatus($"로그 필터: {label}");
            OVLog.Write(LogCategory.Main, LogLevel.Info, $"Log quick filter applied. Filter={label}");
        }

        private string GetToolbarFlowText(string sourceLayer)
        {
            VisionPipelineStep step = selectedPipelineStep;
            if (step != null
                && (!string.IsNullOrWhiteSpace(step.InputLayer) || !string.IsNullOrWhiteSpace(step.OutputLayer)))
            {
                return mainPipelineStepSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary)
                    ? FormatSummaryFlowText(step, summary)
                    : FormatFlowText(step.InputLayer, step.OutputLayer);
            }

            if (lastStandaloneToolRunSummary != null)
            {
                return FormatSummaryFlowText(null, lastStandaloneToolRunSummary);
            }

            VisionPipelineStepResult lastResult = lastPipelineRunResult?.StepResults
                .LastOrDefault(result => result?.Step != null);
            if (lastResult?.Step != null)
            {
                return FormatFlowText(lastResult.Step.InputLayer, lastResult.Step.OutputLayer);
            }

            return FormatFlowText(sourceLayer, null);
        }

        private static string FormatLayerName(string layerName)
        {
            return string.IsNullOrWhiteSpace(layerName) ? "-" : layerName;
        }

        private static string FormatFlowText(string inputLayer, string outputLayer)
        {
            string input = FormatLayerName(inputLayer);
            string output = string.IsNullOrWhiteSpace(outputLayer) ? "출력 없음" : outputLayer;
            return $"{input} -> {output}";
        }

        private static string FormatSummaryFlowText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            string inputLayer = string.IsNullOrWhiteSpace(summary?.InputLayer) ? step?.InputLayer : summary.InputLayer;
            string outputLayer = string.IsNullOrWhiteSpace(summary?.OutputLayer) ? step?.OutputLayer : summary.OutputLayer;
            return FormatFlowText(inputLayer, outputLayer);
        }

        private string FormatSourceModeText(string selectedLayer)
        {
            if (!chkUseLayerImage.Check)
            {
                return "Main 원본";
            }

            return string.Equals(selectedLayer, "Main", StringComparison.OrdinalIgnoreCase)
                ? "Main 레이어"
                : $"선택: {selectedLayer}";
        }

        private static string FormatToolbarLayerStatusText(string focusLayer, int layerCount)
        {
            string layer = string.IsNullOrWhiteSpace(focusLayer) ? "Main" : focusLayer;
            return $"{layer} · {layerCount}개";
        }

        private static string FormatStatusText(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return "대기 중";
            }

            string trimmed = status.Trim();
            switch (trimmed)
            {
                case "Ready":
                    return "대기 중";
                case "Recipe changed":
                    return "Recipe 변경됨";
                case "Result updated":
                    return "결과 갱신됨";
                case "Pipeline Step 없음":
                    return "실행할 검사 Step 없음";
                case "Pipeline 검증 실패":
                    return "파이프라인 설정 확인 필요";
                case "Pipeline 실행 오류":
                    return "파이프라인 실행 오류";
                case "Pipeline 실행 중":
                    return "파이프라인 실행 중";
                case "Step 실행 중":
                    return "선택 Step 실행 중";
                case "RUN":
                    return "실행 중";
                case "OK":
                    return "정상";
                case "NG":
                    return "NG";
                case "SKIP":
                    return "건너뜀";
                case "CANCEL":
                    return "중지됨";
            }

            if (trimmed.StartsWith("Pipeline OK", StringComparison.OrdinalIgnoreCase))
            {
                return "파이프라인 정상" + trimmed.Substring("Pipeline OK".Length);
            }

            if (trimmed.StartsWith("Pipeline NG", StringComparison.OrdinalIgnoreCase))
            {
                return "파이프라인 NG" + trimmed.Substring("Pipeline NG".Length);
            }

            if (trimmed.StartsWith("Pipeline 중지", StringComparison.OrdinalIgnoreCase))
            {
                return "파이프라인" + trimmed.Substring("Pipeline".Length);
            }

            if (trimmed.EndsWith(" RUN", StringComparison.OrdinalIgnoreCase))
            {
                return $"{trimmed.Substring(0, trimmed.Length - 4)} 실행 중";
            }

            if (trimmed.EndsWith(" OK", StringComparison.OrdinalIgnoreCase))
            {
                return $"{trimmed.Substring(0, trimmed.Length - 3)} 정상";
            }

            if (trimmed.EndsWith(" NG", StringComparison.OrdinalIgnoreCase))
            {
                return $"{trimmed.Substring(0, trimmed.Length - 3)} NG";
            }

            return trimmed;
        }

        private static Color GetStatusTextColor(string status)
        {
            string value = status ?? string.Empty;
            if (value.IndexOf("오류", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("실패", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("NG", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("확인 필요", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(255, 180, 120);
            }

            if (value.IndexOf("OK", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("정상", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("완료", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(161, 232, 176);
            }

            if (value.IndexOf("실행 중", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("중지", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.FromArgb(125, 214, 231);
            }

            return Color.FromArgb(250, 252, 255);
        }

        private static ToolbarTileMood GetStatusTileMood(string status)
        {
            string value = status ?? string.Empty;
            if (value.IndexOf("오류", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("실패", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("NG", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("확인 필요", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ToolbarTileMood.Error;
            }

            if (value.IndexOf("중지", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ToolbarTileMood.Warning;
            }

            if (value.IndexOf("실행 중", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ToolbarTileMood.Running;
            }

            if (value.IndexOf("OK", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("정상", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("완료", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("갱신", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ToolbarTileMood.Success;
            }

            if (value.IndexOf("대기", StringComparison.OrdinalIgnoreCase) >= 0
                || value == "-")
            {
                return ToolbarTileMood.Muted;
            }

            return ToolbarTileMood.Active;
        }

        private static ToolbarTileMood GetFlowTileMood(string flowText)
        {
            if (string.IsNullOrWhiteSpace(flowText) || flowText.IndexOf("출력 없음", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ToolbarTileMood.Muted;
            }

            return flowText.IndexOf("->", StringComparison.OrdinalIgnoreCase) >= 0
                ? ToolbarTileMood.Active
                : ToolbarTileMood.Neutral;
        }

        private static ToolbarTileMood GetSourceTileMood(string sourceMode)
        {
            if (string.IsNullOrWhiteSpace(sourceMode))
            {
                return ToolbarTileMood.Muted;
            }

            return sourceMode.IndexOf("선택:", StringComparison.OrdinalIgnoreCase) >= 0
                ? ToolbarTileMood.Active
                : ToolbarTileMood.Neutral;
        }

        private static ToolbarTileMood GetActiveLayerTileMood(string focusLayer)
        {
            return string.Equals(focusLayer, "Main", StringComparison.OrdinalIgnoreCase)
                ? ToolbarTileMood.Neutral
                : ToolbarTileMood.Active;
        }

        private static ToolbarTileMood GetTackTimeTileMood(string tackTime)
        {
            return string.IsNullOrWhiteSpace(tackTime) || tackTime == "-"
                ? ToolbarTileMood.Muted
                : ToolbarTileMood.Success;
        }

        private static void ApplyToolbarStatusTileStyle(Panel tile, ToolbarTileMood mood)
        {
            if (!(tile?.Tag is ToolbarStatusTileParts parts))
            {
                return;
            }

            Color accent;
            Color back;
            Color titleFore;
            Color valueFore;
            switch (mood)
            {
                case ToolbarTileMood.Success:
                    accent = Color.FromArgb(83, 207, 137);
                    back = Color.FromArgb(35, 64, 62);
                    titleFore = Color.FromArgb(174, 225, 203);
                    valueFore = Color.FromArgb(224, 255, 236);
                    break;
                case ToolbarTileMood.Running:
                    accent = Color.FromArgb(100, 214, 239);
                    back = Color.FromArgb(34, 64, 82);
                    titleFore = Color.FromArgb(175, 225, 243);
                    valueFore = Color.FromArgb(229, 251, 255);
                    break;
                case ToolbarTileMood.Warning:
                    accent = Color.FromArgb(235, 177, 84);
                    back = Color.FromArgb(72, 58, 45);
                    titleFore = Color.FromArgb(238, 213, 166);
                    valueFore = Color.FromArgb(255, 239, 205);
                    break;
                case ToolbarTileMood.Error:
                    accent = Color.FromArgb(234, 112, 112);
                    back = Color.FromArgb(75, 47, 58);
                    titleFore = Color.FromArgb(242, 190, 190);
                    valueFore = Color.FromArgb(255, 230, 230);
                    break;
                case ToolbarTileMood.Active:
                    accent = Color.FromArgb(127, 170, 246);
                    back = Color.FromArgb(42, 56, 88);
                    titleFore = Color.FromArgb(184, 208, 247);
                    valueFore = Color.FromArgb(248, 251, 255);
                    break;
                case ToolbarTileMood.Muted:
                    accent = Color.FromArgb(113, 124, 146);
                    back = Color.FromArgb(40, 48, 70);
                    titleFore = Color.FromArgb(154, 175, 206);
                    valueFore = Color.FromArgb(218, 227, 241);
                    break;
                default:
                    accent = Color.FromArgb(115, 165, 235);
                    back = Color.FromArgb(41, 50, 78);
                    titleFore = Color.FromArgb(170, 197, 234);
                    valueFore = Color.FromArgb(250, 252, 255);
                    break;
            }

            tile.BackColor = back;
            parts.AccentPanel.BackColor = accent;
            parts.TitleLabel.ForeColor = titleFore;
            parts.ValueLabel.ForeColor = valueFore;
        }

        private void RefreshToolbarStatus(string runSummary = null)
        {
            string selectedLayer = string.IsNullOrWhiteSpace(displayManager.SelectedItem)
                ? "Main"
                : displayManager.SelectedItem;
            string focusLayer = string.IsNullOrWhiteSpace(displayManager.FocusItem)
                ? selectedLayer
                : displayManager.FocusItem;
            string sourceMode = FormatSourceModeText(selectedLayer);
            string flowText = GetToolbarFlowText(chkUseLayerImage.Check ? selectedLayer : "Main");
            string tackTime = string.IsNullOrWhiteSpace(displayManager.TackTime) ? "-" : displayManager.TackTime;
            if (!string.IsNullOrWhiteSpace(runSummary))
            {
                lastRunSummary = FormatStatusText(runSummary);
            }

            string statusKey = $"{selectedLayer}|{focusLayer}|{sourceMode}|{flowText}|{displayManager.LayerCount}|{tackTime}|{lastRunSummary}";
            if (statusKey == lastToolbarStatus)
            {
                return;
            }

            lastToolbarStatus = statusKey;
            if (lblToolbarActiveLayer != null)
            {
                lblToolbarActiveLayer.Text = FormatToolbarLayerStatusText(focusLayer, displayManager.LayerCount);
                ApplyToolbarStatusTileStyle(pnlToolbarActiveLayer, GetActiveLayerTileMood(focusLayer));
                SetMainToolTip(lblToolbarActiveLayer, $"활성 레이어: {focusLayer}\r\n선택 레이어: {selectedLayer}\r\n전체 레이어: {displayManager.LayerCount}개");
                SetMainToolTip(pnlToolbarActiveLayer, $"활성 레이어: {focusLayer}\r\n선택 레이어: {selectedLayer}\r\n전체 레이어: {displayManager.LayerCount}개");
            }

            if (lblToolbarSourceMode != null)
            {
                lblToolbarSourceMode.Text = sourceMode;
                ApplyToolbarStatusTileStyle(pnlToolbarSourceMode, GetSourceTileMood(sourceMode));
                SetMainToolTip(lblToolbarSourceMode, $"입력 기준: {sourceMode}");
                SetMainToolTip(pnlToolbarSourceMode, $"입력 기준: {sourceMode}");
            }

            if (lblToolbarFlow != null)
            {
                lblToolbarFlow.Text = flowText;
                ApplyToolbarStatusTileStyle(pnlToolbarFlow, GetFlowTileMood(flowText));
                SetMainToolTip(lblToolbarFlow, $"실행 흐름: {flowText}");
                SetMainToolTip(pnlToolbarFlow, $"실행 흐름: {flowText}");
            }

            if (lblToolbarRunSummary != null)
            {
                lblToolbarRunSummary.Text = lastRunSummary;
                lblToolbarRunSummary.ForeColor = GetStatusTextColor(lastRunSummary);
                ApplyToolbarStatusTileStyle(pnlToolbarRunSummary, GetStatusTileMood(lastRunSummary));
                SetMainToolTip(lblToolbarRunSummary, $"작업 상태: {lastRunSummary}");
                SetMainToolTip(pnlToolbarRunSummary, $"작업 상태: {lastRunSummary}");
            }

            if (lblToolbarTackTime != null)
            {
                lblToolbarTackTime.Text = tackTime;
                ApplyToolbarStatusTileStyle(pnlToolbarTackTime, GetTackTimeTileMood(tackTime));
                SetMainToolTip(lblToolbarTackTime, $"소요 시간: {tackTime}");
                SetMainToolTip(pnlToolbarTackTime, $"소요 시간: {tackTime}");
            }
        }

        private void OnDisplayContextChanged(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() => RefreshToolbarStatus());
        }

        private void OnVisionToolRunUpdated(object sender, VisionToolRunEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            this.UIThreadBeginInvoke(() => ApplyVisionToolRunUpdate(e));
        }

        private void ApplyVisionToolRunUpdate(VisionToolRunEventArgs e)
        {
            VisionPipelineStep step = FindMatchingPipelineStep(e);
            VisionPipelineStepResultSummary summary = CreateToolRunSummary(step, e);
            TrackLayerResultSummary(summary);

            if (step == null)
            {
                lastStandaloneToolRunSummary = summary;
                selectedPipelineStep = null;
                lstPipelineSteps?.ClearSelected();
                UpdateSelectedStepSummary();
                RefreshLayerResultPanel();
                if (e.Status == VisionToolRunStatus.Completed && !string.IsNullOrWhiteSpace(e.OutputLayer))
                {
                    SelectLayerListItem(e.OutputLayer);
                }

                RefreshToolbarStatus(BuildToolRunStatusText(e));
                return;
            }

            lastStandaloneToolRunSummary = null;
            selectedPipelineStep = step;
            mainPipelineStepSummaries[step] = summary;
            SelectPipelineStepInList(step);
            lstPipelineSteps?.Invalidate();
            UpdateSelectedStepSummary();
            RefreshLayerResultPanel();

            if (e.Status == VisionToolRunStatus.Completed && !string.IsNullOrWhiteSpace(e.OutputLayer))
            {
                SelectLayerListItem(e.OutputLayer);
            }

            RefreshToolbarStatus(BuildToolRunStatusText(e));
        }

        private VisionPipelineStep FindMatchingPipelineStep(VisionToolRunEventArgs e)
        {
            if (currentPipeline == null || currentPipeline.Steps.Count == 0)
            {
                currentPipeline = LoadCurrentPipelineForMain();
                BindPipelineStepList();
            }

            if (StepMatchesToolRun(selectedPipelineStep, e))
            {
                return selectedPipelineStep;
            }

            return currentPipeline?.Steps.FirstOrDefault(step => StepMatchesToolRun(step, e));
        }

        private static bool StepMatchesToolRun(VisionPipelineStep step, VisionToolRunEventArgs e)
        {
            if (step == null || e == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(e.OutputLayer)
                && string.Equals(step.OutputLayer, e.OutputLayer, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string toolToken = NormalizeVisionToolToken(e.ToolName);
            if (string.IsNullOrWhiteSpace(toolToken))
            {
                return false;
            }

            string stepToolToken = NormalizeVisionToolToken(step.ToolType);
            string stepNameToken = NormalizeVisionToolToken(step.Name);
            return ToolTokensMatch(toolToken, stepToolToken)
                || ToolTokensMatch(toolToken, stepNameToken)
                || (toolToken.StartsWith("line", StringComparison.OrdinalIgnoreCase)
                    && (stepToolToken.Contains("line") || stepNameToken.Contains("line")));
        }

        private static bool ToolTokensMatch(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
            {
                return false;
            }

            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase)
                || left.IndexOf(right, StringComparison.OrdinalIgnoreCase) >= 0
                || right.IndexOf(left, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string NormalizeVisionToolToken(string value)
        {
            string token = (value ?? string.Empty).Trim();
            if (token.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring(0, token.Length - 4);
            }

            return token
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Replace("-", string.Empty)
                .ToLowerInvariant();
        }

        private VisionPipelineStepResultSummary CreateToolRunSummary(VisionPipelineStep step, VisionToolRunEventArgs e)
        {
            string status = e.Status == VisionToolRunStatus.Started
                ? "RUN"
                : e.Status == VisionToolRunStatus.Completed ? "OK" : "NG";
            int stepIndex = step == null ? -1 : currentPipeline?.Steps.IndexOf(step) ?? -1;
            string outputLayer = string.IsNullOrWhiteSpace(e.OutputLayer) ? step?.OutputLayer : e.OutputLayer;
            string inputLayer = string.IsNullOrWhiteSpace(e.SourceLayer) ? step?.InputLayer : e.SourceLayer;
            string message = e.Message ?? string.Empty;
            if (e.Status == VisionToolRunStatus.Completed
                && e.ResultWidth > 0
                && e.ResultHeight > 0
                && !message.Contains($"{e.ResultWidth}x{e.ResultHeight}"))
            {
                message = $"{message} ({e.ResultWidth}x{e.ResultHeight})".Trim();
            }

            return new VisionPipelineStepResultSummary
            {
                Index = stepIndex >= 0 ? stepIndex + 1 : 0,
                Name = step?.Name ?? e.ToolName ?? string.Empty,
                ToolType = step?.ToolType ?? e.ToolName ?? string.Empty,
                InputLayer = inputLayer ?? string.Empty,
                OutputLayer = outputLayer ?? string.Empty,
                Status = status,
                Success = e.Status == VisionToolRunStatus.Completed,
                Skipped = false,
                HasResultImage = e.ResultWidth > 0 && e.ResultHeight > 0,
                ResultImageWidth = e.ResultWidth,
                ResultImageHeight = e.ResultHeight,
                OverlayCount = 0,
                MetricCount = 0,
                ParameterCount = step?.Parameters?.Count ?? 0,
                ElapsedMilliseconds = e.ElapsedMilliseconds,
                Message = message
            };
        }

        private static string BuildToolRunStatusText(VisionToolRunEventArgs e)
        {
            string toolName = string.IsNullOrWhiteSpace(e?.ToolName) ? "검사" : e.ToolName;
            switch (e?.Status)
            {
                case VisionToolRunStatus.Started:
                    return $"{toolName} RUN";
                case VisionToolRunStatus.Completed:
                    return $"{toolName} OK";
                case VisionToolRunStatus.Failed:
                    return $"{toolName} NG";
                default:
                    return $"{toolName} 갱신";
            }
        }

        private int GetLeftDockWidth()
        {
            return Math.Max(360, Math.Min(560, ClientSize.Width / 4));
        }

        private void ShowVisionForms()
        {
            WeifenLuo.WinFormsUI.Docking.DockContent fr;
            foreach (var form in Forms)
            {
                fr = (form.Value as WeifenLuo.WinFormsUI.Docking.DockContent);
                //DockContent system = (Forms[VISION_DOCK_FORM.System] as DockContent);
                switch (form.Key)
                {
                    //case VISION_DOCK_FORM.System:
                    //    fr.Show(this.dockPanel, DockState.DockLeft);
                    //    fr.AutoHidePortion = GetLeftDockWidth();
                    //    break;
                    //case VISION_DOCK_FORM.BLOB:
                    //    fr.Show(system.PanelPane, null);
                    //    break;
                    //case VISION_DOCK_FORM.LINE:
                    //    fr.Show(system.PanelPane, null);
                    //    break;
                    //case VISION_DOCK_FORM.TEACHING:
                    //    fr.Show(system.PanelPane, null);
                    //    break;
                    case VISION_DOCK_FORM.CONTOUR:
                        break;
                    //case VISION_DOCK_FORM.PROPERTY:
                    //    fr.Show(system.PanelPane, DockAlignment.Bottom, 0.47);
                    //    fr.AutoHidePortion = GetLeftDockWidth();
                    //    break;
                    case VISION_DOCK_FORM.THRESHOLD:
                        fr.Show(this.dockPanel, DockState.DockLeftAutoHide);
                        fr.AutoHidePortion = 500;
                        break;
                    case VISION_DOCK_FORM.LOG:
                        fr.Show(this.dockPanel, DockState.DockBottom);
                        break;
                }
            }
            //fr = (Forms[VISION_DOCK_FORM.System] as WeifenLuo.WinFormsUI.Docking.DockContent);
            //fr.Activate();
            //fr = (Forms[VISION_DOCK_FORM.PROPERTY] as WeifenLuo.WinFormsUI.Docking.DockContent);
            //fr.Activate();
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                RefreshPipelineWorkspace();
                RefreshToolbarStatus("Recipe changed");
            });
        }

        public void ShowDockedLogViewer()
        {
            ShowLogDockContent();
        }

        private void OnUpdateResult(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                lbTackTime.Text = displayManager.TackTime;
                RefreshLayerResultPanel();
                RefreshToolbarStatus("Result updated");
                OVLog.Write(LogCategory.Main, LogLevel.Info, $"Result updated. Time={displayManager.TackTime}");
            });          
        }

        private void OnUpdateDisplay(object sender, DockDisplayEventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                displayManager.SetLayerImage(e.Index, e.Image);
                displayManager.RefreshLayer(e.Index);
                displayManager.ActivateLayer(e.Index);
                displayManager.TackTime = e.TackTime;
                lbTackTime.Text = e.TackTime;
                string layerTitle = displayManager.GetLayerTitle(e.Index);
                SelectLayerComboItem(layerTitle);
                RefreshLayerResultPanel();
                RefreshToolbarStatus($"{layerTitle} 갱신");
                OVLog.Write(LogCategory.Main, LogLevel.Info, $"Display layer updated. Layer={layerTitle}, Index={e.Index}, Time={e.TackTime}");
            });
        }             
        
        private void ShowForm(Form form)
        {
            Cursor previousCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                form.TopLevel = true;
                form.TopMost = false;
                form.StartPosition = FormStartPosition.CenterParent;
                if (!AppUtil.OpenCheckForm(form)) return;

                IWin32Window owner = TopLevelControl as IWin32Window ?? this;
                form.Show(owner);
                form.Activate();
            }
            finally
            {
                Cursor.Current = previousCursor;
            }
        }

        private void SelectLayerComboItem(string layerTitle)
        {
            if (string.IsNullOrWhiteSpace(layerTitle) || cbLayerList.Items.Count == 0) { return; }
            if (!cbLayerList.Items.Contains(layerTitle)) { return; }

            cbLayerList.OnSelectedIndexChanged -= cbLayerList_SelectedIndexChanged;
            cbLayerList.SelectedIndex = cbLayerList.Items.IndexOf(layerTitle);
            cbLayerList.OnSelectedIndexChanged += cbLayerList_SelectedIndexChanged;
            displayManager.SelectedItem = layerTitle;
        }

        private void OnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menuItem)) return;
            if (menuItem.HasDropDownItems) return;

            VISION_MENU menu = menuItem.Tag is VISION_MENU taggedMenu
                ? taggedMenu
                : AppUtil.ParseEnum<VISION_MENU>(menuItem.Text);
            string displayName = GetVisionMenuDisplayName(menu);
            OVLog.Write(LogCategory.Main, LogLevel.Info, $"Tool window requested. Tool={menu}, Display={displayName}");
            RefreshToolbarStatus($"{displayName} 열기");
            switch (menu)
            {
                case VISION_MENU.Morphology:
                    FormVision_Morphology morphologyForm = new FormVision_Morphology(displayManager, EventUpdateDisplay);
                    morphologyForm.SetDisplayManager(displayManager);
                    this.ShowForm(morphologyForm);                    
                    break;
                case VISION_MENU.Filter:
                    FormVision_Filter filterForm = new FormVision_Filter(displayManager, EventUpdateDisplay);
                    filterForm.SetDisplayManager(displayManager);
                    this.ShowForm(filterForm);
                    break;
                case VISION_MENU.Arithmetic:
                    FormVision_Arithmetic arithmeticForm = new FormVision_Arithmetic(displayManager, EventUpdateDisplay);
                    arithmeticForm.SetDisplayManager(displayManager);
                    this.ShowForm(arithmeticForm);
                    break;
                case VISION_MENU.Blob:
                    FormVision_Blob blobForm = new FormVision_Blob(displayManager, EventUpdateDisplay);
                    blobForm.SetDisplayManager(displayManager);
                    this.ShowForm(blobForm);
                    break;
                case VISION_MENU.Contour:
                    FormVision_Contour contourForm = new FormVision_Contour(displayManager, EventUpdateDisplay);
                    contourForm.SetDisplayManager(displayManager);
                    this.ShowForm(contourForm);                   
                    break;
                case VISION_MENU.Matching:
                    FormVision_Matching matchingForm = new FormVision_Matching(displayManager, EventUpdateDisplay);
                    matchingForm.SetDisplayManager(displayManager);
                    this.ShowForm(matchingForm);                    
                    break;
                case VISION_MENU.FeatureMatching:
                    FormVision_FeatureMatching featureMatchingForm = new FormVision_FeatureMatching(displayManager, EventUpdateDisplay);
                    featureMatchingForm.SetDisplayManager(displayManager);
                    this.ShowForm(featureMatchingForm);
                    break;
                case VISION_MENU.Line:
                    FormVision_Line lineForm = new FormVision_Line(displayManager, EventUpdateDisplay);
                    lineForm.SetDisplayManager(displayManager);
                    this.ShowForm(lineForm);
                    break;
                case VISION_MENU.EdgeDetection:
                    FormVision_EdgeDetection edgeDetectionForm = new FormVision_EdgeDetection(displayManager, EventUpdateDisplay);
                    edgeDetectionForm.SetDisplayManager(displayManager);
                    this.ShowForm(edgeDetectionForm);                    
                    break;
                case VISION_MENU.RotateAndScale:
                    FormVision_RotateAndScale rotateAndScaleForm = new FormVision_RotateAndScale(displayManager, EventUpdateDisplay);
                    rotateAndScaleForm.SetDisplayManager(displayManager);
                    this.ShowForm(rotateAndScaleForm);                    
                    break;
                case VISION_MENU.Histogram:
                    FormVision_Histogram histogramForm = new FormVision_Histogram(displayManager, EventUpdateDisplay);
                    histogramForm.SetDisplayManager(displayManager);
                    this.ShowForm(histogramForm);                    
                    break;
                case VISION_MENU.Mean:
                    FormVision_Mean meanForm = new FormVision_Mean(displayManager, EventUpdateDisplay);
                    meanForm.SetDisplayManager(displayManager);
                    this.ShowForm(meanForm);                    
                    break;
                case VISION_MENU.HSV:
                    FormVision_HSV hsvForm = new FormVision_HSV(displayManager, EventUpdateDisplay);
                    hsvForm.SetDisplayManager(displayManager);
                    this.ShowForm(hsvForm);                    
                    break;
                case VISION_MENU.Pipeline:
                    FormVision_Pipeline pipelineForm = new FormVision_Pipeline(displayManager, Global.Recipe.Name);
                    pipelineForm.FormClosed += (closedSender, closedArgs) => RefreshPipelineWorkspace();
                    this.ShowForm(pipelineForm);
                    RefreshPipelineWorkspace();
                    break;
            }
        }

        private sealed class MainRunCommandAvailability
        {
            public MainRunCommandAvailability(
                bool canRunSelected,
                bool canRunAll,
                bool canStop,
                bool canResetView,
                string selectedReason,
                string allReason,
                string stopReason,
                string resetReason)
            {
                CanRunSelected = canRunSelected;
                CanRunAll = canRunAll;
                CanStop = canStop;
                CanResetView = canResetView;
                SelectedReason = selectedReason ?? string.Empty;
                AllReason = allReason ?? string.Empty;
                StopReason = stopReason ?? string.Empty;
                ResetReason = resetReason ?? string.Empty;
            }

            public bool CanRunSelected { get; }
            public bool CanRunAll { get; }
            public bool CanStop { get; }
            public bool CanResetView { get; }
            public string SelectedReason { get; }
            public string AllReason { get; }
            public string StopReason { get; }
            public string ResetReason { get; }
        }

        private sealed class RunInputAvailability
        {
            private RunInputAvailability(bool success, string message)
            {
                Success = success;
                Message = message ?? string.Empty;
            }

            public bool Success { get; }
            public string Message { get; }

            public static RunInputAvailability Ok()
            {
                return new RunInputAvailability(true, string.Empty);
            }

            public static RunInputAvailability Fail(string message)
            {
                return new RunInputAvailability(false, string.IsNullOrWhiteSpace(message) ? "실행 입력을 확인해야 합니다." : message);
            }
        }

        private sealed class PipelineStepVisualState
        {
            public PipelineStepVisualState(string badge, string statusText, string detailText, string message, bool enabled, Color accentColor, bool warning = false)
            {
                Badge = string.IsNullOrWhiteSpace(badge) ? "-" : badge;
                StatusText = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText;
                DetailText = string.IsNullOrWhiteSpace(detailText) ? StatusText : detailText;
                Message = string.IsNullOrWhiteSpace(message) ? StatusText : message;
                Enabled = enabled;
                AccentColor = accentColor;
                Warning = warning;
            }

            public string Badge { get; }
            public string StatusText { get; }
            public string DetailText { get; }
            public string Message { get; }
            public bool Enabled { get; }
            public Color AccentColor { get; }
            public bool Warning { get; }
        }

        private sealed class PipelineStepListItem
        {
            public PipelineStepListItem(int index, VisionPipelineStep step)
            {
                Index = index;
                Step = step;
            }

            public int Index { get; }
            public VisionPipelineStep Step { get; }

            public override string ToString()
            {
                string enabled = Step?.Enabled == true ? " " : "-";
                string name = string.IsNullOrWhiteSpace(Step?.Name) ? "Step" : Step.Name;
                string type = string.IsNullOrWhiteSpace(Step?.ToolType) ? "-" : Step.ToolType;
                return $"{Index:00}{enabled} {name} [{type}]";
            }
        }

        private sealed class LayerResultVisualState
        {
            public LayerResultVisualState(string badge, string detailText, string sizeText, Color accentColor)
            {
                Badge = string.IsNullOrWhiteSpace(badge) ? "LAYER" : badge;
                DetailText = string.IsNullOrWhiteSpace(detailText) ? "레이어" : detailText;
                SizeText = string.IsNullOrWhiteSpace(sizeText) ? "-" : sizeText;
                AccentColor = accentColor;
            }

            public string Badge { get; }
            public string DetailText { get; }
            public string SizeText { get; }
            public Color AccentColor { get; }
        }

        private sealed class LayerResultListItem
        {
            public LayerResultListItem(int index, string title, string sizeText, string detailText, string badge, Color accentColor)
            {
                Index = index;
                Title = title ?? string.Empty;
                SizeText = sizeText ?? "-";
                DetailText = detailText ?? "레이어";
                Badge = string.IsNullOrWhiteSpace(badge) ? "LAYER" : badge;
                AccentColor = accentColor;
            }

            public int Index { get; }
            public string Title { get; }
            public string SizeText { get; }
            public string DetailText { get; }
            public string Badge { get; }
            public Color AccentColor { get; }

            public override string ToString()
            {
                return $"{Index:00} {Title}  {FormatLayerResultBadgeText(Badge)}  {DetailText}  {SizeText}";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (PanelCount != displayManager.LayerCount)
            {
                PanelCount = displayManager.LayerCount;
                InitLayListItem();
            }

            if (displayManager.IsLayerImageChanged(displayManager.SelectedItem))
            {
                displayManager.SetImageSrc(Lib.Common.BitmapImageConverter.ToMat(displayManager.GetLayerImage(displayManager.SelectedItem)));
                displayManager.AcceptLayerImageChanged(displayManager.SelectedItem);
            }

            RefreshToolbarStatus();
        
        }
    }   
}






