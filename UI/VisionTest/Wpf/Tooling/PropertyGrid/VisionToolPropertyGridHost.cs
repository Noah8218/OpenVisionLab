using OpenVisionLab.Common;
using OpenVisionLab.PropertyGrid;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpgPropertyGrid = System.Windows.Controls.WpfPropertyGrid.PropertyGrid;

namespace OpenVisionLab
{
    internal sealed class VisionToolPropertyGridHost : IDisposable
    {
        private readonly Border host;
        private readonly EventHandler<PropertyGridPropertyValueChangedEventArgs> propertyValueChanged;
        private readonly VisionToolParameterGuidePresenter parameterGuidePresenter;
        private readonly VisionToolLanguageChangeController parameterGuideLanguageController;
        private bool disposed;

        private VisionToolPropertyGridHost(
            Border host,
            object selectedObject,
            EventHandler<PropertyGridPropertyValueChangedEventArgs> propertyValueChanged)
        {
            this.host = host ?? throw new ArgumentNullException(nameof(host));
            this.propertyValueChanged = propertyValueChanged;

            Grid = OpenVisionToolOpenProfiler.Measure("NewWpgPropertyGrid", () => new WpgPropertyGrid());
            Grid.IsCompactDensity = IsHostedInDockedSingleInputShell(host);
            Binder = OpenVisionToolOpenProfiler.Measure("NewPropertyGridBinder", () => new PropertyGridEventBinder(null));

            // Algorithm tools stay PropertyGrid-driven: model properties define the generated editor UI.
            OpenVisionToolOpenProfiler.Measure("ApplyPropertyGridDisplayOptions", () => Grid.ApplyDisplayOptions(PropertyGridDisplayOptions.ToolForm));
            OpenVisionToolOpenProfiler.Measure("AttachPropertyGridEvents", () =>
            {
                Grid.PropertyValueChanged += OnPropertyValueChanged;
                Grid.SelectedObjectsChanged += Binder.Wpg_SelectedObjectsChanged;
                Grid.SelectedPropertyChanged += OnSelectedPropertyChanged;
            });
            OpenVisionToolOpenProfiler.Measure("AttachPropertyGridToHost", () => this.host.Child = Grid);
            VisionToolSingleInputPropertyToolShell shell = FindShell(host);
            if (shell?.ParameterGuide != null)
            {
                shell.ParameterGuideVisibility = Visibility.Visible;
                parameterGuidePresenter = new VisionToolParameterGuidePresenter(
                    shell.ParameterGuide,
                    selectedObject,
                    Grid.FocusProperty);
                parameterGuideLanguageController =
                    VisionToolLanguageChangeController.Attach(parameterGuidePresenter.Refresh);
            }

            ScheduleInitialSelectedObject(selectedObject);
        }

        public WpgPropertyGrid Grid { get; }

        public PropertyGridEventBinder Binder { get; }

        public void SetCompactDensity(bool compactDensity)
        {
            if (!disposed)
            {
                Grid.IsCompactDensity = compactDensity;
            }
        }

        public void SetThemeVariant(System.Windows.Controls.WpfPropertyGrid.PropertyGridThemeVariant themeVariant)
        {
            if (!disposed)
            {
                Grid.ThemeVariant = themeVariant;
            }
        }

        public static VisionToolPropertyGridHost Attach(
            Border host,
            object selectedObject,
            EventHandler<PropertyGridPropertyValueChangedEventArgs> propertyValueChanged)
        {
            return new VisionToolPropertyGridHost(host, selectedObject, propertyValueChanged);
        }

        private void ScheduleInitialSelectedObject(object selectedObject)
        {
            // WPG must receive SelectedObject during tool creation; deferring this to idle can leave fast-opened tools blank.
            OpenVisionToolOpenProfiler.Measure(
                "ApplyInitialPropertyGridSelectedObject",
                () => ApplyInitialSelectedObject(selectedObject));
        }

        private void ApplyInitialSelectedObject(object selectedObject)
        {
            if (disposed)
            {
                return;
            }

            // WPG's metadata cache is global; partial initial metadata made some tool grids paint blank.
            // Keep first render complete until a real virtualized PropertyGrid path replaces this safely.
            ApplySelectedObject(selectedObject, refresh: false);
        }

        public void SelectObject(object selectedObject)
        {
            Grid.SaveNavigationState();
            ApplySelectedObject(selectedObject, refresh: true);
        }

        public void RefreshSelectedObject()
        {
            Grid.RefreshSelectedObject();
            parameterGuidePresenter?.Refresh();
        }

        public void RefreshAndApplyVisibilityRules()
        {
            Grid.RefreshSelectedObject();
            Binder.ApplyVisibilityRules(Grid);
            parameterGuidePresenter?.Refresh();
        }

        public bool CommitPendingEdit()
        {
            if (disposed)
            {
                return false;
            }

            return Grid.CommitPendingEdit();
        }

        private void ApplySelectedObject(object selectedObject, bool refresh)
        {
            if (disposed)
            {
                return;
            }

            Grid.SelectedObject = selectedObject;
            parameterGuidePresenter?.SelectObject(selectedObject);
            // Conditional PropertyGrid rows must be refreshed when their generated WPG items already exist;
            // otherwise hidden children such as Contour EPSILON remain visible on first open.
            bool visibilityRefreshed = Binder.ApplyVisibilityRules(Grid, refreshOnChange: true);
            if (refresh && !visibilityRefreshed)
            {
                Grid.RefreshSelectedObject();
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Grid.SaveNavigationState();
            Grid.PropertyValueChanged -= OnPropertyValueChanged;
            Grid.SelectedObjectsChanged -= Binder.Wpg_SelectedObjectsChanged;
            Grid.SelectedPropertyChanged -= OnSelectedPropertyChanged;
            parameterGuideLanguageController?.Dispose();
            if (ReferenceEquals(host.Child, Grid))
            {
                host.Child = null;
            }
        }

        private void OnPropertyValueChanged(object sender, PropertyGridPropertyValueChangedEventArgs e)
        {
            Binder.Wpg_PropertyValueChanged(sender, e);
            parameterGuidePresenter?.SelectProperty(
                string.IsNullOrWhiteSpace(parameterGuidePresenter.SelectedPropertyName)
                    ? e?.PropertyName
                    : parameterGuidePresenter.SelectedPropertyName);
            propertyValueChanged?.Invoke(sender, e);
        }

        private void OnSelectedPropertyChanged(
            object sender,
            System.Windows.Controls.WpfPropertyGrid.PropertyGridSelectedPropertyChangedEventArgs e)
        {
            parameterGuidePresenter?.SelectProperty(e?.PropertyName);
        }

        private static bool IsHostedInDockedSingleInputShell(DependencyObject element)
        {
            return FindShell(element)?.IsDockedInspectorMode == true;
        }

        private static VisionToolSingleInputPropertyToolShell FindShell(DependencyObject element)
        {
            DependencyObject current = element;
            while (current != null)
            {
                if (current is VisionToolSingleInputPropertyToolShell shell)
                {
                    return shell;
                }

                current = GetParent(current);
            }

            return null;
        }

        private static DependencyObject GetParent(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            DependencyObject visualParent = VisualTreeHelper.GetParent(element);
            if (visualParent != null)
            {
                return visualParent;
            }

            if (element is FrameworkElement frameworkElement)
            {
                return frameworkElement.Parent ?? frameworkElement.TemplatedParent as DependencyObject;
            }

            return null;
        }
    }
}
