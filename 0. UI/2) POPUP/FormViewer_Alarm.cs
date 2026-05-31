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
using System.IO;

using OpenCvSharp;

using MetroFramework.Forms;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormViewer_Alarm : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        public FormViewer_Alarm()
        {
            InitializeComponent();
        }

        private void FormViewer_Alarm_Load(object sender, EventArgs e)
        {
            try
            {
                dtpEnd.Value = DateTime.Now;

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnLotLastLoad_Click(object sender, EventArgs e)
        {

        }
    }
}

