using Lib.Common;
using OpenVisionLab._1._Core;
using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.Rendering;
using OpenVisionLab.Logging;
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
		private Panel emptyImagePanel;
		private Label emptyImageTitle;
		private Label emptyImageDescription;
		private Button emptyImageLoadButton;
		private Panel colorSwatch;
		private RJCodeUI_M1.RJControls.RJLabel lbLayerInfo;
		private ToolTip viewerStatusToolTip;
		private Bitmap currentImage;
		private Size imageSize = Size.Empty;
		private Timer fitAfterLayoutTimer;
		private bool pendingImageLoad;
		private bool pendingZoomToFit = true;
		private bool waitingForStableFit;
		private bool imageChanged;
		private bool isEmptyPlaceholderImage;

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
			TabText = Text;
			ToolTipText = Text;

			CloseButton = UseCloseButton;
			CloseButtonVisible = UseCloseButton;

			if (ImageSource != null)
			{
				SetImage(ImageSource);
			}
		}

		public void SetImage(Bitmap image, bool zoomToFit = true)
		{
			currentImage = CreateCanvasBitmap(image);
			isEmptyPlaceholderImage = DisplayManagerImageExtensions.IsPlaceholderBitmap(currentImage);
			imageChanged = true;
			SyncImageSpace();
			UpdateEmptyImageOverlay();

			if (currentImage == null || isEmptyPlaceholderImage)
			{
				pendingImageLoad = false;
				pendingZoomToFit = false;
				imageSize = Size.Empty;
				imageViewer?.ClearTexture();
				imageViewer?.RefreshGL();
				return;
			}

			if (!CanLoadTexture())
			{
				pendingImageLoad = true;
				pendingZoomToFit = zoomToFit;
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

		protected override string GetPersistString()
		{
			return $"{typeof(FormLayerDisplay).FullName}:{Text}";
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
			panel3.BackColor = Color.Black;

			imageViewer.Draw += (sender, e) => imageViewer.DrawContent();
			imageViewer.Resized += OnImageViewerResized;
			imageViewer.MouseDown += OnImageViewerMouseDown;
			imageViewer.MouseUp += OnImageViewerMouseUp;
			imageViewer.MouseWheel += OnImageViewerMouseWheel;
			InitializeImageContextMenu();
			imageViewer.ContextMenuStrip = imageContextMenu;

			panel3.Controls.Add(imageViewer);
			InitializeEmptyImageOverlay();
			InitializeFitAfterLayoutTimer();
		}

		private void InitializeEmptyImageOverlay()
		{
			emptyImagePanel = new Panel
			{
				Dock = DockStyle.Fill,
				BackColor = Color.FromArgb(13, 18, 25),
				Visible = false
			};
			emptyImagePanel.Paint += EmptyImagePanel_Paint;
			emptyImagePanel.Resize += (sender, e) => LayoutEmptyImageOverlay();
			emptyImagePanel.MouseUp += (sender, e) =>
			{
				if (e.Button == MouseButtons.Right)
				{
					ShowImageContextMenu(emptyImagePanel, new CanvasMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta));
				}
			};

			emptyImageTitle = new Label
			{
				AutoSize = false,
				Text = "이미지 없음",
				Font = new Font("Segoe UI", 17F, FontStyle.Bold, GraphicsUnit.Point),
				ForeColor = Color.FromArgb(231, 241, 250),
				BackColor = Color.Transparent,
				TextAlign = ContentAlignment.MiddleCenter
			};

			emptyImageDescription = new Label
			{
				AutoSize = false,
				Text = "이미지를 불러오면 이 영역에 표시됩니다.",
				Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
				ForeColor = Color.FromArgb(143, 162, 187),
				BackColor = Color.Transparent,
				TextAlign = ContentAlignment.MiddleCenter
			};

			emptyImageLoadButton = new Button
			{
				AutoSize = false,
				Text = "이미지 불러오기",
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point),
				ForeColor = Color.FromArgb(238, 248, 255),
				BackColor = Color.FromArgb(34, 94, 130),
				Cursor = Cursors.Hand
			};
			emptyImageLoadButton.FlatAppearance.BorderColor = Color.FromArgb(80, 165, 210);
			emptyImageLoadButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(44, 116, 154);
			emptyImageLoadButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(29, 77, 110);
			emptyImageLoadButton.Click += OnImageLoadClicked;

			emptyImagePanel.Controls.Add(emptyImageTitle);
			emptyImagePanel.Controls.Add(emptyImageDescription);
			emptyImagePanel.Controls.Add(emptyImageLoadButton);
			panel3.Controls.Add(emptyImagePanel);
			emptyImagePanel.BringToFront();
			LayoutEmptyImageOverlay();
		}

		private void EmptyImagePanel_Paint(object sender, PaintEventArgs e)
		{
			if (emptyImagePanel == null) { return; }

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			int iconSize = GetEmptyImageIconSize();
			int iconX = (emptyImagePanel.ClientSize.Width - iconSize) / 2;
			int iconY = GetEmptyImageContentTop(iconSize);
			Rectangle iconBounds = new Rectangle(iconX, iconY, iconSize, iconSize);

			using (Pen borderPen = new Pen(Color.FromArgb(58, 78, 104), 2F))
			using (Pen accentPen = new Pen(Color.FromArgb(89, 189, 222), 2F))
			using (SolidBrush fillBrush = new SolidBrush(Color.FromArgb(20, 28, 39)))
			{
				e.Graphics.FillRectangle(fillBrush, iconBounds);
				e.Graphics.DrawRectangle(borderPen, iconBounds);
				e.Graphics.DrawLine(accentPen, iconBounds.Left + 14, iconBounds.Bottom - 18, iconBounds.Left + iconSize / 2, iconBounds.Top + iconSize / 2);
				e.Graphics.DrawLine(accentPen, iconBounds.Left + iconSize / 2, iconBounds.Top + iconSize / 2, iconBounds.Right - 14, iconBounds.Bottom - 24);
				e.Graphics.DrawEllipse(accentPen, iconBounds.Right - 26, iconBounds.Top + 14, 10, 10);
			}
		}

		private void LayoutEmptyImageOverlay()
		{
			if (emptyImagePanel == null || emptyImageTitle == null || emptyImageDescription == null || emptyImageLoadButton == null)
			{
				return;
			}

			int width = Math.Min(460, Math.Max(240, emptyImagePanel.ClientSize.Width - 48));
			int x = Math.Max(12, (emptyImagePanel.ClientSize.Width - width) / 2);
			int iconSize = GetEmptyImageIconSize();
			int y = GetEmptyImageContentTop(iconSize) + iconSize + 18;

			emptyImageTitle.SetBounds(x, y, width, 34);
			emptyImageDescription.SetBounds(x, y + 40, width, 24);
			emptyImageLoadButton.SetBounds(Math.Max(8, (emptyImagePanel.ClientSize.Width - 134) / 2), y + 76, 134, 30);
			emptyImagePanel.Invalidate();
		}

		private int GetEmptyImageIconSize()
		{
			if (emptyImagePanel == null) { return 72; }

			int basis = Math.Min(emptyImagePanel.ClientSize.Width, emptyImagePanel.ClientSize.Height);
			return Math.Min(86, Math.Max(52, basis / 5));
		}

		private int GetEmptyImageContentTop(int iconSize)
		{
			if (emptyImagePanel == null) { return 24; }

			const int textAndButtonHeight = 124;
			int contentHeight = iconSize + 18 + textAndButtonHeight;
			return Math.Max(18, (emptyImagePanel.ClientSize.Height - contentHeight) / 2);
		}

		private void UpdateEmptyImageOverlay()
		{
			if (emptyImagePanel == null)
			{
				return;
			}

			if (emptyImageDescription != null)
			{
				emptyImageDescription.Text = string.Equals(Text, "Main", StringComparison.OrdinalIgnoreCase)
					? "기준 이미지를 불러오면 이 영역에 표시됩니다."
					: "레이어 이미지를 불러오면 이 영역에 표시됩니다.";
			}

			emptyImagePanel.Visible = isEmptyPlaceholderImage;
			if (isEmptyPlaceholderImage)
			{
				emptyImagePanel.BringToFront();
				LayoutEmptyImageOverlay();
				return;
			}

			imageViewer?.BringToFront();
		}

		private void InitializeFitAfterLayoutTimer()
		{
			fitAfterLayoutTimer = new Timer
			{
				Interval = 80
			};
			fitAfterLayoutTimer.Tick += OnFitAfterLayoutTimerTick;
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

			LoadCurrentImage(pendingZoomToFit);
		}

		private void LoadCurrentImage(bool zoomToFit)
		{
			if (currentImage == null || imageViewer == null) { return; }

			try
			{
				pendingImageLoad = false;
				Size previousImageSize = imageSize;

				using (Bitmap textureImage = CreateCanvasBitmap(currentImage))
				using (CvMat mat = BitmapImageConverter.ToMat(textureImage))
				{
					CanvasImageLoader.UploadMatAsTexture(imageViewer, mat, LayerImageName, ref imageSize, false);
				}

				if (zoomToFit)
				{
					if (previousImageSize == imageSize && imageViewer.Visible)
					{
						imageViewer.ZoomToFit();
					}
					else
					{
						BeginStableZoomToFit();
					}
				}
				else
				{
					imageViewer.RefreshGL();
				}
			}
			catch (Exception)
			{
				pendingImageLoad = true;
				pendingZoomToFit = zoomToFit;

			}
		}

		private void BeginStableZoomToFit()
		{
			if (imageViewer == null || imageViewer.IsDisposed) { return; }

			waitingForStableFit = true;
			imageViewer.Visible = false;
			RestartFitAfterLayoutTimer();
		}

		private void OnImageViewerResized(object sender, EventArgs e)
		{
			if (!waitingForStableFit) { return; }

			RestartFitAfterLayoutTimer();
		}

		private void RestartFitAfterLayoutTimer()
		{
			if (fitAfterLayoutTimer == null) { return; }

			fitAfterLayoutTimer.Stop();
			fitAfterLayoutTimer.Start();
		}

		private void OnFitAfterLayoutTimerTick(object sender, EventArgs e)
		{
			fitAfterLayoutTimer.Stop();
			if (imageViewer == null || imageViewer.IsDisposed)
			{
				waitingForStableFit = false;
				return;
			}

			imageViewer.ZoomToFit();
			imageViewer.Visible = true;
			waitingForStableFit = false;
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
			string imagePath = AppUtil.LoadImageFilePath();
			if (string.IsNullOrEmpty(imagePath)) { return; }

			using (Bitmap loadedImage = new Bitmap(imagePath))
			{
				SetImage(CreateCanvasBitmap(loadedImage));
			}

			OVLog.Write(LogCategory.UI, LogLevel.Info, $"Image loaded. Layer={Text}, Path={imagePath}, Size={currentImage?.Width}x{currentImage?.Height}");
		}

		private void OnImageSaveClicked(object sender, EventArgs e)
		{
			if (DisplayManagerImageExtensions.IsPlaceholderBitmap(currentImage)) { return; }

			string imagePath = AppUtil.SaveImageFilePath();
			if (string.IsNullOrEmpty(imagePath)) { return; }

			currentImage.Save(imagePath);
			OVLog.Write(LogCategory.UI, LogLevel.Info, $"Image saved. Layer={Text}, Path={imagePath}, Size={currentImage.Width}x{currentImage.Height}");
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
			timePixelData.Interval = 50;
			panel2.Height = 24;
			panel2.BackColor = Color.FromArgb(16, 23, 32);
			panel2.Padding = new Padding(7, 3, 7, 3);
			panel2.Paint += ViewerStatusBar_Paint;
			panel2.Resize += (sender, e) => AdjustViewerStatusLayout();
			viewerStatusToolTip = new ToolTip
			{
				AutoPopDelay = 8000,
				InitialDelay = 300,
				ReshowDelay = 100,
				ShowAlways = true
			};

			lbLayerInfo = new RJCodeUI_M1.RJControls.RJLabel();

			ConfigureViewerStatusLabel(lbLayerInfo, 260, "레이어 - · -");
			ConfigureViewerStatusLabel(lbXY, 190, "캔버스 X:0 Y:0");
			ConfigureViewerStatusLabel(lbZOOM, 260, "이미지 X:0 Y:0 · 100%");
			ConfigureViewerStatusLabel(lbGV, 74, "GV 0");
			ConfigureViewerStatusLabel(lbRGB, 158, "RGB 0,0,0");

			colorSwatch = new Panel
			{
				Dock = DockStyle.Left,
				Width = 24,
				Height = 16,
				Margin = Padding.Empty,
				Padding = Padding.Empty,
				BackColor = Color.Black,
				BorderStyle = BorderStyle.FixedSingle
			};
			viewerStatusToolTip.SetToolTip(colorSwatch, "현재 마우스 위치의 픽셀 색상입니다.");

			panel2.Controls.Clear();
			panel2.Controls.Add(lbRGB);
			panel2.Controls.Add(colorSwatch);
			panel2.Controls.Add(lbGV);
			panel2.Controls.Add(lbZOOM);
			panel2.Controls.Add(lbXY);
			panel2.Controls.Add(lbLayerInfo);
			AdjustViewerStatusLayout();
		}

		private void ConfigureViewerStatusLabel(RJCodeUI_M1.RJControls.RJLabel label, int width, string text)
		{
			label.BorderStyle = BorderStyle.None;
			label.BackColor = Color.FromArgb(24, 34, 48);
			label.ForeColor = Color.FromArgb(231, 238, 248);
			label.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			label.Dock = DockStyle.Left;
			label.Width = width;
			label.TextAlign = ContentAlignment.MiddleLeft;
			label.Padding = new Padding(8, 0, 8, 0);
			label.Text = text;
			label.AutoEllipsis = true;
		}

		private void ViewerStatusBar_Paint(object sender, PaintEventArgs e)
		{
			using (Pen pen = new Pen(Color.FromArgb(42, 54, 72)))
			{
				e.Graphics.DrawLine(pen, 0, 0, panel2.Width, 0);
			}
		}

		private void AdjustViewerStatusLayout()
		{
			if (panel2 == null || lbLayerInfo == null || lbXY == null || lbZOOM == null || lbGV == null || lbRGB == null)
			{
				return;
			}

			int width = panel2.ClientSize.Width;
			bool compact = width < 980;
			bool narrow = width < 780;

			lbLayerInfo.Width = narrow ? 180 : compact ? 220 : 260;
			lbXY.Width = narrow ? 140 : compact ? 164 : 190;
			lbZOOM.Width = narrow ? 190 : compact ? 222 : 260;
			lbGV.Width = narrow ? 60 : 74;
			lbRGB.Width = narrow ? 128 : compact ? 142 : 158;
			colorSwatch.Width = narrow ? 20 : 24;
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
			if (displayManager == null || string.IsNullOrWhiteSpace(Text)) { return; }

			displayManager.FocusItem = Text;
			displayManager.SelectedItem = Text;
			displayManager.SetImageSrc(currentImage);
		}

		private void ResizeEvent(object sender, EventArgs e)
		{
			Visible = Width > MinimumSize.Width && Height > MinimumSize.Height;
		}

		private void LayerDisplay_FormClosed(object sender, FormClosedEventArgs e)
		{
			imageViewer?.ClearTexture();
			fitAfterLayoutTimer?.Dispose();
			fitAfterLayoutTimer = null;
			imageContextMenu?.Dispose();
			emptyImageLoadButton?.Dispose();
			emptyImageDescription?.Dispose();
			emptyImageTitle?.Dispose();
			emptyImagePanel?.Dispose();
			viewerStatusToolTip?.Dispose();
			viewerStatusToolTip = null;

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
			if (imageViewer == null) { return; }

			Color color = imageViewer.PixelColor;
			PointF pos = imageViewer.PixelPos;
			PointF imagePos = imageViewer.ImagePixelPos;
			string imageText = imageSize.IsEmpty ? "-" : $"{imageSize.Width}x{imageSize.Height}";

			if (isEmptyPlaceholderImage)
			{
				if (lbLayerInfo != null)
				{
					lbLayerInfo.Text = $"레이어 {Text} · 이미지 없음";
					viewerStatusToolTip?.SetToolTip(lbLayerInfo, $"현재 레이어: {Text}\r\n이미지 없음");
				}

				lbXY.Text = "캔버스 -";
				lbZOOM.Text = "이미지 - · -";
				lbGV.Text = "GV -";
				lbRGB.Text = "RGB -";
				colorSwatch.BackColor = Color.Black;
				viewerStatusToolTip?.SetToolTip(lbXY, "이미지가 로드되면 캔버스 좌표가 표시됩니다.");
				viewerStatusToolTip?.SetToolTip(lbZOOM, "이미지가 로드되면 이미지 좌표와 확대율이 표시됩니다.");
				viewerStatusToolTip?.SetToolTip(lbGV, "이미지가 로드되면 Gray Value가 표시됩니다.");
				viewerStatusToolTip?.SetToolTip(lbRGB, "이미지가 로드되면 RGB 값이 표시됩니다.");
				viewerStatusToolTip?.SetToolTip(colorSwatch, "이미지가 로드되면 픽셀 색상이 표시됩니다.");
				return;
			}

			if (lbLayerInfo != null)
			{
				lbLayerInfo.Text = $"레이어 {Text} · {imageText}";
				viewerStatusToolTip?.SetToolTip(lbLayerInfo, $"현재 레이어: {Text}\r\n이미지 크기: {imageText}");
			}

			string canvasText = string.Format("캔버스 X:{0} Y:{1}", (int)pos.X, (int)pos.Y);
			string imagePositionText = string.Format("이미지 X:{0} Y:{1} · {2:0}%", (int)imagePos.X, (int)imagePos.Y, imageViewer.ZoomScale * 100F);
			string gvText = string.Format("GV {0}", imageViewer.GrayValue);
			string rgbText = string.Format("RGB {0},{1},{2}", color.R, color.G, color.B);

			lbXY.Text = canvasText;
			lbZOOM.Text = imagePositionText;
			lbGV.Text = gvText;
			lbRGB.Text = rgbText;
			viewerStatusToolTip?.SetToolTip(lbXY, $"캔버스 좌표(Bottom-Left 기준)\r\n{canvasText}");
			viewerStatusToolTip?.SetToolTip(lbZOOM, $"이미지 좌표(Top-Left 기준) 및 확대율\r\n{imagePositionText}");
			viewerStatusToolTip?.SetToolTip(lbGV, $"Gray Value\r\n{gvText}");
			viewerStatusToolTip?.SetToolTip(lbRGB, $"RGB 값\r\n{rgbText}");
			if (colorSwatch != null)
			{
				colorSwatch.BackColor = color;
				viewerStatusToolTip?.SetToolTip(colorSwatch, $"픽셀 색상\r\nR:{color.R} G:{color.G} B:{color.B}");
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





