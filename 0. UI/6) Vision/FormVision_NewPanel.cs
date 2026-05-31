using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;


//IF 전용 Library
using OpenCvSharp;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;

using MetroFramework;
using MetroFramework.Forms;
using ADOX;
using Keys = System.Windows.Forms.Keys;

namespace KtemVisionSystem
{
    public partial class FormVision_NewPanel : MetroForm
    {
        public string PanelName = "";

        public int PanelCount = 0;

        public FormVision_NewPanel(int Count)
        {
            InitializeComponent();                        
            PanelCount = Count;
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitEvent();            
            tbNewPanel.Text = $"NewPanel_{CUtil.fnGetRandomString(3)}";
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            Keys key = keyData & ~(Keys.Shift | Keys.Control);

            switch (key)
            {
                case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return true;
                case Keys.Enter:
                    btnNewCreate_Click(null, null);
                    return true;                               
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;
                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void btnNewCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();            
        }

        private void btnNewCreate_Click(object sender, EventArgs e)
        {
            PanelName = tbNewPanel.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //try
            //{
            //    if(PanelCount == )
            //}
            //catch (Exception ex)
            //{

            //    CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);                
            //}
        }
    }
 }

