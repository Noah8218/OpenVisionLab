using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public partial class FormMainFrame
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private const double RestoredWindowRatio = 0.82;
        private const int TitleBarButtonSize = 30;
        private const int TitleBarButtonTop = 8;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private Rectangle restoredWindowBounds = Rectangle.Empty;
        private bool isFullScreenLayout = false;
        private bool titleBarMouseDown = false;
        private bool titleBarDragging = false;
        private bool suppressTitleBarDrag = false;
        private Point titleBarMouseDownPoint = Point.Empty;
        private DateTime lastTitleBarClickTime = DateTime.MinValue;
        private Point lastTitleBarClickScreenPoint = Point.Empty;

        private void EnsureStartupWindowVisible()
        {
            if (IsDisposed) return;

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            if (!IsOnVisibleScreen(Bounds))
            {
                FitToScreen(GetInitialScreen());
            }

            if (!Visible)
            {
                Show();
            }

            ShowInTaskbar = true;
            TopMost = false;
            BringToFront();
            Activate();
        }

        private static bool IsOnVisibleScreen(Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return false;
            }

            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(bounds))
                {
                    return true;
                }
            }

            return false;
        }

        private void InitWindowBehavior()
        {
            StartPosition = FormStartPosition.Manual;
            MinimumSize = new Size(1280, 720);

            biUserOptions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnScreenCapture.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbVersion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Resize += FormMainFrame_Resize;
            pnStatusBar.Resize += pnStatusBar_Resize;

            InitFullScreenButton();
            InitStatusBarLayout();

            pnlTitleBar.MouseDown += TitleBar_MouseDown;
            pnlTitleBar.MouseMove += TitleBar_MouseMove;
            pnlTitleBar.MouseUp += TitleBar_MouseUp;
            rjLabel1.MouseDown += TitleBar_MouseDown;
            rjLabel1.MouseMove += TitleBar_MouseMove;
            rjLabel1.MouseUp += TitleBar_MouseUp;

            FitToScreen(GetInitialScreen());
        }

        private void InitStatusBarLayout()
        {
            if (mainLayoutPanel.RowStyles.Count > 2)
            {
                mainLayoutPanel.RowStyles[2].Height = 31F;
            }

            EnsureStatusBarChrome();

            lbVersion.Dock = DockStyle.None;
            lbVersion.AutoSize = false;
            lbVersion.TextAlign = ContentAlignment.MiddleRight;
            lbVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            StyleStatusBarLabel(lbVersion, ContentAlignment.MiddleRight);

            lbDriveC.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lbDriveD.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            StyleStatusBarLabel(lbDriveC, ContentAlignment.MiddleLeft);
            StyleStatusBarLabel(lbDriveD, ContentAlignment.MiddleLeft);

            pgbDriveC.Visible = false;
            pgbDriveD.Visible = false;

            LayoutStatusBar();
        }

        private void InitFullScreenButton()
        {
            btnFullScreen.BringToFront();
            btnClose.BringToFront();
            btnMinimize.BringToFront();
            LayoutTitleBarButtons();
        }

        private void FormMainFrame_Resize(object sender, EventArgs e)
        {
            LayoutTitleBarButtons();
            LayoutStatusBar();
        }

        private void pnStatusBar_Resize(object sender, EventArgs e)
        {
            LayoutStatusBar();
        }

        private void LayoutStatusBar()
        {
            if (pnStatusBar.ClientSize.Width <= 0) return;

            EnsureStatusBarChrome();

            int margin = 10;
            int gap = 22;
            int driveWidth = 236;
            int barWidth = 214;
            int labelHeight = 17;
            int trackHeight = 5;
            int labelTop = 4;
            int trackTop = pnStatusBar.ClientSize.Height - trackHeight - 5;

            statusBarTopLine.SetBounds(0, 0, pnStatusBar.ClientSize.Width, 1);

            LayoutDriveStatus(lbDriveC, driveCTrack, driveWidth, barWidth, margin, labelTop, trackTop, labelHeight, trackHeight);
            LayoutDriveStatus(lbDriveD, driveDTrack, driveWidth, barWidth, margin + driveWidth + gap, labelTop, trackTop, labelHeight, trackHeight);

            int versionLeft = Math.Max(535, driveDTrack.Right + 20);
            int versionWidth = Math.Max(220, pnStatusBar.ClientSize.Width - versionLeft - margin);
            lbVersion.SetBounds(versionLeft, 4, versionWidth, 22);
            FitLabelFontToWidth(lbVersion, 11f, 8f);
        }

        private void EnsureStatusBarChrome()
        {
            if (statusBarToolTip == null)
            {
                statusBarToolTip = new ToolTip
                {
                    AutoPopDelay = 6000,
                    InitialDelay = 250,
                    ReshowDelay = 100,
                    ShowAlways = true
                };
            }

            pnStatusBar.BackColor = Color.FromArgb(30, 40, 63);

            if (statusBarTopLine == null) statusBarTopLine = CreateStatusBarPanel(Color.FromArgb(74, 91, 132));
            if (driveCTrack == null) driveCTrack = CreateDriveTrack();
            if (driveCFill == null) driveCFill = CreateDriveFill(driveCTrack);
            if (driveDTrack == null) driveDTrack = CreateDriveTrack();
            if (driveDFill == null) driveDFill = CreateDriveFill(driveDTrack);

            EnsureParent(statusBarTopLine, pnStatusBar);
            EnsureParent(driveCTrack, pnStatusBar);
            EnsureParent(driveDTrack, pnStatusBar);
            EnsureParent(driveCFill, driveCTrack);
            EnsureParent(driveDFill, driveDTrack);

            statusBarTopLine.BringToFront();
            lbDriveC.BringToFront();
            lbDriveD.BringToFront();
            lbVersion.BringToFront();
        }

        private static Panel CreateStatusBarPanel(Color backColor)
        {
            return new Panel
            {
                BackColor = backColor,
                Margin = Padding.Empty
            };
        }

        private static Panel CreateDriveTrack()
        {
            return new Panel
            {
                BackColor = Color.FromArgb(62, 75, 106),
                Margin = Padding.Empty
            };
        }

        private static Panel CreateDriveFill(Control parent)
        {
            return new Panel
            {
                BackColor = Color.FromArgb(37, 191, 115),
                Location = Point.Empty,
                Margin = Padding.Empty,
                Height = parent?.Height ?? 5
            };
        }

        private static void EnsureParent(Control child, Control parent)
        {
            if (child == null || parent == null || child.Parent == parent)
            {
                return;
            }

            parent.Controls.Add(child);
        }

        private void StyleStatusBarLabel(Label label, ContentAlignment alignment)
        {
            label.BackColor = Color.Transparent;
            label.ForeColor = Color.FromArgb(230, 238, 255);
            label.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            label.TextAlign = alignment;
        }

        private void LayoutDriveStatus(Label label, Panel track, int containerWidth, int trackWidth, int left, int labelTop, int trackTop, int labelHeight, int trackHeight)
        {
            label.SetBounds(left, labelTop, containerWidth, labelHeight);
            track.SetBounds(left + 1, trackTop, trackWidth, trackHeight);
            track.BringToFront();
        }

        private void UpdateDriveBar(Panel fill, Panel track, double percent)
        {
            if (fill == null || track == null) return;

            double clampedPercent = Math.Max(0D, Math.Min(100D, percent));
            fill.Height = track.ClientSize.Height;
            fill.Width = (int)Math.Round(track.ClientSize.Width * clampedPercent / 100D);
            fill.BackColor = GetDriveUsageColor(clampedPercent);
        }

        private static Color GetDriveUsageColor(double percent)
        {
            if (percent >= 90D)
            {
                return Color.FromArgb(236, 89, 89);
            }

            if (percent >= 75D)
            {
                return Color.FromArgb(245, 179, 69);
            }

            return Color.FromArgb(37, 191, 115);
        }

        private void SetStatusBarToolTip(Control control, string text)
        {
            statusBarToolTip?.SetToolTip(control, text);
        }

        private void FitLabelFontToWidth(Label label, float maxSize, float minSize)
        {
            if (string.IsNullOrEmpty(label.Text)) return;

            float fontSize = maxSize;
            Font baseFont = label.Font;
            Size proposedSize = new Size(label.Width, label.Height);

            while (fontSize > minSize)
            {
                using (Font testFont = new Font(baseFont.FontFamily, fontSize, baseFont.Style))
                {
                    Size textSize = TextRenderer.MeasureText(label.Text, testFont, proposedSize, TextFormatFlags.SingleLine);
                    if (textSize.Width <= label.Width && textSize.Height <= label.Height)
                    {
                        break;
                    }
                }

                fontSize -= 0.5f;
            }

            if (Math.Abs(label.Font.Size - fontSize) > 0.1f)
            {
                label.Font = new Font(baseFont.FontFamily, fontSize, baseFont.Style);
            }
        }

        private void LayoutTitleBarButtons()
        {
            if (btnFullScreen == null) return;

            int margin = 6;
            int gap = 5;
            int top = TitleBarButtonTop;
            Size buttonSize = new Size(TitleBarButtonSize, TitleBarButtonSize);
            int right = pnlTitleBar.ClientSize.Width - margin;

            btnClose.Size = buttonSize;
            btnMinimize.Size = buttonSize;
            btnFullScreen.Size = buttonSize;

            btnClose.Location = new Point(right - btnClose.Width, top);
            right = btnClose.Left - gap;

            btnFullScreen.Location = new Point(right - btnFullScreen.Width, top);
            right = btnFullScreen.Left - gap;

            btnMinimize.Location = new Point(right - btnMinimize.Width, top);
            right = btnMinimize.Left - gap;

            btnScreenCapture.Location = new Point(right - btnScreenCapture.Width, 2);
            right = btnScreenCapture.Left - gap;

            biUserOptions.Location = new Point(right - biUserOptions.Width, 4);
        }

        private void FitToCurrentScreen()
        {
            FitToScreen(GetCurrentScreen());
        }

        private Screen GetInitialScreen()
        {
            return Screen.FromPoint(Cursor.Position) ?? Screen.FromControl(this) ?? Screen.PrimaryScreen;
        }

        private void FitToScreen(Screen screen)
        {
            Rectangle workingArea = screen.WorkingArea;

            WindowState = FormWindowState.Normal;
            Bounds = workingArea;
            isFullScreenLayout = true;
        }

        private Screen GetCurrentScreen()
        {
            if (Bounds.Width > 0 && Bounds.Height > 0)
            {
                return Screen.FromRectangle(Bounds);
            }

            return Screen.FromControl(this) ?? Screen.PrimaryScreen;
        }

        private void ToggleWindowState()
        {
            if (isFullScreenLayout || WindowState == FormWindowState.Maximized)
            {
                RestoreToReducedSize();
            }
            else
            {
                FitToCurrentScreen();
            }
        }

        private void RestoreToReducedSize()
        {
            Screen screen = GetCurrentScreen();
            Rectangle workingArea = screen.WorkingArea;

            if (restoredWindowBounds == Rectangle.Empty || !workingArea.Contains(restoredWindowBounds))
            {
                int width = Math.Max(MinimumSize.Width, (int)(workingArea.Width * RestoredWindowRatio));
                int height = Math.Max(MinimumSize.Height, (int)(workingArea.Height * RestoredWindowRatio));
                int x = workingArea.Left + (workingArea.Width - width) / 2;
                int y = workingArea.Top + (workingArea.Height - height) / 2;

                restoredWindowBounds = new Rectangle(x, y, width, height);
            }

            WindowState = FormWindowState.Normal;
            Bounds = ClampBoundsToWorkingArea(restoredWindowBounds, workingArea);
            isFullScreenLayout = false;
        }

        private Rectangle ClampBoundsToWorkingArea(Rectangle bounds, Rectangle workingArea)
        {
            int width = Math.Min(bounds.Width, workingArea.Width);
            int height = Math.Min(bounds.Height, workingArea.Height);
            int x = Math.Max(workingArea.Left, Math.Min(bounds.X, workingArea.Right - width));
            int y = Math.Max(workingArea.Top, Math.Min(bounds.Y, workingArea.Bottom - height));

            return new Rectangle(x, y, width, height);
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Point screenPoint = ((Control)sender).PointToScreen(e.Location);
            bool isDoubleClick =
                (DateTime.Now - lastTitleBarClickTime).TotalMilliseconds <= SystemInformation.DoubleClickTime &&
                Math.Abs(screenPoint.X - lastTitleBarClickScreenPoint.X) <= SystemInformation.DoubleClickSize.Width &&
                Math.Abs(screenPoint.Y - lastTitleBarClickScreenPoint.Y) <= SystemInformation.DoubleClickSize.Height;

            if (isDoubleClick)
            {
                suppressTitleBarDrag = true;
                titleBarMouseDown = false;
                titleBarDragging = false;
                lastTitleBarClickTime = DateTime.MinValue;
                ToggleWindowState();
                return;
            }

            suppressTitleBarDrag = false;
            lastTitleBarClickTime = DateTime.Now;
            lastTitleBarClickScreenPoint = screenPoint;

            titleBarMouseDown = true;
            titleBarDragging = false;
            titleBarMouseDownPoint = e.Location;
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (suppressTitleBarDrag) return;
            if (!titleBarMouseDown || e.Button != MouseButtons.Left || titleBarDragging) return;

            int distanceX = Math.Abs(e.X - titleBarMouseDownPoint.X);
            int distanceY = Math.Abs(e.Y - titleBarMouseDownPoint.Y);
            if (distanceX < SystemInformation.DragSize.Width / 2 && distanceY < SystemInformation.DragSize.Height / 2) return;

            titleBarDragging = true;

            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
            }
            else if (isFullScreenLayout)
            {
                RestoreToReducedSize();
            }

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            restoredWindowBounds = Bounds;
            isFullScreenLayout = false;
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            titleBarMouseDown = false;
            titleBarDragging = false;
            suppressTitleBarDrag = false;
        }
    }
}
