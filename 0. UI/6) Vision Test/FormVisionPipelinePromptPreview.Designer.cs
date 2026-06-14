using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelinePromptPreview
    {
        private TableLayoutPanel rootLayout;
        private Label captionLabel;
        private Panel footerPanel;
        private Button copyButton;
        private Button closeButton;

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            captionLabel = new Label();
            textBox = new TextBox();
            footerPanel = new Panel();
            copyButton = new Button();
            closeButton = new Button();
            rootLayout.SuspendLayout();
            footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(captionLabel, 0, 0);
            rootLayout.Controls.Add(textBox, 0, 1);
            rootLayout.Controls.Add(footerPanel, 0, 2);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(12);
            rootLayout.RowCount = 3;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            rootLayout.Size = new Size(860, 620);
            rootLayout.TabIndex = 0;
            // 
            // captionLabel
            // 
            captionLabel.Dock = DockStyle.Fill;
            captionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            captionLabel.ForeColor = Color.FromArgb(35, 85, 132);
            captionLabel.Location = new Point(15, 12);
            captionLabel.Name = "captionLabel";
            captionLabel.Size = new Size(830, 30);
            captionLabel.TabIndex = 0;
            captionLabel.Text = "Review the prompt, adjust if needed, then copy it for your LLM.";
            captionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox
            // 
            textBox.AcceptsReturn = true;
            textBox.AcceptsTab = true;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Dock = DockStyle.Fill;
            textBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBox.Location = new Point(15, 45);
            textBox.Multiline = true;
            textBox.Name = "textBox";
            textBox.ScrollBars = ScrollBars.Both;
            textBox.Size = new Size(830, 518);
            textBox.TabIndex = 1;
            textBox.WordWrap = false;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(copyButton);
            footerPanel.Controls.Add(closeButton);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.Location = new Point(15, 569);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(830, 36);
            footerPanel.TabIndex = 2;
            // 
            // copyButton
            // 
            copyButton.BackColor = Color.FromArgb(250, 252, 253);
            copyButton.DialogResult = DialogResult.OK;
            copyButton.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            copyButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            copyButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            copyButton.FlatStyle = FlatStyle.Flat;
            copyButton.ForeColor = Color.FromArgb(35, 85, 132);
            copyButton.Location = new Point(0, 7);
            copyButton.Name = "copyButton";
            copyButton.Size = new Size(88, 28);
            copyButton.TabIndex = 0;
            copyButton.Text = "Copy";
            copyButton.UseVisualStyleBackColor = false;
            // 
            // closeButton
            // 
            closeButton.BackColor = Color.FromArgb(250, 252, 253);
            closeButton.DialogResult = DialogResult.Cancel;
            closeButton.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.ForeColor = Color.FromArgb(35, 85, 132);
            closeButton.Location = new Point(96, 7);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(88, 28);
            closeButton.TabIndex = 1;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = false;
            // 
            // FormVisionPipelinePromptPreview
            // 
            AcceptButton = copyButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            CancelButton = closeButton;
            ClientSize = new Size(860, 620);
            Controls.Add(rootLayout);
            MinimumSize = new Size(720, 520);
            Name = "FormVisionPipelinePromptPreview";
            StartPosition = FormStartPosition.CenterParent;
            Text = "AI Recipe Prompt";
            rootLayout.ResumeLayout(false);
            rootLayout.PerformLayout();
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
