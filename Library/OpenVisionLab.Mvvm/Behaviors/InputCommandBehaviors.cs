using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OpenVisionLab.Mvvm.Behaviors
{
    public enum TextInputFilterMode
    {
        None,
        Integer,
        Decimal
    }

    /// <summary>
    /// Small WPF behaviors for view-only input wiring so panels can bind commands without code-behind event relays.
    /// </summary>
    public static class InputCommandBehaviors
    {
        public static readonly DependencyProperty SelectionChangedCommandProperty =
            DependencyProperty.RegisterAttached(
                "SelectionChangedCommand",
                typeof(ICommand),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(null, OnSelectionChangedCommandChanged));

        public static readonly DependencyProperty PreviewKeyDownCommandProperty =
            DependencyProperty.RegisterAttached(
                "PreviewKeyDownCommand",
                typeof(ICommand),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(null, OnPreviewKeyDownCommandChanged));

        public static readonly DependencyProperty TextChangedCommandProperty =
            DependencyProperty.RegisterAttached(
                "TextChangedCommand",
                typeof(ICommand),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(null, OnTextChangedCommandChanged));

        public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseDoubleClickCommand",
                typeof(ICommand),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(null, OnMouseDoubleClickCommandChanged));

        public static readonly DependencyProperty ValueChangedCommandProperty =
            DependencyProperty.RegisterAttached(
                "ValueChangedCommand",
                typeof(ICommand),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(null, OnValueChangedCommandChanged));

        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.RegisterAttached(
                "LoadedCommand",
                typeof(ICommand),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(null, OnLoadedCommandChanged));

        public static readonly DependencyProperty UnloadedCommandProperty =
            DependencyProperty.RegisterAttached(
                "UnloadedCommand",
                typeof(ICommand),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(null, OnUnloadedCommandChanged));

        public static readonly DependencyProperty TextInputFilterProperty =
            DependencyProperty.RegisterAttached(
                "TextInputFilter",
                typeof(TextInputFilterMode),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(TextInputFilterMode.None, OnTextInputFilterChanged));

        public static ICommand GetSelectionChangedCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(SelectionChangedCommandProperty);
        }

        public static void SetSelectionChangedCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(SelectionChangedCommandProperty, value);
        }

        public static ICommand GetPreviewKeyDownCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(PreviewKeyDownCommandProperty);
        }

        public static void SetPreviewKeyDownCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(PreviewKeyDownCommandProperty, value);
        }

        public static ICommand GetTextChangedCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(TextChangedCommandProperty);
        }

        public static void SetTextChangedCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(TextChangedCommandProperty, value);
        }

        public static ICommand GetMouseDoubleClickCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(MouseDoubleClickCommandProperty);
        }

        public static void SetMouseDoubleClickCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(MouseDoubleClickCommandProperty, value);
        }

        public static ICommand GetValueChangedCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(ValueChangedCommandProperty);
        }

        public static void SetValueChangedCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(ValueChangedCommandProperty, value);
        }

        public static ICommand GetLoadedCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(LoadedCommandProperty);
        }

        public static void SetLoadedCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(LoadedCommandProperty, value);
        }

        public static ICommand GetUnloadedCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(UnloadedCommandProperty);
        }

        public static void SetUnloadedCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(UnloadedCommandProperty, value);
        }

        public static TextInputFilterMode GetTextInputFilter(DependencyObject target)
        {
            return (TextInputFilterMode)target.GetValue(TextInputFilterProperty);
        }

        public static void SetTextInputFilter(DependencyObject target, TextInputFilterMode value)
        {
            target.SetValue(TextInputFilterProperty, value);
        }

        private static void OnSelectionChangedCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not Selector selector)
            {
                return;
            }

            selector.SelectionChanged -= Selector_SelectionChanged;
            if (e.NewValue != null)
            {
                selector.SelectionChanged += Selector_SelectionChanged;
            }
        }

        private static void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not DependencyObject target)
            {
                return;
            }

            Execute(GetSelectionChangedCommand(target), e);
        }

        private static void OnPreviewKeyDownCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not UIElement element)
            {
                return;
            }

            element.PreviewKeyDown -= Element_PreviewKeyDown;
            if (e.NewValue != null)
            {
                element.PreviewKeyDown += Element_PreviewKeyDown;
            }
        }

        private static void Element_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not DependencyObject target)
            {
                return;
            }

            Execute(GetPreviewKeyDownCommand(target), e);
        }

        private static void OnTextChangedCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not TextBox textBox)
            {
                return;
            }

            textBox.TextChanged -= TextBox_TextChanged;
            if (e.NewValue != null)
            {
                textBox.TextChanged += TextBox_TextChanged;
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not DependencyObject target)
            {
                return;
            }

            Execute(GetTextChangedCommand(target), e);
        }

        private static void OnMouseDoubleClickCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not Control control)
            {
                return;
            }

            control.MouseDoubleClick -= Control_MouseDoubleClick;
            if (e.NewValue != null)
            {
                control.MouseDoubleClick += Control_MouseDoubleClick;
            }
        }

        private static void Control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DependencyObject target)
            {
                return;
            }

            Execute(GetMouseDoubleClickCommand(target), e);
        }

        private static void OnValueChangedCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not RangeBase rangeBase)
            {
                return;
            }

            rangeBase.ValueChanged -= RangeBase_ValueChanged;
            if (e.NewValue != null)
            {
                rangeBase.ValueChanged += RangeBase_ValueChanged;
            }
        }

        private static void RangeBase_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is not DependencyObject target)
            {
                return;
            }

            Execute(GetValueChangedCommand(target), e);
        }

        private static void OnLoadedCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not FrameworkElement element)
            {
                return;
            }

            element.Loaded -= FrameworkElement_Loaded;
            if (e.NewValue != null)
            {
                element.Loaded += FrameworkElement_Loaded;
            }
        }

        private static void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not DependencyObject target)
            {
                return;
            }

            Execute(GetLoadedCommand(target), e);
        }

        private static void OnUnloadedCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not FrameworkElement element)
            {
                return;
            }

            element.Unloaded -= FrameworkElement_Unloaded;
            if (e.NewValue != null)
            {
                element.Unloaded += FrameworkElement_Unloaded;
            }
        }

        private static void FrameworkElement_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is not DependencyObject target)
            {
                return;
            }

            Execute(GetUnloadedCommand(target), e);
        }

        private static void Execute(ICommand command, object parameter)
        {
            if (command?.CanExecute(parameter) == true)
            {
                command.Execute(parameter);
            }
        }

        private static void OnTextInputFilterChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not TextBox textBox)
            {
                return;
            }

            textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            DataObject.RemovePastingHandler(textBox, TextBox_Pasting);
            if ((TextInputFilterMode)e.NewValue != TextInputFilterMode.None)
            {
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
                DataObject.AddPastingHandler(textBox, TextBox_Pasting);
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = sender is not TextBox textBox || !IsAcceptedInput(textBox, e.Text);
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox textBox || !e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            string pastedText = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
            if (!IsAcceptedInput(textBox, pastedText))
            {
                e.CancelCommand();
            }
        }

        private static bool IsAcceptedInput(TextBox textBox, string input)
        {
            string proposed = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, input ?? string.Empty);

            return GetTextInputFilter(textBox) switch
            {
                TextInputFilterMode.Integer => proposed.All(char.IsDigit),
                TextInputFilterMode.Decimal => proposed.Count(ch => ch == '.') <= 1
                    && proposed.All(ch => char.IsDigit(ch) || ch == '.'),
                _ => true
            };
        }
    }
}
