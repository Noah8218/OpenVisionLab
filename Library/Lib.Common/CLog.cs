using log4net;
using log4net.Appender;
using log4net.Core;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lib.Common
{
    public static class CLOG
    {
        public  enum LOG
        {
            NORMAL = 0,
            ABNORMAL,
            COMM,
            IO,
            Thread,
            INSP,
            MOTION,
            SEQ,
            ALARM,
            INTERLOCK,
            DEVICE,
            TEACHING,
            CONFIG,
            LOT
        }

        public static Color GetColor(string type)
        {
            if (type.Contains("[NORMAL]")) { return Color.White; }
            else if (type.Contains("[IO]")) { return Color.Lime; }
            else if (type.Contains("[ABNORMAL]")) { return Color.Red; }
            else if (type.Contains("[ALARM]")) { return Color.Red; }
            else if (type.Contains("[COMM]")) { return Color.Blue; }
            else if (type.Contains("[MOTION]")) { return Color.Teal; }
            else if (type.Contains("[INSP]")) { return Color.Yellow; }
            else if (type.Contains("[INTERLOCK]")) { return Color.Orange; }
            else if (type.Contains("[SEQ]")) { return Color.Blue; }
            else if (type.Contains("[DEVICE]")) { return Color.Yellow; }
            else if (type.Contains("[Thread]")) { return Color.Silver; }
            else if (type.Contains("Fatal")) { return Color.Blue; }
            return Color.White;
        }

        // 전체 로그
        private static readonly ILog log = LogManager.GetLogger("ROOT_ALL");
        private static readonly ILog log_ABNORMAL = LogManager.GetLogger("ROOT_ABNORMAL");
        private static readonly ILog log_COMM = LogManager.GetLogger("ROOT_COMM");
        private static readonly ILog log_ALRAM = LogManager.GetLogger("ROOT_ALARM");
        private static readonly ILog log_MOTION = LogManager.GetLogger("ROOT_MOTION");
        private static readonly ILog log_DEVICE = LogManager.GetLogger("ROOT_DEVICE");
        private static readonly ILog log_IO = LogManager.GetLogger("ROOT_IO");

        public static string Builder(LOG Log, params object[] T)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("[{0}] ", Log.ToString()));
            for (int i = 0; i < T.Length; i++)
            {
                stringBuilder.Append(T[i]);
            }
            return stringBuilder.ToString();
        }

        public static void Debug(params object[] T) => log.Debug(Builder(LOG.NORMAL,T));
        public static void Warn(params object[] T) => log.Warn(Builder(LOG.ABNORMAL, T));
        public static void Fatal(params object[] T) => log.Fatal(Builder(LOG.NORMAL, T));
        public static void NORMAL(params object[] T) => log.Info(Builder(LOG.NORMAL, T));
        public static void ABNORMAL(params object[] T)
        {
            log.Error(Builder(LOG.ABNORMAL, T));
            log_ABNORMAL.Error(Builder(LOG.ABNORMAL, T));
        }
        public static void COMM(params object[] T)
        {
            log.Info(Builder(LOG.COMM, T)); 
            log_COMM.Info(Builder(LOG.COMM, T));
        }
        public static void ALARM(params object[] T) 
        {
            log.Info(Builder(LOG.ALARM, T));
            log_ALRAM.Info(Builder(LOG.ALARM, T));
        }
        public static void MOTION(params object[] T) 
        {
            log.Info(Builder(LOG.MOTION, T));
            log_MOTION.Info(Builder(LOG.MOTION, T));
        }
        public static void DEVICE(params object[] T) 
        {
            log.Info(Builder(LOG.DEVICE, T));
            log_DEVICE.Info(Builder(LOG.DEVICE, T));
        }

        public static void IO(params object[] T)
        {
            log.Info(Builder(LOG.IO, T)); 
            log_IO.Info(Builder(LOG.IO, T));
        }

        public static void LOT(params object[] T) { log.Info(Builder(LOG.LOT, T)); }
        public static void SEQ(params object[] T) { log.Info(Builder(LOG.SEQ, T)); }
        public static void CONFIG(params object[] T) { log.Info(Builder(LOG.CONFIG, T)); }
        public static void INTERLOCK(params object[] T) { log.Info(Builder(LOG.INTERLOCK, T)); }        
        public static void INSP(params object[] T) { log.Info(Builder(LOG.INSP, T)); }                
        //public static void Thread(params object[] T) { log.Info(Builder(LOG.Thread, T)); }        
    }

    public class CustomMemoryAppender : AppenderSkeleton
    {
        private readonly StringBuilder logBuffer;
        readonly StringWriter stringWriter;
        private readonly object lockObject = new object();

        public CustomMemoryAppender()
        {
            logBuffer = new StringBuilder();
            stringWriter = new StringWriter(logBuffer);

        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (lockObject)
            {
                if (Layout == null)
                {                    
                    if(loggingEvent.LoggerName == "ROOT_ALL")
                    {
                        logBuffer.AppendLine(string.Format("[{0}]{1}", loggingEvent.TimeStamp, loggingEvent.RenderedMessage));
                    }                    
                }
                else
                {
                    Layout.Format(stringWriter, loggingEvent);
                }
            }
        }

        public string ReadBuffer()
        {
            lock (lockObject)
            {
                string retVal = logBuffer.ToString();
                logBuffer.Clear();
                return retVal;
            }
        }
    }
}
