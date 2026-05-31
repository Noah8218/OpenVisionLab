using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using Lib.Common;
using Manina.Windows.Forms;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SortOrder = Manina.Windows.Forms.SortOrder;
using View = Manina.Windows.Forms.View;

namespace OpenVisionLab
{
    public partial class FormMainDefect : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormMainDefect()
        {           
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;

            for(int i=0; i < Global.Device.CAMERA_COUNT; i++)
            {
                InitDefectGridview(i);
            }

            if(Global.Device.CAMERA_COUNT == 1)
            {
                //chrome69Tabcontrol1.TabPages.RemoveAt(DEFINE.CAM_2);
            }

            Global.Thread.CSeqVision.EventSeqComplete += OnInspResult;
        }
     
        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        public struct NumberSales
        {
            public string DateOrTime { get; set; }
            public double UnitSold { get; set; }
        }

        public List<NumberSales> NumberSalesList { get; private set; } = new List<NumberSales>();

        private bool ChangeSize = false;

        private void Form_Load(object sender, EventArgs e)
        {
            
        }    

        private void InitDefectGridview(int index)
        {
            if(index == DEFINE.CAM_1)
            {
                dgvDefectCam1.DataSource = new CDefectList().GetProductsList();
                dgvDefectCam1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                imageListViewCam1.AllowDuplicateFileNames = true;
                imageListViewCam1.SetRenderer(new Manina.Windows.Forms.ImageListViewRenderers.DefaultRenderer());
                imageListViewCam1.SortColumn = 0;
                imageListViewCam1.SortOrder = SortOrder.AscendingNatural;

                Assembly assembly = Assembly.GetAssembly(typeof(ImageListView));

                int j = 0;
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(ImageListView.ImageListViewRenderer))
                    {
                        renderertoolStripComboBox.Items.Add(new RendererComboBoxItem(type));
                        if (type.Name == "DefaultRenderer")
                            renderertoolStripComboBox.SelectedIndex = j;
                        j++;
                    }
                }
                // Find and add custom colors
                Type colorType = typeof(ImageListViewColor);
                j = 0;
                foreach (PropertyInfo field in colorType.GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    colorToolStripComboBox.Items.Add(new ColorComboBoxItem(field));
                    if (field.Name == "Default")
                        colorToolStripComboBox.SelectedIndex = j;
                    j++;
                }

