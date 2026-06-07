using OpenVisionLab.ImageCanvas.CanvasShapes;

namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	public class OpenGlTextureDrawingParam
	{
		public uint OriTextureId { get; set; }
		public uint OriBackgroundTextureId { get; set; }
		public uint MaskTextureId { get; set; }
		public uint MaskBackgroundMaskTextureId { get; set; }
		public uint TransparentBackgroundTextureId { get; set; }
		public uint ThresholdTextureId { get; set; } = 0;
		public uint BppOriginal { get; set; }
		public uint BppBackground { get; set; }
		public string ImageName { get; set; }
		public System.Drawing.RectangleF GLDrawingTextureArea { get; set; } // OpenGL 텍스처를 실제로 그리는 좌표 영역
		public System.Drawing.RectangleF GLTextureArea { get; set; } // OpenGL 텍스처 좌표 영역
		public System.Drawing.Rectangle ImageTexutreArea { get; set; }  // 원본 이미지 기준 텍스처 좌표 영역
		public System.Drawing.Size TextureFullScreen { get; set; } // 전체 텍스처 크기
		public System.Drawing.Size TitleSize { get; set; } // 분할 타일 크기. 예: 5000 x 5000
		public bool IsVisible { get; set; } = true; // 해당 텍스처 표시 여부
		public bool IsTransParency { get; set; } = false; // 투명도 적용 여부
		public bool IsRotated { get; set; } = false; // 회전 적용 여부
		public bool IsThreshold { get; set; } = false;
		public float TransParency { get; set; } = 1.0f; // 투명도 값
		public float RotationAngle { get; set; } = 0.0f; // 회전 각도

		public DotInfo ImageTitleOffset = new DotInfo(); // 전체 이미지 기준에서 분할 이미지 좌표계가 가지는 오프셋
	}
}
