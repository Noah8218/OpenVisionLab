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
using System.Threading.Tasks;
using static KtemVisionSystem.CLOG;

namespace KtemVisionSystem
{    
    public class LogItem
    {
        private LOG m_etype;
        public LOG Type
        {
            get
            {
                return m_etype;
            }

            set
            {
                m_etype = value;
            }
        }

        private string m_strLog;
        public string StrLog
        {
            get
            {
                return m_strLog;
            }

            set
            {
                m_strLog = value;
            }
        }

        private bool m_bDisplay;
        public bool IsDisplay
        {
            get
            {
                return m_bDisplay;
            }

            set
            {
                m_bDisplay = value;
            }
        }

        public LogItem(LOG etype, string strLog)
        {
            this.m_etype = etype;
            this.m_strLog = strLog;
            this.m_bDisplay = true;
        }
    }

    public static class CLogger
    {
        private const int MAX_TRY_WRITE_LOG_FILE = 10;
        //private static List<string> m_lstFilebuffer = new List<string>();
        private static List<LogItem> m_lstLogbuffer = new List<LogItem>();

        private static DateTime timestampOld;
        public static bool m_bStartLog = true;
        private static string m_strLogPath;// = @"D:\";
        private static string m_strAppName;

        public delegate void LogEvent(LogItem item);
        public static event LogEvent EventLog;

        public static Color GetColor(LOG type)
        {
            switch (type)
            {
                case LOG.NORMAL:
                    return Color.White;
                case LOG.IO:
                    return Color.Lime;
                case LOG.ABNORMAL:
                case LOG.ALARM:
                    return Color.Red;
                case LOG.COMM:
                    return Color.Blue;
                case LOG.Thread:
                    return Color.Silver;
                case LOG.INSP:
                    return Color.Yellow;
                case LOG.MOTION:
                    return Color.Teal;
                case LOG.SEQ:
                    return Color.Blue;
                case LOG.INTERLOCK:
                    return Color.Orange;                
                case LOG.DEVICE:
                    return Color.White;

            }

            return Color.Black;
        }

