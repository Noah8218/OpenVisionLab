using MahApps.Metro.IconPacks;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab
{
    public enum VisionToolPreviewImageRole
    {
        Input,
        InputA,
        InputB,
        Output
    }

    public sealed class VisionToolPreviewImageCommandEventArgs : EventArgs
    {
        public VisionToolPreviewImageCommandEventArgs(VisionToolPreviewImageRole role)
        {
            Role = role;
        }

        public VisionToolPreviewImageRole Role { get; }
    }

    public interface IVisionToolPreviewImageCommands
    {
        event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested;
        event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested;
    }

    internal static class VisionToolPreviewSlotBehavior
    {
        private static readonly DependencyProperty SlotStateProperty =
            DependencyProperty.RegisterAttached(
                "SlotState",
                typeof(PreviewSlotState),
                typeof(VisionToolPreviewSlotBehavior),
                new PropertyMetadata(null));

        public static void Refresh(Border frame)
        {
            if (frame?.GetValue(SlotStateProperty) is PreviewSlotState state)
            {
                state.Refresh();
            }
        }

        public static void Detach(Border frame)
        {
            if (frame?.GetValue(SlotStateProperty) is not PreviewSlotState state)
            {
                return;
            }

            state.Dispose();
            frame.ClearValue(SlotStateProperty);
        }

        public static void AttachSingle(
            Border inputFrame,
            Image inputImage,
            Border outputFrame,
            Image outputImage,
            object sender,
            EventHandler<VisionToolPreviewImageCommandEventArgs> loadRequested,
            EventHandler<VisionToolPreviewImageCommandEventArgs> saveRequested)
        {
            // Keep preview image command routing role-based so tool Views do not duplicate load/save lambdas.
            Attach(inputFrame, inputImage, true, CreateRoleHandler(sender, loadRequested, VisionToolPreviewImageRole.Input), CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.Input));
            Attach(outputFrame, outputImage, false, null, CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.Output));
        }

        public static void AttachArithmetic(
            Border inputAFrame,
            Image inputAImage,
            Border inputBFrame,
            Image inputBImage,
            Border outputFrame,
            Image outputImage,
            object sender,
            EventHandler<VisionToolPreviewImageCommandEventArgs> loadRequested,
            EventHandler<VisionToolPreviewImageCommandEventArgs> saveRequested)
        {
            Attach(inputAFrame, inputAImage, true, CreateRoleHandler(sender, loadRequested, VisionToolPreviewImageRole.InputA), CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.InputA));
            Attach(inputBFrame, inputBImage, true, CreateRoleHandler(sender, loadRequested, VisionToolPreviewImageRole.InputB), CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.InputB));
            Attach(outputFrame, outputImage, false, null, CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.Output));
        }

        public static void AttachSingle(
            Border inputFrame,
            VisionToolInlinePreviewSlot inputPreview,
            Border outputFrame,
            VisionToolInlinePreviewSlot outputPreview,
            object sender,
            EventHandler<VisionToolPreviewImageCommandEventArgs> loadRequested,
            EventHandler<VisionToolPreviewImageCommandEventArgs> saveRequested)
        {
            Attach(inputFrame, inputPreview, true, CreateRoleHandler(sender, loadRequested, VisionToolPreviewImageRole.Input), CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.Input));
            Attach(outputFrame, outputPreview, false, null, CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.Output));
        }

        public static void AttachArithmetic(
            Border inputAFrame,
            VisionToolInlinePreviewSlot inputAPreview,
            Border inputBFrame,
            VisionToolInlinePreviewSlot inputBPreview,
            Border outputFrame,
            VisionToolInlinePreviewSlot outputPreview,
            object sender,
            EventHandler<VisionToolPreviewImageCommandEventArgs> loadRequested,
            EventHandler<VisionToolPreviewImageCommandEventArgs> saveRequested)
        {
            Attach(inputAFrame, inputAPreview, true, CreateRoleHandler(sender, loadRequested, VisionToolPreviewImageRole.InputA), CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.InputA));
            Attach(inputBFrame, inputBPreview, true, CreateRoleHandler(sender, loadRequested, VisionToolPreviewImageRole.InputB), CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.InputB));
            Attach(outputFrame, outputPreview, false, null, CreateRoleHandler(sender, saveRequested, VisionToolPreviewImageRole.Output));
        }

        public static void Attach(
            Border frame,
            Image image,
            bool allowLoad,
            RoutedEventHandler loadRequested,
            RoutedEventHandler saveRequested)
        {
            if (frame == null || image == null)
            {
                return;
            }

            PreviewSlotState state = AttachCore(frame, image, allowLoad, loadRequested, saveRequested, () => image.Source != null);
            if (state == null)
            {
                return;
            }

            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
            EventHandler sourceChangedHandler = (_, __) => state.Refresh();
            descriptor?.AddValueChanged(image, sourceChangedHandler);
            state.Track(() => descriptor?.RemoveValueChanged(image, sourceChangedHandler));
        }

        public static void Attach(
            Border frame,
            VisionToolInlinePreviewSlot preview,
            bool allowLoad,
            RoutedEventHandler loadRequested,
            RoutedEventHandler saveRequested)
        {
            if (frame == null || preview == null)
            {
                return;
            }

            PreviewSlotState state = AttachCore(frame, preview, allowLoad, loadRequested, saveRequested, () => preview.HasImage);
            if (state == null)
            {
                return;
            }

            EventHandler imageChangedHandler = (_, __) => state.Refresh();
            preview.ImageChanged += imageChangedHandler;
            state.Track(() => preview.ImageChanged -= imageChangedHandler);
        }

        private static RoutedEventHandler CreateRoleHandler(
            object sender,
            EventHandler<VisionToolPreviewImageCommandEventArgs> requested,
            VisionToolPreviewImageRole role)
        {
            return requested == null
                ? null
                : (_, __) => requested(sender, new VisionToolPreviewImageCommandEventArgs(role));
        }

        private static PreviewSlotState AttachCore(
            Border frame,
            UIElement previewElement,
            bool allowLoad,
            RoutedEventHandler loadRequested,
            RoutedEventHandler saveRequested,
            Func<bool> hasImage)
        {
            if (frame == null || previewElement == null || hasImage == null)
            {
                return null;
            }

            UIElement originalChild = frame.Child;
            Grid host = new Grid { ClipToBounds = true };
            frame.Child = null;
            if (originalChild != null)
            {
                host.Children.Add(originalChild);
            }

            Border emptyOverlay = CreateEmptyOverlay(allowLoad, loadRequested);
            host.Children.Add(emptyOverlay);
            Border routeOverlay = CreateRouteOverlay(allowLoad);
            host.Children.Add(routeOverlay);
            frame.Child = host;

            MenuItem saveItem = CreateMenuItem("ToolView.SaveImage", PackIconMaterialKind.FileExport, saveRequested);
            ContextMenu contextMenu = new ContextMenu();
            MenuItem loadItem = null;
            if (allowLoad)
            {
                loadItem = CreateMenuItem("ToolView.LoadImage", PackIconMaterialKind.ImagePlusOutline, loadRequested);
                contextMenu.Items.Add(loadItem);
            }

            contextMenu.Items.Add(saveItem);
            contextMenu.Opened += (_, __) =>
            {
                saveItem.IsEnabled = hasImage();
            };
            frame.ContextMenu = contextMenu;

            PreviewSlotState state = new PreviewSlotState(frame, previewElement, hasImage, emptyOverlay, routeOverlay, allowLoad, loadItem, saveItem);
            frame.SetValue(SlotStateProperty, state);

            void Refresh()
            {
                state.Refresh();
            }

            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            state.Track(() => OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged);
            host.SizeChanged += OnHostSizeChanged;
            state.Track(() => host.SizeChanged -= OnHostSizeChanged);
            Refresh();

            void OnLanguageChanged(object sender, EventArgs e)
            {
                Refresh();
            }

            void OnHostSizeChanged(object sender, SizeChangedEventArgs e)
            {
                Refresh();
            }

            return state;
        }

        private sealed class PreviewSlotState : IDisposable
        {
            private readonly Border frame;
            private readonly UIElement previewElement;
            private readonly Func<bool> hasImage;
            private readonly Border emptyOverlay;
            private readonly Border routeOverlay;
            private readonly bool allowLoad;
            private readonly MenuItem loadItem;
            private readonly MenuItem saveItem;
            private readonly System.Collections.Generic.List<Action> detachActions = new System.Collections.Generic.List<Action>();
            private bool disposed;

            public PreviewSlotState(Border frame, UIElement previewElement, Func<bool> hasImage, Border emptyOverlay, Border routeOverlay, bool allowLoad, MenuItem loadItem, MenuItem saveItem)
            {
                this.frame = frame;
                this.previewElement = previewElement;
                this.hasImage = hasImage;
                this.emptyOverlay = emptyOverlay;
                this.routeOverlay = routeOverlay;
                this.allowLoad = allowLoad;
                this.loadItem = loadItem;
                this.saveItem = saveItem;
            }

            public void Track(Action detachAction)
            {
                if (detachAction != null)
                {
                    detachActions.Add(detachAction);
                }
            }

            public void Refresh()
            {
                if (disposed)
                {
                    return;
                }

                bool imageVisible = hasImage();
                if (previewElement != null)
                {
                    previewElement.Visibility = imageVisible ? Visibility.Visible : Visibility.Hidden;
                }

                emptyOverlay.Visibility = imageVisible ? Visibility.Collapsed : Visibility.Visible;
                RefreshTexts(emptyOverlay, allowLoad);
                ApplyEmptyOverlayDensity(emptyOverlay);
                ApplyRouteOverlayDensity(routeOverlay, imageVisible, allowLoad);
                if (frame != null)
                {
                    frame.Cursor = imageVisible ? Cursors.Hand : Cursors.Arrow;
                    frame.ToolTip = imageVisible
                        ? OpenVisionLanguageService.T(allowLoad ? "ToolView.RoutePreviewInputToolTip" : "ToolView.RoutePreviewOutputToolTip")
                        : null;
                }

                if (loadItem != null)
                {
                    loadItem.Header = OpenVisionLanguageService.T("ToolView.LoadImage");
                }

                if (saveItem != null)
                {
                    saveItem.Header = OpenVisionLanguageService.T("ToolView.SaveImage");
                    saveItem.IsEnabled = imageVisible;
                }
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                foreach (Action detachAction in detachActions)
                {
                    detachAction();
                }

                detachActions.Clear();
                GC.SuppressFinalize(this);
            }
        }

        private static Border CreateEmptyOverlay(bool allowLoad, RoutedEventHandler loadRequested)
        {
            StackPanel panel = new StackPanel();
            panel.Children.Add(new PackIconMaterial
            {
                Name = "icoPreviewEmpty",
                Kind = PackIconMaterialKind.ImageOffOutline,
                Width = 24,
                Height = 24,
                Foreground = CreateBrush("#157C86"),
                Margin = new Thickness(0, 0, 0, 8)
            });
            panel.Children.Add(new TextBlock
            {
                Name = "txtPreviewEmptyTitle",
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = CreateBrush("#243040"),
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.None
            });
            panel.Children.Add(new TextBlock
            {
                Name = "txtPreviewEmptyDetail",
                Foreground = CreateBrush("#657487"),
                Margin = new Thickness(0, 5, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.None,
                FontSize = 11
            });

            if (allowLoad)
            {
                Button loadButton = new Button
                {
                    Name = "btnPreviewLoadImage",
                    Height = 28,
                    MinWidth = 116,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 12, 0, 0),
                    Padding = new Thickness(9, 0, 9, 0),
                    Background = Brushes.White,
                    BorderBrush = CreateBrush("#157C86"),
                    Foreground = CreateBrush("#157C86"),
                    BorderThickness = new Thickness(1)
                };
                loadButton.Click += loadRequested;
                StackPanel buttonContent = new StackPanel { Orientation = Orientation.Horizontal };
                buttonContent.Children.Add(new PackIconMaterial
                {
                    Kind = PackIconMaterialKind.ImagePlusOutline,
                    Width = 15,
                    Height = 15,
                    Margin = new Thickness(0, 0, 6, 0),
                    Foreground = CreateBrush("#157C86")
                });
                buttonContent.Children.Add(new TextBlock
                {
                    Name = "txtPreviewLoadImageButton",
                    FontWeight = FontWeights.SemiBold,
                    Foreground = CreateBrush("#157C86"),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextTrimming = TextTrimming.None
                });
                loadButton.Content = buttonContent;
                panel.Children.Add(loadButton);
            }

            return new Border
            {
                Width = 252,
                Padding = new Thickness(16),
                Background = CreateBrush("#EEF5F7"),
                BorderBrush = CreateBrush("#75AEB7"),
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = panel
            };
        }

        private static Border CreateRouteOverlay(bool allowLoad)
        {
            Border overlay = new Border
            {
                Name = "bdPreviewRouteOverlay",
                Width = 28,
                Height = 26,
                Margin = new Thickness(0, 6, 6, 0),
                Padding = new Thickness(0),
                Background = CreateBrush("#DDEAF0"),
                BorderBrush = CreateBrush("#75AEB7"),
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                IsHitTestVisible = false,
                Child = new PackIconMaterial
                {
                    Kind = PackIconMaterialKind.ImageMultipleOutline,
                    Width = 15,
                    Height = 15,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = CreateBrush("#157C86")
                }
            };
            AutomationProperties.SetAutomationId(overlay, allowLoad ? "VisionToolInputRoutePreviewHint" : "VisionToolOutputRoutePreviewHint");
            return overlay;
        }

        private static void ApplyEmptyOverlayDensity(Border emptyOverlay)
        {
            if (emptyOverlay?.Child is not StackPanel panel)
            {
                return;
            }

            double availableHeight = (emptyOverlay.Parent as FrameworkElement)?.ActualHeight ?? emptyOverlay.ActualHeight;
            bool compact = availableHeight > 0D && availableHeight < 110D;

            emptyOverlay.Width = compact ? double.NaN : 252D;
            emptyOverlay.Padding = compact ? new Thickness(8, 5, 8, 5) : new Thickness(16);

            foreach (object child in panel.Children)
            {
                if (child is PackIconMaterial icon && string.Equals(icon.Name, "icoPreviewEmpty", StringComparison.Ordinal))
                {
                    icon.Width = compact ? 16D : 24D;
                    icon.Height = compact ? 16D : 24D;
                    icon.Margin = compact ? new Thickness(0, 0, 0, 2) : new Thickness(0, 0, 0, 8);
                }
                else if (child is TextBlock textBlock && string.Equals(textBlock.Name, "txtPreviewEmptyTitle", StringComparison.Ordinal))
                {
                    textBlock.FontSize = compact ? 11D : 13D;
                    textBlock.TextWrapping = compact ? TextWrapping.NoWrap : TextWrapping.Wrap;
                    textBlock.TextTrimming = compact ? TextTrimming.CharacterEllipsis : TextTrimming.None;
                }
                else if (child is TextBlock detailBlock && string.Equals(detailBlock.Name, "txtPreviewEmptyDetail", StringComparison.Ordinal))
                {
                    detailBlock.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
                }
                else if (child is Button button && string.Equals(button.Name, "btnPreviewLoadImage", StringComparison.Ordinal))
                {
                    button.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private static void ApplyRouteOverlayDensity(Border routeOverlay, bool imageVisible, bool allowLoad)
        {
            if (routeOverlay == null)
            {
                return;
            }

            double availableHeight = (routeOverlay.Parent as FrameworkElement)?.ActualHeight ?? routeOverlay.ActualHeight;
            bool compact = availableHeight > 0D && availableHeight < 140D;
            routeOverlay.Visibility = imageVisible && compact ? Visibility.Visible : Visibility.Collapsed;
            routeOverlay.ToolTip = OpenVisionLanguageService.T(allowLoad ? "ToolView.RoutePreviewInputToolTip" : "ToolView.RoutePreviewOutputToolTip");
        }

        private static Border CreateCommandBar(bool allowLoad, RoutedEventHandler loadRequested, RoutedEventHandler saveRequested)
        {
            StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
            if (allowLoad)
            {
                panel.Children.Add(CreateIconButton("ToolView.LoadImage", PackIconMaterialKind.ImagePlusOutline, loadRequested));
            }

            panel.Children.Add(CreateIconButton("ToolView.SaveImage", PackIconMaterialKind.FileExport, saveRequested));
            return new Border
            {
                Padding = new Thickness(4),
                Margin = new Thickness(0, 8, 8, 0),
                Background = CreateBrush("#DDEAF0"),
                BorderBrush = CreateBrush("#75AEB7"),
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Child = panel
            };
        }

        private static Button CreateIconButton(string tooltipKey, PackIconMaterialKind iconKind, RoutedEventHandler click)
        {
            Button button = new Button
            {
                Width = 28,
                Height = 26,
                Padding = new Thickness(0),
                Margin = new Thickness(2, 0, 2, 0),
                Background = Brushes.White,
                BorderBrush = CreateBrush("#8FB9C2"),
                Foreground = CreateBrush("#157C86"),
                BorderThickness = new Thickness(1),
                Content = new PackIconMaterial
                {
                    Kind = iconKind,
                    Width = 15,
                    Height = 15,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = CreateBrush("#157C86")
                }
            };
            button.Click += click;
            button.Tag = tooltipKey;
            return button;
        }

        private static MenuItem CreateMenuItem(string key, PackIconMaterialKind iconKind, RoutedEventHandler click)
        {
            MenuItem item = new MenuItem
            {
                Header = OpenVisionLanguageService.T(key),
                Icon = new PackIconMaterial
                {
                    Kind = iconKind,
                    Width = 16,
                    Height = 16,
                    Foreground = CreateBrush("#157C86")
                }
            };
            item.Click += click;
            return item;
        }

        private static void RefreshTexts(Border emptyOverlay, bool allowLoad)
        {
            if (emptyOverlay?.Child is not StackPanel panel)
            {
                return;
            }

            foreach (object child in panel.Children)
            {
                if (child is TextBlock textBlock && string.Equals(textBlock.Name, "txtPreviewEmptyTitle", StringComparison.Ordinal))
                {
                    textBlock.Text = OpenVisionLanguageService.T(allowLoad ? "ToolView.NoInputImageTitle" : "ToolView.NoOutputImageTitle");
                }
                else if (child is TextBlock detailBlock && string.Equals(detailBlock.Name, "txtPreviewEmptyDetail", StringComparison.Ordinal))
                {
                    detailBlock.Text = OpenVisionLanguageService.T(allowLoad ? "ToolView.NoInputImageDetail" : "ToolView.NoOutputImageDetail");
                }
                else if (child is Button button && button.Content is StackPanel buttonPanel)
                {
                    foreach (object buttonChild in buttonPanel.Children)
                    {
                        if (buttonChild is TextBlock buttonText)
                        {
                            buttonText.Text = OpenVisionLanguageService.T("ToolView.LoadImageButton");
                        }
                    }

                    button.ToolTip = OpenVisionLanguageService.T("ToolView.LoadImage");
                }
            }
        }

        private static void RefreshCommandBar(Border commandBar, bool allowLoad)
        {
            if (commandBar?.Child is not StackPanel panel)
            {
                return;
            }

            foreach (object child in panel.Children)
            {
                if (child is Button button && button.Tag is string key)
                {
                    button.ToolTip = OpenVisionLanguageService.T(key);
                }
            }
        }

        private static Brush CreateBrush(string hex)
        {
            return (Brush)new BrushConverter().ConvertFromString(hex);
        }
    }
}
