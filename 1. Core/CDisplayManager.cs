using Lib.Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab._1._Core
{
    public static class CDisplayManager
    {
        public static EventHandler<EventArgs> EventUpdateParameter;
        public static EventHandler<EventArgs> EventUpdateResult;
        public static EventHandler<EventArgs> EventUpdateCam;

        private static DockPanel DockPanel;
        private static Form Frm;
        public static List<FormLayerDisplay> Displays { get; set; } = new List<FormLayerDisplay>();        
        public static Mat ImageSrc { get; set; } = new Mat();
        public static string SelecteItem { get; set; } = "Main";
        private static int m_CameraIndex;
        public static int CameraIndex
        {
            get { return m_CameraIndex; }
            set 
            {
                m_CameraIndex = value;  
                if(EventUpdateCam != null)
                {
                    EventUpdateCam(null, null);
                }
            }
        }

        public static string FocusItem { get; set; } = "";


        private static string m_TackTime;
        public static string TackTime
        {
            get
            {
                return m_TackTime;
            }
            set
            {
                m_TackTime = value;
                if(EventUpdateResult != null)
                {
                    EventUpdateResult(null, null);
                }
            }
        }
        
        public static void SetForm(Form form) => Frm = form;        
        public static void SetDockPanel(DockPanel dockPanel) => DockPanel = dockPanel;         
        public static void SetDisplayLayerList(List<FormLayerDisplay> Display) => Displays = Display;       
        public static void CreatePanel(Bitmap bitmap = null)
        {
            FormVision_NewPanel formVision_NewPanel = new FormVision_NewPanel(Displays.Count);
            if (formVision_NewPanel.ShowDialog() == DialogResult.OK)
            {
                if (bitmap == null) { CreateLayerDisplay(new Bitmap(10, 10), formVision_NewPanel.PanelName, true); }
                else { CreateLayerDisplay(bitmap, formVision_NewPanel.PanelName, true); }                               
            }
        }

        public static int FindIndex(string strTitle)
        {
            for (int i = 0; i < Displays.Count; i++)
            {
                if (Displays[i].Text == strTitle) { return i; }
            }

            return 0;
        }

        public static int FindIndex()
        {
            for (int i = 0; i < Displays.Count; i++)
            {
                if (Displays[i].Text == SelecteItem) { return i; }
            }

            return 0;
        }

        private static void ClearEmptyDisplay()
        {
            object _ob = new object();
            Frm.Invoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
                lock (_ob)
                {
                    int Count = Displays.Count;
                    for (int i = 0; i < Count; i++)
                    {
                        if (Displays[i].Text == "")
                        {
                            Displays.RemoveAt(i);
                        }
                    }
                }
            });
        }

        public static void CreateLayerDisplay(Mat ImageSource, string strTitle, bool bUseClose = true)
        {
            ClearEmptyDisplay();

            object _ob = new object();
            Frm.Invoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
                lock (_ob)
                {
                    bool bIsLayer = false;
                    int ExsitIndex = 0;

                    for (int i = 0; i < Displays.Count; i++)
                    {
                        if (Displays[i].Text == strTitle)
                        {
                            bIsLayer = true;
                            ExsitIndex = i;
                        }
                    }

                    if (!bIsLayer)
                    {
                        Displays.Add(new FormLayerDisplay(CImageConverter.ToBitmap(ImageSource), Displays.Count, Displays, bUseClose, strTitle));
                        Displays[Displays.Count - 1].Show(DockPanel, DockState.Document);
                        Displays[Displays.Count - 1].ibSource.ZoomToFit();
                    }
                    else
                    {
                        Displays[ExsitIndex].viewer._Ib.Image = CImageConverter.ToBitmap(ImageSource);
                        //Displays[ExsitIndex].viewer._Ib.ZoomToFit();
                        if (DockPanel.ActiveDocument.DockHandler.TabText != strTitle)
                        {
                            Displays[ExsitIndex].Activate();
                        }
                        //SwitchDockpanelDocument(ExsitIndex);                        
                    }


                }
            });
        }

        public static void CreateLayerDisplay(Bitmap ImageSource, string strTitle, bool bUseClose = true)
        {
            ClearEmptyDisplay();

            object _ob = new object();
            Frm.Invoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
                lock (_ob)
                {
                    bool bIsLayer = false;
                    int ExsitIndex = 0;

                    for (int i = 0; i < Displays.Count; i++)
                    {
                        if (Displays[i].Text == strTitle)
                        {
                            bIsLayer = true;
                            ExsitIndex = i;
                        }
                    }
                    
                    if (!bIsLayer)
                    {
                        Displays.Add(new FormLayerDisplay(ImageSource, Displays.Count, Displays, bUseClose, strTitle));
                        Displays[Displays.Count - 1].Show(DockPanel, DockState.Document);
                        Displays[Displays.Count - 1].ibSource.ZoomToFit();
                    }
                    else
                    {
                        Displays[ExsitIndex].viewer._Ib.Image = ImageSource;
                        //Displays[ExsitIndex].viewer._Ib.ZoomToFit();
                        if(DockPanel.ActiveDocument.DockHandler.TabText != strTitle)
                        {
                            Displays[ExsitIndex].Activate();
                        }
                        //SwitchDockpanelDocument(ExsitIndex);                        
                    }

                    
                }
            });
        }

        private static void SwitchDockpanelDocument(int tag)
        {
            if (tag == 0) return;

            var documentList = DockPanel.Documents.ToList();
            var index = documentList.IndexOf(DockPanel.ActiveDocument);
            System.Diagnostics.Debug.WriteLine($"current window index: {index}");
            var nextIndex = -1;
            if (tag > 0)
            {
                System.Diagnostics.Debug.WriteLine("switch to next window");
                nextIndex = (index + 1) % DockPanel.DocumentsCount;
            }
            else if (tag < 0)
            {
                System.Diagnostics.Debug.WriteLine("switch to previous window");
                nextIndex = (index - 1 + DockPanel.DocumentsCount) % DockPanel.DocumentsCount;
            }
            System.Diagnostics.Debug.WriteLine($"next window index: {nextIndex}");
            if (nextIndex == index) return;
            var document = documentList[nextIndex];
            ((DockContent)document).Activate();
        }
    }
}
