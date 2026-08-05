using OpenVisionLab.Vision2D.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using OpenVisionLab.Contracts;
using OpenVisionLab.Services;

namespace OpenVisionLab
{
    public partial class MorphologyToolWpfView : VisionToolSingleInputCustomToolViewBase, ISingleInputPropertyVisionToolWpfView<MorphologyToolProperty>
    {
        private readonly MorphologyToolPresenter presenter;

        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolKernelSizeController kernelSizeController;
        private readonly VisionToolMorphologyInteractionController morphologyInteractionController;
        private readonly MorphologyToolTextPresenter textPresenter;
        private readonly VisionToolCustomParameterGuideBinder parameterGuideBinder;
        private bool suppressEvents = true;

        internal MorphologyToolWpfView(MorphologyToolPresenter presenter)
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
                    txtHeight
                },
                new[]
                {
                    btnKernelPreset3,
                    btnKernelPreset5,
                    btnKernelPreset7
                });
            morphologyInteractionController = new VisionToolMorphologyInteractionController(
                presenter,
                parameterChangeController,
                this,
                new[]
                {
                    btnMorphOperationErode,
                    btnMorphOperationDilate,
                    btnMorphOperationOpen,
                    btnMorphOperationClose,
                    btnMorphOperationTopHat,
                    btnMorphOperationBlackHat,
                    btnMorphOperationHitMiss,
                    btnMorphOperationGradient
                },
                new[]
                {
                    rdoShapeRect,
                    rdoShapeEllipse,
                    rdoShapeCross
                });
            textPresenter = new MorphologyToolTextPresenter(
                gbOperation,
                gbKernel,
                lblKernelWidth,
                lblKernelHeight,
                lblShape,
                morphologyInteractionController.RefreshLabels);
            AttachToolController(
                "VisionMenu.Morphology",
                parameterContentHost,
                refreshViewState: UpdateSummary,
                clearResultReview: null,
                applyToolLocalization: ApplyLocalization);
            parameterGuideBinder = VisionToolCustomParameterGuideBinder.Attach(
                toolShell,
                presenter.CreateProperty,
                new Dictionary<FrameworkElement, string>
                {
                    [btnMorphOperationErode] = nameof(MorphologyToolProperty.Operator),
                    [btnMorphOperationDilate] = nameof(MorphologyToolProperty.Operator),
                    [btnMorphOperationOpen] = nameof(MorphologyToolProperty.Operator),
                    [btnMorphOperationClose] = nameof(MorphologyToolProperty.Operator),
                    [btnMorphOperationTopHat] = nameof(MorphologyToolProperty.Operator),
                    [btnMorphOperationBlackHat] = nameof(MorphologyToolProperty.Operator),
                    [btnMorphOperationHitMiss] = nameof(MorphologyToolProperty.Operator),
                    [btnMorphOperationGradient] = nameof(MorphologyToolProperty.Operator),
                    [rdoShapeRect] = nameof(MorphologyToolProperty.Shape),
                    [rdoShapeEllipse] = nameof(MorphologyToolProperty.Shape),
                    [rdoShapeCross] = nameof(MorphologyToolProperty.Shape),
                    [txtWidth] = nameof(MorphologyToolProperty.KernelWidth),
                    [txtHeight] = nameof(MorphologyToolProperty.KernelHeight)
                });
            ApplyLocalization();
            parameterChangeController.RefreshProgrammatic(morphologyInteractionController.RefreshOperationButtons);
            suppressEvents = false;
        }

        protected override void DisposeToolResources()
        {
            parameterGuideBinder.Dispose();
            morphologyInteractionController.Detach();
            kernelSizeController.Detach();
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            ToolController.ApplyLocalization();
            textPresenter.ApplyLocalization();
        }

        public MorphologyToolProperty CreateProperty()
        {
            kernelSizeController.FlushParameterBindings();
            return presenter.CreateProperty();
        }

        private void UpdateSummary()
        {
            if (!HasToolController)
            {
                return;
            }

            kernelSizeController.FlushParameterBindings();
            ToolController.SetSummaryText(morphologyInteractionController.CreateSummary());
        }
    }
}
