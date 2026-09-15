using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostRecipePanelDragController : IDisposable
    {
        private readonly FrameworkElement rootShellHost;
        private readonly FrameworkElement recipeManagerPanel;
        private readonly FrameworkElement recipeManagerTitleBar;
        private readonly TranslateTransform recipeManagerPanelTransform;
        private bool isDragging;
        private Point dragStartPoint;
        private double dragStartX;
        private double dragStartY;

        public OpenVisionShellHostRecipePanelDragController(
            FrameworkElement rootShellHost,
            FrameworkElement recipeManagerPanel,
            FrameworkElement recipeManagerTitleBar,
            TranslateTransform recipeManagerPanelTransform)
        {
            this.rootShellHost = rootShellHost;
            this.recipeManagerPanel = recipeManagerPanel;
            this.recipeManagerTitleBar = recipeManagerTitleBar;
            this.recipeManagerPanelTransform = recipeManagerPanelTransform;
        }

        public void HandleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (recipeManagerPanel?.Visibility != Visibility.Visible
                || rootShellHost == null
                || recipeManagerPanelTransform == null)
            {
                return;
            }

            isDragging = true;
            dragStartPoint = e.GetPosition(rootShellHost);
            dragStartX = recipeManagerPanelTransform.X;
            dragStartY = recipeManagerPanelTransform.Y;
            Mouse.Capture(recipeManagerTitleBar);
            e.Handled = true;
        }

        public void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || e.LeftButton != MouseButtonState.Pressed || rootShellHost == null)
            {
                return;
            }

            Point current = e.GetPosition(rootShellHost);
            SetOffset(
                dragStartX + current.X - dragStartPoint.X,
                dragStartY + current.Y - dragStartPoint.Y);
            e.Handled = true;
        }

        public void HandleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Stop();
            e.Handled = true;
        }

        public void HandleLostMouseCapture(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        public void SetOffset(double x, double y)
        {
            if (recipeManagerPanelTransform == null)
            {
                return;
            }

            if (rootShellHost == null
                || recipeManagerPanel == null
                || rootShellHost.ActualWidth <= 0D
                || rootShellHost.ActualHeight <= 0D)
            {
                recipeManagerPanelTransform.X = x;
                recipeManagerPanelTransform.Y = y;
                return;
            }

            double currentLeft = recipeManagerPanel.TranslatePoint(new Point(0D, 0D), rootShellHost).X;
            double currentTop = recipeManagerPanel.TranslatePoint(new Point(0D, 0D), rootShellHost).Y;
            double baseLeft = currentLeft - recipeManagerPanelTransform.X;
            double baseTop = currentTop - recipeManagerPanelTransform.Y;
            const double minimumVisibleWidth = 260D;
            const double minimumVisibleHeight = 92D;
            double minX = 8D - baseLeft;
            double maxX = Math.Max(minX, rootShellHost.ActualWidth - minimumVisibleWidth - baseLeft);
            double minY = 8D - baseTop;
            double maxY = Math.Max(minY, rootShellHost.ActualHeight - minimumVisibleHeight - baseTop);

            recipeManagerPanelTransform.X = Math.Min(Math.Max(x, minX), maxX);
            recipeManagerPanelTransform.Y = Math.Min(Math.Max(y, minY), maxY);
        }

        public void Stop()
        {
            isDragging = false;
            if (Mouse.Captured == recipeManagerTitleBar)
            {
                Mouse.Capture(null);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
