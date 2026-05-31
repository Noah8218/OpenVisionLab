using OpenVisionLab._3._Device.DB;
using Lib.Common;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Timers;

namespace OpenVisionLab
{
    public class CMariaSQL
    {                        
        public bool IsAlive { get; set; } = false;       
        public string ServerName { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "3306";        
        public string DataBase { get; set; } = "MariaDB";               
        public string UID { get; set; } = "root";       
        public string Password { get; set; } = "123456789";
        public string MAC { get; set; } = "D8-3B-BF-1A-4B-EE";        
        public string DataBackupPath { get; set; } = "";                        
        public DateTime StartSearchTime { get; set; } = new DateTime();                
        public DateTime EndSearchTime { get; set; } = new DateTime();    
        private string strDefectTable { get; set; } = "defect";
        private string strProductTable { get; set; } = "product";

        public MySqlConnection m_MysqlConnection { get; set; } = new MySqlConnection();

        public CMariaSQL() { }    
        private Int32 m_hours = 0;
        private Int32 m_runAt = 24;
        private System.Timers.Timer m_timer;
        public void StartBackup()
        {
            m_hours = (24 - (DateTime.Now.Hour + 1)) + m_runAt;
            m_timer = new System.Timers.Timer();
            m_timer.Interval = m_hours * 60 * 60 * 1000;
            m_timer.Elapsed += new ElapsedEventHandler(Timeer_Backup);
            m_timer.Start();

           // string strPath = Global.System.MariaSQL.DataBackupPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "_backup.sql";
           // Global.System.MariaSQL.Backup(strPath);
            //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
        }

        private void Timeer_Backup(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (m_hours != 24)
                {
                    m_hours = 24;
                    m_timer.Interval = m_hours * 60 * 60 * 1000;
                    //string strPath = Global.System.MariaSQL.DataBackupPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "_backup.sql";
                    //Global.System.MariaSQL.Backup(strPath);
                    //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                }

            }
            catch (Exception Desc)
            {                
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }

        }

        public bool Backup(string strFile)
        {
            try
            {
                if(strFile == "")
                {
                    strFile = "C:\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_backup.sql";
                }
                //string file = "E:\\backup.sql";
                if (OpenConnection() == true)
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = m_MysqlConnection;
                            mb.ExportToFile(strFile);
                        }
                    }
                    Disconnection();
                }
                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (MySqlException Desc)
            {
                switch (Desc.Number)
                {
                    case 0:
                        CLOG.ABNORMAL($"데이터베이스 서버에 연결할 수 없습니다.");
                        break;

                    case 1045:
                        CLOG.ABNORMAL($"유저 ID 또는 Password를 확인해주세요.");
                        break;
                }
                return false;
            }
        }

