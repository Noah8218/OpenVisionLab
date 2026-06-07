using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using OpenCvSharp;
using RJCodeUI_M1.RJForms;
using Lib.Common;

namespace OpenVisionLab
{
    public sealed class ImageCanvasStatusChangedEventArgs : EventArgs
    {
        public ImageCanvasStatusChangedEventArgs(PointF position, int grayValue, Color pixelColor)
        {
            Position = position;
            GrayValue = grayValue;
            PixelColor = pixelColor;
        }

        public PointF Position { get; }
        public int GrayValue { get; }
        public Color PixelColor { get; }
    }

    public partial class FormImageEditView : RJChildForm
    {
        private CPropertyImageView PropertyImageView = new CPropertyImageView("IMAGE_VIEW");

        private Mat ImageSource = new Mat();        
        public Mat ImageProcess = new Mat();
        private Bitmap SourceBitmap = new Bitmap(10, 10);
        public Rect SelectedRegion = new Rect();
        public List<Rect> SelectedRegions = new List<Rect>();
        private global::OpenVisionLab.ImageCanvas.Views.RoiImageCanvasView ImageCanvasView;
        private global::OpenVisionLab.ImageCanvas.ViewModels.RoiImageCanvasViewModel ImageCanvasViewModel;
        private Bitmap PendingImageCanvasImage;
        private PatternMatchPreviewView PatternPreviewView;
        private Rectangle ImageCanvasSelectedRegion = Rectangle.Empty;
        private readonly Dictionary<string, Rectangle> ImageCanvasSelectedRegions = new Dictionary<string, Rectangle>();
        private readonly List<Rectangle> PendingImageCanvasRois = new List<Rectangle>();
        private Panel commandPanel;
        private Button btnApplySelection;
        private Button btnCancelSelection;
        private string pendingPatternPreviewPath = string.Empty;

        private string Mode = "";

        public event EventHandler<ImageCanvasStatusChangedEventArgs> ImageCanvasStatusChanged = delegate { };

        public FormImageEditView(Bitmap image, string strMode = "Drag")
        {
            InitializeComponent();
            ApplyImageEditViewLayout(strMode);
            SetSourceImage(image);
            InitializeImageCanvas(image, strMode);

            try
            {
                Mode = strMode;
                this.KeyPreview = true;
                this.TopLevel = true;
                this.TopMost = true;

                propertygrid_Parameter.SelectedObject = PropertyImageView;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                this.Close();
            }
        }

        public FormImageEditView(Bitmap image, Rectangle ROI,  string strMode = "")
        {
            InitializeComponent();
            ApplyImageEditViewLayout(strMode);
            SetSourceImage(image);
            InitializeImageCanvas(image, strMode);

            try
            {
                Mode = strMode;
                AddPendingImageCanvasRoi(ROI);

                this.KeyPreview = true;
                this.TopLevel = true;
                this.TopMost = true;
                

                propertygrid_Parameter.SelectedObject = PropertyImageView;
            }
            catch ( Exception Desc )
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                this.Close( );
            }            
        }

        public FormImageEditView(Bitmap image, List<Rect> ROI, string strMode = "")
        {
            InitializeComponent();
            ApplyImageEditViewLayout(strMode);
            SetSourceImage(image);
            InitializeImageCanvas(image, strMode);

            try
            {
                Mode = strMode;
                foreach (Rect roi in ROI)
                {
                    AddPendingImageCanvasRoi(new Rectangle(roi.X, roi.Y, roi.Width, roi.Height));
                }

                this.KeyPreview = true;
                this.TopLevel = true;
                this.TopMost = true;

                propertygrid_Parameter.SelectedObject = PropertyImageView;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {Desc.Message}");
                this.Close();
            }
        }

        private bool IsImageCanvasActive => ImageCanvasViewModel != null && imageCanvasHost != null && imageCanvasHost.Visible;

        private void SetSourceImage(Bitmap image)
        {
            SourceBitmap?.Dispose();
            ImageSource?.Dispose();

            SourceBitmap = image == null ? new Bitmap(10, 10) : (Bitmap)image.Clone();
            ImageSource = Lib.Common.CImageConverter.ToMat(SourceBitmap);
        }

