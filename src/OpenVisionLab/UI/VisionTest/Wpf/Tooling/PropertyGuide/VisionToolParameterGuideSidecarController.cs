using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class VisionToolParameterGuideSidecarController
    {
        private const double SidecarWidth = 430D;
        private const double SidecarHeight = 330D;
        private const double SidecarGap = 8D;

        private readonly VisionToolSingleInputPropertyToolShell shell;
        private readonly VisionToolParameterGuideView guideView;
        private OpenVisionFloatingToolWindow sidecar;
        private Window sidecarOwner;
        private bool available;
        private bool dismissedByOperator;
        private bool closingSidecar;

        public VisionToolParameterGuideSidecarController(
            VisionToolSingleInputPropertyToolShell shell,
            VisionToolParameterGuideView guideView)
        {
            this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            this.guideView = guideView ?? throw new ArgumentNullException(nameof(guideView));
            shell.Loaded += Shell_Loaded;
            shell.Unloaded += Shell_Unloaded;
            guideView.ContentPresented += GuideView_ContentPresented;
        }

        public bool IsVisible => sidecar?.IsVisible == true;

        public void SetAvailable(bool value)
        {
            available = value;
            if (!available)
            {
                dismissedByOperator = false;
                CloseSidecar();
            }
        }

        public void Toggle()
        {
            if (!available)
            {
                return;
            }

            if (sidecar?.IsVisible == true)
            {
                dismissedByOperator = true;
                sidecar.Hide();
                return;
            }

            dismissedByOperator = false;
            ShowSidecar();
        }

        public void OnLayoutModeChanged()
        {
            if (shell.IsDockedInspectorMode && sidecar?.IsVisible == true)
            {
                dismissedByOperator = true;
                sidecar.Hide();
            }
        }

        private void GuideView_ContentPresented(object sender, EventArgs e)
        {
            if (!available || dismissedByOperator || shell.IsDockedInspectorMode)
            {
                return;
            }

            ShowSidecar();
        }

        private void ShowSidecar()
        {
            Window owner = Window.GetWindow(shell);
            if (owner == null || !owner.IsVisible)
            {
                return;
            }

            if (sidecar == null || !ReferenceEquals(sidecarOwner, owner))
            {
                CloseSidecar();
                sidecarOwner = owner;
                sidecar = new OpenVisionFloatingToolWindow(
                    OpenVisionLanguageService.T("VisionTool.ParameterGuide.Header"),
                    guideView)
                {
                    Owner = owner,
                    Width = SidecarWidth,
                    Height = SidecarHeight,
                    MinWidth = 360D,
                    MinHeight = 220D,
                    ResizeMode = ResizeMode.CanResize,
                    ShowInTaskbar = false,
                    ShowActivated = false,
                    WindowStartupLocation = WindowStartupLocation.Manual
                };
                sidecar.IsDockButtonVisible = false;
                sidecar.Closing += Sidecar_Closing;
            }

            sidecar.SetTitle(OpenVisionLanguageService.T("VisionTool.ParameterGuide.Header"));
            PositionNextToShell(sidecar);
            if (!sidecar.IsVisible)
            {
                sidecar.Show();
            }

            PositionNextToShell(sidecar);
        }

        private void PositionNextToShell(Window window)
        {
            Rect anchor = GetScreenBounds(shell);
            Rect workArea = SystemParameters.WorkArea;
            double width = window.Width > 0D ? window.Width : SidecarWidth;
            double height = window.Height > 0D ? window.Height : SidecarHeight;
            double rightCandidate = anchor.Right + SidecarGap;
            double leftCandidate = anchor.Left - SidecarGap - width;
            double left;
            if (rightCandidate + width <= workArea.Right)
            {
                left = rightCandidate;
            }
            else if (leftCandidate >= workArea.Left)
            {
                left = leftCandidate;
            }
            else
            {
                left = Math.Max(workArea.Left, workArea.Right - width);
            }

            window.Left = left;
            window.Top = Math.Max(
                workArea.Top,
                Math.Min(anchor.Top, workArea.Bottom - height));
        }

        private void Sidecar_Closing(object sender, CancelEventArgs e)
        {
            if (closingSidecar)
            {
                return;
            }

            e.Cancel = true;
            dismissedByOperator = true;
            sidecar?.Hide();
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            if (guideView.IsExpandedForTest
                && available
                && !dismissedByOperator
                && !shell.IsDockedInspectorMode)
            {
                ShowSidecar();
            }
        }

        private void Shell_Unloaded(object sender, RoutedEventArgs e)
        {
            CloseSidecar();
        }

        private void CloseSidecar()
        {
            if (sidecar == null)
            {
                sidecarOwner = null;
                return;
            }

            closingSidecar = true;
            try
            {
                sidecar.Closing -= Sidecar_Closing;
                sidecar.ClearHostedContent();
                sidecar.Close();
            }
            finally
            {
                sidecar = null;
                sidecarOwner = null;
                closingSidecar = false;
            }
        }

        private static Rect GetScreenBounds(FrameworkElement element)
        {
            Point topLeft = element.PointToScreen(new Point(0D, 0D));
            Point bottomRight = element.PointToScreen(
                new Point(element.ActualWidth, element.ActualHeight));
            PresentationSource source = PresentationSource.FromVisual(element);
            if (source?.CompositionTarget != null)
            {
                Matrix transform = source.CompositionTarget.TransformFromDevice;
                topLeft = transform.Transform(topLeft);
                bottomRight = transform.Transform(bottomRight);
            }

            return new Rect(topLeft, bottomRight);
        }
    }
}
