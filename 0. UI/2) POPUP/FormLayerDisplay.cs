using Lib.Common;
using OpenVisionLab._1._Core;
using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;
using CvMat = OpenCvSharp.Mat;

namespace OpenVisionLab
{
	public partial class FormLayerDisplay : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		private const string LayerImageName = "LayerDisplayImage";

		private readonly IDisplayManager displayManager;
		private ImageCanvasControl imageViewer;
		private ContextMenuStrip imageContextMenu;
		private Bitmap currentImage;
		private Size imageSize = Size.Empty;
		private bool pendingImageLoad;
		private bool imageChanged;

		public int nIndex = 0;

		public bool ChangeSize { get; set; } = false;
		public List<FormLayerDisplay> DisplayList = new List<FormLayerDisplay>();
		public ImageCanvasControl ImageViewer => imageViewer;
		public Rectangle Roi { get; private set; } = Rectangle.Empty;
		public Rectangle TrainROI { get; private set; } = Rectangle.Empty;
		public bool ImageChanged => imageChanged;

		public FormLayerDisplay(Bitmap ImageSource, int nIndex, List<FormLayerDisplay> LayerDisplayList, bool UseCloseButton = true, string strTitle = "TEST", bool onlyDragMode = false, IDisplayManager displayManager = null)
		{
			InitializeComponent();
			InitializeViewerStatusBar();

			this.nIndex = nIndex;
			this.displayManager = displayManager ?? DisplayManagerService.Default;
			DisplayList = LayerDisplayList;

			InitializeImageViewer();

			Activated += FormLayerDisplay_Activated;
			Shown += FormLayerDisplay_Shown;

			Text = strTitle != "TEST" ? strTitle : "TEST";

			CloseButton = UseCloseButton;
			CloseButtonVisible = UseCloseButton;

			if (ImageSource != null)
			{
				SetImage(ImageSource, false);
			}
		}

		public void SetImage(Bitmap image, bool zoomToFit = true)
		{
			currentImage = CreateCanvasBitmap(image);
			imageChanged = true;
			SyncImageSpace();

			if (currentImage == null)
			{
				pendingImageLoad = false;
				imageViewer?.ClearTexture();
				return;
			}

			if (!CanLoadTexture())
			{
				pendingImageLoad = true;
				return;
			}

			LoadCurrentImage(zoomToFit);
		}

		public Bitmap GetCurrentImage()
		{
			return currentImage;
		}

		public void AcceptImageChanged()
		{
			imageChanged = false;
			SyncImageSpace();
		}

		public void RefreshViewer()
		{
			imageViewer?.RefreshGL();
		}

		public void ZoomToFit()
		{
			imageViewer?.ZoomToFit();
		}

		private void InitializeImageViewer()
		{
			imageViewer = new ImageCanvasControl
			{
				Dock = DockStyle.Fill,
				BackColor = Color.Black,
				IsShowCrossLine = false
			};

			imageViewer.Draw += (sender, e) => imageViewer.DrawContent();
			imageViewer.MouseDown += OnImageViewerMouseDown;
			imageViewer.MouseUp += OnImageViewerMouseUp;
			imageViewer.MouseWheel += OnImageViewerMouseWheel;
			InitializeImageContextMenu();
			imageViewer.ContextMenuStrip = imageContextMenu;

			panel3.Controls.Add(imageViewer);
		}

		private void InitializeImageContextMenu()
		{
			imageContextMenu = CanvasContextMenuFactory.CreateImageMenu(OnImageLoadClicked, OnImageSaveClicked);
		}

		private bool CanLoadTexture()
		{
			return imageViewer != null && imageViewer.IsHandleCreated && !imageViewer.IsDisposed;
		}

		private void LoadPendingImage()
		{
			if (!pendingImageLoad || currentImage == null) { return; }
			if (!CanLoadTexture()) { return; }

			LoadCurrentImage(true);
		}

		private void LoadCurrentImage(bool zoomToFit)
		{
			if (currentImage == null || imageViewer == null) { return; }

			try
			{
				pendingImageLoad = false;
				imageViewer.ClearTexture();

				using (Bitmap textureImage = CreateCanvasBitmap(currentImage))
				using (CvMat mat = CImageConverter.ToMat(textureImage))
				{
					CanvasImageLoader.UploadMatAsTexture(imageViewer, mat, LayerImageName, ref imageSize);
				}

				if (zoomToFit)
				{
					imageViewer.ZoomToFit();
				}
			}
			catch (Exception desc)
			{
				pendingImageLoad = true;
				CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {desc.Message}");
			}
		}

		private void OnImageViewerMouseDown(object sender, CanvasMouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) { return; }

