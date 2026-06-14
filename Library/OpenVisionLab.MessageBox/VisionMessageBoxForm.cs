using FontAwesome.Sharp;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab.MessageDialogs
{
    public partial class VisionMessageBoxForm : Form
    {
        private readonly VisionMessageOptions options;
        private Point dragPoint;
        private bool isDragging;

        public VisionMessageBoxForm()
            : this(new VisionMessageOptions())
        {
        }

        public VisionMessageBoxForm(VisionMessageOptions options)
        {
            this.options = options ?? new VisionMessageOptions();
            InitializeComponent();
            ApplyOptions();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CenterToOwnerOrScreen();
        }

        private void ApplyOptions()
        {
            Text = options.Title;
            titleLabel.Text = string.IsNullOrWhiteSpace(options.Title) ? "Message" : options.Title;
            messageLabel.Text = string.IsNullOrWhiteSpace(options.Message) ? "-" : options.Message;
            detailsTextBox.Text = options.Details ?? string.Empty;
            detailsButton.Visible = !string.IsNullOrWhiteSpace(options.Details);
            detailsPanel.Visible = false;
            TopMost = options.TopMost;

            MessageTheme theme = ResolveTheme(options.Kind);
            titlePanel.BackColor = theme.TitleColor;
            accentBar.BackColor = theme.AccentColor;
            iconBackPanel.BackColor = theme.IconBackColor;
            iconPictureBox.IconColor = theme.IconColor;
            iconPictureBox.IconChar = theme.Icon;
            primaryButton.BackColor = theme.PrimaryButtonColor;
            primaryButton.FlatAppearance.BorderColor = theme.PrimaryButtonColor;
            detailsButton.FlatAppearance.BorderColor = Color.FromArgb(119, 144, 184);
            copyDetailsButton.FlatAppearance.BorderColor = Color.FromArgb(119, 144, 184);

            ConfigureButtons();
            ResizeForMessage();
        }

        private void ConfigureButtons()
        {
            buttonFlowPanel.Controls.Clear();

            ButtonSpec[] specs = ResolveButtons();
            for (int i = specs.Length - 1; i >= 0; i--)
            {
                Button button = CreateDialogButton(specs[i], i == 0);
                buttonFlowPanel.Controls.Add(button);
                if (i == 0)
                {
                    AcceptButton = button;
                }

                if (specs[i].Result == DialogResult.Cancel
                    || specs[i].Result == DialogResult.No)
                {
                    CancelButton = button;
                }
            }
        }

        private ButtonSpec[] ResolveButtons()
        {
            return options.Buttons switch
            {
                MessageBoxButtons.OKCancel => new[]
                {
                    new ButtonSpec(options.PrimaryText ?? "OK", options.PrimaryResult ?? DialogResult.OK),
                    new ButtonSpec(options.SecondaryText ?? "Cancel", options.SecondaryResult ?? DialogResult.Cancel)
                },
                MessageBoxButtons.YesNo => new[]
                {
                    new ButtonSpec(options.PrimaryText ?? "Yes", options.PrimaryResult ?? DialogResult.Yes),
                    new ButtonSpec(options.SecondaryText ?? "No", options.SecondaryResult ?? DialogResult.No)
                },
                MessageBoxButtons.YesNoCancel => new[]
                {
                    new ButtonSpec(options.PrimaryText ?? "Yes", options.PrimaryResult ?? DialogResult.Yes),
                    new ButtonSpec(options.SecondaryText ?? "No", options.SecondaryResult ?? DialogResult.No),
                    new ButtonSpec(options.TertiaryText ?? "Cancel", options.TertiaryResult ?? DialogResult.Cancel)
                },
                MessageBoxButtons.RetryCancel => new[]
                {
                    new ButtonSpec(options.PrimaryText ?? "Retry", options.PrimaryResult ?? DialogResult.Retry),
                    new ButtonSpec(options.SecondaryText ?? "Cancel", options.SecondaryResult ?? DialogResult.Cancel)
                },
                MessageBoxButtons.AbortRetryIgnore => new[]
                {
                    new ButtonSpec(options.PrimaryText ?? "Abort", options.PrimaryResult ?? DialogResult.Abort),
                    new ButtonSpec(options.SecondaryText ?? "Retry", options.SecondaryResult ?? DialogResult.Retry),
                    new ButtonSpec(options.TertiaryText ?? "Ignore", options.TertiaryResult ?? DialogResult.Ignore)
                },
                _ => new[]
                {
                    new ButtonSpec(options.PrimaryText ?? "OK", options.PrimaryResult ?? DialogResult.OK)
                }
            };
        }

        private Button CreateDialogButton(ButtonSpec spec, bool isPrimary)
        {
            Button button = new Button
            {
                Text = spec.Text,
                DialogResult = spec.Result,
                Width = 116,
                Height = 34,
                Margin = new Padding(6, 14, 0, 0),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            if (isPrimary)
            {
                button.ForeColor = Color.White;
                button.BackColor = primaryButton.BackColor;
                button.FlatAppearance.BorderColor = primaryButton.BackColor;
            }
            else
            {
                button.ForeColor = Color.FromArgb(50, 73, 111);
                button.BackColor = Color.White;
                button.FlatAppearance.BorderColor = Color.FromArgb(119, 144, 184);
            }

            button.Click += (sender, e) =>
            {
                DialogResult = spec.Result;
                Close();
            };

            return button;
        }

        private void ResizeForMessage()
        {
            int textWidth = Math.Max(260, messageLabel.Width);
            Size measured = TextRenderer.MeasureText(
                messageLabel.Text,
                messageLabel.Font,
                new Size(textWidth, 400),
                TextFormatFlags.WordBreak);

            int detailHeight = detailsPanel.Visible ? detailsPanel.Height : 0;
            int minimumHeight = detailsPanel.Visible ? 400 : 248;
            int desiredHeight = Math.Max(minimumHeight, Math.Min(520, measured.Height + 170 + detailHeight));
            Height = desiredHeight;
        }

        private void CenterToOwnerOrScreen()
        {
            Rectangle targetBounds = Owner != null && !Owner.IsDisposed
                ? Owner.Bounds
                : Screen.FromPoint(Cursor.Position).WorkingArea;

            Location = new Point(
                targetBounds.Left + Math.Max(0, (targetBounds.Width - Width) / 2),
                targetBounds.Top + Math.Max(0, (targetBounds.Height - Height) / 2));
        }

        private static MessageTheme ResolveTheme(VisionMessageKind kind)
        {
            return kind switch
            {
                VisionMessageKind.Info => new MessageTheme(
                    Color.FromArgb(53, 70, 108),
                    Color.FromArgb(49, 151, 214),
                    Color.FromArgb(224, 243, 255),
                    Color.FromArgb(49, 151, 214),
                    IconChar.InfoCircle),
                VisionMessageKind.Success => new MessageTheme(
                    Color.FromArgb(53, 70, 108),
                    Color.FromArgb(38, 158, 105),
                    Color.FromArgb(224, 248, 238),
                    Color.FromArgb(38, 158, 105),
                    IconChar.CheckCircle),
                VisionMessageKind.Question => new MessageTheme(
                    Color.FromArgb(53, 70, 108),
                    Color.FromArgb(83, 122, 190),
                    Color.FromArgb(231, 238, 252),
                    Color.FromArgb(83, 122, 190),
                    IconChar.QuestionCircle),
                VisionMessageKind.Warning => new MessageTheme(
                    Color.FromArgb(53, 70, 108),
                    Color.FromArgb(231, 142, 41),
                    Color.FromArgb(255, 241, 220),
                    Color.FromArgb(231, 142, 41),
                    IconChar.ExclamationTriangle),
                VisionMessageKind.Error or VisionMessageKind.Stop => new MessageTheme(
                    Color.FromArgb(53, 70, 108),
                    Color.FromArgb(211, 73, 73),
                    Color.FromArgb(255, 232, 232),
                    Color.FromArgb(211, 73, 73),
                    IconChar.TimesCircle),
                _ => new MessageTheme(
                    Color.FromArgb(53, 70, 108),
                    Color.FromArgb(36, 129, 172),
                    Color.FromArgb(229, 242, 248),
                    Color.FromArgb(36, 129, 172),
                    IconChar.CommentDots)
            };
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void DetailsButton_Click(object sender, EventArgs e)
        {
            detailsPanel.Visible = !detailsPanel.Visible;
            detailsButton.Text = detailsPanel.Visible ? "Hide details" : "Details";
            ResizeForMessage();
        }

        private void CopyDetailsButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(detailsTextBox.Text))
            {
                return;
            }

            Clipboard.SetText(detailsTextBox.Text);
            copyDetailsButton.Text = "Copied";
        }

        private void DragSource_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            isDragging = true;
            dragPoint = e.Location;
        }

        private void DragSource_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || (e.Button & MouseButtons.Left) != MouseButtons.Left)
            {
                return;
            }

            Location = new Point(Left - (dragPoint.X - e.X), Top - (dragPoint.Y - e.Y));
        }

        private void DragSource_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private readonly record struct ButtonSpec(string Text, DialogResult Result);

        private readonly record struct MessageTheme(
            Color TitleColor,
            Color AccentColor,
            Color IconBackColor,
            Color IconColor,
            IconChar Icon)
        {
            public Color PrimaryButtonColor => AccentColor;
        }
    }
}
