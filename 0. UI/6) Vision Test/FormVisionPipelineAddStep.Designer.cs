using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineAddStep
    {
        private TableLayoutPanel rootLayout;
        private Label captionLabel;
        private TableLayoutPanel fieldsLayout;
        private Label positionLabel;
        private Label toolLabel;
        private Label stepNameLabel;
        private Label inputLayerLabel;
        private Label outputLayerLabel;
        private TableLayoutPanel statusPanel;
        private FlowLayoutPanel footerPanel;
        private TableLayoutPanel flowPreviewLayout;
        private Panel inputCardPanel;
        private Panel toolCardPanel;
        private Panel outputCardPanel;
        private Label firstArrowLabel;
        private Label secondArrowLabel;

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            captionLabel = new Label();
            fieldsLayout = new TableLayoutPanel();
            positionLabel = new Label();
            toolLabel = new Label();
            stepNameLabel = new Label();
            inputLayerLabel = new Label();
            outputLayerLabel = new Label();
            insertPositionValue = new Label();
            cbToolType = new ComboBox();
            tbStepName = new TextBox();
            cbInputLayer = new ComboBox();
            tbOutputLayer = new TextBox();
            flowPreviewPanel = new Panel();
            flowPreviewLayout = new TableLayoutPanel();
            inputCardPanel = new Panel();
            inputCardTitle = new Label();
            inputCardValue = new Label();
            toolCardPanel = new Panel();
            toolCardTitle = new Label();
            toolCardValue = new Label();
            outputCardPanel = new Panel();
            outputCardTitle = new Label();
            outputCardValue = new Label();
            firstArrowLabel = new Label();
            secondArrowLabel = new Label();
            flowRelationLabel = new Label();
            chkAllowBranch = new CheckBox();
            validationLabel = new Label();
            statusPanel = new TableLayoutPanel();
            footerPanel = new FlowLayoutPanel();
            btnCancel = new Button();
            btnAdd = new Button();
            rootLayout.SuspendLayout();
            fieldsLayout.SuspendLayout();
            flowPreviewPanel.SuspendLayout();
            flowPreviewLayout.SuspendLayout();
            inputCardPanel.SuspendLayout();
            toolCardPanel.SuspendLayout();
            outputCardPanel.SuspendLayout();
            statusPanel.SuspendLayout();
            footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(captionLabel, 0, 0);
            rootLayout.Controls.Add(fieldsLayout, 0, 1);
            rootLayout.Controls.Add(flowPreviewPanel, 0, 2);
            rootLayout.Controls.Add(statusPanel, 0, 3);
            rootLayout.Controls.Add(footerPanel, 0, 4);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(14);
            rootLayout.RowCount = 5;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 186F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            rootLayout.Size = new Size(720, 470);
            rootLayout.TabIndex = 0;
            // 
            // captionLabel
            // 
            captionLabel.Dock = DockStyle.Fill;
            captionLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            captionLabel.ForeColor = Color.FromArgb(35, 85, 132);
            captionLabel.Location = new Point(17, 14);
            captionLabel.Name = "captionLabel";
            captionLabel.Size = new Size(686, 34);
            captionLabel.TabIndex = 0;
            captionLabel.Text = "Add Pipeline Step";
            captionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // fieldsLayout
            // 
            fieldsLayout.ColumnCount = 2;
            fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136F));
            fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            fieldsLayout.Controls.Add(positionLabel, 0, 0);
            fieldsLayout.Controls.Add(toolLabel, 0, 1);
            fieldsLayout.Controls.Add(stepNameLabel, 0, 2);
            fieldsLayout.Controls.Add(inputLayerLabel, 0, 3);
            fieldsLayout.Controls.Add(outputLayerLabel, 0, 4);
            fieldsLayout.Controls.Add(insertPositionValue, 1, 0);
            fieldsLayout.Controls.Add(cbToolType, 1, 1);
            fieldsLayout.Controls.Add(tbStepName, 1, 2);
            fieldsLayout.Controls.Add(cbInputLayer, 1, 3);
            fieldsLayout.Controls.Add(tbOutputLayer, 1, 4);
            fieldsLayout.Dock = DockStyle.Fill;
            fieldsLayout.Location = new Point(17, 51);
            fieldsLayout.Name = "fieldsLayout";
            fieldsLayout.RowCount = 5;
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            fieldsLayout.Size = new Size(686, 180);
            fieldsLayout.TabIndex = 1;
            // 
            // positionLabel
            // 
            positionLabel.Dock = DockStyle.Fill;
            positionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            positionLabel.ForeColor = Color.FromArgb(35, 85, 132);
            positionLabel.Location = new Point(3, 0);
            positionLabel.Name = "positionLabel";
            positionLabel.Size = new Size(130, 36);
            positionLabel.TabIndex = 0;
            positionLabel.Text = "Position";
            positionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolLabel
            // 
            toolLabel.Dock = DockStyle.Fill;
            toolLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            toolLabel.ForeColor = Color.FromArgb(35, 85, 132);
            toolLabel.Location = new Point(3, 36);
            toolLabel.Name = "toolLabel";
            toolLabel.Size = new Size(130, 36);
            toolLabel.TabIndex = 2;
            toolLabel.Text = "1. Tool";
            toolLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // stepNameLabel
            // 
            stepNameLabel.Dock = DockStyle.Fill;
            stepNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            stepNameLabel.ForeColor = Color.FromArgb(35, 85, 132);
            stepNameLabel.Location = new Point(3, 72);
            stepNameLabel.Name = "stepNameLabel";
            stepNameLabel.Size = new Size(130, 36);
            stepNameLabel.TabIndex = 4;
            stepNameLabel.Text = "2. Step Name";
            stepNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // inputLayerLabel
            // 
            inputLayerLabel.Dock = DockStyle.Fill;
            inputLayerLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            inputLayerLabel.ForeColor = Color.FromArgb(35, 85, 132);
            inputLayerLabel.Location = new Point(3, 108);
            inputLayerLabel.Name = "inputLayerLabel";
            inputLayerLabel.Size = new Size(130, 36);
            inputLayerLabel.TabIndex = 6;
            inputLayerLabel.Text = "3. Input Source";
            inputLayerLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // outputLayerLabel
            // 
            outputLayerLabel.Dock = DockStyle.Fill;
            outputLayerLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            outputLayerLabel.ForeColor = Color.FromArgb(35, 85, 132);
            outputLayerLabel.Location = new Point(3, 144);
            outputLayerLabel.Name = "outputLayerLabel";
            outputLayerLabel.Size = new Size(130, 36);
            outputLayerLabel.TabIndex = 8;
            outputLayerLabel.Text = "4. Output Result";
            outputLayerLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // insertPositionValue
            // 
            insertPositionValue.AutoEllipsis = true;
            insertPositionValue.BackColor = Color.FromArgb(244, 248, 252);
            insertPositionValue.BorderStyle = BorderStyle.FixedSingle;
            insertPositionValue.Dock = DockStyle.Fill;
            insertPositionValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            insertPositionValue.ForeColor = Color.FromArgb(35, 85, 132);
            insertPositionValue.Location = new Point(136, 5);
            insertPositionValue.Margin = new Padding(0, 5, 0, 4);
            insertPositionValue.Name = "insertPositionValue";
            insertPositionValue.Padding = new Padding(8, 0, 0, 0);
            insertPositionValue.Size = new Size(550, 27);
            insertPositionValue.TabIndex = 1;
            insertPositionValue.Text = "Designer preview";
            insertPositionValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cbToolType
            // 
            cbToolType.Dock = DockStyle.Fill;
            cbToolType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbToolType.FormattingEnabled = true;
            cbToolType.Location = new Point(136, 41);
            cbToolType.Margin = new Padding(0, 5, 0, 4);
            cbToolType.Name = "cbToolType";
            cbToolType.Size = new Size(550, 23);
            cbToolType.TabIndex = 3;
            cbToolType.SelectedIndexChanged += OnToolTypeChanged;
            // 
            // tbStepName
            // 
            tbStepName.BorderStyle = BorderStyle.FixedSingle;
            tbStepName.Dock = DockStyle.Fill;
            tbStepName.Location = new Point(136, 77);
            tbStepName.Margin = new Padding(0, 5, 0, 4);
            tbStepName.Name = "tbStepName";
            tbStepName.Size = new Size(550, 23);
            tbStepName.TabIndex = 5;
            tbStepName.TextChanged += OnStepNameTextChanged;
            // 
            // cbInputLayer
            // 
            cbInputLayer.Dock = DockStyle.Fill;
            cbInputLayer.DropDownStyle = ComboBoxStyle.DropDownList;
            cbInputLayer.FormattingEnabled = true;
            cbInputLayer.Location = new Point(136, 113);
            cbInputLayer.Margin = new Padding(0, 5, 0, 4);
            cbInputLayer.Name = "cbInputLayer";
            cbInputLayer.Size = new Size(550, 23);
            cbInputLayer.TabIndex = 7;
            cbInputLayer.SelectedIndexChanged += OnInputLayerChanged;
            // 
            // tbOutputLayer
            // 
            tbOutputLayer.BorderStyle = BorderStyle.FixedSingle;
            tbOutputLayer.Dock = DockStyle.Fill;
            tbOutputLayer.Location = new Point(136, 149);
            tbOutputLayer.Margin = new Padding(0, 5, 0, 4);
            tbOutputLayer.Name = "tbOutputLayer";
            tbOutputLayer.Size = new Size(550, 23);
            tbOutputLayer.TabIndex = 9;
            tbOutputLayer.TextChanged += OnOutputLayerTextChanged;
            // 
            // flowPreviewPanel
            // 
            flowPreviewPanel.BackColor = Color.FromArgb(250, 252, 253);
            flowPreviewPanel.BorderStyle = BorderStyle.FixedSingle;
            flowPreviewPanel.Controls.Add(flowPreviewLayout);
            flowPreviewPanel.Dock = DockStyle.Fill;
            flowPreviewPanel.Location = new Point(17, 237);
            flowPreviewPanel.Name = "flowPreviewPanel";
            flowPreviewPanel.Padding = new Padding(10);
            flowPreviewPanel.Size = new Size(686, 114);
            flowPreviewPanel.TabIndex = 2;
            // 
            // flowPreviewLayout
            // 
            flowPreviewLayout.ColumnCount = 5;
            flowPreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            flowPreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28F));
            flowPreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            flowPreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28F));
            flowPreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            flowPreviewLayout.Controls.Add(inputCardPanel, 0, 0);
            flowPreviewLayout.Controls.Add(firstArrowLabel, 1, 0);
            flowPreviewLayout.Controls.Add(toolCardPanel, 2, 0);
            flowPreviewLayout.Controls.Add(secondArrowLabel, 3, 0);
            flowPreviewLayout.Controls.Add(outputCardPanel, 4, 0);
            flowPreviewLayout.Controls.Add(flowRelationLabel, 0, 1);
            flowPreviewLayout.Dock = DockStyle.Fill;
            flowPreviewLayout.Location = new Point(10, 10);
            flowPreviewLayout.Name = "flowPreviewLayout";
            flowPreviewLayout.RowCount = 2;
            flowPreviewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            flowPreviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            flowPreviewLayout.Size = new Size(664, 92);
            flowPreviewLayout.TabIndex = 0;
            flowPreviewLayout.SetColumnSpan(flowRelationLabel, 5);
            // 
            // inputCardPanel
            // 
            inputCardPanel.BackColor = Color.FromArgb(244, 248, 252);
            inputCardPanel.Controls.Add(inputCardValue);
            inputCardPanel.Controls.Add(inputCardTitle);
            inputCardPanel.Dock = DockStyle.Fill;
            inputCardPanel.Location = new Point(0, 0);
            inputCardPanel.Margin = new Padding(0, 0, 0, 4);
            inputCardPanel.Name = "inputCardPanel";
            inputCardPanel.Padding = new Padding(8);
            inputCardPanel.Size = new Size(194, 60);
            inputCardPanel.TabIndex = 0;
            // 
            // inputCardTitle
            // 
            inputCardTitle.Dock = DockStyle.Top;
            inputCardTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
            inputCardTitle.ForeColor = Color.FromArgb(35, 85, 132);
            inputCardTitle.Location = new Point(8, 8);
            inputCardTitle.Name = "inputCardTitle";
            inputCardTitle.Size = new Size(178, 20);
            inputCardTitle.TabIndex = 1;
            inputCardTitle.Text = "INPUT SOURCE";
            inputCardTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // inputCardValue
            // 
            inputCardValue.AutoEllipsis = true;
            inputCardValue.Dock = DockStyle.Fill;
            inputCardValue.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            inputCardValue.ForeColor = Color.FromArgb(20, 38, 58);
            inputCardValue.Location = new Point(8, 28);
            inputCardValue.Name = "inputCardValue";
            inputCardValue.Size = new Size(178, 24);
            inputCardValue.TabIndex = 0;
            inputCardValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // toolCardPanel
            // 
            toolCardPanel.BackColor = Color.FromArgb(244, 248, 252);
            toolCardPanel.Controls.Add(toolCardValue);
            toolCardPanel.Controls.Add(toolCardTitle);
            toolCardPanel.Dock = DockStyle.Fill;
            toolCardPanel.Location = new Point(222, 0);
            toolCardPanel.Margin = new Padding(0, 0, 0, 4);
            toolCardPanel.Name = "toolCardPanel";
            toolCardPanel.Padding = new Padding(8);
            toolCardPanel.Size = new Size(170, 60);
            toolCardPanel.TabIndex = 2;
            // 
            // toolCardTitle
            // 
            toolCardTitle.Dock = DockStyle.Top;
            toolCardTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
            toolCardTitle.ForeColor = Color.FromArgb(35, 85, 132);
            toolCardTitle.Location = new Point(8, 8);
            toolCardTitle.Name = "toolCardTitle";
            toolCardTitle.Size = new Size(154, 20);
            toolCardTitle.TabIndex = 1;
            toolCardTitle.Text = "TOOL";
            toolCardTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolCardValue
            // 
            toolCardValue.AutoEllipsis = true;
            toolCardValue.Dock = DockStyle.Fill;
            toolCardValue.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            toolCardValue.ForeColor = Color.FromArgb(20, 38, 58);
            toolCardValue.Location = new Point(8, 28);
            toolCardValue.Name = "toolCardValue";
            toolCardValue.Size = new Size(154, 24);
            toolCardValue.TabIndex = 0;
            toolCardValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // outputCardPanel
            // 
            outputCardPanel.BackColor = Color.FromArgb(244, 248, 252);
            outputCardPanel.Controls.Add(outputCardValue);
            outputCardPanel.Controls.Add(outputCardTitle);
            outputCardPanel.Dock = DockStyle.Fill;
            outputCardPanel.Location = new Point(420, 0);
            outputCardPanel.Margin = new Padding(0, 0, 0, 4);
            outputCardPanel.Name = "outputCardPanel";
            outputCardPanel.Padding = new Padding(8);
            outputCardPanel.Size = new Size(244, 60);
            outputCardPanel.TabIndex = 4;
            // 
            // outputCardTitle
            // 
            outputCardTitle.Dock = DockStyle.Top;
            outputCardTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
            outputCardTitle.ForeColor = Color.FromArgb(35, 85, 132);
            outputCardTitle.Location = new Point(8, 8);
            outputCardTitle.Name = "outputCardTitle";
            outputCardTitle.Size = new Size(228, 20);
            outputCardTitle.TabIndex = 1;
            outputCardTitle.Text = "OUTPUT RESULT";
            outputCardTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // outputCardValue
            // 
            outputCardValue.AutoEllipsis = true;
            outputCardValue.Dock = DockStyle.Fill;
            outputCardValue.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            outputCardValue.ForeColor = Color.FromArgb(20, 38, 58);
            outputCardValue.Location = new Point(8, 28);
            outputCardValue.Name = "outputCardValue";
            outputCardValue.Size = new Size(228, 24);
            outputCardValue.TabIndex = 0;
            outputCardValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // firstArrowLabel
            // 
            firstArrowLabel.Dock = DockStyle.Fill;
            firstArrowLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            firstArrowLabel.ForeColor = Color.FromArgb(92, 111, 130);
            firstArrowLabel.Location = new Point(197, 0);
            firstArrowLabel.Name = "firstArrowLabel";
            firstArrowLabel.Size = new Size(22, 64);
            firstArrowLabel.TabIndex = 1;
            firstArrowLabel.Text = ">";
            firstArrowLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // secondArrowLabel
            // 
            secondArrowLabel.Dock = DockStyle.Fill;
            secondArrowLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            secondArrowLabel.ForeColor = Color.FromArgb(92, 111, 130);
            secondArrowLabel.Location = new Point(395, 0);
            secondArrowLabel.Name = "secondArrowLabel";
            secondArrowLabel.Size = new Size(22, 64);
            secondArrowLabel.TabIndex = 3;
            secondArrowLabel.Text = ">";
            secondArrowLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // flowRelationLabel
            // 
            flowRelationLabel.AutoEllipsis = true;
            flowRelationLabel.Dock = DockStyle.Fill;
            flowRelationLabel.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold, GraphicsUnit.Point);
            flowRelationLabel.ForeColor = Color.FromArgb(92, 111, 130);
            flowRelationLabel.Location = new Point(3, 64);
            flowRelationLabel.Name = "flowRelationLabel";
            flowRelationLabel.Size = new Size(658, 28);
            flowRelationLabel.TabIndex = 5;
            flowRelationLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // chkAllowBranch
            // 
            chkAllowBranch.Dock = DockStyle.Fill;
            chkAllowBranch.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold, GraphicsUnit.Point);
            chkAllowBranch.ForeColor = Color.FromArgb(168, 92, 0);
            chkAllowBranch.Location = new Point(3, 3);
            chkAllowBranch.Name = "chkAllowBranch";
            chkAllowBranch.Size = new Size(179, 24);
            chkAllowBranch.TabIndex = 0;
            chkAllowBranch.Text = "Allow branch input";
            chkAllowBranch.TextAlign = ContentAlignment.MiddleLeft;
            chkAllowBranch.Visible = false;
            chkAllowBranch.CheckedChanged += OnAllowBranchChanged;
            // 
            // validationLabel
            // 
            validationLabel.AutoEllipsis = true;
            validationLabel.Dock = DockStyle.Fill;
            validationLabel.ForeColor = Color.FromArgb(168, 55, 55);
            validationLabel.Location = new Point(188, 0);
            validationLabel.Name = "validationLabel";
            validationLabel.Size = new Size(495, 30);
            validationLabel.TabIndex = 1;
            validationLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // statusPanel
            // 
            statusPanel.ColumnCount = 2;
            statusPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F));
            statusPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            statusPanel.Controls.Add(chkAllowBranch, 0, 0);
            statusPanel.Controls.Add(validationLabel, 1, 0);
            statusPanel.Dock = DockStyle.Fill;
            statusPanel.Location = new Point(14, 354);
            statusPanel.Margin = new Padding(0);
            statusPanel.Name = "statusPanel";
            statusPanel.RowCount = 1;
            statusPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            statusPanel.Size = new Size(692, 30);
            statusPanel.TabIndex = 3;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(btnCancel);
            footerPanel.Controls.Add(btnAdd);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.FlowDirection = FlowDirection.RightToLeft;
            footerPanel.Location = new Point(17, 387);
            footerPanel.Name = "footerPanel";
            footerPanel.Padding = new Padding(0, 4, 0, 0);
            footerPanel.Size = new Size(686, 36);
            footerPanel.TabIndex = 4;
            footerPanel.WrapContents = false;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(250, 252, 253);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.FromArgb(35, 85, 132);
            btnCancel.Location = new Point(601, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(82, 28);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.FromArgb(250, 252, 253);
            btnAdd.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnAdd.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 232, 247);
            btnAdd.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.ForeColor = Color.FromArgb(35, 85, 132);
            btnAdd.Location = new Point(503, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(92, 28);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "Add Step";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += OnAddClicked;
            // 
            // FormVisionPipelineAddStep
            // 
            AcceptButton = btnAdd;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            CancelButton = btnCancel;
            ClientSize = new Size(720, 470);
            Controls.Add(rootLayout);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(720, 470);
            Name = "FormVisionPipelineAddStep";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Pipeline Step";
            toolTip.SetToolTip(cbToolType, "Vision tool to add to the pipeline.");
            toolTip.SetToolTip(insertPositionValue, "Where the new step will be inserted.");
            toolTip.SetToolTip(tbStepName, "Name shown in the step list.");
            toolTip.SetToolTip(cbInputLayer, "Image source used by this step. The previous output is selected by default; choosing another layer creates a branch.");
            toolTip.SetToolTip(tbOutputLayer, "Result layer created by this step. Keep it different from the input source.");
            toolTip.SetToolTip(flowPreviewPanel, "Shows how this new step will connect to the pipeline.");
            toolTip.SetToolTip(chkAllowBranch, "Turn this on only when this step should intentionally read from a layer other than the previous output.");
            rootLayout.ResumeLayout(false);
            fieldsLayout.ResumeLayout(false);
            fieldsLayout.PerformLayout();
            flowPreviewPanel.ResumeLayout(false);
            flowPreviewLayout.ResumeLayout(false);
            inputCardPanel.ResumeLayout(false);
            toolCardPanel.ResumeLayout(false);
            outputCardPanel.ResumeLayout(false);
            statusPanel.ResumeLayout(false);
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
