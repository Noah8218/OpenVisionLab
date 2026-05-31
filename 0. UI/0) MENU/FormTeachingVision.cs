using OpenVisionLab._1._Core;
using Lib.Common;
using Lib.OpenCV;
using MetroFramework.Forms;
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
    public partial class FormTeachingVision : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;
        private DockPanel dockPanel;
        private int PanelCount = 0;

        #region Event Register                
        private EventHandler<DockDisplayEventArgs> EventUpdateDisplay;
        #endregion

        private Dictionary<VISION_DOCK_FORM, object> Forms = new Dictionary<VISION_DOCK_FORM, object>();

        public FormTeachingVision() => InitializeComponent();
        
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
            for (int i = 0; i < Global.Device.CAMERA_COUNT; i++) { cbCamera.Items.Add("Camera " + (i + 1)); }
        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < CDisplayManager.Displays.Count; i++) { cbLayerList.Items.Add(CDisplayManager.Displays[i].Text); }
        }

        private void btnNewPanel_Click(object sender, EventArgs e) => CDisplayManager.CreatePanel();

        private void chkUseLayerImage_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkUseLayerImage.Check) { CDisplayManager.ImageSrc = Lib.Common.CImageConverter.ToMat((Bitmap)CDisplayManager.Displays[DEFINE.Main].viewer._Ib.Image).Clone(); }
                else
                {
                    if (cbLayerList.SelectedItem == null) { return; }
                    CDisplayManager.SelecteItem = cbLayerList.SelectedItem.ToString();
                    CDisplayManager.ImageSrc = Lib.Common.CImageConverter.ToMat((Bitmap)CDisplayManager.Displays[CDisplayManager.FindIndex(CDisplayManager.SelecteItem)].viewer._Ib.Image).Clone();
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            CDisplayManager.CameraIndex = cbCamera.SelectedIndex;

            if (CDisplayManager.EventUpdateParameter != null)
            {
                CDisplayManager.EventUpdateParameter(null, null);
            }

            CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkUseLayerImage.Check)
            {
                if (cbLayerList.SelectedItem == null) { return; }
                CDisplayManager.SelecteItem = cbLayerList.SelectedItem.ToString();
                CDisplayManager.ImageSrc = Lib.Common.CImageConverter.ToMat((Bitmap)CDisplayManager.Displays[CDisplayManager.FindIndex(CDisplayManager.SelecteItem)].viewer._Ib.Image).Clone();
                CDisplayManager.Displays[CDisplayManager.FindIndex(CDisplayManager.SelecteItem)].Activate();
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


        private void OnGrabEnd(object sender, GrabEventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                if (Global.System.Menu == CSystem.MENU.VISION)
                {
                    if (!COpenCVHelper.IsImageEmpty(e.ImageGrab))
                    {
                        //CDisplayManager.Displays[DEFINE.Main].Activate();
                        CDisplayManager.Displays[DEFINE.Main].ibSource.Image = Lib.Common.CImageConverter.ToBitmap(e.ImageGrab.Clone());

                        e.ImageGrab.Dispose();
                        e.ImageGrab = null;
                    }

                    GC.Collect();
                }
            });           
        }

        private bool InitEvent()
        {
            try
            {
                CDisplayManager.EventUpdateResult += OnUpdateResult;
                EventUpdateDisplay += OnUpdateDisplay;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;                

                for (int i = 0; i < Global.Device.CAMERAS.Count; i++)
                {
                    Global.Device.CAMERAS[i].EventGrabEnd += OnGrabEnd;
                }
                
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
                CDisplayManager.CreateLayerDisplay(args.imageResult, "Result");
                CDisplayManager.Displays[CDisplayManager.FindIndex("Result")].Activate();
                CDisplayManager.Displays[CDisplayManager.FindIndex("Result")].ibSource.ZoomToFit();
                lbTackTime.Text = args.tackTime.ToString() + "ms";
            });
        }

        private void InitUi()
        {
            Font font = new Font("Verdana", 12, FontStyle.Regular);

            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.dockPanel.Theme = new VS2015DarkTheme();
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            TeachingPanel.Controls.Add(this.dockPanel);
            CDisplayManager.SetForm(this);
            CDisplayManager.SetDockPanel(dockPanel);
            CDisplayManager.Displays.Add(new FormLayerDisplay(new Bitmap(10, 10), 0, CDisplayManager.Displays, false, "Main"));
            CDisplayManager.Displays[DEFINE.Main].Show(this.dockPanel, DockState.Document);

            dockPanel.Theme.Skin.DockPaneStripSkin.TextFont = font;
            dockPanel.Theme.Skin.AutoHideStripSkin.TextFont = font;

            Forms.Add(VISION_DOCK_FORM.System, new FormSystem());
            Forms.Add(VISION_DOCK_FORM.BLOB, new FormBlob());
            Forms.Add(VISION_DOCK_FORM.LINE, new FormLine());
            Forms.Add(VISION_DOCK_FORM.PROPERTY, new FormProperty());
            Forms.Add(VISION_DOCK_FORM.THRESHOLD, new FormThreshold());
            Forms.Add(VISION_DOCK_FORM.TEACHING, new FormTeaching());
            ShowVisionForms();

            dockPanel.DockLeftPortion = panel1.Width;
        }

        private void ShowVisionForms()
        {
            WeifenLuo.WinFormsUI.Docking.DockContent fr;
            foreach (var form in Forms)
            {
                fr = (form.Value as WeifenLuo.WinFormsUI.Docking.DockContent);
                DockContent system = (Forms[VISION_DOCK_FORM.System] as DockContent);
                switch (form.Key)
                {
                    case VISION_DOCK_FORM.System:
                        fr.Show(this.dockPanel, DockState.DockLeft);
                        fr.AutoHidePortion = panel1.Width;
                        break;
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
                    case VISION_DOCK_FORM.PROPERTY:
                        fr.Show(system.PanelPane, DockAlignment.Bottom, 0.47);
                        fr.AutoHidePortion = panel1.Width;
                        break;
                    case VISION_DOCK_FORM.THRESHOLD:
                        fr.Show(this.dockPanel, DockState.DockLeftAutoHide);
                        fr.AutoHidePortion = 500;
                        break;
                }
            }
            fr = (Forms[VISION_DOCK_FORM.System] as WeifenLuo.WinFormsUI.Docking.DockContent);
            fr.Activate();
            fr = (Forms[VISION_DOCK_FORM.PROPERTY] as WeifenLuo.WinFormsUI.Docking.DockContent);
            fr.Activate();
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
                lbTackTime.Text = CDisplayManager.TackTime;
            });          
        }

        private void OnUpdateDisplay(object sender, DockDisplayEventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                CDisplayManager.Displays[e.Index].ibSource.Image = e.Image;
                CDisplayManager.Displays[e.Index].ibSource.Refresh();
                CDisplayManager.Displays[e.Index].Activate();
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
                    FormVision_Morphology frm_Morphology = new FormVision_Morphology(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(frm_Morphology);                    
                    break;
                case VISION_MENU.Filter:
                    FormVision_Filter Frm_Filter = new FormVision_Filter(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_Filter);
                    break;
                case VISION_MENU.Arithmetic:
                    FormVision_Arithmetic Frm_Arithmetic = new FormVision_Arithmetic(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_Arithmetic);
                    break;
                case VISION_MENU.Blob:
                    FormVision_Blob Frm_Blob = new FormVision_Blob(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_Blob);
                    break;
                case VISION_MENU.Contour:
                    FormVision_Contour Frm_Contour = new FormVision_Contour(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_Contour);                   
                    break;
                case VISION_MENU.Matching:
                    FormVision_Matching Frm_Matching = new FormVision_Matching(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_Matching);                    
                    break;
                case VISION_MENU.FeatureMatching:
                    FormVision_FeatureMatching Frm_FeatureMatching = new FormVision_FeatureMatching(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_FeatureMatching);
                    break;
                case VISION_MENU.Line:
                    FormVision_Line Frm_Line = new FormVision_Line(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_Line);
                    break;
                case VISION_MENU.EdgeDetection:
                    FormVision_EdgeDection Frm_EdgeDetector = new FormVision_EdgeDection(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_EdgeDetector);                    
                    break;
                case VISION_MENU.RotateAndScale:
                    FormVision_RotateAndScale Frm_RotateAndScale = new FormVision_RotateAndScale(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_RotateAndScale);                    
                    break;
                case VISION_MENU.Histogram:
                    FormVision_Histogram Frm_Histogram = new FormVision_Histogram(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_Histogram);                    
                    break;
                case VISION_MENU.Mean:
                    FormVision_Mean Frm_Mean = new FormVision_Mean(CDisplayManager.Displays, EventUpdateDisplay);
                    this.ShowForm(Frm_Mean);                    
                    break;
                case VISION_MENU.HSV:
                    FormVision_HSV Frm_HSV = new FormVision_HSV(CDisplayManager.Displays, EventUpdateDisplay);
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
                if (PanelCount != CDisplayManager.Displays.Count)
                {
                    PanelCount = CDisplayManager.Displays.Count;
                    InitLayListItem();
                }

                if (CDisplayManager.Displays[CDisplayManager.FindIndex()].viewer._ImageChanged)
                {
                    CDisplayManager.ImageSrc = Lib.Common.CImageConverter.ToMat((Bitmap)CDisplayManager.Displays[CDisplayManager.FindIndex()].viewer._Ib.Image);
                    CDisplayManager.Displays[CDisplayManager.FindIndex()].viewer._ImageChanged = false;
                }                
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }   
}