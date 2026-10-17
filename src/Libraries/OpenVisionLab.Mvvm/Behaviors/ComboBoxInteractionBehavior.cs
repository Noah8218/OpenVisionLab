using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OpenVisionLab.Mvvm.Behaviors
{
    /// <summary>
    /// ComboBox interaction rules shared by tool views as code-behind event plumbing moves to MVVM behaviors.
    /// </summary>
    public static class ComboBoxInteractionBehavior
    {
        public static readonly DependencyProperty OpenOnFieldClickProperty =
            DependencyProperty.RegisterAttached(
                "OpenOnFieldClick",
                typeof(bool),
                typeof(ComboBoxInteractionBehavior),
                new PropertyMetadata(false, OnOpenOnFieldClickChanged));

        public static readonly DependencyProperty CloseOnClickProperty =
            DependencyProperty.RegisterAttached(
                "CloseOnClick",
                typeof(ComboBox),
                typeof(ComboBoxInteractionBehavior),
                new PropertyMetadata(null, OnCloseOnClickChanged));

        public static bool GetOpenOnFieldClick(DependencyObject target)
        {
            return (bool)target.GetValue(OpenOnFieldClickProperty);
        }

        public static void SetOpenOnFieldClick(DependencyObject target, bool value)
        {
            target.SetValue(OpenOnFieldClickProperty, value);
        }

        public static ComboBox GetCloseOnClick(DependencyObject target) => (ComboBox)target.GetValue(CloseOnClickProperty);

        public static void SetCloseOnClick(DependencyObject target, ComboBox value) => target.SetValue(CloseOnClickProperty, value);

        private static void OnCloseOnClickChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!(target is ButtonBase button)) return;
            button.Click -= CloseSelectorOnClick;
            if (e.NewValue is ComboBox) button.Click += CloseSelectorOnClick;
        }

        private static void CloseSelectorOnClick(object sender, RoutedEventArgs e)
        {
            if (sender is ButtonBase button) GetCloseOnClick(button)?.SetCurrentValue(ComboBox.IsDropDownOpenProperty, false);
        }

        private static void OnOpenOnFieldClickChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!(target is ComboBox comboBox))
            {
                return;
            }

            comboBox.PreviewMouseLeftButtonDown -= ComboBox_PreviewMouseLeftButtonDown;
            if (e.NewValue is bool enabled && enabled)
            {
                comboBox.PreviewMouseLeftButtonDown += ComboBox_PreviewMouseLeftButtonDown;
            }
        }

        private static void ComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ComboBox comboBox)
                || comboBox.IsEditable
                || !comboBox.IsEnabled
                || comboBox.IsDropDownOpen)
            {
                return;
            }

            // Non-editable layer selectors should open from the whole field, not only the arrow chrome.
            comboBox.Focus();
            comboBox.IsDropDownOpen = true;
            e.Handled = true;
        }
    }
}