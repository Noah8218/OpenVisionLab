using Cyotek.Windows.Forms;
using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using OpenCvSharp;
using RJCodeUI_M1.RJControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static OpenVisionLab.DEFINE;
using System.Linq;
using static OpenVisionLab._2._Common.CParameterManager;
using Lib.Common;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using Lib.OpenCV.Blob;

namespace OpenVisionLab
{
    public partial class FormTeaching : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        private List<CPropertyParam> ParamList = new List<CPropertyParam>();
        private int TeachingNo = 0;

        private CViewer Train = new CViewer();

        public FormTeaching()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }

        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        private bool ChangeSize = false;

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Train.LoadImageBoxOnlyViewer(ibTrainImage);
            cbProcess.Items.Add(DEFINE.PROCESS_TYPES.Bolt_tightened);
            cbProcess.Items.Add(DEFINE.PROCESS_TYPES.Pin_Inspection);
            cbProcess.Items.Add(DEFINE.PROCESS_TYPES.Connector_Inspection);
            cbProcess.Items.Add(DEFINE.PROCESS_TYPES.ThermalGrease_Inspection);
            cbProcess.SelectedIndex = 0;
            AddTeachingCount();
            propertyGrid_Parameter.PropertyValueChanged += PropertyGrid_Parameter_PropertyValueChanged;
            this.Resize += FormTeaching_Resize;

            Global.Thread.CSeqVision.EventSeqComplete += OnInspResult;
        }

        private void OnInspResult(object sender, EventArgs e)
        {
            if (!(e is InspResultArgs args)) { return; }
            this.UIThreadBeginInvoke(() =>
            {
                if (Global.System.Menu == CSystem.MENU.VISION)
                {

                }
            });
        }

        private void FormTeaching_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void PropertyGrid_Parameter_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ibTrainImage.Image = null;
            if (File.Exists(ParamList[TeachingNo].Matching.PATTERN_PATH))
            {
                ibTrainImage.Image = new Bitmap(ParamList[TeachingNo].Matching.PATTERN_PATH);
                ibTrainImage.ZoomToFit();
            }

            Save();
        }

        private void cbTeachingCount_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            TeachingNo = cbTeachingCount.SelectedIndex;
            ibTrainImage.Image = null;
            gridROI.DataSource = null;
            this.gridROI.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystrokeOrF2;
            gridROI.ColumnHeaderFont = new Font("Verdana", 12);
            ShowDatagridView(ParamList);
            propertyGrid_Parameter.SelectedObject = ParamList[TeachingNo].Matching;
            if (File.Exists(ParamList[TeachingNo].Matching.PATTERN_PATH))
            {
                ibTrainImage.Image = new Bitmap(ParamList[TeachingNo].Matching.PATTERN_PATH);
                ibTrainImage.ZoomToFit();
            }

            if (CDisplayManager.FindIndex("ROI") != 0)
            {
                CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer.SetModeMultiRoi();
                //CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer._RoisOb = ParamList[TeachingNo].ROIs;
                CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer._Ib.Refresh();
            }

            gridROI.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            gridROI.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void ShowDatagridView(List<CPropertyParam> ParamList)
        {
            gridROI.DataSource = GetBindingRoiList(ParamList[TeachingNo].ROIs);
            for (int i = 0; i < ParamList[TeachingNo].ROIs.Count; i++)
            {
                DataGridViewComboBoxCell cCell = new DataGridViewComboBoxCell();
                cCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                cCell.Items.Add(DEFINE.ALGORITHM.BLOB);
                cCell.Items.Add(DEFINE.ALGORITHM.MATCING);
                cCell.Items.Add(DEFINE.ALGORITHM.MEAN);
                //cCell.Items.Add(DEFINE.ALGORITHM.CONTOUR);
                cCell.Value = ParamList[TeachingNo].ROIs[i].Algorithm;
                gridROI.Rows[i].Cells[(int)DEFINE.GRID_ROI_COLUMN.ALGORITHM] = cCell;
            }
        }

        public BindingList<CRectangle> GetBindingRoiList(List<CRectangle> cResultBlobs)
        {
            var list = new BindingList<CRectangle>();

            for (int i = 0; i < cResultBlobs.Count; i++)
            {
                CRectangle cDefectList = cResultBlobs[i].DeepCopy();
                cDefectList.Index = cDefectList.Index + 1;
                list.Add(cDefectList);
            }

            return list;
        }

        private void btnTeachingAdd_Click(object sender, EventArgs e)
        {
            switch (CUtil.ParseEnum<DEFINE.PROCESS_TYPES>(cbProcess.SelectedItem.ToString()))
            {
                case DEFINE.PROCESS_TYPES.Bolt_tightened:
                    ParamList.Add(new CPropertyParam($"{PROCESS_TYPES.Bolt_tightened}-{ParamList.Count + 1}"));
                    break;
                case DEFINE.PROCESS_TYPES.Pin_Inspection:
                    ParamList.Add(new CPropertyParam($"{PROCESS_TYPES.Pin_Inspection}-{ParamList.Count + 1}"));
                    break;
                case DEFINE.PROCESS_TYPES.Connector_Inspection:
                    ParamList.Add(new CPropertyParam($"{PROCESS_TYPES.Connector_Inspection}-{ParamList.Count + 1}"));
                    break;
                case DEFINE.PROCESS_TYPES.ThermalGrease_Inspection:
                    ParamList.Add(new CPropertyParam($"{PROCESS_TYPES.ThermalGrease_Inspection}-{ParamList.Count + 1}"));
                    break;
            }
            AddTeachingCount();
        }

        private void btnTeachingDelete_Click(object sender, EventArgs e)
        {
            if (ParamList.Count > 0)
            {
                ParamList.RemoveAt(ParamList.Count - 1);
            }

            AddTeachingCount();
        }

        private enum TabMenu : int
        {
            Bolb,
            Pin,
            Connector_Line,
            Connector_Mean,
        }

        private void cbProcess_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CUtil.ParseEnum<DEFINE.PROCESS_TYPES>(cbProcess.SelectedItem.ToString()))
            {
                case DEFINE.PROCESS_TYPES.Bolt_tightened:
                    ParamList = Global.Data.ParamList_Bolt;
                    break;
                case DEFINE.PROCESS_TYPES.Pin_Inspection:
                    ParamList = Global.Data.ParamList_Pin;
                    break;
                case DEFINE.PROCESS_TYPES.Connector_Inspection:
                    ParamList = Global.Data.ParamList_Connector;
                    break;
                case DEFINE.PROCESS_TYPES.ThermalGrease_Inspection:
                    ParamList = Global.Data.ParamList_Thermal;
                    break;
            }
            AddTeachingCount();
        }

        private void AddTeachingCount()
        {
            cbTeachingCount.Text = "";
            cbTeachingCount.Items.Clear();
            for (int i = 0; i < ParamList.Count; i++) { cbTeachingCount.Items.Add(ParamList[i].NAME); }
            if (cbTeachingCount.Items.Count > 0)
            {
                TeachingNo = ParamList.Count - 1;
                cbTeachingCount.SelectedIndex = ParamList.Count - 1;
            }
            //rdoMean.Checked = true;
        }

        private void btnSaveVisionParam_Click(object sender, EventArgs e)
        {
            if (CCommon.ShowdialogMessageBox("Save", "ROI 설정을 저장 하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
            {
                Save();
            }
        }

        private void Save()
        {
            for (int i = 0; i < Global.Data.ParamList_Bolt.Count; i++) { Global.Data.ParamList_Bolt[i].SaveConfig(Global.Recipe.Name); }
            Global.Data.ParamList_Bolt_Count = Global.Data.ParamList_Bolt.Count;
            for (int i = 0; i < Global.Data.ParamList_Pin.Count; i++) { Global.Data.ParamList_Pin[i].SaveConfig(Global.Recipe.Name); }
            Global.Data.ParamList_Pin_Count = Global.Data.ParamList_Pin.Count;
            for (int i = 0; i < Global.Data.ParamList_Connector.Count; i++) { Global.Data.ParamList_Connector[i].SaveConfig(Global.Recipe.Name); }
            Global.Data.ParamList_Connector_Count = Global.Data.ParamList_Connector.Count;
            for (int i = 0; i < Global.Data.ParamList_Thermal.Count; i++) { Global.Data.ParamList_Thermal[i].SaveConfig(Global.Recipe.Name); }
            Global.Data.ParamList_Thermal_Count = Global.Data.ParamList_Thermal.Count;
            Global.Data.SaveConfig(Global.Recipe.Name);
        }

        private void btnTeachingRun_Click(object sender, EventArgs e)
        {
            RunMatching();
        }

        private List<CResultMatching> RunMatching(Graphics g, Mat ImageCVSource, List<CPropertyParam> ParamList, int TeachingNo, ref List<Rectangle> calculRoi)
        {
            CPropertyMatching cProperty = (CPropertyMatching)ParamList[TeachingNo].Matching.DeepCopy();

            int Hmc_Path_Index = cProperty.PATTERN_PATH.IndexOf("PATTERN\\") + 8;
            int total_Length = cProperty.PATTERN_PATH.Length;
            string splite_Path = cProperty.PATTERN_PATH.Substring(Hmc_Path_Index, total_Length - Hmc_Path_Index);
            string Pattern_Path = Application.StartupPath + $"\\RECIPE\\{CGlobal.Inst.Recipe.Name}\\PATTERN\\{splite_Path}";

            CVMatching Matching = new CVMatching();
            if (System.IO.File.Exists(Pattern_Path))
            {
                Matching.SetProperty(cProperty);
                Matching.SetTemplateImage(Cv2.ImRead(Pattern_Path));
                Matching.SetSourceImage(ImageCVSource.Clone());
                Matching.ImagePyramidsSingleRun();
            }

            foreach (var item in Matching.results)
            {
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 5), new Rectangle((int)item.Bounding.X, (int)item.Bounding.Y, (int)item.Bounding.Width, (int)item.Bounding.Height));
                g.DrawString("Fiducial", new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 20, (int)item.Bounding.Y - 20);
                g.DrawString(item.Score.ToString("F3"), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Center.X, (int)item.Center.Y);
            }

            if (Matching.results.Count == 0)
            {
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cProperty.CvROI));
                g.DrawString("Fiducial", new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cProperty.CvROI.X - 20, (int)cProperty.CvROI.Y - 20);
                //Result = DEFINE.RESULT.NG;
            }

            var ROIs = ParamList[TeachingNo].ROIs.ToList();
            foreach (var ROI in ROIs)
            {
                if (Matching.results.Count == 0) { break; }
                calculRoi.Add(new Rectangle((int)(Matching.results[0].Center.X + ROI.PidutialX), (int)(Matching.results[0].Center.Y + ROI.PidutialY), ROI.Roi.Width, ROI.Roi.Height));
            }

            return Matching.results;
        }

        private List<CResultMatching> RunMatching()
        {
            List<CResultMatching> cResultMatchings = new List<CResultMatching>();
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, ImageCVSource.Width, ImageCVSource.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    CPropertyMatching cPropertyMatching = (CPropertyMatching)ParamList[TeachingNo].Matching.DeepCopy();

                    Stopwatch stopwatch1 = Stopwatch.StartNew();
                    CVMatching Matching = new CVMatching();
                    Matching.SetProperty(cPropertyMatching);
                    Matching.SetTemplateImage(Lib.Common.CImageConverter.ToMat((Bitmap)ibTrainImage.Image));
                    Matching.SetSourceImage(ImageCVSource);
                    Matching.ImagePyramidsSingleRun();
                    cResultMatchings = Matching.results.ConvertAll(s => s);

                    Graphics g = Graphics.FromImage(Result);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    foreach (var item in cResultMatchings)
                    {
                        RotateDraw(g, item, (float)item.Angle, new System.Drawing.Pen(System.Drawing.Color.Red, 1));
                    }

                    if (Matching.results.Count == 0)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(Matching.property.CvROI));
                        g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)Matching.property.CvROI.X - 20, (int)Matching.property.CvROI.Y - 20);
                    }

                    CDisplayManager.TackTime = stopwatch1.Elapsed.TotalSeconds.ToString() + "s";
                    CDisplayManager.CreateLayerDisplay(Result, "상대좌표 Matching", true);
                    stopwatch1.Stop();
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
            return cResultMatchings;
        }

        private List<CResultMatching> RunMatching(Mat image)
        {
            List<CResultMatching> cResultMatchings = new List<CResultMatching>();
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (Mat ImageCVSource = image.Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, ImageCVSource.Width, ImageCVSource.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    CPropertyMatching cPropertyMatching = (CPropertyMatching)ParamList[TeachingNo].Matching.DeepCopy();

                    Stopwatch stopwatch1 = Stopwatch.StartNew();
                    CVMatching Matching = new CVMatching();
                    Matching.SetProperty(cPropertyMatching);
                    Matching.SetTemplateImage(Lib.Common.CImageConverter.ToMat((Bitmap)ibTrainImage.Image));
                    Matching.SetSourceImage(ImageCVSource);
                    Matching.ImagePyramidsSingleRun();
                    cResultMatchings = Matching.results.ConvertAll(s => s);

                    Graphics g = Graphics.FromImage(Result);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    foreach (var item in cResultMatchings)
                    {
                        RotateDraw(g, item, (float)item.Angle, new System.Drawing.Pen(System.Drawing.Color.Red, 1));
                    }

                    if (Matching.results.Count == 0)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(Matching.property.CvROI));
                        g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)Matching.property.CvROI.X - 20, (int)Matching.property.CvROI.Y - 20);
                    }

                    CDisplayManager.TackTime = stopwatch1.Elapsed.TotalSeconds.ToString() + "s";
                    CDisplayManager.CreateLayerDisplay(Result, "상대좌표 Matching", true);
                    stopwatch1.Stop();
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
            return cResultMatchings;
        }

        public void RotateDraw(Graphics g, CResultMatching item, float angle, System.Drawing.Pen pen)
        {
            using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix())
            {
                RectangleF r = item.Bounding;
                m.RotateAt(angle, new PointF(r.Left + (r.Width / 2), r.Top + (r.Height / 2)));
                g.Transform = m;
                g.DrawRectangles(pen, new[] { r });
                g.DrawLine(pen, r.X + r.Width / 2, r.Y, r.X + r.Width / 2, r.Y + r.Height);
                g.DrawLine(pen, r.X, r.Y + r.Height / 2, r.X + r.Width, r.Y + r.Height / 2);
                g.DrawString((item.Index + 1).ToString(), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 20, (int)item.Bounding.Y - 20);
                g.DrawString(item.Score.ToString("F3"), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Center.X, (int)item.Center.Y);
                g.ResetTransform();
            }
        }

        private void btnTeaching_Click(object sender, EventArgs e)
        {
            if (CCommon.ShowdialogMessageBox("MASTER TEACHING", "해당 이미지로 마스터 각도를 저장하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
            {
                var Results = RunMatching();
                if (Results.Count != 0)
                {
                    ParamList[TeachingNo].Master_T = Results[0].Angle;
                    ParamList[TeachingNo].CenterX = Results[0].Center.X;
                    ParamList[TeachingNo].CenterY = Results[0].Center.Y;
                    ParamList[TeachingNo].SaveConfig(Global.Recipe.Name);
                    Save();
                }
            }
        }

        private void btnRoiSet_Click(object sender, EventArgs e)
        {            
            CDisplayManager.CreateLayerDisplay(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), "ROI", true);
            CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer.SetModeMultiRoi();
            //CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer.ROIs = ParamList[TeachingNo].ROIs;
            CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer._Ib.MouseUp += Ib_MouseUp;
            CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer._Ib.KeyDown += IbSource_KeyDown;

            cbTeachingCount_OnSelectedIndexChanged(null, null);
        }

        public Mat Rotate(Mat src, double angle)
        {
            Mat rotate = new Mat(src.Size(), MatType.CV_8UC3);
            Mat matrix = Cv2.GetRotationMatrix2D(new Point2f(src.Width / 2, src.Height / 2), angle, 1);
            Cv2.WarpAffine(src, rotate, matrix, src.Size(), InterpolationFlags.Linear);
            return rotate;
        }

        private void Ib_MouseUp(object sender, MouseEventArgs e)
        {
            //cbTeachingCount_OnSelectedIndexChanged(null, null);
        }

        private void IbSource_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                case Keys.ControlKey:
                    break;
                case Keys.Delete:
                    cbTeachingCount_OnSelectedIndexChanged(null, null);
                    break;
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    break;
            }

        }

        private void dgvSeletecList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;

                if (dgv.CurrentCell == null) { return; }
                int columnIndex = dgv.CurrentCell.ColumnIndex;
                int rowIndex = dgv.CurrentCell.RowIndex;

                if (dgv.Rows[rowIndex].Cells[1].Value == null) { return; }

                var rect = new Rectangle(int.Parse(dgv.Rows[rowIndex].Cells[1].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[2].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[3].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[4].Value.ToString()));


                Rectangle rt = CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer._Ib.RectangleToScreen(rect);
                CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer._Ib.ZoomToRegion(rt.X, rt.Y, rt.Width, rt.Height);
                CDisplayManager.Displays[CDisplayManager.FindIndex("ROI")].viewer._Ib.ScrollTo(rect.X - (rect.Width / 2), rect.Y - (rect.Height / 2), 1, 1);

                if (CDisplayManager.FindIndex("Threshold") != 0)
                {
                    rt = CDisplayManager.Displays[CDisplayManager.FindIndex("Threshold")].viewer._Ib.RectangleToScreen(rect);
                    CDisplayManager.Displays[CDisplayManager.FindIndex("Threshold")].viewer._Ib.ZoomToRegion(rt.X, rt.Y, rt.Width, rt.Height);
                    CDisplayManager.Displays[CDisplayManager.FindIndex("Threshold")].viewer._Ib.ScrollTo(rect.X - (rect.Width / 2), rect.Y - (rect.Height / 2), 1, 1);
                }

                if (dgv.Rows[rowIndex].Cells[(int)DEFINE.GRID_ROI_COLUMN.ALGORITHM].Value == DEFINE.ALGORITHM.MATCING.ToString())
                {
                    try
                    {
                        Rectangle r = CConverter.RectToRectangle(ParamList[TeachingNo].Matching.CvROI);

                        FormImageEditView FrmImageEdit = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), r, "TRAIN");
                        if (FrmImageEdit.ShowDialog() == DialogResult.OK)
                        {
                            CUtil.InitDirectory($"RECIPE\\{CGlobal.Inst.Recipe.Name}\\PATTERN");
                            string Path = Application.StartupPath + $"\\RECIPE\\{CGlobal.Inst.Recipe.Name}\\PATTERN\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bmp";
                            Mat ImageTemplate = CDisplayManager.ImageSrc.SubMat(FrmImageEdit.SelectedRegion).Clone();
                            Cv2.ImWrite(Path, ImageTemplate);
                        }
                    }
                    catch (Exception Desc)
                    {
                        CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                        CCommon.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                    }
                }

                //switch (e.ColumnIndex)
                //{
                //    case 13:
                //        int nRowIndex = gridROI.CurrentCell.RowIndex;
                //        var oldValue = gridROI[e.ColumnIndex, e.RowIndex].Value;

                //        OpenFileDialog ofd = new OpenFileDialog();
                //        ofd.InitialDirectory = Application.StartupPath + "\\RECIPE\\" + Global.Recipe.Name + "\\Template";
                //        string strFilePath = "";
                //        if (ofd.ShowDialog() == DialogResult.OK)
                //        {
                //            strFilePath = ofd.FileName;
                //            CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                //        }
                //        break;
                //}
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void gridROI_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            int columnIndex = dgv.CurrentCell.ColumnIndex;
            int rowIndex = dgv.CurrentCell.RowIndex;

            if (dgv.Rows[rowIndex].Cells[(int)DEFINE.GRID_ROI_COLUMN.ALGORITHM].Value.ToString() == DEFINE.ALGORITHM.MATCING.ToString())
            {
                Rectangle r = ParamList[TeachingNo].ROIs[rowIndex].Roi;

                FormImageEditView FrmImageView = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), r, "TRAIN");
                if (FrmImageView.ShowDialog() == DialogResult.OK)
                {
                    CUtil.InitDirectory($"RECIPE\\{Global.Recipe.Name}\\PATTERN");
                    string Path = Application.StartupPath + $"\\RECIPE\\{Global.Recipe.Name}\\PATTERN\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bmp";
                    Mat ImageTemplate = CDisplayManager.ImageSrc.SubMat(FrmImageView.SelectedRegion);
                    gridROI[13, rowIndex].Value = Path;
                    ParamList[TeachingNo].ROIs[rowIndex].PATTERN_PATH = Path;
                    Cv2.ImWrite(Path, ImageTemplate);
                }
            }
        }

        private void gridROI_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (gridROI.CurrentCell == null) { return; }
                int rowIndex = gridROI.CurrentCell.RowIndex;
                var oldValue = gridROI[e.ColumnIndex, e.RowIndex].Value;

                if (oldValue == null) { return; }
                Rectangle rect = ParamList[TeachingNo].ROIs[rowIndex].Roi;
                switch (e.ColumnIndex)
                {
                    case (int)DEFINE.GRID_ROI_COLUMN.X:
                        rect = new Rectangle(int.Parse(oldValue.ToString()),
                                                rect.Y,
                                                rect.Width,
                                                rect.Height);
                        ParamList[TeachingNo].ROIs[rowIndex].Roi = rect;
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.Y:
                        rect = new Rectangle(rect.X,
                                                int.Parse(oldValue.ToString()),
                                                rect.Width,
                                                rect.Height);
                        ParamList[TeachingNo].ROIs[rowIndex].Roi = rect;
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.WIDTH:
                        rect = new Rectangle(rect.X,
                                                rect.Y,
                                                int.Parse(oldValue.ToString()),
                                               rect.Height);
                        ParamList[TeachingNo].ROIs[rowIndex].Roi = rect;
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.HEIGHT:
                        rect = new Rectangle(rect.X,
                                                rect.Y,
                                                rect.Width,
                                                int.Parse(oldValue.ToString()));
                        ParamList[TeachingNo].ROIs[rowIndex].Roi = rect;
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.ALGORITHM:
                        ParamList[TeachingNo].ROIs[rowIndex].Algorithm = (DEFINE.ALGORITHM)Enum.Parse(typeof(DEFINE.ALGORITHM), oldValue.ToString());
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.USE_BITWISENOT:
                        ParamList[TeachingNo].ROIs[rowIndex].USE_BITWISENOT = bool.Parse(oldValue.ToString());
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.THRESHOLD:
                        ParamList[TeachingNo].ROIs[rowIndex].THRESHOLD = int.Parse(oldValue.ToString());
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.MIN_AREA:
                        ParamList[TeachingNo].ROIs[rowIndex].MIN_AREA = int.Parse(oldValue.ToString());
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.MAX_AREA:
                        ParamList[TeachingNo].ROIs[rowIndex].MAX_AREA = int.Parse(oldValue.ToString());
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.InspCount:
                        ParamList[TeachingNo].ROIs[rowIndex].InspCount = int.Parse(oldValue.ToString());
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.SCORE:
                        ParamList[TeachingNo].ROIs[rowIndex].SCORE = double.Parse(oldValue.ToString());
                        break;
                    case (int)DEFINE.GRID_ROI_COLUMN.USE_ROI:
                        ParamList[TeachingNo].ROIs[rowIndex].USE_ROI = bool.Parse(oldValue.ToString());
                        break;
                }

                //gridROI.Rows[rowIndex].Frozen = true;
                //gridROI.EnableHeadersVisualStyles = false;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnRoiReleative_Click(object sender, EventArgs e)
        {
            if (CCommon.ShowdialogMessageBox("Save", "ROI 상대좌표로 설정 하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
            {
                var Results = RunMatching();
                if (Results.Count != 0) { SetROI(Results, ref ParamList[TeachingNo].ROIs); }
                for (int i = 0; i < ParamList[TeachingNo].ROIs.Count; i++)
                {
                    ParamList[TeachingNo].ROIs[i].Relative_Coordinates = true;
                }
            }
        }

        private void SetROI(List<CResultMatching> Matching_Results, ref List<CRectangle> ROIs)
        {
            foreach (var ROI in ROIs)
            {
                ROI.PreRoi = ROI.Roi;
                ROI.PidutialX = (float)(ROI.Roi.X - Matching_Results[0].Center.X);
                ROI.PidutialY = (float)(ROI.Roi.Y - Matching_Results[0].Center.Y);
                ROI.Roi = new Rectangle((int)(Matching_Results[0].Center.X + ROI.PidutialX), (int)(Matching_Results[0].Center.Y + ROI.PidutialY), ROI.Roi.Width, ROI.Roi.Height);
            }
        }

        private void SetROI(List<CResultMatching> Matching_Results, ref List<CSpecAreas> ROIs)
        {
            foreach (var ROI in ROIs)
            {
                ROI.PreRoi = ROI.SpecArea;
                ROI.PidutialX = (float)(ROI.SpecArea.X - Matching_Results[0].Center.X);
                ROI.PidutialY = (float)(ROI.SpecArea.Y - Matching_Results[0].Center.Y);
                ROI.SpecArea = new Rectangle((int)(Matching_Results[0].Center.X + ROI.PidutialX), (int)(Matching_Results[0].Center.Y + ROI.PidutialY), ROI.SpecArea.Width, ROI.SpecArea.Height);
            }
        }

        private void SetROI(List<CResultMatching> Matching_Results, ref List<CSpecDistance> ROIs)
        {
            foreach (var ROI in ROIs)
            {
                ROI.PidutialX_Top = (float)(ROI.FitCentersTop.X - Matching_Results[0].Center.X);
                ROI.PidutialY_Top = (float)(ROI.FitCentersTop.Y - Matching_Results[0].Center.Y);

                ROI.PidutialX_Btm = (float)(ROI.FitCentersBtm.X - Matching_Results[0].Center.X);
                ROI.PidutialY_Btm = (float)(ROI.FitCentersBtm.Y - Matching_Results[0].Center.Y);

                ROI.FitCentersTop = new PointF((int)(Matching_Results[0].Center.X + ROI.PidutialX_Top), (int)(Matching_Results[0].Center.Y + ROI.PidutialY_Top));
                ROI.FitCentersBtm = new PointF((int)(Matching_Results[0].Center.X + ROI.PidutialX_Btm), (int)(Matching_Results[0].Center.Y + ROI.PidutialY_Btm));
            }
        }

        private string GetDisplayTitle = "ROI";

        private void Onrdo_CheckedChanged(object sender, EventArgs e)
        {
            RJRadioButton rdoButton = (RJRadioButton)sender;

            if (rdoButton.Checked)
            {
                switch (rdoButton.Text.ToUpper())
                {
                    case "INSPECTION":
                        GetDisplayTitle = "Inspection";
                        break;
                    case "ROI":
                        GetDisplayTitle = "ROI";
                        break;
                    case "THRESHOLD":
                        GetDisplayTitle = "Threshold";
                        break;
                }
            }

            int Index = CDisplayManager.FindIndex(GetDisplayTitle);
            if (CDisplayManager.Displays.Count > Index) { CDisplayManager.Displays[Index].Activate(); }
        }

        private void btnRoiTest_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < ParamList[TeachingNo].ROIs.Count; i++)
            {
                if (!ParamList[TeachingNo].ROIs[i].Relative_Coordinates)
                {
                    CCommon.ShowMessageBox("Check", "ROI 상대좌표 설정 후 검사 가능합니다.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
                    return;
                }
            }

            switch (CUtil.ParseEnum<DEFINE.PROCESS_TYPES>(cbProcess.SelectedItem.ToString()))
            {
               // case DEFINE.PROCESS_TYPES.Bolt_tightened:
               //     Global.SeqVision.RunInspection(new CGrabBuffer(CDisplayManager.ImageSrc.Clone(), TeachingNo, PROCESS_TYPES.Bolt_tightened,
               //PIN_INSPECTION_TYPES.Pin_SpecArea, Global.Data.ParamList_Bolt, Global.Data.ParamList_Pin, Global.Data.ParamList_Connector, Global.Data.ParamList_Thermal));
               //     break;
               // case DEFINE.PROCESS_TYPES.Pin_Inspection:
               //     Global.SeqVision.RunInspection(new CGrabBuffer(CDisplayManager.ImageSrc.Clone(), TeachingNo, PROCESS_TYPES.Pin_Inspection,
               //PIN_INSPECTION_TYPES.Pin_SpecArea, Global.Data.ParamList_Bolt, Global.Data.ParamList_Pin, Global.Data.ParamList_Connector, Global.Data.ParamList_Thermal));
               //     break;
               // case DEFINE.PROCESS_TYPES.ThermalGrease_Inspection:
               //     Global.SeqVision.RunInspection(new CGrabBuffer(CDisplayManager.ImageSrc.Clone(), TeachingNo, PROCESS_TYPES.ThermalGrease_Inspection,
               //     PIN_INSPECTION_TYPES.Pin_SpecArea, Global.Data.ParamList_Bolt, Global.Data.ParamList_Pin, Global.Data.ParamList_Connector
               //     , Global.Data.ParamList_Thermal));
               //     break;
            }


        }

        private void ResultJudgeImage(Graphics g, Bitmap ImageDisplay, DEFINE.RESULT Result)
        {
            Rectangle rtFullScreen = new Rectangle(0, 0, ImageDisplay.Width, ImageDisplay.Height);
            if (Result == DEFINE.RESULT.OK)
            {
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Aquamarine, 30), rtFullScreen);
                g.DrawString(string.Format("OK"), new Font("Arial", 250, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Aquamarine), ImageDisplay.Width - 700, ImageDisplay.Height - 500);
            }
            else
            {
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 30), rtFullScreen);
                g.DrawString(string.Format("NG"), new Font("Arial", 250, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Red), ImageDisplay.Width - 700, ImageDisplay.Height - 500);
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (CCommon.ShowdialogMessageBox("Save", "Spec Area 상대좌표로 설정 하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
            {
                var Results = RunMatching();
                if (Results.Count != 0) { SetROI(Results, ref ParamList[TeachingNo].SpecAreas); }
                for (int i = 0; i < ParamList[TeachingNo].SpecAreas.Count; i++)
                {
                    ParamList[TeachingNo].SpecAreas[i].Relative_Coordinates = true;
                }
            }
        }

        private void btnSpecAreaTest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ParamList[TeachingNo].SpecAreas.Count; i++)
            {
                if (!ParamList[TeachingNo].SpecAreas[i].Relative_Coordinates)
                {
                    CCommon.ShowMessageBox("Check", "Spec Area 상대좌표 설정 후 검사 가능합니다.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
                    return;
                }
            }
            //Global.SeqVision.RunInspection(new CGrabBuffer(CDisplayManager.ImageSrc.Clone(), TeachingNo, PROCESS_TYPES.Pin_Inspection,
            //           PIN_INSPECTION_TYPES.Pin_SpecArea, Global.Data.ParamList_Bolt, Global.Data.ParamList_Pin, Global.Data.ParamList_Connector
            //           , Global.Data.ParamList_Thermal));
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            if (CCommon.ShowdialogMessageBox("Save", "Spec Area 설정을 저장 하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
            {
                Save();
            }
        }

        private void btnPinDistanceCalibration_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CCommon.ShowdialogMessageBox("Calibration", "Calibration 하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit)) { return; }

                Stopwatch stopwatch = Stopwatch.StartNew();
                DEFINE.RESULT Result = DEFINE.RESULT.OK;
                List<CResultMatching> Matching_Results = new List<CResultMatching>();
                using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap ImageDisplay = Lib.Common.CImageConverter.ToBitmap(ImageCVSource).Clone(new Rectangle(0, 0, ImageCVSource.Width, ImageCVSource.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Graphics g = Graphics.FromImage(ImageDisplay);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    List<Rectangle> calculRoi = new List<Rectangle>();

                    List<CResultBlob> pin_Results = new List<CResultBlob>();
                    Matching_Results = RunMatching(g, ImageCVSource, ParamList, TeachingNo, ref calculRoi);
                    foreach (var Param in ParamList)
                    {
                        for (int i = 0; i < Param.ROIs.Count; i++)
                        {
                            CRectangle cRectangle = new CRectangle(Param.ROIs[i]);
                            Rectangle rectangle = new Rectangle();
                            if (calculRoi.Count != 0) { rectangle = calculRoi[i]; }
                            else { rectangle = cRectangle.Roi; }

                            if (!cRectangle.USE_ROI) { continue; }

                            switch (cRectangle.Algorithm)
                            {
                                case DEFINE.ALGORITHM.BLOB:
                                    CPropertyBlob cPropertyBlob = Param.Blob.DeepCopy();
                                    Stopwatch stopwatch1 = Stopwatch.StartNew();

                                    //CVBlob cIVT_CVBlob = new CVBlob();
                                    //cIVT_CVBlob.SetProperty(cPropertyBlob);
                                    //cIVT_CVBlob.property.CvROI = CConverter.RectToCVRect(rectangle);
                                    //cIVT_CVBlob.property.ADAPTIVE_THRESHOLD_ALGORITHM = AdaptiveThresholdTypes.GaussianC;
                                    //cIVT_CVBlob.property.USE_BITWISENOT = cRectangle.USE_BITWISENOT;
                                    ////cIVT_CVBlob.Property.BLACK_THRESHOLD = cRectangle.THRESHOLD;
                                    //cIVT_CVBlob.property.MIN_AREA = cRectangle.MIN_AREA;
                                    //cIVT_CVBlob.property.MAX_AREA = cRectangle.MAX_AREA;
                                    //cIVT_CVBlob.property.USE_ROI = true;
                                    //cIVT_CVBlob.property.USE_THRESHOLD = false;
                                    //cIVT_CVBlob.property.USE_ADAPTIVE_THRESHOLD = true;
                                    //cIVT_CVBlob.SetSourceImage(ImageCVSource);
                                    //cIVT_CVBlob.Run();

                                    int itemcount = 1;
                                    //foreach (var item in cIVT_CVBlob.results)
                                    //{
                                    //    pin_Results.Add(item);
                                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                                    //    g.DrawString($"{(i + 1)}-{itemcount}", new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 20, (int)item.Bounding.Y - 20);
                                    //    g.DrawString(item.Area.ToString(), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                                    //    itemcount++;
                                    //}

                                    //if (cIVT_CVBlob.results.Count == 0 || cIVT_CVBlob.results.Count != cRectangle.InspCount)
                                    //{
                                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cIVT_CVBlob.property.CvROI));
                                    //    g.DrawString((i + 1).ToString(), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cIVT_CVBlob.property.CvROI.X - 20, (int)cIVT_CVBlob.property.CvROI.Y - 20);
                                    //    Result = DEFINE.RESULT.NG;
                                    //}
                                    break;
                                case DEFINE.ALGORITHM.MATCING:
                                    CPropertyMatching cProperty = (CPropertyMatching)Param.Matching.DeepCopy();

                                    int Hmc_Path_Index = cRectangle.PATTERN_PATH.IndexOf("PATTERN\\") + 8;
                                    int total_Length = cRectangle.PATTERN_PATH.Length;
                                    string splite_Path = cRectangle.PATTERN_PATH.Substring(Hmc_Path_Index, total_Length - Hmc_Path_Index);
                                    string Pattern_Path = Application.StartupPath + $"\\RECIPE\\{CGlobal.Inst.Recipe.Name}\\PATTERN\\{splite_Path}";

                                    CVMatching Matching = new CVMatching();
                                    if (System.IO.File.Exists(Pattern_Path))
                                    {
                                        Matching.SetProperty(cProperty);
                                        Matching.property.CvROI = CConverter.RectToCVRect(rectangle);
                                        Matching.property.SCORE_MIN = cRectangle.SCORE;
                                        Matching.SetTemplateImage(Cv2.ImRead(Pattern_Path));
                                        Matching.SetSourceImage(ImageCVSource);
                                        Matching.ImagePyramidsSingleRun();
                                    }

                                    foreach (var item in Matching.results)
                                    {
                                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 5), new Rectangle((int)item.Bounding.X, (int)item.Bounding.Y, (int)item.Bounding.Width, (int)item.Bounding.Height));
                                        g.DrawString((i + 1).ToString(), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 20, (int)item.Bounding.Y - 20);
                                        g.DrawString(item.Score.ToString("F3"), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Center.X, (int)item.Center.Y);
                                    }

                                    if (Matching.results.Count == 0)
                                    {
                                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(Matching.property.CvROI));
                                        g.DrawString((i + 1).ToString(), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)Matching.property.CvROI.X - 20, (int)Matching.property.CvROI.Y - 20);
                                        //Result = DEFINE.RESULT.NG;
                                    }
                                    break;
                            }
                        }
                    }

                    List<OpenCvSharp.Point2d> centersTop = new List<Point2d>();
                    List<System.Drawing.PointF> fitCentersTop = new List<System.Drawing.PointF>();

                    List<OpenCvSharp.Point2d> centersBtm = new List<Point2d>();
                    List<System.Drawing.PointF> fitCentersBtm = new List<System.Drawing.PointF>();

                    // 센터 좌표 모으기
                    for (int i = 0; i < pin_Results.Count; i++)
                    {
                        if (i < pin_Results.Count / 2)
                        {
                            centersTop.Add(pin_Results[i].Center);
                        }
                        else
                        {
                            centersBtm.Add(pin_Results[i].Center);
                        }
                    }

                    for (int h = 0; h < centersTop.Count - 1; h++)
                    {
                        OpenCvSharp.Point ptS = new OpenCvSharp.Point(centersTop[h].X, centersTop[h].Y);
                        OpenCvSharp.Point ptE = new OpenCvSharp.Point(centersTop[h + 1].X, centersTop[h + 1].Y);

                        double Distance = ptS.DistanceTo(ptE) * DEFINE.PIXEL_RESOLUTION_MM;
                        g.DrawString(Distance.ToString("F1") + "mm", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), ptS.X + 5, ptS.Y);

                        g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Lime, 1f), CConverter.CVPointToPoint(ptS), CConverter.CVPointToPoint(ptE));
                    }

                    for (int h = 0; h < centersBtm.Count - 1; h++)
                    {
                        OpenCvSharp.Point ptS = new OpenCvSharp.Point(centersBtm[h].X, centersBtm[h].Y);
                        OpenCvSharp.Point ptE = new OpenCvSharp.Point(centersBtm[h + 1].X, centersBtm[h + 1].Y);

                        double Distance = ptS.DistanceTo(ptE) * DEFINE.PIXEL_RESOLUTION_MM;
                        g.DrawString(Distance.ToString("F1") + "mm", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), ptS.X + 5, ptS.Y);

                        g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Lime, 1f), CConverter.CVPointToPoint(ptS), CConverter.CVPointToPoint(ptE));
                    }

                    foreach (var point in centersBtm) { CDrawBitmap.DrawPoint(g, point); }
                    foreach (var point in centersTop) { CDrawBitmap.DrawPoint(g, point); }

                    // 라인 피팅
                    //fitCentersTop = CVision.RansacLineFittingDrawF(centersTop, out double A, out double B);
                    //fitCentersBtm = CVision.RansacLineFittingDrawF(centersBtm, out double C, out double D);

                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Black, 1f), fitCentersTop[0], fitCentersTop[fitCentersTop.Count - 1]);
                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Black, 1f), fitCentersBtm[0], fitCentersBtm[fitCentersBtm.Count - 1]);

                    ParamList[TeachingNo].SpecDistance.Clear();
                    for (int i = 0; i < fitCentersTop.Count; i++)
                    {
                        CSpecDistance cSpecDistance = new CSpecDistance();
                        cSpecDistance.Index = i + 1;
                        cSpecDistance.FitCentersTop = fitCentersTop[i];
                        cSpecDistance.FitCentersBtm = fitCentersBtm[i];
                        ParamList[TeachingNo].SpecDistance.Add(cSpecDistance);
                    }

                    ParamList[TeachingNo].SaveConfig(Global.Recipe.Name);

                    ResultJudgeImage(g, ImageDisplay, Result);
                    CDisplayManager.CreateLayerDisplay(ImageDisplay, "Calibration", true);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnPinDistanceSetPos_Click(object sender, EventArgs e)
        {
            if (CCommon.ShowdialogMessageBox("Save", "Distance 상대좌표로 설정 하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
            {
                var Results = RunMatching();
                if (Results.Count != 0) { SetROI(Results, ref ParamList[TeachingNo].SpecDistance); }
                for (int i = 0; i < ParamList[TeachingNo].SpecDistance.Count; i++)
                {
                    ParamList[TeachingNo].SpecDistance[i].Relative_Coordinates = true;
                }
            }
        }

        private void btnPinDistanceTest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ParamList[TeachingNo].SpecDistance.Count; i++)
            {
                if (!ParamList[TeachingNo].SpecDistance[i].Relative_Coordinates)
                {
                    CCommon.ShowMessageBox("Check", "Spec Distance 상대좌표 설정 후 검사 가능합니다.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
                    return;
                }
            }
            //Global.SeqVision.RunInspection(new CGrabBuffer(CDisplayManager.ImageSrc.Clone(), TeachingNo, PROCESS_TYPES.Pin_Inspection,
            //           PIN_INSPECTION_TYPES.Pin_Distance, Global.Data.ParamList_Bolt, Global.Data.ParamList_Pin, Global.Data.ParamList_Connector
            //           , Global.Data.ParamList_Thermal));
        }

        private void OnPara_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RJCodeUI_M1.RJControls.RJRadioButton rdoButton = (RJCodeUI_M1.RJControls.RJRadioButton)sender;

                if (rdoButton.Checked)
                {
                    switch (rdoButton.Text)
                    {
                        case "Matching":
                            propertyGrid_Parameter.SelectedObject = ParamList[TeachingNo].Matching;
                            break;
                        case "Blob":
                            propertyGrid_Parameter.SelectedObject = ParamList[TeachingNo].Blob;
                            break;
                        case "Mean_Common":
                            propertyGrid_Parameter.SelectedObject = ParamList[TeachingNo].Mean;
                            break;
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void timePixelData_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
