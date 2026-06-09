using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Lib.Common;
using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.Rendering;
using CvMat = OpenCvSharp.Mat;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;

namespace OpenVisionLab
{
	public partial class FormImageCompare : RJCodeUI_M1.RJForms.RJChildForm
	{
		private readonly object viewSyncLock = new object();

		private ImageCanvasControl imageViewer1;
		private ImageCanvasControl imageViewer2;
		private Bitmap imageCompare1 = new Bitmap(10, 10);
		private Bitmap imageCompare2 = new Bitmap(10, 10);
		private DrawingSize imageSize1 = DrawingSize.Empty;
		private DrawingSize imageSize2 = DrawingSize.Empty;
		private Panel colorSwatch;
		private bool isMarkerVisible = false;
		private DrawingPoint markerImagePoint = DrawingPoint.Empty;

		private enum CompareMode
		{
			Image1,
			Image2
		}

		private CompareMode selectedCompareMode = CompareMode.Image1;

		public FormImageCompare()
		{
			InitializeComponent();
			ConfigureBottomBar();
			InitializeImageViewers();

			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		private void InitializeImageViewers()
		{
			imageViewer1 = CreateImageViewer(CompareMode.Image1);
			imageViewer2 = CreateImageViewer(CompareMode.Image2);

			splitContainer2.Panel1.Controls.Add(imageViewer1);
			splitContainer2.Panel2.Controls.Add(imageViewer2);
		}

		private void ConfigureBottomBar()
		{
			splitContainer1.Panel2.BackColor = Color.FromArgb(15, 18, 23);
			splitContainer1.Panel2.Padding = new Padding(8, 3, 8, 3);
			splitContainer1.FixedPanel = FixedPanel.Panel2;
			splitContainer1.IsSplitterFixed = true;
			splitContainer1.SplitterWidth = 1;
			splitContainer1.Resize += (sender, e) => LayoutBottomBar();

			StyleStatusLabel(lbRGB, 180);
			StyleStatusLabel(lbXY, 300);
			StyleStatusLabel(lbGV, 300);

			colorSwatch = new Panel
			{
				Dock = DockStyle.Left,
				Width = 22,
				BackColor = Color.Black,
				BorderStyle = BorderStyle.FixedSingle
			};
			splitContainer1.Panel2.Controls.Add(colorSwatch);
			splitContainer1.Panel2.Controls.SetChildIndex(colorSwatch, splitContainer1.Panel2.Controls.GetChildIndex(lbRGB));

			btnLoad.Dock = DockStyle.Right;
			btnLoad.Width = 112;
			btnLoad.Margin = Padding.Empty;
			btnLoad.FlatStyle = FlatStyle.Flat;
			btnLoad.FlatAppearance.BorderSize = 0;
			btnLoad.FlatAppearance.MouseOverBackColor = Color.FromArgb(89, 105, 234);
			btnLoad.FlatAppearance.MouseDownBackColor = Color.FromArgb(64, 78, 192);
			btnLoad.BackColor = Color.FromArgb(72, 88, 214);
			btnLoad.ForeColor = Color.White;
			btnLoad.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
			btnLoad.Text = "Load Images";

			lbRGB.Text = "색상 : -";
			lbXY.Text = "LEFT 이미지좌표(좌상)[-,-] GV[-]";
			lbGV.Text = "RIGHT 이미지좌표(좌상)[-,-] GV[-]";
			LayoutBottomBar();
		}

		private void StyleStatusLabel(Label label, int width)
		{
			label.Dock = DockStyle.Left;
			label.Width = width;
			label.Padding = new Padding(7, 0, 7, 0);
			label.Margin = Padding.Empty;
			label.BorderStyle = BorderStyle.None;
			label.BackColor = Color.FromArgb(15, 18, 23);
			label.ForeColor = Color.FromArgb(218, 225, 235);
			label.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			label.TextAlign = ContentAlignment.MiddleLeft;
		}

		private void LayoutBottomBar()
		{
			if (splitContainer1.Height <= 0) { return; }

			int bottomHeight = 28;
			splitContainer1.SplitterDistance = Math.Max(0, splitContainer1.Height - bottomHeight - splitContainer1.SplitterWidth);
		}

		private ImageCanvasControl CreateImageViewer(CompareMode mode)
		{
			ImageCanvasControl viewer = new ImageCanvasControl
			{
				Dock = DockStyle.Fill,
				BackColor = Color.Black,
				IsShowCrossLine = false
			};

			viewer.Draw += (sender, e) => DrawViewerContent(viewer, mode, e);
			viewer.MouseDown += (sender, e) => OnViewerMouseDown(mode, viewer, e);
			viewer.MouseMove += (sender, e) => OnViewerMouseMove(mode, viewer, e);
			viewer.MouseUp += (sender, e) => OnViewerMouseUp(mode, viewer);
			viewer.MouseWheel += (sender, e) => OnViewerMouseWheel(mode, viewer, e);
			viewer.MouseDoubleClicked += (sender, e) => OnViewerMouseDoubleClicked(mode, viewer);
			viewer.MouseLeave += (sender, e) => OnViewerMouseLeave();

			return viewer;
		}

		private void DrawViewerContent(ImageCanvasControl viewer, CompareMode mode, CanvasRenderEventArgs e)
		{
			viewer.DrawContent();
			DrawMarker(mode, viewer);
		}

		private void OnViewerMouseDown(CompareMode mode, ImageCanvasControl viewer, CanvasMouseEventArgs e)
		{
			selectedCompareMode = mode;
			viewer.PreMousePos = viewer.GetCurrentRobotPos(e.X, e.Y);
			viewer.SetViewMode(CanvasInteractionMode.Drag);
		}

		private void OnViewerMouseMove(CompareMode mode, ImageCanvasControl viewer, CanvasMouseEventArgs e)
		{
			selectedCompareMode = mode;
			UpdateCompareStatus(mode, viewer, e.Location);

			if (e.Button == MouseButtons.Left)
			{
				viewer.PreMousePos = viewer.GetCurrentRobotPos(e.X, e.Y);
				SyncViewStateFrom(mode);
			}
		}

		private void OnViewerMouseUp(CompareMode mode, ImageCanvasControl viewer)
		{
			selectedCompareMode = mode;
			viewer.SetViewMode(CanvasInteractionMode.None);
			SyncViewStateFrom(mode);
		}

		private void OnViewerMouseWheel(CompareMode mode, ImageCanvasControl viewer, CanvasMouseEventArgs e)
		{
			selectedCompareMode = mode;
			viewer.ZoomAt(e.Location, e.Delta);
			SyncViewStateFrom(mode);
		}

		private void OnViewerMouseDoubleClicked(CompareMode mode, ImageCanvasControl viewer)
		{
			selectedCompareMode = mode;
			viewer.ZoomToFit();
			SyncViewStateFrom(mode);
		}

		private void OnViewerMouseLeave()
		{
			isMarkerVisible = false;
			lbRGB.Text = "색상 : -";
			if (colorSwatch != null) { colorSwatch.BackColor = Color.Black; }
			RefreshMarkers();
		}

		private void SyncViewStateFrom(CompareMode sourceMode)
		{
			lock (viewSyncLock)
			{
				ImageCanvasControl source = GetViewer(sourceMode);
				ImageCanvasControl target = GetOtherViewer(sourceMode);
				if (source == null || target == null) { return; }

				target.ApplyViewState(source.CaptureViewState());
			}
		}

		private ImageCanvasControl GetViewer(CompareMode mode)
		{
			return mode == CompareMode.Image1 ? imageViewer1 : imageViewer2;
		}

		private ImageCanvasControl GetOtherViewer(CompareMode mode)
		{
			return mode == CompareMode.Image1 ? imageViewer2 : imageViewer1;
		}

		private Bitmap GetBitmap(CompareMode mode)
		{
			return mode == CompareMode.Image1 ? imageCompare1 : imageCompare2;
		}

		private void UpdateCompareStatus(CompareMode mode, ImageCanvasControl viewer, DrawingPoint mouseLocation)
		{
			Bitmap image = GetBitmap(mode);
			if (image == null || image.Width <= 10 || image.Height <= 10)
			{
				isMarkerVisible = false;
				lbRGB.Text = $"{GetModeText(mode)} no image";
				if (colorSwatch != null) { colorSwatch.BackColor = Color.Black; }
				lbXY.Text = "LEFT 이미지좌표(좌상)[-,-] GV[-]";
				lbGV.Text = "RIGHT 이미지좌표(좌상)[-,-] GV[-]";
				RefreshMarkers();
				return;
			}

			DrawingPoint imagePoint = ToTopLeftImagePoint(viewer, image, mouseLocation);
			if (!IsPointInBitmap(image, imagePoint))
			{
				isMarkerVisible = false;
				lbRGB.Text = $"{GetModeText(mode)} out";
				if (colorSwatch != null) { colorSwatch.BackColor = Color.Black; }
				lbXY.Text = FormatViewerStatus("LEFT", imageCompare1, imagePoint);
				lbGV.Text = FormatViewerStatus("RIGHT", imageCompare2, imagePoint);
				RefreshMarkers();
				return;
			}

			try
			{
				Color color = image.GetPixel(imagePoint.X, imagePoint.Y);

				markerImagePoint = imagePoint;
				isMarkerVisible = true;

				lbRGB.Text = $"색상 : RGB({color.R},{color.G},{color.B})";
				if (colorSwatch != null) { colorSwatch.BackColor = color; }
				lbXY.Text = FormatViewerStatus("LEFT", imageCompare1, imagePoint);
				lbGV.Text = FormatViewerStatus("RIGHT", imageCompare2, imagePoint);
				RefreshMarkers();
			}
			catch
			{
			}
		}

		private DrawingPoint ToTopLeftImagePoint(ImageCanvasControl viewer, Bitmap image, DrawingPoint mouseLocation)
		{
			DrawingPoint canvasPoint = viewer.GetCurrentRobotPos(mouseLocation.X, mouseLocation.Y);
			return new DrawingPoint(canvasPoint.X, image.Height - canvasPoint.Y);
		}

		private string FormatViewerStatus(string name, Bitmap image, DrawingPoint imagePoint)
		{
			if (!IsPointInBitmap(image, imagePoint)) { return $"{name} 이미지좌표(좌상)[{imagePoint.X},{imagePoint.Y}] out"; }

			Color color = image.GetPixel(imagePoint.X, imagePoint.Y);
			int brightness = (color.R + color.G + color.B) / 3;
			return $"{name} 이미지좌표(좌상)[{imagePoint.X},{imagePoint.Y}] GV[{brightness}]";
		}

		private static bool IsPointInBitmap(Bitmap image, DrawingPoint imagePoint)
		{
			return image != null &&
				image.Width > 10 &&
				image.Height > 10 &&
				imagePoint.X >= 0 &&
				imagePoint.Y >= 0 &&
				imagePoint.X < image.Width &&
				imagePoint.Y < image.Height;
		}

		private static string GetModeText(CompareMode mode)
		{
			return mode == CompareMode.Image1 ? "LEFT" : "RIGHT";
		}

		private void RefreshMarkers()
		{
			imageViewer1?.RefreshGL();
			imageViewer2?.RefreshGL();
		}

		private void DrawMarker(CompareMode mode, ImageCanvasControl viewer)
		{
			if (!isMarkerVisible) { return; }

			Bitmap image = GetBitmap(mode);
			if (!IsPointInBitmap(image, markerImagePoint)) { return; }

			viewer.DrawImagePointMarker(markerImagePoint, image.Height);
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
							string[] imagePaths = AppUtil.LoadImagesFilePath();
				if (imagePaths == null || imagePaths.Length < 2) { return; }

				ReplaceBitmap(ref imageCompare1, imagePaths[0]);
				ReplaceBitmap(ref imageCompare2, imagePaths[1]);

				LoadBitmapToViewer(imageViewer1, imageCompare1, "CompareImage1", ref imageSize1);
				LoadBitmapToViewer(imageViewer2, imageCompare2, "CompareImage2", ref imageSize2);
				SyncViewStateFrom(CompareMode.Image1);
				isMarkerVisible = false;
				lbRGB.Text = "색상 : -";
				if (colorSwatch != null) { colorSwatch.BackColor = Color.Black; }
				lbXY.Text = "LEFT 이미지좌표(좌상)[-,-] GV[-]";
				lbGV.Text = "RIGHT 이미지좌표(좌상)[-,-] GV[-]";
			
		}

		private static void ReplaceBitmap(ref Bitmap target, string imagePath)
		{
			Bitmap previous = target;
			using (Bitmap fileBitmap = new Bitmap(imagePath))
			{
				target = (Bitmap)fileBitmap.Clone();
			}
			previous?.Dispose();
		}

		private static void LoadBitmapToViewer(ImageCanvasControl viewer, Bitmap bitmap, string imageName, ref DrawingSize imageSize)
		{
			if (viewer == null || bitmap == null) { return; }

			viewer.ClearTexture();
			using (CvMat mat = BitmapImageConverter.ToMat(bitmap))
			{
				CanvasImageLoader.UploadMatAsTexture(viewer, mat, imageName, ref imageSize);
			}
		}

		private void LayerDisplay_FormClosed(object sender, FormClosedEventArgs e)
		{
			imageViewer1?.ClearTexture();
			imageViewer2?.ClearTexture();
			imageCompare1?.Dispose();
			imageCompare2?.Dispose();
		}
	}
}
