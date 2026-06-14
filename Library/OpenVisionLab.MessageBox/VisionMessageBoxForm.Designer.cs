using FontAwesome.Sharp;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab.MessageDialogs
{
    partial class VisionMessageBoxForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel borderPanel;
        private Panel contentPanel;
        private Panel titlePanel;
        private IconPictureBox headerIcon;
        private Label titleLabel;
        private Button closeButton;
        private Panel bodyPanel;
        private TableLayoutPanel bodyLayoutPanel;
        private Panel iconColumnPanel;
        private Panel iconBackPanel;
        private IconPictureBox iconPictureBox;
        private Label messageLabel;
        private Panel footerPanel;
        private Panel accentBar;
        private FlowLayoutPanel buttonFlowPanel;
        private Button primaryButton;
        private Button detailsButton;
        private Panel detailsPanel;
        private Panel detailsHeaderPanel;
        private Label detailsLabel;
        private Button copyDetailsButton;
        private TextBox detailsTextBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            borderPanel = new Panel();
            contentPanel = new Panel();
            bodyPanel = new Panel();
            bodyLayoutPanel = new TableLayoutPanel();
            iconColumnPanel = new Panel();
            iconBackPanel = new Panel();
            iconPictureBox = new IconPictureBox();
            messageLabel = new Label();
            detailsPanel = new Panel();
            detailsTextBox = new TextBox();
            detailsHeaderPanel = new Panel();
            detailsLabel = new Label();
            copyDetailsButton = new Button();
            footerPanel = new Panel();
            buttonFlowPanel = new FlowLayoutPanel();
            primaryButton = new Button();
            detailsButton = new Button();
            accentBar = new Panel();
            titlePanel = new Panel();
            titleLabel = new Label();
            closeButton = new Button();
            headerIcon = new IconPictureBox();
            borderPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            bodyPanel.SuspendLayout();
            bodyLayoutPanel.SuspendLayout();
            iconColumnPanel.SuspendLayout();
            iconBackPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconPictureBox).BeginInit();
            detailsPanel.SuspendLayout();
            detailsHeaderPanel.SuspendLayout();
            footerPanel.SuspendLayout();
            titlePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)headerIcon).BeginInit();
            SuspendLayout();
            // 
            // borderPanel
            // 
            borderPanel.BackColor = Color.FromArgb(69, 86, 130);
            borderPanel.Controls.Add(contentPanel);
            borderPanel.Dock = DockStyle.Fill;
            borderPanel.Location = new Point(0, 0);
            borderPanel.Name = "borderPanel";
            borderPanel.Padding = new Padding(1);
            borderPanel.Size = new Size(620, 270);
            borderPanel.TabIndex = 0;
            // 
            // contentPanel
            // 
            contentPanel.BackColor = Color.FromArgb(240, 245, 250);
            contentPanel.Controls.Add(bodyPanel);
            contentPanel.Controls.Add(detailsPanel);
            contentPanel.Controls.Add(footerPanel);
            contentPanel.Controls.Add(accentBar);
            contentPanel.Controls.Add(titlePanel);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(1, 1);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(618, 268);
            contentPanel.TabIndex = 0;
            // 
            // bodyPanel
            // 
            bodyPanel.BackColor = Color.FromArgb(240, 245, 250);
            bodyPanel.Controls.Add(bodyLayoutPanel);
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Location = new Point(0, 50);
            bodyPanel.Name = "bodyPanel";
            bodyPanel.Size = new Size(618, 154);
            bodyPanel.TabIndex = 1;
            // 
            // bodyLayoutPanel
            // 
            bodyLayoutPanel.ColumnCount = 2;
            bodyLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            bodyLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            bodyLayoutPanel.Controls.Add(iconColumnPanel, 0, 0);
            bodyLayoutPanel.Controls.Add(messageLabel, 1, 0);
            bodyLayoutPanel.Dock = DockStyle.Fill;
            bodyLayoutPanel.Location = new Point(0, 0);
            bodyLayoutPanel.Name = "bodyLayoutPanel";
            bodyLayoutPanel.Padding = new Padding(24, 20, 24, 12);
            bodyLayoutPanel.RowCount = 1;
            bodyLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bodyLayoutPanel.Size = new Size(618, 154);
            bodyLayoutPanel.TabIndex = 0;
            // 
            // iconColumnPanel
            // 
            iconColumnPanel.Controls.Add(iconBackPanel);
            iconColumnPanel.Dock = DockStyle.Fill;
            iconColumnPanel.Location = new Point(27, 23);
            iconColumnPanel.Name = "iconColumnPanel";
            iconColumnPanel.Size = new Size(94, 116);
            iconColumnPanel.TabIndex = 0;
            // 
            // iconBackPanel
            // 
            iconBackPanel.BackColor = Color.FromArgb(229, 242, 248);
            iconBackPanel.Controls.Add(iconPictureBox);
            iconBackPanel.Location = new Point(8, 10);
            iconBackPanel.Name = "iconBackPanel";
            iconBackPanel.Size = new Size(72, 72);
            iconBackPanel.TabIndex = 0;
            // 
            // iconPictureBox
            // 
            iconPictureBox.BackColor = Color.Transparent;
            iconPictureBox.ForeColor = Color.FromArgb(36, 129, 172);
            iconPictureBox.IconChar = IconChar.CommentDots;
            iconPictureBox.IconColor = Color.FromArgb(36, 129, 172);
            iconPictureBox.IconFont = IconFont.Auto;
            iconPictureBox.IconSize = 42;
            iconPictureBox.Location = new Point(15, 15);
            iconPictureBox.Name = "iconPictureBox";
            iconPictureBox.Size = new Size(42, 42);
            iconPictureBox.TabIndex = 0;
            iconPictureBox.TabStop = false;
            // 
            // messageLabel
            // 
            messageLabel.Dock = DockStyle.Fill;
            messageLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
            messageLabel.ForeColor = Color.FromArgb(31, 43, 62);
            messageLabel.Location = new Point(127, 20);
            messageLabel.Name = "messageLabel";
            messageLabel.Size = new Size(464, 122);
            messageLabel.TabIndex = 1;
            messageLabel.Text = "Message";
            messageLabel.TextAlign = ContentAlignment.MiddleLeft;
            messageLabel.MouseDown += DragSource_MouseDown;
            messageLabel.MouseMove += DragSource_MouseMove;
            messageLabel.MouseUp += DragSource_MouseUp;
            // 
            // detailsPanel
            // 
            detailsPanel.BackColor = Color.FromArgb(245, 248, 251);
            detailsPanel.Controls.Add(detailsTextBox);
            detailsPanel.Controls.Add(detailsHeaderPanel);
            detailsPanel.Dock = DockStyle.Bottom;
            detailsPanel.Location = new Point(0, 84);
            detailsPanel.Name = "detailsPanel";
            detailsPanel.Padding = new Padding(24, 0, 24, 12);
            detailsPanel.Size = new Size(618, 120);
            detailsPanel.TabIndex = 4;
            detailsPanel.Visible = false;
            // 
            // detailsTextBox
            // 
            detailsTextBox.BackColor = Color.FromArgb(31, 39, 54);
            detailsTextBox.BorderStyle = BorderStyle.FixedSingle;
            detailsTextBox.Dock = DockStyle.Fill;
            detailsTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            detailsTextBox.ForeColor = Color.FromArgb(228, 235, 244);
            detailsTextBox.Location = new Point(24, 30);
            detailsTextBox.Multiline = true;
            detailsTextBox.Name = "detailsTextBox";
            detailsTextBox.ReadOnly = true;
            detailsTextBox.ScrollBars = ScrollBars.Vertical;
            detailsTextBox.Size = new Size(570, 78);
            detailsTextBox.TabIndex = 1;
            // 
            // detailsHeaderPanel
            // 
            detailsHeaderPanel.Controls.Add(detailsLabel);
            detailsHeaderPanel.Controls.Add(copyDetailsButton);
            detailsHeaderPanel.Dock = DockStyle.Top;
            detailsHeaderPanel.Location = new Point(24, 0);
            detailsHeaderPanel.Name = "detailsHeaderPanel";
            detailsHeaderPanel.Size = new Size(570, 30);
            detailsHeaderPanel.TabIndex = 0;
            // 
            // detailsLabel
            // 
            detailsLabel.Dock = DockStyle.Left;
            detailsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            detailsLabel.ForeColor = Color.FromArgb(58, 78, 113);
            detailsLabel.Location = new Point(0, 0);
            detailsLabel.Name = "detailsLabel";
            detailsLabel.Size = new Size(160, 30);
            detailsLabel.TabIndex = 0;
            detailsLabel.Text = "Technical details";
            detailsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // copyDetailsButton
            // 
            copyDetailsButton.Dock = DockStyle.Right;
            copyDetailsButton.FlatStyle = FlatStyle.Flat;
            copyDetailsButton.Font = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point);
            copyDetailsButton.ForeColor = Color.FromArgb(50, 73, 111);
            copyDetailsButton.Location = new Point(470, 0);
            copyDetailsButton.Name = "copyDetailsButton";
            copyDetailsButton.Size = new Size(100, 30);
            copyDetailsButton.TabIndex = 1;
            copyDetailsButton.Text = "Copy";
            copyDetailsButton.UseVisualStyleBackColor = true;
            copyDetailsButton.Click += CopyDetailsButton_Click;
            // 
            // footerPanel
            // 
            footerPanel.BackColor = Color.FromArgb(232, 239, 246);
            footerPanel.Controls.Add(detailsButton);
            footerPanel.Controls.Add(buttonFlowPanel);
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Location = new Point(0, 204);
            footerPanel.Name = "footerPanel";
            footerPanel.Padding = new Padding(20, 0, 20, 0);
            footerPanel.Size = new Size(618, 64);
            footerPanel.TabIndex = 2;
            // 
            // buttonFlowPanel
            // 
            buttonFlowPanel.Dock = DockStyle.Right;
            buttonFlowPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonFlowPanel.Location = new Point(190, 0);
            buttonFlowPanel.Name = "buttonFlowPanel";
            buttonFlowPanel.Size = new Size(408, 64);
            buttonFlowPanel.TabIndex = 0;
            // 
            // primaryButton
            // 
            primaryButton.BackColor = Color.FromArgb(36, 129, 172);
            primaryButton.FlatStyle = FlatStyle.Flat;
            primaryButton.ForeColor = Color.White;
            primaryButton.Location = new Point(0, 0);
            primaryButton.Name = "primaryButton";
            primaryButton.Size = new Size(75, 23);
            primaryButton.TabIndex = 0;
            primaryButton.Text = "OK";
            primaryButton.UseVisualStyleBackColor = false;
            primaryButton.Visible = false;
            // 
            // detailsButton
            // 
            detailsButton.FlatStyle = FlatStyle.Flat;
            detailsButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            detailsButton.ForeColor = Color.FromArgb(50, 73, 111);
            detailsButton.Location = new Point(24, 15);
            detailsButton.Name = "detailsButton";
            detailsButton.Size = new Size(120, 34);
            detailsButton.TabIndex = 1;
            detailsButton.Text = "Details";
            detailsButton.UseVisualStyleBackColor = true;
            detailsButton.Visible = false;
            detailsButton.Click += DetailsButton_Click;
            // 
            // accentBar
            // 
            accentBar.BackColor = Color.FromArgb(36, 129, 172);
            accentBar.Dock = DockStyle.Top;
            accentBar.Location = new Point(0, 46);
            accentBar.Name = "accentBar";
            accentBar.Size = new Size(618, 4);
            accentBar.TabIndex = 3;
            // 
            // titlePanel
            // 
            titlePanel.BackColor = Color.FromArgb(53, 70, 108);
            titlePanel.Controls.Add(titleLabel);
            titlePanel.Controls.Add(closeButton);
            titlePanel.Controls.Add(headerIcon);
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Location = new Point(0, 0);
            titlePanel.Name = "titlePanel";
            titlePanel.Size = new Size(618, 46);
            titlePanel.TabIndex = 0;
            titlePanel.MouseDown += DragSource_MouseDown;
            titlePanel.MouseMove += DragSource_MouseMove;
            titlePanel.MouseUp += DragSource_MouseUp;
            // 
            // titleLabel
            // 
            titleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            titleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(48, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(520, 46);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Message";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            titleLabel.MouseDown += DragSource_MouseDown;
            titleLabel.MouseMove += DragSource_MouseMove;
            titleLabel.MouseUp += DragSource_MouseUp;
            // 
            // closeButton
            // 
            closeButton.Dock = DockStyle.Right;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 52, 82);
            closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(73, 91, 139);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            closeButton.ForeColor = Color.White;
            closeButton.Location = new Point(574, 0);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(44, 46);
            closeButton.TabIndex = 2;
            closeButton.Text = "x";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += CloseButton_Click;
            // 
            // headerIcon
            // 
            headerIcon.BackColor = Color.Transparent;
            headerIcon.ForeColor = Color.White;
            headerIcon.IconChar = IconChar.CommentDots;
            headerIcon.IconColor = Color.White;
            headerIcon.IconFont = IconFont.Auto;
            headerIcon.IconSize = 22;
            headerIcon.Location = new Point(18, 12);
            headerIcon.Name = "headerIcon";
            headerIcon.Size = new Size(22, 22);
            headerIcon.TabIndex = 0;
            headerIcon.TabStop = false;
            // 
            // VisionMessageBoxForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 245, 250);
            ClientSize = new Size(620, 270);
            ControlBox = false;
            Controls.Add(borderPanel);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            MinimumSize = new Size(480, 230);
            Name = "VisionMessageBoxForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Message";
            borderPanel.ResumeLayout(false);
            contentPanel.ResumeLayout(false);
            bodyPanel.ResumeLayout(false);
            bodyLayoutPanel.ResumeLayout(false);
            iconColumnPanel.ResumeLayout(false);
            iconBackPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)iconPictureBox).EndInit();
            detailsPanel.ResumeLayout(false);
            detailsPanel.PerformLayout();
            detailsHeaderPanel.ResumeLayout(false);
            footerPanel.ResumeLayout(false);
            titlePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)headerIcon).EndInit();
            ResumeLayout(false);
        }
    }
}
