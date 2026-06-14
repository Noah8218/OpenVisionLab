using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed class ThresholdRangeTrackBar : UserControl
    {
        private readonly Panel sliderPanel;
        private readonly Label minBoundLabel;
        private readonly Label maxBoundLabel;
        private readonly Label minLabel;
        private readonly Label maxLabel;
        private readonly TextBox minTextBox;
        private readonly TextBox maxTextBox;
        private readonly CheckBox invertCheckBox;
        private int minimum = 0;
        private int maximum = 255;
        private int rangeMin = 30;
        private int rangeMax = 255;
        private DragHandle activeHandle = DragHandle.None;
        private DragHandle hoverHandle = DragHandle.None;
        private bool updatingText;

        public event EventHandler RangeChanged;

        public ThresholdRangeTrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            Height = 48;
            MinimumSize = new Size(420, 44);
            BackColor = Color.FromArgb(24, 33, 47);
            ForeColor = Color.White;

            minBoundLabel = CreateValueLabel("0");
            maxBoundLabel = CreateValueLabel("255");
            minLabel = CreateCaptionLabel("Min:");
            maxLabel = CreateCaptionLabel("Max:");
            minTextBox = CreateValueTextBox();
            maxTextBox = CreateValueTextBox();
            invertCheckBox = new CheckBox
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                Text = "Invert",
                ForeColor = Color.FromArgb(236, 242, 248),
                BackColor = BackColor,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point),
                CheckAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft
            };

            sliderPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackColor,
                Cursor = Cursors.Hand,
                TabStop = true
            };
            sliderPanel.Paint += OnSliderPaint;
            sliderPanel.MouseDown += OnSliderMouseDown;
            sliderPanel.MouseMove += OnSliderMouseMove;
            sliderPanel.MouseUp += OnSliderMouseUp;
            sliderPanel.MouseLeave += OnSliderMouseLeave;
            sliderPanel.MouseWheel += OnSliderMouseWheel;

            minTextBox.Leave += OnValueTextBoxCommitted;
            maxTextBox.Leave += OnValueTextBoxCommitted;
            minTextBox.Enter += OnValueTextBoxEnter;
            maxTextBox.Enter += OnValueTextBoxEnter;
            minTextBox.KeyDown += OnValueTextBoxKeyDown;
            maxTextBox.KeyDown += OnValueTextBoxKeyDown;
            invertCheckBox.CheckedChanged += (sender, e) => OnRangeChanged();

            Controls.Add(sliderPanel);
            Controls.Add(minBoundLabel);
            Controls.Add(maxBoundLabel);
            Controls.Add(minLabel);
            Controls.Add(minTextBox);
            Controls.Add(maxLabel);
            Controls.Add(maxTextBox);
            Controls.Add(invertCheckBox);

            UpdateTextBoxes();
        }

        public int Minimum
        {
            get => minimum;
            set
            {
                minimum = value;
                if (maximum <= minimum)
                {
                    maximum = minimum + 1;
                }

                SetRange(rangeMin, rangeMax, false);
                minBoundLabel.Text = minimum.ToString(CultureInfo.InvariantCulture);
            }
        }

        public int Maximum
        {
            get => maximum;
            set
            {
                maximum = Math.Max(value, minimum + 1);
                SetRange(rangeMin, rangeMax, false);
                maxBoundLabel.Text = maximum.ToString(CultureInfo.InvariantCulture);
            }
        }

        public int RangeMin
        {
            get => rangeMin;
            set => SetRange(value, rangeMax, true);
        }

        public int RangeMax
        {
            get => rangeMax;
            set => SetRange(rangeMin, value, true);
        }

        public bool Invert
        {
            get => invertCheckBox.Checked;
            set => invertCheckBox.Checked = value;
        }

        public void SetRange(int min, int max, bool raiseEvent)
        {
            int normalizedMin = Clamp(min);
            int normalizedMax = Clamp(max);
            if (normalizedMin > normalizedMax)
            {
                int temp = normalizedMin;
                normalizedMin = normalizedMax;
                normalizedMax = temp;
            }

            bool changed = rangeMin != normalizedMin || rangeMax != normalizedMax;
            rangeMin = normalizedMin;
            rangeMax = normalizedMax;
            UpdateTextBoxes();
            sliderPanel.Invalidate();

            if (changed && raiseEvent)
            {
                OnRangeChanged();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            sliderPanel?.Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            if (sliderPanel == null
                || minBoundLabel == null
                || maxBoundLabel == null
                || minLabel == null
                || maxLabel == null
                || minTextBox == null
                || maxTextBox == null
                || invertCheckBox == null)
            {
                return;
            }

            const int padding = 6;
            const int boundWidth = 28;
            const int labelWidth = 34;
            const int textWidth = 62;
            const int invertWidth = 78;
            const int gap = 8;
            int height = ClientSize.Height;
            int textHeight = 24;
            int textTop = Math.Max(0, (height - textHeight) / 2);
            int labelTop = Math.Max(0, (height - 18) / 2);

            int right = ClientSize.Width - padding;
            invertCheckBox.SetBounds(right - invertWidth, 0, invertWidth, height);
            right -= invertWidth + gap;
            maxTextBox.SetBounds(right - textWidth, textTop, textWidth, textHeight);
            right -= textWidth + 2;
            maxLabel.SetBounds(right - labelWidth, labelTop, labelWidth, 18);
            right -= labelWidth + gap;
            minTextBox.SetBounds(right - textWidth, textTop, textWidth, textHeight);
            right -= textWidth + 2;
            minLabel.SetBounds(right - labelWidth, labelTop, labelWidth, 18);
            right -= labelWidth + gap;
            maxBoundLabel.SetBounds(right - boundWidth, labelTop, boundWidth, 18);
            right -= boundWidth + gap;

            minBoundLabel.SetBounds(padding, labelTop, boundWidth, 18);
            int sliderLeft = minBoundLabel.Right + gap;
            int sliderWidth = Math.Max(24, right - sliderLeft);
            sliderPanel.SetBounds(sliderLeft, 5, sliderWidth, Math.Max(24, height - 10));
        }

        private static Label CreateValueLabel(string text)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Text = text,
                ForeColor = Color.FromArgb(236, 242, 248),
                BackColor = Color.FromArgb(24, 33, 47),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
        }

        private static Label CreateCaptionLabel(string text)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Text = text,
                ForeColor = Color.FromArgb(236, 242, 248),
                BackColor = Color.FromArgb(24, 33, 47),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false
            };
        }

        private static TextBox CreateValueTextBox()
        {
            return new TextBox
            {
                Margin = Padding.Empty,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(34, 43, 56),
                ForeColor = Color.FromArgb(245, 248, 252),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point),
                TextAlign = HorizontalAlignment.Center
            };
        }

        private void OnSliderPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle track = GetTrackRectangle();
            using (Brush channelBrush = new SolidBrush(Color.FromArgb(58, 72, 92)))
            using (Brush selectedBrush = new SolidBrush(Color.FromArgb(38, 128, 143)))
            using (Pen channelBorderPen = new Pen(Color.FromArgb(86, 106, 132)))
            using (Pen selectedBorderPen = new Pen(Color.FromArgb(142, 222, 230)))
            {
                FillRoundedRectangle(e.Graphics, channelBrush, track, 7);
                DrawRoundedRectangle(e.Graphics, channelBorderPen, track, 7);

                int minX = ValueToX(rangeMin);
                int maxX = ValueToX(rangeMax);
                int selectedLeft = Math.Min(minX, maxX);
                int selectedRight = Math.Max(minX, maxX);
                if (selectedRight - selectedLeft < 4)
                {
                    selectedLeft = Math.Max(track.Left, selectedLeft - 2);
                    selectedRight = Math.Min(track.Right, selectedRight + 2);
                }

                Rectangle selected = Rectangle.FromLTRB(selectedLeft, track.Top, selectedRight, track.Bottom);
                FillRoundedRectangle(e.Graphics, selectedBrush, selected, 7);
                DrawRoundedRectangle(e.Graphics, selectedBorderPen, selected, 7);

                DrawHandle(e.Graphics, minX, track, DragHandle.Min);
                DrawHandle(e.Graphics, maxX, track, DragHandle.Max);
            }
        }

        private void DrawHandle(Graphics graphics, int x, Rectangle track, DragHandle handle)
        {
            bool highlighted = activeHandle == handle || hoverHandle == handle;
            Rectangle bounds = new Rectangle(x - 7, track.Top - 8, 14, track.Height + 16);
            Color fill = highlighted
                ? Color.FromArgb(246, 252, 255)
                : Color.FromArgb(236, 241, 247);
            Color border = highlighted
                ? Color.FromArgb(142, 222, 230)
                : Color.FromArgb(36, 44, 54);
            using (Brush handleBrush = new SolidBrush(fill))
            using (Pen handlePen = new Pen(border, highlighted ? 2.4F : 1.6F))
            {
                FillRoundedRectangle(graphics, handleBrush, bounds, 4);
                DrawRoundedRectangle(graphics, handlePen, bounds, 4);
            }
        }

        private Rectangle GetTrackRectangle()
        {
            int height = Math.Max(12, Math.Min(16, sliderPanel.Height - 18));
            int y = Math.Max(4, (sliderPanel.Height - height) / 2);
            return new Rectangle(8, y, Math.Max(20, sliderPanel.Width - 16), height);
        }

        private void OnSliderMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            int minDistance = Math.Abs(e.X - ValueToX(rangeMin));
            int maxDistance = Math.Abs(e.X - ValueToX(rangeMax));
            activeHandle = minDistance <= maxDistance ? DragHandle.Min : DragHandle.Max;
            hoverHandle = activeHandle;
            sliderPanel.Focus();
            sliderPanel.Invalidate();
            ApplyMouseValue(e.X);
        }

        private void OnSliderMouseMove(object sender, MouseEventArgs e)
        {
            if (activeHandle == DragHandle.None)
            {
                DragHandle newHoverHandle = GetNearestHandle(e.X);
                if (hoverHandle != newHoverHandle)
                {
                    hoverHandle = newHoverHandle;
                    sliderPanel.Invalidate();
                }

                return;
            }

            ApplyMouseValue(e.X);
        }

        private void OnSliderMouseUp(object sender, MouseEventArgs e)
        {
            activeHandle = DragHandle.None;
            hoverHandle = GetNearestHandle(e.X);
            sliderPanel.Invalidate();
        }

        private void OnSliderMouseLeave(object sender, EventArgs e)
        {
            if (MouseButtons != MouseButtons.Left)
            {
                activeHandle = DragHandle.None;
                hoverHandle = DragHandle.None;
                sliderPanel.Invalidate();
            }
        }

        private void OnSliderMouseWheel(object sender, MouseEventArgs e)
        {
            DragHandle targetHandle = hoverHandle == DragHandle.None
                ? GetNearestHandle(e.X)
                : hoverHandle;
            int delta = e.Delta > 0 ? 1 : -1;
            if (targetHandle == DragHandle.Min)
            {
                SetRange(rangeMin + delta, rangeMax, true);
            }
            else if (targetHandle == DragHandle.Max)
            {
                SetRange(rangeMin, rangeMax + delta, true);
            }
        }

        private void ApplyMouseValue(int x)
        {
            int value = XToValue(x);
            if (activeHandle == DragHandle.Min)
            {
                SetRange(Math.Min(value, rangeMax), rangeMax, true);
            }
            else if (activeHandle == DragHandle.Max)
            {
                SetRange(rangeMin, Math.Max(value, rangeMin), true);
            }
        }

        private int ValueToX(int value)
        {
            Rectangle track = GetTrackRectangle();
            double ratio = (Clamp(value) - minimum) / (double)(maximum - minimum);
            return track.Left + (int)Math.Round(track.Width * ratio);
        }

        private int XToValue(int x)
        {
            Rectangle track = GetTrackRectangle();
            double ratio = (x - track.Left) / (double)Math.Max(1, track.Width);
            int value = minimum + (int)Math.Round((maximum - minimum) * ratio);
            return Clamp(value);
        }

        private int Clamp(int value)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private DragHandle GetNearestHandle(int x)
        {
            int minDistance = Math.Abs(x - ValueToX(rangeMin));
            int maxDistance = Math.Abs(x - ValueToX(rangeMax));
            return minDistance <= maxDistance ? DragHandle.Min : DragHandle.Max;
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            if (bounds.Width <= 1 || bounds.Height <= 1)
            {
                GraphicsPath emptyPath = new GraphicsPath();
                emptyPath.AddRectangle(bounds);
                emptyPath.CloseFigure();
                return emptyPath;
            }

            radius = Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2);
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            if (diameter <= 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(bounds, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        private static void DrawRoundedRectangle(Graphics graphics, Pen pen, Rectangle bounds, int radius)
        {
            Rectangle adjustedBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            using (GraphicsPath path = CreateRoundedRectanglePath(adjustedBounds, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void OnValueTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            CommitTextBoxValues();
            e.SuppressKeyPress = true;
        }

        private void OnValueTextBoxCommitted(object sender, EventArgs e)
        {
            CommitTextBoxValues();
        }

        private void CommitTextBoxValues()
        {
            if (updatingText)
            {
                return;
            }

            int min = ParseText(minTextBox.Text, rangeMin);
            int max = ParseText(maxTextBox.Text, rangeMax);
            SetRange(min, max, true);
        }

        private void OnValueTextBoxEnter(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private static int ParseText(string text, int fallback)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : fallback;
        }

        private void UpdateTextBoxes()
        {
            updatingText = true;
            try
            {
                minTextBox.Text = rangeMin.ToString(CultureInfo.InvariantCulture);
                maxTextBox.Text = rangeMax.ToString(CultureInfo.InvariantCulture);
            }
            finally
            {
                updatingText = false;
            }
        }

        private void OnRangeChanged()
        {
            RangeChanged?.Invoke(this, EventArgs.Empty);
        }

        private enum DragHandle
        {
            None,
            Min,
            Max
        }
    }
}
