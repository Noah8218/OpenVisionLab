using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineTextPrompt
    {
        private TableLayoutPanel rootLayout;
        private Label titleLabel;
        private Label promptLabel;
        private TextBox textBox;
        private FlowLayoutPanel footerPanel;
        private Button okButton;
        private Button cancelButton;

		private void InitializeComponent()
		{
			rootLayout = new TableLayoutPanel();
			titleLabel = new Label();
			promptLabel = new Label();
			textBox = new TextBox();
			footerPanel = new FlowLayoutPanel();
			cancelButton = new Button();
			okButton = new Button();
			rootLayout.SuspendLayout();
			footerPanel.SuspendLayout();
			SuspendLayout();
			// 
			// rootLayout
			// 
			rootLayout.ColumnCount = 1;
			rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			rootLayout.Controls.Add(titleLabel, 0, 0);
			rootLayout.Controls.Add(promptLabel, 0, 1);
			rootLayout.Controls.Add(textBox, 0, 2);
			rootLayout.Controls.Add(footerPanel, 0, 3);
			rootLayout.Dock = DockStyle.Fill;
			rootLayout.Location = new Point(0, 0);
			rootLayout.Name = "rootLayout";
			rootLayout.Padding = new Padding(18, 16, 18, 14);
			rootLayout.RowCount = 4;
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
			rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
			rootLayout.Size = new Size(480, 220);
			rootLayout.TabIndex = 0;
			// 
			// titleLabel
			// 
			titleLabel.AutoEllipsis = true;
			titleLabel.Dock = DockStyle.Fill;
			titleLabel.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
			titleLabel.ForeColor = Color.FromArgb(35, 85, 132);
			titleLabel.Location = new Point(21, 16);
			titleLabel.Name = "titleLabel";
			titleLabel.Size = new Size(438, 34);
			titleLabel.TabIndex = 0;
			titleLabel.Text = "Save Sample";
			titleLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// promptLabel
			// 
			promptLabel.AutoEllipsis = true;
			promptLabel.Dock = DockStyle.Fill;
			promptLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			promptLabel.ForeColor = Color.FromArgb(71, 91, 119);
			promptLabel.Location = new Point(21, 50);
			promptLabel.Name = "promptLabel";
			promptLabel.Size = new Size(438, 30);
			promptLabel.TabIndex = 1;
			promptLabel.Text = "Sample Name";
			promptLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// textBox
			// 
			textBox.BorderStyle = BorderStyle.FixedSingle;
			textBox.Dock = DockStyle.Fill;
			textBox.Location = new Point(18, 85);
			textBox.Margin = new Padding(0, 5, 0, 8);
			textBox.Name = "textBox";
			textBox.Size = new Size(444, 23);
			textBox.TabIndex = 2;
			textBox.Text = "Contour_TextSymbols_20260614";
			// 
			// footerPanel
			// 
			footerPanel.Controls.Add(cancelButton);
			footerPanel.Controls.Add(okButton);
			footerPanel.Dock = DockStyle.Fill;
			footerPanel.FlowDirection = FlowDirection.RightToLeft;
			footerPanel.Location = new Point(21, 125);
			footerPanel.Name = "footerPanel";
			footerPanel.Padding = new Padding(0, 12, 0, 0);
			footerPanel.Size = new Size(438, 78);
			footerPanel.TabIndex = 3;
			footerPanel.WrapContents = false;
			// 
			// cancelButton
			// 
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.Location = new Point(342, 12);
			cancelButton.Margin = new Padding(8, 0, 0, 0);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(96, 32);
			cancelButton.TabIndex = 0;
			cancelButton.Text = "Cancel";
			// 
			// okButton
			// 
			okButton.DialogResult = DialogResult.OK;
			okButton.Location = new Point(238, 12);
			okButton.Margin = new Padding(8, 0, 0, 0);
			okButton.Name = "okButton";
			okButton.Size = new Size(96, 32);
			okButton.TabIndex = 1;
			okButton.Text = "OK";
			// 
			// FormVisionPipelineTextPrompt
			// 
			AcceptButton = okButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(238, 242, 246);
			CancelButton = cancelButton;
			ClientSize = new Size(480, 220);
			Controls.Add(rootLayout);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(480, 250);
			Name = "FormVisionPipelineTextPrompt";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Save Sample";
			rootLayout.ResumeLayout(false);
			rootLayout.PerformLayout();
			footerPanel.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
