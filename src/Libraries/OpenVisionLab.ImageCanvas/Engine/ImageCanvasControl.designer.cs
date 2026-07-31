namespace OpenVisionLab.ImageCanvas.Rendering
{
	partial class ImageCanvasControl
	{
		/// <summary> 
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 구성 요소 디자이너에서 생성한 코드

		/// <summary> 
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.openGLControl = new SharpGL.OpenGLControl();
			((System.ComponentModel.ISupportInitialize)(this.openGLControl)).BeginInit();
			this.SuspendLayout();
			// 
			// openGLControl
			// 
			this.openGLControl.AutoSize = true;
			this.openGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.openGLControl.DrawFPS = false;
			this.openGLControl.FrameRate = 28;
			this.openGLControl.Location = new System.Drawing.Point(0, 0);
			this.openGLControl.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this.openGLControl.Name = "openGLControl";
			this.openGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
			this.openGLControl.RenderContextType = SharpGL.RenderContextType.NativeWindow;
			this.openGLControl.RenderTrigger = SharpGL.RenderTrigger.Manual;
			this.openGLControl.Size = new System.Drawing.Size(859, 562);
			this.openGLControl.TabIndex = 0;
			// 
			// ImageCanvasControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.openGLControl);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "ImageCanvasControl";
			this.Size = new System.Drawing.Size(859, 562);
			((System.ComponentModel.ISupportInitialize)(this.openGLControl)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private SharpGL.OpenGLControl openGLControl;
	}
}
