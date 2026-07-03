using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OpenVisionLab.Services;

namespace OpenVisionLab
{
    internal sealed class VisionToolMorphologyInteractionController
    {
        private readonly MorphologyToolPresenter presenter;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly FrameworkElement resourceOwner;
        private readonly IReadOnlyList<Button> operationButtons;

        public VisionToolMorphologyInteractionController(
            MorphologyToolPresenter presenter,
            VisionToolParameterChangeController parameterChangeController,
            FrameworkElement resourceOwner,
            IReadOnlyList<Button> operationButtons)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.parameterChangeController = parameterChangeController ?? throw new ArgumentNullException(nameof(parameterChangeController));
            this.resourceOwner = resourceOwner ?? throw new ArgumentNullException(nameof(resourceOwner));
            this.operationButtons = operationButtons ?? throw new ArgumentNullException(nameof(operationButtons));
        }

        public void HandleOperationClick(object sender)
        {
            if (sender is not Button button)
            {
                return;
            }

            parameterChangeController.TryHandle(() =>
            {
                presenter.Operator = VisionToolParameterPolicy.ParseMorphOperation(Convert.ToString(button.Tag));
                RefreshOperationButtons();
            }, schedulePreview: true);
        }

        public void HandleShapeChecked(object sender)
        {
            if (sender is not RadioButton radio)
            {
                return;
            }

            parameterChangeController.TryHandle(() =>
                presenter.Shape = VisionToolParameterPolicy.ParseMorphShape(Convert.ToString(radio.Tag)), schedulePreview: true);
        }

        public void RefreshOperationButtons()
        {
            // Operation visual state belongs to the morphology interaction layer, not to the View's event plumbing.
            foreach (Button button in operationButtons)
            {
                bool selected = string.Equals(Convert.ToString(button.Tag), presenter.Operator.ToString(), StringComparison.OrdinalIgnoreCase);
                button.Background = selected ? GetBrush("VisionTool.SelectionBrush") : GetBrush("VisionTool.PanelBrush");
                button.Foreground = selected ? GetBrush("VisionTool.SelectedButtonTextBrush") : GetBrush("VisionTool.PrimaryTextBrush");
                button.BorderBrush = selected ? GetBrush("VisionTool.AccentBrush") : GetBrush("VisionTool.LineBrush");
            }
        }

        private Brush GetBrush(string resourceKey)
        {
            return resourceOwner.TryFindResource(resourceKey) as Brush ?? Brushes.Transparent;
        }
    }
}
