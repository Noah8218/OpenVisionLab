using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineImageViewer
    {
        private TableLayoutPanel rootLayout;
        private Panel toolbarPanel;
        private Label labelModeLabel;
        private Label pointsLabel;
        private Button btnFit;
        private Button btnActual;
        private Button btnDouble;
        private Label toneLabel;
        private Label strokeLabel;
        private TableLayoutPanel bodyLayout;
        private Panel overlayHostPanel;
        private TableLayoutPanel overlayLayout;
        private Label overlayListLabel;
        private Label selectedOverlayLabel;

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            toolbarPanel = new Panel();
            chkBoxes = new CheckBox();
            labelModeLabel = new Label();
            cbLabelMode = new ComboBox();
            pointsLabel = new Label();
            nudPointLimit = new NumericUpDown();
            btnFit = new Button();
            btnActual = new Button();
            btnDouble = new Button();
            toneLabel = new Label();
            cbOverlayTone = new ComboBox();
            strokeLabel = new Label();
            nudStrokeWidth = new NumericUpDown();
            canvas = new PipelineOverlayImageCanvas();
            overlayGrid = new DataGridView();
            tbOverlayDetail = new TextBox();
            bodyLayout = new TableLayoutPanel();
            overlayHostPanel = new Panel();
            overlayLayout = new TableLayoutPanel();
            overlayListLabel = new Label();
            selectedOverlayLabel = new Label();
            statusLabel = new Label();
            rootLayout.SuspendLayout();
            toolbarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPointLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStrokeWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)overlayGrid).BeginInit();
            bodyLayout.SuspendLayout();
            overlayHostPanel.SuspendLayout();
            overlayLayout.SuspendLayout();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(toolbarPanel, 0, 0);
            rootLayout.Controls.Add(bodyLayout, 0, 1);
            rootLayout.Controls.Add(statusLabel, 0, 2);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(10);
            rootLayout.RowCount = 3;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            rootLayout.Size = new Size(1180, 760);
            rootLayout.TabIndex = 0;
            // 
            // toolbarPanel
            // 
            toolbarPanel.Controls.Add(chkBoxes);
            toolbarPanel.Controls.Add(labelModeLabel);
            toolbarPanel.Controls.Add(cbLabelMode);
            toolbarPanel.Controls.Add(pointsLabel);
            toolbarPanel.Controls.Add(nudPointLimit);
            toolbarPanel.Controls.Add(btnFit);
            toolbarPanel.Controls.Add(btnActual);
            toolbarPanel.Controls.Add(btnDouble);
            toolbarPanel.Controls.Add(toneLabel);
            toolbarPanel.Controls.Add(cbOverlayTone);
            toolbarPanel.Controls.Add(strokeLabel);
            toolbarPanel.Controls.Add(nudStrokeWidth);
            toolbarPanel.Dock = DockStyle.Fill;
            toolbarPanel.Location = new Point(13, 13);
            toolbarPanel.Name = "toolbarPanel";
            toolbarPanel.Size = new Size(1154, 30);
            toolbarPanel.TabIndex = 0;
            // 
            // chkBoxes
            // 
            chkBoxes.AutoSize = true;
            chkBoxes.Checked = true;
            chkBoxes.CheckState = CheckState.Checked;
            chkBoxes.ForeColor = Color.FromArgb(35, 85, 132);
            chkBoxes.Location = new Point(0, 8);
            chkBoxes.Name = "chkBoxes";
            chkBoxes.Size = new Size(56, 19);
            chkBoxes.TabIndex = 0;
            chkBoxes.Text = "Boxes";
            // 
            // labelModeLabel
            // 
            labelModeLabel.AutoSize = true;
            labelModeLabel.ForeColor = Color.FromArgb(35, 85, 132);
            labelModeLabel.Location = new Point(76, 9);
            labelModeLabel.Name = "labelModeLabel";
            labelModeLabel.Size = new Size(35, 15);
            labelModeLabel.TabIndex = 1;
            labelModeLabel.Text = "Label";
            // 
            // cbLabelMode
            // 
            cbLabelMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLabelMode.FlatStyle = FlatStyle.Flat;
            cbLabelMode.FormattingEnabled = true;
            cbLabelMode.Items.AddRange(new object[] { "None", "No", "Details" });
            cbLabelMode.Location = new Point(120, 5);
            cbLabelMode.Name = "cbLabelMode";
            cbLabelMode.SelectedIndex = 1;
            cbLabelMode.Size = new Size(92, 23);
            cbLabelMode.TabIndex = 2;
            // 
            // pointsLabel
            // 
            pointsLabel.AutoSize = true;
            pointsLabel.ForeColor = Color.FromArgb(35, 85, 132);
            pointsLabel.Location = new Point(226, 9);
            pointsLabel.Name = "pointsLabel";
            pointsLabel.Size = new Size(40, 15);
            pointsLabel.TabIndex = 3;
            pointsLabel.Text = "Points";
            // 
            // nudPointLimit
            // 
            nudPointLimit.Increment = 50;
            nudPointLimit.Location = new Point(276, 5);
            nudPointLimit.Maximum = 5000;
            nudPointLimit.Name = "nudPointLimit";
            nudPointLimit.Size = new Size(72, 23);
            nudPointLimit.TabIndex = 4;
            nudPointLimit.Value = 300;
            // 
            // btnFit
            // 
            btnFit.BackColor = Color.FromArgb(250, 252, 253);
            btnFit.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnFit.FlatStyle = FlatStyle.Flat;
            btnFit.ForeColor = Color.FromArgb(35, 85, 132);
            btnFit.Location = new Point(370, 4);
            btnFit.Name = "btnFit";
            btnFit.Size = new Size(58, 26);
            btnFit.TabIndex = 5;
            btnFit.Text = "Fit";
            btnFit.UseVisualStyleBackColor = false;
            btnFit.Click += OnFitClicked;
            // 
            // btnActual
            // 
            btnActual.BackColor = Color.FromArgb(250, 252, 253);
            btnActual.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnActual.FlatStyle = FlatStyle.Flat;
            btnActual.ForeColor = Color.FromArgb(35, 85, 132);
            btnActual.Location = new Point(426, 4);
            btnActual.Name = "btnActual";
            btnActual.Size = new Size(58, 26);
            btnActual.TabIndex = 6;
            btnActual.Text = "100%";
            btnActual.UseVisualStyleBackColor = false;
            btnActual.Click += OnActualClicked;
            // 
            // btnDouble
            // 
            btnDouble.BackColor = Color.FromArgb(250, 252, 253);
            btnDouble.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            btnDouble.FlatStyle = FlatStyle.Flat;
            btnDouble.ForeColor = Color.FromArgb(35, 85, 132);
            btnDouble.Location = new Point(492, 4);
            btnDouble.Name = "btnDouble";
            btnDouble.Size = new Size(58, 26);
            btnDouble.TabIndex = 7;
            btnDouble.Text = "200%";
            btnDouble.UseVisualStyleBackColor = false;
            btnDouble.Click += OnDoubleClicked;
            // 
            // toneLabel
            // 
            toneLabel.AutoSize = true;
            toneLabel.ForeColor = Color.FromArgb(35, 85, 132);
            toneLabel.Location = new Point(570, 9);
            toneLabel.Name = "toneLabel";
            toneLabel.Size = new Size(36, 15);
            toneLabel.TabIndex = 8;
            toneLabel.Text = "Color";
            // 
            // cbOverlayTone
            // 
            cbOverlayTone.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOverlayTone.FlatStyle = FlatStyle.Flat;
            cbOverlayTone.FormattingEnabled = true;
            cbOverlayTone.Items.AddRange(new object[] { "Green", "Cyan", "Orange", "Magenta" });
            cbOverlayTone.Location = new Point(614, 5);
            cbOverlayTone.Name = "cbOverlayTone";
            cbOverlayTone.SelectedIndex = 0;
            cbOverlayTone.Size = new Size(90, 23);
            cbOverlayTone.TabIndex = 9;
            // 
            // strokeLabel
            // 
            strokeLabel.AutoSize = true;
            strokeLabel.ForeColor = Color.FromArgb(35, 85, 132);
            strokeLabel.Location = new Point(718, 9);
            strokeLabel.Name = "strokeLabel";
            strokeLabel.Size = new Size(29, 15);
            strokeLabel.TabIndex = 10;
            strokeLabel.Text = "Line";
            // 
            // nudStrokeWidth
            // 
            nudStrokeWidth.Location = new Point(754, 5);
            nudStrokeWidth.Maximum = 8;
            nudStrokeWidth.Minimum = 1;
            nudStrokeWidth.Name = "nudStrokeWidth";
            nudStrokeWidth.Size = new Size(54, 23);
            nudStrokeWidth.TabIndex = 11;
            nudStrokeWidth.Value = 2;
            // 
            // canvas
            // 
            canvas.BackColor = Color.FromArgb(15, 18, 22);
            canvas.Dock = DockStyle.Fill;
            canvas.LabelMode = PipelineOverlayLabelMode.Number;
            canvas.Location = new Point(3, 3);
            canvas.Name = "canvas";
            canvas.OverlayColor = Color.FromArgb(0, 210, 120);
            canvas.PointLimit = 300;
            canvas.ShowBoxes = true;
            canvas.Size = new Size(838, 674);
            canvas.StrokeWidth = 2F;
            canvas.TabIndex = 0;
            canvas.TabStop = true;
            // 
            // overlayGrid
            // 
            overlayGrid.AllowUserToAddRows = false;
            overlayGrid.AllowUserToDeleteRows = false;
            overlayGrid.AllowUserToResizeRows = false;
            overlayGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 248, 252);
            overlayGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            overlayGrid.BackgroundColor = Color.White;
            overlayGrid.BorderStyle = BorderStyle.FixedSingle;
            overlayGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            overlayGrid.Columns.Add("No", "No");
            overlayGrid.Columns.Add("Kind", "Kind");
            overlayGrid.Columns.Add("Area", "Area");
            overlayGrid.Columns.Add("Center", "Center");
            overlayGrid.Columns[0].FillWeight = 18F;
            overlayGrid.Columns[1].FillWeight = 34F;
            overlayGrid.Columns[2].FillWeight = 24F;
            overlayGrid.Columns[3].FillWeight = 24F;
            overlayGrid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            overlayGrid.Dock = DockStyle.Fill;
            overlayGrid.Location = new Point(11, 27);
            overlayGrid.MultiSelect = false;
            overlayGrid.Name = "overlayGrid";
            overlayGrid.ReadOnly = true;
            overlayGrid.RowHeadersVisible = false;
            overlayGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            overlayGrid.Size = new Size(296, 386);
            overlayGrid.TabIndex = 1;
            // 
            // tbOverlayDetail
            // 
            tbOverlayDetail.BackColor = Color.FromArgb(250, 252, 253);
            tbOverlayDetail.BorderStyle = BorderStyle.FixedSingle;
            tbOverlayDetail.Dock = DockStyle.Fill;
            tbOverlayDetail.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            tbOverlayDetail.ForeColor = Color.FromArgb(25, 55, 84);
            tbOverlayDetail.Location = new Point(11, 443);
            tbOverlayDetail.Multiline = true;
            tbOverlayDetail.Name = "tbOverlayDetail";
            tbOverlayDetail.ReadOnly = true;
            tbOverlayDetail.ScrollBars = ScrollBars.Vertical;
            tbOverlayDetail.Size = new Size(296, 228);
            tbOverlayDetail.TabIndex = 3;
            // 
            // bodyLayout
            // 
            bodyLayout.ColumnCount = 2;
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310F));
            bodyLayout.Controls.Add(canvas, 0, 0);
            bodyLayout.Controls.Add(overlayHostPanel, 1, 0);
            bodyLayout.Dock = DockStyle.Fill;
            bodyLayout.Location = new Point(13, 49);
            bodyLayout.Name = "bodyLayout";
            bodyLayout.RowCount = 1;
            bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bodyLayout.Size = new Size(1154, 680);
            bodyLayout.TabIndex = 1;
            // 
            // overlayHostPanel
            // 
            overlayHostPanel.BackColor = Color.FromArgb(238, 242, 246);
            overlayHostPanel.Controls.Add(overlayLayout);
            overlayHostPanel.Dock = DockStyle.Fill;
            overlayHostPanel.Location = new Point(847, 3);
            overlayHostPanel.Name = "overlayHostPanel";
            overlayHostPanel.Size = new Size(304, 674);
            overlayHostPanel.TabIndex = 2;
            // 
            // overlayLayout
            // 
            overlayLayout.ColumnCount = 1;
            overlayLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            overlayLayout.Controls.Add(overlayListLabel, 0, 0);
            overlayLayout.Controls.Add(overlayGrid, 0, 1);
            overlayLayout.Controls.Add(selectedOverlayLabel, 0, 2);
            overlayLayout.Controls.Add(tbOverlayDetail, 0, 3);
            overlayLayout.Dock = DockStyle.Fill;
            overlayLayout.Location = new Point(0, 0);
            overlayLayout.Name = "overlayLayout";
            overlayLayout.Padding = new Padding(8, 0, 0, 0);
            overlayLayout.RowCount = 4;
            overlayLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            overlayLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 62F));
            overlayLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            overlayLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 38F));
            overlayLayout.Size = new Size(304, 674);
            overlayLayout.TabIndex = 0;
            // 
            // overlayListLabel
            // 
            overlayListLabel.Dock = DockStyle.Fill;
            overlayListLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            overlayListLabel.ForeColor = Color.FromArgb(35, 85, 132);
            overlayListLabel.Location = new Point(11, 0);
            overlayListLabel.Name = "overlayListLabel";
            overlayListLabel.Size = new Size(290, 24);
            overlayListLabel.TabIndex = 0;
            overlayListLabel.Text = "Overlay List";
            overlayListLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // selectedOverlayLabel
            // 
            selectedOverlayLabel.Dock = DockStyle.Fill;
            selectedOverlayLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            selectedOverlayLabel.ForeColor = Color.FromArgb(35, 85, 132);
            selectedOverlayLabel.Location = new Point(11, 416);
            selectedOverlayLabel.Name = "selectedOverlayLabel";
            selectedOverlayLabel.Size = new Size(290, 24);
            selectedOverlayLabel.TabIndex = 2;
            selectedOverlayLabel.Text = "Selected Overlay";
            selectedOverlayLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // statusLabel
            // 
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            statusLabel.ForeColor = Color.FromArgb(35, 85, 132);
            statusLabel.Location = new Point(13, 732);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(1154, 18);
            statusLabel.TabIndex = 2;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FormVisionPipelineImageViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            ClientSize = new Size(1180, 760);
            Controls.Add(rootLayout);
            KeyPreview = true;
            MinimumSize = new Size(920, 520);
            Name = "FormVisionPipelineImageViewer";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pipeline Preview";
            chkBoxes.CheckedChanged += OnOptionChanged;
            cbLabelMode.SelectedIndexChanged += OnOptionChanged;
            nudPointLimit.ValueChanged += OnOptionChanged;
            cbOverlayTone.SelectedIndexChanged += OnOptionChanged;
            nudStrokeWidth.ValueChanged += OnOptionChanged;
            canvas.OverlaySelected += OnCanvasOverlaySelected;
            canvas.ViewChanged += OnCanvasViewChanged;
            overlayGrid.SelectionChanged += OnOverlayGridSelectionChanged;
            KeyDown += OnViewerKeyDown;
            rootLayout.ResumeLayout(false);
            toolbarPanel.ResumeLayout(false);
            toolbarPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPointLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStrokeWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)overlayGrid).EndInit();
            bodyLayout.ResumeLayout(false);
            overlayHostPanel.ResumeLayout(false);
            overlayLayout.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
