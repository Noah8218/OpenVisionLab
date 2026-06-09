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
using Keys = System.Windows.Forms.Keys;
using OpenVisionLab._2._Common;
using RJCodeUI_M1.RJForms;
using Vila.Win32;
using static OpenVisionLab._2._Common.ParameterManager;
using Lib.OpenCV;
using Lib.OpenCV.Result;
using Lib.OpenCV.Blob;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormVision_Result : RJChildForm
    {
        public FormVision_Result(List<BlobResult> cResultBlobs)
        {
            InitializeComponent();                        
            this.TopLevel = true;
            this.TopMost = true;

            //dgvResult.DataSource = new DefectListResult().GetBlobList(cResultBlobs);
            //dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvResult.ColumnHeadersVisible = false;
            dgvResult.DataSource = new DefectListResult().GetBlobList(cResultBlobs);
            dgvResult.ColumnHeadersVisible = true;
            //dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        public FormVision_Result(List<ContourResult> cResultCounts)
        {
            InitializeComponent();
            this.TopLevel = true;
            this.TopMost = true;

            dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvResult.ColumnHeadersVisible = false;
            dgvResult.DataSource = new DefectListResult().GetContourList(cResultCounts);
            dgvResult.ColumnHeadersVisible = true;
        }

        public FormVision_Result()
        {
            InitializeComponent();
            this.TopLevel = true;
            this.TopMost = true;
        }

        public void SetBindingRois(List<SpecRectangle> Rois)
        {
            this.TopLevel = true;
            this.TopMost = true;
            this.Text = "ROIS";

            BindingList<SpecRectangle> binding = new BindingList<SpecRectangle>();

            for (int i = 0; i < Rois.Count; i++) { binding.Add(Rois[i]); }

            dgvResult.DataSource = binding;
            dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        public void SetBindingRoi(SpecRectangle Roi)
        {
            this.TopLevel = true;
            this.TopMost = true;
            this.Text = "ROI";

            BindingList<SpecRectangle> binding = new BindingList<SpecRectangle>();

            binding.Add(Roi);

            dgvResult.DataSource = binding;
            dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitEvent();            
        }

        //protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        //{
        //    Keys key = keyData & ~(Keys.Shift | Keys.Control);

        //    switch (key)
        //    {
        //        case Keys.Escape:
        //            this.DialogResult = DialogResult.Cancel;
        //            this.Close();
        //            return true;                           
        //    }

        //    return base.ProcessCmdKey(ref msg, keyData);
        //}


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
                        this.KeyPreview = true;
            this.KeyDown += Form_KeyDown;
        

            return true;
        }

        private void btnNewCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            dgvResult.Invalidate();
            //dgvDefect.ResetBindings();
        }

        private void FormVision_Result_Resize(object sender, EventArgs e)
        {
            //dgvResult.Invalidate();
        }
    }
 }

