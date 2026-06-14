using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineSamples
    {
        private TableLayoutPanel rootLayout;
        private TableLayoutPanel catalogLayout;
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
            catalogDetailsText = new TextBox();
            savedLayout = new TableLayoutPanel();
            sampleList = new ListBox();
            detailsText = new TextBox();
            footerPanel = new Panel();
            btnOpenCatalog = new Button();
            btnSaveCurrent = new Button();
            btnLoad = new Button();
            btnDelete = new Button();
            btnRefresh = new Button();
            rootLayout.SuspendLayout();
            tabSamples.SuspendLayout();
            tabCatalog.SuspendLayout();
            tabSaved.SuspendLayout();
            catalogLayout.SuspendLayout();
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
            rootLayout.Size = new Size(760, 520);
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
            tabSamples.Size = new Size(730, 442);
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
            tabCatalog.Size = new Size(722, 414);
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
            tabSaved.Size = new Size(722, 414);
            tabSaved.TabIndex = 1;
            tabSaved.Text = "Saved Workspace";
            // 
            // catalogLayout
            // 
            catalogLayout.ColumnCount = 2;
            catalogLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            catalogLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            catalogLayout.Controls.Add(catalogList, 0, 0);
            catalogLayout.Controls.Add(catalogDetailsText, 1, 0);
            catalogLayout.Dock = DockStyle.Fill;
            catalogLayout.Location = new Point(8, 8);
            catalogLayout.Name = "catalogLayout";
            catalogLayout.RowCount = 1;
            catalogLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            catalogLayout.Size = new Size(706, 398);
            catalogLayout.TabIndex = 0;
            // 
            // catalogList
            // 
            catalogList.Dock = DockStyle.Fill;
            catalogList.FormattingEnabled = true;
            catalogList.IntegralHeight = false;
            catalogList.ItemHeight = 15;
            catalogList.Location = new Point(0, 0);
            catalogList.Margin = new Padding(0, 0, 10, 0);
            catalogList.Name = "catalogList";
            catalogList.Size = new Size(286, 398);
            catalogList.TabIndex = 0;
            catalogList.SelectedIndexChanged += OnCatalogSampleSelected;
            // 
            // catalogDetailsText
            // 
            catalogDetailsText.BackColor = Color.FromArgb(250, 252, 253);
            catalogDetailsText.BorderStyle = BorderStyle.FixedSingle;
            catalogDetailsText.Dock = DockStyle.Fill;
            catalogDetailsText.Location = new Point(296, 0);
            catalogDetailsText.Margin = new Padding(0);
            catalogDetailsText.Multiline = true;
            catalogDetailsText.Name = "catalogDetailsText";
            catalogDetailsText.ReadOnly = true;
            catalogDetailsText.ScrollBars = ScrollBars.Vertical;
            catalogDetailsText.Size = new Size(410, 398);
            catalogDetailsText.TabIndex = 1;
            // 
            // savedLayout
            // 
            savedLayout.ColumnCount = 2;
            savedLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            savedLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            savedLayout.Controls.Add(sampleList, 0, 0);
            savedLayout.Controls.Add(detailsText, 1, 0);
            savedLayout.Dock = DockStyle.Fill;
            savedLayout.Location = new Point(8, 8);
            savedLayout.Name = "savedLayout";
            savedLayout.RowCount = 1;
            savedLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            savedLayout.Size = new Size(706, 398);
            savedLayout.TabIndex = 0;
            // 
            // sampleList
            // 
            sampleList.Dock = DockStyle.Fill;
            sampleList.FormattingEnabled = true;
            sampleList.IntegralHeight = false;
            sampleList.ItemHeight = 15;
            sampleList.Location = new Point(0, 0);
            sampleList.Margin = new Padding(0, 0, 10, 0);
            sampleList.Name = "sampleList";
            sampleList.Size = new Size(286, 398);
            sampleList.TabIndex = 0;
            sampleList.SelectedIndexChanged += OnSampleSelected;
            // 
            // detailsText
            // 
            detailsText.BackColor = Color.FromArgb(250, 252, 253);
            detailsText.BorderStyle = BorderStyle.FixedSingle;
            detailsText.Dock = DockStyle.Fill;
            detailsText.Location = new Point(296, 0);
            detailsText.Margin = new Padding(0);
            detailsText.Multiline = true;
            detailsText.Name = "detailsText";
            detailsText.ReadOnly = true;
            detailsText.ScrollBars = ScrollBars.Vertical;
            detailsText.Size = new Size(410, 398);
            detailsText.TabIndex = 1;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(btnOpenCatalog);
            footerPanel.Controls.Add(btnSaveCurrent);
            footerPanel.Controls.Add(btnLoad);
            footerPanel.Controls.Add(btnDelete);
            footerPanel.Controls.Add(btnRefresh);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.Location = new Point(15, 463);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(730, 42);
            footerPanel.TabIndex = 1;
            // 
            // btnOpenCatalog
            // 
            btnOpenCatalog.BackColor = Color.FromArgb(35, 85, 132);
            btnOpenCatalog.FlatStyle = FlatStyle.Flat;
            btnOpenCatalog.ForeColor = Color.White;
            btnOpenCatalog.Location = new Point(0, 8);
            btnOpenCatalog.Name = "btnOpenCatalog";
            btnOpenCatalog.Size = new Size(124, 28);
            btnOpenCatalog.TabIndex = 0;
            btnOpenCatalog.Text = "Open Sample";
            btnOpenCatalog.UseVisualStyleBackColor = false;
            btnOpenCatalog.Click += OnOpenCatalogClicked;
            // 
            // btnSaveCurrent
            // 
            btnSaveCurrent.BackColor = Color.FromArgb(250, 252, 253);
            btnSaveCurrent.FlatStyle = FlatStyle.Flat;
            btnSaveCurrent.ForeColor = Color.FromArgb(35, 85, 132);
            btnSaveCurrent.Location = new Point(132, 8);
            btnSaveCurrent.Name = "btnSaveCurrent";
            btnSaveCurrent.Size = new Size(108, 28);
            btnSaveCurrent.TabIndex = 1;
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
            btnLoad.TabIndex = 2;
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
            btnDelete.TabIndex = 3;
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
            btnRefresh.Location = new Point(650, 8);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(80, 28);
            btnRefresh.TabIndex = 4;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += OnRefreshClicked;
            // 
            // FormVisionPipelineSamples
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 242, 246);
            ClientSize = new Size(760, 520);
            Controls.Add(rootLayout);
            MinimumSize = new Size(660, 430);
            Name = "FormVisionPipelineSamples";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pipeline Samples";
            rootLayout.ResumeLayout(false);
            tabSamples.ResumeLayout(false);
            tabCatalog.ResumeLayout(false);
            tabSaved.ResumeLayout(false);
            catalogLayout.ResumeLayout(false);
            catalogLayout.PerformLayout();
            savedLayout.ResumeLayout(false);
            savedLayout.PerformLayout();
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