                string cacheDir = Path.Combine(
                    Path.GetDirectoryName(new Uri(assembly.GetName().CodeBase).LocalPath),
                    "Cache"
                    );
                imageListViewCam1.Columns.Add(ColumnType.Name);
                imageListViewCam1.Columns.Add(ColumnType.Dimensions);
                imageListViewCam1.Columns.Add(ColumnType.FileSize);
                imageListViewCam1.Columns.Add(ColumnType.FolderName);
                imageListViewCam1.Columns.Add(ColumnType.DateModified);
                imageListViewCam1.Columns.Add(ColumnType.FileType);
                var col = new ImageListView.ImageListViewColumnHeader(ColumnType.Custom, "random", "Random");
                col.Comparer = new RandomColumnComparer();
                imageListViewCam1.Columns.Add(col);
            }
           else
            {
                dgvDefectCam2.DataSource = new CDefectList().GetProductsList();
                dgvDefectCam2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                imageListViewCam2.AllowDuplicateFileNames = true;
                imageListViewCam2.SetRenderer(new Manina.Windows.Forms.ImageListViewRenderers.DefaultRenderer());
                imageListViewCam2.SortColumn = 0;
                imageListViewCam2.SortOrder = SortOrder.AscendingNatural;

                Assembly assembly = Assembly.GetAssembly(typeof(ImageListView));

                int j = 0;
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(ImageListView.ImageListViewRenderer))
                    {
                        renderertoolStripComboBox.Items.Add(new RendererComboBoxItem(type));
                        if (type.Name == "DefaultRenderer")
                            renderertoolStripComboBox.SelectedIndex = j;
                        j++;
                    }
                }
                // Find and add custom colors
                Type colorType = typeof(ImageListViewColor);
                j = 0;
                foreach (PropertyInfo field in colorType.GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    colorToolStripComboBox.Items.Add(new ColorComboBoxItem(field));
                    if (field.Name == "Default")
                        colorToolStripComboBox.SelectedIndex = j;
                    j++;
                }

                string cacheDir = Path.Combine(
                    Path.GetDirectoryName(new Uri(assembly.GetName().CodeBase).LocalPath),
                    "Cache"
                    );
                imageListViewCam2.Columns.Add(ColumnType.Name);
                imageListViewCam2.Columns.Add(ColumnType.Dimensions);
                imageListViewCam2.Columns.Add(ColumnType.FileSize);
                imageListViewCam2.Columns.Add(ColumnType.FolderName);
                imageListViewCam2.Columns.Add(ColumnType.DateModified);
                imageListViewCam2.Columns.Add(ColumnType.FileType);
                var col = new ImageListView.ImageListViewColumnHeader(ColumnType.Custom, "random", "Random");
                col.Comparer = new RandomColumnComparer();
                imageListViewCam2.Columns.Add(col);
            }
        }

        private void OnInspResult(object sender, EventArgs e)
        {
            if (!(e is InspResultArgs args)) { return; }
            this.UIThreadBeginInvoke(() =>
            {
                if (Global.System.Menu == CSystem.MENU.MAIN)
                {
                    if (args.Index == DEFINE.CAM_1)
                    {                        
                        if (args.black_Result.Count != 0 || args.white_Result.Count != 0)
                        {
                            dgvDefectCam1.DataSource = new CDefectList().GetDefectList(args.totalResults.ConvertAll(s => s), args.Index, args.imageOri.Height);

                            imageListViewCam1.Items.Clear();
                            imageListViewCam1.SuspendLayout();
                            imageListViewCam1.ThumbnailSize = new System.Drawing.Size(200, 200);
                        }

                        if (!this.Visible)
                        {
                            return; 
                        }
                        using (Bitmap imageSrc = (Bitmap)args.imageOri)
                        {
                            for (int i = 0; i < args.black_Result.Count; i++)
                            {
                                if (!CGlobal.Inst.Data.runInsp_1) { break; }
                                Rectangle ResizeRect = args.black_Result[i].Bounding;
                                int OffsetSize = 20;
                                ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                                imageListViewCam1.Items.Add("B-Index : " + (i + 1), Lib.Common.CBitmapProcessing.CropAtRect(imageSrc, ResizeRect).Result);
                                Application.DoEvents();
                            }

                            for (int i = 0; i < args.white_Result.Count; i++)
                            {
                                if (!CGlobal.Inst.Data.runInsp_1) { break; }
                                Rectangle ResizeRect = args.white_Result[i].Bounding;
                                int OffsetSize = 20;
                                ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                                imageListViewCam1.Items.Add("W-Index : " + (i + 1), Lib.Common.CBitmapProcessing.CropAtRect(imageSrc, ResizeRect).Result);
                                Application.DoEvents();
                            }
                        }
                        imageListViewCam1.ResumeLayout();
                    }
                    else if (args.Index == DEFINE.CAM_2)
                    {
                        if (args.black_Result.Count != 0 || args.white_Result.Count != 0)
                        {
                            dgvDefectCam2.DataSource = new CDefectList().GetDefectList(args.totalResults.ConvertAll(s => s), args.Index, args.imageOri.Height);

                            imageListViewCam2.Items.Clear();
                            imageListViewCam2.SuspendLayout();
                            imageListViewCam2.ThumbnailSize = new System.Drawing.Size(200, 200);
                        }

                        if (!this.Visible)
                        {
                            return;
                        }

                        using (Bitmap imageSrc = (Bitmap)args.imageOri)
                        {
                            for (int i = 0; i < args.black_Result.Count; i++)
                            {
                                if (!CGlobal.Inst.Data.runInsp_2) { break; }
                                Rectangle ResizeRect = args.black_Result[i].Bounding;
                                int OffsetSize = 20;
                                ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                                imageListViewCam2.Items.Add("B-Index : " + (i + 1), Lib.Common.CBitmapProcessing.CropAtRect(imageSrc, ResizeRect).Result);
                                Application.DoEvents();
                            }

                            for (int i = 0; i < args.white_Result.Count; i++)
                            {
                                if (!CGlobal.Inst.Data.runInsp_2) { break; }
                                Rectangle ResizeRect = args.white_Result[i].Bounding;
                                int OffsetSize = 20;
                                ResizeRect = new Rectangle(ResizeRect.X - OffsetSize, ResizeRect.Y - OffsetSize, ResizeRect.Width + (OffsetSize * 2), ResizeRect.Height + (OffsetSize * 2));
                                imageListViewCam2.Items.Add("W-Index : " + (i + 1), Lib.Common.CBitmapProcessing.CropAtRect(imageSrc, ResizeRect).Result);
                                Application.DoEvents();
                            }
                        }
                        imageListViewCam2.ResumeLayout();
                    }
                }
            });
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
            imageListViewCam1.SetRenderer(renderer);
            imageListViewCam1.Focus();
        }

        private void colorToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PropertyInfo field = ((ColorComboBoxItem)colorToolStripComboBox.SelectedItem).Field;
            ImageListViewColor color = (ImageListViewColor)field.GetValue(null, null);
            imageListViewCam1.Colors = color;
        }

        private void thumbnailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListViewCam1.View = View.Thumbnails;
        }

        private void galleryToolStripButton_Click(object sender, EventArgs e)
        {
            imageListViewCam1.View = View.Gallery;
        }

        private void horizontalStripToolStripButton_Click(object sender, EventArgs e)
        {
            imageListViewCam1.View = View.HorizontalStrip;
        }

        private void verticalStripToolStripButton_Click(object sender, EventArgs e)
        {
            imageListViewCam1.View = View.VerticalStrip;
        }

        private void clearThumbsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListViewCam1.ClearThumbnailCache();
        }

        private void x96ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListViewCam1.ThumbnailSize = new System.Drawing.Size(96, 96);
        }

        private void x200ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListViewCam1.ThumbnailSize = new System.Drawing.Size(200, 200);
        }

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
    }
}
