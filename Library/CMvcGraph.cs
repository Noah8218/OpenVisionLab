using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Drawing.Text;
using Lib.Common;

namespace OpenVisionLab
{  
    public partial class CMvcGraph : UserControl
    {
        /// <summary>
        /// 그래프에 그려지는 LINE객체
        /// </summary>
        public class Line
        {
            public List<CGraphData> Datas = new List<CGraphData>();
            public Color DrawColor = Color.Green;
            public string m_NameID = "IR";
            public int NumID = -1;
            public string UnitName = "";
            public string Desc = "";

            public Line(string name) => m_NameID = name;
            public Line(int num) => NumID = num;
        }

        private CGraphParam m_iGraphConfig = new CGraphParam();
        public CGraphParam Config
        {
            get { return m_iGraphConfig; }
            set
            {
                m_iGraphConfig = value;
                UpdateStyles(m_iGraphConfig);
            }
        }

        private bool GraphMouseOn { get; set; } = false;
        private int Index { get; set; } = 0;
        private int PrePosition { get; set; } = 0;
        public int WarningCount { get; set; } = 0;
        public int AlaramCount { get; set; } = 0;        
        private double m_LineInterval = 10;
        private float ZoomScale = 1.0F;
        private int GridSize { get; set; } = 8;
        private float OffsetX = 0; // Max/Min의 값들이 표시되는 영역 사이즈
        private float OffsetY = 20;
        private int DulplexPointCount = 1;

        private List<Line> Lines = new List<Line>();
        private PointF lastPoint = new PointF();
        private Point MousePoint = new Point();

        public double LineInterval
        {
            set
            {
                if (m_LineInterval != value)
                {
                    m_LineInterval = value;
                    Refresh();
                }
            }
            get { return m_LineInterval; }
        }


        public CMvcGraph(int nIndex)
        {
            InitializeComponent();
            InitializeStyles();

            Index = nIndex;

            UpdateStyles(Config);

            Config.SetIndex(Index);
            Config.ReadInitFile();

            this.MouseWheel += OnGraphMouseWheel;
            this.MouseMove += OnGraphMouseMove;
            this.MouseDown += OnGraphMouseDown;
            this.MouseUp += OnGraphMouseUp;
            this.MouseLeave += On_GraphMouseLeave;
            
            this.MouseDoubleClick += IntelligentGraph_MouseDoubleClick;
            this.Scroll += IntelligentGraph_Scroll;

            //this = true;
            this.AutoScroll = true;
            this.AutoScrollMargin = new Size(100, 50);            
            this.AutoScrollMinSize = new Size(200000, 50);
            this.VerticalScroll.Enabled = false;
            this.VerticalScroll.Visible = false;

            ScrollMaxCount = Config.ListMaxCount;
        }

        private void IntelligentGraph_Scroll(object sender, ScrollEventArgs e)
        {
            this.Refresh();
        }

        private void IntelligentGraph_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CMvcGraph iGraph = sender as CMvcGraph;

            if (e.Button == MouseButtons.Left)
            {
                ScrollMaxCount = Config.ListMaxCount;
                this.AutoScrollMinSize = new Size(200000, 50);

                MousePoint = new Point();
                Config.GapX = 10;
                LineInterval = 10;
                foreach (Line line in Lines)
                {
                    this.AutoScrollMinSize = new Size(200000, 50);
                    int ScrllX = this.AutoScrollPosition.X * -1;                                        
                    int PositionX = (int)(this.Width * 0.5) + ScrllX;

                    double newX = (OffsetX) + (double)(line.Datas.Count * m_LineInterval);

                    this.AutoScrollPosition = new Point((int)lastPoint.X, 0);

                    AlaramCount = ScrllX;
                }
            }
            else
            {
                FormPowerGraphConfig formPowerGraphConfig = new FormPowerGraphConfig(iGraph.Config);
                ShowForm(formPowerGraphConfig);
            }
        }        

        private void On_GraphMouseLeave(object sender, EventArgs e)
        {
            MousePoint = new Point();
        }

