using System;
using System.Windows;
using System.Windows.Controls;
using Lib.OpenCV;
using Lib.OpenCV.Tool;
using OpenCvSharp;

namespace OpenVisionLab
{
    internal sealed class VisionToolFilterInteractionController
    {
        private readonly FilterToolPresenter presenter;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly ComboBox filterTypeComboBox;
        private readonly ComboBox borderTypeComboBox;
        private readonly FrameworkElement panelWidth;
        private readonly FrameworkElement panelHeight;
        private readonly FrameworkElement panelKernelPresets;
        private readonly CheckBox lockSizeCheckBox;
        private readonly FrameworkElement panelMedian;
        private readonly FrameworkElement panelDiameter;
        private readonly FrameworkElement panelSigmaColor;
        private readonly FrameworkElement panelSigmaSpace;
        private readonly TextBox[] parameterTextBoxes;

        public VisionToolFilterInteractionController(
            FilterToolPresenter presenter,
            VisionToolParameterChangeController parameterChangeController,
            ComboBox filterTypeComboBox,
            ComboBox borderTypeComboBox,
            FrameworkElement panelWidth,
            FrameworkElement panelHeight,
            FrameworkElement panelKernelPresets,
            CheckBox lockSizeCheckBox,
            FrameworkElement panelMedian,
            FrameworkElement panelDiameter,
            FrameworkElement panelSigmaColor,
            FrameworkElement panelSigmaSpace,
            params TextBox[] parameterTextBoxes)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.parameterChangeController = parameterChangeController ?? throw new ArgumentNullException(nameof(parameterChangeController));
            this.filterTypeComboBox = filterTypeComboBox ?? throw new ArgumentNullException(nameof(filterTypeComboBox));
            this.borderTypeComboBox = borderTypeComboBox ?? throw new ArgumentNullException(nameof(borderTypeComboBox));
            this.panelWidth = panelWidth ?? throw new ArgumentNullException(nameof(panelWidth));
            this.panelHeight = panelHeight ?? throw new ArgumentNullException(nameof(panelHeight));
            this.panelKernelPresets = panelKernelPresets ?? throw new ArgumentNullException(nameof(panelKernelPresets));
            this.lockSizeCheckBox = lockSizeCheckBox ?? throw new ArgumentNullException(nameof(lockSizeCheckBox));
            this.panelMedian = panelMedian ?? throw new ArgumentNullException(nameof(panelMedian));
            this.panelDiameter = panelDiameter ?? throw new ArgumentNullException(nameof(panelDiameter));
            this.panelSigmaColor = panelSigmaColor ?? throw new ArgumentNullException(nameof(panelSigmaColor));
            this.panelSigmaSpace = panelSigmaSpace ?? throw new ArgumentNullException(nameof(panelSigmaSpace));
            this.parameterTextBoxes = parameterTextBoxes ?? Array.Empty<TextBox>();
        }

        public void InitializeOptions()
        {
            filterTypeComboBox.ItemsSource = Enum.GetValues(typeof(FilterToolType));
            borderTypeComboBox.ItemsSource = new[]
            {
                BorderTypes.Reflect101,
                BorderTypes.Replicate,
                BorderTypes.Reflect,
                BorderTypes.Wrap,
                BorderTypes.Constant,
                BorderTypes.Transparent,
                BorderTypes.Isolated
            };
        }

        public void HandleFilterTypeChanged()
        {
            parameterChangeController.TryHandle(RefreshModePanels, schedulePreview: true);
        }

        public void HandleParameterSelectionChanged()
        {
            parameterChangeController.TryHandle(FlushParameterBindings, schedulePreview: true);
        }

        public void RefreshModePanels()
        {
            // Filter mode controls which parameter group is meaningful; keep that UI policy outside the View event handlers.
            FlushParameterBindings();
            bool sizeMode = presenter.UsesKernelSize;
            bool medianMode = presenter.UsesMedianKernel;
            bool bilateralMode = presenter.UsesBilateral;

            VisionToolControlBinding.SetPanelVisible(panelWidth, sizeMode, fadeWhenHidden: true);
            VisionToolControlBinding.SetPanelVisible(panelHeight, sizeMode, fadeWhenHidden: true);
            VisionToolControlBinding.SetPanelVisible(panelKernelPresets, sizeMode || medianMode, fadeWhenHidden: true);
            lockSizeCheckBox.Visibility = sizeMode ? Visibility.Visible : Visibility.Collapsed;
            lockSizeCheckBox.IsEnabled = sizeMode;
            VisionToolControlBinding.SetPanelVisible(panelMedian, medianMode, fadeWhenHidden: true);
            VisionToolControlBinding.SetPanelVisible(panelDiameter, bilateralMode, fadeWhenHidden: true);
            VisionToolControlBinding.SetPanelVisible(panelSigmaColor, bilateralMode, fadeWhenHidden: true);
            VisionToolControlBinding.SetPanelVisible(panelSigmaSpace, bilateralMode, fadeWhenHidden: true);
        }

        public void FlushParameterBindings()
        {
            VisionToolControlBinding.UpdateSelectionSources(filterTypeComboBox, borderTypeComboBox);
            VisionToolControlBinding.UpdateTextSources(parameterTextBoxes);
        }
    }
}
