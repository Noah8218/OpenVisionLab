using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Result;
using Manina.Windows.Forms;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpenVisionLab.CPropertySocket;
using static Manina.Windows.Forms.ImageListView;
using SortOrder = Manina.Windows.Forms.SortOrder;
using View = Manina.Windows.Forms.View;

namespace OpenVisionLab
{
    public partial class FormBlob : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormBlob()
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


                int Index = CDisplayManager.FindIndex("Defect");

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

        private void btnBlobRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                using (Mat imageCVSource = CDisplayManager.ImageSrc.Clone())
                {
                    if (imageCVSource.Channels() == 3) Cv2.CvtColor(imageCVSource, imageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap imageSrc = Lib.Common.CImageConverter.ToBitmap(imageCVSource);
                    Bitmap resultImg = imageSrc.Clone(new Rectangle(0, 0, imageSrc.Width, imageSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Stopwatch sw1 = Stopwatch.StartNew();
                    
                    Graphics g = Graphics.FromImage(resultImg);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    
                    List<CResultBlob> totalResults = new List<CResultBlob>();
                    List<CResultBlob> black_Result = new List<CResultBlob>();
                    List<CResultBlob> white_Result = new List<CResultBlob>();

                    Mat imageNormalize = new Mat();

                    Cv2.Normalize(imageCVSource, imageNormalize, 30, 255, NormTypes.MinMax);

                    //(totalResults, black_Result, white_Result) = CInspectionAlgorithm.RunSlitterBlob(g, imageNormalize, CVisionTools.Blobs[CDisplayManager.CameraIndex], new Rectangle());
                    CInspectionAlgorithm.DrawBlobResulte(g, CVisionTools.Blobs[CDisplayManager.CameraIndex], black_Result, true, true);
                    CInspectionAlgorithm.DrawBlobResulte(g, CVisionTools.Blobs[CDisplayManager.CameraIndex], white_Result, false, true);

                    sw1.Stop();
                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";
                    var task = Task.Run(() =>
                    {

                        this.Invoke(new MethodInvoker(() =>
                        {
                            dgvDefect.DataSource = new CDefectList_Result().GetBlobList(totalResults);
                            dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            imageListView1.Items.Clear();
                            imageListView1.ThumbnailSize = new System.Drawing.Size(200, 200);

                            for (int i = 0; i < black_Result.Count; i++)
                            {
                                Rectangle ResizeRect = black_Result[i].Bounding;
                                int OffsetSize = 20;
                                ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                                //imageListView1.Items.Add($"{black_Result[i].defectType.ToString()}-Index : " + (i + 1), Lib.Common.CBitmapProcessing.CropAtRect(imageSrc, ResizeRect).Result);
                                Application.DoEvents();
                            }

                            for (int i = 0; i < white_Result.Count; i++)
                            {
                                Rectangle ResizeRect = white_Result[i].Bounding;
                                int OffsetSize = 20;
                                ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                              //  imageListView1.Items.Add($"{white_Result[i].defectType.ToString()}-Index : " + (i + 1), Lib.Common.CBitmapProcessing.CropAtRect(imageSrc, ResizeRect).Result);
                                Application.DoEvents();
                            }
                        }));
                    });

                    CDisplayManager.CreateLayerDisplay(resultImg, "Defect", true);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        //private (List<CResultBlob>, Rect) RunBlob(Graphics g, Mat oriImage, ref List<CResultBlob> cResultBlobs, ref int count, bool findBlackBlob = true)
        //{
        //    //CVBlob cIVT_CVBlob = new CVBlob();
        //    //cIVT_CVBlob.SetProperty(CVisionTools.Blobs[CDisplayManager.CameraIndex].DeepCopy());
        //    //if (!findBlackBlob) { cIVT_CVBlob.property.USE_BITWISENOT = !cIVT_CVBlob.property.USE_BITWISENOT; }
        //    //cIVT_CVBlob.SetSourceImage(oriImage);
        //    //if (findBlackBlob) { cIVT_CVBlob.Run(new Rectangle(), true); }
        //    //else { cIVT_CVBlob.Run(new Rectangle(), false); }

        //    for(int i = 0; i < cIVT_CVBlob.results.Count; i++)
        //    {
        //        cResultBlobs.Add(cIVT_CVBlob.results[i]);
        //    }

        //    return (cIVT_CVBlob.results, cIVT_CVBlob.property.CvROI);
        //}


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

        private object m_ob = new object();

        private void btnTotalRun_Click(object sender, EventArgs e)
        {
            try
            {
                Global.Data.GrabQueue.Enqueue(new CGrabBuffer(CDisplayManager.ImageSrc.Clone(), CDisplayManager.CameraIndex, Global.Data.Total_Encoder, true));
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

    }
}
