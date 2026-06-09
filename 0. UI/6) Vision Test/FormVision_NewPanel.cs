using System;
using System.Windows.Forms;
using System.Reflection;
using Keys = System.Windows.Forms.Keys;
using RJCodeUI_M1.RJForms;
using System.Text;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormVision_NewPanel : RJChildForm
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
            tbNewPanel.Text = $"NewPanel_{fnGetRandomString(3)}";
        }

        public static string fnGetRandomString(int numLength)
        {

            string strResult = "";
            Random rand = new Random();
            string strRandomChar = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789"; //�������� �� ���� �� ���� 

            StringBuilder rs = new StringBuilder();

            //�Ű������� ���� numLength��ŭ �����͸� ���� �� �� �ֽ��ϴ�.
            for (int i = 0; i < numLength; i++)
            {
                rs.Append(strRandomChar[(int)(rand.NextDouble() * strRandomChar.Length)]);
            }
            strResult = rs.ToString();

            return strResult;
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
                        this.KeyPreview = true;
            this.KeyDown += Form_KeyDown;
        

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
            //catch (Exception Desc)
            //{

            //}
        }
    }
 }

