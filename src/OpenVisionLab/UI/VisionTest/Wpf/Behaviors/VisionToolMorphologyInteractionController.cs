using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Services;

namespace OpenVisionLab
{
    internal sealed class VisionToolMorphologyInteractionController
    {
        private static readonly Dictionary<string, string> OperationLocalizationKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Erode"] = "Morphology.Operation.Erode",
            ["Dilate"] = "Morphology.Operation.Dilate",
            ["Open"] = "Morphology.Operation.Open",
            ["Close"] = "Morphology.Operation.Close",
            ["TopHat"] = "Morphology.Operation.TopHat",
            ["BlackHat"] = "Morphology.Operation.BlackHat",
            ["HitMiss"] = "Morphology.Operation.HitMiss",
            ["Gradient"] = "Morphology.Operation.Gradient"
        };

        private static readonly Dictionary<string, string> ShapeLocalizationKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Rect"] = "Morphology.Shape.Rect",
            ["Ellipse"] = "Morphology.Shape.Ellipse",
            ["Cross"] = "Morphology.Shape.Cross"
        };

        private readonly MorphologyToolPresenter presenter;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly FrameworkElement resourceOwner;
        private readonly IReadOnlyList<Button> operationButtons;
        private readonly IReadOnlyList<RadioButton> shapeButtons;

        public VisionToolMorphologyInteractionController(
            MorphologyToolPresenter presenter,
            VisionToolParameterChangeController parameterChangeController,
            FrameworkElement resourceOwner,
            IReadOnlyList<Button> operationButtons,
            IReadOnlyList<RadioButton> shapeButtons)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.parameterChangeController = parameterChangeController ?? throw new ArgumentNullException(nameof(parameterChangeController));
            this.resourceOwner = resourceOwner ?? throw new ArgumentNullException(nameof(resourceOwner));
            this.operationButtons = operationButtons ?? throw new ArgumentNullException(nameof(operationButtons));
            this.shapeButtons = shapeButtons ?? throw new ArgumentNullException(nameof(shapeButtons));
            AttachControls();
        }

        public void Detach()
        {
            foreach (Button button in operationButtons)
            {
                if (button != null)
                {
                    button.Click -= OperationButton_Click;
                }
            }

            foreach (RadioButton radioButton in shapeButtons)
            {
                if (radioButton != null)
                {
                    radioButton.Checked -= ShapeRadioButton_Checked;
                }
            }
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

        public void RefreshLabels()
        {
            foreach (Button button in operationButtons)
            {
                button.Content = ResolveDisplayText(Convert.ToString(button.Tag), OperationLocalizationKeys);
            }

            foreach (RadioButton radioButton in shapeButtons)
            {
                radioButton.Content = ResolveDisplayText(Convert.ToString(radioButton.Tag), ShapeLocalizationKeys);
            }
        }

        public string CreateSummary()
        {
            MorphologyToolProperty property = presenter.CreateProperty();
            return $"{ResolveDisplayText(property.Operator.ToString(), OperationLocalizationKeys)} / {ResolveDisplayText(property.Shape.ToString(), ShapeLocalizationKeys)} / {property.KernelWidth} x {property.KernelHeight}";
        }

        private static string ResolveDisplayText(string value, IReadOnlyDictionary<string, string> localizationKeys)
        {
            if (!string.IsNullOrWhiteSpace(value)
                && localizationKeys.TryGetValue(value, out string localizationKey))
            {
                string localizedText = OpenVisionLanguageService.T(localizationKey);
                if (!string.IsNullOrWhiteSpace(localizedText) && !string.Equals(localizedText, localizationKey, StringComparison.Ordinal))
                {
                    return localizedText;
                }
            }

            return value ?? string.Empty;
        }

        private Brush GetBrush(string resourceKey)
        {
            return resourceOwner.TryFindResource(resourceKey) as Brush ?? Brushes.Transparent;
        }

        private void AttachControls()
        {
            foreach (Button button in operationButtons)
            {
                if (button != null)
                {
                    button.Click += OperationButton_Click;
                }
            }

            foreach (RadioButton radioButton in shapeButtons)
            {
                if (radioButton != null)
                {
                    radioButton.Checked += ShapeRadioButton_Checked;
                }
            }
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            HandleOperationClick(sender);
        }

        private void ShapeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            HandleShapeChecked(sender);
        }
    }
}
