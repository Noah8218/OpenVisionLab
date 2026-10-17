using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenVisionLab
{
    internal sealed class VisionToolPresetButtonPresenter<TProperty> : IDisposable
    {
        private readonly VisionToolSingleInputPropertyToolShell shell;
        private readonly IReadOnlyList<VisionToolPreset<TProperty>> presets;
        private readonly Action<VisionToolPreset<TProperty>> applyPreset;
        private readonly Dictionary<string, Button> buttons;
        private readonly ContextMenu menu;
        private readonly List<MenuItem> menuItems = new List<MenuItem>();
        private VisionToolPreset<TProperty> lastAppliedPreset;
        private bool disposed;

        private VisionToolPresetButtonPresenter(
            VisionToolSingleInputPropertyToolShell shell,
            IReadOnlyList<VisionToolPreset<TProperty>> presets,
            Action<VisionToolPreset<TProperty>> applyPreset)
        {
            this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            this.presets = presets ?? Array.Empty<VisionToolPreset<TProperty>>();
            this.applyPreset = applyPreset ?? throw new ArgumentNullException(nameof(applyPreset));
            buttons = new Dictionary<string, Button>(StringComparer.OrdinalIgnoreCase)
            {
                ["basic"] = shell.PresetBasicButton,
                ["fast"] = shell.PresetFastButton,
                ["precise"] = shell.PresetPreciseButton
            };
            menu = new ContextMenu
            {
                PlacementTarget = shell.PresetMenuButton
            };
            shell.PresetMenuButton.ContextMenu = menu;

            AttachButtons();
            shell.PresetMenuButton.Click += OnPresetMenuClicked;
            shell.DockedInspectorModeChanged += OnDockedInspectorModeChanged;
            shell.WorkbenchStepActions.DataContextChanged += OnWorkbenchEditorChanged;
            ApplyLocalization();
        }

        public static VisionToolPresetButtonPresenter<TProperty> Attach(
            VisionToolSingleInputPropertyToolShell shell,
            IReadOnlyList<VisionToolPreset<TProperty>> presets,
            Action<VisionToolPreset<TProperty>> applyPreset)
        {
            return new VisionToolPresetButtonPresenter<TProperty>(shell, presets, applyPreset);
        }

        public void ApplyLocalization()
        {
            if (disposed)
            {
                return;
            }

            bool hasPreset = presets.Count > 0;
            bool workbench = shell.WorkbenchStepActions.DataContext is OpenVisionWorkbenchToolViewModel;
            bool showBody = hasPreset && (workbench || !shell.IsDockedInspectorMode);
            bool showMenu = hasPreset && !workbench && shell.IsDockedInspectorMode;
            shell.PresetHost.Visibility = showBody ? Visibility.Visible : Visibility.Collapsed;
            shell.PresetGap.Visibility = showBody ? Visibility.Visible : Visibility.Collapsed;
            shell.PresetMenuButton.Visibility = showMenu ? Visibility.Visible : Visibility.Collapsed;
            string menuToolTip = VisionToolVerificationText.T(
                "VisionTool.Preset.MenuToolTip",
                "Choose recommended preset");
            shell.PresetMenuButton.ToolTip = menuToolTip;
            AutomationProperties.SetName(shell.PresetMenuButton, menuToolTip);
            AutomationProperties.SetHelpText(shell.PresetMenuButton, VisionToolVerificationText.T(
                "VisionTool.Preset.Detail",
                "Choose a starting point. It updates PropertyGrid values only; run Preview to verify."));
            ApplyMenuItems();
            if (!showBody)
            {
                return;
            }

            shell.PresetTitleText.Text = workbench ? OpenVisionLanguageService.T("Workbench.Tool.Presets")
                : VisionToolVerificationText.T("VisionTool.Preset.Title", "Recommended presets");
            shell.PresetDetailText.Visibility = workbench ? Visibility.Collapsed : Visibility.Visible;
            if (shell.PresetHost.Child is Grid layout)
            {
                layout.ColumnDefinitions[0].Width = workbench ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                layout.ColumnDefinitions[1].Width = new GridLength(workbench ? 8D : 12D);
                layout.HorizontalAlignment = workbench ? HorizontalAlignment.Left : HorizontalAlignment.Stretch;
                ((FrameworkElement)shell.PresetTitleText.Parent).MinWidth = workbench ? 0D : 160D;
            }
            if (workbench)
            {
                shell.PresetHost.SetBinding(UIElement.IsEnabledProperty, new Binding(nameof(OpenVisionWorkbenchToolViewModel.CanEditParameters))
                    { Source = shell.WorkbenchStepActions.DataContext });
                shell.PresetHost.BorderThickness = new Thickness(0D);
            }
            shell.PresetDetailText.Text = lastAppliedPreset == null
                ? VisionToolVerificationText.T(
                    "VisionTool.Preset.Detail",
                    "Basic = first check / Fast = quick screening / Precise = final tuning. Select one to see why.")
                : string.Format(
                    CultureInfo.CurrentCulture,
                    VisionToolVerificationText.T(
                        "VisionTool.Preset.AppliedDetailFormat",
                        "{0}: {1} Values only; run Preview to verify."),
                    lastAppliedPreset.DisplayName,
                    lastAppliedPreset.Description);

            foreach (KeyValuePair<string, Button> pair in buttons)
            {
                VisionToolPreset<TProperty> preset = presets.FirstOrDefault(item => string.Equals(item.Id, pair.Key, StringComparison.OrdinalIgnoreCase));
                ApplyButton(pair.Value, preset, workbench);
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            shell.DockedInspectorModeChanged -= OnDockedInspectorModeChanged;
            shell.WorkbenchStepActions.DataContextChanged -= OnWorkbenchEditorChanged;
            shell.PresetMenuButton.Click -= OnPresetMenuClicked;
            foreach (Button button in buttons.Values)
            {
                button.Click -= OnPresetClicked;
                button.Tag = null;
            }

            ClearMenuItems();
            shell.PresetMenuButton.ContextMenu = null;
            shell.PresetMenuButton.Visibility = Visibility.Collapsed;
            shell.PresetHost.Visibility = Visibility.Collapsed;
            shell.PresetGap.Visibility = Visibility.Collapsed;
        }

        private void OnDockedInspectorModeChanged(object sender, EventArgs e)
        {
            ApplyLocalization();
        }

        private void OnWorkbenchEditorChanged(object sender, DependencyPropertyChangedEventArgs e) => ApplyLocalization();

        private void AttachButtons()
        {
            foreach (Button button in buttons.Values)
            {
                button.Click -= OnPresetClicked;
                button.Click += OnPresetClicked;
            }
        }

        private void ApplyMenuItems()
        {
            ClearMenuItems();
            foreach (VisionToolPreset<TProperty> preset in presets)
            {
                MenuItem item = new MenuItem
                {
                    Header = preset.DisplayName,
                    ToolTip = preset.Description,
                    Tag = preset
                };
                AutomationProperties.SetName(item, preset.DisplayName);
                AutomationProperties.SetHelpText(item, preset.Description);
                AutomationProperties.SetAutomationId(item, "VisionToolPresetMenuItem_" + preset.Id);
                item.Click += OnPresetMenuItemClicked;
                menu.Items.Add(item);
                menuItems.Add(item);
            }
        }

        private void ClearMenuItems()
        {
            foreach (MenuItem item in menuItems)
            {
                item.Click -= OnPresetMenuItemClicked;
                item.Tag = null;
            }

            menuItems.Clear();
            menu.Items.Clear();
        }

        private static void ApplyButton(Button button, VisionToolPreset<TProperty> preset, bool workbench)
        {
            if (button == null)
            {
                return;
            }

            button.Tag = preset;
            button.Visibility = preset == null ? Visibility.Collapsed : Visibility.Visible;
            if (preset == null)
            {
                return;
            }

            button.Content = preset.DisplayName;
            string tooltip = workbench ? OpenVisionLanguageService.T(preset.Id switch
                { "basic" => "Workbench.Tool.PresetBasic", "fast" => "Workbench.Tool.PresetFast", _ => "Workbench.Tool.PresetPrecise" }) : preset.Description;
            button.ToolTip = tooltip;
            if (workbench)
            {
                button.SetResourceReference(FrameworkElement.StyleProperty, "OpenVision.Workbench.ButtonStyle");
                button.FontSize = 12D;
            }
            AutomationProperties.SetName(button, preset.DisplayName);
            AutomationProperties.SetHelpText(button, tooltip);
        }

        private void OnPresetMenuClicked(object sender, RoutedEventArgs e)
        {
            if (disposed || menu.Items.Count == 0)
            {
                return;
            }

            menu.PlacementTarget = shell.PresetMenuButton;
            menu.IsOpen = true;
        }

        private void OnPresetMenuItemClicked(object sender, RoutedEventArgs e)
        {
            if (disposed)
            {
                return;
            }

            if (sender is MenuItem menuItem && menuItem.Tag is VisionToolPreset<TProperty> preset)
            {
                ApplyPresetAndRefresh(preset);
            }
        }

        private void OnPresetClicked(object sender, RoutedEventArgs e)
        {
            if (disposed)
            {
                return;
            }

            if (sender is Button button && button.Tag is VisionToolPreset<TProperty> preset)
            {
                ApplyPresetAndRefresh(preset);
            }
        }

        private void ApplyPresetAndRefresh(VisionToolPreset<TProperty> preset)
        {
            OpenVisionWorkbenchToolViewModel workbench = shell.WorkbenchStepActions.DataContext as OpenVisionWorkbenchToolViewModel;
            if (workbench != null && !workbench.CanEditParameters) { return; }
            applyPreset(preset);
            lastAppliedPreset = preset;
            workbench?.NotifyPresetApplied(preset.DisplayNameKey);
            ApplyLocalization();
        }
    }
}
