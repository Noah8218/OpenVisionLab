using MahApps.Metro.IconPacks;
using OpenVisionLab.Core;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace OpenVisionLab
{
    public partial class OpenVisionShellHostWindow : Window
    {
        private const double TitleBarHeight = 40D;
        private const double ResizeBorderThickness = 7D;
        private readonly List<MenuItem> recentRecipeMenuItems = new();
        private HwndSource windowSource;
        private OpenVisionWorkbenchViewModel workbenchPresentation;
        private InlineImageEditHost inlineImageEditHost;
        private IInputElement shortcutPreviousFocus;
        private IInputElement recipePreviousFocus;
        private IInputElement validationPreviousFocus;
        private IInputElement historyPreviousFocus;
        private IInputElement llmPreviousFocus;
        private IInputElement layerManagerPreviousFocus;
        private Storyboard productInfoAnimation;
        private double responsiveScale = 1D;
        private bool addShortcutReleasePending;
        public static readonly DependencyProperty ClosingCommandProperty = DependencyProperty.Register(
            nameof(ClosingCommand), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty ShortcutsCloseCommandProperty = DependencyProperty.Register(
            nameof(ShortcutsCloseCommand), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand ShortcutsCloseCommand { get => (ICommand)GetValue(ShortcutsCloseCommandProperty); set => SetValue(ShortcutsCloseCommandProperty, value); }

        public static readonly DependencyProperty MainRecipeCloseCommandProperty = DependencyProperty.Register(
            nameof(MainRecipeCloseCommand), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand MainRecipeCloseCommand { get => (ICommand)GetValue(MainRecipeCloseCommandProperty); set => SetValue(MainRecipeCloseCommandProperty, value); }

        public static readonly DependencyProperty MainValidationCloseCommandProperty = DependencyProperty.Register(
            nameof(MainValidationCloseCommand), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand MainValidationCloseCommand { get => (ICommand)GetValue(MainValidationCloseCommandProperty); set => SetValue(MainValidationCloseCommandProperty, value); }

        public static readonly DependencyProperty LayerManagerCloseCommandProperty = DependencyProperty.Register(nameof(LayerManagerCloseCommand), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand LayerManagerCloseCommand { get => (ICommand)GetValue(LayerManagerCloseCommandProperty); set => SetValue(LayerManagerCloseCommandProperty, value); }

        public static readonly DependencyProperty MainLlmCloseCommandProperty = DependencyProperty.Register(nameof(MainLlmCloseCommand), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand MainLlmCloseCommand { get => (ICommand)GetValue(MainLlmCloseCommandProperty); set => SetValue(MainLlmCloseCommandProperty, value); }

        public static readonly DependencyProperty MainHistoryCloseCommandProperty = DependencyProperty.Register(nameof(MainHistoryCloseCommand), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand MainHistoryCloseCommand { get => (ICommand)GetValue(MainHistoryCloseCommandProperty); set => SetValue(MainHistoryCloseCommandProperty, value); }

        public ICommand ClosingCommand { get => (ICommand)GetValue(ClosingCommandProperty); set => SetValue(ClosingCommandProperty, value); }

        public static readonly DependencyProperty ViewerSingleCommandProperty = DependencyProperty.Register(nameof(ViewerSingleCommand), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand ViewerSingleCommand { get => (ICommand)GetValue(ViewerSingleCommandProperty); set => SetValue(ViewerSingleCommandProperty, value); }
        public static readonly DependencyProperty ViewerSplit2CommandProperty = DependencyProperty.Register(nameof(ViewerSplit2Command), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand ViewerSplit2Command { get => (ICommand)GetValue(ViewerSplit2CommandProperty); set => SetValue(ViewerSplit2CommandProperty, value); }
        public static readonly DependencyProperty ViewerSplit4CommandProperty = DependencyProperty.Register(nameof(ViewerSplit4Command), typeof(ICommand), typeof(OpenVisionShellHostWindow), new PropertyMetadata(null));
        public ICommand ViewerSplit4Command { get => (ICommand)GetValue(ViewerSplit4CommandProperty); set => SetValue(ViewerSplit4CommandProperty, value); }

        public OpenVisionShellHostWindow()
            : this(ApplicationRuntimeContext.CreateDefault())
        {
        }

        public OpenVisionShellHostWindow(ApplicationRuntimeContext runtimeContext)
        {
            InitializeComponent();
            shellTitleBar.TitleText = "OpenVisionLab";
            shellTitleBar.IconKind = PackIconMaterialKind.ImageFilterCenterFocus;
            contentHost.Content = new OpenVisionShellHostView(runtimeContext);
            workbenchPresentation = ShellHostForSmoke.Workbench;
            DataContext = workbenchPresentation;
            inlineImageEditHost = new InlineImageEditHost(this, inlineImageEditOverlay, contentHost, workbenchPresentation);
            PropertyGridEditorFactory.SetImageEditHost(this, inlineImageEditHost.Show);
            RefreshRecentRecipesMenu();
            workbenchPresentation.ProductInfoRequested += ProductInfo_Requested;
            workbenchPresentation.PropertyChanged += MainRecipe_PropertyChanged;
            SetBinding(ViewerSingleCommandProperty, new Binding("Viewer.SingleCommand"));
            SetBinding(ViewerSplit2CommandProperty, new Binding("Viewer.Split2Command"));
            SetBinding(ViewerSplit4CommandProperty, new Binding("Viewer.Split4Command"));
            SetBinding(ShortcutsCloseCommandProperty, new Binding("CloseShortcutsCommand"));
            SetBinding(MainRecipeCloseCommandProperty, new Binding("CloseMainRecipeCommand"));
            SetBinding(LayerManagerCloseCommandProperty, new Binding("CloseLayerManagerCommand"));
            SetBinding(MainLlmCloseCommandProperty, new Binding("CloseMainLlmCommand"));
            SetBinding(MainHistoryCloseCommandProperty, new Binding("CloseMainHistoryCommand"));
            SetBinding(MainValidationCloseCommandProperty, new Binding("CloseMainValidationCommand"));
            SetBinding(ClosingCommandProperty, new Binding("CloseMainWindowCommand"));
            ApplyResponsiveScale(Width, Height);
        }

        public OpenVisionShellHostView ShellHostForSmoke => contentHost.Content as OpenVisionShellHostView;

        public Task StartupPreparationTask =>
            ShellHostForSmoke?.StartupPreparationTask ?? Task.CompletedTask;

        public double ResponsiveScaleForSmoke => responsiveScale;

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ApplyResponsiveScale(sizeInfo.NewSize.Width, sizeInfo.NewSize.Height);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            windowSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            windowSource?.AddHook(WindowProc);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            addShortcutReleasePending = false;
            base.OnDeactivated(e);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && addShortcutReleasePending)
            {
                addShortcutReleasePending = false;
                // WPG commits Enter on KeyUp; the shortcut already committed or rejected the edit on KeyDown.
                e.Handled = true;
                return;
            }
            base.OnPreviewKeyUp(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            addShortcutReleasePending = false;
            if (workbenchPresentation != null)
            {
                workbenchPresentation.ProductInfoRequested -= ProductInfo_Requested;
                workbenchPresentation.PropertyChanged -= MainRecipe_PropertyChanged;
            }
            PropertyGridEditorFactory.SetImageEditHost(this, null);
            inlineImageEditHost?.Dispose(); inlineImageEditHost = null;
            workbenchPresentation = null;
            recentRecipeMenuItems.Clear();
            productInfoAnimation?.Remove(mainProductInfoToast);
            productInfoAnimation = null;
            layerManagerPreviousFocus = null;
            llmPreviousFocus = null;
            historyPreviousFocus = null;
            shortcutPreviousFocus = null;
            recipePreviousFocus = null;
            validationPreviousFocus = null;
            ShellHostForSmoke?.Dispose();
            contentHost.Content = null;
            DataContext = null;
            windowSource?.RemoveHook(WindowProc);
            windowSource = null;
            base.OnClosed(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (inlineImageEditHost?.IsActive == true && inlineImageEditHost.HandleKey(e)) { e.Handled = true; return; }
            if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                addShortcutReleasePending = true;
                e.Handled = true;
                KeyBinding add = InputBindings.OfType<KeyBinding>().FirstOrDefault(binding => binding.Key == Key.Enter && binding.Modifiers == ModifierKeys.Control);
                if (workbenchPresentation.IsMainInteractive && add?.Command?.CanExecute(add.CommandParameter) == true) { add.Command.Execute(add.CommandParameter); }
                return;
            }
            if (layerManagerOverlay.IsVisible)
            {
                if (e.Key == Key.Escape) { if (LayerManagerCloseCommand?.CanExecute(null) == true) { LayerManagerCloseCommand.Execute(null); } e.Handled = true; return; }
                if (e.Key >= Key.F1 && e.Key <= Key.F24 || Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key is not (Key.C or Key.V or Key.A or Key.X)
                    || Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) || e.Key == Key.Delete && Keyboard.FocusedElement is not TextBoxBase) { e.Handled = true; return; }
            }
            if (mainLlmOverlay.IsVisible)
            {
                if (e.Key == Key.Escape) { if (MainLlmCloseCommand?.CanExecute(null) == true) { MainLlmCloseCommand.Execute(null); } e.Handled = true; return; }
                if (e.Key >= Key.F1 && e.Key <= Key.F24 || Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key is not (Key.C or Key.V or Key.A or Key.X)) { e.Handled = true; return; }
            }
            if (mainHistoryOverlay.IsVisible)
            {
                if (e.Key == Key.Escape) { MainHistoryCloseCommand?.Execute(null); e.Handled = true; return; }
                if (e.Key >= Key.F1 && e.Key <= Key.F24 || Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) { e.Handled = true; return; }
            }
            if (mainValidationOverlay.IsVisible)
            {
                if (e.Key == Key.Escape)
                {
                    if (MainValidationCloseCommand?.CanExecute(null) == true) { MainValidationCloseCommand.Execute(null); }
                    e.Handled = true;
                    return;
                }
                if (e.Key >= Key.F1 && e.Key <= Key.F24 || Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key is Key.N or Key.S or Key.O or Key.I or Key.D1 or Key.D2 or Key.D3)
                {
                    e.Handled = true;
                    return;
                }
            }
            if (mainRecipeOverlay.IsVisible)
            {
                if (e.Key == Key.Escape)
                {
                    if (MainRecipeCloseCommand?.CanExecute(null) == true) { MainRecipeCloseCommand.Execute(null); }
                    e.Handled = true;
                    return;
                }
                if (e.Key >= Key.F1 && e.Key <= Key.F24 || Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key is Key.N or Key.S or Key.O or Key.I or Key.D1 or Key.D2 or Key.D3)
                {
                    e.Handled = true;
                    return;
                }
            }
            if (mainShortcutsOverlay.IsVisible)
            {
                if (e.Key == Key.Escape)
                {
                    if (ShortcutsCloseCommand?.CanExecute(null) == true) { ShortcutsCloseCommand.Execute(null); }
                    e.Handled = true;
                    return;
                }
                if (Keyboard.Modifiers != ModifierKeys.None && !(e.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.Shift)
                    || e.Key is not (Key.Tab or Key.Enter or Key.Space or Key.Up or Key.Down or Key.PageUp or Key.PageDown or Key.Home or Key.End))
                {
                    e.Handled = true;
                    return;
                }
            }
            if (!e.Handled && !mainRecipeOverlay.IsVisible && !mainValidationOverlay.IsVisible && Keyboard.Modifiers == ModifierKeys.None
                && Mouse.Captured == null && Mouse.LeftButton == MouseButtonState.Released && Mouse.MiddleButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released
                && Keyboard.FocusedElement is not (TextBoxBase or PasswordBox or Selector or ComboBoxItem or RangeBase)
                && (ShellHostForSmoke?.FindName("workbenchViewerToolbarScroll") is UIElement toolbar && toolbar.IsKeyboardFocusWithin
                    || ShellHostForSmoke?.FindName("hostWorkspaceFrame") is UIElement viewer && viewer.IsKeyboardFocusWithin))
            {
                ICommand command = e.Key switch { Key.D1 => ViewerSingleCommand, Key.D2 => ViewerSplit2Command, Key.D4 => ViewerSplit4Command, _ => null };
                if (command?.CanExecute(null) == true) { command.Execute(null); e.Handled = true; return; }
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && Mouse.Captured == null && !mainRecipeOverlay.IsVisible && !mainValidationOverlay.IsVisible && !mainShortcutsOverlay.IsVisible
                && ShellHostForSmoke?.FindName("hostWorkspaceFrame") is FrameworkElement viewer && viewer.IsEnabled)
            {
                for (DependencyObject source = e.OriginalSource as DependencyObject; source != null;
                    source = source is Visual ? VisualTreeHelper.GetParent(source) : LogicalTreeHelper.GetParent(source))
                {
                    if (source is FrameworkElement { Name: "layerImageSurface" } surface && viewer.IsAncestorOf(surface))
                    {
                        viewer.SetCurrentValue(FocusableProperty, true);
                        viewer.Focus();
                        break;
                    }
                }
            }
            base.OnPreviewMouseDown(e);
        }

        private void MainShortcuts_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                shortcutPreviousFocus = Keyboard.FocusedElement;
                Dispatcher.BeginInvoke(new Action(() => { if (IsVisible && mainShortcutsOverlay.IsVisible) { mainShortcutsClose.Focus(); } }), System.Windows.Threading.DispatcherPriority.Input);
                return;
            }
            IInputElement previous = shortcutPreviousFocus;
            shortcutPreviousFocus = null;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsVisible || mainShortcutsOverlay.IsVisible) { return; }
                if (previous is UIElement element && element.IsVisible && element.IsEnabled && element.Focus()) { return; }
                shellTitleBar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void MainRecipe_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                recipePreviousFocus = Keyboard.FocusedElement;
                Dispatcher.BeginInvoke(new Action(() => { if (IsVisible && mainRecipeOverlay.IsVisible) { mainRecipeNameInput.Focus(); } }), System.Windows.Threading.DispatcherPriority.Input);
                return;
            }
            Dispatcher.BeginInvoke(new Action(RestoreMainRecipeFocus), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void LayerManager_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                layerManagerPreviousFocus = Keyboard.FocusedElement;
                Dispatcher.BeginInvoke(new Action(() => { if (IsVisible && layerManagerOverlay.IsVisible) { managedLayerRows.Focus(); } }), System.Windows.Threading.DispatcherPriority.Input);
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsVisible || layerManagerOverlay.IsVisible || workbenchPresentation?.IsMainInteractive != true) { return; }
                IInputElement previous = layerManagerPreviousFocus; layerManagerPreviousFocus = null;
                if (previous is UIElement element && element.IsVisible && element.IsEnabled && element.Focus()) { return; }
                shellTitleBar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void MainLlm_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                llmPreviousFocus = Keyboard.FocusedElement;
                Dispatcher.BeginInvoke(new Action(() => { if (IsVisible && mainLlmOverlay.IsVisible) { mainLlmGoal.Focus(); } }), System.Windows.Threading.DispatcherPriority.Input);
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsVisible || mainLlmOverlay.IsVisible || workbenchPresentation?.IsMainInteractive != true) { return; }
                IInputElement previous = llmPreviousFocus; llmPreviousFocus = null;
                if (previous is UIElement element && element.IsVisible && element.IsEnabled && element.Focus()) { return; }
                shellTitleBar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void MainLlmResponse_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) { Dispatcher.BeginInvoke(new Action(() => { if (IsVisible && mainLlmOverlay.IsVisible) { mainLlmXml.Focus(); } }), System.Windows.Threading.DispatcherPriority.Input); }
        }

        private void MainLlmReview_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) { Dispatcher.BeginInvoke(new Action(() => { if (IsVisible && mainLlmOverlay.IsVisible) { mainLlmName.Focus(); } }), System.Windows.Threading.DispatcherPriority.Input); }
        }

        private void MainHistory_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                historyPreviousFocus = Keyboard.FocusedElement;
                Dispatcher.BeginInvoke(new Action(() => { if (IsVisible && mainHistoryOverlay.IsVisible) { mainHistoryClose.Focus(); } }), System.Windows.Threading.DispatcherPriority.Input);
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsVisible || mainHistoryOverlay.IsVisible || workbenchPresentation?.IsMainInteractive != true) { return; }
                IInputElement previous = historyPreviousFocus; historyPreviousFocus = null;
                if (previous is UIElement element && element.IsVisible && element.IsEnabled && element.Focus()) { return; }
                shellTitleBar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void MainValidation_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                validationPreviousFocus = Keyboard.FocusedElement;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!IsVisible || !mainValidationOverlay.IsVisible) { return; }
                    if (mainValidationNameInput.IsVisible) { mainValidationNameInput.Focus(); }
                    else { mainValidationClose.Focus(); }
                }), System.Windows.Threading.DispatcherPriority.Input);
                return;
            }
            Dispatcher.BeginInvoke(new Action(RestoreMainValidationFocus), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void RestoreMainValidationFocus()
        {
            if (!IsVisible || mainValidationOverlay.IsVisible || workbenchPresentation?.IsMainInteractive != true || validationPreviousFocus == null) { return; }
            IInputElement previous = validationPreviousFocus;
            validationPreviousFocus = null;
            if (previous is UIElement element && element.IsVisible && element.IsEnabled && element.Focus()) { return; }
            shellTitleBar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        private void RefreshRecentRecipesMenu()
        {
            MenuItem mainFileMenu = (MenuItem)((Grid)shellTitleBar.HeaderContent).Children.OfType<Menu>().Single().Items[0];
            foreach (MenuItem item in recentRecipeMenuItems) { mainFileMenu.Items.Remove(item); }
            recentRecipeMenuItems.Clear();
            MenuItem mainRecentRecipesHeading = (MenuItem)mainFileMenu.Items[2], mainRecentRecipesEmpty = (MenuItem)mainFileMenu.Items[3];
            foreach (KeyValuePair<string, string> recent in workbenchPresentation.RecentRecipes)
            {
                var icon = new PackIconMaterial { Kind = PackIconMaterialKind.History, Width = 16, Height = 16 };
                icon.SetResourceReference(Control.ForegroundProperty, "OpenVision.Workbench.SecondaryTextBrush");
                var header = new TextBlock { Text = recent.Value, TextWrapping = TextWrapping.Wrap, MaxWidth = 480, Margin = new Thickness(0, 6, 0, 6) };
                var item = new MenuItem { Header = header, Height = double.NaN, MinHeight = 34, CommandParameter = recent.Key, Icon = icon, Style = (Style)FindResource("Main.MenuItemStyle") };
                header.SetBinding(TextBlock.ForegroundProperty, new Binding("Foreground") { Source = item });
                System.Windows.Automation.AutomationProperties.SetName(item, recent.Value);
                item.SetBinding(MenuItem.CommandProperty, new Binding("Recipes.OpenMainRecentRecipeCommand") { Source = workbenchPresentation });
                item.SetBinding(IsEnabledProperty, new Binding("IsMainInteractive") { Source = workbenchPresentation });
                mainFileMenu.Items.Insert(mainFileMenu.Items.IndexOf(mainRecentRecipesHeading) + 1 + recentRecipeMenuItems.Count, item);
                recentRecipeMenuItems.Add(item);
            }
            mainRecentRecipesEmpty.Visibility = recentRecipeMenuItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MainRecipe_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(OpenVisionWorkbenchViewModel.RecentRecipes)) { RefreshRecentRecipesMenu(); }
            if (e.PropertyName == nameof(OpenVisionWorkbenchViewModel.IsMainInteractive) && workbenchPresentation?.IsMainInteractive == true)
            {
                Dispatcher.BeginInvoke(new Action(RestoreMainRecipeFocus), System.Windows.Threading.DispatcherPriority.Input);
                Dispatcher.BeginInvoke(new Action(RestoreMainValidationFocus), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private void RestoreMainRecipeFocus()
        {
            if (!IsVisible || mainRecipeOverlay.IsVisible || workbenchPresentation?.IsMainInteractive != true || recipePreviousFocus == null) { return; }
            IInputElement previous = recipePreviousFocus;
            recipePreviousFocus = null;
            if (previous is UIElement element && element.IsVisible && element.IsEnabled && element.Focus()) { return; }
            shellTitleBar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        private void ProductInfo_Requested(object sender, EventArgs e)
        {
            productInfoAnimation?.Remove(mainProductInfoToast);
            mainProductInfoToast.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            productInfoAnimation = ((Storyboard)FindResource("Main.ProductInfoAnimation")).Clone();
            productInfoAnimation.Begin(mainProductInfoToast, HandoffBehavior.SnapshotAndReplace, true);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (ClosingCommand?.CanExecute(e) == true) { ClosingCommand.Execute(e); }
        }

        private void ApplyResponsiveScale(double width, double height)
        {
            responsiveScale = 1D;
            titleBarRow.Height = new GridLength(TitleBarHeight);
            shellWindowChrome.CaptionHeight = TitleBarHeight;
            shellWindowChrome.ResizeBorderThickness = new Thickness(ResizeBorderThickness);
        }

        private IntPtr WindowProc(
            IntPtr hwnd,
            int message,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            if (message == OpenVisionWindowWorkArea.GetMinMaxInfoMessage)
            {
                DpiScale dpi = VisualTreeHelper.GetDpi(this);
                OpenVisionWindowWorkArea.Apply(hwnd, lParam, MinWidth * dpi.DpiScaleX, MinHeight * dpi.DpiScaleY);
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}
