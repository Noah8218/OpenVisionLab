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
        public static readonly DependencyProperty SizeChangedCommandProperty =
            DependencyProperty.RegisterAttached("SizeChangedCommand", typeof(ICommand), typeof(InputCommandBehaviors), new PropertyMetadata(null, OnSizeChangedCommandChanged));

        public static readonly DependencyProperty DragCompletedCommandProperty =
            DependencyProperty.RegisterAttached("DragCompletedCommand", typeof(ICommand), typeof(InputCommandBehaviors), new PropertyMetadata(null, OnDragCompletedCommandChanged));

        public static ICommand GetSizeChangedCommand(DependencyObject target) => (ICommand)target.GetValue(SizeChangedCommandProperty);
        public static void SetSizeChangedCommand(DependencyObject target, ICommand value) => target.SetValue(SizeChangedCommandProperty, value);
        public static ICommand GetDragCompletedCommand(DependencyObject target) => (ICommand)target.GetValue(DragCompletedCommandProperty);
        public static void SetDragCompletedCommand(DependencyObject target, ICommand value) => target.SetValue(DragCompletedCommandProperty, value);

        private static void OnSizeChangedCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not FrameworkElement element) { return; }
            element.SizeChanged -= FrameworkElement_SizeChanged;
            if (e.NewValue != null) { element.SizeChanged += FrameworkElement_SizeChanged; }
        }

        private static void FrameworkElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is DependencyObject target) { Execute(GetSizeChangedCommand(target), e.NewSize); }
        }

        private static void OnDragCompletedCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not Thumb thumb) { return; }
            thumb.DragCompleted -= Thumb_DragCompleted;
            if (e.NewValue != null) { thumb.DragCompleted += Thumb_DragCompleted; }
        }

        private static void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is DependencyObject target && !e.Canceled) { Execute(GetDragCompletedCommand(target), null); }
        }

        public static readonly DependencyProperty PreviewPageNavigationProperty = DependencyProperty.RegisterAttached("PreviewPageNavigation", typeof(bool), typeof(InputCommandBehaviors), new PropertyMetadata(false, OnPreviewPageNavigationChanged));
        public static bool GetPreviewPageNavigation(DependencyObject target) => (bool)target.GetValue(PreviewPageNavigationProperty);
        public static void SetPreviewPageNavigation(DependencyObject target, bool value) => target.SetValue(PreviewPageNavigationProperty, value);

        private static void OnPreviewPageNavigationChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not UIElement element) { return; }
            element.PreviewKeyDown -= Element_PageNavigation;
            if ((bool)e.NewValue) { element.PreviewKeyDown += Element_PageNavigation; }
        }

        private static void Element_PageNavigation(object sender, KeyEventArgs e)
        {
            if (sender is not UIElement element || e.Handled || (e.Key != Key.PageUp && e.Key != Key.PageDown)
                || Keyboard.Modifiers != ModifierKeys.None || e.OriginalSource is TextBoxBase or PasswordBox or ComboBox
                || Keyboard.FocusedElement is TextBoxBase or PasswordBox or ComboBox) { return; }
            KeyBinding binding = element.InputBindings.OfType<KeyBinding>().FirstOrDefault(value => value.Gesture.Matches(element, e));
            if (binding == null) { return; }
            Execute(binding.Command, binding.CommandParameter);
            e.Handled = true;
        }

        public static readonly DependencyProperty FocusOnKeyProperty =
            DependencyProperty.RegisterAttached("FocusOnKey", typeof(Key), typeof(InputCommandBehaviors), new PropertyMetadata(Key.None, OnFocusTargetChanged));

        public static readonly DependencyProperty FocusTargetProperty =
            DependencyProperty.RegisterAttached("FocusTarget", typeof(UIElement), typeof(InputCommandBehaviors), new PropertyMetadata(null, OnFocusTargetChanged));

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

        public static readonly DependencyProperty MouseLeftButtonUpCommandProperty =
            DependencyProperty.RegisterAttached("MouseLeftButtonUpCommand", typeof(ICommand), typeof(InputCommandBehaviors), new PropertyMetadata(null, OnMouseLeftButtonUpCommandChanged));

        public static readonly DependencyProperty IncludeModifierKeysProperty = DependencyProperty.RegisterAttached("IncludeModifierKeys", typeof(bool), typeof(InputCommandBehaviors), new PropertyMetadata(false));
        public static bool GetIncludeModifierKeys(DependencyObject target) => (bool)target.GetValue(IncludeModifierKeysProperty);
        public static void SetIncludeModifierKeys(DependencyObject target, bool value) => target.SetValue(IncludeModifierKeysProperty, value);

        public static readonly DependencyProperty MouseLeftButtonUpCommandParameterProperty =
            DependencyProperty.RegisterAttached("MouseLeftButtonUpCommandParameter", typeof(object), typeof(InputCommandBehaviors), new PropertyMetadata(null));

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

        public static readonly DependencyProperty ScrollSelectedItemIntoViewProperty =
            DependencyProperty.RegisterAttached(
                "ScrollSelectedItemIntoView",
                typeof(bool),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(false, OnScrollSelectedItemIntoViewChanged));

        public static readonly DependencyProperty SelectItemOnRightClickProperty =
            DependencyProperty.RegisterAttached(
                "SelectItemOnRightClick",
                typeof(bool),
                typeof(InputCommandBehaviors),
                new PropertyMetadata(false, OnSelectItemOnRightClickChanged));

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

        public static ICommand GetMouseLeftButtonUpCommand(DependencyObject target) => (ICommand)target.GetValue(MouseLeftButtonUpCommandProperty);
        public static void SetMouseLeftButtonUpCommand(DependencyObject target, ICommand value) => target.SetValue(MouseLeftButtonUpCommandProperty, value);
        public static object GetMouseLeftButtonUpCommandParameter(DependencyObject target) => target.GetValue(MouseLeftButtonUpCommandParameterProperty);
        public static void SetMouseLeftButtonUpCommandParameter(DependencyObject target, object value) => target.SetValue(MouseLeftButtonUpCommandParameterProperty, value);

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

        public static bool GetScrollSelectedItemIntoView(DependencyObject target)
        {
            return (bool)target.GetValue(ScrollSelectedItemIntoViewProperty);
        }

        public static void SetScrollSelectedItemIntoView(DependencyObject target, bool value)
        {
            target.SetValue(ScrollSelectedItemIntoViewProperty, value);
        }

        public static bool GetSelectItemOnRightClick(DependencyObject target)
        {
            return (bool)target.GetValue(SelectItemOnRightClickProperty);
        }

        public static void SetSelectItemOnRightClick(DependencyObject target, bool value)
        {
            target.SetValue(SelectItemOnRightClickProperty, value);
        }

        public static Key GetFocusOnKey(DependencyObject target) => (Key)target.GetValue(FocusOnKeyProperty);

        public static void SetFocusOnKey(DependencyObject target, Key value) => target.SetValue(FocusOnKeyProperty, value);

        public static UIElement GetFocusTarget(DependencyObject target) => (UIElement)target.GetValue(FocusTargetProperty);

        public static void SetFocusTarget(DependencyObject target, UIElement value) => target.SetValue(FocusTargetProperty, value);

        private static void OnFocusTargetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is UIElement element)
            {
                element.KeyDown -= Element_FocusOnKey;
                if (GetFocusOnKey(target) != Key.None && GetFocusTarget(target) != null)
                {
                    element.KeyDown += Element_FocusOnKey;
                }
            }
        }

        private static void Element_FocusOnKey(object sender, KeyEventArgs e)
        {
            if (sender is not UIElement element || e.Key != GetFocusOnKey(element) || Keyboard.Modifiers != ModifierKeys.None)
            {
                return;
            }
            UIElement target = GetFocusTarget(element);
            if (target?.IsVisible != true || !target.IsEnabled || (target is FrameworkElement view && view.ActualWidth <= 0))
            {
                return;
            }
            if (target.Focus() || target.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)))
            {
                if (Keyboard.FocusedElement is TextBox textBox)
                {
                    textBox.SelectAll();
                }
                e.Handled = true;
            }
        }

        private static bool TrySelectListBoxItem(ListBox listBox, DependencyObject source)
        {
            if (listBox == null || source == null)
            {
                return false;
            }

            ListBoxItem item = source as ListBoxItem
                ?? ItemsControl.ContainerFromElement(listBox, source) as ListBoxItem;
            if (item == null)
            {
                return false;
            }

            int index = listBox.Items.IndexOf(item.DataContext);
            if (index < 0)
            {
                index = listBox.ItemContainerGenerator.IndexFromContainer(item);
            }

            if (index < 0)
            {
                return false;
            }

            listBox.SelectedIndex = index;
            item.Focus();
            return true;
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

        private static void OnMouseLeftButtonUpCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not UIElement element) { return; }
            element.MouseLeftButtonUp -= Element_MouseLeftButtonUp;
            if (e.NewValue != null) { element.MouseLeftButtonUp += Element_MouseLeftButtonUp; }
        }

        private static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DependencyObject target) { return; }
            object parameter = GetMouseLeftButtonUpCommandParameter(target);
            if (GetIncludeModifierKeys(target))
            {
                parameter = Tuple.Create(parameter, Keyboard.Modifiers.HasFlag(ModifierKeys.Control), Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
                (sender as UIElement)?.Focus();
            }
            Execute(GetMouseLeftButtonUpCommand(target), parameter);
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

        private static void OnScrollSelectedItemIntoViewChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is DataGrid grid)
            {
                grid.SelectionChanged -= DataGrid_ScrollSelectedItemIntoView;
                grid.Loaded -= DataGrid_ScrollSelectedItemIntoView;
                grid.SizeChanged -= DataGrid_ScrollSelectedItemIntoView;
                if ((bool)e.NewValue) { grid.SelectionChanged += DataGrid_ScrollSelectedItemIntoView; grid.Loaded += DataGrid_ScrollSelectedItemIntoView; grid.SizeChanged += DataGrid_ScrollSelectedItemIntoView; }
                return;
            }
            if (target is not ListBox listBox)
            {
                return;
            }

            listBox.SelectionChanged -= ListBox_ScrollSelectedItemIntoViewSelectionChanged;
            listBox.Loaded -= ListBox_ScrollSelectedItemIntoViewLoaded;
            if ((bool)e.NewValue)
            {
                listBox.SelectionChanged += ListBox_ScrollSelectedItemIntoViewSelectionChanged;
                listBox.Loaded += ListBox_ScrollSelectedItemIntoViewLoaded;
                ScrollSelectedItemIntoView(listBox);
            }
        }

        private static void DataGrid_ScrollSelectedItemIntoView(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                grid.Dispatcher.BeginInvoke(new Action(() => { if (grid.SelectedItem != null) { grid.ScrollIntoView(grid.SelectedItem); } }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private static void ListBox_ScrollSelectedItemIntoViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                ScrollSelectedItemIntoView(listBox);
            }
        }

        private static void ListBox_ScrollSelectedItemIntoViewLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                ScrollSelectedItemIntoView(listBox);
            }
        }

        private static void ScrollSelectedItemIntoView(ListBox listBox)
        {
            if (listBox.SelectedItem == null)
            {
                return;
            }

            listBox.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    if (listBox.SelectedItem != null)
                    {
                        listBox.ScrollIntoView(listBox.SelectedItem);
                    }
                }),
                System.Windows.Threading.DispatcherPriority.Background);
        }

        private static void OnSelectItemOnRightClickChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is not ListBox listBox)
            {
                return;
            }

            listBox.PreviewMouseDown -= ListBox_PreviewMouseDown;
            if ((bool)e.NewValue)
            {
                listBox.PreviewMouseDown += ListBox_PreviewMouseDown;
            }
        }

        private static void ListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox
                && e.ChangedButton == MouseButton.Right
                && e.OriginalSource is DependencyObject source)
            {
                TrySelectListBoxItem(listBox, source);
            }
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
