using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework.Controls;

using OpenCvSharp;
using Cyotek.Windows.Forms;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormVisionAlarm : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        public EventHandler<EventArgs> EventExit;
        public EventHandler<StringEventArgs> EventSkip;
        public EventHandler<StringEventArgs> EventSolved;
        public EventHandler<StringEventArgs> EventReject;
        public EventHandler<StringEventArgs> EventRetry;

        public string SECTION = "";

        public enum VISION_ALARM_RESULT { IDLE, SKIP, RETRY, REJECT};
        public VISION_ALARM_RESULT Result = VISION_ALARM_RESULT.IDLE;


        public string m_strCode = "";

        public FormVisionAlarm(string strSection)
        {
            InitializeComponent();

            this.SECTION = strSection;
            this.TopMost = true;
        }

        private void FormVisionAlarm_Load(object sender, EventArgs e)
        {
            try
            {
                ibSource.MouseWheel += ImageBox_MouseWheelEvent;

                //CDIO_ADLINK7432 DIO = CGlobal.Inst.Device.DIO_BD;

                dgvResultMap.Rows.Clear();
                dgvResultMap.Columns.Clear();
                dgvResultMap.ColumnHeadersVisible = false;

                //dgvResultMap.ColumnCount = Global.iData.PropertyTrayLoading.COLUMNS;

                //int nColWidth = (dgvResultMap.Width - 2) / (Global.iData.PropertyTrayLoading.COLUMNS);
                //int nRowHeight = (dgvResultMap.Height - 2) / (Global.iData.PropertyTrayLoading.ROWS);

                //for (int i = 0; i < Global.iData.PropertyTrayLoading.COLUMNS; i++)
                //{
                //    dgvResultMap.Columns[i].Width = nColWidth;
                //}

                //for (int i = 0; i < Global.iData.PropertyTrayLoading.ROWS; i++)
                //{
                //    string[] strRowData = new string[Global.iData.PropertyTrayLoading.COLUMNS];

                //    for (int j = 0; j < strRowData.Length; j++)
                //    {
                //        //string strPos = $"{j + 1},{i + 1}";
                //        string strPos = "";
                //        strRowData[j] = strPos;
                //    }

                //    dgvResultMap.Rows.Add(strRowData);
                //    dgvResultMap.Rows[dgvResultMap.Rows.Count - 1].Height = nRowHeight;
                //}
                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            try
            {
                Result = VISION_ALARM_RESULT.SKIP;
                if (EventSkip != null) EventSkip(this, new StringEventArgs("SKIP"));

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");

                this.Close();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }    
        }

        private void ImageBox_MouseWheelEvent(object sender, MouseEventArgs e)
        {
            try
            {
                if (!(sender is ImageBox)) return;
                ImageBox ib = (sender as ImageBox);

                if (e.Delta > 0) ib.ZoomIn();
                else ib.ZoomOut();
            }
            catch (Exception Desc)
            {

            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                Result = VISION_ALARM_RESULT.REJECT;
                if (EventReject != null) EventReject(this, new StringEventArgs("REJECT"));

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");

                this.Close();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            try
            {
                Result = VISION_ALARM_RESULT.RETRY;
                if (EventRetry != null) EventRetry(this, new StringEventArgs(m_strCode));

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");

                this.Close();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnBuzzerOff_Click(object sender, EventArgs e)
        {
            try
            {

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        int m_nSelectedRowIndex = 0;
        int m_nSelectedColIndex = 0;
        private void dgvResultMap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int nRowIndex = e.RowIndex;
                int nColIndex = e.ColumnIndex;

                m_nSelectedRowIndex = e.RowIndex;
                m_nSelectedColIndex = e.ColumnIndex;

                //int nIndex = ((nColIndex) * Global.iData.PropertyTrayLoading.ROWS);

                if(nColIndex % 2 == 0)
                {
                    //nIndex += nRowIndex;
                }
                else
                {
                    //nIndex += (Global.iData.PropertyTrayLoading.ROWS - e.RowIndex - 1);
                }

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void OnClickViewMode(object sender, EventArgs e)
        {
            try
            {
                string strIndex = ((MetroLabel)sender).Text;

                CUtil_UI.UpdateLabelOnOff(btnViewOriginal, false);
                CUtil_UI.UpdateLabelOnOff(btnViewBinary, false);

                CUtil_UI.UpdateLabelOnOff(((MetroLabel)sender), true);

                int nRowIndex = m_nSelectedRowIndex;
                int nColIndex = m_nSelectedColIndex;

                //int nIndex = ((nColIndex) * Global.iData.PropertyTrayLoading.ROWS);

                if (nColIndex % 2 == 0)
                {
                  //  nIndex += nRowIndex;
                }
                else
                {
                    //nIndex += (Global.iData.PropertyTrayLoading.ROWS - nRowIndex - 1);
                }

                switch (strIndex)
                {
                    case "ORIGINAL":
                        break;
                    case "BINARY":
                        break;
                }


                ibSource.ZoomToFit();

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void trbThreshold_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                string strIndex = ((MetroLabel)sender).Text;

                CUtil_UI.UpdateLabelOnOff(btnViewOriginal, false);
                CUtil_UI.UpdateLabelOnOff(btnViewBinary, false);

                CUtil_UI.UpdateLabelOnOff(((MetroLabel)sender), true);

                int nRowIndex = m_nSelectedRowIndex;
                int nColIndex = m_nSelectedColIndex;

                //int nIndex = ((nColIndex) * Global.iData.PropertyTrayLoading.ROWS);

                if (nColIndex % 2 == 0)
                {
                  //  nIndex += nRowIndex;
                }
                else
                {
                    //nIndex += (Global.iData.PropertyTrayLoading.ROWS - nRowIndex - 1);
                }

                using (Mat imgBin = new Mat())
                {
                    int nThreshold = trbThreshold.Value;

                    trbThreshold.Value = nThreshold;
                    lbThreshold.Text = trbThreshold.Value.ToString();

                    ibSource.Image = Lib.Common.CImageConverter.ToBitmap(imgBin);
                }

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnReInsp_Click(object sender, EventArgs e)
        {
        }
    }
}
