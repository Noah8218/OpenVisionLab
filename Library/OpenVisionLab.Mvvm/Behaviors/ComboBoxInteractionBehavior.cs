using System.Windows;
using System.Windows.Controls;
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

        public static bool GetOpenOnFieldClick(DependencyObject target)
        {
            return (bool)target.GetValue(OpenOnFieldClickProperty);
        }

        public static void SetOpenOnFieldClick(DependencyObject target, bool value)
        {
            target.SetValue(OpenOnFieldClickProperty, value);
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