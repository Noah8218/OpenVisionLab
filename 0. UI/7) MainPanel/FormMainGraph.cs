using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OpenVisionLab
{
    public partial class FormMainGraph : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        

        public FormMainGraph()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;

            Global.Thread.CSeqVision.EventSeqComplete += OnInspResult;
            InitGraph();
        }

        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        private bool ChangeSize = false;

        private void Form_Load(object sender, EventArgs e)
        {
            this.Resize += FormMainGraph_Resize;                        

        }

        private void OnInspResult(object sender, EventArgs e)
        {
            if (!(e is InspResultArgs args)) { return; }
                this.UIThreadBeginInvoke(() =>
            {
                if (Global.System.Menu == CSystem.MENU.MAIN)
                {
                    for (int i = 0; i < args.avgMM.Count; i++)
                    {
                        if (args.avgMM[i] == 0) { continue; }
                        if (args.avgMM.Count < 2) { break; }
                        CGlobal.Inst.Data.GraphList[args.Index].Push(new CGraphData(args.avgMM[i], args.Index), i, true);
                    }

                    if (this.Visible) { CGlobal.Inst.Data.GraphList[args.Index].UpdateGraph(); }
                        
                }
            });          
        }


        private void FormMainGraph_Resize(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                for (int i = 0; i < Global.Data.GraphList.Count; i++)
                {
                    Global.Data.GraphList[i].UpdateGraph();
                }
            }
            

            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }

        private bool InitGraph()
        {
            try
            {
                AddGraph();
                UpdateGraph();
            }
            catch (Exception Desc)
            {
                MessageBox.Show(Desc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void AddGraph()
        {
            Global.Data.GraphList.Clear();

            for(int i= 0; i < Global.Device.CAMERA_COUNT;i++)
            {
                switch(i)
                {
                    case DEFINE.CAM_1:
                        CMvcGraph graph_pitch1 = new CMvcGraph(0);
                        Global.Data.GraphList.Add(graph_pitch1);
                        break;
                    case DEFINE.CAM_2:
                        CMvcGraph graph_pitch2 = new CMvcGraph(1);
                        Global.Data.GraphList.Add(graph_pitch2);
                        break;
                }
            }
        }

        private bool UpdateGraph()
        {
            try
            {
                GraphPanel.Controls.Clear();
                int nTotalWidth = GraphPanel.Width;
                int nTotalHeight = GraphPanel.Height;

                int nGraphWidth = nTotalWidth;
                int nGraphHeight = nTotalHeight / Global.Data.GraphList.Count - 2;

                for (int i = 0; i < Global.Data.GraphList.Count; i++)
                {
                    GraphPanel.Controls.Add(Global.Data.GraphList[i]);

                    Global.Data.GraphList[i].Config.IsVisibleName = true;
                    Global.Data.GraphList[i].Dock = DockStyle.None;
                    Global.Data.GraphList[i].Visible = true;
                    Global.Data.GraphList[i].Size = new System.Drawing.Size(nGraphWidth, nGraphHeight);
                    Global.Data.GraphList[i].Location = new System.Drawing.Point(0, i * nGraphHeight);

                    switch(i)
                    {
                        case DEFINE.CAM_1:
                            Global.Data.GraphList[i].AddLine(0, "TOP_REE1", "mm(Avg)", Color.Yellow);
                            Global.Data.GraphList[i].AddLine(1, "TOP_REE2", "mm(Avg)", Color.Blue);
                            Global.Data.GraphList[i].AddLine(2, "TOP_REE3", "mm(Avg)", Color.Red);
                            break;
                        case DEFINE.CAM_2:
                            Global.Data.GraphList[i].AddLine(0, "BTM_REE1", "mm(Avg)", Color.Yellow);
                            Global.Data.GraphList[i].AddLine(1, "BTM_REE2", "mm(Avg)", Color.Blue);
                            Global.Data.GraphList[i].AddLine(2, "BTM_REE3", "mm(Avg)", Color.Red);
                            break;
                    }

                    //Global.Data.GraphList[i].AddLine(1, "Test2", "각도2", Color.Blue);
                    //Global.Data.GraphList[i].AddLine(2, "Test3", "각도3", Color.Red);
                }

                for (int i = 0; i < Global.Data.GraphList.Count; i++)
                {
                    Global.Data.GraphList[i].Config.ReadInitFile();
                    Global.Data.GraphList[i].UpdateStyles(Global.Data.GraphList[i].Config);
                    Global.Data.GraphList[i].UpdateGraph();
                }
            }
            catch (Exception Desc)
            {
                return false;
            }

            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void FormMainGraph_Activated(object sender, EventArgs e)
        {
            //for (int i = 0; i < Global.Data.GraphList.Count; i++)
            //{                
            //    Global.Data.GraphList[i].UpdateGraph();
            //}
        }
    }
}