        private void ApplyImageEditViewLayout(string mode)
        {
            CollapseLegacyToolbar();
            ConfigureSidePanel(mode);
            ConfigureCommandPanel(mode);
        }

        private void CollapseLegacyToolbar()
        {
            if (splitContainer2 != null)
            {
                splitContainer2.Panel1MinSize = 0;
                splitContainer2.Panel1Collapsed = true;
                splitContainer2.SplitterWidth = 1;
            }

        }

        private void ConfigureSidePanel(string mode)
        {
            Color panelBackColor = Color.FromArgb(238, 243, 247);
            splitContainer1.Panel2.BackColor = panelBackColor;

            metroTile1.Visible = false;
            metroTile11.Visible = false;
            lbPosition.Visible = false;
            lbGV.Visible = false;
            propertygrid_Parameter.Visible = false;

            bool showPatternPreview = IsPatternPreviewMode(mode);
            splitContainer1.Panel2MinSize = 0;
            splitContainer1.Panel2Collapsed = !showPatternPreview;

            if (!showPatternPreview) { return; }

            PatternPreviewView = new PatternMatchPreviewView
            {
                Dock = DockStyle.Fill
            };

            splitContainer1.Panel2.Controls.Add(PatternPreviewView);
            PatternPreviewView.BringToFront();
            LoadPatternPreviewImage(pendingPatternPreviewPath);
        }

        private static bool IsPatternPreviewMode(string mode) => string.Equals(mode, "TRAIN", StringComparison.OrdinalIgnoreCase);

        public void LoadPatternPreviewImage(string imagePath)
        {
            pendingPatternPreviewPath = imagePath ?? string.Empty;
            if (string.IsNullOrWhiteSpace(pendingPatternPreviewPath) || PatternPreviewView == null) { return; }
            if (!File.Exists(pendingPatternPreviewPath)) { return; }

            using (Bitmap fileImage = new Bitmap(pendingPatternPreviewPath))
            {
                PatternPreviewView.SetPreview((Bitmap)fileImage.Clone());
            }
        }

        private void ConfigureCommandPanel(string mode)
        {
            if (pnlClientArea == null) { return; }

            if (commandPanel == null)
            {
                commandPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 48,
                    BackColor = Color.FromArgb(28, 32, 38),
                    Padding = new Padding(10, 7, 12, 7)
                };

                btnCancelSelection = CreateCommandButton("Cancel", Color.FromArgb(74, 82, 94), Color.White);
                btnCancelSelection.Click += (sender, e) => CancelSelection();

                btnApplySelection = CreateCommandButton(GetApplyButtonText(mode), Color.FromArgb(79, 94, 220), Color.White);
                btnApplySelection.Click += btnCut_Click;

                commandPanel.Controls.Add(btnCancelSelection);
                commandPanel.Controls.Add(btnApplySelection);
                pnlClientArea.Controls.Add(commandPanel);
            }

