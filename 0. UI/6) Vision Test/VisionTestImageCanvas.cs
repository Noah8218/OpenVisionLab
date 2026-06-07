using Lib.Common;
using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.Rendering;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;
using CvMat = OpenCvSharp.Mat;

namespace OpenVisionLab
{
	public sealed class VisionTestImageCanvas : UserControl
	{
		private const string TextureName = "VisionTestImage";

		private ImageCanvasControl imageViewer;
		private ContextMenuStrip imageContextMenu;
		private Bitmap image;
		private Bitmap imageSourceReference;
		private Size imageSize = Size.Empty;
		private bool pendingImageLoad;
		private RectangleF selectionRegion = RectangleF.Empty;

		public VisionTestImageCanvas()
		{
			BackColor = Color.Black;
		}

		public event EventHandler UserImageChanged = delegate { };

		public Image DisplayImage
		{
			get => image;
			set => SetImage(value as Bitmap ?? (value == null ? null : new Bitmap(value)), false);
		}

		public Bitmap DisplayBitmap => image;
		public bool ShowPixelGrid { get; set; }
		public RectangleF SelectionRegion
		{
			get => selectionRegion;
			set => selectionRegion = value;
		}

		public void ZoomToFit()
		{
			EnsureImageViewer().ZoomToFit();
		}

		public void ZoomIn()
		{
			EnsureImageViewer().ZoomAt(GetCenterPoint(), 120);
		}

		public void ZoomOut()
		{
			EnsureImageViewer().ZoomAt(GetCenterPoint(), -120);
		}

		public Point PointToImage(Point location)
		{
			if (image == null) { return Point.Empty; }

			Point robotPoint = EnsureImageViewer().GetCurrentRobotPos(location.X, location.Y);
			int x = Math.Max(0, Math.Min(image.Width - 1, robotPoint.X));
			int y = Math.Max(0, Math.Min(image.Height - 1, image.Height - robotPoint.Y));

			return new Point(x, y);
		}

		public new void Invalidate()
		{
			base.Invalidate();
			imageViewer?.RefreshGL();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				imageViewer?.ClearTextureStateOnly();
				imageContextMenu?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void SetImage(Bitmap source, bool raiseChanged)
		{
			if (!raiseChanged && image != null && !pendingImageLoad && ReferenceEquals(source, imageSourceReference))
			{
				return;
			}

			image = CreateCanvasBitmap(source);
			imageSourceReference = raiseChanged ? image : source;

			if (image == null)
			{
				imageSourceReference = null;
				pendingImageLoad = false;
				imageViewer?.ClearTexture();
			}
			else
			{
				EnsureImageViewer();

				if (!CanLoadTexture())
				{
					pendingImageLoad = true;
				}
				else
				{
					LoadCurrentImage();
				}
			}

			if (raiseChanged)
			{
				UserImageChanged(this, EventArgs.Empty);
			}
		}

		private bool CanLoadTexture()
		{
			return imageViewer != null && imageViewer.IsHandleCreated && !imageViewer.IsDisposed;
		}

		private ImageCanvasControl EnsureImageViewer()
		{
			if (imageViewer != null)
			{
				return imageViewer;
			}

			Stopwatch stopwatch = Stopwatch.StartNew();

			imageViewer = new ImageCanvasControl
			{
				Dock = DockStyle.Fill,
				BackColor = Color.Black,
				IsShowCrossLine = false
			};

			imageViewer.Draw += (sender, e) => imageViewer.DrawContent();
			imageViewer.MouseDown += OnImageViewerMouseDown;
			imageViewer.MouseMove += OnImageViewerMouseMove;
			imageViewer.MouseUp += OnImageViewerMouseUp;
			imageViewer.MouseWheel += OnImageViewerMouseWheel;
			imageViewer.MouseClicked += (sender, e) => OnMouseClick(e as MouseEventArgs);
			imageViewer.Load += (sender, e) => LoadPendingImage();

			imageContextMenu = CanvasContextMenuFactory.CreateImageMenu(OnImageLoadClicked, OnImageSaveClicked);
			imageViewer.ContextMenuStrip = imageContextMenu;

			Controls.Add(imageViewer);
			stopwatch.Stop();
			CLOG.NORMAL($"[PERF] {nameof(VisionTestImageCanvas)} create GL viewer: {stopwatch.ElapsedMilliseconds}ms");

			return imageViewer;
		}

		private void LoadPendingImage()
		{
			if (!pendingImageLoad || image == null) { return; }
			if (!CanLoadTexture()) { return; }

			LoadCurrentImage();
			ZoomToFit();
		}

		private void LoadCurrentImage()
		{
			if (image == null) { return; }

			try
			{
				pendingImageLoad = false;
				imageViewer.ClearTexture();

				Stopwatch stopwatch = Stopwatch.StartNew();
				using (CvMat mat = CImageConverter.ToMat(image))
				{
					CanvasImageLoader.UploadMatAsTexture(imageViewer, mat, TextureName, ref imageSize);
				}
				stopwatch.Stop();
				CLOG.NORMAL($"[PERF] {nameof(VisionTestImageCanvas)} texture upload {image.Width}x{image.Height}: {stopwatch.ElapsedMilliseconds}ms");

				imageViewer.RefreshGL();
			}
			catch (Exception desc)
			{
				pendingImageLoad = true;
				CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {desc.Message}");
			}
		}

		private void OnImageViewerMouseDown(object sender, CanvasMouseEventArgs e)
		{
			OnMouseDown(e);
			if (e.Button != MouseButtons.Left) { return; }

			imageViewer.PreMousePos = imageViewer.GetCurrentRobotPos(e.X, e.Y);
			imageViewer.SetViewMode(CanvasInteractionMode.Drag);
		}

		private void OnImageViewerMouseMove(object sender, CanvasMouseEventArgs e)
		{
			OnMouseMove(e);
		}

		private void OnImageViewerMouseUp(object sender, CanvasMouseEventArgs e)
		{
			OnMouseUp(e);
			if (e.Button == MouseButtons.Right)
			{
				Control target = sender as Control ?? imageViewer;
				imageContextMenu.Show(target, e.Location);
			}

			imageViewer.SetViewMode(CanvasInteractionMode.None);
		}

		private void OnImageViewerMouseWheel(object sender, CanvasMouseEventArgs e)
		{
			OnMouseWheel(e);
			imageViewer.ZoomAt(e.Location, e.Delta);
		}

		private Point GetCenterPoint()
		{
			return new Point(Math.Max(0, Width / 2), Math.Max(0, Height / 2));
		}

		private void OnImageLoadClicked(object sender, EventArgs e)
		{
			string imagePath = CUtil.LoadImageFilePath();
			if (string.IsNullOrEmpty(imagePath)) { return; }

			using (Bitmap loadedImage = new Bitmap(imagePath))
			{
				SetImage(loadedImage, true);
			}

			ZoomToFit();
		}

		private void OnImageSaveClicked(object sender, EventArgs e)
		{
			if (image == null || image.Width <= 10 || image.Height <= 10) { return; }

			string imagePath = CUtil.SaveImageFilePath();
			if (string.IsNullOrEmpty(imagePath)) { return; }

			image.Save(imagePath);
		}

		private static Bitmap CreateCanvasBitmap(Bitmap source)
		{
			if (source == null) { return null; }

			Bitmap bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height));
			}

			return bitmap;
		}
	}
}
