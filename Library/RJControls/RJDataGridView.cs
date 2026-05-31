using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;
using System.Linq;

namespace RJCodeUI_M1.RJControls
{
    public class RJDataGridView : DataGridView
    {
        #region -> Fields

        private bool customizable; // Gets or sets if the control is customizable
        private bool isAlternatingRowsColor; // Gets or sets whether to apply a different background color to odd rows.
        private int borderRadius; // Gets or sets the border radius size (rounded corners)
        DataGridViewCellStyle ColumnHeaderStyle; // Sets or gets the style of the column headers.
        DataGridViewCellStyle RowHeaderStyle; // Sets or gets the style of row headers
        DataGridViewCellStyle RowStyle; // Sets or gets the Rows style

        private bool stopScroll = true; // Sets or gets if the scroll bar is moving.
        // (True = It is NOT scrolling: False = It is scrolling).
        #endregion

        #region -> Constructor

        public RJDataGridView()
        {
            // Initialize data grid view cell style objects
            ColumnHeaderStyle = new DataGridViewCellStyle();
            RowHeaderStyle = new DataGridViewCellStyle();
            RowStyle = new DataGridViewCellStyle();

            //General adjustments
            this.AllowUserToResizeRows = false;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.BackgroundColor = RJColors.LightItemBackground;
            this.BorderStyle = BorderStyle.None;
            this.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.EnableHeadersVisualStyles = false;
            this.GridColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            //this.ReadOnly = true;
            this.RightToLeft = RightToLeft.No;
            this.Size = new System.Drawing.Size(450, 250);

            // Set appearance styles in column headers
            ColumnHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ColumnHeaderStyle.BackColor = Color.MediumPurple;
            ColumnHeaderStyle.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            ColumnHeaderStyle.ForeColor = Color.White;
            ColumnHeaderStyle.WrapMode = DataGridViewTriState.True;
            ColumnHeaderStyle.Padding = new Padding(15, 0, 0, 0);
            this.ColumnHeadersDefaultCellStyle = ColumnHeaderStyle; // Set ColumnHeaderStyle
            this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ColumnHeadersHeight = 40; // Set height (for some reason it doesn't work when adding the control to the form, I put a solution in the resize event).

            // Set appearance styles on row headers
            RowHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            RowHeaderStyle.BackColor = Color.WhiteSmoke;
            RowHeaderStyle.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            RowHeaderStyle.ForeColor = Color.White;
            RowHeaderStyle.SelectionBackColor = Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            RowHeaderStyle.SelectionForeColor = SystemColors.HighlightText;
            RowHeaderStyle.WrapMode = DataGridViewTriState.False;
            this.RowHeadersDefaultCellStyle = RowHeaderStyle; // Set RowHeaderStyle
            this.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.RowHeadersWidth = 30; // Set width
            this.RowHeadersVisible = false; // Hide row header

            // Set appearance styles in rows
            RowStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            RowStyle.BackColor = RJColors.LightItemBackground;
            RowStyle.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            RowStyle.ForeColor = Color.Gray;
            RowStyle.Padding = new Padding(15, 0, 0, 0);
            RowStyle.SelectionBackColor = Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(225)))), ((int)(((byte)(247)))));
            RowStyle.SelectionForeColor = Color.Gray;
            this.RowsDefaultCellStyle = RowStyle; // Set RowStyle
            this.RowTemplate.Height = 40; // Set height
            BorderRadius = 13;

            // Attach scroll events to determine if the scroll bar stopped moving and thus redraw the control.
             HScrollBar horizontalScroll = this.Controls.OfType<HScrollBar>().First (); // Get the horizontal scroll bar.
             VScrollBar verticalalScroll = this.Controls.OfType<VScrollBar>().First (); // Get the vertical scroll bar.
             horizontalScroll.Scroll += new ScrollEventHandler (ScrollBar_Event);
             verticalalScroll.Scroll += new ScrollEventHandler (ScrollBar_Event);
        }
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        [Description("Sets or gets the height of the column header")]
        public int ColumnHeaderHeight
        {
            get { return this.ColumnHeadersHeight; }
            set
            {
                this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                if (value < 40) // Minimum height
                    this.ColumnHeadersHeight = 40;
                else
                    this.ColumnHeadersHeight = value;
            }
        }

        [Category("RJ Code Advance")]
        [Description("Set or get DataGrid view autosize column mode")]
        public DataGridViewAutoSizeColumnsMode ColumnsMode
        {
            get { return this.AutoSizeColumnsMode; }
            set { this.AutoSizeColumnsMode = value; }
        }


        [Category("RJ Code Advance")]
        [Description("Sets or gets if the control is customizable or the design will be set by appearance settings")]
        public bool Customizable
        {
            get { return customizable; }
            set { customizable = value; }
        }
        [Category("RJ Code Advance")]
        [Description("Sets or gets the background color of the column header")]
        public Color ColumnHeaderColor
        {
            get { return this.ColumnHeadersDefaultCellStyle.BackColor; }
            set { this.ColumnHeadersDefaultCellStyle.BackColor = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets the color of the column header text")]
        public Color ColumnHeaderTextColor
        {
            get { return this.ColumnHeadersDefaultCellStyle.ForeColor; }
            set { this.ColumnHeadersDefaultCellStyle.ForeColor = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets the font of the column header")]
        public Font ColumnHeaderFont
        {
            get { return this.ColumnHeadersDefaultCellStyle.Font; }
            set { this.ColumnHeadersDefaultCellStyle.Font = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets the background color of the row header")]
        public Color RowHeaderColor
        {
            get { return this.RowHeadersDefaultCellStyle.BackColor; }
            set { this.RowHeadersDefaultCellStyle.BackColor = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets the background color of the selection")]
        public Color SelectionBackColor
        {
            get { return this.RowHeadersDefaultCellStyle.SelectionBackColor; }
            set
            {
                this.RowHeadersDefaultCellStyle.SelectionBackColor = value;
                this.RowsDefaultCellStyle.SelectionBackColor = value;
            }
        }

        [Category("RJ Code Advance")]
        [Description("Set or get row color")]
        public Color RowsColor
        {
            get { return this.RowsDefaultCellStyle.BackColor; }
            set { this.RowsDefaultCellStyle.BackColor = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets whether to apply a background color to odd rows.")]
        public bool AlternatingRowsColorApply
        {
            get { return isAlternatingRowsColor; }
            set
            {
                isAlternatingRowsColor = value;
                if (value == false) // If the value is false, restore the default color (Color.Empty)
                {
                    this.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty;
                }
            }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets the color of alternate rows")]
        public Color AlternatingRowsColor
        {
            get { return this.AlternatingRowsDefaultCellStyle.BackColor; }
            set
            {
                if (isAlternatingRowsColor)
                    this.AlternatingRowsDefaultCellStyle.BackColor = value;
            }
            // Warning: this property can cause a performance hit when you have many rows.
        }

        [Category("RJ Code Advance")]
        [Description("Set or get the color of the text in the rows")]
        public Color RowsTextColor
        {
            get { return this.RowsDefaultCellStyle.ForeColor; }
            set { this.RowsDefaultCellStyle.ForeColor = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets the color of the selected row")]
        public Color SelectionTextColor
        {
            get { return this.RowsDefaultCellStyle.SelectionForeColor; }
            set { this.RowsDefaultCellStyle.SelectionForeColor = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets the height of the rows")]
        public int RowHeight
        {
            get { return this.RowTemplate.Height; }
            set { this.RowTemplate.Height = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Sets or gets the Background Color of the DataGridView")]
        public Color DgvBackColor
        {
            get { return this.BackgroundColor; }
            set { this.BackgroundColor = value; }
        }

        [Category("RJ Code Advance")]
        [Description("Gets or sets the border radius")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                borderRadius = value;
                this.Invalidate(); // Redraw the control to update the appearance.
            }
        }
        #endregion

        #region -> Private methods

        private void ApplyAppearanceSettings()
        {
            if (customizable == false) // if not customizable, apply appearance settings.
            {
                if (UIAppearance.Theme == UITheme.Dark) // If the theme is dark, set the following properties
                {
                    DgvBackColor = RJColors.DarkItemBackground; //Background color
                    RowsColor = RJColors.DarkItemBackground; // Color of the rows
                    ColumnHeaderColor = RJColors.DarkActiveBackground; // background color column headings
                    ColumnHeaderTextColor = Color.Gainsboro; // Text color of column headers.
                    RowsTextColor = UIAppearance.PrimaryTextColor; // Color of the text rows
                    this.GridColor = UIAppearance.BackgroundColor; // Grid color

                    if (isAlternatingRowsColor) // If color alternate rows, set a color for the color of alternate rows
                    {

                        AlternatingRowsColor = Utils.ColorEditor.Lighten(RJColors.DarkActiveBackground, 5); // Background color of alternate rows
                        SelectionBackColor = Utils.ColorEditor.Lighten(UIAppearance.StyleColor, 30); // Background color of the selection.
                        SelectionTextColor = Color.White; // Selection text color.
                    }
                    else // Normal row color
                    {
                        SelectionBackColor = Utils.ColorEditor.Lighten(RJColors.DarkActiveBackground, 5); // Background color of the selection.
                        SelectionTextColor = UIAppearance.PrimaryTextColor; // Selection text color.
                    }

                }

                else // If the topic is clear, set the following properties
                {
                    DgvBackColor = RJColors.LightItemBackground; // Background color
                    RowsColor = RJColors.LightItemBackground; // Color of the rows
                    ColumnHeaderColor = UIAppearance.StyleColor; // background color column headings
                    ColumnHeaderTextColor = Color.WhiteSmoke; // text color column headings.
                    RowsTextColor = UIAppearance.TextColor; // Color of the text of the rows
                    this.GridColor = Color.Gainsboro; // Grid color

                    if (isAlternatingRowsColor) // If color alternate rows, set a color for the color of alternate rows
                    {
                        AlternatingRowsColor = Utils.ColorEditor.Lighten(UIAppearance.StyleColor, 80); // Background color of alternate rows
                        SelectionBackColor = Utils.ColorEditor.Lighten(UIAppearance.StyleColor, 40); // Background color of the selection.
                        SelectionTextColor = Color.White; // Selection text color.
                    }
                    else // Normal row color
                    {
                        SelectionBackColor = Utils.ColorEditor.Lighten(UIAppearance.StyleColor, 80); // Background color of the selection.
                        SelectionTextColor = UIAppearance.TextColor; // Selection text color.
                    }
                }
            }
        }
        #endregion

        #region -> Event methods
        private void ScrollBar_Event(object sender, ScrollEventArgs e)
        {
            // If the scroll type is EndScroll, save the value and redraw the control.
            if (e.Type == ScrollEventType.EndScroll)
            {
                stopScroll = true;
                this.Invalidate();
            }
            // Otherwise, indicate that the scroll bar is moving.
            else
            {
                stopScroll = false;
            }
        }
        #endregion

        #region -> Overridden methods

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // It is responsible for applying the appearance settings at runtime.
            // and validate the minimum height of the column header.
            if (!this.DesignMode)
                ApplyAppearanceSettings(); // Apply UI appearance settings

            if (this.ColumnHeadersHeight < 40) // Set minimum height
            {
                ColumnHeaderHeight = 40;
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {// Responsible for applying the rounded corners + border smoothing
            base.OnPaint(e);

            if (stopScroll == true) // If the scroll bar is not moving apply and smooth the rounded border.
                Utils.RoundedControl.RegionAndSmoothed(this, borderRadius, e.Graphics);
        }
        #endregion

    }
}
