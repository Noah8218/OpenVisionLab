using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace OpenVisionLab
{
    // 임시 View 배치와 modal 결과만 소유한다. 검사 값과 asset은 기존 호출자가 쓴다.
    internal sealed class InlineImageEditHost : IDisposable
    {
        private readonly Window owner;
        private readonly Grid overlay;
        private readonly ContentControl underlyingContent;
        private readonly OpenVisionWorkbenchViewModel workbench;
        private DispatcherFrame frame;
        private InlineImageEditViewModel model;
        private IInlineImageEditPresentation presentation;
        private IPropertyGridImageEditView editor;
        private bool accepted, disposed;

        internal InlineImageEditHost(Window owner, Grid overlay, ContentControl underlyingContent, OpenVisionWorkbenchViewModel workbench)
        {
            this.owner = owner; this.overlay = overlay; this.underlyingContent = underlyingContent; this.workbench = workbench;
        }

        internal bool IsActive => frame != null;

        internal bool Show(IPropertyGridImageEditView value, DependencyObject caller)
        {
            if (disposed || IsActive || value is not IInlineImageEditPresentation edit || value is not Window editWindow) { return false; }
            var pane = Descendants<OpenVisionLayerViewerView>(owner).Where(view => view.IsVisible && view.MainPane != null)
                .OrderBy(view => view.MainPane.Source == "follow:in" ? 0 : 1).FirstOrDefault();
            if (pane == null) { return false; }
            Window callerWindow = Window.GetWindow(caller as DependencyObject);
            Visibility paneVisibility = pane.Visibility;
            bool contentEnabled = underlyingContent.IsEnabled, callerEnabled = callerWindow?.IsEnabled == true;
            IInputElement previousFocus = Keyboard.FocusedElement;
            var regions = (editWindow.DataContext as RoiEditorViewModel)?.Regions;
            System.Collections.Specialized.NotifyCollectionChangedEventHandler regionsChanged = null;
            KeyboardFocusChangedEventHandler coordinateFocusLost = (_, _) => { if (frame != null && !disposed) { edit.FlushInlineCoordinates(); } };
            SizeChangedEventHandler resized = null; EventHandler layoutChanged = null;
            var moved = new List<(FrameworkElement Element, DependencyObject Parent, int Index, object LocalDataContext)>();
            presentation = edit; editor = value; model = new InlineImageEditViewModel(edit.IsMultiInline, edit.IsTemplateInline, () => edit.InlineRegionCount); accepted = false;
            model.Requested += OnRequested;
            regionsChanged = (_, _) => model.RefreshTitle(); if (regions != null) { regions.CollectionChanged += regionsChanged; }
            try
            {
                var bounds = pane.TransformToVisual(overlay).TransformBounds(new Rect(pane.RenderSize));
                var view = new UserControl { DataContext = model, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(bounds.X, bounds.Y, 0, 0), Width = Math.Max(1, bounds.Width), Height = Math.Max(1, bounds.Height), Focusable = true, ClipToBounds = true };
                ScrollViewer barScroll = null;
                void Place()
                {
                    if (disposed || overlay.Visibility != Visibility.Visible) { return; }
                    Rect current = pane.TransformToVisual(overlay).TransformBounds(new Rect(pane.RenderSize));
                    Thickness margin = new Thickness(current.X, current.Y, 0, 0);
                    if (view.Margin != margin) { view.Margin = margin; }
                    if (view.Width != Math.Max(1, current.Width)) { view.Width = Math.Max(1, current.Width); }
                    if (view.Height != Math.Max(1, current.Height)) { view.Height = Math.Max(1, current.Height); }
                    if (barScroll != null) { barScroll.MaxHeight = Math.Max(80, current.Height * .5); }
                }
                resized = (_, _) => owner.Dispatcher.BeginInvoke((Action)Place, DispatcherPriority.Loaded);
                layoutChanged = (_, _) => Place(); owner.SizeChanged += resized; pane.LayoutUpdated += layoutChanged;
                KeyboardNavigation.SetTabNavigation(view, KeyboardNavigationMode.Cycle);
                var body = new Grid(); body.RowDefinitions.Add(new RowDefinition()); body.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); body.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                FrameworkElement viewport = edit.InlineViewport; Move(viewport, moved, editWindow.DataContext); body.Children.Add(viewport);
                var bar = new StackPanel { Margin = new Thickness(6) }; bar.SetResourceReference(Panel.BackgroundProperty, "OpenVision.Workbench.PanelBrush");
                var labels = new TextBlock { Text = model.Notice, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 6) }; labels.SetResourceReference(TextBlock.ForegroundProperty, "OpenVision.Workbench.TextBrush"); bar.Children.Add(labels);
                var fields = new WrapPanel(); var title = Label(string.Empty); title.SetBinding(TextBlock.TextProperty, new Binding(nameof(model.Title))); fields.Children.Add(title); int index = 0;
                foreach (var field in edit.InlineFields)
                {
                    var pair = new StackPanel { Orientation = Orientation.Horizontal }; pair.Children.Add(Label(new[] { "X", "Y", "W", "H", "°" }[index++])); Move(field, moved, editWindow.DataContext);
                    field.LostKeyboardFocus += coordinateFocusLost;
                    field.Style = (Style)owner.FindResource("Main.RecipeTextBox"); field.IsReadOnly = false; field.MinWidth = 0; field.Width = index == 5 ? 84 : 52; field.Margin = new Thickness(2); pair.Children.Add(field); fields.Children.Add(pair);
                }
                bar.Children.Add(fields);
                if (model.IsTemplate && edit.InlinePatternPreview is FrameworkElement preview)
                {
                    Move(preview, moved, editWindow.DataContext); foreach (var text in Descendants<TextBlock>(preview)) { text.SetResourceReference(TextBlock.ForegroundProperty, "OpenVision.Workbench.SecondaryTextBrush"); } preview.Height = 64; preview.HorizontalAlignment = HorizontalAlignment.Left; bar.Children.Add(preview);
                }
                var buttons = new WrapPanel();
                if (!model.IsTemplate) { buttons.Children.Add(Button(model.WholeText, "WholeCommand")); }
                if (model.IsMulti) { buttons.Children.Add(Button(model.AddText, "AddCommand")); buttons.Children.Add(Button(model.DeleteText, "DeleteCommand")); }
                buttons.Children.Add(Button(model.LargeText, "LargeCommand")); buttons.Children.Add(Button(model.CancelText, "CancelCommand")); buttons.Children.Add(Button(model.ApplyText, "ApplyCommand", true));
                var footer = new Border { Padding = new Thickness(6), Child = buttons }; footer.SetResourceReference(Border.BackgroundProperty, "OpenVision.Workbench.PanelBrush"); Grid.SetRow(footer, 2); body.Children.Add(footer);
                var status = new TextBlock { TextWrapping = TextWrapping.Wrap }; status.SetBinding(TextBlock.TextProperty, new Binding(nameof(model.Status))); status.SetResourceReference(TextBlock.ForegroundProperty, "OpenVision.Workbench.NgBrush"); bar.Children.Add(status);
                barScroll = new ScrollViewer { Content = bar, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, MaxHeight = Math.Max(80, view.Height * .5) }; Grid.SetRow(barScroll, 1); body.Children.Add(barScroll); view.Content = body; overlay.Children.Add(view); overlay.Visibility = Visibility.Visible;
                underlyingContent.IsEnabled = false; pane.Visibility = Visibility.Hidden;
                if (callerWindow != null && !ReferenceEquals(callerWindow, owner)) { callerWindow.IsEnabled = false; callerWindow.Closed += CallerClosed; }
                workbench.SetImageEditOpen(true);
                frame = new DispatcherFrame();
                owner.Dispatcher.BeginInvoke(new Action(() => { if (frame != null && !disposed) { view.UpdateLayout(); edit.InitializeInline(); owner.Activate(); viewport.Focus(); } }), DispatcherPriority.Loaded);
                Dispatcher.PushFrame(frame);
                return accepted && !disposed;
            }
            finally
            {
                if (resized != null) { owner.SizeChanged -= resized; } if (layoutChanged != null) { pane.LayoutUpdated -= layoutChanged; }
                if (callerWindow != null) { callerWindow.Closed -= CallerClosed; if (!ReferenceEquals(callerWindow, owner) && callerWindow.IsLoaded) { callerWindow.IsEnabled = callerEnabled; } }
                foreach (var field in edit.InlineFields) { field.LostKeyboardFocus -= coordinateFocusLost; }
                model.Requested -= OnRequested; if (regions != null) { regions.CollectionChanged -= regionsChanged; }
                overlay.Children.Clear(); overlay.Visibility = Visibility.Collapsed;
                foreach (var item in moved.AsEnumerable().Reverse())
                {
                    if (item.Element.Parent is Panel parent) { parent.Children.Remove(item.Element); }
                    if (item.Parent is Panel originalPanel) { originalPanel.Children.Insert(Math.Min(item.Index, originalPanel.Children.Count), item.Element); }
                    else if (item.Parent is Decorator originalDecorator) { originalDecorator.Child = item.Element; }
                    if (item.LocalDataContext == DependencyProperty.UnsetValue) { item.Element.ClearValue(FrameworkElement.DataContextProperty); } else { item.Element.SetValue(FrameworkElement.DataContextProperty, item.LocalDataContext); }
                }
                underlyingContent.IsEnabled = contentEnabled; if (owner.IsLoaded) { pane.Visibility = paneVisibility; } workbench.SetImageEditOpen(false);
                frame = null; model = null; presentation = null; editor = null;
                if (previousFocus is UIElement element && element.IsVisible && element.IsEnabled) { element.Focus(); }
            }
        }

        internal bool HandleKey(KeyEventArgs args) => HandleKey(args.Key, Keyboard.Modifiers, Keyboard.FocusedElement is System.Windows.Controls.Primitives.TextBoxBase, args.SystemKey);
        internal bool HandleKey(Key key, ModifierKeys modifiers, bool textEditing, Key systemKey = Key.None)
        {
            if (!IsActive) { return false; }
            if (key == Key.Escape) { OnRequested("cancel"); return true; }
            if (key == Key.Enter) { OnRequested("apply"); return true; }
            if (key == Key.Delete && !textEditing) { OnRequested("delete"); return true; }
            if (key == Key.System && systemKey == Key.F4) { return false; }
            if (textEditing && (modifiers & ModifierKeys.Control) != 0 && key is Key.Z or Key.Y) { return false; }
            return key >= Key.F1 && key <= Key.F24 || (modifiers & ModifierKeys.Alt) != 0
                || (modifiers & ModifierKeys.Control) != 0 && key is not (Key.C or Key.V or Key.X or Key.A);
        }

        private void OnRequested(string action)
        {
            if (!IsActive) { return; }
            if (action == "cancel") { accepted = false; frame.Continue = false; }
            else if (action == "apply")
            {
                if (presentation.AcceptInline()) { accepted = true; frame.Continue = false; }
                else { model.Status = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English ? "Check the region coordinates." : "영역 좌표를 확인하세요."; }
            }
            else if (action == "whole") { presentation.WholeInline(); }
            else if (action == "add") { presentation.AddInline(); }
            else if (action == "delete") { presentation.DeleteInline(); }
            else if (action == "large")
            {
                using IPropertyGridImageEditView large = presentation.CreateLargeInlineEditor();
                if (large is Window window) { window.Owner = owner; window.SetBinding(OpenVisionWorkbenchThemeBehavior.ThemeNameProperty, new Binding(nameof(workbench.ThemeName)) { Source = workbench }); }
                if (large.ShowDialog()) { presentation.AcceptLargeInlineEditor(large); accepted = presentation.AcceptInline(); frame.Continue = !accepted; }
            }
        }

        private void CallerClosed(object sender, EventArgs args) => OnRequested("cancel");
        public void Dispose() { disposed = true; if (frame != null) { accepted = false; frame.Continue = false; } }

        private static void Move(FrameworkElement element, List<(FrameworkElement, DependencyObject, int, object)> moved, object context)
        {
            DependencyObject parent = element.Parent;
            int index = parent is Panel panel ? panel.Children.IndexOf(element) : 0;
            if (parent is not Panel && parent is not Decorator) { throw new InvalidOperationException("The editor viewport or coordinate field has no presentation owner."); }
            moved.Add((element, parent, index, element.ReadLocalValue(FrameworkElement.DataContextProperty)));
            if (parent is Panel originalPanel) { originalPanel.Children.Remove(element); } else { ((Decorator)parent).Child = null; }
            element.DataContext = context;
        }
        private static TextBlock Label(string text)
        {
            var label = new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4) }; label.SetResourceReference(TextBlock.ForegroundProperty, "OpenVision.Workbench.TextBrush"); return label;
        }
        private Button Button(string text, string command, bool primary = false)
        {
            var button = new Button { Content = text, Margin = new Thickness(3), Style = (Style)owner.FindResource(primary ? "OpenVision.Workbench.PrimaryButtonStyle" : "OpenVision.Workbench.ButtonStyle") };
            button.SetBinding(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, new Binding(command)); return button;
        }
        private static IEnumerable<T> Descendants<T>(DependencyObject value) where T : DependencyObject
        {
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(value); index++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(value, index); if (child is T match) { yield return match; }
                foreach (var descendant in Descendants<T>(child)) { yield return descendant; }
            }
        }
    }
}
