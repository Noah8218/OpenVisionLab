using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab
{
    public enum VisionToolTextInputMode
    {
        None,
        UnsignedInteger,
        SignedInteger
    }

    public static class VisionToolTextInputBehavior
    {
        public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached(
            "Mode",
            typeof(VisionToolTextInputMode),
            typeof(VisionToolTextInputBehavior),
            new PropertyMetadata(VisionToolTextInputMode.None, OnModeChanged));

        public static VisionToolTextInputMode GetMode(DependencyObject element)
        {
            return element is null ? VisionToolTextInputMode.None : (VisionToolTextInputMode)element.GetValue(ModeProperty);
        }

        public static void SetMode(DependencyObject element, VisionToolTextInputMode value)
        {
            element?.SetValue(ModeProperty, value);
        }

        private static void OnModeChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is not TextBox textBox)
            {
                return;
            }

            textBox.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(textBox, OnPaste);

            if ((VisionToolTextInputMode)e.NewValue == VisionToolTextInputMode.None)
            {
                return;
            }

            // Numeric edit policy is a reusable View behavior, not a per-tool View code-behind concern.
            textBox.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(textBox, OnPaste);
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            switch (GetMode(sender as DependencyObject))
            {
                case VisionToolTextInputMode.UnsignedInteger:
                    VisionToolControlBinding.AllowUnsignedIntegerInput(e);
                    break;
                case VisionToolTextInputMode.SignedInteger:
                    VisionToolControlBinding.AllowSignedIntegerInput(e);
                    break;
            }
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            string text = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
            if (!IsAllowed(GetMode(sender as DependencyObject), text))
            {
                e.CancelCommand();
            }
        }

        private static bool IsAllowed(VisionToolTextInputMode mode, string text)
        {
            foreach (char item in text)
            {
                if (mode == VisionToolTextInputMode.UnsignedInteger && !char.IsDigit(item))
                {
                    return false;
                }

                if (mode == VisionToolTextInputMode.SignedInteger && !char.IsDigit(item) && item != '-')
                {
                    return false;
                }
            }

            return true;
        }
    }
}