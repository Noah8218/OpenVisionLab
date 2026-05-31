namespace KtemVisionSystem
{
    partial class LogView
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerDisplayLog = new System.Windows.Forms.Timer(this.components);
            this.richTextBoxExLog = new KtemVisionSystem.RichTextBoxEx();
            this.ddmLog = new RJCodeUI_M1.RJControls.RJDropdownMenu(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ddmLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerDisplayLog
            // 
            this.timerDisplayLog.Enabled = true;
            this.timerDisplayLog.Tick += new System.EventHandler(this.timerDisplayLog_Tick);
            // 
            // richTextBoxExLog
            // 
            this.richTextBoxExLog.BackColor = System.Drawing.Color.Black;
            this.richTextBoxExLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxExLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.richTextBoxExLog.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxExLog.Name = "richTextBoxExLog";
            this.richTextBoxExLog.ReadOnly = true;
            this.richTextBoxExLog.ShortcutsEnabled = false;
            this.richTextBoxExLog.Size = new System.Drawing.Size(163, 128);
            this.richTextBoxExLog.TabIndex = 1;
            this.richTextBoxExLog.Text = "";
            this.richTextBoxExLog.WordWrap = false;
            // 
            // ddmLog
            // 
            this.ddmLog.ActiveMenuItem = false;
            this.ddmLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76)))));
            this.ddmLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddmLog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.ddmLog.Name = "ddmCapture";
            this.ddmLog.OwnerIsMenuButton = false;
            this.ddmLog.Size = new System.Drawing.Size(181, 48);
            this.ddmLog.Opening += new System.ComponentModel.CancelEventHandler(this.ddmLog_Opening);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem2.Text = "Show Folder";
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.richTextBoxExLog);
            this.DoubleBuffered = true;
            this.Name = "LogView";
            this.Size = new System.Drawing.Size(163, 128);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ddmLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerDisplayLog;
        private RichTextBoxEx richTextBoxExLog;
        private RJCodeUI_M1.RJControls.RJDropdownMenu ddmLog;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}
