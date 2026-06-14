using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.Model;
using OpenVisionLab.ImageCanvas.Canvas;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab.ImageCanvas.Rendering
{
	public partial class ImageCanvasControl : UserControl
	{
		#region Events
		public event EventHandler<OpenVisionLab.ImageCanvas.Canvas.CanvasRenderEventArgs> Draw;
		public event EventHandler Resized = delegate { };
		public event EventHandler MouseDoubleClicked = delegate { };
		public event EventHandler MouseClicked = delegate { };

		public new event EventHandler Load = delegate { };
		public new event EventHandler MouseLeave = delegate { };
		public new event EventHandler<KeyEventArgs> KeyDown = delegate { };
		public new event EventHandler<KeyEventArgs> KeyUp = delegate { };
		public new event EventHandler<CanvasMouseEventArgs> MouseMove = delegate { };
		public new event EventHandler<CanvasMouseEventArgs> MouseDown = delegate { };
		public new event EventHandler<CanvasMouseEventArgs> MouseUp = delegate { };
		public new event EventHandler<CanvasMouseEventArgs> MouseWheel = delegate { };
		#endregion

		#region Constants
		private const float MIN_ZOOM_SCALE = 5000F;

		#endregion

		#region DllImport
		[DllImport("opengl32.dll", SetLastError = true)]
		public static extern void glTexSubImage2D(
				uint target,
				int level,
				int xoffset,
				int yoffset,
				int width,
				int height,
				uint format,
				uint type,
				IntPtr pixels
				);

		[System.Runtime.InteropServices.DllImport("opengl32.dll", EntryPoint = "glTexImage2D", ExactSpelling = true)]
		internal extern static void glTexImage2D(int target, int level, int internalformat, Int32 width, Int32 height, int border, int format, int type, IntPtr pixels);

		[DllImport("opengl32.dll", SetLastError = true)] private static extern void glGetTexImage(uint target, int level, uint format, uint type, byte[] pixels);


		private List<EraserParameter> _eraserParameters = new List<EraserParameter>();

		private List<System.Drawing.PointF> _penPoints = new List<System.Drawing.PointF>();

		#endregion

		#region Variables


		/// <summary>
		/// 줌 관련 값입니다.
		/// </summary>
		public float ZoomScale
		{
			get => _zoom / GetControlMinSize();
		}

		public System.Drawing.RectangleF _fitRect = new System.Drawing.RectangleF();
		private float _zoom = 100000.0f;
		private float _aspectRatio;
		private float _xSpan;
		private float _ySpan;

		/// <summary>
		/// 텍스처 관련 값입니다.
		/// </summary>
		private ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> _textureAreas = new ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>>();
		public ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> TextureAreas
		{
			get => _textureAreas;
			set => _textureAreas = value;
		}
		private List<string> _textureKeysOrder = new List<string>();
		public List<string> TextureKeysOrder
		{
			get => _textureKeysOrder;
			set => _textureKeysOrder = value;
		}

		/// <summary>
		/// 드로잉 관련 값입니다.
		/// </summary>
		private System.Drawing.SizeF _offsetSize = new System.Drawing.SizeF(0.0f, 0.0f);

		/// <summary>
		/// OpenGL은 직접 텍스트 렌더링을 지원하지 않으므로 폰트 비트맵을 display list로 만들어 사용합니다.
		/// </summary>
		private readonly List<OpenGlFontBitmapEntry> _fontBitmapEntries = new List<OpenGlFontBitmapEntry>();
		private readonly object _pixelDatalock = new object();

		private System.Drawing.PointF _pixelPos;
		private System.Drawing.PointF _imagePixelPos;
		private System.Drawing.Color _pixelColor;
		private int _grayValue;
		private bool _isShowCrossLine = false;
		protected int _invertValue = 1; // Y축 방향이 좌하단 기준일 때 좌표를 반전하기 위한 값입니다.

		private CanvasInteractionMode _viewMode = CanvasInteractionMode.None;
		private System.Drawing.PointF _preMousePos;
		private System.Drawing.PointF _postMousePos;

		private CanvasOverlayManager _overlayManager = new CanvasOverlayManager();
		private List<CanvasShape> _shapesViewPort = new List<CanvasShape>();
		private bool _suppressRefresh;

		public float PixelPermm { get; set; } = 0.001f;
		public float HandleSize = 10; // 기본 핸들 크기

		#endregion

		#region Properties
		public bool IsShowCrossLine
		{
			get => _isShowCrossLine;
			set => _isShowCrossLine = value;
		}

		public int GrayValue
		{
			get => _grayValue;
			set => _grayValue = value;
		}

		public System.Drawing.PointF PixelPos
		{
			get => _pixelPos;
			set => _pixelPos = value;
		}

		public System.Drawing.PointF ImagePixelPos
		{
			get => _imagePixelPos;
			set => _imagePixelPos = value;
		}

		public System.Drawing.Color PixelColor
		{
			get => _pixelColor;
			set => _pixelColor = value;
		}

		public bool InvertYAxis
		{
			get { return _invertValue == 1; }
			set
			{
				if (value == true)
					_invertValue = -1;
				else
					_invertValue = 1;
			}
		}
		public System.Drawing.PointF PreMousePos
		{
			get => _preMousePos;
			set => _preMousePos = value;
		}

		public System.Drawing.PointF PostMousePos
		{
			get => _postMousePos;
			set => _postMousePos = value;
		}

		#endregion

		#region Contructor
		public ImageCanvasControl()
		{
			InitializeComponent();

			DoubleBuffered = false;

			openGLControl.Load += OnLoad;
			openGLControl.Resized += OnResized;
			openGLControl.MouseDoubleClick += OnMouseDoubleClick;
			openGLControl.OpenGLDraw += OnDraw;
			openGLControl.KeyUp += OnKeyUp;
			openGLControl.KeyDown += OnKeyDown;
			openGLControl.MouseClick += OnMouseClick;
			openGLControl.MouseDown += OnMouseDown;
			openGLControl.MouseMove += OnMouseMove;
			openGLControl.MouseUp += OnMouseUp;
			openGLControl.MouseWheel += OnMouseWheel;
			openGLControl.MouseLeave += OnMouseLeave;
		}
		#endregion

		#region Event Handlers
		protected void OnLoad(object sender, EventArgs e) => Load(sender, e);
		protected void OnResized(object sender, EventArgs e) => Resized(sender, e);
		protected void OnMouseDoubleClick(object sender, MouseEventArgs e)
		{
			ZoomToFit();
			MouseDoubleClicked(sender, e);
		}

		protected void OnDraw(object sender, SharpGL.RenderEventArgs args)
		{
			OpenGL gl = openGLControl.OpenGL;
			PrepareForDrawing(gl);
			//DrawContent(gl);
			Draw?.Invoke(sender, new OpenVisionLab.ImageCanvas.Canvas.CanvasRenderEventArgs(gl));
		}

		protected void OnKeyUp(object sender, KeyEventArgs e) => KeyUp(sender, e);
		protected void OnKeyDown(object sender, KeyEventArgs e) => KeyDown(sender, e);
		protected void OnMouseClick(object sender, MouseEventArgs e) => MouseClicked(sender, e);
		protected void OnMouseDown(object sender, MouseEventArgs e) => MouseDown(sender, new CanvasMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta));

		protected void OnMouseMove(object sender, MouseEventArgs e)
		{
			UpdatePixelStatus(e);
			if (_viewMode == CanvasInteractionMode.Drag) { DragViewMovement(e); }
			MouseMove(sender, new CanvasMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta));
			CalculatorVisibleOverlays();
			openGLControl.Refresh();
		}

		protected void OnMouseUp(object sender, MouseEventArgs e)
		{
			MouseUp(sender, new CanvasMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta));
			ResetMousePositions();
		}

		protected void OnMouseLeave(object sender, EventArgs e) => MouseLeave(sender, e);

		protected void OnMouseWheel(object sender, MouseEventArgs e)
		{
			MouseWheel(sender, new CanvasMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta));
			OpenGlDrawing.ZoomFactor = ZoomScale;
		}


		#endregion

		private void PrepareForDrawing(OpenGL gl)
		{
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
			gl.LoadIdentity();
			gl.Translate(_offsetSize.Width, _offsetSize.Height, -0f);
			gl.Disable(OpenGL.GL_DEPTH_TEST);
		}

		#region // public methods
		public string GetNameGL() => openGLControl.Name;
		public void SetNameGL(string name) => openGLControl.Name = name;
		public System.Drawing.Size GetSize() => openGLControl.Size;
		public OpenGL GetOpenGL() => openGLControl.OpenGL;
		public OpenGLControl GetOpenGLControl() => openGLControl;
		public CanvasOverlayManager GetCanvasOverlayManager() => _overlayManager;
		public void SetViewMode(CanvasInteractionMode enumViewMode) => _viewMode = enumViewMode;
		public CanvasInteractionMode GetViewMode() => _viewMode;
		public void SetToFitRect(System.Drawing.RectangleF rect) => _fitRect = rect;

		public OpenGlTextDrawOptions GetOpenGlTextDrawOptions()
		{
			return new OpenGlTextDrawOptions(_fontBitmapEntries, _xSpan, _ySpan, _offsetSize, ZoomScale);
		}

		public System.Drawing.Size GetMaxTextureSize()
		{
			OpenGL gl = GetOpenGL();

			int[] maxTextureSize = new int[1];
			gl.GetInteger(OpenGL.GL_MAX_TEXTURE_SIZE, maxTextureSize);

			return new System.Drawing.Size(maxTextureSize[0], maxTextureSize[0]);
		}

		public void RefreshGL()
		{
			if (_suppressRefresh)
			{
				return;
			}

			if (this.InvokeRequired)
			{
				this.BeginInvoke(new MethodInvoker(() =>
				{
					RefreshGL();
				}));
			}
			else
			{
				CalculatorVisibleOverlays();
				openGLControl.Refresh();
			}
		}

		public uint AddODBTexture(IntPtr monoData, IntPtr colorData, IntPtr maskData, IntPtr maskSetColorData, int x, int y, int width, int height, int imageWidth, int imageHeight, int offsetHeight,
			uint bpp1, uint bpp2, string imageName, System.Drawing.Size fullScreen, System.Drawing.Size titleSize, bool useMaskBackgroundWhite = true)
		{
			OpenGL gl = openGLControl.OpenGL;
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			int textureY = (offsetHeight - y);

			uint colorBpp = 4;

			// OpenGL 텍스처를 생성합니다.
			uint oriTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, bpp1);
			uint oriBackgroundTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, bpp2);
			uint maskTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, colorBpp);
			uint backgroundMaskTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, colorBpp);

			// OpenGL 텍스처를 생성합니다.
			UpdateTexture(monoData, width, height, bpp1, oriTextureId);
			UpdateTexture(colorData, width, height, bpp2, oriBackgroundTextureId);
			UpdateTexture(maskData, width, height, colorBpp, maskTextureId);
			UpdateTexture(maskSetColorData, width, height, colorBpp, backgroundMaskTextureId);

			OpenGlTextureDrawingParam glParam = new OpenGlTextureDrawingParam
			{
				OriTextureId = oriTextureId,
				OriBackgroundTextureId = oriBackgroundTextureId,
				MaskTextureId = maskTextureId,
				MaskBackgroundMaskTextureId = backgroundMaskTextureId,
				BppOriginal = bpp1,
				BppBackground = bpp2,
				ImageName = imageName,
				GLDrawingTextureArea = new System.Drawing.RectangleF(x, textureY, width, height * -1),
				GLTextureArea = new RectangleF(x, textureY - height, width, height),
				ImageTexutreArea = new Rectangle(x, y, width, height),
				IsVisible = true,
				TextureFullScreen = fullScreen,
				TitleSize = titleSize,
			};

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			var textureArea = glParam.GLTextureArea;
			var fullSize = glParam.TextureFullScreen;

			System.Drawing.Size tileSize = glParam.TitleSize;

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			int offsetX = (int)(textureArea.X / tileSize.Width) * tileSize.Width;
			int offsetY = 0;
			if (tileSize.Height > textureArea.Y + textureArea.Height)
			{
				offsetY = (int)0;
			}
			else
			{
				offsetY = (int)textureArea.Y;
			}

			glParam.ImageTitleOffset = new DotInfo(offsetX, offsetY);
			if (!_textureAreas.ContainsKey(imageName))
			{
				List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();
				list.Add(glParam);
				_textureAreas.TryAdd(imageName, list);
				//_textureKeysOrder.Add(imageName);
				_textureKeysOrder.Insert(0, imageName);
			}
			else
			{
				List<OpenGlTextureDrawingParam> glTextureDrawingParams = new List<OpenGlTextureDrawingParam>();
				_textureAreas.TryGetValue(imageName, out glTextureDrawingParams);
				glTextureDrawingParams.Add(glParam);
				_textureAreas[imageName] = glTextureDrawingParams;
			}

			// 좌하단 기준으로 y 좌표를 계산합니다.
			System.Drawing.RectangleF imageArea = CalculateBoundingRectangle(_textureAreas);
			SetToFitRect(imageArea);
			return oriTextureId;
		}

		public uint AddTexture(IntPtr data, int x, int y, int width, int height, int imageWidth, int imageHeight, int offsetHeight,
			uint bpp, string imageName, System.Drawing.Size fullScreen, System.Drawing.Size titleSize)
		{
			OpenGL gl = openGLControl.OpenGL;
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			int textureY = (offsetHeight - y);

			// OpenGL 텍스처를 생성합니다.
			uint oriTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, bpp);

			// OpenGL 텍스처를 생성합니다.
			UpdateTexture(data, width, height, bpp, oriTextureId);

			OpenGlTextureDrawingParam glParam = new OpenGlTextureDrawingParam
			{
				OriTextureId = oriTextureId,
				BppOriginal = bpp,
				ImageName = imageName,
				GLDrawingTextureArea = new System.Drawing.RectangleF(x, textureY, width, height * -1),
				GLTextureArea = new RectangleF(x, textureY - height, width, height),
				ImageTexutreArea = new Rectangle(x, y, width, height),
				IsVisible = true,
				TextureFullScreen = fullScreen,
				TitleSize = titleSize,
			};

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			var textureArea = glParam.GLTextureArea;
			var fullSize = glParam.TextureFullScreen;

			System.Drawing.Size tileSize = glParam.TitleSize;

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			int offsetX = (int)(textureArea.X / tileSize.Width) * tileSize.Width;
			int offsetY = 0;
			if (tileSize.Height > textureArea.Y + textureArea.Height)
			{
				offsetY = (int)0;
			}
			else
			{
				offsetY = (int)textureArea.Y;
			}

			glParam.ImageTitleOffset = new DotInfo(offsetX, offsetY);
			if (!_textureAreas.ContainsKey(imageName))
			{
				List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();
				list.Add(glParam);
				_textureAreas.TryAdd(imageName, list);
				//_textureKeysOrder.Add(imageName);
				_textureKeysOrder.Insert(0, imageName);
			}
			else
			{
				List<OpenGlTextureDrawingParam> glTextureDrawingParams = new List<OpenGlTextureDrawingParam>();
				_textureAreas.TryGetValue(imageName, out glTextureDrawingParams);
				glTextureDrawingParams.Add(glParam);
				_textureAreas[imageName] = glTextureDrawingParams;
			}

			// 좌하단 기준으로 y 좌표를 계산합니다.
			System.Drawing.RectangleF imageArea = CalculateBoundingRectangle(_textureAreas);
			SetToFitRect(imageArea);
			return oriTextureId;
		}

		public uint AddTexture(IntPtr monoData, IntPtr colorData, int x, int y, int width, int height, int imageWidth, int imageHeight, int offsetHeight,
			uint bpp1, uint bpp2, string imageName, System.Drawing.Size fullScreen, System.Drawing.Size titleSize, bool useMaskBackgroundWhite = true)
		{
			OpenGL gl = openGLControl.OpenGL;
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			int textureY = (offsetHeight - y);

			// OpenGL 텍스처를 생성합니다.
			uint oriTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, bpp1);
			uint oriBackgroundTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, bpp2);
			uint maskTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, 3);
			uint backgroundMaskTextureId = GenerateOpenGLTexture(imageWidth, imageHeight, 3);

			// 원본 크기와 같은 Mat을 생성합니다.
			OpenCvSharp.Mat combinedMat = new OpenCvSharp.Mat(height, width, OpenCvSharp.MatType.CV_8UC1);

			// OpenGL 텍스처를 생성합니다.
			UpdateTexture(monoData, width, height, bpp1, oriTextureId);
			UpdateTexture(colorData, width, height, bpp2, oriBackgroundTextureId);
			UpdateTexture(combinedMat.Data, width, height, 1, maskTextureId);
			UpdateTexture(combinedMat.Data, width, height, 1, backgroundMaskTextureId);

			OpenGlTextureDrawingParam glParam = new OpenGlTextureDrawingParam
			{
				OriTextureId = oriTextureId,
				OriBackgroundTextureId = oriBackgroundTextureId,
				MaskTextureId = maskTextureId,
				MaskBackgroundMaskTextureId = backgroundMaskTextureId,
				BppOriginal = bpp1,
				BppBackground = bpp2,
				ImageName = imageName,
				GLDrawingTextureArea = new System.Drawing.RectangleF(x, textureY, width, height * -1),
				GLTextureArea = new RectangleF(x, textureY - height, width, height),
				ImageTexutreArea = new Rectangle(x, y, width, height),
				IsVisible = true,
				TextureFullScreen = fullScreen,
				TitleSize = titleSize,
			};

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			var textureArea = glParam.GLTextureArea;
			var fullSize = glParam.TextureFullScreen;

			System.Drawing.Size tileSize = glParam.TitleSize;

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			int offsetX = (int)(textureArea.X / tileSize.Width) * tileSize.Width;
			int offsetY = 0;
			if (tileSize.Height > textureArea.Y + textureArea.Height)
			{
				offsetY = (int)0;
			}
			else
			{
				offsetY = (int)textureArea.Y;
			}

			glParam.ImageTitleOffset = new DotInfo(offsetX, offsetY);
			if (!_textureAreas.ContainsKey(imageName))
			{
				List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();
				list.Add(glParam);
				_textureAreas.TryAdd(imageName, list);
				//_textureKeysOrder.Add(imageName);
				_textureKeysOrder.Insert(0, imageName);
			}
			else
			{
				List<OpenGlTextureDrawingParam> glTextureDrawingParams = new List<OpenGlTextureDrawingParam>();
				_textureAreas.TryGetValue(imageName, out glTextureDrawingParams);
				glTextureDrawingParams.Add(glParam);
				_textureAreas[imageName] = glTextureDrawingParams;
			}

			// 좌하단 기준으로 y 좌표를 계산합니다.
			System.Drawing.RectangleF imageArea = CalculateBoundingRectangle(_textureAreas);
			SetToFitRect(imageArea);
			return oriTextureId;
		}

		public void DeleteTexture(string imageName)
		{
			if (_textureAreas.ContainsKey(imageName))
			{
				ImageCanvasTextureStore.DeleteTextures(openGLControl.OpenGL, ImageCanvasTextureStore.CollectTextureIds(_textureAreas[imageName]));
				List<OpenGlTextureDrawingParam> glTextureDrawingParams = new List<OpenGlTextureDrawingParam>();
				_textureAreas.TryRemove(imageName, out glTextureDrawingParams);
				_textureKeysOrder.Remove(imageName);

				RefreshGL();
			}
		}

		public void DeleteTexture(List<uint> textureIds)
		{
			ImageCanvasTextureStore.DeleteTextures(openGLControl.OpenGL, textureIds);
		}

		public uint? GetTextureIdAtPoint(int mouseX, int mouseY)
		{
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.
					var textureArea = param.GLTextureArea;
					if (textureArea.Contains(mouseX, mouseY))
					{
						return param.OriTextureId;
					}
				}
			}
			return null; // 해당 위치에 맞는 텍스처가 없으면 null을 반환합니다.
		}

		/// <summary>
		/// 텍스처 관련 값입니다.
		/// </summary>
		/// <param name="mouseX"></param>
		/// <param name="mouseY"></param>
		/// <returns></returns>
		public OpenGlTextureDrawingParam GetTextureParameterAtPoint(string textureName, int mouseX, int mouseY)
		{
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.
					var textureArea = param.GLTextureArea;
					if (textureArea.Contains(mouseX, mouseY) && kvp.Key == textureName)
					{
						return param;
					}
				}
			}
			return null; // 해당 위치에 맞는 텍스처가 없으면 null을 반환합니다.
		}

		public OpenGlTextureDrawingParam GetTextureParameterAtPoint(uint textureId)
		{
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.
					var textureArea = param.GLTextureArea;
					//System.Drawing.RectangleF rect = new RectangleF(textureArea.X, textureArea.Y, textureArea.Width, textureArea.Height * -1);
					if (param.OriBackgroundTextureId == textureId)
					{
						return param;
					}
				}
			}
			return null; // 해당 위치에 맞는 텍스처가 없으면 null을 반환합니다.
		}

		/// <summary>
		/// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
		/// </summary>
		/// <param name="mouseX"></param>
		/// <param name="mouseY"></param>
		/// <returns></returns>
		public List<OpenGlTextureDrawingParam> GetTextureParameterListAtPoint(string textureName, int mouseX, int mouseY)
		{
			List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.
					var textureArea = param.GLTextureArea;
					if (textureArea.Contains(mouseX, mouseY) && textureName == kvp.Key)
					{
						list.Add(param);
					}
				}
			}
			return list; // 조건에 맞는 텍스처가 없으면 현재 목록을 반환합니다.
		}

		/// <summary>
		/// 텍스처 관련 값입니다.
		/// OpenGL 텍스처를 생성합니다.
		/// </summary>
		/// <param name="textureName"></param>
		/// <param name="mouseX">원의 중심 X 좌표</param>
		/// <param name="mouseY">원의 중심 Y 좌표</param>
		/// <param name="radius">원의 반경</param>
		/// <returns></returns>
		public List<OpenGlTextureDrawingParam> GetTextureParameterListAtPoint(string textureName, int mouseX, int mouseY, float radius)
		{
			List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.

					var rect = GetRectangleFromCenterAndRadius(new PointF(mouseX, mouseY), radius);

					var textureArea = param.GLTextureArea;
					if (textureArea.IntersectsWith(rect) && textureName == kvp.Key)
					{
						list.Add(param);
					}
				}
			}

			return list;
		}

		public RectangleF GetRectangleFromCenterAndRadius(PointF center, float radius)
		{
			float left = center.X - radius;
			float top = center.Y - radius;
			float width = radius * 2;
			float height = radius * 2;

			return new RectangleF(left, top, width, height);
		}


		public List<OpenGlTextureDrawingParam> GetTextureParameterListAtRect(string textureName, System.Drawing.RectangleF rectangle)
		{
			System.Drawing.Rectangle newRect = new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
			return GetTextureParameterListAtRect(textureName, newRect);
		}

		public List<OpenGlTextureDrawingParam> GetTextureParameterListAtRect(System.Drawing.RectangleF rectangle)
		{
			System.Drawing.Rectangle newRect = new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
			return GetTextureParameterListAtRect(newRect);
		}

		public List<OpenGlTextureDrawingParam> GetTextureParameterListAtPoints(string textureName, List<DotInfo> points)
		{
			List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.
					var textureArea = param.GLTextureArea;

					foreach (var point in points)
					{
						// OpenGL 텍스처를 생성합니다.
						if (textureArea.Contains(point.X, point.Y) && kvp.Key == textureName)
						{
							list.Add(param);
							break; // OpenGL 텍스처를 생성합니다.
						}
					}
				}
			}

			return list; // 조건에 맞는 텍스처가 없으면 현재 목록을 반환합니다.
		}

		public List<OpenGlTextureDrawingParam> GetTextureParameterListAtPoints(List<DotInfo> points)
		{
			List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.
					var textureArea = param.GLTextureArea;

					foreach (var point in points)
					{
						// OpenGL 텍스처를 생성합니다.
						if (textureArea.Contains(point.X, point.Y))
						{
							list.Add(param);
							break; // OpenGL 텍스처를 생성합니다.
						}
					}
				}
			}

			return list; // 조건에 맞는 텍스처가 없으면 현재 목록을 반환합니다.
		}

		public List<OpenGlTextureDrawingParam> GetTextureParameterListAtRect(string textureName, System.Drawing.Rectangle rectangle)
		{
			List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.
					var textureArea = param.GLTextureArea;
					if (textureArea.IntersectsWith(rectangle) && textureName == kvp.Key)
					{
						list.Add(param);
					}
				}
			}
			return list; // 조건에 맞는 텍스처가 없으면 현재 목록을 반환합니다.
		}

		public List<OpenGlTextureDrawingParam> GetTextureParameterListAtRect(System.Drawing.Rectangle rectangle)
		{
			List<OpenGlTextureDrawingParam> list = new List<OpenGlTextureDrawingParam>();
			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			foreach (var kvp in _textureAreas)
			{
				foreach (var param in kvp.Value)
				{
					// 텍스처 관련 값입니다.
					var textureArea = param.GLTextureArea;
					if (textureArea.IntersectsWith(rectangle))
					{
						list.Add(param);
					}
				}
			}
			return list; // 조건에 맞는 텍스처가 없으면 현재 목록을 반환합니다.
		}

		public List<OpenGlTextureDrawingParam> GetTextureParameters(string imageName)
		{
			List<OpenGlTextureDrawingParam> glTextureDrawingParams = new List<OpenGlTextureDrawingParam>();

			_textureAreas.TryGetValue(imageName, out glTextureDrawingParams);

			return glTextureDrawingParams;
		}

		public void SetVisibleTexture(string imageName, bool isVisible)
		{
			if (!_textureAreas.ContainsKey(imageName)) { return; }
			_textureAreas[imageName].ForEach(texture => texture.IsVisible = isVisible);
			System.Drawing.RectangleF imageArea = CalculateBoundingRectangle(_textureAreas);
			SetToFitRect(imageArea);
		}

		public void SetTransparencyToTexture(string imageName, float transparency = 1.0f)
		{
			// 텍스처 관련 값입니다.
			var matchingParams = _textureAreas
				.SelectMany(kv => kv.Value)
				.Where(param => param.ImageName == imageName);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			foreach (var param in matchingParams)
			{
				param.IsTransParency = true;
				param.TransParency = transparency;
			}
			RefreshGL();
		}

		public void SetRotateToTexture(string imageName, float rotate = 1.0f)
		{
			// 텍스처 관련 값입니다.
			var matchingParams = _textureAreas
				.SelectMany(kv => kv.Value)
				.Where(param => param.ImageName == imageName);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			foreach (var param in matchingParams)
			{
				param.IsRotated = true;
				param.RotationAngle = rotate;
			}
			RefreshGL();
		}

		public float GetRotateToTexture(string imageName)
		{
			return _textureAreas
				.SelectMany(kv => kv.Value)
				.Where(param => param.ImageName == imageName).FirstOrDefault().RotationAngle;
		}

		[Obsolete("Use GetRotateToTexture instead.")]
		public float GetRoateToTexture(string imageName)
		{
			return GetRotateToTexture(imageName);
		}

		public float GetTransparencyToTexture(string imageName)
		{
			return _textureAreas
			.SelectMany(kv => kv.Value)
			.Where(param => param.ImageName == imageName).FirstOrDefault().TransParency;
		}

		public System.Drawing.RectangleF CalculateBoundingRectangle(ConcurrentDictionary<string, List<OpenGlTextureDrawingParam>> textureAreas)
		{
			// 최소/최대 좌표를 갱신해 전체 이미지 영역을 계산합니다.
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;

			foreach (var item in textureAreas.Values)
			{
				foreach (var param in item)
				{
					if (!param.IsVisible) { continue; }
					float newHeight = param.GLDrawingTextureArea.Height;
					if (param.GLDrawingTextureArea.Height < 0)
					{
						newHeight = newHeight * -1;
					}
					System.Drawing.RectangleF newItem = new System.Drawing.RectangleF(param.GLDrawingTextureArea.X, param.GLDrawingTextureArea.Y - newHeight, param.GLDrawingTextureArea.Width, newHeight);
					// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
					minX = Math.Min(minX, newItem.Left);
					minY = Math.Min(minY, newItem.Top);

					// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
					maxX = Math.Max(maxX, newItem.Right);
					maxY = Math.Max(maxY, newItem.Bottom);
				}
			}

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			return new System.Drawing.RectangleF(minX, minY, maxX - minX, maxY - minY);
		}

		public List<EraserParameter> GetEraserPoint()
		{
			return _eraserParameters;
		}

		#endregion

		#region // private methods
		private void UpdatePixelStatus(MouseEventArgs e, float scale = 1f)
		{
			lock (_pixelDatalock)
			{
				GrayValue = GetGrayValue(openGLControl.OpenGL, (int)(e.X / scale), (int)(e.Y / scale));
				PixelColor = GetScreenColor(openGLControl.OpenGL, (int)(e.X / scale), (int)(e.Y / scale));
				PixelPos = GetRoundPointF(GetCurrentRobotPos((int)(e.X / scale), (int)(e.Y / scale)));
				PixelPos = new System.Drawing.PointF(PixelPos.X, PixelPos.Y);
				ImagePixelPos = GetRoundPointF(ConvertOpenGlToImagePoint(PixelPos));

				//Console.WriteLine($"UpdatePixelStatus Ori: {e.X},{e.Y}");
				//Console.WriteLine($"UpdatePixelStatus Curr: {PixelPos.X},{PixelPos.Y}");
			}

		}

		private int GetGrayValue(OpenGL gl, int x, int y)
		{
			byte[] pixelData = new byte[4]; // RGBA

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			int invertedY = gl.RenderContextProvider.Height - y;

			gl.ReadPixels(x, invertedY, 1, 1, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, pixelData);

			// RGB 값을 0에서 255 사이의 값으로 변환합니다.
			// 이미지 영역을 좌하단 기준으로 설정합니다.
			///int grayValue = (int)(0.299 * pixelData[0] + 0.587 * pixelData[1] + 0.114 * pixelData[2]);

			int grayValue = (int)(pixelData[0] + pixelData[1] + pixelData[2]) / 3;

			return grayValue;
		}

		public System.Drawing.Color GetScreenColor(OpenGL gl, int x, int y)
		{
			byte[] pixelData = new byte[4]; // RGBA

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			int invertedY = gl.RenderContextProvider.Height - y;

			gl.ReadPixels(x, invertedY, 1, 1, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, pixelData);

			return System.Drawing.Color.FromArgb(pixelData[3], pixelData[0], pixelData[1], pixelData[2]);
		}


		public byte[] ReadTextureRegionColors(OpenGL gl, uint textureId, int x, int y, int readWidth, int readHeight, int textureWidth, int textureHeight)
		{
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

			int[] widthArr = new int[1];
			int[] heightArr = new int[1];
			int[] formatArr = new int[1];
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

			int internalFormat = formatArr[0];

			// PBO를 생성하고 바인딩합니다.
			uint[] pbo = new uint[1];
			gl.GenBuffers(1, pbo);
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			int bufferSize = readWidth * readHeight * 4; // RGBA for each pixel
			gl.BufferData(OpenGL.GL_PIXEL_PACK_BUFFER, bufferSize, IntPtr.Zero, OpenGL.GL_STREAM_READ);

			// RGB 값을 0에서 255 사이의 값으로 변환합니다.
			// 일반적인 가중치 방식으로 그레이스케일 값을 계산합니다.
			gl.ReadPixels(x, (textureHeight - 1) - (y + readHeight - 1), readWidth, readHeight, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);

			// GPU에서 픽셀 데이터를 읽어옵니다.
			byte[] pixelData = new byte[bufferSize];
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			IntPtr ptr = gl.MapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, OpenGL.GL_READ_ONLY);
			if (ptr != IntPtr.Zero)
			{
				Marshal.Copy(ptr, pixelData, 0, bufferSize);
				gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
			}
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0);
			gl.DeleteBuffers(1, pbo);

			byte[] flippedPixels = new byte[pixelData.Length];
			int bytesPerPixel = 4; // RGBA 형식 기준입니다.
			int stride = readWidth * bytesPerPixel;

			for (int row = 0; row < readHeight; row++)
			{
				int sourceIndex = row * stride;
				int destIndex = (readHeight - 1 - row) * stride;
				Array.Copy(pixelData, sourceIndex, flippedPixels, destIndex, stride);
			}
			return flippedPixels;
		}
		public byte[] ReadTextureRegionColors(OpenGL gl, uint textureId, PointF centerPoint, int halfSize, int width, int height)
		{
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

			int minX = (int)centerPoint.X - halfSize;
			int minY = (int)centerPoint.Y - halfSize;

			int maxX = (int)Math.Min(centerPoint.X + halfSize, width);
			int maxY = (int)Math.Min(centerPoint.Y + halfSize, height);

			int readWidth = maxX - minX + 1;
			int readHeight = maxY - minY + 1;


			// PBO를 생성하고 바인딩합니다.
			uint[] pbo = new uint[1];
			gl.GenBuffers(1, pbo);
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			int bufferSize = readWidth * readHeight * 4; // RGBA for each pixel
			gl.BufferData(OpenGL.GL_PIXEL_PACK_BUFFER, bufferSize, IntPtr.Zero, OpenGL.GL_STREAM_READ);

			// glReadPixels 호출
			gl.ReadPixels(minX, (height - 1) - maxY, readWidth, readHeight, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);

			// GPU에서 픽셀 데이터를 읽어옵니다.
			byte[] pixelData = new byte[bufferSize];
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			IntPtr ptr = gl.MapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, OpenGL.GL_READ_ONLY);
			if (ptr != IntPtr.Zero)
			{
				Marshal.Copy(ptr, pixelData, 0, bufferSize);
				gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
			}
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0);
			gl.DeleteBuffers(1, pbo);

			byte[] flippedPixels = new byte[pixelData.Length];
			int bytesPerPixel = 4; // RGBA 형식 기준입니다.
			int stride = readWidth * bytesPerPixel;

			for (int y = 0; y < readHeight; y++)
			{
				int sourceIndex = y * stride;
				int destIndex = (readHeight - 1 - y) * stride;
				Array.Copy(pixelData, sourceIndex, flippedPixels, destIndex, stride);
			}
			return flippedPixels;
		}

		public System.Drawing.Color ReadTextureColor(OpenGL gl, uint textureId, int x, int y)
		{
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

			int[] widthArr = new int[1];
			int[] heightArr = new int[1];
			int[] formatArr = new int[1];
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

			int width = widthArr[0];
			int height = heightArr[0];
			int internalFormat = formatArr[0];

			uint[] pbo = new uint[1];
			gl.GenBuffers(1, pbo);
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			gl.BufferData(OpenGL.GL_PIXEL_PACK_BUFFER, 4, IntPtr.Zero, OpenGL.GL_STREAM_READ);
			int invertedY = height - 1 - y;

			// Use appropriate format based on the internal format
			if (internalFormat == OpenGL.GL_LUMINANCE)
			{
				gl.ReadPixels(x, invertedY, 1, 1, OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			}
			else
			{
				gl.ReadPixels(x, invertedY, 1, 1, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			}

			byte[] pixelData = new byte[4];
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			IntPtr ptr = gl.MapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, OpenGL.GL_READ_ONLY);
			if (ptr != IntPtr.Zero)
			{
				Marshal.Copy(ptr, pixelData, 0, 4);
				gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
			}
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0);
			gl.DeleteBuffers(1, pbo);

			// Interpret the pixel data based on the internal format
			if (internalFormat == OpenGL.GL_LUMINANCE)
			{
				// For RED format, we will return a grayscale color
				return System.Drawing.Color.FromArgb(pixelData[0], pixelData[0], pixelData[0]);
			}
			else
			{
				return System.Drawing.Color.FromArgb(pixelData[3], pixelData[0], pixelData[1], pixelData[2]);
			}
		}


		//public System.Drawing.Color ReadTextureColor(OpenGL gl, uint textureId, int x, int y)
		//{
		//	gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

		//	int[] widthArr = new int[1];
		//	int[] heightArr = new int[1];
		//	int[] formatArr = new int[1];
		//	gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
		//	gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
		//	gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

		//	int width = widthArr[0];
		//	int height = heightArr[0];

		//	//uint[] frameBuffer = new uint[1];
		//	//gl.GenFramebuffersEXT(1, frameBuffer);
		//	//gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);
		//	//gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

		// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
		//	//uint[] renderBuffer = new uint[1];
		//	//gl.GenRenderbuffersEXT(1, renderBuffer);
		//	//gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
		//	//gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);

		// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
		//	//gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);

		//	Stopwatch stopwatch = Stopwatch.StartNew();


		// PBO를 생성하고 바인딩합니다.
		//	uint[] pbo = new uint[1];
		//	gl.GenBuffers(1, pbo);
		//	gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
		//	gl.BufferData(OpenGL.GL_PIXEL_PACK_BUFFER, 4, IntPtr.Zero, OpenGL.GL_STREAM_READ);
		//	int invertedY = height - 1 - y;
		// glReadPixels 호출
		//	gl.ReadPixels(x, invertedY, 1, 1, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);

		// GPU에서 픽셀 데이터를 읽어옵니다.
		//	byte[] pixelData = new byte[4];
		//	gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
		//	IntPtr ptr = gl.MapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, OpenGL.GL_READ_ONLY);
		//	if (ptr != IntPtr.Zero)
		//	{
		//		Marshal.Copy(ptr, pixelData, 0, 4);
		//		gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
		//	}
		//	gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0);
		//	gl.DeleteBuffers(1, pbo);
		//	//gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

		//	//Console.WriteLine($"{stopwatch.ElapsedMilliseconds}");

		// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
		//	//gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
		//	//gl.DeleteFramebuffersEXT(1, frameBuffer);

		//	return System.Drawing.Color.FromArgb(pixelData[3], pixelData[0], pixelData[1], pixelData[2]);
		//}

		public System.Drawing.Color[] ReadTextureColors(uint textureId)
		{
			OpenGL gl = GetOpenGL();
			Stopwatch stopwatch = Stopwatch.StartNew();

			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
			int[] widthArr = new int[1];
			int[] heightArr = new int[1];
			int[] formatArr = new int[1];
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

			int width = widthArr[0];
			int height = heightArr[0];

			OpenGlRenderer.InitializeOpenGLSettings(gl, width, height);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			uint[] ids = new uint[1];
			gl.GenFramebuffersEXT(1, ids);
			uint frameBufferID = ids[0];
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferID);
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

			// PBO를 생성하고 바인딩합니다.
			uint[] pbo = new uint[1];
			gl.GenBuffers(1, pbo);
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			int bufferSize = width * height * 4; // RGBA for each pixel
			gl.BufferData(OpenGL.GL_PIXEL_PACK_BUFFER, bufferSize, IntPtr.Zero, OpenGL.GL_STREAM_READ);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			uint[] renderBuffer = new uint[1];
			gl.GenRenderbuffersEXT(1, renderBuffer);
			gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
			gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);

			// OpenGL 텍스처를 생성합니다.
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

			// OpenGL 텍스처를 생성합니다.
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

			// glReadPixels 호출
			gl.ReadPixels(0, 0, width, height, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);

			// GPU에서 픽셀 데이터를 읽어옵니다.
			byte[] pixelData = new byte[bufferSize];
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			IntPtr ptr = gl.MapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, OpenGL.GL_READ_ONLY);
			if (ptr != IntPtr.Zero)
			{
				Marshal.Copy(ptr, pixelData, 0, bufferSize);
				gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
			}
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0);
			gl.DeleteBuffers(1, pbo);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, ids);
			gl.DeleteRenderbuffersEXT(1, renderBuffer);
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			ReshapeNonRefresh();

			int bytesPerPixel = 4; // RGBA 형식 기준입니다.
			int stride = width * bytesPerPixel;
			byte[] flippedPixels = new byte[pixelData.Length];

			for (int y = 0; y < height; y++)
			{
				int sourceIndex = y * stride;
				int destIndex = (height - 1 - y) * stride;
				Array.Copy(pixelData, sourceIndex, flippedPixels, destIndex, stride);
			}

			// 생성한 텍스처 데이터를 업데이트합니다.
			Array.Copy(flippedPixels, pixelData, pixelData.Length);

			System.Drawing.Color[] colors = new System.Drawing.Color[width * height];

			for (int y = 0; y < height; y++)
			{
				int baseIndex = y * width * 4;
				for (int x = 0; x < width; x++)
				{
					int i = baseIndex + (x * 4);
					colors[x + (y * width)] = System.Drawing.Color.FromArgb(flippedPixels[i + 3], flippedPixels[i], flippedPixels[i + 1], flippedPixels[i + 2]);
				}
			}

			//Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms");

			return colors;
		}

		public byte[] ReadTextureBuffers(uint textureId)
		{
			OpenGL gl = GetOpenGL();
			Stopwatch stopwatch = Stopwatch.StartNew();

			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
			int[] widthArr = new int[1];
			int[] heightArr = new int[1];
			int[] formatArr = new int[1];
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
			gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

			int width = widthArr[0];
			int height = heightArr[0];

			OpenGlRenderer.InitializeOpenGLSettings(gl, width, height);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			uint[] ids = new uint[1];
			gl.GenFramebuffersEXT(1, ids);
			uint frameBufferID = ids[0];
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferID);
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

			// PBO를 생성하고 바인딩합니다.
			uint[] pbo = new uint[1];
			gl.GenBuffers(1, pbo);
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			int bufferSize = width * height * 4; // RGBA for each pixel
			gl.BufferData(OpenGL.GL_PIXEL_PACK_BUFFER, bufferSize, IntPtr.Zero, OpenGL.GL_STREAM_READ);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			uint[] renderBuffer = new uint[1];
			gl.GenRenderbuffersEXT(1, renderBuffer);
			gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
			gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);

			// OpenGL 텍스처를 생성합니다.
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

			// OpenGL 텍스처를 생성합니다.
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

			// glReadPixels 호출
			gl.ReadPixels(0, 0, width, height, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);

			// GPU에서 픽셀 데이터를 읽어옵니다.
			byte[] pixelData = new byte[bufferSize];
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, pbo[0]);
			IntPtr ptr = gl.MapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, OpenGL.GL_READ_ONLY);
			if (ptr != IntPtr.Zero)
			{
				Marshal.Copy(ptr, pixelData, 0, bufferSize);
				gl.UnmapBuffer(OpenGL.GL_PIXEL_PACK_BUFFER);
			}
			gl.BindBuffer(OpenGL.GL_PIXEL_PACK_BUFFER, 0);
			gl.DeleteBuffers(1, pbo);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, ids);
			gl.DeleteRenderbuffersEXT(1, renderBuffer);
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			ReshapeNonRefresh();

			int bytesPerPixel = 4; // RGBA 형식 기준입니다.
			int stride = width * bytesPerPixel;
			byte[] flippedPixels = new byte[pixelData.Length];

			for (int y = 0; y < height; y++)
			{
				int sourceIndex = y * stride;
				int destIndex = (height - 1 - y) * stride;
				Array.Copy(pixelData, sourceIndex, flippedPixels, destIndex, stride);
			}

			// 생성한 텍스처 데이터를 업데이트합니다.
			Array.Copy(flippedPixels, pixelData, pixelData.Length);

			return flippedPixels;
		}

		public byte[] ReadTextureByte(OpenGL gl, uint textureId, int width, int height)
		{
			// 뷰포트를 설정합니다.
			gl.Viewport(0, 0, width, height);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.LoadIdentity();
			// Y축의 시작과 끝을 반전합니다.
			gl.Ortho2D(0, width, height, 0);  // Y축의 시작과 끝을 반전합니다.

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.LoadIdentity();

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			uint[] ids = new uint[1];
			gl.GenFramebuffersEXT(1, ids);
			uint frameBufferID = ids[0];
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferID);
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			uint[] renderBuffer = new uint[1];
			gl.GenRenderbuffersEXT(1, renderBuffer);
			gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
			gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, width, height);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);


			//	Bind our texture object (make it the current texture).
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
			gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

			//  Set linear filtering mode.
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

			// 생성한 텍스처 데이터를 업데이트합니다.
			byte[] pixelData = new byte[width * height * 4]; // RGBA

			// 생성한 텍스처 데이터를 업데이트합니다.
			gl.ReadPixels(0, 0, width, height, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, pixelData);

			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			gl.DeleteFramebuffersEXT(1, ids);
			Reshape();
			return pixelData;
		}

		public System.Drawing.Color GetPixelColor(OpenGL gl, int x, int y)
		{
			byte[] pixelData = new byte[4]; // RGBA

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			var screen = GetScreenPosFromPixelCoordf(x, y);

			int invertedY = (int)(gl.RenderContextProvider.Height - Math.Round(screen.Y));

			gl.ReadPixels((int)Math.Round(screen.X), invertedY, 1, 1, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, pixelData);

			return System.Drawing.Color.FromArgb(pixelData[3], pixelData[0], pixelData[1], pixelData[2]);
		}

		public System.Drawing.Color[] GetAllPixelColors(OpenGL gl, int imageWidth, int imageHeight)
		{
			// RGB 값을 0에서 255 사이의 값으로 변환합니다.
			byte[] pixelData = new byte[imageWidth * imageHeight * 4];

			// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
			gl.ReadPixels(0, 0, imageWidth, imageHeight, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, pixelData);

			// 생성한 텍스처 데이터를 업데이트합니다.
			System.Drawing.Color[] colors = new System.Drawing.Color[imageWidth * imageHeight];

			// 생성한 텍스처 데이터를 업데이트합니다.
			for (int i = 0; i < imageWidth * imageHeight; i++)
			{
				int index = i * 4;
				colors[i] = System.Drawing.Color.FromArgb(pixelData[index + 3], pixelData[index], pixelData[index + 1], pixelData[index + 2]);
			}

			return colors;
		}

		/// <summary>
		/// 뷰포트를 설정합니다.
		/// </summary>
		public void Reshape()
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new MethodInvoker(() =>
				{
					Reshape();
				}));
			}
			else
			{
				var gl = openGLControl.OpenGL;

				if (_zoom == 100000) { return; }
				gl.SetDimensions(this.openGLControl.Width, this.openGLControl.Height);
				gl.Viewport(0, 0, this.openGLControl.Width, this.openGLControl.Height);

				gl.MatrixMode(MatrixMode.Projection);
				gl.LoadIdentity();

				_aspectRatio = ((float)this.openGLControl.Width) / this.openGLControl.Height;
				_xSpan = _zoom;
				_ySpan = _zoom;

				if (_aspectRatio > 1)
				{
					_xSpan *= _aspectRatio;
				}
				else
				{
					_ySpan /= _aspectRatio;
				}

				gl.Ortho2D(0, openGLControl.Width * ZoomScale, 0, openGLControl.Height * ZoomScale);

				gl.MatrixMode(MatrixMode.Modelview);
				gl.LoadIdentity();

				RefreshGL();
			}
		}

		public void ReshapeNonRefresh()
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new MethodInvoker(() =>
				{
					ReshapeNonRefresh();
				}));
			}
			else
			{
				var gl = openGLControl.OpenGL;

				if (_zoom == 100000) { return; }
				gl.SetDimensions(this.openGLControl.Width, this.openGLControl.Height);
				gl.Viewport(0, 0, this.openGLControl.Width, this.openGLControl.Height);

				gl.MatrixMode(MatrixMode.Projection);
				gl.LoadIdentity();

				_aspectRatio = ((float)this.openGLControl.Width) / this.openGLControl.Height;
				_xSpan = _zoom;
				_ySpan = _zoom;

				if (_aspectRatio > 1)
				{
					_xSpan *= _aspectRatio;
				}
				else
				{
					_ySpan /= _aspectRatio;
				}

				//gl.Ortho2D(0, openGLControl.Width * ZoomScale, 0, openGLControl.Height * ZoomScale);

				gl.Ortho2D(0, openGLControl.Width * ZoomScale, 0, openGLControl.Height * ZoomScale);

				//gl.Ortho2D(0, _xSpan, 0, _ySpan);
				//gl.Ortho2D(0, 100, 0, 100);

				//  Back to the modelview.
				gl.MatrixMode(MatrixMode.Modelview);
				gl.LoadIdentity();
			}
		}

		[Obsolete("Use ReshapeNonRefresh instead.")]
		public void ReshapeNonRefrsh()
		{
			ReshapeNonRefresh();
		}

		/// <summary>
		/// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
		/// </summary>
		/// <returns></returns>
		public int GetControlMinSize()
		{
			Func<int> func = delegate
			{
				return Math.Min(this.openGLControl.Height, this.openGLControl.Width); ;
			};
			int minSize = -1;
			if (openGLControl.InvokeRequired == true)
			{
				openGLControl.Invoke(new MethodInvoker(delegate
				{
					minSize = func();
				}));
				return minSize;
			}
			else
			{
				return func();
			}
		}

		public void ClearTexture()
		{
			ClearTexture(true);
		}

		public void ClearTextureStateOnly()
		{
			ClearTexture(false);
		}

		public void ClearTexture(bool deleteOpenGlTextures)
		{
			Action clearState = delegate
			{
				_textureAreas.Clear();
				_textureKeysOrder.Clear();
			};

			Action action = delegate
			{
				if (!deleteOpenGlTextures)
				{
					clearState();
					return;
				}

				if (_textureAreas.Count <= 0)
				{
					_textureKeysOrder.Clear();
					return;
				}

				List<uint> textureIds = ImageCanvasTextureStore.CollectTextureIds(_textureAreas.Values);

				if (textureIds.Count > 0 && openGLControl != null && !openGLControl.IsDisposed)
				{
					ImageCanvasTextureStore.DeleteTextures(openGLControl.OpenGL, textureIds);
				}

				clearState();
			};

			if (openGLControl == null || openGLControl.IsDisposed)
			{
				clearState();
				return;
			}

			try
			{
				if (openGLControl.InvokeRequired)
				{
					openGLControl.Invoke(new Action(action));
				}
				else
				{
					action();
				}
			}
			catch (ObjectDisposedException)
			{
				clearState();
			}
			catch (InvalidOperationException)
			{
				clearState();
			}
		}

		public IDisposable SuppressRefresh()
		{
			return new RefreshScope(this);
		}

		private sealed class RefreshScope : IDisposable
		{
			private readonly ImageCanvasControl owner;
			private readonly bool previousSuppressRefresh;

			public RefreshScope(ImageCanvasControl owner)
			{
				this.owner = owner;
				previousSuppressRefresh = owner._suppressRefresh;
				owner._suppressRefresh = true;
			}

			public void Dispose()
			{
				owner._suppressRefresh = previousSuppressRefresh;
			}
		}

		public void UpdateTexture(IntPtr data, int width, int height, uint bpp, uint textureId)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new MethodInvoker(() =>
				{
					UpdateTexture(data, width, height, bpp, textureId);
				}));
			}
			else
			{
				OpenGL gl = GetOpenGL();

				gl.BindTexture(SharpGL.OpenGL.GL_TEXTURE_2D, textureId);

				// OpenGL 텍스처를 생성합니다.
				if (bpp == 3)
				{
					// for colors
					glTexSubImage2D(SharpGL.OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, data);
				}
				else if (bpp == 4)
				{
					// for RGBA colors
					glTexSubImage2D(SharpGL.OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data);
				}
				else if (bpp == 1)
				{
					// for mono
					glTexSubImage2D(SharpGL.OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, data);
				}

				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP);

				gl.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MIN_FILTER, SharpGL.OpenGL.GL_LINEAR);
				gl.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MAG_FILTER, SharpGL.OpenGL.GL_NEAREST);

				gl.GenerateMipmapEXT(SharpGL.OpenGL.GL_TEXTURE_2D);

				gl.BindTexture(SharpGL.OpenGL.GL_TEXTURE_2D, textureId);
			}
		}


		/// <summary>
		/// OpenGL 텍스처를 생성합니다.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="bpp"></param>
		/// <returns></returns>
		public uint GenerateOpenGLTexture(int width, int height, uint bpp)
		{
			OpenGL gl = GetOpenGL();

			uint textureId = 0;
			Func<uint> action = delegate
			{
				uint[] gtexture = new uint[1];

				gl.GenTextures(1, gtexture); // 텍스처 관련 값입니다.
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]); // 텍스처 관련 값입니다.

				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

				// OpenGL 텍스처를 생성합니다.
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

				gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);

				if (bpp == 3)
				{
					// for Color
					gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, width, height, 0, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
				}
				else if (bpp == 4)
				{
					// for Color
					gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, width, height, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
				}
				else if (bpp == 1)
				{
					// for Mono
					gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_LUMINANCE, width, height, 0, OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
				}

				gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

				int[] widthArr = new int[1];
				int[] heightArr = new int[1];
				int[] formatArr = new int[1];
				gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
				gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
				gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

				gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

				return gtexture[0];
			};
			if (openGLControl.InvokeRequired == true)
			{
				openGLControl.Invoke(new MethodInvoker(delegate
				{
					textureId = action();
				}));
				return textureId;
			}
			else
			{
				return action();
			}
		}

		//public uint GenerateOpenGLTexture(int width, int height, uint bpp)
		//{
		//	OpenGL gl = GetOpenGL();

		//	uint textureId = 0;
		//	Func<uint> action = delegate
		//	{
		//		uint[] gtexture = new uint[1];

		// 텍스처 관련 값입니다.
		//									 //CheckGLError(openGLControl.OpenGL, "GenTextures");

		// 텍스처 관련 값입니다.

		//		gl.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MIN_FILTER, SharpGL.OpenGL.GL_LINEAR);
		//		gl.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MAG_FILTER, SharpGL.OpenGL.GL_NEAREST);
		//		//CheckGLError(openGLControl.OpenGL, "TexParameter");

		// OpenGL 텍스처를 생성합니다.
		//		gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
		//		gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

		//		if (bpp == 3)
		//		{
		//			// for Color
		//			gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
		//			gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, width, height, 0,
		//			 SharpGL.OpenGL.GL_BGR,
		//			 OpenGL.GL_UNSIGNED_BYTE,
		//			 IntPtr.Zero
		//			 );
		//			//CheckGLError(openGLControl.OpenGL, "TexImage2D");
		//		}
		//		if (bpp == 4)
		//		{
		//			// for Color
		//			gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
		//			gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, width, height, 0,
		//			 SharpGL.OpenGL.GL_BGRA,
		//			 OpenGL.GL_UNSIGNED_BYTE,
		//			 IntPtr.Zero
		//			 );
		//		}
		//		else if (bpp == 1)
		//		{
		//			// for Mono
		//			gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
		//			gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_LUMINANCE, width, height, 0,
		//				OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
		//		}

		//		gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

		//		int[] widthArr = new int[1];
		//		int[] heightArr = new int[1];
		//		int[] formatArr = new int[1];
		//		gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_WIDTH, widthArr);
		//		gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_HEIGHT, heightArr);
		//		gl.GetTexLevelParameter(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_TEXTURE_INTERNAL_FORMAT, formatArr);

		//		//openGLControl.OpenGL.BindTexture(1, gtexture[0]);
		//		gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

		//		return gtexture[0];
		//	};
		//	if (openGLControl.InvokeRequired == true)
		//	{
		//		openGLControl.Invoke(new MethodInvoker(delegate
		//		{
		//			textureId = action();
		//		}));
		//		return textureId;
		//	}
		//	else
		//	{
		//		return action();
		//	}
		//}

		//private void CheckGLError(OpenGL gl, string location)
		//{
		//	uint error = gl.GetError();
		//	if (error != OpenGL.GL_NO_ERROR)
		//	{
		//		throw new Exception($"OpenGL error at {location}: {error}");
		//	}
		//}

		#endregion

		#region Draw Object
		public void DrawContent()
		{
			OpenGL gl = GetOpenGL();
			OpenGlDrawing.DrawTexture(gl, _textureAreas, _textureKeysOrder);
			DrawVisibleOverlays(gl);
			if (IsShowCrossLine) { OpenGlDrawing.DrawCrossOfImage(gl, _textureAreas); }
		}

		public void DrawImagePointMarker(System.Drawing.Point imagePoint, int imageHeight)
		{
			if (imageHeight <= 0) { return; }

			OpenGL gl = GetOpenGL();
			if (gl == null) { return; }

			float x = imagePoint.X;
			float y = imageHeight - imagePoint.Y;
			float markerSize = Math.Max(8f, 14f * ZoomScale);
			float gap = Math.Max(2f, 3f * ZoomScale);

			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.LineWidth(2.0f);
			gl.Color(1.0f, 0.85f, 0.05f, 1.0f);
			gl.Begin(OpenGL.GL_LINES);
			gl.Vertex(x - markerSize, y);
			gl.Vertex(x - gap, y);
			gl.Vertex(x + gap, y);
			gl.Vertex(x + markerSize, y);
			gl.Vertex(x, y - markerSize);
			gl.Vertex(x, y - gap);
			gl.Vertex(x, y + gap);
			gl.Vertex(x, y + markerSize);
			gl.End();

			gl.PointSize(5.0f);
			gl.Begin(OpenGL.GL_POINTS);
			gl.Vertex(x, y);
			gl.End();

			float boxSize = Math.Max(4f, 6f * ZoomScale);
			gl.LineWidth(1.0f);
			gl.Begin(OpenGL.GL_LINE_LOOP);
			gl.Vertex(x - boxSize, y - boxSize);
			gl.Vertex(x + boxSize, y - boxSize);
			gl.Vertex(x + boxSize, y + boxSize);
			gl.Vertex(x - boxSize, y + boxSize);
			gl.End();
			gl.Enable(OpenGL.GL_TEXTURE_2D);
		}
		public void DrawODBTexture()
		{
			OpenGL gl = GetOpenGL();
			OpenGlDrawing.DrawODBTexture(gl, _textureAreas, _textureKeysOrder);
		}

		public void DrawMeasurement(OpenGL gl, Measurement measurement, OpenGlFontRenderOptions glFontRenderOptions)
		{
			OpenGlDrawing.DrawMeasurement(gl, measurement, glFontRenderOptions, _fontBitmapEntries, _xSpan, _ySpan, _fitRect, _offsetSize, PixelPermm);
		}

		public Bitmap RenderTextureToBitmap(string imageName, uint textureId, uint bpp)
		{
			Bitmap bitmap = new Bitmap(10, 10);
			if (this.InvokeRequired)
			{

				this.Invoke(new MethodInvoker(() =>
				{
					bitmap = RenderTextureToBitmap(imageName, textureId, bpp);
				}));
				return bitmap;
			}
			else
			{
				OpenGL gl = GetOpenGL();
				Bitmap bmp = OpenGlRenderer.TextureToBitmap(gl, textureId, bpp);

				//bmp.Save("D:\\ori.bmp");

				OpenGlRenderer.InitializeOpenGLSettings(gl, bmp.Width, bmp.Height);

				// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
				uint[] ids = new uint[1];
				gl.GenFramebuffersEXT(1, ids);
				uint frameBufferID = ids[0];
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferID);
				gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureId, 0);

				// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
				uint[] renderBuffer = new uint[1];
				gl.GenRenderbuffersEXT(1, renderBuffer);
				gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);
				gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_STENCIL_INDEX8_EXT, bmp.Width, bmp.Height);

				// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
				gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_STENCIL_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, renderBuffer[0]);

				uint[] gtexture = new uint[1];
				gtexture[0] = GenerateOpenGLTexture(bmp.Width, bmp.Height, bpp);

				//	Get the maximum texture size supported by OpenGL.
				int[] textureMaxSize = { 0 };
				gl.GetInteger(OpenGL.GL_MAX_TEXTURE_SIZE, textureMaxSize);

				//  Ensure that the image does not exceed the maximum texture size.
				if (bmp.Width > textureMaxSize[0] || bmp.Height > textureMaxSize[0])
				{
					throw new InvalidOperationException("Image exceeds the maximum texture size.");
				}

				//	Bind our texture object (make it the current texture).
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);
				gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, gtexture[0], 0);

				//  Lock the image bits (so that we can pass them to OGL).
				BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
					ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				//  Set the image data.
				gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA,
					bmp.Width, bmp.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
					bitmapData.Scan0);

				//  Unlock the image.
				bmp.UnlockBits(bitmapData);

				//  Set linear filtering mode.
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MIN_FILTER, SharpGL.OpenGL.GL_LINEAR);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MAG_FILTER, SharpGL.OpenGL.GL_NEAREST);

				// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
				gl.Enable(OpenGL.GL_STENCIL_TEST);

				// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
				gl.ClearStencil(0);
				gl.Clear(OpenGL.GL_STENCIL_BUFFER_BIT);
				gl.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				// 이미지 영역을 좌하단 기준으로 설정합니다.
				gl.ColorMask(0, 0, 0, 0); // 화면에는 색을 쓰지 않습니다.

				var eraerPoints = GetEraserPoint();
				foreach (var eraerPoint in eraerPoints)
				{
					OpenGlDrawing.DrawWithPen(gl, eraerPoint.EraserPointfs, eraerPoint.EraserWidth, System.Windows.Media.Brushes.Yellow);
				}
				// 마우스 이동량을 계산합니다.
				gl.ColorMask(1, 1, 1, 1);
				gl.StencilFunc(OpenGL.GL_NOTEQUAL, 1, 0xFF);
				gl.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

				OpenGlTextureDrawingParam param = GetTextureParameterAtPoint(textureId);

				// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
				var textureArea = param.GLTextureArea;
				var fullSize = param.TextureFullScreen;

				System.Drawing.Size tileSize = param.TitleSize;

				// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
				int offsetX = (int)(textureArea.X / tileSize.Width) * tileSize.Width;
				int offsetY = 0;
				if (tileSize.Height > textureArea.Y + textureArea.Height)
				{
					offsetY = (int)0;
				}
				else
				{
					if (tileSize.Height > textureArea.Y)
					{
						offsetY = (int)textureArea.Y;
					}
					else
					{
						int qui = (int)textureArea.Y / tileSize.Height;
						offsetY = (int)(textureArea.Y);
					}
				}


				foreach (CanvasOverlayItem overlayItem in GetCanvasOverlayManager().GetOverlaysByGroupType(imageName))
				{
					if (overlayItem.Shape is LineInfo)
					{
						LineInfo lineInfo = overlayItem.Shape as LineInfo;
						// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
						System.Drawing.PointF start = new PointF(lineInfo.StartDot.X - offsetX, lineInfo.StartDot.Y - offsetY);
						System.Drawing.PointF end = new PointF(lineInfo.EndDot.X - offsetX, lineInfo.EndDot.Y - offsetY);

						// RGB 값을 0에서 255 사이의 값으로 변환합니다.
						byte red = (byte)(lineInfo.LineColor[0] * 255);
						byte green = (byte)(lineInfo.LineColor[1] * 255);
						byte blue = (byte)(lineInfo.LineColor[2] * 255);

						// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
						System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));

						OpenGlDrawing.DrawLine(gl, start, end, lineInfo.Width, brush);
					}
					if (overlayItem.Shape is RectInfo)
					{
						RectInfo rectInfo = overlayItem.Shape as RectInfo;

						// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
						System.Drawing.PointF start = new PointF(rectInfo.LeftBottom.X - offsetX, rectInfo.LeftBottom.Y - offsetY);
						System.Drawing.PointF end = new PointF(rectInfo.RightTop.X - offsetX, rectInfo.RightTop.Y - offsetY);

						// RGB 값을 0에서 255 사이의 값으로 변환합니다.
						byte red = (byte)(rectInfo.LineColor[0] * 255);
						byte green = (byte)(rectInfo.LineColor[1] * 255);
						byte blue = (byte)(rectInfo.LineColor[2] * 255);

						System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));

						OpenGlDrawing.DrawRectangle(gl, start, end, rectInfo.Width, rectInfo.IsFill, brush);
					}
					if (overlayItem.Shape is CircleInfo)
					{
						CircleInfo circleInfo = overlayItem.Shape as CircleInfo;

						// OpenGL은 좌하단을 원점으로 사용하므로 y 좌표를 계산하거나 반전합니다.
						System.Drawing.PointF start = new PointF(circleInfo.StartDot.X - offsetX, circleInfo.StartDot.Y - offsetY);
						System.Drawing.PointF end = new PointF(circleInfo.EndDot.X - offsetX, circleInfo.EndDot.Y - offsetY);

						//System.Drawing.PointF start = new PointF(circleInfo.StartDot.X, circleInfo.StartDot.Y);
						//System.Drawing.PointF end = new PointF(circleInfo.EndDot.X, circleInfo.EndDot.Y);

						// RGB 값을 0에서 255 사이의 값으로 변환합니다.
						byte red = (byte)(circleInfo.LineColor[0] * 255);
						byte green = (byte)(circleInfo.LineColor[1] * 255);
						byte blue = (byte)(circleInfo.LineColor[2] * 255);

						System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));
						OpenGlDrawing.DrawCircle(gl, start, end, circleInfo.Width, brush, circleInfo.IsFill);
					}
					if (overlayItem.Shape is PensInfo)
					{
						PensInfo pensInfo = overlayItem.Shape as PensInfo;

						// RGB 값을 0에서 255 사이의 값으로 변환합니다.
						byte red = (byte)(pensInfo.LineColor[0] * 255);
						byte green = (byte)(pensInfo.LineColor[1] * 255);
						byte blue = (byte)(pensInfo.LineColor[2] * 255);

						List<DotInfo> newDots = new List<DotInfo>();
						var imageTitleOffset = pensInfo.ImageTitleOffset;
						foreach (DotInfo dot in pensInfo.Dots)
						{
							newDots.Add(new DotInfo(dot.X - offsetX, dot.Y - offsetY));
						}

						System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));

						OpenGlDrawing.DrawWithPen(gl, newDots, pensInfo.Width, brush);
					}
				}

				// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
				gl.Disable(OpenGL.GL_STENCIL_TEST);

				Bitmap copy = OpenGlRenderer.TextureToBitmap(gl, gtexture[0], bpp);

				// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
				gl.DeleteTextures(1, gtexture);
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
				gl.DeleteFramebuffersEXT(1, ids);
				Reshape();

				return copy;
			}
		}

		/// <summary>
		/// 뷰포트를 설정합니다.
		/// </summary>
		/// <param name="gl"></param>
		public void DrawVisibleOverlays(OpenGL gl)
		{
			// 이미지 캔버스의 좌표와 텍스처 상태를 처리합니다.
			foreach (var shape in _shapesViewPort)
			{
				gl.CallList(shape.DisplayListId);
			}
		}

		/// <summary>
		/// 이미지 영역을 좌하단 기준으로 설정합니다.
		/// </summary>
		private void CalculatorVisibleOverlays()
		{
			// 생성한 텍스처 데이터를 업데이트합니다.
			float viewportLeft = 0 - _offsetSize.Width;
			float viewportRight = _xSpan - _offsetSize.Width;
			float viewportTop = _ySpan - _offsetSize.Height;
			float viewportBottom = 0 - _offsetSize.Height;

			// 마우스 이동량을 계산합니다.
			var visibleOverlays = _overlayManager.GetAllVisibleOverlays();

			if (!visibleOverlays.Any())
			{
				_shapesViewPort = new List<CanvasShape>();
				return;
			}
			// 이미지 영역을 좌하단 기준으로 설정합니다.
			ConcurrentBag<CanvasShape> concurrentBag = new ConcurrentBag<CanvasShape>();
			Parallel.ForEach(visibleOverlays, d =>
			{
				if (d.IsExtentionRectange)
				{
					//if ((d.Shape is CanvasRect<float>))
					//{
					//	(d.Shape as CanvasRect<float>).CreateExtendedRectangleFromSize();
					//}
				}
				//var validDots = d.Shape.ShapePoints.Where(dot => dot != null).ToArray();
				var dots = d.Shape.ShapePoints.ToArray();
				if (dots.Length != 0)
				{
					float shapeLeft = dots.Min(dot => dot.X);
					float shapeRight = dots.Max(dot => dot.X);
					float shapeTop = dots.Max(dot => dot.Y);
					float shapeBottom = dots.Min(dot => dot.Y);

					if (shapeLeft < viewportRight &&
						shapeRight > viewportLeft &&
						shapeTop > viewportBottom &&
						shapeBottom < viewportTop)
					{
						if (d.IsExtentionRectange)
						{
							concurrentBag.Add((d.Shape as CanvasRect<float>).ExtendedRectangle);
						}

						concurrentBag.Add(d.Shape);
					}
				}
			});

			_shapesViewPort = concurrentBag.ToList();
		}
		#endregion
	}
}