        public static void SetPath(string strPath)
        {
            try
            {
                string strDir = strPath;
                if (Directory.Exists(strDir) == false)
                {
                    Directory.CreateDirectory(strPath);
                }

                m_strLogPath = strDir;
                //Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                //Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public static string GetPath()
        {
            return m_strLogPath;
        }

        public static void SetAppName(string strAppName)
        {
            m_strAppName = strAppName;
        }

        public static string GetAppName()
        {
            return m_strAppName;
        }

        public static void AddEvent(LogEvent newMethod)
        {
            EventLog += newMethod;
        }

        static private object lockObject = new object();

        public static string LogStrToFile(LOG Type, string strLog)
        {
            string str = "";
            Stopwatch stopwatch = Stopwatch.StartNew();

            strLog = strLog.TrimEnd('\0');

            DateTime timestampNew = DateTime.Now;
            if (CLogger.m_bStartLog == true)
            {
                CLogger.timestampOld = timestampNew;
                CLogger.m_bStartLog = false;
            }

            string s = stopwatch.ElapsedMilliseconds.ToString();

            TimeSpan timeSpanDelay;
            timeSpanDelay = timestampNew - CLogger.timestampOld;

            int nDelay = (int)timeSpanDelay.TotalMilliseconds;

            string strLogType = "[" + Type.ToString().ToUpper() + "]";
            string strDelay = nDelay.ToString();

            strLogType = strLogType.PadLeft(10, ' ');
            strDelay = strDelay.PadLeft(10, '0');

            str = timestampNew.ToString("yy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + " [" + strDelay + "]  " + strLogType + "  :  " + strLog;
            //Debug.WriteLine(str);

            string s2 = stopwatch.ElapsedMilliseconds.ToString();

            string filename_total = GetPath() + '\\' + timestampNew.ToString("yyMMdd") + m_strAppName != null ? "_" : "" + m_strAppName + ".log";
            string filename_type = GetPath() + '\\' + timestampNew.ToString("yyMMdd") + "_" + Type.ToString() + "_" + (m_strAppName != null ? "_" : "") + m_strAppName + ".log";

            WriteFileLog(filename_type, strLogType + str);

            string s3 = stopwatch.ElapsedMilliseconds.ToString();

            CLogger.timestampOld = timestampNew;
            if (str.Length > 3) { str = str.Remove(0, 3); }

            return str;
        }

        private static void WriteFileLog(string strFileName, string strLog)
        {
            int i = 0;
            while (true)
            {
                try
                {
                    using (StreamWriter writer = File.AppendText(strFileName))
                    {
                        writer.WriteLineAsync(strLog);
                    }

                    break;
                }
                catch (IOException)
                {                    
                    i++;
                    if (i >= MAX_TRY_WRITE_LOG_FILE)
                    {
                        throw new IOException("Log file \"" + strFileName + "\" not accessible after 5 tries");
                    }
                }
            }
            //if (strFileName == "_") { return; }

            //using (StreamWriter writer = File.AppendText(strFileName))
            //{
            //    writer.WriteLineAsync(strLog);
            //}
        }

        public static bool WriteLog(string format, params object[] args)
        {
            string strLog = string.Format(format, args);
            //return Add(LOG.NORMAL, strLog);
            return false;
        }

        //public static bool Add(LOG type, string format, params object[] args)
        //{
        //    string strLog = string.Format(format, args);
        //    string str = LogStrToFile(type, strLog);

        //    LogItem item = new LogItem(type, str);

        //    if (EventLog != null)
        //    {
        //        if (m_lstLogbuffer.Count > 0)
        //        {
        //            /*
        //            foreach (LogItem Log in m_lstLogbuffer)
        //            {
        //                EventLog(Log);
        //            }
        //             */
        //            for (int i = 0; i < m_lstLogbuffer.Count; i++)
        //            {
        //                EventLog(m_lstLogbuffer[i]);
        //            }
        //            m_lstLogbuffer.Clear();
        //            EventLog(item);
        //        }
        //        else
        //        {
        //            EventLog(item);
        //        }
        //    }
        //    else
        //    {
        //        m_lstLogbuffer.Add(item);
        //    }

        //    if (type == LOG.ALARM)
        //    {
        //        //CUtil.ShowMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
        //    }
        //    return true;
        //}

        public static void LoggerFileDelete(int FileCount)
        {
            DirectoryInfo dinfo = new DirectoryInfo(GetPath());
            if (!dinfo.Exists) dinfo.Create();

            if (FileCount < dinfo.GetFiles().Length)
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (FileInfo f in dinfo.GetFiles())
                {
                    if (f.Extension == ".log") files.Add(f);
                }

                files.Sort(new CompareFileInfoEntries());

                for (int i = 0; i <= files.Count - FileCount; i++)
                {
                    File.Delete(dinfo.FullName + "\\" + files[i].Name);
                    WriteLog("Delete File ==> " + dinfo.FullName + "\\" + files[i].Name);
                }
            }
        }

        public static void LoggerFileDelete(string strDir, string strExt, int FileCount)
        {
            DirectoryInfo dinfo = new DirectoryInfo(strDir);
            if (!dinfo.Exists) dinfo.Create();

            if (FileCount < dinfo.GetFiles().Length)
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (FileInfo f in dinfo.GetFiles())
                {
                    if (f.Extension == strExt) files.Add(f);
                }

                files.Sort(new CompareFileInfoEntries());

                for (int i = 0; i <= files.Count - FileCount; i++)
                {
                    File.Delete(dinfo.FullName + "\\" + files[i].Name);
                    WriteLog("Delete File ==> " + dinfo.FullName + "\\" + files[i].Name);
                }
            }
        }
    }

    public class CompareFileInfoEntries : IComparer<FileInfo>
    {
        public int Compare(FileInfo f1, FileInfo f2)
        {
            return (DateTime.Compare(f1.CreationTime, f2.CreationTime));
        }
    }
}
