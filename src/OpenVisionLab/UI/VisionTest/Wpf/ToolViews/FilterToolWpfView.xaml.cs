using Lib.OpenCV.Property;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using OpenVisionLab.Contracts;

namespace OpenVisionLab
{
    public partial class FilterToolWpfView : VisionToolSingleInputCustomToolViewBase, ISingleInputPropertyVisionToolWpfView<FilterToolProperty>
    {
        private readonly FilterToolPresenter presenter;

        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolKernelSizeController kernelSizeController;
        private readonly VisionToolFilterInteractionController filterInteractionController;
        private readonly FilterToolTextPresenter textPresenter;
        private readonly VisionToolCustomParameterGuideBinder parameterGuideBinder;
        private bool suppressEvents = true;

        internal FilterToolWpfView(FilterToolPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            InitializeComponent();
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, RequestRunPreview);
            parameterChangeController = new VisionToolParameterChangeController(() => suppressEvents, UpdateSummary, schedulePreview: previewScheduler.Schedule);
            kernelSizeController = new VisionToolKernelSizeController(
                parameterChangeController,
                txtWidth,
                chkLockSize,
                presenter.SetKernelPreset,
                presenter.SyncKernelHeightToWidth,
                value => suppressEvents = value,
                new[]
                {
                    txtWidth,
                    txtHeight,
                    txtMedianKernel,
                    txtDiameter,
                    txtSigmaColor,
                    txtSigmaSpace
                },
                new[]
                {
                    btnFilterKernelPreset3,
                    btnFilterKernelPreset5,
                    btnFilterKernelPreset7
                });
            filterInteractionController = new VisionToolFilterInteractionController(
                presenter,
                parameterChangeController,
                cbFilterType,
                cbBorderType,
                panelWidth,
                panelHeight,
                panelKernelPresets,
                chkLockSize,
                panelMedian,
                panelDiameter,
                panelSigmaColor,
                panelSigmaSpace,
                txtWidth,
                txtHeight,
                txtMedianKernel,
                txtDiameter,
                txtSigmaColor,
                txtSigmaSpace);
            textPresenter = new FilterToolTextPresenter(
                gbFilterOptions,
                gbKernel,
                lblFilterType,
                lblBorderType,
                lblKernelWidth,
                lblKernelHeight,
                lblMedianKernel,
                lblDiameter,
                lblSigmaColor,
                lblSigmaSpace);
            AttachToolController(
                "VisionMenu.Filter",
                parameterContentHost,
                refreshViewState: UpdateSummary,
                clearResultReview: null,
                applyToolLocalization: ApplyLocalization);
            ToolController.BindSummary(new Binding("Summary"));
            parameterGuideBinder = VisionToolCustomParameterGuideBinder.Attach(
                toolShell,
                presenter.CreateProperty,
                new Dictionary<FrameworkElement, string>
                {
                    [cbFilterType] = nameof(FilterToolProperty.FilterType),
                    [cbBorderType] = nameof(FilterToolProperty.BorderType),
                    [txtWidth] = nameof(FilterToolProperty.KernelWidth),
                    [txtHeight] = nameof(FilterToolProperty.KernelHeight),
                    [txtMedianKernel] = nameof(FilterToolProperty.MedianKernelSize),
                    [txtDiameter] = nameof(FilterToolProperty.Diameter),
                    [txtSigmaColor] = nameof(FilterToolProperty.SigmaColor),
                    [txtSigmaSpace] = nameof(FilterToolProperty.SigmaSpace)
                });
            ApplyLocalization();
            filterInteractionController.InitializeOptions();
            parameterChangeController.RefreshProgrammatic(filterInteractionController.RefreshModePanels);
            suppressEvents = false;
        }

        protected override void DisposeToolResources()
        {
            parameterGuideBinder.Dispose();
            filterInteractionController.Detach();
            kernelSizeController.Detach();
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            ToolController.ApplyLocalization();
            textPresenter.ApplyLocalization();
        }

        public FilterToolProperty CreateProperty()
        {
            filterInteractionController.FlushParameterBindings();
            return presenter.CreateProperty();
        }

        private void UpdateSummary()
        {
            if (!HasToolController)
            {
                return;
            }

            filterInteractionController?.FlushParameterBindings();
            ToolController.RefreshSummaryBinding();
        }

    }
}