			imageViewer.PreMousePos = imageViewer.GetCurrentRobotPos(e.X, e.Y);
			imageViewer.SetViewMode(CanvasInteractionMode.Drag);
		}

		private void OnImageViewerMouseUp(object sender, CanvasMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ShowImageContextMenu(sender, e);
			}

			imageViewer.SetViewMode(CanvasInteractionMode.None);
		}

		private void OnImageViewerMouseWheel(object sender, CanvasMouseEventArgs e)
		{
			imageViewer.ZoomAt(e.Location, e.Delta);
		}

		private void ShowImageContextMenu(object sender, CanvasMouseEventArgs e)
		{
			if (imageContextMenu == null || imageContextMenu.IsDisposed) { return; }

			Control target = sender as Control ?? imageViewer;
			imageContextMenu.Show(target, e.Location);
		}

		private void OnImageLoadClicked(object sender, EventArgs e)
		{
			string imagePath = CUtil.LoadImageFilePath();
			if (string.IsNullOrEmpty(imagePath)) { return; }

			using (Bitmap loadedImage = new Bitmap(imagePath))
			{
				SetImage(CreateCanvasBitmap(loadedImage));
			}

			ZoomToFit();
		}

		private void OnImageSaveClicked(object sender, EventArgs e)
		{
			if (currentImage == null || currentImage.Width <= 10 || currentImage.Height <= 10) { return; }

			string imagePath = CUtil.SaveImageFilePath();
			if (string.IsNullOrEmpty(imagePath)) { return; }

			currentImage.Save(imagePath);
		}

		private void SyncImageSpace()
		{
			if (displayManager?.ImageSpace == null || nIndex < 0) { return; }

			displayManager.ImageSpace.SetImage(nIndex, Text, currentImage);
			displayManager.ImageSpace.SetRoi(nIndex, Roi);
			displayManager.ImageSpace.SetTrainRoi(nIndex, TrainROI);
			displayManager.ImageSpace.MarkImageChanged(Text, imageChanged);
		}

		private void InitializeViewerStatusBar()
		{
			panel2.Height = 20;
			panel2.BackColor = Color.FromArgb(38, 38, 38);
			panel2.Padding = new Padding(4, 0, 0, 0);

			ConfigureViewerStatusLabel(lbXY, 190, " Pos : ((X=0, Y=0)) ");
			ConfigureViewerStatusLabel(lbGV, 90, " Gv : (0) ");
			ConfigureViewerStatusLabel(lbRGB, 210, " Color : (RGB(0,0,0)) ");

			lbZOOM.Visible = false;
			lbZOOM.Width = 0;

			panel2.Controls.Clear();
			panel2.Controls.Add(lbRGB);
			panel2.Controls.Add(lbGV);
			panel2.Controls.Add(lbXY);
		}

		private void ConfigureViewerStatusLabel(RJCodeUI_M1.RJControls.RJLabel label, int width, string text)
		{
			label.BorderStyle = BorderStyle.None;
			label.BackColor = Color.FromArgb(38, 38, 38);
			label.ForeColor = Color.FromArgb(245, 249, 252);
			label.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			label.Dock = DockStyle.Left;
			label.Width = width;
			label.TextAlign = ContentAlignment.MiddleLeft;
			label.Text = text;
		}

		private Bitmap CreateCanvasBitmap(Bitmap source)
		{
			if (source == null) { return null; }

			Bitmap bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height));
			}

			return bitmap;
		}

		private void FormLayerDisplay_Shown(object sender, EventArgs e)
		{
			LoadPendingImage();
		}

		private void FormLayerDisplay_Activated(object sender, EventArgs e)
		{
		}

		private void ResizeEvent(object sender, EventArgs e)
		{
			Visible = Width > MinimumSize.Width && Height > MinimumSize.Height;
		}

		private void LayerDisplay_FormClosed(object sender, FormClosedEventArgs e)
		{
			imageViewer?.ClearTexture();
			imageContextMenu?.Dispose();

			int displayIndex = GetDisplayIndex(Text);
			if (displayIndex >= 0 && displayIndex < DisplayList.Count)
			{
				DisplayList.RemoveAt(displayIndex);
			}
		}

		private int GetDisplayIndex(string strTitle)
		{
			for (int i = 0; i < DisplayList.Count; i++)
			{
				if (DisplayList[i].Text == strTitle) { return i; }
			}

			return -1;
		}

		private void timePixelData_Tick(object sender, EventArgs e)
		{
			try
			{
				if (imageViewer == null) { return; }

				Color color = imageViewer.PixelColor;
				PointF pos = imageViewer.PixelPos;

				lbXY.Text = string.Format(" Pos : ((X={0}, Y={1})) ", (int)pos.X, (int)pos.Y);
				lbGV.Text = string.Format(" Gv : ({0}) ", imageViewer.GrayValue);
				lbRGB.Text = string.Format(" Color : (RGB({0},{1},{2})) ", color.R, color.G, color.B);
			}
			catch (Exception desc)
			{
				CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {desc.Message}");
			}
		}

		private void FormLayerDisplay_VisibleChanged(object sender, EventArgs e)
		{
			if (!ChangeSize)
			{
				if (DockHandler.FloatPane != null)
				{
					DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
				}

				Refresh();
				ChangeSize = true;
				LoadPendingImage();
				ZoomToFit();
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
		}
	}
}
