using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using ADOX;
using System.Data.OleDb;
using System.Data;

namespace KtemVisionSystem
{
    public class CAccessDB
    {
        public string Name { get; set; } = "IF.MDB";
        public string Table { get; set; } = "TABLE";
        public OleDbConnection Conn;

        public CAccessDB(string strName = "")
        {
            if (strName != "") Name = strName;
        }

        public void Init(string strName = "")
        {
            try
            {
                if (strName != "") Name = strName;

                FileInfo fileInfo = new FileInfo(Name);
                if (!fileInfo.Exists) Create(Name);

                Conn = new OleDbConnection();

                if (!IsExistTable(Table)) CreateTable(Table);

                Connect();

                LoadTable(Table); //Test Table을 DataGridView에 연결하자.
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void Close()
        {
            try
            {
                if (Conn != null) Conn.Close();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void Create(string strName)
        {
            try
            {
                string strDBCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strName + ";Jet OLEDB:Database Password=1234";

                ADOX.CatalogClass adoxCC = new ADOX.CatalogClass();
                adoxCC.Create(strDBCon);

                adoxCC.ActiveConnection = null;
                adoxCC = null;

                GC.Collect();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void Connect()
        {
            try
            {
                string strDBCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Name + ";Jet OLEDB:Database Password=1234";

                Conn.ConnectionString = strDBCon;
                Conn.Open();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void LoadTable(string tableName)
        {
            try
            {
                string strQuery = "Select * From " + tableName;
                OleDbDataAdapter m_daDataAdapter = new OleDbDataAdapter(strQuery, Conn);

                OleDbCommandBuilder m_cbCommandBuilder = new OleDbCommandBuilder(m_daDataAdapter);
                DataTable m_dtStudent = new DataTable();

                m_daDataAdapter.Fill(m_dtStudent);
                //dataGridView1.DataSource = m_dtStudent;
                //dataGridView1.MultiSelect = false; //한개의 열만 선택하자.
                //dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //셀선택시 한행 전체 선택
                //dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;//행의 색상을 다르게
                //dataGridView1.ReadOnly = true; // 읽기 전용으로
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void LoadTable(string tableName, DataGridView dgv)
        {
            try
            {
                string strQuery = "Select * From " + tableName;
                OleDbDataAdapter m_daDataAdapter = new OleDbDataAdapter(strQuery, Conn);

                OleDbCommandBuilder m_cbCommandBuilder = new OleDbCommandBuilder(m_daDataAdapter);
                DataTable m_dtStudent = new DataTable();

                m_daDataAdapter.Fill(m_dtStudent);
                dgv.DataSource = m_dtStudent;
                dgv.MultiSelect = false; 
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; 
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
                dgv.ReadOnly = true; 
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void CreateTable(string tableName)
        {
            try
            {
                string strDBCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Name + ";Jet OLEDB:Database Password=1234";

                Conn.ConnectionString = strDBCon;
                Conn.Open();

                string strQuery = $"Create Table {Table}(DATE DATETIME,CODE TEXT(100),DESC TEXT(100))";

                if (strQuery != "") ExcuteQuery(strQuery);

                Conn.Close();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private bool IsExistTable(string tableName)
        {
            Boolean res = false;
            String strDBCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Name + ";Jet OLEDB:Database Password=1234";

            Conn.ConnectionString = strDBCon;
            Conn.Open();

            DataTable schemaTable = Conn.GetOleDbSchemaTable(
                                    OleDbSchemaGuid.Tables,
                                    new object[] { null, null, null, "TABLE" });

            foreach (DataRow row in schemaTable.Rows)
            {
                if (row["TABLE_NAME"].ToString() == tableName)
                {
                    res = true;
                    break;
                }
            }


            Conn.Close();
            return res;

        }

        public int ExcuteQuery(string stSql)
        {
            OleDbCommand cmd = new OleDbCommand(stSql, Conn);

            return cmd.ExecuteNonQuery();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    if (idCheck(txt_ID.Text))
        //    {
        //        updateTestTable(txt_ID.Text, txt_Name.Text, txt_HandPhone.Text, txt_School.Text, txt_Grade.Text);
        //    }
        //    else
        //    {
        //        InsertTestTable(txt_ID.Text, txt_Name.Text, txt_HandPhone.Text, txt_School.Text, txt_Grade.Text);
        //    }
        //    LoadTable("Test");
        //}

        #region QUERY SAMPLE
        //private int InsertTestTable(string stID, string stName, string stHandphone, string stSchool, string stGrade)
        //{
        //    //id int,name nvarchar(50),handphone varchar(30),school nvarchar(100),grade int

        //    String stSql = "insert into Test(id,name,handphone,school,grade) values( " + stID + ",'" + stName + "','" + stHandphone + "','" + stSchool + "'," + stGrade + ")";
        //    return ExcuteQuery(stSql); //-1이면 롤백
        //}

        //private int updateTestTable(string stID, string stName, string stHandphone, string stSchool, string stGrade)
        //{
        //    String stSql = "update Test set name = '" + stName + "',handphone = '" + stHandphone + "',school = '" + stSchool + "',grade = " + stGrade + " where id = " + stID;
        //    return ExcuteQuery(stSql); //-1이면 롤백
        //} 

        //private int DeleteTestTable(string stID)
        //{
        //    String stSql = "Delete from Test where id = " + stID;
        //    return ExcuteQuery(stSql); //-1이면 롤백
        //}    
        #endregion





        private bool idCheck(string stID)
        {
            String strSql = "Select * From Test where id = " + stID;
            OleDbCommand OLECmd = new OleDbCommand(strSql, Conn);
            OLECmd.CommandType = CommandType.Text;
            OleDbDataReader OLEReader = OLECmd.ExecuteReader(CommandBehavior.Default);
            bool res;
            res = OLEReader.Read();
            OLECmd.Dispose();
            OLEReader.Dispose();
            return res;

        }
    }
}
