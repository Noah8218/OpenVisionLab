using OpenVisionLab._1._Core;
using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.MessageDialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public partial class FormThreshold : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private readonly IDisplayManager displayManager;
        private readonly Timer previewTimer = new Timer();
        private readonly Timer layerRefreshTimer = new Timer();
        private readonly ToolTip thresholdToolTip = new ToolTip();
        private Action pendingPreviewAction;
        private ThresholdToolMode currentMode = ThresholdToolMode.Threshold;
        private bool thresholdControlEventsHooked;
        private bool refreshingLayerItems;
        private string inputLayerSnapshot = string.Empty;

        public FormThreshold()
            : this(ApplicationRuntimeContext.CreateDefault().DisplayManager)
        {
        }

        public FormThreshold(IDisplayManager displayManager)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
            previewTimer.Interval = 60;
            previewTimer.Tick += PreviewTimer_Tick;
            layerRefreshTimer.Interval = 300;
            layerRefreshTimer.Tick += LayerRefreshTimer_Tick;
        }

        private bool ChangeSize = false;

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize && DockHandler.FloatPane != null)
            {
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(
                    DockHandler.FloatPane.FloatWindow.Bounds.X,
                    DockHandler.FloatPane.FloatWindow.Bounds.Y,
                    800,
                    650);
                Refresh();
                ChangeSize = true;
            }

            if (Visible)
            {
                RefreshInputLayerItems();
                layerRefreshTimer.Start();
            }
            else
            {
                layerRefreshTimer.Stop();
            }
        }

        public void InitThresholdMenu()
        {
            cbThresholdMenu.Items.Add(ThresholdTypes.Binary);
            cbThresholdMenu.Items.Add(ThresholdTypes.BinaryInv);
            cbThresholdMenu.Items.Add(ThresholdTypes.Mask);
            cbThresholdMenu.Items.Add(ThresholdTypes.Otsu);
            cbThresholdMenu.Items.Add(ThresholdTypes.Tozero);
            cbThresholdMenu.Items.Add(ThresholdTypes.TozeroInv);
            cbThresholdMenu.Items.Add(ThresholdTypes.Triangle);
            cbThresholdMenu.Items.Add(ThresholdTypes.Trunc);
            cbThresholdMenu.SelectedIndex = 0;

            cbAdaptiveThresholdMenu.Items.Add(ThresholdTypes.Binary);
            cbAdaptiveThresholdMenu.Items.Add(ThresholdTypes.BinaryInv);
            cbAdaptiveThresholdMenu.SelectedIndex = 0;

            cbAdaptiveType.Items.Add(AdaptiveThresholdTypes.MeanC);
            cbAdaptiveType.Items.Add(AdaptiveThresholdTypes.GaussianC);
            cbAdaptiveType.SelectedIndex = 0;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            InitThresholdMenu();
            RefreshInputLayerItems();
            InitializeDefaultParameterText();
            ApplyThresholdRuntimeBehavior();
            if (Visible)
            {
                layerRefreshTimer.Start();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            previewTimer.Stop();
            previewTimer.Tick -= PreviewTimer_Tick;
            previewTimer.Dispose();
            layerRefreshTimer.Stop();
            layerRefreshTimer.Tick -= LayerRefreshTimer_Tick;
            layerRefreshTimer.Dispose();
            thresholdToolTip.Dispose();
            base.OnFormClosed(e);
        }

        protected override string GetPersistString()
        {
            return typeof(FormThreshold).FullName;
        }

        private void InitializeDefaultParameterText()
        {
            if (string.IsNullOrWhiteSpace(tbBlockSize.Text))
            {
                tbBlockSize.Text = "25";
            }

            if (string.IsNullOrWhiteSpace(tbWeight.Text))
            {
                tbWeight.Text = "5";
            }
        }

        private void ApplyThresholdRuntimeBehavior()
        {
            panel1.AutoScrollMargin = new System.Drawing.Size(0, 12);

            SyncComboDisplayStyle(cbInputLayer);
            SyncComboDisplayStyle(cbThresholdMenu);
            SyncComboDisplayStyle(cbAdaptiveThresholdMenu);
            SyncComboDisplayStyle(cbAdaptiveType);

            thresholdToolTip.SetToolTip(cbInputLayer, "Input layer used for Threshold preview.");
            thresholdToolTip.SetToolTip(lblOutputLayerValue, "Preview result is written to the Threshold layer.");
            thresholdToolTip.SetToolTip(cbThresholdMenu, "Result type for Basic Threshold.");
            thresholdToolTip.SetToolTip(thresholdValueBar, "Threshold value for Basic Threshold.");
            thresholdToolTip.SetToolTip(rangeThresholdBar, "Min, max, and invert option for Range Threshold.");
            thresholdToolTip.SetToolTip(cbAdaptiveThresholdMenu, "Result type for Adaptive Threshold.");
            thresholdToolTip.SetToolTip(cbAdaptiveType, "Adaptive calculation method.");
            thresholdToolTip.SetToolTip(adaptiveValueBar, "Maximum output value for Adaptive Threshold.");
            thresholdToolTip.SetToolTip(tbBlockSize, "Adaptive block size. Use an odd value.");
            thresholdToolTip.SetToolTip(tbWeight, "Adaptive correction value subtracted from the local mean or Gaussian value.");
            thresholdToolTip.SetToolTip(pnlBasicHeader, "Basic uses one threshold value. Good when object and background brightness are clearly separated.");
            thresholdToolTip.SetToolTip(lblBasicHint, "Basic uses one threshold value. Good when object and background brightness are clearly separated.");
            thresholdToolTip.SetToolTip(pnlRangeHeader, "Range keeps pixels between Min and Max. Good when the target has a known brightness band.");
            thresholdToolTip.SetToolTip(lblRangeHint, "Range keeps pixels between Min and Max. Good when the target has a known brightness band.");
            thresholdToolTip.SetToolTip(pnlAdaptiveHeader, "Adaptive calculates a local threshold. Good for uneven lighting or gradients.");
            thresholdToolTip.SetToolTip(lblAdaptiveHint, "Adaptive calculates a local threshold. Good for uneven lighting or gradients.");

            HookThresholdControlEvents();
            UpdateModeHeaderState(currentMode);
            UpdateLayerFlowSummary();
        }

        private static void SyncComboDisplayStyle(RJCodeUI_M1.RJControls.RJComboBox combo)
        {
            foreach (Control child in combo.Controls)
            {
                if (child is Label label)
                {
                    label.BackColor = combo.BackColor;
                    label.ForeColor = combo.ForeColor;
                    label.Font = combo.Font;
                    if (combo.SelectedItem != null)
                    {
                        label.Text = combo.SelectedItem.ToString();
                    }
                    label.BringToFront();
                }
            }
        }

        private void HookThresholdControlEvents()
        {
            if (thresholdControlEventsHooked)
            {
                return;
            }

            thresholdValueBar.ValueChanged += (sender, e) => ApplyModePreview(ThresholdToolMode.Threshold);
            rangeThresholdBar.RangeChanged += (sender, e) => ApplyModePreview(ThresholdToolMode.Range);
            adaptiveValueBar.ValueChanged += (sender, e) => ApplyModePreview(ThresholdToolMode.Adaptive);
            cbInputLayer.OnSelectedIndexChanged += CbInputLayer_SelectedIndexChanged;
            cbInputLayer.DropDownOpening += CbInputLayer_DropDownOpening;
            cbThresholdMenu.OnSelectedIndexChanged += (sender, e) => ApplyModePreview(ThresholdToolMode.Threshold);
            cbAdaptiveThresholdMenu.OnSelectedIndexChanged += (sender, e) => ApplyModePreview(ThresholdToolMode.Adaptive);
            cbAdaptiveType.OnSelectedIndexChanged += (sender, e) => ApplyModePreview(ThresholdToolMode.Adaptive);
            tbBlockSize.onTextChanged += (sender, e) => ApplyModePreview(ThresholdToolMode.Adaptive);
            tbWeight.onTextChanged += (sender, e) => ApplyModePreview(ThresholdToolMode.Adaptive);
            btnAddToPipeline.Click += BtnAddToPipeline_Click;

            thresholdControlEventsHooked = true;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            RefreshInputLayerItems();
        }

        private void CbInputLayer_DropDownOpening(object sender, EventArgs e)
        {
            RefreshInputLayerItems();
        }

        private void CbInputLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (refreshingLayerItems)
            {
                return;
            }

            UpdateLayerFlowSummary();
            ApplyModePreview(currentMode);
        }

        private void RefreshInputLayerItems()
        {
            if (cbInputLayer == null)
            {
                return;
            }

            string previousLayer = cbInputLayer.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(previousLayer))
            {
                previousLayer = string.IsNullOrWhiteSpace(displayManager.SelectedItem)
                    ? "Main"
                    : displayManager.SelectedItem;
            }

            refreshingLayerItems = true;
            try
            {
                cbInputLayer.Items.Clear();
                for (int i = 0; i < displayManager.LayerCount; i++)
                {
                    string title = displayManager.GetLayerTitle(i);
                    if (!string.IsNullOrWhiteSpace(title) && !cbInputLayer.Items.Contains(title))
                    {
                        cbInputLayer.Items.Add(title);
                    }
                }

                if (cbInputLayer.Items.Count == 0 || !cbInputLayer.Items.Contains("Main"))
                {
                    cbInputLayer.Items.Insert(0, "Main");
                }

                int selectedIndex = 0;
                if (!string.IsNullOrWhiteSpace(previousLayer) && cbInputLayer.Items.Contains(previousLayer))
                {
                    selectedIndex = cbInputLayer.Items.IndexOf(previousLayer);
                }
                else if (cbInputLayer.Items.Contains("Main"))
                {
                    selectedIndex = cbInputLayer.Items.IndexOf("Main");
                }

                cbInputLayer.SelectedIndex = selectedIndex;
                SyncComboDisplayStyle(cbInputLayer);
            }
            finally
            {
                refreshingLayerItems = false;
            }

            UpdateLayerFlowSummary();
            inputLayerSnapshot = BuildInputLayerSnapshot();
        }

        private void LayerRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (!Visible || cbInputLayer == null || refreshingLayerItems)
            {
                return;
            }

            string currentSnapshot = BuildInputLayerSnapshot();
            if (!string.Equals(inputLayerSnapshot, currentSnapshot, StringComparison.Ordinal))
            {
                RefreshInputLayerItems();
            }
        }

        private string BuildInputLayerSnapshot()
        {
            List<string> layerNames = new List<string>();
            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                string title = displayManager.GetLayerTitle(i);
                if (!string.IsNullOrWhiteSpace(title) && !layerNames.Contains(title.Trim()))
                {
                    layerNames.Add(title.Trim());
                }
            }

            if (!layerNames.Contains("Main"))
            {
                layerNames.Insert(0, "Main");
            }

            return string.Join("|", layerNames);
        }

        private string ResolveInputLayerName()
        {
            string selectedLayer = cbInputLayer.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(selectedLayer))
            {
                return selectedLayer.Trim();
            }

            return string.IsNullOrWhiteSpace(displayManager.SelectedItem)
                ? "Main"
                : displayManager.SelectedItem.Trim();
        }

        private void UpdateLayerFlowSummary()
        {
            if (lblOutputLayerValue != null)
            {
                lblOutputLayerValue.Text = DEFINE.Threshold;
            }

            if (lblPreviewDescription != null)
            {
                lblPreviewDescription.Text = $"Preview writes {ResolveInputLayerName()} -> {DEFINE.Threshold}.";
            }
        }

        private void ApplyModePreview(ThresholdToolMode mode)
        {
            currentMode = mode;
            UpdateModeHeaderState(mode);
            ScheduleThresholdPreview(mode);
        }

        private void UpdateModeHeaderState(ThresholdToolMode mode)
        {
            SetModeHeaderState(pnlBasicHeader, lblBasicNo, lblBasicHint, mode == ThresholdToolMode.Threshold);
            SetModeHeaderState(pnlRangeHeader, lblRangeNo, lblRangeHint, mode == ThresholdToolMode.Range);
            SetModeHeaderState(pnlAdaptiveHeader, lblAdaptiveNo, lblAdaptiveHint, mode == ThresholdToolMode.Adaptive);
        }

        private static void SetModeHeaderState(Panel header, Label numberLabel, Label hintLabel, bool active)
        {
            if (header == null)
            {
                return;
            }

            Color activeBack = Color.FromArgb(39, 111, 156);
            Color idleBack = Color.FromArgb(31, 45, 66);
            Color activeAccent = Color.FromArgb(205, 239, 255);
            Color idleAccent = Color.FromArgb(120, 186, 222);
            Color idleHint = Color.FromArgb(163, 190, 218);

            header.BackColor = active ? activeBack : idleBack;
            if (numberLabel != null)
            {
                numberLabel.ForeColor = active ? activeAccent : idleAccent;
            }

            if (hintLabel != null)
            {
                hintLabel.ForeColor = active ? activeAccent : idleHint;
            }
        }

        private void ScheduleThresholdPreview(ThresholdToolMode mode)
        {
            SchedulePreview(() =>
            {
                ThresholdToolProperty property = CreateThresholdProperty(mode);
                Bitmap result = null;

                if (mode == ThresholdToolMode.Threshold)
                {
                    result = CreateLayerOperationResult(image =>
                    {
                        return RunThresholdTool(image, property);
                    }, convertFullImageToGray: true);
                }
                else if (mode == ThresholdToolMode.Range)
                {
                    result = CreateLayerOperationResult(image =>
                    {
                        return RunThresholdTool(image, property);
                    });
                }
                else if (mode == ThresholdToolMode.Adaptive)
                {
                    result = CreateLayerOperationResult(image =>
                    {
                        return RunThresholdTool(image, property);
                    }, image => OpenCvHelper.SetImageChannel1(image));
                }

                PublishThresholdResult(result);
            });
        }

        private void SchedulePreview(Action previewAction)
        {
            pendingPreviewAction = previewAction;
            previewTimer.Stop();
            previewTimer.Start();
        }

        private void PreviewTimer_Tick(object sender, EventArgs e)
        {
            previewTimer.Stop();
            Action action = pendingPreviewAction;
            pendingPreviewAction = null;
            action?.Invoke();
        }

        private void PublishThresholdResult(Bitmap result)
        {
            if (result == null) { return; }

            displayManager.CreateLayerDisplay(result, DEFINE.Threshold, false);
        }

        private Bitmap CreateLayerOperationResult(Func<Mat, Mat> processImage, Action<Mat> prepareSource = null, bool convertFullImageToGray = false)
        {
            string inputLayer = ResolveInputLayerName();
            using (Mat imageSrc = CreateInputLayerMat(inputLayer))
            {
                if (OpenCvHelper.IsImageEmpty(imageSrc)) { return null; }

                prepareSource?.Invoke(imageSrc);

                if (displayManager.IsLayerRoiEmpty(inputLayer))
                {
                    if (convertFullImageToGray && imageSrc.Channels() != 1)
                    {
                        Cv2.CvtColor(imageSrc, imageSrc, ColorConversionCodes.RGB2GRAY);
                    }

                    using (Mat resultImage = processImage(imageSrc))
                    {
                        return BitmapImageConverter.ToBitmap(resultImage);
                    }
                }

                Rect roi = CommonConverter.RectangleToRect(displayManager.GetLayerRoi(inputLayer));
                using (Mat imageRoi = imageSrc.SubMat(roi))
                using (Mat roiResult = processImage(imageRoi))
                using (Bitmap sourceBitmap = BitmapImageConverter.ToBitmap(imageSrc))
                using (Bitmap roiBitmap = BitmapImageConverter.ToBitmap(roiResult))
                {
                    return BitmapProcessing.OverlayImage(sourceBitmap, roiBitmap, roi.Left, roi.Top);
                }
            }
        }

        private Mat CreateInputLayerMat(string inputLayer)
        {
            Bitmap layerImage = displayManager.GetLayerImage(inputLayer);
            if (layerImage != null)
            {
                return BitmapImageConverter.ToMat(layerImage).Clone();
            }

            return displayManager.GetImageSrc()?.Clone();
        }

        private static Mat RunThresholdTool(Mat image, ThresholdToolProperty property)
        {
            ThresholdTool tool = new ThresholdTool();
            tool.SetProperty(property);
            tool.SetSourceImage(image);
            tool.Run();
            return tool.imageResult;
        }

        private void BtnAddToPipeline_Click(object sender, EventArgs e)
        {
            try
            {
                string inputLayer = ResolveInputLayerName();
                VisionPipelineStep step = VisionPipelineAppendService.AddStep(
                    VisionPipelineStepBuilder.FromThresholdProperty(
                        CreateThresholdProperty(currentMode),
                        "Threshold",
                        inputLayer,
                        DEFINE.Threshold));
                VisionMessageBox.Info(
                    this,
                    "Pipeline",
                    $"Added to Pipeline.\r\nStep: {step.Name}\r\nFlow: {inputLayer} -> {DEFINE.Threshold}");
            }
            catch (Exception ex)
            {
                VisionMessageBox.Error(this, "Pipeline", ex.GetBaseException().Message, ex.ToString());
            }
        }

        private ThresholdToolProperty CreateThresholdProperty(ThresholdToolMode mode)
        {
            return new ThresholdToolProperty
            {
                Mode = mode,
                Threshold = thresholdValueBar.Value,
                MaxValue = mode == ThresholdToolMode.Adaptive ? adaptiveValueBar.Value : 255,
                ThresholdType = AppUtil.ParseEnum<ThresholdTypes>(cbThresholdMenu.SelectedItem.ToString()),
                RangeMin = rangeThresholdBar.RangeMin,
                RangeMax = rangeThresholdBar.RangeMax,
                Invert = mode == ThresholdToolMode.Range && rangeThresholdBar.Invert,
                AdaptiveType = AppUtil.ParseEnum<AdaptiveThresholdTypes>(cbAdaptiveType.SelectedItem.ToString()),
                AdaptiveThresholdType = AppUtil.ParseEnum<ThresholdTypes>(cbAdaptiveThresholdMenu.SelectedItem.ToString()),
                BlockSize = NormalizeAdaptiveBlockSize(tbBlockSize.Text, 25),
                Weight = ParseIntOrDefault(tbWeight.Text, 5)
            };
        }

        private static int ParseIntOrDefault(string value, int defaultValue)
        {
            return int.TryParse(value, out int parsed)
                ? parsed
                : defaultValue;
        }

        private static int NormalizeAdaptiveBlockSize(string value, int defaultValue)
        {
            int parsed = ParseIntOrDefault(value, defaultValue);
            parsed = Math.Max(3, Math.Min(255, parsed));
            if (parsed % 2 == 0)
            {
                parsed = parsed == 255 ? parsed - 1 : parsed + 1;
            }

            return parsed;
        }
    }
}
