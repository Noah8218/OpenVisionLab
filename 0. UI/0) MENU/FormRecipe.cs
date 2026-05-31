using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using MetroFramework.Forms;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using RJCodeUI_M1.Models;
using RJCodeUI_M1.RJControls;
using KtemVisionSystem._2._Common;

namespace KtemVisionSystem
{
    public partial class FormRecipe : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        private int originalExStyle = -1;
        private bool enableFormLevelDoubleBuffering = true;

        private int m_nSelectedIndex = 0;

        public FormRecipe()
        {
            InitializeComponent();

            TopLevel = true;
            //TopMost = true;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void FormRecipe_Load(object sender, EventArgs e)
        {
            InitEvent();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        if (originalExStyle == -1)
        //            originalExStyle = base.CreateParams.ExStyle;

        //        CreateParams cp = base.CreateParams;
        //        if (enableFormLevelDoubleBuffering)
        //            cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
        //        else
        //            cp.ExStyle = originalExStyle;

        //        return cp;
        //    }
        //}

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;

                Global.Recipe.EventChagedRecipe += OnChangedRecipe;
              
                lbModelName.Text = Global.Recipe.ModelName;
                lbModelNo.Text = Global.Recipe.ModelNo;
                lbLastUpdateTime.Text = Global.System.LastRecipeUpdateTime;

                InitRecipeList();

                cbRecipeRefList.SelectedIndex = 0;

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedRecipe(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {                
                try
                {
                    string strModelNo = Global.Recipe.Name.Substring(Global.Recipe.Name.Length - 3);
                    string strRecipeName = Global.Recipe.Name.Substring(0, Global.Recipe.Name.Length - 3);

                    lbModelName.Text = strRecipeName;
                    lbModelNo.Text = strModelNo;
                    lbLastUpdateTime.Text = Global.System.LastRecipeUpdateTime;

                    InitRecipeList();

                    CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        public bool InitRecipeList()
        {
            try
            {                
                cbRecipeRefList.Items.Clear();

                dgvRecipeList.DataSource = new CRecipeModel().GetProductsList();
                dgvRecipeList.Columns[1].Width = 800;


                List<string> listRecipe = new List<string>();

                DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "\\RECIPE");
                if (di.Exists)
                {
                    DirectoryInfo[] diRecipies = di.GetDirectories();
                    for (int i = 0; i < diRecipies.Length; i++)
                    {
                        string strRecipe = diRecipies[i].Name;
                        listRecipe.Add(strRecipe);
                    }
                }

                for (int i = 0; i < listRecipe.Count; i++)
                {
                    string strName = listRecipe[i];

                    if (i == 0) { cbRecipeRefList.Text = strName; }              
                    try
                    {
                        string strModelNo = strName.Substring(strName.Length - 3);
                        string strRecipeName = strName.Substring(0, strName.Length - 3);                        
                        cbRecipeRefList.Items.Add(strName);
                    }
                    catch (Exception ex)
                    {
                        CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    }
                }

                // 마지막 정렬
                //dgvRecipeList.Sort(dgvRecipeList.Columns[0], System.ComponentModel.ListSortDirection.Ascending);

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void btnNewRecipeCancel_Click(object sender, EventArgs e)
        {
            pnRecipeNew.Visible = false;
        }

        private void dgvRecipeList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvRecipeList.Rows[dgvRecipeList.CurrentRow.Index].Cells[1].Value == null) { return; }
                m_nSelectedIndex = dgvRecipeList.CurrentRow.Index;
                string strRecipeName = dgvRecipeList.Rows[m_nSelectedIndex].Cells[1].Value.ToString()+dgvRecipeList.Rows[m_nSelectedIndex].Cells[0].Value.ToString();
                lbSelectedModel.Text = strRecipeName;
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                dgvRecipeList.BeginEdit(true);
            }
        }

        private void NumberTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void btnRecipeNew_Click(object sender, EventArgs e)
        {
            pnRecipeNew.Visible = true;
        }

        private void btnNewRecipeCreate_Click(object sender, EventArgs e)
        {
            try
            {
                string strRecipeName = tbRecipeNewName.Text;
                string strPrevRecipe = cbRecipeRefList.Text;

                string strPLCNo = tbRecipeNewNo.Text.PadLeft(3, '0');

                if(CUtil.ShowdialogMessageBox("Add the Recipe", string.Format("Do you want to Add the Recipe ==> [{0}]?", strRecipeName), FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    if (strRecipeName != "")
                    {
                        string strPrevPath = Application.StartupPath + "\\RECIPE\\" + strPrevRecipe;
                        string strNewPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + strPLCNo;

                        DirectoryInfo existingDir = new DirectoryInfo(strPrevPath);
                        DirectoryInfo copyDir = new DirectoryInfo(strNewPath);

                        DirectoryInfo dirRecipe = new DirectoryInfo(strNewPath);
                        if (dirRecipe.Exists == false) dirRecipe.Create();

                        CUtil.SynchFolder(existingDir, copyDir);

                        Global.Recipe.Name = (strRecipeName + strPLCNo);
                        Global.System.LastRecipe = (strRecipeName + strPLCNo);

                        Global.System.SaveConfig();

                        InitRecipeList();

                        Global.System.Notice = string.Format("Change the Recipe {0} ==> {1}", strPrevRecipe, strRecipeName);
                    }
                    else { MessageBox.Show("Please Enter the Recipe Name"); }
                }
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        private void btnRecipeDel_Click(object sender, EventArgs e)
        {
            try
            {
                int nIndex = dgvRecipeList.CurrentRow.Index;
                string strRecipeName = dgvRecipeList.Rows[nIndex].Cells[1].Value.ToString() + dgvRecipeList.Rows[nIndex].Cells[0].Value.ToString();
                string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName;

                if (strRecipeName == Global.Recipe.Name)
                {
                    CUtil.ShowdialogMessageBox("Unable to delete recipe.", "This recipe is currently applied.");
                    return;
                }

                if (CUtil.ShowdialogMessageBox("Select the Recipe", string.Format("Do you want to Delete the Recipe ==> [{0}]?", strRecipeName), FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    DirectoryInfo di = new DirectoryInfo(strPath);
                    di.Delete(true);
                    InitRecipeList();
                }
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnRecipeSelect_Click(object sender, EventArgs e)
        {
            try
            {
                string strRecipeName = "";
                strRecipeName = dgvRecipeList[1, m_nSelectedIndex].Value.ToString() + dgvRecipeList[0, m_nSelectedIndex].Value.ToString();

                if(CUtil.ShowdialogMessageBox("Select the Recipe", string.Format("Do you want to Select the Recipe ==> [{0}]?", strRecipeName), FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    if (strRecipeName != "")
                    {
                        string strPrevRecipe = Global.Recipe.Name;

                        Global.System.LastRecipe = strRecipeName;
                        Global.Recipe.Name = strRecipeName;

                        InitRecipeList();

                        Global.System.SaveConfig();

                        Global.System.Notice = string.Format("Change the Recipe {0} ==> {1}", strPrevRecipe, strRecipeName);
                    }
                    else { MessageBox.Show("Please Select the Recipe"); }
                }
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void dgvRecipeList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvRecipeList.CurrentCell == null)
                {
                    return;
                }

                if (dgvRecipeList.Rows[m_nSelectedIndex].Cells[1].Value == null) { return; }

                if (lbSelectedModel.Text == Global.Recipe.Name)
                {
                    string strModelNo = lbSelectedModel.Text.Substring(lbSelectedModel.Text.Length - 3);
                    string strRecipeName = lbSelectedModel.Text.Substring(0, lbSelectedModel.Text.Length - 3);

                    dgvRecipeList[0, e.RowIndex].Value = strModelNo;                    
                    dgvRecipeList[1, e.RowIndex].Value = strRecipeName;

                    CUtil.ShowdialogMessageBox("Unable to Modify recipe.", "This recipe is currently applied.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
                    return;
                }

                int nRowIndex = dgvRecipeList.CurrentCell.RowIndex;
                var oldValue = dgvRecipeList[e.ColumnIndex, e.RowIndex].Value;

                var Code = dgvRecipeList[0, e.RowIndex].Value.ToString().PadLeft(3, '0');                
                var Name = dgvRecipeList[1, e.RowIndex].Value.ToString();

                string oldName = lbSelectedModel.Text;
                string ReName = "";

                if (oldValue == null)
                {
                    return;
                }
                try
                {
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            ReName = Name + oldValue.ToString().PadLeft(3, '0');
                            break;
                        case 1:
                            ReName = Name + Code.ToString();
                            break;
                    }

                    if(CUtil.ShowdialogMessageBox("Select the Recipe", string.Format("Do you want to Modify the Recipe ==> [{0}]?", oldName), FormMessageBox.MESSAGEBOX_TYPE.Quit))
                    {
                        string strPrevPath = Application.StartupPath + "\\RECIPE\\" + oldName;
                        string strNewPath = Application.StartupPath + "\\RECIPE\\" + ReName;

                        DirectoryInfo dicCheck = new DirectoryInfo(strNewPath);
                        if (dicCheck.Exists)
                        {
                            CUtil.ShowMessageBox("Notice", "The same recipe name exists.");
                            return;
                        }

                        DirectoryInfo existingDir = new DirectoryInfo(strPrevPath);
                        if (existingDir.Exists)
                        {
                            existingDir.MoveTo(strNewPath);
                            existingDir = new DirectoryInfo(strNewPath);
                        }

                        Invoke((MethodInvoker)delegate ()
                        {
                            InitRecipeList();
                        });
                    }
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {ex.Message}");
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}

