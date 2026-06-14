using OpenVisionLab._1._Core;
using OpenVisionLab.MessageDialogs;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineSamples : Form
    {
        private readonly string recipeName;
        private readonly string pipelineName;
        private readonly IDisplayManager displayManager;
        private List<VisionPipelineSampleCatalogItem> catalogSamples = new List<VisionPipelineSampleCatalogItem>();

        private TabControl tabSamples;
        private TabPage tabCatalog;
        private TabPage tabSaved;
        private ListBox catalogList;
        private TextBox catalogDetailsText;
        private ListBox sampleList;
        private TextBox detailsText;
        private Button btnOpenCatalog;
        private Button btnSaveCurrent;
        private Button btnLoad;
        private Button btnDelete;
        private Button btnRefresh;

        public VisionPipelineSampleCatalogItem SelectedCatalogSample { get; private set; }

        public FormVisionPipelineSamples()
            : this("Default", "Pipeline", DisplayManagerService.Default)
        {
        }

        public FormVisionPipelineSamples(string recipeName, string pipelineName, IDisplayManager displayManager)
        {
            this.recipeName = string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName;
            this.pipelineName = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName;
            this.displayManager = displayManager ?? DisplayManagerService.Default;

            InitializeComponent();
            VisionPipelineDialogStyle.Apply(this);
            VisionPipelineDialogStyle.StyleButton(btnOpenCatalog, primary: true);
            VisionPipelineDialogStyle.StyleButton(btnSaveCurrent, primary: true);
            if (!VisionPipelineDesignTime.IsActive)
            {
                RefreshCatalogSamples();
                RefreshSamples();
            }
        }

        private void RefreshCatalogSamples()
        {
            catalogSamples = VisionPipelineSampleCatalogItem.LoadRunnable();
            catalogList.DataSource = null;
            catalogList.DataSource = catalogSamples;
            if (catalogSamples.Count > 0)
            {
                catalogList.SelectedIndex = 0;
            }
            else
            {
                BindCatalogSample(null);
            }

            UpdateButtonStates();
        }

        private void RefreshSamples()
        {
            List<VisionPipelineSampleSetInfo> samples = VisionPipelineSampleSetStorage.List(recipeName, pipelineName);
            sampleList.DataSource = samples;
            if (samples.Count > 0)
            {
                sampleList.SelectedIndex = 0;
            }
            else
            {
                BindSample(null);
            }

            UpdateButtonStates();
        }

        private void OnTabChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void OnCatalogSampleSelected(object sender, EventArgs e)
        {
            BindCatalogSample(catalogList.SelectedItem as VisionPipelineSampleCatalogItem);
        }

        private void OnSampleSelected(object sender, EventArgs e)
        {
            BindSample(sampleList.SelectedItem as VisionPipelineSampleSetInfo);
        }

        private void BindCatalogSample(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                catalogDetailsText.Text = "No catalog sample.";
                UpdateButtonStates();
                return;
            }

            string imageState = sample.CanOpen ? "Ready" : "Missing file";
            catalogDetailsText.Text =
                $"Sample: {sample.SampleName}{Environment.NewLine}" +
                $"Category: {sample.Category}{Environment.NewLine}" +
                $"Goal: {sample.Goal}{Environment.NewLine}" +
                $"Image: {sample.ImagePath} ({sample.Width} x {sample.Height}){Environment.NewLine}" +
                $"Pipeline: {sample.BaselinePipeline}{Environment.NewLine}" +
                $"Expected: {sample.ExpectedText}{Environment.NewLine}" +
                $"State: {imageState}{Environment.NewLine}" +
                $"{Environment.NewLine}{sample.Notes}";

            UpdateButtonStates();
        }

        private void BindSample(VisionPipelineSampleSetInfo sample)
        {
            if (sample == null)
            {
                detailsText.Text = "No sample set.";
                UpdateButtonStates();
                return;
            }

            detailsText.Text =
                $"Name: {sample.Name}{Environment.NewLine}" +
                $"Layers: {sample.LayerCount}{Environment.NewLine}" +
                $"Saved: {sample.SavedAt:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}" +
                $"Path: {sample.DirectoryPath}";

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool catalogActive = tabSamples?.SelectedTab == tabCatalog;
            bool savedActive = tabSamples?.SelectedTab == tabSaved;
            VisionPipelineSampleCatalogItem catalog = catalogList?.SelectedItem as VisionPipelineSampleCatalogItem;
            VisionPipelineSampleSetInfo sample = sampleList?.SelectedItem as VisionPipelineSampleSetInfo;

            if (btnOpenCatalog != null)
            {
                btnOpenCatalog.Visible = catalogActive;
                btnOpenCatalog.Left = 0;
                btnOpenCatalog.Enabled = catalogActive && catalog?.CanOpen == true;
            }

            if (btnSaveCurrent != null)
            {
                btnSaveCurrent.Visible = savedActive;
                btnSaveCurrent.Left = 0;
                btnSaveCurrent.Enabled = savedActive;
            }

            if (btnLoad != null)
            {
                btnLoad.Visible = savedActive;
                btnLoad.Left = 116;
                btnLoad.Enabled = savedActive && sample != null;
            }

            if (btnDelete != null)
            {
                btnDelete.Visible = savedActive;
                btnDelete.Left = 204;
                btnDelete.Enabled = savedActive && sample != null;
            }
        }

        private void OnOpenCatalogClicked(object sender, EventArgs e)
        {
            if (!(catalogList.SelectedItem is VisionPipelineSampleCatalogItem sample) || !sample.CanOpen)
            {
                return;
            }

            SelectedCatalogSample = sample;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnSaveCurrentClicked(object sender, EventArgs e)
        {
            string defaultName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string sampleName = FormVisionPipelineTextPrompt.Show(this, "Save Sample", "Sample Name", defaultName);
            if (string.IsNullOrWhiteSpace(sampleName))
            {
                return;
            }

            try
            {
                int count = VisionPipelineSampleSetStorage.Save(recipeName, pipelineName, sampleName.Trim(), displayManager);
                RefreshSamples();
                SelectSample(sampleName.Trim());
                VisionMessageBox.Info(this, "Pipeline Samples", $"Sample saved. Layers: {count}");
            }
            catch (Exception ex)
            {
                VisionMessageBox.Error(this, "Pipeline Samples", ex.GetBaseException().Message, ex.ToString());
            }
        }

        private void OnLoadClicked(object sender, EventArgs e)
        {
            if (!(sampleList.SelectedItem is VisionPipelineSampleSetInfo sample))
            {
                return;
            }

            try
            {
                int count = VisionPipelineSampleSetStorage.Load(recipeName, pipelineName, sample.Name, displayManager);
                VisionMessageBox.Info(this, "Pipeline Samples", $"Sample loaded. Layers: {count}");
            }
            catch (Exception ex)
            {
                VisionMessageBox.Error(this, "Pipeline Samples", ex.GetBaseException().Message, ex.ToString());
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (!(sampleList.SelectedItem is VisionPipelineSampleSetInfo sample))
            {
                return;
            }

            DialogResult answer = VisionMessageBox.Confirm(
                this,
                "Pipeline Samples",
                $"Delete sample '{sample.Name}'?");
            if (answer != DialogResult.Yes)
            {
                return;
            }

            try
            {
                VisionPipelineSampleSetStorage.Delete(sample);
                RefreshSamples();
            }
            catch (Exception ex)
            {
                VisionMessageBox.Error(this, "Pipeline Samples", ex.GetBaseException().Message, ex.ToString());
            }
        }

        private void OnRefreshClicked(object sender, EventArgs e)
        {
            RefreshCatalogSamples();
            RefreshSamples();
        }

        private void SelectSample(string sampleName)
        {
            for (int i = 0; i < sampleList.Items.Count; i++)
            {
                if (sampleList.Items[i] is VisionPipelineSampleSetInfo sample
                    && string.Equals(sample.Name, sampleName, StringComparison.OrdinalIgnoreCase))
                {
                    sampleList.SelectedIndex = i;
                    return;
                }
            }
        }

    }
}
