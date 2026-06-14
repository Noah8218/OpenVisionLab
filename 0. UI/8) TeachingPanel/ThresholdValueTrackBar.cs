using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed class ThresholdValueTrackBar : UserControl
    {
        private readonly Panel sliderPanel;
        private readonly Label minBoundLabel;
        private readonly Label maxBoundLabel;
        private readonly Label valueLabel;
        private readonly TextBox valueTextBox;
        private int minimum = 0;
        private int maximum = 255;
        private int currentValue = 1;
        private bool dragging;
        private bool hoveringHandle;
        private bool updatingText;

        public event EventHandler ValueChanged;

        public ThresholdValueTrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            Height = 48;
            MinimumSize = new Size(360, 44);
            BackColor = Color.FromArgb(24, 33, 47);
            ForeColor = Color.White;

            minBoundLabel = CreateValueLabel("0");
            maxBoundLabel = CreateValueLabel("255");
            valueLabel = CreateCaptionLabel("Value:");
            valueTextBox = CreateValueTextBox();

            sliderPanel = new Panel
            {
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
            sliderPanel.KeyDown += OnSliderKeyDown;

            valueTextBox.Leave += OnValueTextBoxCommitted;
            valueTextBox.Enter += OnValueTextBoxEnter;
            valueTextBox.KeyDown += OnValueTextBoxKeyDown;

            Controls.Add(sliderPanel);
            Controls.Add(minBoundLabel);
            Controls.Add(maxBoundLabel);
            Controls.Add(valueLabel);
            Controls.Add(valueTextBox);

            UpdateTextBox();
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

                SetValue(currentValue, false);
                minBoundLabel.Text = minimum.ToString(CultureInfo.InvariantCulture);
            }
        }

        public int Maximum
        {
            get => maximum;
            set
            {
                maximum = Math.Max(value, minimum + 1);
                SetValue(currentValue, false);
                maxBoundLabel.Text = maximum.ToString(CultureInfo.InvariantCulture);
            }
        }

        public int Value
        {
            get => currentValue;
            set => SetValue(value, true);
        }

        public string Caption
        {
            get => valueLabel.Text;
            set => valueLabel.Text = value ?? string.Empty;
        }

        public void SetValue(int value, bool raiseEvent)
        {
            int normalized = Clamp(value);
            bool changed = currentValue != normalized;
            currentValue = normalized;
            UpdateTextBox();
            sliderPanel.Invalidate();

            if (changed && raiseEvent)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
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
                || valueLabel == null
                || valueTextBox == null)
            {
                return;
            }

            const int padding = 6;
            const int boundWidth = 28;
            const int captionWidth = 48;
            const int textWidth = 62;
            const int gap = 8;
            int height = ClientSize.Height;
            int textHeight = 24;
            int textTop = Math.Max(0, (height - textHeight) / 2);
            int labelTop = Math.Max(0, (height - 18) / 2);

            int right = ClientSize.Width - padding;
            valueTextBox.SetBounds(right - textWidth, textTop, textWidth, textHeight);
            right -= textWidth + 2;
            valueLabel.SetBounds(right - captionWidth, labelTop, captionWidth, 18);
            right -= captionWidth + gap;
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
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle track = GetTrackRectangle();
            using (Brush channelBrush = new SolidBrush(Color.FromArgb(58, 72, 92)))
            using (Brush selectedBrush = new SolidBrush(Color.FromArgb(38, 128, 143)))
            using (Pen channelBorderPen = new Pen(Color.FromArgb(86, 106, 132)))
            using (Pen selectedBorderPen = new Pen(Color.FromArgb(142, 222, 230)))
            {
                FillRoundedRectangle(e.Graphics, channelBrush, track, 7);
                DrawRoundedRectangle(e.Graphics, channelBorderPen, track, 7);

                int valueX = ValueToX(currentValue);
                int selectedRight = Math.Max(track.Left + 4, valueX);
                Rectangle selected = Rectangle.FromLTRB(track.Left, track.Top, selectedRight, track.Bottom);
                FillRoundedRectangle(e.Graphics, selectedBrush, selected, 7);
                DrawRoundedRectangle(e.Graphics, selectedBorderPen, selected, 7);

                DrawHandle(e.Graphics, valueX, track);
            }
        }

        private void DrawHandle(Graphics graphics, int x, Rectangle track)
        {
            bool highlighted = dragging || hoveringHandle;
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

            dragging = true;
            hoveringHandle = true;
            sliderPanel.Focus();
            ApplyMouseValue(e.X);
            sliderPanel.Invalidate();
        }

        private void OnSliderMouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                ApplyMouseValue(e.X);
                return;
            }

            bool hover = Math.Abs(e.X - ValueToX(currentValue)) <= 10;
            if (hoveringHandle != hover)
            {
                hoveringHandle = hover;
                sliderPanel.Invalidate();
            }
        }

        private void OnSliderMouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            hoveringHandle = Math.Abs(e.X - ValueToX(currentValue)) <= 10;
            sliderPanel.Invalidate();
        }

        private void OnSliderMouseLeave(object sender, EventArgs e)
        {
            if (MouseButtons != MouseButtons.Left)
            {
                dragging = false;
                hoveringHandle = false;
                sliderPanel.Invalidate();
            }
        }

        private void OnSliderMouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta > 0 ? 1 : -1;
            SetValue(currentValue + delta, true);
        }

        private void OnSliderKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Down)
            {
                SetValue(currentValue - 1, true);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Up)
            {
                SetValue(currentValue + 1, true);
                e.Handled = true;
            }
        }

        private void ApplyMouseValue(int x)
        {
            SetValue(XToValue(x), true);
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

            CommitTextBoxValue();
            e.SuppressKeyPress = true;
        }

        private void OnValueTextBoxCommitted(object sender, EventArgs e)
        {
            CommitTextBoxValue();
        }

        private void CommitTextBoxValue()
        {
            if (updatingText)
            {
                return;
            }

            SetValue(ParseText(valueTextBox.Text, currentValue), true);
        }

        private void OnValueTextBoxEnter(object sender, EventArgs e)
        {
            valueTextBox.SelectAll();
        }

        private static int ParseText(string text, int fallback)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : fallback;
        }

        private void UpdateTextBox()
        {
            updatingText = true;
            try
            {
                valueTextBox.Text = currentValue.ToString(CultureInfo.InvariantCulture);
            }
            finally
            {
                updatingText = false;
            }
        }
    }
}
