using RJCodeUI_M1.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace RJCodeUI_M1.RJControls
{
    public class RJChart : Chart
    {
        /// <summary>
        /// This class enherits from <see cref="Chart"/> class.
        /// This control simply sets the colors from the appearance settings.
        /// </summary>
        /// 

        #region -> Private methods
        private void ApplyAppearanceSettings()
        {
            this.BackColor = UIAppearance.ItemBackgroundColor;//Set Background Color

            foreach (var serie in this.Series)
            {
                if (serie.ChartType == SeriesChartType.Doughnut || serie.ChartType == SeriesChartType.Pie)
                {
                    serie.LabelForeColor = Color.White;
                    serie.BorderWidth = 1;
                    serie.BorderColor = UIAppearance.ItemBackgroundColor;
                }
                else
                {
                    serie.LabelForeColor = UIAppearance.TextColor;
                }
            }
            foreach (var chartArea in this.ChartAreas)//Set colors for chart areas
            {
                chartArea.BackColor = UIAppearance.ItemBackgroundColor;//Set chart area background color
                //X axis of the area chart
                chartArea.AxisX.LabelStyle.ForeColor = UIAppearance.TextColor;//Text color
                chartArea.AxisX.LineColor = Utils.ColorEditor.Lighten(UIAppearance.TextColor, 30);//Line color
                chartArea.AxisX.LineWidth = 2;//Line widht
                chartArea.AxisX.MajorTickMark.LineColor = Utils.ColorEditor.Lighten(UIAppearance.TextColor, 30);//Major tick mark line color
                chartArea.AxisX.MajorTickMark.LineWidth = 2;//Major tick mark line widht
                chartArea.AxisX.MajorGrid.LineColor = UIAppearance.ItemBackgroundColor;//Major grid line color
                chartArea.AxisY.MinorGrid.LineColor = UIAppearance.ActiveBackgroundColor;//Minor grid line color

                //Y axis of the area chart
                chartArea.AxisY.LabelStyle.ForeColor = UIAppearance.TextColor;//Text color
                chartArea.AxisY.LineColor = Utils.ColorEditor.Lighten(UIAppearance.TextColor, 30);//Line color
                chartArea.AxisY.LineWidth = 2;//Line widht
                chartArea.AxisY.MajorTickMark.LineColor = Utils.ColorEditor.Lighten(UIAppearance.TextColor, 30);//Major tick mark line color
                chartArea.AxisY.MajorTickMark.LineWidth = 2;//Major tick mark line widht
                chartArea.AxisY.MajorGrid.LineColor = UIAppearance.ActiveBackgroundColor;//Major grid line color
                chartArea.AxisY.MinorGrid.LineColor = UIAppearance.ActiveBackgroundColor;//Minor grid line color

            }
            foreach (var legend in this.Legends) //Set colors for legends
            {
                legend.BackColor = UIAppearance.ItemBackgroundColor;
                legend.ForeColor = UIAppearance.TextColor;
                legend.Font = new Font(legend.Font.FontFamily, 8.5F);
            }
            foreach (var title in this.Titles) //Set colors for titles
            {
                if (UIAppearance.Theme == UITheme.Dark)
                    title.ForeColor = Utils.ColorEditor.Lighten(UIAppearance.TextColor, 30);
                else title.ForeColor = Utils.ColorEditor.Darken(UIAppearance.TextColor, 10);
                title.Font = new Font("Verdana", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                title.Alignment = ContentAlignment.MiddleLeft;
            }
        }
        #endregion

        #region -> Overridden methods
        protected override void OnHandleCreated(EventArgs e)
        {   //Override OnHandleCreated event, occurs when a handle is created,
            //this event is the one that most closely resembles the load event
            base.OnHandleCreated(e);
            ApplyAppearanceSettings();//Apply appearance settings
        }
        #endregion

    }
}