            btnApplySelection.Text = GetApplyButtonText(mode);
            splitContainer1.Dock = DockStyle.Fill;
            commandPanel.BringToFront();
        }

        private static Button CreateCommandButton(string text, Color backColor, Color foreColor)
        {
            return new Button
            {
                Dock = DockStyle.Right,
                Width = 108,
                Margin = new Padding(6, 0, 0, 0),
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = foreColor,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point),
                UseVisualStyleBackColor = false
            };
        }

        private static string GetApplyButtonText(string mode)
        {
            return string.Equals(mode, "TRAIN", StringComparison.OrdinalIgnoreCase) ? "Use Pattern" : "OK";
        }

        private void CancelSelection()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }


        #region Display
        private void ZoomInImage() { }
        private void ZoomOutImage() { }
        private void ZoomFitImage() { }
        private void FormImageView_FormClosing(object sender, FormClosingEventArgs e) 
        { 
            if (ImageCanvasViewModel != null)
            {
                ImageCanvasViewModel.PropertyChanged -= ImageCanvasViewModel_PropertyChanged;
            }

            ImageSource.Dispose();
            SourceBitmap?.Dispose();
            PendingImageCanvasImage?.Dispose();
            PatternPreviewView?.ClearPreview();
        }
        private void btnZoomOut_Click(object sender, EventArgs e) { ZoomOutImage(); }
        private void btnZoomIn_Click(object sender, EventArgs e) { ZoomInImage(); }
        private void btnFit_Click(object sender, EventArgs e) { ZoomFitImage(); }       
        #endregion

        private void InitializeImageCanvas(Bitmap image, string mode)
        {
            if (imageCanvasHost == null) { return; }

            try
            {
                ImageCanvasViewModel = new global::OpenVisionLab.ImageCanvas.ViewModels.RoiImageCanvasViewModel("ImageEditView");
                ImageCanvasViewModel.RoiAdded += ImageCanvas_RoiChanged;
                ImageCanvasViewModel.RoiMouseUp += ImageCanvas_RoiChanged;
                ImageCanvasViewModel.RoiEditingCompleted += ImageCanvas_RoiChanged;
                ImageCanvasViewModel.PropertyChanged += ImageCanvasViewModel_PropertyChanged;
                ImageCanvasViewModel.IsTeachingMode = IsImageCanvasDrawingMode(mode);
                ImageCanvasViewModel.ShowGroupNames = false;
                ImageCanvasViewModel.ShowRoiItemNames = false;
                ImageCanvasViewModel.ShowGroupBounds = false;
                ImageCanvasViewModel.ReplaceExistingRoiOnDraw = IsImageCanvasDrawingMode(mode) && !string.Equals(mode, "MULTI_ROI", StringComparison.OrdinalIgnoreCase);

                ImageCanvasView = new global::OpenVisionLab.ImageCanvas.Views.RoiImageCanvasView
                {
                    DataContext = ImageCanvasViewModel
                };

                imageCanvasHost.Child = ImageCanvasView;
                imageCanvasHost.Visible = true;
                imageCanvasHost.BringToFront();

                PendingImageCanvasImage?.Dispose();
                PendingImageCanvasImage = image == null ? null : (Bitmap)image.Clone();

                Shown -= FormImageEditView_Shown;
                Shown += FormImageEditView_Shown;
            }
            catch (Exception desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {desc.Message}");
                imageCanvasHost.Visible = false;
                Close();
            }
        }

        private static bool IsImageCanvasDrawingMode(string mode)
        {
            string editMode = mode?.ToUpper() ?? string.Empty;
            return editMode == "ROI" || editMode == "TRAIN" || editMode == "MULTI_ROI";
        }

        private void FormImageEditView_Shown(object sender, EventArgs e)
        {
            LoadPendingImageCanvasImage();
        }

        private void LoadPendingImageCanvasImage()
        {
            if (ImageCanvasViewModel == null || PendingImageCanvasImage == null) { return; }

            try
            {
                using (Mat mat = Lib.Common.CImageConverter.ToMat(PendingImageCanvasImage))
                {
                    ImageCanvasViewModel.LoadImage(mat, "Source");
                }

                ApplyPendingImageCanvasRois();
            }
            catch (Exception desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {desc.Message}");
            }
            finally
            {
                PendingImageCanvasImage.Dispose();
                PendingImageCanvasImage = null;
            }
        }

        private void AddPendingImageCanvasRoi(Rectangle roi)
        {
            if (roi.IsEmpty || roi.Width <= 0 || roi.Height <= 0) { return; }

            PendingImageCanvasRois.Add(roi);
            ImageCanvasSelectedRegion = roi;
        }

        private void ApplyPendingImageCanvasRois()
        {
            if (ImageCanvasViewModel == null || PendingImageCanvasRois.Count == 0) { return; }

            foreach (Rectangle roi in PendingImageCanvasRois)
            {
                ImageCanvasViewModel.AddInitialRoi(roi);
            }

            PendingImageCanvasRois.Clear();
        }

        private void ImageCanvas_RoiChanged(object sender, global::OpenVisionLab.ImageCanvas.Model.RoiChangedEventArgs e)
        {
            if (e?.RoiRect == null || e.RoiRect.IsEmpty()) { return; }

            Rectangle rect = ToImageRectangle(e.RoiRect);
            if (rect.IsEmpty) { return; }

            ImageCanvasSelectedRegion = rect;
            if (Mode.Equals("TRAIN", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTrainPreview(rect);
            }

            if (Mode.Equals("MULTI_ROI", StringComparison.OrdinalIgnoreCase))
            {
                string uniqueId = string.IsNullOrWhiteSpace(e.RoiRect.UniqueId) ? Guid.NewGuid().ToString() : e.RoiRect.UniqueId;
                ImageCanvasSelectedRegions[uniqueId] = rect;
            }
        }

        private void ImageCanvasViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ImageCanvasViewModel == null) { return; }
            if (e.PropertyName != "RobotPos" && e.PropertyName != "GrayValue" && e.PropertyName != "PixelColor") { return; }

            PointF position = ImageCanvasViewModel.RobotPos;
            int grayValue = ImageCanvasViewModel.GrayValue;
            Color pixelColor = ImageCanvasViewModel.PixelColor;

            lbPosition.Text = $"{position.X:F0},{position.Y:F0}";
            lbGV.Text = grayValue.ToString();
            ImageCanvasStatusChanged(this, new ImageCanvasStatusChangedEventArgs(position, grayValue, pixelColor));
        }

        private Rectangle ToImageRectangle(global::OpenVisionLab.ImageCanvas.CanvasShapes.CanvasRect<float> rect)
        {
            if (rect == null || rect.IsEmpty()) { return Rectangle.Empty; }

            int x = (int)Math.Round(rect.Left);
            int y = SourceBitmap == null
                ? (int)Math.Round(rect.Bottom)
                : SourceBitmap.Height - (int)Math.Round(rect.Top);
            int width = Math.Max(0, (int)Math.Round(rect.Width));
            int height = Math.Max(0, (int)Math.Round(rect.Height));

            return ClampToSourceBounds(new Rectangle(x, y, width, height));
        }

        private void UpdateTrainPreview(Rectangle roi)
        {
            if (SourceBitmap == null || roi.IsEmpty) { return; }

            Rectangle cropRect = ClampToSourceBounds(roi);
            if (cropRect.Width <= 5 || cropRect.Height <= 5) { return; }

            Bitmap imageTemplate = Lib.Common.CBitmapProcessing.CropAtRect(SourceBitmap, cropRect).Result;
            if (PatternPreviewView != null)
            {
                PatternPreviewView.SetPreview(imageTemplate);
                return;
            }

            imageTemplate.Dispose();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            switch((Keys)e.KeyValue)
            {
                case Keys.Escape:
                    CancelSelection();
                    break;
                case Keys.Enter:
                    btnCut_Click(null, null);
                    break;
            }
        }

        private void btnSelectionMode_Click(object sender, EventArgs e)
        {
            ImageCanvasSelectedRegion = GetDefaultSourceRoi();
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            using(Mat ImageSrc = ImageSource.Clone())
            {
                switch (Mode)
                {
                    case "ROI":
                        SelectedRegion = CConverter.RectToCVRect(GetSelectedRoi());
                        if (SelectedRegion.X < 0)
                        {
                            SelectedRegion.X = 0;
                            SelectedRegion.Width = ImageSrc.Width;
                        }
                        if (SelectedRegion.Y < 0)
                        {
                            SelectedRegion.Y = 0;
                            SelectedRegion.Height = ImageSrc.Height;
                        }
                        break;
                    case "TRAIN":
                        SelectedRegion = CConverter.RectToCVRect(GetSelectedRoi());
                        if (SelectedRegion.X < 0)
                        {
                            SelectedRegion.X = 0;
                            SelectedRegion.Width = ImageSrc.Width;
                        }
                        if (SelectedRegion.Y < 0)
                        {
                            SelectedRegion.Y = 0;
                            SelectedRegion.Height = ImageSrc.Height;
                        }
                        break;                                           
                    case "MULTI_ROI":
                        if (ImageCanvasSelectedRegions.Count > 0)
                        {
                            foreach (Rectangle ROI in ImageCanvasSelectedRegions.Values)
                            {
                                Rectangle clampedRoi = ClampToSourceBounds(ROI);
                                if (clampedRoi.IsEmpty) { continue; }

                                Rect r = CConverter.RectangleToRect(clampedRoi);
                                SelectedRegions.Add(r);
                            }
                            break;
                        }
                        
                        break;
                }

                

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private Rectangle GetSelectedRoi()
        {
            if (!ImageCanvasSelectedRegion.IsEmpty)
            {
                return ClampToSourceBounds(ImageCanvasSelectedRegion);
            }

            return Rectangle.Empty;
        }

        private Rectangle GetAnalysisRoi()
        {
            if (!ImageCanvasSelectedRegion.IsEmpty)
            {
                return ClampToSourceBounds(ImageCanvasSelectedRegion);
            }

            return Rectangle.Empty;
        }

        private Rectangle GetDefaultSourceRoi()
        {
            if (SourceBitmap == null) { return Rectangle.Empty; }

            return new Rectangle(
                SourceBitmap.Width / 2,
                SourceBitmap.Height / 2,
                Math.Max(1, SourceBitmap.Width / 8),
                Math.Max(1, SourceBitmap.Height / 8));
        }

        private Rectangle ClampToSourceBounds(Rectangle roi)
        {
            if (SourceBitmap == null || SourceBitmap.Width <= 0 || SourceBitmap.Height <= 0) { return Rectangle.Empty; }

            Rectangle imageBounds = new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height);
            return Rectangle.Intersect(imageBounds, roi);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            ImageCanvasSelectedRegion = Rectangle.Empty;
        }

        private void btnMean_Click(object sender, EventArgs e)
        {
            Rectangle roi = GetAnalysisRoi();
            if (roi.IsEmpty) { return; }

            using (Mat ImageSrc = ImageSource.Clone())
            {
                Rect rtSubmat = CConverter.RectangleToRect(roi);

                double dMean = Cv2.Mean(ImageSrc.SubMat(rtSubmat)).Val0;
                btnMean.Text = $"Mean : {dMean.ToString("F2")}";
            }
        }

        private void btnMatrixView_Click(object sender, EventArgs e)
        {
            Rectangle roi = GetAnalysisRoi();
            if (roi.IsEmpty) { return; }

            Rect rtSubmatOrg = CConverter.RectangleToRect(roi);

            using (Mat ImageSrc = ImageSource.Clone())
            using (Mat ImageSub = ImageSrc.SubMat(rtSubmatOrg).Clone())
            {
                Bitmap imageDisplay = Lib.Common.CImageConverter.ToBitmap(ImageSub);

                double dMean = Cv2.Mean(ImageSub).Val0-10;

                using (Graphics g = Graphics.FromImage(imageDisplay))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    int nRow = 12;
                    int nCol = 12;

                    int nW = imageDisplay.Width;
                    int nH = imageDisplay.Height;

                    int nSW = nW / nCol;
                    int nSH = nH / nRow;

                    for (int nx = 0; nx < nCol; nx++)
                    {
                        for (int ny = 0; ny < nRow; ny++)
                        {
                            Rect rtSubmat = new Rect((nSW * nx), (nSH * ny), nSW, nSH);

                            double dPartialMean = Cv2.Mean(ImageSub.SubMat(rtSubmat)).Val0;


                            Rectangle rtDrawing = new Rectangle((nSW * nx), (nSH * ny), nSW, nSH);
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Silver, 1), rtDrawing);

                            if (dPartialMean < dMean)
                            {
                                g.DrawString(((int)(dPartialMean)).ToString(), new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(System.Drawing.Color.Red), new PointF((nSW * nx), (nSH * ny)));
                            }
                            else
                            {
                                g.DrawString(((int)(dPartialMean)).ToString(), new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(System.Drawing.Color.Lime), new PointF((nSW * nx), (nSH * ny)));
                            }

                        }
                    }
                }

                imageDisplay.Dispose();
            }
        }
    }
}
