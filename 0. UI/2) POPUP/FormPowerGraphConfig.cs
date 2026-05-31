using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using OpenCvSharp;
using RJCodeUI_M1.RJForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OpenVisionLab
{
    public partial class FormPowerGraphConfig : RJChildForm
    {
        private CGlobal Global = CGlobal.Inst;

        private CGraphParam m_PowerGraphConfigOri = new CGraphParam();
        private CGraphParam m_Config = new CGraphParam();
        public CGraphParam Config
        {
            get { return m_Config; }
            set { m_Config = value; }
        }
        public FormPowerGraphConfig(CGraphParam config)
        {           
            InitializeComponent();
            m_Config = config;
        }
     
        private void Form_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.KeyDown += Form_KeyDown;
            BackupConfig();
            InitControlFromConfig();
        }

        private void InitControlFromConfig()
        {
            pnTextColor.BackColor = m_Config.TextColor;
            pnCurrentDataColor.BackColor = m_Config.CurrentDataColor;
            pnBackgroundColor.BackColor = m_Config.BackgroundColor;
            pnGridColor.BackColor = m_Config.GridColor;
            pnWarningLineColor.BackColor = m_Config.WarningLineColor;
            pnAlarmLineColor.BackColor = m_Config.AlarmLineColor;
            nbLeftFontSize.Value = m_Config.FontSize;
            nbThickness.Value = m_Config.Thickness;
            tbMaxY.Text = m_Config.Max.ToString();
            tbMinY.Text = m_Config.Min.ToString();

            tbListMaxCount.Text = m_Config.ListMaxCount.ToString();
            tbAlaram.Text = m_Config.SpecAlarm.ToString();
            tbWarning.Text = m_Config.SpecWarning.ToString();

            tbName.Text = m_Config.Desc;
            tbUnit.Text = m_Config.Unit;

            cbDisplayAlaram.Checked = m_Config.IsAlaram;
            cbDisplayWarning.Checked = m_Config.IsWarning;

            if (m_Config.IsLine) { rdoLine.Checked = true; }            
            else { rdoDot.Checked = true; }           
            
        }

        private void BackupConfig()
        {
            m_PowerGraphConfigOri.TextColor = m_Config.TextColor;
            m_PowerGraphConfigOri.CurrentDataColor = m_Config.CurrentDataColor;
            m_PowerGraphConfigOri.BackgroundColor = m_Config.BackgroundColor;
            m_PowerGraphConfigOri.GridColor = m_Config.GridColor;
            m_PowerGraphConfigOri.SpecInColor = m_Config.SpecInColor;
            m_PowerGraphConfigOri.SpecOutColor = m_Config.SpecOutColor;
            m_PowerGraphConfigOri.WarningLineColor = m_Config.WarningLineColor;
            m_PowerGraphConfigOri.AlarmLineColor = m_Config.AlarmLineColor;
            m_PowerGraphConfigOri.BorderLineColor = m_Config.BorderLineColor;
            m_PowerGraphConfigOri.GapX = m_Config.GapX;
            m_PowerGraphConfigOri.GapY = m_Config.GapY;
            m_PowerGraphConfigOri.IsViewModeLine = m_Config.IsViewModeLine;
            m_PowerGraphConfigOri.FontSize = m_Config.FontSize;

            m_PowerGraphConfigOri.Max = m_Config.Max;
            m_PowerGraphConfigOri.Min = m_Config.Min;

            //m_PowerGraphConfigOri.ListMaxCount = m_Config.ListMaxCount;
            m_PowerGraphConfigOri.IsLimitPeak = m_Config.IsLimitPeak;
        }

        private void RollBackConfig()
        {
            m_Config.TextColor = m_PowerGraphConfigOri.TextColor;
            m_Config.CurrentDataColor = m_PowerGraphConfigOri.CurrentDataColor;
            m_Config.BackgroundColor = m_PowerGraphConfigOri.BackgroundColor;
            m_Config.GridColor = m_PowerGraphConfigOri.GridColor;
            m_Config.SpecInColor = m_PowerGraphConfigOri.SpecInColor;
            m_Config.SpecOutColor = m_PowerGraphConfigOri.SpecOutColor;
            m_Config.WarningLineColor = m_PowerGraphConfigOri.WarningLineColor;
            m_Config.AlarmLineColor = m_PowerGraphConfigOri.AlarmLineColor;
            m_Config.BorderLineColor = m_PowerGraphConfigOri.BorderLineColor;
            m_Config.GapX = m_PowerGraphConfigOri.GapX;
            m_Config.GapY = m_PowerGraphConfigOri.GapY;
            m_Config.IsViewModeLine = m_PowerGraphConfigOri.IsViewModeLine;
            m_Config.FontSize = m_PowerGraphConfigOri.FontSize;
            m_Config.Thickness = m_PowerGraphConfigOri.Thickness;

            m_Config.Max = m_PowerGraphConfigOri.Max;
            m_Config.Min = m_PowerGraphConfigOri.Min;

            m_Config.ListMaxCount = m_PowerGraphConfigOri.ListMaxCount;

            m_Config.IsLimitPeak = m_PowerGraphConfigOri.IsLimitPeak;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void FormLayerDisplay_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void OnChangeColor(object sender, MouseEventArgs e)
        {
            string strIndex = ((Panel)sender).Name;

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Color ChangeColor = colorDialog.Color;

                switch (strIndex)
                {
                    case "pnTextColor":
                        pnTextColor.BackColor = ChangeColor;
                        break;
                    case "pnCurrentDataColor":
                        pnCurrentDataColor.BackColor = ChangeColor;
                        break;
                    case "pnBackgroundColor":
                        pnBackgroundColor.BackColor = ChangeColor;
                        break;
                    case "pnGridColor":
                        pnGridColor.BackColor = ChangeColor;
                        break;
                    case "pnWarningLineColor":
                        pnWarningLineColor.BackColor = ChangeColor;
                        break;
                    case "pnAlarmLineColor":
                        pnAlarmLineColor.BackColor = ChangeColor;
                        break;

                }
            }
        }

        private void OnTypingOnlyNumber(object sender, KeyPressEventArgs e)
        {
            Lib.Common.CUtil_UI.TypingOnlyNumber(sender, e, false, false);
        }

        private bool UpdateConfig()
        {
            try
            {
                m_Config.TextColor = pnTextColor.BackColor;
                m_Config.CurrentDataColor = pnCurrentDataColor.BackColor;
                m_Config.BackgroundColor = pnBackgroundColor.BackColor;
                m_Config.GridColor = pnGridColor.BackColor;
                m_Config.WarningLineColor = pnWarningLineColor.BackColor;
                m_Config.AlarmLineColor = pnAlarmLineColor.BackColor;          
                m_Config.FontSize = int.Parse(nbLeftFontSize.Value.ToString());
                m_Config.Thickness = int.Parse(nbThickness.Value.ToString());
                m_Config.SpecAlarm = int.Parse(tbAlaram.Text);
                m_Config.SpecWarning = int.Parse(tbWarning.Text);
                m_Config.Desc = tbName.Text;
                m_Config.Unit = tbUnit.Text;
                m_Config.IsWarning = cbDisplayWarning.Checked;
                m_Config.IsAlaram = cbDisplayAlaram.Checked;

                if (rdoLine.Checked) { m_Config.IsLine = true; }
                else { m_Config.IsLine = false; }

                if (tbMaxY.Text == "")
                {
                    MessageBox.Show("MaxY data is empty");
                    return false;
                }
                else { m_Config.Max = double.Parse(tbMaxY.Text); }
                
                if (tbMinY.Text == "")
                {
                    MessageBox.Show("MinY data is empty");
                    return false;
                }
                else { m_Config.Min = double.Parse(tbMinY.Text); }                

                if (tbListMaxCount.Text == "")
                {
                    MessageBox.Show("List Max Count data is empty");
                    return false;
                }
                else { m_Config.ListMaxCount = int.Parse(tbListMaxCount.Text); }                
                if (m_Config.ListMaxCount > 20000)
                {
                    MessageBox.Show("List Max Count is 20000");
                    return false;
                }
                else { m_Config.ListMaxCount = int.Parse(tbListMaxCount.Text); }
                
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void btnSaveParameter_Click(object sender, EventArgs e)
        {
            if (!UpdateConfig())
            {
                MessageBox.Show("Failed update the config");
                RollBackConfig();
                return;
            }
            Config.WriteInitFile();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }
    }
}
