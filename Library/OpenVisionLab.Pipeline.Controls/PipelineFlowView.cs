using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab.Pipeline.Controls
{
    public sealed class PipelineFlowView : UserControl
    {
        private static readonly Brush SurfaceBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private static readonly Brush PanelBrush = new SolidColorBrush(Color.FromRgb(244, 248, 252));
        private static readonly Brush BorderBrush = new SolidColorBrush(Color.FromRgb(205, 217, 229));
        private static readonly Brush AccentBrush = new SolidColorBrush(Color.FromRgb(35, 85, 132));
        private static readonly Brush MutedBrush = new SolidColorBrush(Color.FromRgb(92, 111, 130));
        private static readonly Brush SelectedBrush = new SolidColorBrush(Color.FromRgb(230, 243, 255));
        private static readonly Brush SelectedBorderBrush = new SolidColorBrush(Color.FromRgb(47, 111, 171));
        private static readonly Brush BranchBrush = new SolidColorBrush(Color.FromRgb(173, 96, 0));
        private static readonly Brush BranchBackgroundBrush = new SolidColorBrush(Color.FromRgb(255, 244, 224));

        private readonly StackPanel stepPanel;
        private readonly TextBlock emptyText;
        private readonly Dictionary<int, Border> rowBorders = new Dictionary<int, Border>();
        private readonly Dictionary<int, Border> cardBorders = new Dictionary<int, Border>();
        private readonly Dictionary<int, Border> inputBorders = new Dictionary<int, Border>();
        private readonly Dictionary<int, Border> outputBorders = new Dictionary<int, Border>();
        private readonly List<PipelineFlowStepItem> steps = new List<PipelineFlowStepItem>();

        public event EventHandler<PipelineFlowStepSelectedEventArgs> StepSelected;

        public PipelineFlowView()
        {
            Background = PanelBrush;

            Grid root = new Grid
            {
                Background = PanelBrush
            };

            ScrollViewer scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(2)
            };

            stepPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            emptyText = new TextBlock
            {
                Text = "No pipeline steps",
                Foreground = MutedBrush,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12)
            };

            scrollViewer.Content = stepPanel;
            root.Children.Add(scrollViewer);
            Content = root;
        }

        public int SelectedIndex { get; private set; } = -1;

        public PipelineFlowPreviewMode SelectedPreviewMode { get; private set; } = PipelineFlowPreviewMode.Overlay;

        public void SetSteps(IEnumerable<PipelineFlowStepItem> items)
        {
            steps.Clear();
            if (items != null)
            {
                steps.AddRange(items.Where(item => item != null));
            }

            Rebuild();
        }

        public void SelectStep(int index)
        {
            SelectStep(index, SelectedPreviewMode);
        }

        public void SelectStep(int index, PipelineFlowPreviewMode mode)
        {
            SelectedIndex = index;
            SelectedPreviewMode = mode;
            foreach (PipelineFlowStepItem step in steps)
            {
                step.IsSelected = step.Index == index;
            }

            UpdateSelectionVisuals();
        }

        private void Rebuild()
        {
            stepPanel.Children.Clear();
            rowBorders.Clear();
            cardBorders.Clear();
            inputBorders.Clear();
            outputBorders.Clear();

            if (steps.Count == 0)
            {
                stepPanel.Children.Add(emptyText);
                return;
            }

            foreach (PipelineFlowStepItem step in steps)
            {
                stepPanel.Children.Add(CreateStepRow(step));
            }

            UpdateSelectionVisuals();
        }

        private FrameworkElement CreateStepRow(PipelineFlowStepItem step)
        {
            Border row = new Border
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 0, 8),
                Padding = new Thickness(0),
                Cursor = Cursors.Hand,
                Tag = step
            };

            Grid layout = new Grid();
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Border card = CreateStepCard(step);
            Grid.SetRow(card, 0);
            layout.Children.Add(card);

            Grid layerFlow = new Grid
            {
                Margin = new Thickness(0, 4, 0, 0)
            };
            layerFlow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            layerFlow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
            layerFlow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Border input = CreateLayerPill(ResolveInputPillLabel(step), step.InputLayer, step.HasInputImage, true, step.IsBranch, step.ExpectedInputLayer);
            Grid.SetColumn(input, 0);
            layerFlow.Children.Add(input);

            TextBlock arrow = CreateArrow(step.IsBranch);
            Grid.SetColumn(arrow, 1);
            layerFlow.Children.Add(arrow);

            Border output = CreateLayerPill("OUT", step.OutputLayer, step.HasOutputImage, false, false, null);
            Grid.SetColumn(output, 2);
            layerFlow.Children.Add(output);

            Grid.SetRow(layerFlow, 1);
            layout.Children.Add(layerFlow);

            row.Child = layout;
            row.MouseLeftButtonDown += (sender, args) =>
            {
                RaiseStepSelected(step, PipelineFlowPreviewMode.Overlay);
            };
            card.MouseLeftButtonDown += (sender, args) =>
            {
                args.Handled = true;
                RaiseStepSelected(step, PipelineFlowPreviewMode.Overlay);
            };
            input.MouseLeftButtonDown += (sender, args) =>
            {
                args.Handled = true;
                RaiseStepSelected(step, PipelineFlowPreviewMode.Input);
            };
            output.MouseLeftButtonDown += (sender, args) =>
            {
                args.Handled = true;
                RaiseStepSelected(step, PipelineFlowPreviewMode.Output);
            };

            rowBorders[step.Index] = row;
            cardBorders[step.Index] = card;
            inputBorders[step.Index] = input;
            outputBorders[step.Index] = output;
            return row;
        }

        private void RaiseStepSelected(PipelineFlowStepItem step, PipelineFlowPreviewMode mode)
        {
            if (step == null)
            {
                return;
            }

            SelectStep(step.Index, mode);
            StepSelected?.Invoke(this, new PipelineFlowStepSelectedEventArgs(step.Index, mode));
        }

        private static Border CreateLayerPill(string label, string layerName, bool hasImage, bool isInput, bool isBranch, string expectedInputLayer)
        {
            Brush background = ResolveLayerBackgroundBrush(hasImage, isInput, isBranch);
            Brush foreground = hasImage
                ? (isInput ? new SolidColorBrush(Color.FromRgb(18, 116, 76)) : new SolidColorBrush(Color.FromRgb(39, 89, 145)))
                : MutedBrush;
            if (isInput && isBranch)
            {
                background = BranchBackgroundBrush;
                foreground = BranchBrush;
            }

            StackPanel content = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            content.Children.Add(new TextBlock
            {
                Text = label,
                Foreground = foreground,
                FontSize = 9,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 1)
            });
            content.Children.Add(new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(layerName) ? "-" : layerName.Trim(),
                Foreground = foreground,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            return new Border
            {
                Background = background,
                BorderBrush = isBranch ? BranchBrush : hasImage ? foreground : BorderBrush,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                MinHeight = 36,
                Padding = new Thickness(6, 4, 6, 4),
                Cursor = Cursors.Hand,
                ToolTip = BuildLayerToolTip(label, layerName, isInput, isBranch, expectedInputLayer),
                Child = content
            };
        }

        private static TextBlock CreateArrow(bool isBranch)
        {
            return new TextBlock
            {
                Text = "->",
                Foreground = isBranch ? BranchBrush : MutedBrush,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private static string ResolveInputPillLabel(PipelineFlowStepItem step)
        {
            if (step == null)
            {
                return "INPUT";
            }

            if (step.IsBranch)
            {
                return "BRANCH IN";
            }

            return string.IsNullOrWhiteSpace(step.ExpectedInputLayer)
                ? "SOURCE"
                : "PREV OUT";
        }

        private static Border CreateStepCard(PipelineFlowStepItem step)
        {
            Grid cardGrid = new Grid();
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            cardGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cardGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cardGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            TextBlock name = new TextBlock
            {
                Text = FormatStepName(step),
                Foreground = AccentBrush,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Margin = new Thickness(0, 0, 8, 1)
            };
            Grid.SetColumn(name, 0);
            Grid.SetRow(name, 0);
            cardGrid.Children.Add(name);

            StackPanel badgePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            if (step.IsBranch)
            {
                badgePanel.Children.Add(CreateFlowBadge("BRANCH"));
            }

            badgePanel.Children.Add(CreateStatusBadge(step.Status, step.StatusText));
            Grid.SetColumn(badgePanel, 1);
            Grid.SetRow(badgePanel, 0);
            cardGrid.Children.Add(badgePanel);

            TextBlock tool = new TextBlock
            {
                Text = SafeText(step.ToolType, "Tool"),
                Foreground = MutedBrush,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            Grid.SetColumn(tool, 0);
            Grid.SetColumnSpan(tool, 2);
            Grid.SetRow(tool, 1);
            cardGrid.Children.Add(tool);

            if (!string.IsNullOrWhiteSpace(step.FlowStateText))
            {
                TextBlock flowState = new TextBlock
                {
                    Text = step.FlowStateText.Trim(),
                    Foreground = step.IsBranch ? BranchBrush : MutedBrush,
                    FontSize = 10,
                    FontWeight = FontWeights.SemiBold,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Margin = new Thickness(0, 3, 0, 0)
                };
                Grid.SetColumn(flowState, 0);
                Grid.SetColumnSpan(flowState, 2);
                Grid.SetRow(flowState, 2);
                cardGrid.Children.Add(flowState);
            }

            return new Border
            {
                Background = SurfaceBrush,
                BorderBrush = BorderBrush,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                MinHeight = 56,
                Padding = new Thickness(8, 6, 8, 6),
                Cursor = Cursors.Hand,
                ToolTip = BuildStepToolTip(step),
                Child = cardGrid
            };
        }

        private static Border CreateFlowBadge(string text)
        {
            return new Border
            {
                Background = BranchBrush,
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(5, 2, 5, 2),
                Margin = new Thickness(0, 0, 4, 0),
                Child = new TextBlock
                {
                    Text = SafeText(text, "BRANCH"),
                    Foreground = Brushes.White,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    TextTrimming = TextTrimming.CharacterEllipsis
                }
            };
        }

        private static Border CreateStatusBadge(PipelineFlowStepStatus status, string statusText)
        {
            Brush brush = ResolveStatusBrush(status);
            return new Border
            {
                Background = brush,
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(6, 2, 6, 2),
                Child = new TextBlock
                {
                    Text = SafeText(statusText, FormatStatus(status)),
                    Foreground = Brushes.White,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    TextTrimming = TextTrimming.CharacterEllipsis
                }
            };
        }

        private void UpdateSelectionVisuals()
        {
            foreach (PipelineFlowStepItem step in steps)
            {
                bool selected = step.Index == SelectedIndex;
                if (rowBorders.TryGetValue(step.Index, out Border row))
                {
                    row.Background = selected ? SelectedBrush : Brushes.Transparent;
                    row.Padding = selected ? new Thickness(2) : new Thickness(0);
                    row.BorderThickness = selected ? new Thickness(1) : new Thickness(0);
                    row.BorderBrush = selected ? SelectedBorderBrush : Brushes.Transparent;
                }

                if (cardBorders.TryGetValue(step.Index, out Border card))
                {
                    bool overlaySelected = selected && SelectedPreviewMode == PipelineFlowPreviewMode.Overlay;
                    card.BorderBrush = overlaySelected ? SelectedBorderBrush : step.IsBranch ? BranchBrush : BorderBrush;
                    card.BorderThickness = overlaySelected ? new Thickness(2) : new Thickness(step.IsBranch ? 1.5 : 1);
                }

                if (inputBorders.TryGetValue(step.Index, out Border input))
                {
                    bool inputSelected = selected && SelectedPreviewMode == PipelineFlowPreviewMode.Input;
                    input.Background = inputSelected ? SelectedBrush : ResolveLayerBackgroundBrush(step.HasInputImage, true, step.IsBranch);
                    input.BorderBrush = inputSelected ? SelectedBorderBrush : ResolveLayerBorderBrush(step.HasInputImage, true, step.IsBranch);
                    input.BorderThickness = inputSelected ? new Thickness(2) : new Thickness(1);
                }

                if (outputBorders.TryGetValue(step.Index, out Border output))
                {
                    bool outputSelected = selected && SelectedPreviewMode == PipelineFlowPreviewMode.Output;
                    output.Background = outputSelected ? SelectedBrush : ResolveLayerBackgroundBrush(step.HasOutputImage, false, false);
                    output.BorderBrush = outputSelected ? SelectedBorderBrush : ResolveLayerBorderBrush(step.HasOutputImage, false, false);
                    output.BorderThickness = outputSelected ? new Thickness(2) : new Thickness(1);
                }
            }
        }

        private static Brush ResolveLayerBackgroundBrush(bool hasImage, bool isInput, bool isBranch)
        {
            if (isInput && isBranch)
            {
                return BranchBackgroundBrush;
            }

            if (!hasImage)
            {
                return new SolidColorBrush(Color.FromRgb(248, 250, 252));
            }

            return isInput
                ? new SolidColorBrush(Color.FromRgb(226, 247, 236))
                : new SolidColorBrush(Color.FromRgb(231, 240, 255));
        }

        private static Brush ResolveLayerBorderBrush(bool hasImage, bool isInput, bool isBranch)
        {
            if (isInput && isBranch)
            {
                return BranchBrush;
            }

            if (!hasImage)
            {
                return BorderBrush;
            }

            return isInput
                ? new SolidColorBrush(Color.FromRgb(18, 116, 76))
                : new SolidColorBrush(Color.FromRgb(39, 89, 145));
        }

        private static Brush ResolveStatusBrush(PipelineFlowStepStatus status)
        {
            switch (status)
            {
                case PipelineFlowStepStatus.Passed:
                    return new SolidColorBrush(Color.FromRgb(0, 150, 85));
                case PipelineFlowStepStatus.Failed:
                case PipelineFlowStepStatus.Timeout:
                    return new SolidColorBrush(Color.FromRgb(205, 58, 58));
                case PipelineFlowStepStatus.Canceled:
                    return new SolidColorBrush(Color.FromRgb(145, 70, 70));
                case PipelineFlowStepStatus.Running:
                    return new SolidColorBrush(Color.FromRgb(220, 135, 30));
                case PipelineFlowStepStatus.Loaded:
                    return new SolidColorBrush(Color.FromRgb(47, 111, 171));
                case PipelineFlowStepStatus.Skipped:
                    return new SolidColorBrush(Color.FromRgb(120, 128, 138));
                default:
                    return new SolidColorBrush(Color.FromRgb(116, 128, 142));
            }
        }

        private static string FormatStatus(PipelineFlowStepStatus status)
        {
            switch (status)
            {
                case PipelineFlowStepStatus.Passed:
                    return "OK";
                case PipelineFlowStepStatus.Failed:
                    return "NG";
                case PipelineFlowStepStatus.Running:
                    return "RUN";
                case PipelineFlowStepStatus.Loaded:
                    return "LOAD";
                case PipelineFlowStepStatus.Skipped:
                    return "SKIP";
                case PipelineFlowStepStatus.Canceled:
                    return "CANCEL";
                case PipelineFlowStepStatus.Timeout:
                    return "TIME";
                default:
                    return "WAIT";
            }
        }

        private static string SafeText(string text, string fallback)
        {
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        }

        private static string BuildLayerToolTip(string label, string layerName, bool isInput, bool isBranch, string expectedInputLayer)
        {
            string text = $"{label}: {SafeText(layerName, "-")} - click to inspect input/output image";
            if (isInput && isBranch)
            {
                text += $"\nBranch input. Previous output is {SafeText(expectedInputLayer, "-")}.";
            }

            return text;
        }

        private static string BuildStepToolTip(PipelineFlowStepItem step)
        {
            if (step == null)
            {
                return "Click to inspect overlay preview";
            }

            string text = "Click to inspect overlay preview";
            if (step.IsBranch)
            {
                text += $"\nBranch step. Input '{SafeText(step.InputLayer, "-")}' does not follow previous output '{SafeText(step.ExpectedInputLayer, "-")}'.";
            }

            return text;
        }

        private static string FormatStepName(PipelineFlowStepItem step)
        {
            string name = SafeText(step?.Name, "Step");
            if (HasNumberPrefix(name))
            {
                return name;
            }

            int index = step == null ? 0 : step.Index + 1;
            return $"{index:00} {name}";
        }

        private static bool HasNumberPrefix(string text)
        {
            return !string.IsNullOrWhiteSpace(text)
                && text.Length >= 2
                && char.IsDigit(text[0])
                && char.IsDigit(text[1])
                && (text.Length == 2 || char.IsWhiteSpace(text[2]) || text[2] == '.' || text[2] == '_' || text[2] == '-');
        }
    }
}