        private void ShowForm(Form form)
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            form.TopLevel = true;
            form.TopMost = true;
            form.StartPosition = FormStartPosition.CenterParent;
            if (!CUtil.OpenCheckForm(form)) return;
            form.Show();
        }

        private void InitializeStyles()
        {
            BackColor = Color.Black;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer| ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        public void UpdateStyles(CGraphParam config)
        {
            Config.TextColor = config.TextColor;
            Config.CurrentDataColor = config.CurrentDataColor;
            Config.BackgroundColor = config.BackgroundColor;
            Config.GridColor = config.GridColor;
            Config.SpecInColor = config.SpecInColor;
            Config.SpecOutColor = config.SpecOutColor;
            Config.WarningLineColor = config.WarningLineColor;
            Config.AlarmLineColor = config.AlarmLineColor;
            Config.BorderLineColor = config.BorderLineColor;

            Config.IsViewModeLine = config.IsViewModeLine;
            Config.FontSize = config.FontSize;

            Config.Max = config.Max;
            Config.Min = config.Min;
            Config.IsLimitPeak = config.IsLimitPeak;

            LineInterval = config.GapX;
            this.BackColor = Config.BackgroundColor;

            //this.Update();
        }

        private void OnGraphMouseUp(object sender, MouseEventArgs e)
        {
            CMvcGraph Graph = sender as CMvcGraph;
            if (e.Button == MouseButtons.Left)
            {
                Graph.GraphMouseOn = false;
            }            
        }

        private void OnGraphMouseDown(object sender, MouseEventArgs e)
        {
            CMvcGraph Graph = sender as CMvcGraph;
            if (e.Button == MouseButtons.Left)
            {
               Graph.GraphMouseOn = true;
            }            
        }

        private double gapLimitX = 25;
        private int ScrollMaxCount { get; set; } = 3000;

        private void OnGraphMouseMove(object sender, MouseEventArgs e)
        {
            CMvcGraph Graph = sender as CMvcGraph;

            int ScrllX = this.AutoScrollPosition.X * -1;

            MousePoint = e.Location;
            MousePoint = new Point(MousePoint.X + ScrllX, MousePoint.Y);

            if (Graph.GraphMouseOn)
            {
                int nPosition = Graph.PrePosition - e.Location.X;

                if (nPosition > 0)
                {
                    if(gapLimitX > Graph.Config.GapX)
                    {
                        Graph.Config.GapX = Graph.Config.GapX + 0.05;
                        Graph.PrePosition = e.Location.X;                        
                    }

                    ScrollMaxCount = ScrollMaxCount + 100;
                    this.AutoScrollMinSize = new Size(200000, 50);
                }
                else
                {
                    if (Graph.Config.GapX > 1)
                    {

                        if (Graph.Config.GapX < 1)
                        {
                            Graph.Config.GapX = 1;                            
                        }
                        else 
                        {
                            Graph.Config.GapX = Graph.Config.GapX - 0.05;

                            ScrollMaxCount = ScrollMaxCount - 10;

                            this.AutoScrollMinSize = new Size(200000, 50);
                        }
                    }
                    Graph.PrePosition = e.Location.X;
                }

                Graph.UpdateStyles(Graph.Config);
            }
            //this.Invalidate();

            this.Invalidate();
            this.Update();
        }

        private void OnGraphMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CMvcGraph Graph = sender as CMvcGraph;
            this.Refresh();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            //Refresh();
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {            
            Graphics g = e.Graphics;
            CDrawBitmap.ToLowQuality(g);
            e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            
            OffsetX = 0;

            DrawLabels(ref g);
            DrawGrid(ref g);
            DrawLines(ref g);
            DrawLimitLine(ref g);
            DrawCurrentData(ref g);

            int ScrllX = this.AutoScrollPosition.X * -1;
            if (MousePoint.IsEmpty)
            {
                //
                if (lastPoint.X > ((this.Width) + ScrllX) - 10)
                {
                    int LastPositionX = (int)lastPoint.X - (int)(this.Width * 0.5);
                    
                    this.AutoScrollPosition = new Point((int)LastPositionX, 0);
                    UpdateStyles();
                    UpdateGraph();                    
                }
            }
        }

        public void UpdateGraph()
        {
            CullAndEqualizeMagnitudeCounts();
            this.Refresh();
        }

        protected void DrawLimitLine(ref Graphics g)
        {
            using (Font fontDefault = new Font("Arial", Config.FontSize, FontStyle.Bold))
            using (SolidBrush AlarmBrush = new SolidBrush(Config.AlarmLineColor))
            using (SolidBrush WarningBrush = new SolidBrush(Config.WarningLineColor))
            {
                int ScrllX = this.AutoScrollPosition.X * -1;

                double fZoomGap = ((Config.Max - Config.Min) * (ZoomScale - 1.0F)) / 2.0F;

                double fMaxline = Config.Max - fZoomGap;
                double fMinline = Config.Min + fZoomGap;

                double posY = 0;
                PointF pHighAlarmStart = new PointF();
                PointF pHighAlarmEnd = new PointF();

                double HeightOffset = this.Height - OffsetY;

                if (Config.IsAlaram)
                {
                    posY = HeightOffset - (((Config.SpecAlarm - fZoomGap - Config.Min) / (fMaxline - fMinline)) * HeightOffset);
                    pHighAlarmStart = new PointF(OffsetX, (float)posY);
                    pHighAlarmEnd = new PointF(this.Width + ScrllX, (float)posY);

                    RectangleF rectHighAlarm = new RectangleF(OffsetX + ScrllX, 0, this.Width + ScrllX, (pHighAlarmStart.Y));

                    g.DrawLine(new Pen(AlarmBrush), pHighAlarmStart, pHighAlarmEnd);
                    g.DrawString("Alarm : " + AlaramCount.ToString(), fontDefault, AlarmBrush, OffsetX + ScrllX, rectHighAlarm.Y);
                    Color ColorAlarmArea = Config.AlarmLineColor;
                    ColorAlarmArea = Color.FromArgb(50, ColorAlarmArea.R, ColorAlarmArea.G, ColorAlarmArea.B);
                    g.FillRectangle(new SolidBrush(ColorAlarmArea), rectHighAlarm);
                }

                if (Config.IsWarning)
                {
                    posY = HeightOffset - (int)(((Config.SpecWarning - fZoomGap - Config.Min) / (fMaxline - fMinline)) * HeightOffset);
                    PointF pHighWarningStart = new PointF(OffsetX, (float)posY);
                    PointF pHighWarningEnd = new PointF(this.Width + ScrllX, (float)posY);

                    RectangleF rectHighWarning = new RectangleF(OffsetX + ScrllX, pHighAlarmStart.Y, this.Width + ScrllX, (pHighWarningStart.Y - pHighAlarmStart.Y));

                    g.DrawLine(new Pen(WarningBrush), pHighWarningStart, pHighWarningEnd);
                    g.DrawString("Warning : " + WarningCount.ToString(), fontDefault, WarningBrush, OffsetX + ScrllX, rectHighWarning.Y);

                    Color ColorWarningArea = Config.WarningLineColor;
                    ColorWarningArea = Color.FromArgb(50, ColorWarningArea.R, ColorWarningArea.G, ColorWarningArea.B);
                    g.FillRectangle(new SolidBrush(ColorWarningArea), rectHighWarning);
                }
            }
        }

        private float textWidth = 0;        
        protected void DrawLabels(ref Graphics g)
        {
            using (Font textFnot = new Font("Arial", Config.FontSize, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Config.TextColor))
            //using (SolidBrush descBrush = new SolidBrush(Color.Red))
            using (Pen borderPen = new Pen(Config.GridColor, 1))
            {
                int ScrllX = this.AutoScrollPosition.X * -1;
                SizeF maxSize = g.MeasureString(Config.Max.ToString(), textFnot);
                SizeF middleSize = g.MeasureString(((Config.Max + Config.Min) / 2).ToString(), textFnot);
                SizeF minSize = g.MeasureString(Config.Min.ToString(), textFnot);
                int PosOffset = 10;

                textWidth = ((maxSize.Width > minSize.Width)
                ? maxSize.Width
                : minSize.Width) + 6;
                textWidth = textWidth + ScrllX;
                
                // min, middle, max Display
                double fZoomGap = (double)((double)(Config.Max - Config.Min) * (double)(ZoomScale - 1.0F)) / 2.0F;

                double Max = Config.Max - fZoomGap;

                string maxlabel = string.Format("{0}", Max.ToString("f0"));
                g.DrawString(maxlabel, textFnot, textBrush,
                                    (textWidth + ScrllX) / 2 - (maxSize.Width / 2),
                                    0);

                double Min = Config.Min + fZoomGap;

                int HeightOffset = (int)(Height - OffsetY);

                string minlabel = string.Format("{0}", Min.ToString("f0"));
                g.DrawString(minlabel, textFnot, textBrush,
                                    (textWidth + ScrllX) / 2 - (minSize.Width / 2),
                                    HeightOffset - PosOffset - 5);

                double middle = ((Max + Min) / 2) + fZoomGap;

                string middlelabel = string.Format("{0}", middle.ToString("f0"));
                g.DrawString(middlelabel, textFnot, textBrush,
                                    (textWidth + ScrllX) / 2 - (maxSize.Width / 2),
                                    (HeightOffset / 2) - PosOffset);

                double MaxOffset = (double)((Config.Max - middle) / (GridSize / 2));
                double MinOffset = (double)((Config.Min - middle) / (GridSize / 2));
                float Pos = HeightOffset / GridSize;
                for (int i = 1; i < (GridSize / 2); i++)
                {
                    double diffMax = (double)(Config.Max - (MaxOffset * i));
                    double diffMin = (double)(middle + (MinOffset * i));
                    string Upperlabel = string.Format("{0}", diffMax.ToString("F1"));
                    string Lowerlabel = string.Format("{0}", diffMin.ToString("F1"));
                    SizeF Size = g.MeasureString(Upperlabel, textFnot);
                    g.DrawString(Upperlabel, textFnot, textBrush, (textWidth + ScrllX) / 2 - (maxSize.Width / 2), (HeightOffset / 2) - (Pos * ((GridSize / 2) - i)) - PosOffset);
                    g.DrawString(Lowerlabel, textFnot, textBrush, (textWidth + ScrllX) / 2 - (maxSize.Width / 2), (HeightOffset / 2) + (Pos * i) - PosOffset);
                }

                int textOffset = 15;
                //g.DrawLine(borderPen, textWidth + textOffset + (textWidth + ScrllX), 0, textWidth + textOffset + (textWidth + ScrllX), Height);

                OffsetX = (textWidth + textOffset) - ScrllX;
            }
        }

        protected void DrawCurrentData(ref Graphics g)
        {
            using (Font fontDefault = new Font("Arial", Config.FontSize, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Config.TextColor))
            {
                int ScrllX = this.AutoScrollPosition.X * -1;
                SizeF szText = g.MeasureString("LAST : 000" + Config.Unit, fontDefault);
                RectangleF Currentrt = new RectangleF();
                SizeF szDesc = g.MeasureString(Config.Desc, fontDefault);
                Color ColorArea = Color.FromArgb(50, Config.TextColor);
                Currentrt.X = this.Width  - (szText.Width * 1.3F) + ScrllX;
                Currentrt.Y = 2;
                Currentrt.Width = (szText.Width * 1.3F);
                Currentrt.Height = (szText.Height * 4.5F);

                float LastY = (szText.Height * 1.1F);
                float MaxY = (szText.Height * 2.3F);
                float MinY = (szText.Height * 3.5F);

                for (int i = 0; i < Lines.Count; i++)
                {
                    Line DataLine = Lines[i];
                    
                    // 제목                    
                    g.DrawString(DataLine.Desc, fontDefault, textBrush, this.Width - (szDesc.Width * 2F) + ScrllX, Currentrt.Y);                    

                    // 제목 영역 사이즈
                    g.FillRectangle(new SolidBrush(ColorArea), Currentrt);

                    if (DataLine == null) { return; }
                    if (DataLine.Datas.Count > 0)
                    {
                        double Max = 0;
                        double Min = 0;
                        Max = DataLine.Datas.ToList().Max(p => p.Value);
                        Min = DataLine.Datas.ToList().Min(p => p.Value);

                        string strDara = string.Format("LAST : {0}" + DataLine.UnitName, DataLine.Datas[DataLine.Datas.Count - 1].Value);
                        string strDaraMAX = string.Format("MAX : {0}" + DataLine.UnitName, Max);
                        string strDaraMIN = string.Format("MIN : {0}" + DataLine.UnitName, Min);
                        g.DrawString(strDara, fontDefault, textBrush, this.Width - (szText.Width * 1.3F) + ScrllX, LastY);
                        g.DrawString(strDaraMAX, fontDefault, textBrush, this.Width - (szText.Width * 1.3F) + ScrllX, MaxY);
                        g.DrawString(strDaraMIN, fontDefault, textBrush, this.Width - (szText.Width * 1.3F) + ScrllX, MinY);

                        if (DataLine.Datas.Count > Config.ListMaxCount)
                        {
                            AlaramCount = 0;
                            WarningCount = 0;
                            DataLine.Datas.Clear();                            
                            MousePoint = new Point();
                            Config.GapX = 10;
                            LineInterval = 10;                            
                            this.AutoScrollPosition = new Point(0, 0);
                            lastPoint = new PointF();
                        }

                        LastY = LastY + Currentrt.Height;
                        MaxY = MaxY + Currentrt.Height;
                        MinY = MinY + Currentrt.Height;                        
                        szDesc.Height = (float)(szDesc.Height + Currentrt.Height);
                        Currentrt.Y = Currentrt.Y + Currentrt.Height;
                    }                   
                }
            }
        }

        protected void DrawGrid(ref Graphics g)
        {
            using (Pen gridPen = new Pen(Config.GridColor, 1))
            {
                int ScrllX = this.AutoScrollPosition.X * - 1;

                float Rows = (float)((float)(this.Height - OffsetY) / GridSize);
                float Cols = (float)((float)this.Width / GridSize);

                for (int n = 0; n < GridSize; n++)
                {
                    float Row = Rows * (n + 1);
                    float Col = Cols * (n + 1);
                    g.DrawLine(gridPen, 0, Row, this.Width + ScrllX, Row);
                    //g.DrawLine(gridPen, Col + OffsetX + ScrllX, 0, Col + OffsetX + ScrllX, this.Height);
                }
                //g.DrawRectangle(gridPen, new Rectangle(0 + 1, 0 + 1, (this.Width - 2) + ScrllX, this.Height - 2));
            }
        }

        private void CullAndEqualizeMagnitudeCounts()
        {
            int greatestMCount = 0;
            foreach (Line line in Lines)
            {
                if (greatestMCount < line.Datas.Count) { greatestMCount = line.Datas.Count; }
            }

            if (greatestMCount == 0) { return; }
            foreach (Line line in Lines)
            {
                while (line.Datas.Count < greatestMCount)
                {
                    line.Datas.Add(
                        line.Datas[line.Datas.Count - 1]);
                }

                int ScrllX = this.AutoScrollPosition.X * -1;
                int Temp = (int)((this.Width - OffsetX) / LineInterval);

                //if(!GraphMouseOn)
                //{
                //    if (lastPoint.X > ((this.Width) + ScrllX) - 10)
                //    {
                //        int PositionX = (int)(this.Width * 0.5) + ScrllX;
                //        int LastPositionX = (int)lastPoint.X - (int)(this.Width * 0.7);
                //        if (PositionX < lastPoint.X)
                //        { 
                //            this.AutoScrollPosition = new Point(LastPositionX, 0);
                //        }
                //        else
                //        {
                //            this.AutoScrollPosition = new Point(PositionX, 0);
                //        }                        
                //    }
                //}

                //AlaramCount = ScrllX;


                //if (this.Width < 10) { continue; }
                //if (line.Datas.Count > (this.Width - OffsetX) / LineInterval)
                //{
                //    if (firstVisible)
                //    {
                //        for (int i = 0; i < (line.Datas.Count / 2); i++)
                //        {
                //            line.Datas[i].Visible = false;
                //        }
                //        firstVisible = false;
                //    }
                //    int startIndex = 0;
                //    int endIndex = 0;
                //    // 가장 최근에 숨겨진 포인트를 찾음
                //    for (int i = 0; i < line.Datas.Count; i++)
                //    {
                //        if (!line.Datas[i].Visible)
                //        {
                //            startIndex = i;
                //        }
                //    }

                //    // 해당 포인트부터 마지막 포인트의 카운트를 계산해서 다시 visible 시킴
                //    endIndex = (line.Datas.Count - startIndex) / 2;

                //    if (line.Datas.Count - startIndex > (this.Width - OffsetX) / LineInterval)
                //    {
                //        for (int i = startIndex; i < (startIndex + endIndex); i++)
                //        {
                //            line.Datas[i].Visible = false;
                //        }
                //    }
                //}
            }
        }

        protected void DrawLines(ref Graphics g)
        {
            int ScrllX = this.AutoScrollPosition.X * -1;
            double fZoomGap = ((Config.Max - Config.Min) * (ZoomScale - 1.0F)) / 2.0F;
            double fMaxline = Config.Max - fZoomGap;
            double fMinline = Config.Min + fZoomGap;
            Point MousePoint2 = new Point();
            MousePoint2 = MousePoint;
            foreach (Line line in Lines)
            {
                if (line.Datas.Count == 0) { return; }

                using (Pen linePen = new Pen(line.DrawColor, Config.Thickness))
                {
                    //lastPoint.X = OffsetX - ScrllX;
                    lastPoint.X = OffsetX;
                    lastPoint.Y = (float)((this.Height - OffsetY) - (line.Datas[0].Value * (this.Height - OffsetY)));

                    bool bFirst = true;
                    int nIndex = 0;
                    int nViewIndex = 0;
                    int nNearst = int.MaxValue;
                    double nSelectedX = 0;
                    double nSelectedY = 0;
                    int startIndex = 0;                    

                    for (int n = 0; n < line.Datas.Count; ++n)
                    {
                        try
                        {
                            if (!line.Datas[n].Visible) { continue; }
                            nIndex = startIndex / DulplexPointCount;


                            // 스크롤값에 따라서 X축 값이 결정되므로 문제가 됨
                            // 
                            //double newX = (OffsetX - ScrllX) + (double)(nIndex * m_LineInterval);
                            double newX = (OffsetX) + (double)(nIndex * m_LineInterval);
                            double newY = (Height - OffsetY) - (double)((double)((double)(line.Datas[n].Value - fZoomGap - Config.Min) / (double)(fMaxline - fMinline)) * (this.Height - OffsetY));

                            if (MousePoint2.IsEmpty)
                            {
                                nSelectedX = newX;
                                nSelectedY = newY;
                            }
                            else
                            {
                                if (Math.Abs(MousePoint2.X - newX) < nNearst)
                                {
                                    nNearst = (int)Math.Abs(MousePoint2.X - newX);
                                    nViewIndex = n;

                                    nSelectedX = newX;
                                    nSelectedY = newY;
                                }
                            }


                            if (bFirst)
                            {
                                lastPoint.X = (float)newX;
                                lastPoint.Y = (float)newY;
                            }

                            if (Config.IsLine)
                            {
                                g.DrawLine(linePen, lastPoint.X, lastPoint.Y, (float)newX, (float)newY);
                                g.FillEllipse(new SolidBrush(linePen.Color), (int)(lastPoint.X - 2.5), (int)(lastPoint.Y - 2.5), 5, 5);
                            }
                            else { g.FillEllipse(new SolidBrush(linePen.Color), (int)(lastPoint.X - 2.5), (int)(lastPoint.Y - 2.5), 5, 5); }

                            lastPoint.X = (float)newX;
                            lastPoint.Y = (float)newY;

                            if (bFirst) { bFirst = false; }
                            startIndex++;
                        }
                        catch (Exception Desc)
                        {
                            CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");                            
                        }

                    }

                    using (Font FronDefault = new Font("Arial", Config.FontSize, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Config.TextColor))
                    {
                        if (!MousePoint2.IsEmpty)
                        {
                            g.DrawString(line.Datas[nViewIndex].Value.ToString() + Config.Unit, FronDefault, textBrush, MousePoint2.X + 20, MousePoint2.Y);
                        }
                        line.Datas[nViewIndex].Index = nViewIndex;
                        g.FillEllipse(new SolidBrush(Color.Aquamarine), new RectangleF((float)nSelectedX - 5, (float)nSelectedY - 5, 10, 10));
                    }

                    linePen.Color = Color.Red;
                    g.DrawLine(linePen, lastPoint.X, 0, lastPoint.X, (this.Height - OffsetY));                    
                }
                
                if (!MousePoint2.IsEmpty) { MousePoint2 = new Point(MousePoint2.X, MousePoint2.Y + 25); }                
            }
        }

        public int GetLineCount(int numID)
        {
            Line pLine = GetLine(numID);

            return pLine.Datas.Count;
        }

        public Line GetLine(int numID)
        {
            foreach (Line line in Lines)
            {
                if (numID == line.NumID)
                {
                    return line;
                }
            }
            return null;
        }

        private Line GetLine(string nameID)
        {
            foreach (Line line in Lines)
            {
                if (string.Compare(nameID, line.m_NameID, true) == 0)
                {
                    return line;
                }
            }
            return null;
        }

        public bool LineExists(int numID) => GetLine(numID) != null;
        public bool LineExists(string nameID) => GetLine(nameID) != null;

        //public void AddLine(string nameID, Color clr)
        //{
        //    if (LineExists(nameID))
        //    {
        //        return;
        //    }

        //    Line line = new Line(nameID);
        //    line.DrawColor = clr;

        //    Lines.Add(line);
        //}

        /// <summary>
        /// 그래프에 표시될 라인을 추가합니다.
        /// </summary>
        /// <param name="numID">라인의 인덱스</param>
        /// <param name="clr">라인의 컬러</param>
        /// <returns></returns>
        public void AddLine(int numID, string Desc, string UnitName, Color clr)
        {
            if (LineExists(numID)) { return; }
            Line line = new Line(numID);
            line.DrawColor = clr;
            line.Desc = Desc;
            line.UnitName = UnitName;

            Lines.Add(line);
        }       

        public bool RemoveLine(string nameID)
        {
            Line line = GetLine(nameID);
            if (line == null) { return false; }
            return Lines.Remove(line);
        }

        public bool RemoveLine(int numID)
        {
            Line line = GetLine(numID);
            if (line == null) { return false; }
            return Lines.Remove(line);
        }

        public bool Push(CGraphData data, int nameID, bool bPush)
        {
            Line line = GetLine(nameID);
            if (line == null) { return false; }
            return PushDirect(data, line);
        }


        public void ClearLine(int numID = 0)
        {
            Line line = GetLine(numID);
            if (line != null)
            {
                line.Datas.Clear();
            }
        }

        private bool PushDirect(CGraphData data, Line line)
        {
            line.Datas.Add(data);

            if (Config.SpecWarning < data.Value &&
                Config.SpecAlarm > data.Value)
            {
                if (Config.IsWarning) { WarningCount++; }
            }
            else if (Config.SpecWarning < data.Value &&
                     Config.SpecAlarm < data.Value)
            {
                if (Config.IsAlaram) { AlaramCount++; }
            }

            return true;
        }
    }

    public class CGraphData
    {
        public int Index { get; set; } = 0;
        // 실제 값
        public double Value { get; set; } = 0.0D;

        // 포인트 visible 사용 유무
        public bool Visible { get; set; } = true;

        public CGraphData(double dData, int nIndex = 0)
        {
            Value = dData;
            Index = nIndex;
        }

        public CGraphData Clone()
        {
            return new CGraphData(Value, Index);
        }
    }

    public class CGraphParam
    {
        public string Desc { get; set; } = "";
        public string Unit { get; set; } = "mVDC";
        public Color TextColor { get; set; } = Color.Aquamarine;
        public Color CurrentDataColor { get; set; } = Color.Blue;
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color GridColor { get; set; } = Color.Black;
        public Color SpecInColor { get; set; } = Color.Blue;
        public Color SpecOutColor { get; set; } = Color.Red;
        public Color WarningLineColor { get; set; } = Color.LightPink;
        public Color AlarmLineColor { get; set; } = Color.Red;
        public Color BorderLineColor { get; set; } = Color.White;
        public double GapX { get; set; } = 10;
        public double GapY { get; set; } = 1;
        public int FontSize { get; set; } = 10;
        public int Index { get; set; } = 0;
        public int Thickness { get; set; } = 1;
        public int ListMaxCount { get; set; } = 3000;
        public int SpecAlarm { get; set; } = 100;
        public int SpecWarning { get; set; } = 50;
        public double Max { get; set; } = 300;
        public double Min { get; set; } = 0;

        public bool IsViewModeLine { get; set; } = true;
        public bool IsLimitPeak { get; set; } = true;
        public bool IsVisibleName { get; set; } = true;
        public bool IsWarning { get; set; } = true;
        public bool IsAlaram { get; set; } = true;
        public bool IsLine { get; set; } = true;


        public CGraphParam() { }
        public void SetIndex(int nIndex) => Index = nIndex;

        private string m_XMLName = "PowerGraphConfig";
        public bool ReadInitFile()
        {
            try
            {
                string strPath = Application.StartupPath + "\\RECIPE\\" + CGlobal.Inst.Recipe.Name + "\\GRAPH\\" + m_XMLName + Index.ToString() + ".cfg";
                if (File.Exists(strPath))   //  xml 파일 존재 유무 검사
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);    //  xml 파일 열기

                    try
                    {
                        ReadInitFileFromXML(xmlReader);
                    }
                    catch (Exception e)
                    {
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    WriteInitFile();
                    return false;
                }
            }
            catch (Exception Desc)
            {
                return false;
            }
            return true;
        }

        public bool WriteInitFile()
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + CGlobal.Inst.Recipe.Name + "\\GRAPH\\" + m_XMLName + Index.ToString() + ".cfg";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();

                WriteInitFileToXML(xmlWriter);
                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {

            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }
            return true;
        }

        public bool ReadInitFileFromXML(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "TextColor":
                            if (!xmlReader.Read()) return false;
                            string[] strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                TextColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "CurrentDataColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                CurrentDataColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "BackgroundColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                BackgroundColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "GridColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                GridColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "SpecInColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                SpecInColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "SpecOutColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                SpecOutColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "WarningLineColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                WarningLineColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "AlarmLineColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                AlarmLineColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "IsViewModeLine":
                            if (!xmlReader.Read()) return false;
                            IsViewModeLine = bool.Parse(xmlReader.Value);
                            break;
                        case "IsLimitPeak":
                            if (!xmlReader.Read()) return false;
                            IsLimitPeak = bool.Parse(xmlReader.Value);
                            break;
                        case "IsWarning":
                            if (!xmlReader.Read()) return false;
                            IsWarning = bool.Parse(xmlReader.Value);
                            break;
                        case "IsAlaram":
                            if (!xmlReader.Read()) return false;
                            IsAlaram = bool.Parse(xmlReader.Value);
                            break;
                        case "IsLine":
                            if (!xmlReader.Read()) return false;
                            IsLine = bool.Parse(xmlReader.Value);
                            break;
                        case "FontSize":
                            if (!xmlReader.Read()) return false;
                            FontSize = int.Parse(xmlReader.Value);
                            break;
                        case "Thickness":
                            if (!xmlReader.Read()) return false;
                            Thickness = int.Parse(xmlReader.Value);
                            break;
                        case "ListMaxCount":
                            if (!xmlReader.Read()) return false;
                            ListMaxCount = int.Parse(xmlReader.Value);
                            break;
                        case "SpecWarning":
                            if (!xmlReader.Read()) return false;
                            SpecWarning = int.Parse(xmlReader.Value);
                            break;
                        case "SpecAlarm":
                            if (!xmlReader.Read()) return false;
                            SpecAlarm = int.Parse(xmlReader.Value);
                            break;
                        case "Max":
                            if (!xmlReader.Read()) return false;
                            Max = double.Parse(xmlReader.Value);
                            break;
                        case "Min":
                            if (!xmlReader.Read()) return false;
                            Min = double.Parse(xmlReader.Value);
                            break;
                        //case "GapX":
                        //    if (!xmlReader.Read()) return false;
                        //    GapX = double.Parse(xmlReader.Value);
                        //    break;
                        //case "GapY":
                        //    if (!xmlReader.Read()) return false;
                        //    GapY = double.Parse(xmlReader.Value);
                        //    break;
                        case "Desc":
                            if (!xmlReader.Read()) return false;
                            Desc = xmlReader.Value;
                            break;
                        case "Unit":
                            if (!xmlReader.Read()) return false;
                            Unit = xmlReader.Value;
                            break;
                    }
                }
                else
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (xmlReader.Name == m_XMLName) break;
                    }
                }
            }
            return true;
        }

        public bool WriteInitFileToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(m_XMLName);
            xmlWriter.WriteElementString("TextColor", ColorToString(TextColor));
            xmlWriter.WriteElementString("CurrentDataColor", ColorToString(CurrentDataColor));
            xmlWriter.WriteElementString("BackgroundColor", ColorToString(BackgroundColor));
            xmlWriter.WriteElementString("GridColor", ColorToString(GridColor));
            xmlWriter.WriteElementString("SpecInColor", ColorToString(SpecInColor));
            xmlWriter.WriteElementString("SpecOutColor", ColorToString(SpecOutColor));
            xmlWriter.WriteElementString("WarningLineColor", ColorToString(WarningLineColor));
            xmlWriter.WriteElementString("AlarmLineColor", ColorToString(AlarmLineColor));
            xmlWriter.WriteElementString("IsViewModeLine", IsViewModeLine.ToString());
            xmlWriter.WriteElementString("IsLimitPeak", IsLimitPeak.ToString());
            xmlWriter.WriteElementString("IsWarning", IsWarning.ToString());
            xmlWriter.WriteElementString("IsAlaram", IsAlaram.ToString());
            xmlWriter.WriteElementString("IsLine", IsLine.ToString());
            xmlWriter.WriteElementString("FontSize", FontSize.ToString());
            xmlWriter.WriteElementString("Thickness", Thickness.ToString());
            xmlWriter.WriteElementString("ListMaxCount", ListMaxCount.ToString());
            xmlWriter.WriteElementString("SpecAlarm", SpecAlarm.ToString());
            xmlWriter.WriteElementString("SpecWarning", SpecWarning.ToString());
            xmlWriter.WriteElementString("Max", Max.ToString());
            xmlWriter.WriteElementString("Min", Min.ToString());
            //xmlWriter.WriteElementString("GapX", GapX.ToString());
            //xmlWriter.WriteElementString("GapY", GapY.ToString());
            xmlWriter.WriteElementString("Desc", Desc.ToString());
            xmlWriter.WriteElementString("Unit", Unit.ToString());

            xmlWriter.WriteEndElement();
            return true;
        }
        private string ColorToString(Color cr) => string.Format("{0},{1},{2}", cr.R, cr.G, cr.B);
    }
}

