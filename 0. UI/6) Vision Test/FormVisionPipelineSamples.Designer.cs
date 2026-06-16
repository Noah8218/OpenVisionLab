using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineSamples
    {
        private TableLayoutPanel rootLayout;
        private TableLayoutPanel catalogLayout;
        private TableLayoutPanel catalogDetailLayout;
        private Panel catalogHeaderPanel;
        private Label catalogTitleLabel;
        private Label catalogStatusLabel;
        private Label catalogGoalLabel;
        private Label catalogLearningLabel;
        private Label catalogExpectedLabel;
        private TableLayoutPanel catalogPreviewLayout;
        private TableLayoutPanel catalogSourcePreviewLayout;
        private TableLayoutPanel catalogReferencePreviewLayout;
        private Label catalogSourcePreviewLabel;
        private Label catalogReferencePreviewLabel;
        private PictureBox catalogPreviewBox;
        private Panel catalogReferenceImagePanel;
        private PictureBox catalogReferenceBox;
        private Label catalogReferenceEmptyLabel;
        private TableLayoutPanel savedLayout;
        private Panel footerPanel;

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            tabSamples = new TabControl();
            tabCatalog = new TabPage();
            tabSaved = new TabPage();
            catalogLayout = new TableLayoutPanel();
            catalogList = new ListBox();
            catalogDetailLayout = new TableLayoutPanel();
            catalogHeaderPanel = new Panel();
            catalogTitleLabel = new Label();
            catalogStatusLabel = new Label();
            catalogGoalLabel = new Label();
            catalogLearningLabel = new Label();
            catalogExpectedLabel = new Label();
            catalogPreviewLayout = new TableLayoutPanel();
            catalogSourcePreviewLayout = new TableLayoutPanel();
            catalogSourcePreviewLabel = new Label();
            catalogPreviewBox = new PictureBox();
            catalogReferencePreviewLayout = new TableLayoutPanel();
            catalogReferencePreviewLabel = new Label();
            catalogReferenceImagePanel = new Panel();
            catalogReferenceBox = new PictureBox();
            catalogReferenceEmptyLabel = new Label();
            catalogDetailsText = new TextBox();
            savedLayout = new TableLayoutPanel();
            sampleList = new ListBox();
            detailsText = new TextBox();
            footerPanel = new Panel();
            btnOpenCatalog = new Button();
            btnCheckCatalog = new Button();
            btnSaveCurrent = new Button();
            btnLoad = new Button();
            btnDelete = new Button();
            btnRefresh = new Button();
            rootLayout.SuspendLayout();
            tabSamples.SuspendLayout();
            tabCatalog.SuspendLayout();
            tabSaved.SuspendLayout();
            catalogLayout.SuspendLayout();
            catalogDetailLayout.SuspendLayout();
            catalogHeaderPanel.SuspendLayout();
            catalogPreviewLayout.SuspendLayout();
            catalogSourcePreviewLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)catalogPreviewBox).BeginInit();
            catalogReferencePreviewLayout.SuspendLayout();
            catalogReferenceImagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)catalogReferenceBox).BeginInit();
            savedLayout.SuspendLayout();
            footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(tabSamples, 0, 0);
            rootLayout.Controls.Add(footerPanel, 0, 1);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(12);
            rootLayout.RowCount = 2;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            rootLayout.Size = new Size(940, 620);
            rootLayout.TabIndex = 0;
            // 
            // tabSamples
            // 
            tabSamples.Controls.Add(tabCatalog);
            tabSamples.Controls.Add(tabSaved);
            tabSamples.Dock = DockStyle.Fill;
            tabSamples.Location = new Point(15, 15);
            tabSamples.Name = "tabSamples";
            tabSamples.SelectedIndex = 0;
            tabSamples.Size = new Size(910, 542);
            tabSamples.TabIndex = 0;
            tabSamples.SelectedIndexChanged += OnTabChanged;
            // 
            // tabCatalog
            // 
            tabCatalog.BackColor = Color.FromArgb(238, 242, 246);
            tabCatalog.Controls.Add(catalogLayout);
            tabCatalog.Location = new Point(4, 24);
            tabCatalog.Name = "tabCatalog";
            tabCatalog.Padding = new Padding(8);
            tabCatalog.Size = new Size(902, 514);
            tabCatalog.TabIndex = 0;
            tabCatalog.Text = "Recipe Catalog";
            // 
            // tabSaved
            // 
            tabSaved.BackColor = Color.FromArgb(238, 242, 246);
            tabSaved.Controls.Add(savedLayout);
            tabSaved.Location = new Point(4, 24);
            tabSaved.Name = "tabSaved";
            tabSaved.Padding = new Padding(8);
            tabSaved.Size = new Size(902, 514);
            tabSaved.TabIndex = 1;
            tabSaved.Text = "Saved Workspace";
            // 
            // catalogLayout
            // 
            catalogLayout.ColumnCount = 2;
            catalogLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            catalogLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            catalogLayout.Controls.Add(catalogList, 0, 0);
            catalogLayout.Controls.Add(catalogDetailLayout, 1, 0);
            catalogLayout.Dock = DockStyle.Fill;
            catalogLayout.Location = new Point(8, 8);
            catalogLayout.Name = "catalogLayout";
            catalogLayout.RowCount = 1;
            catalogLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            catalogLayout.Size = new Size(886, 498);
            catalogLayout.TabIndex = 0;
            // 
            // catalogList
            // 
            catalogList.Dock = DockStyle.Fill;
            catalogList.FormattingEnabled = true;
            catalogList.HorizontalScrollbar = true;
            catalogList.IntegralHeight = false;
            catalogList.ItemHeight = 15;
            catalogList.Location = new Point(0, 0);
            catalogList.Margin = new Padding(0, 0, 10, 0);
            catalogList.Name = "catalogList";
            catalogList.Size = new Size(326, 498);
            catalogList.TabIndex = 0;
            catalogList.SelectedIndexChanged += OnCatalogSampleSelected;
            // 
            // catalogDetailLayout
            // 
            catalogDetailLayout.ColumnCount = 1;
            catalogDetailLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            catalogDetailLayout.Controls.Add(catalogHeaderPanel, 0, 0);
            catalogDetailLayout.Controls.Add(catalogPreviewLayout, 0, 1);
            catalogDetailLayout.Controls.Add(catalogDetailsText, 0, 2);
            catalogDetailLayout.Dock = DockStyle.Fill;
            catalogDetailLayout.Location = new Point(336, 0);
            catalogDetailLayout.Margin = new Padding(0);
            catalogDetailLayout.Name = "catalogDetailLayout";
            catalogDetailLayout.RowCount = 3;
            catalogDetailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 132F));
            catalogDetailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 190F));
            catalogDetailLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            catalogDetailLayout.Size = new Size(550, 498);
            catalogDetailLayout.TabIndex = 1;
            // 
            // catalogHeaderPanel
            // 
            catalogHeaderPanel.BackColor = Color.FromArgb(250, 252, 253);
            catalogHeaderPanel.BorderStyle = BorderStyle.FixedSingle;
            catalogHeaderPanel.Controls.Add(catalogTitleLabel);
            catalogHeaderPanel.Controls.Add(catalogStatusLabel);
            catalogHeaderPanel.Controls.Add(catalogGoalLabel);
            catalogHeaderPanel.Controls.Add(catalogLearningLabel);
            catalogHeaderPanel.Controls.Add(catalogExpectedLabel);
            catalogHeaderPanel.Dock = DockStyle.Fill;
            catalogHeaderPanel.Location = new Point(0, 0);
            catalogHeaderPanel.Margin = new Padding(0, 0, 0, 8);
            catalogHeaderPanel.Name = "catalogHeaderPanel";
            catalogHeaderPanel.Size = new Size(550, 124);
            catalogHeaderPanel.TabIndex = 0;
            // 
            // catalogTitleLabel
            // 
            catalogTitleLabel.AutoEllipsis = true;
            catalogTitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            catalogTitleLabel.ForeColor = Color.FromArgb(20, 66, 110);
            catalogTitleLabel.Location = new Point(12, 10);
            catalogTitleLabel.Name = "catalogTitleLabel";
            catalogTitleLabel.Size = new Size(432, 22);
            catalogTitleLabel.TabIndex = 0;
            catalogTitleLabel.Text = "Sample";
            // 
            // catalogStatusLabel
            // 
            catalogStatusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            catalogStatusLabel.BackColor = Color.FromArgb(0, 146, 92);
            catalogStatusLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            catalogStatusLabel.ForeColor = Color.White;
            catalogStatusLabel.Location = new Point(458, 10);
            catalogStatusLabel.Name = "catalogStatusLabel";
            catalogStatusLabel.Size = new Size(78, 22);
            catalogStatusLabel.TabIndex = 1;
            catalogStatusLabel.Text = "READY";
            catalogStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // catalogGoalLabel
            // 
            catalogGoalLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            catalogGoalLabel.ForeColor = Color.FromArgb(53, 71, 89);
            catalogGoalLabel.Location = new Point(12, 40);
            catalogGoalLabel.Name = "catalogGoalLabel";
            catalogGoalLabel.Size = new Size(524, 40);
            catalogGoalLabel.TabIndex = 2;
            catalogGoalLabel.Text = "Goal";
            // 
            // catalogLearningLabel
            // 
            catalogLearningLabel.AutoEllipsis = true;
            catalogLearningLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            catalogLearningLabel.BackColor = Color.FromArgb(246, 249, 252);
            catalogLearningLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            catalogLearningLabel.ForeColor = Color.FromArgb(35, 85, 132);
            catalogLearningLabel.Location = new Point(12, 80);
            catalogLearningLabel.Name = "catalogLearningLabel";
            catalogLearningLabel.Padding = new Padding(8, 0, 8, 0);
            catalogLearningLabel.Size = new Size(524, 22);
            catalogLearningLabel.TabIndex = 3;
            catalogLearningLabel.Text = "Learn";
            catalogLearningLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // catalogExpectedLabel
            // 
            catalogExpectedLabel.AutoEllipsis = true;
            catalogExpectedLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            catalogExpectedLabel.BackColor = Color.FromArgb(232, 241, 250);
            catalogExpectedLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            catalogExpectedLabel.ForeColor = Color.FromArgb(35, 85, 132);
            catalogExpectedLabel.Location = new Point(12, 104);
            catalogExpectedLabel.Name = "catalogExpectedLabel";
            catalogExpectedLabel.Padding = new Padding(8, 0, 8, 0);
            catalogExpectedLabel.Size = new Size(524, 18);
            catalogExpectedLabel.TabIndex = 4;
            catalogExpectedLabel.Text = "Expected";
            catalogExpectedLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // catalogPreviewLayout
            // 
            catalogPreviewLayout.ColumnCount = 2;
            catalogPreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            catalogPreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            catalogPreviewLayout.Controls.Add(catalogSourcePreviewLayout, 0, 0);
            catalogPreviewLayout.Controls.Add(catalogReferencePreviewLayout, 1, 0);
            catalogPreviewLayout.Dock = DockStyle.Fill;
            catalogPreviewLayout.Location = new Point(0, 132);
            catalogPreviewLayout.Margin = new Padding(0, 0, 0, 8);
            catalogPreviewLayout.Name = "catalogPreviewLayout";
            catalogPreviewLayout.RowCount = 1;
            catalogPreviewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            catalogPreviewLayout.Size = new Size(550, 182);
            catalogPreviewLayout.TabIndex = 1;
            // 
            // catalogSourcePreviewLayout
            // 
            catalogSourcePreviewLayout.ColumnCount = 1;
            catalogSourcePreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            catalogSourcePreviewLayout.Controls.Add(catalogSourcePreviewLabel, 0, 0);
            catalogSourcePreviewLayout.Controls.Add(catalogPreviewBox, 0, 1);
            catalogSourcePreviewLayout.Dock = DockStyle.Fill;
            catalogSourcePreviewLayout.Location = new Point(0, 0);
            catalogSourcePreviewLayout.Margin = new Padding(0, 0, 4, 0);
            catalogSourcePreviewLayout.Name = "catalogSourcePreviewLayout";
            catalogSourcePreviewLayout.RowCount = 2;
            catalogSourcePreviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            catalogSourcePreviewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            catalogSourcePreviewLayout.Size = new Size(271, 182);
            catalogSourcePreviewLayout.TabIndex = 0;
            // 
            // catalogSourcePreviewLabel
            // 
            catalogSourcePreviewLabel.BackColor = Color.FromArgb(35, 85, 132);
            catalogSourcePreviewLabel.Dock = DockStyle.Fill;
            catalogSourcePreviewLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            catalogSourcePreviewLabel.ForeColor = Color.White;
            catalogSourcePreviewLabel.Location = new Point(0, 0);
            catalogSourcePreviewLabel.Margin = new Padding(0);
            catalogSourcePreviewLabel.Name = "catalogSourcePreviewLabel";
            catalogSourcePreviewLabel.Padding = new Padding(6, 0, 6, 0);
            catalogSourcePreviewLabel.Size = new Size(271, 18);
            catalogSourcePreviewLabel.TabIndex = 0;
            catalogSourcePreviewLabel.Text = "Source";
            catalogSourcePreviewLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // catalogPreviewBox
            // 
            catalogPreviewBox.BackColor = Color.FromArgb(17, 24, 32);
            catalogPreviewBox.BorderStyle = BorderStyle.FixedSingle;
            catalogPreviewBox.Dock = DockStyle.Fill;
            catalogPreviewBox.Location = new Point(0, 18);
            catalogPreviewBox.Margin = new Padding(0);
            catalogPreviewBox.Name = "catalogPreviewBox";
            catalogPreviewBox.Size = new Size(271, 164);
            catalogPreviewBox.SizeMode = PictureBoxSizeMode.Zoom;
            catalogPreviewBox.TabIndex = 1;
            catalogPreviewBox.TabStop = false;
            // 
            // catalogReferencePreviewLayout
            // 
            catalogReferencePreviewLayout.ColumnCount = 1;
            catalogReferencePreviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            catalogReferencePreviewLayout.Controls.Add(catalogReferencePreviewLabel, 0, 0);
            catalogReferencePreviewLayout.Controls.Add(catalogReferenceImagePanel, 0, 1);
            catalogReferencePreviewLayout.Dock = DockStyle.Fill;
            catalogReferencePreviewLayout.Location = new Point(279, 0);
            catalogReferencePreviewLayout.Margin = new Padding(4, 0, 0, 0);
            catalogReferencePreviewLayout.Name = "catalogReferencePreviewLayout";
            catalogReferencePreviewLayout.RowCount = 2;
            catalogReferencePreviewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            catalogReferencePreviewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            catalogReferencePreviewLayout.Size = new Size(271, 182);
            catalogReferencePreviewLayout.TabIndex = 1;
            // 
            // catalogReferencePreviewLabel
            // 
            catalogReferencePreviewLabel.BackColor = Color.FromArgb(96, 113, 132);
            catalogReferencePreviewLabel.Dock = DockStyle.Fill;
            catalogReferencePreviewLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            catalogReferencePreviewLabel.ForeColor = Color.White;
            catalogReferencePreviewLabel.Location = new Point(0, 0);
            catalogReferencePreviewLabel.Margin = new Padding(0);
            catalogReferencePreviewLabel.Name = "catalogReferencePreviewLabel";
            catalogReferencePreviewLabel.Padding = new Padding(6, 0, 6, 0);
            catalogReferencePreviewLabel.Size = new Size(271, 18);
            catalogReferencePreviewLabel.TabIndex = 0;
            catalogReferencePreviewLabel.Text = "Expected Result";
            catalogReferencePreviewLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // catalogReferenceImagePanel
            // 
            catalogReferenceImagePanel.BackColor = Color.FromArgb(17, 24, 32);
            catalogReferenceImagePanel.BorderStyle = BorderStyle.FixedSingle;
            catalogReferenceImagePanel.Controls.Add(catalogReferenceBox);
            catalogReferenceImagePanel.Controls.Add(catalogReferenceEmptyLabel);
            catalogReferenceImagePanel.Dock = DockStyle.Fill;
            catalogReferenceImagePanel.Location = new Point(0, 18);
            catalogReferenceImagePanel.Margin = new Padding(0);
            catalogReferenceImagePanel.Name = "catalogReferenceImagePanel";
            catalogReferenceImagePanel.Size = new Size(271, 164);
            catalogReferenceImagePanel.TabIndex = 1;
            // 
            // catalogReferenceBox
            // 
            catalogReferenceBox.BackColor = Color.FromArgb(17, 24, 32);
            catalogReferenceBox.Dock = DockStyle.Fill;
            catalogReferenceBox.Location = new Point(0, 0);
            catalogReferenceBox.Margin = new Padding(0);
            catalogReferenceBox.Name = "catalogReferenceBox";
            catalogReferenceBox.Size = new Size(269, 162);
            catalogReferenceBox.SizeMode = PictureBoxSizeMode.Zoom;
            catalogReferenceBox.TabIndex = 0;
            catalogReferenceBox.TabStop = false;
            // 
            // catalogReferenceEmptyLabel
            // 
            catalogReferenceEmptyLabel.BackColor = Color.FromArgb(17, 24, 32);
            catalogReferenceEmptyLabel.Dock = DockStyle.Fill;
            catalogReferenceEmptyLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            catalogReferenceEmptyLabel.ForeColor = Color.FromArgb(150, 164, 180);
            catalogReferenceEmptyLabel.Location = new Point(0, 0);
            catalogReferenceEmptyLabel.Margin = new Padding(0);
            catalogReferenceEmptyLabel.Name = "catalogReferenceEmptyLabel";
            catalogReferenceEmptyLabel.Padding = new Padding(12);
            catalogReferenceEmptyLabel.Size = new Size(269, 162);
            catalogReferenceEmptyLabel.TabIndex = 1;
            catalogReferenceEmptyLabel.Text = "No expected result yet";
            catalogReferenceEmptyLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // catalogDetailsText
            // 
            catalogDetailsText.BackColor = Color.FromArgb(250, 252, 253);
            catalogDetailsText.BorderStyle = BorderStyle.FixedSingle;
            catalogDetailsText.Dock = DockStyle.Fill;
            catalogDetailsText.Location = new Point(0, 322);
            catalogDetailsText.Margin = new Padding(0);
            catalogDetailsText.Multiline = true;
            catalogDetailsText.Name = "catalogDetailsText";
            catalogDetailsText.ReadOnly = true;
            catalogDetailsText.ScrollBars = ScrollBars.Vertical;
            catalogDetailsText.Size = new Size(550, 176);
            catalogDetailsText.TabIndex = 2;
            // 
            // savedLayout
            // 
            savedLayout.ColumnCount = 2;
            savedLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            savedLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            savedLayout.Controls.Add(sampleList, 0, 0);
            savedLayout.Controls.Add(detailsText, 1, 0);
            savedLayout.Dock = DockStyle.Fill;
            savedLayout.Location = new Point(8, 8);
            savedLayout.Name = "savedLayout";
            savedLayout.RowCount = 1;
            savedLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            savedLayout.Size = new Size(886, 498);
            savedLayout.TabIndex = 0;
            // 
            // sampleList
            // 
            sampleList.Dock = DockStyle.Fill;
            sampleList.FormattingEnabled = true;
            sampleList.HorizontalScrollbar = true;
            sampleList.IntegralHeight = false;
            sampleList.ItemHeight = 15;
            sampleList.Location = new Point(0, 0);
            sampleList.Margin = new Padding(0, 0, 10, 0);
            sampleList.Name = "sampleList";
            sampleList.Size = new Size(326, 498);
            sampleList.TabIndex = 0;
            sampleList.SelectedIndexChanged += OnSampleSelected;
            // 
            // detailsText
            // 
            detailsText.BackColor = Color.FromArgb(250, 252, 253);
            detailsText.BorderStyle = BorderStyle.FixedSingle;
            detailsText.Dock = DockStyle.Fill;
            detailsText.Location = new Point(336, 0);
            detailsText.Margin = new Padding(0);
            detailsText.Multiline = true;
            detailsText.Name = "detailsText";
            detailsText.ReadOnly = true;
            detailsText.ScrollBars = ScrollBars.Vertical;
            detailsText.Size = new Size(550, 498);
            detailsText.TabIndex = 1;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(btnOpenCatalog);
            footerPanel.Controls.Add(btnCheckCatalog);
            footerPanel.Controls.Add(btnSaveCurrent);
            footerPanel.Controls.Add(btnLoad);
            footerPanel.Controls.Add(btnDelete);
            footerPanel.Controls.Add(btnRefresh);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.Location = new Point(15, 563);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(910, 42);
            footerPanel.TabIndex = 1;
            // 
            // btnOpenCatalog
            // 
            btnOpenCatalog.BackColor = Color.FromArgb(35, 85, 132);
            btnOpenCatalog.FlatStyle = FlatStyle.Flat;
            btnOpenCatalog.ForeColor = Color.White;
            btnOpenCatalog.Location = new Point(0, 8);
            btnOpenCatalog.Name = "btnOpenCatalog";
            btnOpenCatalog.Size = new Size(132, 28);
            btnOpenCatalog.TabIndex = 0;
            btnOpenCatalog.Text = "Open + Preview";
            btnOpenCatalog.UseVisualStyleBackColor = false;
            btnOpenCatalog.Click += OnOpenCatalogClicked;
            // 
            // btnCheckCatalog
            // 
            btnCheckCatalog.BackColor = Color.FromArgb(250, 252, 253);
            btnCheckCatalog.FlatStyle = FlatStyle.Flat;
            btnCheckCatalog.ForeColor = Color.FromArgb(35, 85, 132);
            btnCheckCatalog.Location = new Point(140, 8);
            btnCheckCatalog.Name = "btnCheckCatalog";
            btnCheckCatalog.Size = new Size(112, 28);
            btnCheckCatalog.TabIndex = 1;
            btnCheckCatalog.Text = "Check Sample";
            btnCheckCatalog.UseVisualStyleBackColor = false;
            btnCheckCatalog.Click += OnCheckCatalogClicked;
            // 
            // btnSaveCurrent
            // 
            btnSaveCurrent.BackColor = Color.FromArgb(250, 252, 253);
            btnSaveCurrent.FlatStyle = FlatStyle.Flat;
            btnSaveCurrent.ForeColor = Color.FromArgb(35, 85, 132);
            btnSaveCurrent.Location = new Point(252, 8);
            btnSaveCurrent.Name = "btnSaveCurrent";
            btnSaveCurrent.Size = new Size(108, 28);
            btnSaveCurrent.TabIndex = 2;
            btnSaveCurrent.Text = "Save Current";
            btnSaveCurrent.UseVisualStyleBackColor = false;
            btnSaveCurrent.Click += OnSaveCurrentClicked;
            // 
            // btnLoad
            // 
            btnLoad.BackColor = Color.FromArgb(250, 252, 253);
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.ForeColor = Color.FromArgb(35, 85, 132);
            btnLoad.Location = new Point(248, 8);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(80, 28);
            btnLoad.TabIndex = 3;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = false;
            btnLoad.Click += OnLoadClicked;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.FromArgb(250, 252, 253);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.ForeColor = Color.FromArgb(35, 85, 132);
            btnDelete.Location = new Point(336, 8);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(80, 28);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += OnDeleteClicked;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.BackColor = Color.FromArgb(250, 252, 253);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.ForeColor = Color.FromArgb(35, 85, 132);
            btnRefresh.Location = new Point(830, 8);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(80, 28);
            btnRefresh.TabIndex = 5;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += OnRefreshClicked;
            // 
            // FormVisionPipelineSamples
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            ClientSize = new Size(940, 620);
            Controls.Add(rootLayout);
            MinimumSize = new Size(820, 520);
            Name = "FormVisionPipelineSamples";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pipeline Samples";
            rootLayout.ResumeLayout(false);
            tabSamples.ResumeLayout(false);
            tabCatalog.ResumeLayout(false);
            tabSaved.ResumeLayout(false);
            catalogLayout.ResumeLayout(false);
            catalogDetailLayout.ResumeLayout(false);
            catalogDetailLayout.PerformLayout();
            catalogHeaderPanel.ResumeLayout(false);
            catalogPreviewLayout.ResumeLayout(false);
            catalogSourcePreviewLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)catalogPreviewBox).EndInit();
            catalogReferencePreviewLayout.ResumeLayout(false);
            catalogReferenceImagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)catalogReferenceBox).EndInit();
            savedLayout.ResumeLayout(false);
            savedLayout.PerformLayout();
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
