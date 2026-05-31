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
using KtemVisionSystem._2._Common;

namespace KtemVisionSystem
{
    public partial class FormVision_Blob_Result : MetroForm
    {
        public FormVision_Blob_Result(List<CResultBlob> cResultBlobs)
        {
            InitializeComponent();                        
            this.TopLevel = true;
            this.TopMost = true;

            dgvDefect.DataSource = new CDefectList_Blob().GetBlobList(cResultBlobs);
            dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        public FormVision_Blob_Result(List<CResultContour> cResultBlobs)
        {
            InitializeComponent();
            this.TopLevel = true;
            this.TopMost = true;

            dgvDefect.DataSource = new CDefectList_Blob().GetBlobList(cResultBlobs);
            dgvDefect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitEvent();            
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
    }
 }

