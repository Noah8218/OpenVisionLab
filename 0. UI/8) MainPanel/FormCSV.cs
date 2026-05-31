using System.Data;
using System.Text;

namespace Thermofisher_intern_CSV_viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // constants and variables
        private string _delimiter = ",";

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

            sb.AppendJoin(_delimiter, headers);
            sb.Append("\r\n"); // New Line

            for (int row = 0; row < dataGridViewCSV.Rows.Count - 1; row++)
            {
                List<string> rowItems = new List<string>();

                for (int i = 0; i < dataGridViewCSV.Columns.Count; i++)
                {
                    rowItems.Add(dataGridViewCSV.Rows[row].Cells[i].Value.ToString());
                }

                sb.AppendJoin(_delimiter, rowItems);
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
            _delimiter = ",";
        }

        private void radioButtonSemicolon_CheckedChanged(object sender, EventArgs e)
        {
            _delimiter = ";";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridViewCSV.DataSource = null;
        }
    }
}