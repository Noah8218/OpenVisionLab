
namespace KtemVisionSystem
{
    partial class KtemViewer
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
            this.components = new System.ComponentModel.Container();
            this.timerDrawRect = new System.Windows.Forms.Timer(this.components);
            this.ddmImageMenu = new RJCodeUI_M1.RJControls.RJDropdownMenu(this.components);
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ItemROI = new FontAwesome.Sharp.IconMenuItem();
            this.ItemTrainROI = new FontAwesome.Sharp.IconMenuItem();
            this.ItemMultiROI = new FontAwesome.Sharp.IconMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ddmImageMenu.SuspendLayout();
            // 
            // timerDrawRect
            // 
            this.timerDrawRect.Tick += new System.EventHandler(this.timerDrawRect_Tick);
            // 
            // ddmImageMenu
            // 
            this.ddmImageMenu.ActiveMenuItem = false;
            this.ddmImageMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76)))));
            this.ddmImageMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddmImageMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem5,
            this.toolStripMenuItem2,
            this.toolStripMenuItem1});
            this.ddmImageMenu.Name = "ddmDevice";
            this.ddmImageMenu.OwnerIsMenuButton = false;
            this.ddmImageMenu.Size = new System.Drawing.Size(196, 114);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem6.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76)))));
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(195, 22);
            this.toolStripMenuItem6.Text = "Image Load";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem7.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76)))));
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(195, 22);
            this.toolStripMenuItem7.Text = "Image Save";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(195, 22);
            this.toolStripMenuItem5.Text = "Show Folder";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(195, 22);
            this.toolStripMenuItem2.Text = "Function Collection";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem3.Text = "3 Point Measure";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem4.Text = "Image Compare";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ItemROI,
            this.ItemTrainROI,
            this.ItemMultiROI});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
            this.toolStripMenuItem1.Text = "ROI";
            // 
            // ItemROI
            // 
            this.ItemROI.IconChar = FontAwesome.Sharp.IconChar.None;
            this.ItemROI.IconColor = System.Drawing.Color.Black;
            this.ItemROI.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.ItemROI.Name = "ItemROI";
            this.ItemROI.Size = new System.Drawing.Size(137, 22);
            this.ItemROI.Text = "ROI";
            // 
            // ItemTrainROI
            // 
            this.ItemTrainROI.IconChar = FontAwesome.Sharp.IconChar.None;
            this.ItemTrainROI.IconColor = System.Drawing.Color.Black;
            this.ItemTrainROI.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.ItemTrainROI.Name = "ItemTrainROI";
            this.ItemTrainROI.Size = new System.Drawing.Size(137, 22);
            this.ItemTrainROI.Text = "Train ROI";
            // 
            // ItemMultiROI
            // 
            this.ItemMultiROI.IconChar = FontAwesome.Sharp.IconChar.None;
            this.ItemMultiROI.IconColor = System.Drawing.Color.Black;
            this.ItemMultiROI.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.ItemMultiROI.Name = "ItemMultiROI";
            this.ItemMultiROI.Size = new System.Drawing.Size(137, 22);
            this.ItemMultiROI.Text = "Multi ROI";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.ddmImageMenu.ResumeLayout(false);

        }

        #endregion

        private RJCodeUI_M1.RJControls.RJDropdownMenu ddmImageMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.Timer timerDrawRect;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private FontAwesome.Sharp.IconMenuItem ItemROI;
        private FontAwesome.Sharp.IconMenuItem ItemTrainROI;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private FontAwesome.Sharp.IconMenuItem ItemMultiROI;
        private System.Windows.Forms.Timer timer1;
    }
}
