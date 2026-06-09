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
        private readonly GlobalState Global;
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
                        if (!chkUseLayerImage.Check) { displayManager.SetImageSrc(Lib.Common.BitmapImageConverter.ToMat(displayManager.GetLayerImage(DEFINE.Main)).Clone()); }
            else
            {
                if (cbLayerList.SelectedItem == null) { return; }
                displayManager.SelectedItem = cbLayerList.SelectedItem.ToString();
                displayManager.SetImageSrc(Lib.Common.BitmapImageConverter.ToMat(displayManager.GetLayerImage(displayManager.SelectedItem)).Clone());
            }
        
        }

        private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayManager.SetCameraIndex(cbCamera.SelectedIndex);
            displayManager.NotifyParameterChanged();

        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkUseLayerImage.Check)
            {
                if (cbLayerList.SelectedItem == null) { return; }
                displayManager.SelectedItem = cbLayerList.SelectedItem.ToString();
                displayManager.SetImageSrc(Lib.Common.BitmapImageConverter.ToMat(displayManager.GetLayerImage(displayManager.SelectedItem)).Clone());
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
            EventUpdateDisplay += OnUpdateDisplay;
            Global.Recipe.EventChagedRecipe += OnChangedRecipe;
        
            return true;
        }
        private void InitUi()
        {
            Font font = new Font("Verdana", 12, FontStyle.Regular);

            this.dockPanel = TeachingVisionDockPanelFactory.Create(TeachingPanel, font);
            displayHostBinder.SetForm(this);
            displayHostBinder.SetDockPanel(dockPanel);
            PropertyGridEditorFactory.SetRuntimeContext(() => displayManager);
            PropertyGridEditorFactory.SetRecipeNameContext(() => Global.Recipe.Name);
            displayManager.CreateLayerDisplay(new Bitmap(10, 10), "Main", false);

            
            Forms.Add(VISION_DOCK_FORM.THRESHOLD, new FormThreshold(displayManager));
            Forms.Add(VISION_DOCK_FORM.LOG, new FormLogViewer());
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
                    case VISION_DOCK_FORM.LOG:
                        fr.Show(this.dockPanel, DockState.DockLeftAutoHide);
                        fr.AutoHidePortion = 650;
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

        public void ShowDockedLogViewer()
        {
            if (!Forms.TryGetValue(VISION_DOCK_FORM.LOG, out object value) || !(value is DockContent logViewer))
            {
                return;
            }

            if (logViewer.DockPanel == null)
            {
                logViewer.Show(this.dockPanel, DockState.DockLeftAutoHide);
                logViewer.AutoHidePortion = 650;
            }

            logViewer.Activate();
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
            if (!AppUtil.OpenCheckForm(form)) return;
            form.Show();
        }

        private void OnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem)) return;

            string strIndex = (sender as ToolStripMenuItem).Text;
            if(strIndex == "Image Processing") return;
            switch (AppUtil.ParseEnum<VISION_MENU>(strIndex))
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
        
        }
    }   
}
