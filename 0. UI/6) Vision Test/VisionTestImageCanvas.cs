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
		public string EmptyTitle { get; set; } = "No image";
		public string EmptyDescription { get; set; } = "Select a layer or load an image.";
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
				if (imageViewer != null)
				{
					imageViewer.ClearTexture();
					imageViewer.Visible = false;
				}
				base.Invalidate();
			}
			else
			{
				EnsureImageViewer().Visible = true;

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

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (image != null) { return; }

			using Font titleFont = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
			using Font descriptionFont = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
			using Brush titleBrush = new SolidBrush(Color.FromArgb(220, 230, 240));
			using Brush descriptionBrush = new SolidBrush(Color.FromArgb(140, 155, 170));
			using Pen framePen = new Pen(Color.FromArgb(68, 82, 98), 1F);

			Rectangle iconRect = new Rectangle((Width - 42) / 2, Math.Max(18, Height / 2 - 46), 42, 34);
			e.Graphics.DrawRectangle(framePen, iconRect);
			e.Graphics.DrawLine(framePen, iconRect.Left + 8, iconRect.Bottom - 8, iconRect.Left + 18, iconRect.Top + 18);
			e.Graphics.DrawLine(framePen, iconRect.Left + 18, iconRect.Top + 18, iconRect.Left + 27, iconRect.Bottom - 10);
			e.Graphics.DrawEllipse(framePen, iconRect.Right - 13, iconRect.Top + 7, 5, 5);

			DrawCenteredText(e.Graphics, EmptyTitle, titleFont, titleBrush, iconRect.Bottom + 10);
			if (!string.IsNullOrWhiteSpace(EmptyDescription))
			{
				DrawCenteredText(e.Graphics, EmptyDescription, descriptionFont, descriptionBrush, iconRect.Bottom + 32);
			}
		}

		private void DrawCenteredText(Graphics graphics, string text, Font font, Brush brush, int top)
		{
			if (string.IsNullOrWhiteSpace(text)) { return; }

			SizeF textSize = graphics.MeasureString(text, font);
			float left = Math.Max(4, (Width - textSize.Width) / 2F);
			graphics.DrawString(text, font, brush, left, top);
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

			return imageViewer;
		}

		private void LoadPendingImage()
		{
			if (!pendingImageLoad || image == null) { return; }
			if (!CanLoadTexture()) { return; }

			LoadCurrentImage();
		}

		private void LoadCurrentImage()
		{
			if (image == null) { return; }

			try
			{
				pendingImageLoad = false;

				Stopwatch stopwatch = Stopwatch.StartNew();
				using (CvMat mat = BitmapImageConverter.ToMat(image))
				{
					using (imageViewer.SuppressRefresh())
					{
						CanvasImageLoader.UploadMatAsTexture(imageViewer, mat, TextureName, ref imageSize, false);
						imageViewer.ZoomToFit();
					}
				}
				stopwatch.Stop();

				imageViewer.RefreshGL();
			}
			catch (Exception)
			{
				pendingImageLoad = true;

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
			string imagePath = AppUtil.LoadImageFilePath();
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

			string imagePath = AppUtil.SaveImageFilePath();
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
