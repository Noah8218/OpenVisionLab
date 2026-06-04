using OpenVisionLab._1._Core;
using Lib.Common;
using Lib.OpenCV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static OpenVisionLab.DEFINE;
using Cursors = System.Windows.Forms.Cursors;

namespace OpenVisionLab
{
    public partial class FormTeachingVision : Form
    {
        private readonly CGlobal Global;
        private readonly IDisplayManager displayManager;
        private readonly IDisplayHostBinder displayHostBinder;
        private DockPanel dockPanel;
        private int PanelCount = 0;

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

            toolTip1.SetToolTip(btnNewPanel, "Create New Layer");
            cbCamera.SelectedIndex = 0;
            cbLayerList.SelectedIndex = 0;
        }

        private void InitCameraItem()
        {
            cbCamera.Items.Clear();
            cbCamera.Items.Add("Camera 1");
        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < displayManager.LayerCount; i++) { cbLayerList.Items.Add(displayManager.GetLayerTitle(i)); }
        }

        private void btnNewPanel_Click(object sender, EventArgs e) => displayManager.CreatePanel();

        private void chkUseLayerImage_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkUseLayerImage.Check) { displayManager.SetImageSrc(Lib.Common.CImageConverter.ToMat(displayManager.GetLayerImage(DEFINE.Main)).Clone()); }
                else
                {
                    if (cbLayerList.SelectedItem == null) { return; }
                    displayManager.SelectedItem = cbLayerList.SelectedItem.ToString();
                    displayManager.SetImageSrc(Lib.Common.CImageConverter.ToMat(displayManager.GetLayerImage(displayManager.SelectedItem)).Clone());
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayManager.SetCameraIndex(cbCamera.SelectedIndex);
            displayManager.NotifyParameterChanged();

            CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkUseLayerImage.Check)
            {
                if (cbLayerList.SelectedItem == null) { return; }
                displayManager.SelectedItem = cbLayerList.SelectedItem.ToString();
                displayManager.SetImageSrc(Lib.Common.CImageConverter.ToMat(displayManager.GetLayerImage(displayManager.SelectedItem)).Clone());
                displayManager.ActivateLayer(displayManager.SelectedItem);
            }
        }

        // 최상위 keys 명령어 이기 때문에 
        // Datagridview 같은곳에 editmode f2번같은게 먹지 않는다.        
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            Keys key = keyData & ~(Keys.Shift | Keys.Control);

            switch (key)
            {
                case Keys.Escape:
                    //if (CCommon.ShowMessageBox("Notice", "창을 닫으시겠습니까?"))
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
            try
            {
                displayManager.UpdateResult += OnUpdateResult;
                EventUpdateDisplay += OnUpdateDisplay;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }
            return true;
        }

        private void OnInspResult(object sender, EventArgs e)
        {
            if (!(e is InspResultArgs args)) { return; }
            this.UIThreadInvoke(() =>
            {
                displayManager.CreateLayerDisplay(args.imageResult, "Result");
                displayManager.ActivateLayer("Result");
                displayManager.ZoomLayerToFit("Result");
                lbTackTime.Text = args.tackTime.ToString() + "ms";
            });
        }

        private void InitUi()
        {
            Font font = new Font("Verdana", 12, FontStyle.Regular);

            this.dockPanel = TeachingVisionDockPanelFactory.Create(TeachingPanel, font);
            displayHostBinder.SetForm(this);
            displayHostBinder.SetDockPanel(dockPanel);
            CPropertyGridEditor.SetRuntimeContext(() => displayManager);
            CPropertyGridEditor.SetRecipeNameContext(() => Global.Recipe.Name);
            displayManager.CreateLayerDisplay(new Bitmap(10, 10), "Main", false);

            
            Forms.Add(VISION_DOCK_FORM.THRESHOLD, new FormThreshold(displayManager));            
            ShowVisionForms();

            dockPanel.DockLeftPortion = GetLeftDockWidth();
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
            });
        }

        private void OnUpdateResult(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                lbTackTime.Text = displayManager.TackTime;
            });          
        }

        private void OnUpdateDisplay(object sender, DockDisplayEventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                displayManager.SetLayerImage(e.Index, e.Image);
                displayManager.RefreshLayer(e.Index);
                displayManager.ActivateLayer(e.Index);
                lbTackTime.Text = e.TackTime;                
            });
        }             
        
        private void ShowForm(Form form)
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            form.TopLevel = true;
            form.TopMost = true;
            form.StartPosition = FormStartPosition.CenterParent;
            if (!CUtil.OpenCheckForm(form)) return;
            form.Show();
        }

        private void OnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem)) return;

            string strIndex = (sender as ToolStripMenuItem).Text;
            if(strIndex == "Image Processing") return;
            switch (CUtil.ParseEnum<VISION_MENU>(strIndex))
            {
                case VISION_MENU.Morphology:
                    FormVision_Morphology frm_Morphology = new FormVision_Morphology(displayManager, EventUpdateDisplay);
                    frm_Morphology.SetDisplayManager(displayManager);
                    this.ShowForm(frm_Morphology);                    
                    break;
                case VISION_MENU.Filter:
                    FormVision_Filter Frm_Filter = new FormVision_Filter(displayManager, EventUpdateDisplay);
                    Frm_Filter.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_Filter);
                    break;
                case VISION_MENU.Arithmetic:
                    FormVision_Arithmetic Frm_Arithmetic = new FormVision_Arithmetic(displayManager, EventUpdateDisplay);
                    Frm_Arithmetic.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_Arithmetic);
                    break;
                case VISION_MENU.Blob:
                    FormVision_Blob Frm_Blob = new FormVision_Blob(displayManager, EventUpdateDisplay);
                    Frm_Blob.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_Blob);
                    break;
                case VISION_MENU.Contour:
                    FormVision_Contour Frm_Contour = new FormVision_Contour(displayManager, EventUpdateDisplay);
                    Frm_Contour.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_Contour);                   
                    break;
                case VISION_MENU.Matching:
                    FormVision_Matching Frm_Matching = new FormVision_Matching(displayManager, EventUpdateDisplay);
                    Frm_Matching.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_Matching);                    
                    break;
                case VISION_MENU.FeatureMatching:
                    FormVision_FeatureMatching Frm_FeatureMatching = new FormVision_FeatureMatching(displayManager, EventUpdateDisplay);
                    Frm_FeatureMatching.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_FeatureMatching);
                    break;
                case VISION_MENU.Line:
                    FormVision_Line Frm_Line = new FormVision_Line(displayManager, EventUpdateDisplay);
                    Frm_Line.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_Line);
                    break;
                case VISION_MENU.EdgeDetection:
                    FormVision_EdgeDection Frm_EdgeDetector = new FormVision_EdgeDection(displayManager, EventUpdateDisplay);
                    Frm_EdgeDetector.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_EdgeDetector);                    
                    break;
                case VISION_MENU.RotateAndScale:
                    FormVision_RotateAndScale Frm_RotateAndScale = new FormVision_RotateAndScale(displayManager, EventUpdateDisplay);
                    Frm_RotateAndScale.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_RotateAndScale);                    
                    break;
                case VISION_MENU.Histogram:
                    FormVision_Histogram Frm_Histogram = new FormVision_Histogram(displayManager, EventUpdateDisplay);
                    Frm_Histogram.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_Histogram);                    
                    break;
                case VISION_MENU.Mean:
                    FormVision_Mean Frm_Mean = new FormVision_Mean(displayManager, EventUpdateDisplay);
                    Frm_Mean.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_Mean);                    
                    break;
                case VISION_MENU.HSV:
                    FormVision_HSV Frm_HSV = new FormVision_HSV(displayManager, EventUpdateDisplay);
                    Frm_HSV.SetDisplayManager(displayManager);
                    this.ShowForm(Frm_HSV);                    
                    break;
            }
        }

        private void rjLabel2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PanelCount != displayManager.LayerCount)
                {
                    PanelCount = displayManager.LayerCount;
                    InitLayListItem();
                }

                if (displayManager.IsLayerImageChanged(displayManager.SelectedItem))
                {
                    displayManager.SetImageSrc(Lib.Common.CImageConverter.ToMat(displayManager.GetLayerImage(displayManager.SelectedItem)));
                    displayManager.AcceptLayerImageChanged(displayManager.SelectedItem);
                }                
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }   
}
