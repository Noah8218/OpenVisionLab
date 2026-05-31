using KtemVisionSystem._1._Core;
using KtemVisionSystem._2._Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KtemVisionSystem
{
    public partial class FormMainDefect : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormMainDefect()
        {           
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
     
        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        public struct NumberSales
        {
            public string DateOrTime { get; set; }
            public double UnitSold { get; set; }
        }

        public List<NumberSales> NumberSalesList { get; private set; } = new List<NumberSales>();

        private bool ChangeSize = false;

        private void Form_Load(object sender, EventArgs e)
        {
            dgvDefect.DataSource = new CDefectList().GetProductsList();
            dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            for (int i = 0; i < 10; i++)
            {
                NumberSales numberSales = new NumberSales();
                numberSales.DateOrTime = i.ToString();
                numberSales.UnitSold = i * 10;
                NumberSalesList.Add(numberSales);
            }

            //chartNumberSales.Series[0].Points.DataBind(NumberSalesList, "DateOrTime", "UnitSold", "");
            //chartNumberSales.Series[1].Points.DataBind(NumberSalesList, "DateOrTime", "UnitSold", "");
        }

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }
    }
}
