using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class SimplePreprocessParameterController
    {
        // Runtime-generated preprocess parameters share one registry and sync path so each tool does not reimplement UI state rules.
        private readonly Dictionary<string, ComboBox> choiceControls = new Dictionary<string, ComboBox>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, TextBox> numberControls = new Dictionary<string, TextBox>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, CheckBox> checkControls = new Dictionary<string, CheckBox>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Slider> sliderControls = new Dictionary<string, Slider>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FrameworkElement> parameterRows = new Dictionary<string, FrameworkElement>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, NumberOptions> numberOptions = new Dictionary<string, NumberOptions>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, TextBlock> parameterLabels = new Dictionary<string, TextBlock>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, LocalizedLabel> parameterLabelTexts = new Dictionary<string, LocalizedLabel>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, LocalizedLabel> checkLabelTexts = new Dictionary<string, LocalizedLabel>(StringComparer.OrdinalIgnoreCase);
        private readonly Panel parameterPanel;
        private readonly FrameworkElement resourceOwner;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly Func<bool> isSuppressed;
        private readonly Action<bool> setSuppressed;

        public SimplePreprocessParameterController(
            Panel parameterPanel,
            FrameworkElement resourceOwner,
            VisionToolParameterChangeController parameterChangeController,
            Func<bool> isSuppressed,
            Action<bool> setSuppressed)
        {
            this.parameterPanel = parameterPanel ?? throw new ArgumentNullException(nameof(parameterPanel));
            this.resourceOwner = resourceOwner ?? throw new ArgumentNullException(nameof(resourceOwner));
            this.parameterChangeController = parameterChangeController ?? throw new ArgumentNullException(nameof(parameterChangeController));
            this.isSuppressed = isSuppressed ?? throw new ArgumentNullException(nameof(isSuppressed));
            this.setSuppressed = setSuppressed ?? throw new ArgumentNullException(nameof(setSuppressed));
        }

        public void Clear()
        {
            parameterPanel.Children.Clear();
            choiceControls.Clear();
            numberControls.Clear();
            checkControls.Clear();
            sliderControls.Clear();
            parameterRows.Clear();
            numberOptions.Clear();
            parameterLabels.Clear();
            parameterLabelTexts.Clear();
            checkLabelTexts.Clear();
        }

        public ComboBox AddChoice(string key, string label, IEnumerable<object> values, object selectedValue, string labelLocalizationKey = null)
        {
            ComboBox comboBox = new ComboBox
            {
                Name = "cb" + key,
                ItemsSource = values?.ToList() ?? new List<object>()
            };
            comboBox.SelectionChanged += Parameter_Changed;
            Border row = CreateParameterRow(key, label, comboBox, labelLocalizationKey);
            parameterPanel.Children.Add(row);
            choiceControls[key] = comboBox;
            parameterRows[key] = row;
            SelectComboValue(comboBox, selectedValue);
            return comboBox;
        }

        public TextBox AddNumber(string key, string label, double value, double minimum, double maximum, bool allowDecimal, bool allowNegative, string labelLocalizationKey = null)
        {
            TextBox textBox = new TextBox
            {
                Name = "txt" + key,
                Text = FormatNumber(value)
            };
            textBox.PreviewTextInput += NumberTextBox_PreviewTextInput;
            textBox.TextChanged += NumberTextBox_TextChanged;
            Border row = CreateParameterRow(key, label, textBox, labelLocalizationKey);
            parameterPanel.Children.Add(row);
            numberControls[key] = textBox;
            parameterRows[key] = row;
            numberOptions[key] = new NumberOptions(minimum, maximum, allowDecimal, allowNegative);
            return textBox;
        }

        public void AddSlider(string key, string label, double minimum, double maximum, double value, double tickFrequency, string labelLocalizationKey = null)
        {
            double clampedValue = Math.Max(minimum, Math.Min(maximum, value));
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Grid header = new Grid();
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            TextBlock title = new TextBlock
            {
                Name = "lbl" + key,
                Text = ResolveText(labelLocalizationKey, label),
                Style = (Style)resourceOwner.FindResource("SectionTitleStyle"),
                VerticalAlignment = VerticalAlignment.Center
            };
            RegisterParameterLabel(key, title, label, labelLocalizationKey);

            TextBox textBox = new TextBox
            {
                Name = "txt" + key,
                Text = FormatNumber(clampedValue),
                Width = 76,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            textBox.PreviewTextInput += NumberTextBox_PreviewTextInput;
            textBox.TextChanged += NumberTextBox_TextChanged;

            Grid.SetColumn(title, 0);
            Grid.SetColumn(textBox, 1);
            header.Children.Add(title);
            header.Children.Add(textBox);

            Slider slider = new Slider
            {
                Name = "slider" + key,
                Minimum = minimum,
                Maximum = maximum,
                Value = clampedValue,
                TickFrequency = tickFrequency,
                IsSnapToTickEnabled = tickFrequency >= 1d,
                IsMoveToPointEnabled = true
            };
            slider.ValueChanged += Slider_ValueChanged;

            Grid rangeText = new Grid();
            rangeText.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rangeText.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            TextBlock minimumText = new TextBlock
            {
                Text = FormatNumber(minimum),
                FontSize = 11,
                Foreground = FindBrush("VisionTool.SecondaryTextBrush"),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            TextBlock maximumText = new TextBlock
            {
                Text = FormatNumber(maximum),
                FontSize = 11,
                Foreground = FindBrush("VisionTool.SecondaryTextBrush"),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(minimumText, 0);
            Grid.SetColumn(maximumText, 1);
            rangeText.Children.Add(minimumText);
            rangeText.Children.Add(maximumText);

            Grid.SetRow(header, 0);
            Grid.SetRow(slider, 2);
            Grid.SetRow(rangeText, 4);
            grid.Children.Add(header);
            grid.Children.Add(slider);
            grid.Children.Add(rangeText);

            Border row = CreateParameterContainer();
            row.Child = grid;
            parameterPanel.Children.Add(row);

            numberControls[key] = textBox;
            sliderControls[key] = slider;
            parameterRows[key] = row;
            numberOptions[key] = new NumberOptions(minimum, maximum, true, minimum < 0d);
        }

        public void AddRangeSliderPair(
            string groupKey,
            string groupLabel,
            string minKey,
            string minLabel,
            string maxKey,
            string maxLabel,
            double minimum,
            double maximum,
            double minValue,
            double maxValue,
            double tickFrequency,
            string groupLocalizationKey = null,
            string minLabelLocalizationKey = null,
            string maxLabelLocalizationKey = null)
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Grid header = new Grid();
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock title = new TextBlock
            {
                Name = "lbl" + groupKey,
                Text = ResolveText(groupLocalizationKey, groupLabel),
                Style = (Style)resourceOwner.FindResource("SectionTitleStyle"),
                VerticalAlignment = VerticalAlignment.Center
            };
            RegisterParameterLabel(groupKey, title, groupLabel, groupLocalizationKey);

            TextBlock domain = new TextBlock
            {
                Text = FormatNumber(minimum) + " - " + FormatNumber(maximum),
                FontSize = 11,
                Foreground = FindBrush("VisionTool.SecondaryTextBrush"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(title, 0);
            Grid.SetColumn(domain, 1);
            header.Children.Add(title);
            header.Children.Add(domain);

            Grid minRow = CreateRangeSliderRow(minKey, minLabel, minimum, maximum, minValue, tickFrequency, minLabelLocalizationKey);
            Grid maxRow = CreateRangeSliderRow(maxKey, maxLabel, minimum, maximum, maxValue, tickFrequency, maxLabelLocalizationKey);

            Grid.SetRow(header, 0);
            Grid.SetRow(minRow, 2);
            Grid.SetRow(maxRow, 4);
            grid.Children.Add(header);
            grid.Children.Add(minRow);
            grid.Children.Add(maxRow);

            Border row = CreateParameterContainer();
            row.Padding = new Thickness(0, 0, 0, 3);
            row.Margin = new Thickness(0, 0, 0, 3);
            row.Child = grid;
            parameterPanel.Children.Add(row);
            parameterRows[groupKey] = row;
            parameterRows[minKey] = row;
            parameterRows[maxKey] = row;
        }

        public CheckBox AddCheck(string key, string label, bool isChecked, string labelLocalizationKey = null)
        {
            CheckBox checkBox = new CheckBox
            {
                Name = "chk" + key,
                Content = ResolveText(labelLocalizationKey, label),
                IsChecked = isChecked
            };
            checkBox.Checked += Parameter_Changed;
            checkBox.Unchecked += Parameter_Changed;
            RegisterCheckLabel(key, label, labelLocalizationKey);
            Border row = CreateParameterRow(string.Empty, string.Empty, checkBox);
            parameterPanel.Children.Add(row);
            checkControls[key] = checkBox;
            parameterRows[key] = row;
            return checkBox;
        }

        public void SetParameterVisible(string key, bool visible)
        {
            if (string.IsNullOrWhiteSpace(key) || !parameterRows.TryGetValue(key, out FrameworkElement row))
            {
                return;
            }

            row.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetParametersVisible(IEnumerable<string> keys, bool visible)
        {
            if (keys == null)
            {
                return;
            }

            foreach (string key in keys)
            {
                SetParameterVisible(key, visible);
            }
        }

        public IReadOnlyDictionary<FrameworkElement, string> CreateParameterGuideBindings(
            IEnumerable<string> propertyNames)
        {
            Dictionary<FrameworkElement, string> bindings =
                new Dictionary<FrameworkElement, string>();
            if (propertyNames == null)
            {
                return bindings;
            }

            foreach (string propertyName in propertyNames.Where(name => !string.IsNullOrWhiteSpace(name)))
            {
                AddParameterGuideBinding(bindings, choiceControls, propertyName);
                AddParameterGuideBinding(bindings, numberControls, propertyName);
                AddParameterGuideBinding(bindings, checkControls, propertyName);
                AddParameterGuideBinding(bindings, sliderControls, propertyName);
            }

            return bindings;
        }

        public IReadOnlyDictionary<FrameworkElement, string> CreateParameterGuideBindings(
            IReadOnlyDictionary<string, string> controlPropertyNames)
        {
            Dictionary<FrameworkElement, string> bindings =
                new Dictionary<FrameworkElement, string>();
            if (controlPropertyNames == null)
            {
                return bindings;
            }

            foreach (KeyValuePair<string, string> binding in controlPropertyNames)
            {
                if (string.IsNullOrWhiteSpace(binding.Key)
                    || string.IsNullOrWhiteSpace(binding.Value))
                {
                    continue;
                }

                AddParameterGuideBinding(bindings, choiceControls, binding.Key, binding.Value);
                AddParameterGuideBinding(bindings, numberControls, binding.Key, binding.Value);
                AddParameterGuideBinding(bindings, checkControls, binding.Key, binding.Value);
                AddParameterGuideBinding(bindings, sliderControls, binding.Key, binding.Value);
            }

            return bindings;
        }

        public T GetEnum<T>(string key, T fallback)
            where T : struct
        {
            if (!choiceControls.TryGetValue(key, out ComboBox comboBox))
            {
                return fallback;
            }

            object selected = comboBox.SelectedItem;
            if (selected is T typed)
            {
                return typed;
            }

            string text = Convert.ToString(selected, CultureInfo.InvariantCulture) ?? comboBox.Text;
            return Enum.TryParse(text, true, out T parsed) ? parsed : fallback;
        }

        public string GetChoiceText(string key, string fallback)
        {
            if (!choiceControls.TryGetValue(key, out ComboBox comboBox))
            {
                return fallback;
            }

            string text = GetComboText(comboBox);
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }

        public int GetInt(string key, int fallback)
        {
            return (int)Math.Round(GetDouble(key, fallback));
        }

        public double GetDouble(string key, double fallback)
        {
            if (!numberControls.TryGetValue(key, out TextBox textBox))
            {
                return fallback;
            }

            if (!double.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                && !double.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
            {
                return fallback;
            }

            if (numberOptions.TryGetValue(key, out NumberOptions options))
            {
                value = Math.Max(options.Minimum, Math.Min(options.Maximum, value));
            }

            return value;
        }

        public bool GetBool(string key, bool fallback)
        {
            if (!checkControls.TryGetValue(key, out CheckBox checkBox))
            {
                return fallback;
            }

            return checkBox.IsChecked ?? fallback;
        }

        public SimplePreprocessToolSettings CaptureSettings()
        {
            SimplePreprocessToolSettings settings = new SimplePreprocessToolSettings();

            foreach (KeyValuePair<string, ComboBox> item in choiceControls)
            {
                settings.Parameters.Add(new ToolParameterValue
                {
                    Key = item.Key,
                    Value = GetComboText(item.Value)
                });
            }

            foreach (KeyValuePair<string, TextBox> item in numberControls)
            {
                settings.Parameters.Add(new ToolParameterValue
                {
                    Key = item.Key,
                    Value = item.Value.Text ?? string.Empty
                });
            }

            foreach (KeyValuePair<string, CheckBox> item in checkControls)
            {
                settings.Parameters.Add(new ToolParameterValue
                {
                    Key = item.Key,
                    Value = (item.Value.IsChecked == true).ToString()
                });
            }

            return settings;
        }

        public void ApplySettings(SimplePreprocessToolSettings settings)
        {
            if (settings?.Parameters != null && settings.Parameters.Count > 0)
            {
                bool wasSuppressed = isSuppressed();
                setSuppressed(true);
                try
                {
                    foreach (ToolParameterValue item in settings.Parameters)
                    {
                        if (item == null || string.IsNullOrWhiteSpace(item.Key))
                        {
                            continue;
                        }

                        if (choiceControls.TryGetValue(item.Key, out ComboBox comboBox))
                        {
                            SelectComboValue(comboBox, item.Value);
                        }

                        if (numberControls.TryGetValue(item.Key, out TextBox textBox))
                        {
                            textBox.Text = item.Value ?? string.Empty;
                        }

                        if (sliderControls.TryGetValue(item.Key, out Slider slider)
                            && double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double sliderValue))
                        {
                            slider.Value = Math.Max(slider.Minimum, Math.Min(slider.Maximum, sliderValue));
                        }

                        if (checkControls.TryGetValue(item.Key, out CheckBox checkBox)
                            && bool.TryParse(item.Value, out bool checkedValue))
                        {
                            checkBox.IsChecked = checkedValue;
                        }
                    }
                }
                finally
                {
                    setSuppressed(wasSuppressed);
                }
            }

            parameterChangeController.RefreshProgrammatic(notifyChanged: true);
        }

        public void RefreshLabels()
        {
            foreach (KeyValuePair<string, TextBlock> item in parameterLabels)
            {
                if (parameterLabelTexts.TryGetValue(item.Key, out LocalizedLabel label))
                {
                    item.Value.Text = ResolveText(label.LocalizationKey, label.FallbackText);
                }
            }

            foreach (KeyValuePair<string, LocalizedLabel> item in checkLabelTexts)
            {
                if (checkControls.TryGetValue(item.Key, out CheckBox checkBox))
                {
                    checkBox.Content = ResolveText(item.Value.LocalizationKey, item.Value.FallbackText);
                }
            }
        }

        private Grid CreateRangeSliderRow(string key, string label, double minimum, double maximum, double value, double tickFrequency, string labelLocalizationKey)
        {
            double clampedValue = Math.Max(minimum, Math.Min(maximum, value));
            Grid row = new Grid();
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(118) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(76) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock title = new TextBlock
            {
                Name = "lbl" + key,
                Text = ResolveText(labelLocalizationKey, label),
                Foreground = FindBrush("VisionTool.PrimaryTextBrush"),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            RegisterParameterLabel(key, title, label, labelLocalizationKey);

            TextBox textBox = new TextBox
            {
                Name = "txt" + key,
                Text = FormatNumber(clampedValue),
                Width = 72,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };
            textBox.PreviewTextInput += NumberTextBox_PreviewTextInput;
            textBox.TextChanged += NumberTextBox_TextChanged;

            Slider slider = new Slider
            {
                Name = "slider" + key,
                Minimum = minimum,
                Maximum = maximum,
                Value = clampedValue,
                TickFrequency = tickFrequency,
                IsSnapToTickEnabled = tickFrequency >= 1d,
                IsMoveToPointEnabled = true
            };
            slider.ValueChanged += Slider_ValueChanged;

            Grid.SetColumn(title, 0);
            Grid.SetColumn(textBox, 1);
            Grid.SetColumn(slider, 3);
            row.Children.Add(title);
            row.Children.Add(textBox);
            row.Children.Add(slider);

            numberControls[key] = textBox;
            sliderControls[key] = slider;
            numberOptions[key] = new NumberOptions(minimum, maximum, true, minimum < 0d);

            return row;
        }

        private void Parameter_Changed(object sender, EventArgs e)
        {
            parameterChangeController.TryHandle(notifyChanged: true, schedulePreview: true);
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(sender is TextBox textBox) || !TryFindNumberOptions(textBox, out NumberOptions options))
            {
                return;
            }

            e.Handled = e.Text.Any(ch => !IsAllowedNumberCharacter(ch, options));
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isSuppressed() || !(sender is TextBox textBox))
            {
                return;
            }

            string key = FindKey(numberControls, textBox);
            if (!string.IsNullOrWhiteSpace(key)
                && sliderControls.TryGetValue(key, out Slider slider)
                && double.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                setSuppressed(true);
                try
                {
                    slider.Value = Math.Max(slider.Minimum, Math.Min(slider.Maximum, value));
                }
                finally
                {
                    setSuppressed(false);
                }
            }

            parameterChangeController.RefreshProgrammatic(notifyChanged: true, schedulePreview: true);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isSuppressed() || !(sender is Slider slider))
            {
                return;
            }

            string key = FindKey(sliderControls, slider);
            if (string.IsNullOrWhiteSpace(key) || !numberControls.TryGetValue(key, out TextBox textBox))
            {
                return;
            }

            setSuppressed(true);
            try
            {
                textBox.Text = FormatNumber(slider.Value);
            }
            finally
            {
                setSuppressed(false);
            }

            parameterChangeController.RefreshProgrammatic(notifyChanged: true, schedulePreview: true);
        }

        private Border CreateParameterContainer()
        {
            return new Border
            {
                Background = Brushes.Transparent,
                BorderBrush = FindBrush("VisionTool.HeaderBorderBrush", new SolidColorBrush(Color.FromRgb(215, 226, 237))),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(0, 0, 0, 6),
                Margin = new Thickness(0, 0, 0, 6)
            };
        }

        private Border CreateParameterRow(string key, string label, FrameworkElement editor, string labelLocalizationKey = null)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(145) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            if (!string.IsNullOrWhiteSpace(label))
            {
                TextBlock title = new TextBlock
                {
                    Name = "lbl" + key,
                    Text = ResolveText(labelLocalizationKey, label),
                    Style = (Style)resourceOwner.FindResource("SectionTitleStyle"),
                    VerticalAlignment = VerticalAlignment.Center
                };
                RegisterParameterLabel(key, title, label, labelLocalizationKey);
                Grid.SetColumn(title, 0);
                grid.Children.Add(title);
            }

            editor.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetColumn(editor, 1);
            grid.Children.Add(editor);

            Border row = CreateParameterContainer();
            row.Child = grid;
            return row;
        }

        private void RegisterParameterLabel(string key, TextBlock textBlock, string fallbackText, string localizationKey)
        {
            if (string.IsNullOrWhiteSpace(key) || textBlock == null)
            {
                return;
            }

            parameterLabels[key] = textBlock;
            parameterLabelTexts[key] = new LocalizedLabel(localizationKey, fallbackText);
        }

        private void RegisterCheckLabel(string key, string fallbackText, string localizationKey)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            checkLabelTexts[key] = new LocalizedLabel(localizationKey, fallbackText);
        }

        private static string ResolveText(string localizationKey, string fallbackText)
        {
            if (string.IsNullOrWhiteSpace(localizationKey))
            {
                return fallbackText ?? string.Empty;
            }

            string value = OpenVisionLanguageService.T(localizationKey);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, localizationKey, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string GetComboText(ComboBox comboBox)
        {
            return Convert.ToString(comboBox.SelectedItem, CultureInfo.InvariantCulture) ?? comboBox.Text ?? string.Empty;
        }

        private static void SelectComboValue(ComboBox comboBox, object selectedValue)
        {
            if (comboBox == null)
            {
                return;
            }

            object match = comboBox.Items.Cast<object>()
                .FirstOrDefault(item => Equals(item, selectedValue)
                    || string.Equals(Convert.ToString(item, CultureInfo.InvariantCulture), Convert.ToString(selectedValue, CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase));
            comboBox.SelectedItem = match ?? selectedValue;
        }

        private static string FormatNumber(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static bool IsAllowedNumberCharacter(char ch, NumberOptions options)
        {
            if (char.IsDigit(ch))
            {
                return true;
            }

            if (options.AllowDecimal && (ch == '.' || ch == ','))
            {
                return true;
            }

            return options.AllowNegative && ch == '-';
        }

        private bool TryFindNumberOptions(TextBox textBox, out NumberOptions options)
        {
            string key = FindKey(numberControls, textBox);
            return numberOptions.TryGetValue(key ?? string.Empty, out options);
        }

        private Brush FindBrush(string resourceKey, Brush fallback = null)
        {
            return resourceOwner.TryFindResource(resourceKey) as Brush ?? fallback ?? Brushes.Transparent;
        }

        private static string FindKey<T>(Dictionary<string, T> dictionary, T value)
            where T : class
        {
            foreach (KeyValuePair<string, T> item in dictionary)
            {
                if (ReferenceEquals(item.Value, value))
                {
                    return item.Key;
                }
            }

            return string.Empty;
        }

        private static void AddParameterGuideBinding<T>(
            IDictionary<FrameworkElement, string> bindings,
            IReadOnlyDictionary<string, T> controls,
            string propertyName)
            where T : FrameworkElement
        {
            AddParameterGuideBinding(bindings, controls, propertyName, propertyName);
        }

        private static void AddParameterGuideBinding<T>(
            IDictionary<FrameworkElement, string> bindings,
            IReadOnlyDictionary<string, T> controls,
            string controlName,
            string propertyName)
            where T : FrameworkElement
        {
            if (controls.TryGetValue(controlName, out T control))
            {
                bindings[control] = propertyName;
            }
        }

        private readonly struct NumberOptions
        {
            public NumberOptions(double minimum, double maximum, bool allowDecimal, bool allowNegative)
            {
                Minimum = minimum;
                Maximum = maximum;
                AllowDecimal = allowDecimal;
                AllowNegative = allowNegative;
            }

            public double Minimum { get; }
            public double Maximum { get; }
            public bool AllowDecimal { get; }
            public bool AllowNegative { get; }
        }

        private readonly struct LocalizedLabel
        {
            public LocalizedLabel(string localizationKey, string fallbackText)
            {
                LocalizationKey = localizationKey;
                FallbackText = fallbackText;
            }

            public string LocalizationKey { get; }
            public string FallbackText { get; }
        }
    }
}
