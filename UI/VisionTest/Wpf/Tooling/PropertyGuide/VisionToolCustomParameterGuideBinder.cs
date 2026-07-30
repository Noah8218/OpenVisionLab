using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class VisionToolCustomParameterGuideBinder : IDisposable
    {
        private readonly VisionToolSingleInputPropertyToolShell shell;
        private readonly Func<object> selectedObjectFactory;
        private readonly IReadOnlyDictionary<FrameworkElement, string> bindings;
        private readonly VisionToolParameterGuidePresenter presenter;
        private readonly VisionToolLanguageChangeController languageController;
        private string selectedPropertyName = string.Empty;
        private bool disposed;

        private VisionToolCustomParameterGuideBinder(
            VisionToolSingleInputPropertyToolShell shell,
            Func<object> selectedObjectFactory,
            IReadOnlyDictionary<FrameworkElement, string> bindings)
        {
            this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            this.selectedObjectFactory =
                selectedObjectFactory ?? throw new ArgumentNullException(nameof(selectedObjectFactory));
            this.bindings = bindings ?? throw new ArgumentNullException(nameof(bindings));
            presenter = new VisionToolParameterGuidePresenter(
                shell.ParameterGuide,
                selectedObjectFactory(),
                FocusProperty);
            languageController = VisionToolLanguageChangeController.Attach(Refresh);
            shell.ParameterGuideVisibility = Visibility.Visible;

            foreach (FrameworkElement element in bindings.Keys)
            {
                element.AddHandler(
                    Keyboard.GotKeyboardFocusEvent,
                    new KeyboardFocusChangedEventHandler(OnParameterSelected),
                    true);
                element.AddHandler(
                    Mouse.PreviewMouseDownEvent,
                    new MouseButtonEventHandler(OnParameterSelected),
                    true);
                element.AddHandler(
                    ButtonBase.ClickEvent,
                    new RoutedEventHandler(OnParameterValueChanged),
                    true);
                element.AddHandler(
                    System.Windows.Controls.TextBox.TextChangedEvent,
                    new System.Windows.Controls.TextChangedEventHandler(OnParameterValueChanged),
                    true);
                element.AddHandler(
                    System.Windows.Controls.Primitives.Selector.SelectionChangedEvent,
                    new System.Windows.Controls.SelectionChangedEventHandler(OnParameterValueChanged),
                    true);
                element.AddHandler(
                    ToggleButton.CheckedEvent,
                    new RoutedEventHandler(OnParameterValueChanged),
                    true);
                element.AddHandler(
                    ToggleButton.UncheckedEvent,
                    new RoutedEventHandler(OnParameterValueChanged),
                    true);
                element.AddHandler(
                    RangeBase.ValueChangedEvent,
                    new RoutedPropertyChangedEventHandler<double>(OnRangeValueChanged),
                    true);
            }
        }

        public static VisionToolCustomParameterGuideBinder Attach(
            VisionToolSingleInputPropertyToolShell shell,
            Func<object> selectedObjectFactory,
            IReadOnlyDictionary<FrameworkElement, string> bindings)
        {
            return new VisionToolCustomParameterGuideBinder(
                shell,
                selectedObjectFactory,
                bindings);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            languageController.Dispose();
            foreach (FrameworkElement element in bindings.Keys)
            {
                element.RemoveHandler(
                    Keyboard.GotKeyboardFocusEvent,
                    new KeyboardFocusChangedEventHandler(OnParameterSelected));
                element.RemoveHandler(
                    Mouse.PreviewMouseDownEvent,
                    new MouseButtonEventHandler(OnParameterSelected));
                element.RemoveHandler(
                    ButtonBase.ClickEvent,
                    new RoutedEventHandler(OnParameterValueChanged));
                element.RemoveHandler(
                    System.Windows.Controls.TextBox.TextChangedEvent,
                    new System.Windows.Controls.TextChangedEventHandler(OnParameterValueChanged));
                element.RemoveHandler(
                    System.Windows.Controls.Primitives.Selector.SelectionChangedEvent,
                    new System.Windows.Controls.SelectionChangedEventHandler(OnParameterValueChanged));
                element.RemoveHandler(
                    ToggleButton.CheckedEvent,
                    new RoutedEventHandler(OnParameterValueChanged));
                element.RemoveHandler(
                    ToggleButton.UncheckedEvent,
                    new RoutedEventHandler(OnParameterValueChanged));
                element.RemoveHandler(
                    RangeBase.ValueChangedEvent,
                    new RoutedPropertyChangedEventHandler<double>(OnRangeValueChanged));
            }
        }

        private void OnParameterSelected(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element
                && bindings.TryGetValue(element, out string propertyName))
            {
                SelectProperty(propertyName);
            }
        }

        private void OnParameterValueChanged(object sender, RoutedEventArgs e)
        {
            ScheduleRefresh();
        }

        private void OnRangeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScheduleRefresh();
        }

        private void SelectProperty(string propertyName)
        {
            if (disposed || string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            selectedPropertyName = propertyName;
            presenter.SelectObject(selectedObjectFactory());
            presenter.SelectProperty(propertyName);
        }

        private void ScheduleRefresh()
        {
            if (disposed || string.IsNullOrWhiteSpace(selectedPropertyName))
            {
                return;
            }

            shell.Dispatcher.BeginInvoke(
                new Action(Refresh),
                DispatcherPriority.DataBind);
        }

        private void Refresh()
        {
            if (disposed || string.IsNullOrWhiteSpace(selectedPropertyName))
            {
                return;
            }

            presenter.SelectObject(selectedObjectFactory());
            presenter.SelectProperty(selectedPropertyName);
        }

        private bool FocusProperty(string propertyName)
        {
            FrameworkElement element = bindings
                .Where(pair => string.Equals(
                    pair.Value,
                    propertyName,
                    StringComparison.Ordinal))
                .Select(pair => pair.Key)
                .FirstOrDefault(candidate => candidate.IsVisible && candidate.IsEnabled);
            if (element == null)
            {
                return false;
            }

            bool focused = element.Focus();
            if (focused)
            {
                SelectProperty(propertyName);
            }

            return focused;
        }
    }
}