        public bool Restore(string strFile)
        {
            try
            {
                if (strFile == "")
                {
                    return false;
                }

                if (OpenConnection() == true)
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = m_MysqlConnection;
                            mb.ImportFromFile(strFile);
                        }
                    }
                    Disconnection();
                }

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (MySqlException Desc)
            {
                switch (Desc.Number)
                {
                    case 0:
                        CLOG.ABNORMAL($"데이터베이스 서버에 연결할 수 없습니다.");
                        break;

                    case 1045:
                        CLOG.ABNORMAL($"유저 ID 또는 Password를 확인해주세요.");
                        break;
                }
                return false;
            }
        }

        public bool OpenConnection()
        {
            try
            {
                //string strConnection = $"Server=corebarcode.iptime.org;Port=7777;Uid=swhitech;Pwd=swhitech1234";

                string strConnection = $"Server=" + ServerName + ";Port=" + Port + ";database=" + DataBase + ";Uid=" + UID + ";Pwd=" + Password + ";Allow User Variables=True";

                m_MysqlConnection = new MySqlConnection(strConnection);

                m_MysqlConnection.Open();

                //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (MySqlException Desc)
            {
                switch (Desc.Number)
                {
                    case 0:
                        CLOG.ABNORMAL($"데이터베이스 서버에 연결할 수 없습니다.");
                        break;

                    case 1045:
                        CLOG.ABNORMAL($"유저 ID 또는 Password를 확인해주세요.");
                        break;
                }
                return false;
            }
        }

        public bool Disconnection()
        {
            try
            {
                if (m_MysqlConnection.State == System.Data.ConnectionState.Open)
                {
                    m_MysqlConnection.Close();
                }
                else
                {
                    return false;
                }
                //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (MySqlException Desc)
            {                
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        private object m_ob = new object();

        public void InsertBulkToData(List<CDefect> defects)
        {
            lock (m_ob)
            {                
                StringBuilder sCommand = new StringBuilder($"INSERT INTO {strDefectTable} (`Index`, LotID, DefectType, CAM, Reel_No, CenterX, CenterY, Width, Height, Area, ImagePath, `DateTime`) VALUES ");
                List<string>  Rows = new List<string>();
                for (int i = 0; i < defects.Count; i++)
                {
                    Rows.Add(string.Format("({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", defects[i].Index,
                        MySqlHelper.EscapeString(defects[i].LotID), MySqlHelper.EscapeString(defects[i].DefectType),
                        MySqlHelper.EscapeString(defects[i].Position),
                        MySqlHelper.EscapeString(defects[i].Reel_No.ToString()), MySqlHelper.EscapeString(defects[i].CenterX.ToString()),
                        MySqlHelper.EscapeString(defects[i].CenterY.ToString()), MySqlHelper.EscapeString(defects[i].Width.ToString()),
                        MySqlHelper.EscapeString(defects[i].Height.ToString()), MySqlHelper.EscapeString(defects[i].Area.ToString()),
                        MySqlHelper.EscapeString(defects[i].ImagePath.ToString()), DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");

                if (OpenConnection())
                {
                    if(defects.Count > 0)
                    {
                        using (MySqlCommand cmdProduct = new MySqlCommand(sCommand.ToString(), m_MysqlConnection))
                        {
                            cmdProduct.CommandType = CommandType.Text;
                            cmdProduct.ExecuteNonQuery();
                        }
                    }                  
                }
            }            
        }

        public bool InsertData(CProduct Product, CDefect Defect)
        {
            try
            {
                lock(m_ob)
                {
                    if (OpenConnection())
                    {                        
                        string sqlProduct = "INSERT INTO " + strProductTable + " VALUES(@Index, @LotID, @DateTime)";
                        MySqlCommand cmdProduct = new MySqlCommand(sqlProduct, m_MysqlConnection);                                                                        
                        cmdProduct.Parameters.Add("@Index", MySqlDbType.Int32, 65535);                        
                        cmdProduct.Parameters.Add("@LotID", MySqlDbType.Int32, 4);                        
                        cmdProduct.Parameters.Add("@DateTime", MySqlDbType.Datetime);

                        cmdProduct.Parameters["@Index"].Value = null;
                        cmdProduct.Parameters["@LotID"].Value = Product.LotID;                        
                        cmdProduct.Parameters["@DateTime"].Value = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");

                        cmdProduct.ExecuteNonQuery();
                        cmdProduct.Dispose();

                        string sqlDefect = "INSERT INTO " + strDefectTable + " VALUES(@LotID, @DefectType, @CAM, @Reel_No, @CenterX, @CenterY, @Width, @Height, @Area, @ImagePath)";
                        MySqlCommand cmdDefect = new MySqlCommand(sqlDefect, m_MysqlConnection);
                        cmdDefect.Parameters.Add("@LotID", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@DefectType", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@CAM", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@Reel_No", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@CenterX", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@CenterY", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@Width", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@Height", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@Area", MySqlDbType.Text, 65535);
                        cmdDefect.Parameters.Add("@ImagePath", MySqlDbType.Text, 65535);

                        cmdDefect.Parameters["@LotID"].Value = Defect.LotID;
                        cmdDefect.Parameters["@DefectType"].Value = Defect.DefectType;
                        cmdDefect.Parameters["@CAM"].Value = Defect.Position;
                        cmdDefect.Parameters["@Reel_No"].Value = Defect.Reel_No;
                        cmdDefect.Parameters["@CenterX"].Value = Defect.CenterX;
                        cmdDefect.Parameters["@CenterY"].Value = Defect.CenterY;
                        cmdDefect.Parameters["@Width"].Value = Defect.Width;
                        cmdDefect.Parameters["@Height"].Value = Defect.Height;
                        cmdDefect.Parameters["@Area"].Value = Defect.Area;
                        cmdDefect.Parameters["@ImagePath"].Value = Defect.ImagePath;

                        cmdDefect.ExecuteNonQuery();
                        cmdDefect.Dispose();

                        Disconnection();
                    }
                    else
                    {
                        CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}  Open Connection Faile");                        
                        return false;
                    }
                }
                
                //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (MySqlException Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        //public bool SearchFromWhereData(string LotNo, DefectType defectType, out List<CDefect> defects)
        //{
        //    defects = new List<CDefect>();
        //    try
        //    {
        //        if (!OpenConnection())
        //        {
        //            CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> Open Connection Faile");
        //            return false;
        //        }

        //        // LotID기준 DEFECT 테이블의 데이터를 가져오는 쿼리 
        //        // 하지만 LOTID가 생산시 1개를 입력받는 구조기 때문에 
        //        //string strWhere = " WHERE DateTime >= " + strStartTime + " AND DateTime <=" +strEndTime;
        //        //string strDefect = $"SELECT * FROM {strDefectTable} INNER JOIN {strProductTable} ON {strDefectTable}.LotID = {strProductTable}.LotID {strWhere}";

        //        string strStartTime = "'" + StartSearchTime.ToString("yyyy-MM-dd HH:mm:dd") + "'";
        //        string strEndTime = "'" + EndSearchTime.ToString("yyyy-MM-dd HH:mm:dd") + "'";
        //        string strWhere = "";
        //        if (defectType == DefectType.전체)
        //        {
        //            strWhere = $" WHERE DateTime >= {strStartTime} AND DateTime <= {strEndTime} AND LotID = '{LotNo}'";
        //        }
        //        else
        //        {
        //            strWhere = $" WHERE DateTime >= {strStartTime} AND DateTime <= {strEndTime} AND DefectType = '{defectType.ToString()}' AND LotID = '{LotNo}'";
        //        }
                
        //        string strDefect = $"SELECT * FROM {strDefectTable} {strWhere}";

        //        List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();

        //        using (MySqlCommand cmdDefect = new MySqlCommand(strDefect, m_MysqlConnection))
        //        {
        //            using (MySqlDataReader tableDefect = cmdDefect.ExecuteReader())
        //            {
        //                while (tableDefect.Read())
        //                {
        //                    Dictionary<string, object> record = new Dictionary<string, object>();
        //                    for (int i = 0; i < tableDefect.FieldCount; i++)
        //                    {
        //                        record[tableDefect.GetName(i)] = tableDefect.GetValue(i);
        //                    }
        //                    records.Add(record);
        //                }
        //            }
        //        }
        //        ConcurrentBag<CDefect> defectsS = new ConcurrentBag<CDefect>();
        //        int nCount = 0;                
        //        Parallel.ForEach(records, record =>
        //        {
        //            string Index = record["Index"].ToString();
        //            string LotID = record["LotID"].ToString();
        //            string DefectType = record["DefectType"].ToString();
        //            string CAM = record["CAM"].ToString();
        //            string Reel_No = record["Reel_No"].ToString();
        //            string CenterX = record["CenterX"].ToString();
        //            string CenterY = record["CenterY"].ToString();
        //            string Width = record["Width"].ToString();
        //            string Height = record["Height"].ToString();
        //            string Area = record["Area"].ToString();
        //            string ImagePath = record["ImagePath"].ToString();
        //            string dateTime = record["DateTime"].ToString();

        //            CDefect cDefect = new CDefect()
        //            {
        //                Index = int.Parse(Index),
        //                LotID = LotID,
        //                Reel_No = int.Parse(Reel_No),
        //                Position = CAM,
        //                CenterX = double.Parse(CenterX),
        //                CenterY = double.Parse(CenterY),
        //                Width = double.Parse(Width),
        //                Height = double.Parse(Height),
        //                Area = double.Parse(Area),
        //                DefectType = DefectType,
        //                ImagePath = ImagePath,
        //                DateTime = DateTime.Parse(dateTime)
        //            };
        //            defectsS.Add(cDefect);
        //            nCount++;
        //        });

        //        defects = defectsS.OrderBy(c => c.Index).ToList();

        //        defects = defects.Select((defect, index) =>
        //        {
        //            defect.Index = index + 1;
        //            return defect;
        //        }).ToList();

        //        //MySqlCommand cmdDefect = new MySqlCommand(strDefect, m_MysqlConnection);
        //        //MySqlDataReader tableDefect = cmdDefect.ExecuteReader();

        //        //int nCount = 0;
        //        //while (tableDefect.Read())
        //        //{
        //        //    nCount++;

        //        //    string LotID = tableDefect["LotID"].ToString();
        //        //    string DefectType = tableDefect["DefectType"].ToString();
        //        //    string CAM = tableDefect["CAM"].ToString();
        //        //    string Reel_No = tableDefect["Reel_No"].ToString();
        //        //    string CenterX = tableDefect["CenterX"].ToString();
        //        //    string CenterY = tableDefect["CenterY"].ToString();
        //        //    string Width = tableDefect["Width"].ToString();
        //        //    string Height = tableDefect["Height"].ToString();
        //        //    string Area = tableDefect["Area"].ToString();
        //        //    string ImagePath = tableDefect["ImagePath"].ToString();
        //        //    string dateTime = tableDefect["DateTime"].ToString();

        //        //    CDefect cDefect = new CDefect()
        //        //    {
        //        //        Index = nCount,
        //        //        LotID = LotID,
        //        //        Reel_No = int.Parse(Reel_No),
        //        //        Position = CAM,
        //        //        CenterX = double.Parse(CenterX),
        //        //        CenterY = double.Parse(CenterY),
        //        //        Width = double.Parse(Width),
        //        //        Height = double.Parse(Height),
        //        //        Area = double.Parse(Area),
        //        //        DefectType = DefectType,
        //        //        ImagePath = ImagePath,
        //        //        DateTime = DateTime.Parse(dateTime)
        //        //    };
        //        //    defects.Add(cDefect);
        //        //}

        //        CLOG.NORMAL($"[OK] Search Data :{defectsS.Count}");
        //        //cmdDefect.Clone();
        //        //cmdDefect.Dispose();

        //        Disconnection();
        //        CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");

        //        return true;
        //    }
        //    catch (MySqlException Desc)
        //    {
        //        CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
        //        return false;
        //    }
        //}

        public bool SearchData(out List<CDefect> defects)
        {
            defects = new List<CDefect>();
            try
            {                
                if (!OpenConnection())
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> Open Connection Faile");
                    return false;
                }

                // LotID기준 DEFECT 테이블의 데이터를 가져오는 쿼리 
                // 하지만 LOTID가 생산시 1개를 입력받는 구조기 때문에 
                //string strWhere = " WHERE DateTime >= " + strStartTime + " AND DateTime <=" +strEndTime;
                //string strDefect = $"SELECT * FROM {strDefectTable} INNER JOIN {strProductTable} ON {strDefectTable}.LotID = {strProductTable}.LotID {strWhere}";

                string strStartTime = "'" + StartSearchTime.ToString("yyyy-MM-dd HH:mm:dd") + "'";
                string strEndTime = "'" + EndSearchTime.ToString("yyyy-MM-dd HH:mm:dd") + "'";
                string strWhere = " WHERE DateTime >= " + strStartTime + " AND DateTime <=" + strEndTime;
                string strDefect = $"SELECT * FROM {strDefectTable} {strWhere}";

                MySqlCommand cmdDefect = new MySqlCommand(strDefect, m_MysqlConnection);
                MySqlDataReader tableDefect = cmdDefect.ExecuteReader();

                int nCount = 0;                
                while (tableDefect.Read())
                {
                    nCount++;

                    string LotID = tableDefect["LotID"].ToString();
                    string DefectType = tableDefect["DefectType"].ToString();
                    string CAM = tableDefect["CAM"].ToString();
                    string Reel_No = tableDefect["Reel_No"].ToString();
                    string CenterX = tableDefect["CenterX"].ToString();
                    string CenterY = tableDefect["CenterY"].ToString();
                    string Width = tableDefect["Width"].ToString();
                    string Height = tableDefect["Height"].ToString();
                    string Area = tableDefect["Area"].ToString();
                    string ImagePath = tableDefect["ImagePath"].ToString();
                    string dateTime = tableDefect["DateTime"].ToString();

                    CDefect cDefect = new CDefect()
                    {
                        Index = nCount,
                        LotID = LotID,
                        Reel_No = int.Parse(Reel_No),
                        Position = CAM,
                        CenterX = double.Parse(CenterX),
                        CenterY = double.Parse(CenterY),
                        Width = double.Parse(Width),
                        Height = double.Parse(Height),
                        Area = double.Parse(Area),
                        DefectType = DefectType,
                        ImagePath = ImagePath,
                        DateTime = DateTime.Parse(dateTime)
                    };
                    defects.Add(cDefect);
                }

                CLOG.NORMAL($"[OK] Search Data :{nCount}");
                cmdDefect.Clone();
                cmdDefect.Dispose();

                Disconnection();
                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");

                return true;
            }
            catch (MySqlException Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool AlterDataTable()
        {
            try
            {
                if (OpenConnection() == true)
                {

                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (MySqlException Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool DeleteRow()
        {
            try
            {
                if (OpenConnection() == true)
                {

                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (MySqlException Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool DeleteData2(string strDeleteName = "")
        {
            try
            {
                if (OpenConnection() == true)
                {
                    string strqry = "DELETE FROM " + strProductTable;
                    MySqlCommand cmd = new MySqlCommand(strqry, m_MysqlConnection);
                    cmd.ExecuteNonQuery();

                    cmd.Dispose();

                    Disconnection();
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (MySqlException Desc)
            {
                switch (Desc.Number)
                {
                    case 0:
                        CLOG.ABNORMAL($"데이터베이스 서버에 연결할 수 없습니다.");
                        break;

                    case 1045:
                        CLOG.ABNORMAL($"유저 ID 또는 Password를 확인해주세요.");
                        break;
                }

                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool DeleteData(string strDeleteName = "")
        {
            try
            {
                if (OpenConnection() == true)
                {
                    string strqry = "DELETE FROM " + strDefectTable;
                    MySqlCommand cmd = new MySqlCommand(strqry, m_MysqlConnection);
                    cmd.ExecuteNonQuery();

                    cmd.Dispose();

                    Disconnection();
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (MySqlException Desc)
            {
                switch (Desc.Number)
                {
                    case 0:
                        CLOG.ABNORMAL($"데이터베이스 서버에 연결할 수 없습니다.");                        
                        break;

                    case 1045:
                        CLOG.ABNORMAL($"유저 ID 또는 Password를 확인해주세요.");                        
                        break;
                }

                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

    
        public bool CreateTable()
        {
            try
            {
                if (m_MysqlConnection.State == System.Data.ConnectionState.Open)
                {
                    //string sql = "create table " + strTableDB + "(name VARCHAR(20),age VARCHAR(20))";
                    //MySqlCommand cmd = new MySqlCommand(sql, m_MysqlConnection);
                    //cmd.ExecuteNonQuery();

                    //// Mysql DB Table 값 입력
                    ////string sql2 = "insert into " + strTableDB + " values('홍길동','100')";
                    ////MySqlCommand cmd2 = new MySqlCommand(sql2, m_MysqlConnection);
                    ////cmd2.ExecuteNonQuery();

                    ////// Mysql DB Table 값 가져오기
                    ////string sql3 = "select * from " + strTableDB;
                    ////MySqlCommand cmd3 = new MySqlCommand(sql3, m_MysqlConnection);
                    ////MySqlDataReader rdr = cmd3.ExecuteReader();

                    ////while (rdr.Read())
                    ////{
                    ////    string name = rdr[0].ToString();
                    ////    string age = rdr[1].ToString();
                    ////}
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (MySqlException Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }       
    }
}
