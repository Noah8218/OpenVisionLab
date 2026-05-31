using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public partial class FormCSV : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public bool ChangeSize { get; set; } = false;

        public FormCSV()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
            this.Load += FormCSV_Load;
        }

        private void FormCSV_Load(object sender, EventArgs e)
        {
            //Panel panel = splitContainer1.Panel1;
            //panel.MaximumSize = new Size(559, 100); //max 300 x 300
            splitContainer1.SizeChanged += splitContainer1_SizeChanged;
            splitContainer1.SplitterMoved += splitContainer1_SplitterMoved;
        }

        private const int Panel1MaxWidth = 100;
        private void splitContainer1_SizeChanged(object sender, EventArgs e)
        {
            if (splitContainer1.Panel1.Height > Panel1MaxWidth)
            {
                splitContainer1.SplitterDistance = Panel1MaxWidth;
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (splitContainer1.Panel1.Height > Panel1MaxWidth)
            {
                splitContainer1.SplitterDistance = Panel1MaxWidth;
            }
        }

        private void FormLayerDisplay_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }

        // constants and variables
        private Char _delimiter = ',';

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = openFileDialog1.FileName;
                textBoxPath.Text = path;
                textBoxPath.Focus();
                DataGridViewPopulate(path);

            }
        }

        private void DataGridViewPopulate(string filePath)
        {
            DataTable dt = new DataTable();
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length > 0)
            {

                // Header
                string headerLine = lines[0];
                string[] headerLabels = headerLine.Split(_delimiter);
                foreach (string headerLabel in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerLabel));                    
                }
                

                // Data
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] lineData = lines[i].Split(_delimiter);
                    DataRow dr = dt.NewRow();
                    int columnIndex = 0;
                    foreach (string headerLabel in headerLabels)
                    {
                        dr[headerLabel] = lineData[columnIndex];
                        columnIndex++;
                    }

                    dt.Rows.Add(dr);
                }
            }

            if (dt.Rows.Count > 0)
            {
                dataGridViewCSV.DataSource = dt;
            }
        }

        private void loadCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.FileName == string.Empty) 
            {
                MessageBox.Show("You have to select a CSV file first.",
                    "Error - No CSV file selected", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
            else {
                DataGridViewPopulate(openFileDialog1.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridViewSaveToCSV()
        {
            StringBuilder sb = new StringBuilder();

            // Header
            List<string> headers = new List<string>();
            foreach (DataGridViewColumn column in dataGridViewCSV.Columns)
            {
                headers.Add(column.HeaderText);
            }

            string s = string.Join(_delimiter.ToString(), headers);
            sb.Append(s);
            sb.Append("\r\n"); // New Line

            for (int row = 0; row < dataGridViewCSV.Rows.Count - 1; row++)
            {
                List<string> rowItems = new List<string>();

                for (int i = 0; i < dataGridViewCSV.Columns.Count; i++)
                {
                    rowItems.Add(dataGridViewCSV.Rows[row].Cells[i].Value.ToString());
                }

                string s2 = string.Join(_delimiter.ToString(), rowItems);
                sb.Append(s2);
                sb.Append("\r\n"); // New Line 
            }

            File.WriteAllText(saveFileDialog1.FileName, sb.ToString(), Encoding.UTF8);

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewCSV.Rows.Count < 1)
            {
                MessageBox.Show("No data to export!","Error - no data.",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            saveFileDialog1.Filter = "Comma separated values (*.csv)|*.csv";
            saveFileDialog1.RestoreDirectory = true;

            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
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

            MessageBox.Show("File saved sucessfully",
                "File saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void dataGridViewCSV_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void dataGridViewCSV_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files =(string[])e.Data.GetData(DataFormats.FileDrop);

                string filePath = files[0];
                openFileDialog1.FileName = filePath;
                textBoxPath.Text = filePath;
                DataGridViewPopulate(filePath);
            }
        }

        private void radioButtonComma_CheckedChanged(object sender, EventArgs e)
        {
            _delimiter = ',';
        }

        private void radioButtonSemicolon_CheckedChanged(object sender, EventArgs e)
        {
            _delimiter = ';';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridViewCSV.DataSource = null;
        }
    }
}