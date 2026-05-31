using OpenVisionLab._3._Device.DB;
using Lib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public partial class FormSearchDB : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;        
        public FormSearchDB()
        {           
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
            this.Resize += FormSearchDB_Resize;

      //      this.cbPeriod.DataSource = Enum.GetValues(typeof(DefectType));
            this.cbPeriod.SelectedIndex = this.cbPeriod.Items.Count - 1;
        }

        private void FormSearchDB_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {            
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        private bool ChangeSize = false;

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            //int nYear = int.Parse(DateTime.Now.ToString("yyyy"));
            //int nMonth = int.Parse(DateTime.Now.ToString("MM"));
            //int nDay = int.Parse(DateTime.Now.ToString("dd"));            
            //EndPicker.Value = new DateTime(nYear, nMonth, nDay, 0, 0, 0);

            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            int nYear = int.Parse(DateTime.Now.ToString("yyyy"));
            int nMonth = int.Parse(DateTime.Now.ToString("MM"));
            int nDay = int.Parse(DateTime.Now.ToString("dd"));
            StartPickerTime.Value = new DateTime(nYear, nMonth, nDay, 1, 0, 0);
            StartPicker.Value = new DateTime(nYear, nMonth, nDay, 0, 0, 0);
            EndPicker.Value = new DateTime(nYear, nMonth, nDay, 0, 0, 0);

            //this.dgvSearchDB.RowPostPaint += DgvSearchDB_RowPostPaint;
        }

        private void DgvSearchDB_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // 원하는 칼럼에 자동 번호 매기기
            //this.dgvSearchDB.Rows[e.RowIndex].Cells[0].Value = (e.RowIndex + 1).ToString();
        }

        private void btnSearchData_Click(object sender, EventArgs e)
        {
            DateTime StartSearchTime = StartPicker.Value + StartPickerTime.Value.TimeOfDay;
            DateTime EndSearchTime = EndPicker.Value + EndPickerTime.Value.TimeOfDay;
            List<CDefect> defects = new List<CDefect>();
            var task = Task.Run(() =>
            {
                Global.Device.DB.StartSearchTime = StartSearchTime;
                Global.Device.DB.EndSearchTime = EndSearchTime;                
                Global.Device.DB.SearchData(out defects);
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    dgvSearchDB.DataSource = new CDefect().GetDefectList(defects);
                    dgvSearchDB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }));                
            });            
        }

        private void btnDayEdit(object sender, EventArgs e)
        {
            string strIndex = (sender as Button).Text;

            DateTime DataTimeStart = StartPicker.Value;
            switch (strIndex)
            {
                case "Today":
                    StartPicker.Value = DateTime.Today;
                    EndPicker.Value = DateTime.Today;
                    break;
                case "Week":
                    DateTime dataTimeWeek = StartPicker.Value;
                    EndPicker.Value = dataTimeWeek.AddDays(+7);
                    break;
                case "Month":
                    DateTime startYMD = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    StartPicker.Value = startYMD;
                    EndPicker.Value = startYMD.AddMonths(1).AddDays(-1);
                    break;
                case "-1 Day":
                    StartPicker.Value = DataTimeStart.AddDays(-1);
                    break;
                case "+1 Day":
                    StartPicker.Value = DataTimeStart.AddDays(1);
                    break;
            }            
        }

        private void btnBackupDB_Click(object sender, EventArgs e)
        {
            //string strPath = Global.System.MariaSQL.DataBackupPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "_backup.sql";

            CUtil.LoadFolderPath(out string path);

            path = $"{path}\\{DateTime.Now.ToString("yyyyMMddHHmmss")} _backup.sql";

            Global.Device.DB.Backup(path);
        }

        private void btnRestoreDB_Click(object sender, EventArgs e)
        {
            string strFilePath = string.Empty;

            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    strFilePath = openFileDialog.FileName;
                    Global.Device.DB.Restore(strFilePath);
                }
            }
        }

        private void FormSearchDB_Enter(object sender, EventArgs e)
        {
            
        }

        private void btnSearchLot_Click(object sender, EventArgs e)
        {
            DateTime StartSearchTime = StartPicker.Value + StartPickerTime.Value.TimeOfDay;
            DateTime EndSearchTime = EndPicker.Value + EndPickerTime.Value.TimeOfDay;
            List<CDefect> defects = new List<CDefect>();

            string Lot = tbSearch.Text;

            var text = cbPeriod.SelectedItem.ToString();
            var task = Task.Run(() =>
            {
                //DefectType defectType;
                //Enum.TryParse<DefectType>(text, out defectType);
                //Global.Device.DB.StartSearchTime = StartSearchTime;
                //Global.Device.DB.EndSearchTime = EndSearchTime;
                //Global.Device.DB.SearchFromWhereData(Lot, defectType, out defects);
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    dgvSearchDB.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                    // or even better, use .DisableResizing. Most time consuming enum is DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders

                    // set it to false if not needed
                    dgvSearchDB.RowHeadersVisible = false;
                    dgvSearchDB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    dgvSearchDB.SuspendLayout();
                    dgvSearchDB.DataSource = new CDefect().GetDefectList(defects);
                    dgvSearchDB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }));

            });
        }

        private void btnSaveCsv_Click(object sender, EventArgs e)
        {
            if (dgvSearchDB.Rows.Count < 1)
            {
                MessageBox.Show("No data to export!", "Error - no data.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            saveFileDialog1.Filter = "Comma separated values (*.csv)|*.csv";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dataGridViewSaveToCSV();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error: " + exception.Message);
                    throw;
                }
            }
        }
        private Char _delimiter = ',';
        private void dataGridViewSaveToCSV()
        {
            StringBuilder sb = new StringBuilder();

            // Header
            List<string> headers = new List<string>();
            foreach (DataGridViewColumn column in dgvSearchDB.Columns)
            {
                headers.Add(column.HeaderText);
            }

            string s = string.Join(_delimiter.ToString(), headers);
            sb.Append(s);
            sb.Append("\r\n"); // New Line

            for (int row = 0; row < dgvSearchDB.Rows.Count - 1; row++)
            {
                List<string> rowItems = new List<string>();

                for (int i = 0; i < dgvSearchDB.Columns.Count; i++)
                {
                    rowItems.Add(dgvSearchDB.Rows[row].Cells[i].Value.ToString());
                }

                string s2 = string.Join(_delimiter.ToString(), rowItems);
                sb.Append(s2);
                sb.Append("\r\n"); // New Line 
            }

            File.WriteAllText(saveFileDialog1.FileName, sb.ToString(), Encoding.UTF8);
        }
    }
}
