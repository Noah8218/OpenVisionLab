using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OpenVisionLab
{
    internal static class VisionToolControlBinding
    {
        // WPF bindings on tool parameter controls must be flushed in one shared way before creating OpenCV properties.
        public static void UpdateTextSource(TextBox textBox)
        {
            textBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        }

        public static void UpdateTextSources(params TextBox[] textBoxes)
        {
            if (textBoxes == null)
            {
                return;
            }

            foreach (TextBox textBox in textBoxes)
            {
                UpdateTextSource(textBox);
            }
        }

        public static void UpdateSelectionSource(Selector selector)
        {
            selector?.GetBindingExpression(Selector.SelectedItemProperty)?.UpdateSource();
        }

        public static void UpdateSelectionSources(params Selector[] selectors)
        {
            if (selectors == null)
            {
                return;
            }

            foreach (Selector selector in selectors)
            {
                UpdateSelectionSource(selector);
            }
        }

        public static void UpdateSliderSource(Slider slider)
        {
            slider?.GetBindingExpression(Slider.ValueProperty)?.UpdateSource();
        }

        public static void UpdateToggleSource(ToggleButton toggleButton)
        {
            toggleButton?.GetBindingExpression(ToggleButton.IsCheckedProperty)?.UpdateSource();
        }

        public static void UpdateToggleSources(params ToggleButton[] toggleButtons)
        {
            if (toggleButtons == null)
            {
                return;
            }

            foreach (ToggleButton toggleButton in toggleButtons)
            {
                UpdateToggleSource(toggleButton);
            }
        }

        public static void SetPanelVisible(FrameworkElement panel, bool visible, bool fadeWhenHidden = false)
        {
            if (panel == null)
            {
                return;
            }

            panel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            panel.IsEnabled = visible;
            if (fadeWhenHidden)
            {
                panel.Opacity = visible ? 1.0 : 0.0;
            }
        }

        public static void AllowUnsignedIntegerInput(TextCompositionEventArgs e)
        {
            if (e != null)
            {
                e.Handled = e.Text.Any(item => !char.IsDigit(item));
            }
        }

        public static void AllowSignedIntegerInput(TextCompositionEventArgs e)
        {
            if (e != null)
            {
                e.Handled = e.Text.Any(item => !char.IsDigit(item) && item != '-');
            }
        }
    }
}