using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using Lib.Common;
using Lib.Line;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using Manina.Windows.Forms;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using SortOrder = Manina.Windows.Forms.SortOrder;
using View = Manina.Windows.Forms.View;

namespace OpenVisionLab
{
    public partial class FormContour : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormContour()
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

        private void Form_Load(object sender, EventArgs e)
        {
            dgvDefect.DataSource = new CDefectList_Result().GetBlobList(new List<CResultBlob>());
            dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDefect.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            imageListView1.AllowDuplicateFileNames = true;
            imageListView1.SetRenderer(new Manina.Windows.Forms.ImageListViewRenderers.DefaultRenderer());
            imageListView1.SortColumn = 0;
            imageListView1.SortOrder = SortOrder.AscendingNatural;

            Assembly assembly = Assembly.GetAssembly(typeof(ImageListView));

            int i = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType == typeof(ImageListView.ImageListViewRenderer))
                {
                    renderertoolStripComboBox.Items.Add(new RendererComboBoxItem(type));
                    if (type.Name == "DefaultRenderer")
                        renderertoolStripComboBox.SelectedIndex = i;
                    i++;
                }
            }
            // Find and add custom colors
            Type colorType = typeof(ImageListViewColor);
            i = 0;
            foreach (PropertyInfo field in colorType.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                colorToolStripComboBox.Items.Add(new ColorComboBoxItem(field));
                if (field.Name == "Default")
                    colorToolStripComboBox.SelectedIndex = i;
                i++;
            }

            string cacheDir = Path.Combine(
                Path.GetDirectoryName(new Uri(assembly.GetName().CodeBase).LocalPath),
                "Cache"
                );
            imageListView1.Columns.Add(ColumnType.Name);
            imageListView1.Columns.Add(ColumnType.Dimensions);
            imageListView1.Columns.Add(ColumnType.FileSize);
            imageListView1.Columns.Add(ColumnType.FolderName);
            imageListView1.Columns.Add(ColumnType.DateModified);
            imageListView1.Columns.Add(ColumnType.FileType);
            var col = new ImageListView.ImageListViewColumnHeader(ColumnType.Custom, "random", "Random");
            col.Comparer = new RandomColumnComparer();
            imageListView1.Columns.Add(col);
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
                var rect = new Rectangle(int.Parse(dgv.Rows[rowIndex].Cells[5].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[6].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[7].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[8].Value.ToString()));


                int Index = CDisplayManager.FindIndex("Contour");

                double Factor_W = 0.95;
                double Factor_H = 2;

                int W = (int)(CDisplayManager.Displays[Index].viewer._Ib.Width / Factor_W);
                int H = (int)(CDisplayManager.Displays[Index].viewer._Ib.Height / Factor_H);

                Rectangle rt = CDisplayManager.Displays[Index].viewer._Ib.RectangleToScreen(rect);
                CDisplayManager.Displays[Index].viewer._Ib.ZoomToRegion(rt.X, rt.Y, (int)(W), (int)(H));
                CDisplayManager.Displays[Index].viewer._Ib.ScrollTo(rt.X, rt.Y, W, H);
                CDisplayManager.Displays[Index].Activate();

                //trbThreshold_Scroll(null, null);

                //Rectangle rt2 = CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.RectangleToScreen(rect);
                //CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.ZoomToRegion(rt.X, rt.Y, 500, 500);
                //CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.ScrollTo(rt.X, rt.Y, 400, 400);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Stopwatch stopwatch1 = Stopwatch.StartNew();
   
                    CVContour cIVT_CVContour = new CVContour();
                    cIVT_CVContour.SetProperty(CVisionTools.Contours[CDisplayManager.CameraIndex]);                    
                    cIVT_CVContour.SetSourceImage(ImageCVSource);
                    cIVT_CVContour.Run();

                    Graphics g = Graphics.FromImage(Result);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    if (!cIVT_CVContour.property.USE_DRAW_IMAGE)
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        int Count = 0;
                        foreach (var item in cIVT_CVContour.results)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                            g.DrawString((Count + 1).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                            g.DrawString(item.Area.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                            Count++;
                        }

                        if (cIVT_CVContour.results.Count == 0)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cIVT_CVContour.property.CvROI));
                            g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cIVT_CVContour.property.CvROI.X - 20, (int)cIVT_CVContour.property.CvROI.Y - 20);
                        }
                    }
                    else
                    {
                        Result = Lib.Common.CImageConverter.ToBitmap(cIVT_CVContour.imageResult);
                    }

                    List<CResultContour> TotalResults = new List<CResultContour>();

                    TotalResults = cIVT_CVContour.results;

                    stopwatch1.Stop();
                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";
                    var myTask = Task.Run(() =>
                    {

                        this.Invoke(new System.Windows.Forms.MethodInvoker(() =>
                        {
                            dgvDefect.DataSource = new CDefectList_Result().GetContourList(TotalResults);
                            dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            imageListView1.Items.Clear();
                            imageListView1.ThumbnailSize = new System.Drawing.Size(200, 200);


                            for (int i = 0; i < TotalResults.Count; i++)
                            {
                                Rectangle ResizeRect = TotalResults[i].Bounding;
                                int OffsetSize = 20;
                                ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                                imageListView1.Items.Add("Index : " + (i + 1), Lib.Common.CBitmapProcessing.CropAtRect(Result, ResizeRect).Result);
                            }
                        }));

                    });


                    CDisplayManager.CreateLayerDisplay(Result, "Contour", true);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
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

        private void renderertoolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(ImageListView));
            RendererComboBoxItem item = (RendererComboBoxItem)renderertoolStripComboBox.SelectedItem;
            ImageListView.ImageListViewRenderer renderer = (ImageListView.ImageListViewRenderer)assembly.CreateInstance(item.FullName);
            if (renderer == null)
            {
                assembly = Assembly.GetExecutingAssembly();
                renderer = (ImageListView.ImageListViewRenderer)assembly.CreateInstance(item.FullName);
            }
            colorToolStripComboBox.Enabled = renderer.CanApplyColors;
            imageListView1.SetRenderer(renderer);
            imageListView1.Focus();
        }

        private void colorToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PropertyInfo field = ((ColorComboBoxItem)colorToolStripComboBox.SelectedItem).Field;
            ImageListViewColor color = (ImageListViewColor)field.GetValue(null, null);
            imageListView1.Colors = color;
        }

        private void thumbnailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = View.Thumbnails;
        }

        private void galleryToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = View.Gallery;
        }

        private void horizontalStripToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = View.HorizontalStrip;
        }

        private void verticalStripToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = View.VerticalStrip;
        }

        private void clearThumbsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.ClearThumbnailCache();
        }

        private void x96ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.ThumbnailSize = new System.Drawing.Size(96, 96);
        }

        private void x200ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.ThumbnailSize = new System.Drawing.Size(200, 200);
        }

        private void btnTotalRun_Click(object sender, EventArgs e)
        {
            
        }

        private void btnDifferent_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                {
                    Mat oriImg = ImageCVSource.Clone();
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);
                    if (oriImg.Channels() != 3) Cv2.CvtColor(oriImg, oriImg, ColorConversionCodes.GRAY2RGB);

                    Bitmap TempSrc = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Stopwatch stopwatch1 = Stopwatch.StartNew();

                    List<CResultContour> TotalResults = new List<CResultContour>();
                    CVContour cIVT_CVContour = new CVContour();
                    cIVT_CVContour.SetProperty(CVisionTools.Contours[CDisplayManager.CameraIndex]);
                    cIVT_CVContour.property.USE_DRAW_IMAGE = true;
                    cIVT_CVContour.SetSourceImage(ImageCVSource);
                    cIVT_CVContour.Run();
                    TotalResults = cIVT_CVContour.results;

                    Graphics g = Graphics.FromImage(Result);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    Mat resultImage = new Mat();
                    Cv2.BitwiseXor(oriImg, cIVT_CVContour.imageResult, resultImage);

                    Result = Lib.Common.CImageConverter.ToBitmap(resultImage);
                    
                    stopwatch1.Stop();
                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";
                    var myTask = Task.Run(() =>
                    {

                        this.Invoke(new System.Windows.Forms.MethodInvoker(() =>
                        {
                            dgvDefect.DataSource = new CDefectList_Result().GetContourList(TotalResults);
                            dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            imageListView1.Items.Clear();
                            imageListView1.ThumbnailSize = new System.Drawing.Size(200, 200);


                            for (int i = 0; i < TotalResults.Count; i++)
                            {
                                Rectangle ResizeRect = TotalResults[i].Bounding;
                                int OffsetSize = 20;
                                ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                                imageListView1.Items.Add("Index : " + (i + 1), Lib.Common.CBitmapProcessing.CropAtRect(Result, ResizeRect).Result);
                            }
                        }));

                    });


                    CDisplayManager.CreateLayerDisplay(Result, "Contour", true);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }

    public class RandomColumnComparer : IComparer<ImageListViewItem>
    {
        public int Compare(ImageListViewItem x, ImageListViewItem y)
        {
            return int.Parse(x.SubItems["random"].Text).CompareTo(int.Parse(y.SubItems["random"].Text));
        }
    }

    public struct RendererComboBoxItem
    {
        public string Name;
        public string FullName;

        public override string ToString()
        {
            return Name;
        }

        public RendererComboBoxItem(Type type)
        {
            Name = type.Name;
            FullName = type.FullName;
        }
    }

    public struct ColorComboBoxItem
    {
        public string Name;
        public PropertyInfo Field;

        public override string ToString()
        {
            return Name;
        }

        public ColorComboBoxItem(PropertyInfo field)
        {
            Name = field.Name;
            Field = field;
        }
    }
}
